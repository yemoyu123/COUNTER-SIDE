using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionAwaken : NKMEventConditionDetail
{
	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return cNKMUnit.GetUnitTempletBase().m_bAwaken;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionAwaken();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
