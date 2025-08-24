using System.Collections.Generic;
using ClientPacket.Office;
using NKC.UI.Component.Office;
using NKM;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIPopupOfficeInteriorSelect : NKCUIBase
{
	public enum Category
	{
		DECO,
		FURNITURE,
		THEME
	}

	public enum Mode
	{
		FurnitureEdit,
		Listview
	}

	public delegate void OnSelectInterior(int id);

	public delegate void OnSelectPreset(int id);

	private const string ASSET_BUNDLE_NAME = "ab_ui_office";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_OFFICE_FNC_LIST";

	private static NKCUIPopupOfficeInteriorSelect m_Instance;

	public Text m_lbTitle;

	public NKCUIComStateButton m_csbtnClose;

	[Header("왼쪽")]
	public NKCUIComToggle m_tglTabFuniture;

	public NKCUIComToggle m_tglTabBackground;

	public NKCUIComToggle m_tglTabThemePreset;

	[Header("가구 프리뷰")]
	public NKCUIComOfficeFurniturePreview m_comFurniturePreview;

	public GameObject m_objNoFunitureSelect;

	public Text m_lbNoFurnitureSelect;

	public NKCUIComStateButton m_csbtnInstall;

	[Header("가구목록")]
	public NKCUIComMiscSortOptions m_sortOptions;

	public LoopScrollRect m_srFuniture;

	public NKCUISlot m_pfbSlot;

	public GridLayoutGroup m_GridLayoutGroup;

	public GameObject m_objNoFuniture;

	public Text m_lbNoFuniture;

	private const int MIN_COLUMN_COUNT = 3;

	[Header("상호작용 관련")]
	public NKCUIComOfficeInteriorDetail m_comInteriorDetail;

	public NKCUIComOfficeInteriorInteractionBubble m_comInteriorInteractionBubble;

	private int m_SelectedFunitureID = -1;

	private int m_SelectedThemeID = -1;

	private List<NKMOfficeInteriorTemplet> m_lstFuniture = new List<NKMOfficeInteriorTemplet>();

	private List<NKMOfficeInteriorTemplet> m_lstDecoration = new List<NKMOfficeInteriorTemplet>();

	private OnSelectInterior dOnSelectInterior;

	private OnSelectPreset dOnSelectPreset;

	private Category m_eCurrentCategory = Category.FURNITURE;

	private NKCMiscSortSystem m_ssActive;

	private List<NKMOfficeThemePresetTemplet> m_lstThemePreset;

	private Dictionary<int, int> m_dicFurnitureListView;

	private Mode m_eMode;

	private List<NKCUISlot> m_lstVisibleSlot = new List<NKCUISlot>();

	public static NKCUIPopupOfficeInteriorSelect Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupOfficeInteriorSelect>("ab_ui_office", "AB_UI_POPUP_OFFICE_FNC_LIST", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupOfficeInteriorSelect>();
				m_Instance.InitUI();
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

	public override string MenuName => "창고";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		if (m_eMode == Mode.Listview)
		{
			m_ssActive = null;
		}
		m_lstThemePreset = null;
		m_dicFurnitureListView = null;
		m_comFurniturePreview.Clear();
		base.gameObject.SetActive(value: false);
	}

	public override void OnCloseInstance()
	{
		if (m_lstVisibleSlot != null)
		{
			for (int i = 0; i < m_lstVisibleSlot.Count; i++)
			{
				Object.Destroy(m_lstVisibleSlot[i].gameObject);
			}
			m_lstVisibleSlot.Clear();
		}
	}

	private void InitUI()
	{
		NKCUtil.SetToggleValueChangedDelegate(m_tglTabFuniture, OnTglTabFuniture);
		NKCUtil.SetToggleValueChangedDelegate(m_tglTabBackground, OnTglTabBackground);
		NKCUtil.SetToggleValueChangedDelegate(m_tglTabThemePreset, OnTglTabThemePreset);
		NKCUtil.SetButtonClickDelegate(m_csbtnInstall, OnInstallFuniture);
		NKCUtil.SetHotkey(m_csbtnInstall, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		m_srFuniture.dOnGetObject += GetObject;
		m_srFuniture.dOnReturnObject += ReturnObject;
		m_srFuniture.dOnProvideData += ProvideData;
		m_srFuniture.SetAutoResize(3);
		m_srFuniture.PrepareCells();
		if (m_sortOptions != null)
		{
			m_sortOptions.Init(OnSorted);
		}
		NKCUtil.SetScrollHotKey(m_srFuniture);
	}

	public void Open(OnSelectInterior onSelectInterior, OnSelectPreset onSelectPreset)
	{
		NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_PF_OFFICE_ROOM_UI_FRONT_WAREHOUSE"));
		NKCUtil.SetLabelText(m_lbNoFurnitureSelect, NKCStringTable.GetString("SI_PF_POPUP_OFFICE_FNC_LIST_SELECT_NONE"));
		NKCUtil.SetLabelText(m_lbNoFuniture, NKCStringTable.GetString("SI_PF_POPUP_OFFICE_FNC_LIST_NONE"));
		dOnSelectInterior = onSelectInterior;
		dOnSelectPreset = onSelectPreset;
		m_eMode = Mode.FurnitureEdit;
		BuildFunitureList();
		NKCUtil.SetGameobjectActive(m_tglTabThemePreset, bValue: true);
		NKCUtil.SetGameobjectActive(m_csbtnInstall, bValue: true);
		UIOpened();
		SelectTab(m_eCurrentCategory);
	}

	public void OpenForListView(Dictionary<int, int> dicFurniture)
	{
		NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_PF_SHOP_BANNER_INTERIOR_VIEW_TEXT"));
		NKCUtil.SetLabelText(m_lbNoFurnitureSelect, NKCStringTable.GetString("SI_PF_SHOP_INTERIOR_02"));
		NKCUtil.SetLabelText(m_lbNoFuniture, NKCStringTable.GetString("SI_PF_SHOP_INTERIOR_01"));
		m_dicFurnitureListView = dicFurniture;
		m_eMode = Mode.Listview;
		m_ssActive = null;
		NKCUtil.SetGameobjectActive(m_tglTabThemePreset, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnInstall, bValue: false);
		BuildFunitureList();
		UIOpened();
		SelectTab(Category.FURNITURE);
	}

	private void BuildFunitureList()
	{
		m_lstDecoration.Clear();
		m_lstFuniture.Clear();
		switch (m_eMode)
		{
		case Mode.FurnitureEdit:
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			{
				foreach (NKMInteriorData allInteriorDatum in nKMUserData.OfficeData.GetAllInteriorData())
				{
					NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet2 = NKMOfficeInteriorTemplet.Find(allInteriorDatum.itemId);
					if (nKMUserData.OfficeData.GetInteriorCount(allInteriorDatum.itemId) != 0L && nKMOfficeInteriorTemplet2 != null)
					{
						switch (nKMOfficeInteriorTemplet2.InteriorCategory)
						{
						case InteriorCategory.DECO:
							m_lstDecoration.Add(nKMOfficeInteriorTemplet2);
							break;
						case InteriorCategory.FURNITURE:
							m_lstFuniture.Add(nKMOfficeInteriorTemplet2);
							break;
						}
					}
				}
				break;
			}
		}
		case Mode.Listview:
		{
			foreach (KeyValuePair<int, int> item in m_dicFurnitureListView)
			{
				NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(item.Key);
				if (item.Value != 0 && nKMOfficeInteriorTemplet != null)
				{
					switch (nKMOfficeInteriorTemplet.InteriorCategory)
					{
					case InteriorCategory.DECO:
						m_lstDecoration.Add(nKMOfficeInteriorTemplet);
						break;
					case InteriorCategory.FURNITURE:
						m_lstFuniture.Add(nKMOfficeInteriorTemplet);
						break;
					}
				}
			}
			break;
		}
		}
	}

	private void SelectTab(Category category)
	{
		m_eCurrentCategory = category;
		switch (category)
		{
		case Category.THEME:
			m_ssActive = null;
			NKCUtil.SetGameobjectActive(m_sortOptions, bValue: false);
			m_tglTabThemePreset.Select(bSelect: true, bForce: true);
			if (m_lstThemePreset == null)
			{
				MakeThemePresetList();
			}
			m_srFuniture.TotalCount = m_lstThemePreset.Count;
			m_srFuniture.SetIndexPosition(0);
			NKCUtil.SetGameobjectActive(m_objNoFuniture, m_lstThemePreset.Count == 0);
			OnSelectTheme(-1, bForce: true);
			return;
		case Category.FURNITURE:
			m_tglTabFuniture.Select(bSelect: true, bForce: true);
			m_ssActive = new NKCMiscSortSystem(NKCScenManager.CurrentUserData(), m_lstFuniture, MakeSortOption());
			break;
		case Category.DECO:
			m_tglTabBackground.Select(bSelect: true, bForce: true);
			m_ssActive = new NKCMiscSortSystem(NKCScenManager.CurrentUserData(), m_lstDecoration, MakeSortOption());
			break;
		}
		NKCUtil.SetGameobjectActive(m_sortOptions, bValue: true);
		m_sortOptions.RegisterCategories(NKCMiscSortSystem.GetDefaultInteriorFilterCategory(), NKCMiscSortSystem.GetDefaultInteriorSortCategory());
		m_sortOptions.RegisterMiscSort(m_ssActive);
		m_sortOptions.ResetUI();
		m_ssActive.FilterList(m_ssActive.FilterSet);
		OnSelectFuniture(-1, bForce: true);
		OnSorted(bResetScroll: true);
	}

	private NKCMiscSortSystem.MiscListOptions MakeSortOption()
	{
		if (m_ssActive != null)
		{
			return m_ssActive.Options;
		}
		return new NKCMiscSortSystem.MiscListOptions
		{
			lstSortOption = NKCMiscSortSystem.GetDefaultInteriorSortList(),
			setFilterOption = new HashSet<NKCMiscSortSystem.eFilterOption>()
		};
	}

	private void OnTglTabFuniture(bool value)
	{
		if (value)
		{
			SelectTab(Category.FURNITURE);
		}
	}

	private void OnTglTabBackground(bool value)
	{
		if (value)
		{
			SelectTab(Category.DECO);
		}
	}

	private void OnTglTabThemePreset(bool value)
	{
		if (value)
		{
			SelectTab(Category.THEME);
		}
	}

	private void OnInstallFuniture()
	{
		if (m_eCurrentCategory == Category.THEME)
		{
			Close();
			dOnSelectPreset?.Invoke(m_SelectedThemeID);
		}
		else if (NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(m_SelectedFunitureID) <= 0)
		{
			NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_OFFICE_FURNITURE_NOT_REMAINS);
		}
		else
		{
			Close();
			dOnSelectInterior?.Invoke(m_SelectedFunitureID);
		}
	}

	private RectTransform GetObject(int idx)
	{
		NKCUISlot nKCUISlot = Object.Instantiate(m_pfbSlot);
		nKCUISlot.Init();
		RectTransform component = nKCUISlot.GetComponent<RectTransform>();
		if (component == null)
		{
			Object.Destroy(nKCUISlot.gameObject);
		}
		m_lstVisibleSlot.Add(nKCUISlot);
		return component;
	}

	private void ReturnObject(Transform tr)
	{
		tr.SetParent(base.transform);
		tr.gameObject.SetActive(value: false);
		NKCUISlot component = tr.GetComponent<NKCUISlot>();
		if (component != null)
		{
			m_lstVisibleSlot.Remove(component);
		}
		Object.Destroy(tr.gameObject);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUISlot component = tr.GetComponent<NKCUISlot>();
		if (component == null)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		if (m_eCurrentCategory == Category.THEME)
		{
			if (idx < 0 || idx >= m_lstThemePreset.Count)
			{
				NKCUtil.SetGameobjectActive(tr, bValue: false);
				return;
			}
			NKMOfficeThemePresetTemplet nKMOfficeThemePresetTemplet = m_lstThemePreset[idx];
			NKCUISlot.SlotData slotData = new NKCUISlot.SlotData();
			slotData.eType = NKCUISlot.eSlotMode.Etc;
			slotData.ID = nKMOfficeThemePresetTemplet.Key;
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("AB_INVEN_ICON_FNC_THEME", nKMOfficeThemePresetTemplet.ThemaPresetIMG));
			component.SetEtcData(slotData, orLoadAssetResource, "", "", OnSelectSlot);
			component.SetSelected(m_SelectedThemeID == nKMOfficeThemePresetTemplet.Key);
			return;
		}
		if (idx < 0 || idx >= m_ssActive.SortedMiscList.Count)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = m_ssActive.SortedMiscList[idx] as NKMOfficeInteriorTemplet;
		switch (m_eMode)
		{
		case Mode.FurnitureEdit:
		{
			long freeInteriorCount = NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(nKMOfficeInteriorTemplet.m_ItemMiscID);
			NKCUISlot.SlotData data2 = NKCUISlot.SlotData.MakeMiscItemData(nKMOfficeInteriorTemplet.m_ItemMiscID, freeInteriorCount);
			switch (m_eCurrentCategory)
			{
			case Category.FURNITURE:
			{
				long interiorCount = NKCScenManager.CurrentUserData().OfficeData.GetInteriorCount(nKMOfficeInteriorTemplet.m_ItemMiscID);
				component.SetData(data2, bShowName: false, bShowNumber: true, bEnableLayoutElement: true, OnSelectSlot);
				if (freeInteriorCount == 0L)
				{
					component.SetSlotItemCountString(bShow: true, $"<color=#ff0000>{freeInteriorCount}</color>/{interiorCount}");
				}
				else
				{
					component.SetSlotItemCountString(bShow: true, $"{freeInteriorCount}/{interiorCount}");
				}
				break;
			}
			case Category.DECO:
				component.SetData(data2, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, OnSelectSlot);
				break;
			}
			break;
		}
		case Mode.Listview:
		{
			if (!m_dicFurnitureListView.TryGetValue(nKMOfficeInteriorTemplet.m_ItemMiscID, out var value))
			{
				value = 0;
			}
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(nKMOfficeInteriorTemplet.m_ItemMiscID, value);
			component.SetData(data, bShowName: false, m_eCurrentCategory == Category.FURNITURE, bEnableLayoutElement: true, OnSelectSlot);
			break;
		}
		}
		component.SetSelected(m_SelectedFunitureID == nKMOfficeInteriorTemplet.Id);
		if (!NKCDefineManager.DEFINE_SERVICE() && NKMItemMiscTemplet.Find(nKMOfficeInteriorTemplet.m_ItemMiscID) == null)
		{
			component.OverrideName(nKMOfficeInteriorTemplet.PrefabName, supportRichText: false);
		}
	}

	private void OnSelectSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (!bLocked)
		{
			if (m_eCurrentCategory == Category.THEME)
			{
				OnSelectTheme(slotData.ID);
			}
			else
			{
				OnSelectFuniture(slotData.ID);
			}
		}
	}

	private void OnSorted(bool bResetScroll)
	{
		m_srFuniture.TotalCount = m_ssActive.SortedMiscList.Count;
		if (bResetScroll)
		{
			m_srFuniture.SetIndexPosition(0);
		}
		else
		{
			m_srFuniture.RefreshCells();
		}
		NKCUtil.SetGameobjectActive(m_objNoFuniture, m_ssActive.SortedMiscList.Count == 0);
	}

	private void OnSelectFuniture(int id, bool bForce = false)
	{
		if (!bForce && m_SelectedFunitureID == id)
		{
			return;
		}
		m_SelectedFunitureID = id;
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMItemMiscTemplet.FindInterior(id);
		NKCUtil.SetGameobjectActive(m_objNoFunitureSelect, nKMOfficeInteriorTemplet == null);
		NKCUtil.SetGameobjectActive(m_comFurniturePreview, nKMOfficeInteriorTemplet != null);
		if (m_ssActive != null && m_srFuniture != null)
		{
			foreach (Transform item in m_srFuniture.content)
			{
				item.GetComponent<NKCUISlot>().SetSelected(bSelected: false);
			}
			int index = m_ssActive.SortedMiscList.FindIndex((NKMItemMiscTemplet x) => x.m_ItemMiscID == id);
			Transform child = m_srFuniture.GetChild(index);
			if (child != null)
			{
				NKCUISlot component = child.GetComponent<NKCUISlot>();
				if (component != null)
				{
					component.SetSelected(bSelected: true);
				}
			}
		}
		if (m_comInteriorDetail != null)
		{
			m_comInteriorDetail.SetData(nKMOfficeInteriorTemplet);
		}
		if (m_comInteriorInteractionBubble != null)
		{
			m_comInteriorInteractionBubble.SetData(nKMOfficeInteriorTemplet);
		}
		if (nKMOfficeInteriorTemplet != null)
		{
			m_comFurniturePreview?.SetData(nKMOfficeInteriorTemplet);
		}
		m_csbtnInstall.SetLock(nKMOfficeInteriorTemplet == null);
	}

	internal RectTransform GetTutorialItemSlot(int itemID)
	{
		int num = m_ssActive.SortedMiscList.FindIndex((NKMItemMiscTemplet x) => x.m_ItemMiscID == itemID);
		if (num < 0)
		{
			return null;
		}
		m_srFuniture.SetIndexPosition(num);
		NKCUISlot nKCUISlot = m_lstVisibleSlot.Find((NKCUISlot x) => x.GetSlotData().ID == itemID);
		if (nKCUISlot == null)
		{
			return null;
		}
		return nKCUISlot.GetComponent<RectTransform>();
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (NKMOfficeInteriorTemplet.Find(itemData.ItemID) != null)
		{
			BuildFunitureList();
			SelectTab(m_eCurrentCategory);
		}
	}

	public override void OnInteriorInventoryUpdate(NKMInteriorData interiorData, bool bAdded)
	{
		if (bAdded)
		{
			BuildFunitureList();
			SelectTab(m_eCurrentCategory);
		}
		else
		{
			m_srFuniture.RefreshCells();
		}
	}

	private void MakeThemePresetList()
	{
		m_lstThemePreset = new List<NKMOfficeThemePresetTemplet>();
		foreach (NKMOfficeThemePresetTemplet value in NKMTempletContainer<NKMOfficeThemePresetTemplet>.Values)
		{
			if (!value.EnableByTag)
			{
				continue;
			}
			if (value.AlwaysAppearOnList)
			{
				m_lstThemePreset.Add(value);
				continue;
			}
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			foreach (NKMOfficeInteriorTemplet allInterior in value.AllInteriors)
			{
				if (nKMUserData.OfficeData.GetInteriorCount(allInterior) > 0)
				{
					m_lstThemePreset.Add(value);
					break;
				}
			}
		}
	}

	private void OnSelectTheme(int id, bool bForce = false)
	{
		m_SelectedThemeID = id;
		NKMOfficeThemePresetTemplet nKMOfficeThemePresetTemplet = NKMOfficeThemePresetTemplet.Find(id);
		NKCUtil.SetGameobjectActive(m_objNoFunitureSelect, nKMOfficeThemePresetTemplet == null);
		NKCUtil.SetGameobjectActive(m_comFurniturePreview, nKMOfficeThemePresetTemplet != null);
		NKCUtil.SetGameobjectActive(m_comInteriorDetail, bValue: false);
		NKCUtil.SetGameobjectActive(m_comInteriorInteractionBubble, bValue: false);
		if (m_comFurniturePreview != null)
		{
			m_comFurniturePreview.SetData(nKMOfficeThemePresetTemplet);
		}
		if (m_lstThemePreset != null && m_srFuniture != null)
		{
			foreach (Transform item in m_srFuniture.content)
			{
				item.GetComponent<NKCUISlot>().SetSelected(bSelected: false);
			}
			int index = m_lstThemePreset.FindIndex((NKMOfficeThemePresetTemplet x) => x.Key == id);
			Transform child = m_srFuniture.GetChild(index);
			if (child != null)
			{
				NKCUISlot component = child.GetComponent<NKCUISlot>();
				if (component != null)
				{
					component.SetSelected(bSelected: true);
				}
			}
		}
		m_csbtnInstall.SetLock(nKMOfficeThemePresetTemplet == null);
	}
}
