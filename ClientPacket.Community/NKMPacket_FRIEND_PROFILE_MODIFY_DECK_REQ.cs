using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ)]
public sealed class NKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ : ISerializable
{
	public NKMDeckIndex deckIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
	}
}
