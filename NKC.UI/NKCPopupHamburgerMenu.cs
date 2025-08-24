using System;
using System.Collections.Generic;
using NKC.Templet;
using NKC.UI.Guide;
using NKC.UI.Office;
using NKC.UI.Option;
using NKC.UI.Result;
using NKC.UI.Shop;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupHamburgerMenu : NKCUIBase
{
	public enum eButtonMenuType
	{
		Guide,
		Guild,
		Chat,
		Friends,
		Hangar,
		Inventory,
		Collection,
		Factory,
		UnitList,
		Personnel,
		Lab,
		DeckSetup,
		Pvp,
		Worldmap,
		Operation,
		Mission,
		Contract,
		Base,
		Shop,
		OfficeDorm,
		OfficeFacility,
		Ranking
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_HAMBURGER_MENU";

	private static NKCPopupHamburgerMenu m_Instance;

	public Animator m_ani;

	public NKCUIComStateButton m_btnBackground;

	[Header("Top")]
	public Text m_lbUserName;

	public NKCUIComStateButton m_btnUserBuff;

	public Text m_lbBuffCount;

	public NKCUIComStateButton m_btnMail;

	public GameObject m_objNewMail;

	public NKCUIComStateButton m_btnOption;

	public NKCUIComStateButton m_btnClose;

	[Header("")]
	public NKCPopupHamburgerMenuSimpleButton m_btnGuide;

	public NKCPopupHamburgerMenuGuildButton m_btnGuild;

	public NKCPopupHamburgerMenuSimpleButton m_btnChat;

	[Header("Line 1")]
	public NKCPopupHamburgerMenuSimpleButton m_btnCollection;

	public NKCPopupHamburgerMenuSimpleButton m_btnOfficeDorm;

	public NKCPopupHamburgerMenuSimpleButton m_btnInventory;

	[Header("Line 2")]
	public NKCPopupHamburgerMenuSimpleButton m_btnHangar;

	public NKCPopupHamburgerMenuSimpleButton m_btnLab;

	public NKCPopupHamburgerMenuSimpleButton m_btnUnitList;

	[Header("Line 3")]
	public NKCPopupHamburgerMenuSimpleButton m_btnPersonnel;

	public NKCPopupHamburgerMenuSimpleButton m_btnFactory;

	public GameObject m_objFactoryEvent;

	public NKCPopupHamburgerMenuSimpleButton m_btnDeckSetup;

	[Header("Line 4")]
	public NKCPopupHamburgerMenuSimpleButton m_btnPvp;

	public NKCPopupHamburgerMenuSimpleButton m_btnWorldmap;

	public NKCPopupHamburgerMenuSimpleButton m_btnOperation;

	[Header("DownLeft")]
	public NKCPopupHamburgerMenuSimpleButton m_btnFriends;

	public NKCPopupHamburgerMenuSimpleButton m_btnOfficeFacility;

	public NKCPopupHamburgerMenuSimpleButton m_btnMission;

	public NKCPopupHamburgerMenuSimpleButton m_btnRanking;

	[Header("DownRight")]
	public NKCPopupHamburgerMenuSimpleButton m_btnContract;

	public NKCPopupHamburgerMenuShop m_btnShop;

	[Header("추적중인 미션")]
	public GameObject m_objTrackMission;

	public Image m_imgMissionBG;

	public Image m_imgMissionIcon;

	public Text m_lbMissionNum;

	public Text m_lbMissionTitle;

	public Text m_lbMissionDesc;

	public Text m_lbProgress;

	public Slider m_sliderProgress;

	public List<NKCUISlot> m_lstRewardSlot;

	public NKCUIComStateButton m_btnQuickMove;

	public NKCUIComStateButton m_btnComplete;

	private Dictionary<eButtonMenuType, NKCPopupHamburgerMenuSimpleButton> m_dicButtons = new Dictionary<eButtonMenuType, NKCPopupHamburgerMenuSimpleButton>();

	private NKMMissionTemplet m_trackingMissionTemplet;

	public static NKCPopupHamburgerMenu instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupHamburgerMenu>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_HAMBURGER_MENU", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupHamburgerMenu>();
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

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.ON_PLAY_GAME;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "Menu";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void OnCloseInstance()
	{
		NKCMailManager.dOnMailFlagChange = (NKCMailManager.OnMailFlagChange)Delegate.Remove(NKCMailManager.dOnMailFlagChange, new NKCMailManager.OnMailFlagChange(SetNewMail));
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		m_dicButtons.Clear();
		if (m_btnBackground != null)
		{
			m_btnBackground.PointerClick.RemoveAllListeners();
			m_btnBackground.PointerClick.AddListener(base.Close);
		}
		if (m_btnUserBuff != null)
		{
			m_btnUserBuff.PointerClick.RemoveAllListeners();
			m_btnUserBuff.PointerClick.AddListener(OnBtnBuff);
		}
		if (m_btnMail != null)
		{
			m_btnMail.PointerClick.RemoveAllListeners();
			m_btnMail.PointerClick.AddListener(OnBtnMail);
		}
		if (m_objNewMail != null)
		{
			NKCMailManager.dOnMailFlagChange = (NKCMailManager.OnMailFlagChange)Delegate.Combine(NKCMailManager.dOnMailFlagChange, new NKCMailManager.OnMailFlagChange(SetNewMail));
		}
		if (m_btnOption != null)
		{
			m_btnOption.PointerClick.RemoveAllListeners();
			m_btnOption.PointerClick.AddListener(OnBtnOption);
		}
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(OnBtnClose);
			NKCUtil.SetHotkey(m_btnClose, HotkeyEventType.HamburgerMenu);
		}
		if (m_btnGuide != null)
		{
			m_dicButtons.Add(eButtonMenuType.Guide, m_btnGuide);
			m_btnGuide.Init(null, OnBtnGuide);
		}
		if (m_btnGuild != null)
		{
			m_dicButtons.Add(eButtonMenuType.Guild, m_btnGuild);
			m_btnGuild.Init(NKCAlarmManager.CheckGuildNotify, OnBtnGuild, ContentsType.GUILD);
		}
		if (m_btnChat != null)
		{
			m_dicButtons.Add(eButtonMenuType.Chat, m_btnChat);
			m_btnChat.Init(NKCAlarmManager.CheckChatNotify, OnBtnChat, ContentsType.FRIENDS);
		}
		if (m_btnCollection != null)
		{
			m_dicButtons.Add(eButtonMenuType.Collection, m_btnCollection);
			m_btnCollection.Init(NKCAlarmManager.CheckCollectionNotify, OnBtnCollection);
		}
		if (m_btnOfficeDorm != null)
		{
			m_dicButtons.Add(eButtonMenuType.OfficeDorm, m_btnOfficeDorm);
			m_btnOfficeDorm.Init(NKCAlarmManager.CheckOfficeDormNotify, OnBtnOfficeDorm, ContentsType.OFFICE);
		}
		if (m_btnInventory != null)
		{
			m_dicButtons.Add(eButtonMenuType.Inventory, m_btnInventory);
			m_btnInventory.Init(NKCAlarmManager.CheckInventoryNotify, OnBtnInventory);
		}
		if (m_btnHangar != null)
		{
			m_dicButtons.Add(eButtonMenuType.Hangar, m_btnHangar);
			m_btnHangar.Init(NKCAlarmManager.CheckHangarNotify, OnBtnHangar, ContentsType.BASE_HANGAR);
		}
		if (m_btnLab != null)
		{
			m_dicButtons.Add(eButtonMenuType.Lab, m_btnLab);
			m_btnLab.Init(null, OnBtnLab, ContentsType.BASE_LAB);
		}
		if (m_btnUnitList != null)
		{
			m_dicButtons.Add(eButtonMenuType.UnitList, m_btnUnitList);
			m_btnUnitList.Init(null, OnBtnUnitList);
		}
		if (m_btnPersonnel != null)
		{
			m_dicButtons.Add(eButtonMenuType.Personnel, m_btnPersonnel);
			m_btnPersonnel.Init(NKCAlarmManager.CheckScoutNotify, OnBtnPersonnel, ContentsType.BASE_PERSONNAL);
		}
		if (m_btnFactory != null)
		{
			m_dicButtons.Add(eButtonMenuType.Factory, m_btnFactory);
			m_btnFactory.Init(NKCAlarmManager.CheckFactoryNotify, OnBtnFactory, ContentsType.BASE_FACTORY);
			m_btnFactory.SetEnableEvent(NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT) || NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT) || NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT));
		}
		if (m_btnDeckSetup != null)
		{
			m_dicButtons.Add(eButtonMenuType.DeckSetup, m_btnDeckSetup);
			m_btnDeckSetup.Init(null, OnBtnDeckSetup, ContentsType.DECKVIEW);
		}
		if (m_btnPvp != null)
		{
			m_dicButtons.Add(eButtonMenuType.Pvp, m_btnPvp);
			m_btnPvp.Init(NKCAlarmManager.CheckPVPNotify, OnBtnPvp, ContentsType.PVP);
		}
		if (m_btnWorldmap != null)
		{
			m_dicButtons.Add(eButtonMenuType.Worldmap, m_btnWorldmap);
			m_btnWorldmap.Init(NKCAlarmManager.CheckWorldMapNotify, OnBtnWorldmap, ContentsType.WORLDMAP);
		}
		if (m_btnOperation != null)
		{
			m_dicButtons.Add(eButtonMenuType.Operation, m_btnOperation);
			m_btnOperation.Init(null, OnBtnOperation);
		}
		if (m_btnFriends != null)
		{
			m_dicButtons.Add(eButtonMenuType.Friends, m_btnFriends);
			m_btnFriends.Init(NKCAlarmManager.CheckFriendNotify, OnBtnFriends, ContentsType.FRIENDS);
		}
		if (m_btnOfficeFacility != null)
		{
			m_dicButtons.Add(eButtonMenuType.OfficeFacility, m_btnOfficeFacility);
			m_btnOfficeFacility.Init(null, OnBtnOfficeFacility, ContentsType.BASE);
		}
		if (m_btnRanking != null)
		{
			m_dicButtons.Add(eButtonMenuType.Ranking, m_btnRanking);
			m_btnRanking.Init(NKCAlarmManager.CheckleaderBoardNotify, OnBtnRanking, ContentsType.LEADERBOARD);
		}
		if (m_btnMission != null)
		{
			m_dicButtons.Add(eButtonMenuType.Mission, m_btnMission);
			m_btnMission.Init(NKCAlarmManager.CheckMissionNotify, OnBtnMission, ContentsType.LOBBY_SUBMENU);
		}
		if (m_btnContract != null)
		{
			m_dicButtons.Add(eButtonMenuType.Contract, m_btnContract);
			m_btnContract.Init(NKCAlarmManager.CheckContractNotify, OnBtnContract, ContentsType.CONTRACT);
		}
		if (m_btnShop != null)
		{
			m_dicButtons.Add(eButtonMenuType.Shop, m_btnShop);
			m_btnShop.Init(NKCAlarmManager.CheckShopNotify, OnBtnShop, ContentsType.LOBBY_SUBMENU);
		}
		if (m_btnQuickMove != null)
		{
			m_btnQuickMove.PointerClick.RemoveAllListeners();
			m_btnQuickMove.PointerClick.AddListener(OnBtnQuickMove);
		}
		if (m_btnComplete != null)
		{
			m_btnComplete.PointerClick.RemoveAllListeners();
			m_btnComplete.PointerClick.AddListener(OnBtnQuickMove);
		}
		if (NKMMissionManager.GetTrackingMissionTemplet() == null)
		{
			NKMMissionManager.SetDefaultTrackingMissionToGrowthMission();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OpenUI()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		foreach (KeyValuePair<eButtonMenuType, NKCPopupHamburgerMenuSimpleButton> dicButton in m_dicButtons)
		{
			dicButton.Value.UpdateData(nKMUserData);
		}
		SetNewMail(NKCAlarmManager.CheckMailNotify(nKMUserData));
		m_lbUserName.text = NKCUtilString.GetUserNickname(nKMUserData.m_UserNickName, bOpponent: false);
		SetGuildData();
		SetUserBuffCount();
		SetMissionTab();
		UIOpened();
	}

	private void SetMissionTab()
	{
		if (NKMMissionManager.GetTrackingMissionTemplet() == null)
		{
			NKMMissionManager.SetDefaultTrackingMissionToGrowthMission();
		}
		m_trackingMissionTemplet = NKMMissionManager.GetTrackingMissionTemplet();
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.LOBBY_SUBMENU) || m_trackingMissionTemplet == null || !NKCTutorialManager.TutorialCompleted(TutorialStep.Achieventment))
		{
			NKCUtil.SetGameobjectActive(m_objTrackMission, bValue: false);
			return;
		}
		NKCUtil.SetImageSprite(m_imgMissionBG, NKCUtil.GetMissionThumbnailSprite(m_trackingMissionTemplet), bDisableIfSpriteNull: true);
		NKCUtil.SetImageSprite(m_imgMissionIcon, NKCUtil.GetGrowthMissionHamburgerIconSprite(m_trackingMissionTemplet), bDisableIfSpriteNull: true);
		NKCUtil.SetLabelText(m_lbMissionTitle, m_trackingMissionTemplet.GetTitle());
		NKCUtil.SetLabelText(m_lbMissionDesc, m_trackingMissionTemplet.GetDesc());
		long num = 0L;
		long num2 = 0L;
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(m_trackingMissionTemplet.m_MissionTabId);
		if (missionTabTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_objTrackMission, bValue: false);
			return;
		}
		if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.GROWTH_COMPLETE)
		{
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(m_trackingMissionTemplet.m_MissionRequire);
			if (missionTemplet != null)
			{
				num = NKMMissionManager.GetMissionTempletListByType(missionTemplet.m_MissionTabId).Count;
			}
			num2 = num;
		}
		else
		{
			num = m_trackingMissionTemplet.m_Times;
			if (NKMMissionManager.IsCumulativeCondition(m_trackingMissionTemplet.m_MissionCond.mission_cond))
			{
				NKMMissionData missionData = NKCScenManager.CurrentUserData().m_MissionData.GetMissionData(m_trackingMissionTemplet);
				if (missionData != null)
				{
					num2 = Math.Min(missionData.times, num);
				}
			}
			else
			{
				num2 = NKMMissionManager.GetNonCumulativeMissionTimes(m_trackingMissionTemplet, NKCScenManager.GetScenManager().GetMyUserData(), bShowErrorlog: false);
			}
		}
		NKCUtil.SetLabelText(m_lbProgress, $"{num2} / {num}");
		m_sliderProgress.value = (float)num2 / (float)num;
		int num3 = 0;
		new List<NKMMissionTemplet>();
		num3 = ((missionTabTemplet.m_MissionType != NKM_MISSION_TYPE.GROWTH_COMPLETE) ? NKMMissionManager.GetMissionTempletListByType(m_trackingMissionTemplet.m_MissionTabId).FindIndex((NKMMissionTemplet x) => x == m_trackingMissionTemplet) : NKMMissionManager.GetMissionTempletListByType(NKMMissionManager.GetMissionTemplet(m_trackingMissionTemplet.m_MissionRequire).m_MissionTabId).Count);
		NKCUtil.SetLabelText(m_lbMissionNum, string.Format(NKCUtilString.GET_STRING_POPUP_HAMBER_MENU_MISSION_TITLE_DESC_01, num3 + 1));
		for (int num4 = 0; num4 < m_trackingMissionTemplet.m_MissionReward.Count; num4++)
		{
			MissionReward missionReward = m_trackingMissionTemplet.m_MissionReward[num4];
			m_lstRewardSlot[num4].SetData(NKCUISlot.SlotData.MakeRewardTypeData(missionReward.reward_type, missionReward.reward_id, missionReward.reward_value));
			m_lstRewardSlot[num4].SetActive(bSet: true);
		}
		for (int num5 = m_trackingMissionTemplet.m_MissionReward.Count; num5 < m_lstRewardSlot.Count; num5++)
		{
			m_lstRewardSlot[num5].SetActive(bSet: false);
		}
		NKCUtil.SetGameobjectActive(m_btnQuickMove.gameObject, num2 < num && m_trackingMissionTemplet.m_ShortCutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE);
		NKCUtil.SetGameobjectActive(m_btnComplete.gameObject, num2 >= num);
		NKCUtil.SetGameobjectActive(m_objTrackMission, bValue: true);
	}

	private void SetGuildData()
	{
		m_btnGuild?.SetGuildData();
	}

	private void SetUserBuffCount()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKCCompanyBuff.RemoveExpiredBuffs(nKMUserData.m_companyBuffDataList);
			int count = nKMUserData.m_companyBuffDataList.Count;
			NKCUtil.SetGameobjectActive(m_btnUserBuff, count > 0);
			NKCUtil.SetLabelText(m_lbBuffCount, count.ToString());
			bool bValue = NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT) || NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT) || NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT);
			NKCUtil.SetGameobjectActive(m_objFactoryEvent, bValue);
		}
	}

	public void Refresh()
	{
		SetUserBuffCount();
		SetMissionTab();
		NKMUserData userData = NKCScenManager.CurrentUserData();
		foreach (KeyValuePair<eButtonMenuType, NKCPopupHamburgerMenuSimpleButton> dicButton in m_dicButtons)
		{
			dicButton.Value.UpdateData(userData);
		}
	}

	public void OnMissionComplete(NKMRewardData rewardData)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKCUIResult.OnClose onClose = null;
		if (NKCGameEventManager.IsWaiting())
		{
			onClose = NKCGameEventManager.WaitFinished;
		}
		NKCUIResult.Instance.OpenComplexResult(myUserData.m_ArmyData, rewardData, onClose, 0L);
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.F1))
		{
			OnBtnGuide();
		}
	}

	private void SetNewMail(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objNewMail, bValue);
	}

	private void OnBtnBuff()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKCCompanyBuff.RemoveExpiredBuffs(nKMUserData.m_companyBuffDataList);
			if (nKMUserData.m_companyBuffDataList.Count > 0)
			{
				Close();
				NKCPopupCompanyBuff.Instance.Open();
			}
			else
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_LOBBY_USER_BUFF_NONE);
			}
		}
	}

	private void OnBtnMail()
	{
		if (!NKCUIMail.IsInstanceOpen)
		{
			Close();
			NKCUIMail.Instance.Open();
		}
	}

	private void OnBtnOption()
	{
		Close();
		NKCUIGameOption.Instance.Open(NKC_GAME_OPTION_MENU_TYPE.NORMAL);
	}

	private void OnBtnClose()
	{
		Close();
	}

	private void OnBtnGuide()
	{
		NKCUIPopUpGuide.Instance.Open();
	}

	private void OnBtnGuild()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_GUILD, "");
	}

	private void OnBtnChat()
	{
		if (NKMOpenTagManager.IsOpened("CHAT_PRIVATE"))
		{
			bool bAdmin;
			switch (NKCContentManager.CheckContentStatus(ContentsType.FRIENDS, out bAdmin))
			{
			case NKCContentManager.eContentStatus.Open:
				if (NKCScenManager.GetScenManager().GetGameOptionData().UseChatContent)
				{
					Close();
					NKCPopupPrivateChatLobby.Instance.Open(0L);
				}
				else
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_OPTION_GAME_CHAT_NOTICE);
				}
				break;
			case NKCContentManager.eContentStatus.Lock:
				NKCContentManager.ShowLockedMessagePopup(ContentsType.FRIENDS);
				break;
			}
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_COMING_SOON_SYSTEM);
		}
	}

	private void OnBtnFriends()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_FRIEND_ADD, "");
	}

	private void OnBtnHangar()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, NKMOfficeRoomTemplet.RoomType.Hangar.ToString());
	}

	private void OnBtnInventory()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_INVENTORY, "");
	}

	private void OnBtnOfficeDorm()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, NKCUIOfficeMapFront.SectionType.Room.ToString());
	}

	private void OnBtnOfficeFacility()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, NKCUIOfficeMapFront.SectionType.Facility.ToString());
	}

	private void OnBtnCollection()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION, "");
	}

	private void OnBtnFactory()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, NKMOfficeRoomTemplet.RoomType.Forge.ToString());
	}

	private void OnBtnUnitList()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_UNITLIST, "");
	}

	private void OnBtnPersonnel()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, NKMOfficeRoomTemplet.RoomType.CEO.ToString());
	}

	private void OnBtnLab()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, NKMOfficeRoomTemplet.RoomType.Lab.ToString());
	}

	private void OnBtnDeckSetup()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_DECKSETUP, "");
	}

	private void OnBtnPvp()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_PVP_MAIN, "");
	}

	private void OnBtnWorldmap()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_WORLDMAP_MISSION, "");
	}

	private void OnBtnOperation()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_MAINSTREAM, "");
	}

	private void OnBtnMission()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_MISSION, "");
	}

	private void OnBtnContract()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_CONTRACT, "");
	}

	private void OnBtnBase()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_BASEMAIN, "");
	}

	private void OnBtnShop()
	{
		Close();
		if (NKCShopCategoryTemplet.Find(NKCShopManager.ShopTabCategory.PACKAGE) != null)
		{
			NKCUIShop.Instance.Open(NKCShopManager.ShopTabCategory.PACKAGE);
		}
		else
		{
			NKCUIShop.Instance.Open(NKCShopManager.ShopTabCategory.EXCHANGE, "TAB_SUPPLY");
		}
	}

	private void OnBtnRanking()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_RANKING, "");
	}

	private void OnBtnQuickMove()
	{
		if (NKCUIForge.IsInstanceOpen && NKCScenManager.GetScenManager().GetMyUserData().hasReservedEquipTuningData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				BtnQuickMove();
			});
		}
		else if (NKCUIForge.IsInstanceOpen && NKCScenManager.GetScenManager().GetMyUserData().hasReservedSetOptionData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_SET_OPTION_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				BtnQuickMove();
			});
		}
		else if (NKCUIForge.IsInstanceOpen && NKCScenManager.GetScenManager().GetMyUserData().hasReservedHiddenOptionRerollData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_RELIC_REROLL_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_REQ();
				BtnQuickMove();
			});
		}
		else if (NKCPopupShipCommandModule.IsInstanceOpen && NKCScenManager.GetScenManager().GetMyUserData().GetShipCandidateData()
			.shipUid > 0)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SHIP_COMMANDMODULE_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ();
				BtnQuickMove();
			});
		}
		else
		{
			BtnQuickMove();
		}
	}

	private void BtnQuickMove()
	{
		NKMMissionTemplet trackingMissionTemplet = NKMMissionManager.GetTrackingMissionTemplet();
		NKMUserData userData = NKCScenManager.CurrentUserData();
		if (trackingMissionTemplet != null)
		{
			bool flag = false;
			NKMMissionData missionData = NKCScenManager.CurrentUserData().m_MissionData.GetMissionData(trackingMissionTemplet);
			if (!NKMMissionManager.IsCumulativeCondition(trackingMissionTemplet.m_MissionCond.mission_cond))
			{
				flag = trackingMissionTemplet.m_MissionCond.mission_cond == NKM_MISSION_COND.JUST_OPEN || NKMMissionManager.CanComplete(trackingMissionTemplet, userData, missionData) == NKM_ERROR_CODE.NEC_OK;
			}
			else if (missionData != null)
			{
				flag = NKMMissionManager.CanComplete(trackingMissionTemplet, userData, missionData) == NKM_ERROR_CODE.NEC_OK;
			}
			if (flag)
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(trackingMissionTemplet);
				return;
			}
			Close();
			NKCContentManager.MoveToShortCut(trackingMissionTemplet.m_ShortCutType, trackingMissionTemplet.m_ShortCut);
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
	}

	public override void OnEquipChange(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipItem)
	{
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
	}

	public override void OnUserLevelChanged(NKMUserData newUserData)
	{
	}

	public override void OnGuildDataChanged()
	{
		SetGuildData();
	}

	public void OnUserBuffChanged()
	{
		SetUserBuffCount();
	}
}
