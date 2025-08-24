using System;
using Cs.Protocol;

namespace NKM;

public class NKMMissionData : ISerializable
{
	public int tabId;

	public int mission_id;

	public int group_id;

	public long times;

	public long last_update_date;

	private bool isComplete;

	public bool isEnable = true;

	public DateTime endDate;

	public bool IsComplete => isComplete;

	public DateTime LastUpdateDate => new DateTime(last_update_date);

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tabId);
		stream.PutOrGet(ref mission_id);
		stream.PutOrGet(ref group_id);
		stream.PutOrGet(ref times);
		stream.PutOrGet(ref last_update_date);
		stream.PutOrGet(ref isComplete);
	}

	public NKMMissionData()
	{
	}

	public NKMMissionData(NKMMissionTemplet missionTemplet, long lastUpdateDate)
	{
		tabId = missionTemplet.m_MissionTabId;
		group_id = missionTemplet.m_GroupId;
		mission_id = missionTemplet.m_MissionID;
		endDate = missionTemplet.m_TabTemplet.m_endTime;
		ResetMission(lastUpdateDate);
	}

	public NKMMissionData(NKMMissionTemplet missionTemplet, int dbGroupId, long dbTimes, bool isReward, DateTime dbEndDate, long lastUpdateDate)
	{
		tabId = missionTemplet.m_MissionTabId;
		mission_id = missionTemplet.m_MissionID;
		group_id = dbGroupId;
		endDate = dbEndDate;
		times = dbTimes;
		isComplete = isReward;
		last_update_date = lastUpdateDate;
	}

	public void ProceedNextMission(int nextMissionId)
	{
		mission_id = nextMissionId;
		isComplete = false;
	}

	public void UpdateMissionData(int missionId, long missionTimes, bool missionIsComplete, long lastUpdateDate)
	{
		mission_id = missionId;
		times = missionTimes;
		isComplete = missionIsComplete;
		last_update_date = lastUpdateDate;
	}

	public void ResetMission(long lastUpdateDate)
	{
		times = 0L;
		isComplete = false;
		last_update_date = lastUpdateDate;
	}

	public void SetComplete()
	{
		isComplete = true;
	}

	public bool CheckKeepAccumulating()
	{
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(mission_id);
		if (missionTemplet == null)
		{
			return false;
		}
		if (NKMMissionManager.IsAchiveMissionType(missionTemplet.m_TabTemplet.m_MissionType))
		{
			return true;
		}
		if (IsComplete)
		{
			return false;
		}
		if (NKMMissionManager.IsUnitGrowthCondition(missionTemplet.m_MissionCond.mission_cond) && missionTemplet.m_Times <= times)
		{
			return false;
		}
		return true;
	}
}
