using System.Collections.Generic;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

[RequireComponent(typeof(RectTransform))]
public class NKCUIContractUnitSelectList : MonoBehaviour
{
	public delegate void OnClicked(int uid, NKM_UNIT_TYPE eType);

	private RectTransform m_rtRoot;

	[Header("Unit Slot")]
	public NKCDeckViewUnitSelectListSlot m_pfbUnitSlot;

	public Vector2 m_vUnitSlotSize;

	public Vector2 m_vUnitSlotSpacing;

	public NKCUIOperatorDeckSelectSlot m_pfbOperatorSlot;

	public Vector2 m_vOperatorSlotSize;

	public Vector2 m_vOperatorSlotSpacing;

	public LoopScrollRect m_LoopScrollRect;

	public GridLayoutGroup m_GridLayoutGroup;

	public RectTransform m_rectSlotPoolRect;

	private bool m_bOpen;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnConfirm;

	public NKCUIComStateButton m_csbtnBack;

	private OnClicked dOnUnitSelect;

	private OnClicked dOnConfirm;

	private List<NKCUIUnitSelectListSlotBase> m_lstVisibleSlot = new List<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkUnitSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkOperatorSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private NKCUnitSortSystem m_UnitSortSystem;

	private NKCUnitSortSystem.UnitListOptions m_sortOptions;

	private NKCOperatorSortSystem m_OperatorSortSystem;

	private NKCOperatorSortSystem.OperatorListOptions m_OperatorSortOptions;

	private UnityAction dOnClose;

	public NKCUIComUnitSortOptions m_SortUI;

	private int m_iAlreadySelectedUnitID;

	private int m_iSelectedUnitID;

	public bool IsOpen => m_bOpen;

	public NKM_UNIT_TYPE CurrentTargetUnitType { get; private set; }

	public NKCUnitSortSystem.UnitListOptions SortOptions => m_sortOptions;

	public NKCOperatorSortSystem.OperatorListOptions SortOperatorOptions => m_OperatorSortOptions;

	public void Init(OnClicked onUnitSelect, OnClicked onConfirm, UnityAction OnClose)
	{
		m_sortOptions.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		m_sortOptions.bDescending = true;
		m_sortOptions.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
		m_sortOptions.bIncludeUndeckableUnit = false;
		m_OperatorSortOptions.setFilterOption = new HashSet<NKCOperatorSortSystem.eFilterOption>();
		m_OperatorSortOptions.SetBuildOption(true, BUILD_OPTIONS.DESCENDING);
		m_OperatorSortOptions.lstSortOption = NKCOperatorSortSystem.GetDefaultSortOptions(bIsCollection: false);
		m_rtRoot = GetComponent<RectTransform>();
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnSlot;
			m_LoopScrollRect.dOnProvideData += ProvideSlotData;
		}
		dOnUnitSelect = onUnitSelect;
		dOnConfirm = onConfirm;
		dOnClose = OnClose;
		NKCUtil.SetGameobjectActive(m_rectSlotPoolRect, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetBindFunction(m_csbtnClose, Close);
		NKCUtil.SetBindFunction(m_csbtnBack, Close);
		NKCUtil.SetBindFunction(m_csbtnConfirm, OnClickConfirm);
		if (null != m_SortUI)
		{
			m_SortUI.Init(OnSortChanged, bIsCollection: false);
			m_SortUI.RegisterCategories(new HashSet<NKCUnitSortSystem.eFilterCategory>
			{
				NKCUnitSortSystem.eFilterCategory.Have,
				NKCUnitSortSystem.eFilterCategory.UnitType,
				NKCUnitSortSystem.eFilterCategory.UnitRole,
				NKCUnitSortSystem.eFilterCategory.UnitMoveType,
				NKCUnitSortSystem.eFilterCategory.UnitTargetType,
				NKCUnitSortSystem.eFilterCategory.Cost
			}, null, bFavoriteFilterActive: false);
		}
	}

	private RectTransform GetSlot(int index)
	{
		Stack<NKCUIUnitSelectListSlotBase> stack;
		NKCUIUnitSelectListSlotBase original;
		switch (CurrentTargetUnitType)
		{
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			stack = m_stkOperatorSlotPool;
			original = m_pfbOperatorSlot;
			break;
		case NKM_UNIT_TYPE.NUT_NORMAL:
			stack = m_stkUnitSlotPool;
			original = m_pfbUnitSlot;
			break;
		default:
			return null;
		}
		if (stack.Count > 0)
		{
			NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = stack.Pop();
			NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlotBase, bValue: true);
			nKCUIUnitSelectListSlotBase.transform.localScale = Vector3.one;
			m_lstVisibleSlot.Add(nKCUIUnitSelectListSlotBase);
			return nKCUIUnitSelectListSlotBase.GetComponent<RectTransform>();
		}
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase2 = Object.Instantiate(original);
		nKCUIUnitSelectListSlotBase2.Init();
		NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlotBase2, bValue: true);
		nKCUIUnitSelectListSlotBase2.transform.localScale = Vector3.one;
		m_lstVisibleSlot.Add(nKCUIUnitSelectListSlotBase2);
		return nKCUIUnitSelectListSlotBase2.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		NKCUIUnitSelectListSlotBase component = go.GetComponent<NKCUIUnitSelectListSlotBase>();
		m_lstVisibleSlot.Remove(component);
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(m_rectSlotPoolRect);
		if (component is NKCDeckViewUnitSelectListSlot)
		{
			m_stkUnitSlotPool.Push(component);
		}
		else if (component is NKCUIOperatorDeckSelectSlot)
		{
			m_stkOperatorSlotPool.Push(component);
		}
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		if (!m_bOpen)
		{
			return;
		}
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR && m_OperatorSortSystem == null)
		{
			Debug.LogError("Slot Operator Sort System Null!!");
			return;
		}
		if (CurrentTargetUnitType != NKM_UNIT_TYPE.NUT_OPERATOR && m_UnitSortSystem == null)
		{
			Debug.LogError("Slot Sort System Null!!");
			return;
		}
		NKCUIUnitSelectListSlotBase component = tr.GetComponent<NKCUIUnitSelectListSlotBase>();
		if (!(component == null))
		{
			ProvideSlotDataDefault(component, idx);
		}
	}

	private void ProvideSlotDataDefault(NKCUIUnitSelectListSlotBase slot, int idx)
	{
		int num = 0;
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			if (m_OperatorSortSystem.SortedOperatorList.Count <= idx)
			{
				return;
			}
			NKMOperator nKMOperator = m_OperatorSortSystem.SortedOperatorList[idx];
			_ = nKMOperator.uid;
			num = nKMOperator.id;
			slot.SetData(nKMOperator, NKMDeckIndex.None, bEnableLayoutElement: true, OnSlotSelected);
			slot.SetTouchHoldEvent(ShowOperatorInfo);
		}
		else
		{
			if (m_UnitSortSystem.SortedUnitList.Count <= idx)
			{
				return;
			}
			NKMUnitData nKMUnitData = m_UnitSortSystem.SortedUnitList[idx];
			_ = nKMUnitData.m_UnitUID;
			num = nKMUnitData.m_UnitID;
			slot.SetData(nKMUnitData, NKMDeckIndex.None, bEnableLayoutElement: true, OnSlotSelected);
		}
		NKCDeckViewUnitSelectListSlot nKCDeckViewUnitSelectListSlot = slot as NKCDeckViewUnitSelectListSlot;
		if (null != nKCDeckViewUnitSelectListSlot)
		{
			nKCDeckViewUnitSelectListSlot.SetSlotSelect(m_iSelectedUnitID == num);
		}
	}

	private void ShowUnitInfo(NKMUnitData unitData)
	{
		if (unitData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
			NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(m_UnitSortSystem.SortedUnitList);
			if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				NKCUIUnitInfo.Instance.Open(unitData, null, openOption);
			}
		}
	}

	private void ShowOperatorInfo(NKMOperator operatorData)
	{
		if (operatorData == null)
		{
			return;
		}
		int num = 0;
		using (List<NKMOperator>.Enumerator enumerator = m_OperatorSortSystem.SortedOperatorList.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current.uid != operatorData.uid)
			{
				num++;
			}
		}
		NKCUIOperatorInfo.OpenOption option = new NKCUIOperatorInfo.OpenOption(m_OperatorSortSystem.SortedOperatorList, num);
		NKCUIOperatorInfo.Instance.Open(operatorData, option);
	}

	private NKCUnitSortSystem.UnitListOptions MakeUnitSortOptions(NKCUnitSortSystem sortSystem)
	{
		NKCUnitSortSystem.UnitListOptions result = new NKCUnitSortSystem.UnitListOptions
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
			setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>(),
			lstSortOption = sortSystem.lstSortOption,
			bDescending = false,
			bIncludeUndeckableUnit = false,
			bHideDeckedUnit = false,
			bPushBackUnselectable = true
		};
		result.setExcludeUnitUID = new HashSet<long>();
		result.setDuplicateUnitID = new HashSet<int>();
		return result;
	}

	public void Open(NKCUnitSortSystem sortSystem, int SelectedTargetUnitID)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (!m_bOpen)
		{
			m_rtRoot.DOKill();
			m_rtRoot.anchoredPosition = new Vector2(m_rtRoot.GetWidth() * 1.5f, 0f);
			m_rtRoot.DOAnchorPosX(0f, 0.4f).SetEase(Ease.OutCubic);
		}
		m_UnitSortSystem = sortSystem;
		m_bOpen = true;
		m_sortOptions = MakeUnitSortOptions(sortSystem);
		m_iSelectedUnitID = SelectedTargetUnitID;
		m_iAlreadySelectedUnitID = SelectedTargetUnitID;
		m_SortUI.RegisterUnitSort(m_UnitSortSystem);
		m_SortUI.ResetUI();
		RefreshLoopScrollList(NKM_UNIT_TYPE.NUT_NORMAL, bResetPosition: true);
	}

	public void Open(NKCOperatorSortSystem.OperatorListOptions sortOptions)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (!m_bOpen)
		{
			m_rtRoot.DOKill();
			m_rtRoot.anchoredPosition = new Vector2(m_rtRoot.GetWidth() * 1.5f, 0f);
			m_rtRoot.DOAnchorPosX(0f, 0.4f).SetEase(Ease.OutCubic);
		}
		m_iSelectedUnitID = 0;
		m_bOpen = true;
		m_OperatorSortOptions = sortOptions;
		RefreshLoopScrollList(NKM_UNIT_TYPE.NUT_OPERATOR, bResetPosition: true);
	}

	public void Open(NKCOperatorSortSystem operSortSystem, NKCOperatorSortSystem.OperatorListOptions sortOptions, int SelectedTargetOperatorID)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (!m_bOpen)
		{
			m_rtRoot.DOKill();
			m_rtRoot.anchoredPosition = new Vector2(m_rtRoot.GetWidth() * 1.5f, 0f);
			m_rtRoot.DOAnchorPosX(0f, 0.4f).SetEase(Ease.OutCubic);
		}
		m_OperatorSortSystem = operSortSystem;
		m_bOpen = true;
		m_OperatorSortOptions = sortOptions;
		m_iSelectedUnitID = SelectedTargetOperatorID;
		m_iAlreadySelectedUnitID = SelectedTargetOperatorID;
		m_SortUI.RegisterOperatorSort(m_OperatorSortSystem);
		m_SortUI.ResetUI();
		m_SortUI.SetEnableFilter(!m_OperatorSortOptions.bHideFilter);
		RefreshLoopScrollList(NKM_UNIT_TYPE.NUT_OPERATOR, bResetPosition: true);
	}

	public void Close()
	{
		m_bOpen = false;
		m_iSelectedUnitID = 0;
		Cleanup();
		CurrentTargetUnitType = NKM_UNIT_TYPE.NUT_INVALID;
		dOnClose?.Invoke();
		m_rtRoot.DOKill();
		m_rtRoot.DOAnchorPosX(m_rtRoot.GetWidth() * 1.5f, 0.4f).SetEase(Ease.OutCubic).OnComplete(delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	private void Cleanup()
	{
		m_UnitSortSystem = null;
		m_OperatorSortSystem = null;
		m_SortUI?.ResetUI();
	}

	private void OnSortChanged(bool bResetScroll)
	{
		if (m_UnitSortSystem != null)
		{
			m_sortOptions = m_UnitSortSystem.Options;
			m_LoopScrollRect.TotalCount = m_UnitSortSystem.SortedUnitList.Count;
			if (bResetScroll)
			{
				m_LoopScrollRect.SetIndexPosition(0);
			}
			else
			{
				m_LoopScrollRect.RefreshCells();
			}
		}
	}

	private void RefreshLoopScrollList(NKM_UNIT_TYPE targetType, bool bResetPosition)
	{
		bool flag = CurrentTargetUnitType != targetType;
		CurrentTargetUnitType = targetType;
		if (flag)
		{
			m_sortOptions.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			m_sortOptions.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(targetType, bIsCollection: false);
			m_sortOptions.bDescending = true;
			m_sortOptions.bHideDeckedUnit = m_sortOptions.eDeckType == NKM_DECK_TYPE.NDT_NORMAL;
			m_OperatorSortOptions.setFilterOption = new HashSet<NKCOperatorSortSystem.eFilterOption>();
			m_OperatorSortOptions.lstSortOption = NKCOperatorSortSystem.GetDefaultSortOptions(bIsCollection: false);
			m_OperatorSortOptions.SetBuildOption(false, BUILD_OPTIONS.DESCENDING);
			m_OperatorSortOptions.SetBuildOption(m_OperatorSortOptions.eDeckType == NKM_DECK_TYPE.NDT_NORMAL, BUILD_OPTIONS.HIDE_DECKED_UNIT);
		}
		if (flag)
		{
			m_sortOptions.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			m_OperatorSortOptions.setFilterOption = new HashSet<NKCOperatorSortSystem.eFilterOption>();
			switch (targetType)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				m_LoopScrollRect.ContentConstraintCount = 3;
				m_GridLayoutGroup.constraintCount = 3;
				m_GridLayoutGroup.cellSize = m_vUnitSlotSize;
				m_GridLayoutGroup.spacing = m_vUnitSlotSpacing;
				break;
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				m_LoopScrollRect.ContentConstraintCount = 3;
				m_GridLayoutGroup.constraintCount = 3;
				m_GridLayoutGroup.cellSize = m_vOperatorSlotSize;
				m_GridLayoutGroup.spacing = m_vOperatorSlotSpacing;
				break;
			}
			m_LoopScrollRect.ResetContentSpacing();
			m_LoopScrollRect.PrepareCells();
		}
		if (m_UnitSortSystem != null)
		{
			m_sortOptions = m_UnitSortSystem.Options;
			m_LoopScrollRect.TotalCount = m_UnitSortSystem.SortedUnitList.Count;
			if (bResetPosition)
			{
				m_LoopScrollRect.SetIndexPosition(0);
			}
			else
			{
				m_LoopScrollRect.RefreshCells();
			}
		}
		else if (m_OperatorSortSystem != null)
		{
			m_OperatorSortOptions = m_OperatorSortSystem.Options;
			m_LoopScrollRect.TotalCount = m_OperatorSortSystem.SortedOperatorList.Count;
			if (bResetPosition)
			{
				m_LoopScrollRect.SetIndexPosition(0);
			}
			else
			{
				m_LoopScrollRect.RefreshCells();
			}
		}
	}

	private void OnSlotSelected(NKMUnitData selectedUnit, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		m_iSelectedUnitID = unitTempletBase.m_UnitID;
		dOnUnitSelect?.Invoke(m_iSelectedUnitID, CurrentTargetUnitType);
		UpdateSlotSelectUI(unitTempletBase.m_UnitID);
	}

	private void OnSlotSelected(NKMOperator selectedOperator, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		m_iSelectedUnitID = unitTempletBase.m_UnitID;
		dOnUnitSelect?.Invoke(m_iSelectedUnitID, CurrentTargetUnitType);
		UpdateSlotSelectUI(unitTempletBase.m_UnitID);
	}

	private void UpdateSlotSelectUI(int unitID)
	{
		foreach (NKCUIUnitSelectListSlotBase item in m_lstVisibleSlot)
		{
			if (item.NKMUnitTempletBase != null && item is NKCDeckViewUnitSelectListSlot)
			{
				(item as NKCDeckViewUnitSelectListSlot).SetSlotSelect(item.NKMUnitTempletBase.m_UnitID == unitID);
			}
		}
	}

	private void OnClickConfirm()
	{
		if (m_iSelectedUnitID == 0)
		{
			return;
		}
		if (m_iAlreadySelectedUnitID == m_iSelectedUnitID)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_DUPLICATE_WARNING_DESC);
			return;
		}
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_SELECT_WARNING_DESC, delegate
		{
			dOnConfirm?.Invoke(m_iSelectedUnitID, CurrentTargetUnitType);
		});
	}
}
