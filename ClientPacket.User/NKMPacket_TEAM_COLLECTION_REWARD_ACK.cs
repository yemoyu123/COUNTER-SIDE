using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_TEAM_COLLECTION_REWARD_ACK)]
public sealed class NKMPacket_TEAM_COLLECTION_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public NKMTeamCollectionData teamCollectionData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref teamCollectionData);
	}
}
