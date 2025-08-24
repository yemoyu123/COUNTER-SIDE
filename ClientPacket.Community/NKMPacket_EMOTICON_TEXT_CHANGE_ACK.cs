using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_EMOTICON_TEXT_CHANGE_ACK)]
public sealed class NKMPacket_EMOTICON_TEXT_CHANGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int presetIndex;

	public int emoticonId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref presetIndex);
		stream.PutOrGet(ref emoticonId);
	}
}
