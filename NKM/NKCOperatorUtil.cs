using System;
using System.Collections.Generic;
using ClientPacket.Common;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKM;

public static class NKCOperatorUtil
{
	public static List<NKCOperatorPassiveToken> m_lstPassiveToken = new List<NKCOperatorPassiveToken>();

	public static Color BAN_COLOR_RED => NKCUtil.GetColor("#EC2020");

	public static bool IsHide()
	{
		return !NKMContentsVersionManager.HasTag("OPERATOR");
	}

	public static bool IsActive()
	{
		return NKCContentManager.IsContentsUnlocked(ContentsType.OPERATOR);
	}

	public static bool IsActiveCastingBan()
	{
		return NKMOpenTagManager.IsOpened("PVP_OPR_BAN");
	}

	public static bool IsOperatorUnit(int operatorID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorID);
		if (unitTempletBase != null)
		{
			return unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR;
		}
		return false;
	}

	public static bool IsContractFromUnit(long operatorUID)
	{
		return GetOperatorData(operatorUID)?.fromContract ?? false;
	}

	public static NKMOperator GetOperatorData(long operatorUID)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null && myUserData.m_ArmyData != null)
		{
			return myUserData.m_ArmyData.GetOperatorFromUId(operatorUID);
		}
		return null;
	}

	public static bool IsMyOperator(long OperatorUID)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		if (nKMUserData.m_ArmyData == null)
		{
			return false;
		}
		if (nKMUserData.m_ArmyData.m_dicMyOperator == null)
		{
			return false;
		}
		return nKMUserData.m_ArmyData.m_dicMyOperator.ContainsKey(OperatorUID);
	}

	public static bool IsSameOperatorGroup(long operatorA, long operatorB)
	{
		return GetPassiveGroupID(operatorA) == GetPassiveGroupID(operatorB);
	}

	public static void UpdateLockState(long operatorUID, bool bLock)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && nKMUserData.m_ArmyData.m_dicMyOperator.ContainsKey(operatorUID))
		{
			nKMUserData.m_ArmyData.m_dicMyOperator[operatorUID].bLock = bLock;
		}
	}

	public static bool IsLock(long operatorUID)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		if (!nKMUserData.m_ArmyData.m_dicMyOperator.ContainsKey(operatorUID))
		{
			return false;
		}
		return nKMUserData.m_ArmyData.m_dicMyOperator[operatorUID].bLock;
	}

	public static NKMOperatorSkillTemplet GetMainSkill(int operatorID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorID);
		if (unitTempletBase == null)
		{
			return null;
		}
		if (unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			return null;
		}
		if (unitTempletBase.m_lstSkillStrID != null && unitTempletBase.m_lstSkillStrID.Count > 0)
		{
			return GetSkillTemplet(unitTempletBase.m_lstSkillStrID[0]);
		}
		return null;
	}

	public static NKMOperatorSkillTemplet GetSkillTemplet(string operSkillStrID)
	{
		foreach (NKMOperatorSkillTemplet value in NKMTempletContainer<NKMOperatorSkillTemplet>.Values)
		{
			if (string.Equals(value.m_OperSkillStrID, operSkillStrID))
			{
				return value;
			}
		}
		return null;
	}

	public static NKMOperatorSkillTemplet GetSkillTemplet(int skillID)
	{
		return NKMTempletContainer<NKMOperatorSkillTemplet>.Find(skillID);
	}

	public static int GetPassiveGroupID(long operatorUID)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && nKMUserData.m_ArmyData != null && nKMUserData.m_ArmyData.m_dicMyOperator != null && nKMUserData.m_ArmyData.m_dicMyOperator.ContainsKey(operatorUID))
		{
			return GetPassiveGroupID(nKMUserData.m_ArmyData.m_dicMyOperator[operatorUID].id);
		}
		return -1;
	}

	public static int GetPassiveGroupID(int operatorID)
	{
		return NKMUnitManager.GetUnitTempletBase(operatorID)?.m_OprPassiveGroupID ?? (-1);
	}

	public static bool IsMaximumSkillLevel(int skillID, int skillLevel)
	{
		NKMOperatorSkillTemplet skillTemplet = GetSkillTemplet(skillID);
		if (skillTemplet == null)
		{
			return false;
		}
		if (skillTemplet.m_MaxSkillLevel <= skillLevel)
		{
			return true;
		}
		return false;
	}

	public static bool IsCanEnhanceMainSkill(NKMOperator mainOp, NKMOperator targetOp)
	{
		if (mainOp == null || targetOp == null)
		{
			return false;
		}
		if (mainOp.mainSkill.id != targetOp.mainSkill.id)
		{
			return false;
		}
		if (IsMaximumSkillLevel(mainOp.mainSkill.id, mainOp.mainSkill.level))
		{
			return false;
		}
		return true;
	}

	public static bool IsCanEnhanceSubSkill(NKMOperator mainOp, NKMOperator targetOp)
	{
		if (mainOp == null || targetOp == null)
		{
			return false;
		}
		return IsCanEnhanceSubSkill(mainOp, targetOp.subSkill.id);
	}

	public static bool IsCanEnhanceSubSkill(NKMOperator mainOp, int targetSubSkillID)
	{
		if (IsMaximumSkillLevel(mainOp.subSkill.id, mainOp.subSkill.level))
		{
			return false;
		}
		return mainOp.subSkill.id == targetSubSkillID;
	}

	public static int GetEnhanceSuccessfulRate(NKMOperator targetOp, bool bSameOperator = false)
	{
		if (targetOp == null)
		{
			return 0;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(targetOp.id);
		if (unitTempletBase != null)
		{
			NKMOperatorConstTemplet.MaterialUnit materialUnit = NKMCommonConst.OperatorConstTemplet.materialUntis.Find((NKMOperatorConstTemplet.MaterialUnit e) => e.m_NKM_UNIT_GRADE == unitTempletBase.m_NKM_UNIT_GRADE);
			if (materialUnit != null)
			{
				if (!bSameOperator)
				{
					return materialUnit.levelUpSuccessRatePercent;
				}
				return materialUnit.commandLevelUpPercent;
			}
		}
		return 0;
	}

	public static int GetEnhanceSuccessfulRate(NKM_ITEM_GRADE targetTokenGrade)
	{
		NKMOperatorConstTemplet.PassiveToken[] listPassiveToken = NKMCommonConst.OperatorConstTemplet.listPassiveToken;
		foreach (NKMOperatorConstTemplet.PassiveToken passiveToken in listPassiveToken)
		{
			if (passiveToken.m_NKM_ITEM_GRADE == targetTokenGrade)
			{
				return passiveToken.LevelUpSuccessRatePercent;
			}
		}
		return 0;
	}

	public static int GetTransferSuccessfulRate(NKMOperator targetOp)
	{
		if (targetOp == null)
		{
			return 0;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(targetOp.id);
		if (unitTempletBase != null)
		{
			NKMOperatorConstTemplet.MaterialUnit materialUnit = NKMCommonConst.OperatorConstTemplet.materialUntis.Find((NKMOperatorConstTemplet.MaterialUnit e) => e.m_NKM_UNIT_GRADE == unitTempletBase.m_NKM_UNIT_GRADE);
			if (materialUnit != null)
			{
				return materialUnit.transportSuccessRatePercent;
			}
		}
		return 0;
	}

	public static int GetTransferSuccessfulRate(NKM_ITEM_GRADE targetTokenGrade)
	{
		NKMOperatorConstTemplet.PassiveToken[] listPassiveToken = NKMCommonConst.OperatorConstTemplet.listPassiveToken;
		foreach (NKMOperatorConstTemplet.PassiveToken passiveToken in listPassiveToken)
		{
			if (passiveToken.m_NKM_ITEM_GRADE == targetTokenGrade)
			{
				return passiveToken.TransportSuccessRatePercent;
			}
		}
		return 0;
	}

	public static bool IsMaximumLevel(int operatorLv)
	{
		return NKMCommonConst.OperatorConstTemplet.unitMaximumLevel <= operatorLv;
	}

	public static int CalcNegotiationTotalExp(List<MiscItemData> itemList)
	{
		int num = 0;
		foreach (MiscItemData item in itemList)
		{
			NKMOperatorConstTemplet.Negotiation operatorConstTempletNeogitiation = GetOperatorConstTempletNeogitiation(item.itemId);
			if (operatorConstTempletNeogitiation != null)
			{
				num += operatorConstTempletNeogitiation.exp * item.count;
			}
		}
		return num;
	}

	public static int CalcNegotiationCostCredit(List<MiscItemData> itemList)
	{
		int num = 0;
		foreach (MiscItemData item in itemList)
		{
			NKMOperatorConstTemplet.Negotiation operatorConstTempletNeogitiation = GetOperatorConstTempletNeogitiation(item.itemId);
			if (operatorConstTempletNeogitiation != null)
			{
				num += operatorConstTempletNeogitiation.credit * item.count;
			}
		}
		return num;
	}

	private static NKMOperatorConstTemplet.Negotiation GetOperatorConstTempletNeogitiation(int itemID)
	{
		return Array.Find(NKMCommonConst.OperatorConstTemplet.list, (NKMOperatorConstTemplet.Negotiation e) => e.itemId == itemID);
	}

	public static int GetOperatorLevelByTotalExp(int unitId, int totalExp)
	{
		int num = 0;
		int unitMaximumLevel = NKMCommonConst.OperatorConstTemplet.unitMaximumLevel;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitId);
		if (unitTempletBase == null)
		{
			return -1;
		}
		foreach (NKMOperatorExpData value in NKMOperatorExpTemplet.Find(unitTempletBase.m_NKM_UNIT_GRADE).values)
		{
			if (value.m_iExpCumulatedOpr > totalExp || num >= unitMaximumLevel)
			{
				break;
			}
			num = value.m_iLevel;
		}
		return num;
	}

	public static int GetOperatorLevelExp(int unitId, int level, int totalExp)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitId);
		if (unitTempletBase == null)
		{
			return -1;
		}
		NKMOperatorExpData nKMOperatorExpData = NKMOperatorExpTemplet.Find(unitTempletBase.m_NKM_UNIT_GRADE).values.Find((NKMOperatorExpData e) => e.m_iLevel == level);
		if (nKMOperatorExpData == null)
		{
			return 0;
		}
		return totalExp - nKMOperatorExpData.m_iExpCumulatedOpr;
	}

	public static int GetOperatorTotalExp(int unitId, int level, int exp)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitId);
		if (unitTempletBase == null)
		{
			return -1;
		}
		return NKMOperatorExpTemplet.Find(unitTempletBase.m_NKM_UNIT_GRADE).values.Find((NKMOperatorExpData e) => e.m_iLevel == level).m_iExpCumulatedOpr + exp;
	}

	public static int GetRequiredExp(NKMOperator operatorData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
		if (unitTempletBase != null)
		{
			return GetRequiredExp(unitTempletBase.m_NKM_UNIT_GRADE, operatorData.level);
		}
		return 0;
	}

	public static int GetRequiredExp(int unitID, int level)
	{
		return GetRequiredExp(NKMUnitManager.GetUnitTempletBase(unitID).m_NKM_UNIT_GRADE, level);
	}

	public static int GetRequiredExp(NKM_UNIT_GRADE grade, int level)
	{
		NKMOperatorExpTemplet nKMOperatorExpTemplet = NKMOperatorExpTemplet.Find(grade);
		if (nKMOperatorExpTemplet != null)
		{
			foreach (NKMOperatorExpData value in nKMOperatorExpTemplet.values)
			{
				if (value.m_iLevel == level)
				{
					return value.m_iExpRequiredOpr;
				}
			}
		}
		return 0;
	}

	public static void CalculateFutureOperatorExpAndLevel(NKMOperator operatorData, int expGain, out int Level, out int Exp)
	{
		if (operatorData == null)
		{
			Level = 0;
			Exp = 0;
			return;
		}
		Level = operatorData.level;
		Exp = operatorData.exp + expGain;
		int unitMaximumLevel = NKMCommonConst.OperatorConstTemplet.unitMaximumLevel;
		if (Level >= unitMaximumLevel)
		{
			Exp = 0;
			return;
		}
		int num = GetRequiredExp(operatorData);
		if (num == 0)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
		if (unitTempletBase == null)
		{
			return;
		}
		while (num <= Exp)
		{
			Level++;
			if (Level >= unitMaximumLevel)
			{
				Exp = 0;
				break;
			}
			Exp -= num;
			num = GetRequiredUnitExp(unitTempletBase.m_NKM_UNIT_GRADE, Level);
			if (num == 0)
			{
				break;
			}
		}
	}

	public static int CalculateNeedExpForUnitMaxLevel(NKMOperator operatorData, NKM_UNIT_GRADE grade)
	{
		if (operatorData == null)
		{
			return 0;
		}
		return CalculateNeedExpForUnitMaxLevel(operatorData.level, operatorData.exp, grade);
	}

	private static int CalculateNeedExpForUnitMaxLevel(int Level, int Exp, NKM_UNIT_GRADE grade)
	{
		int unitMaximumLevel = NKMCommonConst.OperatorConstTemplet.unitMaximumLevel;
		if (Level == unitMaximumLevel)
		{
			return 0;
		}
		int num = GetRequiredUnitExp(grade, Level);
		if (num == 0)
		{
			return 0;
		}
		while (Level < unitMaximumLevel)
		{
			Level++;
			if (Level == unitMaximumLevel)
			{
				break;
			}
			num += GetRequiredUnitExp(grade, Level);
		}
		return num - Exp;
	}

	public static int GetRequiredUnitExp(NKM_UNIT_GRADE grade, int level)
	{
		NKMOperatorExpTemplet nKMOperatorExpTemplet = NKMOperatorExpTemplet.Find(grade);
		if (nKMOperatorExpTemplet != null)
		{
			foreach (NKMOperatorExpData value in nKMOperatorExpTemplet.values)
			{
				if (value.m_iLevel == level)
				{
					return value.m_iExpRequiredOpr;
				}
			}
		}
		return 0;
	}

	private static bool IsVailedStat(NKM_STAT_TYPE type)
	{
		if ((uint)type <= 2u || type == NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE)
		{
			return true;
		}
		return false;
	}

	public static float GetStateValue(NKMOperator operatorData, NKM_STAT_TYPE type)
	{
		if (!IsVailedStat(type))
		{
			return 0f;
		}
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(operatorData.id);
		if (unitStatTemplet == null)
		{
			return 0f;
		}
		return unitStatTemplet.m_StatData.GetStatBase(type) + unitStatTemplet.m_StatData.GetStatPerLevel(type) * (float)(operatorData.level - 1);
	}

	public static string GetStatPercentageString(NKMOperator operatorData, NKM_STAT_TYPE type)
	{
		if (operatorData == null)
		{
			return "0%";
		}
		return GetStatPercentageString(operatorData.id, operatorData.level, type);
	}

	public static string GetStatPercentageString(int unitID, int level, NKM_STAT_TYPE type)
	{
		if (!IsVailedStat(type))
		{
			return "0%";
		}
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitID);
		if (unitStatTemplet == null)
		{
			return "0%";
		}
		return GetStatPercentageString(unitStatTemplet.m_StatData.GetStatBase(type) + unitStatTemplet.m_StatData.GetStatPerLevel(type) * (float)(level - 1));
	}

	public static string GetStatPercentageString(float value)
	{
		return (value * 0.01f).ToString("N2") + "%";
	}

	public static bool IsPercentageStat(NKCUnitSortSystem.eSortOption sortOption)
	{
		if ((uint)(sortOption - 15) <= 5u || (uint)(sortOption - 27) <= 1u)
		{
			return true;
		}
		return false;
	}

	public static bool IsPercentageStat(NKCOperatorSortSystem.eSortOption sortOption)
	{
		if ((uint)(sortOption - 13) <= 7u)
		{
			return true;
		}
		return false;
	}

	public static Sprite GetSpriteRandomSlot()
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_operator_deck_sprite", "NKM_UI_OPERATOR_DECK_SLOT_RANDOM");
	}

	public static Sprite GetSpriteEmptySlot()
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_operator_deck_sprite", "NKM_UI_OPERATOR_DECK_SLOT_RARE_EMPTY");
	}

	public static Sprite GetSpriteLockSlot()
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_operator_deck_sprite", "NKM_UI_OPERATOR_DECK_SLOT_RARE_LOCK");
	}

	public static string MakeOperatorSkillDesc(NKMOperatorSkillTemplet templet, int level)
	{
		if (templet == null)
		{
			return "";
		}
		NKMBuffTemplet nKMBuffTemplet = null;
		string result = "";
		switch (templet.m_OperSkillType)
		{
		case OperatorSkillType.m_Tactical:
			result = NKCUtilString.ApplyBuffValueToString(NKMTacticalCommandManager.GetTacticalCommandTempletByStrID(templet.m_OperSkillTarget), level);
			break;
		case OperatorSkillType.m_Passive:
		{
			using (Dictionary<string, int>.KeyCollection.Enumerator enumerator = NKMBattleConditionManager.GetTempletByStrID(templet.m_OperSkillTarget).GetAllyBuffList().Keys.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					nKMBuffTemplet = NKMBuffManager.GetBuffTempletByStrID(enumerator.Current);
					result = NKCUtilString.ApplyBuffValueToString(NKCStringTable.GetString(templet.m_OperSkillDescStrID), nKMBuffTemplet, level, level);
				}
			}
			break;
		}
		}
		return result;
	}

	public static void PlayVoice(NKMDeckIndex deckIdx, VOICE_TYPE type, bool bStopCurrentVoice = true)
	{
		NKMDeckData deckData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(deckIdx);
		if (deckData != null && deckData.m_OperatorUID != 0L)
		{
			NKMOperator operatorData = GetOperatorData(deckData.m_OperatorUID);
			if (operatorData != null)
			{
				NKCUIVoiceManager.PlayVoice(type, operatorData, bShowCaption: false, bStopCurrentVoice);
			}
		}
	}

	public static NKMOperator GetDummyOperator(int unitID, bool bSetMaximum = false)
	{
		return GetDummyOperator(NKMUnitManager.GetUnitTempletBase(unitID), bSetMaximum);
	}

	public static NKMOperator GetDummyOperator(NKMUnitTempletBase _templet, bool bSetMaximum = false)
	{
		if (_templet == null)
		{
			return null;
		}
		NKMOperator nKMOperator = new NKMOperator();
		nKMOperator.id = _templet.m_UnitID;
		nKMOperator.uid = _templet.m_UnitID;
		if (bSetMaximum)
		{
			nKMOperator.level = NKMCommonConst.OperatorConstTemplet.unitMaximumLevel;
		}
		else
		{
			nKMOperator.level = 1;
		}
		if (bSetMaximum)
		{
			nKMOperator.exp = GetRequiredExp(_templet.m_NKM_UNIT_GRADE, NKMCommonConst.OperatorConstTemplet.unitMaximumLevel);
		}
		else
		{
			nKMOperator.exp = 0;
		}
		nKMOperator.bLock = false;
		nKMOperator.fromContract = false;
		nKMOperator.mainSkill = new NKMOperatorSkill();
		nKMOperator.subSkill = new NKMOperatorSkill();
		NKMOperatorSkillTemplet skillTemplet = GetSkillTemplet(_templet.m_lstSkillStrID[0]);
		if (skillTemplet != null)
		{
			nKMOperator.mainSkill.id = skillTemplet.m_OperSkillID;
			nKMOperator.mainSkill.level = (byte)((!bSetMaximum) ? 1u : ((uint)skillTemplet.m_MaxSkillLevel));
		}
		nKMOperator.subSkill.id = 0;
		nKMOperator.subSkill.level = 0;
		return nKMOperator;
	}

	public static void GetExtractPriceItem(NKM_UNIT_GRADE grade, out int priceItemID, out int value)
	{
		priceItemID = 0;
		value = 0;
		foreach (NKMOperatorConstTemplet.HostUnit hostUnit in NKMCommonConst.OperatorConstTemplet.hostUnits)
		{
			if (hostUnit.m_NKM_UNIT_GRADE == grade)
			{
				priceItemID = hostUnit.extractPriceItemId;
				value = hostUnit.extractPrice;
				break;
			}
		}
	}

	public static int GetExtractItemID(int passiveGroupID, int operatorSubSkillID, NKM_UNIT_GRADE unitGrade)
	{
		NKMOperatorRandomPassiveGroupTemplet nKMOperatorRandomPassiveGroupTemplet = NKMOperatorRandomPassiveGroupTemplet.Find(passiveGroupID);
		if (nKMOperatorRandomPassiveGroupTemplet == null)
		{
			return 0;
		}
		foreach (NKMOperatorRandomPassiveTemplet group in nKMOperatorRandomPassiveGroupTemplet.Groups)
		{
			if (group.operSkillId == operatorSubSkillID)
			{
				switch (unitGrade)
				{
				case NKM_UNIT_GRADE.NUG_SSR:
					return group.extractItemID_SSR;
				case NKM_UNIT_GRADE.NUG_SR:
					return group.extractItemID_SR;
				case NKM_UNIT_GRADE.NUG_R:
					return group.extractItemID_R;
				case NKM_UNIT_GRADE.NUG_N:
					return group.extractItemID_N;
				}
			}
		}
		return 0;
	}

	public static void UpdateOperatorPassiveToken()
	{
		m_lstPassiveToken.Clear();
		NKMInventoryData inventoryData = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData;
		NKMOperatorConstTemplet.PassiveToken[] listPassiveToken = NKMCommonConst.OperatorConstTemplet.listPassiveToken;
		foreach (NKMOperatorConstTemplet.PassiveToken passiveToken in listPassiveToken)
		{
			foreach (int item in passiveToken.ItemID)
			{
				long countMiscItem = inventoryData.GetCountMiscItem(item);
				if (countMiscItem > 0)
				{
					m_lstPassiveToken.Add(new NKCOperatorPassiveToken(passiveToken.m_NKM_ITEM_GRADE, item, countMiscItem, GetOperatorSkill(item)));
				}
			}
		}
	}

	public static int GetOperatorSkill(NKCOperatorPassiveToken passiveToken)
	{
		return GetOperatorSkill(passiveToken.ItemID);
	}

	public static int GetOperatorSkill(int iOperatorPassiveTokenItemID)
	{
		foreach (NKMOperatorRandomPassiveGroupTemplet value in NKMTempletContainer<NKMOperatorRandomPassiveGroupTemplet>.Values)
		{
			foreach (NKMOperatorRandomPassiveTemplet group in value.Groups)
			{
				if (group.extractItemID_SSR == iOperatorPassiveTokenItemID || group.extractItemID_SR == iOperatorPassiveTokenItemID || group.extractItemID_R == iOperatorPassiveTokenItemID || group.extractItemID_N == iOperatorPassiveTokenItemID)
				{
					return group.operSkillId;
				}
			}
		}
		return 0;
	}

	public static bool IsCanTransferGrade(NKM_UNIT_GRADE unitGrade, NKM_ITEM_GRADE itemGrade)
	{
		if (unitGrade == NKM_UNIT_GRADE.NUG_SSR && itemGrade == NKM_ITEM_GRADE.NIG_SSR)
		{
			return true;
		}
		if (unitGrade == NKM_UNIT_GRADE.NUG_SR && itemGrade >= NKM_ITEM_GRADE.NIG_SR)
		{
			return true;
		}
		if (unitGrade == NKM_UNIT_GRADE.NUG_R && itemGrade >= NKM_ITEM_GRADE.NIG_R)
		{
			return true;
		}
		if (unitGrade == NKM_UNIT_GRADE.NUG_N && itemGrade >= NKM_ITEM_GRADE.NIG_N)
		{
			return true;
		}
		return false;
	}
}
