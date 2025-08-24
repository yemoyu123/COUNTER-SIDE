using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientPacket.Common;
using ClientPacket.Game;
using ClientPacket.LeaderBoard;
using NKC.UI;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKCFierceBattleSupportDataMgr
{
	public enum FIERCE_STATUS
	{
		FS_UNUSABLE,
		FS_LOCKED,
		FS_WAIT,
		FS_ACTIVATE,
		FS_REWARD,
		FS_COMPLETE
	}

	private struct FierceBossData
	{
		public int point;

		public bool bClear;

		public NKMEventDeckData deckData;

		public FierceBossData(int _point, bool _clear, NKMEventDeckData _deckData)
		{
			point = _point;
			bClear = _clear;
			deckData = _deckData;
		}
	}

	private struct FierceRankData
	{
		public NKMLeaderBoardFierceData LeaderBoardFierceData;

		public int UserRank;

		public bool IsAll;
	}

	public const int FIERCE_RANKING_DISPLAY_SIZE = 50;

	private NKMFierceTemplet m_FierceTemplet;

	private int m_iCurBossID;

	private int m_iCurFierceBossGroupID;

	private DateTime m_NextStepTime = DateTime.MinValue;

	private List<NKMFierceBoss> m_lstFierceBoss = new List<NKMFierceBoss>();

	private HashSet<int> m_PointReward = new HashSet<int>();

	private bool m_bReceivedRankReward;

	private int totalRankNumber;

	private int totalRankPercent;

	private Dictionary<int, FierceRankData> m_dicFierceRanking = new Dictionary<int, FierceRankData>();

	public NKMFierceTemplet FierceTemplet => m_FierceTemplet;

	public int CurBossID => m_iCurBossID;

	public int CurBossGroupID => m_iCurFierceBossGroupID;

	public bool m_fierceDailyRewardReceived { get; private set; }

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKMFierceTemplet>.Load("AB_SCRIPT", "LUA_FIERCE_TEMPLET", "FIERCE_TEMPLET", NKMFierceTemplet.LoadFromLUA);
		NKMTempletContainer<NKMFierceBossGroupTemplet>.Load("AB_SCRIPT", "LUA_FIERCE_BOSS_GROUP_TEMPLET", "FIERCE_GROUP_TEMPLET", NKMFierceBossGroupTemplet.LoadFromLUA);
		NKMTempletContainer<NKMFiercePointRewardTemplet>.Load("AB_SCRIPT", "LUA_FIERCE_POINT_REWARD", "FIERCE_POINT_REWARD_TEMPLET", NKMFiercePointRewardTemplet.LoadFromLUA);
		NKMTempletContainer<NKMFierceRankRewardTemplet>.Load("AB_SCRIPT", "LUA_FIERCE_RANK_REWARD", "FIERCE_RANK_REWARD_TEMPLET", NKMFierceRankRewardTemplet.LoadFromLUA);
		NKMTempletContainer<NKMFiercePenaltyTemplet>.Load("AB_SCRIPT", "LUA_FIERCE_PENALTY", "FIERCE_PENALTY", NKMFiercePenaltyTemplet.LoadFromLua);
	}

	public void Init(int fierceID)
	{
		foreach (NKMFierceTemplet value in NKMFierceTemplet.Values)
		{
			if (value.FierceID == fierceID)
			{
				m_FierceTemplet = value;
				ResetBossData();
				return;
			}
		}
		Debug.Log($"격전지원 정보를 확인 할 수 없습니다. - {fierceID}");
	}

	private void ResetBossData()
	{
		if (m_FierceTemplet != null)
		{
			m_iCurFierceBossGroupID = m_FierceTemplet.FierceBossGroupIdList[0];
			if (!NKMFierceBossGroupTemplet.Groups.ContainsKey(m_iCurFierceBossGroupID))
			{
				Debug.Log("격전 지원 데이터 확인 필요!");
				return;
			}
			NKMFierceBossGroupTemplet nKMFierceBossGroupTemplet = NKMFierceBossGroupTemplet.Groups[m_iCurFierceBossGroupID].First();
			m_iCurBossID = nKMFierceBossGroupTemplet.FierceBossID;
		}
	}

	public int GetFierceBattleID()
	{
		if (m_FierceTemplet != null)
		{
			return m_FierceTemplet.FierceID;
		}
		return 0;
	}

	public int GetCurSelectedBossLv()
	{
		return GetCurSelectedBossLv(m_iCurFierceBossGroupID);
	}

	public int GetCurSelectedBossLv(int iFierceBossGroupID)
	{
		if (!NKMFierceBossGroupTemplet.Groups.ContainsKey(iFierceBossGroupID))
		{
			return 0;
		}
		foreach (NKMFierceBossGroupTemplet item in NKMFierceBossGroupTemplet.Groups[iFierceBossGroupID])
		{
			if (item.FierceBossID == m_iCurBossID)
			{
				return item.Level;
			}
		}
		return 0;
	}

	public int GetMatchedBossLv(int iFierceBossGroupID, int iBossID)
	{
		if (!NKMFierceBossGroupTemplet.Groups.ContainsKey(iFierceBossGroupID))
		{
			return 0;
		}
		foreach (NKMFierceBossGroupTemplet item in NKMFierceBossGroupTemplet.Groups[iFierceBossGroupID])
		{
			if (item.FierceBossID == iBossID)
			{
				return item.Level;
			}
		}
		return 0;
	}

	public string GetCurSelectedBossLvDesc(int iCurBossGroupID)
	{
		int curSelectedBossLv = GetCurSelectedBossLv(iCurBossGroupID);
		if (curSelectedBossLv > 3)
		{
			return string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, NKCUtilString.GET_STRING_FIERCE_BOSS_LEVEL_EXPERT);
		}
		return string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, curSelectedBossLv);
	}

	public FIERCE_STATUS GetStatus()
	{
		if (NKCContentManager.IsContentAlwaysLocked(ContentsType.FIERCE, 0))
		{
			return FIERCE_STATUS.FS_UNUSABLE;
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.FIERCE))
		{
			return FIERCE_STATUS.FS_LOCKED;
		}
		if (m_FierceTemplet != null)
		{
			if (!m_FierceTemplet.EnableByTag)
			{
				return FIERCE_STATUS.FS_WAIT;
			}
			if (NKCSynchronizedTime.IsFinished(NKMTime.LocalToUTC(m_FierceTemplet.FierceGameStart)))
			{
				if (NKCSynchronizedTime.IsEventTime(NKMTime.LocalToUTC(m_FierceTemplet.FierceGameStart), NKMTime.LocalToUTC(m_FierceTemplet.FierceGameEnd)))
				{
					return FIERCE_STATUS.FS_ACTIVATE;
				}
				if (NKCSynchronizedTime.IsEventTime(NKMTime.LocalToUTC(m_FierceTemplet.FierceRewardPeriodStart), NKMTime.LocalToUTC(m_FierceTemplet.FierceRewardPeriodEnd)))
				{
					if (IsPossibleRankReward())
					{
						return FIERCE_STATUS.FS_REWARD;
					}
					return FIERCE_STATUS.FS_COMPLETE;
				}
			}
			return FIERCE_STATUS.FS_WAIT;
		}
		return FIERCE_STATUS.FS_LOCKED;
	}

	private void UpdateNextStepUTCTime()
	{
		m_NextStepTime = DateTime.MinValue;
		switch (GetStatus())
		{
		case FIERCE_STATUS.FS_WAIT:
			m_NextStepTime = NKMTime.LocalToUTC(m_FierceTemplet.FierceGameStart);
			break;
		case FIERCE_STATUS.FS_ACTIVATE:
			m_NextStepTime = NKMTime.LocalToUTC(m_FierceTemplet.FierceGameEnd);
			break;
		case FIERCE_STATUS.FS_REWARD:
		case FIERCE_STATUS.FS_COMPLETE:
			m_NextStepTime = NKMTime.LocalToUTC(m_FierceTemplet.FierceRewardPeriodEnd);
			break;
		case FIERCE_STATUS.FS_UNUSABLE:
		case FIERCE_STATUS.FS_LOCKED:
			break;
		}
	}

	public string GetLeftTimeString()
	{
		string result = "";
		if (m_NextStepTime == DateTime.MinValue)
		{
			UpdateNextStepUTCTime();
		}
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(m_NextStepTime);
		if (m_NextStepTime != DateTime.MinValue && timeLeft.Ticks <= 0)
		{
			UpdateNextStepUTCTime();
			timeLeft = NKCSynchronizedTime.GetTimeLeft(m_NextStepTime);
		}
		switch (GetStatus())
		{
		case FIERCE_STATUS.FS_WAIT:
			result = ((!(timeLeft.TotalDays >= 1.0)) ? ((!(timeLeft.TotalHours >= 1.0)) ? ((!(timeLeft.TotalMinutes >= 1.0)) ? string.Format(NKCUtilString.GET_FIERCE_WAIT_ACTIVATE_SECOND_DESC_01, (int)timeLeft.TotalSeconds) : string.Format(NKCUtilString.GET_FIERCE_WAIT_ACTIVATE_MINUTE_DESC_01, (int)timeLeft.TotalMinutes)) : string.Format(NKCUtilString.GET_FIERCE_WAIT_ACTIVATE_HOUR_DESC_01, (int)timeLeft.TotalHours)) : string.Format(NKCUtilString.GET_FIERCE_WAIT_ACTIVATE_DAY_DESC_01, (int)timeLeft.TotalDays));
			break;
		case FIERCE_STATUS.FS_ACTIVATE:
			result = ((!(timeLeft.TotalDays >= 1.0)) ? ((!(timeLeft.TotalHours >= 1.0)) ? ((!(timeLeft.TotalMinutes >= 1.0)) ? string.Format(NKCUtilString.GET_FIERCE_WAIT_REWARD_SECOND_DESC_01, (int)timeLeft.TotalSeconds) : string.Format(NKCUtilString.GET_FIERCE_WAIT_REWARD_MINUTE_DESC_01, (int)timeLeft.TotalMinutes)) : string.Format(NKCUtilString.GET_FIERCE_WAIT_REWARD_HOUR_DESC_01, (int)timeLeft.TotalHours)) : string.Format(NKCUtilString.GET_FIERCE_WAIT_REWARD_DAY_DESC_01, (int)timeLeft.TotalDays));
			break;
		case FIERCE_STATUS.FS_REWARD:
		case FIERCE_STATUS.FS_COMPLETE:
			result = ((!(timeLeft.TotalDays >= 1.0)) ? ((!(timeLeft.TotalHours >= 1.0)) ? ((!(timeLeft.TotalMinutes >= 1.0)) ? string.Format(NKCUtilString.GET_FIERCE_WAIT_END_SECOND_DESC_01, (int)timeLeft.TotalSeconds) : string.Format(NKCUtilString.GET_FIERCE_WAIT_END_MINUTE_DESC_01, (int)timeLeft.TotalMinutes)) : string.Format(NKCUtilString.GET_FIERCE_WAIT_END_HOUR_DESC_01, (int)timeLeft.TotalHours)) : string.Format(NKCUtilString.GET_FIERCE_WAIT_END_DAY_DESC_01, (int)timeLeft.TotalDays));
			break;
		}
		return result;
	}

	public bool IsCanAccessFierce()
	{
		FIERCE_STATUS status = GetStatus();
		if ((uint)(status - 3) <= 2u)
		{
			return true;
		}
		return false;
	}

	public string GetAccessDeniedMessage()
	{
		if (GetStatus() == FIERCE_STATUS.FS_WAIT && m_FierceTemplet != null)
		{
			if (!NKCSynchronizedTime.IsFinished(NKMTime.LocalToUTC(m_FierceTemplet.FierceGameStart)))
			{
				return string.Format(NKCUtilString.GET_FIERCE_ENTER_WAIT_DESC_01, NKCSynchronizedTime.GetTimeLeftString(NKMTime.LocalToUTC(m_FierceTemplet.FierceGameStart)));
			}
			return NKCUtilString.GET_FIERCE_BATTLE_ENTER_SEASON_END;
		}
		return NKCUtilString.GET_FIERCE_CAN_NOT_ENTER_FIERCE_BATTLE_SUPPORT;
	}

	public void SetCurBossID(int iCurBossID)
	{
		m_iCurBossID = iCurBossID;
	}

	public int GetBossGroupPoint()
	{
		int num = 0;
		IReadOnlyList<NKMFierceBossGroupTemplet> bossGroupList = GetBossGroupList();
		if (bossGroupList != null && bossGroupList.Count > 0)
		{
			foreach (NKMFierceBossGroupTemplet group in bossGroupList)
			{
				NKMFierceBoss nKMFierceBoss = m_lstFierceBoss.Find((NKMFierceBoss e) => e.bossId == group.FierceBossID);
				if (nKMFierceBoss != null && num < nKMFierceBoss.point)
				{
					num = nKMFierceBoss.point;
				}
			}
		}
		return num;
	}

	public void UpdateFierceData(NKMPacket_FIERCE_DATA_ACK sPacket)
	{
		m_PointReward = sPacket.pointRewardHistory;
		m_bReceivedRankReward = sPacket.isRankRewardGotten;
		UpdateFierceBosses(sPacket.bossList);
		UpdateMyFierceRank(sPacket.rankNumber, sPacket.rankPercent);
	}

	public void UpdateMyFierceRank(int rankNumber, int rankPercent)
	{
		totalRankNumber = rankNumber;
		totalRankPercent = rankPercent;
	}

	public void UpdateFierceBosses(List<NKMFierceBoss> fierceBosses)
	{
		if (fierceBosses != null)
		{
			m_lstFierceBoss = fierceBosses;
		}
	}

	public void SetReceivedRankReward()
	{
		m_bReceivedRankReward = true;
	}

	public void SetDailyRewardReceived(bool bfierceDailyRewardReceived)
	{
		m_fierceDailyRewardReceived = bfierceDailyRewardReceived;
	}

	public void UpdateFierceData(NKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_ACK sPacket)
	{
		if (m_dicFierceRanking.ContainsKey(sPacket.fierceBossGroupId))
		{
			FierceRankData value = m_dicFierceRanking[sPacket.fierceBossGroupId];
			value.IsAll = sPacket.isAll;
			value.LeaderBoardFierceData = sPacket.leaderBoardfierceData;
			value.UserRank = sPacket.userRank;
			m_dicFierceRanking[sPacket.fierceBossGroupId] = value;
		}
		else
		{
			FierceRankData value2 = new FierceRankData
			{
				IsAll = sPacket.isAll,
				LeaderBoardFierceData = sPacket.leaderBoardfierceData,
				UserRank = sPacket.userRank
			};
			m_dicFierceRanking.Add(sPacket.fierceBossGroupId, value2);
		}
	}

	public bool IsReceivedPointReward(int rewardID)
	{
		if (m_PointReward != null)
		{
			return m_PointReward.Contains(rewardID);
		}
		return false;
	}

	public bool IsCanReceivePointReward()
	{
		if (NKMFiercePointRewardTemplet.Groups.ContainsKey(m_FierceTemplet.PointRewardGroupID))
		{
			List<NKMFiercePointRewardTemplet> list = NKMFiercePointRewardTemplet.Groups[m_FierceTemplet.PointRewardGroupID];
			int totalPoint = GetTotalPoint();
			foreach (NKMFiercePointRewardTemplet item in list)
			{
				if (item.Point <= totalPoint && !IsReceivedPointReward(item.FiercePointRewardID))
				{
					return true;
				}
			}
		}
		return false;
	}

	public string GetRankingDesc()
	{
		int num = 0;
		string result = string.Format(NKCUtilString.GET_FIERCE_RANK_DESC_01, 100);
		if (NKMFierceBossGroupTemplet.Groups.ContainsKey(m_iCurFierceBossGroupID))
		{
			foreach (NKMFierceBossGroupTemplet data in NKMFierceBossGroupTemplet.Groups[m_iCurFierceBossGroupID])
			{
				NKMFierceBoss nKMFierceBoss = m_lstFierceBoss.Find((NKMFierceBoss i) => i.bossId == data.FierceBossID);
				if (nKMFierceBoss != null && nKMFierceBoss.point > num)
				{
					num = nKMFierceBoss.point;
					result = ((nKMFierceBoss.rankNumber == 0 || nKMFierceBoss.rankNumber > 100) ? string.Format(NKCUtilString.GET_FIERCE_RANK_DESC_01, nKMFierceBoss.rankPercent) : string.Format(NKCUtilString.GET_FIERCE_RANK_IN_TOP_100_DESC_01, nKMFierceBoss.rankNumber));
				}
			}
		}
		return result;
	}

	public string GetRankingTotalDesc()
	{
		if (totalRankNumber != 0 && totalRankNumber <= 100)
		{
			return string.Format(NKCUtilString.GET_FIERCE_RANK_IN_TOP_100_DESC_01, totalRankNumber);
		}
		return string.Format(NKCUtilString.GET_FIERCE_RANK_DESC_01, totalRankPercent);
	}

	public int GetRankingTotalNumber()
	{
		return totalRankNumber;
	}

	public int GetRankingTotalPercent()
	{
		return totalRankPercent;
	}

	public int GetTotalPoint()
	{
		int num = 0;
		if (m_FierceTemplet != null)
		{
			foreach (int fierceBossGroupId in m_FierceTemplet.FierceBossGroupIdList)
			{
				num += GetMaxPoint(fierceBossGroupId);
			}
		}
		return num;
	}

	public int GetMaxPoint(int targetGroupID = 0)
	{
		int num = 0;
		targetGroupID = ((targetGroupID == 0) ? m_iCurFierceBossGroupID : targetGroupID);
		if (NKMFierceBossGroupTemplet.Groups.ContainsKey(targetGroupID))
		{
			foreach (NKMFierceBossGroupTemplet data in NKMFierceBossGroupTemplet.Groups[targetGroupID])
			{
				NKMFierceBoss nKMFierceBoss = m_lstFierceBoss.Find((NKMFierceBoss i) => i.bossId == data.FierceBossID);
				if (nKMFierceBoss != null)
				{
					num = Math.Max(nKMFierceBoss.point, num);
				}
			}
		}
		return num;
	}

	public int GetClearLevel(int bossGroupID = 0)
	{
		bossGroupID = ((bossGroupID == 0) ? m_iCurFierceBossGroupID : bossGroupID);
		int num = 0;
		if (NKMFierceBossGroupTemplet.Groups.ContainsKey(bossGroupID))
		{
			foreach (NKMFierceBossGroupTemplet data in NKMFierceBossGroupTemplet.Groups[bossGroupID])
			{
				NKMFierceBoss nKMFierceBoss = m_lstFierceBoss.Find((NKMFierceBoss i) => i.bossId == data.FierceBossID);
				if (nKMFierceBoss != null && nKMFierceBoss.isCleared)
				{
					num = data.Level;
				}
			}
		}
		Debug.Log($"보스 클리어 레벨 - bossGroupID : {bossGroupID}, clearLevel : {num}");
		return num;
	}

	public int GetTargetDungeonID()
	{
		IReadOnlyList<NKMFierceBossGroupTemplet> bossGroupList = GetBossGroupList();
		if (bossGroupList != null)
		{
			int curSelectedBossLv = GetCurSelectedBossLv(m_iCurFierceBossGroupID);
			foreach (NKMFierceBossGroupTemplet item in bossGroupList)
			{
				if (item.Level == curSelectedBossLv)
				{
					return item.DungeonID;
				}
			}
		}
		return 0;
	}

	public string GetCurBossName()
	{
		return GetTargetBossName(m_iCurFierceBossGroupID, GetCurSelectedBossLv(m_iCurFierceBossGroupID));
	}

	public string GetTargetBossName(int targetBossGroupID, int curBossLv = 0)
	{
		IReadOnlyList<NKMFierceBossGroupTemplet> bossGroupList = GetBossGroupList(targetBossGroupID);
		if (bossGroupList != null && bossGroupList.Count > 0)
		{
			foreach (NKMFierceBossGroupTemplet item in bossGroupList)
			{
				if (item.Level != curBossLv)
				{
					continue;
				}
				NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(item.DungeonID);
				if (dungeonTemplet != null)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(dungeonTemplet.m_BossUnitStrID);
					if (unitTempletBase != null)
					{
						return unitTempletBase.GetUnitName();
					}
				}
			}
		}
		return "";
	}

	public string GetCurBossDesc()
	{
		NKMFierceBossGroupTemplet bossGroupTemplet = GetBossGroupTemplet();
		if (bossGroupTemplet != null && !string.IsNullOrEmpty(bossGroupTemplet.UI_BossDesc))
		{
			return NKCStringTable.GetString(bossGroupTemplet.UI_BossDesc);
		}
		return "";
	}

	public List<NKMBattleConditionTemplet> GetCurBattleCondition(bool bAdditionSelfPenalty = false)
	{
		List<NKMBattleConditionTemplet> list = new List<NKMBattleConditionTemplet>();
		foreach (NKMFierceBossGroupTemplet value in NKMFierceBossGroupTemplet.Values)
		{
			if (value.FierceBossID == m_iCurBossID)
			{
				NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(value.BCondStrID_1);
				if (templetByStrID != null)
				{
					list.Add(templetByStrID);
				}
				NKMBattleConditionTemplet templetByStrID2 = NKMBattleConditionManager.GetTempletByStrID(value.BCondStrID_2);
				if (templetByStrID2 != null)
				{
					list.Add(templetByStrID2);
				}
				break;
			}
		}
		if (bAdditionSelfPenalty)
		{
			List<NKMBattleConditionTemplet> selfPenaltyBattleCondList = GetSelfPenaltyBattleCondList();
			if (selfPenaltyBattleCondList != null)
			{
				list.AddRange(selfPenaltyBattleCondList);
			}
		}
		return list;
	}

	public List<int> GetCurPreConditionGroup()
	{
		return NKMFierceBossGroupTemplet.Values.FirstOrDefault((NKMFierceBossGroupTemplet x) => x.FierceBossID == m_iCurBossID)?.BCPreconditionGroups;
	}

	public IReadOnlyList<NKMFierceBossGroupTemplet> GetBossGroupList()
	{
		return GetBossGroupList(m_iCurFierceBossGroupID);
	}

	public IReadOnlyList<NKMFierceBossGroupTemplet> GetBossGroupList(int fierceBossGroupID)
	{
		if (NKMFierceBossGroupTemplet.Groups.ContainsKey(fierceBossGroupID))
		{
			return NKMFierceBossGroupTemplet.Groups[fierceBossGroupID];
		}
		return null;
	}

	public NKMEventDeckData GetBestLineUp()
	{
		NKMEventDeckData result = null;
		NKMFierceBossGroupTemplet[] array = GetBossGroupList().ToArray();
		if (array != null)
		{
			int num = 0;
			foreach (NKMFierceBoss item in m_lstFierceBoss)
			{
				NKMFierceBossGroupTemplet[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					if (array2[i].FierceBossID == item.bossId && item.deckData != null)
					{
						if (item.deckData.m_dicUnit.Count > 0 && item.point > num)
						{
							num = item.point;
							result = item.deckData;
						}
						break;
					}
				}
			}
		}
		return result;
	}

	public string GetStringCurBossSelfPenalty()
	{
		string result = "";
		List<int> selfPenalty = GetSelfPenalty();
		if (selfPenalty != null && selfPenalty.Count > 0)
		{
			float num = 0f;
			foreach (int item in selfPenalty)
			{
				NKMFiercePenaltyTemplet nKMFiercePenaltyTemplet = NKMTempletContainer<NKMFiercePenaltyTemplet>.Find(item);
				if (nKMFiercePenaltyTemplet != null)
				{
					num += nKMFiercePenaltyTemplet.FierceScoreRate;
				}
			}
			num *= 0.01f;
			if (num < 0f)
			{
				num *= -1f;
				result = string.Format(NKCUtilString.GET_STRING_FIERCE_PENALTY_SCORE_MINUS_DESC, num);
			}
			else
			{
				result = string.Format(NKCUtilString.GET_STRING_FIERCE_PENALTY_SCORE_PLUS_DESC, num);
			}
		}
		return result;
	}

	public bool IsCanStart()
	{
		NKMFierceBossGroupTemplet bossGroupTemplet = GetBossGroupTemplet(m_iCurBossID);
		if (bossGroupTemplet != null)
		{
			NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
			if (inventoryData != null && inventoryData.GetCountMiscItem(bossGroupTemplet.StageReqItemID) < bossGroupTemplet.StageReqItemCount)
			{
				return false;
			}
		}
		if (NKCScenManager.CurrentUserData() != null && m_FierceTemplet.DailyEnterLimit - NKCScenManager.CurrentUserData().GetStatePlayCnt(NKMFierceConst.StageId, IsServiceTime: true) <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_FIERCE_BATTLE_ENTER_LIMIT);
			return false;
		}
		return true;
	}

	public int GetRecommandOperationPower()
	{
		return GetBossGroupTemplet(m_iCurBossID).OperationPower;
	}

	public NKMFierceBossGroupTemplet GetBossGroupTemplet()
	{
		return GetBossGroupTemplet(m_iCurBossID);
	}

	private NKMFierceBossGroupTemplet GetBossGroupTemplet(int bossID)
	{
		return NKMFierceBossGroupTemplet.Find(bossID);
	}

	public bool IsHasFierceRankingData(bool All = false)
	{
		bool flag = false;
		if (m_dicFierceRanking.ContainsKey(m_iCurFierceBossGroupID))
		{
			flag = m_dicFierceRanking[m_iCurFierceBossGroupID].LeaderBoardFierceData != null && m_dicFierceRanking[m_iCurFierceBossGroupID].LeaderBoardFierceData.fierceData != null && m_dicFierceRanking[m_iCurFierceBossGroupID].LeaderBoardFierceData.fierceData.Count > 0;
		}
		if (All && flag)
		{
			flag = m_dicFierceRanking[m_iCurFierceBossGroupID].IsAll;
		}
		return flag;
	}

	public int GetBossGroupRankingDataCnt(int targetBossGroupID = 0)
	{
		targetBossGroupID = ((targetBossGroupID == 0) ? m_iCurFierceBossGroupID : targetBossGroupID);
		if (m_dicFierceRanking.ContainsKey(targetBossGroupID) && m_dicFierceRanking[targetBossGroupID].LeaderBoardFierceData != null && m_dicFierceRanking[targetBossGroupID].LeaderBoardFierceData.fierceData != null)
		{
			return m_dicFierceRanking[targetBossGroupID].LeaderBoardFierceData.fierceData.Count;
		}
		return 0;
	}

	public NKMFierceData GetFierceRankingData(int Rank)
	{
		if (!m_dicFierceRanking.ContainsKey(m_iCurFierceBossGroupID))
		{
			return null;
		}
		NKMLeaderBoardFierceData leaderBoardFierceData = m_dicFierceRanking[m_iCurFierceBossGroupID].LeaderBoardFierceData;
		if (leaderBoardFierceData != null && leaderBoardFierceData.fierceData != null && leaderBoardFierceData.fierceData.Count > Rank)
		{
			return leaderBoardFierceData.fierceData[Rank];
		}
		return null;
	}

	public bool IsPossibleRankReward()
	{
		if (GetTotalPoint() > 0)
		{
			return !m_bReceivedRankReward;
		}
		return false;
	}

	public void UpdateRecevePointRewardID(int receivedPointRewardID)
	{
		if (m_PointReward != null && !m_PointReward.Contains(receivedPointRewardID))
		{
			m_PointReward.Add(receivedPointRewardID);
		}
	}

	public List<NKMBattleConditionTemplet> GetSelfPenaltyBattleCondList()
	{
		List<NKMBattleConditionTemplet> list = new List<NKMBattleConditionTemplet>();
		foreach (int item in GetSelfPenalty())
		{
			NKMFiercePenaltyTemplet nKMFiercePenaltyTemplet = NKMTempletContainer<NKMFiercePenaltyTemplet>.Find(item);
			if (nKMFiercePenaltyTemplet != null && nKMFiercePenaltyTemplet.battleCondition != null)
			{
				list.Add(nKMFiercePenaltyTemplet.battleCondition);
			}
		}
		return list;
	}

	public List<int> GetSelfPenalty()
	{
		List<int> list = new List<int>();
		string curPenaltyKey = GetCurPenaltyKey(m_iCurBossID);
		if (PlayerPrefs.HasKey(curPenaltyKey))
		{
			string text = PlayerPrefs.GetString(curPenaltyKey);
			Debug.Log("<color=red>[격전지원:패널티]GetCurBossSelfPenalty - " + text + "</color>");
			string[] array = text.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				int.TryParse(array[i], out var result);
				NKMFiercePenaltyTemplet nKMFiercePenaltyTemplet = NKMTempletContainer<NKMFiercePenaltyTemplet>.Find(result);
				if (nKMFiercePenaltyTemplet != null)
				{
					list.Add(result);
				}
			}
		}
		return list;
	}

	public void SetSelfPenalty(List<int> lstPenalyIDs)
	{
		string curPenaltyKey = GetCurPenaltyKey(m_iCurBossID);
		if (lstPenalyIDs != null && lstPenalyIDs.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int lstPenalyID in lstPenalyIDs)
			{
				stringBuilder.Append($"{lstPenalyID},");
			}
			Debug.Log("<color=red>[격전지원:패널티]SetCurBossSelfPenalty - " + stringBuilder.ToString() + "</color>");
			PlayerPrefs.SetString(curPenaltyKey, stringBuilder.ToString());
		}
		else
		{
			PlayerPrefs.DeleteKey(curPenaltyKey);
		}
	}

	public void SendPenaltyReq()
	{
		foreach (NKMFierceBossGroupTemplet bossGroup in GetBossGroupList())
		{
			if (bossGroup.FierceBossID == m_iCurBossID && !bossGroup.IsHardModeLevel())
			{
				return;
			}
		}
		List<int> selfPenalty = GetSelfPenalty();
		NKCPacketSender.Send_NKMPacket_FIERCE_PENALTY_REQ(m_iCurBossID, selfPenalty);
	}

	private static string GetCurPenaltyKey(int targetBossID)
	{
		long userUID = NKCScenManager.CurrentUserData().m_UserUID;
		return string.Format($"FIERCE_PENALTY_{userUID.ToString()}_{targetBossID}");
	}

	public static void DeleteCurBossPenaltyData(int targetBossID)
	{
		string curPenaltyKey = GetCurPenaltyKey(targetBossID);
		if (!string.IsNullOrEmpty(curPenaltyKey))
		{
			PlayerPrefs.DeleteKey(curPenaltyKey);
		}
	}
}
