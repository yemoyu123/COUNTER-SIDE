using System;
using System.Collections;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Office;
using Cs.Protocol;
using NKC.Office;
using NKC.Templet;
using NKC.UI.Component.Office;
using NKC.UI.NPC;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIOffice : NKCUIBase
{
	[Serializable]
	public class NPCInfo
	{
		public NKMOfficeRoomTemplet.RoomType Type;

		public Vector2 Offset;

		public string BundleName;
	}

	private enum Mode
	{
		Normal,
		Edit,
		EditAdd,
		EditMove,
		Facility,
		Hide,
		Visit,
		Preview
	}

	private enum PresetMode
	{
		MyPreset,
		Theme
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_office";

	private const string UI_ASSET_NAME = "AB_UI_OFFICE";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public NKCOfficeBuilding m_OfficeBuilding;

	private NKCOfficeFacility m_NKCUIOfficeFacility;

	[Header("상단 바")]
	public NKCUIOfficeUpsideMenu m_uiOfficeUpsideMenu;

	[Header("미니맵")]
	public NKCUIComStateButton m_csbtnMinimap;

	[Header("기본 UI")]
	public GameObject m_objNormalMode;

	public GameObject m_objNormalButtons;

	public NKCUIComStateButton m_csbtnShop;

	public NKCUIComStateButton m_csbtnEditMode;

	public NKCUIComStateButton m_csbtnWarehouse;

	public NKCUIComStateButton m_csbtnDeployUnit;

	public NKCUIComStateButton m_csbtnCommunity;

	public GameObject m_objCommunityReddot;

	public NKCUIComStateButton m_csbtnParty;

	[Header("기본 UI - 방문")]
	public GameObject m_objVisitButtons;

	public NKCUIComStateButton m_csbtnToMyOffice;

	public NKCUIComStateButton m_csbtnRandomVisit;

	public NKCUIComStateButton m_csbtnSendBizCard;

	public Text m_lbSendBizCardCount;

	[Header("환경점수")]
	public NKCUIComOfficeEnvScore m_comEnvScore;

	[Header("시설 UI")]
	public RectTransform m_rtNPCRoot;

	public NKCUIOfficeFacilityButtons m_UIOfficeFacilityButtons;

	public List<NPCInfo> m_lstNPCInfo;

	private List<NKCASUIUnitIllust> m_lstNPCIllust = new List<NKCASUIUnitIllust>();

	[Header("메뉴 숨기기 관련")]
	public NKCUIComStateButton m_csbtnHideMenu;

	public NKCUIComStateButton m_csbtnUnhideMenu;

	public GameObject m_objHideMenu;

	[Header("편집 모드 관련")]
	public GameObject m_objEditMode;

	public NKCUIComStateButton m_csbtnEditInfo;

	public NKCUIComStateButton m_csbtnEditInvert;

	public NKCUIComStateButton m_csbtnEditStore;

	public NKCUIComStateButton m_csbtnEditStoreAll;

	public NKCUIComStateButton m_csbtnEditSave;

	public NKCUIComStateButton m_csbtnEditWarehouse;

	public NKCUIComStateButton m_csbtnEditClose;

	public NKCUIComStateButton m_csbtnEditCopyPreset;

	public NKCUIComOfficeEnvScore m_comEditEnvScore;

	public GameObject m_objEditFurnitureRoot;

	public Text m_lbEditFurnitureEnvScore;

	public Text m_lbEditFurnitureName;

	public GameObject m_goLastFuniture;

	public NKCUISlot m_slotLastFuniture;

	[Header("프리셋/프리뷰 모드 관련")]
	public NKCUIComStateButton m_csbtnPresetList;

	public GameObject m_objPreviewMode;

	public NKCUIComStateButton m_csbtnPreviewInteriorList;

	public NKCUIComStateButton m_csbtnPreviewOK;

	public NKCUIComStateButton m_csbtnPreviewCancel;

	private NKMOfficeRoomTemplet m_currentRoomTemplet;

	private NKCOfficeRoomData m_currentRoom;

	private Mode m_eMode;

	private List<NKMUserProfileData> m_lstVisitor = new List<NKMUserProfileData>();

	private PresetMode m_ePresetMode;

	private int m_currentPresetId = -1;

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

	public NKCUIOfficeFacilityButtons OfficeFacilityInterfaces => m_UIOfficeFacilityButtons;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "사옥";

	public int RoomID
	{
		get
		{
			if (m_currentRoom == null)
			{
				return 0;
			}
			return m_currentRoom.ID;
		}
	}

	private long CurrentVisitUID
	{
		get
		{
			if (m_currentRoom == null)
			{
				return 0L;
			}
			return m_currentRoom.m_OwnerUID;
		}
	}

	private bool IsVisiting => CurrentVisitUID > 0;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIOffice>("ab_ui_office", "AB_UI_OFFICE", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIOffice GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIOffice>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		NKCSoundManager.PlayScenMusic(NKCScenManager.GetScenManager().GetNowScenID());
		CleanUp();
	}

	public override void OnCloseInstance()
	{
		CleanUp();
	}

	private void CleanUp()
	{
		if (m_OfficeBuilding != null)
		{
			m_OfficeBuilding.transform.SetParent(base.transform);
			m_OfficeBuilding.CleanUp();
		}
		if (m_NKCUIOfficeFacility != null)
		{
			m_NKCUIOfficeFacility.transform.SetParent(base.transform);
			m_NKCUIOfficeFacility.CleanUp();
			UnityEngine.Object.Destroy(m_NKCUIOfficeFacility.gameObject);
			m_NKCUIOfficeFacility = null;
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		PlayRoomMusic();
	}

	public override void OnBackButton()
	{
		switch (m_eMode)
		{
		case Mode.Preview:
			CancelPreview();
			break;
		case Mode.EditAdd:
		case Mode.EditMove:
			ChangeMode(Mode.Edit, -1, -1L);
			break;
		case Mode.Hide:
			HideUI(value: false);
			break;
		case Mode.Edit:
			ChangeMode(Mode.Normal, -1, -1L);
			break;
		default:
			Close();
			break;
		}
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		switch (hotkey)
		{
		case HotkeyEventType.ShowHotkey:
			if (m_eMode == Mode.Normal || m_eMode == Mode.Visit)
			{
				NKCUIComHotkeyDisplay.OpenInstance(m_uiOfficeUpsideMenu.m_RoomInfoBg, HotkeyEventType.NextTab);
			}
			break;
		case HotkeyEventType.NextTab:
			if (m_eMode == Mode.Normal || m_eMode == Mode.Visit)
			{
				m_uiOfficeUpsideMenu.OnBtnRightMove();
			}
			break;
		case HotkeyEventType.PrevTab:
			if (m_eMode == Mode.Normal || m_eMode == Mode.Visit)
			{
				m_uiOfficeUpsideMenu.OnBtnLeftMove();
			}
			break;
		}
		return false;
	}

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnMinimap, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnShop, OnBtnShop);
		NKCUtil.SetButtonClickDelegate(m_csbtnEditMode, OnBtnEditMode);
		NKCUtil.SetButtonClickDelegate(m_csbtnWarehouse, OnBtnWarehouse);
		NKCUtil.SetButtonClickDelegate(m_csbtnDeployUnit, OnBtnDiployUnit);
		NKCUtil.SetButtonClickDelegate(m_csbtnCommunity, OnBtnCommunity);
		NKCUtil.SetButtonClickDelegate(m_csbtnParty, OnBtnParty);
		if (m_csbtnParty != null)
		{
			m_csbtnParty.m_bGetCallbackWhileLocked = true;
		}
		NKCUtil.SetGameobjectActive(m_csbtnParty, NKMOpenTagManager.IsOpened("OFFICE_PARTY"));
		NKCUtil.SetButtonClickDelegate(m_csbtnToMyOffice, OnBtnToMyOffice);
		NKCUtil.SetButtonClickDelegate(m_csbtnRandomVisit, OnBtnRandomVisit);
		NKCUtil.SetButtonClickDelegate(m_csbtnSendBizCard, OnBtnSendBizCard);
		NKCUtil.SetButtonClickDelegate(m_csbtnEditInfo, OnBtnEditInfo);
		NKCUtil.SetButtonClickDelegate(m_csbtnEditInvert, OnBtnEditInvert);
		NKCUtil.SetHotkey(m_csbtnEditInvert, HotkeyEventType.NextTab);
		NKCUtil.SetButtonClickDelegate(m_csbtnEditStore, OnBtnEditStore);
		NKCUtil.SetButtonClickDelegate(m_csbtnEditStoreAll, OnBtnEditStoreAll);
		NKCUtil.SetButtonClickDelegate(m_csbtnEditSave, OnBtnEditSave);
		NKCUtil.SetHotkey(m_csbtnEditSave, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnEditWarehouse, OnBtnWarehouse);
		NKCUtil.SetButtonClickDelegate(m_csbtnEditClose, OnBtnEditClose);
		NKCUtil.SetButtonClickDelegate(m_csbtnEditCopyPreset, OnBtnEditCopyPreset);
		NKCUtil.SetGameobjectActive(m_csbtnEditCopyPreset, NKCScenManager.CurrentUserData().IsSuperUser());
		NKCUtil.SetButtonClickDelegate(m_csbtnPresetList, OnBtnPresetList);
		NKCUtil.SetGameobjectActive(m_csbtnPresetList, bValue: true);
		NKCUtil.SetButtonClickDelegate(m_csbtnPreviewOK, TryApplyPreset);
		NKCUtil.SetHotkey(m_csbtnPreviewOK, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnPreviewCancel, OnBtnPreviewCancel);
		NKCUtil.SetButtonClickDelegate(m_csbtnPreviewInteriorList, OnPresetInteriorList);
		NKCUtil.SetButtonClickDelegate(m_csbtnHideMenu, OnBtnHideMenu);
		NKCUtil.SetButtonClickDelegate(m_csbtnUnhideMenu, OnBtnUnhideMenu);
		if (m_slotLastFuniture != null)
		{
			m_slotLastFuniture.Init();
			m_slotLastFuniture.SetHotkey(HotkeyEventType.NextTab);
		}
		NKCUtil.SetGameobjectActive(m_goLastFuniture, bValue: false);
		m_OfficeBuilding.Init(OnSelectFuniture);
		m_UIOfficeFacilityButtons.Init(base.Close);
		m_uiOfficeUpsideMenu?.Init();
	}

	public void Preload()
	{
	}

	private void CleanupRooms()
	{
		if (m_OfficeBuilding != null)
		{
			m_OfficeBuilding.CleanUp();
			m_OfficeBuilding.gameObject.SetActive(value: false);
		}
		if (m_NKCUIOfficeFacility != null)
		{
			m_NKCUIOfficeFacility.CleanUp();
			UnityEngine.Object.Destroy(m_NKCUIOfficeFacility.gameObject);
		}
	}

	public void Open(long uid, int roomID)
	{
		if (NKCScenManager.CurrentUserData().OfficeData.GetFriendRoom(uid, roomID) == null)
		{
			Debug.LogError("Office room not unlocked");
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_OFFICE_FRIEND_CANNOT_VISIT"), delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
		}
		else
		{
			CleanupRooms();
			OpenFriendRoom(uid, roomID);
			UIOpened();
		}
	}

	private void OpenFriendRoom(long uid, int roomID)
	{
		NKMOfficeRoom friendRoom = NKCScenManager.CurrentUserData().OfficeData.GetFriendRoom(uid, roomID);
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(roomID);
		if (nKMOfficeRoomTemplet == null)
		{
			return;
		}
		if (m_OfficeBuilding == null)
		{
			Debug.LogError("Office building object not set");
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GAME_LOAD_FAILED, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return;
		}
		m_OfficeBuilding.CleanUp();
		m_OfficeBuilding.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIMidCanvas));
		m_OfficeBuilding.transform.position = new Vector3(0f, 0f, 1000f);
		m_OfficeBuilding.transform.localScale = Vector3.one;
		NKCUtil.SetGameobjectActive(m_OfficeBuilding, bValue: true);
		m_currentRoom = new NKCOfficeRoomData(friendRoom, uid);
		m_currentRoomTemplet = nKMOfficeRoomTemplet;
		m_lstVisitor.Clear();
		m_OfficeBuilding.SetRoomData(m_currentRoom, m_lstVisitor);
		m_OfficeBuilding.SetCameraOffset(0f);
		m_OfficeBuilding.SetCamera();
		UpdateEnvScore();
		ChangeMode(Mode.Visit, -1, -1L);
	}

	public void Open(int roomID)
	{
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(roomID);
		if (nKMOfficeRoomTemplet != null)
		{
			CleanupRooms();
			if (nKMOfficeRoomTemplet.IsFacility)
			{
				OpenFacility(nKMOfficeRoomTemplet);
			}
			else
			{
				OpenRoom(nKMOfficeRoomTemplet);
			}
			UIOpened();
		}
	}

	public void MoveToRoom(int roomID)
	{
		StartCoroutine(MoveRoomProcess(roomID));
	}

	private IEnumerator MoveRoomProcess(int roomID)
	{
		if (RoomID == roomID)
		{
			yield break;
		}
		NKMOfficeRoomTemplet roomTemplet = NKMOfficeRoomTemplet.Find(roomID);
		if (roomTemplet != null)
		{
			NKCUIFadeInOut.FadeOut(0.1f, null, bWhite: false, 3f);
			while (!NKCUIFadeInOut.IsFinshed())
			{
				yield return null;
			}
			CleanupRooms();
			if (roomTemplet.IsFacility)
			{
				OpenFacility(roomTemplet);
			}
			else if (IsVisiting)
			{
				OpenFriendRoom(CurrentVisitUID, roomID);
			}
			else
			{
				OpenRoom(roomTemplet);
			}
			NKCUIFadeInOut.FadeIn(0.1f);
		}
	}

	private void OpenRoom(NKMOfficeRoomTemplet roomTemplet)
	{
		if (m_OfficeBuilding == null)
		{
			Debug.LogError("Office building object not set");
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GAME_LOAD_FAILED, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return;
		}
		if (NKCScenManager.CurrentUserData()?.OfficeData == null)
		{
			Debug.LogError("Office data not set");
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GAME_LOAD_FAILED, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return;
		}
		NKMOfficeRoom officeRoom = NKCScenManager.CurrentUserData().OfficeData.GetOfficeRoom(roomTemplet.ID);
		if (officeRoom == null)
		{
			Debug.LogError("Office room not unlocked");
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_OFFICE_NOT_OPEND_ROOM, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return;
		}
		m_OfficeBuilding.CleanUp();
		LoadNPCSpineIllust(roomTemplet.Type, m_rtNPCRoot);
		m_OfficeBuilding.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIMidCanvas));
		m_OfficeBuilding.transform.position = new Vector3(0f, 0f, 1000f);
		m_OfficeBuilding.transform.localScale = Vector3.one;
		NKCUtil.SetGameobjectActive(m_OfficeBuilding, bValue: true);
		m_currentRoom = new NKCOfficeRoomData(officeRoom, 0L);
		m_currentRoomTemplet = roomTemplet;
		if (roomTemplet.ID == 1 && !IsVisiting)
		{
			m_lstVisitor = NKCScenManager.CurrentUserData().OfficeData.GetRandomVisitor(4);
		}
		else
		{
			m_lstVisitor.Clear();
		}
		m_OfficeBuilding.SetRoomData(m_currentRoom, m_lstVisitor);
		m_OfficeBuilding.SetCameraOffset(0f);
		m_OfficeBuilding.SetCamera();
		if (roomTemplet.ID == 1 && !IsVisiting)
		{
			m_OfficeBuilding.AddNPC("ab_unit_office_sd@UNIT_OFFICE_SD_NPC_LOBBY", "AB_UNIT_SD_SPINE_NKM_UNIT_OFFICE_KIM_HANA", "OFFICE_BT_IDLE", new Vector3(700f, 0f));
		}
		UpdateEnvScore();
		ChangeMode(Mode.Normal, -1, -1L);
		NKCTutorialManager.TutorialRequired(TutorialPoint.OfficeRoom);
	}

	private void OpenFacility(NKMOfficeRoomTemplet roomTemplet)
	{
		if (m_NKCUIOfficeFacility != null)
		{
			m_NKCUIOfficeFacility.CleanUp();
			UnityEngine.Object.Destroy(m_NKCUIOfficeFacility.gameObject);
		}
		m_currentRoomTemplet = roomTemplet;
		m_NKCUIOfficeFacility = NKCOfficeFacility.GetInstance(roomTemplet);
		if (m_NKCUIOfficeFacility == null)
		{
			Debug.LogError($"Facility load failed! ID : {roomTemplet.ID}, resource : {roomTemplet.FacilityPrefab}");
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_NOT_FOUND, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return;
		}
		m_NKCUIOfficeFacility.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIMidCanvas));
		m_NKCUIOfficeFacility.transform.position = new Vector3(0f, 0f, 1000f);
		m_NKCUIOfficeFacility.transform.localScale = Vector3.one;
		m_NKCUIOfficeFacility.CalculateRoomSize();
		LoadNPCSpineIllust(roomTemplet.Type, m_rtNPCRoot);
		if (m_lstNPCIllust.Count > 0)
		{
			m_NKCUIOfficeFacility.SetCameraOffset(m_rtNPCRoot.GetWidth());
		}
		else
		{
			m_NKCUIOfficeFacility.SetCameraOffset(0f);
		}
		m_NKCUIOfficeFacility.SetCamera();
		ChangeMode(Mode.Facility, -1, -1L);
		switch (roomTemplet.Type)
		{
		case NKMOfficeRoomTemplet.RoomType.CEO:
			NKCTutorialManager.TutorialRequired(TutorialPoint.OfficeCEO);
			break;
		case NKMOfficeRoomTemplet.RoomType.Forge:
			NKCTutorialManager.TutorialRequired(TutorialPoint.OfficeFactory);
			break;
		case NKMOfficeRoomTemplet.RoomType.Hangar:
			NKCTutorialManager.TutorialRequired(TutorialPoint.OfficeHangar);
			break;
		case NKMOfficeRoomTemplet.RoomType.Lab:
			NKCTutorialManager.TutorialRequired(TutorialPoint.OfficeLab);
			break;
		case NKMOfficeRoomTemplet.RoomType.Terrabrain:
			NKCTutorialManager.TutorialRequired(TutorialPoint.OfficeTerrabrain);
			break;
		}
	}

	private void OnFunitureAddSelected(int id)
	{
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMItemMiscTemplet.FindInterior(id);
		if (nKMOfficeInteriorTemplet != null)
		{
			switch (nKMOfficeInteriorTemplet.InteriorCategory)
			{
			case InteriorCategory.FURNITURE:
				ChangeMode(Mode.EditAdd, id, -1L);
				break;
			case InteriorCategory.DECO:
				TryApplyDecoration(nKMOfficeInteriorTemplet);
				break;
			}
		}
	}

	private void ChangeMode(Mode mode, int ID = -1, long uid = -1L)
	{
		switch (m_eMode)
		{
		case Mode.EditAdd:
			if (m_OfficeBuilding.HasSelection)
			{
				m_OfficeBuilding.ClearSelection();
			}
			break;
		case Mode.EditMove:
			if (m_OfficeBuilding.HasSelection)
			{
				m_OfficeBuilding.CancelMoveFuniture();
			}
			break;
		}
		switch (mode)
		{
		case Mode.EditAdd:
		{
			NKM_ERROR_CODE nKM_ERROR_CODE2 = m_OfficeBuilding.AddFunitureMode(ID);
			if (nKM_ERROR_CODE2 != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE2);
				mode = Mode.Normal;
			}
			break;
		}
		case Mode.EditMove:
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = m_OfficeBuilding.MoveFunitureMode(uid);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
				mode = Mode.Normal;
			}
			break;
		}
		}
		m_eMode = mode;
		if (mode == Mode.Facility && m_currentRoomTemplet != null)
		{
			m_UIOfficeFacilityButtons.SetMode(m_currentRoomTemplet.Type);
		}
		bool flag = false;
		switch (mode)
		{
		case Mode.Facility:
			m_uiOfficeUpsideMenu?.SetState(NKCUIOfficeUpsideMenu.MenuState.Facility, m_currentRoomTemplet);
			break;
		default:
			CheckCommunityReddot();
			m_uiOfficeUpsideMenu?.SetState(NKCUIOfficeUpsideMenu.MenuState.Room, m_currentRoomTemplet);
			break;
		case Mode.Edit:
		case Mode.EditAdd:
		case Mode.EditMove:
			flag = true;
			UpdateEditButtons();
			m_uiOfficeUpsideMenu?.SetState(NKCUIOfficeUpsideMenu.MenuState.Decoration, m_currentRoomTemplet);
			break;
		case Mode.Visit:
			UpdatePostState();
			m_uiOfficeUpsideMenu?.SetState(NKCUIOfficeUpsideMenu.MenuState.Room, m_currentRoomTemplet);
			break;
		}
		if (m_OfficeBuilding != null)
		{
			m_OfficeBuilding.SetEnableUnitTouch(!flag);
			m_OfficeBuilding.SetEnableUnitExtraUI(mode == Mode.Normal);
		}
		NKCUtil.SetGameobjectActive(m_uiOfficeUpsideMenu, mode != Mode.Hide && mode != Mode.Preview);
		NKCUtil.SetGameobjectActive(m_objHideMenu, mode == Mode.Hide);
		NKCUtil.SetGameobjectActive(m_objNormalMode, mode == Mode.Normal || mode == Mode.Visit);
		NKCUtil.SetGameobjectActive(m_objPreviewMode, mode == Mode.Preview);
		NKCUtil.SetGameobjectActive(m_objNormalButtons, mode == Mode.Normal);
		NKCUtil.SetGameobjectActive(m_objVisitButtons, mode == Mode.Visit);
		NKCUtil.SetGameobjectActive(m_csbtnParty, mode == Mode.Normal && NKMOpenTagManager.IsOpened("OFFICE_PARTY"));
		if (m_csbtnParty != null && m_currentRoom != null)
		{
			m_csbtnParty.SetLock(m_currentRoom.m_lstUnitUID.Count == 0);
		}
		NKCUtil.SetGameobjectActive(m_UIOfficeFacilityButtons, mode == Mode.Facility);
		NKCUtil.SetGameobjectActive(m_objEditMode, flag);
		UpdateEnvScore();
		UpdateUpsideMenu();
		PlayRoomMusic();
	}

	private void HideUI(bool value)
	{
		if (value)
		{
			ChangeMode(Mode.Hide, -1, -1L);
		}
		else if (IsVisiting)
		{
			ChangeMode(Mode.Visit, -1, -1L);
		}
		else if (m_currentRoomTemplet != null && m_currentRoomTemplet.IsFacility)
		{
			ChangeMode(Mode.Facility, -1, -1L);
		}
		else
		{
			ChangeMode(Mode.Normal, -1, -1L);
		}
	}

	private void SetLastFurniture(int newItemID)
	{
		if (m_slotLastFuniture != null)
		{
			NKCUtil.SetGameobjectActive(m_goLastFuniture, bValue: true);
			long freeInteriorCount = NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(newItemID);
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(newItemID, freeInteriorCount);
			m_slotLastFuniture.SetData(data, bShowName: false, bShowNumber: true, bEnableLayoutElement: false, OnRepeatLastAdd);
			m_slotLastFuniture.SetDisable(freeInteriorCount <= 0);
		}
	}

	private void UpdateLastFurniture()
	{
		if (!(m_goLastFuniture == null) && m_goLastFuniture.activeInHierarchy && m_slotLastFuniture != null)
		{
			NKCUISlot.SlotData slotData = m_slotLastFuniture.GetSlotData();
			long num = (slotData.Count = NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(slotData.ID));
			m_slotLastFuniture.SetData(slotData, bShowName: false, bShowNumber: true, bEnableLayoutElement: false, OnRepeatLastAdd);
			m_slotLastFuniture.SetDisable(num <= 0);
		}
	}

	private void TryFurnitureAdd(int id, BuildingFloor target, int x, int y, bool bInvert)
	{
		Debug.Log($"TryFurnitureAdd : {id} {target}({x},{y}) invert : {bInvert}");
		NKCOfficeFunitureData funitureData = new NKCOfficeFunitureData(-1L, id, target, x, y, bInvert);
		NKM_ERROR_CODE nKM_ERROR_CODE = m_currentRoom.CanAddFuniture(funitureData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_OFFICE_ADD_FURNITURE_REQ(RoomID, id, (OfficePlaneType)target, x, y, bInvert);
		}
	}

	public void OnAddFurniture(int roomID, NKMOfficeFurniture nkmFurniture)
	{
		if (roomID == RoomID)
		{
			NKCOfficeFunitureData funitureData = new NKCOfficeFunitureData(nkmFurniture);
			m_currentRoom.AddFuniture(funitureData);
			SetLastFurniture(nkmFurniture.itemId);
			m_OfficeBuilding.AddFuniture(funitureData);
			ChangeMode(Mode.Edit, -1, -1L);
		}
	}

	private void OnRepeatLastAdd(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(slotData.ID) <= 0)
		{
			NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_OFFICE_FURNITURE_NOT_REMAINS);
		}
		else
		{
			OnFunitureAddSelected(slotData.ID);
		}
	}

	private void OnSelectFuniture(int id, long uid)
	{
		switch (m_eMode)
		{
		case Mode.Edit:
			OnFunitureMoveMode(id, uid);
			break;
		case Mode.Normal:
		case Mode.Facility:
		case Mode.Hide:
			m_OfficeBuilding.TouchFurniture(uid);
			break;
		case Mode.EditAdd:
		case Mode.EditMove:
			break;
		}
	}

	private void OnFunitureMoveMode(int id, long uid)
	{
		ChangeMode(Mode.EditMove, id, uid);
	}

	private void TryFurnitureMove(long uid, BuildingFloor target, int x, int y, bool bInvert)
	{
		Debug.Log($"TryFunitureMove : {uid} {target}({x},{y}) invert : {bInvert}");
		NKM_ERROR_CODE nKM_ERROR_CODE = m_currentRoom.CanMoveFuniture(uid, target, x, y, bInvert);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_OFFICE_UPDATE_FURNITURE_REQ(RoomID, uid, (OfficePlaneType)target, x, y, bInvert);
		}
	}

	public void OnFurnitureMove(int roomID, NKMOfficeFurniture nkmOfficeFurniture)
	{
		if (roomID == RoomID)
		{
			NKCOfficeFunitureData nKCOfficeFunitureData = new NKCOfficeFunitureData(nkmOfficeFurniture);
			m_currentRoom.MoveFuniture(nKCOfficeFunitureData.uid, nKCOfficeFunitureData.eTarget, nKCOfficeFunitureData.PosX, nKCOfficeFunitureData.PosY, nKCOfficeFunitureData.bInvert);
			m_OfficeBuilding.MoveFuniture(nKCOfficeFunitureData);
			ChangeMode(Mode.Edit, -1, -1L);
		}
	}

	private void TryRemoveFurniture(long uid)
	{
		Debug.Log($"TryFurnitureRemove : {uid}");
		NKM_ERROR_CODE nKM_ERROR_CODE = m_currentRoom.CanRemoveFurniture(uid);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
		}
		NKCPacketSender.Send_NKMPacket_OFFICE_REMOVE_FURNITURE_REQ(RoomID, uid);
	}

	public void OnRemoveFurniture(int roomID, long uid)
	{
		if (roomID == RoomID)
		{
			m_currentRoom.RemoveFuniture(uid);
			m_OfficeBuilding.RemoveFuniture(uid);
			UpdateLastFurniture();
			ChangeMode(Mode.Edit, -1, -1L);
		}
	}

	private void TryRemoveAllFurnitures()
	{
		NKCPacketSender.Send_NKMPacket_OFFICE_CLEAR_ALL_FURNITURE_REQ(RoomID);
	}

	public void OnRemoveAllFurnitures(int roomID)
	{
		if (roomID == RoomID)
		{
			m_currentRoom.ClearAllFunitures();
			m_OfficeBuilding.ClearAllFunitures();
			UpdateEnvScore();
			UpdateLastFurniture();
			ChangeMode(Mode.Edit, -1, -1L);
		}
	}

	private void UpdateSelectedFurnitureInfo(int furnitureID)
	{
		if (furnitureID <= 0)
		{
			NKCUtil.SetGameobjectActive(m_objEditFurnitureRoot, bValue: false);
			return;
		}
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(furnitureID);
		if (nKMOfficeInteriorTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objEditFurnitureRoot, bValue: true);
			NKCUtil.SetLabelText(m_lbEditFurnitureName, nKMOfficeInteriorTemplet.GetItemName());
			NKCUtil.SetLabelText(m_lbEditFurnitureEnvScore, nKMOfficeInteriorTemplet.InteriorScore.ToString());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objEditFurnitureRoot, bValue: false);
		}
	}

	private void TryApplyDecoration(NKMOfficeInteriorTemplet templet)
	{
		switch (templet.Target)
		{
		case InteriorTarget.Background:
			NKCPacketSender.Send_NKMPacket_OFFICE_SET_ROOM_BACKGROUND_REQ(m_currentRoom.ID, templet.Key);
			break;
		case InteriorTarget.Floor:
			NKCPacketSender.Send_NKMPacket_OFFICE_SET_ROOM_FLOOR_REQ(m_currentRoom.ID, templet.Key);
			break;
		case InteriorTarget.Wall:
			NKCPacketSender.Send_NKMPacket_OFFICE_SET_ROOM_WALL_REQ(m_currentRoom.ID, templet.Key);
			break;
		case InteriorTarget.Tile:
			break;
		}
	}

	public void OnApplyDecoration(int id)
	{
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMItemMiscTemplet.FindInterior(id);
		NKM_ERROR_CODE nKM_ERROR_CODE = m_currentRoom.SetDecoration(nKMOfficeInteriorTemplet.Key, nKMOfficeInteriorTemplet.Target);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			Debug.LogError(nKM_ERROR_CODE);
		}
		m_OfficeBuilding.SetDecoration(nKMOfficeInteriorTemplet);
		UpdateEnvScore();
	}

	private void OnBtnShop()
	{
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_SHOP, "TAB_EXCHANGE_OFFICE");
	}

	private void OnBtnEditMode()
	{
		if (!IsVisiting)
		{
			ChangeMode(Mode.Edit, -1, -1L);
		}
	}

	private void OnBtnWarehouse()
	{
		if (!IsVisiting)
		{
			NKCUIPopupOfficeInteriorSelect.Instance.Open(OnFunitureAddSelected, OnThemePresetSelected);
		}
	}

	private void OnBtnDiployUnit()
	{
		if (!IsVisiting)
		{
			NKCUIPopupOfficeMemberEdit.Instance.Open(RoomID);
		}
	}

	private void OnBtnCommunity()
	{
		if (!IsVisiting)
		{
			NKCUIPopupOfficeInteract.Instance.Open();
		}
	}

	private void OnBtnParty()
	{
		if (NKMOpenTagManager.IsOpened("OFFICE_PARTY"))
		{
			if (m_currentRoom.m_lstUnitUID.Count == 0)
			{
				NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_OFFICE_PARTY_NO_UNIT_EXIST);
			}
			else
			{
				NKCUIPopupOfficePartyConfirm.Instance.Open(RoomID, OnPartyConfirm);
			}
		}
	}

	private void OnPartyConfirm(int roomID)
	{
		int itemMiscID = NKMCommonConst.Office.PartyUseItem.m_ItemMiscID;
		if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemMiscID) < 1)
		{
			NKCShopManager.OpenItemLackPopup(itemMiscID, 1);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_OFFICE_PARTY_REQ(roomID);
		}
	}

	public void OnPartyFinished(NKCOfficePartyTemplet partyTemplet)
	{
		if (m_OfficeBuilding != null)
		{
			m_OfficeBuilding.OnPartyFinished(partyTemplet);
		}
	}

	private void OnBtnToMyOffice()
	{
		Close();
		NKCUIOfficeMapFront.GetInstance().SetMyOfficeData();
	}

	private void OnBtnRandomVisit()
	{
		NKCPacketSender.Send_NKMPacket_OFFICE_RANDOM_VISIT_REQ();
	}

	private void OnBtnSendBizCard()
	{
		if (GetPostSendCount() <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("NEC_FAIL_OFFICE_POST_DAILY_LIMIT_FULL"));
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_OFFICE_POST_SEND_REQ(CurrentVisitUID);
		}
	}

	public void OnRoomUnitUpdated()
	{
		if (!IsVisiting)
		{
			NKMOfficeRoom officeRoom = NKCScenManager.CurrentUserData().OfficeData.GetOfficeRoom(RoomID);
			if (officeRoom != null)
			{
				m_currentRoom.m_lstUnitUID = new List<long>(officeRoom.unitUids);
				m_OfficeBuilding.UpdateSDCharacters(officeRoom.unitUids, m_lstVisitor);
			}
		}
	}

	private void UpdateEditButtons()
	{
		switch (m_eMode)
		{
		case Mode.Normal:
		case Mode.Edit:
			m_csbtnEditInfo.Lock();
			m_csbtnEditInvert.Lock();
			m_csbtnEditStore.Lock();
			m_csbtnEditStoreAll.UnLock();
			m_csbtnEditSave.Lock();
			UpdateSelectedFurnitureInfo(0);
			break;
		case Mode.EditAdd:
		case Mode.EditMove:
			if (m_OfficeBuilding.m_SelectedFunitureData != null)
			{
				m_csbtnEditInfo.UnLock();
				m_csbtnEditInvert.SetLock(m_OfficeBuilding.m_SelectedFunitureData.Templet.Target == InteriorTarget.Wall);
				m_csbtnEditStore.UnLock();
				m_csbtnEditStoreAll.UnLock();
				m_csbtnEditSave.UnLock();
				UpdateSelectedFurnitureInfo(m_OfficeBuilding.m_SelectedFunitureData.itemID);
			}
			else
			{
				m_csbtnEditInfo.Lock();
				m_csbtnEditInvert.Lock();
				m_csbtnEditStore.Lock();
				m_csbtnEditStoreAll.UnLock();
				m_csbtnEditSave.Lock();
				UpdateSelectedFurnitureInfo(0);
			}
			break;
		}
	}

	private void OnBtnEditSave()
	{
		switch (m_eMode)
		{
		case Mode.EditAdd:
			TryFurnitureAdd(m_OfficeBuilding.m_SelectedFunitureData.itemID, m_OfficeBuilding.m_SelectedFunitureData.eTarget, m_OfficeBuilding.m_SelectedFunitureData.PosX, m_OfficeBuilding.m_SelectedFunitureData.PosY, m_OfficeBuilding.m_SelectedFunitureData.bInvert);
			break;
		case Mode.EditMove:
			TryFurnitureMove(m_OfficeBuilding.m_SelectedFunitureData.uid, m_OfficeBuilding.m_SelectedFunitureData.eTarget, m_OfficeBuilding.m_SelectedFunitureData.PosX, m_OfficeBuilding.m_SelectedFunitureData.PosY, m_OfficeBuilding.m_SelectedFunitureData.bInvert);
			break;
		}
	}

	private void OnBtnEditStoreAll()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_OFFICE_CONFIRM_STORE_ALL"), TryRemoveAllFurnitures);
	}

	private void OnBtnEditStore()
	{
		switch (m_eMode)
		{
		case Mode.EditAdd:
			ChangeMode(Mode.Edit, -1, -1L);
			break;
		case Mode.EditMove:
			TryRemoveFurniture(m_OfficeBuilding.m_SelectedFunitureData.uid);
			break;
		}
	}

	private void OnBtnEditInvert()
	{
		if (m_OfficeBuilding.m_SelectedFunitureData.Templet.Target != InteriorTarget.Wall)
		{
			m_OfficeBuilding.InvertSelection();
		}
	}

	private void OnBtnEditInfo()
	{
		NKCPopupItemBox.Instance.OpenItemBox(m_OfficeBuilding.m_SelectedFunitureData.itemID);
	}

	private void OnBtnEditClose()
	{
		Mode eMode = m_eMode;
		if (eMode != Mode.Edit && (uint)(eMode - 2) <= 1u)
		{
			ChangeMode(Mode.Edit, -1, -1L);
		}
		else
		{
			ChangeMode(Mode.Normal, -1, -1L);
		}
	}

	private void OnBtnEditCopyPreset()
	{
		if (NKCScenManager.CurrentUserData().IsSuperUser() && m_currentRoom != null)
		{
			GUIUtility.systemCopyBuffer = m_currentRoom.MakePresetFromRoom().ToBase64();
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, "클립보드로 복사되었습니다");
		}
	}

	private void UpdateEnvScore()
	{
		NKMOfficeRoom room = ((!IsVisiting) ? NKCScenManager.CurrentUserData().OfficeData.GetOfficeRoom(RoomID) : NKCScenManager.CurrentUserData().OfficeData.GetFriendRoom(CurrentVisitUID, RoomID));
		UpdateEnvScore(room);
	}

	private void UpdateEnvScore(NKMOfficeRoom room)
	{
		if (m_comEnvScore != null)
		{
			m_comEnvScore.UpdateEnvScore(room);
		}
		if (m_comEditEnvScore != null)
		{
			m_comEditEnvScore.UpdateEnvScore(room);
		}
	}

	private void LoadNPCSpineIllust(NKMOfficeRoomTemplet.RoomType type, RectTransform parent)
	{
		foreach (NKCASUIUnitIllust item in m_lstNPCIllust)
		{
			item.Unload();
		}
		m_lstNPCIllust.Clear();
		foreach (NPCInfo item2 in m_lstNPCInfo)
		{
			if (item2.Type != type)
			{
				continue;
			}
			NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(item2.BundleName, item2.BundleName);
			NKCASUIUnitIllust nKCASUIUnitIllust = NKCResourceUtility.OpenSpineIllust(nKMAssetName.m_BundleName, nKMAssetName.m_AssetName);
			if (nKCASUIUnitIllust != null)
			{
				NKCUINPCBase componentInChildren = nKCASUIUnitIllust.GetRectTransform().GetComponentInChildren<NKCUINPCBase>();
				if (componentInChildren != null)
				{
					componentInChildren.Init();
				}
			}
			nKCASUIUnitIllust.SetParent(parent, worldPositionStays: false);
			nKCASUIUnitIllust.GetRectTransform().anchoredPosition = item2.Offset;
			m_lstNPCIllust.Add(nKCASUIUnitIllust);
		}
	}

	private void PlayRoomMusic()
	{
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = null;
		if (m_currentRoom != null)
		{
			foreach (NKCOfficeFunitureData value in m_currentRoom.m_dicFuniture.Values)
			{
				if (value.Templet.HasBGM)
				{
					nKMOfficeInteriorTemplet = value.Templet;
				}
			}
		}
		if (nKMOfficeInteriorTemplet != null)
		{
			Debug.Log("Playing BGM furniture music : from " + nKMOfficeInteriorTemplet.GetItemName() + " : " + nKMOfficeInteriorTemplet.PlayBGM);
			NKCSoundManager.PlayMusic(nKMOfficeInteriorTemplet.PlayBGM, bLoop: true, nKMOfficeInteriorTemplet.GetBGMVolume);
		}
		else if (m_currentRoomTemplet != null && !string.IsNullOrEmpty(m_currentRoomTemplet.DefaultBGM))
		{
			NKCSoundManager.PlayMusic(m_currentRoomTemplet.DefaultBGM, bLoop: true);
		}
	}

	private void SetPreview(PresetMode mode, NKMOfficePreset preset)
	{
		if (preset != null)
		{
			m_ePresetMode = mode;
			m_currentPresetId = preset.presetId;
			m_OfficeBuilding.SetTempFurniture(preset);
			ChangeMode(Mode.Preview, -1, -1L);
			NKCUIManager.SetAsTopmost(this, bForce: true);
		}
	}

	private void CancelPreview()
	{
		m_OfficeBuilding.SetRoomData(m_currentRoom, m_lstVisitor);
		m_currentPresetId = -1;
		ChangeMode(Mode.Edit, -1, -1L);
	}

	public void OnApplyPreset(NKMOfficeRoom room)
	{
		if (room != null && room.id == m_currentRoom.ID)
		{
			m_currentRoom = new NKCOfficeRoomData(room, 0L);
			m_OfficeBuilding.SetRoomData(m_currentRoom, m_lstVisitor);
			ChangeMode(Mode.Edit, -1, -1L);
		}
	}

	private void OnBtnPresetList()
	{
		NKCUIPopupOfficePresetList.Instance.Open(RoomID, OnSlotAction);
	}

	private void OnSlotAction(NKCUIPopupOfficePresetList.ActionType type, int id, string name)
	{
		switch (type)
		{
		case NKCUIPopupOfficePresetList.ActionType.Add:
		{
			NKCPopupInventoryAdd.SliderInfo sliderInfo = new NKCPopupInventoryAdd.SliderInfo
			{
				increaseCount = 1,
				maxCount = NKMCommonConst.Office.PresetConst.MaxCount,
				currentCount = NKCScenManager.CurrentUserData().OfficeData.GetPresetCount(),
				inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_NONE
			};
			NKCPopupInventoryAdd.Instance.Open(NKCStringTable.GetString("SI_PF_OFFICE_DECO_MODE_PRESET_BUY_TOP_TEXT"), NKCStringTable.GetString("SI_PF_OFFICE_DECO_MODE_PRESET_BUY_INFO_TEXT"), sliderInfo, NKMCommonConst.Office.PresetConst.ExpandCostValue, NKMCommonConst.Office.PresetConst.ExpandCostItem.m_ItemMiscID, delegate(int count)
			{
				NKCPacketSender.Send_NKMPacket_OFFICE_PRESET_ADD_REQ(count);
			}, showResource: true);
			break;
		}
		case NKCUIPopupOfficePresetList.ActionType.Clear:
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_OFFICE_DECO_MODE_PRESET_RESET_DESC"), delegate
			{
				NKCPacketSender.Send_NKMPacket_OFFICE_PRESET_RESET_REQ(id);
			});
			break;
		case NKCUIPopupOfficePresetList.ActionType.Load:
		{
			NKMOfficePreset preset = NKCScenManager.CurrentUserData().OfficeData.GetPreset(id);
			SetPreview(PresetMode.MyPreset, preset);
			break;
		}
		case NKCUIPopupOfficePresetList.ActionType.Rename:
			NKCPacketSender.Send_NKMPacket_OFFICE_PRESET_CHANGE_NAME_REQ(id, name);
			break;
		case NKCUIPopupOfficePresetList.ActionType.Save:
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_OFFICE_DECO_MODE_PRESET_SAVE_DESC"), delegate
			{
				NKCPacketSender.Send_NKMPacket_OFFICE_PRESET_REGISTER_REQ(RoomID, id);
			});
			break;
		}
	}

	private void OnThemePresetSelected(int id)
	{
		NKMOfficeThemePresetTemplet nKMOfficeThemePresetTemplet = NKMOfficeThemePresetTemplet.Find(id);
		SetPreview(PresetMode.Theme, nKMOfficeThemePresetTemplet.OfficePreset);
		m_currentPresetId = id;
	}

	private void TryApplyPreset()
	{
		switch (m_ePresetMode)
		{
		case PresetMode.MyPreset:
			NKCPacketSender.Send_NKMPacket_OFFICE_PRESET_APPLY_REQ(RoomID, m_currentPresetId);
			break;
		case PresetMode.Theme:
			NKCPacketSender.Send_NKMPacket_OFFICE_PRESET_APPLY_THEMA_REQ(RoomID, m_currentPresetId);
			break;
		}
	}

	private void OnBtnPreviewCancel()
	{
		CancelPreview();
	}

	private void OnPresetInteriorList()
	{
		NKMOfficePreset nKMOfficePreset = null;
		switch (m_ePresetMode)
		{
		case PresetMode.MyPreset:
			nKMOfficePreset = NKCScenManager.CurrentUserData().OfficeData.GetPreset(m_currentPresetId);
			break;
		case PresetMode.Theme:
		{
			NKMOfficeThemePresetTemplet nKMOfficeThemePresetTemplet = NKMOfficeThemePresetTemplet.Find(m_currentPresetId);
			if (nKMOfficeThemePresetTemplet != null)
			{
				nKMOfficePreset = nKMOfficeThemePresetTemplet.OfficePreset;
			}
			break;
		}
		}
		if (nKMOfficePreset == null)
		{
			return;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		dictionary[nKMOfficePreset.floorInteriorId] = 1;
		dictionary[nKMOfficePreset.wallInteriorId] = 1;
		dictionary[nKMOfficePreset.backgroundId] = 1;
		foreach (NKMOfficeFurniture furniture in nKMOfficePreset.furnitures)
		{
			if (furniture != null)
			{
				if (dictionary.ContainsKey(furniture.itemId))
				{
					dictionary[furniture.itemId]++;
				}
				else
				{
					dictionary[furniture.itemId] = 1;
				}
			}
		}
		NKCUIPopupOfficeInteriorSelect.Instance.OpenForListView(dictionary);
	}

	private void OnBtnHideMenu()
	{
		HideUI(value: true);
	}

	private void OnBtnUnhideMenu()
	{
		HideUI(value: false);
	}

	public void OnUnitTakeHeart(NKMUnitData unitData)
	{
		if (m_currentRoom != null && m_currentRoom.m_lstUnitUID.Contains(unitData.m_UnitUID))
		{
			m_OfficeBuilding.OnUnitTakeHeart(unitData);
		}
	}

	public void UpdatePostState()
	{
		int postSendCount = GetPostSendCount();
		NKCUtil.SetLabelText(m_lbSendBizCardCount, $"{postSendCount}/{5}");
		CheckCommunityReddot();
	}

	private int GetPostSendCount()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return 0;
		}
		int num = 0;
		if (NKCSynchronizedTime.IsFinished(NKCSynchronizedTime.ToUtcTime(nKMUserData.OfficeData.PostState.nextResetDate)))
		{
			return 5;
		}
		return 5 - nKMUserData.OfficeData.PostState.sendCount;
	}

	public void UpdateAlarm()
	{
		CheckCommunityReddot();
		if (m_NKCUIOfficeFacility != null)
		{
			m_NKCUIOfficeFacility.UpdateAlarm();
		}
		if (m_UIOfficeFacilityButtons != null)
		{
			m_UIOfficeFacilityButtons.UpdateAlarm();
		}
	}

	private void CheckCommunityReddot()
	{
		NKCUtil.SetGameobjectActive(m_objCommunityReddot, NKCAlarmManager.CheckOfficeCommunityNotify(NKCScenManager.CurrentUserData()));
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		UpdateAlarm();
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		UpdateAlarm();
		if (eEventType == NKMUserData.eChangeNotifyType.Update && m_OfficeBuilding != null)
		{
			m_OfficeBuilding.OnUnitUpdated(unitData);
		}
	}

	public override void OnScreenResolutionChanged()
	{
		base.OnScreenResolutionChanged();
		if (m_OfficeBuilding != null)
		{
			m_OfficeBuilding.SetCameraOffset(0f);
			m_OfficeBuilding.SetCamera();
		}
		if (m_NKCUIOfficeFacility != null)
		{
			if (m_lstNPCIllust.Count > 0)
			{
				m_NKCUIOfficeFacility.SetCameraOffset(m_rtNPCRoot.GetWidth());
			}
			else
			{
				m_NKCUIOfficeFacility.SetCameraOffset(0f);
			}
			m_NKCUIOfficeFacility.SetCamera();
		}
	}

	public bool CanPlayInteraction()
	{
		Mode eMode = m_eMode;
		if ((uint)(eMode - 1) <= 2u)
		{
			return false;
		}
		return true;
	}
}
