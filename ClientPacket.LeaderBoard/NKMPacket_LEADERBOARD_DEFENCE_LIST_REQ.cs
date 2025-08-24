using Cs.Protocol;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_DEFENCE_LIST_REQ)]
public sealed class NKMPacket_LEADERBOARD_DEFENCE_LIST_REQ : ISerializable
{
	public int defenceId;

	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref defenceId);
		stream.PutOrGet(ref isAll);
	}
}
