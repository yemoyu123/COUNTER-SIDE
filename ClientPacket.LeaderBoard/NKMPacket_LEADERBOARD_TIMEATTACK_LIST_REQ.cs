using Cs.Protocol;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_TIMEATTACK_LIST_REQ)]
public sealed class NKMPacket_LEADERBOARD_TIMEATTACK_LIST_REQ : ISerializable
{
	public int stageId;

	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
		stream.PutOrGet(ref isAll);
	}
}
