using Cs.Protocol;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_FIERCE_LIST_REQ)]
public sealed class NKMPacket_LEADERBOARD_FIERCE_LIST_REQ : ISerializable
{
	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isAll);
	}
}
