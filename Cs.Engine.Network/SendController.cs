using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using Cs.Engine.Network.Buffer;
using Cs.Logging;
using Cs.Protocol;

namespace Cs.Engine.Network;

internal sealed class SendController : IDisposable
{
	private readonly Socket socket_;

	private SocketAsyncEventArgs eventArgs_ = new SocketAsyncEventArgs();

	private ConcurrentQueue<Packet> sendQueue_ = new ConcurrentQueue<Packet>();

	private SendBuffer sendingBuffer_ = new SendBuffer();

	private int messageCount_;

	private int sendingMessageCount_;

	public int MessageCount => messageCount_;

	public SendController(Socket socket)
	{
		socket_ = socket;
		eventArgs_.SetBuffer(sendingBuffer_.Data, 0, sendingBuffer_.Data.Length);
		eventArgs_.Completed += OnSendCompleted;
	}

	public void Dispose()
	{
		eventArgs_.Dispose();
	}

	public void Push(Packet data)
	{
		sendQueue_.Enqueue(data);
		if (Interlocked.Increment(ref messageCount_) != 1)
		{
			return;
		}
		if (!socket_.Connected)
		{
			Log.Warn("[SendController] socket is not connected.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/SendController.cs", 46);
			return;
		}
		TryFillBuffer();
		if (!RequestSendAsync())
		{
			Log.Error("send data failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/SendController.cs", 53);
		}
	}

	public void TryConsumeQueue()
	{
		if (sendQueue_.Count != 0)
		{
			TryFillBuffer();
			if (!RequestSendAsync())
			{
				Log.Error("send data failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/SendController.cs", 67);
			}
		}
	}

	private void TryFillBuffer()
	{
		Packet result;
		while (sendQueue_.TryDequeue(out result))
		{
			sendingMessageCount_++;
			result.WriteTo(sendingBuffer_);
		}
	}

	private bool RequestSendAsync()
	{
		eventArgs_.SetBuffer(0, sendingBuffer_.HeadOffset);
		return socket_.SendAsync(eventArgs_);
	}

	private void OnSendCompleted(object sender, SocketAsyncEventArgs arg)
	{
		sendingBuffer_.Consume(arg.BytesTransferred);
		if (!sendingBuffer_.HasData)
		{
			int num = sendingMessageCount_;
			sendingMessageCount_ = 0;
			if (Interlocked.Add(ref messageCount_, -num) == 0)
			{
				return;
			}
		}
		TryFillBuffer();
		if (!RequestSendAsync())
		{
			Log.Error("send data failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/SendController.cs", 106);
		}
	}
}
