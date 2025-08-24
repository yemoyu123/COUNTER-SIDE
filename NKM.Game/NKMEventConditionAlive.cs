using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionAlive : NKMEventConditionDetail
{
	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		if (!cNKMUnit.IsDyingOrDie())
		{
			return cNKMUnit.GetHP() > 0f;
		}
		return false;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionAlive();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
