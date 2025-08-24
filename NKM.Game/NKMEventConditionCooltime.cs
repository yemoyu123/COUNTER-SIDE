using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionCooltime : NKMEventConditionDetail
{
	public bool m_bHyper;

	public NKMMinMaxFloat m_Range = new NKMMinMaxFloat();

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return (byte)(1u & (cNKMLua.GetData("m_bHyper", ref m_bHyper) ? 1u : 0u) & (m_Range.LoadFromLua(cNKMLua, "m_Range") ? 1u : 0u)) != 0;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		string stateName;
		if (m_bHyper)
		{
			NKMAttackStateData fastestCoolTimeHyperSkillData = cNKMUnit.GetFastestCoolTimeHyperSkillData();
			if (fastestCoolTimeHyperSkillData == null)
			{
				return false;
			}
			stateName = fastestCoolTimeHyperSkillData.m_StateName;
		}
		else
		{
			NKMAttackStateData fastestCoolTimeSkillData = cNKMUnit.GetFastestCoolTimeSkillData();
			if (fastestCoolTimeSkillData == null)
			{
				return false;
			}
			stateName = fastestCoolTimeSkillData.m_StateName;
		}
		float value = 1f - cNKMUnit.GetStateCoolTime(stateName) / cNKMUnit.GetStateMaxCoolTime(stateName);
		return m_Range.IsBetween(value, NegativeIsOpen: true);
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionCooltime nKMEventConditionCooltime = new NKMEventConditionCooltime();
		nKMEventConditionCooltime.m_bHyper = m_bHyper;
		nKMEventConditionCooltime.m_Range.DeepCopyFromSource(m_Range);
		return nKMEventConditionCooltime;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
