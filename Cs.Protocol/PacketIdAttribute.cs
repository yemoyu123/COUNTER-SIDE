using System;

namespace Cs.Protocol;

[AttributeUsage(AttributeTargets.Class)]
public class PacketIdAttribute : Attribute
{
	public const ushort InvalidPacketId = ushort.MaxValue;

	public ushort PacketId { get; }

	public string PacketIdStr { get; }

	public PacketIdAttribute(object packetId)
	{
		string text = packetId.ToString();
		PacketId = (ushort)Enum.Parse(packetId.GetType(), text);
		PacketIdStr = $"[{PacketId}] {text}";
	}
}
