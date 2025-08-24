using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeCraftMold : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_factory";

	private const string UI_ASSET_NAME = "NKM_UI_FACTORY_CRAFT_MOLD";

	private static NKCUIForgeCraftMold m_Instance;

	private GameObject m_NUM_FORGE;

	public LoopScrollRect m_LoopScrollRect;

	public Transform m_ContentTransform;

	public GridLayoutGroup m_GridLayoutGroup;

	public NKCUIComSafeArea m_SafeArea;

	public Vector2 m_vSlotSize;

	public Vector2 m_vSlotSpacing;

	public NKCUIComToggle m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_ARRAY;

	private bool m_bFirstOpen = true;

	private int m_Index;

	public GameObject m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_TEXT;

	[Header("탭")]
	public NKCUIComToggle m_ctgl_FACTORY_CRAFT_EQUIP;

	public NKCUIComToggle m_ctgl_FACTORY_CRAFT_LIMITBREAK;

	private NKM_CRAFT_TAB_TYPE m_eTapType;

	public RectTransform m_rtNKM_UI_FACTORY_CRAFT_LEFT;

	private NKCUIComToggleGroup m_ctglGroup_FactoryCraftLeft;

	private const float m_fCrateTabOffsetY = -126f;

	private List<NKCRandomMoldTabTemplet> m_lstMoldTab = new List<NKCRandomMoldTabTemplet>();

	private string m_defaultSortKey;

	private Dictionary<NKM_CRAFT_TAB_TYPE, NKCUIComToggle> m_dicTabToggle = new Dictionary<NKM_CRAFT_TAB_TYPE, NKCUIComToggle>();

	[Header("필터")]
	public NKCUIComToggle m_tgl_NKM_UI_FACTORY_CRAFT_MENU_VERTICAL;

	public NKCUIComStateButton m_sbtn_NKM_UI_FACTORY_CRAFT_MENU_FILTER;

	public GameObject m_objFilterSelected;

	[Header("정렬")]
	public NKCPopupMoldSort m_NKCMoldPopupSort;

	public NKCUIComToggle m_cbtnSortTypeMenu;

	public GameObject m_objSortSelected;

	public Text m_lbSortType;

	public Text m_lbSelectedSortType;

	private NKCMoldSortSystem m_MoldSortSystem;

	public GameObject m_objNoneText;

	public static NKCUIForgeCraftMold Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIForgeCraftMold>("ab_ui_nkm_ui_factory", "NKM_UI_FACTORY_CRAFT_MOLD", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIForgeCraftMold>();
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

	public override string MenuName => NKCUtilString.GET_STRING_FORGE_CRAFT_MOLD;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override List<int> UpsideMenuShowResourceList => new List<int> { 1, 2, 101 };

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
		base.gameObject.SetActive(value: false);
		InitLoopScrollRect();
		InitFunctionButton();
	}

	private void InitLoopScrollRect()
	{
		m_LoopScrollRect.dOnGetObject += GetSlot;
		m_LoopScrollRect.dOnReturnObject += ReturnSlot;
		m_LoopScrollRect.dOnProvideData += ProvideData;
		m_LoopScrollRect.dOnRepopulate += CalculateContentRectSize;
		NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		if (m_SafeArea != null)
		{
			m_SafeArea.SetSafeAreaBase();
		}
		CalculateContentRectSize();
	}

	private void CalculateContentRectSize()
	{
		int minColumn = 2;
		Vector2 vSlotSize = m_vSlotSize;
		Vector2 vSlotSpacing = m_vSlotSpacing;
		NKCUtil.CalculateContentRectSize(m_LoopScrollRect, m_GridLayoutGroup, minColumn, vSlotSize, vSlotSpacing);
	}

	public RectTransform GetSlot(int index)
	{
		NKCUIForgeCraftMoldSlot newInstance = NKCUIForgeCraftMoldSlot.GetNewInstance(m_ContentTransform, OnClickMoldSlot);
		if (newInstance != null)
		{
			return newInstance.GetComponent<RectTransform>();
		}
		return null;
	}

	public void ReturnSlot(Transform tr)
	{
		NKCUIForgeCraftMoldSlot component = tr.GetComponent<NKCUIForgeCraftMoldSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideData(Transform tr, int index)
	{
		NKCUIForgeCraftMoldSlot component = tr.GetComponent<NKCUIForgeCraftMoldSlot>();
		if (component != null)
		{
			component.SetData(index);
		}
	}

	private void OnClickMoldSlot(NKMMoldItemData cNKMMoldItemData)
	{
		NKCPopupForgeCraft.Instance.Open(cNKMMoldItemData);
	}

	private void ProcessUIFromCurrentDisplayedSortData()
	{
		if (m_MoldSortSystem != null)
		{
			NKCUtil.SetLabelText(m_lbSortType, m_MoldSortSystem.GetSortName());
			NKCUtil.SetLabelText(m_lbSelectedSortType, m_MoldSortSystem.GetSortName());
		}
	}

	public NKMMoldItemData GetSortedMoldItemData(int index)
	{
		if ((m_MoldSortSystem != null && m_MoldSortSystem.lstSortedList != null && m_MoldSortSystem.lstSortedList.Count > index) || index >= 0)
		{
			return m_MoldSortSystem.lstSortedList[index];
		}
		return null;
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (IsInstanceOpen && m_MoldSortSystem != null)
		{
			m_MoldSortSystem.Update(m_eTapType);
			UpdateLoopScroll();
		}
	}

	public override void OnCompanyBuffUpdate(NKMUserData userData)
	{
		if (IsInstanceOpen && m_MoldSortSystem != null)
		{
			m_MoldSortSystem.Update(m_eTapType);
			UpdateLoopScroll();
		}
	}

	public void Open()
	{
		if (m_bFirstOpen)
		{
			m_bFirstOpen = false;
			m_LoopScrollRect.PrepareCells();
			PrepareUI();
		}
		if (m_MoldSortSystem == null)
		{
			m_MoldSortSystem = new NKCMoldSortSystem(NKCMoldSortSystem.GetDefaultSortOption(m_defaultSortKey));
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objSortSelected, bValue: false);
		OnSelectTab(m_eTapType);
		GameObject gameObject = NKCUIManager.OpenUI("NUM_FACTORY_BG");
		if (gameObject != null)
		{
			NKCCamera.RescaleRectToCameraFrustrum(gameObject.GetComponent<RectTransform>(), NKCCamera.GetCamera(), new Vector2(200f, 200f), -1000f);
		}
		NKCUtil.SetGameobjectActive(m_objFilterSelected, m_MoldSortSystem.FilterSet.Count > 0);
		UIOpened();
	}

	private void PrepareUI()
	{
		GameObject orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<GameObject>("ab_ui_nkm_ui_factory", "NKM_UI_FACTORY_CRAFT_TAB");
		m_lstMoldTab.Clear();
		m_lstMoldTab = NKMItemManager.MoldTapTemplet.Values.ToList();
		if (m_lstMoldTab.Count <= 0 || !(orLoadAssetResource != null))
		{
			return;
		}
		m_lstMoldTab.Sort((NKCRandomMoldTabTemplet x, NKCRandomMoldTabTemplet y) => x.m_TabOrder.CompareTo(y.m_TabOrder));
		m_eTapType = m_lstMoldTab[0].m_MoldTabID;
		m_defaultSortKey = m_lstMoldTab[0].m_MoldTab_Sort[0];
		for (int num = 0; num < m_lstMoldTab.Count; num++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(orLoadAssetResource);
			if (NKCUtil.IsNullObject(gameObject, "PrepareUI - 탭 오브젝트를 생성할 수 없습니다."))
			{
				break;
			}
			gameObject.transform.SetParent(m_rtNKM_UI_FACTORY_CRAFT_LEFT, worldPositionStays: false);
			gameObject.GetComponent<RectTransform>();
			NKCUIComToggle component = gameObject.GetComponent<NKCUIComToggle>();
			if (!m_lstMoldTab[num].EnableByTag)
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
			if (component != null && m_ctglGroup_FactoryCraftLeft != null)
			{
				component.SetToggleGroup(m_ctglGroup_FactoryCraftLeft);
				component.OnValueChanged.RemoveAllListeners();
				NKM_CRAFT_TAB_TYPE TabType = m_lstMoldTab[num].m_MoldTabID;
				component.OnValueChanged.AddListener(delegate(bool val)
				{
					if (val)
					{
						OnSelectTab(TabType);
					}
				});
				if (!m_dicTabToggle.ContainsKey(TabType))
				{
					m_dicTabToggle.Add(TabType, component);
				}
				component.Select(m_lstMoldTab[num].m_MoldTabID == m_eTapType, bForce: true);
			}
			SetCraftTabIcon(gameObject, "NKM_UI_CRAFT_MENU_BUTTON_ON/NKM_UI_CRAFT_MENU_ICON", m_lstMoldTab[num].m_MoldTabIconName);
			SetCraftTabIcon(gameObject, "NKM_UI_CRAFT_MENU_BUTTON_OFF/NKM_UI_CRAFT_MENU_ICON", m_lstMoldTab[num].m_MoldTabIconName);
			SetCraftTabName(gameObject, "NKM_UI_CRAFT_MENU_BUTTON_ON/NKM_UI_CRAFT_MENU_TEXT", m_lstMoldTab[num].m_MoldTabName);
			SetCraftTabName(gameObject, "NKM_UI_CRAFT_MENU_BUTTON_OFF/NKM_UI_CRAFT_MENU_TEXT", m_lstMoldTab[num].m_MoldTabName);
		}
	}

	private bool SetCraftTabIcon(GameObject rootObj, string targetPath, string iconName)
	{
		if (rootObj == null)
		{
			return false;
		}
		GameObject gameObject = rootObj.transform.Find(targetPath).gameObject;
		if (gameObject == null)
		{
			return false;
		}
		Image component = gameObject.GetComponent<Image>();
		if (component == null)
		{
			return false;
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_factory_sprite", iconName);
		if (orLoadAssetResource == null)
		{
			return false;
		}
		NKCUtil.SetImageSprite(component, orLoadAssetResource);
		return true;
	}

	private bool SetCraftTabName(GameObject rootObj, string targetPath, string title)
	{
		if (rootObj == null)
		{
			return false;
		}
		GameObject gameObject = rootObj.transform.Find(targetPath).gameObject;
		if (gameObject == null)
		{
			return false;
		}
		Text component = gameObject.GetComponent<Text>();
		if (component == null)
		{
			return false;
		}
		NKCUtil.SetLabelText(component, NKCStringTable.GetString(title));
		return true;
	}

	public void SelectCraftTab(NKM_CRAFT_TAB_TYPE tabType)
	{
		if (m_dicTabToggle.ContainsKey(tabType))
		{
			m_dicTabToggle[tabType].Select(bSelect: true);
		}
	}

	public override void CloseInternal()
	{
		NKCPopupForgeCraft.CheckInstanceAndClose();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnCloseInstance()
	{
		m_LoopScrollRect.ClearCells();
		m_eTapType = NKM_CRAFT_TAB_TYPE.MT_EQUIP;
		m_dicTabToggle.Clear();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void UnHide()
	{
		base.UnHide();
	}

	private void OnSelectTab(NKM_CRAFT_TAB_TYPE newStatus)
	{
		bool flag = false;
		foreach (NKM_CRAFT_TAB_TYPE value in Enum.GetValues(typeof(NKM_CRAFT_TAB_TYPE)))
		{
			if (value == newStatus)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			Debug.LogError($"허용되지 않는 enum 값, 추가 설정이 필요 : {newStatus}");
			return;
		}
		if (m_lstMoldTab.Count <= 0)
		{
			Debug.LogError("m_lstMoldTab 데이터를 확인 할 수 없습니다.");
			return;
		}
		if (m_eTapType != newStatus)
		{
			foreach (NKCRandomMoldTabTemplet item in m_lstMoldTab)
			{
				if (item.m_MoldTabID != newStatus)
				{
					continue;
				}
				if (item.m_MoldTab_Sort.Count <= 0)
				{
					Debug.LogError("m_MoldTab_Sort 설정을 확인 해주세요");
					return;
				}
				for (int i = 0; i < item.m_MoldTab_Sort.Count; i++)
				{
					if (i == 0)
					{
						m_MoldSortSystem = new NKCMoldSortSystem(NKCMoldSortSystem.GetDefaultSortOption(item.m_MoldTab_Sort[i]));
					}
				}
				break;
			}
			SetOpenSortingMenu(bOpen: false);
			NKCUtil.SetGameobjectActive(m_objSortSelected, bValue: false);
		}
		m_eTapType = newStatus;
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_TEXT, m_eTapType == NKM_CRAFT_TAB_TYPE.MT_EQUIP);
		m_MoldSortSystem.Update(m_eTapType);
		UpdateLoopScroll();
		ProcessUIFromCurrentDisplayedSortData();
	}

	private void InitFunctionButton()
	{
		m_tgl_NKM_UI_FACTORY_CRAFT_MENU_VERTICAL.OnValueChanged.RemoveAllListeners();
		m_tgl_NKM_UI_FACTORY_CRAFT_MENU_VERTICAL.OnValueChanged.AddListener(OnCheckAscend);
		m_sbtn_NKM_UI_FACTORY_CRAFT_MENU_FILTER.PointerClick.RemoveAllListeners();
		m_sbtn_NKM_UI_FACTORY_CRAFT_MENU_FILTER.PointerClick.AddListener(OnClickFilterBtn);
		m_cbtnSortTypeMenu.OnValueChanged.RemoveAllListeners();
		m_cbtnSortTypeMenu.OnValueChanged.AddListener(OnSortMenuOpen);
		if (m_rtNKM_UI_FACTORY_CRAFT_LEFT != null)
		{
			m_ctglGroup_FactoryCraftLeft = m_rtNKM_UI_FACTORY_CRAFT_LEFT.GetComponent<NKCUIComToggleGroup>();
		}
	}

	public void OnSortMenuOpen(bool bValue)
	{
		if (m_MoldSortSystem == null || m_MoldSortSystem.lstSortOption.Count <= 1)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objSortSelected, m_MoldSortSystem.lstSortOption[1] != NKCMoldSortSystem.GetDefaultSortOption()[1]);
		foreach (NKCRandomMoldTabTemplet item in m_lstMoldTab)
		{
			if (item.m_MoldTabID == m_eTapType)
			{
				m_NKCMoldPopupSort.OpenMoldSortMenu(m_MoldSortSystem.GetCurActiveOption(), OnSort, NKCMoldSortSystem.GetDescendingBySorting(m_MoldSortSystem.lstSortOption), bValue, item.m_MoldTab_Sort);
				break;
			}
		}
		SetOpenSortingMenu(bValue);
	}

	private void OnSort(NKCMoldSortSystem.eSortOption selectedSortOption)
	{
		NKCUtil.SetGameobjectActive(m_objSortSelected, selectedSortOption != NKCMoldSortSystem.GetDefaultSortOption()[1]);
		SetOpenSortingMenu(bOpen: false);
		bool bChangedList = false;
		if (m_MoldSortSystem.GetCurActiveOption() != selectedSortOption)
		{
			m_MoldSortSystem.Sort(selectedSortOption);
			bChangedList = true;
		}
		UpdateMoldList(bChangedList);
	}

	public void OnCheckAscend(bool bValue)
	{
		m_MoldSortSystem.OnCheckAscend(bValue, UpdateLoopScroll);
	}

	public void SetOpenSortingMenu(bool bOpen, bool bAnimate = true)
	{
		m_cbtnSortTypeMenu.Select(bOpen, bForce: true);
		m_NKCMoldPopupSort.StartRectMove(bOpen, bAnimate);
	}

	public void OnClickFilterBtn()
	{
		if (m_MoldSortSystem == null)
		{
			return;
		}
		foreach (NKCRandomMoldTabTemplet item in m_lstMoldTab)
		{
			if (item.m_MoldTabID == m_eTapType)
			{
				NKCPopupFilterMold.Instance.Open(m_MoldSortSystem.FilterSet, OnSelectFilter, item.m_MoldTab_Filter);
				break;
			}
		}
	}

	public void OnSelectFilter(HashSet<NKCMoldSortSystem.eFilterOption> setFilterOption)
	{
		if (m_MoldSortSystem != null)
		{
			m_MoldSortSystem.FilterList(setFilterOption, m_eTapType);
			UpdateLoopScroll();
		}
	}

	private void UpdateLoopScroll()
	{
		if (m_MoldSortSystem == null)
		{
			return;
		}
		for (int i = 0; i < m_MoldSortSystem.lstSortedList.Count; i++)
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(m_MoldSortSystem.lstSortedList[i].m_MoldID);
			if (itemMoldTempletByID != null && itemMoldTempletByID.HasDateLimit && !itemMoldTempletByID.IntervalTemplet.IsValidTime(ServiceTime.Recent))
			{
				m_MoldSortSystem.lstSortedList.RemoveAt(i);
				i--;
			}
		}
		NKCUtil.SetGameobjectActive(m_objNoneText, m_MoldSortSystem.lstSortedList.Count <= 0);
		NKCUtil.SetGameobjectActive(m_objFilterSelected, m_MoldSortSystem.FilterSet.Count > 0);
		m_LoopScrollRect.TotalCount = m_MoldSortSystem.lstSortedList.Count;
		m_LoopScrollRect.velocity = new Vector2(0f, 0f);
		m_LoopScrollRect.SetIndexPosition(0);
		m_LoopScrollRect.RefreshCells(bForce: true);
	}

	private void UpdateMoldList(bool bChangedList)
	{
		if (bChangedList)
		{
			UpdateLoopScroll();
		}
		ProcessUIFromCurrentDisplayedSortData();
	}

	public NKCUIForgeCraftMoldSlot GetMoldSlot(int moldID)
	{
		if (m_MoldSortSystem == null)
		{
			return null;
		}
		int num = m_MoldSortSystem.lstSortedList.FindIndex((NKMMoldItemData v) => v.m_MoldID == moldID);
		if (num < 0)
		{
			return null;
		}
		m_LoopScrollRect.SetIndexPosition(num);
		NKCUIForgeCraftMoldSlot[] componentsInChildren = m_LoopScrollRect.content.GetComponentsInChildren<NKCUIForgeCraftMoldSlot>();
		for (int num2 = 0; num2 < componentsInChildren.Length; num2++)
		{
			if (componentsInChildren[num2].MoldID == moldID)
			{
				return componentsInChildren[num2];
			}
		}
		return null;
	}
}
