using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_CASTING_VOTE_OPERATOR_ACK)]
public sealed class NKMPacket_PVP_CASTING_VOTE_OPERATOR_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public PvpCastingVoteData pvpCastingVoteData = new PvpCastingVoteData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref pvpCastingVoteData);
	}
}
