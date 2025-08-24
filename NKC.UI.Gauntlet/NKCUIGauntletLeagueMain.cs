using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Pvp;
using Cs.Core.Util;
using Cs.Logging;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLeagueMain : NKCUIBase
{
	[Serializable]
	public class LeagueUserInfoUI
	{
		public delegate void OnClickPickedUnit(int index);

		public NKCDeckViewShip DeckViewShip;

		public NKCUIDeckViewOperator DeckViewOperator;

		public NKCUIOperatorDeckSlot OperatorSlot;

		public GameObject OperatorEmpty;

		public GameObject OperatorSkillInfo;

		public NKCUIOperatorSkill OperatorMainSkill;

		public NKCUIOperatorSkill OperatorSubSkill;

		public NKCUIOperatorTacticalSkillCombo OperatorSkillCombo;

		public GameObject ETCSelection;

		public GameObject PickEffectETC;

		public GameObject PickEffectShip;

		public GameObject PickEffectOperator;

		public NKCDeckViewUnitSlot[] Unit = new NKCDeckViewUnitSlot[9];

		public NKCUILeagueTier TierIcon;

		public Text TierScore;

		public Text Level;

		public Text Name;

		public GameObject Guild;

		public NKCUIGuildBadge GuildBadge;

		public Text GuildName;

		public NKCDeckViewUnitSelectListSlot[] GlobalBanList = new NKCDeckViewUnitSelectListSlot[2];

		public Image GlobalBanShip;

		public NKCDeckViewUnitSelectListSlot LocalBan;

		public void Init(OnClickPickedUnit onClickDeckSlot, NKCDeckViewShip.OnShipClicked onShipClicked)
		{
			for (int i = 0; i < Unit.Length; i++)
			{
				NKCDeckViewUnitSlot nKCDeckViewUnitSlot = Unit[i];
				nKCDeckViewUnitSlot.Init(0, bEnableDrag: false);
				nKCDeckViewUnitSlot.SetData(null, bEnableButton: false);
				nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect: false);
				nKCDeckViewUnitSlot.m_NKCUIComButton.PointerClick.RemoveAllListeners();
				if (onClickDeckSlot != null)
				{
					int index = i;
					nKCDeckViewUnitSlot.m_NKCUIComButton.PointerClick.AddListener(delegate
					{
						onClickDeckSlot(index);
					});
				}
			}
			DeckViewShip.Init(onShipClicked);
			DeckViewShip.Open(null);
			DeckViewShip.SetSelectEffect(value: false);
			DeckViewOperator.Init();
			DeckViewOperator.Enable();
			OperatorSlot.Init();
			OperatorSlot.SetData(null);
			for (int num = 0; num < GlobalBanList.Length; num++)
			{
				NKCDeckViewUnitSelectListSlot obj = GlobalBanList[num];
				obj.Init();
				obj.SetData(null, 0, bEnableLayoutElement: false, null);
			}
			LocalBan?.Init();
		}

		public void SetData(DraftPvpRoomData.DraftTeamData draftTeamData, bool isLeftTeam, bool includePickBan, bool forceEnableButton, int selectedBanIndex, int selectedLeaderIndex, bool isPrivatePvp)
		{
			int num = NKCPVPManager.FindPvPSeasonID(NKM_GAME_TYPE.NGT_PVP_LEAGUE, NKCSynchronizedTime.GetServerUTCTime());
			bool flag = false;
			if (!NKMLeaguePvpRankTemplet.FindByTier(num, draftTeamData.userProfileData.leaguePvpData.leagueTierId, out var templet) && !isPrivatePvp)
			{
				Log.Debug($"NKMLeaguePvpRankTemplet is null - seasonID : {num}, tierID : {draftTeamData.userProfileData.leaguePvpData.leagueTierId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueMain.cs", 122);
				flag = true;
			}
			bool flag2 = draftTeamData.userProfileData.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID;
			bool flag3 = NKCLeaguePVPMgr.DraftRoomData != null && NKCLeaguePVPMgr.DraftRoomData.gameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE && !flag2;
			if (TierIcon != null)
			{
				if (NKCLeaguePVPMgr.DraftRoomData != null)
				{
					if (NKCLeaguePVPMgr.DraftRoomData.gameType == NKM_GAME_TYPE.NGT_PVP_EVENT || flag)
					{
						TierIcon.SetDisableNormalTier();
						TierIcon.SetEventmatchIcon(value: true);
					}
					else if (NKCLeaguePVPMgr.DraftRoomData.gameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE && flag3)
					{
						TierIcon.SetDisableNormalTier();
						TierIcon.SetBlind(bValue: true);
					}
					else
					{
						TierIcon.SetUI(templet);
					}
				}
				else
				{
					TierIcon.SetUI(templet);
				}
			}
			if (flag3)
			{
				NKCUtil.SetLabelText(TierScore, "?");
				NKCUtil.SetLabelText(Level, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, "?");
				NKCUtil.SetLabelText(Name, NKCUtilString.GET_STRING_LEAGUE_DRAFT_UNKNOWN);
			}
			else
			{
				NKCUtil.SetLabelText(TierScore, draftTeamData.userProfileData.leaguePvpData.score.ToString());
				NKCUtil.SetLabelText(Level, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, draftTeamData.userProfileData.commonProfile.level);
				NKCUtil.SetLabelText(Name, NKCUtilString.GetUserNickname(draftTeamData.userProfileData.commonProfile.nickname, !isLeftTeam));
			}
			if (isPrivatePvp)
			{
				if (TierIcon != null)
				{
					TierIcon.SetDisableNormalTier();
					TierIcon.SetPrivatePvp(bValue: true);
				}
				NKCUtil.SetGameobjectActive(TierScore?.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(TierScore?.transform.parent?.gameObject, bValue: false);
			}
			bool flag4 = true;
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD) || draftTeamData.userProfileData.guildData == null || draftTeamData.userProfileData.guildData.guildUid == 0L)
			{
				flag4 = false;
			}
			NKCUtil.SetGameobjectActive(Guild, flag4 && !flag3);
			if (Guild != null && Guild.activeSelf)
			{
				GuildBadge.SetData(draftTeamData.userProfileData.guildData.badgeId, !isLeftTeam);
				NKCUtil.SetLabelText(GuildName, NKCUtilString.GetUserGuildName(draftTeamData.userProfileData.guildData.guildName, !isLeftTeam));
			}
			for (int i = 0; i < draftTeamData.globalBanUnitIdList.Count; i++)
			{
				NKMUnitTempletBase templetBase = NKMUnitTempletBase.Find(draftTeamData.globalBanUnitIdList[i]);
				GlobalBanList[i].SetDataForBan(templetBase, bEnableLayoutElement: true, null);
			}
			if ((NKCLeaguePVPMgr.IsPrivate() || NKCLeaguePVPMgr.IsEvent() || NKCLeaguePVPMgr.IsLeague()) && draftTeamData.globalBanShipGroupIdList.Count > 0)
			{
				int num2 = 0;
				if (num2 < draftTeamData.globalBanShipGroupIdList.Count)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(NKCBanManager.ConvertShipGroupIdToShipId(draftTeamData.globalBanShipGroupIdList[num2]));
					NKCUtil.SetImageSprite(GlobalBanShip, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase), bDisableIfSpriteNull: true);
					NKCUtil.SetGameobjectActive(GlobalBanShip, unitTempletBase != null);
				}
			}
			int num3 = 0;
			for (int j = 0; j < draftTeamData.pickUnitList.Count; j++)
			{
				NKMAsyncUnitData nKMAsyncUnitData = draftTeamData.pickUnitList[j];
				if (nKMAsyncUnitData == null)
				{
					continue;
				}
				if (j == draftTeamData.banishedUnitIndex && !includePickBan)
				{
					SetLocalBanUnit(nKMAsyncUnitData.unitId);
					continue;
				}
				NKMUnitData nKMUnitData = NKMDungeonManager.MakeUnitData(nKMAsyncUnitData, -1L);
				bool flag5 = false;
				if (!NKCLeaguePVPMgr.IsObserver())
				{
					if (!isLeftTeam && includePickBan && draftTeamData.banishedUnitIndex == -1 && NKCLeaguePVPMgr.DraftRoomData.roomState == DRAFT_PVP_ROOM_STATE.BAN_OPPONENT)
					{
						if (j != selectedBanIndex)
						{
							flag5 = true;
						}
					}
					else if (isLeftTeam && includePickBan && forceEnableButton && j != selectedLeaderIndex)
					{
						flag5 = true;
					}
				}
				Unit[num3].SetData(nKMUnitData, flag5);
				Unit[num3].SetLeaguePickEnable(flag5);
				if (j == draftTeamData.banishedUnitIndex)
				{
					SetLocalBanUnit(nKMUnitData.m_UnitID);
					Unit[num3].SetLeagueBan(bBanUnit: true);
					Unit[num3].SetLeaguePickEnable(bEnable: false);
				}
				if (j == selectedBanIndex || j == selectedLeaderIndex)
				{
					Unit[num3].SetSelectable(bSelectable: true);
				}
				else
				{
					Unit[num3].SetSelectable(bSelectable: false);
				}
				if ((isLeftTeam || NKCLeaguePVPMgr.DraftRoomData.roomState != DRAFT_PVP_ROOM_STATE.PICK_ETC || NKCLeaguePVPMgr.IsObserver()) && j == draftTeamData.leaderIndex)
				{
					Unit[num3].SetLeader(bLeader: true, bEffect: true);
				}
				num3++;
			}
			int currentSelectedSlot = NKCLeaguePVPMgr.GetCurrentSelectedSlot(draftTeamData.teamType);
			foreach (int item in NKCLeaguePVPMgr.GetPickEnabledSlot(draftTeamData.teamType))
			{
				if (Unit[item].m_NKMUnitData == null)
				{
					Unit[item].SetLeaguePickEnable(bEnable: true);
				}
				if (NKCLeaguePVPMgr.DraftRoomData.selectedUnit != null && currentSelectedSlot == item)
				{
					Log.Info($"[League] SelectedUnit [{currentSelectedSlot}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueMain.cs", 295);
					NKMUnitData cNKMUnitData = NKMDungeonManager.MakeUnitData(NKCLeaguePVPMgr.DraftRoomData.selectedUnit, -1L);
					Unit[currentSelectedSlot].SetData(cNKMUnitData, bEnableButton: false);
					Unit[currentSelectedSlot].SetSelectable(bSelectable: false);
				}
			}
			if (!isLeftTeam && NKCLeaguePVPMgr.DraftRoomData.roomState == DRAFT_PVP_ROOM_STATE.PICK_ETC && !NKCLeaguePVPMgr.IsObserver())
			{
				NKCUtil.SetGameobjectActive(OperatorSlot, bValue: false);
				NKCUtil.SetGameobjectActive(OperatorSkillInfo, bValue: false);
				NKCUtil.SetGameobjectActive(OperatorSkillCombo, bValue: false);
				NKCUtil.SetGameobjectActive(OperatorEmpty, bValue: false);
				return;
			}
			if (draftTeamData.mainShip != null && draftTeamData.mainShip.unitId != 0)
			{
				Log.Info($"[League][{isLeftTeam}] ShipInfo ID[{draftTeamData.mainShip.unitId}] Level[{draftTeamData.mainShip.unitLevel}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueMain.cs", 319);
				NKMUnitData shipUnitData = NKMDungeonManager.MakeUnitData(draftTeamData.mainShip, -1L);
				DeckViewShip.Open(shipUnitData);
			}
			bool flag6 = NKCContentManager.IsContentsUnlocked(ContentsType.OPERATOR);
			NKCUtil.SetGameobjectActive(OperatorSlot, draftTeamData.operatorUnit != null && flag6);
			NKCUtil.SetGameobjectActive(OperatorSkillInfo, draftTeamData.operatorUnit != null && flag6);
			NKCUtil.SetGameobjectActive(OperatorSkillCombo, draftTeamData.operatorUnit != null && flag6);
			NKCUtil.SetGameobjectActive(OperatorEmpty, draftTeamData.operatorUnit == null && flag6);
			if (draftTeamData.operatorUnit != null && draftTeamData.operatorUnit.id != 0 && flag6)
			{
				Log.Info($"[League][{isLeftTeam}] OperatorInfo ID[{draftTeamData.operatorUnit.id}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueMain.cs", 333);
				OperatorSlot.SetData(draftTeamData.operatorUnit);
				OperatorMainSkill.SetData(draftTeamData.operatorUnit.mainSkill.id, draftTeamData.operatorUnit.mainSkill.level);
				OperatorSubSkill.SetData(draftTeamData.operatorUnit.subSkill.id, draftTeamData.operatorUnit.subSkill.level);
				OperatorSkillCombo.SetData(draftTeamData.operatorUnit.id);
			}
		}

		public void ShowPickEffect(bool showShipEffect, bool showOperatorEffect)
		{
			if (showShipEffect || showOperatorEffect)
			{
				NKCUtil.SetGameobjectActive(ETCSelection, bValue: true);
			}
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.OPERATOR))
			{
				showOperatorEffect = false;
			}
			NKCUtil.SetGameobjectActive(PickEffectETC, showShipEffect || showOperatorEffect);
			NKCUtil.SetGameobjectActive(PickEffectShip, showShipEffect);
			NKCUtil.SetGameobjectActive(PickEffectOperator, showOperatorEffect);
		}

		public void Close()
		{
			if (DeckViewShip != null)
			{
				DeckViewShip.Close();
			}
			if (DeckViewOperator != null)
			{
				DeckViewOperator.Close();
			}
			for (int i = 0; i < GlobalBanList.Length; i++)
			{
				GlobalBanList[i] = null;
			}
			for (int j = 0; j < Unit.Length; j++)
			{
				Unit[j].CloseInstance();
			}
		}

		private void SetLocalBanUnit(int unitID)
		{
			if (null != LocalBan)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
				NKCUtil.SetGameobjectActive(LocalBan, unitTempletBase != null);
				LocalBan.SetDataForBan(unitTempletBase, bEnableLayoutElement: true, null);
			}
		}
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_LEAGUE_MAIN";

	[SerializeField]
	[Header("유저정보")]
	public LeagueUserInfoUI m_UserLeft;

	public LeagueUserInfoUI m_UserRight;

	public RectTransform m_rectCenterContent;

	public GameObject m_objUnitListParent;

	public NKCLeaguePvpUnitSelectList m_NKCLeaguePvpUnitSelectList;

	public NKCUIComStateButton m_SelectConfirmButton;

	public NKCUIComButton m_LeaveRoom;

	public NKCComTMPUIText m_lbLeaveRoom;

	public GameObject m_objPickDisableScreen;

	public GameObject m_objWatingNotice;

	[Header("애니메이터")]
	public Animator m_animatorStart;

	public Animator m_animatorPick;

	public Animator m_animatorCountDown;

	[Header("시간")]
	public Text m_timeText;

	[Header("유닛 일러스트")]
	public NKCUICharacterView m_CharacterView;

	[Header("팝업 가이드")]
	public GameObject m_PopupSequenceGuide;

	public Text m_sequenceGuideText;

	public Animator m_animatorsequenceGuide;

	private NKM_UNIT_TYPE m_eCurrentSelectListType;

	private NKCUIDeckViewer.DeckViewerOption m_ViewerOptions;

	private DateTime m_endTime;

	private int m_prevRemainingSeconds;

	private NKCUIUnitSelectListSlotBase m_currentSelectedUnit;

	private long m_currentSelectedUnitUID;

	private long m_currentSelectedShipUID;

	private long m_currentSelectedOperatorUID;

	private int m_currentSelectedLeaderIndex = -1;

	private int m_currentSelectedBanIndex = -1;

	private bool m_bSelectLeader;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override bool IgnoreBackButtonWhenOpen => true;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET;

	public override List<int> UpsideMenuShowResourceList => new List<int> { 101 };

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		return NKCUIManager.OpenNewInstanceAsync<NKCUIGauntletLeagueMain>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LEAGUE_MAIN", NKCUIManager.eUIBaseRect.UIFrontCommon, null);
	}

	public override void CloseInternal()
	{
		if (m_CharacterView != null)
		{
			m_CharacterView.CleanUp();
			m_CharacterView = null;
		}
		if (m_UserLeft != null)
		{
			m_UserLeft.Close();
		}
		if (m_UserRight != null)
		{
			m_UserRight.Close();
		}
		if (m_animatorStart != null)
		{
			m_animatorStart.Play("MAIN_OUTRO");
		}
	}

	public void Init()
	{
		m_prevRemainingSeconds = 0;
		m_ViewerOptions = default(NKCUIDeckViewer.DeckViewerOption);
		m_ViewerOptions.MenuName = "";
		m_ViewerOptions.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.LeaguePvPMain;
		m_ViewerOptions.dOnSideMenuButtonConfirm = null;
		NKCUtil.SetGameobjectActive(this, bValue: false);
		if (m_CharacterView != null)
		{
			m_CharacterView.Init(null, delegate
			{
			});
		}
		m_UserLeft.Init(OnClickSetLeaderUnit, null);
		m_UserRight.Init(OnClickBanPickedUnit, null);
		m_NKCLeaguePvpUnitSelectList.Init(null, OnDeckUnitChangeClicked, null, null);
		if (m_SelectConfirmButton != null)
		{
			m_SelectConfirmButton.PointerClick.RemoveAllListeners();
			m_SelectConfirmButton.PointerClick.AddListener(OnClickSlotSelectConfirm);
		}
		if (m_LeaveRoom != null)
		{
			m_LeaveRoom.PointerClick.RemoveAllListeners();
			m_LeaveRoom.PointerClick.AddListener(OnClickGiveup);
		}
		NKCUtil.SetGameobjectActive(m_SelectConfirmButton, bValue: false);
		NKCUtil.SetGameobjectActive(m_objWatingNotice, bValue: false);
		NKCUtil.SetGameobjectActive(m_objPickDisableScreen, NKCLeaguePVPMgr.IsObserver());
		NKCUtil.SetGameobjectActive(m_LeaveRoom, bValue: false);
		if (NKCLeaguePVPMgr.IsObserver())
		{
			m_rectCenterContent.offsetMax = new Vector2(0f, m_rectCenterContent.offsetMax.y);
		}
	}

	public void Open(DateTime endTime)
	{
		NKCUtil.SetGameobjectActive(this, bValue: true);
		m_animatorStart.Play("MAIN_INTRO");
		UpdatePickAnimation();
		UIOpened();
		RefreshDraftData(endTime, isStateChanged: true);
		NKCPopupGauntletLeagueBanList.Instance.Open();
	}

	public void ProcessBackButton()
	{
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
		NKCUtil.SetLabelText(m_lbLeaveRoom, NKCLeaguePVPMgr.IsObserver() ? NKCStringTable.GetString("SI_PF_OBSERVE_EXIT_BUTTON_TEXT") : NKCStringTable.GetString("SI_PF_LEAGUEMATCH_LEAVE"));
	}

	public void UpdatePickAnimation()
	{
		bool num = NKCLeaguePVPMgr.GetLeftDraftTeamData().teamType == NKM_TEAM_TYPE.NTT_A1;
		string text = (num ? "PICK_LEFT_IN" : "PICK_RIGHT_IN");
		string text2 = (num ? "PICK_RIGHT_IN" : "PICK_LEFT_IN");
		bool flag = NKCLeaguePVPMgr.CanPickUnit(NKCLeaguePVPMgr.MyTeamType) || NKCLeaguePVPMgr.CanPickETC();
		NKCUtil.SetGameobjectActive(m_SelectConfirmButton, flag && !NKCLeaguePVPMgr.IsObserver());
		NKCUtil.SetGameobjectActive(m_objWatingNotice, !flag && !NKCLeaguePVPMgr.IsObserver());
		string text3 = "PICK_NONE";
		switch (NKCLeaguePVPMgr.DraftRoomData.roomState)
		{
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_1:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_3:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_5:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_7:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_9:
			text3 = text;
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_2:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_4:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_6:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_8:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_10:
			text3 = text2;
			break;
		case DRAFT_PVP_ROOM_STATE.BAN_OPPONENT:
			text3 = "PICK_BOTH_IN";
			NKCUtil.SetGameobjectActive(m_SelectConfirmButton, !NKCLeaguePVPMgr.IsObserver());
			NKCUtil.SetGameobjectActive(m_objPickDisableScreen, bValue: true);
			NKCUtil.SetGameobjectActive(m_objWatingNotice, bValue: false);
			if (m_SelectConfirmButton != null)
			{
				m_SelectConfirmButton.PointerClick.RemoveAllListeners();
				m_SelectConfirmButton.PointerClick.AddListener(OnClickOponentBanConfirm);
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_ETC:
			text3 = "PICK_BOTH_IN";
			NKCUtil.SetGameobjectActive(m_SelectConfirmButton, !NKCLeaguePVPMgr.IsObserver());
			NKCUtil.SetGameobjectActive(m_objPickDisableScreen, NKCLeaguePVPMgr.IsObserver());
			NKCUtil.SetGameobjectActive(m_objWatingNotice, bValue: false);
			break;
		}
		if (!string.IsNullOrEmpty(text3))
		{
			m_animatorPick.Play(text3);
		}
		if (m_currentSelectedUnitUID > 0 && NKCLeaguePVPMgr.DraftRoomData.selectedUnit == null)
		{
			SelectUnit(m_eCurrentSelectListType, m_currentSelectedUnitUID);
		}
	}

	public void RefreshDraftData(DateTime endTime, bool isStateChanged)
	{
		if (!base.IsOpen)
		{
			Open(endTime);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objUnitListParent, !NKCLeaguePVPMgr.IsObserver());
		SetEndTime(endTime);
		UpdateUserInfo();
		UpdateCharacterView();
		UpdateSelectList();
	}

	public void SetEndTime(DateTime endTime)
	{
		m_endTime = endTime;
		m_prevRemainingSeconds = Math.Max(0, Convert.ToInt32((m_endTime - ServiceTime.Now).TotalSeconds));
	}

	public void UpdateUserInfo()
	{
		m_UserLeft.SetData(NKCLeaguePVPMgr.GetLeftDraftTeamData(), isLeftTeam: true, includePickBan: true, m_bSelectLeader, -1, m_currentSelectedLeaderIndex, NKCLeaguePVPMgr.IsPrivate());
		m_UserRight.SetData(NKCLeaguePVPMgr.GetRightDraftTeamData(), isLeftTeam: false, includePickBan: true, forceEnableButton: false, m_currentSelectedBanIndex, -1, NKCLeaguePVPMgr.IsPrivate());
		if (NKCLeaguePVPMgr.DraftRoomData.roomState != DRAFT_PVP_ROOM_STATE.PICK_ETC)
		{
			m_UserLeft.ShowPickEffect(showShipEffect: false, showOperatorEffect: false);
			m_UserRight.ShowPickEffect(showShipEffect: false, showOperatorEffect: false);
		}
	}

	private void UpdateCharacterView()
	{
		if (NKCLeaguePVPMgr.DraftRoomData != null && NKCLeaguePVPMgr.DraftRoomData.selectedUnit != null && !(m_CharacterView == null))
		{
			if (NKCLeaguePVPMgr.DraftRoomData.selectedUnit.skinId != 0)
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(NKCLeaguePVPMgr.DraftRoomData.selectedUnit.skinId);
				m_CharacterView.SetCharacterIllust(skinTemplet, bAsync: false, bEnableBackground: false);
			}
			else if (NKCLeaguePVPMgr.DraftRoomData.selectedUnit.unitId != 0)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(NKCLeaguePVPMgr.DraftRoomData.selectedUnit.unitId);
				m_CharacterView.SetCharacterIllust(unitTempletBase, 0, bAsync: false, bEnableBackground: false);
			}
			NKMUnitData unitData = NKMDungeonManager.MakeUnitData(NKCLeaguePVPMgr.DraftRoomData.selectedUnit, -1L);
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_BATTLE_READY, unitData);
		}
	}

	private void UpdateSelectList()
	{
		if (m_NKCLeaguePvpUnitSelectList != null && m_NKCLeaguePvpUnitSelectList.m_LoopScrollRect != null && m_NKCLeaguePvpUnitSelectList.gameObject.activeInHierarchy)
		{
			m_NKCLeaguePvpUnitSelectList.m_LoopScrollRect.RefreshCells();
		}
		if (m_currentSelectedUnit != null && m_currentSelectedUnit.UnitSelectState != NKCUIUnitSelectList.eUnitSlotSelectState.SELECTED)
		{
			m_currentSelectedUnit = null;
			m_currentSelectedUnitUID = 0L;
		}
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
				m_animatorCountDown.Play("COUNT_IN");
			}
		}
	}

	public void OpenUnitSelect()
	{
		OpenDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL);
		UpdateSelectList();
	}

	public void OnClickSetLeaderUnit(int index)
	{
		Log.Info("[League][Clicked - SetLeaderUnit]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueMain.cs", 721);
		if (NKCLeaguePVPMgr.DraftRoomData.roomState == DRAFT_PVP_ROOM_STATE.PICK_ETC && NKCLeaguePVPMgr.GetLeftDraftTeamData().leaderIndex == -1 && NKCLeaguePVPMgr.GetLeftDraftTeamData().banishedUnitIndex != index)
		{
			m_currentSelectedLeaderIndex = index;
			UpdateUserInfo();
		}
	}

	public void OnClickBanPickedUnit(int index)
	{
		Log.Info("[League][Clicked - BanPickedUnit]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueMain.cs", 743);
		if (NKCLeaguePVPMgr.DraftRoomData.roomState == DRAFT_PVP_ROOM_STATE.BAN_OPPONENT && NKCLeaguePVPMgr.GetRightDraftTeamData().banishedUnitIndex == -1)
		{
			m_currentSelectedBanIndex = index;
			UpdateUserInfo();
		}
	}

	public void OnClickSlotSelectConfirm()
	{
		if (m_eCurrentSelectListType == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			if (NKCLeaguePVPMgr.CanPickUnit(NKCLeaguePVPMgr.MyTeamType) && m_currentSelectedUnitUID != 0L)
			{
				Log.Info($"[League][UnitPick - confirm] UID[{m_currentSelectedUnitUID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueMain.cs", 764);
				NKCLeaguePVPMgr.Send_NKMPacket_DRAFT_PVP_PICK_UNIT_REQ(m_currentSelectedUnitUID);
			}
		}
		else if (m_eCurrentSelectListType == NKM_UNIT_TYPE.NUT_SHIP)
		{
			if (NKCLeaguePVPMgr.CanPickETC() && m_currentSelectedShipUID != 0L)
			{
				Log.Info($"[League][ShipPick - confirm] UID[{m_currentSelectedShipUID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueMain.cs", 773);
				NKCLeaguePVPMgr.Send_NKMPacket_DRAFT_PVP_PICK_SHIP_REQ(m_currentSelectedShipUID);
			}
		}
		else if (m_eCurrentSelectListType == NKM_UNIT_TYPE.NUT_OPERATOR && NKCLeaguePVPMgr.CanPickETC())
		{
			Log.Info($"[League][OperatorPick - confirm] UID[{m_currentSelectedOperatorUID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueMain.cs", 783);
			NKCLeaguePVPMgr.Send_NKMPacket_DRAFT_PVP_PICK_OPERATOR_REQ(m_currentSelectedOperatorUID);
		}
	}

	public void OnClickOponentBanConfirm()
	{
		if (m_currentSelectedBanIndex == -1)
		{
			ShowSequenceGuidePopup(NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_LOCAL_BAN"));
		}
		else
		{
			NKCLeaguePVPMgr.Send_NKMPacket_DRAFT_PVP_OPPONENT_BAN_REQ(m_currentSelectedBanIndex);
		}
	}

	public void OnRecv(NKMPacket_DRAFT_PVP_OPPONENT_BAN_ACK sPacket)
	{
		m_currentSelectedBanIndex = -1;
		NKCUtil.SetGameobjectActive(m_SelectConfirmButton, bValue: false);
	}

	public void OnClickLeaderSelectConfirm()
	{
		if (m_currentSelectedLeaderIndex == -1)
		{
			ShowSequenceGuidePopup(NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_LEADER"));
		}
		else
		{
			NKCLeaguePVPMgr.Send_NKMPacket_DRAFT_PVP_PICK_LEADER_REQ(m_currentSelectedLeaderIndex);
		}
	}

	public void OnDeckUnitChangeClicked(NKMDeckIndex selectedDeckIndex, long unitUID, NKM_UNIT_TYPE unitType)
	{
		Log.Info($"[League][Clicked - unit] Type[{unitType.ToString()}] - UID[{unitUID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLeagueMain.cs", 823);
		SelectUnit(unitType, unitUID);
	}

	private void SelectUnit(NKM_UNIT_TYPE unitType, long unitUID)
	{
		if (m_currentSelectedUnit != null)
		{
			m_currentSelectedUnit.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.NONE);
		}
		if (unitType != m_eCurrentSelectListType || !m_NKCLeaguePvpUnitSelectList.gameObject.activeInHierarchy)
		{
			return;
		}
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = m_NKCLeaguePvpUnitSelectList.FindSlotFromCurrentList(unitType, unitUID);
		if (!(nKCUIUnitSelectListSlotBase == null))
		{
			nKCUIUnitSelectListSlotBase.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.SELECTED);
			m_currentSelectedUnit = nKCUIUnitSelectListSlotBase;
			switch (unitType)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				m_currentSelectedUnitUID = unitUID;
				NKCLeaguePVPMgr.Send_NKMPacket_DRAFT_PVP_SELECT_UNIT_REQ(m_currentSelectedUnitUID);
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
				m_currentSelectedShipUID = unitUID;
				break;
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				m_currentSelectedOperatorUID = unitUID;
				break;
			}
		}
	}

	public void OpenShipSelect(bool bObserver)
	{
		string strID = "SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_SHIP";
		if (bObserver)
		{
			UpdateUpsideMenu();
			UpdateUserInfo();
		}
		else
		{
			OpenDeckSelectList(NKM_UNIT_TYPE.NUT_SHIP);
			UpdateSelectList();
		}
		ShowSequenceGuidePopup(NKCStringTable.GetString(strID));
		m_UserLeft.ShowPickEffect(showShipEffect: true, showOperatorEffect: false);
		m_UserRight.ShowPickEffect(showShipEffect: true, showOperatorEffect: false);
		if (m_SelectConfirmButton != null && !bObserver)
		{
			m_SelectConfirmButton.PointerClick.RemoveAllListeners();
			m_SelectConfirmButton.PointerClick.AddListener(OnClickSlotSelectConfirm);
		}
	}

	public void OpenOperatorSelect(bool bObserver)
	{
		string strID = "SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_OPERATOR";
		if (bObserver)
		{
			strID = "SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_OPERATOR_OBSERVER";
		}
		ShowSequenceGuidePopup(NKCStringTable.GetString(strID));
		m_UserLeft.ShowPickEffect(showShipEffect: false, showOperatorEffect: true);
		m_UserRight.ShowPickEffect(showShipEffect: false, showOperatorEffect: true);
		if (NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetCurrentOperatorCount() == 0)
		{
			m_currentSelectedUnit = null;
			m_currentSelectedOperatorUID = 0L;
			m_eCurrentSelectListType = NKM_UNIT_TYPE.NUT_OPERATOR;
			OnClickSlotSelectConfirm();
		}
		else if (bObserver)
		{
			UpdateUpsideMenu();
			UpdateUserInfo();
		}
		else
		{
			OpenDeckSelectList(NKM_UNIT_TYPE.NUT_OPERATOR);
			UpdateSelectList();
		}
	}

	public void OpenLeaderSelect()
	{
		m_UserLeft.ShowPickEffect(showShipEffect: false, showOperatorEffect: false);
		m_UserRight.ShowPickEffect(showShipEffect: false, showOperatorEffect: false);
		ShowSequenceGuidePopup(NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_LEADER"));
		m_bSelectLeader = true;
		NKCUtil.SetGameobjectActive(m_objPickDisableScreen, bValue: true);
		UpdateUserInfo();
		if (m_SelectConfirmButton != null)
		{
			m_SelectConfirmButton.PointerClick.RemoveAllListeners();
			m_SelectConfirmButton.PointerClick.AddListener(OnClickLeaderSelectConfirm);
		}
	}

	public void CloseUnitSelectList()
	{
		if (m_currentSelectedUnit != null)
		{
			m_currentSelectedUnit.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.NONE);
			m_currentSelectedUnit = null;
		}
		m_bSelectLeader = false;
		m_currentSelectedUnitUID = 0L;
		m_currentSelectedShipUID = 0L;
		m_currentSelectedOperatorUID = 0L;
		m_currentSelectedBanIndex = -1;
		m_currentSelectedLeaderIndex = -1;
	}

	public void ApplyPickETCResults()
	{
		m_UserLeft.ShowPickEffect(showShipEffect: false, showOperatorEffect: false);
		NKCUtil.SetGameobjectActive(m_objPickDisableScreen, bValue: true);
		NKCUtil.SetGameobjectActive(m_objWatingNotice, !NKCLeaguePVPMgr.IsObserver());
		CloseUnitSelectList();
		UpdateUserInfo();
	}

	private void OpenDeckSelectList(NKM_UNIT_TYPE eUnitType)
	{
		m_currentSelectedUnit = null;
		m_currentSelectedUnitUID = 0L;
		m_currentSelectedShipUID = 0L;
		m_currentSelectedOperatorUID = 0L;
		m_currentSelectedLeaderIndex = -1;
		m_currentSelectedBanIndex = -1;
		m_bSelectLeader = false;
		m_eCurrentSelectListType = eUnitType;
		if (m_NKCLeaguePvpUnitSelectList.IsOpen)
		{
			UpdateDeckSelectList(eUnitType);
		}
		else if (eUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			m_NKCLeaguePvpUnitSelectList.Open(bAnimate: true, eUnitType, MakeOperatorSortOptions(), m_ViewerOptions);
		}
		else
		{
			m_NKCLeaguePvpUnitSelectList.Open(bAnimate: true, eUnitType, MakeSortOptions(), m_ViewerOptions);
		}
		UpdateUpsideMenu();
		UpdateUserInfo();
	}

	private void UpdateDeckSelectList(NKM_UNIT_TYPE eUnitType)
	{
		m_eCurrentSelectListType = eUnitType;
		if (m_eCurrentSelectListType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			m_NKCLeaguePvpUnitSelectList.UpdateLoopScrollList(eUnitType, MakeOperatorSortOptions());
		}
		else
		{
			m_NKCLeaguePvpUnitSelectList.UpdateLoopScrollList(eUnitType, MakeSortOptions());
		}
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

	private NKCOperatorSortSystem.OperatorListOptions MakeOperatorSortOptions()
	{
		return new NKCOperatorSortSystem.OperatorListOptions
		{
			eDeckType = NKM_DECK_TYPE.NDT_NONE,
			setExcludeOperatorID = null,
			setOnlyIncludeOperatorID = null,
			setDuplicateOperatorID = null,
			setExcludeOperatorUID = null,
			setFilterOption = m_NKCLeaguePvpUnitSelectList.SortOperatorOptions.setFilterOption,
			lstSortOption = m_NKCLeaguePvpUnitSelectList.SortOperatorOptions.lstSortOption
		};
	}

	public void ShowSequenceGuidePopup(string text)
	{
		NKCUtil.SetGameobjectActive(m_PopupSequenceGuide, bValue: true);
		NKCUtil.SetLabelText(m_sequenceGuideText, text);
		if (m_animatorsequenceGuide != null && !NKCLeaguePVPMgr.IsObserver())
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
