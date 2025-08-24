using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Game;

public class NKMEventConditionUnitCount : NKMEventConditionDetail
{
	public NKMMinMaxFloat m_Range = new NKMMinMaxFloat();

	public NKMEventConditionV2 m_TargetCondition;

	public NKMMinMaxInt m_Count = new NKMMinMaxInt(-1, -1);

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Range.LoadFromLua(cNKMLua, "m_Range");
		m_TargetCondition = NKMEventConditionV2.LoadFromLUA(cNKMLua, "m_TargetConditionV2");
		m_Count.LoadFromLua(cNKMLua, "m_Count");
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		int count = 0;
		cNKMGame.UnitBatch(cNKMUnit, m_Range.m_Min, m_Range.m_Max, Batch);
		return m_Count.IsBetween(count, negativeIsTrue: true);
		bool Batch(NKMUnit targetUnit, NKMUnit finderUnit, float distance)
		{
			if (targetUnit.CheckEventCondition(m_TargetCondition, finderUnit))
			{
				count++;
			}
			return false;
		}
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionUnitCount nKMEventConditionUnitCount = new NKMEventConditionUnitCount();
		nKMEventConditionUnitCount.m_Range.DeepCopyFromSource(m_Range);
		nKMEventConditionUnitCount.m_TargetCondition = NKMEventConditionV2.Clone(m_TargetCondition);
		nKMEventConditionUnitCount.m_Count.DeepCopyFromSource(m_Count);
		return nKMEventConditionUnitCount;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		if (m_Range.m_Min == m_Range.m_Max)
		{
			NKMTempletError.Add("[NKMEventConditionUnitCount] m_Range\ufffd\ufffd min/max\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 1804);
			return false;
		}
		return true;
	}
}
