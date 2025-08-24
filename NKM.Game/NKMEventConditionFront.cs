using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionFront : NKMEventConditionDetail
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
		if (cUnitConditionOwner.GetUnitSyncData().m_bRight)
		{
			return cNKMUnit.GetUnitFrameData().m_PosXCalc >= cUnitConditionOwner.GetUnitFrameData().m_PosXCalc;
		}
		return cNKMUnit.GetUnitFrameData().m_PosXCalc <= cUnitConditionOwner.GetUnitFrameData().m_PosXCalc;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionFront();
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
