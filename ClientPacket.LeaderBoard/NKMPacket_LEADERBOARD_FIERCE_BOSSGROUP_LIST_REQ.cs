using Cs.Protocol;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ)]
public sealed class NKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ : ISerializable
{
	public int fierceBossGroupId;

	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref fierceBossGroupId);
		stream.PutOrGet(ref isAll);
	}
}
