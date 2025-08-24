using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionLand : NKMEventConditionDetail
{
	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return !cNKMUnit.IsAirUnit();
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionLand();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
