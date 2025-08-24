using System;
using System.Collections.Concurrent;
using Cs.Engine.Network;
using Cs.Logging;
using Cs.Protocol;
using NKC.UI;

namespace NKC;

public abstract class NKCConnectBase
{
	private enum NKC_CONNECT_MSG
	{
		NCM_INVALID,
		NCM_ON_CONNECTED,
		NCM_ON_CONNECTED_FAILED,
		NCM_ON_DISCONNECTED
	}

	private readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(10.0);

	private readonly string typeName_;

	private readonly Type handlerType_;

	private readonly ConcurrentQueue<NKC_CONNECT_MSG> connectionEvents_ = new ConcurrentQueue<NKC_CONNECT_MSG>();

	private string remoteIp_;

	private int remotePort_;

	private Connection connection_;

	public long SendSequence => connection_?.SendSequence ?? 0;

	protected string TypeName => typeName_;

	public bool IsConnected => connection_?.IsConnected ?? false;

	public void SetRemoteAddress(string ip, int port)
	{
		remoteIp_ = ip;
		remotePort_ = port;
	}

	public virtual void ResetConnection()
	{
		Log.Info(typeName_ + " ResetConnection", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCConnectBase.cs", 34);
		if (connection_ != null)
		{
			Connection connection = connection_;
			connection_ = null;
			connection.CloseConnection();
		}
	}

	public void SimulateDisconnect()
	{
		connection_?.CloseConnection();
	}

	public virtual void LoginComplete()
	{
		Log.Info(typeName_ + " LoginComplete", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCConnectBase.cs", 54);
	}

	public bool Send(ISerializable packet, NKC_OPEN_WAIT_BOX_TYPE eNKC_OPEN_WAIT_BOX_TYPE, bool bSendFailBox = true)
	{
		bool flag = IsConnected;
		if (flag)
		{
			flag = connection_.Send(packet);
		}
		if (!flag)
		{
			if (bSendFailBox)
			{
				if (NKCScenManager.GetScenManager().IsReconnectScen())
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ERROR_RECONNECT);
					NKCScenManager.GetScenManager().SetAppEnableConnectCheckTime(4f);
				}
				else
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ERROR_DECONNECT);
				}
			}
		}
		else
		{
			NKMPopUpBox.OpenWaitBox(eNKC_OPEN_WAIT_BOX_TYPE);
		}
		return flag;
	}

	public void RawSend(ISerializable data)
	{
		connection_?.Send(data);
	}

	public virtual void Update()
	{
		connection_?.ProcessResponses();
		NKC_CONNECT_MSG result;
		while (connectionEvents_.TryDequeue(out result))
		{
			switch (result)
			{
			case NKC_CONNECT_MSG.NCM_ON_CONNECTED:
				OnConnectedMainThread();
				break;
			case NKC_CONNECT_MSG.NCM_ON_CONNECTED_FAILED:
				OnConnectFailedMainThread();
				break;
			case NKC_CONNECT_MSG.NCM_ON_DISCONNECTED:
				OnDisconnectedMainThread();
				break;
			}
		}
	}

	protected NKCConnectBase(Type handlerType)
	{
		typeName_ = GetType().Name;
		handlerType_ = handlerType;
	}

	protected void Connect()
	{
		Log.Info($"[ConnectBase][{typeName_}] Connect to [{remoteIp_}:{remotePort_}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCConnectBase.cs", 122);
		Connection.Create(remoteIp_, remotePort_, typeName_, ConnectCompleted, ConnectTimeout);
		NKMPopUpBox.OpenWaitBox();
	}

	protected void ConnectCompleted(Connection connection)
	{
		if (connection == null)
		{
			connectionEvents_.Enqueue(NKC_CONNECT_MSG.NCM_ON_CONNECTED_FAILED);
			return;
		}
		connection_ = connection;
		connection_.RegisterHandler(handlerType_);
		connection_.OnDisconnected += OnDisconnected;
		connectionEvents_.Enqueue(NKC_CONNECT_MSG.NCM_ON_CONNECTED);
	}

	protected void OnDisconnected(Connection connection)
	{
		if (connection == connection_)
		{
			connectionEvents_.Enqueue(NKC_CONNECT_MSG.NCM_ON_DISCONNECTED);
		}
	}

	protected virtual void OnConnectedMainThread()
	{
		Log.Info(typeName_ + " OnConnectedMainThread", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCConnectBase.cs", 154);
	}

	protected virtual void OnConnectFailedMainThread()
	{
		Log.Warn(typeName_ + " OnConnectFailedMainThread", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCConnectBase.cs", 159);
		NKMPopUpBox.CloseWaitBox();
	}

	protected virtual void OnDisconnectedMainThread()
	{
		Log.Warn(typeName_ + " OnDisconnectedMainThread", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCConnectBase.cs", 165);
		NKMPopUpBox.CloseWaitBox();
	}
}
