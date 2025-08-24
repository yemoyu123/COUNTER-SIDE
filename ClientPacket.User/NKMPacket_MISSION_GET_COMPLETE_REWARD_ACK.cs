using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISSION_GET_COMPLETE_REWARD_ACK)]
public sealed class NKMPacket_MISSION_GET_COMPLETE_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public NKMMissionData updatedMissionData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref updatedMissionData);
	}
}
