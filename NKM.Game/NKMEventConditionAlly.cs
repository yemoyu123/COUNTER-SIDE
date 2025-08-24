using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionAlly : NKMEventConditionDetail
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
		return cNKMUnit.IsAlly(cUnitConditionOwner);
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionAlly();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
