using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionDistance : NKMEventConditionDetail
{
	public NKMMinMaxFloat m_Range = new NKMMinMaxFloat();

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return m_Range.LoadFromLua(cNKMLua, "m_Range");
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return cUnitConditionOwner?.IsInRange(cNKMUnit, m_Range.m_Min, m_Range.m_Max, bUseUnitSize: true) ?? false;
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionDistance nKMEventConditionDistance = new NKMEventConditionDistance();
		nKMEventConditionDistance.m_Range.DeepCopyFromSource(m_Range);
		return nKMEventConditionDistance;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
