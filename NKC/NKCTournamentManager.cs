using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Game;
using Cs.Logging;
using NKC.UI;
using NKC.UI.Module;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public static class NKCTournamentManager
{
	public const int NoLeaderIndex = -1;

	public static NKMPacket_TOURNAMENT_INFO_ACK m_TournamentInfo;

	public static NKCUIModuleSubUITournament.EventModuleDataTouranment m_TournamentRewardInfo;

	public static List<List<LeaderBoardSlotData>> m_lstLeaderBoardSlotData = new List<List<LeaderBoardSlotData>>();

	public static List<NKMTournamentProfileData> m_lstFinalUserInfos = new List<NKMTournamentProfileData>();

	public static NKMTournamentState m_LastRankReceivedState = NKMTournamentState.Ended;

	public static NKMTournamentTemplet m_TournamentTemplet { get; private set; }

	public static bool m_TournamentApply { get; private set; }

	public static NKMAsyncDeckData m_TournamentApplyDeckData { get; private set; }

	public static List<NKMTournamentInfo> m_TournamentInfoPredict { get; private set; }

	public static NKMTournamentGroups m_replayTournamentGroup { get; private set; }

	public static int m_eventCollectionIndexId { get; private set; }

	public static bool m_TournamentInfoChanged { get; private set; }

	public static int TournamentId
	{
		get
		{
			if (m_TournamentInfo == null)
			{
				return 0;
			}
			return m_TournamentInfo.tournamentId;
		}
	}

	public static bool m_bRankInfoReceived { get; private set; }

	public static void Initialize()
	{
		m_TournamentInfoPredict = new List<NKMTournamentInfo>();
		m_lstLeaderBoardSlotData.Clear();
		m_lstFinalUserInfos.Clear();
		m_TournamentApplyDeckData = new NKMAsyncDeckData();
		m_TournamentApply = false;
		m_TournamentTemplet = null;
		m_bRankInfoReceived = false;
		m_LastRankReceivedState = NKMTournamentState.Ended;
		m_TournamentInfo = null;
		m_TournamentRewardInfo = null;
		m_eventCollectionIndexId = 0;
		m_replayTournamentGroup = NKMTournamentGroups.None;
		m_TournamentInfoChanged = true;
	}

	public static NKMTournamentState GetTournamentState()
	{
		if (m_TournamentInfo == null)
		{
			return NKMTournamentState.Ended;
		}
		return m_TournamentInfo.state;
	}

	public static DateTime GetTournamentStateStartDate(NKMTournamentState state)
	{
		if (m_TournamentTemplet == null)
		{
			return default(DateTime);
		}
		return m_TournamentTemplet.GetTournamentStateStartDate(state);
	}

	public static string GetTournamentTryoutKey()
	{
		return $"TOURNAMENT_TRYOUT_CHECK_COUNT_{TournamentId}_{NKCScenManager.CurrentUserData().m_UserUID}";
	}

	public static int GetTryoutCheckIndex()
	{
		if (PlayerPrefs.HasKey(GetTournamentTryoutKey()))
		{
			return PlayerPrefs.GetInt(GetTournamentTryoutKey());
		}
		return -1;
	}

	public static void SetTryoutCheckIndex(int idx)
	{
		PlayerPrefs.SetInt(GetTournamentTryoutKey(), idx);
	}

	public static void SetTournamentApply(NKMAsyncDeckData deckData)
	{
		m_TournamentApply = deckData != null && deckData.ship.unitUid > 0;
		m_TournamentApplyDeckData = deckData;
	}

	public static List<NKMTournamentPlayInfo> GetTryoutHistory()
	{
		List<NKMTournamentPlayInfo> list = new List<NKMTournamentPlayInfo>();
		if (m_TournamentInfo != null && m_TournamentInfo.history != null)
		{
			for (int i = 0; i < m_TournamentInfo.history.Count; i++)
			{
				if (m_TournamentInfo.history[i] != null)
				{
					if (m_TournamentInfo.history[i].history == null)
					{
						Log.Error($"m_TournamentInfo.history[{i}].history is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCTournamentManager.cs", 125);
					}
					else
					{
						list.Add(m_TournamentInfo.history[i]);
					}
				}
			}
		}
		return list;
	}

	public static LeaderBoardSlotData GetCurSeasonTopRank()
	{
		LeaderBoardSlotData result = new LeaderBoardSlotData();
		if (m_lstLeaderBoardSlotData.Count > 0)
		{
			for (int num = m_lstLeaderBoardSlotData.Count - 1; num >= 0; num--)
			{
				List<LeaderBoardSlotData> list = m_lstLeaderBoardSlotData[num];
				if (list.Count > 0 && list[0].memberCount == TournamentId)
				{
					return list[0];
				}
			}
		}
		return result;
	}

	public static bool CanRecvReward()
	{
		if (GetTournamentState() != NKMTournamentState.Closing)
		{
			return false;
		}
		if (m_TournamentInfo == null || !m_TournamentInfo.canRecvReward)
		{
			return false;
		}
		if (!m_TournamentApply && m_TournamentInfoPredict.Count == 0)
		{
			return false;
		}
		return true;
	}

	public static bool IsTryoutAllChecked()
	{
		return m_TournamentInfo.history.Count == GetTryoutCheckIndex() + 1;
	}

	public static bool IsWinTryout()
	{
		return m_TournamentInfo.history[m_TournamentInfo.history.Count - 1].history.Result == PVP_RESULT.WIN;
	}

	public static void LoadLastDeckData()
	{
		List<long> list = new List<long>();
		for (int i = 0; i < 8; i++)
		{
			if (m_TournamentApplyDeckData.units.Count > i && m_TournamentApplyDeckData.units[i] != null)
			{
				list.Add(m_TournamentApplyDeckData.units[i].unitUid);
			}
			else
			{
				list.Add(0L);
			}
		}
		NKMDeckData nKMDeckData = new NKMDeckData(NKM_DECK_TYPE.NDT_TOURNAMENT);
		nKMDeckData.m_listDeckUnitUID = list;
		nKMDeckData.m_ShipUID = GetAppliedShipUID();
		nKMDeckData.m_OperatorUID = GetAppliedOperatorUID();
		nKMDeckData.power = m_TournamentApplyDeckData.operationPower;
		nKMDeckData.m_LeaderIndex = (sbyte)m_TournamentApplyDeckData.leaderIndex;
		if (NKCScenManager.CurrentArmyData().GetDeckData(NKM_DECK_TYPE.NDT_TOURNAMENT, 0) != null)
		{
			NKCScenManager.CurrentArmyData().SetTournamentDeck(nKMDeckData);
		}
		else
		{
			NKCScenManager.CurrentArmyData().AddDeck(NKM_DECK_TYPE.NDT_TOURNAMENT, nKMDeckData);
		}
	}

	public static NKMAsyncUnitData GetAppliedAsyncUnitData(long unitUid)
	{
		return m_TournamentApplyDeckData.units.Find((NKMAsyncUnitData x) => x != null && x.unitUid == unitUid);
	}

	public static long GetAppliedShipUID()
	{
		if (m_TournamentApplyDeckData.ship != null)
		{
			return m_TournamentApplyDeckData.ship.unitUid;
		}
		return 0L;
	}

	public static long GetAppliedOperatorUID()
	{
		if (m_TournamentApplyDeckData.operatorUnit != null)
		{
			return m_TournamentApplyDeckData.operatorUnit.uid;
		}
		return 0L;
	}

	public static float CalculateDeckAvgSummonCost()
	{
		int num = 0;
		int num2 = 0;
		if (m_TournamentApplyDeckData == null)
		{
			return 0f;
		}
		for (int i = 0; i < 8 && i < m_TournamentApplyDeckData.units.Count; i++)
		{
			NKMAsyncUnitData nKMAsyncUnitData = m_TournamentApplyDeckData.units[i];
			if (nKMAsyncUnitData != null && nKMAsyncUnitData.unitId != 0)
			{
				num++;
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(nKMAsyncUnitData.unitId);
				if (unitStatTemplet == null)
				{
					Log.Error($"Cannot found UnitStatTemplet. UnitId:{nKMAsyncUnitData.unitId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCTournamentManager.cs", 253);
				}
				else
				{
					num2 += unitStatTemplet.GetRespawnCost(i == m_TournamentApplyDeckData.leaderIndex, null, null);
				}
			}
		}
		if (num == 0)
		{
			return 0f;
		}
		return (float)num2 / (float)num;
	}

	public static bool IsUnitChanged(int idx, NKMUnitData unitData)
	{
		if (m_TournamentApplyDeckData.units.Count <= idx)
		{
			return true;
		}
		NKMAsyncUnitData nKMAsyncUnitData = m_TournamentApplyDeckData.units[idx];
		if (unitData == null && nKMAsyncUnitData == null)
		{
			return false;
		}
		if (unitData == null || nKMAsyncUnitData == null)
		{
			return true;
		}
		return IsUnitChanged(nKMAsyncUnitData, unitData);
	}

	public static bool IsUnitChanged(NKMAsyncUnitData asyncUnitData, NKMUnitData unitData)
	{
		if (asyncUnitData == null && unitData == null)
		{
			return false;
		}
		if (asyncUnitData == null || unitData == null)
		{
			return true;
		}
		if (asyncUnitData.unitUid == 0L || unitData.m_UnitUID == 0L)
		{
			return true;
		}
		if (asyncUnitData.unitUid != unitData.m_UnitUID)
		{
			return true;
		}
		if (asyncUnitData.unitLevel != unitData.m_UnitLevel)
		{
			return true;
		}
		if (asyncUnitData.limitBreakLevel != unitData.m_LimitBreakLevel)
		{
			return true;
		}
		for (int i = 0; i < asyncUnitData.skillLevel.Count; i++)
		{
			if (asyncUnitData.skillLevel[i] != unitData.m_aUnitSkillLevel[i])
			{
				return true;
			}
		}
		if (asyncUnitData.equipUids.Count != unitData.EquipItemUids.Count)
		{
			return true;
		}
		for (int j = 0; j < asyncUnitData.equipUids.Count; j++)
		{
			if (IsEquipChanged(asyncUnitData.equipUids[j], unitData.EquipItemUids[j]))
			{
				return true;
			}
		}
		if (asyncUnitData.reactorLevel != unitData.reactorLevel)
		{
			return true;
		}
		if (asyncUnitData.tacticLevel != unitData.tacticLevel)
		{
			return true;
		}
		return false;
	}

	public static bool IsEquipChanged(long asyncEquipUid, long unitEquipUid)
	{
		if (asyncEquipUid != unitEquipUid)
		{
			return true;
		}
		if (asyncEquipUid > 0 && unitEquipUid > 0)
		{
			NKMEquipItemData asyncEquipData = m_TournamentApplyDeckData.equips.Find((NKMEquipItemData x) => x.m_ItemUid == asyncEquipUid);
			NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(unitEquipUid);
			return IsEquipChanged(asyncEquipData, itemEquip);
		}
		return false;
	}

	public static bool IsEquipChanged(NKMEquipItemData asyncEquipData, NKMEquipItemData unitEquipData)
	{
		if (!IsSameEquipStat(asyncEquipData.m_Stat, unitEquipData.m_Stat))
		{
			return true;
		}
		if (asyncEquipData.m_SetOptionId != unitEquipData.m_SetOptionId)
		{
			return true;
		}
		return false;
	}

	public static bool IsSameEquipStat(List<EQUIP_ITEM_STAT> lStat, List<EQUIP_ITEM_STAT> rStat)
	{
		if (lStat.Count != rStat.Count)
		{
			return false;
		}
		for (int i = 0; i < lStat.Count; i++)
		{
			if (lStat[i].type != rStat[i].type)
			{
				return false;
			}
			if (lStat[i].stat_value != rStat[i].stat_value)
			{
				return false;
			}
			if (lStat[i].stat_level_value != rStat[i].stat_level_value)
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsShipChanged(NKMUnitData shipData)
	{
		NKMAsyncUnitData nKMAsyncUnitData = m_TournamentApplyDeckData.ship;
		if (nKMAsyncUnitData.unitId == 0)
		{
			nKMAsyncUnitData = null;
		}
		if (shipData == null && nKMAsyncUnitData == null)
		{
			return false;
		}
		if (shipData == null || nKMAsyncUnitData == null)
		{
			return true;
		}
		if (nKMAsyncUnitData.unitUid != shipData.m_UnitUID)
		{
			return true;
		}
		if (nKMAsyncUnitData.unitId != shipData.m_UnitID)
		{
			return true;
		}
		if (nKMAsyncUnitData.unitLevel != shipData.m_UnitLevel)
		{
			return true;
		}
		if (nKMAsyncUnitData.limitBreakLevel != shipData.m_LimitBreakLevel)
		{
			return true;
		}
		for (int i = 0; i < nKMAsyncUnitData.skillLevel.Count; i++)
		{
			if (nKMAsyncUnitData.skillLevel[i] != shipData.m_aUnitSkillLevel[i])
			{
				return true;
			}
		}
		int num = Math.Max(nKMAsyncUnitData.shipModules.Count, shipData.ShipCommandModule.Count);
		for (int j = 0; j < num; j++)
		{
			if (j >= nKMAsyncUnitData.shipModules.Count)
			{
				nKMAsyncUnitData.shipModules.Add(new NKMShipCmdModule());
			}
			if (j >= shipData.ShipCommandModule.Count)
			{
				shipData.ShipCommandModule.Add(new NKMShipCmdModule());
			}
			if (IsChangedShipModule(nKMAsyncUnitData.shipModules[j], shipData.ShipCommandModule[j]))
			{
				return true;
			}
		}
		if (nKMAsyncUnitData.tacticLevel != shipData.tacticLevel)
		{
			return true;
		}
		return false;
	}

	private static bool IsChangedShipModule(NKMShipCmdModule lItem, NKMShipCmdModule rItem)
	{
		if (rItem != null && rItem.slots == null)
		{
			for (int i = 0; i < rItem.slots.Length; i++)
			{
				if (rItem.slots[i] != null)
				{
					if (lItem.slots == null || lItem.slots.Length <= i || lItem.slots[i] != rItem.slots[i])
					{
						return true;
					}
				}
				else if (lItem.slots != null && lItem.slots.Length > i && lItem.slots[i] != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsOperatorChanged(NKMOperator operatorData)
	{
		NKMOperator operatorUnit = m_TournamentApplyDeckData.operatorUnit;
		if (operatorUnit == null && operatorData == null)
		{
			return false;
		}
		if (operatorUnit == null || operatorData == null)
		{
			return true;
		}
		if (operatorUnit.uid == 0L || operatorData.uid == 0L)
		{
			return true;
		}
		if (operatorUnit.id != operatorData.id)
		{
			return true;
		}
		if (operatorUnit.level != operatorData.level)
		{
			return true;
		}
		if (operatorUnit.mainSkill.id != operatorData.mainSkill.id)
		{
			return true;
		}
		if (operatorUnit.mainSkill.level != operatorData.mainSkill.level)
		{
			return true;
		}
		if (operatorUnit.subSkill.id != operatorData.subSkill.id)
		{
			return true;
		}
		if (operatorUnit.subSkill.level != operatorData.subSkill.level)
		{
			return true;
		}
		if (operatorUnit.power != operatorData.power)
		{
			return true;
		}
		return false;
	}

	public static void SetTournamentPredictInfo(NKMTournamentInfo info)
	{
		int num = m_TournamentInfoPredict.FindIndex((NKMTournamentInfo e) => (int)e.groupIndex % 10 == (int)info.groupIndex % 10);
		if (num >= 0)
		{
			m_TournamentInfoPredict[num] = info;
		}
		else
		{
			m_TournamentInfoPredict.Add(info);
		}
	}

	public static void SetTournamentPredictInfo(List<NKMTournamentInfo> infoList)
	{
		m_TournamentInfoPredict = infoList;
	}

	public static NKMTournamentInfo GetTournamentInfo(NKMTournamentGroups group)
	{
		return m_TournamentInfo?.infos?.Find((NKMTournamentInfo e) => (int)e.groupIndex % 10 == (int)group % 10);
	}

	public static NKMTournamentInfo GetTournamentInfoPredict(NKMTournamentGroups group)
	{
		return m_TournamentInfoPredict?.Find((NKMTournamentInfo e) => (int)e.groupIndex % 10 == (int)group % 10);
	}

	public static void SetReplayTournamentGroup(NKMTournamentGroups replayGroup)
	{
		m_replayTournamentGroup = replayGroup;
	}

	public static void SetEventCollectionIndexId(int id)
	{
		m_eventCollectionIndexId = id;
	}

	public static string GetNextIntervalTimeStr()
	{
		if (m_TournamentTemplet != null)
		{
			return m_TournamentTemplet.GetTournamentStateEndDate(GetTournamentState()).ToString();
		}
		return string.Empty;
	}

	public static HashSet<int> GetTournamentFinalBanIds(NKM_UNIT_TYPE unitType)
	{
		if (m_TournamentInfo.tournamentBanResult == null)
		{
			return new HashSet<int>();
		}
		HashSet<int> hashSet = new HashSet<int>();
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			foreach (KeyValuePair<int, NKMTournamentBanResult> item in m_TournamentInfo.tournamentBanResult)
			{
				hashSet.UnionWith(item.Value.unitBanList);
			}
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
			foreach (KeyValuePair<int, NKMTournamentBanResult> item2 in m_TournamentInfo.tournamentBanResult)
			{
				hashSet.UnionWith(item2.Value.shipBanList);
			}
			break;
		}
		return hashSet;
	}

	public static bool IsEmpty(NKMTournamentBanResult banResult)
	{
		if (banResult == null)
		{
			return true;
		}
		if (banResult.unitBanList.Count > 0)
		{
			return false;
		}
		if (banResult.shipBanList.Count > 0)
		{
			return false;
		}
		return true;
	}

	public static NKMTournamentGroups GetOriginalGroup(NKMTournamentGroups group)
	{
		if (m_TournamentTemplet == null)
		{
			return NKMTournamentGroups.None;
		}
		bool isUnify = m_TournamentTemplet.IsUnify;
		switch (group)
		{
		default:
			return NKMTournamentGroups.None;
		case NKMTournamentGroups.GroupA:
		case NKMTournamentGroups.GlobalGroupA:
			if (isUnify)
			{
				return NKMTournamentGroups.GlobalGroupA;
			}
			return NKMTournamentGroups.GroupA;
		case NKMTournamentGroups.GroupB:
		case NKMTournamentGroups.GlobalGroupB:
			if (isUnify)
			{
				return NKMTournamentGroups.GlobalGroupB;
			}
			return NKMTournamentGroups.GroupB;
		case NKMTournamentGroups.GroupC:
		case NKMTournamentGroups.GlobalGroupC:
			if (isUnify)
			{
				return NKMTournamentGroups.GlobalGroupC;
			}
			return NKMTournamentGroups.GroupC;
		case NKMTournamentGroups.GroupD:
		case NKMTournamentGroups.GlobalGroupD:
			if (isUnify)
			{
				return NKMTournamentGroups.GlobalGroupD;
			}
			return NKMTournamentGroups.GroupD;
		case NKMTournamentGroups.Finals:
		case NKMTournamentGroups.GlobalFinals:
			if (isUnify)
			{
				return NKMTournamentGroups.GlobalFinals;
			}
			return NKMTournamentGroups.Finals;
		}
	}

	public static void SetTournamentInfoChanged(bool bChanged)
	{
		m_TournamentInfoChanged = bChanged;
	}

	public static void SetTournamentInfo(NKMPacket_TOURNAMENT_INFO_ACK sPacket)
	{
		m_TournamentInfo = sPacket;
		m_TournamentApplyDeckData = sPacket.deck;
		if (m_TournamentApplyDeckData.units.Count < 8)
		{
			for (int i = m_TournamentApplyDeckData.units.Count; i < 8; i++)
			{
				m_TournamentApplyDeckData.units.Add(null);
			}
		}
		m_lstFinalUserInfos = new List<NKMTournamentProfileData>();
		for (int j = 0; j < sPacket.infos.Count; j++)
		{
			NKMTournamentInfo info = sPacket.infos[j];
			int k;
			for (k = 0; k < info.slotUserUid.Count; k++)
			{
				if (info.slotUserUid[k] > 0)
				{
					if (!info.userInfo.ContainsKey(info.slotUserUid[k]))
					{
						Log.Error($"UID : {info.slotUserUid[k]} - userInfo is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCTournamentManager.cs", 668);
					}
					else if (m_lstFinalUserInfos.Find((NKMTournamentProfileData x) => x.commonProfile.userUid == info.slotUserUid[k]) == null && info.userInfo.ContainsKey(info.slotUserUid[k]))
					{
						m_lstFinalUserInfos.Add(info.userInfo[info.slotUserUid[k]]);
					}
				}
			}
		}
		m_TournamentTemplet = NKMTournamentTemplet.Find(sPacket.tournamentId);
		m_TournamentApply = sPacket.deck != null && sPacket.deck.ship != null && sPacket.deck.ship.unitUid > 0;
		SetTournamentInfoChanged(bChanged: false);
		foreach (NKCUIModuleHome item in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
		{
			if (item.IsOpen)
			{
				item.UpdateUI();
			}
		}
		if (NKCUIModuleSubUITournamentLobby.IsInstanceOpen)
		{
			NKCUIModuleSubUITournamentLobby.Instance.Refresh();
		}
	}

	public static void SetRankInfos(List<NKMTournamentRankInfo> lstRankInfos)
	{
		m_lstLeaderBoardSlotData.Clear();
		for (int i = 0; i < lstRankInfos.Count; i++)
		{
			List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
			bool flag = false;
			for (int j = 0; j < lstRankInfos[i].profiles.Count; j++)
			{
				if (lstRankInfos[i].ranks[j] > 0)
				{
					flag = true;
				}
				LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(LeaderBoardType.BT_TOURNAMENT, lstRankInfos[i].profiles[j], j + 1, lstRankInfos[i].tournamentId);
				list.Add(item);
			}
			if (flag)
			{
				m_lstLeaderBoardSlotData.Add(list);
			}
		}
		m_lstLeaderBoardSlotData.Sort(CompRankData);
		m_bRankInfoReceived = true;
		m_LastRankReceivedState = GetTournamentState();
	}

	private static int CompRankData(List<LeaderBoardSlotData> lItem, List<LeaderBoardSlotData> rItem)
	{
		if (lItem.Count > 0 && rItem.Count > 0)
		{
			return rItem.FirstOrDefault().memberCount.CompareTo(lItem.FirstOrDefault().memberCount);
		}
		return -1;
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_REWARD_ACK sPacket)
	{
		m_TournamentInfo.canRecvReward = false;
		m_TournamentRewardInfo = new NKCUIModuleSubUITournament.EventModuleDataTouranment();
		m_TournamentRewardInfo.tournamentid = sPacket.tournamentid;
		m_TournamentRewardInfo.bOpenRank = false;
		m_TournamentRewardInfo.bOpenResult = true;
		m_TournamentRewardInfo.hitCount = sPacket.hitCount;
		m_TournamentRewardInfo.predictionRewardData = sPacket.predictionRewardData;
		m_TournamentRewardInfo.groupIndex = sPacket.groupIndex;
		m_TournamentRewardInfo.winData = sPacket.winData;
		m_TournamentRewardInfo.rankRewardData = sPacket.rankRewardData;
		if (NKCUIModuleHome.IsAnyInstanceOpen())
		{
			NKCUIModuleHome.SendMessage(m_TournamentRewardInfo);
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_REWARD_INFO_ACK sPacket)
	{
		m_TournamentRewardInfo = new NKCUIModuleSubUITournament.EventModuleDataTouranment();
		m_TournamentRewardInfo.tournamentid = sPacket.tournamentid;
		m_TournamentRewardInfo.bOpenRank = false;
		m_TournamentRewardInfo.bOpenResult = true;
		m_TournamentRewardInfo.hitCount = sPacket.hitCount;
		m_TournamentRewardInfo.predictionRewardData = sPacket.predictionRewardData;
		m_TournamentRewardInfo.groupIndex = sPacket.groupIndex;
		m_TournamentRewardInfo.winData = sPacket.winData;
		m_TournamentRewardInfo.rankRewardData = sPacket.rankRewardData;
		if (NKCUIModuleHome.IsAnyInstanceOpen())
		{
			NKCUIModuleHome.SendMessage(m_TournamentRewardInfo);
		}
	}

	public static void SetCastingVoteUnitList(List<int> lstUnitId)
	{
		if (m_TournamentInfo != null)
		{
			m_TournamentInfo.pvpCastingVoteData.unitIdList = lstUnitId;
		}
	}

	public static void SetCastingVoteShipList(List<int> lstShipGroupIds)
	{
		if (m_TournamentInfo != null)
		{
			m_TournamentInfo.pvpCastingVoteData.shipGroupIdList = lstShipGroupIds;
		}
	}
}
