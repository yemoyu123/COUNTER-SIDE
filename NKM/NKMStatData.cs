using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using Cs.Math;
using NKM.Templet;

namespace NKM;

[Serializable]
public class NKMStatData
{
	private struct SecondaryBuffStatInfo
	{
		public NKM_STAT_TYPE statType;

		public float value;

		public bool isDebuff;

		public short casterUID;
	}

	private Dictionary<NKM_STAT_TYPE, float> m_StatBase = new Dictionary<NKM_STAT_TYPE, float>();

	private Dictionary<NKM_STAT_TYPE, float> m_StatBonusBaseValue = new Dictionary<NKM_STAT_TYPE, float>();

	private Dictionary<NKM_STAT_TYPE, float> m_StatBuffFinalValue = new Dictionary<NKM_STAT_TYPE, float>();

	private Dictionary<NKM_STAT_TYPE, float> m_StatDebuffFinalValue = new Dictionary<NKM_STAT_TYPE, float>();

	private Dictionary<NKM_STAT_TYPE, float> m_StatSystemValue = new Dictionary<NKM_STAT_TYPE, float>();

	private Dictionary<NKM_STAT_TYPE, float> m_StatFinal = new Dictionary<NKM_STAT_TYPE, float>();

	private List<SecondaryBuffStatInfo> m_lstSecondaryBuff = new List<SecondaryBuffStatInfo>();

	private static readonly HashSet<NKM_STAT_TYPE> s_hsSecondaryBuffStat = new HashSet<NKM_STAT_TYPE> { NKM_STAT_TYPE.NST_HP_REGEN_RATE };

	private Dictionary<NKM_STAT_TYPE, float> m_StatPerLevel = new Dictionary<NKM_STAT_TYPE, float>();

	private Dictionary<NKM_STAT_TYPE, float> m_StatMaxPerLevel = new Dictionary<NKM_STAT_TYPE, float>();

	public Dictionary<NKM_STAT_TYPE, float> dicStatBase => m_StatBase;

	public Dictionary<NKM_STAT_TYPE, float> dicStatBonus => m_StatBonusBaseValue;

	public bool HasEnchantFeedExp => false;

	private bool IsSecondaryBuffStat(NKM_STAT_TYPE statType)
	{
		return s_hsSecondaryBuffStat.Contains(statType);
	}

	private float GetStat(Dictionary<NKM_STAT_TYPE, float> dicStat, NKM_STAT_TYPE statType)
	{
		if (dicStat.TryGetValue(statType, out var value))
		{
			return value;
		}
		return 0f;
	}

	private float GetStat(Dictionary<NKM_STAT_TYPE, float> dicStat, int statType)
	{
		return GetStat(dicStat, (NKM_STAT_TYPE)statType);
	}

	private bool SetStat(Dictionary<NKM_STAT_TYPE, float> dicStat, NKM_STAT_TYPE statType, float fStat)
	{
		if (fStat == 0f)
		{
			dicStat.Remove(statType);
		}
		else
		{
			dicStat[statType] = fStat;
		}
		return true;
	}

	private float GetStatBuffFinalValue(NKM_STAT_TYPE statType)
	{
		return GetStat(m_StatBuffFinalValue, statType);
	}

	private bool SetStatBuffFinalValue(NKM_STAT_TYPE statType, float fStat)
	{
		return SetStat(m_StatBuffFinalValue, statType, fStat);
	}

	private float GetStatDebuffFinalValue(NKM_STAT_TYPE statType)
	{
		return GetStat(m_StatDebuffFinalValue, statType);
	}

	private bool SetStatDebuffFinalValue(NKM_STAT_TYPE statType, float fStat)
	{
		return SetStat(m_StatDebuffFinalValue, statType, fStat);
	}

	private float GetStatSystemValue(NKM_STAT_TYPE statType)
	{
		return GetStat(m_StatSystemValue, statType);
	}

	private bool SetStatSystemValue(NKM_STAT_TYPE statType, float fStat)
	{
		return SetStat(m_StatSystemValue, statType, fStat);
	}

	private float GetStatBonusBaseValue(NKM_STAT_TYPE statType)
	{
		return GetStat(m_StatBonusBaseValue, statType);
	}

	private bool SetStatBonusBaseValue(NKM_STAT_TYPE statType, float fStat)
	{
		return SetStat(m_StatBonusBaseValue, statType, fStat);
	}

	private float GetStatBonusBaseFactor(NKM_STAT_TYPE statType)
	{
		NKM_STAT_TYPE factorStat = NKMUnitStatManager.GetFactorStat(statType);
		if (factorStat == NKM_STAT_TYPE.NST_END)
		{
			return 0f;
		}
		return GetStatBonusBaseValue(factorStat);
	}

	private float GetStatBuffFinalFactor(NKM_STAT_TYPE statType)
	{
		NKM_STAT_TYPE factorStat = NKMUnitStatManager.GetFactorStat(statType);
		if (factorStat == NKM_STAT_TYPE.NST_END)
		{
			return 0f;
		}
		return GetStatBuffFinalValue(factorStat);
	}

	private float GetStatDebuffFinalFactor(NKM_STAT_TYPE statType)
	{
		NKM_STAT_TYPE factorStat = NKMUnitStatManager.GetFactorStat(statType);
		if (factorStat == NKM_STAT_TYPE.NST_END)
		{
			return 0f;
		}
		return GetStatDebuffFinalValue(factorStat);
	}

	public float GetStatPerLevel(int statType)
	{
		return GetStat(m_StatPerLevel, statType);
	}

	public float GetStatPerLevel(NKM_STAT_TYPE statType)
	{
		return GetStat(m_StatPerLevel, statType);
	}

	public float GetStatMaxPerLevel(NKM_STAT_TYPE statType)
	{
		return GetStat(m_StatMaxPerLevel, statType);
	}

	public float GetStatEXP(NKM_STAT_TYPE statType)
	{
		return 0f;
	}

	public float GetStatEnhanceFeedEXP(int statType)
	{
		return 0f;
	}

	public float GetStatBase(NKM_STAT_TYPE statType)
	{
		return GetStat(m_StatBase, statType);
	}

	public bool SetStatBase(NKM_STAT_TYPE statType, float fStat)
	{
		return SetStat(m_StatBase, statType, fStat);
	}

	public float GetStatFinal(int statType)
	{
		return GetStat(m_StatFinal, statType);
	}

	public float GetStatFinal(NKM_STAT_TYPE statType)
	{
		return GetStat(m_StatFinal, statType);
	}

	public bool SetStatFinal(NKM_STAT_TYPE statType, float fStat)
	{
		return SetStat(m_StatFinal, statType, fStat);
	}

	public float GetRuntimeStatFinal(NKM_STAT_TYPE statType, float hpRate)
	{
		switch (statType)
		{
		case NKM_STAT_TYPE.NST_ATK:
		{
			float statFinal5 = GetStatFinal(NKM_STAT_TYPE.NST_ATK);
			float statFinal6 = GetStatFinal(NKM_STAT_TYPE.NST_HP_GROWN_ATK_RATE);
			float num3 = Math.Max(0f, statFinal6 * (1f - hpRate));
			return statFinal5 * (1f + num3);
		}
		case NKM_STAT_TYPE.NST_DEF:
		{
			float statFinal3 = GetStatFinal(NKM_STAT_TYPE.NST_DEF);
			float statFinal4 = GetStatFinal(NKM_STAT_TYPE.NST_HP_GROWN_DEF_RATE);
			float num2 = Math.Max(0f, statFinal4 * (1f - hpRate));
			return statFinal3 * (1f + num2);
		}
		case NKM_STAT_TYPE.NST_EVADE:
		{
			float statFinal = GetStatFinal(NKM_STAT_TYPE.NST_EVADE);
			float statFinal2 = GetStatFinal(NKM_STAT_TYPE.NST_HP_GROWN_EVADE_RATE);
			float num = Math.Max(0f, statFinal2 * (1f - hpRate));
			return statFinal * (1f + num);
		}
		default:
			return GetStatFinal(statType);
		}
	}

	public NKMStatData()
	{
		Init();
	}

	public void Init()
	{
		m_StatFinal.Clear();
		m_StatBase.Clear();
		m_StatPerLevel.Clear();
		m_StatMaxPerLevel.Clear();
		m_StatBonusBaseValue.Clear();
		m_StatBuffFinalValue.Clear();
		m_StatDebuffFinalValue.Clear();
		m_StatSystemValue.Clear();
	}

	public bool LoadFromLUA(NKMLua cNKMLua, bool bDungeonRespawn = false)
	{
		LoadStatFromLUA(cNKMLua, "m_Stat", ref m_StatBase);
		LoadStatFromLUA(cNKMLua, "m_StatPerLevel", ref m_StatPerLevel);
		LoadStatFromLUA(cNKMLua, "m_StatMaxPerLevel", ref m_StatMaxPerLevel);
		if (bDungeonRespawn)
		{
			LoadStatFromLUA(cNKMLua, "m_StatValue", ref m_StatBase);
			LoadStatFromLUA(cNKMLua, "m_StatFactor", ref m_StatPerLevel);
		}
		return true;
	}

	private void LoadStatFromLUA(NKMLua cNKMLua, string tableKey, ref Dictionary<NKM_STAT_TYPE, float> refStat)
	{
		refStat.Clear();
		Dictionary<NKM_STAT_TYPE, float> dictionary = cNKMLua.OpenTableAsDictionary<NKM_STAT_TYPE>(tableKey, "tableKey " + tableKey + " Open failed!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 554, bHaltOnFail: false);
		if (dictionary == null)
		{
			return;
		}
		foreach (KeyValuePair<NKM_STAT_TYPE, float> item in dictionary)
		{
			SetStat(refStat, item.Key, item.Value);
		}
	}

	private void CopyStatDic(Dictionary<NKM_STAT_TYPE, float> target, Dictionary<NKM_STAT_TYPE, float> source)
	{
		target.Clear();
		foreach (KeyValuePair<NKM_STAT_TYPE, float> item in source)
		{
			target.Add(item.Key, item.Value);
		}
	}

	public void DeepCopyFromSource(NKMStatData source)
	{
		CopyStatDic(m_StatFinal, source.m_StatFinal);
		CopyStatDic(m_StatBase, source.m_StatBase);
		CopyStatDic(m_StatPerLevel, source.m_StatPerLevel);
		CopyStatDic(m_StatMaxPerLevel, source.m_StatMaxPerLevel);
		CopyStatDic(m_StatBonusBaseValue, source.m_StatBonusBaseValue);
		CopyStatDic(m_StatBuffFinalValue, source.m_StatBuffFinalValue);
		CopyStatDic(m_StatDebuffFinalValue, source.m_StatDebuffFinalValue);
		CopyStatDic(m_StatSystemValue, source.m_StatSystemValue);
	}

	public float CalculateOperatorStat(NKM_STAT_TYPE type, NKMStatData unitStatData, int level)
	{
		if (unitStatData == null)
		{
			return 0f;
		}
		return unitStatData.GetStatBase(type) + unitStatData.GetStatPerLevel(type) * (float)(level - 1);
	}

	public float GetDungeonRespawnAddStat(NKMDungeonRespawnUnitTemplet cDungeonRespawnUnitTemplet, float fTargetStat, NKM_STAT_TYPE statType)
	{
		if (cDungeonRespawnUnitTemplet != null)
		{
			if (cDungeonRespawnUnitTemplet.m_AddStatData.GetStatBase(statType) != 0f)
			{
				return fTargetStat + cDungeonRespawnUnitTemplet.m_AddStatData.GetStatBase(statType);
			}
			if (cDungeonRespawnUnitTemplet.m_AddStatData.GetStatPerLevel(statType) != 0f)
			{
				return fTargetStat * cDungeonRespawnUnitTemplet.m_AddStatData.GetStatPerLevel(statType);
			}
		}
		return fTargetStat;
	}

	public void MakeBaseStat(NKMGameData cNKMGameData, bool bPvP, NKMUnitData unitData, NKMStatData unitStatData, bool bPure = false, int buffUnitLevel = 0, NKMOperator cNKMOperator = null, bool bInitStat = true)
	{
		if (bInitStat)
		{
			DeepCopyFromSource(unitStatData);
		}
		int num = unitData.m_UnitLevel + buffUnitLevel;
		if (num < 1)
		{
			num = 1;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		int operatorBanLevel = 0;
		if (bPvP && cNKMGameData != null && cNKMOperator != null && unitTempletBase.IsShip() && cNKMGameData.IsBanOperator(cNKMOperator.id))
		{
			operatorBanLevel = cNKMGameData.GetBanOperatorLevel(cNKMOperator.id);
		}
		foreach (NKM_STAT_TYPE value in Enum.GetValues(typeof(NKM_STAT_TYPE)))
		{
			if (NKMUnitStatManager.IsMainStat(value))
			{
				NKMGameStatRate cGameStatRate = cNKMGameData?.GameStatRate;
				if (bPure)
				{
					float fTargetStat = NKMUnitStatManager.CalculateStat(value, unitStatData, unitData.m_listStatEXP, num, 0, 0f, cGameStatRate, null, 0, unitTempletBase.m_NKM_UNIT_TYPE);
					fTargetStat = GetDungeonRespawnAddStat(unitData.m_DungeonRespawnUnitTemplet, fTargetStat, value);
					SetStatBase(value, fTargetStat);
				}
				else
				{
					float fTargetStat2 = NKMUnitStatManager.CalculateStat(value, unitStatData, unitData.m_listStatEXP, num, unitData.m_LimitBreakLevel, unitData.GetMultiplierByPermanentContract(), cGameStatRate, cNKMOperator, operatorBanLevel, unitTempletBase.m_NKM_UNIT_TYPE);
					fTargetStat2 = GetDungeonRespawnAddStat(unitData.m_DungeonRespawnUnitTemplet, fTargetStat2, value);
					SetStatBase(value, fTargetStat2);
				}
			}
			else
			{
				float dungeonRespawnAddStat = GetDungeonRespawnAddStat(unitData.m_DungeonRespawnUnitTemplet, unitStatData.GetStatBase(value), value);
				SetStatBase(value, dungeonRespawnAddStat);
			}
		}
		NKM_TEAM_TYPE teamType = NKM_TEAM_TYPE.NTT_INVALID;
		if (cNKMGameData != null)
		{
			teamType = cNKMGameData.GetTeamType(unitData.m_UserUID);
		}
		if (bPvP && cNKMGameData != null && cNKMGameData.IsUpUnit(unitTempletBase.m_UnitID) && NKMGame.ApplyUpBanByGameType(cNKMGameData, teamType))
		{
			int upUnitLevel = cNKMGameData.GetUpUnitLevel(unitTempletBase.m_UnitID);
			float num2 = GetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE) + NKMUnitStatManager.m_fPercentPerUpLevel * (float)upUnitLevel;
			if (num2 > GetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE) + NKMUnitStatManager.m_fMaxPercentPerUpLevel * (float)upUnitLevel)
			{
				num2 = GetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE) + NKMUnitStatManager.m_fMaxPercentPerUpLevel * (float)upUnitLevel;
			}
			SetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE, num2);
			for (int i = 0; i <= 5; i++)
			{
				NKM_STAT_TYPE statType = (NKM_STAT_TYPE)i;
				num2 = GetStatBase(statType) * (1f + NKMUnitStatManager.m_fPercentPerUpLevel * (float)upUnitLevel);
				if (num2 > GetStatBase(statType) * (1f + NKMUnitStatManager.m_fMaxPercentPerUpLevel))
				{
					num2 = GetStatBase(statType) * (1f + NKMUnitStatManager.m_fMaxPercentPerUpLevel);
				}
				SetStatBase(statType, num2);
			}
		}
		if (!bPvP || cNKMGameData == null || !unitTempletBase.IsShip() || !NKMGame.ApplyUpBanByGameType(cNKMGameData, teamType))
		{
			return;
		}
		if (cNKMGameData.IsBanShip(unitTempletBase.m_ShipGroupID))
		{
			int banShipLevel = cNKMGameData.GetBanShipLevel(unitTempletBase.m_ShipGroupID);
			float num3 = GetStatBase(NKM_STAT_TYPE.NST_ATK) * (1f - NKMUnitStatManager.m_fPercentPerBanLevel * (float)banShipLevel);
			if (num3 < GetStatBase(NKM_STAT_TYPE.NST_ATK) * (1f - NKMUnitStatManager.m_fMaxPercentPerBanLevel))
			{
				num3 = GetStatBase(NKM_STAT_TYPE.NST_ATK) * (1f - NKMUnitStatManager.m_fMaxPercentPerBanLevel);
			}
			SetStatBase(NKM_STAT_TYPE.NST_ATK, num3);
			num3 = GetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE) - NKMUnitStatManager.m_fPercentPerBanLevel * (float)banShipLevel;
			if (num3 < GetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE) - NKMUnitStatManager.m_fMaxPercentPerBanLevel)
			{
				num3 = GetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE) - NKMUnitStatManager.m_fMaxPercentPerBanLevel;
			}
			SetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE, num3);
		}
		if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PVP_LEAGUE)
		{
			NKMPvpCommonConst.LeaguePvpConst leaguePvp = NKMPvpCommonConst.Instance.LeaguePvp;
			float statBase = GetStatBase(NKM_STAT_TYPE.NST_HP);
			float num4 = statBase * leaguePvp.ShipHpMultiply;
			Log.Debug($"[UnitStat] leaguePvp ship HP:{statBase}->{num4} unitName:{unitTempletBase.Name}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 733);
			SetStatBase(NKM_STAT_TYPE.NST_HP, num4);
			statBase = GetStatBase(NKM_STAT_TYPE.NST_ATK);
			num4 = statBase * leaguePvp.ShipAttackPowerMultiply;
			Log.Debug($"[UnitStat] leaguePvp ship Atk:{statBase}->{num4} unitName:{unitTempletBase.Name}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 738);
			SetStatBase(NKM_STAT_TYPE.NST_ATK, num4);
		}
	}

	public void MakeBaseBonusFactor(NKMUnitData unitData, IReadOnlyDictionary<long, NKMEquipItemData> dicEquipItemData, List<NKMShipCmdModule> ShipCommandModule, NKMGameStatRate cGameStatRate)
	{
		if (unitData == null)
		{
			Log.Error("UnitData is null.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 754);
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase == null)
		{
			Log.Error($"Invalid UnitTemplet. UnitId : {unitData.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 762);
			return;
		}
		bool flag = true;
		m_StatBonusBaseValue.Clear();
		if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			int unitSkillCount = unitData.GetUnitSkillCount();
			for (int i = 0; i < unitSkillCount; i++)
			{
				NKMUnitSkillTemplet unitSkillTempletByIndex = unitData.GetUnitSkillTempletByIndex(i);
				if (unitSkillTempletByIndex == null || unitSkillTempletByIndex.m_UnlockReqUpgrade > unitData.m_LimitBreakLevel)
				{
					continue;
				}
				foreach (SkillStatData lstSkillStatDatum in unitSkillTempletByIndex.m_lstSkillStatData)
				{
					if (lstSkillStatDatum.m_NKM_STAT_TYPE < NKM_STAT_TYPE.NST_END)
					{
						flag &= SetStatBonusBaseValue(lstSkillStatDatum.m_NKM_STAT_TYPE, GetStatBonusBaseValue(lstSkillStatDatum.m_NKM_STAT_TYPE) + lstSkillStatDatum.m_fStatValue);
					}
				}
			}
		}
		if (dicEquipItemData != null)
		{
			float num = cGameStatRate?.GetEquipStatRate() ?? 1f;
			for (int j = 0; j < 4; j++)
			{
				long equipUid = unitData.GetEquipUid((ITEM_EQUIP_POSITION)j);
				if (equipUid <= 0 || !dicEquipItemData.TryGetValue(equipUid, out var value))
				{
					continue;
				}
				bool flag2 = unitData.m_UnitID == value.m_ImprintUnitId;
				NKMCommonConst.ImprintMainOptEffect imprintMainOptEffect = (flag2 ? NKMCommonConst.GetEquipIimprintMainOptEffect((ITEM_EQUIP_POSITION)j) : new NKMCommonConst.ImprintMainOptEffect(1f, 1f));
				if (value.m_Stat == null)
				{
					Log.Warn($"[CrashReport1] UnitUID[{unitData.m_UnitUID}] ItemUID[{value.m_ItemUid}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 813);
				}
				foreach (var (eQUIP_ITEM_STAT, num2) in value.m_Stat.Select((EQUIP_ITEM_STAT stat, int index) => (stat: stat, index: index)))
				{
					if (eQUIP_ITEM_STAT == null)
					{
						Log.Warn($"[CrashReport2] UnitUID[{unitData.m_UnitUID}] ItemUID[{value.m_ItemUid}] Index[{num2}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 821);
					}
					float num3 = (eQUIP_ITEM_STAT.stat_value + eQUIP_ITEM_STAT.stat_level_value * (float)value.m_EnchantLevel) * num;
					if (flag2 && num2 == 0)
					{
						bool isMainStat = NKMUnitStatManager.IsMainStat(eQUIP_ITEM_STAT.type);
						num3 *= imprintMainOptEffect.GetMultiplyValue(isMainStat);
					}
					flag &= SetStatBonusBaseValue(eQUIP_ITEM_STAT.type, GetStatBonusBaseValue(eQUIP_ITEM_STAT.type) + num3);
				}
				if (value.potentialOptions.Count <= 0)
				{
					continue;
				}
				for (int num4 = 0; num4 < value.potentialOptions.Count; num4++)
				{
					NKMPotentialOption nKMPotentialOption = value.potentialOptions[num4];
					float num5 = 0f;
					NKMPotentialOption.SocketData[] sockets = nKMPotentialOption.sockets;
					foreach (NKMPotentialOption.SocketData socketData in sockets)
					{
						if (socketData == null)
						{
							break;
						}
						num5 += socketData.statValue;
					}
					flag &= SetStatBonusBaseValue(nKMPotentialOption.statType, GetStatBonusBaseValue(nKMPotentialOption.statType) + num5 * num);
				}
			}
			dicEquipItemData.TryGetValue(unitData.GetEquipItemWeaponUid(), out var value2);
			dicEquipItemData.TryGetValue(unitData.GetEquipItemDefenceUid(), out var value3);
			dicEquipItemData.TryGetValue(unitData.GetEquipItemAccessoryUid(), out var value4);
			dicEquipItemData.TryGetValue(unitData.GetEquipItemAccessory2Uid(), out var value5);
			List<NKMItemEquipSetOptionTemplet> activatedSetItem = NKMItemManager.GetActivatedSetItem(new NKMEquipmentSet(value2, value3, value4, value5));
			for (int num7 = 0; num7 < activatedSetItem.Count; num7++)
			{
				NKMItemEquipSetOptionTemplet nKMItemEquipSetOptionTemplet = activatedSetItem[num7];
				if (nKMItemEquipSetOptionTemplet.m_StatType_1 != NKM_STAT_TYPE.NST_RANDOM && nKMItemEquipSetOptionTemplet.m_StatType_1 != NKM_STAT_TYPE.NST_END)
				{
					flag &= SetStatBonusBaseValue(nKMItemEquipSetOptionTemplet.m_StatType_1, GetStatBonusBaseValue(nKMItemEquipSetOptionTemplet.m_StatType_1) + nKMItemEquipSetOptionTemplet.m_StatValue_1 * num);
				}
				if (nKMItemEquipSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_RANDOM && nKMItemEquipSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_END)
				{
					flag &= SetStatBonusBaseValue(nKMItemEquipSetOptionTemplet.m_StatType_2, GetStatBonusBaseValue(nKMItemEquipSetOptionTemplet.m_StatType_2) + nKMItemEquipSetOptionTemplet.m_StatValue_2 * num);
				}
			}
		}
		if (ShipCommandModule != null)
		{
			for (int num8 = 0; num8 < ShipCommandModule.Count; num8++)
			{
				if (ShipCommandModule[num8] == null || ShipCommandModule[num8].slots == null)
				{
					continue;
				}
				for (int num9 = 0; num9 < ShipCommandModule[num8].slots.Length; num9++)
				{
					NKMShipCmdSlot nKMShipCmdSlot = ShipCommandModule[num8].slots[num9];
					if (nKMShipCmdSlot != null && nKMShipCmdSlot.CanApply(unitData))
					{
						flag &= SetStatBonusBaseValue(nKMShipCmdSlot.statType, GetStatBonusBaseValue(nKMShipCmdSlot.statType) + nKMShipCmdSlot.statValue);
					}
				}
			}
		}
		if (unitData.reactorLevel > 0)
		{
			NKMUnitReactorTemplet nKMUnitReactorTemplet = NKMUnitReactorTemplet.Find(unitTempletBase.m_ReactorId);
			if (nKMUnitReactorTemplet != null)
			{
				NKMReactorSkillTemplet skillTemplets = nKMUnitReactorTemplet.GetSkillTemplets(unitData.reactorLevel);
				if (skillTemplets != null)
				{
					foreach (SkillStatData statData in skillTemplets.StatDatas)
					{
						flag &= SetStatBonusBaseValue(statData.m_NKM_STAT_TYPE, GetStatBonusBaseValue(statData.m_NKM_STAT_TYPE) + statData.m_fStatValue);
					}
				}
			}
		}
		for (int num10 = 1; num10 <= unitData.tacticLevel; num10++)
		{
			NKMTacticUpdateTemplet.TacticUpdateData tacticUpdateData = NKMTacticUpdateTemplet.Find(unitTempletBase, num10);
			if (tacticUpdateData == null)
			{
				Log.Error($"NKMTacticUpdateTemplet level {num10} not found!!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 928);
				flag = false;
			}
			else
			{
				flag &= SetStatBonusBaseValue(tacticUpdateData.m_StatType, GetStatBonusBaseValue(tacticUpdateData.m_StatType) + tacticUpdateData.m_StatValue * 0.0001f);
			}
		}
		if (!flag)
		{
			Log.Error($"[UnitStat] stat setting failed. userUid:{unitData.m_UserUID} unitUid:{unitData.m_UnitUID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 937);
		}
	}

	public static float GetBuffStatVal(NKM_STAT_TYPE statType, int statVal, int statPerLevel, byte buffLevel, byte buffOverlap)
	{
		if (statVal == 0)
		{
			return 0f;
		}
		return (float)((statVal + statPerLevel * (buffLevel - 1)) * buffOverlap) * 0.0001f;
	}

	public void AddBuffStat(NKM_STAT_TYPE statType, int statVal, int statPerLevel, byte buffLevel, byte buffOverlap, bool bDebuff, short casterUID, bool bSystem)
	{
		if (bSystem)
		{
			float num = (float)((statVal + statPerLevel * (buffLevel - 1)) * buffOverlap) * 0.0001f;
			SetStatSystemValue(statType, GetStatSystemValue(statType) + num);
		}
		else if (IsSecondaryBuffStat(statType))
		{
			SecondaryBuffStatInfo item = new SecondaryBuffStatInfo
			{
				statType = statType,
				value = (float)((statVal + statPerLevel * (buffLevel - 1)) * buffOverlap) * 0.0001f,
				isDebuff = bDebuff,
				casterUID = casterUID
			};
			m_lstSecondaryBuff.Add(item);
		}
		else
		{
			float num2 = (float)((statVal + statPerLevel * (buffLevel - 1)) * buffOverlap) * 0.0001f;
			if (bDebuff)
			{
				SetStatDebuffFinalValue(statType, GetStatDebuffFinalValue(statType) + num2);
			}
			else
			{
				SetStatBuffFinalValue(statType, GetStatBuffFinalValue(statType) + num2);
			}
		}
	}

	private void AddSecondaryBuffStat(NKMUnit self, SecondaryBuffStatInfo statInfo, NKMGame cNKMGame)
	{
		NKMUnit unit = cNKMGame.GetUnit(statInfo.casterUID);
		float num = ProcessSecondaryBuffStat(self, unit, statInfo.statType, statInfo.value);
		if (statInfo.isDebuff)
		{
			SetStatDebuffFinalValue(statInfo.statType, GetStatDebuffFinalValue(statInfo.statType) + num);
		}
		else
		{
			SetStatBuffFinalValue(statInfo.statType, GetStatBuffFinalValue(statInfo.statType) + num);
		}
	}

	private float ProcessSecondaryBuffStat(NKMUnit self, NKMUnit caster, NKM_STAT_TYPE statType, float value)
	{
		if (statType == NKM_STAT_TYPE.NST_HP_REGEN_RATE)
		{
			if (value <= 0f)
			{
				if (self != null && self.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_HP_RATE_DAMAGE))
				{
					return 0f;
				}
				return value;
			}
			float num = 0f;
			if (caster != null)
			{
				num = caster.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HEAL_RATE);
			}
			float num2 = 1f + num - GetStatFinal(NKM_STAT_TYPE.NST_HEAL_REDUCE_RATE);
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			return value * num2;
		}
		return value;
	}

	private float GetSecondaryExtraFactor(NKM_STAT_TYPE statType)
	{
		if (statType == NKM_STAT_TYPE.NST_HP_REGEN_RATE)
		{
			float num = 1f - GetStatFinal(NKM_STAT_TYPE.NST_HEAL_REDUCE_RATE);
			if (num < 0f)
			{
				num = 0f;
			}
			return num;
		}
		return 1f;
	}

	public void UpdateFinalStat(NKMGame cNKMGame, NKMGameStatRate cGameStatRate, NKMUnit cNKMUnit, bool bConserveHPRate = false)
	{
		if (cNKMUnit != null)
		{
			NKMUnitFrameData unitFrameData = cNKMUnit.GetUnitFrameData();
			NKMUnitSyncData unitSyncData = cNKMUnit.GetUnitSyncData();
			bool flag = false;
			float num = 1f;
			if (unitSyncData != null)
			{
				num = unitSyncData.GetHP() / GetStatFinal(NKM_STAT_TYPE.NST_HP);
				flag = unitSyncData.GetHP() >= GetStatFinal(NKM_STAT_TYPE.NST_HP);
			}
			m_StatBuffFinalValue.Clear();
			m_StatDebuffFinalValue.Clear();
			m_lstSecondaryBuff.Clear();
			m_StatSystemValue.Clear();
			foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in unitFrameData.m_dicBuffData)
			{
				NKMBuffData value = dicBuffDatum.Value;
				if (value.m_BuffSyncData.m_bAffect)
				{
					bool bDebuff = value.m_NKMBuffTemplet.m_bDebuff;
					if (value.m_BuffSyncData.m_bRangeSon)
					{
						bDebuff = value.m_NKMBuffTemplet.m_bDebuffSon;
					}
					if (value.m_NKMBuffTemplet.m_StatType1 != NKM_STAT_TYPE.NST_END)
					{
						AddBuffStat(value.m_NKMBuffTemplet.m_StatType1, value.m_NKMBuffTemplet.m_StatValue1, value.m_NKMBuffTemplet.m_StatAddPerLevel1, value.m_BuffSyncData.m_BuffStatLevel, value.m_BuffSyncData.m_OverlapCount, bDebuff, value.m_BuffSyncData.m_MasterGameUnitUID, value.m_NKMBuffTemplet.m_bSystem);
					}
					if (value.m_NKMBuffTemplet.m_StatType2 != NKM_STAT_TYPE.NST_END)
					{
						AddBuffStat(value.m_NKMBuffTemplet.m_StatType2, value.m_NKMBuffTemplet.m_StatValue2, value.m_NKMBuffTemplet.m_StatAddPerLevel2, value.m_BuffSyncData.m_BuffStatLevel, value.m_BuffSyncData.m_OverlapCount, bDebuff, value.m_BuffSyncData.m_MasterGameUnitUID, value.m_NKMBuffTemplet.m_bSystem);
					}
					if (value.m_NKMBuffTemplet.m_StatType3 != NKM_STAT_TYPE.NST_END)
					{
						AddBuffStat(value.m_NKMBuffTemplet.m_StatType3, value.m_NKMBuffTemplet.m_StatValue3, value.m_NKMBuffTemplet.m_StatAddPerLevel3, value.m_BuffSyncData.m_BuffStatLevel, value.m_BuffSyncData.m_OverlapCount, bDebuff, value.m_BuffSyncData.m_MasterGameUnitUID, value.m_NKMBuffTemplet.m_bSystem);
					}
				}
			}
			if (cNKMUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_STABLE_HP))
			{
				SetStatBuffFinalValue(NKM_STAT_TYPE.NST_HP_FACTOR, 0f);
				SetStatBuffFinalValue(NKM_STAT_TYPE.NST_HP, 0f);
				SetStatDebuffFinalValue(NKM_STAT_TYPE.NST_HP_FACTOR, 0f);
				SetStatDebuffFinalValue(NKM_STAT_TYPE.NST_HP, 0f);
			}
			ApplyBuffStatToFinalStat(cNKMUnit, cGameStatRate, NKM_STAT_TYPE.NST_CC_RESIST_RATE, bApplyResistDebuff: false, bSecondary: false);
			foreach (NKM_STAT_TYPE value2 in Enum.GetValues(typeof(NKM_STAT_TYPE)))
			{
				if (value2 != NKM_STAT_TYPE.NST_CC_RESIST_RATE && !IsSecondaryBuffStat(value2))
				{
					ApplyBuffStatToFinalStat(cNKMUnit, cGameStatRate, value2, value2 != NKM_STAT_TYPE.NST_HP_REGEN_RATE, bSecondary: false);
				}
			}
			foreach (SecondaryBuffStatInfo item in m_lstSecondaryBuff)
			{
				AddSecondaryBuffStat(cNKMUnit, item, cNKMGame);
			}
			foreach (NKM_STAT_TYPE item2 in s_hsSecondaryBuffStat)
			{
				ApplyBuffStatToFinalStat(cNKMUnit, cGameStatRate, item2, item2 != NKM_STAT_TYPE.NST_HP_REGEN_RATE, bSecondary: true);
			}
			if (GetStatFinal(NKM_STAT_TYPE.NST_MAIN_STAT_RATE) != 0f)
			{
				float num2 = 1f + GetStatFinal(NKM_STAT_TYPE.NST_MAIN_STAT_RATE);
				if (!cNKMUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_STABLE_HP))
				{
					float num3 = GetStatFinal(NKM_STAT_TYPE.NST_HP) * num2;
					if (num3 < 1f)
					{
						num3 = 1f;
					}
					SetStatFinal(NKM_STAT_TYPE.NST_HP, num3);
				}
				SetStatFinal(NKM_STAT_TYPE.NST_ATK, GetStatFinal(NKM_STAT_TYPE.NST_ATK) * num2);
				SetStatFinal(NKM_STAT_TYPE.NST_DEF, GetStatFinal(NKM_STAT_TYPE.NST_DEF) * num2);
				SetStatFinal(NKM_STAT_TYPE.NST_CRITICAL, GetStatFinal(NKM_STAT_TYPE.NST_CRITICAL) * num2);
				SetStatFinal(NKM_STAT_TYPE.NST_HIT, GetStatFinal(NKM_STAT_TYPE.NST_HIT) * num2);
				SetStatFinal(NKM_STAT_TYPE.NST_EVADE, GetStatFinal(NKM_STAT_TYPE.NST_EVADE) * num2);
			}
			if (unitSyncData != null)
			{
				if (bConserveHPRate)
				{
					unitSyncData.SetHP(GetStatFinal(NKM_STAT_TYPE.NST_HP) * num);
				}
				else if (flag)
				{
					unitSyncData.SetHP(GetStatFinal(NKM_STAT_TYPE.NST_HP));
				}
				else if (unitSyncData.GetHP() >= GetStatFinal(NKM_STAT_TYPE.NST_HP))
				{
					unitSyncData.SetHP(GetStatFinal(NKM_STAT_TYPE.NST_HP));
				}
			}
			NKMUnitTempletBase unitTempletBase = cNKMUnit.GetUnitTempletBase();
			if (unitTempletBase != null)
			{
				if (!unitTempletBase.HasSourceType(NKM_UNIT_SOURCE_TYPE.NUST_LIBERAL))
				{
					SetStatFinal(NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_LIBERAL_G4, 0f);
					SetStatFinal(NKM_STAT_TYPE.NST_DEFEND_VS_SOURCE_LIBERAL_G4, 0f);
				}
				if (!unitTempletBase.HasSourceType(NKM_UNIT_SOURCE_TYPE.NUST_CONFLICT))
				{
					SetStatFinal(NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_CONFLICT_G4, 0f);
					SetStatFinal(NKM_STAT_TYPE.NST_DEFEND_VS_SOURCE_CONFLICT_G4, 0f);
				}
				if (!unitTempletBase.HasSourceType(NKM_UNIT_SOURCE_TYPE.NUST_STABLE))
				{
					SetStatFinal(NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_STABLE_G4, 0f);
					SetStatFinal(NKM_STAT_TYPE.NST_DEFEND_VS_SOURCE_STABLE_G4, 0f);
				}
			}
			return;
		}
		m_StatFinal.Clear();
		foreach (KeyValuePair<NKM_STAT_TYPE, float> item3 in m_StatBase)
		{
			SetStatFinal(item3.Key, item3.Value);
		}
	}

	private void ApplyBuffStatToFinalStat(NKMUnit cNKMUnit, NKMGameStatRate cGameStatRate, NKM_STAT_TYPE stattype, bool bApplyResistDebuff, bool bSecondary)
	{
		float num = cGameStatRate?.GetStatValueRate(stattype) ?? 1f;
		float num2 = cGameStatRate?.GetStatFactorRate(stattype) ?? 1f;
		float num3 = 1f;
		if (bApplyResistDebuff)
		{
			num3 -= GetStatFinal(NKM_STAT_TYPE.NST_CC_RESIST_RATE);
		}
		float num4 = GetStatBonusBaseFactor(stattype) + GetStatBuffFinalFactor(stattype) + GetStatDebuffFinalFactor(stattype) * num3;
		float num5 = 1f;
		float num6;
		if (bSecondary)
		{
			num6 = ProcessSecondaryBuffStat(cNKMUnit, null, stattype, GetStatBonusBaseValue(stattype)) + GetStatBuffFinalValue(stattype) + GetStatDebuffFinalValue(stattype) * num3;
			num5 = GetSecondaryExtraFactor(stattype);
		}
		else
		{
			num6 = GetStatBonusBaseValue(stattype) + GetStatBuffFinalValue(stattype) + GetStatDebuffFinalValue(stattype) * num3;
		}
		float num7 = GetStatBase(stattype) * (1f + num4 * num2) * num5 + num6 * num;
		switch (stattype)
		{
		case NKM_STAT_TYPE.NST_MOVE_SPEED_RATE:
			if (num7 < 0f && cNKMUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_MOVE_SLOW))
			{
				num7 = 0f;
			}
			break;
		case NKM_STAT_TYPE.NST_ATTACK_SPEED_RATE:
			if (num7 < 0f && cNKMUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_ATTACK_SLOW))
			{
				num7 = 0f;
			}
			break;
		}
		num7 += GetStatSystemValue(stattype);
		num7 = ApplyStatCap(stattype, num7);
		SetStatFinal(stattype, num7);
	}

	private float ApplyStatCap(NKM_STAT_TYPE statType, float value)
	{
		switch (statType)
		{
		case NKM_STAT_TYPE.NST_HP:
			if (value < 1f)
			{
				value = 1f;
			}
			break;
		case NKM_STAT_TYPE.NST_HP_REGEN_RATE:
		case NKM_STAT_TYPE.NST_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_MOVE_SPEED_RATE:
		case NKM_STAT_TYPE.NST_ATTACK_SPEED_RATE:
		case NKM_STAT_TYPE.NST_ATTACK_DAMAGE_MODIFY_G2:
			if (value < -0.9f)
			{
				value = -0.9f;
			}
			break;
		case NKM_STAT_TYPE.NST_CC_RESIST_RATE:
			value = value.Clamp(-1f, 0.8f);
			break;
		case NKM_STAT_TYPE.NST_DAMAGE_BACK_RESIST:
			value = value.Clamp(-1f, 1f);
			break;
		case NKM_STAT_TYPE.NST_MAIN_STAT_RATE:
			if (value < -0.99f)
			{
				value = -0.99f;
			}
			break;
		case NKM_STAT_TYPE.NST_DEF_PENETRATE_RATE:
			value = value.Clamp(0f, 1f);
			break;
		case NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE:
			if (value < -1f)
			{
				value = -1f;
			}
			break;
		case NKM_STAT_TYPE.NST_UNIT_TYPE_COUNTER_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_UNIT_TYPE_COUNTER_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_UNIT_TYPE_SOLDIER_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_UNIT_TYPE_SOLDIER_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_UNIT_TYPE_MECHANIC_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_UNIT_TYPE_MECHANIC_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_STRIKER_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_STRIKER_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_RANGER_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_RANGER_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_SNIPER_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_SNIPER_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_DEFFENDER_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_DEFFENDER_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_SUPPOERTER_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_SUPPOERTER_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_SIEGE_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_SIEGE_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_TOWER_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_TOWER_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_MOVE_TYPE_LAND_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_MOVE_TYPE_LAND_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_MOVE_TYPE_AIR_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_MOVE_TYPE_AIR_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_LONG_RANGE_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_LONG_RANGE_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_SHORT_RANGE_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_SHORT_RANGE_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_HEAL_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_SKILL_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_HYPER_SKILL_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_UNIT_TYPE_CORRUPTED_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_UNIT_TYPE_CORRUPTED_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_UNIT_TYPE_REPLACER_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_UNIT_TYPE_REPLACER_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_DAMAGE_RATE:
		case NKM_STAT_TYPE.NST_ROLE_TYPE_DAMAGE_REDUCE_RATE:
		case NKM_STAT_TYPE.NST_HP_GROWN_ATK_RATE:
		case NKM_STAT_TYPE.NST_HP_GROWN_DEF_RATE:
		case NKM_STAT_TYPE.NST_HP_GROWN_EVADE_RATE:
		case NKM_STAT_TYPE.NST_NON_CRITICAL_DAMAGE_TAKE_RATE:
		case NKM_STAT_TYPE.NST_HEAL_RATE:
		case NKM_STAT_TYPE.NST_ATTACK_VS_BOSS_DAMAGE_MODIFY_G1:
		case NKM_STAT_TYPE.NST_DEFEND_VS_BOSS_DAMAGE_MODIFY_G1:
		case NKM_STAT_TYPE.NST_ATTACK_VS_SUMMON_DAMAGE_MODIFY_G1:
		case NKM_STAT_TYPE.NST_DEFEND_VS_SUMMON_DAMAGE_MODIFY_G1:
			if (value < -2f)
			{
				value = -2f;
			}
			break;
		case NKM_STAT_TYPE.NST_COST_RETURN_RATE:
			value = value.Clamp(0f, 0.5f);
			break;
		case NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_CONFLICT_G4:
		case NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_STABLE_G4:
		case NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_LIBERAL_G4:
			value = value.Clamp(0f, NKMUnitStatManager.m_fSourceAttackStatMax);
			break;
		case NKM_STAT_TYPE.NST_DAMAGE_RESIST:
			value = value.Clamp(-0.8f, 0.8f);
			break;
		case NKM_STAT_TYPE.NST_ATTACK_ATTACK_DAMAGE_MODIFY_G2:
			value = value.Clamp(-2f, 2f);
			break;
		case NKM_STAT_TYPE.NST_DEFEND_ATTACK_DAMAGE_MODIFY_G2:
			value = value.Clamp(0f, 0.5f);
			break;
		default:
			if (value < 0f)
			{
				value = 0f;
			}
			break;
		case NKM_STAT_TYPE.NST_EXTRA_ADJUST_DAMAGE_DEALT:
		case NKM_STAT_TYPE.NST_EXTRA_ADJUST_DAMAGE_RECEIVE:
			break;
		}
		return value;
	}

	public float GetBaseBonusStat(NKM_STAT_TYPE eNKM_STAT_TYPE)
	{
		if (eNKM_STAT_TYPE >= NKM_STAT_TYPE.NST_END)
		{
			return 0f;
		}
		return GetStatBase(eNKM_STAT_TYPE) * GetStatBonusBaseFactor(eNKM_STAT_TYPE) + GetStatBonusBaseValue(eNKM_STAT_TYPE);
	}

	public HashSet<NKM_STAT_TYPE> GetUnitBonusStatList()
	{
		HashSet<NKM_STAT_TYPE> hashSet = new HashSet<NKM_STAT_TYPE>();
		hashSet.UnionWith(m_StatBase.Keys);
		hashSet.UnionWith(m_StatBonusBaseValue.Keys);
		return hashSet;
	}
}
