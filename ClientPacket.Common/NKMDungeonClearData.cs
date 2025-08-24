using System.Collections.Generic;
using Cs.Protocol;
using NKM.Templet;

namespace ClientPacket.Common;

public sealed class NKMDungeonClearData : ISerializable
{
	public int dungeonId;

	public bool missionResult1;

	public bool missionResult2;

	public NKMRewardData missionReward;

	public bool missionRewardResult;

	public NKMRewardData oneTimeRewards;

	public List<bool> onetimeRewardResults = new List<bool>();

	public NKMRewardData rewardData;

	public int unitExp;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref dungeonId);
		stream.PutOrGet(ref missionResult1);
		stream.PutOrGet(ref missionResult2);
		stream.PutOrGet(ref missionReward);
		stream.PutOrGet(ref missionRewardResult);
		stream.PutOrGet(ref oneTimeRewards);
		stream.PutOrGet(ref onetimeRewardResults);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref unitExp);
	}
}
