using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_SET_EMBLEM_REQ)]
public sealed class NKMPacket_SET_EMBLEM_REQ : ISerializable
{
	public sbyte index;

	public int itemId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref index);
		stream.PutOrGet(ref itemId);
	}
}
