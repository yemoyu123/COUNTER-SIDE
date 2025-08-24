using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionLeader : NKMEventConditionDetail
{
	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		NKMGameTeamData teamData = cNKMUnit.GetTeamData();
		if (teamData == null)
		{
			return false;
		}
		if (teamData.GetLeaderUnitData() == null)
		{
			return false;
		}
		if (cNKMUnit.IsSummonUnit() || cNKMUnit.HasMasterUnit())
		{
			NKMUnit masterUnit = cNKMUnit.GetMasterUnit();
			if (masterUnit == null)
			{
				return false;
			}
			if (teamData.GetLeaderUnitData().m_UnitUID != masterUnit.GetUnitData().m_UnitUID)
			{
				return false;
			}
		}
		else if (teamData.GetLeaderUnitData().m_UnitUID != cNKMUnit.GetUnitData().m_UnitUID)
		{
			return false;
		}
		return true;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionLeader();
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return true;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
