using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionAir : NKMEventConditionDetail
{
	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return cNKMUnit.IsAirUnit();
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionAir();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
