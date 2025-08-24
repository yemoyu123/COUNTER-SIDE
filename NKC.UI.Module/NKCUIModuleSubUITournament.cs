using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using Cs.Core.Util;
using Cs.Logging;
using NKC.UI.Component;
using NKC.UI.Fierce;
using NKM;
using NKM.Event;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Module;

public class NKCUIModuleSubUITournament : NKCUIModuleSubUIBase
{
	public class EventModuleDataTouranment : NKCUIModuleHome.EventModuleMessageDataBase
	{
		public int tournamentid;

		public bool bOpenRank;

		public bool bOpenResult;

		public int hitCount;

		public NKMRewardData predictionRewardData;

		public NKMTournamentGroups groupIndex;

		public int winData;

		public NKMRewardData rankRewardData;
	}

	[Header("\ufffd»\ufffd\ufffd \ufffdؽ\ufffdƮ")]
	public Text m_lbTitle;

	public TMP_Text m_lbTotalDate;

	public TMP_Text m_lbApplyDate;

	[Header("\ufffd\ufffd")]
	public Text m_lbRule;

	[Header("\ufffd\ufffd\ufffd")]
	public GameObject m_objArcade;

	public NKCUIComStateButton m_btnArcade;

	public NKCUIComStateButton m_btnInfo;

	public NKCUIComStateButton m_btnResult;

	[Header("\ufffd\ufffd\ufffdϴ\ufffd")]
	public GameObject m_objTopRank;

	public NKCUILeaderBoardSlot m_slotTopRank;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public NKCUIComStateButton m_btnBan;

	public GameObject m_objBanTime;

	public TMP_Text m_lbBanTime;

	public NKCUIComStateButton m_btnApply;

	public GameObject m_objApplyComplete;

	public GameObject m_objApplyRemainTime;

	public TMP_Text m_lbApplyRemainTime;

	public NKCUIComStateButton m_btnEnter;

	public GameObject m_objState;

	public NKCComTMPUIText m_lbState;

	[Header("\ufffd\ufffd\ufffdϴ\ufffd")]
	public NKCUIComStateButton m_btnRank;

	public NKCUIComStateButton m_btnReward;

	public GameObject m_objUserCount;

	public Text m_lbUserCount;

	private NKMTournamentTemplet m_NKMTournamentTemplet;

	private Dictionary<int, List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData>> m_dicRewardData = new Dictionary<int, List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData>>();

	private List<string> m_lstRewardTabStrID = new List<string>();

	private List<string> m_lstRewardTitleStrID = new List<string>();

	private List<string> m_lstRewardDescStrID = new List<string>();

	private NKCPopupImage m_NKCPopupImage;

	private DateTime m_tLastCheckTime;

	private DateTime m_tBanEndTime;

	private DateTime m_tApplyEndTime;

	private bool m_bUpdateBanTime;

	private bool m_bUpdateApplyTime;

	private bool m_bWaitForReward;

	private NKCUIModuleSubUITournamentRank m_NKCUIModuleSubUITournamentRank;

	private float m_fDeltaTime;

	public override void Init()
	{
		base.Init();
		if (m_btnRank != null)
		{
			m_btnRank.PointerClick.RemoveAllListeners();
			m_btnRank.PointerClick.AddListener(OnClickRank);
		}
		if (m_btnReward != null)
		{
			m_btnReward.PointerClick.RemoveAllListeners();
			m_btnReward.PointerClick.AddListener(OnClickReward);
		}
		if (m_btnApply != null)
		{
			m_btnApply.PointerClick.RemoveAllListeners();
			m_btnApply.PointerClick.AddListener(OnClickApply);
			m_btnApply.m_bGetCallbackWhileLocked = true;
		}
		if (m_btnInfo != null)
		{
			m_btnInfo.PointerClick.RemoveAllListeners();
			m_btnInfo.PointerClick.AddListener(OnClickInfo);
		}
		if (m_btnEnter != null)
		{
			m_btnEnter.PointerClick.RemoveAllListeners();
			m_btnEnter.PointerClick.AddListener(OnClickEnter);
			m_btnEnter.m_bGetCallbackWhileLocked = true;
		}
		if (m_btnResult != null)
		{
			m_btnResult.PointerClick.RemoveAllListeners();
			m_btnResult.PointerClick.AddListener(OnClickResult);
		}
		if (m_btnBan != null)
		{
			m_btnBan.PointerClick.RemoveAllListeners();
			m_btnBan.PointerClick.AddListener(OnClickBan);
		}
		if (m_btnArcade != null)
		{
			m_btnArcade.PointerClick.RemoveAllListeners();
			m_btnArcade.PointerClick.AddListener(OnClickArcade);
		}
	}

	public override void OnClose()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_dicRewardData.Clear();
		m_lstRewardTabStrID.Clear();
		m_lstRewardTitleStrID.Clear();
		m_lstRewardDescStrID.Clear();
		m_NKCPopupImage = null;
		m_NKCUIModuleSubUITournamentRank = null;
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		ModuleID = templet.Key;
		NKCTournamentManager.SetEventCollectionIndexId(ModuleID);
		m_tLastCheckTime = ServiceTime.Now;
		m_bWaitForReward = false;
		int intValue = NKCUtil.GetIntValue(templet.m_Option, "TournamentTempletID", 0);
		m_NKMTournamentTemplet = NKMTournamentTemplet.Find(intValue);
		if (m_NKMTournamentTemplet == null)
		{
			Log.Error($"NKMTournamentTemplet is null - tournamentTempletID : {intValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCUIModuleSubUITournament.cs", 158);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		}
		else if (NKCTournamentManager.m_TournamentInfoChanged)
		{
			NKCPacketSender.Send_NKMPacket_TOURNAMENT_INFO_REQ();
		}
		else
		{
			SetData();
		}
	}

	private void SetData()
	{
		if (ServiceTime.Now > NKCTournamentManager.GetTournamentStateStartDate(NKMTournamentState.Closing) && !NKCTournamentManager.m_bRankInfoReceived)
		{
			NKCPacketSender.Send_NKMPacket_TOURNAMENT_RANK_REQ();
		}
		if (NKCTournamentManager.CanRecvReward() && !m_bWaitForReward)
		{
			m_bWaitForReward = true;
			NKCPacketSender.Send_NKMPacket_TOURNAMENT_REWARD_REQ();
		}
		SetButtonState();
		SetText();
		SetButtonState();
		NKCUtil.SetGameobjectActive(m_objArcade, NKCEventPvpMgr.CanAccessEventPvp() && NKCEventPvpMgr.IsTournamentPractice());
		NKCUtil.SetGameobjectActive(m_objTopRank, NKCTournamentManager.GetTournamentState() == NKMTournamentState.Closing);
		if (m_objTopRank.activeSelf)
		{
			m_slotTopRank.SetData(NKCTournamentManager.GetCurSeasonTopRank(), NKCTournamentManager.TournamentId, null, bUsePercentRank: false, bShowMyRankIcon: false);
		}
		m_tLastCheckTime = ServiceTime.Now;
	}

	private void SetButtonState()
	{
		NKCUtil.SetGameobjectActive(m_btnBan, NKCTournamentManager.m_TournamentTemplet.UseCastingBan);
		if (NKCTournamentManager.m_TournamentTemplet.UseCastingBan)
		{
			if (ServiceTime.Now > NKCTournamentManager.m_TournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.BanVote))
			{
				m_bUpdateBanTime = false;
				NKCUtil.SetGameobjectActive(m_objBanTime, bValue: false);
			}
			else
			{
				m_fDeltaTime = 0f;
				m_bUpdateBanTime = true;
				m_tBanEndTime = NKCTournamentManager.m_TournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.BanVote);
				NKCUtil.SetGameobjectActive(m_objBanTime, bValue: true);
				UpdateBanTime();
			}
		}
		else
		{
			m_bUpdateBanTime = false;
		}
		if (NKCTournamentManager.GetTournamentState() == NKMTournamentState.PreBooking && m_NKMTournamentTemplet.CanEnterDeckApply(ServiceTime.Now))
		{
			m_fDeltaTime = 0f;
			m_bUpdateApplyTime = true;
			m_tApplyEndTime = NKCTournamentManager.m_TournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.PreBooking);
			NKCUtil.SetGameobjectActive(m_objApplyRemainTime, bValue: true);
			m_btnApply.UnLock();
		}
		else
		{
			m_bUpdateApplyTime = false;
			m_tApplyEndTime = DateTime.MinValue;
			NKCUtil.SetGameobjectActive(m_objApplyRemainTime, bValue: false);
			m_btnApply.Lock();
		}
		NKCUtil.SetGameobjectActive(m_objApplyComplete, NKCTournamentManager.m_TournamentApply);
		m_btnEnter.UnLock();
		NKCUtil.SetLabelText(m_lbState, GetStateDesc());
		NKCUtil.SetGameobjectActive(m_objUserCount, !m_NKMTournamentTemplet.IsUnify);
		if (m_objUserCount != null && m_objUserCount.activeInHierarchy)
		{
			if (NKCTournamentManager.m_TournamentInfo != null)
			{
				NKCUtil.SetLabelText(m_lbUserCount, NKCTournamentManager.m_TournamentInfo.userCount.ToString("N0"));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbUserCount, "-");
			}
		}
		NKCUtil.SetGameobjectActive(m_objTopRank, NKCTournamentManager.GetTournamentState() == NKMTournamentState.Closing && NKCTournamentManager.m_bRankInfoReceived);
		if (m_objTopRank.activeSelf)
		{
			m_slotTopRank.SetData(NKCTournamentManager.GetCurSeasonTopRank(), NKCTournamentManager.TournamentId, null, bUsePercentRank: false, bShowMyRankIcon: false);
		}
		NKCUtil.SetGameobjectActive(m_btnResult, NKCTournamentManager.GetTournamentState() == NKMTournamentState.Closing);
	}

	private string GetStateDesc()
	{
		switch (NKCTournamentManager.GetTournamentState())
		{
		case NKMTournamentState.Ended:
			return "";
		case NKMTournamentState.BanVote:
			return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_BAN;
		case NKMTournamentState.PreBooking:
			return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_DECK;
		case NKMTournamentState.Tryout:
			return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_QUALIFY;
		case NKMTournamentState.Final32:
			if (ServiceTime.Now < m_NKMTournamentTemplet.GetGroupCheeringEndTime(NKMTournamentGroups.GroupA))
			{
				return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_A;
			}
			if (ServiceTime.Now < m_NKMTournamentTemplet.GetGroupCheeringEndTime(NKMTournamentGroups.GroupB))
			{
				return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_B;
			}
			if (ServiceTime.Now < m_NKMTournamentTemplet.GetGroupCheeringEndTime(NKMTournamentGroups.GroupC))
			{
				return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_C;
			}
			if (ServiceTime.Now < m_NKMTournamentTemplet.GetGroupCheeringEndTime(NKMTournamentGroups.GroupD))
			{
				return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_GROUP_D;
			}
			return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_FINAL;
		case NKMTournamentState.Final4:
			if (ServiceTime.Now < m_NKMTournamentTemplet.GetGroupCheeringEndTime(NKMTournamentGroups.Finals))
			{
				return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_FINAL;
			}
			return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_END;
		case NKMTournamentState.Closing:
			return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_END;
		case NKMTournamentState.Progressing:
			if (ServiceTime.Now < m_NKMTournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.Tryout))
			{
				return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_QUALIFY;
			}
			if (ServiceTime.Now < m_NKMTournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.Final32))
			{
				return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_GROUP;
			}
			if (ServiceTime.Now < m_NKMTournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.Final4))
			{
				return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_FINAL;
			}
			if (ServiceTime.Now < m_NKMTournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.Closing))
			{
				return NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_INFO_READY_END;
			}
			break;
		default:
			Log.Error($"state : {NKCTournamentManager.GetTournamentState()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCUIModuleSubUITournament.cs", 318);
			break;
		}
		return "";
	}

	private void SetText()
	{
		NKCUtil.SetLabelText(m_lbRule, m_NKMTournamentTemplet.GetTournamentSeasonDesc());
		NKCUtil.SetLabelText(m_lbTotalDate, string.Format(NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL, m_NKMTournamentTemplet.IntervalTemplet.GetStartDate().ToString("yyyy-MM-dd HH:mm"), m_NKMTournamentTemplet.IntervalTemplet.GetEndDate().ToString("yyyy-MM-dd HH:mm")));
		NKCUtil.SetLabelText(m_lbApplyDate, string.Format(NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_INTERVAL_DECK_ENTER, m_NKMTournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.PreBooking).ToString("yyyy-MM-dd HH:mm"), m_NKMTournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.PreBooking).ToString("yyyy-MM-dd HH:mm")));
	}

	public override void Refresh()
	{
		SetData();
	}

	private void OnClickRank()
	{
		if (NKCTournamentManager.m_bRankInfoReceived && NKCTournamentManager.GetTournamentState() == NKCTournamentManager.m_LastRankReceivedState)
		{
			if (NKCTournamentManager.m_lstLeaderBoardSlotData.Count > 0)
			{
				m_NKCUIModuleSubUITournamentRank = NKCUIModuleSubUITournamentRank.OpenInstance("UI_SINGLE_TOURNAMENT", "UI_SINGLE_POPUP_TOURNAMENT_RANK");
				m_NKCUIModuleSubUITournamentRank.Open(NKCTournamentManager.m_lstLeaderBoardSlotData, LeaderBoardType.BT_TOURNAMENT);
			}
			else
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GE_STRING_TOURNAMENT_HOF_NO_RECORD, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
		}
		else if (NKCTournamentManager.GetTournamentState() != NKCTournamentManager.m_LastRankReceivedState)
		{
			NKCPacketSender.Send_NKMPacket_TOURNAMENT_RANK_REQ();
		}
		else if (NKCTournamentManager.m_lstLeaderBoardSlotData.Count == 0)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GE_STRING_TOURNAMENT_HOF_NO_RECORD, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
	}

	private void OnClickReward()
	{
		if (m_dicRewardData.Count == 0)
		{
			SetRewardData();
		}
		NKCPopupRewardInfoToggle.Instance.Open(NKCUIPopupFierceBattleRewardInfo.REWARD_SLOT_TYPE.Normal, m_dicRewardData, m_lstRewardTabStrID, m_lstRewardTitleStrID, m_lstRewardDescStrID, bShowTitle: false, bShowDesc: false, bShowMyRank: false);
	}

	private void SetRewardData()
	{
		m_dicRewardData.Clear();
		List<NKMTournamentPredictRewardTemplet> list = NKCTournamentManager.m_TournamentTemplet.PredictRewardGroupTemplet.RewardTemplets.OrderBy((NKMTournamentPredictRewardTemplet e) => e.index).ToList();
		List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData> list2 = new List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData>();
		for (int num = 0; num < list.Count; num++)
		{
			list2.Add(new NKCUIPopupFierceBattleRewardInfo.RankUIRewardData(list[num]));
		}
		m_dicRewardData.Add(0, list2);
		List<NKMTournamentRankRewardTemplet> list3 = NKCTournamentManager.m_TournamentTemplet.rankRewardTemplets.OrderBy((NKMTournamentRankRewardTemplet e) => e.Key).ToList();
		List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData> list4 = new List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData>();
		for (int num2 = 0; num2 < list3.Count; num2++)
		{
			list4.Add(new NKCUIPopupFierceBattleRewardInfo.RankUIRewardData(list3[num2]));
		}
		m_dicRewardData.Add(1, list4);
		m_lstRewardTabStrID.Clear();
		m_lstRewardTitleStrID.Clear();
		m_lstRewardDescStrID.Clear();
		m_lstRewardTabStrID.Add("SI_PF_TOURNAMENT_REWARD_PREDICT_TITLE");
		m_lstRewardTabStrID.Add("SI_PF_TOURNAMENT_REWARD_RANK_TITLE");
	}

	private void OnClickApply()
	{
		if (ServiceTime.Now < NKCTournamentManager.m_TournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.PreBooking))
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_BAN_TIME, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKCTournamentManager.LoadLastDeckData();
		NKCUIDeckViewer.DeckViewerOption options = default(NKCUIDeckViewer.DeckViewerOption);
		options.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.TournamentApply;
		options.DeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_TOURNAMENT, 0);
		options.dOnBackButton = null;
		options.bEnableDefaultBackground = true;
		options.bNoUseLeaderBtn = false;
		options.bSlot24Extend = false;
		options.bUpsideMenuHomeButton = false;
		options.bUsableOperationSkip = false;
		options.bUsableSupporter = false;
		options.bUseAsyncDeckSetting = true;
		options.CostItemID = 0;
		options.CostItemCount = 0;
		options.dOnSideMenuButtonConfirm = OnApplyDeck;
		options.SelectLeaderUnitOnOpen = true;
		options.dOnBackButton = OnApplyCancel;
		options.DeckListButtonStateText = NKCUtilString.GET_STRING_TOURNAMENT_DECK_ENTER_TITLE;
		NKCUIDeckViewer.Instance.Open(options);
	}

	private void OnClickEnter()
	{
		NKCUIModuleSubUITournamentLobby.Instance.Open();
	}

	public void OpenPrevTournamentUI()
	{
		if (NKCTournamentManager.m_replayTournamentGroup == NKMTournamentGroups.None)
		{
			NKCUIModuleSubUITournamentLobby.Instance.OpenTryout();
		}
		else
		{
			OnClickEnter();
		}
	}

	public void OnApplyDeck(NKMDeckIndex selectedDeckIndex, long supportUserUID = 0L)
	{
		if (m_NKMTournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.PreBooking) < ServiceTime.Now)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_TIMEOUT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		if (!NKCScenManager.CurrentArmyData().GetDeckData(selectedDeckIndex).CheckAllSlotFilled())
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_EMPTY, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		if (NKCScenManager.CurrentArmyData().GetArmyAvarageOperationPower(selectedDeckIndex) < NKMCommonConst.TournamentMinimumDeckCP)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(string.Format(NKCUtilString.GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_MINIMUM_CP, NKMCommonConst.TournamentMinimumDeckCP), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TOURNAMENT_DECK_ENTER_POPUP, delegate
		{
			NKCUIPopupTournamentDeckChange.Instance.Open(OnApplyConfirm, NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_CHANGE);
		});
	}

	private void OnApplyCancel()
	{
		NKCTournamentManager.LoadLastDeckData();
		NKCUIDeckViewer.Instance.Close();
	}

	private void OnApplyConfirm()
	{
		NKCPacketSender.Send_NKMPacket_TOURNAMENT_APPLY_REQ(NKCScenManager.CurrentArmyData().GetDeckData(NKM_DECK_TYPE.NDT_TOURNAMENT, 0));
		NKCUIDeckViewer.Instance.Close();
	}

	private void OnClickInfo()
	{
		m_NKCPopupImage = NKCPopupImage.OpenInstance("UI_SINGLE_TOURNAMENT", "UI_SINGLE_POPUP_TOURNAMENT_TIMEINFO");
		m_NKCPopupImage.Open(NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_GUIDE, "", m_NKMTournamentTemplet.UseCastingBan);
	}

	private void OnClickResult()
	{
		NKMTournamentTemplet tournamentTemplet = NKCTournamentManager.m_TournamentTemplet;
		if (tournamentTemplet != null)
		{
			if (NKCTournamentManager.m_TournamentRewardInfo == null)
			{
				NKCPacketSender.Send_NKMPacket_TOURNAMENT_REWARD_INFO_REQ();
			}
			else
			{
				NKCUIModuleSubUITournamentReward.Instance.Open(tournamentTemplet.GetTournamentSeasonTitle(), NKCTournamentManager.m_TournamentRewardInfo.hitCount, NKCTournamentManager.m_TournamentRewardInfo.predictionRewardData, NKCTournamentManager.m_TournamentRewardInfo.winData, NKCTournamentManager.m_TournamentRewardInfo.rankRewardData, NKCTournamentManager.m_TournamentRewardInfo.groupIndex);
			}
		}
	}

	private void OnClickBan()
	{
		NKCPopupTournamentBan.Instance.Open();
	}

	private void OnClickArcade()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_EVENT_READY);
	}

	public override void PassData(NKCUIModuleHome.EventModuleMessageDataBase data)
	{
		if (!(data is EventModuleDataTouranment eventModuleDataTouranment))
		{
			return;
		}
		if (eventModuleDataTouranment.bOpenResult)
		{
			NKMTournamentTemplet nKMTournamentTemplet = NKMTournamentTemplet.Find(eventModuleDataTouranment.tournamentid);
			if (nKMTournamentTemplet != null)
			{
				NKCUIModuleSubUITournamentReward.Instance.Open(nKMTournamentTemplet.GetTournamentSeasonTitle(), eventModuleDataTouranment.hitCount, eventModuleDataTouranment.predictionRewardData, eventModuleDataTouranment.winData, eventModuleDataTouranment.rankRewardData, eventModuleDataTouranment.groupIndex);
			}
		}
		if (eventModuleDataTouranment.bOpenRank)
		{
			NKCUtil.SetGameobjectActive(m_objTopRank, NKCTournamentManager.GetTournamentState() == NKMTournamentState.Closing && NKCTournamentManager.m_bRankInfoReceived);
			if (m_objTopRank.activeSelf)
			{
				m_slotTopRank.SetData(NKCTournamentManager.GetCurSeasonTopRank(), NKCTournamentManager.TournamentId, null, bUsePercentRank: false, bShowMyRankIcon: false);
			}
			m_NKCUIModuleSubUITournamentRank = NKCUIModuleSubUITournamentRank.OpenInstance("UI_SINGLE_TOURNAMENT", "UI_SINGLE_POPUP_TOURNAMENT_RANK");
			m_NKCUIModuleSubUITournamentRank.Open(NKCTournamentManager.m_lstLeaderBoardSlotData, LeaderBoardType.BT_TOURNAMENT);
		}
	}

	private void UpdateBanTime()
	{
		NKCUtil.SetLabelText(m_lbBanTime, string.Format(NKCUtilString.GET_STRING_GAUNTLET_THIS_WEEK_LEAGUE_CASTING_BEN_ONE_PARAM, NKCUtilString.GetRemainTimeStringEx(ServiceTime.ToUtcTime(m_tBanEndTime))));
	}

	private void UpdateApplyTime()
	{
		NKCUtil.SetLabelText(m_lbApplyRemainTime, NKCUtilString.GetRemainTimeStringEx(ServiceTime.ToUtcTime(m_tApplyEndTime)));
	}

	private void Update()
	{
		m_fDeltaTime += Time.deltaTime;
		if (!(m_fDeltaTime > 1f))
		{
			return;
		}
		m_fDeltaTime -= 1f;
		if (m_bUpdateBanTime || m_bUpdateApplyTime)
		{
			if (NKCTournamentManager.m_TournamentInfoChanged || (m_tLastCheckTime.AddMinutes(1.0) <= ServiceTime.Now && !m_NKMTournamentTemplet.IsUnify))
			{
				NKCTournamentManager.SetTournamentInfoChanged(bChanged: false);
				NKCPacketSender.Send_NKMPacket_TOURNAMENT_INFO_REQ();
			}
			if (m_bUpdateBanTime)
			{
				UpdateBanTime();
			}
			if (m_bUpdateApplyTime)
			{
				UpdateApplyTime();
			}
		}
	}
}
