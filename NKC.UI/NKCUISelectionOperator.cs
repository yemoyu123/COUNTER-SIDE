using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISelectionOperator : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_UNIT_SELECTION";

	private const string UI_ASSET_NAME = "NKM_UI_OPERATOR_SELECTION";

	private static NKCUISelectionOperator m_Instance;

	public NKCUIComSafeArea m_SafeArea;

	[Header("오퍼레이터")]
	public LoopScrollRect m_loopScrollRect;

	public Transform m_trContentParent;

	public Image m_imgBanner;

	public Text m_lbDesc;

	[Header("프리팹")]
	public NKCUIOperatorSelectListSlot m_pfbSlot;

	[Header("필터/정렬 통합ui")]
	public NKCUIComUnitSortOptions m_SortUI;

	private List<int> m_lstRewardId = new List<int>();

	private List<NKCUIOperatorSelectListSlot> m_lstVisibleSlot = new List<NKCUIOperatorSelectListSlot>();

	private Stack<NKCUIOperatorSelectListSlot> m_stkSlotPool = new Stack<NKCUIOperatorSelectListSlot>();

	private NKMItemMiscTemplet m_NKMItemMiscTemplet;

	private NKCOperatorSortSystem m_ssActive;

	private readonly HashSet<NKCOperatorSortSystem.eSortCategory> m_setUnitSortCategory = new HashSet<NKCOperatorSortSystem.eSortCategory>
	{
		NKCOperatorSortSystem.eSortCategory.ID,
		NKCOperatorSortSystem.eSortCategory.Rarity
	};

	public static NKCUISelectionOperator Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUISelectionOperator>("AB_UI_NKM_UI_UNIT_SELECTION", "NKM_UI_OPERATOR_SELECTION", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUISelectionOperator>();
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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_USE_CHOICE;

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

	private void InitUI()
	{
		m_loopScrollRect.dOnGetObject += GetObject;
		m_loopScrollRect.dOnReturnObject += ReturnObject;
		m_loopScrollRect.dOnProvideData += ProvideData;
		m_loopScrollRect.dOnRepopulate += CalculateContentRectSize;
		NKCUtil.SetScrollHotKey(m_loopScrollRect);
		if (m_SortUI != null)
		{
			m_SortUI.Init(OnSortChanged, bIsCollection: false);
			if (m_SortUI.m_NKCPopupSort != null)
			{
				m_SortUI.m_NKCPopupSort.m_bUseDefaultSortAdd = false;
			}
		}
	}

	public override void CloseInternal()
	{
		m_SortUI.ResetUI();
		m_lstRewardId = new List<int>();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnCloseInstance()
	{
		m_ssActive = null;
		m_NKMItemMiscTemplet = null;
	}

	public void Open(NKMItemMiscTemplet itemMiscTemplet)
	{
		if (itemMiscTemplet == null)
		{
			return;
		}
		m_NKMItemMiscTemplet = itemMiscTemplet;
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(m_NKMItemMiscTemplet.m_RewardGroupID);
		if (randomBoxItemTempletList == null)
		{
			return;
		}
		for (int i = 0; i < randomBoxItemTempletList.Count; i++)
		{
			m_lstRewardId.Add(randomBoxItemTempletList[i].m_RewardID);
		}
		NKCScenManager.CurrentUserData();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		CalculateContentRectSize();
		SetChoiceList();
		NKCUtil.SetImageSprite(m_imgBanner, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_UNIT_SELECTION_TEXTURE", itemMiscTemplet.m_BannerImage));
		string msg = string.Empty;
		if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
		{
			NKMCustomBoxTemplet nKMCustomBoxTemplet = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
			if (nKMCustomBoxTemplet != null)
			{
				msg = string.Format(NKCUtilString.GET_STRING_OPERATOR_SELECTION_DESC_LEVEL, nKMCustomBoxTemplet.Level);
			}
		}
		else
		{
			msg = NKCUtilString.GET_STRING_OPERATOR_SELECTION_DESC;
		}
		NKCUtil.SetLabelText(m_lbDesc, msg);
		UIOpened();
	}

	private void CalculateContentRectSize()
	{
		m_SafeArea?.SetSafeAreaBase();
		int minColumn = 4;
		Vector2 cellSize = m_trContentParent.GetComponent<GridLayoutGroup>().cellSize;
		Vector2 spacing = m_trContentParent.GetComponent<GridLayoutGroup>().spacing;
		NKCUtil.CalculateContentRectSize(m_loopScrollRect, m_trContentParent.GetComponent<GridLayoutGroup>(), minColumn, cellSize, spacing);
	}

	private int CompOrderList(NKMRandomBoxItemTemplet lItem, NKMRandomBoxItemTemplet rItem)
	{
		if (lItem.m_OrderList == rItem.m_OrderList)
		{
			return lItem.m_RewardID.CompareTo(rItem.m_RewardID);
		}
		return lItem.m_OrderList.CompareTo(rItem.m_OrderList);
	}

	private RectTransform GetObject(int index)
	{
		NKCUIOperatorSelectListSlot nKCUIOperatorSelectListSlot = null;
		if (m_stkSlotPool.Count > 0)
		{
			nKCUIOperatorSelectListSlot = m_stkSlotPool.Pop();
		}
		else
		{
			nKCUIOperatorSelectListSlot = Object.Instantiate(m_pfbSlot);
			nKCUIOperatorSelectListSlot.Init();
		}
		NKCUtil.SetGameobjectActive(nKCUIOperatorSelectListSlot, bValue: true);
		m_lstVisibleSlot.Add(nKCUIOperatorSelectListSlot);
		return nKCUIOperatorSelectListSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform go)
	{
		NKCUIOperatorSelectListSlot component = go.GetComponent<NKCUIOperatorSelectListSlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		go.SetParent(base.transform);
		if (component != null)
		{
			m_lstVisibleSlot.Remove(component);
			m_stkSlotPool.Push(component);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		if (idx < 0 || idx >= m_lstRewardId.Count)
		{
			Debug.LogError("out of index");
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		NKCUIOperatorSelectListSlot component = tr.GetComponent<NKCUIOperatorSelectListSlot>();
		int levelToDisplay = 1;
		if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
		{
			NKMCustomBoxTemplet nKMCustomBoxTemplet = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
			if (nKMCustomBoxTemplet != null && nKMCustomBoxTemplet.Level > 0)
			{
				levelToDisplay = nKMCustomBoxTemplet.Level;
			}
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_ssActive.SortedOperatorList[idx]);
		int operatorCountByID = NKCScenManager.CurrentUserData().m_ArmyData.GetOperatorCountByID(m_ssActive.SortedOperatorList[idx].id);
		component.SetData(unitTempletBase, levelToDisplay, 0, bEnableLayoutElement: true, OnSelectSlot);
		component.SetHaveCount(operatorCountByID, bShowBtn: false);
		NKCUtil.SetGameobjectActive(component.gameObject, bValue: true);
	}

	private void SetChoiceList()
	{
		NKCOperatorSortSystem.OperatorListOptions options = new NKCOperatorSortSystem.OperatorListOptions
		{
			eDeckType = NKM_DECK_TYPE.NDT_NORMAL,
			setFilterOption = new HashSet<NKCOperatorSortSystem.eFilterOption>(),
			lstSortOption = NKCOperatorSortSystem.GetDefaultSortOptions(bIsCollection: true, bIsSelection: true),
			lstCustomSortFunc = new Dictionary<NKCOperatorSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMOperator>.CompareFunc>>()
		};
		List<NKMOperator> list = new List<NKMOperator>();
		for (int i = 0; i < m_lstRewardId.Count; i++)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_lstRewardId[i]);
			if (unitTempletBase != null)
			{
				NKMOperator dummyOperator = NKCOperatorUtil.GetDummyOperator(unitTempletBase);
				list.Add(dummyOperator);
			}
		}
		m_ssActive = new NKCGenericOperatorSort(NKCScenManager.CurrentUserData(), options, list);
		m_SortUI.RegisterCategories(NKCOperatorSortSystem.MakeDefaultFilterCategory(NKCOperatorSortSystem.FILTER_OPEN_TYPE.SELECTION), m_setUnitSortCategory, bFavoriteFilterActive: false);
		m_SortUI.RegisterOperatorSort(m_ssActive);
		m_SortUI.ResetUI();
		m_loopScrollRect.PrepareCells();
		m_loopScrollRect.TotalCount = m_ssActive.SortedOperatorList.Count;
		m_loopScrollRect.RefreshCells(bForce: true);
	}

	public void OnSelectSlot(NKMOperator operatorData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		if (unitTempletBase == null)
		{
			return;
		}
		bool flag = false;
		if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
		{
			NKMCustomBoxTemplet nKMCustomBoxTemplet = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
			if (nKMCustomBoxTemplet != null && nKMCustomBoxTemplet.CustomOperatorSkillIds.Count > 0)
			{
				flag = true;
			}
		}
		if (flag)
		{
			NKCPopupSelectionConfirmOperatorSkill.Instance.Open(m_NKMItemMiscTemplet, unitTempletBase.m_UnitID, OnSelectSubSkill);
		}
		else
		{
			NKCPopupSelectionConfirm.Instance.Open(m_NKMItemMiscTemplet, unitTempletBase.m_UnitID, 1L);
		}
	}

	private void OnSelectSubSkill(int operUnitID, int subSkillID)
	{
		NKCPopupSelectionConfirm.Instance.Open(m_NKMItemMiscTemplet, operUnitID, 0L, 0, subSkillID);
	}

	private void OnSortChanged(bool bResetScroll = false)
	{
		m_loopScrollRect.TotalCount = m_ssActive.SortedOperatorList.Count;
		if (bResetScroll)
		{
			m_loopScrollRect.SetIndexPosition(0);
		}
		else
		{
			m_loopScrollRect.RefreshCells();
		}
	}
}
