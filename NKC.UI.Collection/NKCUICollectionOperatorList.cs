using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionOperatorList : MonoBehaviour
{
	[Header("유닛 슬롯 프리팹 & 사이즈 설정")]
	public NKCUIUnitSelectListSlotBase m_pfbUnitSlot;

	public Vector2 m_vUnitSlotSize;

	public Vector2 m_vUnitSlotSpacing;

	[Header("UI Components")]
	public RectTransform m_rectContentRect;

	public RectTransform m_rectSlotPoolRect;

	public LoopScrollRect m_LoopScrollRect;

	public GridLayoutGroup m_GridLayoutGroup;

	public NKCUIComSafeArea m_safeArea;

	[Header("정렬/필터 통합UI")]
	public NKCUIComUnitSortOptions m_SortUI;

	[Header("유닛 설정")]
	public int minColumnUnit = 5;

	private int m_iTotalUnit;

	private int m_iCollectionUnit;

	[Header("수집률")]
	public NKCUICollectionRate m_CollectionRate;

	private NKCOperatorSortSystem m_OperatorSortSystem;

	private readonly HashSet<NKCOperatorSortSystem.eFilterCategory> eOperatorFilterCategories = new HashSet<NKCOperatorSortSystem.eFilterCategory> { NKCOperatorSortSystem.eFilterCategory.Rarity };

	private readonly HashSet<NKCOperatorSortSystem.eSortCategory> eOperatorSortCategories = new HashSet<NKCOperatorSortSystem.eSortCategory>
	{
		NKCOperatorSortSystem.eSortCategory.IDX,
		NKCOperatorSortSystem.eSortCategory.Rarity,
		NKCOperatorSortSystem.eSortCategory.UnitPower,
		NKCOperatorSortSystem.eSortCategory.UnitAttack,
		NKCOperatorSortSystem.eSortCategory.UnitHealth,
		NKCOperatorSortSystem.eSortCategory.UnitDefense,
		NKCOperatorSortSystem.eSortCategory.UnitReduceSkillCool
	};

	private List<NKMOperator> m_lstCollectionOperatorData = new List<NKMOperator>();

	private NKCUIUnitSelectList.UnitSelectListOptions m_currentOption;

	private NKCUICollection.OnSyncCollectingData dOnOnSyncCollectingData;

	private List<NKCUIUnitSelectListSlotBase> m_lstVisibleSlot = new List<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkUnitSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private List<NKMOperator> m_lstCurOperatorData = new List<NKMOperator>();

	private bool m_bCellPrepared;

	public static List<NKMOperator> UpdateCollectionUnitList(ref int collectionUnit, ref int totalUnit, bool createOperatorDataList)
	{
		List<NKMOperator> list = null;
		if (createOperatorDataList)
		{
			list = new List<NKMOperator>();
		}
		List<int> unitList = NKCCollectionManager.GetUnitList(NKM_UNIT_TYPE.NUT_OPERATOR);
		for (int i = 0; i < unitList.Count; i++)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitList[i]);
			if (unitTempletBase == null || !unitTempletBase.CollectionEnableByTag)
			{
				continue;
			}
			NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(unitTempletBase.m_UnitID);
			if (unitTemplet == null || !unitTemplet.m_bExclude || IsHasOperator(unitList[i]))
			{
				if (createOperatorDataList)
				{
					list.Add(NKCOperatorUtil.GetDummyOperator(unitList[i], bSetMaximum: true));
				}
				if (IsHasOperator(unitList[i]))
				{
					collectionUnit++;
				}
				totalUnit++;
			}
		}
		return list;
	}

	private static bool IsHasOperator(int UnitID = 0)
	{
		bool result = false;
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (armyData != null && !armyData.IsFirstGetUnit(UnitID))
		{
			return true;
		}
		return result;
	}

	public void Init(NKCUICollection.OnSyncCollectingData callBack)
	{
		if (null != m_LoopScrollRect)
		{
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnSlot;
			m_LoopScrollRect.dOnProvideData += ProvideSlotData;
		}
		if (m_SortUI != null)
		{
			m_SortUI.Init(OnSortChanged, bIsCollection: true);
			if (NKCCollectionManager.IsCollectionV2Active)
			{
				eOperatorFilterCategories.Add(NKCOperatorSortSystem.eFilterCategory.Collected);
			}
			m_SortUI.RegisterCategories(eOperatorFilterCategories, eOperatorSortCategories, bFavoriteFilterActive: false);
			if (m_SortUI.m_NKCPopupSort != null)
			{
				m_SortUI.m_NKCPopupSort.m_bUseDefaultSortAdd = false;
			}
		}
		base.gameObject.SetActive(value: false);
		if (callBack != null)
		{
			dOnOnSyncCollectingData = callBack;
		}
		m_bCellPrepared = false;
	}

	public void Open()
	{
		m_iTotalUnit = 0;
		m_iCollectionUnit = 0;
		m_OperatorSortSystem = null;
		NKCUIUnitSelectList.UnitSelectListOptions currentOption = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_OPERATOR, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
		currentOption.setOperatorFilterOption = new HashSet<NKCOperatorSortSystem.eFilterOption>();
		currentOption.lstOperatorSortOption = NKCOperatorSortSystem.GetDefaultSortOptions(bIsCollection: true);
		currentOption.bShowHideDeckedUnitMenu = true;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_currentOption = currentOption;
		PrepareUnitList();
		UpdateCollectingRate();
	}

	public void Clear()
	{
		NKCUICollectionOperatorInfo.CheckInstanceAndClose();
		NKCScenManager.GetScenManager().m_NKCMemoryCleaner.UnloadObjectPool();
	}

	private void UpdateCollectingRate()
	{
		if (dOnOnSyncCollectingData != null)
		{
			dOnOnSyncCollectingData(NKCUICollectionGeneral.CollectionType.CT_OPERATOR, m_iCollectionUnit, m_iTotalUnit);
		}
		m_CollectionRate?.SetData(NKCUICollectionGeneral.CollectionType.CT_OPERATOR, m_iCollectionUnit, m_iTotalUnit);
	}

	private RectTransform GetSlot(int index)
	{
		Stack<NKCUIUnitSelectListSlotBase> stkUnitSlotPool = m_stkUnitSlotPool;
		NKCUIUnitSelectListSlotBase pfbUnitSlot = m_pfbUnitSlot;
		if (stkUnitSlotPool.Count > 0)
		{
			NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = stkUnitSlotPool.Pop();
			NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlotBase, bValue: true);
			nKCUIUnitSelectListSlotBase.transform.localScale = Vector3.one;
			m_lstVisibleSlot.Add(nKCUIUnitSelectListSlotBase);
			return nKCUIUnitSelectListSlotBase.GetComponent<RectTransform>();
		}
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase2 = Object.Instantiate(pfbUnitSlot);
		nKCUIUnitSelectListSlotBase2.Init(resetLocalScale: true);
		NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlotBase2, bValue: true);
		nKCUIUnitSelectListSlotBase2.transform.localScale = Vector3.one;
		m_lstVisibleSlot.Add(nKCUIUnitSelectListSlotBase2);
		return nKCUIUnitSelectListSlotBase2.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		NKCUIUnitSelectListSlotBase component = go.GetComponent<NKCUIUnitSelectListSlotBase>();
		m_lstVisibleSlot.Remove(component);
		go.SetParent(m_rectSlotPoolRect);
		if (component is NKCUIUnitSelectListSlot)
		{
			m_stkUnitSlotPool.Push(component);
		}
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		if (m_OperatorSortSystem == null)
		{
			Debug.LogError("Slot Sort System Null!!");
			return;
		}
		NKCUIUnitSelectListSlotBase component = tr.GetComponent<NKCUIUnitSelectListSlotBase>();
		if (component == null)
		{
			return;
		}
		component.Init(resetLocalScale: true);
		if (m_OperatorSortSystem.SortedOperatorList.Count <= idx)
		{
			return;
		}
		NKMOperator nKMOperator = m_OperatorSortSystem.SortedOperatorList[idx];
		long uid = nKMOperator.uid;
		NKMDeckIndex deckIndexCache = m_OperatorSortSystem.GetDeckIndexCache(uid, bTargetDecktypeOnly: true);
		_ = NKCScenManager.CurrentUserData().m_ArmyData;
		component.SetDataForCollection(nKMOperator, deckIndexCache, OnSlotSelected, IsHasOperator(nKMOperator.id));
		if (m_OperatorSortSystem.lstSortOption.Count > 0)
		{
			switch (m_OperatorSortSystem.lstSortOption[0])
			{
			case NKCOperatorSortSystem.eSortOption.Power_Low:
			case NKCOperatorSortSystem.eSortOption.Power_High:
				component.SetSortingTypeValue(bSet: true, NKCOperatorSortSystem.eSortOption.Power_High, m_OperatorSortSystem.GetUnitPowerCache(uid), "N0");
				break;
			case NKCOperatorSortSystem.eSortOption.Attack_Low:
			case NKCOperatorSortSystem.eSortOption.Attack_High:
				component.SetSortingTypeValue(bSet: true, NKCOperatorSortSystem.eSortOption.Attack_High, m_OperatorSortSystem.GetUnitAttackCache(uid));
				break;
			case NKCOperatorSortSystem.eSortOption.Health_Low:
			case NKCOperatorSortSystem.eSortOption.Health_High:
				component.SetSortingTypeValue(bSet: true, NKCOperatorSortSystem.eSortOption.Health_High, m_OperatorSortSystem.GetUnitHPCache(uid));
				break;
			case NKCOperatorSortSystem.eSortOption.Unit_Defense_Low:
			case NKCOperatorSortSystem.eSortOption.Unit_Defense_High:
				component.SetSortingTypeValue(bSet: true, NKCOperatorSortSystem.eSortOption.Unit_Defense_High, m_OperatorSortSystem.GetUnitDefCache(uid));
				break;
			case NKCOperatorSortSystem.eSortOption.Unit_ReduceSkillCool_Low:
			case NKCOperatorSortSystem.eSortOption.Unit_ReduceSkillCool_High:
				component.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_High, m_OperatorSortSystem.GetUnitSkillCoolCache(uid));
				break;
			default:
				component.SetSortingTypeValue(bSet: false);
				break;
			}
		}
		else
		{
			component.SetSortingTypeValue(bSet: false);
		}
		component.SetSlotState(m_OperatorSortSystem.GetUnitSlotState(nKMOperator.uid));
		m_currentOption.dOnSlotOperatorSetData?.Invoke(component, nKMOperator, deckIndexCache);
	}

	private void OnSlotSelected(NKMOperator selectedUnit, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		if (selectedUnit == null)
		{
			return;
		}
		m_lstCurOperatorData = m_OperatorSortSystem.GetCurrentOperatorList();
		int num = -1;
		for (int i = 0; i < m_lstCurOperatorData.Count; i++)
		{
			if (m_lstCurOperatorData[i].id == selectedUnit.id)
			{
				num = i;
				break;
			}
		}
		if (num > -1 && m_lstCurOperatorData.Contains(selectedUnit))
		{
			NKCUIOperatorInfo.OpenOption openOption = new NKCUIOperatorInfo.OpenOption(m_lstCurOperatorData, num);
			NKCUICollectionUnitInfo.CheckInstanceAndOpen(m_lstCurOperatorData[num], openOption);
		}
	}

	private void PrepareUnitList(bool bForceRebuildList = false)
	{
		if (!m_bCellPrepared)
		{
			m_bCellPrepared = true;
			CalculateContentRectSize();
			m_LoopScrollRect.PrepareCells();
		}
		m_lstCollectionOperatorData = UpdateCollectionUnitList(ref m_iCollectionUnit, ref m_iTotalUnit, createOperatorDataList: true);
		m_OperatorSortSystem = new NKCGenericOperatorSort(null, m_currentOption.m_OperatorSortOptions, m_lstCollectionOperatorData);
		m_SortUI.RegisterOperatorSort(m_OperatorSortSystem);
		m_SortUI.ResetUI();
		OnSortChanged(bResetScroll: true);
	}

	private void CalculateContentRectSize()
	{
		if (m_safeArea != null)
		{
			m_safeArea.SetSafeAreaBase();
		}
		NKCUtil.CalculateContentRectSize(m_LoopScrollRect, m_GridLayoutGroup, minColumnUnit, m_vUnitSlotSize, m_vUnitSlotSpacing);
	}

	private void OnSortChanged(bool bResetScroll)
	{
		if (m_OperatorSortSystem != null)
		{
			m_LoopScrollRect.TotalCount = m_OperatorSortSystem.SortedOperatorList.Count;
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
}
