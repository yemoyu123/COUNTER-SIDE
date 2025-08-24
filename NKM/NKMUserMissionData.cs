using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Protocol;
using Cs.Shared.Time;
using NKC;

namespace NKM;

public class NKMUserMissionData : ISerializable
{
	private long achievePoint;

	private Dictionary<int, int> dicRefreshInfo = new Dictionary<int, int>();

	private Dictionary<int, NKMMissionData> dicMissions = new Dictionary<int, NKMMissionData>();

	private HashSet<int> completeFlag = new HashSet<int>();

	private DateTime m_dLastRandomMissionRefreshTime;

	public List<NKMMissionData> GetAllMissions => dicMissions.Values.ToList();

	public NKMMissionData GetCompletedMissionData(int missionID)
	{
		NKMMissionData missionDataByMissionId = GetMissionDataByMissionId(missionID);
		if (missionDataByMissionId == null)
		{
			return null;
		}
		if (!missionDataByMissionId.IsComplete)
		{
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionID);
			if (missionTemplet == null)
			{
				Log.Error($"Can not found MissionTemplet. missionId : {missionID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUserMissionData.cs", 31);
				return null;
			}
			if (missionTemplet.m_MissionRequire == 0)
			{
				return null;
			}
		}
		return missionDataByMissionId;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref dicRefreshInfo);
		stream.PutOrGet(ref dicMissions);
		stream.PutOrGet(ref achievePoint);
	}

	public void SetAchievePoint(long point)
	{
		achievePoint = point;
	}

	public void AddAchievePoint(long addPoint)
	{
		achievePoint += addPoint;
	}

	public long GetAchiecePoint()
	{
		return achievePoint;
	}

	public void AddMission(NKMMissionData missionData)
	{
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionData.mission_id);
		if (missionTemplet == null)
		{
			Log.Error($"Invalid MissionId. {missionData.mission_id}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUserMissionData.cs", 61);
			return;
		}
		if (dicMissions.ContainsKey(missionData.group_id))
		{
			Log.Error($"MissionGroup Already exist. {missionData.mission_id}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUserMissionData.cs", 67);
			return;
		}
		dicMissions.Add(missionData.group_id, missionData);
		if (missionTemplet.IsRandomMission && !dicRefreshInfo.ContainsKey(missionData.tabId))
		{
			dicRefreshInfo.Add(missionData.tabId, missionTemplet.m_TabTemplet.m_MissionRefreshFreeCount);
		}
	}

	public void RemoveMission(int groupId)
	{
		dicMissions.Remove(groupId);
	}

	public void RemoveAllRandomMissionInTab(int tabId)
	{
		foreach (NKMMissionData item in dicMissions.Values.Where((NKMMissionData e) => e.tabId == tabId && NKMMissionManager.GetMissionTemplet(e.mission_id).IsRandomMission).ToList())
		{
			dicMissions.Remove(item.group_id);
		}
	}

	public NKMMissionData GetMissionData(NKMMissionTemplet templet)
	{
		if (templet == null)
		{
			return null;
		}
		dicMissions.TryGetValue(templet.m_GroupId, out var value);
		return value;
	}

	public NKMMissionData GetMissionDataByMissionId(int missionId)
	{
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionId);
		if (missionTemplet == null)
		{
			return null;
		}
		dicMissions.TryGetValue(missionTemplet.m_GroupId, out var value);
		return value;
	}

	public NKMMissionData GetMissionDataByGroupId(int groupId)
	{
		dicMissions.TryGetValue(groupId, out var value);
		return value;
	}

	public void SetRandomMissionRefreshCount(int tabId, int refreshCount)
	{
		if (!dicRefreshInfo.ContainsKey(tabId))
		{
			dicRefreshInfo.Add(tabId, refreshCount);
		}
		else
		{
			dicRefreshInfo[tabId] = refreshCount;
		}
	}

	public int GetRandomMissionRefreshCount(int tabId)
	{
		if (dicRefreshInfo.TryGetValue(tabId, out var value))
		{
			return value;
		}
		return 0;
	}

	public bool DecreaseRandomMissionRefreshCount(int tabId)
	{
		if (!dicRefreshInfo.TryGetValue(tabId, out var value))
		{
			return false;
		}
		if (value <= 0)
		{
			return false;
		}
		dicRefreshInfo[tabId] = value - 1;
		return true;
	}

	public List<NKMMissionData> GetAllMissionList(int tabId)
	{
		return dicMissions.Values.Where((NKMMissionData e) => e.tabId == tabId).ToList();
	}

	public List<NKMMissionData> GetRandomMissionList(int tabId)
	{
		return dicMissions.Values.Where((NKMMissionData e) => e.tabId == tabId && NKMMissionManager.GetMissionTemplet(e.mission_id).IsRandomMission)?.ToList();
	}

	public bool IsTabComplete(int tabId)
	{
		return completeFlag.Contains(tabId);
	}

	public void SetTabComplete(int tabId)
	{
		if (!completeFlag.Contains(tabId))
		{
			completeFlag.Add(tabId);
		}
	}

	public bool HasRandomMission(int tabId)
	{
		List<NKMMissionData> allMissionList = GetAllMissionList(tabId);
		if (allMissionList.Count == 0)
		{
			return false;
		}
		IEnumerable<NKMMissionData> enumerable = allMissionList.Where((NKMMissionData e) => NKMMissionManager.GetMissionTemplet(e.mission_id).IsRandomMission);
		if (enumerable == null)
		{
			return true;
		}
		return enumerable.Count() != 0;
	}

	public bool WaitingForRandomMissionRefresh()
	{
		if (m_dLastRandomMissionRefreshTime.Ticks > 0)
		{
			return m_dLastRandomMissionRefreshTime < WeeklyReset.CalcLastReset(ServiceTime.Recent, DayOfWeek.Monday);
		}
		return false;
	}

	public void OnRandomMissionRefresh()
	{
		m_dLastRandomMissionRefreshTime = WeeklyReset.CalcLastReset(ServiceTime.Recent, DayOfWeek.Monday);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
	}

	public bool CheckCompletableMission(NKMUserData user_data, bool bOnlyVisibleTab = true)
	{
		foreach (NKMMissionTabTemplet value in NKMMissionManager.DicMissionTab.Values)
		{
			if (value.m_MissionType != NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION && CheckCompletableMission(user_data, value.m_tabID, bOnlyVisibleTab))
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckCompletableGuideMission(NKMUserData user_data, bool bOnlyVisibleTab = true)
	{
		foreach (NKMMissionTabTemplet value in NKMMissionManager.DicMissionTab.Values)
		{
			if (value.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION && CheckCompletableMission(user_data, value.m_tabID, bOnlyVisibleTab))
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckCompletableMission(NKMUserData user_data, int _NKM_MISSION_TAB_ID, bool bOnlyVisibleTab = false)
	{
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(_NKM_MISSION_TAB_ID);
		if (missionTabTemplet == null || (bOnlyVisibleTab && !missionTabTemplet.m_Visible))
		{
			return false;
		}
		if (!missionTabTemplet.EnableByTag)
		{
			return false;
		}
		if (!NKMMissionManager.CheckMissionTabUnlocked(missionTabTemplet.m_tabID, user_data))
		{
			return false;
		}
		foreach (NKMMissionTemplet item in NKMMissionManager.GetMissionTempletListByType(_NKM_MISSION_TAB_ID))
		{
			NKMMissionData missionData = GetMissionData(item);
			if (missionData != null && !missionData.IsComplete && NKMMissionManager.CanComplete(item, user_data, missionData) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
		}
		if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
		{
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionTabTemplet.m_completeMissionID);
			if (missionTemplet != null && NKMMissionManager.GetMissionStateData(missionTemplet.m_MissionID).IsMissionCanClear)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAlreadyCompleteMission(int missionTabID)
	{
		foreach (NKMMissionData allMission in NKCScenManager.CurrentUserData().m_MissionData.GetAllMissionList(missionTabID))
		{
			if (allMission != null && allMission.IsComplete)
			{
				return true;
			}
		}
		return false;
	}

	public Dictionary<int, NKMMissionData> GetAlreadyCompleteMission(int missionTabID)
	{
		Dictionary<int, NKMMissionData> dictionary = new Dictionary<int, NKMMissionData>();
		foreach (NKMMissionData allMission in NKCScenManager.CurrentUserData().m_MissionData.GetAllMissionList(missionTabID))
		{
			if (allMission != null && allMission.IsComplete && !dictionary.ContainsKey(allMission.mission_id))
			{
				dictionary.Add(allMission.mission_id, allMission);
			}
		}
		return dictionary;
	}

	public void AddOrUpdateMission(NKMMissionData missionData)
	{
		if (NKMMissionManager.GetMissionTemplet(missionData.mission_id) == null)
		{
			Log.Error($"Invalid MissionId. {missionData.mission_id}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMMissionManagerEx.cs", 1484);
		}
		else
		{
			dicMissions[missionData.group_id] = missionData;
		}
	}

	public void SetCompleteMissionData(int missionID)
	{
		NKMMissionData missionDataByMissionId = GetMissionDataByMissionId(missionID);
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionID);
		if (missionTemplet != null)
		{
			NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId);
			if (missionDataByMissionId != null)
			{
				SetComplete(missionDataByMissionId);
			}
			else if (missionTabTemplet != null && missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.TUTORIAL)
			{
				NKMMissionData nKMMissionData = new NKMMissionData(missionTemplet, 0L);
				nKMMissionData.times = 1L;
				SetComplete(nKMMissionData);
			}
			if (NKMMissionManager.GetTrackingMissionTemplet() == missionTemplet)
			{
				NKMMissionManager.SetTrackingMissionTemplet(null);
			}
		}
	}

	private void SetComplete(NKMMissionData missionData)
	{
		NKMMissionTemplet nextMissionTemplet = NKMMissionManager.GetNextMissionTemplet(missionData);
		if (nextMissionTemplet != null)
		{
			if (nextMissionTemplet.m_GroupId == missionData.group_id)
			{
				missionData.ProceedNextMission(nextMissionTemplet.m_MissionID);
			}
			else
			{
				if (nextMissionTemplet.m_GroupId != missionData.group_id && !NKMMissionManager.IsCumulativeCondition(nextMissionTemplet.m_MissionCond.mission_cond))
				{
					AddMission(new NKMMissionData(nextMissionTemplet, NKCSynchronizedTime.GetServerUTCTime().Ticks));
				}
				missionData.SetComplete();
			}
		}
		else
		{
			missionData.SetComplete();
		}
		missionData.last_update_date = NKCSynchronizedTime.GetServerUTCTime().Ticks;
		if (NKMMissionManager.GetMissionTemplet(missionData.mission_id) != null && !dicMissions.ContainsKey(missionData.group_id))
		{
			AddMission(missionData);
		}
	}

	public bool IsMissionCompleted(NKMMissionTemplet cNKMMissionTemplet)
	{
		if (cNKMMissionTemplet == null)
		{
			return false;
		}
		NKMMissionData missionData = GetMissionData(cNKMMissionTemplet);
		if (missionData != null)
		{
			if (NKMMissionManager.CheckCanReset(cNKMMissionTemplet.m_ResetInterval, missionData))
			{
				return false;
			}
			if (missionData.IsComplete)
			{
				return true;
			}
			if (missionData.mission_id > cNKMMissionTemplet.m_MissionID)
			{
				return true;
			}
		}
		return false;
	}
}
