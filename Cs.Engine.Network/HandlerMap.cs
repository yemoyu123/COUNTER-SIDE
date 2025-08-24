using System;
using System.Collections.Generic;
using Cs.Logging;
using Cs.Protocol;
using Protocol;

namespace Cs.Engine.Network;

internal sealed class HandlerMap
{
	private readonly Dictionary<ushort, PacketHandler> handlers_ = new Dictionary<ushort, PacketHandler>(300);

	public void RegisterHandler(Type containerType)
	{
		foreach (PacketHandler item in PacketHandler.Extract(containerType))
		{
			handlers_.Add(item.PacketId, item);
		}
		if (handlers_.Count == 0)
		{
			throw new Exception("No packet handlers registered.");
		}
	}

	public void Process(ISerializable message, ushort packetId, Connection connection)
	{
		if (!handlers_.TryGetValue(packetId, out var value))
		{
			Log.Error("packet handler not found. packetId:" + PacketController.Instance.GetIdStr(packetId) + ", Connection Type:" + connection.ServerType, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/HandlerMap.cs", 35);
			return;
		}
		if (packetId != 822 && packetId != 601)
		{
			ClientPacketId clientPacketId = (ClientPacketId)packetId;
			Log.Info("<color=#00FF00FF>" + clientPacketId.ToString() + "</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/HandlerMap.cs", 44);
		}
		value.Execute(message, connection);
	}
}
