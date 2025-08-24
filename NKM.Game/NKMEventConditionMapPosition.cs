using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionMapPosition : NKMEventConditionDetail
{
	public NKMMinMaxFloat m_MapPositon = new NKMMinMaxFloat(-1f, -1f);

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return m_MapPositon.LoadFromLua(cNKMLua, "m_MapPositon");
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		float mapFactor = cNKMGame.GetMapTemplet().GetMapFactor(cNKMUnit.GetUnitSyncData().m_PosX, cNKMGame.IsATeam(cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE));
		return m_MapPositon.IsBetween(mapFactor, NegativeIsOpen: true);
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionMapPosition nKMEventConditionMapPosition = new NKMEventConditionMapPosition();
		nKMEventConditionMapPosition.m_MapPositon.DeepCopyFromSource(m_MapPositon);
		return nKMEventConditionMapPosition;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return NKMEventConditionV2.ValidateNKMMinMax(m_MapPositon, "[EventConditionMapPosition] m_MapPositon\ufffd\ufffd \ufffdǹ\u033e\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd");
	}
}
