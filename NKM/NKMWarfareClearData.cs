using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public class NKMWarfareClearData : ISerializable
{
	public int m_WarfareID;

	public bool m_mission_result_1;

	public bool m_mission_result_2;

	public NKMRewardData m_RewardDataList;

	public NKMRewardData m_ContainerRewards;

	public int m_enemiesKillCount;

	public NKMRewardData m_MissionReward;

	public bool m_MissionRewardResult;

	public NKMRewardData m_OnetimeRewards;

	public List<bool> m_OnetimeRewardResults = new List<bool>(3);

	public NKMStagePlayData m_StagePlayData;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_WarfareID);
		stream.PutOrGet(ref m_mission_result_1);
		stream.PutOrGet(ref m_mission_result_2);
		stream.PutOrGet(ref m_RewardDataList);
		stream.PutOrGet(ref m_ContainerRewards);
		stream.PutOrGet(ref m_enemiesKillCount);
		stream.PutOrGet(ref m_MissionReward);
		stream.PutOrGet(ref m_MissionRewardResult);
		stream.PutOrGet(ref m_OnetimeRewards);
		stream.PutOrGet(ref m_OnetimeRewardResults);
		stream.PutOrGet(ref m_StagePlayData);
	}
}
