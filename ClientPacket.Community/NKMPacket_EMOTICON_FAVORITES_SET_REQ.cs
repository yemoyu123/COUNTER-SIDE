using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_EMOTICON_FAVORITES_SET_REQ)]
public sealed class NKMPacket_EMOTICON_FAVORITES_SET_REQ : ISerializable
{
	public int emoticonId;

	public bool favoritesOption;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref emoticonId);
		stream.PutOrGet(ref favoritesOption);
	}
}
