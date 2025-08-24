using System.Collections.Generic;
using DG.Tweening;
using NKC.Office;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIOfficeMapFront : NKCUIBase
{
	public enum MinimapState
	{
		Main,
		Edit,
		Visit
	}

	public enum SectionType
	{
		Room,
		Facility
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_office";

	private const string UI_ASSET_NAME = "AB_UI_OFFICE_MAP_UI_FRONT";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public bool IgnoreRoomLockState;

	[Header("UI 타입 Root")]
	public GameObject m_objMainUI;

	public GameObject m_objMemberEditUI;

	public GameObject m_objVisitorUI;

	[Space]
	public GameObject m_objRoomRedDot;

	public GameObject m_objFacilityRedDot;

	public GameObject m_objDormitoryLock;

	public NKCUIComToggle m_tglRoom;

	public NKCUIComToggle m_tglFacility;

	public NKCUIComStateButton m_csbtnMemberEdit;

	public NKCUIComMapSectionButton[] m_csbtnSectionArray;

	public NKCUIComMapSectionButton[] m_csbtnVisitorSectionArray;

	public NKCUIOfficeUpsideMenu m_officeUpsideMenu;

	private MinimapState m_eCurrentMinimapState;

	private SectionType m_eCurrentSectionType;

	private NKCOfficeData m_NKCOfficeData;

	private NKCUIOfficeMinimap m_NKCMinimap;

	private IOfficeMinimap m_NKCMinimapRoom;

	private IOfficeMinimap m_NKCMinimapFacility;

	private IOfficeMinimap m_currentMinimap;

	private Dictionary<SectionType, List<int>> m_dicSectionId;

	private static bool m_bVisiting;

	private const bool IgnoreSuperUser = true;

	private static NKM_SCEN_ID m_eReserveScenId;

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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "사옥 미니맵";

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public NKCUIOfficeUpsideMenu OfficeUpsideMenu => m_officeUpsideMenu;

	public NKCOfficeData OfficeData => m_NKCOfficeData;

	public MinimapState CurrentMinimapState => m_eCurrentMinimapState;

	public SectionType CurrentSectionType => m_eCurrentSectionType;

	public bool Visiting => m_bVisiting;

	public static NKM_SCEN_ID ReserveScenID
	{
		set
		{
			m_eReserveScenId = value;
		}
	}

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIOfficeMapFront>("ab_ui_office", "AB_UI_OFFICE_MAP_UI_FRONT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIOfficeMapFront GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIOfficeMapFront>();
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
		m_NKCMinimap.Release();
		OfficeData.ResetFriendUId();
		Object.Destroy(m_NKCMinimap.gameObject);
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
		m_NKCOfficeData = null;
		m_NKCMinimap = null;
		m_NKCMinimapRoom = null;
		m_NKCMinimapFacility = null;
		m_currentMinimap = null;
		m_dicSectionId?.Clear();
		m_dicSectionId = null;
	}

	public void Init()
	{
		GameObject orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<GameObject>("ab_ui_office", "AB_UI_OFFICE_MINIMAP");
		if (orLoadAssetResource != null)
		{
			GameObject gameObject = Object.Instantiate(orLoadAssetResource, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommonLow));
			NKCUIOfficeMinimap nKCUIOfficeMinimap = gameObject.GetComponent<NKCUIOfficeMinimap>();
			if (nKCUIOfficeMinimap == null)
			{
				nKCUIOfficeMinimap = gameObject.AddComponent<NKCUIOfficeMinimap>();
			}
			if (nKCUIOfficeMinimap != null)
			{
				nKCUIOfficeMinimap.Init();
				m_NKCMinimap = nKCUIOfficeMinimap;
				m_NKCMinimapRoom = nKCUIOfficeMinimap.m_NKCUIMinimapRoom;
				m_NKCMinimapFacility = nKCUIOfficeMinimap.m_NKCUIMinimapFacility;
			}
		}
		m_officeUpsideMenu?.Init();
		NKCUtil.SetToggleValueChangedDelegate(m_tglRoom, OnToggleRoom);
		NKCUtil.SetToggleValueChangedDelegate(m_tglFacility, OnToggleFacility);
		NKCUtil.SetButtonClickDelegate(m_csbtnMemberEdit, OnBtnMemberEdit);
		m_tglRoom.SetbReverseSeqCallbackCall(bSet: true);
		m_dicSectionId = new Dictionary<SectionType, List<int>>();
		m_dicSectionId.Add(SectionType.Facility, new List<int>());
		m_dicSectionId.Add(SectionType.Room, new List<int>());
		foreach (NKMOfficeSectionTemplet value in NKMTempletContainer<NKMOfficeSectionTemplet>.Values)
		{
			m_dicSectionId[value.IsFacility ? SectionType.Facility : SectionType.Room].Add(value.SectionId);
		}
		m_dicSectionId[SectionType.Facility].Sort();
		m_dicSectionId[SectionType.Room].Sort();
		NKCUtil.SetGameobjectActive(m_objRoomRedDot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFacilityRedDot, bValue: false);
		NKCUtil.SetScrollHotKey(m_NKCMinimapFacility.GetScrollRect(), this);
		SetupScrollRects(m_NKCMinimapFacility.GetGameObject());
		NKCUtil.SetScrollHotKey(m_NKCMinimapRoom.GetScrollRect(), this);
		SetupScrollRects(m_NKCMinimapRoom.GetGameObject());
		NKCUtil.SetHotkey(m_tglFacility, HotkeyEventType.NextTab);
		NKCUtil.SetHotkey(m_tglRoom, HotkeyEventType.NextTab);
	}

	public void Open()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		m_NKCOfficeData = nKMUserData.OfficeData;
		m_bVisiting = m_NKCOfficeData.IsVisiting;
		if (!m_bVisiting)
		{
			SetMinimapState(MinimapState.Main);
			m_NKCMinimapFacility.UpdateRoomStateAll();
			m_NKCMinimapRoom.UpdateRoomStateAll();
			bool flag = NKCContentManager.IsContentsUnlocked(ContentsType.OFFICE);
			NKCUtil.SetGameobjectActive(m_objDormitoryLock, !flag);
			if (m_tglFacility != null)
			{
				m_tglFacility.Select(bSelect: false, bForce: true);
				m_tglFacility.Select(bSelect: true);
			}
			NKCUtil.SetGameobjectActive(m_objFacilityRedDot, m_NKCMinimapFacility.IsRedDotOn());
			NKCUtil.SetGameobjectActive(m_objRoomRedDot, m_NKCMinimapRoom.IsRedDotOn());
		}
		else
		{
			SetMinimapState(MinimapState.Visit);
			m_NKCMinimapRoom.UpdateRoomStateAll();
			if (m_tglRoom != null)
			{
				m_tglRoom.Select(bSelect: false, bForce: true);
				m_tglRoom.Select(bSelect: true);
			}
		}
		m_officeUpsideMenu?.SetRedDotNotify();
		UIOpened();
	}

	public override void Hide()
	{
		base.Hide();
		m_NKCMinimap.SetAcive(value: false);
	}

	public override void UnHide()
	{
		base.UnHide();
		m_NKCMinimap.SetAcive(value: true);
		m_currentMinimap.UpdateCameraPosition();
		NKCUtil.SetGameobjectActive(m_objFacilityRedDot, !m_bVisiting && m_NKCMinimapFacility.IsRedDotOn());
		NKCUtil.SetGameobjectActive(m_objRoomRedDot, !m_bVisiting && m_NKCMinimapRoom.IsRedDotOn());
		m_officeUpsideMenu?.SetRedDotNotify();
		OnScreenResolutionChanged();
	}

	public void SetMyOfficeData(NKMOfficeRoomTemplet reservationRoomTemplet = null)
	{
		OfficeData.ResetFriendUId();
		m_bVisiting = false;
		SetMinimapState(MinimapState.Main);
		m_NKCMinimapFacility.UpdateRoomStateAll();
		m_NKCMinimapRoom.UpdateRoomStateAll();
		bool flag = NKCContentManager.IsContentsUnlocked(ContentsType.OFFICE);
		NKCUtil.SetGameobjectActive(m_objDormitoryLock, !flag);
		bool flag2 = reservationRoomTemplet != null && reservationRoomTemplet.Type != NKMOfficeRoomTemplet.RoomType.Dorm;
		if (flag && !flag2)
		{
			if (m_tglRoom != null)
			{
				m_tglRoom.Select(bSelect: false, bForce: true);
				m_tglRoom.Select(bSelect: true);
			}
		}
		else if (m_tglFacility != null)
		{
			m_tglFacility.Select(bSelect: false, bForce: true);
			m_tglFacility.Select(bSelect: true);
		}
		NKCUtil.SetGameobjectActive(m_objFacilityRedDot, m_NKCMinimapFacility.IsRedDotOn());
		NKCUtil.SetGameobjectActive(m_objRoomRedDot, m_NKCMinimapRoom.IsRedDotOn());
		m_officeUpsideMenu?.SetRedDotNotify();
	}

	public void SetFriendOfficeData()
	{
		m_bVisiting = true;
		SetMinimapState(MinimapState.Visit);
		m_NKCMinimapRoom.UpdateRoomStateAll();
		if (m_tglRoom != null)
		{
			m_tglRoom.Select(bSelect: false, bForce: true);
			m_tglRoom.Select(bSelect: true);
		}
		m_officeUpsideMenu?.SetRedDotNotify();
	}

	public override void OnBackButton()
	{
		if (m_eCurrentMinimapState == MinimapState.Edit)
		{
			SetMinimapState(MinimapState.Main);
			return;
		}
		base.OnBackButton();
		if (m_eReserveScenId == NKM_SCEN_ID.NSI_INVALID)
		{
			NKCScenManager.GetScenManager()?.ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
		}
		else
		{
			NKCScenManager.GetScenManager()?.ScenChangeFade(m_eReserveScenId, bForce: false);
		}
		m_eReserveScenId = NKM_SCEN_ID.NSI_INVALID;
	}

	public IOfficeMinimap GetCurrentMinimap()
	{
		return m_currentMinimap;
	}

	public void UpdateSectionLockState(int sectionId)
	{
		SetSectionButtonInfo(m_eCurrentSectionType);
		MoveScrollToSection(sectionId);
	}

	public static string GetDefaultRoomName(int roomId)
	{
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(roomId);
		if (nKMOfficeRoomTemplet != null)
		{
			return NKCStringTable.GetString(nKMOfficeRoomTemplet.Name);
		}
		return "Room";
	}

	public static string GetSectionName(NKMOfficeSectionTemplet sectionTemplet)
	{
		if (sectionTemplet != null)
		{
			return NKCStringTable.GetString(sectionTemplet.SectionName);
		}
		return $"Section {sectionTemplet.SectionId}";
	}

	public void MoveMiniMap(Vector3 targetPosition, bool restrictScroll = true)
	{
		if (m_currentMinimap == null)
		{
			return;
		}
		ScrollRect scrollRect = m_currentMinimap.GetScrollRect();
		if (scrollRect == null)
		{
			return;
		}
		scrollRect.StopMovement();
		Vector3 position = scrollRect.content.position;
		Vector3 position2 = new Vector3(position.x - targetPosition.x, position.y, position.z);
		if (restrictScroll)
		{
			Vector3 vector = NKCCamera.GetSubUICamera().WorldToScreenPoint(position2);
			float num = scrollRect.content.rect.width * NKCUIManager.FrontCanvas.scaleFactor;
			float num2 = vector.x - scrollRect.content.pivot.x * num;
			float num3 = vector.x + (1f - scrollRect.content.pivot.x) * num;
			if (num2 > 0f)
			{
				Vector3 vector2 = NKCCamera.GetSubUICamera().ScreenToWorldPoint(new Vector3(num2, vector.y, vector.z));
				Vector3 vector3 = NKCCamera.GetSubUICamera().ScreenToWorldPoint(new Vector3(0f, vector.y, vector.z));
				float num4 = Mathf.Abs(vector2.x - vector3.x);
				position2.x -= num4;
			}
			else if (num3 < (float)Screen.width)
			{
				Vector3 vector4 = NKCCamera.GetSubUICamera().ScreenToWorldPoint(new Vector3(num3, vector.y, vector.z));
				Vector3 vector5 = NKCCamera.GetSubUICamera().ScreenToWorldPoint(new Vector3(Screen.width, vector.y, vector.z));
				float num5 = Mathf.Abs(vector4.x - vector5.x);
				position2.x += num5;
			}
		}
		else if (scrollRect.normalizedPosition.x > 0.5f)
		{
			m_currentMinimap.ExpandScrollRectRange();
		}
		scrollRect.content.DOKill();
		scrollRect.content.DOMoveX(position2.x, 0.5f).SetEase(Ease.OutQuint).SetDelay(0.1f);
	}

	public void MoveMiniMap(float horizonralNormalizedValue, UnityAction onComplete)
	{
		if (m_currentMinimap == null)
		{
			return;
		}
		ScrollRect scrollRect = m_currentMinimap.GetScrollRect();
		if (scrollRect == null)
		{
			return;
		}
		scrollRect.StopMovement();
		scrollRect.DOKill();
		scrollRect.DOHorizontalNormalizedPos(horizonralNormalizedValue, 0.5f).SetEase(Ease.OutQuint).OnComplete(delegate
		{
			if (onComplete != null)
			{
				onComplete();
			}
		});
	}

	public void RevertScrollRectRange()
	{
		if (m_currentMinimap != null)
		{
			m_currentMinimap.RevertScrollRectRange();
		}
	}

	public void UpdateFactoryState()
	{
		if (m_eCurrentSectionType == SectionType.Facility)
		{
			m_currentMinimap.UpdateRoomState(NKMOfficeRoomTemplet.RoomType.Forge);
		}
		NKCUtil.SetGameobjectActive(m_objFacilityRedDot, m_NKCMinimapFacility.IsRedDotOn());
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		if (m_bVisiting)
		{
			return false;
		}
		if ((uint)(hotkey - 5) <= 1u)
		{
			if (m_NKCMinimapFacility.GetGameObject().activeSelf)
			{
				m_tglRoom.Select(bSelect: true);
			}
			else
			{
				m_tglFacility.Select(bSelect: true);
			}
			return true;
		}
		return false;
	}

	public override void OnScreenResolutionChanged()
	{
		base.OnScreenResolutionChanged();
		m_NKCMinimap.CalcCamMoveBoundRect();
		m_currentMinimap.UpdateCameraPosition();
	}

	public void SelectFacilityTab()
	{
		if (!m_bVisiting)
		{
			SetMinimapState(MinimapState.Main);
		}
		if (!(m_tglFacility == null))
		{
			m_tglFacility.Select(bSelect: false, bForce: true);
			m_tglFacility.Select(bSelect: true);
		}
	}

	public void SelectRoomTab()
	{
		if (!m_bVisiting)
		{
			SetMinimapState(MinimapState.Main);
		}
		if (!(m_tglRoom == null))
		{
			m_tglRoom.Select(bSelect: false, bForce: true);
			m_tglRoom.Select(bSelect: true);
		}
	}

	public static ContentsType GetFacilityContentType(NKMOfficeRoomTemplet.RoomType facilityType)
	{
		return facilityType switch
		{
			NKMOfficeRoomTemplet.RoomType.Lab => ContentsType.BASE_LAB, 
			NKMOfficeRoomTemplet.RoomType.Forge => ContentsType.BASE_FACTORY, 
			NKMOfficeRoomTemplet.RoomType.Hangar => ContentsType.BASE_HANGAR, 
			NKMOfficeRoomTemplet.RoomType.CEO => ContentsType.BASE_PERSONNAL, 
			NKMOfficeRoomTemplet.RoomType.Terrabrain => ContentsType.TERRA_BRAIN, 
			_ => ContentsType.None, 
		};
	}

	private bool IsSectionUnlocked(NKMOfficeSectionTemplet sectionTemplet, ref ContentsType lockedContentType, ref bool purchaseEnable, ref bool isPurchased)
	{
		if (sectionTemplet == null)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		purchaseEnable = true;
		isPurchased = true;
		lockedContentType = ContentsType.None;
		if (sectionTemplet.IsFacility)
		{
			foreach (KeyValuePair<int, NKMOfficeRoomTemplet> room in sectionTemplet.Rooms)
			{
				NKMOfficeRoomTemplet value = room.Value;
				if (value != null)
				{
					ContentsType facilityContentType = GetFacilityContentType(value.Type);
					flag2 |= NKCContentManager.IsContentsUnlocked(facilityContentType);
					if (!flag2 && lockedContentType == ContentsType.None)
					{
						lockedContentType = facilityContentType;
					}
				}
			}
			return flag2;
		}
		flag2 = NKCContentManager.IsContentsUnlocked(ContentsType.OFFICE);
		if (!flag2)
		{
			lockedContentType = ContentsType.OFFICE;
		}
		if (sectionTemplet.HasUnlockType)
		{
			purchaseEnable = NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), new UnlockInfo(sectionTemplet.UnlockReqType, sectionTemplet.UnlockReqValue), ignoreSuperUser: true);
		}
		isPurchased = m_NKCOfficeData.IsOpenedSection(sectionTemplet.SectionId);
		return flag2 & purchaseEnable & isPurchased;
	}

	private bool IsSectionVisitUnlocked(NKMOfficeSectionTemplet sectionTemplet)
	{
		if (sectionTemplet == null)
		{
			return false;
		}
		return m_NKCOfficeData.IsOpenedSection(sectionTemplet.SectionId);
	}

	private void SetMinimapState(MinimapState minimapState)
	{
		m_eCurrentMinimapState = minimapState;
		NKCUtil.SetGameobjectActive(m_objMainUI, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMemberEditUI, bValue: false);
		NKCUtil.SetGameobjectActive(m_objVisitorUI, bValue: false);
		switch (minimapState)
		{
		case MinimapState.Main:
			NKCUtil.SetGameobjectActive(m_objMainUI, bValue: true);
			m_NKCMinimap.m_NKCUIMinimapRoom.UpdateRoomFxAll();
			break;
		case MinimapState.Edit:
			NKCUtil.SetGameobjectActive(m_objMemberEditUI, bValue: true);
			m_NKCMinimap.m_NKCUIMinimapRoom.UpdateRoomFxAll();
			break;
		case MinimapState.Visit:
			NKCUtil.SetGameobjectActive(m_objVisitorUI, bValue: true);
			break;
		}
	}

	private void SetSectionButtonInfo(SectionType sectionType)
	{
		if (m_csbtnSectionArray == null)
		{
			return;
		}
		int num = m_csbtnSectionArray.Length;
		for (int i = 0; i < num; i++)
		{
			if (m_dicSectionId[sectionType].Count <= i)
			{
				NKCUtil.SetGameobjectActive(m_csbtnSectionArray[i].m_csbtnButton, bValue: false);
				continue;
			}
			int num2 = m_dicSectionId[sectionType][i];
			NKCUtil.SetGameobjectActive(m_csbtnSectionArray[i].m_csbtnButton, bValue: true);
			NKMOfficeSectionTemplet sectionTemplet = NKMOfficeSectionTemplet.Find(num2);
			if (sectionTemplet == null)
			{
				NKCUtil.SetLabelText(m_csbtnSectionArray[i].m_lbLockText, "Error");
				m_csbtnSectionArray[i].SetLock(num2, value: true);
				continue;
			}
			NKCUtil.SetLabelText(m_csbtnSectionArray[i].m_lbNormalText, GetSectionName(sectionTemplet));
			NKCUtil.SetLabelText(m_csbtnSectionArray[i].m_lbLockText, GetSectionName(sectionTemplet));
			bool purchaseEnable = true;
			bool isPurchased = true;
			ContentsType lockedContentType = ContentsType.None;
			bool flag = IsSectionUnlocked(sectionTemplet, ref lockedContentType, ref purchaseEnable, ref isPurchased);
			NKCUtil.SetButtonClickDelegate(m_csbtnSectionArray[i].m_csbtnButton, (UnityAction)delegate
			{
				OnBtnArea(sectionTemplet.SectionId);
			});
			m_csbtnSectionArray[i].SetLock(num2, !flag);
		}
	}

	private void SetSectionButtonVisitInfo()
	{
		if (m_csbtnVisitorSectionArray == null)
		{
			return;
		}
		int num = m_csbtnVisitorSectionArray.Length;
		for (int i = 0; i < num; i++)
		{
			if (m_dicSectionId[SectionType.Room].Count <= i)
			{
				NKCUtil.SetGameobjectActive(m_csbtnVisitorSectionArray[i].m_csbtnButton, bValue: false);
				continue;
			}
			int num2 = m_dicSectionId[SectionType.Room][i];
			NKCUtil.SetGameobjectActive(m_csbtnVisitorSectionArray[i].m_csbtnButton, bValue: true);
			NKMOfficeSectionTemplet sectionTemplet = NKMOfficeSectionTemplet.Find(num2);
			if (sectionTemplet == null)
			{
				NKCUtil.SetLabelText(m_csbtnVisitorSectionArray[i].m_lbLockText, "Error");
				m_csbtnVisitorSectionArray[i].SetLock(num2, value: true);
				continue;
			}
			NKCUtil.SetLabelText(m_csbtnVisitorSectionArray[i].m_lbNormalText, GetSectionName(sectionTemplet));
			NKCUtil.SetLabelText(m_csbtnVisitorSectionArray[i].m_lbLockText, GetSectionName(sectionTemplet));
			bool flag = IsSectionVisitUnlocked(sectionTemplet);
			NKCUtil.SetButtonClickDelegate(m_csbtnVisitorSectionArray[i].m_csbtnButton, (UnityAction)delegate
			{
				OnBtnArea(sectionTemplet.SectionId);
			});
			m_csbtnVisitorSectionArray[i].SetLock(num2, !flag);
		}
	}

	private bool IsDormSectionRedDotOn()
	{
		if (m_bVisiting)
		{
			return false;
		}
		bool result = false;
		int count = m_dicSectionId[SectionType.Room].Count;
		for (int i = 0; i < count; i++)
		{
			NKMOfficeSectionTemplet nKMOfficeSectionTemplet = NKMOfficeSectionTemplet.Find(m_dicSectionId[SectionType.Room][i]);
			if (nKMOfficeSectionTemplet != null)
			{
				bool purchaseEnable = true;
				bool isPurchased = true;
				ContentsType lockedContentType = ContentsType.None;
				IsSectionUnlocked(nKMOfficeSectionTemplet, ref lockedContentType, ref purchaseEnable, ref isPurchased);
				if (purchaseEnable && !isPurchased)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private void LockRoomsInSection()
	{
		int num = m_csbtnSectionArray.Length;
		for (int i = 0; i < num; i++)
		{
			if (m_csbtnSectionArray[i].IsLocked())
			{
				m_currentMinimap.LockRoomsInSection(m_csbtnSectionArray[i].m_iSectionId);
			}
		}
	}

	private void MoveScrollToSection(int sectionId)
	{
		Transform transform = m_currentMinimap?.GetScrollTargetTileTransform(sectionId);
		if (!(transform == null))
		{
			MoveMiniMap(transform.position);
		}
	}

	private void OnBtnArea(int sectionId)
	{
		NKMOfficeSectionTemplet nKMOfficeSectionTemplet = NKMOfficeSectionTemplet.Find(sectionId);
		if (nKMOfficeSectionTemplet == null)
		{
			return;
		}
		bool purchaseEnable = true;
		bool isPurchased = true;
		ContentsType lockedContentType = ContentsType.None;
		bool flag = false;
		if (!(m_bVisiting ? IsSectionVisitUnlocked(nKMOfficeSectionTemplet) : IsSectionUnlocked(nKMOfficeSectionTemplet, ref lockedContentType, ref purchaseEnable, ref isPurchased)))
		{
			if (m_bVisiting)
			{
				return;
			}
			if (lockedContentType != ContentsType.None)
			{
				NKCContentManager.ShowLockedMessagePopup(lockedContentType);
			}
			else if (!purchaseEnable)
			{
				if (nKMOfficeSectionTemplet.HasUnlockType)
				{
					string message = NKCContentManager.MakeUnlockConditionString(new UnlockInfo(nKMOfficeSectionTemplet.UnlockReqType, nKMOfficeSectionTemplet.UnlockReqValue), bSimple: false);
					NKCUIManager.NKCPopupMessage.Open(new PopupMessage(message, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				}
			}
			else
			{
				if (isPurchased)
				{
					return;
				}
				string content = string.Format(NKCUtilString.GET_STRING_OFFICE_PURCHASE_SECTION, NKCStringTable.GetString(nKMOfficeSectionTemplet.SectionName));
				if (nKMOfficeSectionTemplet.PriceItem == null)
				{
					NKCPopupResourceConfirmBox.Instance.OpenForConfirm(NKCUtilString.GET_STRING_UNLOCK, content, delegate
					{
						NKCPacketSender.Send_NKMPacket_OFFICE_OPEN_SECTION_REQ(sectionId);
					});
				}
				else
				{
					NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_UNLOCK, content, nKMOfficeSectionTemplet.PriceItem.m_ItemMiscID, nKMOfficeSectionTemplet.Price, delegate
					{
						NKCPacketSender.Send_NKMPacket_OFFICE_OPEN_SECTION_REQ(sectionId);
					});
				}
			}
		}
		else
		{
			MoveScrollToSection(sectionId);
		}
	}

	private void OnToggleRoom(bool value)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.OFFICE))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.OFFICE);
			m_tglFacility.Select(bSelect: true, bForce: true);
			return;
		}
		m_NKCMinimap.SetActiveRoom(value);
		NKCUtil.SetGameobjectActive(m_csbtnMemberEdit, value);
		m_currentMinimap = m_NKCMinimapRoom;
		m_eCurrentSectionType = SectionType.Room;
		if (m_eCurrentMinimapState == MinimapState.Visit)
		{
			SetSectionButtonVisitInfo();
		}
		else
		{
			SetSectionButtonInfo(SectionType.Room);
		}
		m_officeUpsideMenu?.SetState(NKCUIOfficeUpsideMenu.MenuState.MinimapRoom);
		NKCTutorialManager.TutorialRequired(TutorialPoint.OfficeMiniMap);
	}

	private void OnToggleFacility(bool value)
	{
		m_NKCMinimap.SetActiveFacility(value);
		NKCUtil.SetGameobjectActive(m_csbtnMemberEdit, !value);
		m_currentMinimap = m_NKCMinimapFacility;
		m_eCurrentSectionType = SectionType.Facility;
		SetSectionButtonInfo(SectionType.Facility);
		m_officeUpsideMenu?.SetState(NKCUIOfficeUpsideMenu.MenuState.MinimapFacility);
		NKCTutorialManager.TutorialRequired(TutorialPoint.OfficeBaseMiniMap);
	}

	private void OnBtnMemberEdit()
	{
		SetMinimapState(MinimapState.Edit);
		m_NKCMinimap.m_NKCUIMinimapRoom.UpdateRoomFxAll();
	}
}
