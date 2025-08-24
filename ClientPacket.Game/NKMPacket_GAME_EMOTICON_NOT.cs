using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_EMOTICON_NOT)]
public sealed class NKMPacket_GAME_EMOTICON_NOT : ISerializable
{
	public long senderUserUID;

	public int emoticonID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref senderUserUID);
		stream.PutOrGet(ref emoticonID);
	}
}
