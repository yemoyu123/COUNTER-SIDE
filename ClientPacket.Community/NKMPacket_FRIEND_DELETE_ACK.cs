using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_DELETE_ACK)]
public sealed class NKMPacket_FRIEND_DELETE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long friendCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref friendCode);
	}
}
