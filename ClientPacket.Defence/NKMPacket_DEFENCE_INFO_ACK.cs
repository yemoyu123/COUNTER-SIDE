using System.Collections.Generic;
using ClientPacket.LeaderBoard;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_INFO_ACK)]
public sealed class NKMPacket_DEFENCE_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int defenceTempletId;

	public int bestScore;

	public bool missionResult1;

	public bool missionResult2;

	public int rank;

	public int rankPercent;

	public bool canReceiveRankReward;

	public NKMDefenceRankData topRankProfile = new NKMDefenceRankData();

	public List<int> scoreRewardIds = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref defenceTempletId);
		stream.PutOrGet(ref bestScore);
		stream.PutOrGet(ref missionResult1);
		stream.PutOrGet(ref missionResult2);
		stream.PutOrGet(ref rank);
		stream.PutOrGet(ref rankPercent);
		stream.PutOrGet(ref canReceiveRankReward);
		stream.PutOrGet(ref topRankProfile);
		stream.PutOrGet(ref scoreRewardIds);
	}
}
