using System.Collections.Generic;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionUnitListV2 : MonoBehaviour
{
	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd & \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCUIUnitSelectListSlotBase m_pfbUnitSlot;

	public Vector2 m_vUnitSlotSize;

	public Vector2 m_vUnitSlotSpacing;

	[Header("UI Components")]
	public RectTransform m_rectContentRect;

	public RectTransform m_rectSlotPoolRect;

	public LoopScrollRect m_LoopScrollRect;

	public GridLayoutGroup m_GridLayoutGroup;

	public NKCUIComSafeArea m_safeArea;

	[Header("\ufffd\ufffd\ufffd\ufffd/\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffdUI")]
	public NKCUIComUnitSortOptions m_SortUI;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public int minColumnUnit = 5;

	public NKM_UNIT_TYPE m_eUnitType = NKM_UNIT_TYPE.NUT_NORMAL;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\u033c\ufffd")]
	public GameObject m_objAchievementRate;

	public Image m_imgAchievementGauge;

	public TMP_Text m_lbAchievememtCount;

	public TMP_Text m_lbAchievementPercent;

	public GameObject m_objAchievementRedDot;

	[Header("\ufffd\ufffdũ\ufffdѷ\ufffdƮ \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public RectTransform m_rectScrollAreaRect;

	public float m_fScrollRectUnitMissionOffTop;

	public float m_fScrollRectUnitMissionOnTop;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\u05b4\ufffd \ufffd\ufffd\ufffd\u05b8\ufffd \ufffd\ufffd\ufffd\u0334\ufffd \ufffd\ufffd\ufffd")]
	public NKCUIComToggle m_tglShowHasReward;

	[Header("etc")]
	public GameObject m_objNone;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\u07bc\ufffd\ufffd\ufffd")]
	public NKCUICollectionRate m_CollectionRate;

	private int m_iTotalUnit;

	private int m_iCollectionUnit;

	private NKCUnitSortSystem m_ssActive;

	private readonly HashSet<NKCUnitSortSystem.eFilterCategory> eUnitFilterCategories = new HashSet<NKCUnitSortSystem.eFilterCategory>
	{
		NKCUnitSortSystem.eFilterCategory.UnitType,
		NKCUnitSortSystem.eFilterCategory.SpecialType,
		NKCUnitSortSystem.eFilterCategory.UnitRole,
		NKCUnitSortSystem.eFilterCategory.UnitTargetType,
		NKCUnitSortSystem.eFilterCategory.Rarity,
		NKCUnitSortSystem.eFilterCategory.Cost,
		NKCUnitSortSystem.eFilterCategory.Collected,
		NKCUnitSortSystem.eFilterCategory.UnitMoveType,
		NKCUnitSortSystem.eFilterCategory.SkillType,
		NKCUnitSortSystem.eFilterCategory.AttackRange,
		NKCUnitSortSystem.eFilterCategory.SourceType
	};

	private readonly HashSet<NKCUnitSortSystem.eFilterCategory> eShipFilterCategories = new HashSet<NKCUnitSortSystem.eFilterCategory>
	{
		NKCUnitSortSystem.eFilterCategory.ShipType,
		NKCUnitSortSystem.eFilterCategory.Rarity,
		NKCUnitSortSystem.eFilterCategory.Collected
	};

	private readonly HashSet<NKCUnitSortSystem.eSortCategory> eUnitSortCategories = new HashSet<NKCUnitSortSystem.eSortCategory>
	{
		NKCUnitSortSystem.eSortCategory.IDX,
		NKCUnitSortSystem.eSortCategory.Rarity,
		NKCUnitSortSystem.eSortCategory.UnitSummonCost,
		NKCUnitSortSystem.eSortCategory.UnitPower,
		NKCUnitSortSystem.eSortCategory.UnitAttack,
		NKCUnitSortSystem.eSortCategory.UnitHealth,
		NKCUnitSortSystem.eSortCategory.UnitDefense,
		NKCUnitSortSystem.eSortCategory.UnitCrit,
		NKCUnitSortSystem.eSortCategory.UnitHit,
		NKCUnitSortSystem.eSortCategory.UnitEvade
	};

	private readonly HashSet<NKCUnitSortSystem.eSortCategory> eShipSortCategories = new HashSet<NKCUnitSortSystem.eSortCategory>
	{
		NKCUnitSortSystem.eSortCategory.IDX,
		NKCUnitSortSystem.eSortCategory.Rarity,
		NKCUnitSortSystem.eSortCategory.UnitAttack,
		NKCUnitSortSystem.eSortCategory.UnitHealth
	};

	private List<NKMUnitData> m_lstCollectionUnitData = new List<NKMUnitData>();

	private Dictionary<int, (int, int)> m_dicUnitMissionCompletedCount = new Dictionary<int, (int, int)>();

	private NKCUIUnitSelectList.UnitSelectListOptions m_currentOption;

	private NKCUICollection.OnSyncCollectingData dOnOnSyncCollectingData;

	private List<NKCUIUnitSelectListSlotBase> m_lstVisibleSlot = new List<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkUnitSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private List<NKMUnitData> m_lstCurUnitData = new List<NKMUnitData>();

	private bool m_bCellPrepared;

	public static List<NKMUnitData> UpdateCollectionUnitList(ref int collectionUnit, ref int totalUnit, NKM_UNIT_TYPE eUnitType, bool getUnitDataList)
	{
		List<NKMUnitData> list = null;
		if (getUnitDataList)
		{
			list = new List<NKMUnitData>();
		}
		List<int> unitList = NKCCollectionManager.GetUnitList(eUnitType);
		for (int i = 0; i < unitList.Count; i++)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitList[i]);
			if (unitTempletBase == null)
			{
				continue;
			}
			NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(unitTempletBase.m_UnitID);
			if ((unitTemplet == null || !unitTemplet.m_bExclude || NKCUtil.IsUnitObtainedAtLeastOnce(eUnitType, unitTempletBase.m_UnitID)) && unitTempletBase.PickupEnableByTag)
			{
				if (getUnitDataList)
				{
					list.Add(NKCUtil.MakeDummyUnit(unitList[i], 100, 3));
				}
				if (NKCUtil.IsUnitObtainedAtLeastOnce(eUnitType, unitList[i]))
				{
					collectionUnit++;
				}
				totalUnit++;
			}
		}
		return list;
	}

	public void UpdateCollectionMissionRate()
	{
		if (m_eUnitType != NKM_UNIT_TYPE.NUT_NORMAL)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objAchievementRate, NKCUnitMissionManager.GetOpenTagCollectionMission());
		if (m_objAchievementRate == null || !m_objAchievementRate.activeSelf)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		if (m_dicUnitMissionCompletedCount.Count != m_lstCollectionUnitData.Count)
		{
			m_dicUnitMissionCompletedCount.Clear();
		}
		if (m_dicUnitMissionCompletedCount.Count <= 0)
		{
			int count = m_lstCollectionUnitData.Count;
			for (int i = 0; i < count; i++)
			{
				int total = 0;
				int achieved = 0;
				NKCUnitMissionManager.GetUnitMissionAchievedCount(m_lstCollectionUnitData[i].m_UnitID, ref total, ref achieved);
				num += total;
				num2 += achieved;
				if (!m_dicUnitMissionCompletedCount.ContainsKey(m_lstCollectionUnitData[i].m_UnitID))
				{
					m_dicUnitMissionCompletedCount.Add(m_lstCollectionUnitData[i].m_UnitID, (achieved, total));
				}
			}
		}
		else
		{
			foreach (KeyValuePair<int, (int, int)> item in m_dicUnitMissionCompletedCount)
			{
				num2 += item.Value.Item1;
				num += item.Value.Item2;
			}
		}
		UpdateUnitMissionRate(num2, num);
	}

	public void UpdateCollectionMissionRate(List<int> unitIdList)
	{
		if (m_eUnitType != NKM_UNIT_TYPE.NUT_NORMAL || !NKCUnitMissionManager.GetOpenTagCollectionMission() || unitIdList == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int count = unitIdList.Count;
		for (int i = 0; i < count; i++)
		{
			num = 0;
			num2 = 0;
			NKCUnitMissionManager.GetUnitMissionAchievedCount(unitIdList[i], ref num, ref num2);
			if (m_dicUnitMissionCompletedCount.ContainsKey(unitIdList[i]))
			{
				m_dicUnitMissionCompletedCount[unitIdList[i]] = (num2, num);
			}
		}
		int num3 = 0;
		int num4 = 0;
		foreach (KeyValuePair<int, (int, int)> item in m_dicUnitMissionCompletedCount)
		{
			num4 += item.Value.Item1;
			num3 += item.Value.Item2;
		}
		UpdateUnitMissionRate(num4, num3);
		m_LoopScrollRect.RefreshCells();
	}

	private void UpdateUnitMissionRate(int achievedCount, int totalCount)
	{
		if (m_objAchievementRate != null && m_objAchievementRate.activeSelf)
		{
			float num = Mathf.Clamp(Mathf.Floor(achievedCount) / Mathf.Floor(totalCount), 0f, 1f);
			NKCUtil.SetImageFillAmount(m_imgAchievementGauge, num);
			NKCUtil.SetLabelText(m_lbAchievememtCount, $"{achievedCount}/{totalCount}");
			NKCUtil.SetLabelText(m_lbAchievementPercent, $"{Mathf.FloorToInt(num * 100f)}%");
		}
	}

	public void Init(NKCUICollection.OnSyncCollectingData callBack)
	{
		if (null != m_LoopScrollRect)
		{
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnSlot;
			m_LoopScrollRect.dOnProvideData += ProvideSlotData;
			m_LoopScrollRect.dOnRepopulate += CalculateContentRectSize;
			NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		}
		if (m_SortUI != null)
		{
			m_SortUI.Init(OnSortChanged, bIsCollection: true);
			NKM_UNIT_TYPE eUnitType = m_eUnitType;
			if (eUnitType == NKM_UNIT_TYPE.NUT_NORMAL || eUnitType != NKM_UNIT_TYPE.NUT_SHIP)
			{
				m_SortUI.RegisterCategories(eUnitFilterCategories, eUnitSortCategories, bFavoriteFilterActive: false);
			}
			else
			{
				m_SortUI.RegisterCategories(eShipFilterCategories, eShipSortCategories, bFavoriteFilterActive: false);
			}
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
		if (m_tglShowHasReward != null)
		{
			m_tglShowHasReward.OnValueChanged.RemoveAllListeners();
			m_tglShowHasReward.OnValueChanged.AddListener(OnToggleHasReward);
		}
		m_bCellPrepared = false;
		m_CollectionRate?.Init();
	}

	public void Open()
	{
		m_iTotalUnit = 0;
		m_iCollectionUnit = 0;
		m_ssActive = null;
		NKCUtil.SetGameobjectActive(m_objAchievementRedDot, bValue: false);
		NKCUIUnitSelectList.UnitSelectListOptions currentOption = new NKCUIUnitSelectList.UnitSelectListOptions(m_eUnitType, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
		currentOption.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		NKM_UNIT_TYPE eUnitType = m_eUnitType;
		if (eUnitType == NKM_UNIT_TYPE.NUT_NORMAL || eUnitType != NKM_UNIT_TYPE.NUT_SHIP)
		{
			currentOption.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: true);
			bool bValue = NKCUnitMissionManager.HasRewardEnableMission();
			NKCUtil.SetGameobjectActive(m_objAchievementRedDot, bValue);
			m_tglShowHasReward?.Select(bSelect: false, bForce: true);
		}
		else
		{
			currentOption.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_SHIP, bIsCollection: true);
		}
		currentOption.bDescending = false;
		currentOption.bShowRemoveSlot = false;
		currentOption.bMultipleSelect = false;
		currentOption.iMaxMultipleSelect = 0;
		currentOption.bExcludeLockedUnit = false;
		currentOption.bExcludeDeckedUnit = false;
		currentOption.bShowHideDeckedUnitMenu = true;
		currentOption.bHideDeckedUnit = false;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_currentOption = currentOption;
		PrepareUnitList();
		UpdateCollectingRate();
		UpdateCollectionMissionRate();
	}

	public void Clear()
	{
		NKCUICollectionShipInfo.CheckInstanceAndClose();
		NKCUICollectionUnitInfo.CheckInstanceAndClose();
		NKCScenManager.GetScenManager().m_NKCMemoryCleaner.UnloadObjectPool();
		m_dicUnitMissionCompletedCount.Clear();
		m_dicUnitMissionCompletedCount = null;
	}

	private void UpdateCollectingRate()
	{
		if (dOnOnSyncCollectingData != null)
		{
			switch (m_eUnitType)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				dOnOnSyncCollectingData(NKCUICollectionGeneral.CollectionType.CT_UNIT, m_iCollectionUnit, m_iTotalUnit);
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
				dOnOnSyncCollectingData(NKCUICollectionGeneral.CollectionType.CT_SHIP, m_iCollectionUnit, m_iTotalUnit);
				break;
			}
		}
		switch (m_eUnitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			m_CollectionRate?.SetData(NKCUICollectionGeneral.CollectionType.CT_UNIT, m_iCollectionUnit, m_iTotalUnit);
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
			m_CollectionRate?.SetData(NKCUICollectionGeneral.CollectionType.CT_SHIP, m_iCollectionUnit, m_iTotalUnit);
			break;
		}
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
		if (m_ssActive == null)
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
		if (m_currentOption.bShowRemoveSlot)
		{
			if (idx == 0)
			{
				component.SetEmpty(bEnableLayoutElement: true, OnSlotSelected);
				return;
			}
			idx--;
		}
		if (m_ssActive.SortedUnitList.Count <= idx)
		{
			return;
		}
		NKMUnitData nKMUnitData = m_ssActive.SortedUnitList[idx];
		long unitUID = nKMUnitData.m_UnitUID;
		NKMDeckIndex deckIndexCache = m_ssActive.GetDeckIndexCache(unitUID, bTargetDecktypeOnly: true);
		_ = NKCScenManager.CurrentUserData().m_ArmyData;
		component.SetDataForCollection(nKMUnitData, deckIndexCache, OnSlotSelected, NKCUtil.IsUnitObtainedAtLeastOnce(m_eUnitType, nKMUnitData.m_UnitID));
		if (m_ssActive.lstSortOption.Count > 0)
		{
			switch (m_ssActive.lstSortOption[0])
			{
			case NKCUnitSortSystem.eSortOption.Power_Low:
			case NKCUnitSortSystem.eSortOption.Power_High:
				component.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Power_High, m_ssActive.GetUnitPowerCache(unitUID), "N0");
				break;
			case NKCUnitSortSystem.eSortOption.Attack_Low:
			case NKCUnitSortSystem.eSortOption.Attack_High:
				component.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Attack_High, m_ssActive.GetUnitAttackCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Health_Low:
			case NKCUnitSortSystem.eSortOption.Health_High:
				component.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Health_High, m_ssActive.GetUnitHPCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Defense_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Defense_High:
				component.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Defense_High, m_ssActive.GetUnitDefCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Crit_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Crit_High:
				component.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Crit_High, m_ssActive.GetUnitCritCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Hit_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Hit_High:
				component.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Hit_High, m_ssActive.GetUnitHitCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Evade_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Evade_High:
				component.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Evade_High, m_ssActive.GetUnitEvadeCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_Low:
			case NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_High:
				component.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_High, m_ssActive.GetUnitSkillCoolCache(unitUID));
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
		component.SetSlotState(m_ssActive.GetUnitSlotState(nKMUnitData.m_UnitUID));
		m_currentOption.dOnSlotSetData?.Invoke(component, nKMUnitData, deckIndexCache);
	}

	private void OnSlotSelected(NKMUnitData selectedUnit, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		if (selectedUnit == null)
		{
			return;
		}
		int unitID = selectedUnit.m_UnitID;
		m_lstCurUnitData = m_ssActive.GetCurrentUnitList();
		int num = -1;
		for (int i = 0; i < m_lstCurUnitData.Count; i++)
		{
			if (m_lstCurUnitData[i].m_UnitID == unitID)
			{
				num = i;
				break;
			}
		}
		if (num > -1 && m_lstCurUnitData.Contains(selectedUnit))
		{
			NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(m_lstCurUnitData, num);
			NKM_UNIT_TYPE eUnitType = m_eUnitType;
			if (eUnitType == NKM_UNIT_TYPE.NUT_NORMAL || eUnitType != NKM_UNIT_TYPE.NUT_SHIP)
			{
				NKCUICollectionUnitInfo.CheckInstanceAndOpen(m_lstCurUnitData[num], openOption);
			}
			else
			{
				NKCUICollectionShipInfo.CheckInstanceAndOpen(m_lstCurUnitData[num], NKMDeckIndex.None, openOption);
			}
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
		m_lstCollectionUnitData = UpdateCollectionUnitList(ref m_iCollectionUnit, ref m_iTotalUnit, m_eUnitType, getUnitDataList: true);
		m_ssActive = new NKCGenericUnitSort(null, m_currentOption.m_SortOptions, m_lstCollectionUnitData);
		m_SortUI.RegisterUnitSort(m_ssActive);
		m_SortUI.ResetUI();
		OnSortChanged(bResetScroll: true);
	}

	private void CalculateContentRectSize()
	{
		if (m_safeArea != null)
		{
			m_safeArea.SetSafeAreaBase();
		}
		NKCUtil.CalculateContentRectSize(m_LoopScrollRect, m_GridLayoutGroup, minColumnUnit, m_vUnitSlotSize, m_vUnitSlotSpacing, m_eUnitType == NKM_UNIT_TYPE.NUT_SHIP);
	}

	private void AdjustScrollRectTop()
	{
		if (!(m_rectScrollAreaRect == null) && m_eUnitType == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			bool openTagCollectionMission = NKCUnitMissionManager.GetOpenTagCollectionMission();
			Vector2 offsetMax = m_rectScrollAreaRect.offsetMax;
			offsetMax.y = (openTagCollectionMission ? (0f - m_fScrollRectUnitMissionOnTop) : (0f - m_fScrollRectUnitMissionOffTop));
			m_rectScrollAreaRect.offsetMax = offsetMax;
		}
	}

	private void OnSortChanged(bool bResetScroll)
	{
		if (m_ssActive != null)
		{
			m_LoopScrollRect.TotalCount = m_ssActive.SortedUnitList.Count;
			NKCUtil.SetGameobjectActive(m_objNone, m_ssActive.SortedUnitList.Count == 0);
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

	private void OnToggleHasReward(bool bValue)
	{
		bool bResetScroll = false;
		if (bValue)
		{
			if (!m_ssActive.FilterSet.Contains(NKCUnitSortSystem.eFilterOption.Collection_HasAchieve))
			{
				m_ssActive.FilterSet.Add(NKCUnitSortSystem.eFilterOption.Collection_HasAchieve);
			}
			bResetScroll = true;
		}
		else if (m_ssActive.FilterSet.Contains(NKCUnitSortSystem.eFilterOption.Collection_HasAchieve))
		{
			m_ssActive.FilterSet.Remove(NKCUnitSortSystem.eFilterOption.Collection_HasAchieve);
		}
		m_ssActive.FilterList(m_ssActive.FilterSet, m_ssActive.Options.bHideDeckedUnit);
		OnSortChanged(bResetScroll);
	}

	public void CheckRewardToggle()
	{
		if (!(m_tglShowHasReward == null))
		{
			bool flag = NKCUnitMissionManager.HasRewardEnableMission();
			NKCUtil.SetGameobjectActive(m_objAchievementRedDot, flag);
			m_tglShowHasReward.Select(m_ssActive != null && m_ssActive.FilterSet != null && m_ssActive.FilterSet.Contains(NKCUnitSortSystem.eFilterOption.Collection_HasAchieve), bForce: true);
			OnToggleHasReward(flag && m_tglShowHasReward.m_bSelect);
		}
	}

	public bool IsOpenUnitInfo()
	{
		return NKCUICollectionUnitInfo.IsInstanceOpen;
	}
}
