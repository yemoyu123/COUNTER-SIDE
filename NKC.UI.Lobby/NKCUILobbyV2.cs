using System.Collections.Generic;
using Cs.Logging;
using NKC.PacketHandler;
using NKC.Publisher;
using NKC.Templet;
using NKC.UI.Collection;
using NKC.UI.Option;
using NKC.UI.Shop;
using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyV2 : NKCUIBase
{
	public enum eUIMenu
	{
		Mission,
		Shop,
		Event,
		Operation,
		CurrentEvent,
		DeckSetup,
		PVP,
		Contract,
		Base,
		Worldmap,
		Fierce,
		Option,
		Mail,
		UnitList,
		Collection,
		Inventory,
		Office,
		Friends,
		LeaderBoard,
		Guild,
		Attendance,
		CounterPass,
		GuideMission,
		Chat,
		Survey
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_lobby";

	private const string UI_ASSET_NAME = "NUF_HOME_PREFAB_RENEWAL";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	[Header("\ufffd\ufffd\ufffd")]
	public NKCUILobbyMenuMail m_UIMail;

	public NKCUILobbySimpleMenu m_UIOption;

	public NKCUIComStateButton m_btnHamburger;

	public GameObject m_objHamburgerNotify;

	[Header("3D \ufffd\ufffd\ufffd\ufffd\ufffd\ufffdƮ")]
	public NKCUILobby3DV2 m_UILobby3D;

	public NKCUILobbyMenuOperation m_UIOperation;

	public NKCUILobbyMenuCurrentEvent m_UICurrentEvent;

	public List<NKCUILobbyEventIndexSlot> m_lstUIEventIndexSlot;

	public NKCUILobbyMenuEventPass m_UIEventPass;

	public NKCUILobbyMenuPVP m_UIPVP;

	public NKCUILobbyMenuWorldmap m_UIWorldmap;

	public NKCUILobbyMenuFierce m_UIFierce;

	public NKCUILobbyEventIndexSlot m_UIEventBigBanner;

	[Header("\ufffd»\ufffd")]
	public NKCUILobbyUserInfo m_UIUserInfo;

	public NKCUILobbySimpleMenu m_UIAttendance;

	public NKCUILobbySimpleMenu m_UINotice;

	public NKCUIComStateButton m_csbtnInfo;

	public NKCUILobbySimpleMenu m_UIChat;

	public NKCUISurvey m_UISurvey;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public NKCUILobbyMenuShop m_UIShop;

	public NKCUILobbySimpleMenu m_UIMissionGuide;

	public GameObject m_objMissionGuideAllComplete;

	public Text m_lbMissionGuideCompleteCnt;

	public NKCUILobbyEventPanel m_UIEventPanel;

	public NKCUIComStateButton m_csbtnServiceTransferRegist;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objBottomRightMenu;

	public NKCUILobbySimpleMenu m_UIMission;

	public NKCUILobbySimpleMenu m_UICollection;

	public NKCUILobbySimpleMenu m_UIGuild;

	public NKCUILobbySimpleMenu m_UIBase;

	public NKCUILobbySimpleMenu m_UIInventory;

	public NKCUILobbySimpleMenu m_UIUnitList;

	public NKCUILobbySimpleMenu m_UIDeckSetup;

	public NKCUILobbySimpleMenu m_UIContract;

	[Header("\ufffd\u05b4\ufffd")]
	public Animator[] m_lstAnimator;

	private Dictionary<eUIMenu, NKCUILobbyMenuButtonBase> m_dicButton = new Dictionary<eUIMenu, NKCUILobbyMenuButtonBase>();

	private Dictionary<NKM_SHORTCUT_TYPE, NKCUILobbyMenuButtonBase> m_dicShortCutButton = new Dictionary<NKM_SHORTCUT_TYPE, NKCUILobbyMenuButtonBase>();

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffdư")]
	public List<NKCUILobbyEventShortCut> m_lstUIShortCutButton;

	[Header("UI\ufffd\ufffd\ufffd\ufffd")]
	public CanvasGroup m_CanvasGroup;

	public NKCUIComToggle m_tglUIHide;

	private Transform m_trHideBtnRoot;

	private bool m_hideUI;

	public static bool IsInstanceOpen
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsUIOpen;
			}
			return false;
		}
	}

	public static bool IsInstanceLoaded
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsLoadComplete;
			}
			return false;
		}
	}

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "MainLobbyV2";

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUILobbyV2>("ab_ui_nkm_ui_lobby", "NUF_HOME_PREFAB_RENEWAL", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		else
		{
			Log.Debug("[NKCUILobbyV2] Already Loaded : bundle [ab_ui_nkm_ui_lobby] ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Lobby/NKCUILobbyV2.cs", 34);
		}
		return s_LoadedUIData;
	}

	public static NKCUILobbyV2 GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUILobbyV2>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		Log.Debug("[NKCUILobbyV2] CleanupInstance ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Lobby/NKCUILobbyV2.cs", 58);
		GetInstance()?.Close();
		Log.Debug("[NKCUILobbyV2] Close", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Lobby/NKCUILobbyV2.cs", 61);
		s_LoadedUIData = null;
	}

	public void Init()
	{
		RegisterButtons();
		m_UILobby3D.Init();
		base.gameObject.SetActive(value: false);
		NKCUtil.SetGameobjectActive(m_UILobby3D, bValue: false);
		m_UIUserInfo.Init();
		m_UIEventPanel.Init();
		m_trHideBtnRoot = m_tglUIHide.transform.parent;
		NKCUtil.SetToggleValueChangedDelegate(m_tglUIHide, OnTglUIHide);
		m_tglUIHide.Select(bSelect: true, bForce: true, bImmediate: true);
	}

	public override void OnBackButton()
	{
		if (m_hideUI)
		{
			TryUIUnhide();
		}
		else
		{
			OpenQuitApplicationPopup();
		}
	}

	public override void CloseInternal()
	{
		if (m_hideUI)
		{
			TryUIUnhide();
		}
		SetPlayIntro(value: false);
		m_UILobby3D.transform.SetParent(base.transform);
		m_UILobby3D.CleanUp();
		base.gameObject.SetActive(value: false);
		CleanUpButtons();
	}

	public override void Hide()
	{
		base.Hide();
		m_CanvasGroup.alpha = 1f;
		NKCUtil.SetGameobjectActive(m_UILobby3D, bValue: false);
		m_UILobby3D.m_MenuCanvasGroup.alpha = 1f;
	}

	public override void UnHide()
	{
		base.UnHide();
		NKCUtil.SetGameobjectActive(m_UILobby3D, bValue: true);
		m_UILobby3D.SetData(NKCScenManager.CurrentUserData());
		m_UIUserInfo.SetData(NKCScenManager.CurrentUserData());
		m_UIEventPanel.CheckReddot();
		m_UIEventPanel.SetData(NKCScenManager.CurrentUserData());
		UpdateAllButtons(NKCScenManager.CurrentUserData());
		UpdateEventIntervalSlot();
		SetUIVisible(value: true);
		SetPlayIntro(value: true);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME().OnHomeEnter();
			NKCSoundManager.PlayScenMusic(NKM_SCEN_ID.NSI_HOME);
		}
	}

	private void RegisterButtons()
	{
		if (m_UIMail != null)
		{
			m_UIMail.Init();
			RegisterButton(eUIMenu.Mail, m_UIMail);
		}
		if (m_UIOption != null)
		{
			m_UIOption.Init(null, OnMenuOption);
			RegisterButton(eUIMenu.Option, m_UIOption);
		}
		if (m_btnHamburger != null)
		{
			m_btnHamburger.PointerClick.RemoveAllListeners();
			m_btnHamburger.PointerClick.AddListener(OnClickHamburger);
			NKCUtil.SetHotkey(m_btnHamburger, HotkeyEventType.HamburgerMenu, this);
		}
		if (m_UIOperation != null)
		{
			m_UIOperation.Init();
			RegisterButton(eUIMenu.Operation, m_UIOperation);
		}
		if (m_UICurrentEvent != null)
		{
			m_UICurrentEvent.Init();
			RegisterButton(eUIMenu.CurrentEvent, m_UICurrentEvent);
		}
		if (m_UIEventPass != null)
		{
			m_UIEventPass.Init(ContentsType.COUNTER_PASS);
			RegisterButton(eUIMenu.CounterPass, m_UIEventPass);
		}
		if (m_UIPVP != null)
		{
			m_UIPVP.Init(ContentsType.PVP);
			RegisterButton(eUIMenu.PVP, m_UIPVP);
		}
		if (m_UIWorldmap != null)
		{
			m_UIWorldmap.Init(ContentsType.WORLDMAP);
			RegisterButton(eUIMenu.Worldmap, m_UIWorldmap);
		}
		if (m_UIFierce != null)
		{
			m_UIFierce.Init(ContentsType.FIERCE);
			RegisterButton(eUIMenu.Fierce, m_UIFierce);
		}
		if (m_lstUIEventIndexSlot != null)
		{
			foreach (NKCUILobbyEventIndexSlot item in m_lstUIEventIndexSlot)
			{
				if (!(item == null))
				{
					item.Init();
				}
			}
		}
		if (m_UIEventBigBanner != null)
		{
			m_UIEventBigBanner.Init();
		}
		RegisterButton(m_UIAttendance, eUIMenu.Attendance, ContentsType.ATTENDANCE, ChechAttendanceEnable, OnBtnAttendance);
		RegisterButton(m_UINotice, eUIMenu.Event, ContentsType.ATTENDANCE, null, OnBtnNotice);
		RegisterButton(m_UIChat, eUIMenu.Chat, ContentsType.FRIENDS, NKCAlarmManager.CheckChatNotify, OnBtnChat);
		NKCUtil.SetGameobjectActive(m_csbtnInfo, NKCPublisherModule.PublisherType == NKCPublisherModule.ePublisherType.Zlong && (NKMContentsVersionManager.HasCountryTag(CountryTagType.TWN) || NKMContentsVersionManager.HasCountryTag(CountryTagType.CHN)));
		if (m_csbtnInfo != null)
		{
			m_csbtnInfo.PointerClick.RemoveAllListeners();
			m_csbtnInfo.PointerClick.AddListener(OnClickInfoBtn);
		}
		if (m_UISurvey != null)
		{
			m_UISurvey.Init();
			RegisterButton(eUIMenu.Survey, m_UISurvey);
		}
		if (m_UIShop != null)
		{
			m_UIShop.Init(HasSpecialItem, OnBtnShop, ContentsType.CASH_SHOP);
			RegisterButton(eUIMenu.Shop, m_UIShop);
		}
		RegisterButton(m_UIMissionGuide, eUIMenu.GuideMission, ContentsType.MISSION, HasClearableMissionGuide, OnBtnMissionGuide);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.SERVICE_TRANSFER_REGIST))
		{
			NKCUtil.SetGameobjectActive(m_csbtnServiceTransferRegist, bValue: true);
			NKCUtil.SetButtonClickDelegate(m_csbtnServiceTransferRegist, NKCServiceTransferMgr.StartServiceTransferRegistProcess);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnServiceTransferRegist, bValue: false);
		}
		RegisterButton(m_UIMission, eUIMenu.Mission, ContentsType.MISSION, HasClearableMission, OnBtnMission);
		RegisterButton(m_UICollection, eUIMenu.Collection, ContentsType.COLLECTION, HasCollectionReward, OnBtnCollection);
		RegisterButton(m_UIGuild, eUIMenu.Guild, ContentsType.GUILD, NKCAlarmManager.CheckGuildNotify, OnBtnGuild);
		RegisterButton(m_UIBase, eUIMenu.Base, ContentsType.BASE, BaseNotifyCondition, OnMenuBase);
		RegisterButton(m_UIInventory, eUIMenu.Inventory, ContentsType.None, HasInventoryNotify, OnMenuInventory);
		RegisterButton(m_UIUnitList, eUIMenu.UnitList, ContentsType.None, null, OnMenuUnitList);
		RegisterButton(m_UIDeckSetup, eUIMenu.DeckSetup, ContentsType.DECKVIEW, null, OnMenuDeckSetup);
		RegisterButton(m_UIContract, eUIMenu.Contract, ContentsType.CONTRACT, CheckFreeContract, OnBtnContract);
		List<NKCLobbyIconTemplet> availableLobbyIconTemplet = NKCLobbyIconManager.GetAvailableLobbyIconTemplet();
		if (m_lstUIShortCutButton == null || availableLobbyIconTemplet.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < m_lstUIShortCutButton.Count; i++)
		{
			if (i < availableLobbyIconTemplet.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstUIShortCutButton[i], bValue: true);
				m_lstUIShortCutButton[i].Init(null, OnBtnShortCut, availableLobbyIconTemplet[i]);
				RegisterButton(availableLobbyIconTemplet[i].m_ShortCutType, m_lstUIShortCutButton[i]);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstUIShortCutButton[i], bValue: false);
			}
		}
	}

	public void Open(NKMUserData userData)
	{
		base.gameObject.SetActive(value: true);
		m_UILobby3D.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIMidCanvas));
		NKCUtil.SetGameobjectActive(m_UILobby3D, bValue: true);
		m_UILobby3D.transform.localPosition = Vector3.zero;
		m_UILobby3D.transform.localRotation = Quaternion.identity;
		m_UILobby3D.transform.localScale = Vector3.one;
		if (userData != null)
		{
			UpdateAllButtons(userData);
			m_UILobby3D.SetData(userData);
			m_UIUserInfo.SetData(userData);
			m_UIEventPanel.SetData(userData);
			bool bValue = NKCContentManager.IsContentsUnlocked(ContentsType.LOBBY_SUBMENU);
			NKCUtil.SetGameobjectActive(m_UIEventPanel, bValue);
			NKCUtil.SetGameobjectActive(m_objBottomRightMenu, bValue);
		}
		UpdateEventIntervalSlot();
		UIOpened();
		SetUIVisible(value: true);
		SetPlayIntro(value: true);
	}

	public void RegisterButton(eUIMenu menu, NKCUILobbyMenuButtonBase button)
	{
		if (button == null)
		{
			Debug.LogError(menu.ToString() + " button null!");
		}
		else
		{
			m_dicButton.Add(menu, button);
		}
	}

	public void RegisterButton(NKCUILobbySimpleMenu button, eUIMenu menuType, ContentsType contentsType, NKCUILobbySimpleMenu.DotEnableConditionFunction conditionFunc = null, NKCUILobbySimpleMenu.OnButton onButton = null)
	{
		if (!(button == null))
		{
			button.Init(conditionFunc, onButton, contentsType);
			RegisterButton(menuType, button);
		}
	}

	public void RegisterButton(NKM_SHORTCUT_TYPE shortCut, NKCUILobbyMenuButtonBase button)
	{
		m_dicShortCutButton.Add(shortCut, button);
	}

	private NKCUILobbyMenuButtonBase GetButton(eUIMenu menu)
	{
		if (m_dicButton.TryGetValue(menu, out var value))
		{
			return value;
		}
		return null;
	}

	private NKCUILobbyMenuButtonBase GetButton(NKM_SHORTCUT_TYPE shortCut)
	{
		if (m_dicShortCutButton.TryGetValue(shortCut, out var value))
		{
			return value;
		}
		return null;
	}

	public void UpdateAllButtons(NKMUserData userData)
	{
		bool bValue = NKCContentManager.IsContentsUnlocked(ContentsType.ATTENDANCE);
		NKCUtil.SetGameobjectActive(m_UIAttendance, bValue);
		NKCUtil.SetGameobjectActive(m_UINotice, bValue);
		NKCUtil.SetLabelText(m_lbMissionGuideCompleteCnt, NKMMissionManager.GetGuideMissionClearCount().ToString());
		NKCUtil.SetGameobjectActive(m_objMissionGuideAllComplete, NKMMissionManager.IsGuideMissionAllClear());
		foreach (KeyValuePair<eUIMenu, NKCUILobbyMenuButtonBase> item in m_dicButton)
		{
			item.Value.UpdateData(userData);
		}
		foreach (KeyValuePair<NKM_SHORTCUT_TYPE, NKCUILobbyMenuButtonBase> item2 in m_dicShortCutButton)
		{
			if (item2.Value != null)
			{
				item2.Value.UpdateData(userData);
			}
		}
		UpdateHamburgerNotify();
	}

	public void UpdateButton(eUIMenu menu, NKMUserData userData)
	{
		NKCUILobbyMenuButtonBase button = GetButton(menu);
		if (button != null)
		{
			button.UpdateData(userData);
		}
		UpdateHamburgerNotify();
	}

	public void CleanUpButtons()
	{
		foreach (KeyValuePair<eUIMenu, NKCUILobbyMenuButtonBase> item in m_dicButton)
		{
			if (item.Value != null)
			{
				item.Value.CleanUp();
			}
		}
		foreach (KeyValuePair<NKM_SHORTCUT_TYPE, NKCUILobbyMenuButtonBase> item2 in m_dicShortCutButton)
		{
			if (item2.Value != null)
			{
				item2.Value.CleanUp();
			}
		}
	}

	private void OnBtnShortCut(NKCLobbyIconTemplet templet)
	{
		if (templet != null)
		{
			NKCContentManager.MoveToShortCut(templet.m_ShortCutType, templet.m_shortCutParam);
		}
	}

	private void OnClickHamburger()
	{
		NKCPopupHamburgerMenu.instance.OpenUI();
	}

	private void OnMenuOption()
	{
		NKCUIGameOption.Instance.Open(NKC_GAME_OPTION_MENU_TYPE.NORMAL);
	}

	private void OnBtnShop()
	{
		if (NKCShopCategoryTemplet.Find(NKCShopManager.ShopTabCategory.PACKAGE) != null)
		{
			NKCUIShop.Instance.Open(NKCShopManager.ShopTabCategory.PACKAGE);
		}
		else
		{
			NKCUIShop.Instance.Open(NKCShopManager.ShopTabCategory.EXCHANGE, "TAB_SUPPLY");
		}
	}

	private void OnBtnMission()
	{
		NKCUIMissionAchievement.Instance.Open();
	}

	private void OnBtnMissionGuide()
	{
		NKCUIMissionGuide.Instance.Open();
	}

	private bool HasSpecialItem(NKMUserData userData)
	{
		ShopReddotType reddotType;
		return NKCShopManager.CheckTabReddotCount(out reddotType) > 0;
	}

	private bool HasClearableMission(NKMUserData userData)
	{
		return NKMMissionManager.GetHaveClearedMission();
	}

	private bool HasClearableMissionGuide(NKMUserData userData)
	{
		return NKMMissionManager.GetHaveClearedMissionGuide();
	}

	private void OnBtnAttendance()
	{
		if (NKMAttendanceManager.IsAttendanceBlocked)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(NKM_ERROR_CODE.NEC_FAIL_SYSTEM_CONTENTS_BLOCK));
		}
		else if (NKCContentManager.IsContentsUnlocked(ContentsType.LOBBY_SUBMENU))
		{
			List<int> attendanceKeyList = NKCScenManager.GetScenManager().Get_SCEN_HOME().GetAttendanceKeyList();
			if (attendanceKeyList != null && attendanceKeyList.Count > 0)
			{
				NKCUIAttendance.Instance.Open(attendanceKeyList);
				return;
			}
			NKCScenManager.GetScenManager().GetMyUserData().m_AttendanceData = NKMAttendanceManager.AddNeedAttendanceKeyByTemplet(NKCScenManager.GetScenManager().GetMyUserData().m_AttendanceData);
			List<int> needAttendanceKey = NKMAttendanceManager.GetNeedAttendanceKey();
			NKCUIAttendance.Instance.Open(needAttendanceKey);
		}
	}

	private bool ChechAttendanceEnable(NKMUserData userData)
	{
		return NKCScenManager.GetScenManager().Get_SCEN_HOME().GetAttendanceRequired();
	}

	private void OnBtnNotice()
	{
		NKCPublisherModule.Notice.OpenNotice(null);
	}

	private bool CheckFreeContract(NKMUserData userData)
	{
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		if (nKCContractDataMgr == null)
		{
			return false;
		}
		bool flag = nKCContractDataMgr.IsActiveNewFreeChance();
		if (!nKCContractDataMgr.PossibleFreeContract && !flag)
		{
			return false;
		}
		if (flag || nKCContractDataMgr.IsPossibleFreeChance())
		{
			return true;
		}
		return false;
	}

	private void OnBtnContract()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CONTRACT, bForce: false);
	}

	private void OnClickInfoBtn()
	{
		NKCPublisherModule.Notice.OpenInfoWindow(null);
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

	private bool BaseNotifyCondition(NKMUserData userData)
	{
		if (userData?.m_CraftData?.SlotList == null)
		{
			return false;
		}
		foreach (KeyValuePair<byte, NKMCraftSlotData> slot in userData.m_CraftData.SlotList)
		{
			if (slot.Value.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED)
			{
				return true;
			}
		}
		if (NKCAlarmManager.CheckScoutNotify(userData))
		{
			return true;
		}
		return false;
	}

	private void OnMenuBase()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OFFICE, bForce: false);
	}

	private void OnMenuDeckSetup()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_TEAM, bForce: false);
	}

	private void UpdateHamburgerNotify()
	{
		NKCUtil.SetGameobjectActive(m_objHamburgerNotify, NKCAlarmManager.CheckAllNotify(NKCScenManager.CurrentUserData()));
	}

	public void RefreshUserBuff()
	{
		m_UIUserInfo.UpdateUserBuffCount(NKCScenManager.CurrentUserData());
	}

	public bool HasCollectionReward(NKMUserData userData)
	{
		NKCCollectionManager.Init();
		NKMArmyData nKMArmyData = userData?.m_ArmyData;
		if (nKMArmyData == null)
		{
			return false;
		}
		int hasTeamUpCount = 0;
		int totalTeamUpCount = 0;
		NKCUICollectionTeamUp.UpdateTeamUpList(ref hasTeamUpCount, ref totalTeamUpCount, nKMArmyData, getTeamUpList: false, out var bNotify);
		if (bNotify && NKCUnitMissionManager.GetOpenTagCollectionTeamUp())
		{
			return true;
		}
		if (NKCUnitMissionManager.HasRewardEnableMission())
		{
			return true;
		}
		if (NKCCollectionManager.IsMiscCollectionOpened && NKCCollectionManager.IsMiscRewardEnable())
		{
			return true;
		}
		return false;
	}

	public void OnBtnCollection()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_COLLECTION);
	}

	public void OnBtnGuild()
	{
		if (NKCGuildManager.HasGuild())
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_INTRO);
		}
	}

	public bool HasInventoryNotify(NKMUserData userData)
	{
		foreach (NKMItemMiscData value in NKCScenManager.CurrentUserData().m_InventoryData.MiscItems.Values)
		{
			if (value.TotalCount > 0)
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(value.ItemID);
				if (itemMiscTempletByID != null && itemMiscTempletByID.IsUsable() && itemMiscTempletByID.IsTimeIntervalItem)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void OnMenuInventory()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_INVENTORY, bForce: false);
	}

	public void OnMenuUnitList()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_UNIT_LIST, bForce: false);
	}

	public bool UseCameraTracking()
	{
		return m_UILobby3D.CameraTracking();
	}

	public void RefreshNickname()
	{
		if (m_UIUserInfo != null)
		{
			m_UIUserInfo.RefreshNickname();
		}
	}

	public void RefreshRechargeEternium()
	{
		if (m_UIUserInfo != null)
		{
			m_UIUserInfo.RefreshRechargeEternium();
		}
	}

	public void SetEventPanelAutoScroll(bool value)
	{
		if (m_UIEventPanel != null && m_UIEventPanel.m_EventSlidePanel != null)
		{
			m_UIEventPanel.m_EventSlidePanel.m_bAutoMove = value;
		}
	}

	private void Update()
	{
	}

	public void TryUIUnhide()
	{
		if (m_hideUI)
		{
			SetUIVisible(value: true);
			SetPlayIntro(value: true);
		}
	}

	private void OnTglUIHide(bool value)
	{
		SetPlayIntro(value);
		SetUIVisible(value);
	}

	public void SetUIVisible(bool value)
	{
		if (value)
		{
			if (m_hideUI)
			{
				m_hideUI = false;
				m_CanvasGroup.alpha = 1f;
				m_UILobby3D.m_MenuCanvasGroup.alpha = 1f;
				NKCUtil.SetGameobjectActive(m_UILobby3D.m_MenuCanvasGroup.gameObject, bValue: true);
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
				NKCUtil.SetGameobjectActive(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), bValue: true);
				NKCUtil.SetGameobjectActive(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), bValue: true);
				if (m_tglUIHide != null)
				{
					m_tglUIHide.transform.SetParent(m_trHideBtnRoot, worldPositionStays: true);
					m_tglUIHide.Select(bSelect: true, bForce: true);
				}
				RefreshRechargeEternium();
			}
		}
		else
		{
			m_hideUI = true;
			NKCUtil.SetGameobjectActive(m_UILobby3D.m_MenuCanvasGroup.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), bValue: false);
			NKCUtil.SetGameobjectActive(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), bValue: false);
			if (m_tglUIHide != null)
			{
				m_tglUIHide.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommonLow), worldPositionStays: true);
				m_tglUIHide.Select(bSelect: false, bForce: true);
			}
		}
	}

	private void OpenQuitApplicationPopup()
	{
		if (NKCPublisherModule.Auth.CheckExitCallFirst())
		{
			NKCPublisherModule.Auth.Exit(OnCompleteExitFirst);
		}
		else
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_LOBBY_CHECK_QUIT_GAME, OnQuit);
		}
	}

	private void OnQuit()
	{
		NKMPopUpBox.OpenWaitBox();
		NKCPublisherModule.Auth.Exit(OnCompleteExit);
	}

	private void OnCompleteExit(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError))
		{
			Application.Quit();
		}
	}

	private void OnCompleteExitFirst(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError))
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_LOBBY_CHECK_QUIT_GAME, delegate
			{
				Application.Quit();
			});
		}
	}

	public override void OnScreenResolutionChanged()
	{
		base.OnScreenResolutionChanged();
		m_UILobby3D.AdjustPositionByScreenRatio();
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		m_UIUserInfo.OnResourceValueChange(itemData);
		UpdateButton(eUIMenu.Inventory, NKCScenManager.CurrentUserData());
		m_UIUserInfo.UpdateUserBuffCount(NKCScenManager.CurrentUserData());
	}

	public override void OnEquipChange(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipItem)
	{
		switch (eType)
		{
		case NKMUserData.eChangeNotifyType.Add:
		case NKMUserData.eChangeNotifyType.Remove:
			UpdateButton(eUIMenu.Inventory, NKCScenManager.CurrentUserData());
			break;
		case NKMUserData.eChangeNotifyType.Update:
			break;
		}
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		switch (eEventType)
		{
		case NKMUserData.eChangeNotifyType.Add:
		case NKMUserData.eChangeNotifyType.Remove:
			UpdateButton(eUIMenu.UnitList, NKCScenManager.CurrentUserData());
			break;
		case NKMUserData.eChangeNotifyType.Update:
			break;
		}
	}

	public override void OnUserLevelChanged(NKMUserData newUserData)
	{
		m_UIUserInfo.UpdateLevelAndExp(newUserData);
		m_UIUserInfo.UpdateUserBuffCount(NKCScenManager.CurrentUserData());
		UpdateButton(eUIMenu.Guild, NKCScenManager.CurrentUserData());
		UpdateHamburgerNotify();
	}

	public override void OnGuildDataChanged()
	{
		m_UIUserInfo.SetGuildData();
		GetButton(eUIMenu.Guild)?.UpdateData(NKCScenManager.CurrentUserData());
	}

	public override void OnMissionUpdated()
	{
		UpdateHamburgerNotify();
	}

	private void SetPlayIntro(bool value)
	{
		Animator[] lstAnimator;
		if (value)
		{
			lstAnimator = m_lstAnimator;
			foreach (Animator animator in lstAnimator)
			{
				if (!(animator == null))
				{
					animator.SetTrigger("Intro");
				}
			}
			return;
		}
		lstAnimator = m_lstAnimator;
		foreach (Animator animator2 in lstAnimator)
		{
			if (!(animator2 == null))
			{
				animator2.ResetTrigger("Intro");
			}
		}
	}

	private void UpdateEventIntervalSlot()
	{
		List<NKCLobbyEventIndexTemplet> currentLobbyEvents = NKCLobbyEventIndexTemplet.GetCurrentLobbyEvents();
		int i = 0;
		if (NKCPublisherModule.Instance.IsReviewServer())
		{
			NKCUtil.SetGameobjectActive(m_UIEventBigBanner, bValue: false);
			for (int j = 0; j < m_lstUIEventIndexSlot.Count; j++)
			{
				NKCUtil.SetGameobjectActive(m_lstUIEventIndexSlot[j], bValue: false);
			}
			return;
		}
		for (int k = 0; k < currentLobbyEvents.Count; k++)
		{
			if (currentLobbyEvents[k].bBigBanner)
			{
				NKCUtil.SetGameobjectActive(m_UIEventBigBanner, bValue: true);
				if (m_UIEventBigBanner != null)
				{
					m_UIEventBigBanner.SetData(currentLobbyEvents[k]);
				}
			}
			else if (m_lstUIEventIndexSlot.Count > i)
			{
				NKCUILobbyEventIndexSlot nKCUILobbyEventIndexSlot = m_lstUIEventIndexSlot[i];
				if (!(nKCUILobbyEventIndexSlot == null))
				{
					NKCUtil.SetGameobjectActive(nKCUILobbyEventIndexSlot, bValue: true);
					nKCUILobbyEventIndexSlot.SetData(currentLobbyEvents[k]);
					i++;
				}
			}
		}
		for (; i < m_lstUIEventIndexSlot.Count; i++)
		{
			NKCUILobbyEventIndexSlot nKCUILobbyEventIndexSlot2 = m_lstUIEventIndexSlot[i];
			if (!(nKCUILobbyEventIndexSlot2 == null))
			{
				NKCUtil.SetGameobjectActive(nKCUILobbyEventIndexSlot2, bValue: true);
				nKCUILobbyEventIndexSlot2.SetData(null);
			}
		}
	}
}
