using System.Collections.Generic;
using NKC.UI;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKC_SCEN_UNIT_LIST : NKC_SCEN_BASIC
{
	public enum UNIT_LIST_TAB
	{
		ULT_NORMAL,
		ULT_SHIP,
		ULT_OPERATOR,
		ULT_TROPHY
	}

	public enum eUIOpenReserve
	{
		Nothing,
		ShipRepair,
		ShipModule,
		UnitInfo,
		UnitSkillTraining,
		UnitLimitbreak,
		UnitNegotiate
	}

	private UNIT_LIST_TAB m_reservedTab;

	private static int m_SelectUnitIndex = 0;

	private static List<NKMUnitData> m_UnitSortList = new List<NKMUnitData>();

	private static List<NKMOperator> m_OperatorSortList = new List<NKMOperator>();

	private eUIOpenReserve m_eUIOpenReserve;

	private long m_reserveUnitUID;

	public NKC_SCEN_UNIT_LIST()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_UNIT_LIST;
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_DAILY);
		options.bShowUnitShipChangeMenu = true;
		options.bEnableLockUnitSystem = true;
		options.bEnableRemoveUnitSystem = true;
		options.bEnableExtractOperatorSystem = false;
		options.dOnAutoSelectFilter = FilterRemoveAuto;
		options.bUseRemoveSmartAutoSelect = true;
		options.bShowHideDeckedUnitMenu = false;
		options.bCanSelectUnitInMission = true;
		options.m_SortOptions.bIncludeSeizure = true;
		options.m_SortOptions.bIgnoreWorldMapLeader = true;
		options.m_SortOptions.lstDeckTypeOrder = new List<NKM_DECK_TYPE> { NKM_DECK_TYPE.NDT_NORMAL };
		options.m_OperatorSortOptions.SetBuildOption(true, BUILD_OPTIONS.INCLUDE_SEIZURE, BUILD_OPTIONS.IGNORE_WORLD_MAP_LEADER);
		options.strUpsideMenuName = NKCUtilString.GET_STRING_MANAGEMENT;
		options.dOnClose = MoveToHomeScene;
		options.bPushBackUnselectable = false;
		options.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		options.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		options.setShipFilterCategory = NKCUnitSortSystem.setDefaultShipFilterCategory;
		options.setShipSortCategory = NKCUnitSortSystem.setDefaultShipSortCategory;
		options.ShopShortcutTargetTab = "TAB_EXCHANGE_REMOVE_CARD";
		options.setOperatorFilterCategory = NKCPopupFilterOperator.MakeDefaultFilterCategory(NKCPopupFilterOperator.FILTER_OPEN_TYPE.NORMAL);
		options.setOperatorSortCategory = NKCOperatorSortSystem.setDefaultOperatorSortCategory;
		options.m_bUseFavorite = true;
		NKCUIUnitSelectList.Instance.Open(options, OpenUnitData, OnUnitSortList, OnOperatorSortList);
		if (m_eUIOpenReserve != eUIOpenReserve.Nothing)
		{
			OpenUnitData(new List<long> { m_reserveUnitUID }, m_eUIOpenReserve);
			m_eUIOpenReserve = eUIOpenReserve.Nothing;
		}
		if (m_reservedTab != UNIT_LIST_TAB.ULT_NORMAL)
		{
			switch (m_reservedTab)
			{
			case UNIT_LIST_TAB.ULT_SHIP:
				NKCUIUnitSelectList.Instance.OnSelectShipMode(value: true);
				break;
			case UNIT_LIST_TAB.ULT_OPERATOR:
				NKCUIUnitSelectList.Instance.OnSelectOperatorMode(value: true);
				break;
			case UNIT_LIST_TAB.ULT_TROPHY:
				NKCUIUnitSelectList.Instance.OnSelectTrophyMode(value: true);
				break;
			}
			m_reservedTab = UNIT_LIST_TAB.ULT_NORMAL;
		}
	}

	private bool FilterRemoveAuto(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase != null)
		{
			if (unitData.m_UnitLevel > 1)
			{
				return false;
			}
			if (unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
			{
				return false;
			}
			NKM_UNIT_GRADE nKM_UNIT_GRADE = unitTempletBase.m_NKM_UNIT_GRADE;
			if ((uint)(nKM_UNIT_GRADE - 2) <= 2u)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private void MoveToHomeScene()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
	}

	public override void ScenEnd()
	{
		NKCUIUnitSelectList.CheckInstanceAndClose();
		base.ScenEnd();
	}

	public void SetReservedTab(UNIT_LIST_TAB tabType)
	{
		m_reservedTab = tabType;
	}

	private static void OpenUnitData(List<long> lstUnitUIDs)
	{
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		NKMUnitData unitOrShipFromUID = armyData.GetUnitOrShipFromUID(lstUnitUIDs[0]);
		if (unitOrShipFromUID != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitOrShipFromUID.m_UnitID);
			NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(m_UnitSortList, m_SelectUnitIndex);
			if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				NKCUIUnitInfo.Instance.Open(unitOrShipFromUID, OnRemoveUnit, openOption);
			}
			else if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				NKMDeckIndex shipDeckIndex = armyData.GetShipDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, unitOrShipFromUID.m_UnitUID);
				NKCUIShipInfo.Instance.Open(unitOrShipFromUID, shipDeckIndex, openOption);
			}
		}
		else
		{
			NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(lstUnitUIDs[0]);
			if (operatorData != null)
			{
				NKCUIOperatorInfo.OpenOption option = new NKCUIOperatorInfo.OpenOption(m_OperatorSortList, m_SelectUnitIndex);
				NKCUIOperatorInfo.Instance.Open(operatorData, option);
			}
		}
	}

	private void OpenUnitData(List<long> lstUnitUIDs, eUIOpenReserve reserveUI)
	{
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		NKMUnitData unitOrShipFromUID = armyData.GetUnitOrShipFromUID(lstUnitUIDs[0]);
		if (unitOrShipFromUID != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitOrShipFromUID.m_UnitID);
			NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(m_UnitSortList, m_SelectUnitIndex);
			if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				NKCUIUnitInfo.Instance.Open(unitOrShipFromUID, OnRemoveUnit, openOption, reserveUI);
			}
			else if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				NKMDeckIndex shipDeckIndex = armyData.GetShipDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, unitOrShipFromUID.m_UnitUID);
				NKCUIShipInfo.Instance.Open(unitOrShipFromUID, shipDeckIndex, openOption, reserveUI);
			}
		}
	}

	private static void OnUnitSortList(long UID, List<NKMUnitData> unitUIDList)
	{
		m_UnitSortList = unitUIDList;
		if (m_UnitSortList.Count <= 1)
		{
			return;
		}
		for (int i = 0; i < m_UnitSortList.Count; i++)
		{
			if (m_UnitSortList[i].m_UnitUID == UID)
			{
				m_SelectUnitIndex = i;
				break;
			}
		}
	}

	private static void OnOperatorSortList(long UID, List<NKMOperator> operatorUIDList)
	{
		m_OperatorSortList = operatorUIDList;
		if (m_OperatorSortList.Count <= 1)
		{
			return;
		}
		for (int i = 0; i < m_OperatorSortList.Count; i++)
		{
			if (m_OperatorSortList[i].uid == UID)
			{
				m_SelectUnitIndex = i;
				break;
			}
		}
	}

	public void OnUnitUpdate(long uid, NKMUnitData unitData)
	{
		int num = m_UnitSortList.FindIndex((NKMUnitData x) => x.m_UnitUID == uid);
		if (num >= 0 && num < m_UnitSortList.Count)
		{
			m_UnitSortList[num] = unitData;
		}
	}

	public static void OnRemoveUnit(NKMUnitData UnitData)
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitDeckPosition(NKM_DECK_TYPE.NDT_NORMAL, UnitData.m_UnitUID, out var unitDeckIndex, out var unitSlotIndex))
		{
			NKCPacketSender.Send_NKMPacket_DECK_UNIT_SET_REQ(unitDeckIndex, unitSlotIndex, 0L);
		}
	}

	public void SetOpenReserve(eUIOpenReserve UIToOpen, long unitUID = 0L, bool bForce = false)
	{
		if (bForce || !CheckIgnoreReservedUI(UIToOpen))
		{
			m_eUIOpenReserve = UIToOpen;
			m_reserveUnitUID = unitUID;
		}
	}

	private bool CheckIgnoreReservedUI(eUIOpenReserve UIToOpen)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_UNIT_LIST)
		{
			return false;
		}
		switch (UIToOpen)
		{
		case eUIOpenReserve.ShipRepair:
			return NKCUIShipInfo.IsInstanceOpen;
		case eUIOpenReserve.UnitInfo:
		case eUIOpenReserve.UnitSkillTraining:
		case eUIOpenReserve.UnitLimitbreak:
		case eUIOpenReserve.UnitNegotiate:
			return NKCUIUnitInfo.IsInstanceOpen;
		default:
			return false;
		}
	}
}
