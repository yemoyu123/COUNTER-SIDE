using System.Collections.Generic;
using NKM.Templet;

namespace NKM;

public class NKMUnitFrameData
{
	public NKMStatData m_StatData = new NKMStatData();

	public float m_fInitHP;

	public float m_fLiveTime;

	public float m_fStateTimeBack;

	public float m_fStateTime;

	public float m_fAnimTimeBack;

	public float m_fAnimTime;

	public float m_fAnimTimeMax;

	public int m_AnimPlayCount;

	public bool m_bAnimPlayCountAddThisFrame;

	public float m_fAnimSpeedOrg = 1f;

	public float m_fAnimSpeed = 1f;

	public float m_fAirHigh;

	public float m_fTargetAirHigh;

	public float m_fFindTargetTime;

	public float m_fFindSubTargetTime;

	public float m_fAttackedTarget;

	public bool m_bInvincible;

	public float m_fAddAttackRange;

	public NKM_SUPER_ARMOR_LEVEL m_SuperArmorLevel = NKM_SUPER_ARMOR_LEVEL.NSAL_NO;

	public NKM_SUPER_ARMOR_LEVEL m_BuffSuperArmorLevel = NKM_SUPER_ARMOR_LEVEL.NSAL_NO;

	public NKMBuffData m_BarrierBuffData;

	public HashSet<NKM_UNIT_STATUS_EFFECT> m_hsImmuneStatus = new HashSet<NKM_UNIT_STATUS_EFFECT>();

	public HashSet<NKM_UNIT_STATUS_EFFECT> m_hsStatus = new HashSet<NKM_UNIT_STATUS_EFFECT>();

	public Dictionary<NKM_UNIT_STATUS_EFFECT, float> m_dicStatusTime = new Dictionary<NKM_UNIT_STATUS_EFFECT, float>();

	public bool m_bNotCastSummon;

	public bool m_bImmortalStart;

	public short m_fDamageTransferGameUnitUID;

	public float m_fDamageTransfer;

	public float m_fDamageReflection;

	public float m_fHealFeedback;

	public short m_fHealFeedbackMasterGameUnitUID;

	public float m_fHealTransfer;

	public short m_HealTransferMasterGameUnitUID;

	public short m_GuardGameUnitUID;

	public int m_BuffUnitLevel;

	public bool m_bFootOnLand = true;

	public int m_PhaseNow;

	public float m_LastTargetPosX;

	public float m_LastTargetPosZ;

	public float m_LastTargetJumpYPos;

	public float m_PosXBefore;

	public float m_PosZBefore;

	public float m_JumpYPosBefore;

	public float m_PosXCalc;

	public float m_PosZCalc;

	public float m_JumpYPosCalc;

	public float m_fSpeedX;

	public float m_fSpeedY;

	public float m_fSpeedZ;

	public float m_fDamageSpeedX;

	public float m_fDamageSpeedZ;

	public float m_fDamageSpeedJumpY;

	public float m_fDamageSpeedKeepTimeX;

	public float m_fDamageSpeedKeepTimeZ;

	public float m_fDamageSpeedKeepTimeJumpY;

	public float m_fStopReserveTime;

	public float[] m_StopTime = new float[3];

	public float m_fHitLightTime;

	public List<byte> m_listHitFeedBackCount = new List<byte>();

	public List<byte> m_listHitCriticalFeedBackCount = new List<byte>();

	public List<byte> m_listHitEvadeFeedBackCount = new List<byte>();

	public float m_fColorEventTime;

	public NKMTrackingFloat m_ColorR = new NKMTrackingFloat();

	public NKMTrackingFloat m_ColorG = new NKMTrackingFloat();

	public NKMTrackingFloat m_ColorB = new NKMTrackingFloat();

	public bool m_bFindTargetThisFrame;

	public bool m_bFindSubTargetThisFrame;

	public bool m_bTargetChangeThisFrame;

	public float m_fDamageThisFrame;

	public float m_fDamageBeforeFrame;

	public bool m_bSyncShipSkill;

	public NKMShipSkillTemplet m_ShipSkillTemplet;

	public float m_fShipSkillPosX;

	public float m_fTargetLostDurationTime;

	public List<NKMUnitAccumStateData> m_listUnitAccumStateData = new List<NKMUnitAccumStateData>();

	public Dictionary<short, NKMBuffData> m_dicBuffData = new Dictionary<short, NKMBuffData>();

	public HashSet<short> m_hashNoReuseBuffID = new HashSet<short>();

	public float m_fDangerChargeTime = -1f;

	public float m_fDangerChargeDamage;

	public int m_DangerChargeHitCount;

	public byte m_AddAttackUnitCount;

	public int m_KillCount;

	public NKM_UNIT_SOURCE_TYPE m_UnitSourceType;

	public NKM_UNIT_SOURCE_TYPE m_UnitSourceTypeSub;

	public NKM_SUPER_ARMOR_LEVEL CurrentSuperArmorLevel
	{
		get
		{
			if (m_BuffSuperArmorLevel <= m_SuperArmorLevel)
			{
				return m_SuperArmorLevel;
			}
			return m_BuffSuperArmorLevel;
		}
	}

	public void RespawnInit()
	{
		m_fInitHP = 0f;
		m_fLiveTime = 0f;
		m_fStateTimeBack = 0f;
		m_fStateTime = 0f;
		m_fAnimTimeBack = 0f;
		m_fAnimTime = 0f;
		m_fAnimTimeMax = 0f;
		m_AnimPlayCount = 0;
		m_bAnimPlayCountAddThisFrame = false;
		m_fFindTargetTime = 0f;
		m_fFindSubTargetTime = 0f;
		m_fAttackedTarget = 0f;
		m_bInvincible = false;
		m_fAddAttackRange = 0f;
		m_SuperArmorLevel = NKM_SUPER_ARMOR_LEVEL.NSAL_NO;
		m_BuffSuperArmorLevel = NKM_SUPER_ARMOR_LEVEL.NSAL_NO;
		m_BarrierBuffData = null;
		m_bImmortalStart = false;
		m_fDamageTransferGameUnitUID = 0;
		m_fDamageTransfer = 0f;
		m_fDamageReflection = 0f;
		m_GuardGameUnitUID = 0;
		m_fHealFeedback = 0f;
		m_fHealFeedbackMasterGameUnitUID = 0;
		m_fHealTransfer = 0f;
		m_HealTransferMasterGameUnitUID = 0;
		m_BuffUnitLevel = 0;
		m_bFootOnLand = true;
		m_PhaseNow = 0;
		m_LastTargetPosX = -1f;
		m_LastTargetPosZ = -1f;
		m_LastTargetJumpYPos = -1f;
		m_PosXBefore = -1f;
		m_PosZBefore = -1f;
		m_JumpYPosBefore = -1f;
		m_PosXCalc = 0f;
		m_PosZCalc = 0f;
		m_JumpYPosCalc = 0f;
		m_fSpeedX = 0f;
		m_fSpeedY = 0f;
		m_fSpeedZ = 0f;
		m_fDamageSpeedX = 0f;
		m_fDamageSpeedZ = 0f;
		m_fDamageSpeedJumpY = 0f;
		m_fDamageSpeedKeepTimeX = 0f;
		m_fDamageSpeedKeepTimeZ = 0f;
		m_fDamageSpeedKeepTimeJumpY = 0f;
		m_fStopReserveTime = 0f;
		for (int i = 0; i < m_StopTime.Length; i++)
		{
			m_StopTime[i] = 0f;
		}
		m_fHitLightTime = 0f;
		for (int j = 0; j < m_listHitFeedBackCount.Count; j++)
		{
			m_listHitFeedBackCount[j] = 0;
		}
		for (int k = 0; k < m_listHitCriticalFeedBackCount.Count; k++)
		{
			m_listHitCriticalFeedBackCount[k] = 0;
		}
		for (int l = 0; l < m_listHitEvadeFeedBackCount.Count; l++)
		{
			m_listHitEvadeFeedBackCount[l] = 0;
		}
		m_bFindTargetThisFrame = false;
		m_bTargetChangeThisFrame = false;
		m_fDamageThisFrame = 0f;
		m_fDamageBeforeFrame = 0f;
		m_fTargetLostDurationTime = 0f;
		for (int m = 0; m < m_listUnitAccumStateData.Count; m++)
		{
			m_listUnitAccumStateData[m].m_dicAccumStateChange.Clear();
		}
		m_fDangerChargeTime = -1f;
		m_fDangerChargeDamage = 0f;
		m_DangerChargeHitCount = 0;
		m_AddAttackUnitCount = 0;
		m_KillCount = 0;
		m_hashNoReuseBuffID.Clear();
		m_hsStatus.Clear();
		m_hsImmuneStatus.Clear();
		m_dicStatusTime.Clear();
	}

	public void AddNoReuseBuff(short buffID)
	{
		if (!m_hashNoReuseBuffID.Contains(buffID))
		{
			m_hashNoReuseBuffID.Add(buffID);
		}
	}

	public bool IsNoReuseBuff(short buffID)
	{
		if (m_hashNoReuseBuffID.Contains(buffID))
		{
			return true;
		}
		return false;
	}
}
