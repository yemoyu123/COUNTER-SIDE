using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISelection : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_UNIT_SELECTION";

	private const string UI_ASSET_NAME = "NKM_UI_UNIT_SELECTION";

	private static NKCUISelection m_Instance;

	public NKCUIComSafeArea m_SafeArea;

	[Header("유닛")]
	public GameObject m_objUnitChoice;

	public LoopScrollRect m_loopScrollRectUnit;

	public Transform m_trContentParentUnit;

	public Image m_imgBannerUnit;

	public Text m_lbUnitDesc;

	[Header("함선")]
	public GameObject m_objShipChoice;

	public LoopScrollRect m_loopScrollRectShip;

	public Transform m_trContentParentShip;

	public Image m_imgBannerShip;

	public Text m_lbShipDesc;

	[Header("프리팹")]
	public NKCUIUnitSelectListSlot m_pfbUnitSlot;

	public NKCUIShipSelectListSlot m_pfbShipSlot;

	[Header("필터/정렬 통합ui")]
	public NKCUIComUnitSortOptions m_SortUI;

	private NKM_ITEM_MISC_TYPE m_NKM_ITEM_MISC_TYPE = NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT;

	private List<int> m_lstRewardId = new List<int>();

	private NKCUnitSortSystem m_ssActive;

	private List<NKCUIUnitSelectListSlotBase> m_lstVisibleSlot = new List<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkUnitSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkShipSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private NKMItemMiscTemplet m_NKMItemMiscTemplet;

	private Vector2 SHIP_SELECTION_CELL_SIZE = new Vector2(565f, 266f);

	private readonly HashSet<NKCUnitSortSystem.eFilterCategory> m_setUnitFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
	{
		NKCUnitSortSystem.eFilterCategory.Have,
		NKCUnitSortSystem.eFilterCategory.UnitType,
		NKCUnitSortSystem.eFilterCategory.UnitRole,
		NKCUnitSortSystem.eFilterCategory.UnitTargetType,
		NKCUnitSortSystem.eFilterCategory.Rarity,
		NKCUnitSortSystem.eFilterCategory.Cost
	};

	private readonly HashSet<NKCUnitSortSystem.eSortCategory> m_setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>
	{
		NKCUnitSortSystem.eSortCategory.ID,
		NKCUnitSortSystem.eSortCategory.Rarity,
		NKCUnitSortSystem.eSortCategory.UnitSummonCost,
		NKCUnitSortSystem.eSortCategory.UnitAttack,
		NKCUnitSortSystem.eSortCategory.UnitHealth,
		NKCUnitSortSystem.eSortCategory.UnitDefense,
		NKCUnitSortSystem.eSortCategory.UnitHit,
		NKCUnitSortSystem.eSortCategory.UnitEvade
	};

	private readonly HashSet<NKCUnitSortSystem.eFilterCategory> m_setShipFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
	{
		NKCUnitSortSystem.eFilterCategory.Have,
		NKCUnitSortSystem.eFilterCategory.ShipType,
		NKCUnitSortSystem.eFilterCategory.Rarity
	};

	private readonly HashSet<NKCUnitSortSystem.eSortCategory> m_setShipSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>
	{
		NKCUnitSortSystem.eSortCategory.ID,
		NKCUnitSortSystem.eSortCategory.Rarity,
		NKCUnitSortSystem.eSortCategory.UnitAttack,
		NKCUnitSortSystem.eSortCategory.UnitHealth
	};

	public static NKCUISelection Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUISelection>("AB_UI_NKM_UI_UNIT_SELECTION", "NKM_UI_UNIT_SELECTION", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUISelection>();
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

	public override string MenuName => m_NKM_ITEM_MISC_TYPE switch
	{
		NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT => NKCUtilString.GET_STRING_CHOICE_UNIT, 
		NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP => NKCUtilString.GET_STRING_CHOICE_SHIP, 
		_ => NKCUtilString.GET_STRING_USE_CHOICE, 
	};

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
		m_loopScrollRectUnit.dOnGetObject += GetObject;
		m_loopScrollRectUnit.dOnReturnObject += ReturnObject;
		m_loopScrollRectUnit.dOnProvideData += ProvideData;
		m_loopScrollRectUnit.dOnRepopulate += CalculateContentRectSize;
		NKCUtil.SetScrollHotKey(m_loopScrollRectUnit);
		m_loopScrollRectShip.dOnGetObject += GetObject;
		m_loopScrollRectShip.dOnReturnObject += ReturnObject;
		m_loopScrollRectShip.dOnProvideData += ProvideData;
		m_loopScrollRectShip.dOnRepopulate += CalculateContentRectSize;
		NKCUtil.SetScrollHotKey(m_loopScrollRectShip);
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
		m_lstRewardId = new List<int>();
		m_SortUI.ResetUI();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnCloseInstance()
	{
		m_ssActive = null;
		m_ssActive = null;
	}

	public void Open(NKMItemMiscTemplet itemMiscTemplet)
	{
		if (itemMiscTemplet == null)
		{
			return;
		}
		m_NKMItemMiscTemplet = itemMiscTemplet;
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(itemMiscTemplet.m_RewardGroupID);
		if (randomBoxItemTempletList == null)
		{
			return;
		}
		for (int i = 0; i < randomBoxItemTempletList.Count; i++)
		{
			m_lstRewardId.Add(randomBoxItemTempletList[i].m_RewardID);
		}
		m_NKM_ITEM_MISC_TYPE = itemMiscTemplet.m_ItemMiscType;
		NKCUtil.SetGameobjectActive(m_objUnitChoice, m_NKM_ITEM_MISC_TYPE == NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT);
		NKCUtil.SetGameobjectActive(m_objShipChoice, m_NKM_ITEM_MISC_TYPE == NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		CalculateContentRectSize();
		switch (m_NKM_ITEM_MISC_TYPE)
		{
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT:
		{
			SetUnitChoiceList();
			Sprite titleSprite2 = GetTitleSprite(itemMiscTemplet.m_BannerImage);
			NKCUtil.SetImageSprite(m_imgBannerUnit, titleSprite2);
			string msg2 = "";
			if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
			{
				NKMCustomBoxTemplet nKMCustomBoxTemplet2 = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
				if (nKMCustomBoxTemplet2 != null)
				{
					msg2 = string.Format(NKCUtilString.GET_STRING_SELECTION_UNIT_LEVEL_INFO_TEXT, nKMCustomBoxTemplet2.Level, nKMCustomBoxTemplet2.SkillLevel);
				}
			}
			else
			{
				msg2 = NKCUtilString.GET_STRING_MISC_UNIT_SELECTION_UNIT_INFO;
			}
			NKCUtil.SetLabelText(m_lbUnitDesc, msg2);
			break;
		}
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP:
		{
			SetShipChoiceList();
			Sprite titleSprite = GetTitleSprite(itemMiscTemplet.m_BannerImage);
			NKCUtil.SetImageSprite(m_imgBannerShip, titleSprite);
			string msg = string.Empty;
			if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
			{
				NKMCustomBoxTemplet nKMCustomBoxTemplet = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
				if (nKMCustomBoxTemplet != null)
				{
					msg = string.Format(NKCUtilString.GET_STRING_SELECTION_SHIP_INFO_TEXT_LEVEL, nKMCustomBoxTemplet.Level);
				}
			}
			else
			{
				msg = NKCUtilString.GET_STRING_SELECTION_SHIP_INFO_TEXT;
			}
			NKCUtil.SetLabelText(m_lbShipDesc, msg);
			break;
		}
		}
		UIOpened();
	}

	private Sprite GetTitleSprite(string imageAsset)
	{
		Sprite sprite = null;
		if (!string.IsNullOrEmpty(imageAsset))
		{
			sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("AB_UI_NKM_UI_UNIT_SELECTION_TEXTURE", imageAsset));
		}
		if (sprite == null)
		{
			sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_UNIT_SELECTION_TEXTURE", "NKM_UI_UNIT_SELECTION_UNIT");
		}
		return sprite;
	}

	private void CalculateContentRectSize()
	{
		NKM_ITEM_MISC_TYPE nKM_ITEM_MISC_TYPE = m_NKM_ITEM_MISC_TYPE;
		m_SafeArea?.SetSafeAreaBase();
		int num = 1;
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		LoopScrollRect loopScrollRect = null;
		GridLayoutGroup gridLayoutGroup = null;
		switch (nKM_ITEM_MISC_TYPE)
		{
		default:
			return;
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT:
			loopScrollRect = m_loopScrollRectUnit;
			gridLayoutGroup = m_trContentParentUnit.GetComponent<GridLayoutGroup>();
			num = 4;
			zero = gridLayoutGroup.cellSize;
			zero2 = gridLayoutGroup.spacing;
			break;
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP:
			loopScrollRect = m_loopScrollRectShip;
			gridLayoutGroup = m_trContentParentShip.GetComponent<GridLayoutGroup>();
			num = 2;
			zero = SHIP_SELECTION_CELL_SIZE;
			zero2 = gridLayoutGroup.spacing;
			break;
		}
		NKCUtil.CalculateContentRectSize(loopScrollRect, gridLayoutGroup, num, zero, zero2, nKM_ITEM_MISC_TYPE == NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP);
	}

	private RectTransform GetObject(int index)
	{
		NKM_ITEM_MISC_TYPE nKM_ITEM_MISC_TYPE = m_NKM_ITEM_MISC_TYPE;
		if ((uint)(nKM_ITEM_MISC_TYPE - 7) <= 1u)
		{
			Stack<NKCUIUnitSelectListSlotBase> stack;
			NKCUIUnitSelectListSlotBase original;
			switch (m_NKM_ITEM_MISC_TYPE)
			{
			case NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT:
				stack = m_stkUnitSlotPool;
				original = m_pfbUnitSlot;
				break;
			case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP:
				stack = m_stkShipSlotPool;
				original = m_pfbShipSlot;
				break;
			default:
				return null;
			}
			NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = null;
			if (stack.Count > 0)
			{
				nKCUIUnitSelectListSlotBase = stack.Pop();
			}
			else
			{
				nKCUIUnitSelectListSlotBase = Object.Instantiate(original);
				nKCUIUnitSelectListSlotBase.Init();
			}
			NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlotBase, bValue: true);
			nKCUIUnitSelectListSlotBase.transform.localScale = Vector3.one;
			m_lstVisibleSlot.Add(nKCUIUnitSelectListSlotBase);
			return nKCUIUnitSelectListSlotBase.GetComponent<RectTransform>();
		}
		return null;
	}

	private void ReturnObject(Transform go)
	{
		NKCUIUnitSelectListSlotBase component = go.GetComponent<NKCUIUnitSelectListSlotBase>();
		if (component != null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			go.SetParent(base.transform);
			m_lstVisibleSlot.Remove(component);
			if (component is NKCUIUnitSelectListSlot)
			{
				m_stkUnitSlotPool.Push(component);
			}
			else if (component is NKCUIShipSelectListSlot)
			{
				m_stkShipSlotPool.Push(component);
			}
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		switch (m_NKM_ITEM_MISC_TYPE)
		{
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT:
		{
			if (m_ssActive == null)
			{
				Debug.LogError("Slot Sort System Null!!");
				break;
			}
			NKCUIUnitSelectListSlotBase component2 = tr.GetComponent<NKCUIUnitSelectListSlotBase>();
			if (!(component2 == null) && m_ssActive.SortedUnitList.Count > idx)
			{
				NKCUtil.SetGameobjectActive(component2, bValue: true);
				NKMUnitData unitData2 = m_ssActive.SortedUnitList[idx];
				SetUnitSlotData(component2, unitData2);
			}
			break;
		}
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP:
		{
			if (m_ssActive == null)
			{
				Debug.LogError("Slot Sort System Null!!");
				break;
			}
			NKCUIShipSelectListSlot component = tr.GetComponent<NKCUIShipSelectListSlot>();
			if (!(component == null) && m_ssActive.SortedUnitList.Count > idx)
			{
				NKCUtil.SetGameobjectActive(component, bValue: true);
				NKMUnitData unitData = m_ssActive.SortedUnitList[idx];
				SetShipSlotData(component, unitData);
			}
			break;
		}
		}
	}

	private void SetUnitChoiceList()
	{
		NKCUnitSortSystem.UnitListOptions options = new NKCUnitSortSystem.UnitListOptions
		{
			setOnlyIncludeUnitID = new HashSet<int>(),
			eDeckType = NKM_DECK_TYPE.NDT_NORMAL,
			lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: true, bIsSelection: true),
			bIncludeUndeckableUnit = true
		};
		int unitLevel = 1;
		int limitBreakLevel = 0;
		int reactor = 0;
		int skillLevel = -1;
		int tacticLevel = 0;
		if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
		{
			NKMCustomBoxTemplet nKMCustomBoxTemplet = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
			if (nKMCustomBoxTemplet != null)
			{
				if (nKMCustomBoxTemplet.Level > 0)
				{
					unitLevel = nKMCustomBoxTemplet.Level;
				}
				if (nKMCustomBoxTemplet.LimitBreak > 0)
				{
					limitBreakLevel = nKMCustomBoxTemplet.LimitBreak;
				}
				if (nKMCustomBoxTemplet.ReactorLevel > 0)
				{
					reactor = nKMCustomBoxTemplet.ReactorLevel;
				}
				if (nKMCustomBoxTemplet.SkillLevel > 0)
				{
					skillLevel = nKMCustomBoxTemplet.SkillLevel;
				}
				if (nKMCustomBoxTemplet.TacticUpdate > 0)
				{
					tacticLevel = nKMCustomBoxTemplet.TacticUpdate;
				}
			}
		}
		List<NKMUnitData> list = new List<NKMUnitData>();
		for (int i = 0; i < m_lstRewardId.Count; i++)
		{
			NKMUnitData item = NKMUnitManager.CreateUnitData(m_lstRewardId[i], m_lstRewardId[i], unitLevel, limitBreakLevel, skillLevel, reactor, tacticLevel, 0, fromContract: false).unitData;
			list.Add(item);
			if (!options.setOnlyIncludeUnitID.Contains(m_lstRewardId[i]))
			{
				options.setOnlyIncludeUnitID.Add(m_lstRewardId[i]);
			}
		}
		m_ssActive = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options, list);
		m_SortUI.RegisterCategories(m_setUnitFilterCategory, m_setUnitSortCategory, bFavoriteFilterActive: false);
		m_SortUI.RegisterUnitSort(m_ssActive);
		m_SortUI.ResetUI();
		m_loopScrollRectUnit.PrepareCells();
		m_loopScrollRectUnit.TotalCount = list.Count;
		m_loopScrollRectUnit.RefreshCells(bForce: true);
	}

	private void SetUnitSlotData(NKCUIUnitSelectListSlotBase slot, NKMUnitData unitData)
	{
		long unitUID = unitData.m_UnitUID;
		NKMDeckIndex deckIndexCache = m_ssActive.GetDeckIndexCache(unitUID, bTargetDecktypeOnly: true);
		slot.SetData(unitData, deckIndexCache, bEnableLayoutElement: true, OnSelectUnitSlot);
		slot.SetLock(unitData.m_bLock);
		slot.SetFavorite(unitData);
		slot.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		if (m_ssActive.Options.lstSortOption.Count > 0)
		{
			switch (m_ssActive.Options.lstSortOption[0])
			{
			case NKCUnitSortSystem.eSortOption.Power_Low:
			case NKCUnitSortSystem.eSortOption.Power_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Power_High, m_ssActive.GetUnitPowerCache(unitUID), "N0");
				break;
			case NKCUnitSortSystem.eSortOption.Attack_Low:
			case NKCUnitSortSystem.eSortOption.Attack_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Attack_High, m_ssActive.GetUnitAttackCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Health_Low:
			case NKCUnitSortSystem.eSortOption.Health_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Health_High, m_ssActive.GetUnitHPCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Defense_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Defense_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Defense_High, m_ssActive.GetUnitDefCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Crit_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Crit_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Crit_High, m_ssActive.GetUnitCritCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Hit_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Hit_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Hit_High, m_ssActive.GetUnitHitCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Evade_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Evade_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Evade_High, m_ssActive.GetUnitEvadeCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_Low:
			case NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_High, m_ssActive.GetUnitSkillCoolCache(unitUID));
				break;
			default:
				slot.SetSortingTypeValue(bSet: false);
				break;
			}
		}
		else
		{
			slot.SetSortingTypeValue(bSet: false);
		}
		NKCUIUnitSelectList.eUnitSlotSelectState slotSelectState = NKCUIUnitSelectList.eUnitSlotSelectState.NONE;
		slot.SetSlotSelectState(slotSelectState);
		slot.SetCityLeaderMark(value: false);
		int unitCountByID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitCountByID(unitData.m_UnitID);
		slot.SetHaveCount(unitCountByID);
	}

	private void SetShipChoiceList()
	{
		NKCUnitSortSystem.UnitListOptions options = new NKCUnitSortSystem.UnitListOptions
		{
			setOnlyIncludeUnitID = new HashSet<int>(),
			eDeckType = NKM_DECK_TYPE.NDT_NORMAL,
			lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_SHIP, bIsCollection: true, bIsSelection: true)
		};
		List<NKMUnitData> list = new List<NKMUnitData>();
		for (int i = 0; i < m_lstRewardId.Count; i++)
		{
			int num = m_lstRewardId[i];
			int unitLevel = 1;
			int limitBreakLevel = 0;
			if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
			{
				NKMCustomBoxTemplet nKMCustomBoxTemplet = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
				if (nKMCustomBoxTemplet != null)
				{
					if (nKMCustomBoxTemplet.Level > 0)
					{
						unitLevel = nKMCustomBoxTemplet.Level;
					}
					if (nKMCustomBoxTemplet.LimitBreak > 0)
					{
						limitBreakLevel = nKMCustomBoxTemplet.LimitBreak;
					}
				}
			}
			NKMUnitData item = NKMUnitManager.CreateShipData(num, num, unitLevel, limitBreakLevel).unitData;
			list.Add(item);
			if (!options.setOnlyIncludeUnitID.Contains(num))
			{
				options.setOnlyIncludeUnitID.Add(num);
			}
		}
		m_ssActive = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options, list);
		m_SortUI.RegisterCategories(m_setShipFilterCategory, m_setShipSortCategory, bFavoriteFilterActive: false);
		m_SortUI.RegisterUnitSort(m_ssActive);
		m_SortUI.ResetUI();
		m_loopScrollRectShip.PrepareCells();
		m_loopScrollRectShip.TotalCount = list.Count;
		m_loopScrollRectShip.RefreshCells(bForce: true);
	}

	private void SetShipSlotData(NKCUIShipSelectListSlot slot, NKMUnitData unitData)
	{
		long unitUID = unitData.m_UnitUID;
		NKMDeckIndex deckIndexCache = m_ssActive.GetDeckIndexCache(unitUID, bTargetDecktypeOnly: true);
		slot.SetData(unitData, deckIndexCache, bEnableLayoutElement: true, OnSelectUnitSlot);
		slot.SetLock(unitData.m_bLock);
		slot.SetFavorite(unitData);
		if (m_ssActive.Options.lstSortOption.Count > 0)
		{
			switch (m_ssActive.Options.lstSortOption[0])
			{
			case NKCUnitSortSystem.eSortOption.Power_Low:
			case NKCUnitSortSystem.eSortOption.Power_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Power_High, m_ssActive.GetUnitPowerCache(unitUID), "N0");
				break;
			case NKCUnitSortSystem.eSortOption.Attack_Low:
			case NKCUnitSortSystem.eSortOption.Attack_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Attack_High, m_ssActive.GetUnitAttackCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Health_Low:
			case NKCUnitSortSystem.eSortOption.Health_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Health_High, m_ssActive.GetUnitHPCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Defense_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Defense_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Defense_High, m_ssActive.GetUnitDefCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Crit_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Crit_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Crit_High, m_ssActive.GetUnitCritCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Hit_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Hit_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Hit_High, m_ssActive.GetUnitHitCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Evade_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Evade_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Evade_High, m_ssActive.GetUnitEvadeCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_Low:
			case NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_High, m_ssActive.GetUnitSkillCoolCache(unitUID));
				break;
			default:
				slot.SetSortingTypeValue(bSet: false);
				break;
			}
		}
		else
		{
			slot.SetSortingTypeValue(bSet: false);
		}
		NKCUIUnitSelectList.eUnitSlotSelectState slotSelectState = NKCUIUnitSelectList.eUnitSlotSelectState.NONE;
		slot.SetSlotSelectState(slotSelectState);
		slot.SetCityLeaderMark(value: false);
		int sameKindShipCountFromID = NKCScenManager.CurrentUserData().m_ArmyData.GetSameKindShipCountFromID(unitData.m_UnitID);
		slot.SetHaveCount(sameKindShipCountFromID);
	}

	private void OnSortChanged(bool bResetScroll)
	{
		if (m_ssActive == null)
		{
			return;
		}
		LoopScrollRect loopScrollRect = null;
		switch (m_NKM_ITEM_MISC_TYPE)
		{
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT:
			loopScrollRect = m_loopScrollRectUnit;
			break;
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP:
			loopScrollRect = m_loopScrollRectShip;
			break;
		}
		if (loopScrollRect != null)
		{
			loopScrollRect.TotalCount = m_ssActive.SortedUnitList.Count;
			if (bResetScroll)
			{
				loopScrollRect.SetIndexPosition(0);
			}
			else
			{
				loopScrollRect.RefreshCells();
			}
		}
	}

	public void OnSelectUnitSlot(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		NKCPopupSelectionConfirm.Instance.Open(m_NKMItemMiscTemplet, unitData.m_UnitID, 1L);
	}
}
