using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISSION_GET_COMPLETE_REWARD_REQ)]
public sealed class NKMPacket_MISSION_GET_COMPLETE_REWARD_REQ : ISerializable
{
	public int missionID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref missionID);
	}
}
