using NKM.Templet.Base;

namespace NKM;

public class NKMEventTrigger : NKMUnitStateAreaEventOneTime
{
	public bool m_bUseTargetUnitTrigger;

	public string m_TargetTrigger = "";

	public override EventRollbackType RollbackType => EventRollbackType.Prohibited;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventTrigger source)
	{
		DeepCopy(source);
		m_TargetTrigger = source.m_TargetTrigger;
		m_bUseTargetUnitTrigger = source.m_bUseTargetUnitTrigger;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_TargetTrigger", ref m_TargetTrigger);
		cNKMLua.GetData("m_bUseTargetUnitTrigger", ref m_bUseTargetUnitTrigger);
		return true;
	}

	public override void OnAreaEventToTarget(NKMGame cNKMGame, NKMUnit eventOwner, NKMUnit target)
	{
		if (m_bUseTargetUnitTrigger)
		{
			int triggerID = target.GetUnitTemplet().GetTriggerID(m_TargetTrigger);
			target.InvokeTrigger(target, triggerID);
		}
		else
		{
			int triggerID2 = eventOwner.GetUnitTemplet().GetTriggerID(m_TargetTrigger);
			target.InvokeTrigger(eventOwner, triggerID2);
		}
	}

	public bool Validate(NKMUnitTemplet templet)
	{
		if (!m_bUseTargetUnitTrigger && templet.GetTriggerSet(m_TargetTrigger) == null)
		{
			NKMTempletError.Add("[NKMEventTrigger] Trigger " + m_TargetTrigger + " not found from unit " + templet.m_UnitTempletBase.DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 4634);
			return false;
		}
		return true;
	}
}
