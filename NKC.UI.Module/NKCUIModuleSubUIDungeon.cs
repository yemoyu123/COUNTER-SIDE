using Cs.Core.Util;
using NKC.UI.Fierce;
using NKM;
using NKM.Event;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Module;

public class NKCUIModuleSubUIDungeon : NKCUIModuleSubUIBase
{
	public TMP_Text m_lbSeasonName;

	[Space]
	public GameObject m_objMaxWave;

	public TMP_Text m_lbBestScore;

	[Space]
	public NKCUIComMedal m_Medal;

	public NKCUIComDungeonRewardList m_RewardList;

	[Space]
	public GameObject m_objDesc;

	public Text m_lbDesc;

	[Space]
	public NKCUIComStateButton m_btnRule;

	[Space]
	public NKCUIComStateButton m_btnEnemyLevel;

	public Text m_lbEnemyLevel;

	[Space]
	public TMP_Text m_lbDate;

	public NKCUIComStateButton m_btnStart;

	[Space]
	public GameObject m_objRank;

	public NKCUIComStateButton m_btnRank;

	public NKCUILeaderBoardSlot m_slotRank;

	public GameObject m_objMyRank;

	public TMP_Text m_lbMyRank;

	public NKCUIComStateButton m_btnRankReward;

	[Space]
	public NKCUIComStateButton m_btnScoreReward;

	public GameObject m_objReddot;

	private NKMDefenceTemplet m_DefenceTemplet;

	private NKMDungeonTempletBase m_DungeonTempletBase;

	public override void Init()
	{
		base.Init();
		if (m_btnStart != null)
		{
			m_btnStart.PointerClick.RemoveAllListeners();
			m_btnStart.PointerClick.AddListener(OnClickStart);
			m_btnStart.m_bGetCallbackWhileLocked = true;
		}
		if (m_RewardList != null)
		{
			m_RewardList.InitUI();
		}
		if (m_btnEnemyLevel != null)
		{
			m_btnEnemyLevel.PointerClick.RemoveAllListeners();
			m_btnEnemyLevel.PointerClick.AddListener(OnClickEnemyLevel);
		}
		if (m_btnRank != null)
		{
			m_btnRank.PointerClick.RemoveAllListeners();
			m_btnRank.PointerClick.AddListener(OnClickRank);
		}
		if (m_slotRank != null)
		{
			m_slotRank.InitUI();
		}
		if (m_btnRankReward != null)
		{
			m_btnRankReward.PointerClick.RemoveAllListeners();
			m_btnRankReward.PointerClick.AddListener(OnClickRankReward);
		}
		if (m_btnScoreReward != null)
		{
			m_btnScoreReward.PointerClick.RemoveAllListeners();
			m_btnScoreReward.PointerClick.AddListener(OnClickScoreReward);
		}
		if (m_btnRule != null)
		{
			m_btnRule.PointerClick.RemoveAllListeners();
			m_btnRule.PointerClick.AddListener(OnClickRule);
		}
	}

	public override void UnHide()
	{
	}

	public override void OnClose()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		ModuleID = templet.Key;
		int intValue = NKCUtil.GetIntValue(templet.m_Option, "DefenceTempletID", 0);
		m_DefenceTemplet = NKMDefenceTemplet.Find(intValue);
		if (m_DefenceTemplet != null)
		{
			m_DungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_DefenceTemplet.m_DungeonID);
			if (m_DungeonTempletBase != null)
			{
				SetData();
			}
		}
	}

	public override void OnOpen(NKMEventTabTemplet eventTabTemplet)
	{
		m_DefenceTemplet = NKMDefenceTemplet.Find(eventTabTemplet.m_EventID);
		if (m_DefenceTemplet != null)
		{
			m_DungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_DefenceTemplet.m_DungeonID);
			if (m_DungeonTempletBase != null)
			{
				SetData();
			}
		}
	}

	private void SetData()
	{
		if (!string.IsNullOrEmpty(m_DefenceTemplet.m_SeasonName))
		{
			NKCUtil.SetLabelText(m_lbSeasonName, m_DefenceTemplet.GetSeasonName());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSeasonName, "");
		}
		NKCUtil.SetLabelText(m_lbDate, NKCUtilString.GetTimeIntervalString(m_DefenceTemplet.IntervalTemplet.GetStartDate(), m_DefenceTemplet.IntervalTemplet.GetEndDate(), NKMTime.INTERVAL_FROM_UTC));
		if (m_Medal != null)
		{
			m_Medal.SetData(m_DefenceTemplet);
		}
		NKCUtil.SetLabelText(m_lbEnemyLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, m_DungeonTempletBase.m_DungeonLevel));
		if (m_objDesc != null)
		{
			NKCUtil.SetGameobjectActive(m_objDesc, !string.IsNullOrEmpty(m_DefenceTemplet.m_BannerDescKey));
			if (!string.IsNullOrEmpty(m_DefenceTemplet.m_BannerDescKey))
			{
				NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString(m_DefenceTemplet.m_BannerDescKey));
			}
		}
		if (m_RewardList != null)
		{
			if (m_RewardList.CreateRewardSlotDataList(NKCScenManager.CurrentUserData(), m_DefenceTemplet, m_DefenceTemplet.m_DungeonID.ToString()))
			{
				NKCUtil.SetGameobjectActive(m_RewardList, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_RewardList, bValue: false);
			}
		}
		if (m_objMaxWave != null)
		{
			NKCUtil.SetGameobjectActive(m_objMaxWave, bValue: true);
			NKCUtil.SetLabelText(m_lbBestScore, string.Format(NKCUtilString.GET_STRING_DEFENCE_BANNER_BEST_SCORE, NKCDefenceDungeonManager.m_BestClearScore.ToString("#,##0")));
		}
		if (m_DefenceTemplet.m_ScoreRewardGroupID > 0)
		{
			NKCUtil.SetGameobjectActive(m_btnScoreReward, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_btnScoreReward, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_btnRule, bValue: true);
		SetRankData();
		NKCPacketSender.Send_NKMPacket_DEFENCE_INFO_REQ(m_DefenceTemplet.Key);
	}

	private void SetRankData()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVE_DEFENCE_RANK_REWARD))
		{
			NKCUtil.SetGameobjectActive(m_slotRank, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMyRank, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRank, bValue: true);
			NKCUtil.SetLabelText(m_lbMyRank, NKCDefenceDungeonManager.GetRankingTotalDesc());
			NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_DEFENCE, 0);
			if (nKMLeaderBoardTemplet != null && m_slotRank != null)
			{
				LeaderBoardSlotData leaderBoardSlotData = LeaderBoardSlotData.MakeSlotData(NKCDefenceDungeonManager.m_topRankData, 1);
				if (leaderBoardSlotData != null && leaderBoardSlotData.userUid > 0)
				{
					m_slotRank.SetData(leaderBoardSlotData, nKMLeaderBoardTemplet.m_BoardCriteria, null);
				}
				else
				{
					m_slotRank.SetEmptySlot();
				}
			}
			if (NKCDefenceDungeonManager.m_bCanReceiveRankReward && m_DefenceTemplet.RewardIntervalTemplet.IsValidTime(ServiceTime.Now) && (NKCDefenceDungeonManager.m_MyRankNum > 0 || NKCDefenceDungeonManager.m_MyRankPercent > 0))
			{
				NKCPacketSender.Send_NKMPacket_DEFENCE_RANK_REWARD_REQ();
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_slotRank, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMyRank, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRank, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objReddot, NKCDefenceDungeonManager.IsCanReceivePointReward());
		if (m_DefenceTemplet.CheckGameEnable(ServiceTime.Now, out var _))
		{
			m_btnStart.SetLock(value: false);
		}
		else if (m_DefenceTemplet.RewardIntervalTemplet.IsValidTime(ServiceTime.Now))
		{
			m_btnStart.SetLock(value: true);
		}
	}

	public override void Refresh()
	{
		SetRankData();
	}

	private void OnClickStart()
	{
		if (m_btnStart.m_bLock)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_DEF_TOAST_EVENT_END, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_DefenceTemplet.m_DungeonID);
		NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(dungeonTempletBase, DeckContents.DEFENCE);
		NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetBackButtonShortcut(NKM_SHORTCUT_TYPE.SHORTCUT_EVENT_COLLECTION, ModuleID.ToString());
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
	}

	private void OnClickEnemyLevel()
	{
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_DefenceTemplet.m_DungeonID);
		NKCPopupEnemyList.Instance.Open(dungeonTempletBase);
	}

	private void OnClickRank()
	{
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_DEFENCE, 0);
		if (nKMLeaderBoardTemplet != null)
		{
			NKCPopupLeaderBoardSingle.Instance.OpenSingle(nKMLeaderBoardTemplet);
		}
	}

	private void OnClickRankReward()
	{
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_DEFENCE, 0);
		if (nKMLeaderBoardTemplet != null)
		{
			NKCUIPopupFierceBattleRewardInfo.Instance.Open(nKMLeaderBoardTemplet.m_BoardTab);
		}
	}

	private void OnClickScoreReward()
	{
		NKCUIPopupDungeonScoreRewardInfo.Instance.Open();
	}

	private void OnClickRule()
	{
		NKCUIPopupDungeonGuide.Instance.Open(m_DungeonTempletBase.GetDungeonName(), NKCStringTable.GetString(m_DefenceTemplet.m_BannerDescKey));
	}

	public void Update()
	{
		if (base.gameObject.activeInHierarchy && m_RewardList != null)
		{
			m_RewardList.ShowRewardListUpdate();
		}
	}
}
