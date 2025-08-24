using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Chat;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_CHAT_ACK)]
public sealed class NKMPacket_PRIVATE_CHAT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long messageUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref messageUid);
	}
}
