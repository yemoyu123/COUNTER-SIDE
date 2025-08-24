using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMBuffTemplet : INKMTemplet
{
	public enum BuffEndDTType
	{
		NoUse,
		End,
		Dispel
	}

	public enum AffectSummonType : byte
	{
		All,
		SummonOnly,
		SummonNo
	}

	public short m_BuffID;

	public string m_BuffStrID = "";

	public bool m_bSystem;

	public bool m_bDebuff;

	public bool m_bDebuffSon;

	public bool m_bInfinity;

	public float m_fLifeTime = -1f;

	public float m_fLifeTimePerLevel;

	public float m_Range;

	public int m_RangeSonCount = 99999;

	public bool m_RangeOverlap;

	public bool m_bUseUnitSize;

	public bool m_bNoRefresh;

	public byte m_MaxOverlapCount = 1;

	public string m_MaxOverlapBuffStrID = "";

	public int m_DecreaseOverlapOnTimeover;

	public int m_DecreaseStatLevelOnTimeover;

	public bool m_bShipSkillPos;

	public float m_fOffsetX;

	public bool m_bShowBuffIcon = true;

	public bool m_bShowBuffText = true;

	public string m_IconName = "";

	public string m_RangeEffectName = "";

	public string m_MasterEffectName = "";

	public string m_MasterEffectNameSkinDic = "";

	public string m_MasterEffectBoneName = "";

	public string m_SlaveEffectName = "";

	public string m_SlaveEffectNameSkinDic = "";

	public string m_SlaveEffectBoneName = "";

	public bool m_bIgnoreUnitScaleFactor;

	public float m_MasterColorR = -1f;

	public float m_MasterColorG = -1f;

	public float m_MasterColorB = -1f;

	public float m_ColorR = -1f;

	public float m_ColorG = -1f;

	public float m_ColorB = -1f;

	public bool m_bNoReuse;

	public bool m_bAllowBoss = true;

	public List<int> m_listAllowUnitID = new List<int>();

	public List<int> m_listIgnoreUnitID = new List<int>();

	public HashSet<NKM_UNIT_STYLE_TYPE> m_listAllowStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public HashSet<NKM_UNIT_STYLE_TYPE> m_listIgnoreStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public HashSet<NKM_UNIT_ROLE_TYPE> m_listAllowRoleType = new HashSet<NKM_UNIT_ROLE_TYPE>();

	public HashSet<NKM_UNIT_ROLE_TYPE> m_listIgnoreRoleType = new HashSet<NKM_UNIT_ROLE_TYPE>();

	public HashSet<NKM_UNIT_TAG> m_listAllowTagType = new HashSet<NKM_UNIT_TAG>();

	public HashSet<NKM_UNIT_TAG> m_listIgnoreTagType = new HashSet<NKM_UNIT_TAG>();

	public bool m_bAllowAirUnit = true;

	public bool m_bAllowLandUnit = true;

	public bool m_bAllowAwaken = true;

	public bool m_bAllowNormal = true;

	public bool m_bRangeSonAllowBoss = true;

	public List<int> m_listRangeSonAllowUnitID = new List<int>();

	public List<int> m_listRangeSonIgnoreUnitID = new List<int>();

	public HashSet<NKM_UNIT_STYLE_TYPE> m_listRangeSonAllowStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public HashSet<NKM_UNIT_STYLE_TYPE> m_listRangeSonIgnoreStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public HashSet<NKM_UNIT_ROLE_TYPE> m_listRangeSonAllowRoleType = new HashSet<NKM_UNIT_ROLE_TYPE>();

	public HashSet<NKM_UNIT_ROLE_TYPE> m_listRangeSonIgnoreRoleType = new HashSet<NKM_UNIT_ROLE_TYPE>();

	public HashSet<NKM_UNIT_TAG> m_listRangeSonAllowTagType = new HashSet<NKM_UNIT_TAG>();

	public HashSet<NKM_UNIT_TAG> m_listRangeSonIgnoreTagType = new HashSet<NKM_UNIT_TAG>();

	public bool m_bRangeSonAllowAirUnit = true;

	public bool m_bRangeSonAllowLandUnit = true;

	public bool m_bRangeSonAllowAwaken = true;

	public bool m_bRangeSonAllowNormal = true;

	public bool m_bRangeSonOnlyTarget;

	public bool m_bRangeSonOnlySubTarget;

	public bool m_AffectMe = true;

	public bool m_AffectMasterTeam;

	public bool m_AffectMasterEnemyTeam;

	public int m_AffectMultiRespawnMinCount;

	public int m_AffectSonMultiRespawnMinCount;

	public bool m_bBuffCount;

	public NKMMinMaxInt m_AffectCostRange = new NKMMinMaxInt(-1, -1);

	public NKMMinMaxInt m_RangeSonAffectCostRange = new NKMMinMaxInt(-1, -1);

	public AffectSummonType m_eAffectSummonType;

	public AffectSummonType m_eAffectRangeSonSummonType;

	public byte m_AddAttackUnitCount;

	public float m_fAddAttackRange;

	public NKM_SUPER_ARMOR_LEVEL m_SuperArmorLevel = NKM_SUPER_ARMOR_LEVEL.NSAL_NO;

	public HashSet<NKM_UNIT_STATUS_EFFECT> m_ApplyStatus = new HashSet<NKM_UNIT_STATUS_EFFECT>();

	public HashSet<NKM_UNIT_STATUS_EFFECT> m_ImmuneStatus = new HashSet<NKM_UNIT_STATUS_EFFECT>();

	public bool m_bNotDispel;

	public bool m_bRangeSonDispelBuff;

	public bool m_bRangeSonDispelDebuff;

	public bool m_bDispelBuff;

	public bool m_bDispelDebuff;

	public bool m_bNotCastSummon;

	public bool m_bIgnoreBlock;

	public float m_fDamageTransfer;

	public float m_fDamageReflection;

	public float m_fHealFeedback;

	public float m_fHealFeedbackPerLevel;

	public float m_fHealTransfer;

	public bool m_bGuard;

	public bool m_bBarrierHPRate;

	public float m_fBarrierHP = -1f;

	public float m_fBarrierHPPerLevel;

	public string m_BarrierDamageEffectName = "";

	public string m_DamageTempletStrID = "";

	public NKMDamageTemplet m_NKMDamageTemplet;

	public float m_fOneTimeHPDamageRate;

	public string m_StartDTStrID = "";

	public NKMDamageTemplet m_DTStart;

	public string m_EndDTStrID = "";

	public NKMDamageTemplet m_DTEnd;

	public string m_DispelDTStrID = "";

	public NKMDamageTemplet m_DTDispel;

	public string m_EventHealStrID = "";

	public NKMEventHeal m_NKMEventHeal;

	public bool m_bUnitDieEvent;

	public int m_UnitLevel;

	public string m_FinalUnitStateChange = "";

	public string m_FinalBuffStrID = "";

	public NKM_STAT_TYPE m_StatType1 = NKM_STAT_TYPE.NST_END;

	public int m_StatValue1;

	public int m_StatAddPerLevel1;

	public NKM_STAT_TYPE m_StatType2 = NKM_STAT_TYPE.NST_END;

	public int m_StatValue2;

	public int m_StatAddPerLevel2;

	public NKM_STAT_TYPE m_StatType3 = NKM_STAT_TYPE.NST_END;

	public int m_StatValue3;

	public int m_StatAddPerLevel3;

	private Dictionary<int, string> m_dicMasterEffect;

	private Dictionary<int, string> m_dicSlaveEffect;

	public int Key => m_BuffID;

	public bool IsBarrierBuff => m_fBarrierHP > 0f;

	public static NKMBuffTemplet Find(string key)
	{
		return NKMTempletContainer<NKMBuffTemplet>.Find(key);
	}

	public static NKMBuffTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMBuffTemplet nKMBuffTemplet = new NKMBuffTemplet();
		cNKMLua.GetData("m_BuffID", ref nKMBuffTemplet.m_BuffID);
		cNKMLua.GetData("m_BuffStrID", ref nKMBuffTemplet.m_BuffStrID);
		cNKMLua.GetData("m_bSystem", ref nKMBuffTemplet.m_bSystem);
		cNKMLua.GetData("m_bDebuff", ref nKMBuffTemplet.m_bDebuff);
		nKMBuffTemplet.m_bDebuffSon = nKMBuffTemplet.m_bDebuff;
		cNKMLua.GetData("m_bDebuffSon", ref nKMBuffTemplet.m_bDebuffSon);
		cNKMLua.GetData("m_bInfinity", ref nKMBuffTemplet.m_bInfinity);
		cNKMLua.GetData("m_fLifeTime", ref nKMBuffTemplet.m_fLifeTime);
		cNKMLua.GetData("m_fLifeTimePerLevel", ref nKMBuffTemplet.m_fLifeTimePerLevel);
		cNKMLua.GetData("m_Range", ref nKMBuffTemplet.m_Range);
		cNKMLua.GetData("m_bUseUnitSize", ref nKMBuffTemplet.m_bUseUnitSize);
		cNKMLua.GetData("m_RangeSonCount", ref nKMBuffTemplet.m_RangeSonCount);
		cNKMLua.GetData("m_RangeOverlap", ref nKMBuffTemplet.m_RangeOverlap);
		cNKMLua.GetData("m_bNoRefresh", ref nKMBuffTemplet.m_bNoRefresh);
		cNKMLua.GetData("m_MaxOverlapCount", ref nKMBuffTemplet.m_MaxOverlapCount);
		cNKMLua.GetData("m_MaxOverlapBuffStrID", ref nKMBuffTemplet.m_MaxOverlapBuffStrID);
		cNKMLua.GetData("m_DecreaseOverlapOnTimeover", ref nKMBuffTemplet.m_DecreaseOverlapOnTimeover);
		cNKMLua.GetData("m_DecreaseStatLevelOnTimeover", ref nKMBuffTemplet.m_DecreaseStatLevelOnTimeover);
		cNKMLua.GetData("m_bShipSkillPos", ref nKMBuffTemplet.m_bShipSkillPos);
		cNKMLua.GetData("m_fOffsetX", ref nKMBuffTemplet.m_fOffsetX);
		cNKMLua.GetData("m_bShowBuffIcon", ref nKMBuffTemplet.m_bShowBuffIcon);
		cNKMLua.GetData("m_bShowBuffText", ref nKMBuffTemplet.m_bShowBuffText);
		cNKMLua.GetData("m_IconName", ref nKMBuffTemplet.m_IconName);
		cNKMLua.GetData("m_RangeEffectName", ref nKMBuffTemplet.m_RangeEffectName);
		cNKMLua.GetData("m_MasterEffectName", ref nKMBuffTemplet.m_MasterEffectName);
		cNKMLua.GetData("m_MasterEffectNameSkinDic", ref nKMBuffTemplet.m_MasterEffectNameSkinDic);
		cNKMLua.GetData("m_MasterEffectBoneName", ref nKMBuffTemplet.m_MasterEffectBoneName);
		cNKMLua.GetData("m_SlaveEffectName", ref nKMBuffTemplet.m_SlaveEffectName);
		cNKMLua.GetData("m_SlaveEffectNameSkinDic", ref nKMBuffTemplet.m_SlaveEffectNameSkinDic);
		cNKMLua.GetData("m_SlaveEffectBoneName", ref nKMBuffTemplet.m_SlaveEffectBoneName);
		cNKMLua.GetData("m_bIgnoreUnitScaleFactor", ref nKMBuffTemplet.m_bIgnoreUnitScaleFactor);
		cNKMLua.GetData("m_MasterColorR", ref nKMBuffTemplet.m_MasterColorR);
		cNKMLua.GetData("m_MasterColorG", ref nKMBuffTemplet.m_MasterColorG);
		cNKMLua.GetData("m_MasterColorB", ref nKMBuffTemplet.m_MasterColorB);
		cNKMLua.GetData("m_ColorR", ref nKMBuffTemplet.m_ColorR);
		cNKMLua.GetData("m_ColorG", ref nKMBuffTemplet.m_ColorG);
		cNKMLua.GetData("m_ColorB", ref nKMBuffTemplet.m_ColorB);
		cNKMLua.GetData("m_bNoReuse", ref nKMBuffTemplet.m_bNoReuse);
		cNKMLua.GetData("m_bAllowAirUnit", ref nKMBuffTemplet.m_bAllowAirUnit);
		cNKMLua.GetData("m_bAllowLandUnit", ref nKMBuffTemplet.m_bAllowLandUnit);
		cNKMLua.GetData("m_bAllowAwaken", ref nKMBuffTemplet.m_bAllowAwaken);
		cNKMLua.GetData("m_bAllowNormal", ref nKMBuffTemplet.m_bAllowNormal);
		cNKMLua.GetData("m_bRangeSonAllowAirUnit", ref nKMBuffTemplet.m_bRangeSonAllowAirUnit);
		cNKMLua.GetData("m_bRangeSonAllowLandUnit", ref nKMBuffTemplet.m_bRangeSonAllowLandUnit);
		cNKMLua.GetData("m_bRangeSonAllowAwaken", ref nKMBuffTemplet.m_bRangeSonAllowAwaken);
		cNKMLua.GetData("m_bRangeSonAllowNormal", ref nKMBuffTemplet.m_bRangeSonAllowNormal);
		cNKMLua.GetData("m_bRangeSonOnlyTarget", ref nKMBuffTemplet.m_bRangeSonOnlyTarget);
		cNKMLua.GetData("m_bRangeSonOnlySubTarget", ref nKMBuffTemplet.m_bRangeSonOnlySubTarget);
		cNKMLua.GetData("m_bAllowBoss", ref nKMBuffTemplet.m_bAllowBoss);
		cNKMLua.GetData("m_bBuffCount", ref nKMBuffTemplet.m_bBuffCount);
		nKMBuffTemplet.m_listAllowUnitID.Clear();
		if (cNKMLua.OpenTable("m_listAllowUnitID"))
		{
			int i = 1;
			for (int rValue = 0; cNKMLua.GetData(i, ref rValue); i++)
			{
				nKMBuffTemplet.m_listAllowUnitID.Add(rValue);
			}
			cNKMLua.CloseTable();
		}
		nKMBuffTemplet.m_listIgnoreUnitID.Clear();
		if (cNKMLua.OpenTable("m_listIgnoreUnitID"))
		{
			int j = 1;
			for (int rValue2 = 0; cNKMLua.GetData(j, ref rValue2); j++)
			{
				nKMBuffTemplet.m_listIgnoreUnitID.Add(rValue2);
			}
			cNKMLua.CloseTable();
		}
		cNKMLua.GetDataListEnum("m_listAllowStyleType", nKMBuffTemplet.m_listAllowStyleType);
		cNKMLua.GetDataListEnum("m_listIgnoreStyleType", nKMBuffTemplet.m_listIgnoreStyleType);
		cNKMLua.GetDataListEnum("m_listAllowRoleType", nKMBuffTemplet.m_listAllowRoleType);
		cNKMLua.GetDataListEnum("m_listIgnoreRoleType", nKMBuffTemplet.m_listIgnoreRoleType);
		cNKMLua.GetDataListEnum("m_listAllowTagType", nKMBuffTemplet.m_listAllowTagType);
		cNKMLua.GetDataListEnum("m_listIgnoreTagType", nKMBuffTemplet.m_listIgnoreTagType);
		cNKMLua.GetData("m_bRangeSonAllowBoss", ref nKMBuffTemplet.m_bRangeSonAllowBoss);
		nKMBuffTemplet.m_listRangeSonAllowUnitID.Clear();
		if (cNKMLua.OpenTable("m_listRangeSonAllowUnitID"))
		{
			int k = 1;
			for (int rValue3 = 0; cNKMLua.GetData(k, ref rValue3); k++)
			{
				nKMBuffTemplet.m_listRangeSonAllowUnitID.Add(rValue3);
			}
			cNKMLua.CloseTable();
		}
		nKMBuffTemplet.m_listRangeSonIgnoreUnitID.Clear();
		if (cNKMLua.OpenTable("m_listRangeSonIgnoreUnitID"))
		{
			int l = 1;
			for (int rValue4 = 0; cNKMLua.GetData(l, ref rValue4); l++)
			{
				nKMBuffTemplet.m_listRangeSonIgnoreUnitID.Add(rValue4);
			}
			cNKMLua.CloseTable();
		}
		cNKMLua.GetDataListEnum("m_listRangeSonAllowStyleType", nKMBuffTemplet.m_listRangeSonAllowStyleType);
		cNKMLua.GetDataListEnum("m_listRangeSonIgnoreStyleType", nKMBuffTemplet.m_listRangeSonIgnoreStyleType);
		cNKMLua.GetDataListEnum("m_listRangeSonAllowRoleType", nKMBuffTemplet.m_listRangeSonAllowRoleType);
		cNKMLua.GetDataListEnum("m_listRangeSonIgnoreRoleType", nKMBuffTemplet.m_listRangeSonIgnoreRoleType);
		cNKMLua.GetDataListEnum("m_listRangeSonAllowTagType", nKMBuffTemplet.m_listRangeSonAllowTagType);
		cNKMLua.GetDataListEnum("m_listRangeSonIgnoreTagType", nKMBuffTemplet.m_listRangeSonIgnoreTagType);
		cNKMLua.GetData("m_AffectMe", ref nKMBuffTemplet.m_AffectMe);
		cNKMLua.GetData("m_AffectMasterTeam", ref nKMBuffTemplet.m_AffectMasterTeam);
		cNKMLua.GetData("m_AffectMasterEnemyTeam", ref nKMBuffTemplet.m_AffectMasterEnemyTeam);
		cNKMLua.GetData("m_AffectMultiRespawnMinCount", ref nKMBuffTemplet.m_AffectMultiRespawnMinCount);
		cNKMLua.GetData("m_AffectSonMultiRespawnMinCount", ref nKMBuffTemplet.m_AffectSonMultiRespawnMinCount);
		nKMBuffTemplet.m_AffectCostRange.LoadFromLua(cNKMLua, "m_AffectCostRange");
		nKMBuffTemplet.m_RangeSonAffectCostRange.LoadFromLua(cNKMLua, "m_RangeSonAffectCostRange");
		cNKMLua.GetDataEnum<AffectSummonType>("m_eAffectSummonType", out nKMBuffTemplet.m_eAffectSummonType);
		cNKMLua.GetDataEnum<AffectSummonType>("m_eAffectRangeSonSummonType", out nKMBuffTemplet.m_eAffectRangeSonSummonType);
		nKMBuffTemplet.m_ApplyStatus.Clear();
		cNKMLua.GetDataListEnum("m_ApplyStatus", nKMBuffTemplet.m_ApplyStatus);
		nKMBuffTemplet.m_ImmuneStatus.Clear();
		cNKMLua.GetDataListEnum("m_ImmuneStatus", nKMBuffTemplet.m_ImmuneStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_DETECTER, "m_bDetect", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_CLOCKING, "m_bNoTargeted", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP, "m_bSleep", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP, "m_bImmuneSleep", ref nKMBuffTemplet.m_ImmuneStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE, "m_bSilence", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE, "m_bImmuneSilence", ref nKMBuffTemplet.m_ImmuneStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE, "m_bInvincible", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_FORCE_MISS, "m_bForceMiss", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_FORCE_MISS, "m_bImmuneForceMiss", ref nKMBuffTemplet.m_ImmuneStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_FORCE_EVADE, "m_bForceEvade", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_FORCE_HIT, "m_bForceHit", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE, "m_bConfuse", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE, "m_bImmuneConfuse", ref nKMBuffTemplet.m_ImmuneStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_IMMORTAL, "m_bImmortal", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_BUFF_DAMAGE_IMMUNE, "m_bImmuneBuffDamage", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_NULLIFY_BARRIER, "m_bCrashBarrier", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_NULLIFY_BARRIER, "m_bImmuneBarrier", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_MOVE_SLOW, "m_bImmuneMoveSlow", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_ATTACK_SLOW, "m_bImmuneAttackSlow", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_AIRBORNE, "m_bImmuneAirborne", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_SKILL_COOLTIME_DAMAGE, "m_bImmuneSkillCoolTime", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_STUN, "m_bStun", ref nKMBuffTemplet.m_ApplyStatus);
		LoadStatusFromBool(cNKMLua, NKM_UNIT_STATUS_EFFECT.NUSE_STUN, "m_bImmuneStun", ref nKMBuffTemplet.m_ImmuneStatus);
		cNKMLua.GetData("m_AddAttackUnitCount", ref nKMBuffTemplet.m_AddAttackUnitCount);
		cNKMLua.GetData("m_fAddAttackRange", ref nKMBuffTemplet.m_fAddAttackRange);
		cNKMLua.GetData("m_SuperArmorLevel", ref nKMBuffTemplet.m_SuperArmorLevel);
		cNKMLua.GetData("m_bNotDispel", ref nKMBuffTemplet.m_bNotDispel);
		cNKMLua.GetData("m_bRangeSonDispelBuff", ref nKMBuffTemplet.m_bRangeSonDispelBuff);
		cNKMLua.GetData("m_bRangeSonDispelDebuff", ref nKMBuffTemplet.m_bRangeSonDispelDebuff);
		cNKMLua.GetData("m_bDispelBuff", ref nKMBuffTemplet.m_bDispelBuff);
		cNKMLua.GetData("m_bDispelDebuff", ref nKMBuffTemplet.m_bDispelDebuff);
		cNKMLua.GetData("m_bNotCastSummon", ref nKMBuffTemplet.m_bNotCastSummon);
		cNKMLua.GetData("m_bIgnoreBlock", ref nKMBuffTemplet.m_bIgnoreBlock);
		cNKMLua.GetData("m_fDamageTransfer", ref nKMBuffTemplet.m_fDamageTransfer);
		cNKMLua.GetData("m_fDamageReflection", ref nKMBuffTemplet.m_fDamageReflection);
		cNKMLua.GetData("m_fHealFeedback", ref nKMBuffTemplet.m_fHealFeedback);
		cNKMLua.GetData("m_fHealFeedbackPerLevel", ref nKMBuffTemplet.m_fHealFeedbackPerLevel);
		cNKMLua.GetData("m_fHealTransfer", ref nKMBuffTemplet.m_fHealTransfer);
		cNKMLua.GetData("m_bGuard", ref nKMBuffTemplet.m_bGuard);
		cNKMLua.GetData("m_bBarrierHPRate", ref nKMBuffTemplet.m_bBarrierHPRate);
		cNKMLua.GetData("m_fBarrierHP", ref nKMBuffTemplet.m_fBarrierHP);
		cNKMLua.GetData("m_fBarrierHPPerLevel", ref nKMBuffTemplet.m_fBarrierHPPerLevel);
		cNKMLua.GetData("m_BarrierDamageEffectName", ref nKMBuffTemplet.m_BarrierDamageEffectName);
		cNKMLua.GetData("m_DamageTempletStrID", ref nKMBuffTemplet.m_DamageTempletStrID);
		if (nKMBuffTemplet.m_DamageTempletStrID.Length > 1)
		{
			nKMBuffTemplet.m_NKMDamageTemplet = NKMDamageManager.GetTempletByStrID(nKMBuffTemplet.m_DamageTempletStrID);
		}
		else
		{
			nKMBuffTemplet.m_NKMDamageTemplet = null;
		}
		cNKMLua.GetData("m_StartDTStrID", ref nKMBuffTemplet.m_StartDTStrID);
		if (nKMBuffTemplet.m_StartDTStrID.Length > 1)
		{
			nKMBuffTemplet.m_DTStart = NKMDamageManager.GetTempletByStrID(nKMBuffTemplet.m_StartDTStrID);
		}
		else
		{
			nKMBuffTemplet.m_DTStart = null;
		}
		cNKMLua.GetData("m_EndDTStrID", ref nKMBuffTemplet.m_EndDTStrID);
		if (nKMBuffTemplet.m_EndDTStrID.Length > 1)
		{
			nKMBuffTemplet.m_DTEnd = NKMDamageManager.GetTempletByStrID(nKMBuffTemplet.m_EndDTStrID);
		}
		else
		{
			nKMBuffTemplet.m_DTEnd = null;
		}
		cNKMLua.GetData("m_DispelDTStrID", ref nKMBuffTemplet.m_DispelDTStrID);
		if (nKMBuffTemplet.m_DispelDTStrID.Length > 1)
		{
			nKMBuffTemplet.m_DTDispel = NKMDamageManager.GetTempletByStrID(nKMBuffTemplet.m_DispelDTStrID);
		}
		else
		{
			nKMBuffTemplet.m_DTDispel = null;
		}
		cNKMLua.GetData("m_fOneTimeHPDamageRate", ref nKMBuffTemplet.m_fOneTimeHPDamageRate);
		cNKMLua.GetData("m_EventHealStrID", ref nKMBuffTemplet.m_EventHealStrID);
		if (nKMBuffTemplet.m_EventHealStrID.Length > 1)
		{
			nKMBuffTemplet.m_NKMEventHeal = NKMCommonUnitEvent.GetNKMEventHeal(nKMBuffTemplet.m_EventHealStrID);
		}
		else
		{
			nKMBuffTemplet.m_NKMEventHeal = null;
		}
		cNKMLua.GetData("m_bUnitDieEvent", ref nKMBuffTemplet.m_bUnitDieEvent);
		cNKMLua.GetData("m_UnitLevel", ref nKMBuffTemplet.m_UnitLevel);
		cNKMLua.GetData("m_FinalUnitStateChange", ref nKMBuffTemplet.m_FinalUnitStateChange);
		cNKMLua.GetData("m_FinalBuffStrID", ref nKMBuffTemplet.m_FinalBuffStrID);
		LoadStat(cNKMLua, "m_StatType1", "m_StatValue1", "m_StatFactor1", ref nKMBuffTemplet.m_StatType1, ref nKMBuffTemplet.m_StatValue1, nKMBuffTemplet.m_BuffStrID);
		cNKMLua.GetData("m_StatAddPerLevel1", ref nKMBuffTemplet.m_StatAddPerLevel1);
		LoadStat(cNKMLua, "m_StatType2", "m_StatValue2", "m_StatFactor2", ref nKMBuffTemplet.m_StatType2, ref nKMBuffTemplet.m_StatValue2, nKMBuffTemplet.m_BuffStrID);
		cNKMLua.GetData("m_StatAddPerLevel2", ref nKMBuffTemplet.m_StatAddPerLevel2);
		LoadStat(cNKMLua, "m_StatType3", "m_StatValue3", "m_StatFactor3", ref nKMBuffTemplet.m_StatType3, ref nKMBuffTemplet.m_StatValue3, nKMBuffTemplet.m_BuffStrID);
		cNKMLua.GetData("m_StatAddPerLevel3", ref nKMBuffTemplet.m_StatAddPerLevel3);
		return nKMBuffTemplet;
	}

	public static void LoadStat(NKMLua cNKMLua, string statTypeName, string valueName, string factorName, ref NKM_STAT_TYPE statType, ref int statValue, string debugName)
	{
		NKM_STAT_TYPE result = NKM_STAT_TYPE.NST_END;
		int rValue = 0;
		int rValue2 = 0;
		cNKMLua.GetData(statTypeName, ref result);
		cNKMLua.GetData(valueName, ref rValue);
		cNKMLua.GetData(factorName, ref rValue2);
		if (rValue2 != 0)
		{
			NKM_STAT_TYPE factorStat = NKMUnitStatManager.GetFactorStat(result);
			if (factorStat == NKM_STAT_TYPE.NST_END)
			{
				Log.ErrorAndExit($"[BuffManager] {debugName} : Non-Factor stat {result} have factor value!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBuffManager.cs", 607);
				return;
			}
			statType = factorStat;
			statValue = rValue2;
		}
		else
		{
			statType = result;
			statValue = rValue;
		}
	}

	private static void LoadStatusFromBool(NKMLua cNKMLua, NKM_UNIT_STATUS_EFFECT status, string oldName, ref HashSet<NKM_UNIT_STATUS_EFFECT> targetHashSet)
	{
		bool rbValue = false;
		cNKMLua.GetData(oldName, ref rbValue);
		if (rbValue)
		{
			targetHashSet.Add(status);
		}
	}

	public bool IsFixedPosBuff()
	{
		if (m_bShipSkillPos)
		{
			return true;
		}
		if (m_fOffsetX != 0f)
		{
			return true;
		}
		return false;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (!string.IsNullOrEmpty(m_FinalBuffStrID) && NKMBuffManager.GetBuffTempletByStrID(m_FinalBuffStrID) == null)
		{
			Log.ErrorAndExit("[NKMBuffTemplet] m_FinalBuffStrID is invalid. m_BuffStrID [" + m_BuffStrID + "], m_FinalBuffStrID [" + m_FinalBuffStrID + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBuffManager.cs", 652);
		}
		if (m_RangeOverlap)
		{
			if (m_MaxOverlapCount > 1)
			{
				Log.ErrorAndExit("[NKMBuffTemplet] m_MaxOverlapCount > 1 is not allowed when m_RangeOverlap is true.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBuffManager.cs", 660);
			}
			if (m_RangeSonCount < 1)
			{
				Log.ErrorAndExit("[NKMBuffTemplet] m_RangeSonCount must 1 or higher for m_RangeOverlap buff", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBuffManager.cs", 665);
			}
		}
		if (!string.IsNullOrEmpty(m_MaxOverlapBuffStrID) && m_MaxOverlapCount <= 1)
		{
			Log.ErrorAndExit("[NKMBuffTemplet] if buff has m_MaxOverlapBuffStrID, m_MaxOverlapCount must 2 or higher!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBuffManager.cs", 673);
		}
		if (m_bSystem && (!m_bInfinity || !m_bNotDispel || !m_bIgnoreBlock))
		{
			Log.ErrorAndExit("[NKMBuffTemplet] m_bSystem = true buff must have m_bInfinity, m_bNotDispel, m_bIgnoreBlock", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBuffManager.cs", 681);
		}
	}

	public float GetBarrierHPMax(float fHPMax, float fReinforceBarrier, int buffLevel)
	{
		if (m_fBarrierHP < 0f)
		{
			return -1f;
		}
		if (!m_bBarrierHPRate)
		{
			return (m_fBarrierHP + m_fBarrierHPPerLevel * (float)(buffLevel - 1)) * (1f + fReinforceBarrier);
		}
		float num = fHPMax * (m_fBarrierHP + m_fBarrierHPPerLevel * (float)(buffLevel - 1));
		return num + num * fReinforceBarrier;
	}

	public float GetLifeTimeMax(int timeLevel)
	{
		float num = m_fLifeTime;
		if (m_fLifeTime > 0f && timeLevel > 0)
		{
			num += m_fLifeTimePerLevel * (float)(timeLevel - 1);
		}
		return num;
	}

	public bool HasStatus(NKM_UNIT_STATUS_EFFECT status)
	{
		return m_ApplyStatus.Contains(status);
	}

	public bool HasImmuneStatus(NKM_UNIT_STATUS_EFFECT status)
	{
		return m_ImmuneStatus.Contains(status);
	}

	public bool HasCrowdControlStatus()
	{
		foreach (NKM_UNIT_STATUS_EFFECT item in m_ApplyStatus)
		{
			if (NKMUnitStatusTemplet.IsCrowdControlStatus(item))
			{
				return true;
			}
		}
		return false;
	}

	public string GetMasterEffectName(int skinID)
	{
		if (skinID == 0 || m_dicMasterEffect == null)
		{
			return m_MasterEffectName;
		}
		if (m_dicMasterEffect.TryGetValue(skinID, out var value))
		{
			return value;
		}
		return m_MasterEffectName;
	}

	public string GetSlaveEffectName(int skinID)
	{
		if (skinID == 0 || m_dicSlaveEffect == null)
		{
			return m_SlaveEffectName;
		}
		if (m_dicSlaveEffect.TryGetValue(skinID, out var value))
		{
			return value;
		}
		return m_SlaveEffectName;
	}

	public void ParseSkinDic()
	{
		if (!string.IsNullOrEmpty(m_MasterEffectNameSkinDic))
		{
			m_dicMasterEffect = NKCUtil.ParseIntKeyTable(m_MasterEffectNameSkinDic);
		}
		else
		{
			m_dicMasterEffect = null;
		}
		if (!string.IsNullOrEmpty(m_SlaveEffectNameSkinDic))
		{
			m_dicSlaveEffect = NKCUtil.ParseIntKeyTable(m_SlaveEffectNameSkinDic);
		}
		else
		{
			m_dicSlaveEffect = null;
		}
	}

	public static void ParseAllSkinDic()
	{
		foreach (NKMBuffTemplet value in NKMTempletContainer<NKMBuffTemplet>.Values)
		{
			value.ParseSkinDic();
		}
	}
}
