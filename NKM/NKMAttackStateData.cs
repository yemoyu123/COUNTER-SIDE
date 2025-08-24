using Cs.Math;
using NKM.Unit;

namespace NKM;

public class NKMAttackStateData : IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public NKM_ATTACK_STATE_DATA_TYPE m_NKM_ATTACK_STATE_DATA_TYPE;

	public string m_StateName = "";

	public float m_fStartCool;

	public float m_fStartCoolPVP = 1f;

	public bool m_bNoTarget;

	public bool m_bTargetLandOnly;

	public bool m_bTargetAirOnly;

	public float m_fRangeMin;

	public float m_fRangeMax;

	public float m_fUseHPRateOver;

	public float m_fUseHPRateUnder;

	public int m_PhaseOver = -1;

	public int m_PhaseLess = -1;

	public int m_Ratio = 1;

	public NKMEventCondition Condition => m_Condition;

	public bool CanUseAttack(NKM_ATTACK_STATE_DATA_TYPE eNKM_ATTACK_STATE_DATA_TYPE, float fHPRate, int phaseNow, float fRangeFactor)
	{
		if (!m_bNoTarget)
		{
			return false;
		}
		return CanUseAttack(eNKM_ATTACK_STATE_DATA_TYPE, fHPRate, bAirUnit: false, 0f, phaseNow, fRangeFactor);
	}

	public bool CanUseAttack(NKM_ATTACK_STATE_DATA_TYPE eNKM_ATTACK_STATE_DATA_TYPE, float fHPRate, bool bAirUnit, float fDistToTarget, int phaseNow, float fRangeFactor)
	{
		if (m_StateName.Length <= 1)
		{
			return false;
		}
		if (m_NKM_ATTACK_STATE_DATA_TYPE != eNKM_ATTACK_STATE_DATA_TYPE)
		{
			return false;
		}
		if (!m_bNoTarget)
		{
			float num = m_fRangeMin * fRangeFactor;
			float num2 = m_fRangeMax * fRangeFactor;
			if (m_bTargetLandOnly && bAirUnit)
			{
				return false;
			}
			if (m_bTargetAirOnly && !bAirUnit)
			{
				return false;
			}
			if (num > 0f && num > fDistToTarget)
			{
				return false;
			}
			if (num2 < fDistToTarget)
			{
				return false;
			}
		}
		if (m_fUseHPRateOver > 0f && m_fUseHPRateOver > fHPRate)
		{
			return false;
		}
		if (m_fUseHPRateUnder > 0f && m_fUseHPRateUnder < fHPRate)
		{
			return false;
		}
		if (m_PhaseOver > -1 && m_PhaseOver > phaseNow)
		{
			return false;
		}
		if (m_PhaseLess > -1 && m_PhaseLess <= phaseNow)
		{
			return false;
		}
		return true;
	}

	public bool LoadFromLUA(NKMLua cNKMLua, float fTargetNearRange)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_NKM_ATTACK_STATE_DATA_TYPE", ref m_NKM_ATTACK_STATE_DATA_TYPE);
		cNKMLua.GetData("m_StateName", ref m_StateName);
		cNKMLua.GetData("m_fStartCool", ref m_fStartCool);
		cNKMLua.GetData("m_fStartCoolPVP", ref m_fStartCoolPVP);
		cNKMLua.GetData("m_bNoTarget", ref m_bNoTarget);
		cNKMLua.GetData("m_bTargetLandOnly", ref m_bTargetLandOnly);
		cNKMLua.GetData("m_bTargetAirOnly", ref m_bTargetAirOnly);
		cNKMLua.GetData("m_fRangeMin", ref m_fRangeMin);
		cNKMLua.GetData("m_fRangeMax", ref m_fRangeMax);
		cNKMLua.GetData("m_fUseHPRateOver", ref m_fUseHPRateOver);
		cNKMLua.GetData("m_fUseHPRateUnder", ref m_fUseHPRateUnder);
		cNKMLua.GetData("m_PhaseOver", ref m_PhaseOver);
		cNKMLua.GetData("m_PhaseLess", ref m_PhaseLess);
		cNKMLua.GetData("m_Ratio", ref m_Ratio);
		if (m_fRangeMax.IsNearlyZero())
		{
			m_fRangeMax = fTargetNearRange;
		}
		return true;
	}

	public void DeepCopyFromSource(NKMAttackStateData source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_NKM_ATTACK_STATE_DATA_TYPE = source.m_NKM_ATTACK_STATE_DATA_TYPE;
		m_StateName = source.m_StateName;
		m_fStartCool = source.m_fStartCool;
		m_fStartCoolPVP = source.m_fStartCoolPVP;
		m_bNoTarget = source.m_bNoTarget;
		m_bTargetLandOnly = source.m_bTargetLandOnly;
		m_bTargetAirOnly = source.m_bTargetAirOnly;
		m_fRangeMin = source.m_fRangeMin;
		m_fRangeMax = source.m_fRangeMax;
		m_fUseHPRateOver = source.m_fUseHPRateOver;
		m_fUseHPRateUnder = source.m_fUseHPRateUnder;
		m_PhaseOver = source.m_PhaseOver;
		m_PhaseLess = source.m_PhaseLess;
		m_Ratio = source.m_Ratio;
	}
}
