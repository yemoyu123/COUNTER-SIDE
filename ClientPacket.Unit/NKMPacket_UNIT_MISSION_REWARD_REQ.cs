using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_UNIT_MISSION_REWARD_REQ)]
public sealed class NKMPacket_UNIT_MISSION_REWARD_REQ : ISerializable
{
	public int unitId;

	public int missionId;

	public int stepId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitId);
		stream.PutOrGet(ref missionId);
		stream.PutOrGet(ref stepId);
	}
}
