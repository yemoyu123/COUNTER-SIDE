using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using Cs.Core.Util;
using Cs.Logging;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupTournamentBan : NKCUIBase
{
	private enum TAB_TYPE
	{
		TAB_NONE,
		TAB_GLOBAL,
		TAB_FINAL,
		TAB_CASTING
	}

	private const string ASSET_BUNDLE_NAME = "UI_SINGLE_TOURNAMENT";

	private const string UI_ASSET_NAME = "UI_SINGLE_POPUP_TOURNAMENT_BAN";

	private static NKCPopupTournamentBan m_Instance;

	public TMP_Text m_lbTitle;

	public Text m_lbDesc;

	public NKCUIComStateButton m_btnClose;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public NKCUIComToggle m_tglGlobalBan;

	public NKCUIComToggle m_tglFinalBan;

	public NKCUIComToggle m_tglCastingBan;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotFinalUnit;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotFinalShip;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotCastingUnit;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotCastingShip;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public ScrollRect m_lvsrUnit;

	public NKCPopupTournamentBanSlot m_pfbBanSlot;

	public GameObject m_objEmpty;

	public Text m_lbEmpty;

	public NKCUIComStateButton m_btnBan;

	public Text m_lbBanRemainTime;

	private PvpCastingVoteData m_PvpCastingVoteData = new PvpCastingVoteData();

	private Dictionary<int, NKMTournamentBanResult> m_TournamentBanResult = new Dictionary<int, NKMTournamentBanResult>();

	private Stack<NKCPopupTournamentBanSlot> m_stkBanSlot = new Stack<NKCPopupTournamentBanSlot>();

	private List<NKCPopupTournamentBanSlot> m_lstBanSlot = new List<NKCPopupTournamentBanSlot>();

	private Dictionary<NKMTournamentCountryCode, List<int>> m_dicBanId = new Dictionary<NKMTournamentCountryCode, List<int>>();

	private const int MAX_BAN_SELECT_COUNT = 3;

	private TAB_TYPE m_curTabType;

	private NKM_UNIT_TYPE m_SelectSubTabType;

	private Dictionary<TAB_TYPE, List<NKCUIPopupGuideSubSlot>> m_dicBanMainTab = new Dictionary<TAB_TYPE, List<NKCUIPopupGuideSubSlot>>();

	private bool m_bNeedUpdate;

	private DateTime m_tVoteEndDate;

	private NKCUIUnitSelectList m_UIUnitSelectList;

	private float m_fDeltaTIme;

	public static NKCPopupTournamentBan Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupTournamentBan>("UI_SINGLE_TOURNAMENT", "UI_SINGLE_POPUP_TOURNAMENT_BAN", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupTournamentBan>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

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
		while (m_Instance.m_stkBanSlot.Count > 0)
		{
			UnityEngine.Object.Destroy(m_Instance.m_stkBanSlot.Pop());
		}
		m_Instance = null;
	}

	private void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		NKCUtil.SetScrollHotKey(m_lvsrUnit);
		m_btnBan.PointerClick.RemoveAllListeners();
		m_btnBan.PointerClick.AddListener(OnClickSelectList);
		NKCUtil.SetToggleValueChangedDelegate(m_tglGlobalBan, delegate
		{
			OnClickTab(TAB_TYPE.TAB_GLOBAL);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_tglFinalBan, delegate
		{
			OnClickTab(TAB_TYPE.TAB_FINAL);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_tglCastingBan, delegate
		{
			OnClickTab(TAB_TYPE.TAB_CASTING);
		});
		InitLeftMenu();
	}

	private void InitLeftMenu()
	{
		string title = NKCStringTable.GetString("SI_PVP_POPUP_BAN_LIST_MENU_BAN_UNIT");
		string title2 = NKCStringTable.GetString("SI_PVP_POPUP_BAN_LIST_MENU_BAN_SHIP");
		m_GuideSubSlotFinalUnit.Init(title, NKM_UNIT_TYPE.NUT_NORMAL.ToString(), OnClickedSubSlot);
		m_GuideSubSlotFinalShip.Init(title2, NKM_UNIT_TYPE.NUT_SHIP.ToString(), OnClickedSubSlot);
		m_GuideSubSlotCastingUnit.Init(title, NKM_UNIT_TYPE.NUT_NORMAL.ToString(), OnClickedSubSlot);
		m_GuideSubSlotCastingShip.Init(title2, NKM_UNIT_TYPE.NUT_SHIP.ToString(), OnClickedSubSlot);
		List<NKCUIPopupGuideSubSlot> value = new List<NKCUIPopupGuideSubSlot>();
		List<NKCUIPopupGuideSubSlot> value2 = new List<NKCUIPopupGuideSubSlot> { m_GuideSubSlotFinalUnit, m_GuideSubSlotFinalShip };
		List<NKCUIPopupGuideSubSlot> value3 = new List<NKCUIPopupGuideSubSlot> { m_GuideSubSlotCastingUnit, m_GuideSubSlotCastingShip };
		m_dicBanMainTab.Add(TAB_TYPE.TAB_GLOBAL, value);
		m_dicBanMainTab.Add(TAB_TYPE.TAB_FINAL, value2);
		m_dicBanMainTab.Add(TAB_TYPE.TAB_CASTING, value3);
	}

	public override void CloseInternal()
	{
		ReturnAllSlot();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private NKCPopupTournamentBanSlot GetSlot()
	{
		NKCPopupTournamentBanSlot nKCPopupTournamentBanSlot = null;
		nKCPopupTournamentBanSlot = ((m_stkBanSlot.Count <= 0) ? UnityEngine.Object.Instantiate(m_pfbBanSlot, m_lvsrUnit.content) : m_stkBanSlot.Pop());
		NKCUtil.SetGameobjectActive(nKCPopupTournamentBanSlot.gameObject, bValue: false);
		return nKCPopupTournamentBanSlot;
	}

	private void ReturnAllSlot()
	{
		for (int i = 0; i < m_lstBanSlot.Count; i++)
		{
			NKCPopupTournamentBanSlot nKCPopupTournamentBanSlot = m_lstBanSlot[i];
			if (!(nKCPopupTournamentBanSlot == null))
			{
				m_stkBanSlot.Push(nKCPopupTournamentBanSlot);
				NKCUtil.SetGameobjectActive(nKCPopupTournamentBanSlot.gameObject, bValue: false);
			}
		}
		m_lstBanSlot.Clear();
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_PvpCastingVoteData = NKCTournamentManager.m_TournamentInfo.pvpCastingVoteData;
		m_TournamentBanResult = NKCTournamentManager.m_TournamentInfo.tournamentBanResult;
		m_curTabType = TAB_TYPE.TAB_FINAL;
		m_SelectSubTabType = NKM_UNIT_TYPE.NUT_NORMAL;
		m_tglFinalBan.Select(bSelect: true, bForce: true);
		OnClickTab(m_curTabType, bForce: true);
		UIOpened();
	}

	private void OnClickTab(TAB_TYPE tabType, bool bForce = false)
	{
		if (m_curTabType != tabType || bForce)
		{
			m_curTabType = tabType;
			OnClickedSubSlot(NKM_UNIT_TYPE.NUT_NORMAL.ToString());
		}
	}

	public void OnClickedSubSlot(string ArticleID, int i = 0)
	{
		foreach (KeyValuePair<TAB_TYPE, List<NKCUIPopupGuideSubSlot>> item in m_dicBanMainTab)
		{
			foreach (NKCUIPopupGuideSubSlot item2 in item.Value)
			{
				NKCUtil.SetGameobjectActive(item2.gameObject, bValue: true);
				item2.OnActive(item.Key == m_curTabType);
				item2.OnSelectedObject(ArticleID);
			}
		}
		m_SelectSubTabType = (NKM_UNIT_TYPE)Enum.Parse(typeof(NKM_UNIT_TYPE), ArticleID);
		if (m_curTabType == TAB_TYPE.TAB_FINAL)
		{
			NKCUtil.SetGameobjectActive(m_lbDesc, bValue: true);
			if (m_SelectSubTabType == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				NKCUtil.SetLabelText(m_lbDesc, string.Format(NKCUtilString.GET_STRING_TOURNAMENT_BAN_INFO_UNIT, NKMCommonConst.TournamentBanHighUnitCount, NKCTournamentManager.m_TournamentTemplet.UnitBanCount - NKMCommonConst.TournamentBanHighUnitCount));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbDesc, string.Format(NKCUtilString.GET_STRING_TOURNAMENT_BAN_INFO_SHIP, NKMCommonConst.TournamentBanHighShipCount, NKCTournamentManager.m_TournamentTemplet.ShipBanCount - NKMCommonConst.TournamentBanHighShipCount));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbDesc, bValue: false);
		}
		UpdateRightUI(m_curTabType, ArticleID);
	}

	private void UpdateRightUI(TAB_TYPE tabType, string articleID)
	{
		if (NKCTournamentManager.GetTournamentState() == NKMTournamentState.BanVote && ServiceTime.Now < NKCTournamentManager.m_TournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.BanVote))
		{
			m_bNeedUpdate = true;
			m_tVoteEndDate = NKCTournamentManager.m_TournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.BanVote);
			UpdateBanTime();
			m_btnBan.UnLock();
		}
		else
		{
			m_bNeedUpdate = false;
			m_btnBan.Lock();
		}
		UpdateBanTime();
		NKCUtil.SetGameobjectActive(m_btnBan, tabType == TAB_TYPE.TAB_CASTING);
		NKCUtil.SetGameobjectActive(m_lbBanRemainTime, tabType == TAB_TYPE.TAB_CASTING);
		NKM_UNIT_TYPE nKM_UNIT_TYPE = (NKM_UNIT_TYPE)Enum.Parse(typeof(NKM_UNIT_TYPE), articleID);
		switch (nKM_UNIT_TYPE)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			m_dicBanId = null;
			switch (tabType)
			{
			case TAB_TYPE.TAB_GLOBAL:
				m_dicBanId = GetGlobalBanUnitIdList();
				break;
			case TAB_TYPE.TAB_CASTING:
				m_dicBanId = new Dictionary<NKMTournamentCountryCode, List<int>>();
				m_dicBanId.Add(NKMTournamentCountryCode.None, m_PvpCastingVoteData.unitIdList);
				m_dicBanId[NKMTournamentCountryCode.None].RemoveAll((int x) => x <= 0);
				break;
			default:
				m_dicBanId = GetFinalBanUnitIdList();
				break;
			}
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
			m_dicBanId = new Dictionary<NKMTournamentCountryCode, List<int>>();
			NKCCollectionManager.GetUnitList(NKM_UNIT_TYPE.NUT_SHIP);
			switch (tabType)
			{
			case TAB_TYPE.TAB_GLOBAL:
				Log.Error("\ufffd۷ι\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdԼ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd, \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdȵ\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Tournament/NKCPopupTournamentBan.cs", 298);
				break;
			case TAB_TYPE.TAB_CASTING:
				m_dicBanId.Add(NKMTournamentCountryCode.None, new List<int>());
				foreach (int ShipGroupId2 in m_PvpCastingVoteData.shipGroupIdList)
				{
					if (NKMUnitManager.GetUnitTempletBase(ShipGroupId2) != null)
					{
						m_dicBanId[NKMTournamentCountryCode.None].Add(ShipGroupId2);
						continue;
					}
					List<NKMUnitTempletBase> list2 = NKMUnitTempletBase.Values.Where((NKMUnitTempletBase e) => e.m_ShipGroupID == ShipGroupId2).ToList();
					if (list2.Count > 0)
					{
						m_dicBanId[NKMTournamentCountryCode.None].Add(list2[list2.Count - 1].m_UnitID);
					}
				}
				break;
			default:
				foreach (KeyValuePair<int, NKMTournamentBanResult> item in m_TournamentBanResult)
				{
					if (item.Value == null || item.Value.shipBanList == null || item.Value.shipBanList.Count <= 0)
					{
						continue;
					}
					m_dicBanId.Add((NKMTournamentCountryCode)item.Key, new List<int>());
					foreach (int ShipGroupId in item.Value.shipBanList)
					{
						if (NKMUnitManager.GetUnitTempletBase(ShipGroupId) != null)
						{
							m_dicBanId[(NKMTournamentCountryCode)item.Key].Add(ShipGroupId);
							continue;
						}
						List<NKMUnitTempletBase> list = NKMUnitTempletBase.Values.Where((NKMUnitTempletBase e) => e.m_ShipGroupID == ShipGroupId).ToList();
						if (list.Count > 0)
						{
							m_dicBanId[(NKMTournamentCountryCode)item.Key].Add(list[list.Count - 1].m_UnitID);
						}
					}
				}
				break;
			}
			break;
		}
		bool bValue = false;
		foreach (KeyValuePair<NKMTournamentCountryCode, List<int>> item2 in m_dicBanId)
		{
			item2.Value.Sort();
			if (item2.Value.Count == 0)
			{
				bValue = true;
			}
		}
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue);
		if (m_objEmpty != null && m_objEmpty.activeSelf)
		{
			switch (tabType)
			{
			case TAB_TYPE.TAB_CASTING:
				NKCUtil.SetLabelText(m_lbEmpty, NKCUtilString.GET_STRING_TOURNAMENT_BAN_NO_SELECT);
				break;
			case TAB_TYPE.TAB_FINAL:
				NKCUtil.SetLabelText(m_lbEmpty, NKCUtilString.GET_STRING_TOURNAMENT_BAN_NO_LIST);
				break;
			}
		}
		ReturnAllSlot();
		for (int num = 0; num <= 2; num++)
		{
			if (!m_dicBanId.ContainsKey((NKMTournamentCountryCode)num))
			{
				continue;
			}
			List<int> list3 = m_dicBanId[(NKMTournamentCountryCode)num];
			if (list3.Count > 0)
			{
				NKCPopupTournamentBanSlot slot = GetSlot();
				NKCUtil.SetGameobjectActive(slot.gameObject, bValue: true);
				m_lstBanSlot.Add(slot);
				NKMTournamentCountryCode countryCode = NKMTournamentCountryCode.None;
				if (NKCTournamentManager.m_TournamentTemplet.IsUnify)
				{
					countryCode = (NKMTournamentCountryCode)num;
				}
				slot.SetData(nKM_UNIT_TYPE, list3, GetBanDataTypeFromTabType(m_curTabType), countryCode);
			}
		}
		m_lvsrUnit.normalizedPosition = Vector2.zero;
	}

	private Dictionary<NKMTournamentCountryCode, List<int>> GetGlobalBanUnitIdList()
	{
		Dictionary<NKMTournamentCountryCode, List<int>> dictionary = new Dictionary<NKMTournamentCountryCode, List<int>>();
		if (NKCTournamentManager.m_TournamentTemplet == null)
		{
			return dictionary;
		}
		dictionary.Add(NKMTournamentCountryCode.None, new List<int>());
		for (int i = 0; i < NKCTournamentManager.m_TournamentTemplet.GlobalBanUnits.Count; i++)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(NKCTournamentManager.m_TournamentTemplet.GlobalBanUnits[i]);
			if (unitTempletBase != null && unitTempletBase.ContractEnableByTag)
			{
				dictionary[NKMTournamentCountryCode.None].Add(NKCTournamentManager.m_TournamentTemplet.GlobalBanUnits[i]);
			}
		}
		return dictionary;
	}

	private Dictionary<NKMTournamentCountryCode, List<int>> GetFinalBanUnitIdList()
	{
		Dictionary<NKMTournamentCountryCode, List<int>> dictionary = new Dictionary<NKMTournamentCountryCode, List<int>>();
		if (NKCTournamentManager.m_TournamentTemplet == null)
		{
			return dictionary;
		}
		foreach (KeyValuePair<int, NKMTournamentBanResult> item in m_TournamentBanResult)
		{
			if (NKCTournamentManager.IsEmpty(item.Value))
			{
				continue;
			}
			foreach (int unitBan in item.Value.unitBanList)
			{
				if (unitBan > 0)
				{
					if (!dictionary.ContainsKey((NKMTournamentCountryCode)item.Key))
					{
						dictionary.Add((NKMTournamentCountryCode)item.Key, new List<int>());
					}
					dictionary[(NKMTournamentCountryCode)item.Key].Add(unitBan);
				}
			}
		}
		return dictionary;
	}

	private NKCBanManager.BAN_DATA_TYPE GetBanDataTypeFromTabType(TAB_TYPE tabType)
	{
		return tabType switch
		{
			TAB_TYPE.TAB_CASTING => NKCBanManager.BAN_DATA_TYPE.CASTING, 
			TAB_TYPE.TAB_FINAL => NKCBanManager.BAN_DATA_TYPE.FINAL, 
			TAB_TYPE.TAB_GLOBAL => NKCBanManager.BAN_DATA_TYPE.FINAL, 
			_ => NKCBanManager.BAN_DATA_TYPE.FINAL, 
		};
	}

	private void OnClickSelectList()
	{
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(m_SelectSubTabType, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL, NKCUIUnitSelectList.eUnitSelectListMode.CUSTOM_LIST);
		options.setDuplicateUnitID = null;
		options.setExcludeUnitUID = null;
		options.bExcludeLockedUnit = false;
		options.bExcludeDeckedUnit = false;
		options.strUpsideMenuName = NKCUtilString.GET_STRING_GAUNTLET_CASTING_BAN_SELECT_LIST_TITLE;
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(m_SelectSubTabType, bIsCollection: false);
		options.bDescending = false;
		options.bShowRemoveSlot = false;
		options.iMaxMultipleSelect = 3;
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
		if (m_SelectSubTabType == NKM_UNIT_TYPE.NUT_NORMAL)
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
			options.lstSortOption = NKCUnitSortSystem.AddDefaultSortOptions(list, m_SelectSubTabType, bIsCollection: false);
			options.setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>
			{
				NKCUnitSortSystem.eSortCategory.Rarity,
				NKCUnitSortSystem.eSortCategory.UnitSummonCost
			};
		}
		else if (m_SelectSubTabType == NKM_UNIT_TYPE.NUT_SHIP)
		{
			options.setUnitFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
			{
				NKCUnitSortSystem.eFilterCategory.ShipType,
				NKCUnitSortSystem.eFilterCategory.Rarity
			};
			List<NKCUnitSortSystem.eSortOption> list2 = new List<NKCUnitSortSystem.eSortOption>();
			list2.Add(NKCUnitSortSystem.eSortOption.Rarity_High);
			options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>();
			options.lstSortOption = NKCUnitSortSystem.AddDefaultSortOptions(list2, m_SelectSubTabType, bIsCollection: false);
			options.setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory> { NKCUnitSortSystem.eSortCategory.Rarity };
			options.setShipFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
			{
				NKCUnitSortSystem.eFilterCategory.ShipType,
				NKCUnitSortSystem.eFilterCategory.Rarity
			};
		}
		if (m_dicBanId.Count > 0)
		{
			foreach (KeyValuePair<NKMTournamentCountryCode, List<int>> item in m_dicBanId)
			{
				for (int i = 0; i < item.Value.Count; i++)
				{
					options.setSelectedUnitUID.Add(item.Value[i]);
				}
			}
		}
		options.bShowBanMsg = true;
		options.bOpenedAtRearmExtract = true;
		options.m_bHideUnitCount = true;
		List<int> unitList = NKCCollectionManager.GetUnitList(m_SelectSubTabType);
		options.setOnlyIncludeUnitID = new HashSet<int>();
		foreach (int item2 in unitList)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item2);
			if (unitTempletBase != null && unitTempletBase.PickupEnableByTag && (!unitTempletBase.IsShip() || unitTempletBase.m_NKM_UNIT_GRADE >= NKM_UNIT_GRADE.NUG_SSR))
			{
				options.setOnlyIncludeUnitID.Add(item2);
			}
		}
		UnitSelectList.Open(options, OnSelectedUnits);
	}

	private void OnSelectedUnits(List<long> lstUnits)
	{
		if (UnitSelectList.IsOpen)
		{
			UnitSelectList.Close();
		}
		if (m_SelectSubTabType == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			NKCPacketSender.Send_NKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_REQ(lstUnits.ConvertAll((long i) => (int)i));
		}
		else
		{
			if (m_SelectSubTabType != NKM_UNIT_TYPE.NUT_SHIP)
			{
				return;
			}
			List<int> list = new List<int>();
			foreach (long lstUnit in lstUnits)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase((int)lstUnit);
				if (unitTempletBase != null)
				{
					list.Add(unitTempletBase.m_ShipGroupID);
				}
			}
			NKCPacketSender.Send_NKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_REQ(list);
		}
	}

	public void RefreshUI()
	{
		m_PvpCastingVoteData = NKCTournamentManager.m_TournamentInfo.pvpCastingVoteData;
		m_TournamentBanResult = NKCTournamentManager.m_TournamentInfo.tournamentBanResult;
		OnClickedSubSlot(m_SelectSubTabType.ToString());
	}

	private void UpdateBanTime()
	{
		if (ServiceTime.Now > m_tVoteEndDate)
		{
			m_bNeedUpdate = false;
			m_btnBan.Lock();
			NKCUtil.SetLabelText(m_lbBanRemainTime, NKCUtilString.GET_STRING_TOURNAMENT_BAN_TIMEOUT);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbBanRemainTime, string.Format(NKCUtilString.GET_STRING_GAUNTLET_THIS_WEEK_LEAGUE_CASTING_BEN_ONE_PARAM, NKCUtilString.GetRemainTimeStringEx(ServiceTime.ToUtcTime(m_tVoteEndDate))));
		}
	}

	private void Update()
	{
		if (m_bNeedUpdate)
		{
			m_fDeltaTIme += Time.deltaTime;
			if (m_fDeltaTIme > 1f)
			{
				m_fDeltaTIme -= 1f;
				UpdateBanTime();
			}
		}
	}
}
