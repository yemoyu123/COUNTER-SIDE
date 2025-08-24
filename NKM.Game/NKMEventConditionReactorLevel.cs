using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionReactorLevel : NKMEventConditionDetail
{
	public NKMMinMaxInt m_ReactorLevel = new NKMMinMaxInt(-1, -1);

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return m_ReactorLevel.LoadFromLua(cNKMLua, "m_ReactorLevel");
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		int reactorLevel;
		if (cNKMUnit.HasMasterUnit())
		{
			NKMUnit masterUnit = cNKMUnit.GetMasterUnit();
			if (masterUnit == null)
			{
				return false;
			}
			reactorLevel = masterUnit.GetUnitData().reactorLevel;
		}
		else
		{
			reactorLevel = cNKMUnit.GetUnitData().reactorLevel;
		}
		return m_ReactorLevel.IsBetween(reactorLevel, negativeIsTrue: true);
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionReactorLevel nKMEventConditionReactorLevel = new NKMEventConditionReactorLevel();
		nKMEventConditionReactorLevel.m_ReactorLevel.DeepCopyFromSource(m_ReactorLevel);
		return nKMEventConditionReactorLevel;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return NKMEventConditionV2.ValidateNKMMinMax(m_ReactorLevel, "[NKMEventConditionReactorLevel] m_ReactorLevel\ufffd\ufffd \ufffdǹ\u033e\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd");
	}
}
