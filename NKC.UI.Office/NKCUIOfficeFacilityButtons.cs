using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.Events;

namespace NKC.UI.Office;

public class NKCUIOfficeFacilityButtons : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnMinimap;

	[Header("연구소")]
	public GameObject m_objLabRoot;

	public NKCUIComStateButton m_csbtnLabUnitList;

	public NKCUIComStateButton m_csbtnLabUnitRearm;

	public NKCUIComStateButton m_csbtnLabUnitExtract;

	[Header("공방")]
	public GameObject m_objForgeRoot;

	public NKCUIComStateButton m_csbtnForgeUpgrade;

	public NKCUIComStateButton m_csbtnForgeEnhance;

	public NKCUIComStateButton m_csbtnForgeEquipList;

	public NKCUIComStateButton m_csbtnForgeBuild;

	public NKCUIComStateButton m_csbtnForgeFinishAll;

	[Header("격납고")]
	public GameObject m_objHangarRoot;

	public NKCUIComStateButton m_csbtnHangarBuild;

	public GameObject m_objHangarBuildReddot;

	public NKCUIComStateButton m_csbtnHangerShipList;

	[Header("사장실")]
	public GameObject m_objCEORoot;

	public NKCUIComStateButton m_csbtnCEOLifetime;

	public NKCUIComStateButton m_csbtnCEOScout;

	public NKCUIComStateButton m_csbtnJukeBox;

	public GameObject m_objCEOScoutReddot;

	private static int m_SelectUnitIndex = 0;

	private static List<NKMUnitData> m_UnitSortList = new List<NKMUnitData>();

	private static List<NKMOperator> m_OperatorSortList = new List<NKMOperator>();

	public void UpdateAlarm()
	{
		NKCUtil.SetGameobjectActive(m_objHangarBuildReddot, NKCAlarmManager.CheckHangarNotify(NKCScenManager.CurrentUserData()));
		NKCUtil.SetGameobjectActive(m_objCEOScoutReddot, NKCAlarmManager.CheckScoutNotify(NKCScenManager.CurrentUserData()));
	}

	public void Init(UnityAction OnClose)
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnMinimap, OnClose);
		NKCUtil.SetButtonClickDelegate(m_csbtnLabUnitList, OnLabUnitList);
		NKCUtil.SetButtonClickDelegate(m_csbtnLabUnitRearm, OnLabUnitRearm);
		NKCUtil.SetButtonClickDelegate(m_csbtnLabUnitExtract, OnLabUnitExtract);
		if (!NKMOpenTagManager.IsOpened("EQUIP_UPGRADE"))
		{
			NKCUtil.SetGameobjectActive(m_csbtnForgeUpgrade, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnForgeUpgrade, bValue: true);
			if (NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_UPGRADE))
			{
				m_csbtnForgeUpgrade.UnLock();
			}
			else
			{
				m_csbtnForgeUpgrade.Lock();
			}
			NKCUtil.SetButtonClickDelegate(m_csbtnForgeUpgrade, OnClickUpgrade);
			m_csbtnForgeUpgrade.m_bGetCallbackWhileLocked = true;
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnForgeEnhance, OnForgeEnhance);
		NKCUtil.SetButtonClickDelegate(m_csbtnForgeEquipList, OnForgeEquipList);
		NKCUtil.SetButtonClickDelegate(m_csbtnForgeBuild, OnForgeBuild);
		NKCUtil.SetButtonClickDelegate(m_csbtnForgeFinishAll, OnForgeFinishAll);
		NKCUtil.SetButtonClickDelegate(m_csbtnHangarBuild, OnHangarBuild);
		NKCUtil.SetButtonClickDelegate(m_csbtnHangerShipList, OnHangerShipList);
		NKCUtil.SetButtonClickDelegate(m_csbtnCEOLifetime, OnBtnCEOLifetime);
		NKCUtil.SetButtonClickDelegate(m_csbtnCEOScout, OnCEOScout);
		NKCUtil.SetButtonClickDelegate(m_csbtnJukeBox, OnJukeBox);
		NKCUtil.SetGameobjectActive(m_csbtnLabUnitRearm.gameObject, NKCRearmamentUtil.IsCanUseContent());
		NKCUtil.SetGameobjectActive(m_csbtnLabUnitExtract.gameObject, NKCRearmamentUtil.CanUseExtract());
	}

	public void SetMode(NKMOfficeRoomTemplet.RoomType type)
	{
		NKCUtil.SetGameobjectActive(m_objLabRoot, type == NKMOfficeRoomTemplet.RoomType.Lab);
		NKCUtil.SetGameobjectActive(m_objForgeRoot, type == NKMOfficeRoomTemplet.RoomType.Forge);
		NKCUtil.SetGameobjectActive(m_objHangarRoot, type == NKMOfficeRoomTemplet.RoomType.Hangar);
		NKCUtil.SetGameobjectActive(m_objCEORoot, type == NKMOfficeRoomTemplet.RoomType.CEO);
		if (m_csbtnCEOLifetime != null)
		{
			m_csbtnCEOLifetime.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_LIFETIME));
		}
		if (m_csbtnCEOScout != null)
		{
			m_csbtnCEOScout.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_SCOUT));
		}
		if (m_csbtnForgeBuild != null)
		{
			m_csbtnForgeBuild.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_CRAFT));
		}
		if (m_csbtnForgeEnhance != null)
		{
			m_csbtnForgeEnhance.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT));
		}
		if (m_csbtnHangarBuild != null)
		{
			m_csbtnHangarBuild.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPBUILD));
		}
		if (m_csbtnLabUnitRearm != null)
		{
			m_csbtnLabUnitRearm.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.REARM));
		}
		if (m_csbtnLabUnitExtract != null)
		{
			m_csbtnLabUnitExtract.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.EXTRACT));
		}
		UpdateAlarm();
	}

	public void OnCEOScout()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_SCOUT))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.PERSONNAL_SCOUT);
		}
		else
		{
			NKCUIScout.Instance.Open();
		}
	}

	private void OnBtnCEOLifetime()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_LIFETIME))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.PERSONNAL_LIFETIME);
		}
		else
		{
			OnCEOLifetime(0L);
		}
	}

	public void OnCEOLifetime(long uid)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_LIFETIME))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.PERSONNAL_LIFETIME);
			return;
		}
		NKCUIPersonnel.Instance.Open();
		if (uid > 0)
		{
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(uid);
			NKCUIPersonnel.Instance.ReserveUnitData(unitFromUID);
		}
	}

	private void OnJukeBox()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_JUKEBOX_CONTENTS_UNLOCK);
		}
		else
		{
			NKCUIJukeBox.Instance.Open(bLobbyMusicSelectMode: false);
		}
	}

	public void OnHangerShipList()
	{
		NKCUIUnitSelectList.UnitSelectListOptions options = MakeUnitListOptions(NKM_UNIT_TYPE.NUT_SHIP);
		NKCUIUnitSelectList.Instance.Open(options, OpenUnitData, OnUnitSortList, OnOperatorSortList);
	}

	public void OnHangarBuild()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPBUILD))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.HANGER_SHIPBUILD);
		}
		else
		{
			NKCUIHangarBuild.Instance.Open();
		}
	}

	private void OnClickUpgrade()
	{
		if (NKMOpenTagManager.IsOpened("EQUIP_UPGRADE"))
		{
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
			}
			else if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_UPGRADE))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_UPGRADE);
			}
			else
			{
				NKCUIForgeUpgrade.Instance.Open();
			}
		}
	}

	private void OnForgeEnhance()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
		}
		else if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_ENCHANT);
		}
		else
		{
			NKCUIForge.Instance.Open(NKCUIForge.NKC_FORGE_TAB.NFT_ENCHANT, 0L);
		}
	}

	public void OnForgeEquipList()
	{
		NKCUIInventory.EquipSelectListOptions options = new NKCUIInventory.EquipSelectListOptions(NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL, _bMultipleSelect: false);
		options.lstSortOption = NKCEquipSortSystem.FORGE_TARGET_SORT_LIST;
		options.strEmptyMessage = NKCUtilString.GET_STRING_INVEN_MISC_NO_EXIST;
		options.m_dOnSelectedEquipSlot = delegate(NKCUISlotEquip slot, NKMEquipItemData data)
		{
			NKCPopupItemEquipBox.Open(data, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_ENFORCE_AND_EQUIP, delegate
			{
				SetLatestOpenNKMEquipItemData(data);
			});
		};
		NKCUIInventory.Instance.Open(options, null, 0L);
	}

	private void SetLatestOpenNKMEquipItemData(NKMEquipItemData equipItemData)
	{
		NKCUIInventory.Instance.SetLatestOpenNKMEquipItemDataAndOpenUnitSelect(equipItemData);
	}

	public void OnForgeBuild()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
		}
		else if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_CRAFT))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_CRAFT);
		}
		else if (NKCScenManager.GetScenManager().GetMyUserData().m_CraftData != null)
		{
			NKCUIForgeCraftMold.Instance.Open();
		}
	}

	public void OnForgeFinishAll()
	{
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_COMING_SOON_SYSTEM);
	}

	public void OnLabUnitList()
	{
		NKCUIUnitSelectList.UnitSelectListOptions options = MakeUnitListOptions(NKM_UNIT_TYPE.NUT_NORMAL);
		NKCUIUnitSelectList.Instance.Open(options, OpenUnitData, OnUnitSortList, OnOperatorSortList);
	}

	public void OnLabUnitRearm()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.REARM))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.REARM);
		}
		else
		{
			NKCUIRearmament.Instance.Open();
		}
	}

	public void OnLabUnitExtract()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.EXTRACT))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.EXTRACT);
		}
		else
		{
			NKCUIRearmament.Instance.Open(NKCUIRearmament.REARM_TYPE.RT_EXTRACT);
		}
	}

	private NKCUIUnitSelectList.UnitSelectListOptions MakeUnitListOptions(NKM_UNIT_TYPE type)
	{
		NKCUIUnitSelectList.UnitSelectListOptions result = new NKCUIUnitSelectList.UnitSelectListOptions(type, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
		result.bShowUnitShipChangeMenu = false;
		result.bEnableLockUnitSystem = false;
		result.bEnableRemoveUnitSystem = false;
		result.dOnAutoSelectFilter = FilterRemoveAuto;
		result.bUseRemoveSmartAutoSelect = true;
		result.bShowHideDeckedUnitMenu = false;
		result.bCanSelectUnitInMission = true;
		result.m_SortOptions.bIncludeSeizure = true;
		result.m_SortOptions.bIgnoreWorldMapLeader = true;
		result.m_OperatorSortOptions.SetBuildOption(true, BUILD_OPTIONS.INCLUDE_SEIZURE, BUILD_OPTIONS.IGNORE_WORLD_MAP_LEADER);
		result.strUpsideMenuName = NKCUtilString.GET_STRING_MANAGEMENT;
		result.bPushBackUnselectable = false;
		result.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		result.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		result.setShipFilterCategory = NKCUnitSortSystem.setDefaultShipFilterCategory;
		result.setShipSortCategory = NKCUnitSortSystem.setDefaultShipSortCategory;
		result.ShopShortcutTargetTab = null;
		result.setOperatorFilterCategory = NKCOperatorSortSystem.setDefaultOperatorFilterCategory;
		result.setOperatorSortCategory = NKCOperatorSortSystem.setDefaultOperatorSortCategory;
		result.m_bUseFavorite = true;
		return result;
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
				NKCUIUnitInfo.Instance.Open(unitOrShipFromUID, null, openOption);
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
}
