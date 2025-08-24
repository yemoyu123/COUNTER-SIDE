using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMMentoringTemplet : INKMTemplet, INKMTempletEx
{
	public int SeasonId { get; private set; }

	public int RewardGroupId { get; private set; }

	public int MissionTabId { get; private set; }

	public int AllClearMissionId { get; private set; }

	public DateTime StartDate { get; private set; }

	public DateTime EndDate { get; private set; }

	public static IEnumerable<NKMMentoringTemplet> Values => NKMTempletContainer<NKMMentoringTemplet>.Values;

	public int Key => SeasonId;

	public static NKMMentoringTemplet Find(int seasonId)
	{
		return NKMTempletContainer<NKMMentoringTemplet>.Find(seasonId);
	}

	public static NKMMentoringTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 37))
		{
			return null;
		}
		int rValue = 0;
		bool data = lua.GetData("m_MentoringSeasonID", ref rValue);
		int rValue2 = 0;
		bool num = data & lua.GetData("m_MentoringAllClearMissionID", ref rValue2);
		int rValue3 = 0;
		bool num2 = num & lua.GetData("m_MissionTabID", ref rValue3);
		int rValue4 = 0;
		if (!(num2 & lua.GetData("m_RewardGroupID", ref rValue4)))
		{
			return null;
		}
		return new NKMMentoringTemplet
		{
			SeasonId = rValue,
			MissionTabId = rValue3,
			RewardGroupId = rValue4,
			AllClearMissionId = rValue2
		};
	}

	public static NKMMentoringTemplet GetMentoringTemplet(in DateTime current)
	{
		foreach (NKMMentoringTemplet value in Values)
		{
			if (current >= value.StartDate && current < value.EndDate)
			{
				return value;
			}
		}
		return null;
	}

	public static bool TryGetNextMentoringTemplet(int curSeasonId, out NKMMentoringTemplet nextTemplet)
	{
		nextTemplet = Values.FirstOrDefault((NKMMentoringTemplet e) => e.SeasonId > curSeasonId);
		if (nextTemplet == null)
		{
			return false;
		}
		return true;
	}

	public bool IsSeasonOut(DateTime current)
	{
		if (current < StartDate || current > EndDate)
		{
			return true;
		}
		return false;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void JoinIntervalTemplet()
	{
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(MissionTabId);
		if (missionTabTemplet == null)
		{
			Log.ErrorAndExit($"tab templet is null, tab id: {MissionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 117);
			return;
		}
		StartDate = missionTabTemplet.m_startTime;
		EndDate = missionTabTemplet.m_endTime;
	}

	public void Validate()
	{
		NKMMentoringTemplet nKMMentoringTemplet = Values.FirstOrDefault((NKMMentoringTemplet e) => e.SeasonId == SeasonId - 1);
		if (nKMMentoringTemplet != null)
		{
			TimeSpan timeSpan = TimeSpan.FromMinutes(NKMMentoringConst.MentoringSeasonInitMinutes);
			if (StartDate - nKMMentoringTemplet.EndDate < timeSpan)
			{
				Log.ErrorAndExit($"전 시즌의 endDate과 이번 시즌의 startDate의 차이가 정산기간 보다 적습니다. season id: {SeasonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 134);
				return;
			}
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(MissionTabId);
		if (missionTabTemplet == null)
		{
			Log.ErrorAndExit($"TabId가 MissionTabTemplet에 존재하지 않습니다. season id: {SeasonId}, tabId: {MissionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 142);
			return;
		}
		if (missionTabTemplet.m_MissionType != NKM_MISSION_TYPE.MENTORING)
		{
			Log.ErrorAndExit($"MissionTabTemplet의 MissionType이 유효하지 않습니다. mission type: {missionTabTemplet.m_MissionType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 148);
			return;
		}
		if (NKMMentoringRewardTemplet.GetRewardGroupList(RewardGroupId) == null)
		{
			Log.ErrorAndExit($"RewardGroupId가 MentoringRewardTemplet에 존재하지 않았습니다. season id: {SeasonId}, rewardGroupId: {RewardGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 155);
			return;
		}
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(AllClearMissionId);
		if (missionTemplet == null)
		{
			Log.ErrorAndExit($"MissionId가 MissionTemplet에 존재하지 않았습니다. season id: {SeasonId}, missionId: {AllClearMissionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 162);
			return;
		}
		if (missionTemplet.m_TabTemplet.m_tabID != MissionTabId)
		{
			Log.ErrorAndExit($"멘토링의 MissionTabId와 MissionTemplet의 TabTemplet의 TabId가 같지 않습니다. mission tab id: {missionTemplet.m_TabTemplet.m_tabID}, mentroing tab id: {MissionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 168);
			return;
		}
		if (missionTemplet.m_TabTemplet.m_MissionType != NKM_MISSION_TYPE.MENTORING)
		{
			Log.ErrorAndExit($"MissionTemplt.TabTemplet의 MissionType이 유요하지 않습니다. season id: {SeasonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 174);
			return;
		}
		if (missionTemplet.m_MissionCond.mission_cond != NKM_MISSION_COND.MENTORING_MISSION_CLEARED)
		{
			Log.ErrorAndExit($"MissionConditionType이 유효하지 않습니다. season id: {SeasonId}, missionConditionType: {missionTemplet.m_MissionCond.mission_cond}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 180);
			return;
		}
		if (missionTemplet.m_MissionCond.value1.Count == 0)
		{
			Log.ErrorAndExit($"MissionConditionValue가 비어 있습니다. season id: {SeasonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 186);
			return;
		}
		if (missionTemplet.m_Times != missionTemplet.m_MissionCond.value1.Count)
		{
			Log.ErrorAndExit($"MissionTempet의 Times와 MissionCond 조건 카운트가 일치하지 않습니다. season id: {SeasonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 192);
			return;
		}
		foreach (int item in missionTemplet.m_MissionCond.value1)
		{
			missionTemplet = NKMMissionManager.GetMissionTemplet(item);
			if (missionTemplet == null)
			{
				Log.ErrorAndExit($"missionTemplet의 missionCond에 해당 mission id가 존재하지 않습니다. mission id: {item}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 201);
				break;
			}
			if (missionTemplet.m_TabTemplet.m_tabID != MissionTabId)
			{
				Log.ErrorAndExit($"멘토링의 MissionTabId와 MissionTemplet의 TabTemplet의 TabId가 같지 않습니다. mission tab id: {missionTemplet.m_TabTemplet.m_tabID}, mentroing tab id: {MissionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringTemplet.cs", 207);
				break;
			}
		}
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
