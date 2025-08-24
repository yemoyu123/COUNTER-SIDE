using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKC;
using NKC.UI;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public static class NKMMissionManager
{
	public enum MissionState
	{
		CAN_COMPLETE,
		REPEAT_CAN_COMPLETE,
		ONGOING,
		REPEAT_COMPLETED,
		LOCKED,
		COMPLETED
	}

	public struct MissionStateData
	{
		public MissionState state;

		public long progressCount;

		public bool IsMissionOngoing
		{
			get
			{
				MissionState missionState = state;
				if ((uint)missionState <= 2u)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsMissionCompleted
		{
			get
			{
				MissionState missionState = state;
				if (missionState == MissionState.REPEAT_COMPLETED || missionState == MissionState.COMPLETED)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsMissionCanClear
		{
			get
			{
				MissionState missionState = state;
				if ((uint)missionState <= 1u)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsLocked => state == MissionState.LOCKED;

		public MissionStateData(MissionState missionState)
		{
			state = missionState;
			progressCount = 0L;
		}

		public MissionStateData(MissionState missionState, long count)
		{
			state = missionState;
			progressCount = count;
		}
	}

	public static Dictionary<int, NKMMissionTemplet> DicMissionId = null;

	public static Dictionary<int, NKMMissionTabTemplet> DicMissionTab = null;

	public static Dictionary<int, DateTime> DicCounterGroupResetDate = null;

	public static Dictionary<int, List<int>> StageClearToMissionMap = new Dictionary<int, List<int>>();

	public static Dictionary<int, List<int>> DicPoolList = new Dictionary<int, List<int>>();

	public static Dictionary<int, HashSet<int>> DicGroupListByTab = new Dictionary<int, HashSet<int>>();

	public static int GuildTabId = 0;

	private static NKMMissionTemplet m_TrackingMissionTemplet = null;

	private static bool m_bHaveCompletedMission = false;

	private static bool m_bHaveCompletedMissionGuide = false;

	public static bool LoadFromLUA(string filename, string tabFileName)
	{
		DicMissionTab = NKMTempletLoader.LoadDictionary("AB_SCRIPT", tabFileName, "MissionTabTemplet", NKMMissionTabTemplet.LoadFromLUA);
		if (DicMissionTab == null)
		{
			return false;
		}
		DicMissionId = NKMTempletLoader.LoadDictionary("AB_SCRIPT", filename, "m_dicNKMMissionTempletByID", NKMMissionTemplet.LoadFromLUA);
		if (DicMissionId == null)
		{
			return false;
		}
		foreach (NKMMissionTemplet value in DicMissionId.Values)
		{
			MakeDicPoolList(value);
			MakeDicMissionTab(value);
		}
		foreach (NKMMissionTabTemplet value2 in DicMissionTab.Values)
		{
			value2.Join();
		}
		if (DicMissionTab.Values.Any((NKMMissionTabTemplet e) => e.m_MissionType == NKM_MISSION_TYPE.GUILD))
		{
			GuildTabId = DicMissionTab.Values.First((NKMMissionTabTemplet e) => e.m_MissionType == NKM_MISSION_TYPE.GUILD).m_tabID;
		}
		return true;
	}

	public static NKMMissionTemplet GetMissionTemplet(int mission_id)
	{
		if (DicMissionId == null)
		{
			return null;
		}
		if (!DicMissionId.TryGetValue(mission_id, out var value))
		{
			return null;
		}
		return value;
	}

	public static List<NKMMissionTemplet> GetMissionTempletListByType(int missionTabId)
	{
		List<NKMMissionTemplet> list = new List<NKMMissionTemplet>();
		foreach (KeyValuePair<int, NKMMissionTemplet> item in DicMissionId)
		{
			NKMMissionTemplet value = item.Value;
			if (value != null && value.EnableByTag && value.EnableByInterval && value.m_MissionTabId == missionTabId)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static List<NKMMissionTemplet> GetMissionTempletListByGroupID(int counterGroupId)
	{
		List<NKMMissionTemplet> list = new List<NKMMissionTemplet>();
		foreach (NKMMissionTemplet value in DicMissionId.Values)
		{
			if (value != null && value.m_GroupId == counterGroupId)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static List<NKMMissionTemplet> GetMissionTempletListByTabID(int missionTabId)
	{
		List<NKMMissionTemplet> list = new List<NKMMissionTemplet>();
		foreach (NKMMissionTemplet value in DicMissionId.Values)
		{
			if (value != null && value.m_MissionTabId == missionTabId)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static bool ContainsCounterGroupResetData(int counterGroupId)
	{
		if (DicCounterGroupResetDate == null)
		{
			return false;
		}
		return DicCounterGroupResetDate.ContainsKey(counterGroupId);
	}

	public static void AddCounterGroupResetData(int counterGroupId, NKMMissionTemplet missionTemplet)
	{
		if (DicCounterGroupResetDate == null)
		{
			DicCounterGroupResetDate = new Dictionary<int, DateTime>();
		}
		if (DicCounterGroupResetDate.TryGetValue(counterGroupId, out var value))
		{
			if (value < missionTemplet.m_TabTemplet.m_endTime)
			{
				DicCounterGroupResetDate[counterGroupId] = missionTemplet.m_TabTemplet.m_endTime;
			}
		}
		else
		{
			DicCounterGroupResetDate.Add(counterGroupId, missionTemplet.m_TabTemplet.m_endTime);
		}
	}

	public static bool IsAccumulateType(NKMMissionTemplet missionTemplet)
	{
		return false;
	}

	public static void CheckValidation()
	{
		MissionGroupIdValidator missionGroupIdValidator = new MissionGroupIdValidator();
		foreach (NKMMissionTemplet value in DicMissionId.Values)
		{
			if (value.m_Times < 0)
			{
				Log.ErrorAndExit($"[NKMMissionManager] Invalid m_Times value. m_Times:{value.m_Times} < 0", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1098);
			}
			if (value.m_MissionCond.mission_cond == NKM_MISSION_COND.NONE)
			{
				Log.ErrorAndExit($"Invalid Mission CondData. MissionId:{value.m_MissionID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1103);
				return;
			}
			value.Join();
			missionGroupIdValidator.Add(value);
			foreach (MissionReward item in value.m_MissionReward)
			{
				if (item.reward_type != NKM_REWARD_TYPE.RT_NONE && item.reward_id > 0)
				{
					if (!NKMRewardTemplet.IsValidReward(item.reward_type, item.reward_id) || item.reward_value <= 0)
					{
						Log.ErrorAndExit($"[MissionTemplet] \ufffd\u033c\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_MissionID : {value.m_MissionID}, reward_type : {item.reward_type}, reward_id : {item.reward_id}, reward_value : {item.reward_value}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1118);
					}
					NKMMissionTabTemplet missionTabTemplet = GetMissionTabTemplet(value.m_MissionTabId);
					if (missionTabTemplet == null)
					{
						Log.ErrorAndExit($"[MissionTemplet] \ufffd\ufffd \ufffd\ufffd\ufffdø\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. missionId:{value.m_MissionID} tabId:{value.m_MissionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1124);
						return;
					}
					if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.BINGO && item.reward_type != NKM_REWARD_TYPE.RT_BINGO_TILE)
					{
						Log.ErrorAndExit($"[MissionTemplet] \ufffd\u033c\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd Ÿ\ufffdϸ\ufffd \ufffdԷ\ufffd \ufffd\ufffd\ufffd\ufffd m_MissionID : {value.m_MissionID}, reward_type : {item.reward_type}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1132);
					}
				}
			}
			if (value.m_MissionRequire > 0 && !DicMissionId.ContainsKey(value.m_MissionRequire))
			{
				Log.ErrorAndExit($"[MissionTemplet] \ufffd\ufffd\ufffd\ufffd Ŭ\ufffd\ufffd\ufffd\ufffd \ufffd\u033c\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_MissionID : {value.m_MissionID}, m_MissionRequire : {value.m_MissionRequire}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1142);
			}
			switch (value.m_MissionCond.mission_cond)
			{
			case NKM_MISSION_COND.USE_RESOURCE:
			case NKM_MISSION_COND.COLLECT_RESOURCE:
				if (value.m_MissionCond.value1.Count <= 0)
				{
					Log.ErrorAndExit($"[MissionTemplet] USE_RESOURCE, COLLECT_RESOURCE\ufffd\ufffd values ī\ufffd\ufffdƮ\ufffd\ufffd 0 \ufffd\ufffd\ufffdϸ\ufffd \ufffdȵ\ufffd. missionId:{value.m_MissionID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1152);
				}
				break;
			case NKM_MISSION_COND.USE_ETERNIUM:
				if (value.m_Times <= 0)
				{
					Log.ErrorAndExit($"[MissionTemplet] USE_ETERNIUM, times \ufffd\ufffd\ufffd\ufffd 0 \ufffd\ufffd\ufffdϸ\ufffd \ufffdȵ\ufffd. missionId:{value.m_MissionID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1158);
				}
				if (value.m_MissionCond.value1.Count > 0)
				{
					Log.ErrorAndExit($"[MissionTemplet] USE_ETERNIUM, values\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdȵ\ufffd. missionId:{value.m_MissionID} valuesCount:{value.m_MissionCond.value1.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1163);
				}
				break;
			case NKM_MISSION_COND.COLLECT_EQUIP_ENCHANT_LEVEL:
				if (value.m_MissionCond.value1.Count != 1)
				{
					Log.ErrorAndExit($"[MissionTemplet] COLLECT_EQUIP_ENCHANT_LEVEL\ufffd\ufffd values \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd 1\ufffd\u033e\ufffd\ufffd \ufffd\ufffd. missionId:{value.m_MissionID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1169);
				}
				break;
			case NKM_MISSION_COND.ACHIEVEMENT_CLEARED:
				if (value.m_MissionCond.value1.Count > 0 && value.m_Times > value.m_MissionCond.value1.Count)
				{
					Log.ErrorAndExit($"[NKMMissionManager]ACHIEVEMENT_CLEARED \ufffd\u033cǿ\ufffd value\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\u05b4µ\ufffd, times \ufffd\ufffd\ufffd\ufffd mission_value\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd Ŭ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. missionId:{value.m_MissionID} m_Times:{value.m_Times} valueCount:{value.m_MissionCond.value1.Count} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1178);
				}
				break;
			case NKM_MISSION_COND.DONATE_MISSION_ITEM:
			{
				if (value.m_MissionCond.value1.Count != 1)
				{
					Log.ErrorAndExit($"[MissionTemplet] DONATE_MISSION_ITEM, values ī\ufffd\ufffdƮ\ufffd\ufffd 1\ufffd\ufffd \ufffd\u033e\ufffd\ufffd\ufffd\ufffd. mission id: {value.m_MissionID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1186);
					return;
				}
				if (value.m_Times <= 0)
				{
					Log.ErrorAndExit($"[MissionTemplet] DONATE_MISSION_ITEM, m_Times\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd 0\ufffd\ufffd\ufffd\ufffd \ufffd۰ų\ufffd \ufffd\ufffd\ufffd\ufffd\ufffdϴ\ufffd. mission id: {value.m_MissionID}, value: {value.m_Times}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1192);
					return;
				}
				int num = value.m_MissionCond.value1[0];
				if (NKMItemManager.GetItemMiscTempletByID(num) == null)
				{
					Log.ErrorAndExit($"[MissionTemplet] ITEM_MISC_ACCEPT, values 0\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd misc item\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffdϴ\ufffd. mission id: {value.m_MissionID}, misc item id: {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1199);
					return;
				}
				break;
			}
			case NKM_MISSION_COND.LOGIN_TIMES:
				if (value.m_MissionCond.value1.Count == 0 || value.m_MissionCond.value2 == 0 || value.m_Times != 1 || value.m_ResetInterval != NKM_MISSION_RESET_INTERVAL.DAILY)
				{
					Log.ErrorAndExit($"LOGIN_TIMES \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdԷ\ufffd \ufffd\ufffd\ufffd\ufffd. {value.m_MissionID},{value.m_MissionCond.value1.Count},{value.m_MissionCond.value2},{value.m_Times},{value.m_ResetInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1212);
					return;
				}
				if (value.m_MissionCond.value1[0] > value.m_MissionCond.value2)
				{
					Log.ErrorAndExit($"LOGIN_TIMES \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdԷ\ufffd \ufffd\ufffd\ufffd\ufffd. \ufffd\ufffd\ufffd۽ð\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffdð\ufffd\ufffd\ufffd\ufffd\ufffd ŭ {value.m_MissionID},{value.m_MissionCond.value1.Count},{value.m_MissionCond.value2},{value.m_Times},{value.m_ResetInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1218);
					return;
				}
				if (value.m_MissionCond.value1[0] < 0 || 24 < value.m_MissionCond.value1[0])
				{
					Log.ErrorAndExit($"LOGIN_TIMES \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdԷ\ufffd \ufffd\ufffd\ufffd\ufffd. \ufffdð\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd߸\ufffd\ufffd\ufffd {value.m_MissionID},{value.m_MissionCond.value1.Count},{value.m_MissionCond.value2},{value.m_Times},{value.m_ResetInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1224);
					return;
				}
				break;
			case NKM_MISSION_COND.PVP_NPC_CLEAR:
			case NKM_MISSION_COND.PVP_NPC_PLAY:
				if (value.m_MissionCond.value1.Count > 1)
				{
					NKMTempletError.Add(string.Format("[NKMMissionManager] NPC PVP \ufffd\u033cǿ\ufffd Index \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd 1\ufffd\ufffd \ufffd\u033b\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd. missionId:{0} cond:{1} valueData:{2}", value.m_MissionID, value.m_MissionCond.mission_cond, string.Join(",", value.m_MissionCond.value1)), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1234);
				}
				break;
			}
		}
		foreach (NKMMissionTemplet value2 in DicMissionId.Values)
		{
			value2.Validate();
		}
		missionGroupIdValidator.Validate();
		foreach (NKMMissionTabTemplet value3 in DicMissionTab.Values)
		{
			foreach (UnlockInfo item2 in value3.m_UnlockInfo)
			{
				if (!NKMContentUnlockManager.IsValidMissionUnlockType(item2))
				{
					Log.ErrorAndExit($"[MissionTabTemplet] \ufffd\ufffdȿ\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd  m_MissionTab : {value3.m_MissionType}, reqType : {item2.eReqType}, reqValue : {item2.reqValue}, reqStr : {item2.reqValueStr}, reqDateTime : {item2.reqDateTime}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1257);
				}
				if (item2.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_REGISTER_DATE && item2.reqDateTime == DateTime.MinValue)
				{
					Log.ErrorAndExit($"[NKMMissionManager] Invalid Data. m_UnlockReqType must have m_UnlockReqValueStr value. m_tabId:{value3.m_tabID} m_UnlockInfo.eReqType:{item2.eReqType}, m_UnlockReqValueStr:{item2.reqDateTime}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1262);
				}
			}
			if (value3.m_tabID == 0)
			{
				Log.ErrorAndExit("[MissionTabTemplet] Tab Templet Id \ufffd\ufffd 0 \ufffdԴϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1268);
			}
			if ((value3.m_startTime.Ticks > 0 || value3.m_endTime.Ticks > 0) && value3.m_startTime.Ticks > value3.m_endTime.Ticks)
			{
				Log.ErrorAndExit($"[MissionTabTemplet] \ufffd\ufffd\ufffd۽ð\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffdð\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd  m_MissionTab : {value3.m_MissionType}, m_startTime : {value3.m_startTime}, m_endTime : {value3.m_endTime}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1274);
			}
		}
		if (DicMissionTab.Values.Where((NKMMissionTabTemplet e) => e.m_MissionType == NKM_MISSION_TYPE.GUILD).Count() > 1)
		{
			Log.ErrorAndExit("[NKMMissionManager]\ufffd\ufffd\ufffd \ufffd\u033c\ufffd \ufffd\ufffd\ufffd\ufffd 1\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdϿ\ufffd\ufffd\ufffd \ufffd\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1280);
		}
		foreach (NKMMissionTabTemplet value4 in DicMissionTab.Values)
		{
			if (value4.m_MissionPoolID > 0)
			{
				if (!DicPoolList.ContainsKey(value4.m_MissionPoolID))
				{
					Log.ErrorAndExit($"MissionPool is not exist - m_MissionPoolID : {value4.m_MissionPoolID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1289);
				}
				if (value4.m_MissionDisplayCount > DicPoolList[value4.m_MissionPoolID].Count)
				{
					Log.ErrorAndExit($"MissionPoolSize is less than DisplayCount - m_MissionPoolID : {value4.m_MissionPoolID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1294);
				}
			}
		}
	}

	public static NKMMissionTabTemplet GetMissionTabTemplet(int tabID)
	{
		NKMMissionTabTemplet value = null;
		if (!DicMissionTab.TryGetValue(tabID, out value))
		{
			return null;
		}
		return value;
	}

	public static NKMMissionTabTemplet GetMissionTabTemplet(NKM_MISSION_TYPE missionType)
	{
		NKMMissionTabTemplet result = null;
		foreach (KeyValuePair<int, NKMMissionTabTemplet> item in DicMissionTab)
		{
			if (item.Value.m_MissionType == missionType)
			{
				return item.Value;
			}
		}
		return result;
	}

	public static bool IsCumulativeCondition(NKM_MISSION_COND cond)
	{
		switch (cond)
		{
		case NKM_MISSION_COND.ACCOUNT_LEVEL:
		case NKM_MISSION_COND.COLLECT_UNIT_LEVEL:
		case NKM_MISSION_COND.COLLECT_EQUIP_ENCHANT_LEVEL:
		case NKM_MISSION_COND.COLLECT_SHIP_LEVEL:
		case NKM_MISSION_COND.WARFARE_CLEARED:
		case NKM_MISSION_COND.DUNGEON_CLEARED:
		case NKM_MISSION_COND.PHASE_CLEAR:
		case NKM_MISSION_COND.WORLDMAP_BRANCH_NUMBER:
		case NKM_MISSION_COND.JUST_OPEN:
		case NKM_MISSION_COND.COLLECT_SHIP_GET:
		case NKM_MISSION_COND.COLLECT_SHIP_GET_LEVEL:
		case NKM_MISSION_COND.COLLECT_SHIP_UPGRADE:
		case NKM_MISSION_COND.COUNTER_CASE_OPENED:
		case NKM_MISSION_COND.PVP_HIGHEST_TIER_ASYNC:
		case NKM_MISSION_COND.ACHIEVEMENT_CLEARED:
		case NKM_MISSION_COND.DIVE_HIGHEST_CLEARED:
		case NKM_MISSION_COND.PVP_HIGHEST_TIER_CLEARED:
		case NKM_MISSION_COND.MISSION_EVENT_TAB_CLEAR:
		case NKM_MISSION_COND.MENTORING_MISSION_CLEARED:
		case NKM_MISSION_COND.UNIT_GROWTH_GET:
		case NKM_MISSION_COND.UNIT_POWER_HIGHEST:
		case NKM_MISSION_COND.DUNGEON_SQUAD_UNIT_POWER_HIGHEST:
		case NKM_MISSION_COND.WARFARE_SQUAD_UNIT_POWER_HIGHEST:
		case NKM_MISSION_COND.PVP_TOTAL_CLEAR_BOTH:
		case NKM_MISSION_COND.PVP_TOTAL_CLEAR_ASYNC:
		case NKM_MISSION_COND.PVP_TOTAL_CLEAR_RANK:
		case NKM_MISSION_COND.COLLECT_UNIT_TACTICS_LEVEL:
		case NKM_MISSION_COND.MISSION_CLEAR:
		case NKM_MISSION_COND.GET_SKIN:
		case NKM_MISSION_COND.PALACE_CLEARED:
		case NKM_MISSION_COND.UNLOCKED_UNIT_REACTOR:
		case NKM_MISSION_COND.COLLECT_OPR_LEVEL:
			return false;
		case NKM_MISSION_COND.LOGIN_DAYS:
		case NKM_MISSION_COND.USE_RESOURCE:
		case NKM_MISSION_COND.COLLECT_RESOURCE:
		case NKM_MISSION_COND.COLLECT_UNIT:
		case NKM_MISSION_COND.COLLECT_UNIT_GRADE:
		case NKM_MISSION_COND.COLLECT_EQUIP:
		case NKM_MISSION_COND.COLLECT_EQUIP_GRADE:
		case NKM_MISSION_COND.COLLECT_SHIP:
		case NKM_MISSION_COND.COLLECT_SHIP_GRADE:
		case NKM_MISSION_COND.COLLECT_MEDAL_GOLD_MAINSTREAM:
		case NKM_MISSION_COND.COLLECT_MEDAL_SILVER_MAINSTREAM:
		case NKM_MISSION_COND.COLLECT_MEDAL_BRONZE_MAINSTREAM:
		case NKM_MISSION_COND.UNIT_CONTRACT:
		case NKM_MISSION_COND.UNIT_DISMISS:
		case NKM_MISSION_COND.UNIT_ENCHANT:
		case NKM_MISSION_COND.UNIT_TRAINING:
		case NKM_MISSION_COND.UNIT_LIMITBREAK:
		case NKM_MISSION_COND.UNIT_LIMITBREAK_CONFLUENCE:
		case NKM_MISSION_COND.EQUIP_ENCHANT:
		case NKM_MISSION_COND.SHIP_MAKE:
		case NKM_MISSION_COND.SHIP_UPGRADE:
		case NKM_MISSION_COND.SHIP_LEVELUP:
		case NKM_MISSION_COND.SHIP_LIMITBREAK:
		case NKM_MISSION_COND.WARFARE_CLEAR:
		case NKM_MISSION_COND.WARFARE_DEFEATED:
		case NKM_MISSION_COND.DUNGEON_CLEAR:
		case NKM_MISSION_COND.DUNGEON_DEFEATED:
		case NKM_MISSION_COND.DAILY_DUNGEON_PLAY:
		case NKM_MISSION_COND.DIVE_CLEAR:
		case NKM_MISSION_COND.DIVE_PLAY_RECORD:
		case NKM_MISSION_COND.RAID_FIND:
		case NKM_MISSION_COND.RAID_PLAY:
		case NKM_MISSION_COND.RAID_HIGH_SCORE:
		case NKM_MISSION_COND.PVP_PLAY_RANK:
		case NKM_MISSION_COND.PVP_CLEAR_RANK:
		case NKM_MISSION_COND.PVP_CLEAR_FRIENDLY:
		case NKM_MISSION_COND.PVP_DEFEATED_RANK:
		case NKM_MISSION_COND.PVP_DEFEATED_FRIENDLY:
		case NKM_MISSION_COND.PVP_HIGHEST_TIER:
		case NKM_MISSION_COND.WORLDMAP_BRANCH_TOTAL_LEVEL:
		case NKM_MISSION_COND.WORLDMAP_MISSION_TOTAL_LEVEL:
		case NKM_MISSION_COND.WORLDMAP_MISSION_TOTAL_TIME:
		case NKM_MISSION_COND.WORLDMAP_MISSION_CLEAR:
		case NKM_MISSION_COND.SHOP_BUY:
		case NKM_MISSION_COND.HAVE_FRIEND:
		case NKM_MISSION_COND.TUTORIAL:
		case NKM_MISSION_COND.MISSION_CLEAR_DAILY:
		case NKM_MISSION_COND.MISSION_CLEAR_WEEKLY:
		case NKM_MISSION_COND.MISSION_CLEAR_MONTHLY:
		case NKM_MISSION_COND.COUNTER_CASE_OPEN:
		case NKM_MISSION_COND.NEGOTIATION_TRY:
		case NKM_MISSION_COND.NEGOTIATION_SUCCESS:
		case NKM_MISSION_COND.NEGOTIATION_FAIL:
		case NKM_MISSION_COND.EQUIP_MAKE:
		case NKM_MISSION_COND.EQUIP_TUNING:
		case NKM_MISSION_COND.ACHIEVEMENT_CLEAR:
		case NKM_MISSION_COND.HAVE_DAILY_POINT:
		case NKM_MISSION_COND.HAVE_WEEKLY_POINT:
		case NKM_MISSION_COND.PVP_PLAY_ASYNC:
		case NKM_MISSION_COND.PVP_CLEAR_ASYNC:
		case NKM_MISSION_COND.PVP_DEFEATED_ASYNC:
		case NKM_MISSION_COND.EPISODE_PLAY_COUNT:
		case NKM_MISSION_COND.EPISODE_PLAY_COUNT_HARD:
		case NKM_MISSION_COND.DUNGEON_CLEAR_PERFECT:
		case NKM_MISSION_COND.DUNGEON_CLEARED_PERFECT:
		case NKM_MISSION_COND.WARFARE_CLEAR_PERFECT:
		case NKM_MISSION_COND.WARFARE_CLEARED_PERFECT:
		case NKM_MISSION_COND.PHASE_CLEAR_PERFECT:
		case NKM_MISSION_COND.PVP_LEAGUE_POINT_RANK:
		case NKM_MISSION_COND.PVP_LEAGUE_POINT_ASYNC:
		case NKM_MISSION_COND.UNIT_LEVEL_CHECK:
		case NKM_MISSION_COND.SHIP_LEVEL_CHECK:
		case NKM_MISSION_COND.PALACE_CLEAR:
		case NKM_MISSION_COND.SUPPORT_PLATOON_USED:
		case NKM_MISSION_COND.GUILD_DONATE:
		case NKM_MISSION_COND.GUILD_ATTENDANCE:
		case NKM_MISSION_COND.FIERCE_RANK_TOP:
		case NKM_MISSION_COND.EC_SUPPLY_CLEAR:
		case NKM_MISSION_COND.DONATE_MISSION_ITEM:
		case NKM_MISSION_COND.UNIT_GROWTH_LEVEL:
		case NKM_MISSION_COND.UNIT_GROWTH_LIMIT:
		case NKM_MISSION_COND.UNIT_GROWTH_SKILL_LEVEL_3:
		case NKM_MISSION_COND.UNIT_GROWTH_SKILL_LEVEL_MAX:
		case NKM_MISSION_COND.UNIT_GROWTH_LOYALTY:
		case NKM_MISSION_COND.UNIT_GROWTH_PERMANENT:
		case NKM_MISSION_COND.UNIT_GROWTH_TACTICAL:
		case NKM_MISSION_COND.UNIT_USE_CLEAR_DUNGEON:
		case NKM_MISSION_COND.UNIT_USE_CLEAR_DAILY:
		case NKM_MISSION_COND.UNIT_USE_CLEAR_SUPPLY:
		case NKM_MISSION_COND.UNIT_USE_CLEAR_PHASE:
		case NKM_MISSION_COND.UNIT_USE_PLAY_PVP_ASYNC:
		case NKM_MISSION_COND.UNIT_USE_PLAY_PVP_RANK:
		case NKM_MISSION_COND.UNIT_USE_GO:
		case NKM_MISSION_COND.UNIT_USE_TIMEATK_SUCCESS:
		case NKM_MISSION_COND.UNIT_USE_GO_SUCCESS:
		case NKM_MISSION_COND.UNIT_USE_LIFE_SUCCESS:
		case NKM_MISSION_COND.UNIT_USE_LOBBY:
		case NKM_MISSION_COND.UNIT_USE_WORLDMAP:
		case NKM_MISSION_COND.UNIT_USE_GO_UNIT_ID:
		case NKM_MISSION_COND.LOGIN_TIMES:
		case NKM_MISSION_COND.RAID_HELP_PUSH:
		case NKM_MISSION_COND.RAID_FIND_LEVEL_HIGH:
		case NKM_MISSION_COND.RAID_PLAY_LEVEL_HIGH:
		case NKM_MISSION_COND.RAID_REWARD_FRIEND:
		case NKM_MISSION_COND.RAID_STAGE_CLEARED:
		case NKM_MISSION_COND.RAID_CLEAR_MVP:
		case NKM_MISSION_COND.RAID_ASSIST_COUNT:
		case NKM_MISSION_COND.WORLDMAP_REWARD_SUCCESS:
		case NKM_MISSION_COND.COLLECT_EQUIP_TIER:
		case NKM_MISSION_COND.PVP_PLAY_LEAGUE:
		case NKM_MISSION_COND.COLLECT_ITEM_INTERIOR:
		case NKM_MISSION_COND.COLLECT_OFFICE_ROOM:
		case NKM_MISSION_COND.GET_OFFICE_HEART:
		case NKM_MISSION_COND.GIVE_NAME_CARD:
		case NKM_MISSION_COND.GIVE_NAME_CARD_ALL:
		case NKM_MISSION_COND.TRY_EXTRACT_UNIT:
		case NKM_MISSION_COND.USE_ETERNIUM:
		case NKM_MISSION_COND.PVP_NPC_CLEAR_ASYNC:
		case NKM_MISSION_COND.PVP_NPC_PLAY_ASYNC:
		case NKM_MISSION_COND.PVP_NPC_CLEAR:
		case NKM_MISSION_COND.PVP_NPC_PLAY:
		case NKM_MISSION_COND.EVENT_COLLECT_UNIT_COLLECT:
		case NKM_MISSION_COND.EVENT_COLLECT_UNIT_COUNT:
		case NKM_MISSION_COND.PVP_CLEAR_BOTH:
		case NKM_MISSION_COND.PVP_PLAY_BOTH:
		case NKM_MISSION_COND.MAKE_COUNT_MOLD:
		case NKM_MISSION_COND.PVP_ALLVICTORIES_RANK:
		case NKM_MISSION_COND.PVP_ALLVICTORIES_ASYNC:
		case NKM_MISSION_COND.RAID_STAGE_MVP_CLEARED:
		case NKM_MISSION_COND.TRIM_DUNGEON_CLEARED:
		case NKM_MISSION_COND.SHOP_BOUGHT:
		case NKM_MISSION_COND.PVP_RANK_TOP:
		case NKM_MISSION_COND.DEFENCE_RANK_TOP:
		case NKM_MISSION_COND.PVP_CLEAR_LEAGUE:
		case NKM_MISSION_COND.UNIT_USE_CLEAR_CHALLENGE:
			return true;
		default:
			throw new InvalidOperationException($"[IsCumulativeCondition] unspecified mission type, value: {cond}");
		}
	}

	public static bool IsUnitGrowthCondition(NKM_MISSION_COND cond)
	{
		if ((uint)(cond - 98) <= 7u)
		{
			return true;
		}
		return false;
	}

	public static bool IsAchiveMissionType(NKM_MISSION_TYPE missionType)
	{
		if (missionType != NKM_MISSION_TYPE.ACHIEVE && missionType != NKM_MISSION_TYPE.TROPHY)
		{
			return missionType == NKM_MISSION_TYPE.TITLE;
		}
		return true;
	}

	public static bool CanCompleteCumulative(NKMMissionTemplet templet, NKMMissionData missionData)
	{
		return missionData.times >= templet.m_Times;
	}

	private static void MakeDicPoolList(NKMMissionTemplet templet)
	{
		if (!DicPoolList.TryGetValue(templet.m_MissionPoolID, out var value))
		{
			value = new List<int>();
			DicPoolList.Add(templet.m_MissionPoolID, value);
		}
		value.Add(templet.m_MissionID);
	}

	private static void MakeDicMissionTab(NKMMissionTemplet templet)
	{
		if (!DicGroupListByTab.TryGetValue(templet.m_MissionTabId, out var value))
		{
			value = new HashSet<int>();
			DicGroupListByTab.Add(templet.m_MissionTabId, value);
		}
		if (!value.Contains(templet.m_GroupId))
		{
			value.Add(templet.m_GroupId);
		}
	}

	public static void PostJoin()
	{
		foreach (NKMMissionTabTemplet value in DicMissionTab.Values)
		{
			value.PostJoin();
		}
		foreach (NKMMissionTemplet value2 in DicMissionId.Values)
		{
			value2.PostJoin();
		}
	}

	public static NKMMissionTemplet GetNextMissionTemplet(NKMMissionData missionData)
	{
		foreach (NKMMissionTemplet value in DicMissionId.Values)
		{
			if (value.m_MissionRequire == missionData.mission_id)
			{
				return value;
			}
		}
		return null;
	}

	public static NKMMissionTabTemplet GetNextMissionTabTemplet(int missionTabID)
	{
		NKMMissionTabTemplet missionTabTemplet = GetMissionTabTemplet(missionTabID);
		if (missionTabTemplet != null)
		{
			foreach (NKMMissionTemplet value in DicMissionId.Values)
			{
				if (value.m_MissionRequire == missionTabTemplet.m_completeMissionID)
				{
					return GetMissionTabTemplet(value.m_MissionTabId);
				}
			}
		}
		return null;
	}

	public static bool CheckMissionTabUnlocked(int tabID, NKMUserData userData)
	{
		NKMMissionTabTemplet missionTabTemplet = GetMissionTabTemplet(tabID);
		if (missionTabTemplet == null)
		{
			return true;
		}
		if (!NKMContentUnlockManager.IsContentUnlocked(userData, in missionTabTemplet.m_UnlockInfo))
		{
			return false;
		}
		if (IsMissionTabExpired(missionTabTemplet, userData))
		{
			return false;
		}
		return true;
	}

	public static bool IsMissionTabExpired(int tabID, NKMUserData userData)
	{
		return IsMissionTabExpired(GetMissionTabTemplet(tabID), userData);
	}

	public static bool IsMissionTabExpired(NKMMissionTabTemplet tabTemplet, NKMUserData userData)
	{
		if (tabTemplet == null || userData == null)
		{
			return true;
		}
		if (tabTemplet.IsReturningMission)
		{
			if (!userData.IsReturnUser(tabTemplet.m_ReturningUserType))
			{
				return true;
			}
		}
		else if (tabTemplet.IsNewbieMission && !userData.IsNewbieUser(tabTemplet.m_NewbieDate))
		{
			return true;
		}
		if (tabTemplet.HasDateLimit)
		{
			DateTime utcCurrent = (tabTemplet.IsReturningMission ? userData.GetReturnStartDate(tabTemplet.m_ReturningUserType) : ((!tabTemplet.IsNewbieMission) ? NKCSynchronizedTime.GetServerUTCTime() : userData.m_NKMUserDateData.m_RegisterTime));
			if (!NKCSynchronizedTime.IsEventTime(utcCurrent, tabTemplet.intervalId, tabTemplet.m_startTimeUtc, tabTemplet.m_endTimeUtc))
			{
				return true;
			}
		}
		return false;
	}

	public static bool TryGetMissionTabExpireUtcTime(NKMMissionTabTemplet tabTemplet, NKMUserData userData, out DateTime endUtcTime)
	{
		if (userData == null)
		{
			endUtcTime = DateTime.MinValue;
			return false;
		}
		if (tabTemplet.HasDateLimit)
		{
			if (tabTemplet.IsNewbieMission)
			{
				endUtcTime = userData.GetNewbieEndDate(tabTemplet.m_NewbieDate);
				return true;
			}
			if (tabTemplet.IsReturningMission)
			{
				endUtcTime = userData.GetReturnEndDate(tabTemplet.m_ReturningUserType);
				return true;
			}
			endUtcTime = tabTemplet.m_endTimeUtc;
			return true;
		}
		endUtcTime = DateTime.MinValue;
		return false;
	}

	public static NKM_ERROR_CODE CanGetCompleteMissionReward(NKMMissionTemplet missionTemplet, NKMUserData userData, NKMMissionData missionData)
	{
		if (missionTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_INVALID_MISSION_ID;
		}
		if (missionTemplet.m_ResetInterval != NKM_MISSION_RESET_INTERVAL.ON_COMPLETE)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_INVALID_MISSION_TAB;
		}
		if (missionData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_NO_MISSION_DATA;
		}
		if (!missionTemplet.EnableByInterval)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_OUT_OF_DATE;
		}
		if (!missionTemplet.EnableByTag)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_OPEN_TAG_CLOSED;
		}
		if (!CheckMissionTabUnlocked(missionTemplet.m_MissionTabId, userData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_NOT_ENOUGH_MISSION_COND_VALUE;
		}
		long num = missionData.times - missionTemplet.m_MinRewardTimes;
		if (num <= 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_NOT_ENOUGH_MISSION_COND_VALUE;
		}
		if (missionTemplet.m_RewardTimes == 0L)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_NOT_ENOUGH_MISSION_COND_VALUE;
		}
		if (Convert.ToInt32(num / missionTemplet.m_RewardTimes) <= 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_NOT_ENOUGH_MISSION_COND_VALUE;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanComplete(NKMMissionTemplet templet, NKMUserData userData, NKMMissionData missionData)
	{
		if (templet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_INVALID_MISSION_ID;
		}
		if (missionData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_INVALID_MISSION_ID;
		}
		if (userData == null || userData.m_MissionData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_USER_DATA_NULL;
		}
		if (missionData.IsComplete)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_ALREADY_COMPLETED;
		}
		if (missionData.mission_id != templet.m_MissionID)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_INVALID_MISSION_ID;
		}
		if (templet.m_MissionRequire > 0)
		{
			NKMMissionTemplet missionTemplet = GetMissionTemplet(templet.m_MissionRequire);
			NKMMissionData missionDataByMissionId = userData.m_MissionData.GetMissionDataByMissionId(templet.m_MissionRequire);
			if (missionTemplet == null || missionDataByMissionId == null)
			{
				return NKM_ERROR_CODE.NEC_FAIL_MISSION_NOT_ENOUGH_MISSION_COND_VALUE;
			}
			if (templet.m_GroupId != missionTemplet.m_GroupId && !missionDataByMissionId.IsComplete)
			{
				return NKM_ERROR_CODE.NEC_FAIL_MISSION_NOT_ENOUGH_MISSION_COND_VALUE;
			}
		}
		if (!CheckMissionTabUnlocked(templet.m_MissionTabId, userData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_NOT_ENOUGH_MISSION_COND_VALUE;
		}
		if (IsCumulativeCondition(templet.m_MissionCond.mission_cond))
		{
			if (!CanCompleteCumulative(templet, missionData))
			{
				return NKM_ERROR_CODE.NEC_FAIL_MISSION_NOT_ENOUGH_MISSION_COND_VALUE;
			}
		}
		else if (!CanCompleteNonCumulative(templet, userData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_NOT_ENOUGH_MISSION_COND_VALUE;
		}
		if (CheckCanReset(templet.m_ResetInterval, missionData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_MISSION_INVALID_MISSION_ID;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static bool CheckCanReset(NKM_MISSION_RESET_INTERVAL resetInterval, NKMMissionData missionData)
	{
		if (missionData == null)
		{
			return resetInterval != NKM_MISSION_RESET_INTERVAL.NONE;
		}
		return resetInterval switch
		{
			NKM_MISSION_RESET_INTERVAL.NONE => false, 
			NKM_MISSION_RESET_INTERVAL.DAILY => missionData.last_update_date < NKMTime.GetResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Day).Ticks, 
			NKM_MISSION_RESET_INTERVAL.WEEKLY => missionData.last_update_date < NKMTime.GetResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Week).Ticks, 
			NKM_MISSION_RESET_INTERVAL.MONTHLY => missionData.last_update_date < NKMTime.GetResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Month).Ticks, 
			NKM_MISSION_RESET_INTERVAL.ALWAYS => true, 
			_ => false, 
		};
	}

	public static NKMMissionTemplet GetLastCompletedMissionTempletByTab(int missionTabID)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		List<NKMMissionTemplet> missionTempletListByType = GetMissionTempletListByType(missionTabID);
		for (int num = missionTempletListByType.Count - 1; num >= 0; num--)
		{
			NKMMissionData missionData = myUserData.m_MissionData.GetMissionData(missionTempletListByType[num]);
			if (missionData != null && missionData.IsComplete)
			{
				return missionTempletListByType[num];
			}
		}
		return null;
	}

	public static NKMMissionTemplet GetCurrentGrowthMissionTemplet()
	{
		NKMMissionTemplet nKMMissionTemplet = null;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return nKMMissionTemplet;
		}
		bool bCompleteAll = false;
		List<NKMMissionTabTemplet> list = new List<NKMMissionTabTemplet>();
		foreach (NKMMissionTabTemplet value in DicMissionTab.Values)
		{
			if (value.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION && !IsMissionTabExpired(value, nKMUserData) && value.EnableByTag)
			{
				list.Add(value);
			}
		}
		list.Sort(CompGrowthSort);
		for (int i = 0; i < list.Count; i++)
		{
			nKMMissionTemplet = GetGrowthMissionIngTempletByTab(list[i].m_tabID, out bCompleteAll);
			if (!bCompleteAll || nKMMissionTemplet != null)
			{
				break;
			}
		}
		if (bCompleteAll)
		{
			return null;
		}
		if (nKMMissionTemplet == null && list.Count > 0)
		{
			return GetMissionTemplet(list[0].m_firstMissionID);
		}
		return nKMMissionTemplet;
	}

	private static int CompGrowthSort(NKMMissionTabTemplet lTemplet, NKMMissionTabTemplet rTemplet)
	{
		if (lTemplet.m_OrderList.CompareTo(rTemplet.m_OrderList) == 0)
		{
			return lTemplet.m_tabID.CompareTo(rTemplet.m_tabID);
		}
		return lTemplet.m_OrderList.CompareTo(rTemplet.m_OrderList);
	}

	public static NKMMissionTemplet GetGrowthMissionIngTempletByTab(int missionTabID, out bool bCompleteAll)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bCompleteAll = false;
		if (nKMUserData == null)
		{
			return null;
		}
		NKMMissionTabTemplet missionTabTemplet = GetMissionTabTemplet(missionTabID);
		if (missionTabTemplet != null)
		{
			if (IsMissionTabExpired(missionTabTemplet, nKMUserData))
			{
				return null;
			}
			NKMMissionTemplet missionTemplet = GetMissionTemplet(missionTabTemplet.m_completeMissionID);
			if (missionTemplet != null)
			{
				if (nKMUserData.m_MissionData.GetMissionData(missionTemplet) != null)
				{
					if (nKMUserData.m_MissionData.GetMissionData(missionTemplet).IsComplete)
					{
						bCompleteAll = true;
						return null;
					}
					return missionTemplet;
				}
				NKMMissionData missionDataByMissionId = nKMUserData.m_MissionData.GetMissionDataByMissionId(missionTemplet.m_MissionRequire);
				if (missionDataByMissionId != null && missionDataByMissionId.IsComplete)
				{
					return missionTemplet;
				}
			}
			NKMMissionTemplet templet = GetLastCompletedMissionTempletByTab(missionTabID);
			if (templet != null)
			{
				return GetMissionTempletListByType(missionTabID).Find((NKMMissionTemplet x) => x.m_MissionRequire == templet.m_MissionID);
			}
			NKMMissionTemplet missionTemplet2 = GetMissionTemplet(missionTabTemplet.m_firstMissionID);
			if (missionTemplet2 != null && missionTemplet2.m_MissionRequire != 0 && nKMUserData.m_MissionData.GetMissionDataByMissionId(missionTemplet2.m_MissionRequire) != null)
			{
				return missionTemplet2;
			}
			return GetMissionTempletListByType(missionTabID).Find((NKMMissionTemplet x) => x.m_MissionRequire == 0);
		}
		return null;
	}

	public static void SetDefaultTrackingMissionToGrowthMission()
	{
		SetTrackingMissionTemplet(GetCurrentGrowthMissionTemplet());
	}

	public static void SetTrackingMissionTemplet(NKMMissionTemplet templet)
	{
		m_TrackingMissionTemplet = templet;
	}

	public static NKMMissionTemplet GetTrackingMissionTemplet()
	{
		return m_TrackingMissionTemplet;
	}

	public static NKMMissionTabTemplet GetFirstGrowthUnitMissionTabTemplet()
	{
		return null;
	}

	public static long GetRepeatMissionDataTimes(NKM_MISSION_TYPE tabType)
	{
		NKMMissionTemplet nKMMissionTemplet = null;
		NKM_MISSION_COND nKM_MISSION_COND = NKM_MISSION_COND.NONE;
		nKM_MISSION_COND = ((tabType != NKM_MISSION_TYPE.REPEAT_DAILY) ? NKM_MISSION_COND.HAVE_WEEKLY_POINT : NKM_MISSION_COND.HAVE_DAILY_POINT);
		foreach (NKMMissionTemplet value in DicMissionId.Values)
		{
			if (value.m_MissionCond.mission_cond == nKM_MISSION_COND)
			{
				nKMMissionTemplet = value;
				break;
			}
		}
		if (nKMMissionTemplet != null)
		{
			NKMMissionData missionData = NKCScenManager.CurrentUserData().m_MissionData.GetMissionData(nKMMissionTemplet);
			if (missionData != null)
			{
				if (!CheckCanReset(nKMMissionTemplet.m_ResetInterval, missionData))
				{
					return missionData.times;
				}
				return 0L;
			}
		}
		return 0L;
	}

	public static string GetMissionTabUnlockCondition(int tabID, NKMUserData userData)
	{
		NKMMissionTabTemplet missionTabTemplet = GetMissionTabTemplet(tabID);
		if (missionTabTemplet == null)
		{
			return "";
		}
		if (missionTabTemplet.IsNewbieMission && !userData.IsNewbieUser(missionTabTemplet.m_NewbieDate))
		{
			return NKCStringTable.GetString("SI_DP_MISSION_TAB_FINISHED");
		}
		if (missionTabTemplet.IsReturningMission && !userData.IsReturnUser(missionTabTemplet.m_ReturningUserType))
		{
			return NKCStringTable.GetString("SI_DP_MISSION_TAB_FINISHED");
		}
		if (missionTabTemplet.HasDateLimit)
		{
			DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
			if (serverUTCTime < missionTabTemplet.m_startTimeUtc)
			{
				TimeSpan timeSpan = missionTabTemplet.m_startTimeUtc - serverUTCTime;
				if (timeSpan.Days > 0)
				{
					return NKCStringTable.GetString("SI_DP_MISSION_OPEN_UNTIL_DAY", timeSpan.Days, timeSpan.Hours);
				}
				if (timeSpan.Hours <= 0)
				{
					return NKCStringTable.GetString("SI_DP_MISSION_OPEN_UNTIL_MINUTE", timeSpan.Minutes);
				}
				return NKCStringTable.GetString("SI_DP_MISSION_OPEN_UNTIL_HOUR", timeSpan.Hours, timeSpan.Minutes);
			}
			if (TryGetMissionTabExpireUtcTime(missionTabTemplet, userData, out var endUtcTime) && serverUTCTime > endUtcTime)
			{
				return NKCStringTable.GetString("SI_DP_MISSION_TAB_FINISHED");
			}
		}
		return NKCUtilString.GetUnlockConditionRequireDesc(missionTabTemplet.m_UnlockInfo, bLockedOnly: true);
	}

	public static bool IsMissionOpened(NKMMissionTemplet cNKMMissionTemplet)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (cNKMMissionTemplet == null || nKMUserData == null)
		{
			return false;
		}
		if (cNKMMissionTemplet.m_MissionRequire != 0 && !GetMissionStateData(cNKMMissionTemplet.m_MissionRequire).IsMissionCompleted)
		{
			return false;
		}
		return true;
	}

	public static bool IsGuideMissionAllClear()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		List<NKMMissionTabTemplet> list = new List<NKMMissionTabTemplet>();
		foreach (NKMMissionTabTemplet value in DicMissionTab.Values)
		{
			if (value.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION && !IsMissionTabExpired(value, nKMUserData))
			{
				list.Add(value);
			}
		}
		bool bCompleteAll = false;
		for (int i = 0; i < list.Count; i++)
		{
			NKMMissionTemplet growthMissionIngTempletByTab = GetGrowthMissionIngTempletByTab(list[i].m_tabID, out bCompleteAll);
			if (!bCompleteAll || growthMissionIngTempletByTab != null)
			{
				break;
			}
		}
		return bCompleteAll;
	}

	public static int GetGuideMissionClearCount()
	{
		int num = 0;
		if (NKCScenManager.CurrentUserData() == null)
		{
			return num;
		}
		foreach (NKMMissionTabTemplet value in DicMissionTab.Values)
		{
			if (value.m_MissionType != NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
			{
				continue;
			}
			foreach (NKMMissionTemplet item in GetMissionTempletListByType(value.m_tabID))
			{
				if (GetMissionStateData(item).IsMissionCanClear)
				{
					num++;
				}
			}
		}
		return num;
	}

	public static List<NKMMissionTemplet> GetGuildUserMissionTemplets()
	{
		List<NKMMissionTemplet> list = new List<NKMMissionTemplet>();
		NKMUserMissionData nKMUserMissionData = NKCScenManager.CurrentUserData()?.m_MissionData;
		if (nKMUserMissionData == null)
		{
			return list;
		}
		NKMMissionTabTemplet missionTabTemplet = GetMissionTabTemplet(NKM_MISSION_TYPE.GUILD);
		if (missionTabTemplet == null)
		{
			Log.Error($"tabTemplet is null - NKM_MISSION_TYPE : {NKM_MISSION_TYPE.GUILD}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMMissionManagerEx.cs", 730);
			return list;
		}
		List<NKMMissionTemplet> missionTempletListByType = GetMissionTempletListByType(missionTabTemplet.m_tabID);
		for (int i = 0; i < missionTempletListByType.Count; i++)
		{
			NKMMissionTemplet nKMMissionTemplet = missionTempletListByType[i];
			if (nKMMissionTemplet.IsRandomMission && nKMUserMissionData.GetMissionData(nKMMissionTemplet) == null)
			{
				continue;
			}
			if (nKMMissionTemplet.m_MissionRequire > 0)
			{
				if (nKMUserMissionData.WaitingForRandomMissionRefresh())
				{
					continue;
				}
				NKMMissionData missionDataByGroupId = nKMUserMissionData.GetMissionDataByGroupId(nKMMissionTemplet.m_GroupId);
				if (missionDataByGroupId == null)
				{
					continue;
				}
				if (missionDataByGroupId.mission_id == nKMMissionTemplet.m_MissionID)
				{
					list.Add(nKMMissionTemplet);
					continue;
				}
				missionDataByGroupId = nKMUserMissionData.GetMissionDataByMissionId(nKMMissionTemplet.m_MissionRequire);
				if (missionDataByGroupId == null)
				{
					continue;
				}
				if (missionDataByGroupId.IsComplete && missionDataByGroupId.mission_id == nKMMissionTemplet.m_MissionRequire)
				{
					list.Add(nKMMissionTemplet);
					continue;
				}
				if (missionDataByGroupId.mission_id <= nKMMissionTemplet.m_MissionRequire)
				{
					continue;
				}
			}
			list.Add(missionTempletListByType[i]);
		}
		list.Sort(Comparer);
		return list;
	}

	public static int Comparer(NKMMissionTemplet x, NKMMissionTemplet y)
	{
		MissionStateData missionStateData = GetMissionStateData(x);
		MissionStateData missionStateData2 = GetMissionStateData(y);
		if (missionStateData.state != missionStateData2.state)
		{
			return missionStateData.state.CompareTo(missionStateData2.state);
		}
		if (x.m_MissionPoolID != y.m_MissionPoolID)
		{
			return x.m_MissionPoolID.CompareTo(y.m_MissionPoolID);
		}
		return x.m_MissionID.CompareTo(y.m_MissionID);
	}

	public static MissionState GetMissionState(NKMMissionTemplet missionTemplet)
	{
		return GetMissionStateData(missionTemplet).state;
	}

	public static MissionStateData GetMissionStateData(int missionID)
	{
		return GetMissionStateData(GetMissionTemplet(missionID));
	}

	public static MissionStateData GetMissionStateData(NKMMissionTemplet missionTemplet)
	{
		if (missionTemplet == null)
		{
			return new MissionStateData(MissionState.LOCKED);
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return new MissionStateData(MissionState.LOCKED);
		}
		if (!CheckMissionTabUnlocked(missionTemplet.m_MissionTabId, nKMUserData))
		{
			return new MissionStateData(MissionState.LOCKED);
		}
		if (missionTemplet == null)
		{
			return new MissionStateData(MissionState.LOCKED);
		}
		NKMMissionData missionDataByGroupId = NKCScenManager.GetScenManager().GetMyUserData().m_MissionData.GetMissionDataByGroupId(missionTemplet.m_GroupId);
		bool flag;
		bool flag2;
		bool flag3;
		if (missionDataByGroupId != null)
		{
			flag = CheckCanReset(missionTemplet.m_ResetInterval, missionDataByGroupId);
			if (flag)
			{
				flag2 = false;
				flag3 = false;
			}
			else if (missionDataByGroupId.IsComplete)
			{
				flag3 = true;
				flag2 = false;
			}
			else if (missionDataByGroupId.mission_id > missionTemplet.m_MissionID)
			{
				flag3 = true;
				flag2 = false;
			}
			else if (missionDataByGroupId.mission_id == missionTemplet.m_MissionID)
			{
				flag3 = false;
				flag2 = true;
			}
			else
			{
				flag3 = false;
				flag2 = false;
			}
		}
		else
		{
			flag3 = false;
			flag2 = false;
			flag = false;
		}
		NKMMissionTabTemplet missionTabTemplet = GetMissionTabTemplet(missionTemplet.m_MissionTabId);
		if (missionTabTemplet != null && missionTabTemplet.m_MissionPoolID > 0 && NKCScenManager.CurrentUserData().m_MissionData.WaitingForRandomMissionRefresh())
		{
			flag = false;
		}
		long count = 0L;
		if (IsCumulativeCondition(missionTemplet.m_MissionCond.mission_cond) && !flag)
		{
			if (flag2)
			{
				count = missionDataByGroupId.times;
			}
			else if (flag3)
			{
				count = missionTemplet.m_Times;
			}
		}
		else
		{
			count = GetNonCumulativeMissionTimes(missionTemplet, NKCScenManager.GetScenManager().GetMyUserData(), bShowErrorlog: false);
		}
		if (flag2)
		{
			if (missionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.ON_COMPLETE)
			{
				if (CanGetCompleteMissionReward(missionTemplet, nKMUserData, missionDataByGroupId) == NKM_ERROR_CODE.NEC_OK)
				{
					return new MissionStateData(MissionState.REPEAT_CAN_COMPLETE, count);
				}
				return new MissionStateData(MissionState.ONGOING, count);
			}
			if (!flag && CanComplete(missionTemplet, nKMUserData, missionDataByGroupId) == NKM_ERROR_CODE.NEC_OK)
			{
				return new MissionStateData(MissionState.CAN_COMPLETE, count);
			}
			return new MissionStateData(MissionState.ONGOING, count);
		}
		if (flag3)
		{
			if (missionTemplet.m_ResetInterval != NKM_MISSION_RESET_INTERVAL.NONE)
			{
				return new MissionStateData(MissionState.REPEAT_COMPLETED, count);
			}
			return new MissionStateData(MissionState.COMPLETED, count);
		}
		if (IsMissionOpened(missionTemplet))
		{
			return new MissionStateData(MissionState.ONGOING, 0L);
		}
		return new MissionStateData(MissionState.LOCKED);
	}

	public static NKMMissionData GetMissionData(NKMMissionTemplet missionTemplet)
	{
		return NKCScenManager.CurrentUserData()?.m_MissionData.GetMissionData(missionTemplet);
	}

	public static bool CanCompleteNonCumulative(NKMMissionTemplet templet, NKMUserData userData)
	{
		return GetNonCumulativeMissionTimes(templet, userData) >= templet.m_Times;
	}

	public static long GetNonCumulativeMissionTimes(NKMMissionTemplet templet, NKMUserData userData, bool bShowErrorlog = true)
	{
		long num = 0L;
		if (templet.m_MissionCond.value1.Count == 0)
		{
			switch (templet.m_MissionCond.mission_cond)
			{
			case NKM_MISSION_COND.ACCOUNT_LEVEL:
				return userData.m_UserLevel;
			case NKM_MISSION_COND.WORLDMAP_BRANCH_NUMBER:
				return userData.m_WorldmapData.GetUnlockedCityCount();
			case NKM_MISSION_COND.JUST_OPEN:
				return 1L;
			case NKM_MISSION_COND.COUNTER_CASE_OPENED:
				break;
			case NKM_MISSION_COND.UNLOCKED_UNIT_REACTOR:
				return userData.m_ArmyData.m_dicMyUnit.Where((KeyValuePair<long, NKMUnitData> e) => e.Value.reactorLevel > 0).Count();
			case NKM_MISSION_COND.COLLECT_OPR_LEVEL:
				return userData.m_ArmyData.m_dicMyOperator.Count;
			default:
				if (bShowErrorlog)
				{
					Log.Error($"[NKMMissionManager] \ufffd\u033c\ufffd\ufffd\ufffd values \ufffd\ufffd\ufffd\ufffd 0\ufffd\ufffd \ufffd\u033c\ufffd. missionId:{templet.m_MissionID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMMissionManagerEx.cs", 1100);
				}
				return 0L;
			}
			num += userData.GetCounterCaseClearCount(0);
		}
		else
		{
			foreach (int value in templet.m_MissionCond.value1)
			{
				switch (templet.m_MissionCond.mission_cond)
				{
				case NKM_MISSION_COND.COLLECT_UNIT_LEVEL:
					num += userData.m_ArmyData.GetUnitCountByLevel(value);
					break;
				case NKM_MISSION_COND.COLLECT_SHIP_LEVEL:
					num += userData.m_ArmyData.GetShipCountByLevel(value);
					break;
				case NKM_MISSION_COND.WARFARE_CLEARED:
					num += (userData.CheckWarfareClear(value) ? 1 : 0);
					break;
				case NKM_MISSION_COND.DUNGEON_CLEARED:
					num += (userData.CheckDungeonClear(value) ? 1 : 0);
					break;
				case NKM_MISSION_COND.PHASE_CLEAR:
					num += (userData.CheckPhaseClear(value) ? 1 : 0);
					break;
				case NKM_MISSION_COND.COLLECT_SHIP_GET:
					num += (userData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_SHIP, value, NKMArmyData.UNIT_SEARCH_OPTION.None, 0) ? 1 : 0);
					break;
				case NKM_MISSION_COND.COLLECT_SHIP_GET_LEVEL:
					num += (userData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_SHIP, value, NKMArmyData.UNIT_SEARCH_OPTION.Level, (int)templet.m_Times) ? templet.m_Times : 0);
					break;
				case NKM_MISSION_COND.COLLECT_SHIP_UPGRADE:
					num += (userData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_SHIP, value, NKMArmyData.UNIT_SEARCH_OPTION.StarGrade, (int)templet.m_Times) ? templet.m_Times : 0);
					break;
				case NKM_MISSION_COND.COUNTER_CASE_OPENED:
					num += userData.GetCounterCaseClearCount(value);
					break;
				case NKM_MISSION_COND.ACHIEVEMENT_CLEARED:
				{
					NKMMissionTemplet missionTemplet = GetMissionTemplet(value);
					if (missionTemplet == null)
					{
						Log.Error($"[NKMMissionManager] Find Templet Fail. missionId:{value}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMMissionManagerEx.cs", 1150);
						break;
					}
					NKMMissionData missionDataByGroupId = userData.m_MissionData.GetMissionDataByGroupId(missionTemplet.m_GroupId);
					if (missionDataByGroupId != null)
					{
						if (missionDataByGroupId.IsComplete)
						{
							num++;
						}
						else if (value < missionDataByGroupId.mission_id)
						{
							num++;
						}
					}
					break;
				}
				case NKM_MISSION_COND.MENTORING_MISSION_CLEARED:
				{
					NKMMissionData missionDataByMissionId = userData.m_MissionData.GetMissionDataByMissionId(value);
					if (missionDataByMissionId != null && missionDataByMissionId.IsComplete)
					{
						num++;
					}
					break;
				}
				case NKM_MISSION_COND.COLLECT_EQUIP_ENCHANT_LEVEL:
					num += userData.m_InventoryData.GetEquipCountByEnchantLevel(value);
					break;
				case NKM_MISSION_COND.PVP_HIGHEST_TIER_CLEARED:
					num += ((userData.m_PvpData.MaxLeagueTierID >= value) ? 1 : 0);
					break;
				case NKM_MISSION_COND.DIVE_HIGHEST_CLEARED:
					num += (userData.m_DiveClearData.Contains(value) ? 1 : 0);
					break;
				case NKM_MISSION_COND.MISSION_EVENT_TAB_CLEAR:
				{
					int num5 = DicMissionId.Values.Where((NKMMissionTemplet e) => e.m_MissionTabId == value).Count((NKMMissionTemplet e) => userData.m_MissionData.IsMissionCompleted(e));
					num += num5;
					break;
				}
				case NKM_MISSION_COND.UNIT_POWER_HIGHEST:
				{
					int num4 = 0;
					foreach (NKMUnitData value3 in userData.m_ArmyData.m_dicMyUnit.Values)
					{
						NKMEquipmentSet equipmentSet = value3.GetEquipmentSet(userData.m_InventoryData);
						if (equipmentSet != null && value3.CalculateUnitOperationPower(equipmentSet) >= value)
						{
							num4++;
						}
					}
					num += num4;
					break;
				}
				case NKM_MISSION_COND.DUNGEON_SQUAD_UNIT_POWER_HIGHEST:
				{
					int num3 = (from e in userData.m_ArmyData.GetDeckList(NKM_DECK_TYPE.NDT_DAILY)
						where userData.m_ArmyData.GetArmyAvarageOperationPower(e) >= value
						select e).Count();
					num += num3;
					break;
				}
				case NKM_MISSION_COND.WARFARE_SQUAD_UNIT_POWER_HIGHEST:
				{
					int num2 = (from e in userData.m_ArmyData.GetDeckList(NKM_DECK_TYPE.NDT_NORMAL)
						where userData.m_ArmyData.GetArmyAvarageOperationPower(e) >= value
						select e).Count();
					num += num2;
					break;
				}
				case NKM_MISSION_COND.PVP_HIGHEST_TIER_ASYNC:
					num += ((userData.m_AsyncData.MaxLeagueTierID >= value) ? 1 : 0);
					break;
				case NKM_MISSION_COND.PVP_TOTAL_CLEAR_ASYNC:
					num += userData.m_AsyncData.WinCount;
					break;
				case NKM_MISSION_COND.PVP_TOTAL_CLEAR_RANK:
					num += userData.m_PvpData.WinCount;
					break;
				case NKM_MISSION_COND.PVP_TOTAL_CLEAR_BOTH:
					num += userData.m_AsyncData.WinCount;
					num += userData.m_PvpData.WinCount;
					break;
				case NKM_MISSION_COND.COLLECT_UNIT_TACTICS_LEVEL:
					num += userData.m_ArmyData.GetUnitCountByTactic(value);
					break;
				case NKM_MISSION_COND.MISSION_CLEAR:
				{
					DicMissionId.TryGetValue(value, out var value2);
					if (userData.m_MissionData.IsMissionCompleted(value2))
					{
						num++;
					}
					break;
				}
				case NKM_MISSION_COND.UNIT_GROWTH_GET:
					if (userData.m_ArmyData.IsCollectedUnit(value))
					{
						num++;
					}
					break;
				case NKM_MISSION_COND.GET_SKIN:
					num += (userData.m_InventoryData.HasItemSkin(value) ? 1 : 0);
					break;
				case NKM_MISSION_COND.PALACE_CLEARED:
					num += (NKMShadowPalaceManager.IsClearPalace(value) ? 1 : 0);
					break;
				case NKM_MISSION_COND.UNLOCKED_UNIT_REACTOR:
					num += userData.m_ArmyData.m_dicMyUnit.Where((KeyValuePair<long, NKMUnitData> e) => e.Value.reactorLevel >= value).Count();
					break;
				case NKM_MISSION_COND.COLLECT_OPR_LEVEL:
					num += userData.m_ArmyData.GetOperatorCountByLevel(value);
					break;
				default:
					num = num;
					break;
				}
			}
		}
		return num;
	}

	public static bool GetHaveClearedMission()
	{
		return m_bHaveCompletedMission;
	}

	public static void SetHaveClearedMission(bool bSet, bool bVisible = true)
	{
		if (bVisible)
		{
			m_bHaveCompletedMission = bSet;
		}
		NKCUIManager.OnMissionUpdated();
	}

	public static bool IsGuideMissionOpen()
	{
		foreach (KeyValuePair<int, NKMMissionTabTemplet> item in DicMissionTab)
		{
			if (item.Value.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION && item.Value.EnableByTag && item.Value.m_firstMissionID > 0)
			{
				NKMMissionTemplet missionTemplet = GetMissionTemplet(item.Value.m_firstMissionID);
				if (missionTemplet != null && missionTemplet.EnableByTag)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool GetHaveClearedMissionGuide()
	{
		return m_bHaveCompletedMissionGuide;
	}

	public static void SetHaveClearedMissionGuide(bool bSet, bool bVisible = true)
	{
		if (bVisible)
		{
			m_bHaveCompletedMissionGuide = bSet;
		}
		NKCUIManager.OnMissionUpdated();
	}
}
