using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Event;
using Cs.Logging;
using NKC;
using NKC.Templet;
using NKM.Event;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMEventManager
{
	private static Dictionary<int, NKMEventBingoTemplet> dicBingoTempletByMissionTab = new Dictionary<int, NKMEventBingoTemplet>();

	private static Dictionary<int, List<NKMEventBingoRewardTemplet>> dicBingoRewardTemplet = new Dictionary<int, List<NKMEventBingoRewardTemplet>>();

	private static Dictionary<int, EventBingo> dicEventBingo = new Dictionary<int, EventBingo>();

	public static bool LoadFromLua()
	{
		NKMTempletContainer<NKMEventTabTemplet>.Load("AB_SCRIPT", "LUA_EVENT_TAB_TEMPLET", "EVENT_TAB_TEMPLET", NKMEventTabTemplet.LoadFromLUA);
		NKMTempletContainer<NKMEventBingoTemplet>.Load("AB_SCRIPT", "LUA_EVENT_BINGO_TEMPLET", "EVENT_BINGO_TEMPLET", NKMEventBingoTemplet.LoadFromLUA);
		dicBingoTempletByMissionTab = NKMTempletContainer<NKMEventBingoTemplet>.Values.ToDictionary((NKMEventBingoTemplet e) => e.m_BingoMissionTabId, (NKMEventBingoTemplet e) => e);
		dicBingoRewardTemplet = NKMTempletLoader<NKMEventBingoRewardTemplet>.LoadGroup("AB_SCRIPT", "LUA_EVENT_BINGO_REWARD_TEMPLET", "EVENT_BINGO_REWARD_TEMPLET", NKMEventBingoRewardTemplet.LoadFromLUA);
		if (dicBingoRewardTemplet == null)
		{
			Log.ErrorAndExit("NKMEventBingoRewardTemplet load failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventManager.cs", 33);
			return false;
		}
		NKMTempletContainer<NKMEventBarTemplet>.Load("AB_SCRIPT", "LUA_EVENT_BAR_TEMPLET", "EVENT_BAR_TEMPLET", NKMEventBarTemplet.LoadFromLua);
		return true;
	}

	public static NKMEventTabTemplet GetTabTemplet(int eventId)
	{
		return NKMTempletContainer<NKMEventTabTemplet>.Find(eventId);
	}

	public static NKMEventBingoTemplet GetBingoTemplet(int eventId)
	{
		return NKMTempletContainer<NKMEventBingoTemplet>.Find(eventId);
	}

	public static List<NKMEventBingoRewardTemplet> GetBingoRewardTempletList(int eventID)
	{
		NKMEventBingoTemplet nKMEventBingoTemplet = NKMTempletContainer<NKMEventBingoTemplet>.Find(eventID);
		if (nKMEventBingoTemplet != null && dicBingoRewardTemplet.TryGetValue(nKMEventBingoTemplet.m_BingoCompletRewardGroupID, out var value))
		{
			return value;
		}
		return null;
	}

	public static NKMEventBingoRewardTemplet GetBingoRewardTemplet(int groupId, BingoCompleteType completeType, int completeTypeValue)
	{
		if (!dicBingoRewardTemplet.TryGetValue(groupId, out var value))
		{
			return null;
		}
		return value.Find((NKMEventBingoRewardTemplet e) => e.m_BingoCompletType.Equals(completeType) && e.m_BingoCompletTypeValue.Equals(completeTypeValue));
	}

	public static NKMEventBingoRewardTemplet GetBingoRewardTemplet(int groupId, int rewardIndex)
	{
		if (!dicBingoRewardTemplet.TryGetValue(groupId, out var value))
		{
			return null;
		}
		return value.Find((NKMEventBingoRewardTemplet e) => e.ZeroBaseTileIndex == rewardIndex);
	}

	public static List<NKMEventTabTemplet> GetBingoEvents()
	{
		return NKMTempletContainer<NKMEventTabTemplet>.Values.Where((NKMEventTabTemplet e) => e.m_EventType == NKM_EVENT_TYPE.BINGO && e.EnableByTag).ToList();
	}

	public static void CheckValidation()
	{
		foreach (List<NKMEventBingoRewardTemplet> value in dicBingoRewardTemplet.Values)
		{
			foreach (NKMEventBingoRewardTemplet item in value)
			{
				item.Validate();
			}
		}
	}

	public static bool CheckRedDot(bool checkActiveBannerOnly)
	{
		foreach (NKMEventTabTemplet value in NKMTempletContainer<NKMEventTabTemplet>.Values)
		{
			if ((!checkActiveBannerOnly || value == null || value.ShowEventBanner()) && CheckRedDot(value))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckRedDot(NKMEventTabTemplet templet)
	{
		if (templet == null)
		{
			return false;
		}
		if (!templet.IsAvailable)
		{
			return false;
		}
		switch (templet.m_EventType)
		{
		case NKM_EVENT_TYPE.BINGO:
		{
			int eventID = templet.m_EventID;
			if (CheckRedDotBingoSingle(eventID) || CheckRedDotBingoSet(eventID) || CheckRedDotBingoMission(eventID))
			{
				return true;
			}
			break;
		}
		case NKM_EVENT_TYPE.MISSION:
		case NKM_EVENT_TYPE.ONTIME:
		case NKM_EVENT_TYPE.RACE:
		case NKM_EVENT_TYPE.MISSION_ROW:
		{
			NKCEventMissionTemplet nKCEventMissionTemplet3 = NKCEventMissionTemplet.Find(templet.m_EventID);
			if (nKCEventMissionTemplet3 == null)
			{
				break;
			}
			NKMUserData nKMUserData3 = NKCScenManager.CurrentUserData();
			foreach (int item in nKCEventMissionTemplet3.m_lstMissionTab)
			{
				if (nKMUserData3.m_MissionData.CheckCompletableMission(nKMUserData3, item))
				{
					return true;
				}
			}
			if (nKMUserData3.m_MissionData.CheckCompletableMission(nKMUserData3, nKCEventMissionTemplet3.m_SpecialMissionTab))
			{
				return true;
			}
			break;
		}
		case NKM_EVENT_TYPE.KAKAOEMOTE:
		{
			NKCEventMissionTemplet nKCEventMissionTemplet2 = NKCEventMissionTemplet.Find(templet.m_EventID);
			NKMUserData nKMUserData2 = NKCScenManager.CurrentUserData();
			if (nKMUserData2.kakaoMissionData != null)
			{
				switch (nKMUserData2.kakaoMissionData.state)
				{
				case KakaoMissionState.Initialized:
				case KakaoMissionState.Registered:
				case KakaoMissionState.Sent:
				case KakaoMissionState.Failed:
				case KakaoMissionState.Flopped:
					return true;
				}
			}
			if (nKCEventMissionTemplet2 == null)
			{
				break;
			}
			foreach (int item2 in nKCEventMissionTemplet2.m_lstMissionTab)
			{
				if (nKMUserData2.m_MissionData.CheckCompletableMission(nKMUserData2, item2))
				{
					return true;
				}
			}
			if (nKMUserData2.m_MissionData.CheckCompletableMission(nKMUserData2, nKCEventMissionTemplet2.m_SpecialMissionTab))
			{
				return true;
			}
			break;
		}
		case NKM_EVENT_TYPE.KILLCOUNT:
		{
			int num = 0;
			int num2 = 0;
			long num3 = 0L;
			NKMKillCountData killCountData = NKCKillCountManager.GetKillCountData(templet.m_EventID);
			if (killCountData != null)
			{
				num = killCountData.serverCompleteStep;
				num2 = killCountData.userCompleteStep;
				num3 = killCountData.killCount;
			}
			NKMKillCountTemplet nKMKillCountTemplet = NKMKillCountTemplet.Find(templet.m_EventID);
			if (nKMKillCountTemplet == null)
			{
				return false;
			}
			int maxServerStep = nKMKillCountTemplet.GetMaxServerStep();
			if (num < maxServerStep)
			{
				NKMKillCountStepTemplet result = null;
				nKMKillCountTemplet.TryGetServerStep(num + 1, out result);
				NKMServerKillCountData killCountServerData = NKCKillCountManager.GetKillCountServerData(templet.m_EventID);
				if (result != null && killCountServerData != null && result.KillCount <= killCountServerData.killCount)
				{
					return true;
				}
			}
			int maxUserStep = nKMKillCountTemplet.GetMaxUserStep();
			if (num2 < maxUserStep)
			{
				NKMKillCountStepTemplet result2 = null;
				nKMKillCountTemplet.TryGetUserStep(num2 + 1, out result2);
				if (result2 != null && result2.KillCount <= num3)
				{
					return true;
				}
			}
			break;
		}
		case NKM_EVENT_TYPE.BAR:
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData == null)
			{
				break;
			}
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(NKCEventBarManager.DailyCocktailItemID);
			NKMEventBarTemplet nKMEventBarTemplet = NKMEventBarTemplet.Find(NKCEventBarManager.DailyCocktailItemID);
			if (nKMEventBarTemplet == null)
			{
				break;
			}
			if (countMiscItem >= nKMEventBarTemplet.DeliveryValue && NKCEventBarManager.RemainDeliveryLimitValue > 0)
			{
				return true;
			}
			NKCEventMissionTemplet nKCEventMissionTemplet = NKCEventMissionTemplet.Find(templet.m_EventID);
			if (nKCEventMissionTemplet == null)
			{
				break;
			}
			int count = nKCEventMissionTemplet.m_lstMissionTab.Count;
			for (int i = 0; i < count; i++)
			{
				if (nKMUserData.m_MissionData.CheckCompletableMission(nKMUserData, nKCEventMissionTemplet.m_lstMissionTab[i]))
				{
					return true;
				}
			}
			break;
		}
		}
		return false;
	}

	public static void SetEventInfo(EventInfo eventInfo)
	{
		dicEventBingo.Clear();
		foreach (BingoInfo item in eventInfo.bingoInfo)
		{
			AddBingo(item.eventId, item);
		}
	}

	public static bool IsEventCompleted(NKMEventTabTemplet tabTemplet)
	{
		if (tabTemplet == null)
		{
			return false;
		}
		switch (tabTemplet.m_EventType)
		{
		case NKM_EVENT_TYPE.BINGO:
		{
			NKMEventBingoTemplet bingoTemplet = GetBingoTemplet(tabTemplet.m_EventID);
			if (bingoTemplet == null)
			{
				return false;
			}
			EventBingo bingoData = GetBingoData(bingoTemplet.m_EventID);
			if (bingoData != null && bingoData.Completed())
			{
				return true;
			}
			break;
		}
		case NKM_EVENT_TYPE.MISSION:
		case NKM_EVENT_TYPE.ONTIME:
		case NKM_EVENT_TYPE.RACE:
		case NKM_EVENT_TYPE.MISSION_ROW:
		{
			NKCEventMissionTemplet nKCEventMissionTemplet2 = NKCEventMissionTemplet.Find(tabTemplet.m_EventID);
			if (nKCEventMissionTemplet2 == null)
			{
				break;
			}
			NKMUserData cNKMUserData2 = NKCScenManager.CurrentUserData();
			foreach (int item in nKCEventMissionTemplet2.m_lstMissionTab)
			{
				if (!NKMContentUnlockManager.IsContentUnlocked(cNKMUserData2, new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_MISSION_TAB_ALL_CLEAR, item), ignoreSuperUser: true))
				{
					return false;
				}
			}
			if (!NKMContentUnlockManager.IsContentUnlocked(cNKMUserData2, new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_MISSION_TAB_ALL_CLEAR, nKCEventMissionTemplet2.m_SpecialMissionTab), ignoreSuperUser: true))
			{
				return false;
			}
			return true;
		}
		case NKM_EVENT_TYPE.KAKAOEMOTE:
		{
			if (NKCScenManager.CurrentUserData().kakaoMissionData != null)
			{
				switch (NKCScenManager.CurrentUserData().kakaoMissionData.state)
				{
				default:
					return false;
				case KakaoMissionState.Confirmed:
				case KakaoMissionState.NotEnoughBudget:
				case KakaoMissionState.OutOfDate:
					break;
				}
			}
			NKCEventMissionTemplet nKCEventMissionTemplet = NKCEventMissionTemplet.Find(tabTemplet.m_EventID);
			if (nKCEventMissionTemplet == null)
			{
				break;
			}
			NKMUserData cNKMUserData = NKCScenManager.CurrentUserData();
			foreach (int item2 in nKCEventMissionTemplet.m_lstMissionTab)
			{
				if (!NKMContentUnlockManager.IsContentUnlocked(cNKMUserData, new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_MISSION_TAB_ALL_CLEAR, item2), ignoreSuperUser: true))
				{
					return false;
				}
			}
			if (!NKMContentUnlockManager.IsContentUnlocked(cNKMUserData, new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_MISSION_TAB_ALL_CLEAR, nKCEventMissionTemplet.m_SpecialMissionTab), ignoreSuperUser: true))
			{
				return false;
			}
			return true;
		}
		}
		return false;
	}

	private static void AddBingo(int eventID, BingoInfo bingoInfo)
	{
		if (!dicEventBingo.ContainsKey(eventID))
		{
			dicEventBingo.Add(eventID, new EventBingo(eventID, bingoInfo));
		}
	}

	public static EventBingo GetBingoData(int eventID)
	{
		if (dicEventBingo.TryGetValue(eventID, out var value))
		{
			return value;
		}
		return null;
	}

	public static bool IsReceiveableBingoReward(int eventID, int rewardIndex)
	{
		if (!dicEventBingo.TryGetValue(eventID, out var value))
		{
			return false;
		}
		if (value.m_bingoInfo.rewardList.Contains(rewardIndex))
		{
			return false;
		}
		NKMEventBingoRewardTemplet bingoRewardTemplet = GetBingoRewardTemplet(value.m_bingoTemplet.m_BingoCompletRewardGroupID, rewardIndex);
		if (bingoRewardTemplet == null)
		{
			return false;
		}
		List<int> bingoLine = value.GetBingoLine();
		if (bingoRewardTemplet.m_BingoCompletType == BingoCompleteType.LINE_SINGLE)
		{
			return bingoLine.Contains(bingoRewardTemplet.m_BingoCompletTypeValue - 1);
		}
		if (bingoRewardTemplet.m_BingoCompletType == BingoCompleteType.LINE_SET)
		{
			return bingoLine.Count >= bingoRewardTemplet.m_BingoCompletTypeValue;
		}
		return false;
	}

	public static bool CheckRedDotBingoSingle(int eventID)
	{
		EventBingo bingoData = GetBingoData(eventID);
		if (bingoData == null)
		{
			return false;
		}
		List<int> bingoLine = bingoData.GetBingoLine();
		for (int i = 0; i < bingoLine.Count; i++)
		{
			int num = bingoLine[i];
			NKMEventBingoRewardTemplet bingoRewardTemplet = GetBingoRewardTemplet(bingoData.m_bingoTemplet.m_BingoCompletRewardGroupID, BingoCompleteType.LINE_SINGLE, num + 1);
			if (bingoRewardTemplet != null && IsReceiveableBingoReward(eventID, bingoRewardTemplet.ZeroBaseTileIndex))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckRedDotBingoSet(int eventID)
	{
		EventBingo bingoData = GetBingoData(eventID);
		if (bingoData == null)
		{
			return false;
		}
		List<int> bingoLine = bingoData.GetBingoLine();
		foreach (NKMEventBingoRewardTemplet bingoRewardTemplet in GetBingoRewardTempletList(eventID))
		{
			if (bingoRewardTemplet.m_BingoCompletType == BingoCompleteType.LINE_SET && bingoLine.Count >= bingoRewardTemplet.m_BingoCompletTypeValue && !bingoData.m_bingoInfo.rewardList.Contains(bingoRewardTemplet.ZeroBaseTileIndex))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckRedDotBingoMission(int eventID)
	{
		NKMEventBingoTemplet bingoTemplet = GetBingoTemplet(eventID);
		if (bingoTemplet == null)
		{
			return false;
		}
		List<NKMMissionTemplet> missionTempletListByType = NKMMissionManager.GetMissionTempletListByType(bingoTemplet.m_BingoMissionTabId);
		if (missionTempletListByType == null)
		{
			return false;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		for (int i = 0; i < missionTempletListByType.Count; i++)
		{
			NKMMissionTemplet templet = missionTempletListByType[i];
			NKMMissionData missionData = nKMUserData.m_MissionData.GetMissionData(templet);
			if (missionData != null && !missionData.IsComplete && NKMMissionManager.CanComplete(templet, nKMUserData, missionData) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
		}
		return false;
	}

	public static NKMEventBingoRewardTemplet GetBingoLastRewardTemplet(int eventID)
	{
		List<NKMEventBingoRewardTemplet> bingoRewardTempletList = GetBingoRewardTempletList(eventID);
		if (bingoRewardTempletList == null)
		{
			return null;
		}
		bingoRewardTempletList = bingoRewardTempletList.FindAll((NKMEventBingoRewardTemplet v) => v.m_BingoCompletType == BingoCompleteType.LINE_SET);
		if (bingoRewardTempletList.Count == 0)
		{
			return null;
		}
		return bingoRewardTempletList[bingoRewardTempletList.Count - 1];
	}
}
