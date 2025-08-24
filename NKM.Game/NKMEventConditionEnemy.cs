using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionEnemy : NKMEventConditionDetail
{
	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		if (cUnitConditionOwner == null)
		{
			return false;
		}
		return cNKMUnit.IsEnemy(cUnitConditionOwner);
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionEnemy();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
