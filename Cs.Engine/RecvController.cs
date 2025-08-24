using System;
using System.Collections.Concurrent;
using Cs.Engine.Network;
using Cs.Protocol;

namespace Cs.Engine;

internal sealed class RecvController
{
	private readonly struct Node
	{
		public ISerializable Message { get; }

		public ushort PacketId { get; }

		public Node(ISerializable message, ushort packetId)
		{
			Message = message;
			PacketId = packetId;
		}
	}

	private readonly ConcurrentQueue<Node> queue_ = new ConcurrentQueue<Node>();

	private HandlerMap handlerMap_ = new HandlerMap();

	public void Enqueue(ISerializable packet, ushort packetId)
	{
		queue_.Enqueue(new Node(packet, packetId));
	}

	public void RegisterHandler(Type containerType)
	{
		handlerMap_.RegisterHandler(containerType);
	}

	public void ProcessResponses(Connection connection)
	{
		Node result;
		while (queue_.TryDequeue(out result))
		{
			handlerMap_.Process(result.Message, result.PacketId, connection);
		}
	}
}
