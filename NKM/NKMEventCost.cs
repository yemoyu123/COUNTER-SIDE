using Cs.Logging;
using NKM.Unit;

namespace NKM;

public class NKMEventCost : NKMUnitStateEventOneTime, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public float m_AdjustCostAlly;

	public float m_AdjustCostEnemy;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventCost source)
	{
		DeepCopy(source);
		m_AdjustCostAlly = source.m_AdjustCostAlly;
		m_AdjustCostEnemy = source.m_AdjustCostEnemy;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_AddCost", ref m_AdjustCostAlly);
		if (cNKMLua.GetData("m_RemoveCost", ref m_AdjustCostEnemy))
		{
			m_AdjustCostEnemy *= -1f;
		}
		cNKMLua.GetData("m_AdjustCostAlly", ref m_AdjustCostAlly);
		cNKMLua.GetData("m_AdjustCostEnemy", ref m_AdjustCostEnemy);
		float rValue = 0f;
		if (cNKMLua.GetData("m_CostPerSkillLevel", ref rValue))
		{
			Log.ErrorAndExit("m_CostPerSkillLevel is Deprecated!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 2361);
		}
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.SetCost(m_AdjustCostAlly, m_AdjustCostEnemy);
	}

	public override void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		cNKMUnit.SetCost(m_AdjustCostAlly, m_AdjustCostEnemy);
	}
}
