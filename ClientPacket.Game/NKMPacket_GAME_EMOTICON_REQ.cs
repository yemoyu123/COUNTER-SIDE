using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_EMOTICON_REQ)]
public sealed class NKMPacket_GAME_EMOTICON_REQ : ISerializable
{
	public int emoticonID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref emoticonID);
	}
}
