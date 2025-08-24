using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;
using NKC.UI.Gauntlet;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCUIPopupFierceBattleRewardInfo : NKCUIBase
{
	public enum REWARD_SLOT_TYPE
	{
		Normal,
		GauntletLeague
	}

	public class RankUIRewardData
	{
		public enum REWARD_UI_TYPE
		{
			PVP_RANK,
			PVP_LEAGUE_SEASON
		}

		public int RankRewardID { get; private set; }

		public string RankDescStrID { get; private set; }

		public List<NKM_REWARD_DATA> Rewards { get; private set; }

		public RankUIRewardData(NKMFierceRankRewardTemplet templet)
		{
			RankRewardID = templet.FierceRankRewardID;
			RankDescStrID = templet.RankDescStrID;
			Rewards = templet.Rewards;
		}

		public RankUIRewardData(NKMDefenceRankRewardTemplet templet)
		{
			RankRewardID = templet.DefenceRankRewardID;
			RankDescStrID = templet.RankDescStrID;
			Rewards = new List<NKM_REWARD_DATA>();
			for (int i = 0; i < templet.Rewards.Count; i++)
			{
				NKM_REWARD_DATA item = new NKM_REWARD_DATA
				{
					RewardID = templet.Rewards[i].ID,
					RewardQuantity = templet.Rewards[i].Count,
					RewardType = templet.Rewards[i].rewardType
				};
				Rewards.Add(item);
			}
		}

		public RankUIRewardData(NKMTournamentRankRewardTemplet templet)
		{
			RankRewardID = templet.Key;
			RankDescStrID = templet.RankRewardDesc;
			Rewards = new List<NKM_REWARD_DATA>();
			for (int i = 0; i < templet.Rewards.Count; i++)
			{
				NKM_REWARD_DATA item = new NKM_REWARD_DATA
				{
					RewardID = templet.Rewards[i].ID,
					RewardQuantity = templet.Rewards[i].Count,
					RewardType = templet.Rewards[i].rewardType
				};
				Rewards.Add(item);
			}
		}

		public RankUIRewardData(NKMTournamentPredictRewardTemplet templet)
		{
			RankRewardID = templet.Key;
			RankDescStrID = templet.PredictRewardDesc;
			Rewards = new List<NKM_REWARD_DATA>();
			for (int i = 0; i < templet.Rewards.Count; i++)
			{
				NKM_REWARD_DATA item = new NKM_REWARD_DATA
				{
					RewardID = templet.Rewards[i].ID,
					RewardQuantity = templet.Rewards[i].Count,
					RewardType = templet.Rewards[i].rewardType
				};
				Rewards.Add(item);
			}
		}

		public RankUIRewardData(NKMLeaguePvpRankTemplet templet)
		{
			RankRewardID = templet.GroupId;
			RankDescStrID = REWARD_UI_TYPE.PVP_RANK.ToString();
			Rewards = new List<NKM_REWARD_DATA>();
			for (int i = 0; i < templet.RewardSeason.Count; i++)
			{
				NKM_REWARD_DATA item = new NKM_REWARD_DATA
				{
					RewardID = templet.RewardSeason[i].ID,
					RewardQuantity = templet.RewardSeason[i].Count,
					RewardType = templet.RewardSeason[i].rewardType
				};
				Rewards.Add(item);
			}
		}

		public RankUIRewardData(NKMLeaguePvpRankSeasonRewardTemplet templet)
		{
			RankRewardID = templet.SeasonRewardGroupId;
			RankDescStrID = REWARD_UI_TYPE.PVP_LEAGUE_SEASON.ToString();
			Rewards = new List<NKM_REWARD_DATA>();
			for (int i = 0; i < templet.Rewards.Count; i++)
			{
				NKM_REWARD_DATA item = new NKM_REWARD_DATA
				{
					RewardID = templet.Rewards[i].ID,
					RewardQuantity = templet.Rewards[i].Count,
					RewardType = templet.Rewards[i].rewardType
				};
				Rewards.Add(item);
			}
		}
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_LEADER_BOARD_DETAIL";

	private const string UI_ASSET_NAME = "AB_UI_LEADER_BOARD_DETAIL_REWARD_INFO";

	private static NKCUIPopupFierceBattleRewardInfo m_Instance;

	public GameObject m_objTitle;

	public Text m_lbTitle;

	public GameObject m_objDesc;

	public Text m_lbDesc;

	public GameObject m_objMyRank;

	public EventTrigger m_POPUP_FIERCE_BATTLE_REWARD_INFO_Bg;

	public NKCUIComStateButton m_NKM_UI_POPUP_CLOSE_BUTTON;

	public Text m_REWARD_INFO_MyRank_Text;

	public LoopScrollRect m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect;

	private List<RankUIRewardData> m_lstRankRewardData = new List<RankUIRewardData>();

	protected REWARD_SLOT_TYPE m_RewardSlotType;

	protected bool m_bPrepared;

	public Transform m_slotParent;

	private List<NKCUIFierceBattleRewardInfoSlot> m_slotList = new List<NKCUIFierceBattleRewardInfoSlot>();

	private List<NKCPopupGauntletLGSlot> m_lstGauntletSlot = new List<NKCPopupGauntletLGSlot>();

	private Stack<RectTransform> m_slotPool = new Stack<RectTransform>();

	private bool bPrepared;

	public static NKCUIPopupFierceBattleRewardInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupFierceBattleRewardInfo>("AB_UI_LEADER_BOARD_DETAIL", "AB_UI_LEADER_BOARD_DETAIL_REWARD_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupFierceBattleRewardInfo>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "POPUP_FIERCE_BATTLE_REWARD_INFO";

	private static void CleanupInstance()
	{
		m_Instance.Clear();
		m_Instance = null;
	}

	public virtual void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		Clear();
	}

	protected void Init()
	{
		if (m_POPUP_FIERCE_BATTLE_REWARD_INFO_Bg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_POPUP_FIERCE_BATTLE_REWARD_INFO_Bg.triggers.Add(entry);
		}
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_CLOSE_BUTTON, CheckInstanceAndClose);
		if (m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect != null)
		{
			m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect.dOnGetObject += GetSlot;
			m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect.dOnReturnObject += ReturnSlot;
			m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect.dOnProvideData += ProvideData;
			NKCUtil.SetScrollHotKey(m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect);
		}
	}

	public void Open(LeaderBoardType leaderBoardType, REWARD_SLOT_TYPE slotType = REWARD_SLOT_TYPE.Normal)
	{
		m_bPrepared = false;
		m_RewardSlotType = slotType;
		List<RankUIRewardData> list = new List<RankUIRewardData>();
		switch (leaderBoardType)
		{
		case LeaderBoardType.BT_FIERCE:
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_POPUP_FIERCE_BATTLE_REWARD_INFO_TOP_TEXT);
			NKCUtil.SetLabelText(m_lbDesc, NKCUtilString.GET_STRING_POPUP_FIERCE_BATTLE_REWARD_INFO_DESC);
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			if (nKCFierceBattleSupportDataMgr == null || nKCFierceBattleSupportDataMgr.FierceTemplet == null)
			{
				break;
			}
			if (NKMFierceRankRewardTemplet.NumericRankGroupMap.ContainsKey(nKCFierceBattleSupportDataMgr.FierceTemplet.RankRewardGroupID))
			{
				List<NKMFierceRankRewardTemplet> list2 = NKMFierceRankRewardTemplet.NumericRankGroupMap[nKCFierceBattleSupportDataMgr.FierceTemplet.RankRewardGroupID].ToList();
				if (NKMFierceRankRewardTemplet.PercentRankGroupMap.ContainsKey(nKCFierceBattleSupportDataMgr.FierceTemplet.RankRewardGroupID))
				{
					list2.AddRange(NKMFierceRankRewardTemplet.PercentRankGroupMap[nKCFierceBattleSupportDataMgr.FierceTemplet.RankRewardGroupID].ToList());
				}
				list2.OrderBy((NKMFierceRankRewardTemplet e) => e.ShowIndex);
				for (int num = 0; num < list2.Count; num++)
				{
					list.Add(new RankUIRewardData(list2[num]));
				}
			}
			NKCUtil.SetLabelText(m_REWARD_INFO_MyRank_Text, nKCFierceBattleSupportDataMgr.GetRankingTotalDesc());
			break;
		}
		case LeaderBoardType.BT_DEFENCE:
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_POPUP_FIERCE_BATTLE_REWARD_INFO_TOP_TEXT);
			NKCUtil.SetLabelText(m_lbDesc, NKCUtilString.GET_STRING_POPUP_DEF_REWARD_INFO_DESC);
			if (NKCDefenceDungeonManager.m_DefenceTempletId <= 0)
			{
				break;
			}
			NKMDefenceTemplet currentDefenceDungeonTemplet = NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Now);
			if (currentDefenceDungeonTemplet != null)
			{
				List<NKMDefenceRankRewardTemplet> list3 = currentDefenceDungeonTemplet.numberickRewardTemplets.ToList();
				if (currentDefenceDungeonTemplet.percentRewardTemplets.Count() > 0)
				{
					list3.AddRange(currentDefenceDungeonTemplet.percentRewardTemplets.ToList());
				}
				for (int num2 = 0; num2 < list3.Count; num2++)
				{
					list.Add(new RankUIRewardData(list3[num2]));
				}
			}
			NKCUtil.SetLabelText(m_REWARD_INFO_MyRank_Text, NKCDefenceDungeonManager.GetRankingTotalDesc());
			break;
		}
		case LeaderBoardType.BT_LEAGUE:
		case LeaderBoardType.BT_UNLIMITED:
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_POPUP_FIERCE_BATTLE_REWARD_INFO_TOP_TEXT);
			if (leaderBoardType == LeaderBoardType.BT_LEAGUE)
			{
				NKCUtil.SetLabelText(m_lbDesc, NKCUtilString.GET_STRING_POPUP_LEAGUE_REWARD_INFO_DESC);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbDesc, NKCUtilString.GET_STRING_POPUP_UNLIMITED_REWARD_INFO_DESC);
			}
			NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(NKCPVPManager.FindPvPSeasonID(NKM_GAME_TYPE.NGT_PVP_LEAGUE, ServiceTime.ToUtcTime(ServiceTime.Now)));
			if (nKMLeaguePvpRankSeasonTemplet != null)
			{
				NKMLeaguePvpRankSeasonRewardGroupTemplet nKMLeaguePvpRankSeasonRewardGroupTemplet = NKMLeaguePvpRankSeasonRewardGroupTemplet.Find(nKMLeaguePvpRankSeasonTemplet.RankSeasonRewardGroup);
				if (nKMLeaguePvpRankSeasonRewardGroupTemplet != null)
				{
					List<NKMLeaguePvpRankSeasonRewardTemplet> rewardTempletList = nKMLeaguePvpRankSeasonRewardGroupTemplet.GetRewardTempletList();
					for (int i = 0; i < rewardTempletList.Count; i++)
					{
						list.Add(new RankUIRewardData(rewardTempletList[i]));
					}
				}
			}
			NKCUtil.SetLabelText(m_REWARD_INFO_MyRank_Text, NKCPVPManager.GetRankingTotalDesc(leaderBoardType));
			break;
		}
		}
		if (SetData(list))
		{
			UIOpened();
		}
	}

	public bool SetData(List<RankUIRewardData> lstData, bool bShowTitle = true, string title = "", bool bShowDesc = true, string desc = "", bool bShowMyRank = true, string myRankDesc = "")
	{
		m_lstRankRewardData = new List<RankUIRewardData>();
		m_lstRankRewardData.AddRange(lstData);
		int count = lstData.Count;
		if (count == 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return false;
		}
		NKCUtil.SetGameobjectActive(m_objTitle, bShowTitle);
		if (!string.IsNullOrEmpty(title))
		{
			NKCUtil.SetLabelText(m_lbTitle, title);
		}
		NKCUtil.SetGameobjectActive(m_objDesc, bShowDesc);
		if (!string.IsNullOrEmpty(desc))
		{
			NKCUtil.SetLabelText(m_lbDesc, desc);
		}
		NKCUtil.SetGameobjectActive(m_objMyRank, bShowMyRank);
		if (!string.IsNullOrEmpty(myRankDesc))
		{
			NKCUtil.SetLabelText(m_REWARD_INFO_MyRank_Text, myRankDesc);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (!m_bPrepared)
		{
			m_bPrepared = true;
			m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect.PrepareCells();
		}
		m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect.TotalCount = count;
		m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect.SetIndexPosition(0);
		m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect.RefreshCells(bForce: true);
		return true;
	}

	public void Clear()
	{
		m_POPUP_FIERCE_BATTLE_REWARD_INFO_ScrollRect.ReturnAllChild();
		for (int i = 0; i < m_slotList.Count; i++)
		{
			m_slotList[i].DestoryInstance();
		}
		for (int j = 0; j < m_lstGauntletSlot.Count; j++)
		{
			m_lstGauntletSlot[j].DestoryInstance();
		}
		m_slotList.Clear();
		m_lstGauntletSlot.Clear();
		while (m_slotPool.Count > 0)
		{
			RectTransform rectTransform = m_slotPool.Pop();
			if (!(rectTransform != null))
			{
				continue;
			}
			if (m_RewardSlotType == REWARD_SLOT_TYPE.Normal)
			{
				NKCUIFierceBattleRewardInfoSlot component = rectTransform.GetComponent<NKCUIFierceBattleRewardInfoSlot>();
				if (component != null)
				{
					component.DestoryInstance();
				}
			}
			else if (m_RewardSlotType == REWARD_SLOT_TYPE.GauntletLeague)
			{
				NKCPopupGauntletLGSlot component2 = rectTransform.GetComponent<NKCPopupGauntletLGSlot>();
				if (component2 != null)
				{
					component2.DestoryInstance();
				}
			}
		}
	}

	private RectTransform GetSlot(int index)
	{
		if (m_slotPool.Count > 0)
		{
			RectTransform rectTransform = m_slotPool.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		if (m_RewardSlotType == REWARD_SLOT_TYPE.Normal)
		{
			NKCUIFierceBattleRewardInfoSlot newInstance = NKCUIFierceBattleRewardInfoSlot.GetNewInstance(m_slotParent);
			if (newInstance == null)
			{
				return null;
			}
			newInstance.transform.localScale = Vector3.one;
			m_slotList.Add(newInstance);
			return newInstance.GetComponent<RectTransform>();
		}
		if (m_RewardSlotType == REWARD_SLOT_TYPE.GauntletLeague)
		{
			NKCPopupGauntletLGSlot newInstance2 = NKCPopupGauntletLGSlot.GetNewInstance(m_slotParent, "AB_UI_LEADER_BOARD_DETAIL", "AB_UI_LEADER_BOARD_DETAIL_REWARD_INFO_SLOT_CS");
			if (newInstance2 == null)
			{
				return null;
			}
			newInstance2.transform.localScale = Vector3.one;
			m_lstGauntletSlot.Add(newInstance2);
			return newInstance2.GetComponent<RectTransform>();
		}
		Log.Error($"Invalid SlotType - {m_RewardSlotType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/FierceBattleSupport/NKCUIPopupFierceBattleRewardInfo.cs", 418);
		return null;
	}

	public void ReturnSlot(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
		m_slotPool.Push(tr.GetComponent<RectTransform>());
	}

	public void ProvideData(Transform tr, int index)
	{
		if (m_RewardSlotType == REWARD_SLOT_TYPE.Normal)
		{
			NKCUIFierceBattleRewardInfoSlot component = tr.GetComponent<NKCUIFierceBattleRewardInfoSlot>();
			if (component != null && m_lstRankRewardData.Count > index)
			{
				component.SetData(m_lstRankRewardData[index]);
			}
		}
		else
		{
			if (m_RewardSlotType != REWARD_SLOT_TYPE.GauntletLeague)
			{
				return;
			}
			NKCPopupGauntletLGSlot component2 = tr.GetComponent<NKCPopupGauntletLGSlot>();
			if (!(component2 != null) || m_lstRankRewardData.Count <= index)
			{
				return;
			}
			switch ((RankUIRewardData.REWARD_UI_TYPE)Enum.Parse(typeof(RankUIRewardData.REWARD_UI_TYPE), m_lstRankRewardData[index].RankDescStrID))
			{
			case RankUIRewardData.REWARD_UI_TYPE.PVP_RANK:
			{
				NKMLeaguePvpRankGroupTemplet nKMLeaguePvpRankGroupTemplet = NKMLeaguePvpRankGroupTemplet.Find(m_lstRankRewardData[index].RankRewardID);
				if (nKMLeaguePvpRankGroupTemplet != null)
				{
					List<NKMLeaguePvpRankTemplet> list = nKMLeaguePvpRankGroupTemplet.List.ToList();
					if (list.Count > index)
					{
						int index2 = list.Count - 1 - index;
						bool flag = NKCScenManager.CurrentUserData().m_LeagueData.WinCount + NKCScenManager.CurrentUserData().m_LeagueData.LoseCount > 0;
						component2.SetUI(bSeason: true, list[index2], flag && list[index2] == nKMLeaguePvpRankGroupTemplet.GetByTier(NKCScenManager.CurrentUserData().m_LeagueData.LeagueTierID));
					}
				}
				break;
			}
			case RankUIRewardData.REWARD_UI_TYPE.PVP_LEAGUE_SEASON:
			{
				NKMLeaguePvpRankSeasonRewardGroupTemplet nKMLeaguePvpRankSeasonRewardGroupTemplet = NKMLeaguePvpRankSeasonRewardGroupTemplet.Find(m_lstRankRewardData[index].RankRewardID);
				if (nKMLeaguePvpRankSeasonRewardGroupTemplet != null)
				{
					List<NKMLeaguePvpRankSeasonRewardTemplet> rewardTempletList = nKMLeaguePvpRankSeasonRewardGroupTemplet.GetRewardTempletList();
					if (rewardTempletList.Count > index)
					{
						component2.SetData(rewardTempletList[index]);
					}
				}
				break;
			}
			}
		}
	}
}
