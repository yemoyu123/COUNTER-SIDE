using System.Collections.Generic;
using System.Linq;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletBan : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_POPUP_BANNED_LIST_CASTING";

	private static NKCPopupGauntletBan m_Instance;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public EventTrigger m_etBG;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnVote;

	public Text m_lbRemainTime;

	public Text m_lbVote;

	public Text m_lbVoteLock;

	[Header("\ufffd\ufffd\ufffd")]
	public Text m_lbSubTitle;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public NKCUIComToggle m_ctglUnit;

	public NKCUIComToggle m_ctglShip;

	public NKCUIComToggle m_ctglOper;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public LoopVerticalScrollRect m_lvsrUnit;

	public LoopVerticalScrollRect m_lvsrShip;

	public LoopVerticalScrollRect m_lvsrOper;

	public GameObject m_objRemainTime;

	public GameObject m_objUnitList;

	public GameObject m_objShipList;

	public GameObject m_objOperList;

	public GameObject m_objNone;

	public NKCUIUnitSelectListSlot m_pfbUnitSlotForBan;

	public NKCUIShipSelectListSlot m_pfbShipSlotForBan;

	public NKCUIOperatorSelectListSlot m_pfbOperatorSlotForBan;

	private NKM_UNIT_TYPE m_SelectTabType = NKM_UNIT_TYPE.NUT_NORMAL;

	private List<int> m_lstCastingVotedUnits = new List<int>();

	private bool m_bCastingBan;

	private bool m_bCheckCastingBanRemainTime = true;

	private NKCUIUnitSelectList m_UIUnitSelectList;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupGauntletBanList";

	public static NKCPopupGauntletBan Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGauntletBan>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_POPUP_BANNED_LIST_CASTING", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupGauntletBan>();
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

	private NKCUIUnitSelectList UnitSelectList
	{
		get
		{
			if (m_UIUnitSelectList == null)
			{
				m_UIUnitSelectList = NKCUIUnitSelectList.OpenNewInstance(bWillCloseUnderPopupOnOpen: false);
			}
			return m_UIUnitSelectList;
		}
	}

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

	public void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetEventTriggerDelegate(m_etBG, base.Close);
		NKCUtil.SetBindFunction(m_csbtnVote, OnClickSelectList);
		NKCUtil.SetBindFunction(m_csbtnClose, base.Close);
		if (null != m_lvsrUnit)
		{
			m_lvsrUnit.dOnGetObject += GetUnitSlot;
			m_lvsrUnit.dOnReturnObject += ReturnUnitSlot;
			m_lvsrUnit.dOnProvideData += ProvideUnitSlotData;
			NKCUtil.SetScrollHotKey(m_lvsrUnit);
			m_lvsrUnit.PrepareCells();
		}
		if (null != m_lvsrShip)
		{
			m_lvsrShip.dOnGetObject += GetShipSlot;
			m_lvsrShip.dOnReturnObject += ReturnShipSlot;
			m_lvsrShip.dOnProvideData += ProvideShipSlotData;
			NKCUtil.SetScrollHotKey(m_lvsrShip);
			m_lvsrShip.PrepareCells();
		}
		if (null != m_lvsrOper)
		{
			m_lvsrOper.dOnGetObject += GetOperSlot;
			m_lvsrOper.dOnReturnObject += ReturnOperSlot;
			m_lvsrOper.dOnProvideData += ProvideOperSlotData;
			NKCUtil.SetScrollHotKey(m_lvsrOper);
			m_lvsrOper.PrepareCells();
		}
		NKCUtil.SetGameobjectActive(m_ctglOper.gameObject, NKCOperatorUtil.IsActiveCastingBan() && m_bCastingBan);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglUnit, delegate
		{
			OnChangeTab(NKM_UNIT_TYPE.NUT_NORMAL);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglShip, delegate
		{
			OnChangeTab(NKM_UNIT_TYPE.NUT_SHIP);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglOper, delegate
		{
			OnChangeTab(NKM_UNIT_TYPE.NUT_OPERATOR);
		});
	}

	private RectTransform GetUnitSlot(int index)
	{
		NKCUIUnitSelectListSlot nKCUIUnitSelectListSlot = Object.Instantiate(m_pfbUnitSlotForBan);
		nKCUIUnitSelectListSlot.Init();
		NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlot, bValue: true);
		nKCUIUnitSelectListSlot.transform.localScale = Vector3.one;
		return nKCUIUnitSelectListSlot.GetComponent<RectTransform>();
	}

	private void ReturnUnitSlot(Transform go)
	{
		go.SetParent(base.transform);
		Object.Destroy(go.gameObject);
	}

	private void ProvideUnitSlotData(Transform tr, int idx)
	{
		NKCUIUnitSelectListSlotBase component = tr.GetComponent<NKCUIUnitSelectListSlotBase>();
		if (!(component == null))
		{
			if (idx < 0 || idx >= m_lstCastingVotedUnits.Count)
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(component, bValue: true);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_lstCastingVotedUnits[idx]);
			component.SetEnableShowBan(bSet: false);
			component.SetDataForBan(unitTempletBase, bEnableLayoutElement: true, null, bUp: false, bSetOriginalCost: true);
			component.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		}
	}

	private RectTransform GetShipSlot(int index)
	{
		NKCUIShipSelectListSlot nKCUIShipSelectListSlot = Object.Instantiate(m_pfbShipSlotForBan);
		nKCUIShipSelectListSlot.Init();
		NKCUtil.SetGameobjectActive(nKCUIShipSelectListSlot, bValue: true);
		nKCUIShipSelectListSlot.transform.localScale = Vector3.one;
		return nKCUIShipSelectListSlot.GetComponent<RectTransform>();
	}

	private void ReturnShipSlot(Transform go)
	{
		go.SetParent(base.transform);
		Object.Destroy(go.gameObject);
	}

	private void ProvideShipSlotData(Transform tr, int idx)
	{
		NKCUIUnitSelectListSlotBase component = tr.GetComponent<NKCUIUnitSelectListSlotBase>();
		if (!(component == null))
		{
			if (idx < 0 || idx >= m_lstCastingVotedUnits.Count)
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(component, bValue: true);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_lstCastingVotedUnits[idx]);
			component.SetEnableShowBan(bSet: false);
			component.SetDataForBan(unitTempletBase, bEnableLayoutElement: true, null);
		}
	}

	private RectTransform GetOperSlot(int index)
	{
		NKCUIOperatorSelectListSlot nKCUIOperatorSelectListSlot = Object.Instantiate(m_pfbOperatorSlotForBan);
		nKCUIOperatorSelectListSlot.Init();
		NKCUtil.SetGameobjectActive(nKCUIOperatorSelectListSlot, bValue: true);
		nKCUIOperatorSelectListSlot.transform.localScale = Vector3.one;
		return nKCUIOperatorSelectListSlot.GetComponent<RectTransform>();
	}

	private void ReturnOperSlot(Transform go)
	{
		go.SetParent(base.transform);
		Object.Destroy(go.gameObject);
	}

	private void ProvideOperSlotData(Transform tr, int idx)
	{
		NKCUIOperatorSelectListSlot component = tr.GetComponent<NKCUIOperatorSelectListSlot>();
		if (!(component == null))
		{
			if (idx < 0 || idx >= m_lstCastingVotedUnits.Count)
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetEnableShowBan(bSet: false);
			component.SetDataForBan(NKCOperatorUtil.GetDummyOperator(m_lstCastingVotedUnits[idx]), bEnableLayoutElement: true, null);
			component.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		UnitSelectList?.Close();
		m_UIUnitSelectList = null;
	}

	public void Open(bool bIsCastingBan = false)
	{
		m_ctglUnit.Select(bSelect: true, bForce: true);
		m_SelectTabType = NKM_UNIT_TYPE.NUT_NORMAL;
		m_bCheckCastingBanRemainTime = (m_bCastingBan = bIsCastingBan);
		UpdateUI();
		UIOpened();
	}

	public void OnChangeTab(NKM_UNIT_TYPE newTab)
	{
		if (m_SelectTabType != newTab)
		{
			m_SelectTabType = newTab;
			UpdateUI();
		}
	}

	public void UpdateUI()
	{
		bool flag = false;
		if (m_SelectTabType == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			m_lstCastingVotedUnits.Clear();
			m_lstCastingVotedUnits = GetBanUnitList(m_SelectTabType).ToList().FindAll((int e) => e > 0);
			flag = m_lstCastingVotedUnits.Count > 0;
			if (flag)
			{
				m_lstCastingVotedUnits.Sort();
				m_lvsrUnit.TotalCount = m_lstCastingVotedUnits.Count;
				m_lvsrUnit.RefreshCells(bForce: true);
			}
			m_lvsrUnit.SetIndexPosition(0);
		}
		else if (m_SelectTabType == NKM_UNIT_TYPE.NUT_SHIP)
		{
			m_lstCastingVotedUnits.Clear();
			List<int> unitList = NKCCollectionManager.GetUnitList(NKM_UNIT_TYPE.NUT_SHIP);
			foreach (int ShipGroupID in GetBanUnitList(m_SelectTabType))
			{
				List<NKMUnitTempletBase> list = NKMUnitTempletBase.Values.Where((NKMUnitTempletBase e) => e.m_ShipGroupID == ShipGroupID).ToList();
				if (list.Count <= 0)
				{
					continue;
				}
				foreach (int collectionShips in unitList)
				{
					NKMUnitTempletBase nKMUnitTempletBase = list.Find((NKMUnitTempletBase id) => id.m_UnitID == collectionShips);
					if (nKMUnitTempletBase != null)
					{
						m_lstCastingVotedUnits.Add(collectionShips);
						break;
					}
				}
			}
			flag = m_lstCastingVotedUnits.Count > 0;
			if (flag)
			{
				m_lstCastingVotedUnits.Sort();
				m_lvsrShip.TotalCount = m_lstCastingVotedUnits.Count;
				m_lvsrShip.RefreshCells(bForce: true);
			}
			m_lvsrShip.SetIndexPosition(0);
		}
		else if (m_SelectTabType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			m_lstCastingVotedUnits.Clear();
			m_lstCastingVotedUnits = GetBanUnitList(m_SelectTabType).ToList().FindAll((int e) => e > 0);
			flag = m_lstCastingVotedUnits.Count > 0;
			if (flag)
			{
				m_lstCastingVotedUnits.Sort();
				m_lvsrOper.TotalCount = m_lstCastingVotedUnits.Count;
				m_lvsrOper.RefreshCells(bForce: true);
			}
			m_lvsrOper.SetIndexPosition(0);
		}
		string msg = "";
		switch (m_SelectTabType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			msg = NKCUtilString.GET_STRING_GAUNTLET_CASTING_BAN_SELECT_UNIT;
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
			msg = NKCUtilString.GET_STRING_GAUNTLET_CASTING_BAN_SELECT_SHIP;
			break;
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			msg = NKCUtilString.GET_STRING_GAUNTLET_CASTING_BAN_SELECT_OPER;
			break;
		}
		NKCUtil.SetLabelText(m_lbVote, msg);
		NKCUtil.SetLabelText(m_lbVoteLock, msg);
		NKCUtil.SetGameobjectActive(m_objUnitList, m_SelectTabType == NKM_UNIT_TYPE.NUT_NORMAL && flag);
		NKCUtil.SetGameobjectActive(m_objShipList, m_SelectTabType == NKM_UNIT_TYPE.NUT_SHIP && flag);
		NKCUtil.SetGameobjectActive(m_objOperList, m_SelectTabType == NKM_UNIT_TYPE.NUT_OPERATOR && flag);
		NKCUtil.SetGameobjectActive(m_objNone, !flag);
		NKCUtil.SetGameobjectActive(m_objRemainTime, m_bCastingBan);
		NKCUtil.SetLabelText(m_lbSubTitle, m_bCastingBan ? NKCUtilString.GET_STRING_PVP_CASTING_BAN_SELECT_STATUS_SUB_TEXT : NKCUtilString.GET_STRING_GAUNTLET_GLOBAL_BAN_POPUP_SUB_TITLE);
	}

	private List<int> GetBanUnitList(NKM_UNIT_TYPE unitType)
	{
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			if (m_bCastingBan)
			{
				return NKCBanManager.m_CastingVoteData.unitIdList;
			}
			return NKCBanManager.m_GlobalVoteData.unitIdList;
		case NKM_UNIT_TYPE.NUT_SHIP:
			if (m_bCastingBan)
			{
				return NKCBanManager.m_CastingVoteData.shipGroupIdList;
			}
			return NKCBanManager.m_GlobalVoteData.shipGroupIdList;
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			if (m_bCastingBan)
			{
				return NKCBanManager.m_CastingVoteData.operatorIdList;
			}
			return NKCBanManager.m_GlobalVoteData.operatorIdList;
		default:
			return new List<int>();
		}
	}

	private void Update()
	{
		if (m_bCheckCastingBanRemainTime)
		{
			UpdateRemainTimeUI();
		}
	}

	private void UpdateRemainTimeUI()
	{
		if (NKCPVPManager.GetPvpRankSeasonTemplet(NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime())) != null)
		{
			if (!NKCUIGauntletLobbyRightSideRank.CheckCanPlayPVPRankGame())
			{
				m_bCheckCastingBanRemainTime = false;
				m_csbtnVote.SetLock(!m_bCheckCastingBanRemainTime);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbRemainTime, string.Format(NKCUtilString.GET_STRING_GAUNTLET_THIS_WEEK_LEAGUE_CASTING_BEN_ONE_PARAM, NKCUtilString.GetRemainTimeStringForGauntletWeekly()));
			}
		}
	}

	private void OnClickSelectList()
	{
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(m_SelectTabType, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL, NKCUIUnitSelectList.eUnitSelectListMode.CUSTOM_LIST);
		options.setDuplicateUnitID = null;
		options.setExcludeUnitUID = null;
		options.bExcludeLockedUnit = false;
		options.bExcludeDeckedUnit = false;
		options.strUpsideMenuName = (m_bCastingBan ? NKCUtilString.GET_STRING_GAUNTLET_CASTING_BAN_SELECT_LIST_TITLE : NKCUtilString.GET_STRING_GAUNTLET_GLOBAL_BAN_SELECT_UNIT_DESC);
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(m_SelectTabType, bIsCollection: false);
		options.bDescending = false;
		options.bShowRemoveSlot = false;
		options.iMaxMultipleSelect = GetBanUnitCount();
		if (m_bCastingBan)
		{
			options.MaxUnitNotSelectMsg = NKCUtilString.GET_STRING_GAUNTLET_CASTING_BAN_SELECT_COMPLET;
		}
		else
		{
			options.MaxUnitNotSelectMsg2 = NKCUtilString.GET_STRING_GAUNTLET_GLOBAL_BAN_NO_MAX_SELECT;
		}
		options.m_SortOptions.bUseDeckedState = true;
		options.m_SortOptions.bUseLockedState = true;
		options.m_SortOptions.bUseDormInState = true;
		options.m_SortOptions.bIncludeSeizure = false;
		options.m_SortOptions.bIgnoreWorldMapLeader = false;
		options.bShowHideDeckedUnitMenu = false;
		options.bHideDeckedUnit = false;
		options.dOnAutoSelectFilter = null;
		options.bUseRemoveSmartAutoSelect = false;
		options.setSelectedUnitUID = new HashSet<long>();
		options.bCanSelectUnitInMission = false;
		options.dOnClose = null;
		options.bPushBackUnselectable = false;
		options.setUnitFilterCategory = null;
		options.setUnitSortCategory = null;
		if (m_SelectTabType == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			options.setUnitFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
			{
				NKCUnitSortSystem.eFilterCategory.UnitType,
				NKCUnitSortSystem.eFilterCategory.UnitRole,
				NKCUnitSortSystem.eFilterCategory.UnitMoveType,
				NKCUnitSortSystem.eFilterCategory.UnitTargetType,
				NKCUnitSortSystem.eFilterCategory.Rarity,
				NKCUnitSortSystem.eFilterCategory.Cost
			};
			List<NKCUnitSortSystem.eSortOption> list = new List<NKCUnitSortSystem.eSortOption>();
			list.Add(NKCUnitSortSystem.eSortOption.Rarity_High);
			list.Add(NKCUnitSortSystem.eSortOption.Unit_SummonCost_High);
			options.lstSortOption = NKCUnitSortSystem.AddDefaultSortOptions(list, m_SelectTabType, bIsCollection: false);
			options.setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>
			{
				NKCUnitSortSystem.eSortCategory.Rarity,
				NKCUnitSortSystem.eSortCategory.UnitSummonCost
			};
		}
		else if (m_SelectTabType == NKM_UNIT_TYPE.NUT_SHIP)
		{
			options.setUnitFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
			{
				NKCUnitSortSystem.eFilterCategory.ShipType,
				NKCUnitSortSystem.eFilterCategory.Rarity
			};
			List<NKCUnitSortSystem.eSortOption> list2 = new List<NKCUnitSortSystem.eSortOption>();
			list2.Add(NKCUnitSortSystem.eSortOption.Rarity_High);
			options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>();
			options.lstSortOption = NKCUnitSortSystem.AddDefaultSortOptions(list2, m_SelectTabType, bIsCollection: false);
			options.setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory> { NKCUnitSortSystem.eSortCategory.Rarity };
			options.setShipFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
			{
				NKCUnitSortSystem.eFilterCategory.ShipType,
				NKCUnitSortSystem.eFilterCategory.Rarity
			};
		}
		else if (m_SelectTabType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			options.setOperatorFilterCategory = new HashSet<NKCOperatorSortSystem.eFilterCategory> { NKCOperatorSortSystem.eFilterCategory.Rarity };
			options.lstOperatorSortOption = new List<NKCOperatorSortSystem.eSortOption> { NKCOperatorSortSystem.eSortOption.Rarity_High };
			options.setOperatorSortCategory = new HashSet<NKCOperatorSortSystem.eSortCategory> { NKCOperatorSortSystem.eSortCategory.Rarity };
		}
		if (m_lstCastingVotedUnits.Count > 0)
		{
			foreach (int lstCastingVotedUnit in m_lstCastingVotedUnits)
			{
				options.setSelectedUnitUID.Add(lstCastingVotedUnit);
			}
		}
		options.bShowBanMsg = true;
		options.bOpenedAtRearmExtract = true;
		options.m_bHideUnitCount = true;
		List<int> unitList = NKCCollectionManager.GetUnitList(m_SelectTabType);
		options.setOnlyIncludeUnitID = new HashSet<int>();
		foreach (int item in unitList)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item);
			if (unitTempletBase != null && unitTempletBase.PickupEnableByTag && (!m_bCastingBan || m_SelectTabType != NKM_UNIT_TYPE.NUT_NORMAL || !NKCBanManager.IsBanUnit(item) || NKCBanManager.GetUnitBanLevel(item) < NKMCommonConst.MaxCastingBanLevel) && (m_SelectTabType == NKM_UNIT_TYPE.NUT_OPERATOR || m_bCastingBan || unitTempletBase.m_NKM_UNIT_GRADE >= NKM_UNIT_GRADE.NUG_SR) && (!m_bCastingBan || m_SelectTabType != NKM_UNIT_TYPE.NUT_SHIP || !NKCBanManager.IsBanShip(unitTempletBase.m_ShipGroupID) || NKCBanManager.GetShipBanLevel(unitTempletBase.m_ShipGroupID) < NKMCommonConst.MaxCastingBanLevel) && (!m_bCastingBan || m_SelectTabType != NKM_UNIT_TYPE.NUT_OPERATOR || !NKCBanManager.IsBanOperator(item) || NKCBanManager.GetOperBanLevel(item) < NKMCommonConst.MaxCastingBanLevel))
			{
				options.setOnlyIncludeUnitID.Add(item);
			}
		}
		UnitSelectList.Open(options, OnSelectedUnits);
	}

	private int GetBanUnitCount()
	{
		if (m_bCastingBan)
		{
			return NKMCommonConst.CustomCastingVoteCount;
		}
		return m_SelectTabType switch
		{
			NKM_UNIT_TYPE.NUT_NORMAL => 2, 
			_ => 1, 
		};
	}

	private void OnSelectedUnits(List<long> lstUnits)
	{
		if (UnitSelectList.IsOpen)
		{
			UnitSelectList.Close();
		}
		if (m_bCastingBan)
		{
			switch (m_SelectTabType)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				NKCPacketSender.Send_NKMPacket_PVP_CASTING_VOTE_UNIT_REQ(lstUnits.ConvertAll((long i) => (int)i));
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
			{
				List<int> list = new List<int>();
				foreach (long lstUnit in lstUnits)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase((int)lstUnit);
					if (unitTempletBase != null)
					{
						list.Add(unitTempletBase.m_ShipGroupID);
					}
				}
				NKCPacketSender.Send_NKMPacket_PVP_CASTING_VOTE_SHIP_REQ(list);
				break;
			}
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				NKCPacketSender.Send_NKMPacket_PVP_CASTING_VOTE_OPERATOR_REQ(lstUnits.ConvertAll((long i) => (int)i));
				break;
			}
			return;
		}
		switch (m_SelectTabType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			NKCPacketSender.Send_NKMPacket_DRAFT_PVP_CASTING_VOTE_UNIT_REQ(lstUnits.ConvertAll((long i) => (int)i));
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
		{
			List<int> list2 = new List<int>();
			foreach (long lstUnit2 in lstUnits)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase((int)lstUnit2);
				if (unitTempletBase2 != null)
				{
					list2.Add(unitTempletBase2.m_ShipGroupID);
				}
			}
			NKCPacketSender.Send_NKMPacket_DRAFT_PVP_CASTING_VOTE_SHIP_REQ(list2);
			break;
		}
		default:
			Debug.Log($"<color=red>NKCPopupGauntletBan::OnSelectedUnits - Not Support Type : {m_SelectTabType} </color>");
			break;
		}
	}
}
