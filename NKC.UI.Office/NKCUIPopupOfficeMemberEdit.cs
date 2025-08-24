using System.Collections.Generic;
using ClientPacket.Office;
using NKC.UI.Component.Office;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIPopupOfficeMemberEdit : NKCUIBase
{
	private enum UnitListType
	{
		Normal,
		Trophy
	}

	public delegate void OnCloseMemberEdit();

	private const string ASSET_BUNDLE_NAME = "ab_ui_office";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_OFFICE_MEMBER_EDIT";

	private static NKCUIPopupOfficeMemberEdit m_Instance;

	public RectTransform m_popupContent;

	public LoopScrollRect m_loopScrollRect;

	public Text m_lbFloorName;

	public InputField m_inputRoomNameInput;

	public Text m_lbMemberCount;

	public Animator m_animator;

	public NKCUIComOfficeEnvScore m_EnvScore;

	[Space]
	public NKCUIComToggle m_tglTabUnit;

	public NKCUIComToggle m_tglTabTrophy;

	[Space]
	public NKCUIComStateButton m_csbtnRoomNameChange;

	public NKCUIComStateButton m_csbtnDeselectAll;

	public NKCUIComStateButton m_csbtnAutoSelect;

	public NKCUIComStateButton m_csbtnFilter;

	public GameObject m_objFilterSelected;

	public NKCUIComToggle m_tglFavorite;

	public NKCUIComStateButton m_csbtnConfirm;

	[Header("환경점수")]
	public Text m_lbEnvScore;

	public Text m_lbEnvInformation;

	private NKCUnitSortSystem.UnitListOptions m_unitListOption;

	private NKCUnitSortSystem m_unitSortSystem;

	private UnitListType m_eUnitListType;

	private int m_iMaxRoomNameLength = 8;

	private int m_iRoomId;

	private int m_iMaxUnitCount;

	private int m_iTutorialUnitId;

	private List<long> m_unitAssignList;

	private string m_strRoomName;

	private float m_fPopupWidth;

	private OnCloseMemberEdit m_dOnCloseMemberEdit;

	public static NKCUIPopupOfficeMemberEdit Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupOfficeMemberEdit>("ab_ui_office", "AB_UI_POPUP_OFFICE_MEMBER_EDIT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupOfficeMemberEdit>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

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

	public override string MenuName => "사원 배치";

	public override eMenutype eUIType => eMenutype.Popup;

	public float PopupWidth => m_fPopupWidth;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance.Release();
		m_Instance = null;
	}

	public void InitUI()
	{
		if (m_loopScrollRect != null)
		{
			m_loopScrollRect.dOnGetObject += GetMemberSlot;
			m_loopScrollRect.dOnReturnObject += ReturnMemberSlot;
			m_loopScrollRect.dOnProvideData += ProvideMemberData;
			GridLayoutGroup component = m_loopScrollRect.content.GetComponent<GridLayoutGroup>();
			if (component != null)
			{
				m_loopScrollRect.ContentConstraintCount = component.constraintCount;
			}
			m_loopScrollRect.PrepareCells();
		}
		m_iMaxRoomNameLength = NKMCommonConst.Office.OfficeNamingLimit;
		NKCUtil.SetButtonClickDelegate(m_csbtnConfirm, OnBtnConfirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnRoomNameChange, OnBtnNameChange);
		NKCUtil.SetButtonClickDelegate(m_csbtnAutoSelect, OnBtnAutoSelect);
		NKCUtil.SetButtonClickDelegate(m_csbtnDeselectAll, OnBtnDeselectAll);
		NKCUtil.SetButtonClickDelegate(m_csbtnFilter, OnBtnFilter);
		NKCUtil.SetToggleValueChangedDelegate(m_tglFavorite, OnTglFavorite);
		NKCUtil.SetToggleValueChangedDelegate(m_tglTabUnit, OnTglUnit);
		NKCUtil.SetToggleValueChangedDelegate(m_tglTabTrophy, OnTglTrophy);
		m_inputRoomNameInput.onValueChanged.RemoveAllListeners();
		m_inputRoomNameInput.onValueChanged.AddListener(OnInputNameChanged);
		m_inputRoomNameInput.onValidateInput = NKCFilterManager.FilterEmojiInput;
		m_inputRoomNameInput.onEndEdit.RemoveAllListeners();
		m_inputRoomNameInput.onEndEdit.AddListener(OnInputRoomNameEnd);
		if (m_popupContent == null)
		{
			m_popupContent = base.transform.Find("Contents")?.GetComponent<RectTransform>();
		}
		if (m_animator == null)
		{
			m_animator = GetComponent<Animator>();
		}
		m_fPopupWidth = -1f;
		if (m_popupContent != null && NKCUIManager.FrontCanvas != null)
		{
			m_fPopupWidth = m_popupContent.rect.width * NKCUIManager.FrontCanvas.scaleFactor;
		}
		base.gameObject.SetActive(value: false);
	}

	public override void OnBackButton()
	{
		if (IsSameUnitAssined())
		{
			Close();
		}
		else
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_OFFICE_ASSIGN_CANCEL, OnBtnConfirm, base.Close, NKCUtilString.GET_STRING_SAVE, NKCStringTable.GetString("SI_PF_COMMON_CLOSE"));
		}
	}

	public void Open(int roomId, OnCloseMemberEdit onCloseMemberEdit = null)
	{
		m_iRoomId = roomId;
		m_strRoomName = "";
		NKMOfficeRoom nKMOfficeRoom = NKCUIOfficeMapFront.GetInstance()?.OfficeData.GetOfficeRoom(roomId);
		if (nKMOfficeRoom != null)
		{
			m_strRoomName = nKMOfficeRoom.name;
		}
		m_inputRoomNameInput.text = m_strRoomName;
		m_dOnCloseMemberEdit = onCloseMemberEdit;
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(roomId);
		if (nKMOfficeRoomTemplet != null)
		{
			m_iMaxUnitCount = nKMOfficeRoomTemplet.UnitLimitCount;
		}
		string sectionName = NKCUIOfficeMapFront.GetSectionName(nKMOfficeRoomTemplet.SectionTemplet);
		NKCUtil.SetLabelText(m_lbFloorName, sectionName);
		NKCUtil.SetLabelText(m_inputRoomNameInput.placeholder.GetComponent<Text>(), NKCUIOfficeMapFront.GetDefaultRoomName(roomId));
		SetRoomMemberList(roomId);
		SetRoomMemberCount(m_unitAssignList);
		m_unitListOption.eDeckType = NKM_DECK_TYPE.NDT_NONE;
		m_unitListOption.bHideDeckedUnit = false;
		m_unitListOption.bDescending = true;
		m_unitListOption.bPushBackUnselectable = true;
		m_unitListOption.lstSortOption = new List<NKCUnitSortSystem.eSortOption>();
		m_unitListOption.lstSortOption = NKCUnitSortSystem.AddDefaultSortOptions(m_unitListOption.lstSortOption, NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
		m_unitListOption.lstSortOption.Insert(0, NKCUnitSortSystem.eSortOption.CustomDescend1);
		m_unitListOption.lstDefaultSortOption = null;
		m_unitListOption.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		m_unitListOption.lstCustomSortFunc = new Dictionary<NKCUnitSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>>();
		m_unitListOption.lstCustomSortFunc.Add(NKCUnitSortSystem.eSortCategory.Custom1, new KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>(NKCUtilString.GET_STRING_OFFICE_ROOM_IN, SortUnitByRoomInOut));
		m_unitListOption.setOnlyIncludeFilterOption = null;
		m_unitListOption.PreemptiveSortFunc = null;
		m_unitListOption.AdditionalExcludeFilterFunc = IsUnitStyleType;
		m_unitListOption.bExcludeLockedUnit = false;
		m_unitListOption.bExcludeDeckedUnit = false;
		m_unitListOption.setExcludeUnitUID = null;
		m_unitListOption.setExcludeUnitID = null;
		m_unitListOption.setOnlyIncludeUnitID = null;
		m_unitListOption.setDuplicateUnitID = null;
		m_unitListOption.bIncludeUndeckableUnit = true;
		m_unitListOption.bIncludeSeizure = false;
		m_unitListOption.bUseDeckedState = false;
		m_unitListOption.bUseLockedState = false;
		m_unitListOption.bUseLobbyState = false;
		m_unitListOption.bIgnoreCityState = false;
		m_unitListOption.bIgnoreWorldMapLeader = false;
		m_unitListOption.AdditionalUnitStateFunc = null;
		m_unitListOption.bIgnoreMissionState = false;
		SelectUnitListType(UnitListType.Normal);
		NKCUtil.SetGameobjectActive(m_tglTabTrophy, HasUsableTrophy());
		if (m_tglFavorite != null)
		{
			m_tglFavorite.Select(bSelect: false, bForce: true);
		}
		base.gameObject.SetActive(value: true);
		RefreshScrollRect();
		if (base.IsOpen)
		{
			m_animator.Play("AB_UI_POPUP_OFFICE_MEMBER_EDIT_INTRO", 0, 0f);
			return;
		}
		m_EnvScore?.UpdateEnvScore(nKMOfficeRoom);
		UIOpened();
	}

	public void SortSpecifitUnitFirst(int unitId)
	{
		m_iTutorialUnitId = unitId;
		m_unitListOption.lstCustomSortFunc[NKCUnitSortSystem.eSortCategory.Custom1] = new KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>(NKCUtilString.GET_STRING_OFFICE_ROOM_IN, SortSpecificUnitIdFirst);
		m_unitSortSystem.SortList(m_unitSortSystem.lstSortOption, bForce: true);
		RefreshScrollRect();
	}

	public override void CloseInternal()
	{
		m_unitAssignList = null;
		m_unitSortSystem = null;
		m_strRoomName = null;
		if (m_dOnCloseMemberEdit != null)
		{
			m_dOnCloseMemberEdit();
		}
		m_iTutorialUnitId = 0;
		base.gameObject.SetActive(value: false);
	}

	public void Release()
	{
	}

	public void UpdateRoomName(string roomName)
	{
		m_strRoomName = roomName;
		m_inputRoomNameInput.text = roomName;
	}

	public RectTransform GetRectTransformUnitSlot(int unitId)
	{
		NKCUIOfficeMemberEditSlot[] componentsInChildren = m_loopScrollRect.content.GetComponentsInChildren<NKCUIOfficeMemberEditSlot>();
		int num = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			if (componentsInChildren[i].UnitId == unitId)
			{
				return componentsInChildren[i].GetComponent<RectTransform>();
			}
		}
		return null;
	}

	private bool IsUnitStyleType(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return false;
		}
		return NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID)?.IsUnitStyleType() ?? false;
	}

	private bool IsUnitAssignedInOtherRoom(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return false;
		}
		if (unitData.OfficeRoomId > 0 && m_iRoomId != unitData.OfficeRoomId)
		{
			return true;
		}
		return false;
	}

	private int SortUnitByRoomInOut(NKMUnitData e1, NKMUnitData e2)
	{
		bool flag = e1.OfficeRoomId == m_iRoomId;
		bool flag2 = e2.OfficeRoomId == m_iRoomId;
		if (flag && !flag2)
		{
			return -1;
		}
		if (!flag && flag2)
		{
			return 1;
		}
		return 0;
	}

	private int SortSpecificUnitIdFirst(NKMUnitData e1, NKMUnitData e2)
	{
		bool flag = e1.m_UnitID == m_iTutorialUnitId;
		bool flag2 = e2.m_UnitID == m_iTutorialUnitId;
		if (flag && !flag2)
		{
			return -1;
		}
		if (!flag && flag2)
		{
			return 1;
		}
		return 0;
	}

	private void SetRoomMemberCount(List<long> unitAssignedList)
	{
		int num = 0;
		if (unitAssignedList != null)
		{
			num = unitAssignedList.Count;
		}
		NKCUtil.SetLabelText(m_lbMemberCount, $"{num}/{m_iMaxUnitCount}");
	}

	private void SetRoomMemberList(int roomId)
	{
		m_unitAssignList = new List<long>();
		(NKCUIOfficeMapFront.GetInstance()?.OfficeData.GetOfficeRoom(roomId))?.unitUids.ForEach(delegate(long e)
		{
			m_unitAssignList.Add(e);
		});
	}

	private RectTransform GetMemberSlot(int index)
	{
		return NKCUIOfficeMemberEditSlot.GetNewInstance(null)?.GetComponent<RectTransform>();
	}

	private void ReturnMemberSlot(Transform tr)
	{
		NKCUIOfficeMemberEditSlot component = tr.GetComponent<NKCUIOfficeMemberEditSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	private void ProvideMemberData(Transform tr, int index)
	{
		NKCUIOfficeMemberEditSlot component = tr.GetComponent<NKCUIOfficeMemberEditSlot>();
		if (!(component == null) && m_unitSortSystem != null && m_unitSortSystem.SortedUnitList.Count > index)
		{
			NKMUnitData unitData = m_unitSortSystem.SortedUnitList[index];
			component.SetData(m_unitAssignList, unitData, m_iRoomId, OnSelectUnit);
		}
	}

	private void RefreshScrollRect()
	{
		m_loopScrollRect.TotalCount = m_unitSortSystem.SortedUnitList.Count;
		m_loopScrollRect.StopMovement();
		m_loopScrollRect.SetIndexPosition(0);
		NKCUtil.SetGameobjectActive(m_objFilterSelected, m_unitSortSystem.FilterSet.Count > 0);
	}

	private void AssignUnit(long unitUId)
	{
		if (m_unitAssignList.Count < m_iMaxUnitCount)
		{
			m_unitAssignList.Add(unitUId);
			SetRoomMemberCount(m_unitAssignList);
			m_loopScrollRect.RefreshCells();
		}
	}

	private bool IsSameUnitAssined()
	{
		if (m_unitAssignList == null)
		{
			return true;
		}
		NKMOfficeRoom nKMOfficeRoom = NKCUIOfficeMapFront.GetInstance()?.OfficeData.GetOfficeRoom(m_iRoomId);
		if (nKMOfficeRoom == null)
		{
			return true;
		}
		int count = m_unitAssignList.Count;
		bool result = true;
		if (count != nKMOfficeRoom.unitUids.Count)
		{
			result = false;
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				if (!nKMOfficeRoom.unitUids.Contains(m_unitAssignList[i]))
				{
					result = false;
				}
			}
		}
		return result;
	}

	private void SendUnitAssignPacket()
	{
		if (m_unitAssignList != null)
		{
			NKCPacketSender.Send_NKMPacket_OFFICE_ROOM_SET_UNIT_REQ(m_iRoomId, m_unitAssignList);
		}
	}

	private void OnSelectUnit(long unitUId)
	{
		if (NKMOfficeRoomTemplet.Find(m_iRoomId) == null)
		{
			return;
		}
		NKMUnitData nKMUnitData = NKCScenManager.CurrentUserData()?.m_ArmyData.GetUnitOrTrophyFromUID(unitUId);
		if (nKMUnitData == null || m_unitAssignList == null)
		{
			return;
		}
		if (m_unitAssignList.Contains(unitUId))
		{
			m_unitAssignList.Remove(unitUId);
			SetRoomMemberCount(m_unitAssignList);
			m_loopScrollRect.RefreshCells();
		}
		else if (m_unitAssignList.Count >= m_iMaxUnitCount)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_OFFICE_FULL_ASSIGNED, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (nKMUnitData.OfficeRoomId > 0 && nKMUnitData.OfficeRoomId != m_iRoomId)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_OFFICE_ALREADY_ASSIGNED_UNIT, delegate
			{
				AssignUnit(unitUId);
			});
		}
		else
		{
			AssignUnit(unitUId);
		}
	}

	private void OnInputNameChanged(string _string)
	{
		m_inputRoomNameInput.text = NKCFilterManager.CheckBadChat(m_inputRoomNameInput.text);
		if (m_inputRoomNameInput.text.Length >= m_iMaxRoomNameLength)
		{
			m_inputRoomNameInput.text = m_inputRoomNameInput.text.Substring(0, m_iMaxRoomNameLength);
		}
	}

	private void OnInputRoomNameEnd(string _string)
	{
		m_inputRoomNameInput.text = NKCFilterManager.CheckBadChat(m_inputRoomNameInput.text);
		if (!(m_inputRoomNameInput.text == m_strRoomName))
		{
			if (m_inputRoomNameInput.text.Length >= m_iMaxRoomNameLength)
			{
				string roomName = m_inputRoomNameInput.text.Substring(0, m_iMaxRoomNameLength);
				NKCPacketSender.Send_NKMPacket_OFFICE_SET_ROOM_NAME_REQ(m_iRoomId, roomName);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_OFFICE_SET_ROOM_NAME_REQ(m_iRoomId, m_inputRoomNameInput.text);
			}
		}
	}

	private void OnBtnNameChange()
	{
		m_inputRoomNameInput.Select();
		m_inputRoomNameInput.ActivateInputField();
	}

	private void OnBtnDeselectAll()
	{
		m_unitAssignList.Clear();
		SetRoomMemberCount(m_unitAssignList);
		m_loopScrollRect.RefreshCells();
	}

	private void OnBtnAutoSelect()
	{
		List<NKMUnitData> list = new List<NKMUnitData>();
		int count = m_unitSortSystem.SortedUnitList.Count;
		for (int i = 0; i < count; i++)
		{
			if (!IsUnitAssignedInOtherRoom(m_unitSortSystem.SortedUnitList[i]))
			{
				list.Add(m_unitSortSystem.SortedUnitList[i]);
			}
		}
		list.Sort(delegate(NKMUnitData e1, NKMUnitData e2)
		{
			if (e1.loyalty >= 10000 || e2.loyalty >= 10000)
			{
				if (e1.loyalty >= 10000 && e2.loyalty < 10000)
				{
					return 1;
				}
				if (e1.loyalty < 10000 && e2.loyalty >= 10000)
				{
					return -1;
				}
			}
			if (e1.loyalty != e2.loyalty)
			{
				if (e1.loyalty < e2.loyalty)
				{
					return 1;
				}
				return -1;
			}
			if (e1.m_UnitLevel != e2.m_UnitLevel)
			{
				if (e1.m_UnitLevel < e2.m_UnitLevel)
				{
					return 1;
				}
				return -1;
			}
			if (e1.m_iUnitLevelEXP != e2.m_iUnitLevelEXP)
			{
				if (e1.m_iUnitLevelEXP < e2.m_iUnitLevelEXP)
				{
					return 1;
				}
				return -1;
			}
			NKM_UNIT_GRADE unitGrade = e1.GetUnitGrade();
			NKM_UNIT_GRADE unitGrade2 = e2.GetUnitGrade();
			if (unitGrade != unitGrade2)
			{
				if (unitGrade < unitGrade2)
				{
					return 1;
				}
				return -1;
			}
			int starGrade = e1.GetStarGrade();
			int starGrade2 = e2.GetStarGrade();
			if (starGrade != starGrade2)
			{
				if (starGrade < starGrade2)
				{
					return 1;
				}
				return -1;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(e1.m_UnitID);
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(e2.m_UnitID);
			if (unitTempletBase != null && unitTempletBase2 != null)
			{
				NKMUnitStatTemplet statTemplet = unitTempletBase.StatTemplet;
				NKMUnitStatTemplet statTemplet2 = unitTempletBase2.StatTemplet;
				if (statTemplet != null && statTemplet2 != null)
				{
					int respawnCost = statTemplet.GetRespawnCost(bLeader: false, null);
					int respawnCost2 = statTemplet2.GetRespawnCost(bLeader: false, null);
					if (respawnCost != respawnCost2)
					{
						if (respawnCost < respawnCost2)
						{
							return 1;
						}
						return -1;
					}
				}
			}
			if (e1.m_UnitID != e2.m_UnitID)
			{
				if (e1.m_UnitID > e2.m_UnitID)
				{
					return 1;
				}
				return -1;
			}
			return (e1.m_UnitUID != e2.m_UnitUID) ? ((e1.m_UnitUID < e2.m_UnitUID) ? 1 : (-1)) : 0;
		});
		m_unitAssignList.Clear();
		_ = m_unitAssignList.Count;
		for (int num = 0; num < m_iMaxUnitCount && list.Count > num; num++)
		{
			m_unitAssignList.Add(list[num].m_UnitUID);
		}
		m_loopScrollRect.RefreshCells();
		SetRoomMemberCount(m_unitAssignList);
	}

	private void OnSelectFilter(HashSet<NKCUnitSortSystem.eFilterOption> setFilterOption)
	{
		if (m_unitSortSystem == null)
		{
			return;
		}
		if (m_tglFavorite != null)
		{
			if (m_tglFavorite.IsSelected)
			{
				m_unitSortSystem.FilterSet.Add(NKCUnitSortSystem.eFilterOption.Favorite);
			}
			else
			{
				m_unitSortSystem.FilterSet.Remove(NKCUnitSortSystem.eFilterOption.Favorite);
			}
		}
		m_unitSortSystem.FilterSet = setFilterOption;
		RefreshScrollRect();
	}

	private void OnTglFavorite(bool value)
	{
		OnSelectFilter(m_unitSortSystem.FilterSet);
	}

	private void OnBtnFilter()
	{
		HashSet<NKCUnitSortSystem.eFilterCategory> hashSet = new HashSet<NKCUnitSortSystem.eFilterCategory>();
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.InRoom);
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.Rarity);
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.Locked);
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.UnitType);
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.UnitRole);
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.UnitMoveType);
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.UnitTargetType);
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.Rarity);
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.Cost);
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.Locked);
		hashSet.Add(NKCUnitSortSystem.eFilterCategory.SpecialType);
		if (m_eUnitListType == UnitListType.Normal)
		{
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Loyalty);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.LifeContract);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Level);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Decked);
		}
		NKCPopupFilterUnit.Instance.Open(hashSet, m_unitSortSystem.FilterSet, OnSelectFilter, NKCPopupFilterUnit.FILTER_TYPE.UNIT);
	}

	private void OnBtnConfirm()
	{
		if (m_unitAssignList != null)
		{
			if (!IsSameUnitAssined())
			{
				SendUnitAssignPacket();
			}
			else
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_OFFICE_ASSIGN_COMPLETE, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
			Close();
		}
	}

	private void UpdateEnvScore(int roomID)
	{
		NKMOfficeRoom officeRoom = NKCScenManager.CurrentUserData().OfficeData.GetOfficeRoom(roomID);
		if (officeRoom == null)
		{
			NKCUtil.SetLabelText(m_lbEnvScore, "-");
			NKCUtil.SetLabelText(m_lbEnvInformation, "");
			return;
		}
		NKCUtil.SetLabelText(m_lbEnvScore, officeRoom.interiorScore.ToString());
		NKMOfficeGradeTemplet nKMOfficeGradeTemplet = NKMOfficeGradeTemplet.Find(officeRoom.grade);
		if (nKMOfficeGradeTemplet != null)
		{
			string msg = NKCStringTable.GetString("SI_DP_OFFICE_LOYALTY_SPEED", nKMOfficeGradeTemplet.ChargingTimeHour);
			NKCUtil.SetLabelText(m_lbEnvInformation, msg);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEnvInformation, "");
		}
	}

	private void OnTglUnit(bool value)
	{
		if (value)
		{
			SelectUnitListType(UnitListType.Normal);
		}
	}

	private void OnTglTrophy(bool value)
	{
		if (value)
		{
			SelectUnitListType(UnitListType.Trophy);
		}
	}

	private void SelectUnitListType(UnitListType type)
	{
		switch (type)
		{
		case UnitListType.Normal:
			m_unitListOption.AdditionalExcludeFilterFunc = null;
			m_unitSortSystem = new NKCUnitSort(NKCScenManager.CurrentUserData(), m_unitListOption);
			m_tglTabUnit.Select(bSelect: true, bForce: true);
			break;
		case UnitListType.Trophy:
			m_unitListOption.AdditionalExcludeFilterFunc = FilterUnusableTrophy;
			m_unitSortSystem = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), m_unitListOption, NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyTrophy.Values);
			m_unitSortSystem.lstSortOption = new List<NKCUnitSortSystem.eSortOption>
			{
				NKCUnitSortSystem.eSortOption.Rarity_High,
				NKCUnitSortSystem.eSortOption.UID_First
			};
			m_tglTabTrophy.Select(bSelect: true, bForce: true);
			break;
		}
		m_eUnitListType = type;
		RefreshScrollRect();
	}

	private bool FilterUnusableTrophy(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(unitTempletBase.m_SpineSDName))
		{
			return false;
		}
		return unitTempletBase.m_bDorm;
	}

	private bool HasUsableTrophy()
	{
		foreach (NKMUnitData value in NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyTrophy.Values)
		{
			if (FilterUnusableTrophy(value))
			{
				return true;
			}
		}
		return false;
	}
}
