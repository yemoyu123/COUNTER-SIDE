using System.Collections.Generic;
using NKM.Game;
using NKM.Templet.Base;

namespace NKM;

public class NKMEventTriggerBranch : NKMUnitStateEventOneTime
{
	public struct BranchNode
	{
		public NKMEventConditionV2 eventCondition;

		public string triggerName;

		public bool Inverse;

		public BranchNode(BranchNode node)
		{
			eventCondition = node.eventCondition;
			triggerName = node.triggerName;
			Inverse = node.Inverse;
		}
	}

	public string defaultTrigger;

	public List<BranchNode> m_lstBranch = new List<BranchNode>();

	public bool Inverse;

	public override EventRollbackType RollbackType => EventRollbackType.Prohibited;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventTriggerBranch source)
	{
		DeepCopy(source);
		defaultTrigger = source.defaultTrigger;
		Inverse = source.Inverse;
		m_lstBranch.Clear();
		foreach (BranchNode item in source.m_lstBranch)
		{
			m_lstBranch.Add(new BranchNode(item));
		}
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_DefaultTrigger", ref defaultTrigger);
		if (cNKMLua.OpenTable("m_listBranch"))
		{
			m_lstBranch.Clear();
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				BranchNode item = default(BranchNode);
				cNKMLua.GetData("m_Trigger", ref item.triggerName);
				item.Inverse = false;
				cNKMLua.GetData("Inverse", ref item.Inverse);
				item.eventCondition = NKMEventConditionV2.LoadFromLUA(cNKMLua);
				m_lstBranch.Add(item);
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		foreach (BranchNode item in m_lstBranch)
		{
			if (cNKMUnit.CheckEventCondition(item.eventCondition) != item.Inverse)
			{
				cNKMUnit.InvokeTrigger(item.triggerName);
				return;
			}
		}
		cNKMUnit.InvokeTrigger(defaultTrigger);
	}

	public bool Validate(NKMUnitTemplet templet)
	{
		bool flag = true;
		foreach (BranchNode item in m_lstBranch)
		{
			if (templet.GetTriggerSet(item.triggerName) == null)
			{
				NKMTempletError.Add("[NKMEventTriggerBranch] Trigger " + item.triggerName + " not found from unit " + templet.m_UnitTempletBase.DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 4733);
				flag = false;
			}
			if (item.eventCondition != null)
			{
				flag &= item.eventCondition.Validate(templet, null);
			}
		}
		if (!string.IsNullOrEmpty(defaultTrigger) && templet.GetTriggerSet(defaultTrigger) == null)
		{
			NKMTempletError.Add("[NKMEventTriggerBranch] Trigger " + defaultTrigger + " not found from unit " + templet.m_UnitTempletBase.DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 4747);
			flag = false;
		}
		return flag;
	}
}
