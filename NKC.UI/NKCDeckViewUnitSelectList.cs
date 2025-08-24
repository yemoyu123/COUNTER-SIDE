using System.Collections.Generic;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

[RequireComponent(typeof(RectTransform))]
public class NKCDeckViewUnitSelectList : MonoBehaviour
{
	public enum SlotType
	{
		Normal,
		Empty,
		ClearAll,
		AutoComplete
	}

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

	[Header("보유 유닛 카운트")]
	public Text m_txtUnitCout;

	public NKCUIComStateButton m_sbtnUnitListAdd;

	[Header("그 외")]
	public NKCUIComToggle m_tglHideDeckedUnit;

	public Animator m_AniHideDeckedUnit;

	public NKCUIComStateButton m_sbtnFinish;

	private bool m_bOpen;

	private OnDeckUnitChangeClicked dOnDeckUnitChangeClicked;

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

	public bool IsOpen => m_bOpen;

	public NKM_UNIT_TYPE CurrentTargetUnitType { get; private set; }

	public NKCUnitSortSystem.UnitListOptions SortOptions => m_sortOptions;

	public NKCOperatorSortSystem.OperatorListOptions SortOperatorOptions => m_OperatorSortOptions;

	public void Init(NKCUIDeckViewer deckView, OnDeckUnitChangeClicked onDeckUnitChangeClicked, OnClose onClose, UnityAction onClearDeck, UnityAction onAutoCompleteDeck)
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
		if (m_tglHideDeckedUnit != null)
		{
			m_tglHideDeckedUnit.OnValueChanged.RemoveAllListeners();
			m_tglHideDeckedUnit.OnValueChanged.AddListener(ToggleHideDeckedUnit);
		}
		if (m_sbtnUnitListAdd != null)
		{
			m_sbtnUnitListAdd.PointerClick.RemoveAllListeners();
			m_sbtnUnitListAdd.PointerClick.AddListener(OnExpandInventoryPopup);
		}
		dOnDeckUnitChangeClicked = onDeckUnitChangeClicked;
		dOnClose = onClose;
		dOnAutoCompleteDeck = onAutoCompleteDeck;
		dOnClearDeck = onClearDeck;
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
			ProvideSlotDataDefault(component, idx);
		}
	}

	private void ProvideSlotDataDefault(NKCUIUnitSelectListSlotBase slot, int idx)
	{
		switch (GetSlottypeByIndex(idx))
		{
		case SlotType.Empty:
			slot.SetEmpty(bEnableLayoutElement: true, OnSlotSelected, OnSlotSelected);
			return;
		case SlotType.ClearAll:
			slot.SetMode(NKCUIUnitSelectListSlotBase.eUnitSlotMode.ClearAll, bEnableLayoutElement: true, OnSelectClearDeck);
			return;
		case SlotType.AutoComplete:
			slot.SetMode(NKCUIUnitSelectListSlotBase.eUnitSlotMode.AutoComplete, bEnableLayoutElement: true, OnSelectAutoCompleteDeck);
			return;
		}
		idx -= GetExtraSlotCount();
		slot.SetEnableShowBan(NKCUtil.CheckPossibleShowBan(m_DeckViewerOptions.eDeckviewerMode));
		slot.SetEnableShowUpUnit(NKCUtil.CheckPossibleShowUpUnit(m_DeckViewerOptions.eDeckviewerMode));
		long num = 0L;
		bool cityLeaderMark = false;
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			if (m_OperatorSortSystem.SortedOperatorList.Count <= idx)
			{
				return;
			}
			NKMOperator nKMOperator = m_OperatorSortSystem.SortedOperatorList[idx];
			num = nKMOperator.uid;
			NKMDeckIndex deckIndexCache = m_OperatorSortSystem.GetDeckIndexCache(num, bTargetDecktypeOnly: true);
			slot.SetData(nKMOperator, deckIndexCache, bEnableLayoutElement: true, OnSlotSelected);
			slot.SetSlotState(m_OperatorSortSystem.GetUnitSlotState(num));
			slot.SetTouchHoldEvent(ShowOperatorInfo);
		}
		else
		{
			if (m_ssActive.SortedUnitList.Count <= idx)
			{
				return;
			}
			NKMUnitData nKMUnitData = m_ssActive.SortedUnitList[idx];
			num = nKMUnitData.m_UnitUID;
			cityLeaderMark = m_ssActive.GetCityStateCache(num) != NKMWorldMapManager.WorldMapLeaderState.None;
			NKMDeckIndex deckIndexCacheByOption = m_ssActive.GetDeckIndexCacheByOption(num, bTargetDeckTypeOnly: true);
			slot.SetData(nKMUnitData, deckIndexCacheByOption, bEnableLayoutElement: true, OnSlotSelected);
			slot.SetSlotState(m_ssActive.GetUnitSlotState(num));
			slot.SetTouchHoldEvent(ShowUnitInfo);
		}
		slot.SetCityLeaderMark(cityLeaderMark);
		if (m_DeckViewerOptions.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.WorldMapMissionDeckSelect)
		{
			NKCDeckViewUnitSelectListSlot nKCDeckViewUnitSelectListSlot = slot as NKCDeckViewUnitSelectListSlot;
			if (nKCDeckViewUnitSelectListSlot != null)
			{
				NKMWorldMapManager.WorldMapLeaderState unitWorldMapLeaderState = NKMWorldMapManager.GetUnitWorldMapLeaderState(NKCScenManager.CurrentUserData(), num, m_DeckViewerOptions.WorldMapMissionCityID);
				nKCDeckViewUnitSelectListSlot.SetCityLeaderTag(unitWorldMapLeaderState);
			}
		}
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
		return m_ssActive.SortedUnitList.Count + GetExtraSlotCount();
	}

	private int GetExtraSlotCount()
	{
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_SHIP || CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			return 1;
		}
		return 3;
	}

	private SlotType GetSlottypeByIndex(int index)
	{
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_SHIP || CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			if (GetExtraSlotCount() > 0 && index == 0)
			{
				return SlotType.Empty;
			}
			return SlotType.Normal;
		}
		if (GetExtraSlotCount() > 0)
		{
			if (index == 0)
			{
				return SlotType.Empty;
			}
			index--;
		}
		if (GetExtraSlotCount() > 1)
		{
			if (index == 0)
			{
				return SlotType.ClearAll;
			}
			index--;
		}
		if (GetExtraSlotCount() > 2 && index == 0)
		{
			return SlotType.AutoComplete;
		}
		return SlotType.Normal;
	}

	private int GetIndexBySlottype(SlotType type)
	{
		int num = 0;
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_SHIP)
		{
			if (type == SlotType.Empty)
			{
				return num;
			}
			return num + 1;
		}
		if (type == SlotType.Empty)
		{
			return num;
		}
		num++;
		if (type == SlotType.ClearAll)
		{
			return num;
		}
		num++;
		if (type == SlotType.AutoComplete)
		{
			return num;
		}
		return num + 1;
	}

	private void OnSelectClearDeck(NKMUnitData selectedUnit, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		dOnClearDeck?.Invoke();
	}

	private void OnSelectAutoCompleteDeck(NKMUnitData selectedUnit, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		dOnAutoCompleteDeck?.Invoke();
	}

	private NKCUnitSortSystem GetUnitSortSystem(NKM_UNIT_TYPE type)
	{
		if (m_dicUnitSortSystem.ContainsKey(type) && m_dicUnitSortSystem[type] != null)
		{
			return m_dicUnitSortSystem[type];
		}
		NKCUnitSortSystem nKCUnitSortSystem = ((type != NKM_UNIT_TYPE.NUT_NORMAL && type == NKM_UNIT_TYPE.NUT_SHIP) ? ((NKCUnitSortSystem)new NKCShipSort(NKCScenManager.CurrentUserData(), m_sortOptions)) : ((NKCUnitSortSystem)new NKCUnitSort(NKCScenManager.CurrentUserData(), m_sortOptions)));
		nKCUnitSortSystem.SetEnableShowBan(NKCUtil.CheckPossibleShowBan(m_DeckViewerOptions.eDeckviewerMode));
		nKCUnitSortSystem.SetEnableShowUpUnit(NKCUtil.CheckPossibleShowUpUnit(m_DeckViewerOptions.eDeckviewerMode));
		m_dicUnitSortSystem[type] = nKCUnitSortSystem;
		return nKCUnitSortSystem;
	}

	private NKCUnitSortSystem GetLocalUnitSortSystem(NKM_UNIT_TYPE type)
	{
		if (m_dicUnitSortSystem.ContainsKey(type) && m_dicUnitSortSystem[type] != null)
		{
			return m_dicUnitSortSystem[type];
		}
		NKCUnitSortSystem nKCUnitSortSystem = ((type != NKM_UNIT_TYPE.NUT_NORMAL && type == NKM_UNIT_TYPE.NUT_SHIP) ? ((NKCUnitSortSystem)new NKCShipSort(NKCScenManager.CurrentUserData(), m_sortOptions, useLocal: true)) : ((NKCUnitSortSystem)new NKCUnitSort(NKCScenManager.CurrentUserData(), m_sortOptions, useLocal: true)));
		nKCUnitSortSystem.SetEnableShowBan(NKCUtil.CheckPossibleShowBan(m_DeckViewerOptions.eDeckviewerMode));
		nKCUnitSortSystem.SetEnableShowUpUnit(NKCUtil.CheckPossibleShowUpUnit(m_DeckViewerOptions.eDeckviewerMode));
		m_dicUnitSortSystem[type] = nKCUnitSortSystem;
		return nKCUnitSortSystem;
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
		if (sortOptions.eDeckType == NKM_DECK_TYPE.NDT_NORMAL)
		{
			NKCUtil.SetGameobjectActive(m_tglHideDeckedUnit, bValue: true);
			if (m_tglHideDeckedUnit != null)
			{
				m_tglHideDeckedUnit.Select(sortOptions.bHideDeckedUnit, bForce: true);
			}
			if (m_AniHideDeckedUnit != null)
			{
				m_AniHideDeckedUnit.SetTrigger("Enable");
			}
		}
		else
		{
			m_sortOptions.bHideDeckedUnit = false;
			m_OperatorSortOptions.SetBuildOption(false, BUILD_OPTIONS.HIDE_DECKED_UNIT);
			NKCUtil.SetGameobjectActive(m_tglHideDeckedUnit, bValue: false);
			if (m_AniHideDeckedUnit != null)
			{
				m_AniHideDeckedUnit.SetTrigger("Disable");
			}
		}
		m_bOpen = true;
		m_sortOptions = sortOptions;
		m_DeckViewerOptions = deckViewerOption;
		RefreshLoopScrollList(targetType, bResetPosition: true);
		UpdateUnitCount();
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
		if (sortOptions.eDeckType == NKM_DECK_TYPE.NDT_NORMAL)
		{
			NKCUtil.SetGameobjectActive(m_tglHideDeckedUnit, bValue: true);
			if (m_tglHideDeckedUnit != null)
			{
				m_tglHideDeckedUnit.Select(sortOptions.IsHasBuildOption(BUILD_OPTIONS.HIDE_DECKED_UNIT), bForce: true);
			}
			if (m_AniHideDeckedUnit != null)
			{
				m_AniHideDeckedUnit.SetTrigger("Enable");
			}
		}
		else
		{
			m_sortOptions.bHideDeckedUnit = false;
			m_OperatorSortOptions.SetBuildOption(false, BUILD_OPTIONS.HIDE_DECKED_UNIT);
			NKCUtil.SetGameobjectActive(m_tglHideDeckedUnit, bValue: false);
			if (m_AniHideDeckedUnit != null)
			{
				m_AniHideDeckedUnit.SetTrigger("Disable");
			}
		}
		m_bOpen = true;
		m_OperatorSortOptions = sortOptions;
		m_DeckViewerOptions = deckViewerOption;
		RefreshLoopScrollList(targetType, bResetPosition: true);
		UpdateUnitCount();
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

	public void UpdateUnitCount()
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		int currentUnitCount = armyData.GetCurrentUnitCount();
		int maxUnitCount = armyData.m_MaxUnitCount;
		if (m_txtUnitCout != null)
		{
			m_txtUnitCout.text = $"{currentUnitCount}/{maxUnitCount}";
		}
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
				m_LoopScrollRect.TotalCount = m_OperatorSortSystem.SortedOperatorList.Count + GetExtraSlotCount();
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
			m_LoopScrollRect.TotalCount = m_ssActive.SortedUnitList.Count + GetExtraSlotCount();
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

	private void ToggleHideDeckedUnit(bool value)
	{
		m_sortOptions.bHideDeckedUnit = value;
		m_OperatorSortOptions.SetBuildOption(value, BUILD_OPTIONS.HIDE_DECKED_UNIT);
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			m_OperatorSortSystem.FilterList(m_OperatorSortSystem.FilterSet, value);
		}
		else
		{
			m_ssActive.FilterList(m_ssActive.FilterSet, value);
		}
		RefreshLoopScrollList(CurrentTargetUnitType, bResetPosition: true);
	}

	private void OnCloseBtn()
	{
		Close(bAnimate: true);
	}

	public void UpdateLoopScrollList(NKM_UNIT_TYPE eType, NKCUnitSortSystem.UnitListOptions options)
	{
		if (!m_bOpen)
		{
			return;
		}
		if (CurrentTargetUnitType != eType || m_sortOptions.eDeckType != options.eDeckType)
		{
			m_sortOptions = options;
			if (m_sortOptions.eDeckType == NKM_DECK_TYPE.NDT_NORMAL)
			{
				NKCUtil.SetGameobjectActive(m_tglHideDeckedUnit, bValue: true);
				if (m_tglHideDeckedUnit != null)
				{
					m_tglHideDeckedUnit.Select(m_sortOptions.bHideDeckedUnit, bForce: true);
				}
				if (m_AniHideDeckedUnit != null)
				{
					m_AniHideDeckedUnit.SetTrigger("Enable");
				}
			}
			else
			{
				m_sortOptions.bHideDeckedUnit = false;
				m_OperatorSortOptions.SetBuildOption(false, BUILD_OPTIONS.HIDE_DECKED_UNIT);
				NKCUtil.SetGameobjectActive(m_tglHideDeckedUnit, bValue: false);
				if (m_AniHideDeckedUnit != null)
				{
					m_AniHideDeckedUnit.SetTrigger("Disable");
				}
			}
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

	public void UpdateLoopScrollList(NKM_UNIT_TYPE eType, NKCOperatorSortSystem.OperatorListOptions options)
	{
		if (!m_bOpen)
		{
			return;
		}
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
		if (m_sortOptions.eDeckType == NKM_DECK_TYPE.NDT_NORMAL)
		{
			NKCUtil.SetGameobjectActive(m_tglHideDeckedUnit, bValue: true);
			if (m_tglHideDeckedUnit != null)
			{
				m_tglHideDeckedUnit.Select(m_sortOptions.bHideDeckedUnit, bForce: true);
			}
			if (m_AniHideDeckedUnit != null)
			{
				m_AniHideDeckedUnit.SetTrigger("Enable");
			}
		}
		else
		{
			m_sortOptions.bHideDeckedUnit = false;
			m_OperatorSortOptions.SetBuildOption(false, BUILD_OPTIONS.HIDE_DECKED_UNIT);
			NKCUtil.SetGameobjectActive(m_tglHideDeckedUnit, bValue: false);
			if (m_AniHideDeckedUnit != null)
			{
				m_AniHideDeckedUnit.SetTrigger("Disable");
			}
		}
		m_dicUnitSortSystem.Remove(eType);
		RefreshLoopScrollList(eType, bResetPosition: false);
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
		if (m_tglHideDeckedUnit != null)
		{
			if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
			{
				m_tglHideDeckedUnit.Select(m_OperatorSortOptions.IsHasBuildOption(BUILD_OPTIONS.HIDE_DECKED_UNIT), bForce: true);
			}
			else
			{
				m_tglHideDeckedUnit.Select(m_sortOptions.bHideDeckedUnit, bForce: true);
			}
		}
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			if (m_DeckViewerOptions.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.PrepareLocalDeck)
			{
				m_OperatorSortSystem = new NKCOperatorSort(NKCScenManager.CurrentUserData(), m_OperatorSortOptions, local: true);
			}
			else
			{
				m_OperatorSortSystem = new NKCOperatorSort(NKCScenManager.CurrentUserData(), m_OperatorSortOptions);
			}
			if (m_SortUI != null)
			{
				m_SortUI.RegisterOperatorSort(m_OperatorSortSystem);
			}
		}
		else
		{
			if (m_DeckViewerOptions.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.PrepareLocalDeck)
			{
				m_ssActive = GetLocalUnitSortSystem(targetType);
			}
			else
			{
				m_ssActive = GetUnitSortSystem(targetType);
			}
			if (m_SortUI != null)
			{
				m_SortUI.RegisterUnitSort(m_ssActive);
			}
		}
		if (flag)
		{
			if (m_SortUI != null)
			{
				if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
				{
					m_SortUI.RegisterCategories(GetOprFilterCategorySet(), GetOprSortCategorySet(), bFavoriteFilterActive: false);
				}
				else
				{
					m_SortUI.RegisterCategories(GetFilterCategorySet(), GetSortCategorySet(), bFavoriteFilterActive: false);
				}
				m_SortUI.ResetUI(CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_NORMAL);
			}
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
			m_LoopScrollRect.ResetContentSpacing();
			m_LoopScrollRect.PrepareCells();
		}
		OnSortChanged(flag || bResetPosition);
	}

	private HashSet<NKCUnitSortSystem.eFilterCategory> GetFilterCategorySet()
	{
		switch (CurrentTargetUnitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			return NKCPopupFilterUnit.MakeDefaultFilterOption(NKM_UNIT_TYPE.NUT_NORMAL, NKCPopupFilterUnit.FILTER_OPEN_TYPE.NORMAL);
		case NKM_UNIT_TYPE.NUT_SHIP:
			return NKCPopupFilterUnit.MakeDefaultFilterOption(NKM_UNIT_TYPE.NUT_SHIP, NKCPopupFilterUnit.FILTER_OPEN_TYPE.NORMAL);
		default:
			Debug.LogError($"여기서 {CurrentTargetUnitType} 을 사용하는게 맞는지 확인 필요함. 사용할 경우 코드 추가");
			return new HashSet<NKCUnitSortSystem.eFilterCategory>();
		}
	}

	private HashSet<NKCOperatorSortSystem.eFilterCategory> GetOprFilterCategorySet()
	{
		return NKCPopupFilterOperator.MakeDefaultFilterCategory(NKCPopupFilterOperator.FILTER_OPEN_TYPE.NORMAL);
	}

	private HashSet<NKCUnitSortSystem.eSortCategory> GetSortCategorySet()
	{
		return CurrentTargetUnitType switch
		{
			NKM_UNIT_TYPE.NUT_NORMAL => NKCPopupSort.MakeDefaultSortSet(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false), 
			NKM_UNIT_TYPE.NUT_SHIP => NKCPopupSort.MakeDefaultSortSet(NKM_UNIT_TYPE.NUT_SHIP, bIsCollection: false), 
			_ => new HashSet<NKCUnitSortSystem.eSortCategory>(), 
		};
	}

	private HashSet<NKCOperatorSortSystem.eSortCategory> GetOprSortCategorySet()
	{
		NKM_UNIT_TYPE currentTargetUnitType = CurrentTargetUnitType;
		if ((uint)(currentTargetUnitType - 2) <= 1u || currentTargetUnitType != NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			return new HashSet<NKCOperatorSortSystem.eSortCategory>();
		}
		return NKCPopupSort.MakeDefaultOprSortSet(NKM_UNIT_TYPE.NUT_OPERATOR, bIsCollection: false);
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
		default:
			return;
		case NKCUnitSortSystem.eUnitState.DUPLICATE:
			if (m_DeckViewerOptions.eDeckviewerMode != NKCUIDeckViewer.DeckViewerMode.PrepareLocalDeck)
			{
				return;
			}
			break;
		case NKCUnitSortSystem.eUnitState.NONE:
		case NKCUnitSortSystem.eUnitState.SEIZURE:
		case NKCUnitSortSystem.eUnitState.LOBBY_UNIT:
			break;
		}
		if (m_sortOptions.eDeckType == NKM_DECK_TYPE.NDT_NORMAL && ((CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_NORMAL && m_ssActive.GetDeckIndexCache(targetUID, bTargetDecktypeOnly: true) != NKMDeckIndex.None) || (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR && m_OperatorSortSystem.GetDeckIndexCache(targetUID, bTargetDecktypeOnly: true) != NKMDeckIndex.None)))
		{
			OpenUnitDeckChangeWarning(targetUID);
		}
		else if (m_DeckViewerOptions.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.PrepareLocalDeck && IsLocalDeckedUnitId(CurrentTargetUnitType, unitTempletBase) && unitSlotState == NKCUnitSortSystem.eUnitState.DUPLICATE)
		{
			OpenUnitDeckChangeWarning(targetUID);
		}
		else
		{
			ConfirmSlotSelected(targetUID);
		}
	}

	private void ConfirmSlotSelected(long UID)
	{
		NKMDeckIndex deckIndex = ((CurrentTargetUnitType != NKM_UNIT_TYPE.NUT_OPERATOR) ? m_ssActive.GetDeckIndexCache(UID, bTargetDecktypeOnly: true) : m_OperatorSortSystem.GetDeckIndexCache(UID, bTargetDecktypeOnly: true));
		dOnDeckUnitChangeClicked?.Invoke(deckIndex, UID, CurrentTargetUnitType);
	}

	public void UpdateSlot(long uid, NKMUnitData unitData)
	{
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(unitData);
		if (nKCUIUnitSelectListSlotBase != null)
		{
			NKMDeckIndex deckIndex = ((m_ssActive == null) ? NKMDeckIndex.None : m_ssActive.GetDeckIndexCacheByOption(unitData.m_UnitUID, bTargetDeckTypeOnly: true));
			nKCUIUnitSelectListSlotBase.SetData(unitData, deckIndex, bEnableLayoutElement: true, OnSlotSelected);
		}
	}

	public void UpdateSlot(long uid, NKMOperator operatorData)
	{
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(operatorData);
		if (nKCUIUnitSelectListSlotBase != null)
		{
			NKMDeckIndex deckIndex = ((m_OperatorSortSystem == null) ? NKMDeckIndex.None : m_OperatorSortSystem.GetDeckIndexCache(operatorData.uid, bTargetDecktypeOnly: true));
			nKCUIUnitSelectListSlotBase.SetData(operatorData, deckIndex, bEnableLayoutElement: true, OnSlotSelected);
		}
	}

	private NKCUIUnitSelectListSlotBase FindSlotFromCurrentList(NKMUnitData unitData)
	{
		return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMUnitData != null && x.NKMUnitData.m_UnitUID == unitData.m_UnitUID);
	}

	private NKCUIUnitSelectListSlotBase FindSlotFromCurrentList(NKMOperator operatorData)
	{
		return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMOperatorData != null && x.NKMOperatorData.uid == operatorData.uid);
	}

	public NKCUIUnitSelectListSlotBase FindSlotFromCurrentList(NKM_UNIT_TYPE unitType, long unitUID)
	{
		if (unitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMOperatorData != null && x.NKMOperatorData.uid == unitUID);
		}
		return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMUnitData != null && x.NKMUnitData.m_UnitUID == unitUID);
	}

	private void OpenUnitDeckChangeWarning(long UID)
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_DECK_CHANGE_UNIT_WARNING, delegate
		{
			ConfirmSlotSelected(UID);
		});
	}

	private bool IsLocalDeckedUnitId(NKM_UNIT_TYPE unitType, NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase == null)
		{
			return false;
		}
		bool result = false;
		if (unitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			if (m_OperatorSortSystem == null)
			{
				return result;
			}
			result = m_OperatorSortSystem.IsDeckedOperatorId(unitTempletBase.m_UnitID);
			if (!result && unitTempletBase.m_BaseUnitID > 0)
			{
				result = m_OperatorSortSystem.IsDeckedOperatorId(unitTempletBase.m_BaseUnitID);
			}
		}
		else
		{
			if (m_ssActive == null)
			{
				return result;
			}
			result = m_ssActive.IsDeckedUnitId(unitTempletBase.m_NKM_UNIT_TYPE, unitTempletBase.m_UnitID);
			if (!result && unitTempletBase.m_BaseUnitID > 0)
			{
				result = m_ssActive.IsDeckedUnitId(unitTempletBase.m_NKM_UNIT_TYPE, unitTempletBase.m_BaseUnitID);
			}
		}
		return result;
	}

	public NKCUIUnitSelectListSlotBase GetAndScrollToTargetUnitSlot(int unitID)
	{
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = null;
		if (CurrentTargetUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			int num = m_OperatorSortSystem.SortedOperatorList.FindIndex((NKMOperator x) => x.id == unitID);
			if (num < 0)
			{
				Debug.LogError("Target unit not found!!");
				return null;
			}
			NKMOperator operatorData = m_OperatorSortSystem.SortedOperatorList[num];
			num += GetExtraSlotCount();
			m_LoopScrollRect.SetIndexPosition(num);
			return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMOperatorData != null && x.NKMOperatorData.uid == operatorData.uid);
		}
		int num2 = m_ssActive.SortedUnitList.FindIndex((NKMUnitData x) => x.m_UnitID == unitID);
		if (num2 < 0)
		{
			Debug.LogError("Target unit not found!!");
			return null;
		}
		NKMUnitData unitData = m_ssActive.SortedUnitList[num2];
		num2 += GetExtraSlotCount();
		m_LoopScrollRect.SetIndexPosition(num2);
		return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMUnitData != null && x.NKMUnitData.m_UnitUID == unitData.m_UnitUID);
	}

	public NKCUIUnitSelectListSlotBase GetAndScrollSlotBySlotType(SlotType type)
	{
		int indexBySlottype = GetIndexBySlottype(type);
		m_LoopScrollRect.SetIndexPosition(indexBySlottype);
		NKCUIUnitSelectListSlotBase result = null;
		if (indexBySlottype < m_lstVisibleSlot.Count)
		{
			result = m_lstVisibleSlot[indexBySlottype];
		}
		return result;
	}
}
