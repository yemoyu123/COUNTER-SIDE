using System.Collections.Generic;
using System.Linq;
using NKC.UI;
using NKC.UI.Collection;
using NKC.UI.Component;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIRearmamentProcess : MonoBehaviour
{
	public delegate void ChangeState(NKCUIRearmament.REARM_TYPE newType);

	public NKCUICharInfoSummary m_CharInfoSummary;

	public NKCUICharacterView m_charView;

	[Header("재무장 유닛 선택 리스트")]
	public NKCUIRearmamentProcessInfoSummary m_RearmSummaryInfo;

	public GameObject m_objRearmList;

	public LoopVerticalScrollRect m_LoopScroll;

	public NKCUIComStateButton m_UnitSelectBtn;

	public NKCUIComMiscSortOptions m_SortOptions;

	[Header("정렬/필터 통합UI")]
	public NKCUIComUnitSortOptions m_SortUI;

	[Space]
	[Space]
	[Header("재무장 유닛 진행")]
	public NKCUIRearmamentProcessInfoSummary m_RearmSummarySkill;

	public GameObject m_objRearmProcess;

	public NKCUIComStateButton m_RearmUnitInfo;

	public NKCUIComStateButton m_RearmBtn;

	public Image m_imgRearmBtn;

	public Text m_lbRearmBtn;

	[Space]
	public NKCUIItemCostSlot m_RequestLvSlot;

	public List<NKCUIItemCostSlot> m_lstRequestItemSlot;

	[Space]
	public NKCUIComStateButton m_csbtnSelectResourceUnit;

	public GameObject m_TargetEmptyObj;

	public NKCUIUnitSelectListSlot m_RearmTargetUnitSlot;

	public GameObject m_TargetEmptyText;

	public GameObject m_TargetSelectedText;

	private ChangeState dOk;

	private int m_iCurSelectedUnitID;

	private long m_iCurRearmResourceUID;

	private NKCUIRearmament.REARM_TYPE m_curUIType;

	private readonly HashSet<NKCUnitSortSystem.eFilterCategory> eRearmFilterCategories = new HashSet<NKCUnitSortSystem.eFilterCategory>
	{
		NKCUnitSortSystem.eFilterCategory.UnitType,
		NKCUnitSortSystem.eFilterCategory.UnitRole,
		NKCUnitSortSystem.eFilterCategory.UnitMoveType,
		NKCUnitSortSystem.eFilterCategory.UnitTargetType,
		NKCUnitSortSystem.eFilterCategory.Rarity,
		NKCUnitSortSystem.eFilterCategory.Cost
	};

	private readonly HashSet<NKCUnitSortSystem.eSortCategory> eRearmSortCategories = new HashSet<NKCUnitSortSystem.eSortCategory>
	{
		NKCUnitSortSystem.eSortCategory.IDX,
		NKCUnitSortSystem.eSortCategory.Rarity,
		NKCUnitSortSystem.eSortCategory.UnitSummonCost
	};

	private static readonly List<NKCUnitSortSystem.eSortOption> eRearmSortLists = new List<NKCUnitSortSystem.eSortOption>
	{
		NKCUnitSortSystem.eSortOption.IDX_First,
		NKCUnitSortSystem.eSortOption.Rarity_High,
		NKCUnitSortSystem.eSortOption.Unit_SummonCost_High
	};

	private NKCUIUnitSelectList.UnitSelectListOptions m_currentOption;

	private NKCUnitSortSystem m_SortSystem;

	public NKCUIUnitSelectListSlot m_pfbUnitSelectSlot;

	private List<NKMUnitRearmamentTemplet> m_lstRearm = new List<NKMUnitRearmamentTemplet>();

	private List<NKCUIUnitSelectListSlot> m_lstVisbleSlots = new List<NKCUIUnitSelectListSlot>();

	public NKCUIRearmamentSubUISelectList m_RearmSelectSubUI;

	private NKCUIUnitSelectList m_UIUnitSelectList;

	private NKCUIUnitSelectList UnitSelectList
	{
		get
		{
			if (m_UIUnitSelectList == null)
			{
				m_UIUnitSelectList = NKCUIUnitSelectList.OpenNewInstance();
			}
			return m_UIUnitSelectList;
		}
	}

	public void Init(ChangeState func = null)
	{
		NKCUtil.SetBindFunction(m_RearmUnitInfo, OnClickUnitInfo);
		NKCUtil.SetBindFunction(m_UnitSelectBtn, OnClickTargetSelectBtn);
		NKCUtil.SetBindFunction(m_RearmBtn, OnClickRearmBtn);
		if (m_LoopScroll != null)
		{
			m_LoopScroll.dOnGetObject += GetUnitSelectSlot;
			m_LoopScroll.dOnReturnObject += ReturnUnitSelectSlot;
			m_LoopScroll.dOnProvideData += ProvideUnitSelectSlot;
			m_LoopScroll.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScroll);
		}
		if (m_SortUI != null)
		{
			m_SortUI.Init(OnSortChanged, bIsCollection: true);
			m_SortUI.RegisterCategories(eRearmFilterCategories, eRearmSortCategories, bFavoriteFilterActive: false);
		}
		InitRearmData();
		InitProcessUI();
		m_CharInfoSummary.Init(bShowLevel: false);
		dOk = func;
	}

	public void SetReserveRearmData(int iTargetRearmTypeUnitID, long iResourceRearmUnitUID)
	{
		m_iCurSelectedUnitID = iTargetRearmTypeUnitID;
		m_iCurRearmResourceUID = iResourceRearmUnitUID;
	}

	public void Clear()
	{
		m_iCurSelectedUnitID = 0;
		m_iCurRearmResourceUID = 0L;
		m_UIUnitSelectList?.Close();
		m_UIUnitSelectList = null;
	}

	private void OnClickUnitInfo()
	{
		if (m_iCurSelectedUnitID != 0)
		{
			NKCUICollectionUnitInfo.CheckInstanceAndOpen(NKCUtil.MakeDummyUnit(m_iCurSelectedUnitID, 100, 3), null);
		}
	}

	private void OnClickTargetSelectBtn()
	{
		if (m_iCurSelectedUnitID != 0)
		{
			m_iCurRearmResourceUID = 0L;
			dOk?.Invoke(NKCUIRearmament.REARM_TYPE.RT_PROCESS);
		}
	}

	public void Open(NKCUIRearmament.REARM_TYPE type = NKCUIRearmament.REARM_TYPE.RT_LIST)
	{
		if (type == NKCUIRearmament.REARM_TYPE.RT_LIST)
		{
			InitRearmListSortSystem();
			if (m_iCurSelectedUnitID == 0)
			{
				m_iCurSelectedUnitID = m_SortSystem.SortedUnitList[0].m_UnitID;
			}
			ChangeSlotSelectedState(m_iCurSelectedUnitID);
			OnSortChanged(bResetScroll: true);
		}
		else
		{
			int fromUnitID = NKCRearmamentUtil.GetFromUnitID(m_iCurSelectedUnitID);
			if (fromUnitID != 0)
			{
				m_RearmSelectSubUI.SetData(fromUnitID, m_iCurSelectedUnitID, m_iCurRearmResourceUID);
			}
			else
			{
				m_RearmSelectSubUI.SetData(m_lstRearm[0].FromUnitTemplet.m_UnitID, 0, 0L);
			}
		}
		ChangeUI(type);
		UpdateUI();
	}

	private void ChangeUI(NKCUIRearmament.REARM_TYPE type)
	{
		NKCUtil.SetGameobjectActive(m_objRearmProcess, type == NKCUIRearmament.REARM_TYPE.RT_PROCESS);
		NKCUtil.SetGameobjectActive(m_objRearmList, type == NKCUIRearmament.REARM_TYPE.RT_LIST);
		NKCUtil.SetGameobjectActive(m_objRearmProcess, type == NKCUIRearmament.REARM_TYPE.RT_PROCESS);
		NKCUtil.SetGameobjectActive(m_objRearmList, type == NKCUIRearmament.REARM_TYPE.RT_LIST);
		NKCUtil.SetGameobjectActive(m_LoopScroll.content.gameObject, type == NKCUIRearmament.REARM_TYPE.RT_LIST);
		m_curUIType = type;
	}

	private void InitRearmListSortSystem()
	{
		m_SortSystem = null;
		NKCUIUnitSelectList.UnitSelectListOptions currentOption = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
		currentOption.setUnitFilterCategory = eRearmFilterCategories;
		currentOption.lstSortOption = eRearmSortLists;
		currentOption.bShowHideDeckedUnitMenu = true;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_currentOption = currentOption;
		List<NKMUnitData> list = new List<NKMUnitData>();
		foreach (NKMUnitRearmamentTemplet item in m_lstRearm)
		{
			NKMUnitData nKMUnitData = new NKMUnitData();
			nKMUnitData.m_UnitID = item.Key;
			nKMUnitData.m_UnitUID = item.Key;
			list.Add(nKMUnitData);
		}
		m_SortSystem = new NKCGenericUnitSort(null, m_currentOption.m_SortOptions, list);
		m_SortUI.RegisterUnitSort(m_SortSystem);
		m_SortUI.ResetUI();
	}

	private void OnSortChanged(bool bResetScroll)
	{
		if (m_SortSystem != null)
		{
			m_LoopScroll.TotalCount = m_SortSystem.SortedUnitList.Count;
			if (bResetScroll)
			{
				m_LoopScroll.SetIndexPosition(0);
				m_LoopScroll.RefreshCells(bForce: true);
			}
			else
			{
				m_LoopScroll.RefreshCells();
			}
			ChangeSlotSelectedState(m_iCurSelectedUnitID);
		}
	}

	public RectTransform GetRearmSlotRectTransform(int iRearmUnitID)
	{
		foreach (NKCUIUnitSelectListSlot lstVisbleSlot in m_lstVisbleSlots)
		{
			if (lstVisbleSlot.NKMUnitData.m_UnitID == iRearmUnitID)
			{
				return lstVisbleSlot.GetComponent<RectTransform>();
			}
		}
		return null;
	}

	private void InitRearmData()
	{
		m_lstRearm.Clear();
		foreach (NKMUnitRearmamentTemplet value in NKMTempletContainer<NKMUnitRearmamentTemplet>.Values)
		{
			if (value.EnableByTag)
			{
				m_lstRearm.Add(value);
			}
		}
		if (m_lstRearm.Count <= 0)
		{
			Debug.Log("<color=red>Unit Rearm Data is null</color>");
			return;
		}
		m_lstRearm.OrderBy((NKMUnitRearmamentTemplet x) => x.Key);
	}

	private RectTransform GetUnitSelectSlot(int index)
	{
		NKCUIUnitSelectListSlot nKCUIUnitSelectListSlot = Object.Instantiate(m_pfbUnitSelectSlot);
		nKCUIUnitSelectListSlot.Init();
		nKCUIUnitSelectListSlot.transform.localScale = Vector3.one;
		m_lstVisbleSlots.Add(nKCUIUnitSelectListSlot);
		return nKCUIUnitSelectListSlot.GetComponent<RectTransform>();
	}

	private void ReturnUnitSelectSlot(Transform go)
	{
		NKCUIUnitSelectListSlot slot = go.GetComponent<NKCUIUnitSelectListSlot>();
		if (slot != null)
		{
			NKCUIUnitSelectListSlot nKCUIUnitSelectListSlot = m_lstVisbleSlots.Find((NKCUIUnitSelectListSlot x) => x.NKMUnitData.m_UnitID == slot.NKMUnitData.m_UnitID);
			if (nKCUIUnitSelectListSlot != null)
			{
				m_lstVisbleSlots.Remove(nKCUIUnitSelectListSlot);
			}
		}
		NKCUtil.SetGameobjectActive(go, bValue: false);
		Object.Destroy(go.gameObject);
	}

	private void ProvideUnitSelectSlot(Transform tr, int idx)
	{
		if (m_SortSystem == null)
		{
			Debug.LogError("Slot Sort System Null!!");
			return;
		}
		NKCUIUnitSelectListSlot component = tr.GetComponent<NKCUIUnitSelectListSlot>();
		if (!(component != null))
		{
			return;
		}
		if (m_SortSystem.SortedUnitList.Count <= idx || idx < 0)
		{
			Debug.LogError($"m_SortSystem.SortedUnitList - 잘못된 인덱스 입니다, {idx}");
			return;
		}
		tr.SetParent(m_LoopScroll.content);
		NKMUnitData nKMUnitData = new NKMUnitData();
		nKMUnitData.m_UnitID = m_SortSystem.SortedUnitList[idx].m_UnitID;
		nKMUnitData.m_SkinID = 0;
		nKMUnitData.m_UnitLevel = 1;
		component.Init(resetLocalScale: true);
		component.SetDataForRearm(nKMUnitData, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), bEnableLayoutElement: false, OnSlotClicked, bShowEqup: false);
		if (nKMUnitData.m_UnitID == m_iCurSelectedUnitID)
		{
			component.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.SELECTED);
		}
		else
		{
			component.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.DISABLE);
		}
		m_lstVisbleSlots.Add(component);
	}

	private void UpdateUI()
	{
		UpdateCommonUI();
		if (m_curUIType == NKCUIRearmament.REARM_TYPE.RT_LIST)
		{
			UpdateRearmListUI();
		}
		else
		{
			UpdateRearmProcessUI();
		}
	}

	private void UpdateRearmListUI()
	{
		ChangeSlotSelectedState(m_iCurSelectedUnitID);
		m_RearmSummaryInfo.SetData(m_iCurSelectedUnitID);
	}

	private void UpdateCommonUI()
	{
		NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(m_iCurSelectedUnitID, 100, 3);
		m_CharInfoSummary.SetData(nKMUnitData);
		m_CharInfoSummary.SetEnableClassStar(bEnable: false);
		m_charView.CloseCharacterIllust();
		m_charView.SetCharacterIllust(nKMUnitData);
	}

	public void OnSlotClicked(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		m_iCurSelectedUnitID = unitData.m_UnitID;
		UpdateUI();
	}

	private void ChangeSlotSelectedState(int unitID)
	{
		foreach (NKCUIUnitSelectListSlot lstVisbleSlot in m_lstVisbleSlots)
		{
			if (lstVisbleSlot.NKMUnitData != null && lstVisbleSlot.NKMUnitData.m_UnitID == unitID)
			{
				lstVisbleSlot.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.SELECTED);
			}
			else
			{
				lstVisbleSlot.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.DISABLE);
			}
		}
	}

	private void InitProcessUI()
	{
		m_RearmTargetUnitSlot?.Init(resetLocalScale: true);
		NKCUtil.SetBindFunction(m_csbtnSelectResourceUnit, delegate
		{
			OnClickSelectResourceUnit(0L);
		});
		m_RearmSelectSubUI.Init(SelectTargetRearmUnit);
	}

	private void SelectTargetRearmUnit(int targetUnitID)
	{
		m_iCurSelectedUnitID = targetUnitID;
		UpdateUI();
	}

	private void UpdateRearmProcessUI()
	{
		LeftRearmTargetUI();
		UpdateRearmSubUIUnitSlotUI();
		UpdateCostSlotUI();
		UpdateButtonUI();
		m_RearmSummarySkill.SetData(m_iCurSelectedUnitID, bSkill: true);
	}

	private void UpdateRearmSubUIUnitSlotUI()
	{
		m_RearmSelectSubUI.UpdateReamUnitSlotData(m_iCurRearmResourceUID);
	}

	private void UpdateCostSlotUI()
	{
		int num = 1;
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(m_iCurRearmResourceUID);
		if (unitFromUID != null)
		{
			num = unitFromUID.m_UnitLevel;
		}
		m_RequestLvSlot?.SetData(910, 110, num);
		NKMUnitRearmamentTemplet rearmamentTemplet = NKCRearmamentUtil.GetRearmamentTemplet(m_iCurSelectedUnitID);
		if (rearmamentTemplet != null)
		{
			NKCUIItemCostSlot.SetDataList(m_lstRequestItemSlot, rearmamentTemplet.RearmamentUseItems);
		}
	}

	private void LeftRearmTargetUI()
	{
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(m_iCurRearmResourceUID);
		if (unitFromUID != null)
		{
			m_RearmTargetUnitSlot.SetDataForRearm(unitFromUID, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), bEnableLayoutElement: false, OnResourceSlotClicked, bShowEqup: false);
		}
		bool flag = unitFromUID != null;
		NKCUtil.SetGameobjectActive(m_TargetEmptyObj, !flag);
		NKCUtil.SetGameobjectActive(m_RearmTargetUnitSlot.gameObject, flag);
		NKCUtil.SetGameobjectActive(m_TargetEmptyText.gameObject, !flag);
		NKCUtil.SetGameobjectActive(m_TargetSelectedText.gameObject, flag);
	}

	private void UpdateButtonUI()
	{
		bool flag = false;
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(m_iCurRearmResourceUID);
		if (unitFromUID != null && unitFromUID.m_UnitLevel >= 110)
		{
			bool flag2 = true;
			NKMUnitRearmamentTemplet rearmamentTemplet = NKCRearmamentUtil.GetRearmamentTemplet(m_iCurSelectedUnitID);
			if (rearmamentTemplet != null)
			{
				foreach (MiscItemUnit rearmamentUseItem in rearmamentTemplet.RearmamentUseItems)
				{
					if ((int)rearmamentUseItem.Count > NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(rearmamentUseItem.ItemId))
					{
						flag2 = false;
						break;
					}
				}
				flag = flag2;
			}
		}
		if (flag)
		{
			NKCUtil.SetImageSprite(m_imgRearmBtn, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW));
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgRearmBtn, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
		}
		NKCUtil.SetLabelTextColor(m_lbRearmBtn, NKCUtil.GetButtonUIColor(flag));
	}

	private void OnClickRearmBtn()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(m_iCurRearmResourceUID);
		if (unitFromUID == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REARM_PROCESS_BLOCK_MESSAGE_EMPTY_TARGET_UNIT);
			return;
		}
		if (unitFromUID.m_UnitLevel < 110)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REARM_PROCESS_BLOCK_MESSAGE_LACK_COND);
			return;
		}
		NKMEquipmentSet equipmentSet = unitFromUID.GetEquipmentSet(nKMUserData.m_InventoryData);
		if (equipmentSet.Weapon != null || equipmentSet.Defence != null || equipmentSet.Accessory != null || equipmentSet.Accessory2 != null)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REARM_PROCESS_BLOCK_MESSAGE_EQUIPED, UnEquipAllItems);
			return;
		}
		NKMUnitRearmamentTemplet rearmamentTemplet = NKCRearmamentUtil.GetRearmamentTemplet(m_iCurSelectedUnitID);
		if (rearmamentTemplet != null)
		{
			foreach (MiscItemUnit rearmamentUseItem in rearmamentTemplet.RearmamentUseItems)
			{
				if ((int)rearmamentUseItem.Count > NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(rearmamentUseItem.ItemId))
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REARM_PROCESS_BLOCK_MESSAGE_LACK_COND);
					return;
				}
			}
		}
		NKCUIPopupRearmamentConfirm.Instance.Open(m_iCurSelectedUnitID, m_iCurRearmResourceUID);
	}

	private void UnEquipAllItems()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(m_iCurRearmResourceUID);
		if (unitFromUID != null)
		{
			NKMEquipmentSet equipmentSet = unitFromUID.GetEquipmentSet(nKMUserData.m_InventoryData);
			SendUnEquipItemREQ(m_iCurRearmResourceUID, equipmentSet.Weapon, ITEM_EQUIP_POSITION.IEP_WEAPON);
			SendUnEquipItemREQ(m_iCurRearmResourceUID, equipmentSet.Defence, ITEM_EQUIP_POSITION.IEP_DEFENCE);
			SendUnEquipItemREQ(m_iCurRearmResourceUID, equipmentSet.Accessory, ITEM_EQUIP_POSITION.IEP_ACC);
			SendUnEquipItemREQ(m_iCurRearmResourceUID, equipmentSet.Accessory2, ITEM_EQUIP_POSITION.IEP_ACC2);
		}
	}

	private void SendUnEquipItemREQ(long unitUID, NKMEquipItemData equipData, ITEM_EQUIP_POSITION position)
	{
		if (equipData != null)
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: false, unitUID, equipData.m_ItemUid, position);
		}
	}

	public void OnResourceSlotClicked(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotStatem, NKCUIUnitSelectList.eUnitSlotSelectState slotSelectState)
	{
		OnClickSelectResourceUnit(unitData.m_UnitUID);
	}

	private void OnClickSelectResourceUnit(long lSelectedUnitUID = 0L)
	{
		NKMUnitRearmamentTemplet rearmamentTemplet = NKCRearmamentUtil.GetRearmamentTemplet(m_iCurSelectedUnitID);
		if (rearmamentTemplet != null)
		{
			NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
			options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			options.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
			options.bDescending = false;
			options.bShowRemoveSlot = false;
			options.bMultipleSelect = false;
			options.bExcludeLockedUnit = false;
			options.bExcludeDeckedUnit = false;
			options.m_SortOptions.bUseDeckedState = false;
			options.m_SortOptions.bUseLockedState = false;
			options.m_SortOptions.bUseDormInState = false;
			options.dOnSelectedUnitWarning = null;
			options.m_SortOptions.bIgnoreCityState = true;
			options.m_SortOptions.bIgnoreWorldMapLeader = true;
			options.bShowHideDeckedUnitMenu = false;
			options.bHideDeckedUnit = false;
			options.setOnlyIncludeUnitID = new HashSet<int>();
			options.setOnlyIncludeUnitID.Add(rearmamentTemplet.FromUnitTemplet.m_BaseUnitID);
			options.setSelectedUnitUID = new HashSet<long>();
			if (lSelectedUnitUID != 0L)
			{
				options.setSelectedUnitUID.Add(lSelectedUnitUID);
			}
			options.setExcludeUnitID = NKCUnitSortSystem.GetDefaultExcludeUnitIDs();
			options.strEmptyMessage = NKCUtilString.GET_STRING_REARM_PROCESS_UNIT_SELECT_LIST_TITLE;
			options.dOnSlotSetData = null;
			options.setUnitFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
			{
				NKCUnitSortSystem.eFilterCategory.Level,
				NKCUnitSortSystem.eFilterCategory.Locked,
				NKCUnitSortSystem.eFilterCategory.Decked
			};
			options.setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory> { NKCUnitSortSystem.eSortCategory.Level };
			options.m_bHideUnitCount = true;
			options.m_bUseFavorite = true;
			UnitSelectList.Open(options, OnConsumeUnitSelected);
		}
	}

	public void OnConsumeUnitSelected(List<long> selectedList)
	{
		if (UnitSelectList.IsOpen)
		{
			UnitSelectList.Close();
		}
		if (selectedList.Count > 0)
		{
			m_iCurRearmResourceUID = selectedList[0];
			UpdateUI();
		}
	}

	public void OnInventoryChange()
	{
		UpdateCostSlotUI();
		UpdateButtonUI();
	}
}
