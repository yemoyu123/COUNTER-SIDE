using System;
using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEmblemList : NKCUIBase
{
	private class ItemMiscDataAndTemplet
	{
		public NKMItemMiscData m_NKMItemMiscData;

		public NKMItemMiscTemplet m_NKMItemMiscTemplet;
	}

	public enum PopupType
	{
		EMBLEM,
		PROFILE
	}

	private enum ProfileType
	{
		UNIT,
		FRAME,
		TITLE
	}

	public delegate void dOnClickOK(int id);

	public delegate void dOnClickProfileOK(int unitId, int skinId, int frameId, int titleId);

	private class CompGradeAndID : IComparer<ItemMiscDataAndTemplet>
	{
		public int Compare(ItemMiscDataAndTemplet x, ItemMiscDataAndTemplet y)
		{
			if (x == null || x.m_NKMItemMiscTemplet == null || x.m_NKMItemMiscData == null)
			{
				return 1;
			}
			if (y == null || y.m_NKMItemMiscTemplet == null || y.m_NKMItemMiscData == null)
			{
				return -1;
			}
			if (x.m_NKMItemMiscTemplet.m_NKM_ITEM_GRADE > y.m_NKMItemMiscTemplet.m_NKM_ITEM_GRADE)
			{
				return -1;
			}
			if (x.m_NKMItemMiscTemplet.m_NKM_ITEM_GRADE < y.m_NKMItemMiscTemplet.m_NKM_ITEM_GRADE)
			{
				return 1;
			}
			return x.m_NKMItemMiscTemplet.m_ItemMiscID.CompareTo(y.m_NKMItemMiscTemplet.m_ItemMiscID);
		}
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_FRIEND";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_EMBLEM";

	private static NKCPopupEmblemList m_Instance;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	public EventTrigger m_etBG;

	public NKCPopupEmblemBigSlot m_NKCPopupEmblemBigSlot;

	public GameObject m_objSlotList;

	public GameObject m_objNoSlotList;

	public LoopScrollRect m_lsrEmblem;

	public LoopScrollRect m_lsrTitle;

	public GridLayoutGroup m_GridLayoutGroup;

	public Text m_lbTitle;

	[Header("프로필 설정")]
	public GameObject m_toggleGroup;

	public NKCUIComToggle m_tglUnitSelect;

	public NKCUIComToggle m_tglFrameSelect;

	public NKCUIComToggle m_tglTitleSelect;

	[Header("프로필 정렬")]
	public NKCUIComUnitSortOptions m_unitSortOptions;

	public GameObject m_objSortRoot;

	public GameObject m_objFilter;

	[Header("프레임/칭호 정렬")]
	public NKCUIComMiscSortOptions m_miscSortOptions;

	public GameObject m_objMiscSortRoot;

	public GameObject m_objMiscFilter;

	private List<NKCUISlot.SlotData> m_lstItemData = new List<NKCUISlot.SlotData>();

	private List<NKCUISlot.SlotData> m_lstFilteredUnitSlotData = new List<NKCUISlot.SlotData>();

	private List<NKCUISlot.SlotData> m_lstFilteredMiscSlotData = new List<NKCUISlot.SlotData>();

	private HashSet<int> m_hsetUnit;

	private bool m_bFirstOpen = true;

	private int m_SelectedID = -1;

	private dOnClickOK m_dOnClickOK;

	private dOnClickProfileOK m_dOnClickProfileOK;

	private int m_EquippedEmblemID = -1;

	private int m_selectedUnitId;

	private int m_selectedSkinId;

	private int m_selectedFrameId = -1;

	private int m_selectedTitleId = -1;

	private PopupType m_popupType;

	private ProfileType m_profileType;

	private NKCUnitSortSystem m_unitSortSystem;

	private List<NKMUnitData> m_lstUnitData = new List<NKMUnitData>();

	private NKCMiscSortSystem m_miscSortSystem;

	private List<NKMItemMiscTemplet> m_lstMiscTemplet = new List<NKMItemMiscTemplet>();

	private NKCPopupEmblemChangeConfirm m_NKCPopupEmblemChangeConfirm;

	public static NKCPopupEmblemList Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEmblemList>("AB_UI_NKM_UI_FRIEND", "NKM_UI_POPUP_EMBLEM", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupEmblemList>();
				m_Instance?.InitUI();
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

	public override string MenuName => "PopupEmblemList";

	private NKCPopupEmblemChangeConfirm NKCPopupEmblemChangeConfirm
	{
		get
		{
			if (m_NKCPopupEmblemChangeConfirm == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupEmblemChangeConfirm>("AB_UI_NKM_UI_FRIEND", "NKM_UI_POPUP_EMBLEM_CONFIRM", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCPopupEmblemChangeConfirm = loadedUIData.GetInstance<NKCPopupEmblemChangeConfirm>();
				m_NKCPopupEmblemChangeConfirm.InitUI();
			}
			return m_NKCPopupEmblemChangeConfirm;
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void CheckNKCPopupEmblemChangeConfirmAndClose()
	{
		if (m_NKCPopupEmblemChangeConfirm != null && m_NKCPopupEmblemChangeConfirm.IsOpen)
		{
			m_NKCPopupEmblemChangeConfirm.Close();
		}
	}

	private void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_lsrEmblem.dOnGetObject += GetEmblemSlot;
		m_lsrEmblem.dOnReturnObject += ReturnSlot;
		m_lsrEmblem.dOnProvideData += ProvideEmblemSlot;
		NKCUtil.SetScrollHotKey(m_lsrEmblem);
		m_lsrEmblem.ContentConstraintCount = 8;
		m_lsrTitle.dOnGetObject += GetTitleSlot;
		m_lsrTitle.dOnReturnObject += ReturnSlot;
		m_lsrTitle.dOnProvideData += ProvideTitleSlot;
		NKCUtil.SetScrollHotKey(m_lsrTitle);
		NKCUtil.SetBindFunction(m_csbtnOK, OnClickOK);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_csbtnCancel, OnCloseBtn);
		NKCUtil.SetToggleValueChangedDelegate(m_tglUnitSelect, OnToggleUnitSelect);
		NKCUtil.SetToggleValueChangedDelegate(m_tglFrameSelect, OnToggleFrameSelect);
		NKCUtil.SetToggleValueChangedDelegate(m_tglTitleSelect, OnToggleTitleSelect);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			OnCloseBtn();
		});
		m_etBG.triggers.Add(entry);
	}

	private void OnClickOK()
	{
		if (m_popupType == PopupType.EMBLEM)
		{
			if (m_SelectedID != -1)
			{
				NKCPopupEmblemChangeConfirm.Open(m_EquippedEmblemID, m_SelectedID, delegate(int e)
				{
					m_dOnClickOK(e);
				});
			}
			else
			{
				Close();
			}
		}
		else if (m_popupType == PopupType.PROFILE)
		{
			NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
			bool flag = m_selectedUnitId == userProfileData.commonProfile.mainUnitId && m_selectedSkinId == userProfileData.commonProfile.mainUnitSkinId;
			bool num = m_selectedUnitId > 0 && !flag;
			bool flag2 = m_selectedFrameId == userProfileData.commonProfile.frameId;
			bool flag3 = m_selectedFrameId >= 0 && !flag2;
			bool flag4 = m_selectedTitleId == userProfileData.commonProfile.titleId;
			bool flag5 = NKMTitleTemplet.IsOwnedTitle(m_selectedTitleId);
			bool flag6 = (m_selectedTitleId == 0 || flag5) && !flag4;
			if (!num && !flag3 && !flag6)
			{
				Close();
			}
			else if (m_dOnClickProfileOK != null)
			{
				m_dOnClickProfileOK(m_selectedUnitId, m_selectedSkinId, m_selectedFrameId, m_selectedTitleId);
			}
		}
	}

	public RectTransform GetEmblemSlot(int index)
	{
		return NKCUISlot.GetNewInstance(null).GetComponent<RectTransform>();
	}

	public void ReturnSlot(Transform tr)
	{
		tr.SetParent(base.transform);
		UnityEngine.Object.Destroy(tr.gameObject);
	}

	public void ProvideEmblemSlot(Transform tr, int index)
	{
		NKCUISlot component = tr.GetComponent<NKCUISlot>();
		if (component == null)
		{
			return;
		}
		if (m_popupType == PopupType.EMBLEM)
		{
			if (m_lstItemData.Count > index)
			{
				if (m_lstItemData[index].ID != 0)
				{
					component.SetData(m_lstItemData[index], bEnableLayoutElement: true, OnClickSlot);
				}
				else
				{
					component.SetEmpty(OnClickSlot);
				}
				component.SetSelected(m_SelectedID == m_lstItemData[index].ID);
			}
		}
		else
		{
			if (m_popupType != PopupType.PROFILE)
			{
				return;
			}
			if (m_profileType == ProfileType.UNIT)
			{
				if (m_lstFilteredUnitSlotData.Count > index)
				{
					component.SetUnitData(m_lstFilteredUnitSlotData[index], bShowName: false, bShowLevel: false, bEnableLayoutElement: true, OnClickSlot);
					component.SetMaxLevelTacticFX(NKCTacticUpdateUtil.HasMaxTacticLevelToSameUnits(m_lstFilteredUnitSlotData[index].ID));
					bool flag = false;
					if (m_lstFilteredUnitSlotData[index].Data != 0 && !m_hsetUnit.Contains(m_lstFilteredUnitSlotData[index].ID))
					{
						flag = true;
					}
					if (flag)
					{
						component.SetShowArrowBGText(bSet: true);
						component.SetArrowBGText(NKCStringTable.GetString("SI_DP_PROFILE_REPRESENT_SKIN_ONLY_NO_UNIT"), NKCUtil.GetColor("#A30000"));
						component.SetDisable(disable: true);
					}
					else
					{
						component.SetShowArrowBGText(bSet: false);
						component.SetDisable(disable: false);
					}
					component.SetSelected(m_selectedUnitId == m_lstFilteredUnitSlotData[index].ID && m_selectedSkinId == m_lstFilteredUnitSlotData[index].Data);
				}
			}
			else
			{
				if (m_profileType != ProfileType.FRAME)
				{
					return;
				}
				if (index == 0)
				{
					component.SetEmpty(OnClickSlot);
					component.SetSelected(m_selectedFrameId == 0);
					return;
				}
				if (m_lstFilteredMiscSlotData[index - 1].ID != 0)
				{
					component.SetMiscItemData(m_lstFilteredMiscSlotData[index - 1], bShowName: false, bShowCount: false, bEnableLayoutElement: true, OnClickSlot);
				}
				else
				{
					component.SetEmpty(OnClickSlot);
				}
				component.SetSelected(m_selectedFrameId == m_lstFilteredMiscSlotData[index - 1].ID);
			}
		}
	}

	public RectTransform GetTitleSlot(int index)
	{
		return NKCUISlotTitle.GetNewInstance(null).GetComponent<RectTransform>();
	}

	public void ProvideTitleSlot(Transform tr, int index)
	{
		if (m_lstFilteredMiscSlotData.Count < index)
		{
			return;
		}
		NKCUISlotTitle component = tr.GetComponent<NKCUISlotTitle>();
		if (!(component == null))
		{
			if (index == 0)
			{
				component.SetData(0, showEmpty: true, showLock: true, OnClickTitleSlot, showTimeLeft: true, showEffect: false);
				component.SetSelected(m_selectedTitleId == 0);
			}
			else
			{
				component.SetData(m_lstFilteredMiscSlotData[index - 1].ID, showEmpty: true, showLock: true, OnClickTitleSlot, showTimeLeft: true, showEffect: false);
				component.SetSelected(m_selectedTitleId == m_lstFilteredMiscSlotData[index - 1].ID);
			}
		}
	}

	private void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (slotData != null)
		{
			if (m_popupType == PopupType.EMBLEM)
			{
				m_SelectedID = slotData.ID;
				m_NKCPopupEmblemBigSlot.SetEmblemData(m_SelectedID);
			}
			else if (m_popupType == PopupType.PROFILE)
			{
				if (m_profileType == ProfileType.UNIT)
				{
					m_selectedUnitId = slotData.ID;
					m_selectedSkinId = slotData.Data;
					SetProfilePreview(m_profileType, m_selectedUnitId, m_selectedSkinId, m_selectedFrameId);
				}
				else if (m_profileType == ProfileType.FRAME)
				{
					m_selectedFrameId = slotData.ID;
					SetProfilePreview(m_profileType, m_selectedUnitId, m_selectedSkinId, m_selectedFrameId);
				}
			}
		}
		else if (m_popupType == PopupType.EMBLEM)
		{
			m_SelectedID = 0;
			m_NKCPopupEmblemBigSlot.SetEmblemEmpty(NKCUtilString.GET_STRING_EMBLEM_EQUIPPED_EMBLEM_UNEQUIP);
		}
		else if (m_popupType == PopupType.PROFILE)
		{
			m_selectedFrameId = 0;
			SetProfilePreview(m_profileType, m_selectedUnitId, m_selectedSkinId, m_selectedFrameId);
		}
		m_lsrEmblem.RefreshCells();
	}

	private void OnClickTitleSlot(int titleId)
	{
		m_selectedTitleId = titleId;
		SetTitlePreview(m_selectedTitleId);
		m_lsrTitle.RefreshCells();
		bool flag = NKMTitleTemplet.IsOwnedTitle(titleId);
		m_csbtnOK.SetLock(titleId > 0 && !flag);
	}

	private void GetEmblemDataList(NKMUserProfileData cNKMUserProfileData, bool bUseEmpty, ref List<NKCUISlot.SlotData> dataList)
	{
		List<NKMItemMiscData> emblemData = NKCScenManager.CurrentUserData().m_InventoryData.GetEmblemData();
		for (int i = 0; i < cNKMUserProfileData.emblems.Count; i++)
		{
			if (cNKMUserProfileData.emblems[i] == null)
			{
				continue;
			}
			for (int j = 0; j < emblemData.Count; j++)
			{
				if (emblemData[j].ItemID == cNKMUserProfileData.emblems[i].id)
				{
					emblemData.RemoveAt(j);
					break;
				}
			}
		}
		List<ItemMiscDataAndTemplet> list = new List<ItemMiscDataAndTemplet>();
		dataList.Clear();
		for (int k = 0; k < emblemData.Count; k++)
		{
			ItemMiscDataAndTemplet itemMiscDataAndTemplet = new ItemMiscDataAndTemplet();
			itemMiscDataAndTemplet.m_NKMItemMiscData = emblemData[k];
			itemMiscDataAndTemplet.m_NKMItemMiscTemplet = NKMItemManager.GetItemMiscTempletByID(emblemData[k].ItemID);
			list.Add(itemMiscDataAndTemplet);
		}
		list.Sort(new CompGradeAndID());
		if (bUseEmpty)
		{
			ItemMiscDataAndTemplet itemMiscDataAndTemplet2 = new ItemMiscDataAndTemplet();
			itemMiscDataAndTemplet2.m_NKMItemMiscData = null;
			itemMiscDataAndTemplet2.m_NKMItemMiscTemplet = null;
			list.Insert(0, itemMiscDataAndTemplet2);
		}
		int count = list.Count;
		for (int l = 0; l < count; l++)
		{
			NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeMiscItemData(list[l].m_NKMItemMiscData);
			dataList.Add(item);
		}
	}

	private void GetProfileList(ref List<NKCUISlot.SlotData> dataList)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		m_hsetUnit = new HashSet<int>(myUserData.m_ArmyData.m_illustrateUnit);
		IEnumerable<int> skinIds = myUserData.m_InventoryData.SkinIds;
		if (m_hsetUnit == null)
		{
			return;
		}
		List<(NKMUnitTempletBase, NKMSkinTemplet)> list = new List<(NKMUnitTempletBase, NKMSkinTemplet)>();
		if (skinIds != null)
		{
			foreach (int item2 in skinIds)
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(item2);
				if (skinTemplet != null)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(skinTemplet.m_SkinEquipUnitID);
					if (unitTempletBase != null && unitTempletBase.m_bProfileUnit)
					{
						list.Add((unitTempletBase, skinTemplet));
					}
				}
			}
		}
		HashSet<int> hashSet = new HashSet<int>();
		foreach (int item3 in m_hsetUnit)
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(item3);
			if (unitTempletBase2 != null && unitTempletBase2.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && unitTempletBase2.m_bProfileUnit)
			{
				list.Add((unitTempletBase2, null));
				if (!m_hsetUnit.Contains(unitTempletBase2.m_BaseUnitID))
				{
					hashSet.Add(unitTempletBase2.m_BaseUnitID);
				}
			}
		}
		m_hsetUnit.UnionWith(hashSet);
		if (list.Count <= 0)
		{
			return;
		}
		dataList.Clear();
		m_lstUnitData.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			int num = 0;
			int unitID = list[i].Item1.m_UnitID;
			if (list[i].Item2 != null)
			{
				num = list[i].Item2.m_SkinID;
			}
			if (num == 0 && list[i].Item1.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && !list[i].Item1.IsTrophy && NKCCollectionManager.GetUnitTemplet(unitID) == null)
			{
				continue;
			}
			NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeUnitData(unitID, 0, num);
			dataList.Add(item);
			NKMUnitData unitData = new NKMUnitData(unitID, i, islock: false, isPermanentContract: false, isSeized: false, fromContract: false);
			unitData.m_SkinID = num;
			unitData.m_UnitUID = DateTime.MaxValue.Ticks;
			unitData.tacticLevel = 0;
			List<NKMUnitData> unitListByUnitID = NKCScenManager.CurrentArmyData().GetUnitListByUnitID(unitID);
			if (unitListByUnitID.Count > 0)
			{
				for (int j = 0; j < unitListByUnitID.Count; j++)
				{
					if (unitListByUnitID[j].m_regDate.Ticks < unitData.m_UnitUID)
					{
						unitData.m_UnitUID = unitListByUnitID[j].m_regDate.Ticks;
					}
					if (unitListByUnitID[j].tacticLevel > unitData.tacticLevel)
					{
						unitData.tacticLevel = unitListByUnitID[j].tacticLevel;
					}
				}
			}
			else
			{
				unitData.m_UnitUID = DateTime.Now.Ticks;
			}
			if (unitData.m_SkinID > 0)
			{
				unitData.m_UnitUID += unitData.m_SkinID;
			}
			while (m_lstUnitData.Find((NKMUnitData x) => x.m_UnitUID == unitData.m_UnitUID) != null)
			{
				unitData.m_UnitUID++;
			}
			m_lstUnitData.Add(unitData);
		}
	}

	private int Compare(NKCUISlot.SlotData x, NKCUISlot.SlotData y)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(x.ID);
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(x.Data);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(y.ID);
		NKMSkinTemplet skinTemplet2 = NKMSkinManager.GetSkinTemplet(y.Data);
		if (skinTemplet != null && skinTemplet2 == null)
		{
			if (!m_unitSortSystem.Descending)
			{
				return 1;
			}
			return -1;
		}
		if (skinTemplet == null && skinTemplet2 != null)
		{
			if (!m_unitSortSystem.Descending)
			{
				return -1;
			}
			return 1;
		}
		if (skinTemplet != null && skinTemplet2 != null)
		{
			if (skinTemplet.m_SkinEquipUnitID < skinTemplet2.m_SkinEquipUnitID)
			{
				if (m_unitSortSystem.Descending)
				{
					return 1;
				}
				return -1;
			}
			if (skinTemplet.m_SkinEquipUnitID > skinTemplet2.m_SkinEquipUnitID)
			{
				if (m_unitSortSystem.Descending)
				{
					return -1;
				}
				return 1;
			}
			if (skinTemplet.m_SkinGrade > skinTemplet2.m_SkinGrade)
			{
				if (m_unitSortSystem.Descending)
				{
					return 1;
				}
				return -1;
			}
			if (skinTemplet.m_SkinGrade < skinTemplet2.m_SkinGrade)
			{
				if (m_unitSortSystem.Descending)
				{
					return -1;
				}
				return 1;
			}
			return skinTemplet.m_SkinID.CompareTo(skinTemplet2.m_SkinID);
		}
		if (m_unitSortSystem.Descending)
		{
			return unitTempletBase2.m_UnitID.CompareTo(unitTempletBase.m_UnitID);
		}
		return unitTempletBase.m_UnitID.CompareTo(unitTempletBase2.m_UnitID);
	}

	private void GetFrameList(ref List<NKCUISlot.SlotData> dataList)
	{
		m_lstMiscTemplet.Clear();
		dataList.Clear();
		NKMInventoryData nKMInventoryData = NKCScenManager.CurrentUserData()?.m_InventoryData;
		if (nKMInventoryData == null)
		{
			return;
		}
		foreach (NKMItemMiscTemplet value in NKMItemMiscTemplet.Values)
		{
			if (value.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME && nKMInventoryData.GetCountMiscItem(value.Key) > 0)
			{
				NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeMiscItemData(value.Key, 1L);
				dataList.Add(item);
				m_lstMiscTemplet.Add(value);
			}
		}
	}

	private void GetTitleList(ref List<NKCUISlot.SlotData> dataList)
	{
		m_lstMiscTemplet.Clear();
		dataList.Clear();
		NKMInventoryData inventoryData = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData;
		if (inventoryData == null)
		{
			return;
		}
		foreach (NKMItemMiscTemplet value in NKMItemMiscTemplet.Values)
		{
			if (value.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_TITLE)
			{
				continue;
			}
			NKMTitleTemplet nKMTitleTemplet = NKMTitleTemplet.Find(value.m_ItemMiscID);
			if (nKMTitleTemplet == null || !nKMTitleTemplet.EnableByTag)
			{
				continue;
			}
			if (nKMTitleTemplet.bExclude)
			{
				NKMItemMiscData itemMisc = inventoryData.GetItemMisc(value.m_ItemMiscID);
				if (itemMisc == null || itemMisc.TotalCount == 0L)
				{
					continue;
				}
			}
			NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeMiscItemData(value.Key, 1L);
			dataList.Add(item);
			m_lstMiscTemplet.Add(value);
		}
	}

	private void ResetSortUI(HashSet<NKCUnitSortSystem.eSortCategory> sortCategory, HashSet<NKCUnitSortSystem.eFilterCategory> filterCategory)
	{
		NKCUnitSortSystem.UnitListOptions options = default(NKCUnitSortSystem.UnitListOptions);
		options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>
		{
			NKCUnitSortSystem.eSortOption.CustomAscend1,
			NKCUnitSortSystem.eSortOption.IDX_First,
			NKCUnitSortSystem.eSortOption.UID_First
		};
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.bIncludeUndeckableUnit = true;
		options.lstCustomSortFunc = new Dictionary<NKCUnitSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>>();
		options.lstCustomSortFunc.Add(NKCUnitSortSystem.eSortCategory.Custom1, new KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>(NKCUtilString.GET_STRING_SORT_IDX, SortUnitByUnitID));
		m_unitSortSystem = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options, m_lstUnitData);
		m_unitSortSystem.Descending = false;
		m_unitSortOptions.Init(OnSorted, bIsCollection: false);
		m_unitSortOptions.RegisterCategories(filterCategory, sortCategory, bFavoriteFilterActive: false);
		m_unitSortOptions.RegisterUnitSort(m_unitSortSystem);
		m_unitSortOptions.m_NKCPopupSort.m_bUseDefaultSortAdd = false;
		m_unitSortOptions.ResetUI();
		m_lstFilteredUnitSlotData.Clear();
		for (int i = 0; i < m_unitSortSystem.SortedUnitList.Count; i++)
		{
			NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeUnitData(m_unitSortSystem.SortedUnitList[i]);
			m_lstFilteredUnitSlotData.Add(item);
		}
		NKCUtil.SetGameobjectActive(m_objFilter, filterCategory != null && filterCategory.Count > 0);
	}

	private int SortUnitByUnitID(NKMUnitData lData, NKMUnitData rData)
	{
		return rData.m_UnitID.CompareTo(lData.m_UnitID);
	}

	private void ResetSortUI(HashSet<NKCMiscSortSystem.eSortCategory> sortCategory, HashSet<NKCMiscSortSystem.eFilterCategory> filterCategory)
	{
		NKCMiscSortSystem.MiscListOptions options = default(NKCMiscSortSystem.MiscListOptions);
		if (m_profileType == ProfileType.FRAME)
		{
			options.lstSortOption = NKCMiscSortSystem.GetFrameSortList();
		}
		else if (m_profileType == ProfileType.TITLE)
		{
			options.lstSortOption = NKCMiscSortSystem.GetTitleSortList();
		}
		options.setFilterOption = new HashSet<NKCMiscSortSystem.eFilterOption>();
		m_miscSortSystem = new NKCMiscSortSystem(NKCScenManager.CurrentUserData(), m_lstMiscTemplet, options);
		m_miscSortOptions.Init(OnSorted);
		m_miscSortOptions.RegisterCategories(filterCategory, sortCategory);
		m_miscSortOptions.RegisterMiscSort(m_miscSortSystem);
		m_miscSortOptions.ResetUI();
		m_lstFilteredMiscSlotData.Clear();
		m_lstFilteredMiscSlotData.AddRange(m_lstItemData);
		NKCUtil.SetGameobjectActive(m_objFilter, filterCategory != null && filterCategory.Count > 0);
	}

	private void OnSorted(bool bResetScroll)
	{
		if (m_unitSortSystem.lstSortOption == null || m_unitSortSystem.lstSortOption.Count <= 0 || m_unitSortSystem.lstSortOption[0] == NKCUnitSortSystem.eSortOption.None)
		{
			return;
		}
		if (m_profileType == ProfileType.UNIT)
		{
			m_lstFilteredUnitSlotData.Clear();
			m_lstUnitData = m_unitSortSystem.SortedUnitList;
			for (int i = 0; i < m_unitSortSystem.SortedUnitList.Count; i++)
			{
				NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeUnitData(m_unitSortSystem.SortedUnitList[i]);
				m_lstFilteredUnitSlotData.Add(item);
			}
			m_lsrEmblem.TotalCount = m_lstFilteredUnitSlotData.Count;
			m_lsrEmblem.SetIndexPosition(0);
		}
		else if (m_profileType == ProfileType.FRAME)
		{
			m_lstFilteredMiscSlotData.Clear();
			NKMInventoryData nKMInventoryData = NKCScenManager.CurrentUserData()?.m_InventoryData;
			for (int j = 0; j < m_miscSortSystem.SortedMiscList.Count; j++)
			{
				if (nKMInventoryData.GetCountMiscItem(m_miscSortSystem.SortedMiscList[j].Key) > 0)
				{
					NKCUISlot.SlotData item2 = NKCUISlot.SlotData.MakeMiscItemData(m_miscSortSystem.SortedMiscList[j].Key, 1L);
					m_lstFilteredMiscSlotData.Add(item2);
				}
			}
			m_lsrEmblem.TotalCount = m_lstFilteredMiscSlotData.Count + 1;
			m_lsrEmblem.SetIndexPosition(0);
		}
		else
		{
			if (m_profileType != ProfileType.TITLE)
			{
				return;
			}
			m_lstFilteredMiscSlotData.Clear();
			_ = NKCScenManager.CurrentUserData()?.m_InventoryData;
			for (int k = 0; k < m_miscSortSystem.SortedMiscList.Count; k++)
			{
				NKMTitleTemplet nKMTitleTemplet = NKMTitleTemplet.Find(m_miscSortSystem.SortedMiscList[k].m_ItemMiscID);
				if (nKMTitleTemplet != null && nKMTitleTemplet.EnableByTag)
				{
					NKCUISlot.SlotData item3 = NKCUISlot.SlotData.MakeMiscItemData(m_miscSortSystem.SortedMiscList[k].Key, 1L);
					m_lstFilteredMiscSlotData.Add(item3);
				}
			}
			m_lsrTitle.TotalCount = m_lstFilteredMiscSlotData.Count + 1;
			m_lsrTitle.SetIndexPosition(0);
		}
	}

	private void SetProfilePreview(ProfileType profileType, int selectedUnitId, int selectedSkinId, int selectedFrameId)
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		int frameId = userProfileData?.commonProfile.frameId ?? 0;
		int unitId = userProfileData?.commonProfile.mainUnitId ?? 0;
		int skinId = userProfileData?.commonProfile.mainUnitSkinId ?? 0;
		bool flag = NKCTacticUpdateUtil.HasMaxTacticLevelToSameUnits(selectedUnitId);
		if (selectedUnitId > 0)
		{
			unitId = selectedUnitId;
			skinId = selectedSkinId;
		}
		if (selectedFrameId >= 0)
		{
			frameId = selectedFrameId;
		}
		switch (profileType)
		{
		case ProfileType.UNIT:
			m_NKCPopupEmblemBigSlot.SetProfileData(unitId, skinId, frameId, flag);
			break;
		case ProfileType.FRAME:
			m_NKCPopupEmblemBigSlot.SetFrameData(unitId, skinId, frameId, flag);
			break;
		}
	}

	private void SetTitlePreview(int selectedTitleId)
	{
		int titleData = NKCScenManager.CurrentUserData().UserProfileData?.commonProfile.titleId ?? 0;
		if (selectedTitleId >= 0)
		{
			titleData = selectedTitleId;
		}
		m_NKCPopupEmblemBigSlot.SetTitleData(titleData);
	}

	public void Open(int equippedEmblemID, bool bUseEmpty, dOnClickOK _dOnClickOK = null)
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData != null)
		{
			m_popupType = PopupType.EMBLEM;
			NKCUtil.SetGameobjectActive(m_objSortRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMiscSortRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_toggleGroup, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCPopupEmblemBigSlot, bValue: true);
			NKCUtil.SetGameobjectActive(m_lsrEmblem, bValue: true);
			NKCUtil.SetGameobjectActive(m_lsrTitle, bValue: false);
			m_dOnClickOK = _dOnClickOK;
			m_EquippedEmblemID = equippedEmblemID;
			m_SelectedID = -1;
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_PF_FRIEND_EMBLEM"));
			if (bUseEmpty)
			{
				m_NKCPopupEmblemBigSlot.SetEmblemData(equippedEmblemID);
			}
			else
			{
				m_NKCPopupEmblemBigSlot.SetEmblemEmpty();
			}
			GetEmblemDataList(userProfileData, bUseEmpty, ref m_lstItemData);
			if (m_lstItemData.Count <= 0)
			{
				NKCUtil.SetGameobjectActive(m_objNoSlotList, bValue: true);
				m_NKCPopupEmblemBigSlot.SetEmblemEmpty();
				NKCUtil.SetGameobjectActive(m_objSlotList, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objNoSlotList, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSlotList, bValue: true);
			}
			m_csbtnOK.SetLock(value: false);
			UIOpened();
			if (m_bFirstOpen)
			{
				m_lsrEmblem.PrepareCells();
				m_lsrTitle.PrepareCells();
				m_bFirstOpen = false;
			}
			m_lsrEmblem.TotalCount = m_lstItemData.Count;
			m_lsrEmblem.SetIndexPosition(0);
			m_lsrEmblem.velocity = new Vector2(0f, 0f);
		}
	}

	public void OpenProfileEdit(dOnClickProfileOK onClickProfileOk)
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData == null)
		{
			NKCUtil.SetGameobjectActive(m_NKCPopupEmblemBigSlot, bValue: false);
			return;
		}
		m_popupType = PopupType.PROFILE;
		NKCUtil.SetGameobjectActive(m_toggleGroup, bValue: true);
		NKCUtil.SetGameobjectActive(m_tglFrameSelect, NKCContentManager.IsContentsUnlocked(ContentsType.PROFILE_FRAME));
		NKCUtil.SetGameobjectActive(m_tglTitleSelect, NKMTitleTemplet.TitleOpenTagEnabled);
		m_dOnClickProfileOK = onClickProfileOk;
		m_selectedUnitId = 0;
		m_selectedSkinId = 0;
		m_selectedFrameId = -1;
		m_selectedTitleId = -1;
		NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_PF_PROFILE_TITLE"));
		NKCUtil.SetGameobjectActive(m_NKCPopupEmblemBigSlot, bValue: true);
		m_NKCPopupEmblemBigSlot.SetProfileData(userProfileData);
		NKCUtil.SetGameobjectActive(m_objSlotList, bValue: true);
		NKCUtil.SetGameobjectActive(m_objNoSlotList, bValue: false);
		UIOpened();
		if (m_bFirstOpen)
		{
			m_lsrEmblem.PrepareCells();
			m_lsrTitle.PrepareCells();
			m_bFirstOpen = false;
		}
		m_tglUnitSelect.Select(bSelect: false, bForce: true);
		m_tglUnitSelect.Select(bSelect: true);
	}

	public void CloseEmblemListPopup()
	{
		Close();
	}

	public void OnCloseBtn()
	{
		Close();
	}

	private void OnToggleUnitSelect(bool value)
	{
		if (value)
		{
			if (m_profileType == ProfileType.TITLE && m_selectedTitleId > 0 && !NKMTitleTemplet.IsOwnedTitle(m_selectedTitleId))
			{
				m_selectedTitleId = -1;
				SetTitlePreview(m_selectedTitleId);
			}
			NKCUtil.SetGameobjectActive(m_lsrEmblem, bValue: true);
			NKCUtil.SetGameobjectActive(m_lsrTitle, bValue: false);
			m_profileType = ProfileType.UNIT;
			GetProfileList(ref m_lstItemData);
			ResetSortUI(new HashSet<NKCUnitSortSystem.eSortCategory>
			{
				NKCUnitSortSystem.eSortCategory.Custom1,
				NKCUnitSortSystem.eSortCategory.UID,
				NKCUnitSortSystem.eSortCategory.Rarity
			}, new HashSet<NKCUnitSortSystem.eFilterCategory>
			{
				NKCUnitSortSystem.eFilterCategory.UnitType,
				NKCUnitSortSystem.eFilterCategory.UnitRole,
				NKCUnitSortSystem.eFilterCategory.Rarity,
				NKCUnitSortSystem.eFilterCategory.TacticLv,
				NKCUnitSortSystem.eFilterCategory.Skin
			});
			NKCUtil.SetGameobjectActive(m_objSortRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMiscSortRoot, bValue: false);
			m_lsrEmblem.TotalCount = m_unitSortSystem.SortedUnitList.Count;
			m_lsrEmblem.SetIndexPosition(0);
			m_lsrEmblem.velocity = new Vector2(0f, 0f);
			SetProfilePreview(m_profileType, m_selectedUnitId, m_selectedSkinId, m_selectedFrameId);
			m_csbtnOK.SetLock(value: false);
		}
	}

	private void OnToggleFrameSelect(bool value)
	{
		if (NKCContentManager.IsContentsUnlocked(ContentsType.PROFILE_FRAME) && value)
		{
			if (m_profileType == ProfileType.TITLE && m_selectedTitleId > 0 && !NKMTitleTemplet.IsOwnedTitle(m_selectedTitleId))
			{
				m_selectedTitleId = -1;
				SetTitlePreview(m_selectedTitleId);
			}
			NKCUtil.SetGameobjectActive(m_lsrEmblem, bValue: true);
			NKCUtil.SetGameobjectActive(m_lsrTitle, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSortRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMiscSortRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMiscFilter, bValue: false);
			m_profileType = ProfileType.FRAME;
			GetFrameList(ref m_lstItemData);
			ResetSortUI(NKCMiscSortSystem.GetDefaultFrameSortCategory(), NKCMiscSortSystem.GetDefaultFrameFilterCategory());
			m_lsrEmblem.TotalCount = m_lstItemData.Count + 1;
			m_lsrEmblem.SetIndexPosition(0);
			m_lsrEmblem.velocity = new Vector2(0f, 0f);
			SetProfilePreview(m_profileType, m_selectedUnitId, m_selectedSkinId, m_selectedFrameId);
			m_csbtnOK.SetLock(value: false);
		}
	}

	private void OnToggleTitleSelect(bool value)
	{
		if (value)
		{
			NKCUtil.SetGameobjectActive(m_lsrEmblem, bValue: false);
			NKCUtil.SetGameobjectActive(m_lsrTitle, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSortRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMiscSortRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMiscFilter, bValue: true);
			m_profileType = ProfileType.TITLE;
			GetTitleList(ref m_lstItemData);
			ResetSortUI(NKCMiscSortSystem.GetDefaultTitleSortCategory(), NKCMiscSortSystem.GetDefaultTitleFilterCategory());
			m_lsrTitle.TotalCount = m_lstItemData.Count + 1;
			m_lsrTitle.SetIndexPosition(0);
			m_lsrTitle.velocity = new Vector2(0f, 0f);
			SetTitlePreview(m_selectedTitleId);
			bool flag = NKMTitleTemplet.IsOwnedTitle(m_selectedTitleId);
			m_csbtnOK.SetLock(m_selectedTitleId > 0 && !flag);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		CheckNKCPopupEmblemChangeConfirmAndClose();
		m_lstItemData.Clear();
		m_hsetUnit?.Clear();
		m_hsetUnit = null;
		m_EquippedEmblemID = -1;
		m_SelectedID = -1;
		m_selectedUnitId = 0;
		m_selectedSkinId = 0;
		m_selectedFrameId = -1;
		m_selectedTitleId = -1;
		m_dOnClickOK = null;
		m_dOnClickProfileOK = null;
	}

	private void OnDestroy()
	{
		m_unitSortSystem = null;
		m_miscSortSystem = null;
		m_lstUnitData.Clear();
		m_lstMiscTemplet.Clear();
		m_Instance = null;
	}
}
