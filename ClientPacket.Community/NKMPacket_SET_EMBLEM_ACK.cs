using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_SET_EMBLEM_ACK)]
public sealed class NKMPacket_SET_EMBLEM_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public sbyte index;

	public int itemId;

	public long count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref index);
		stream.PutOrGet(ref itemId);
		stream.PutOrGet(ref count);
	}
}
