using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionNoDamageState : NKMEventConditionDetail
{
	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return !cNKMUnit.GetUnitStateNow().IsDamageOrDieState;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionNoDamageState();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
