using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC;

[RequireComponent(typeof(RectTransform))]
public class NKCLeaguePvpUnitSelectList : MonoBehaviour
{
	public delegate void OnDeckUnitChangeClicked(NKMDeckIndex deckIndex, long uid, NKM_UNIT_TYPE eType);

	public delegate void OnClose(NKM_UNIT_TYPE eType);

	private RectTransform m_rtRoot;

	public NKCDeckViewUnitSelectListSlot m_pfbUnitSlot;

	public Vector2 m_vUnitSlotSize;

	public Vector2 m_vUnitSlotSpacing;

	public NKCUIShipSelectListSlot m_pfbShipSlot;

	public Vector2 m_vShipSlotSize;

	public Vector2 m_vShipSlotSpacing;

	public NKCUIOperatorDeckSelectSlot m_pfbOperatorSlot;

	public Vector2 m_vOperatorSlotSize;

	public Vector2 m_vOperatorSlotSpacing;

	public LoopScrollRect m_LoopScrollRect;

	public GridLayoutGroup m_GridLayoutGroup;

	public RectTransform m_rectSlotPoolRect;

	[Header("정렬 관련 통합 UI")]
	public NKCUIComUnitSortOptions m_SortUI;

	[Header("그 외")]
	public NKCUIComStateButton m_sbtnFinish;

	private bool m_bOpen;

	private OnDeckUnitChangeClicked dOnDeckUnitChangeClicked;

	private NKCUIUnitSelectListSlotBase.OnSelectThisSlot dOnSelectThisSlot;

	private OnClose dOnClose;

	private UnityAction dOnClearDeck;

	private UnityAction dOnAutoCompleteDeck;

	private List<NKCUIUnitSelectListSlotBase> m_lstVisibleSlot = new List<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkUnitSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkShipSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkOperatorSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private NKCUnitSortSystem m_ssActive;

	private Dictionary<NKM_UNIT_TYPE, NKCUnitSortSystem> m_dicUnitSortSystem = new Dictionary<NKM_UNIT_TYPE, NKCUnitSortSystem>();

	private NKCUnitSortSystem.UnitListOptions m_sortOptions;

	private NKCUIDeckViewer.DeckViewerOption m_DeckViewerOptions;

	private NKCOperatorSortSystem m_OperatorSortSystem;

	private NKCOperatorSortSystem.OperatorListOptions m_OperatorSortOptions;

	private List<NKMUnitData> m_unitCandidateList = new List<NKMUnitData>();

	private string m_banCandidateSearchString = "";

	public bool IsOpen => m_bOpen;

	public NKM_UNIT_TYPE CurrentTargetUnitType { get; private set; }

	public NKCUnitSortSystem.UnitListOptions SortOptions => m_sortOptions;

	public NKCOperatorSortSystem.OperatorListOptions SortOperatorOptions => m_OperatorSortOptions;

	public void Init(NKCUIDeckViewer deckView, OnDeckUnitChangeClicked onDeckUnitChangeClicked, NKCUIUnitSelectListSlotBase.OnSelectThisSlot onSelectThisSlot, OnClose onClose)
	{
		m_banCandidateSearchString = "";
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
			NKCUtil.SetScrollHotKey(m_LoopScrollRect, deckView);
		}
		if (m_sbtnFinish != null)
		{
			m_sbtnFinish.PointerClick.RemoveAllListeners();
			m_sbtnFinish.PointerClick.AddListener(OnCloseBtn);
		}
		if (m_SortUI != null)
		{
			m_SortUI.Init(OnSortChanged, bIsCollection: false);
			m_SortUI.RegisterCategories(NKCPopupFilterUnit.MakeDefaultFilterOption(NKM_UNIT_TYPE.NUT_NORMAL, NKCPopupFilterUnit.FILTER_OPEN_TYPE.NORMAL), NKCPopupSort.MakeDefaultSortSet(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false), bFavoriteFilterActive: false);
		}
		dOnDeckUnitChangeClicked = onDeckUnitChangeClicked;
		dOnSelectThisSlot = onSelectThisSlot;
		dOnClose = onClose;
		NKCUtil.SetGameobjectActive(m_rectSlotPoolRect, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private RectTransform GetSlot(int index)
	{
		Stack<NKCUIUnitSelectListSlotBase> stack;
		NKCUIUnitSelectListSlotBase original;
		switch (CurrentTargetUnitType)
		{
		case NKM_UNIT_TYPE.NUT_SHIP:
			stack = m_stkShipSlotPool;
			original = m_pfbShipSlot;
			break;
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
		else if (component is NKCUIShipSelectListSlot)
		{
			m_stkShipSlotPool.Push(component);
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
		if (CurrentTargetUnitType != NKM_UNIT_TYPE.NUT_OPERATOR && m_ssActive == null)
		{
			Debug.LogError("Slot Sort System Null!!");
			return;
		}
		NKCUIUnitSelectListSlotBase component = tr.GetComponent<NKCUIUnitSelectListSlotBase>();
		if (!(component == null))
		{
			if (m_DeckViewerOptions.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.LeaguePvPMain)
			{
				ProvideSlotDataForLeaguePvp(component, idx);
			}
			else
			{
				ProvideSlotDataForLegueGlobalBan(component, idx);
			}
		}
	}

	private void ProvideSlotDataForLeaguePvp(NKCUIUnitSelectListSlotBase slot, int idx)
	{
		long num = 0L;
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			if (m_OperatorSortSystem.SortedOperatorList.Count <= idx)
			{
				return;
			}
			NKMOperator nKMOperator = m_OperatorSortSystem.SortedOperatorList[idx];
			num = nKMOperator.uid;
			if (slot.NKMOperatorData == null || slot.NKMOperatorData.uid != num)
			{
				NKMDeckIndex deckIndexCache = m_OperatorSortSystem.GetDeckIndexCache(num, bTargetDecktypeOnly: true);
				slot.SetData(nKMOperator, deckIndexCache, bEnableLayoutElement: true, OnSlotSelected);
			}
			NKCUnitSortSystem.eUnitState unitSlotState = NKCLeaguePVPMgr.GetUnitSlotState(CurrentTargetUnitType, nKMOperator.id, checkMyTeamOnly: false);
			if (unitSlotState != NKCUnitSortSystem.eUnitState.NONE)
			{
				slot.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.NONE);
				slot.SetSlotState(unitSlotState);
			}
		}
		else
		{
			if (m_ssActive.SortedUnitList.Count <= idx)
			{
				return;
			}
			NKMUnitData nKMUnitData = m_ssActive.SortedUnitList[idx];
			num = nKMUnitData.m_UnitUID;
			if (slot.NKMUnitData == null || slot.NKMUnitData.m_UnitUID != num)
			{
				NKMDeckIndex deckIndexCacheByOption = m_ssActive.GetDeckIndexCacheByOption(num, bTargetDeckTypeOnly: true);
				slot.SetData(nKMUnitData, deckIndexCacheByOption, bEnableLayoutElement: true, OnSlotSelected);
			}
			NKCUnitSortSystem.eUnitState unitSlotState2 = NKCLeaguePVPMgr.GetUnitSlotState(CurrentTargetUnitType, nKMUnitData.m_UnitID, checkMyTeamOnly: false);
			if (unitSlotState2 != NKCUnitSortSystem.eUnitState.NONE)
			{
				slot.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.NONE);
				slot.SetSlotState(unitSlotState2);
			}
			slot.SetEnableLeagueBan(unitSlotState2 == NKCUnitSortSystem.eUnitState.LEAGUE_BANNED);
		}
		slot.SetEnableShowBan(bSet: false);
		slot.SetEnableShowUpUnit(bSet: false);
		slot.SetCityLeaderMark(value: false);
	}

	private void ProvideSlotDataForLegueGlobalBan(NKCUIUnitSelectListSlotBase slot, int idx)
	{
		if (m_ssActive.SortedUnitList.Count > idx)
		{
			NKMUnitData nKMUnitData = m_ssActive.SortedUnitList[idx];
			if (slot.NKMUnitData == null || slot.NKMUnitData.m_UnitID != nKMUnitData.m_UnitID)
			{
				NKMUnitTempletBase templetBase = NKMUnitTempletBase.Find(nKMUnitData.m_UnitID);
				slot.SetData(templetBase, 0, 0, bEnableLayoutElement: false, dOnSelectThisSlot);
			}
			NKCUnitSortSystem.eUnitState unitSlotState = NKCLeaguePVPMgr.GetUnitSlotState(CurrentTargetUnitType, nKMUnitData.m_UnitID, checkMyTeamOnly: true);
			if (unitSlotState != NKCUnitSortSystem.eUnitState.NONE)
			{
				slot.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.DISABLE);
				slot.SetSlotState(unitSlotState);
			}
		}
	}

	public void UpdateSearchString(string searchString)
	{
		m_banCandidateSearchString = searchString;
		InvalidateSortData(NKM_UNIT_TYPE.NUT_NORMAL);
	}

	private void ShowUnitInfo(NKMUnitData unitData)
	{
		if (unitData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
			NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(m_ssActive.SortedUnitList);
			switch (unitTempletBase.m_NKM_UNIT_TYPE)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				NKCUIUnitInfo.Instance.Open(unitData, null, openOption);
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
			{
				NKMDeckIndex shipDeckIndex = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetShipDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, unitData.m_UnitUID);
				NKCUIShipInfo.Instance.Open(unitData, shipDeckIndex, openOption);
				break;
			}
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

	private int GetSlotCount()
	{
		return m_ssActive.SortedUnitList.Count;
	}

	private NKCUnitSortSystem GetUnitSortSystem(NKM_UNIT_TYPE type)
	{
		if (m_dicUnitSortSystem.ContainsKey(type) && m_dicUnitSortSystem[type] != null)
		{
			return m_dicUnitSortSystem[type];
		}
		NKCUnitSortSystem nKCUnitSortSystem;
		if (type == NKM_UNIT_TYPE.NUT_NORMAL || type != NKM_UNIT_TYPE.NUT_SHIP)
		{
			if (m_DeckViewerOptions.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.LeaguePvPGlobalBan)
			{
				UpdateGlobalBanUnitList();
				nKCUnitSortSystem = new NKCGenericUnitSort(null, m_sortOptions, m_unitCandidateList);
			}
			else
			{
				UpdateLeaguePickUnitList();
				nKCUnitSortSystem = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), m_sortOptions, m_unitCandidateList);
			}
		}
		else
		{
			nKCUnitSortSystem = new NKCShipSort(NKCScenManager.CurrentUserData(), m_sortOptions);
		}
		nKCUnitSortSystem.SetEnableShowBan(NKCUtil.CheckPossibleShowBan(m_DeckViewerOptions.eDeckviewerMode));
		nKCUnitSortSystem.SetEnableShowUpUnit(NKCUtil.CheckPossibleShowUpUnit(m_DeckViewerOptions.eDeckviewerMode));
		m_dicUnitSortSystem[type] = nKCUnitSortSystem;
		return nKCUnitSortSystem;
	}

	private void UpdateGlobalBanUnitList()
	{
		m_unitCandidateList.Clear();
		foreach (NKMUnitTempletBase item in NKMUnitTempletBase.Get_listNKMUnitTempletBaseForUnit())
		{
			if (item.CollectionEnableByTag && item.m_bContractable && NKMUnitManager.CanUnitUsedInDeck(item) && item.m_NKM_UNIT_GRADE >= NKM_UNIT_GRADE.NUG_SR && (string.IsNullOrEmpty(m_banCandidateSearchString) || item.GetUnitName().Contains(m_banCandidateSearchString)))
			{
				m_unitCandidateList.Add(NKCUtil.MakeDummyUnit(item.m_UnitID, 100, 3));
			}
		}
	}

	private void UpdateLeaguePickUnitList()
	{
		m_unitCandidateList.Clear();
		foreach (NKMUnitData value in NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyUnit.Values)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(value.m_UnitID);
			if (nKMUnitTempletBase != null && nKMUnitTempletBase.CollectionEnableByTag && nKMUnitTempletBase.m_bContractable && NKMUnitManager.CanUnitUsedInDeck(nKMUnitTempletBase))
			{
				m_unitCandidateList.Add(value);
			}
		}
	}

	public void InvalidateSortData(NKM_UNIT_TYPE type)
	{
		if (m_bOpen && CurrentTargetUnitType == type)
		{
			m_dicUnitSortSystem.Remove(type);
			RefreshLoopScrollList(type, bResetPosition: true);
		}
		else
		{
			m_dicUnitSortSystem.Remove(type);
		}
	}

	public void Open(bool bAnimate, NKM_UNIT_TYPE targetType, NKCUnitSortSystem.UnitListOptions sortOptions, NKCUIDeckViewer.DeckViewerOption deckViewerOption)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		foreach (KeyValuePair<NKM_UNIT_TYPE, NKCUnitSortSystem> item in m_dicUnitSortSystem)
		{
			NKCUnitSortSystem value = item.Value;
			if (value != null)
			{
				value.SetEnableShowBan(NKCUtil.CheckPossibleShowBan(deckViewerOption.eDeckviewerMode));
				value.SetEnableShowUpUnit(NKCUtil.CheckPossibleShowUpUnit(deckViewerOption.eDeckviewerMode));
			}
		}
		if (!m_bOpen)
		{
			m_rtRoot.DOKill();
			if (bAnimate)
			{
				m_rtRoot.anchoredPosition = new Vector2(m_rtRoot.GetWidth() * 1.5f, 0f);
				m_rtRoot.DOAnchorPosX(0f, 0.4f).SetEase(Ease.OutCubic);
			}
			else
			{
				m_rtRoot.anchoredPosition = Vector2.zero;
			}
		}
		if (sortOptions.eDeckType != NKM_DECK_TYPE.NDT_NORMAL)
		{
			m_sortOptions.bHideDeckedUnit = false;
			m_OperatorSortOptions.SetBuildOption(false, BUILD_OPTIONS.HIDE_DECKED_UNIT);
		}
		m_bOpen = true;
		m_sortOptions = sortOptions;
		m_DeckViewerOptions = deckViewerOption;
		RefreshLoopScrollList(targetType, bResetPosition: true);
	}

	public void Open(bool bAnimate, NKM_UNIT_TYPE targetType, NKCOperatorSortSystem.OperatorListOptions sortOptions, NKCUIDeckViewer.DeckViewerOption deckViewerOption)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (!m_bOpen)
		{
			m_rtRoot.DOKill();
			if (bAnimate)
			{
				m_rtRoot.anchoredPosition = new Vector2(m_rtRoot.GetWidth() * 1.5f, 0f);
				m_rtRoot.DOAnchorPosX(0f, 0.4f).SetEase(Ease.OutCubic);
			}
			else
			{
				m_rtRoot.anchoredPosition = Vector2.zero;
			}
		}
		if (sortOptions.eDeckType != NKM_DECK_TYPE.NDT_NORMAL)
		{
			m_sortOptions.bHideDeckedUnit = false;
			m_OperatorSortOptions.SetBuildOption(false, BUILD_OPTIONS.HIDE_DECKED_UNIT);
		}
		m_bOpen = true;
		m_OperatorSortOptions = sortOptions;
		m_DeckViewerOptions = deckViewerOption;
		RefreshLoopScrollList(targetType, bResetPosition: true);
	}

	public void Close(bool bAnimate)
	{
		m_bOpen = false;
		Cleanup();
		dOnClose?.Invoke(CurrentTargetUnitType);
		CurrentTargetUnitType = NKM_UNIT_TYPE.NUT_INVALID;
		m_rtRoot.DOKill();
		if (bAnimate)
		{
			m_rtRoot.DOAnchorPosX(m_rtRoot.GetWidth() * 1.5f, 0.4f).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
		else
		{
			m_rtRoot.anchoredPosition = new Vector2(m_rtRoot.GetWidth() * 1.5f, 0f);
			base.gameObject.SetActive(value: false);
		}
	}

	public void Cleanup()
	{
		m_ssActive = null;
		m_OperatorSortSystem = null;
		m_SortUI?.ResetUI();
		m_dicUnitSortSystem.Clear();
	}

	public void OnExpandInventoryPopup()
	{
		_ = NKCScenManager.CurrentUserData().m_ArmyData.m_MaxUnitCount;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKM_INVENTORY_EXPAND_TYPE nKM_INVENTORY_EXPAND_TYPE = NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT;
		int count = 1;
		int resultCount;
		bool flag = !NKCAdManager.IsAdRewardInventory(nKM_INVENTORY_EXPAND_TYPE) || !NKMInventoryManager.CanExpandInventoryByAd(nKM_INVENTORY_EXPAND_TYPE, myUserData, count, out resultCount);
		if (!NKMInventoryManager.CanExpandInventory(nKM_INVENTORY_EXPAND_TYPE, myUserData, count, out resultCount) && flag)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_CANNOT_EXPAND_INVENTORY));
			return;
		}
		string expandDesc = NKCUtilString.GetExpandDesc(nKM_INVENTORY_EXPAND_TYPE);
		NKCPopupInventoryAdd.SliderInfo sliderInfo = new NKCPopupInventoryAdd.SliderInfo
		{
			increaseCount = 5,
			maxCount = 1100,
			currentCount = myUserData.m_ArmyData.m_MaxUnitCount,
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT
		};
		NKCPopupInventoryAdd.Instance.Open(NKCUtilString.GET_STRING_INVENTORY_UNIT, expandDesc, sliderInfo, 100, 101, delegate(int value)
		{
			NKCPacketSender.Send_NKMPacket_INVENTORY_EXPAND_REQ(NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT, value);
		});
	}

	private void OnSortChanged(bool bResetScroll)
	{
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			if (m_OperatorSortSystem != null)
			{
				m_OperatorSortOptions = m_OperatorSortSystem.Options;
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
		else if (m_ssActive != null)
		{
			m_sortOptions = m_ssActive.Options;
			m_LoopScrollRect.TotalCount = m_ssActive.SortedUnitList.Count;
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

	private void OnCloseBtn()
	{
		Close(bAnimate: true);
	}

	public void UpdateLoopScrollList(NKM_UNIT_TYPE eType, NKCUnitSortSystem.UnitListOptions options)
	{
		if (m_bOpen)
		{
			if (CurrentTargetUnitType != eType)
			{
				m_sortOptions = options;
			}
			else
			{
				m_sortOptions.setDuplicateUnitID = options.setDuplicateUnitID;
				m_sortOptions.setExcludeUnitID = options.setExcludeUnitID;
				m_sortOptions.setExcludeUnitUID = options.setExcludeUnitUID;
				m_OperatorSortOptions.setDuplicateOperatorID = options.setDuplicateUnitID;
				m_OperatorSortOptions.setExcludeOperatorID = options.setExcludeUnitID;
				m_OperatorSortOptions.setExcludeOperatorUID = options.setExcludeUnitUID;
			}
			m_dicUnitSortSystem.Remove(eType);
			RefreshLoopScrollList(eType, bResetPosition: false);
		}
	}

	public void UpdateLoopScrollList(NKM_UNIT_TYPE eType, NKCOperatorSortSystem.OperatorListOptions options)
	{
		if (m_bOpen)
		{
			if (CurrentTargetUnitType != eType)
			{
				m_OperatorSortOptions = options;
			}
			else
			{
				m_OperatorSortOptions.setDuplicateOperatorID = options.setDuplicateOperatorID;
				m_OperatorSortOptions.setExcludeOperatorID = options.setExcludeOperatorID;
				m_OperatorSortOptions.setExcludeOperatorUID = options.setExcludeOperatorUID;
			}
			m_dicUnitSortSystem.Remove(eType);
			RefreshLoopScrollList(eType, bResetPosition: false);
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
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			m_OperatorSortSystem = new NKCOperatorSort(NKCScenManager.CurrentUserData(), m_OperatorSortOptions);
			if (m_SortUI != null)
			{
				m_SortUI.RegisterOperatorSort(m_OperatorSortSystem);
			}
		}
		else
		{
			m_ssActive = GetUnitSortSystem(targetType);
			if (m_SortUI != null)
			{
				m_SortUI.RegisterUnitSort(m_ssActive);
			}
			MoveLastGlobalBanUnits(ref m_ssActive);
		}
		if (flag)
		{
			if (m_SortUI != null)
			{
				if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
				{
					m_SortUI.RegisterCategories(NKCPopupFilterUnit.MakeOprDefaultFilterOption(NKM_UNIT_TYPE.NUT_OPERATOR, NKCPopupFilterUnit.FILTER_OPEN_TYPE.NORMAL), NKCPopupSort.MakeDefaultOprSortSet(NKM_UNIT_TYPE.NUT_OPERATOR, bIsCollection: false), bFavoriteFilterActive: false);
				}
				else
				{
					m_SortUI.RegisterCategories(GetFilterCategorySet(), GetSortCategorySet(), bFavoriteFilterActive: false);
				}
				bool bUseFavorite = m_DeckViewerOptions.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.LeaguePvPMain;
				m_SortUI.ResetUI(bUseFavorite);
			}
			m_sortOptions.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			m_OperatorSortOptions.setFilterOption = new HashSet<NKCOperatorSortSystem.eFilterOption>();
			switch (targetType)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				m_LoopScrollRect.ContentConstraintCount = GetContentContraintCount();
				m_GridLayoutGroup.constraintCount = GetContentContraintCount();
				m_GridLayoutGroup.cellSize = m_vUnitSlotSize;
				m_GridLayoutGroup.spacing = m_vUnitSlotSpacing;
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
				m_LoopScrollRect.ContentConstraintCount = 1;
				m_GridLayoutGroup.constraintCount = 1;
				m_GridLayoutGroup.cellSize = m_vShipSlotSize;
				m_GridLayoutGroup.spacing = m_vShipSlotSpacing;
				break;
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				m_LoopScrollRect.ContentConstraintCount = 3;
				m_GridLayoutGroup.constraintCount = 3;
				m_GridLayoutGroup.cellSize = m_vOperatorSlotSize;
				m_GridLayoutGroup.spacing = m_vOperatorSlotSpacing;
				break;
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(m_LoopScrollRect.GetComponent<RectTransform>());
			StartCoroutine(DelayedOnSortChanged(flag || bResetPosition));
		}
		else
		{
			OnSortChanged(flag || bResetPosition);
		}
	}

	private IEnumerator DelayedOnSortChanged(bool bResetScroll)
	{
		yield return new WaitForEndOfFrame();
		m_LoopScrollRect.ResetContentSpacing();
		m_LoopScrollRect.PrepareCells();
		yield return new WaitForEndOfFrame();
		OnSortChanged(bResetScroll);
	}

	private int GetContentContraintCount()
	{
		if (m_DeckViewerOptions.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.LeaguePvPMain)
		{
			return 3;
		}
		return 7;
	}

	private void MoveLastGlobalBanUnits(ref NKCUnitSortSystem unitSortSystem)
	{
		List<int> list = new List<int>();
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			list = NKCBanManager.GetGlobalBanUnitList(NKM_UNIT_TYPE.NUT_NORMAL);
		}
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_SHIP)
		{
			list = NKCBanManager.GetGlobalBanUnitList(NKM_UNIT_TYPE.NUT_SHIP);
		}
		if (list.Count <= 0)
		{
			return;
		}
		List<NKMUnitData> list2 = new List<NKMUnitData>();
		for (int i = 0; i < unitSortSystem.SortedUnitList.Count; i++)
		{
			NKMUnitData nKMUnitData = unitSortSystem.SortedUnitList[i];
			if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_NORMAL && list.Contains(nKMUnitData.m_UnitID))
			{
				list2.Add(nKMUnitData);
				unitSortSystem.SortedUnitList.RemoveAt(i);
				i--;
			}
			else if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_SHIP && list.Contains(nKMUnitData.GetUnitTempletBase().m_ShipGroupID))
			{
				list2.Add(nKMUnitData);
				unitSortSystem.SortedUnitList.RemoveAt(i);
				i--;
			}
		}
		unitSortSystem.SortedUnitList.AddRange(list2);
	}

	private HashSet<NKCUnitSortSystem.eFilterCategory> GetFilterCategorySet()
	{
		switch (CurrentTargetUnitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			return NKCPopupFilterUnit.MakeDefaultFilterOption(NKM_UNIT_TYPE.NUT_NORMAL, NKCPopupFilterUnit.FILTER_OPEN_TYPE.NORMAL);
		case NKM_UNIT_TYPE.NUT_SHIP:
			return NKCPopupFilterUnit.MakeDefaultFilterOption(NKM_UNIT_TYPE.NUT_SHIP, NKCPopupFilterUnit.FILTER_OPEN_TYPE.NORMAL);
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			Debug.LogError($"여기서 {CurrentTargetUnitType} 을 사용하는게 맞는지 확인 필요함. 오퍼레이터는 이쪽으로 들어오면 안됨.");
			return new HashSet<NKCUnitSortSystem.eFilterCategory>();
		default:
			Debug.LogError($"여기서 {CurrentTargetUnitType} 을 사용하는게 맞는지 확인 필요함. 사용할 경우 코드 추가");
			return new HashSet<NKCUnitSortSystem.eFilterCategory>();
		}
	}

	private HashSet<NKCUnitSortSystem.eSortCategory> GetSortCategorySet()
	{
		switch (CurrentTargetUnitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			return NKCPopupSort.MakeDefaultSortSet(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
		case NKM_UNIT_TYPE.NUT_SHIP:
			return NKCPopupSort.MakeDefaultSortSet(NKM_UNIT_TYPE.NUT_SHIP, bIsCollection: false);
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			Debug.LogError($"여기서 {CurrentTargetUnitType} 을 사용하는게 맞는지 확인 필요함. 오퍼레이터는 이쪽으로 들어오면 안됨.");
			return new HashSet<NKCUnitSortSystem.eSortCategory>();
		default:
			return new HashSet<NKCUnitSortSystem.eSortCategory>();
		}
	}

	private void OnSlotSelected(NKMUnitData selectedUnit, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		long targetUID = selectedUnit?.m_UnitUID ?? 0;
		OnSlotSelected(targetUID, unitTempletBase, selectedUnitDeckIndex, unitSlotState, unitSlotSelectState);
	}

	private void OnSlotSelected(NKMOperator selectedOperator, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		long targetUID = selectedOperator?.uid ?? 0;
		OnSlotSelected(targetUID, unitTempletBase, selectedUnitDeckIndex, unitSlotState, unitSlotSelectState);
	}

	private void OnSlotSelected(long targetUID, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		switch (unitSlotState)
		{
		case NKCUnitSortSystem.eUnitState.NONE:
		case NKCUnitSortSystem.eUnitState.SEIZURE:
		case NKCUnitSortSystem.eUnitState.LOBBY_UNIT:
			if (m_sortOptions.eDeckType == NKM_DECK_TYPE.NDT_NORMAL && ((CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_NORMAL && m_ssActive.GetDeckIndexCache(targetUID, bTargetDecktypeOnly: true) != NKMDeckIndex.None) || (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR && m_OperatorSortSystem.GetDeckIndexCache(targetUID, bTargetDecktypeOnly: true) != NKMDeckIndex.None)))
			{
				OpenUnitDeckChangeWarning(targetUID);
			}
			else
			{
				ConfirmSlotSelected(targetUID);
			}
			break;
		}
	}

	private void ConfirmSlotSelected(long UID)
	{
		NKMDeckIndex deckIndex = ((CurrentTargetUnitType != NKM_UNIT_TYPE.NUT_OPERATOR) ? m_ssActive.GetDeckIndexCache(UID, bTargetDecktypeOnly: true) : m_OperatorSortSystem.GetDeckIndexCache(UID, bTargetDecktypeOnly: true));
		dOnDeckUnitChangeClicked?.Invoke(deckIndex, UID, CurrentTargetUnitType);
	}

	public NKCUIUnitSelectListSlotBase FindSlotFromCurrentList(NKM_UNIT_TYPE unitType, long unitUID)
	{
		if (unitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMOperatorData != null && x.NKMOperatorData.uid == unitUID);
		}
		return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMUnitData != null && x.NKMUnitData.m_UnitUID == unitUID);
	}

	public NKCUIUnitSelectListSlotBase FindSlotFromCurrentList(int unitID)
	{
		return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMUnitTempletBase != null && x.NKMUnitTempletBase.m_UnitID == unitID);
	}

	private void OpenUnitDeckChangeWarning(long UID)
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_DECK_CHANGE_UNIT_WARNING, delegate
		{
			ConfirmSlotSelected(UID);
		});
	}
}
