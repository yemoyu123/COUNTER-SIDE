using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Cs.Engine.Network.Buffer;
using Cs.Logging;
using Cs.Protocol;
using Protocol;

namespace Cs.Engine.Network;

public sealed class Connection : IDisposable
{
	public delegate void ConnectionEventHandler(Connection connection);

	private readonly Socket socket_ = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

	private readonly string host;

	private readonly int port;

	private readonly byte[] receiveBuffer_ = new byte[4096];

	private readonly SocketAsyncEventArgs receiveArgs_ = new SocketAsyncEventArgs();

	private readonly SendController sendController_;

	private readonly RecvController recvController_ = new RecvController();

	private readonly MemoryPipe receivePipe_ = new MemoryPipe();

	private bool finalized_;

	private long sendSequence;

	public string ServerType { get; }

	public bool IsConnected => socket_.Connected;

	public long SendSequence => sendSequence;

	public event ConnectionEventHandler OnDisconnected;

	private Connection(string ip, int port, string serverType)
	{
		socket_.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, optionValue: false);
		ServerType = serverType;
		host = ip;
		this.port = port;
		receiveArgs_.SetBuffer(receiveBuffer_, 0, receiveBuffer_.Length);
		receiveArgs_.Completed += OnReceiveCompleted;
		sendController_ = new SendController(socket_);
	}

	public static Connection Create(string ip, int port, string serverType, Action<Connection> onConnected, TimeSpan timeout)
	{
		Connection connection = new Connection(ip, port, serverType);
		connection.TryConnect(onConnected, timeout);
		return connection;
	}

	public void Dispose()
	{
		socket_.Dispose();
		receiveArgs_.Dispose();
		receivePipe_.Dispose();
		sendController_.Dispose();
	}

	public void RegisterHandler(Type containerType)
	{
		recvController_.RegisterHandler(containerType);
	}

	public void ProcessResponses()
	{
		recvController_.ProcessResponses(this);
	}

	public override string ToString()
	{
		return $"endpoint:{host}:{port} connected:{socket_.Connected} send_queue:{sendController_.MessageCount}";
	}

	public void CloseConnection()
	{
		if (finalized_)
		{
			return;
		}
		finalized_ = true;
		if (!socket_.Connected)
		{
			return;
		}
		try
		{
			socket_.Shutdown(SocketShutdown.Both);
		}
		catch (SocketException ex)
		{
			Log.Warn($"Shutdown failed. code:[{ex.ErrorCode}]{ex.SocketErrorCode.ToString()} msg:{ex.Message}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 87);
		}
		finally
		{
			socket_.Close();
			socket_.Dispose();
			receivePipe_.Dispose();
			this.OnDisconnected?.Invoke(this);
		}
	}

	public bool Send(ISerializable msg)
	{
		if (msg == null)
		{
			return false;
		}
		if (!socket_.Connected)
		{
			Log.Warn("socket connected == false", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 109);
			CloseConnection();
			return false;
		}
		long sequence = Interlocked.Increment(ref sendSequence);
		Packet? packet = Packet.Pack(msg, sequence);
		if (!packet.HasValue)
		{
			Log.Error($"data serializing failed. packetid:{PacketController.Instance.GetId(msg)}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 120);
			return false;
		}
		ClientPacketId id = (ClientPacketId)PacketController.Instance.GetId(msg);
		if (id != ClientPacketId.kNKMPacket_HEART_BIT_REQ)
		{
			Log.Info("<color=#FFFF00FF>" + id.ToString() + "</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 126);
		}
		sendController_.Push(packet.Value);
		return true;
	}

	private void BeginReceive()
	{
		try
		{
			if (!socket_.ReceiveAsync(receiveArgs_))
			{
				OnReceiveCompleted(null, receiveArgs_);
			}
		}
		catch (Exception ex)
		{
			Log.Info(ServerType + " ReceiveAsync " + ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 147);
			CloseConnection();
		}
	}

	private void OnReceiveCompleted(object sender, SocketAsyncEventArgs arg)
	{
		if (arg.BytesTransferred <= 0)
		{
			Log.Info("OnRecvCallback transferred zero", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 157);
			CloseConnection();
		}
		else if (arg.SocketError != SocketError.Success)
		{
			if (arg.SocketError != SocketError.ConnectionReset)
			{
				Log.Warn($"[Connection] OnReceiveCompleted ErrorCode:{arg.SocketError} serverType:{ServerType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 166);
			}
			Log.Warn("socket error != success", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 169);
			CloseConnection();
		}
		else
		{
			receivePipe_.Write(receiveBuffer_, 0, arg.BytesTransferred);
			if (!Packet.ProcessRecv(receivePipe_, recvController_))
			{
				Log.Warn("packet process Recv fail", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 177);
				CloseConnection();
			}
			else
			{
				BeginReceive();
			}
		}
	}

	private void TryConnect(Action<Connection> callback, TimeSpan timeout)
	{
		Task.Run(delegate
		{
			try
			{
				Log.Info($"{ServerType} socket.ConnectAsync {host}:{port}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 192);
				IAsyncResult asyncResult = socket_.BeginConnect(host, port, null, null);
				asyncResult.AsyncWaitHandle.WaitOne((int)timeout.TotalMilliseconds, exitContext: false);
				if (socket_.Connected)
				{
					socket_.EndConnect(asyncResult);
					BeginReceive();
					callback(this);
				}
				else
				{
					CloseConnection();
					callback(null);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ServerType + " connection failed:" + ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Connection.cs", 212);
				CloseConnection();
				callback(null);
			}
		});
	}
}
