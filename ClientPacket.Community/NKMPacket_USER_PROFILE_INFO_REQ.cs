using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_USER_PROFILE_INFO_REQ)]
public sealed class NKMPacket_USER_PROFILE_INFO_REQ : ISerializable
{
	public long userUID;

	public NKM_DECK_TYPE deckType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUID);
		stream.PutOrGetEnum(ref deckType);
	}
}
