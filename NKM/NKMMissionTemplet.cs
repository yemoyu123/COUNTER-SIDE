using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMMissionTemplet : INKMTemplet
{
	private string m_DateStrID = string.Empty;

	public int m_MissionTabId;

	public int m_GroupId;

	public int m_MissionID;

	public long m_Times;

	public int m_MissionRequire;

	public string m_MissionIcon = string.Empty;

	public string m_MissionTitle = string.Empty;

	public string m_MissionDesc = string.Empty;

	public string m_MissionTip = string.Empty;

	public string m_ShortCut = string.Empty;

	public int m_ForceClearStage;

	public NKM_SHORTCUT_TYPE m_ShortCutType;

	public NKM_MISSION_RESET_INTERVAL m_ResetInterval = NKM_MISSION_RESET_INTERVAL.NONE;

	public int m_MissionPoolID;

	public bool m_bResetCounterGroup;

	private bool m_ResetCounterGroupId = true;

	public string m_TrackingEvent = string.Empty;

	public long m_RewardTimes;

	public long m_MinRewardTimes;

	private string m_OpenTag;

	public NKMIntervalTemplet intervalTemplet = NKMIntervalTemplet.Invalid;

	public MissionCond m_MissionCond = new MissionCond();

	public List<MissionChange> m_MissionChange = new List<MissionChange>();

	public List<MissionReward> m_MissionRewardOpened = new List<MissionReward>();

	public List<MissionReward> m_MissionRewardOriginal = new List<MissionReward>();

	public NKMMissionTabTemplet m_TabTemplet;

	public List<MissionReward> m_MissionReward => m_MissionRewardOpened;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public bool EnableByInterval
	{
		get
		{
			if (intervalTemplet.IsValid)
			{
				return intervalTemplet.IsValidTime(ServiceTime.Now);
			}
			return true;
		}
	}

	public int Key => m_MissionID;

	public bool IsRandomMission => m_MissionPoolID != 0;

	public static NKMMissionTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 525))
		{
			return null;
		}
		NKMMissionTemplet nKMMissionTemplet = new NKMMissionTemplet();
		bool flag = true;
		cNKMLua.GetData("m_ResetCounterGroupId", ref nKMMissionTemplet.m_ResetCounterGroupId);
		nKMMissionTemplet.m_MissionCond.value1 = new List<int>();
		flag &= cNKMLua.GetData("m_MissionCounterGroupID", ref nKMMissionTemplet.m_GroupId);
		flag &= cNKMLua.GetData("m_MissionID", ref nKMMissionTemplet.m_MissionID);
		flag &= cNKMLua.GetData("m_MissionTabId", ref nKMMissionTemplet.m_MissionTabId);
		flag &= cNKMLua.GetData("m_ResetInterval", ref nKMMissionTemplet.m_ResetInterval);
		flag &= cNKMLua.GetData("m_Times", ref nKMMissionTemplet.m_Times);
		cNKMLua.GetData("m_RewardTimes", ref nKMMissionTemplet.m_RewardTimes);
		cNKMLua.GetData("m_MinRewardTimes", ref nKMMissionTemplet.m_MinRewardTimes);
		cNKMLua.GetData("m_MissionIcon", ref nKMMissionTemplet.m_MissionIcon);
		cNKMLua.GetData("m_MissionTitle", ref nKMMissionTemplet.m_MissionTitle);
		cNKMLua.GetData("m_MissionDesc", ref nKMMissionTemplet.m_MissionDesc);
		cNKMLua.GetData("m_MissionTip", ref nKMMissionTemplet.m_MissionTip);
		cNKMLua.GetData("m_MissionRequire", ref nKMMissionTemplet.m_MissionRequire);
		cNKMLua.GetData("m_ShortCutType", ref nKMMissionTemplet.m_ShortCutType);
		cNKMLua.GetData("m_ShortCut", ref nKMMissionTemplet.m_ShortCut);
		cNKMLua.GetData("m_MissionCond", ref nKMMissionTemplet.m_MissionCond.mission_cond);
		cNKMLua.GetData("m_MissionPoolID", ref nKMMissionTemplet.m_MissionPoolID);
		string rValue = null;
		cNKMLua.GetData("m_MissionValue", ref rValue);
		if (!string.IsNullOrEmpty(rValue))
		{
			nKMMissionTemplet.m_MissionCond.value1 = (from e in rValue.Split(',')
				select int.Parse(e)).ToList();
		}
		cNKMLua.GetData("m_MissionValue2", ref nKMMissionTemplet.m_MissionCond.value2);
		cNKMLua.GetData("m_MissionValue3", ref nKMMissionTemplet.m_MissionCond.value3);
		cNKMLua.GetData("m_ForceClearStage", ref nKMMissionTemplet.m_ForceClearStage);
		cNKMLua.GetData("m_OpenTag", ref nKMMissionTemplet.m_OpenTag);
		cNKMLua.GetData("m_DateStrID", ref nKMMissionTemplet.m_DateStrID);
		if (nKMMissionTemplet.m_MissionCond.mission_cond == NKM_MISSION_COND.USE_RESOURCE)
		{
			List<int> list = new List<int>();
			int num = 0;
			while (true)
			{
				if (cNKMLua.OpenTable($"m_MissionValueChange{num + 1}"))
				{
					int num2 = 1;
					for (int rValue2 = 0; cNKMLua.GetData(num2, ref rValue2); num2++)
					{
						list.Add(rValue2);
					}
					cNKMLua.CloseTable();
				}
				if (list.Count != 2)
				{
					break;
				}
				nKMMissionTemplet.m_MissionChange.Add(new MissionChange
				{
					value = list[0],
					tiems = list[1]
				});
				list.Clear();
				num++;
			}
		}
		int num3 = 0;
		while (true)
		{
			MissionReward missionReward = new MissionReward();
			cNKMLua.GetData($"m_RewardID_{num3 + 1}", ref missionReward.reward_id);
			cNKMLua.GetData($"m_RewardValue_{num3 + 1}", ref missionReward.reward_value);
			if (!cNKMLua.GetData($"m_RewardType_{num3 + 1}", ref missionReward.reward_type))
			{
				break;
			}
			if (nKMMissionTemplet.m_MissionRewardOriginal == null)
			{
				nKMMissionTemplet.m_MissionRewardOriginal = new List<MissionReward>();
			}
			nKMMissionTemplet.m_MissionRewardOriginal.Add(missionReward);
			num3++;
		}
		cNKMLua.GetData("m_bResetCounterGroup", ref nKMMissionTemplet.m_bResetCounterGroup);
		cNKMLua.GetData("m_TrackingEvent", ref nKMMissionTemplet.m_TrackingEvent);
		if (!flag)
		{
			Log.Error($"NKMMissionTemplet Load failed. groupId:{nKMMissionTemplet.m_GroupId} missionId:{nKMMissionTemplet.m_MissionID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 629);
			return null;
		}
		nKMMissionTemplet.m_TabTemplet = NKMMissionManager.GetMissionTabTemplet(nKMMissionTemplet.m_MissionTabId);
		if (nKMMissionTemplet.m_TabTemplet == null)
		{
			Log.Error($"NKMMissioNTemplet Load failed, tabTemplet is null, tab id: {nKMMissionTemplet.m_MissionTabId} missionId:{nKMMissionTemplet.m_MissionID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 636);
			return null;
		}
		return nKMMissionTemplet;
	}

	public void RecalculateMissionTemplets()
	{
		if (!EnableByTag)
		{
			return;
		}
		NKM_MISSION_TYPE missionType = m_TabTemplet.m_MissionType;
		if ((missionType == NKM_MISSION_TYPE.EVENT || (uint)(missionType - 10) <= 1u) && m_ResetCounterGroupId && !string.IsNullOrWhiteSpace(m_TabTemplet.intervalId))
		{
			NKMMissionManager.AddCounterGroupResetData(m_GroupId, this);
		}
		m_MissionRewardOpened.Clear();
		foreach (MissionReward item in m_MissionRewardOriginal)
		{
			if (NKMRewardTemplet.IsOpenedReward(item.reward_type, item.reward_id, useRandomContract: false))
			{
				m_MissionRewardOpened.Add(item);
			}
			else
			{
				NKMTempletError.Add($"[RecalculateMission] \ufffd\u033cǿ\ufffd \ufffdɸ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd±\u05ff\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\u0735\u02f4ϴ\ufffd. missionId:{m_MissionID} rewardId:{item.reward_id} RewardType:{item.reward_type}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 681);
			}
		}
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void JoinIntervalTemplet()
	{
		if (!string.IsNullOrEmpty(m_DateStrID))
		{
			intervalTemplet = NKMIntervalTemplet.Find(m_DateStrID);
			if (intervalTemplet == null)
			{
				NKMTempletError.Add($"[MissionTemplet: {Key}] \ufffd\ufffd\ufffd\u0379\ufffd\ufffd\ufffd ã\ufffd\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffdϴ\ufffd. m_DateStrID:{m_DateStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 701);
			}
		}
	}

	public void Join()
	{
		RecalculateMissionTemplets();
		NKMOpenTagManager.AddRecalculateAction("Mission_" + Key, RecalculateMissionTemplets);
	}

	public void Validate()
	{
		if (NKMMissionManager.ContainsCounterGroupResetData(m_GroupId))
		{
			if (!m_ResetCounterGroupId)
			{
				Log.ErrorAndExit($"[MissionTemplet: {Key}] \ufffd\ufffd\ufffdºҰ\ufffd \ufffd\u033c\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd CounterGroupId: {m_GroupId} MissionTabID: {m_MissionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 717);
			}
			if (string.IsNullOrWhiteSpace(m_TabTemplet.intervalId))
			{
				Log.ErrorAndExit($"[MissionTemplet: {Key}] \ufffd\ufffdȣ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdð\ufffd CounterGroupId: {m_GroupId} MissionTabID: {m_MissionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 722);
			}
		}
		if (m_MissionRequire > 0)
		{
			if (m_MissionRequire == m_MissionID)
			{
				Log.ErrorAndExit($"[MissionTemplet: {Key}] \ufffdڱ\ufffd \ufffdڽ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd: {m_GroupId} MissionTabID: {m_MissionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 731);
			}
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(m_MissionRequire);
			if (missionTemplet == null)
			{
				Log.ErrorAndExit($"[MissionTemplet: {Key}] \ufffd\ufffd\ufffd\ufffd \ufffd\u033c\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. RequireMissionID : {m_MissionRequire}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 737);
			}
			if (missionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.ON_COMPLETE)
			{
				NKMTempletError.Add($"[MissionTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd \ufffd\u033c\ufffd\ufffd\ufffd \ufffdʱ\ufffdȭ Ÿ\ufffd\ufffd\ufffd\ufffd ON_COMPLETE\ufffd\ufffd \ufffd\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd RequireMissionID : {m_MissionRequire}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 742);
			}
		}
		NKM_MISSION_TYPE missionType = m_TabTemplet.m_MissionType;
		if (((uint)(missionType - 1) <= 3u || (uint)(missionType - 18) <= 1u) && !string.IsNullOrEmpty(m_DateStrID))
		{
			NKMTempletError.Add($"[MissionTemplet: {Key}] \ufffd\ufffd\ufffd\u0379\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\u033c\ufffd\ufffdԴϴ\ufffd. MissionType:{m_TabTemplet.m_MissionType} MissionTabID: {m_MissionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 756);
		}
		if (m_MissionChange.Count > 0)
		{
			if (m_MissionChange.Any((MissionChange e) => e.value <= 0))
			{
				NKMTempletError.Add($"[MissionTemplet:{Key}] \ufffd\u033cǰ\ufffd \ufffd\ufffdȯ value \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 765);
			}
			if (m_MissionChange.Any((MissionChange e) => e.tiems <= 0))
			{
				NKMTempletError.Add($"[MissionTemplet:{Key}] \ufffd\u033cǰ\ufffd \ufffd\ufffdȯ times\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 770);
			}
			if ((from e in m_MissionChange
				group e by e.value).Any((IGrouping<int, MissionChange> e) => e.Count() > 1))
			{
				NKMTempletError.Add($"[MissionTemplet:{Key}] \ufffd\u033cǰ\ufffd \ufffd\ufffdȯ \ufffd\ufffd\ufffd\ufffd\ufffdͿ\ufffd \ufffdߺ\ufffd\ufffd\ufffd value \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 775);
			}
		}
		if (m_ResetInterval == NKM_MISSION_RESET_INTERVAL.ON_COMPLETE && m_RewardTimes <= 0)
		{
			NKMTempletError.Add($"[MissionTemplet:{Key}] \ufffdݺ\ufffd \ufffd\u033c\ufffd \ufffdޱ\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd Ƚ\ufffd\ufffd(m_RewardTimes)\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\u05bc\ufffd\ufffd\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 783);
		}
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}

	public string GetTitle()
	{
		return NKCStringTable.GetString(m_MissionTitle);
	}

	public string GetDesc()
	{
		return NKCStringTable.GetString(m_MissionDesc);
	}

	public string GetTip()
	{
		if (string.IsNullOrEmpty(NKCStringTable.GetString(m_MissionTip)))
		{
			return m_MissionTip;
		}
		return NKCStringTable.GetString(m_MissionTip);
	}
}
