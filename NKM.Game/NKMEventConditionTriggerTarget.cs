using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionTriggerTarget : NKMEventConditionDetail
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
		return cNKMUnit.GetUnitGameUID() == cUnitConditionOwner.CurrentTriggerTarget.GetUnitGameUID();
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionTriggerTarget();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
