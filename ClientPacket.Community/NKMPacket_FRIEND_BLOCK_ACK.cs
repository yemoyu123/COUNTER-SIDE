using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_BLOCK_ACK)]
public sealed class NKMPacket_FRIEND_BLOCK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long friendCode;

	public bool isCancel;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref friendCode);
		stream.PutOrGet(ref isCancel);
	}
}
