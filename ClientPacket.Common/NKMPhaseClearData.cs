using System.Collections.Generic;
using Cs.Protocol;
using NKM.Templet;

namespace ClientPacket.Common;

public sealed class NKMPhaseClearData : ISerializable
{
	public int stageId;

	public bool missionResult1;

	public bool missionResult2;

	public NKMRewardData missionReward;

	public bool missionRewardResult;

	public NKMRewardData oneTimeRewards;

	public List<bool> onetimeRewardResults = new List<bool>();

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
		stream.PutOrGet(ref missionResult1);
		stream.PutOrGet(ref missionResult2);
		stream.PutOrGet(ref missionReward);
		stream.PutOrGet(ref missionRewardResult);
		stream.PutOrGet(ref oneTimeRewards);
		stream.PutOrGet(ref onetimeRewardResults);
		stream.PutOrGet(ref rewardData);
	}
}
