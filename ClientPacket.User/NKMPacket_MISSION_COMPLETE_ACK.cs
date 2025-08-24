using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISSION_COMPLETE_ACK)]
public sealed class NKMPacket_MISSION_COMPLETE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int missionID;

	public NKMRewardData rewardData;

	public NKMAdditionalReward additionalReward = new NKMAdditionalReward();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref missionID);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref additionalReward);
	}
}
