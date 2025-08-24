using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Core.Util;
using Cs.Logging;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLeagueGlobalBan : NKCUIBase
{
	[Serializable]
	public class UserInfo
	{
		public NKCUILeagueTier TierIcon;

		public Text TierScore;

		public Text Level;

		public Text Name;

		public GameObject Guild;

		public NKCUIGuildBadge GuildBadge;

		public Text GuildName;

		public GameObject FirstPick;

		public GameObject BanProgress;

		public GameObject BanFinished;

		public void SetData(NKMUserProfileData userProfileData, bool bFirstPick, bool isMyTeam, bool isPrivatePVP)
		{
			int num = NKCPVPManager.FindPvPSeasonID(NKM_GAME_TYPE.NGT_PVP_LEAGUE, NKCSynchronizedTime.GetServerUTCTime());
			if (!NKMLeaguePvpRankTemplet.FindByTier(num, userProfileData.leaguePvpData.leagueTierId, out var templet))
			{
				Log.Error($"NKMLeaguePvpRankTemplet is null - seasonID : {num}, tierID : {userProfileData.leaguePvpData.leagueTierId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueGlobalBan.cs", 72);
			}
			if (TierIcon != null)
			{
				TierIcon.SetUI(templet);
			}
			NKCUtil.SetLabelText(TierScore, userProfileData.leaguePvpData.score.ToString());
			if (isPrivatePVP)
			{
				NKCUtil.SetGameobjectActive(TierIcon?.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(TierScore?.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(TierScore?.transform.parent?.gameObject, bValue: false);
			}
			NKCUtil.SetLabelText(Level, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, userProfileData.commonProfile.level);
			NKCUtil.SetLabelText(Name, NKCUtilString.GetUserNickname(userProfileData.commonProfile.nickname, !isMyTeam));
			bool bValue = true;
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD) || userProfileData.guildData == null || userProfileData.guildData.guildUid == 0L)
			{
				bValue = false;
			}
			NKCUtil.SetGameobjectActive(Guild, bValue);
			if (Guild != null && Guild.activeSelf)
			{
				GuildBadge.SetData(userProfileData.guildData.badgeId, !isMyTeam);
				NKCUtil.SetLabelText(GuildName, NKCUtilString.GetUserGuildName(userProfileData.guildData.guildName, !isMyTeam));
			}
			NKCUtil.SetGameobjectActive(FirstPick, bFirstPick);
			SetBanProgressFinished(bFinished: false);
		}

		public void SetBanProgressFinished(bool bFinished)
		{
			NKCUtil.SetGameobjectActive(BanProgress, !bFinished);
			NKCUtil.SetGameobjectActive(BanFinished, bFinished);
		}
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_LEAGUE_POPUP_GLOBAL_BAN";

	[SerializeField]
	public UserInfo m_userSlotLeft;

	public UserInfo m_userSlotRight;

	public GameObject m_SelectedObject;

	public Text m_SelectedCount;

	public NKCDeckViewUnitSelectListSlot[] m_SelectedUnitList;

	public GameObject m_CandidateObject;

	public NKCLeaguePvpUnitSelectList m_NKCLeaguePvpUnitSelectList;

	public Animator m_FinalResultAnimator;

	public GameObject m_FinalResultObject;

	public NKCUIUnitSelectListSlot[] m_FinalResultUnitList;

	public Text m_timeText;

	public GameObject m_BanCandidateSearchObject;

	public InputField m_BanCandidateSearchInputField;

	public NKCUIComButton m_BanCandidateSearchButton;

	public NKCUIComButton m_BanSelectConfirm;

	public NKCUIComButton m_LeaveRoom;

	public GameObject m_objWatingNotice;

	[Header("팝업 가이드")]
	public GameObject m_PopupSequenceGuide;

	public Text m_sequenceGuideText;

	public Animator m_animatorsequenceGuide;

	private NKCUIDeckViewer.DeckViewerOption m_ViewerOptions;

	private NKMUnitTempletBase m_currentSelectedUnitTemplet;

	private string m_banCandidateSearchString = "";

	private DateTime m_endTime;

	private int m_prevRemainingSeconds;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override bool IgnoreBackButtonWhenOpen => true;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET;

	public override List<int> UpsideMenuShowResourceList => new List<int> { 101 };

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		return NKCUIManager.OpenNewInstanceAsync<NKCUIGauntletLeagueGlobalBan>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LEAGUE_POPUP_GLOBAL_BAN", NKCUIManager.eUIBaseRect.UIFrontCommon, null);
	}

	public override void CloseInternal()
	{
		m_NKCLeaguePvpUnitSelectList.Close(bAnimate: false);
	}

	public void Init()
	{
		m_prevRemainingSeconds = 0;
		NKCUtil.SetGameobjectActive(m_SelectedObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_CandidateObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_FinalResultObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_BanSelectConfirm, bValue: false);
		NKCUtil.SetGameobjectActive(m_objWatingNotice, bValue: false);
		NKCUtil.SetGameobjectActive(m_LeaveRoom, bValue: false);
		NKCUtil.SetGameobjectActive(m_BanCandidateSearchObject, bValue: false);
		if (m_BanSelectConfirm != null)
		{
			m_BanSelectConfirm.PointerClick.RemoveAllListeners();
			m_BanSelectConfirm.PointerClick.AddListener(OnClickSelectConfirm);
		}
		if (m_LeaveRoom != null)
		{
			m_LeaveRoom.PointerClick.RemoveAllListeners();
			m_LeaveRoom.PointerClick.AddListener(OnClickGiveup);
		}
		if (m_BanCandidateSearchInputField != null)
		{
			Log.Info("[League] BanCandidateSearch", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueGlobalBan.cs", 194);
			m_BanCandidateSearchInputField.onEndEdit.RemoveAllListeners();
			m_BanCandidateSearchInputField.onEndEdit.AddListener(OnEndEditSearch);
		}
		if (m_BanCandidateSearchButton != null)
		{
			m_BanCandidateSearchButton.PointerClick.RemoveAllListeners();
			m_BanCandidateSearchButton.PointerClick.AddListener(OnClickSearch);
		}
		m_ViewerOptions = default(NKCUIDeckViewer.DeckViewerOption);
		m_ViewerOptions.MenuName = "";
		m_ViewerOptions.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.LeaguePvPGlobalBan;
		m_ViewerOptions.dOnSideMenuButtonConfirm = null;
		m_NKCLeaguePvpUnitSelectList.Init(null, null, OnSelectCandidateSlot, null);
		NKCUtil.SetLabelText(m_SelectedCount, $"{0}/{2}");
		NKCDeckViewUnitSelectListSlot[] selectedUnitList = m_SelectedUnitList;
		for (int i = 0; i < selectedUnitList.Length; i++)
		{
			NKCUtil.SetGameobjectActive(selectedUnitList[i], bValue: false);
		}
		UIOpened();
	}

	public void SetEndTime(DateTime endTime)
	{
		m_endTime = endTime;
		m_prevRemainingSeconds = Math.Max(0, Convert.ToInt32((m_endTime - ServiceTime.Now).TotalSeconds));
	}

	public void OpenCandidateList()
	{
		if (!NKCLeaguePVPMgr.IsObserver())
		{
			m_NKCLeaguePvpUnitSelectList.Open(bAnimate: true, NKM_UNIT_TYPE.NUT_NORMAL, MakeSortOptions(), m_ViewerOptions);
		}
		if (NKCLeaguePVPMgr.GetMyDraftTeamData() != null)
		{
			NKCUtil.SetGameobjectActive(m_BanSelectConfirm, bValue: true);
			NKCUtil.SetGameobjectActive(m_LeaveRoom, bValue: true);
			NKCUtil.SetGameobjectActive(m_SelectedObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_CandidateObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_BanCandidateSearchObject, bValue: true);
		}
		RefreshBanProgress();
	}

	private NKCUnitSortSystem.UnitListOptions MakeSortOptions()
	{
		return new NKCUnitSortSystem.UnitListOptions
		{
			eDeckType = NKM_DECK_TYPE.NDT_NONE,
			setExcludeUnitID = null,
			setOnlyIncludeUnitID = null,
			setDuplicateUnitID = null,
			setExcludeUnitUID = null,
			bExcludeLockedUnit = false,
			bExcludeDeckedUnit = false,
			bIgnoreCityState = true,
			bIgnoreWorldMapLeader = true,
			setFilterOption = m_NKCLeaguePvpUnitSelectList.SortOptions.setFilterOption,
			lstSortOption = m_NKCLeaguePvpUnitSelectList.SortOptions.lstSortOption,
			bDescending = m_NKCLeaguePvpUnitSelectList.SortOptions.bDescending,
			bIncludeUndeckableUnit = true,
			bHideDeckedUnit = false,
			bPushBackUnselectable = true
		};
	}

	public void ShowFinalResult()
	{
		RefreshBanProgress();
		NKCUtil.SetGameobjectActive(m_SelectedObject, !NKCLeaguePVPMgr.IsObserver());
		NKCUtil.SetGameobjectActive(m_CandidateObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_BanCandidateSearchObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_BanSelectConfirm, bValue: false);
		NKCUtil.SetGameobjectActive(m_objWatingNotice, bValue: false);
		NKCUtil.SetGameobjectActive(m_LeaveRoom, bValue: false);
		HideSequenceGuidePopup();
		for (int i = 0; i < NKCLeaguePVPMgr.GetLeftDraftTeamData().globalBanUnitIdList.Count; i++)
		{
			NKMUnitTempletBase templetBase = NKMUnitTempletBase.Find(NKCLeaguePVPMgr.GetLeftDraftTeamData().globalBanUnitIdList[i]);
			m_FinalResultUnitList[i].SetDataForBan(templetBase, bEnableLayoutElement: true, null);
		}
		for (int j = 0; j < NKCLeaguePVPMgr.GetRightDraftTeamData().globalBanUnitIdList.Count; j++)
		{
			NKMUnitTempletBase templetBase2 = NKMUnitTempletBase.Find(NKCLeaguePVPMgr.GetRightDraftTeamData().globalBanUnitIdList[j]);
			m_FinalResultUnitList[j + 2].SetDataForBan(templetBase2, bEnableLayoutElement: true, null);
		}
		NKCUtil.SetGameobjectActive(m_FinalResultObject, bValue: true);
		if (m_FinalResultAnimator != null)
		{
			m_FinalResultAnimator.Play("BAN_LIST_INTRO");
		}
		NKCSoundManager.PlaySound("FX_UI_TITLE_IN_TEST", 1f, 0f, 0f);
	}

	public void ProcessBackButton()
	{
	}

	private void UpdateUserInfo()
	{
		bool flag = NKCLeaguePVPMgr.GetLeftDraftTeamData().teamType == NKM_TEAM_TYPE.NTT_A1;
		m_userSlotLeft.SetData(NKCLeaguePVPMgr.GetLeftDraftTeamData().userProfileData, flag, isMyTeam: true, NKCLeaguePVPMgr.IsPrivate());
		m_userSlotRight.SetData(NKCLeaguePVPMgr.GetRightDraftTeamData().userProfileData, !flag, isMyTeam: false, NKCLeaguePVPMgr.IsPrivate());
	}

	private void RefreshBanCandidates()
	{
		if (m_NKCLeaguePvpUnitSelectList != null && m_NKCLeaguePvpUnitSelectList.m_LoopScrollRect != null)
		{
			m_NKCLeaguePvpUnitSelectList.m_LoopScrollRect.RefreshCells();
		}
	}

	private void OnSelectCandidateSlot(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		Debug.Log("[League] OnSelectCandidateSlot : " + unitTempletBase.m_UnitStrID);
		SelectUnit(unitTempletBase);
	}

	public void OnClickSelectConfirm()
	{
		if (m_currentSelectedUnitTemplet != null)
		{
			NKCLeaguePVPMgr.SelectGlobalBanUnit(m_currentSelectedUnitTemplet);
		}
	}

	public void OnClickGiveup()
	{
		if (!NKCLeaguePVPMgr.CanLeaveRoom())
		{
			return;
		}
		if (NKCLeaguePVPMgr.DraftRoomData == null)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_GIVEUP_POPUP"), delegate
			{
				NKCLeaguePVPMgr.Send_NKMPacket_LEAGUE_PVP_GIVEUP_REQ();
			});
		}
		else
		{
			if (NKCLeaguePVPMgr.DraftRoomData == null)
			{
				return;
			}
			switch (NKCLeaguePVPMgr.DraftRoomData.gameType)
			{
			case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
				if (NKCLeaguePVPMgr.IsObserver())
				{
					NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_PRIVATE_PVP_OBSERVE_EXIT, delegate
					{
						NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_EXIT_REQ();
					});
				}
				else
				{
					NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_PRIVATE_PVP_READY_CANCEL_TITLE, NKCUtilString.GET_STRING_PRIVATE_PVP_READY_CANCEL, delegate
					{
						NKCPacketSender.Send_NKMPacket_PRIVATE_PVP_DRAFT_GIVEUP_REQ();
					});
				}
				break;
			case NKM_GAME_TYPE.NGT_PVP_EVENT:
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_PRIVATE_PVP_READY_CANCEL_TITLE, NKCUtilString.GET_STRING_PRIVATE_PVP_READY_CANCEL, delegate
				{
					NKCLeaguePVPMgr.Send_NKMPacket_EVENT_PVP_EXIT_REQ();
				});
				break;
			default:
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_GIVEUP_POPUP"), delegate
				{
					NKCLeaguePVPMgr.Send_NKMPacket_LEAGUE_PVP_GIVEUP_REQ();
				});
				break;
			}
		}
	}

	public void SetLeaveRoomState()
	{
		NKCUtil.SetGameobjectActive(m_LeaveRoom, NKCLeaguePVPMgr.CanLeaveRoom());
	}

	public void BanSelectedUnit()
	{
		if (m_currentSelectedUnitTemplet != null)
		{
			DeSelectUnit(m_currentSelectedUnitTemplet.m_UnitID);
		}
		m_currentSelectedUnitTemplet = null;
	}

	public void RefreshMyBanUnitList(List<int> banUnitList, int oponentTeamBanCount)
	{
		int count = banUnitList.Count;
		NKCUtil.SetLabelText(m_SelectedCount, $"{count}/{2}");
		if (count == 2 && oponentTeamBanCount < 2)
		{
			ShowSequenceGuidePopup(NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_OPPONENT"));
		}
		for (int i = 0; i < banUnitList.Count; i++)
		{
			if (m_SelectedUnitList[i].NKMUnitTempletBase == null)
			{
				NKMUnitTempletBase templetBase = NKMUnitTempletBase.Find(banUnitList[i]);
				m_SelectedUnitList[i].SetData(templetBase, 0, bEnableLayoutElement: false, null);
				NKCUtil.SetGameobjectActive(m_SelectedUnitList[i], bValue: true);
			}
		}
	}

	public void RefreshBanProgress()
	{
		SetEndTime(NKCLeaguePVPMgr.DraftRoomData.stateEndTime);
		UpdateUserInfo();
		if (NKCLeaguePVPMgr.IsObserver())
		{
			ShowSequenceGuidePopup(NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_BAN_OBSERVER"));
		}
		m_userSlotLeft.SetBanProgressFinished(NKCLeaguePVPMgr.GetLeftDraftTeamData().globalBanUnitIdList.Count >= 2);
		m_userSlotRight.SetBanProgressFinished(NKCLeaguePVPMgr.GetRightDraftTeamData().globalBanUnitIdList.Count >= 2);
		if (NKCLeaguePVPMgr.GetMyDraftTeamData() != null)
		{
			bool flag = NKCLeaguePVPMgr.GetMyDraftTeamData().globalBanUnitIdList.Count < 2;
			NKCUtil.SetGameobjectActive(m_BanSelectConfirm, flag);
			NKCUtil.SetGameobjectActive(m_LeaveRoom, flag);
			NKCUtil.SetGameobjectActive(m_objWatingNotice, !flag);
		}
		RefreshBanCandidates();
	}

	private void Update()
	{
		if (m_prevRemainingSeconds > 0)
		{
			int num = Math.Max(0, Convert.ToInt32((m_endTime - ServiceTime.Now).TotalSeconds));
			if (num != m_prevRemainingSeconds)
			{
				NKCUtil.SetLabelText(m_timeText, num.ToString());
				m_prevRemainingSeconds = num;
			}
		}
	}

	private void SelectUnit(NKMUnitTempletBase unitTempletBase)
	{
		if (m_currentSelectedUnitTemplet != null)
		{
			DeSelectUnit(m_currentSelectedUnitTemplet.m_UnitID);
		}
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = m_NKCLeaguePvpUnitSelectList.FindSlotFromCurrentList(unitTempletBase.m_UnitID);
		if (nKCUIUnitSelectListSlotBase != null)
		{
			Debug.Log($"[League] FindSlotFromCurrentList : {unitTempletBase.m_UnitID}");
			nKCUIUnitSelectListSlotBase.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.SELECTED);
			m_currentSelectedUnitTemplet = unitTempletBase;
		}
	}

	private void DeSelectUnit(int unitID)
	{
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = m_NKCLeaguePvpUnitSelectList.FindSlotFromCurrentList(unitID);
		if (nKCUIUnitSelectListSlotBase != null)
		{
			nKCUIUnitSelectListSlotBase.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.NONE);
		}
	}

	public void OnEndEditSearch(string inputText)
	{
		if (NKCInputManager.IsChatSubmitEnter())
		{
			Log.Info("[League] SearchEnter", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueGlobalBan.cs", 493);
			if (!m_BanCandidateSearchButton.m_bLock)
			{
				OnClickSearch();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	private void OnClickSearch()
	{
		if (m_BanCandidateSearchInputField != null)
		{
			m_banCandidateSearchString = m_BanCandidateSearchInputField.text;
		}
		Log.Info("[League] UpdateSearchString", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueGlobalBan.cs", 510);
		m_NKCLeaguePvpUnitSelectList.UpdateSearchString(m_banCandidateSearchString);
		RefreshBanCandidates();
	}

	public void ShowSequenceGuidePopup(string text)
	{
		NKCUtil.SetGameobjectActive(m_PopupSequenceGuide, bValue: true);
		NKCUtil.SetLabelText(m_sequenceGuideText, text);
		if (m_animatorsequenceGuide != null)
		{
			m_animatorsequenceGuide.Play("NKM_UI_GAUNTLET_LEAGUE_SEQUENCE_GUIDE_INTRO");
		}
	}

	public void HideSequenceGuidePopup()
	{
		if (m_animatorsequenceGuide != null)
		{
			m_animatorsequenceGuide.Play("NKM_UI_GAUNTLET_LEAGUE_SEQUENCE_GUIDE_OUTRO");
		}
	}
}
