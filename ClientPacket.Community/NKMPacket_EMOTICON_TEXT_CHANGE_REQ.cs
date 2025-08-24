using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_EMOTICON_TEXT_CHANGE_REQ)]
public sealed class NKMPacket_EMOTICON_TEXT_CHANGE_REQ : ISerializable
{
	public int presetIndex;

	public int emoticonId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref presetIndex);
		stream.PutOrGet(ref emoticonId);
	}
}
