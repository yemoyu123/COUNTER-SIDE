using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionPVP : NKMEventConditionDetail
{
	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return cNKMGame.IsPVP();
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionPVP();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
