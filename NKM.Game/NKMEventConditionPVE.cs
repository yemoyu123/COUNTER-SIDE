using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionPVE : NKMEventConditionDetail
{
	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return cNKMGame.IsPVE();
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionPVE();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
