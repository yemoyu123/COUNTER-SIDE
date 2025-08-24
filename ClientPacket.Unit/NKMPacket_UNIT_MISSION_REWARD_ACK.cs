using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_UNIT_MISSION_REWARD_ACK)]
public sealed class NKMPacket_UNIT_MISSION_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitMissionData missionData = new NKMUnitMissionData();

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref missionData);
		stream.PutOrGet(ref rewardData);
	}
}
