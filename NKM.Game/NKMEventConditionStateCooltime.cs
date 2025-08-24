using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionStateCooltime : NKMEventConditionDetail
{
	public string m_TargetState = string.Empty;

	public NKMMinMaxFloat m_Range = new NKMMinMaxFloat();

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return (byte)(1u & (cNKMLua.GetData("m_TargetState", ref m_TargetState) ? 1u : 0u) & (m_Range.LoadFromLua(cNKMLua, "m_Range") ? 1u : 0u)) != 0;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		if (cNKMUnit.GetUnitState(m_TargetState) == null)
		{
			return false;
		}
		float value = 1f - cNKMUnit.GetStateCoolTime(m_TargetState) / cNKMUnit.GetStateMaxCoolTime(m_TargetState);
		return m_Range.IsBetween(value, NegativeIsOpen: true);
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionStateCooltime nKMEventConditionStateCooltime = new NKMEventConditionStateCooltime();
		nKMEventConditionStateCooltime.m_TargetState = m_TargetState;
		nKMEventConditionStateCooltime.m_Range.DeepCopyFromSource(m_Range);
		return nKMEventConditionStateCooltime;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
