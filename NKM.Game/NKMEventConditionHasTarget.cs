using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionHasTarget : NKMEventConditionDetail
{
	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return cNKMUnit.GetTargetUnit() != null;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionHasTarget();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
