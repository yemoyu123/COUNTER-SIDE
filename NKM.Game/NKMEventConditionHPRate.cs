using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionHPRate : NKMEventConditionDetail
{
	public NKMMinMaxFloat m_fHPRate = new NKMMinMaxFloat(-1f, -1f);

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return m_fHPRate.LoadFromLua(cNKMLua, "m_fHPRate");
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return m_fHPRate.IsBetween(cNKMUnit.GetHPRate(), NegativeIsOpen: true);
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionHPRate nKMEventConditionHPRate = new NKMEventConditionHPRate();
		nKMEventConditionHPRate.m_fHPRate.DeepCopyFromSource(m_fHPRate);
		return nKMEventConditionHPRate;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return NKMEventConditionV2.ValidateNKMMinMax(m_fHPRate, "[EventConditionHPRate] m_fHPRate\ufffd\ufffd \ufffdǹ\u033e\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd");
	}
}
