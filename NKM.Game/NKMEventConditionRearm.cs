using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionRearm : NKMEventConditionDetail
{
	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return cNKMUnit.GetUnitTempletBase().IsRearmUnit;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionRearm();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
