using System.Collections.Generic;
using ClientPacket.Unit;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShipInfoRepair : MonoBehaviour
{
	public enum RepairState
	{
		None,
		LevelUp,
		Upgrade,
		LimitBreak
	}

	public class ShipRepairInfo
	{
		public readonly RepairState eRepairState;

		public readonly NKMUnitData ShipData;

		public readonly int iMaximumLevel;

		public readonly int iMinimumLevel;

		public int iTargetLevel;

		public int iNextShipID;

		public int iCurStar;

		public int iNextStar;

		public int iCurShipMaxLevel;

		public int iNextShipMaxLevel;

		public float fCurShipAtk;

		public float fNextShipAtk;

		public float fCurShipHP;

		public float fNextShipHP;

		public float fCurShipDef;

		public float fNextShipDef;

		public float fCurShipCritical;

		public float fNextShipCritical;

		public float fCurShipHit;

		public float fNextShipHit;

		public float fCurShipEvade;

		public float fNextShipEvade;

		public int iCurSkillCnt;

		public int iNeedCredit;

		public bool bCanTryLevelUp;

		public bool bPossibleLevelUp;

		public bool bPossibleUpgrade;

		public bool bPossibleLimitBreak;

		public Dictionary<int, int> dicMaterialList;

		public int iCurShipLevel => ShipData.m_UnitLevel;

		public ShipRepairInfo(long shipUID)
		{
			ShipData = NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(shipUID);
			if (ShipData == null)
			{
				return;
			}
			int num = NKCExpManager.GetUnitMaxLevel(ShipData);
			if (ShipData.m_UnitLevel < num)
			{
				eRepairState = RepairState.LevelUp;
			}
			else
			{
				NKMShipLimitBreakTemplet shipLimitBreakTemplet = NKMShipManager.GetShipLimitBreakTemplet(ShipData.m_UnitID, ShipData.m_LimitBreakLevel + 1);
				if (ShipData.m_UnitLevel == num && ShipData.m_UnitLevel >= 100)
				{
					if (shipLimitBreakTemplet != null)
					{
						eRepairState = RepairState.LimitBreak;
						num = shipLimitBreakTemplet.ShipLimitBreakMaxLevel;
					}
					else
					{
						eRepairState = RepairState.LevelUp;
						num = ShipData.m_UnitLevel;
					}
				}
				else
				{
					eRepairState = RepairState.Upgrade;
				}
			}
			iMinimumLevel = (iTargetLevel = ((num == iCurShipLevel) ? num : (iCurShipLevel + 1)));
			iMaximumLevel = num;
			dicMaterialList = new Dictionary<int, int>();
			UpdateShipData();
		}

		public void UpdateShipData()
		{
			if (ShipData != null)
			{
				UpdateMaterial();
				CheckCanTryLevelUp();
				UpdateShipAbility();
				UpdateUpgradeInfo();
				UpdateShipCredit();
			}
		}

		public int GetMaximumLevel()
		{
			int i = iCurShipLevel;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (; i < iCurShipMaxLevel; i++)
			{
				List<LevelupMaterial> shipLevelupMaterialList = NKMShipManager.GetShipLevelupTempletByLevel(iCurShipLevel, ShipData.GetUnitGrade(), ShipData.m_LimitBreakLevel).ShipLevelupMaterialList;
				for (int j = 0; j < shipLevelupMaterialList.Count; j++)
				{
					LevelupMaterial levelupMaterial = shipLevelupMaterialList[j];
					int levelupMaterialItemID = levelupMaterial.m_LevelupMaterialItemID;
					if (dictionary.ContainsKey(levelupMaterialItemID))
					{
						dictionary[levelupMaterialItemID] += levelupMaterial.m_LevelupMaterialCount;
					}
					else
					{
						dictionary.Add(levelupMaterialItemID, levelupMaterial.m_LevelupMaterialCount);
					}
				}
				NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
				foreach (KeyValuePair<int, int> item in dictionary)
				{
					if (nKMUserData.m_InventoryData.GetCountMiscItem(item.Key) < item.Value)
					{
						return i;
					}
				}
			}
			return i;
		}

		private void UpdateMaterial()
		{
			dicMaterialList.Clear();
			if (eRepairState == RepairState.LevelUp)
			{
				int i = iCurShipLevel;
				for (int num = iTargetLevel - 1; i <= num; i++)
				{
					List<LevelupMaterial> shipLevelupMaterialList = NKMShipManager.GetShipLevelupTempletByLevel(i, ShipData.GetUnitGrade(), ShipData.m_LimitBreakLevel).ShipLevelupMaterialList;
					for (int j = 0; j < shipLevelupMaterialList.Count; j++)
					{
						LevelupMaterial levelupMaterial = shipLevelupMaterialList[j];
						int levelupMaterialItemID = levelupMaterial.m_LevelupMaterialItemID;
						if (dicMaterialList.ContainsKey(levelupMaterialItemID))
						{
							dicMaterialList[levelupMaterialItemID] += levelupMaterial.m_LevelupMaterialCount;
						}
						else
						{
							dicMaterialList.Add(levelupMaterialItemID, levelupMaterial.m_LevelupMaterialCount);
						}
					}
				}
			}
			else if (eRepairState == RepairState.Upgrade)
			{
				if (NKMUnitManager.GetUnitTempletBase(ShipData.m_UnitID) == null)
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_HANGAR_SHIPYARD_CANNOT_CHANGE_SHIP);
					return;
				}
				NKMShipBuildTemplet shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(ShipData.m_UnitID);
				if (shipBuildTemplet == null)
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_HANGAR_SHIPYARD_CANNOT_FIND_INFORMATION);
					return;
				}
				shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(shipBuildTemplet.ShipUpgradeTarget1);
				if (shipBuildTemplet == null)
				{
					return;
				}
				for (int k = 0; k < shipBuildTemplet.UpgradeMaterialList.Count; k++)
				{
					UpgradeMaterial upgradeMaterial = shipBuildTemplet.UpgradeMaterialList[k];
					int shipUpgradeMaterial = upgradeMaterial.m_ShipUpgradeMaterial;
					if (dicMaterialList.ContainsKey(shipUpgradeMaterial))
					{
						dicMaterialList[shipUpgradeMaterial] += upgradeMaterial.m_ShipUpgradeMaterialCount;
					}
					else
					{
						dicMaterialList.Add(shipUpgradeMaterial, upgradeMaterial.m_ShipUpgradeMaterialCount);
					}
				}
			}
			else if (eRepairState == RepairState.LimitBreak)
			{
				NKMShipLimitBreakTemplet shipLimitBreakTemplet = NKMShipManager.GetShipLimitBreakTemplet(ShipData.m_UnitID, ShipData.m_LimitBreakLevel + 1);
				if (shipLimitBreakTemplet == null)
				{
					return;
				}
				for (int l = 0; l < shipLimitBreakTemplet.ShipLimitBreakItems.Count; l++)
				{
					int itemId = shipLimitBreakTemplet.ShipLimitBreakItems[l].ItemId;
					if (itemId != 1)
					{
						int count = shipLimitBreakTemplet.ShipLimitBreakItems[l].Count32;
						if (dicMaterialList.ContainsKey(itemId))
						{
							dicMaterialList[itemId] += count;
						}
						else
						{
							dicMaterialList.Add(itemId, count);
						}
					}
				}
			}
			else
			{
				Debug.LogError("something was warong - NKCUIShipInfoRepair::UpdateMaterial >> unkonw eRepairState");
			}
		}

		private void CheckCanTryLevelUp()
		{
			bCanTryLevelUp = false;
			if (eRepairState == RepairState.LevelUp)
			{
				iCurShipMaxLevel = NKMShipManager.GetShipMaxLevel(ShipData);
				int nextLevel = ((iTargetLevel >= iCurShipMaxLevel) ? (iCurShipMaxLevel - 1) : iTargetLevel);
				bCanTryLevelUp = NKMShipManager.CanShipLevelup(NKCScenManager.CurrentUserData(), ShipData, nextLevel) == NKM_ERROR_CODE.NEC_OK;
			}
		}

		private void UpdateShipAbility()
		{
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(ShipData.m_UnitID);
			if (unitStatTemplet != null)
			{
				fCurShipHP = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_HP, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, ShipData.m_UnitLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
				fCurShipAtk = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_ATK, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, ShipData.m_UnitLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
				fCurShipDef = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_DEF, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, ShipData.m_UnitLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
				fCurShipCritical = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_CRITICAL, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, ShipData.m_UnitLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
				fCurShipHit = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_HIT, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, ShipData.m_UnitLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
				fCurShipEvade = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_EVADE, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, ShipData.m_UnitLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
				if (eRepairState == RepairState.LevelUp)
				{
					fNextShipHP = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_HP, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, iTargetLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
					fNextShipAtk = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_ATK, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, iTargetLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
					fNextShipDef = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_DEF, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, iTargetLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
					fNextShipCritical = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_CRITICAL, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, iTargetLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
					fNextShipHit = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_HIT, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, iTargetLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
					fNextShipEvade = NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_EVADE, unitStatTemplet.m_StatData, ShipData.m_listStatEXP, iTargetLevel, ShipData.m_LimitBreakLevel, ShipData.GetMultiplierByPermanentContract(), null, null, 0, NKM_UNIT_TYPE.NUT_SHIP);
				}
			}
		}

		private void UpdateShipCredit()
		{
			if (eRepairState == RepairState.LevelUp)
			{
				int levelUpCredit = NKMShipLevelUpTemplet.GetLevelUpCredit(ShipData.GetStarGrade(), ShipData.GetUnitGrade(), ShipData.m_LimitBreakLevel);
				iNeedCredit = levelUpCredit * (iTargetLevel - iCurShipLevel);
			}
			else if (eRepairState == RepairState.Upgrade)
			{
				NKMShipBuildTemplet shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(iNextShipID);
				if (shipBuildTemplet != null)
				{
					iNeedCredit = shipBuildTemplet.ShipUpgradeCredit;
				}
			}
			else
			{
				if (eRepairState != RepairState.LimitBreak || ShipData == null || ShipData.m_UnitLevel < 100)
				{
					return;
				}
				NKMShipLimitBreakTemplet shipLimitBreakTemplet = NKMShipManager.GetShipLimitBreakTemplet(ShipData.m_UnitID, ShipData.m_LimitBreakLevel + 1);
				if (shipLimitBreakTemplet != null)
				{
					iNeedCredit = shipLimitBreakTemplet.ShipLimitBreakItems.Find((MiscItemUnit x) => x.ItemId == 1).Count32;
				}
			}
		}

		private void UpdateUpgradeInfo()
		{
			if (eRepairState == RepairState.Upgrade)
			{
				NKMShipBuildTemplet shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(ShipData.m_UnitID);
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipBuildTemplet.ShipUpgradeTarget1);
				NKM_UNIT_GRADE grade = NKM_UNIT_GRADE.NUG_N;
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(ShipData.m_UnitID);
				if (unitTempletBase2 != null)
				{
					grade = unitTempletBase2.m_NKM_UNIT_GRADE;
				}
				else
				{
					Debug.LogError($"유닛 정보(unit ID : {ShipData.m_UnitID})를 찾을 수 없습니다.");
				}
				iNextShipID = shipBuildTemplet.ShipUpgradeTarget1;
				iCurShipMaxLevel = NKMShipLevelUpTemplet.GetMaxLevel(ShipData.GetStarGrade(), grade, ShipData.m_LimitBreakLevel);
				iCurStar = ShipData.GetStarGrade();
				if (unitTempletBase != null)
				{
					iNextShipMaxLevel = NKMShipLevelUpTemplet.GetMaxLevel(unitTempletBase.m_StarGradeMax, grade, ShipData.m_LimitBreakLevel);
					iNextStar = unitTempletBase.m_StarGradeMax;
				}
				else
				{
					iNextShipMaxLevel = iCurShipMaxLevel;
					iNextStar = iCurStar;
				}
			}
			else if (eRepairState == RepairState.LimitBreak)
			{
				NKM_UNIT_GRADE grade2 = NKM_UNIT_GRADE.NUG_N;
				NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(ShipData.m_UnitID);
				if (unitTempletBase3 != null)
				{
					grade2 = unitTempletBase3.m_NKM_UNIT_GRADE;
				}
				else
				{
					Debug.LogError($"유닛 정보(unit ID : {ShipData.m_UnitID})를 찾을 수 없습니다.");
				}
				NKMShipLimitBreakTemplet shipLimitBreakTemplet = NKMShipManager.GetShipLimitBreakTemplet(ShipData.m_UnitID, ShipData.m_LimitBreakLevel);
				NKMShipLimitBreakTemplet shipLimitBreakTemplet2 = NKMShipManager.GetShipLimitBreakTemplet(ShipData.m_UnitID, ShipData.m_LimitBreakLevel + 1);
				if (shipLimitBreakTemplet != null)
				{
					iCurShipMaxLevel = shipLimitBreakTemplet.ShipLimitBreakMaxLevel;
				}
				else
				{
					iCurShipMaxLevel = NKMShipLevelUpTemplet.GetMaxLevel(ShipData.GetStarGrade(), grade2);
				}
				if (shipLimitBreakTemplet2 != null)
				{
					iNextShipMaxLevel = shipLimitBreakTemplet2.ShipLimitBreakMaxLevel;
				}
				else
				{
					iNextShipMaxLevel = NKMShipLevelUpTemplet.GetMaxLevel(ShipData.GetStarGrade(), grade2);
				}
				iNextShipID = ShipData.m_UnitID;
				iCurStar = ShipData.GetStarGrade();
				iNextStar = ShipData.GetStarGrade();
			}
			NKMUnitTempletBase unitTempletBase4 = NKMUnitManager.GetUnitTempletBase(ShipData.m_UnitID);
			iCurSkillCnt = unitTempletBase4.GetSkillCount();
		}
	}

	public GameObject m_objCommonRoot;

	public NKCUIHangarShipyardLevelup m_ShipLevelUp;

	public NKCUIHangarShipyardUpgrade m_ShipUpgrade;

	public NKCUIHangarShipyardLimitBreak m_ShipLimitBreak;

	[Header("작전능력")]
	public Text m_txt_NKM_UI_HANGAR_SHIPYARD_STAT_POWER_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_ATTACK_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_ATTACK_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_HP_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_HP_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_DEFENCE_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_DEFENCE_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_CRITICAL_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_CRITICAL_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_HIT_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_HIT_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_EVADE_TEXT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_EVADE_TEXT;

	[Header("함선 스킬")]
	public NKCUIShipSkillSlot[] m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT;

	[Header("이식 재료 함선")]
	public GameObject m_objCostShip;

	public NKCUISlot m_CostShipSlot;

	public NKCUIComStateButton m_btnEmptyShipSlot;

	[Header("개조 재료")]
	public Text m_lbCostName;

	public NKCUIItemCostSlot[] m_CostSlot;

	[Header("실행 버튼")]
	public NKCUIComStateButton m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp;

	public GameObject m_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_DISABLE;

	public GameObject m_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_LIGHT;

	public Text m_txt_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_TEXT;

	public Image m_img_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_ICON;

	public NKCUIComStateButton m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade;

	public GameObject m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_DISABLE;

	public GameObject m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_LIGHT;

	public Text m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_TEXT;

	public Image m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_ICON;

	public NKCUIComStateButton m_btnLimitBreak;

	[Header("함선 능력치 툴팁")]
	public NKCComStatInfoToolTip m_ToolTipHP;

	public NKCComStatInfoToolTip m_ToolTipATK;

	public NKCComStatInfoToolTip m_ToolTipDEF;

	public NKCComStatInfoToolTip m_ToolTipCritical;

	public NKCComStatInfoToolTip m_ToolTipHit;

	public NKCComStatInfoToolTip m_ToolTipEvade;

	private List<int> m_lstCostShipIDList = new List<int>();

	private long m_SelectedCostShipUID;

	private ShipRepairInfo m_curShipRepairInfo;

	private NKCUIUnitSelectList m_UIUnitSelectList;

	public RepairState Status
	{
		get
		{
			if (m_curShipRepairInfo != null)
			{
				return m_curShipRepairInfo.eRepairState;
			}
			return RepairState.None;
		}
	}

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

	public void Init(UnityAction callback = null)
	{
		m_ShipLevelUp.Init(ClickLevelMinBtn, ClickLevelPervBtn, ClickLevelNextBtn, ClickLevelMaxBtn);
		m_ShipUpgrade.Init();
		m_ShipLimitBreak?.Init();
		NKCUtil.SetBindFunction(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp, OnClickLevelUp);
		m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp.m_bGetCallbackWhileLocked = true;
		NKCUtil.SetBindFunction(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade, OnClickUpgrade);
		NKCUtil.SetBindFunction(m_btnLimitBreak, OnClickLimitBreak);
		m_btnLimitBreak.m_bGetCallbackWhileLocked = true;
		if (m_ToolTipHP != null)
		{
			m_ToolTipHP.SetType(NKM_STAT_TYPE.NST_HP);
		}
		if (m_ToolTipATK != null)
		{
			m_ToolTipATK.SetType(NKM_STAT_TYPE.NST_ATK);
		}
		if (m_ToolTipDEF != null)
		{
			m_ToolTipDEF.SetType(NKM_STAT_TYPE.NST_DEF);
		}
		if (m_ToolTipCritical != null)
		{
			m_ToolTipCritical.SetType(NKM_STAT_TYPE.NST_CRITICAL);
		}
		if (m_ToolTipHit != null)
		{
			m_ToolTipHit.SetType(NKM_STAT_TYPE.NST_HIT);
		}
		if (m_ToolTipEvade != null)
		{
			m_ToolTipEvade.SetType(NKM_STAT_TYPE.NST_EVADE);
		}
		NKCUIShipSkillSlot[] slot_NKM_UI_SHIP_INFO_SKILL_SLOT = m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT;
		for (int i = 0; i < slot_NKM_UI_SHIP_INFO_SKILL_SLOT.Length; i++)
		{
			slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i].Init(callback);
		}
		if (m_CostShipSlot != null)
		{
			m_CostShipSlot.Init();
			m_CostShipSlot.SetOnClick(OnClickCostShipSlot);
			m_CostShipSlot.SetEmpty(OnClickCostShipSlot);
		}
		m_btnEmptyShipSlot?.PointerClick.RemoveAllListeners();
		m_btnEmptyShipSlot?.PointerClick.AddListener(delegate
		{
			OnClickCostShipSlot(null, bLocked: false);
		});
		m_lstCostShipIDList = new List<int>();
	}

	public void SetData(NKMUnitData targetShipData)
	{
		m_SelectedCostShipUID = 0L;
		m_CostShipSlot.SetEmpty(OnClickCostShipSlot);
		UpdateShipRepairInfo(targetShipData);
		UpdateUI();
	}

	private void UpdateSkillUI()
	{
		if (NKCUtil.IsNullObject(m_curShipRepairInfo.ShipData))
		{
			return;
		}
		NKMShipBuildTemplet shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(m_curShipRepairInfo.ShipData.m_UnitID);
		if (NKCUtil.IsNullObject(shipBuildTemplet, $"buildTemplet is null - target Unit ID{m_curShipRepairInfo.ShipData.m_UnitID}"))
		{
			return;
		}
		NKMUnitTempletBase nKMUnitTempletBase = null;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_curShipRepairInfo.ShipData.m_UnitID);
		if (NKCUtil.IsNullObject(unitTempletBase, $"ShipTemplet is null - target Unit ID{m_curShipRepairInfo.ShipData.m_UnitID}"))
		{
			return;
		}
		if (m_curShipRepairInfo.eRepairState == RepairState.Upgrade)
		{
			nKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(m_curShipRepairInfo.ShipData.m_UnitID);
			if (NKCUtil.IsNullObject(nKMUnitTempletBase, $"prevShipTemplet is null - target Unit ID{m_curShipRepairInfo.ShipData.m_UnitID}"))
			{
				return;
			}
			shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(shipBuildTemplet.ShipUpgradeTarget1);
			if (NKCUtil.IsNullObject(shipBuildTemplet, "buildTemplet is null"))
			{
				return;
			}
			unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipBuildTemplet.ShipID);
			if (NKCUtil.IsNullObject(unitTempletBase, $"ShipTemplet is null - target Unit ID{m_curShipRepairInfo.ShipData.m_UnitID}"))
			{
				return;
			}
		}
		for (int i = 0; i < m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT.Length; i++)
		{
			NKCUIShipSkillSlot nKCUIShipSkillSlot = m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i];
			NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(unitTempletBase, i);
			if (NKCUtil.IsNullObject(shipSkillTempletByIndex, $"UpdateSkill- Can not found skill templet : shipID{unitTempletBase.m_UnitID}, target idx : {i}"))
			{
				NKCUtil.SetGameobjectActive(nKCUIShipSkillSlot, bValue: false);
				nKCUIShipSkillSlot.Cleanup();
				continue;
			}
			NKCUtil.SetGameobjectActive(nKCUIShipSkillSlot, bValue: true);
			nKCUIShipSkillSlot.SetData(shipSkillTempletByIndex);
			if (m_curShipRepairInfo.eRepairState == RepairState.Upgrade && nKMUnitTempletBase != null)
			{
				bool flag = false;
				if (i < nKMUnitTempletBase.m_lstSkillStrID.Count && i < unitTempletBase.m_lstSkillStrID.Count)
				{
					if (!string.Equals(nKMUnitTempletBase.m_lstSkillStrID[i], unitTempletBase.m_lstSkillStrID[i]))
					{
						if (m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i] != null)
						{
							m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i].SetText(NKCUtilString.GET_STRING_HANGAR_SHIPYARD_SKILL_UPGRADE);
						}
						flag = true;
					}
					if (m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i] != null)
					{
						m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i].SetStatus();
					}
				}
				else if (i < unitTempletBase.m_lstSkillStrID.Count)
				{
					if (m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i] != null)
					{
						m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i].SetText(NKCUtilString.GET_STRING_HANGAR_SHIPYARD_SKILL_NEW);
						m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i].SetStatus(NKCUIShipSkillSlot.eShipSkillSlotStatus.NEW_GET_POPUP);
					}
					flag = true;
				}
				if (m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i] != null)
				{
					if (flag)
					{
						m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i].SetStatus(NKCUIShipSkillSlot.eShipSkillSlotStatus.NEW_GET_POPUP);
					}
					else
					{
						m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i].SetStatus();
					}
				}
			}
			else
			{
				m_slot_NKM_UI_SHIP_INFO_SKILL_SLOT[i].SetStatus();
			}
		}
	}

	private void UpdateSubUI()
	{
		if (m_curShipRepairInfo.eRepairState == RepairState.LevelUp)
		{
			m_ShipLevelUp.UpdateShipData(m_curShipRepairInfo.ShipData.m_UnitID, m_curShipRepairInfo.ShipData.GetStarGrade(), m_curShipRepairInfo.iCurShipLevel, m_curShipRepairInfo.iMinimumLevel, m_curShipRepairInfo.iMaximumLevel, m_curShipRepairInfo.iTargetLevel, m_curShipRepairInfo.bCanTryLevelUp, m_curShipRepairInfo.ShipData.m_LimitBreakLevel);
		}
		else if (m_curShipRepairInfo.eRepairState == RepairState.Upgrade)
		{
			m_ShipUpgrade.UpdateShipData(m_curShipRepairInfo);
		}
		else if (m_curShipRepairInfo.eRepairState == RepairState.LimitBreak)
		{
			m_ShipLimitBreak?.UpdateShipData(m_curShipRepairInfo);
		}
	}

	private void UpdateStatUI()
	{
		NKCUtil.SetGameobjectActive(m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_ATTACK_TEXT.gameObject, m_curShipRepairInfo.eRepairState == RepairState.LevelUp);
		NKCUtil.SetGameobjectActive(m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_HP_TEXT.gameObject, m_curShipRepairInfo.eRepairState == RepairState.LevelUp);
		NKCUtil.SetGameobjectActive(m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_DEFENCE_TEXT.gameObject, m_curShipRepairInfo.eRepairState == RepairState.LevelUp);
		NKCUtil.SetGameobjectActive(m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_CRITICAL_TEXT.gameObject, m_curShipRepairInfo.eRepairState == RepairState.LevelUp);
		NKCUtil.SetGameobjectActive(m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_HIT_TEXT.gameObject, m_curShipRepairInfo.eRepairState == RepairState.LevelUp);
		NKCUtil.SetGameobjectActive(m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_EVADE_TEXT.gameObject, m_curShipRepairInfo.eRepairState == RepairState.LevelUp);
		if (m_curShipRepairInfo.eRepairState == RepairState.LevelUp)
		{
			m_txt_NKM_UI_HANGAR_SHIPYARD_STAT_POWER_TEXT.text = m_curShipRepairInfo.ShipData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData, m_curShipRepairInfo.iTargetLevel).ToString("N0");
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_ATTACK_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipAtk)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_ATTACK_TEXT.text = $"(+{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipAtk) - Mathf.RoundToInt(m_curShipRepairInfo.fCurShipAtk)})";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_HP_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipHP)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_HP_TEXT.text = $"(+{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipHP) - Mathf.RoundToInt(m_curShipRepairInfo.fCurShipHP)})";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_DEFENCE_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipDef)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_DEFENCE_TEXT.text = $"(+{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipDef) - Mathf.RoundToInt(m_curShipRepairInfo.fCurShipDef)})";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_CRITICAL_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipCritical)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_CRITICAL_TEXT.text = $"(+{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipCritical) - Mathf.RoundToInt(m_curShipRepairInfo.fCurShipCritical)})";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_HIT_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipHit)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_HIT_TEXT.text = $"(+{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipHit) - Mathf.RoundToInt(m_curShipRepairInfo.fCurShipHit)})";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_EVADE_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipEvade)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_EVADE_TEXT.text = $"(+{Mathf.RoundToInt(m_curShipRepairInfo.fNextShipEvade) - Mathf.RoundToInt(m_curShipRepairInfo.fCurShipEvade)})";
		}
		else
		{
			m_txt_NKM_UI_HANGAR_SHIPYARD_STAT_POWER_TEXT.text = m_curShipRepairInfo.ShipData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData, m_curShipRepairInfo.iCurShipLevel).ToString("N0");
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_HP_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fCurShipHP)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_ATTACK_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fCurShipAtk)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_DEFENCE_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fCurShipDef)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_CRITICAL_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fCurShipCritical)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_HIT_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fCurShipHit)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_EVADE_TEXT.text = $"{Mathf.RoundToInt(m_curShipRepairInfo.fCurShipEvade)}";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_ATTACK_TEXT.text = "";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_HP_TEXT.text = "";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_DEFENCE_TEXT.text = "";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_CRITICAL_TEXT.text = "";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_HIT_TEXT.text = "";
			m_txt_NKM_UI_HANGAR_UNIT_INFO_STAT_PLUS_EVADE_TEXT.text = "";
		}
	}

	private void CheckCanRepair()
	{
		if (!NKCUtil.IsNullObject(m_curShipRepairInfo.ShipData))
		{
			m_curShipRepairInfo.bPossibleLevelUp = m_curShipRepairInfo.eRepairState == RepairState.LevelUp && CanShipLevelUp(m_curShipRepairInfo) == NKM_ERROR_CODE.NEC_OK;
			m_curShipRepairInfo.bPossibleUpgrade = m_curShipRepairInfo.eRepairState == RepairState.Upgrade && CanShipUpgrade(m_curShipRepairInfo) == NKM_ERROR_CODE.NEC_OK;
			m_curShipRepairInfo.bPossibleLimitBreak = m_curShipRepairInfo.eRepairState == RepairState.LimitBreak && CanShipLimitBreak(m_curShipRepairInfo) == NKM_ERROR_CODE.NEC_OK;
		}
	}

	private void UpdateButtonUI()
	{
		if (m_curShipRepairInfo.eRepairState == RepairState.LevelUp)
		{
			if (m_curShipRepairInfo.bPossibleLevelUp)
			{
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp.gameObject, bValue: true);
				NKCUtil.SetGameobjectActive(m_btnLimitBreak, bValue: false);
				m_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_DISABLE.SetActive(value: false);
				m_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_LIGHT.SetActive(value: true);
				m_txt_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_TEXT.color = NKCUtil.GetButtonUIColor();
				m_img_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_ICON.color = NKCUtil.GetButtonUIColor();
				m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp.UnLock();
			}
			else
			{
				m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp.Lock();
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp.gameObject, bValue: true);
				NKCUtil.SetGameobjectActive(m_btnLimitBreak, bValue: false);
				m_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_DISABLE.SetActive(value: false);
				m_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_LIGHT.SetActive(value: false);
				m_txt_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_TEXT.color = NKCUtil.GetButtonUIColor(Active: false);
				m_img_NKM_UI_HANGAR_UNIT_INFO_BUTTON_LevelUp_BG_ICON.color = NKCUtil.GetButtonUIColor(Active: false);
			}
		}
		else if (m_curShipRepairInfo.eRepairState == RepairState.Upgrade)
		{
			if (m_curShipRepairInfo.bPossibleUpgrade)
			{
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade.gameObject, bValue: true);
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_btnLimitBreak, bValue: false);
				m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_DISABLE.SetActive(value: false);
				m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_LIGHT.SetActive(value: true);
				m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_TEXT.color = NKCUtil.GetButtonUIColor();
				m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_ICON.color = NKCUtil.GetButtonUIColor();
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade.gameObject, bValue: true);
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_btnLimitBreak, bValue: false);
				m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_LIGHT.SetActive(value: false);
				m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_DISABLE.SetActive(value: true);
				m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_TEXT.color = NKCUtil.GetButtonUIColor(Active: false);
				m_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade_BG_ICON.color = NKCUtil.GetButtonUIColor(Active: false);
			}
		}
		else if (m_curShipRepairInfo.eRepairState == RepairState.LimitBreak)
		{
			if (m_curShipRepairInfo.bPossibleLimitBreak && !m_CostShipSlot.IsEmpty() && NKMShipManager.CanUseForShipLimitBreakMaterial(m_curShipRepairInfo.ShipData, m_CostShipSlot.GetSlotData().ID))
			{
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_btnLimitBreak, bValue: true);
				m_btnLimitBreak.UnLock();
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_Upgrade.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_cbtn_NKM_UI_HANGAR_SHIPYARD_BUTTON_LevelUp.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_btnLimitBreak, bValue: true);
				m_btnLimitBreak.Lock();
			}
		}
	}

	private void UpdateMaterialSlotUI()
	{
		NKCUtil.SetGameobjectActive(m_CostSlot[0], m_curShipRepairInfo.iNeedCredit > 0);
		if (m_curShipRepairInfo.iNeedCredit > 0)
		{
			m_CostSlot[0].SetData(1, m_curShipRepairInfo.iNeedCredit, NKCScenManager.CurrentUserData().GetCredit());
		}
		if (NKMOpenTagManager.IsOpened("SHIP_LIMITBREAK"))
		{
			bool flag = false;
			NKMShipLimitBreakTemplet shipLimitBreakTemplet = NKMShipManager.GetShipLimitBreakTemplet(m_curShipRepairInfo.ShipData.m_UnitID, m_curShipRepairInfo.ShipData.m_LimitBreakLevel + 1);
			if (m_curShipRepairInfo.iCurShipLevel == m_curShipRepairInfo.iCurShipMaxLevel && m_curShipRepairInfo.iCurShipLevel >= 100 && shipLimitBreakTemplet != null)
			{
				flag = true;
			}
			if (flag)
			{
				NKCUtil.SetLabelText(m_lbCostName, NKCUtilString.GET_STRING_HANGAR_UPGRADE_COST_2);
				NKCUtil.SetGameobjectActive(m_objCostShip, bValue: true);
				if (m_CostShipSlot != null)
				{
					if (m_SelectedCostShipUID > 0)
					{
						NKCUtil.SetGameobjectActive(m_CostShipSlot, bValue: true);
						if (m_CostShipSlot.GetSlotData().UID != m_SelectedCostShipUID)
						{
							new NKCUISlot.SlotData
							{
								UID = m_SelectedCostShipUID,
								ID = NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(m_SelectedCostShipUID).m_UnitID,
								Count = 1L,
								eType = NKCUISlot.eSlotMode.Unit
							};
						}
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_CostShipSlot, bValue: false);
					}
				}
				m_lstCostShipIDList = shipLimitBreakTemplet.ListMaterialShipId;
			}
			else
			{
				NKCUtil.SetLabelText(m_lbCostName, NKCUtilString.GET_STRING_HANGAR_UPGRADE_COST);
				NKCUtil.SetGameobjectActive(m_objCostShip, bValue: false);
				NKCUtil.SetGameobjectActive(m_CostShipSlot, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbCostName, NKCUtilString.GET_STRING_HANGAR_UPGRADE_COST);
			NKCUtil.SetGameobjectActive(m_objCostShip, bValue: false);
			NKCUtil.SetGameobjectActive(m_CostShipSlot, bValue: false);
		}
		Dictionary<int, int>.Enumerator enumerator = m_curShipRepairInfo.dicMaterialList.GetEnumerator();
		bool flag2 = true;
		for (int i = 1; i < m_CostSlot.Length; i++)
		{
			NKCUIItemCostSlot nKCUIItemCostSlot = m_CostSlot[i];
			flag2 = enumerator.MoveNext();
			NKCUtil.SetGameobjectActive(nKCUIItemCostSlot, flag2);
			if (flag2)
			{
				long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(enumerator.Current.Key);
				nKCUIItemCostSlot.SetData(enumerator.Current.Key, enumerator.Current.Value, countMiscItem);
			}
			else
			{
				nKCUIItemCostSlot.SetData(0, 0, 0L);
			}
		}
	}

	private NKM_ERROR_CODE CanShipLevelUp(ShipRepairInfo repairData)
	{
		if (repairData.ShipData.m_UnitLevel == repairData.iCurShipMaxLevel)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_MAX_LEVEL;
		}
		if (NKCUtil.IsNullObject(repairData.ShipData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (repairData.ShipData.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_IS_SEIZED;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		return nKMUserData.m_ArmyData.GetUnitDeckState(repairData.ShipData.m_UnitUID) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKMShipManager.CanShipLevelup(nKMUserData, repairData.ShipData, repairData.iTargetLevel), 
		};
	}

	private NKM_ERROR_CODE CanShipUpgrade(ShipRepairInfo repairData)
	{
		if (NKCUtil.IsNullObject(repairData.ShipData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (repairData.ShipData.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_IS_SEIZED;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		return nKMUserData.m_ArmyData.GetUnitDeckState(repairData.ShipData.m_UnitUID) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKMShipManager.CanShipUpgrade(nKMUserData, repairData.ShipData, repairData.iNextShipID), 
		};
	}

	private NKM_ERROR_CODE CanShipLimitBreak(ShipRepairInfo repairData)
	{
		if (NKCUtil.IsNullObject(repairData.ShipData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (repairData.ShipData.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_IS_SEIZED;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		return nKMUserData.m_ArmyData.GetUnitDeckState(repairData.ShipData.m_UnitUID) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKMShipManager.CanShipLimitBreak(nKMUserData, repairData.ShipData, m_SelectedCostShipUID), 
		};
	}

	private void UpdateUI()
	{
		if (m_curShipRepairInfo != null)
		{
			m_curShipRepairInfo.UpdateShipData();
		}
		UpdateSkillUI();
		UpdateStatUI();
		UpdateSubUI();
		UpdateButtonUI();
		UpdateMaterialSlotUI();
		NKCUtil.SetGameobjectActive(m_ShipLevelUp, m_curShipRepairInfo.eRepairState == RepairState.LevelUp);
		NKCUtil.SetGameobjectActive(m_ShipUpgrade, m_curShipRepairInfo.eRepairState == RepairState.Upgrade);
		NKCUtil.SetGameobjectActive(m_ShipLimitBreak, m_curShipRepairInfo.eRepairState == RepairState.LimitBreak);
	}

	private void UpdateShipRepairInfo(NKMUnitData targetShip)
	{
		if (targetShip != null)
		{
			m_curShipRepairInfo = new ShipRepairInfo(targetShip.m_UnitUID);
			CheckCanRepair();
		}
	}

	private void ClickLevelMinBtn()
	{
		if (!IsMinimumLevel())
		{
			m_curShipRepairInfo.iTargetLevel = m_curShipRepairInfo.iMinimumLevel;
			UpdateUI();
		}
	}

	private void ClickLevelPervBtn()
	{
		if (!IsMinimumLevel())
		{
			m_curShipRepairInfo.iTargetLevel--;
			UpdateUI();
		}
	}

	private void ClickLevelNextBtn()
	{
		if (!IsMaximumLevel())
		{
			m_curShipRepairInfo.iTargetLevel++;
			UpdateUI();
		}
	}

	private void ClickLevelMaxBtn()
	{
		if (!IsMaximumLevel())
		{
			m_curShipRepairInfo.iTargetLevel = m_curShipRepairInfo.iMaximumLevel;
			UpdateUI();
			m_ShipLevelUp.OnClickMaximumButton();
		}
	}

	private bool IsMinimumLevel()
	{
		if (m_curShipRepairInfo.iMinimumLevel >= m_curShipRepairInfo.iTargetLevel)
		{
			return true;
		}
		return false;
	}

	private bool IsMaximumLevel()
	{
		if (m_curShipRepairInfo.iTargetLevel == m_curShipRepairInfo.iMaximumLevel || !m_curShipRepairInfo.bCanTryLevelUp)
		{
			return true;
		}
		if (m_curShipRepairInfo.iTargetLevel > m_curShipRepairInfo.GetMaximumLevel())
		{
			return true;
		}
		return false;
	}

	private void OnClickLevelUp()
	{
		if (m_curShipRepairInfo.ShipData == null)
		{
			return;
		}
		if (m_curShipRepairInfo.bPossibleLevelUp)
		{
			NKCUIHangarShipyardPopup.Instance.Open(m_curShipRepairInfo, tryShipLevelup, tryShipUpgrade, tryShipLimitBreak);
			return;
		}
		NKMUnitData shipFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(m_curShipRepairInfo.ShipData.m_UnitUID);
		if (shipFromUID == null)
		{
			return;
		}
		if (NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_ID == NKMShipManager.CanShipLevelup(NKCScenManager.CurrentUserData(), shipFromUID, m_curShipRepairInfo.iTargetLevel))
		{
			foreach (KeyValuePair<int, int> item in NKMShipManager.GetMaterialListInLevelup(shipFromUID.m_UnitID, shipFromUID.m_UnitLevel, m_curShipRepairInfo.iTargetLevel))
			{
				if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(item.Key) < item.Value)
				{
					NKCShopManager.OpenItemLackPopup(item.Key, item.Value);
					return;
				}
			}
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = CanShipLevelUp(m_curShipRepairInfo);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
		}
	}

	private void OnClickUpgrade()
	{
		if (m_curShipRepairInfo.ShipData == null)
		{
			return;
		}
		if (m_curShipRepairInfo.bPossibleUpgrade)
		{
			NKCUIHangarShipyardPopup.Instance.Open(m_curShipRepairInfo, tryShipLevelup, tryShipUpgrade, tryShipLimitBreak);
			return;
		}
		NKMUnitData shipFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(m_curShipRepairInfo.ShipData.m_UnitUID);
		if (shipFromUID == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		if (NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM == NKMShipManager.CanShipUpgrade(nKMUserData, shipFromUID, m_curShipRepairInfo.iNextShipID))
		{
			NKMShipBuildTemplet shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(m_curShipRepairInfo.iNextShipID);
			NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
			foreach (UpgradeMaterial upgradeMaterial in shipBuildTemplet.UpgradeMaterialList)
			{
				if (inventoryData.GetCountMiscItem(upgradeMaterial.m_ShipUpgradeMaterial) < upgradeMaterial.m_ShipUpgradeMaterialCount)
				{
					NKCShopManager.OpenItemLackPopup(upgradeMaterial.m_ShipUpgradeMaterial, upgradeMaterial.m_ShipUpgradeMaterialCount);
					return;
				}
			}
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = CanShipUpgrade(m_curShipRepairInfo);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
		}
	}

	private void OnClickLimitBreak()
	{
		if (m_curShipRepairInfo.ShipData == null)
		{
			return;
		}
		if (m_curShipRepairInfo.bPossibleLimitBreak)
		{
			NKCUIHangarShipyardPopup.Instance.Open(m_curShipRepairInfo, tryShipLevelup, tryShipUpgrade, tryShipLimitBreak, m_CostShipSlot.GetSlotData());
			return;
		}
		if (m_CostShipSlot.IsEmpty())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_SHIP_LIMITBREAK_NOT_CHOICE_SHIP);
			return;
		}
		NKMUnitData shipFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(m_curShipRepairInfo.ShipData.m_UnitUID);
		if (shipFromUID == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		if (NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM == NKMShipManager.CanShipLimitBreak(nKMUserData, shipFromUID, m_SelectedCostShipUID))
		{
			NKMShipLimitBreakTemplet shipLimitBreakTemplet = NKMShipManager.GetShipLimitBreakTemplet(m_curShipRepairInfo.ShipData.m_UnitID, m_curShipRepairInfo.ShipData.m_LimitBreakLevel + 1);
			if (shipLimitBreakTemplet != null)
			{
				NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
				foreach (MiscItemUnit shipLimitBreakItem in shipLimitBreakTemplet.ShipLimitBreakItems)
				{
					if (inventoryData.GetCountMiscItem(shipLimitBreakItem.ItemId) < shipLimitBreakItem.Count)
					{
						NKCShopManager.OpenItemLackPopup(shipLimitBreakItem.ItemId, shipLimitBreakItem.Count32);
						return;
					}
				}
			}
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = CanShipLimitBreak(m_curShipRepairInfo);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
		}
	}

	private void tryShipLevelup()
	{
		NKCPacketSender.Send_NKMPacket_SHIP_LEVELUP_REQ(m_curShipRepairInfo.ShipData.m_UnitUID, m_curShipRepairInfo.iTargetLevel);
	}

	private void tryShipUpgrade()
	{
		NKCUIHangarShipyardPopup.CheckInstanceAndClose();
		NKCPacketSender.Send_NKMPacket_SHIP_UPGRADE_REQ(m_curShipRepairInfo.ShipData.m_UnitUID, m_curShipRepairInfo.iNextShipID);
	}

	private void tryShipLimitBreak()
	{
		NKCUIHangarShipyardPopup.CheckInstanceAndClose();
		NKCPacketSender.Send_NKMPacket_LIMIT_BREAK_SHIP_REQ(m_curShipRepairInfo.ShipData.m_UnitUID, m_SelectedCostShipUID);
	}

	private void OnClickCostShipSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_SHIP, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
		options.setShipFilterCategory = NKCUnitSortSystem.setDefaultShipFilterCategory;
		options.setShipSortCategory = NKCUnitSortSystem.setDefaultShipSortCategory;
		options.m_SortOptions.AdditionalExcludeFilterFunc = CheckUnitListFilter;
		options.setExcludeUnitUID = new HashSet<long> { m_curShipRepairInfo.ShipData.m_UnitUID };
		options.bCanSelectUnitInMission = false;
		options.bPushBackUnselectable = true;
		options.m_SortOptions.bUseDeckedState = true;
		options.m_SortOptions.bUseLockedState = true;
		options.m_bShowShipBuildShortcut = true;
		options.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_SELECT_SHIP;
		UnitSelectList.Open(options, OnChangedCostShip);
	}

	private bool CheckUnitListFilter(NKMUnitData unitData)
	{
		return m_lstCostShipIDList.Contains(unitData.m_UnitID);
	}

	private void OnChangedCostShip(List<long> listUnitUID)
	{
		if (listUnitUID.Count != 1)
		{
			Debug.LogError("Fatal Error : UnitSelectList returned wrong list");
			return;
		}
		long num = listUnitUID[0];
		NKMUnitData selectedShipData = NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(num);
		if (selectedShipData != null && selectedShipData.m_UnitLevel > 1)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SHIP_LIMITBREAK_WARNING, delegate
			{
				SetCostShip(selectedShipData.m_UnitUID);
			});
		}
		else
		{
			SetCostShip(num);
		}
	}

	private void SetCostShip(long unitUID)
	{
		UnitSelectList.Close();
		m_SelectedCostShipUID = unitUID;
		NKCUISlot.SlotData slotData = new NKCUISlot.SlotData();
		slotData.UID = unitUID;
		slotData.ID = NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(unitUID).m_UnitID;
		slotData.Count = 1L;
		slotData.eType = NKCUISlot.eSlotMode.Unit;
		NKCUtil.SetGameobjectActive(m_CostShipSlot, bValue: true);
		m_CostShipSlot.SetData(slotData, bShowName: false, bShowNumber: false, bEnableLayoutElement: false, OnClickCostShipSlot);
		CheckCanRepair();
		m_btnLimitBreak.UnLock();
	}

	public void OnRecv(NKMPacket_SHIP_UPGRADE_ACK sPacket)
	{
		if (m_curShipRepairInfo.ShipData.m_UnitUID == sPacket.shipUnitData.m_UnitUID)
		{
			NKCUIGameResultGetUnit.ShowShipTranscendence(NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(sPacket.shipUnitData.m_UnitUID), m_curShipRepairInfo.iCurSkillCnt, m_curShipRepairInfo.iCurShipMaxLevel, m_curShipRepairInfo.iNextShipMaxLevel);
		}
	}

	private void FindComponent<T>(string name, ref T target) where T : UnityEngine.Component
	{
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null)
		{
			Debug.LogErrorFormat("NKCUIHangarShipyard::FindGameObject - FAILE NAME : {0}", name);
			return;
		}
		target = gameObject.GetComponent<T>();
		if (target == null)
		{
			Debug.LogErrorFormat("NKCUIHangarShipyard::FindGameObject - FAILE NAME : {0}", name);
		}
	}

	private void FindGameObject(string name, ref GameObject target)
	{
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null)
		{
			Debug.LogErrorFormat("NKCUIHangarShipyard::FindGameObject - FAILE NAME : {0}", name);
		}
		target = gameObject;
	}
}
