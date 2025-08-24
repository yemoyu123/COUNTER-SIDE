using ClientPacket.LeaderBoard;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_LEAGUE_PVP_RANK_LIST_REQ)]
public sealed class NKMPacket_LEAGUE_PVP_RANK_LIST_REQ : ISerializable
{
	public RANK_TYPE rankType;

	public LeaderBoardRangeType range;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref rankType);
		stream.PutOrGetEnum(ref range);
	}
}
