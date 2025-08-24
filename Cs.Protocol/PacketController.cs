using System;
using System.Collections.Generic;
using System.Reflection;
using Cs.Logging;
using NKC;

namespace Cs.Protocol;

public sealed class PacketController
{
	internal sealed class PacketDescription
	{
		public Type Type { get; set; }

		public ushort Id { get; set; }

		public string IdStr { get; set; }
	}

	private readonly PacketDescription[] packets = new PacketDescription[65536];

	private readonly Dictionary<Type, ushort> type2Id = new Dictionary<Type, ushort>();

	private bool initialized;

	public static PacketController Instance { get; } = new PacketController();

	internal IEnumerable<PacketDescription> Descriptions => packets;

	public void Initialize()
	{
		if (initialized)
		{
			return;
		}
		Type[] types = Assembly.GetExecutingAssembly().GetTypes();
		foreach (Type type in types)
		{
			if (Attribute.GetCustomAttribute(type, typeof(PacketIdAttribute)) is PacketIdAttribute { PacketId: var packetId } packetIdAttribute)
			{
				if (packets[packetId] != null)
				{
					PacketDescription packetDescription = packets[packetId];
					Log.ErrorAndExit($"packet id duplicated. id:{packetId} typeA:{packetDescription.Type.Name} typeB:{type.Name}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Cs.Protocol/PacketController.cs", 37);
				}
				type2Id.Add(type, packetId);
				packets[packetId] = new PacketDescription
				{
					Type = type,
					Id = packetId,
					IdStr = packetIdAttribute.PacketIdStr
				};
			}
		}
		initialized = true;
	}

	public ushort GetId(ISerializable target)
	{
		if (!type2Id.TryGetValue(target.GetType(), out var value))
		{
			return ushort.MaxValue;
		}
		return value;
	}

	public ushort GetId(Type type)
	{
		if (!type2Id.TryGetValue(type, out var value))
		{
			return ushort.MaxValue;
		}
		return value;
	}

	public string GetIdStr(ISerializable target)
	{
		return GetDescription(target.GetType())?.IdStr ?? "[Not a PacketType]";
	}

	public string GetIdStr(ushort id)
	{
		return packets[id]?.IdStr ?? $"[invalid id:{id}]";
	}

	public bool IsPacket(ISerializable target)
	{
		return type2Id.ContainsKey(target.GetType());
	}

	public ISerializable Create(ushort id)
	{
		PacketDescription packetDescription = packets[id];
		if (packetDescription == null)
		{
			Log.Error($"invalid packet id:{id}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Cs.Protocol/PacketController.cs", 94);
			return null;
		}
		return (ISerializable)NKCPacketObjectPool.OpenObject(packetDescription.Type);
	}

	private PacketDescription GetDescription(Type type)
	{
		if (!type2Id.TryGetValue(type, out var value))
		{
			return null;
		}
		return packets[value];
	}
}
