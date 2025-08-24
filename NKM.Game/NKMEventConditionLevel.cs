using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionLevel : NKMEventConditionDetail
{
	public NKMMinMaxInt m_LevelRange = new NKMMinMaxInt(-1, -1);

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return m_LevelRange.LoadFromLua(cNKMLua, "m_LevelRange");
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return m_LevelRange.IsBetween(cNKMUnit.GetUnitData().m_UnitLevel, negativeIsTrue: true);
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionLevel nKMEventConditionLevel = new NKMEventConditionLevel();
		nKMEventConditionLevel.m_LevelRange.DeepCopyFromSource(m_LevelRange);
		return nKMEventConditionLevel;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return NKMEventConditionV2.ValidateNKMMinMax(m_LevelRange, "[NKMEventConditionLevel] m_LevelRange\ufffd\ufffd \ufffdǹ\u033e\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd");
	}
}
