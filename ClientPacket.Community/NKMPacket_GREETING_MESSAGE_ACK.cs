using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_GREETING_MESSAGE_ACK)]
public sealed class NKMPacket_GREETING_MESSAGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public string message;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref message);
	}
}
