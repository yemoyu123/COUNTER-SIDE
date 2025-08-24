using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_TEAM_COLLECTION_REWARD_REQ)]
public sealed class NKMPacket_TEAM_COLLECTION_REWARD_REQ : ISerializable
{
	public int teamID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref teamID);
	}
}
