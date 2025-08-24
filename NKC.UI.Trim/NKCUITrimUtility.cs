using System;
using System.Collections.Generic;
using System.Text;
using ClientPacket.Common;
using Cs.Logging;
using NKC.Templet;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Trim;

public static class NKCUITrimUtility
{
	private const string openTag = "DIMENSION_TRIM";

	private static bool intervalTempletJoined;

	public static bool OpenTagEnabled => NKMOpenTagManager.IsOpened("DIMENSION_TRIM");

	public static void TrimIntervalJoin()
	{
		if (!intervalTempletJoined)
		{
			NKMTrimIntervalTemplet.Join();
			intervalTempletJoined = true;
		}
	}

	public static void InitBattleCondition(Transform battleCondRoot, bool showToolTip)
	{
		if (battleCondRoot == null)
		{
			return;
		}
		NKCUIComBattleCondition[] componentsInChildren = battleCondRoot.GetComponentsInChildren<NKCUIComBattleCondition>(includeInactive: true);
		int num = ((componentsInChildren != null) ? componentsInChildren.Length : 0);
		for (int i = 0; i < num; i++)
		{
			if (showToolTip)
			{
				componentsInChildren[i].Init(OnBCButtonDown);
			}
			else
			{
				componentsInChildren[i].Init();
			}
		}
	}

	private static void OnBCButtonDown(string battleCondId, Vector3 position)
	{
		NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(battleCondId);
		if (templetByStrID != null)
		{
			NKCUITrimToolTip.Instance.Open(templetByStrID.BattleCondDesc_Translated, position);
		}
	}

	public static void SetBattleCondition(Transform battleCondRoot, NKMTrimTemplet trimTemplet, int trimLevel, bool showToolTip)
	{
		if (battleCondRoot == null)
		{
			return;
		}
		SortedSet<int> conditionList = new SortedSet<int>();
		if (trimTemplet != null)
		{
			foreach (KeyValuePair<int, List<NKMTrimDungeonTemplet>> trimDungeonTemplet in trimTemplet.TrimDungeonTemplets)
			{
				trimDungeonTemplet.Value.Find((NKMTrimDungeonTemplet e) => e.TrimLevelLow <= trimLevel && e.TrimLevelHigh >= trimLevel)?.TrimDungeonBattleCondition.ForEach(delegate(string e)
				{
					NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(e);
					if (templetByStrID != null && !templetByStrID.m_bHide)
					{
						conditionList.Add(templetByStrID.BattleCondID);
					}
				});
			}
		}
		NKCUIComBattleCondition[] componentsInChildren = battleCondRoot.GetComponentsInChildren<NKCUIComBattleCondition>(includeInactive: true);
		int num = ((componentsInChildren != null) ? componentsInChildren.Length : 0);
		_ = conditionList.Count;
		int num2 = 0;
		foreach (int item in conditionList)
		{
			NKCUIComBattleCondition nKCUIComBattleCondition = null;
			if (num2 >= num)
			{
				nKCUIComBattleCondition = NKCUIComBattleCondition.GetNewInstance("AB_UI_TRIM", "AB_UI_TRIM_BATTLE_CONDITION", battleCondRoot);
				if (showToolTip)
				{
					nKCUIComBattleCondition.SetButtonDownFunc(OnBCButtonDown);
				}
			}
			else
			{
				nKCUIComBattleCondition = componentsInChildren[num2];
			}
			if (nKCUIComBattleCondition != null)
			{
				NKCUtil.SetGameobjectActive(nKCUIComBattleCondition.gameObject, bValue: true);
				nKCUIComBattleCondition.SetData(item);
			}
			num2++;
		}
		for (int num3 = num2; num3 < num; num3++)
		{
			if (!(componentsInChildren[num3] == null))
			{
				NKCUtil.SetGameobjectActive(componentsInChildren[num3].gameObject, bValue: false);
			}
		}
	}

	public static string GetTrimDeckKey(int trimId, int trimDungeonId, long userUId)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("TRIM_");
		stringBuilder.Append(trimId);
		stringBuilder.Append("_");
		stringBuilder.Append(trimDungeonId);
		stringBuilder.Append("_");
		stringBuilder.Append(userUId);
		return stringBuilder.ToString();
	}

	public static int GetRemainEnterCount(NKMTrimIntervalTemplet trimInterval)
	{
		NKMTrimIntervalData nKMTrimIntervalData = NKCScenManager.CurrentUserData()?.TrimData.TrimIntervalData;
		int num = trimInterval?.WeeklyEnterLimit ?? 0;
		if (nKMTrimIntervalData != null)
		{
			num -= nKMTrimIntervalData.trimTryCount;
		}
		return num;
	}

	public static string GetEnterLimitMsg(NKMTrimIntervalTemplet trimInterval)
	{
		int num = 0;
		if (trimInterval != null)
		{
			num = trimInterval.WeeklyEnterLimit;
		}
		int num2 = GetRemainEnterCount(trimInterval);
		if (num2 < 0)
		{
			Log.Debug("\ufffd\ufffd\ufffd\ufffd Ƚ\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdѾ\uec23 \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Trim/NKCUITrimUtility.cs", 152);
			num2 = 0;
		}
		string value = string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_WEEK_02, num2, num);
		StringBuilder stringBuilder = new StringBuilder();
		if (num2 > 0)
		{
			stringBuilder.Append("<color=#ffffffff>");
		}
		else
		{
			stringBuilder.Append("<color=#ff0000ff>");
		}
		stringBuilder.Append(value);
		stringBuilder.Append("</color>");
		return stringBuilder.ToString();
	}

	public static DateTime GetRemainDateMsg()
	{
		NKC_SCEN_TRIM nKC_SCEN_TRIM = NKCScenManager.GetScenManager()?.Get_NKC_SCEN_TRIM();
		if (nKC_SCEN_TRIM != null)
		{
			NKMTrimIntervalTemplet nKMTrimIntervalTemplet = NKMTrimIntervalTemplet.Find(nKC_SCEN_TRIM.TrimIntervalId);
			if (nKMTrimIntervalTemplet != null)
			{
				return NKCSynchronizedTime.ToUtcTime(nKMTrimIntervalTemplet.IntervalTemplet.EndDate);
			}
		}
		return DateTime.MinValue;
	}

	public static int GetTrimLevelScore(int trimId, int trimLevel)
	{
		return (NKCScenManager.CurrentUserData()?.TrimData?.TrimClearList?.Find((NKMTrimClearData e) => e.trimId == trimId && e.trimLevel == trimLevel))?.score ?? 0;
	}

	public static int GetRecommendedPower(int trimGroup, int trimLevel)
	{
		return NKMTrimPointTemplet.Find(trimGroup, trimLevel)?.RecommendCombatPoint ?? 0;
	}

	public static bool IsEnterCountRemaining(NKMTrimIntervalTemplet trimInterval)
	{
		return GetRemainEnterCount(trimInterval) > 0;
	}

	public static bool IsRestoreEnterCountEnable(NKMTrimIntervalTemplet trimInterval, NKMUserData userData)
	{
		int num = trimInterval?.RestoreLimitCount ?? 0;
		int num2 = userData?.TrimData.TrimIntervalData.trimRestoreCount ?? 0;
		return num > num2;
	}

	public static int GetRemainRestoreCount(NKMTrimIntervalTemplet trimInterval, NKMUserData userData)
	{
		int num = trimInterval?.RestoreLimitCount ?? 0;
		if (userData != null)
		{
			num -= userData.TrimData.TrimIntervalData.trimRestoreCount;
		}
		return num;
	}

	public static int GetRestoreLimitCount(NKMTrimIntervalTemplet trimInterval)
	{
		return trimInterval?.RestoreLimitCount ?? 0;
	}

	public static int GetRestoreItemReqId(NKMTrimIntervalTemplet trimInterval)
	{
		return trimInterval?.RestoreLimitReqItemId ?? 0;
	}

	public static int GetRestoreItemReqCount(NKMTrimIntervalTemplet trimInterval, NKMUserData userData)
	{
		if (trimInterval == null || userData == null)
		{
			return 0;
		}
		int trimRestoreCount = userData.TrimData.TrimIntervalData.trimRestoreCount;
		if (trimInterval.RestoreLimitReqItemCount.Length <= trimRestoreCount)
		{
			Log.Debug("\ufffd\ufffd\ufffd\ufffd Ƚ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd Ƚ\ufffd\ufffd \ufffdѰ\ufffd \ufffdʰ\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Trim/NKCUITrimUtility.cs", 247);
			return 0;
		}
		return trimInterval.RestoreLimitReqItemCount[trimRestoreCount];
	}

	public static int GetClearedTrimLevel(NKMUserData userData, int trimId)
	{
		int result = 0;
		if (userData != null)
		{
			result = userData.TrimData.GetClearedTrimLevel(trimId);
		}
		return result;
	}

	public static bool IsEnterCountLimited(NKMTrimIntervalTemplet trimInterval)
	{
		if (trimInterval != null)
		{
			return !trimInterval.IsWeeklyUnLimit;
		}
		return false;
	}

	public static bool IsRestoreEnterCountLimited(NKMTrimIntervalTemplet trimInterval)
	{
		if (trimInterval != null)
		{
			return !trimInterval.IsRestoreUnLimit;
		}
		return false;
	}

	public static bool IsUnlockInfoActive(UnlockInfo unlockInfo)
	{
		if (unlockInfo.reqValue == 0)
		{
			return unlockInfo.eReqType != STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE;
		}
		return true;
	}

	public static bool HaveEventDrop()
	{
		NKMTrimIntervalTemplet nKMTrimIntervalTemplet = NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime);
		if (nKMTrimIntervalTemplet == null)
		{
			return false;
		}
		int[] trimSlot = nKMTrimIntervalTemplet.TrimSlot;
		int num = trimSlot.Length;
		for (int i = 0; i < num; i++)
		{
			if (HaveEventDrop(trimSlot[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool HaveEventDrop(int trimId)
	{
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(trimId);
		if (nKMTrimTemplet == null)
		{
			return false;
		}
		for (int i = 1; i <= nKMTrimTemplet.MaxTrimLevel; i++)
		{
			if (HaveEventDrop(trimId, i))
			{
				return true;
			}
		}
		return false;
	}

	public static bool HaveEventDrop(int trimId, int trimLevel)
	{
		NKCTrimRewardTemplet nKCTrimRewardTemplet = NKCTrimRewardTemplet.Find(trimId, trimLevel);
		if (nKCTrimRewardTemplet == null)
		{
			return false;
		}
		bool result = false;
		int count = nKCTrimRewardTemplet.EventDropIndex.Count;
		for (int i = 0; i < count; i++)
		{
			NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(nKCTrimRewardTemplet.EventDropIndex[i]);
			if (rewardGroup == null)
			{
				continue;
			}
			int count2 = rewardGroup.List.Count;
			for (int j = 0; j < count2; j++)
			{
				if (rewardGroup.List[j].intervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime))
				{
					result = true;
				}
			}
		}
		return result;
	}
}
