using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_DRAFT_PVP_CASTING_VOTE_SHIP_ACK)]
public sealed class NKMPacket_DRAFT_PVP_CASTING_VOTE_SHIP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public PvpCastingVoteData pvpCastingVoteData = new PvpCastingVoteData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref pvpCastingVoteData);
	}
}
