using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionCost : NKMEventConditionDetail
{
	public NKMMinMaxInt m_Range = new NKMMinMaxInt(-1, -1);

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Range.LoadFromLua(cNKMLua, "m_Range");
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		int respawnCost = cNKMGame.GetRespawnCost(cNKMUnit.GetUnitTempletBase().StatTemplet, bLeader: false, cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE);
		return m_Range.IsBetween(respawnCost, negativeIsTrue: true);
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionCost nKMEventConditionCost = new NKMEventConditionCost();
		nKMEventConditionCost.m_Range.DeepCopyFromSource(m_Range);
		return nKMEventConditionCost;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return NKMEventConditionV2.ValidateNKMMinMax(m_Range, "[NKMEventConditionCost] m_Range\ufffd\ufffd \ufffdǹ\u033e\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd");
	}
}
