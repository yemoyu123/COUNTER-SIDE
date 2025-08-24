using System.Collections.Generic;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIScout : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_personnel";

	public const string UI_ASSET_NAME = "NKM_UI_PERSONNEL_SCOUT";

	private static NKCUIScout m_Instance;

	[Header("사장실 공통")]
	public NKCUIPersonnelShortCutMenu m_NKCUIPersonnelShortCutMenu;

	public NKCUICharInfoSummary m_unitInfoSummary;

	public NKCUICharacterView m_CharView;

	[Header("오른쪽 목록")]
	public LoopScrollRect m_LoopScrollRect;

	public NKCUIScoutSelectListSlot m_pfbScoutSlot;

	public NKCUIComUnitSortOptions m_SortOptions;

	public RectTransform m_rtSlotPool;

	[Header("스카우트 관련")]
	public NKCUIScoutUnitPiece m_UnitPiece;

	public GameObject m_objUnitNotSelected;

	public GameObject m_objUnitNotInCollection;

	public NKCUIComStateButton m_csbtnScout;

	[Header("도감 버튼")]
	public NKCUIComStateButton m_csbtnCollection;

	private NKCUnitSortSystem m_UnitSortSystem;

	private NKMPieceTemplet m_SelectedPieceTemplet;

	private bool m_bCellPrepared;

	private readonly HashSet<NKCUnitSortSystem.eSortCategory> setSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>
	{
		NKCUnitSortSystem.eSortCategory.IDX,
		NKCUnitSortSystem.eSortCategory.Rarity,
		NKCUnitSortSystem.eSortCategory.UnitSummonCost,
		NKCUnitSortSystem.eSortCategory.ScoutProgress
	};

	private readonly HashSet<NKCUnitSortSystem.eFilterCategory> setFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
	{
		NKCUnitSortSystem.eFilterCategory.UnitType,
		NKCUnitSortSystem.eFilterCategory.UnitRole,
		NKCUnitSortSystem.eFilterCategory.UnitTargetType,
		NKCUnitSortSystem.eFilterCategory.Rarity,
		NKCUnitSortSystem.eFilterCategory.Cost,
		NKCUnitSortSystem.eFilterCategory.Collected,
		NKCUnitSortSystem.eFilterCategory.Scout
	};

	private Dictionary<int, NKMUnitData> m_dicFakeUnit = new Dictionary<int, NKMUnitData>();

	private Stack<RectTransform> m_stkObj = new Stack<RectTransform>();

	private const string SCOUT_ALARM_OFF_KEY = "SCOUT_ALARM_OFF_{0}";

	public static NKCUIScout Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIScout>("ab_ui_nkm_ui_personnel", "NKM_UI_PERSONNEL_SCOUT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIScout>();
				m_Instance.Init();
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

	public override string GuideTempletID => "ARTICLE_SYSTEM_SCOUT";

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCStringTable.GetString("SI_PF_PERSONNEL_SCOUT_TEXT");

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
		m_CharView.CleanUp();
		base.gameObject.SetActive(value: false);
	}

	private void Init()
	{
		m_CharView.Init();
		m_unitInfoSummary.Init(bShowLevel: false);
		if (m_SortOptions != null)
		{
			m_SortOptions.Init(OnSortChanged, bIsCollection: false);
			m_SortOptions.RegisterCategories(setFilterCategory, setSortCategory, bFavoriteFilterActive: false);
			if (m_SortOptions.m_NKCPopupSort != null)
			{
				m_SortOptions.m_NKCPopupSort.m_bUseDefaultSortAdd = false;
			}
		}
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetObject;
			m_LoopScrollRect.dOnReturnObject += ReturnObject;
			m_LoopScrollRect.dOnProvideData += ProvideData;
			NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		}
		if (m_csbtnScout != null)
		{
			m_csbtnScout.PointerClick.RemoveAllListeners();
			m_csbtnScout.PointerClick.AddListener(OnBtnScout);
		}
		NKCUtil.SetGameobjectActive(m_csbtnCollection, bValue: false);
		if (m_csbtnCollection != null)
		{
			m_csbtnCollection.PointerClick.RemoveAllListeners();
			m_csbtnCollection.PointerClick.AddListener(OnBtnCollection);
		}
	}

	public void Open()
	{
		m_UnitSortSystem = MakeScoutSortSystem();
		m_SortOptions.RegisterUnitSort(m_UnitSortSystem);
		m_SortOptions.ResetUI();
		base.gameObject.SetActive(value: true);
		SetCommonUI();
		if (!m_bCellPrepared)
		{
			m_LoopScrollRect.PrepareCells();
			m_bCellPrepared = true;
		}
		NKMPieceTemplet nKMPieceTemplet = FindReddotTargetUnit();
		if (nKMPieceTemplet != null)
		{
			m_SelectedPieceTemplet = nKMPieceTemplet;
		}
		SetUnitData(m_SelectedPieceTemplet);
		m_LoopScrollRect.TotalCount = m_UnitSortSystem.SortedUnitList.Count;
		m_LoopScrollRect.SetIndexPosition(0);
		UIOpened();
		CheckTutorial();
	}

	public void Refresh()
	{
		SetUnitData(m_SelectedPieceTemplet);
		RefreshScoutList();
	}

	public override void UnHide()
	{
		base.UnHide();
		Refresh();
	}

	private void SetCommonUI()
	{
		m_NKCUIPersonnelShortCutMenu.SetData(NKC_SCEN_BASE.eUIOpenReserve.Personnel_Scout);
	}

	private void SetUnitData(NKMPieceTemplet templet)
	{
		m_SelectedPieceTemplet = templet;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (templet == null || !m_dicFakeUnit.TryGetValue(templet.Key, out var value))
		{
			value = null;
			NKCUtil.SetGameobjectActive(m_unitInfoSummary, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitNotSelected, bValue: true);
			NKCUtil.SetGameobjectActive(m_objUnitNotInCollection, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitPiece, bValue: false);
			m_csbtnScout.Lock();
			return;
		}
		NKCUtil.SetGameobjectActive(m_objUnitNotSelected, bValue: false);
		NKCUtil.SetGameobjectActive(m_unitInfoSummary, bValue: true);
		NKCUtil.SetGameobjectActive(m_UnitPiece, bValue: true);
		m_CharView?.SetCharacterIllust(value);
		m_unitInfoSummary?.SetData(value);
		m_UnitPiece?.SetData(templet);
		bool flag = nKMUserData.m_ArmyData.IsCollectedUnit(templet.m_PieceGetUintId);
		long num = (flag ? templet.m_PieceReq : templet.m_PieceReqFirst);
		long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(templet.m_PieceId);
		NKCUtil.SetGameobjectActive(m_objUnitNotInCollection, !flag);
		bool num2 = countMiscItem >= num;
		if (num2)
		{
			m_csbtnScout.UnLock();
		}
		else
		{
			m_csbtnScout.Lock();
		}
		if (num2)
		{
			RegisterAlarmOff(templet.Key);
		}
	}

	private NKCUnitSortSystem MakeScoutSortSystem()
	{
		m_dicFakeUnit.Clear();
		List<NKMUnitData> list = new List<NKMUnitData>();
		foreach (NKMPieceTemplet value in NKMTempletContainer<NKMPieceTemplet>.Values)
		{
			if (value.EnableByTag)
			{
				NKMUnitData nKMUnitData = NKCUnitSortSystem.MakeTempUnitData(value.m_PieceGetUintId, 1, 0);
				nKMUnitData.m_UnitUID = value.Key;
				list.Add(nKMUnitData);
				m_dicFakeUnit.Add(value.Key, nKMUnitData);
			}
		}
		NKCUnitSortSystem.UnitListOptions options = new NKCUnitSortSystem.UnitListOptions
		{
			eDeckType = NKM_DECK_TYPE.NDT_NONE,
			lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.Rarity_High },
			bDescending = true,
			AdditionalExcludeFilterFunc = ScoutListFilterFunc,
			lstDefaultSortOption = new List<NKCUnitSortSystem.eSortOption>
			{
				NKCUnitSortSystem.eSortOption.Rarity_High,
				NKCUnitSortSystem.eSortOption.UID_First
			},
			bHideTokenFiltering = false,
			bIncludeUndeckableUnit = true
		};
		NKCGenericUnitSort nKCGenericUnitSort = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options, list);
		nKCGenericUnitSort.UpdateScoutProgressCache();
		return nKCGenericUnitSort;
	}

	private bool ScoutListFilterFunc(NKMUnitData unitData)
	{
		NKMPieceTemplet nKMPieceTemplet = NKMTempletContainer<NKMPieceTemplet>.Find((int)unitData.m_UnitUID);
		if (nKMPieceTemplet == null)
		{
			return false;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (!nKMUserData.m_ArmyData.IsCollectedUnit(unitData.m_UnitID) && nKMUserData.m_InventoryData.GetCountMiscItem(nKMPieceTemplet.m_PieceId) == 0L)
		{
			return false;
		}
		return true;
	}

	private NKMPieceTemplet FindReddotTargetUnit()
	{
		List<NKMUnitData> list = new List<NKMUnitData>();
		foreach (NKMPieceTemplet value in NKMTempletContainer<NKMPieceTemplet>.Values)
		{
			if (value.EnableByTag && IsReddotNeeded(NKCScenManager.CurrentUserData(), value.Key))
			{
				NKMUnitData nKMUnitData = NKCUnitSortSystem.MakeTempUnitData(value.m_PieceGetUintId, 1, 0);
				nKMUnitData.m_UnitUID = value.Key;
				list.Add(nKMUnitData);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		NKCUnitSortSystem.UnitListOptions options = new NKCUnitSortSystem.UnitListOptions
		{
			eDeckType = NKM_DECK_TYPE.NDT_NONE,
			lstSortOption = new List<NKCUnitSortSystem.eSortOption>
			{
				NKCUnitSortSystem.eSortOption.Rarity_High,
				NKCUnitSortSystem.eSortOption.ID_First
			},
			bDescending = true,
			bIncludeUndeckableUnit = true
		};
		return NKMTempletContainer<NKMPieceTemplet>.Find((int)new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options, list).AutoSelect(null).m_UnitUID);
	}

	private RectTransform GetObject(int index)
	{
		if (m_stkObj.Count > 0)
		{
			RectTransform rectTransform = m_stkObj.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		if (m_pfbScoutSlot == null)
		{
			Debug.LogError("Scout slot prefab null!");
			return null;
		}
		NKCUIScoutSelectListSlot nKCUIScoutSelectListSlot = Object.Instantiate(m_pfbScoutSlot);
		nKCUIScoutSelectListSlot.Init();
		return nKCUIScoutSelectListSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(m_rtSlotPool);
		m_stkObj.Push(go.GetComponent<RectTransform>());
	}

	private void ProvideData(Transform tr, int idx)
	{
		if (idx < 0)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		NKCUIScoutSelectListSlot component = tr.GetComponent<NKCUIScoutSelectListSlot>();
		if (component == null)
		{
			return;
		}
		if (idx >= m_UnitSortSystem.SortedUnitList.Count)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		NKMUnitData nKMUnitData = m_UnitSortSystem.SortedUnitList[idx];
		if (nKMUnitData == null)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		int num = (int)nKMUnitData.m_UnitUID;
		NKCUtil.SetGameobjectActive(tr, bValue: true);
		bool bSelected = m_SelectedPieceTemplet != null && m_SelectedPieceTemplet.Key == num;
		if (nKMUnitData == null)
		{
			Debug.LogError("Potential logic error : null unit in UIScout");
			component.SetData(null, null, bSelected, OnSelectSlot);
		}
		else
		{
			NKMPieceTemplet templet = NKMTempletContainer<NKMPieceTemplet>.Find(num);
			component.SetData(templet, nKMUnitData, bSelected, OnSelectSlot);
		}
	}

	private void RefreshScoutList()
	{
		m_UnitSortSystem.UpdateScoutProgressCache();
		m_LoopScrollRect.TotalCount = m_UnitSortSystem.SortedUnitList.Count;
		m_LoopScrollRect.RefreshCells();
	}

	private void OnSortChanged(bool bResetScroll)
	{
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

	private void OnSelectSlot(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		bool flag = false;
		if (unitData != null)
		{
			NKMPieceTemplet nKMPieceTemplet = NKMTempletContainer<NKMPieceTemplet>.Find((int)unitData.m_UnitUID);
			if (m_SelectedPieceTemplet != nKMPieceTemplet)
			{
				flag = true;
			}
			m_SelectedPieceTemplet = nKMPieceTemplet;
		}
		if (flag)
		{
			SetUnitData(m_SelectedPieceTemplet);
		}
		m_LoopScrollRect.RefreshCells();
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemData.ItemID);
		if (itemMiscTempletByID != null && itemMiscTempletByID.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_PIECE)
		{
			m_UnitSortSystem.UpdateScoutProgressCache();
			m_LoopScrollRect.RefreshCells();
			SetUnitData(m_SelectedPieceTemplet);
		}
	}

	private void OnBtnScout()
	{
		if (m_SelectedPieceTemplet != null)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = m_SelectedPieceTemplet.CanExchange(NKCScenManager.CurrentUserData());
			if (nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_OK)
			{
				NKCUIPopupScoutConfirm.Instance.Open(m_SelectedPieceTemplet, OnScoutConfirm);
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
			}
		}
	}

	private void OnScoutConfirm(int count)
	{
		NKCPacketSender.Send_NKMPacket_EXCHANGE_PIECE_TO_UNIT_REQ(m_SelectedPieceTemplet.m_PieceId, count);
	}

	private void OnBtnCollection()
	{
	}

	public static bool IsReddotNeeded(NKMUserData userData, int templetKey)
	{
		NKMPieceTemplet nKMPieceTemplet = NKMTempletContainer<NKMPieceTemplet>.Find(templetKey);
		long num = (userData.m_ArmyData.IsCollectedUnit(nKMPieceTemplet.m_PieceGetUintId) ? nKMPieceTemplet.m_PieceReq : nKMPieceTemplet.m_PieceReqFirst);
		if (userData.m_InventoryData.GetCountMiscItem(nKMPieceTemplet.m_PieceId) < num)
		{
			return false;
		}
		if (IsAlarmOffRegistered(templetKey))
		{
			return false;
		}
		return true;
	}

	public static bool IsAlarmOffRegistered(int templetKey)
	{
		return PlayerPrefs.GetInt($"SCOUT_ALARM_OFF_{templetKey}", 0) == 1;
	}

	public static void RegisterAlarmOff(int templetKey)
	{
		PlayerPrefs.SetInt($"SCOUT_ALARM_OFF_{templetKey}", 1);
		PlayerPrefs.Save();
	}

	public static void UnregisgerAlarmOff(int templetKey)
	{
		PlayerPrefs.DeleteKey($"SCOUT_ALARM_OFF_{templetKey}");
		PlayerPrefs.Save();
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Scout);
	}
}
