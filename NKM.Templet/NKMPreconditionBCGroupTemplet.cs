using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMPreconditionBCGroupTemplet : INKMTemplet
{
	public enum PRE_COND
	{
		NONE,
		COMBATPOINT,
		UNITLEVEL110_COUNT,
		UNITLEVEL_COUNT,
		UNIT_ID
	}

	public int m_PreConditionGroup;

	public PRE_COND m_PreConditionType;

	public int m_PreconditionValue = -1;

	public List<int> m_lstPreCondition;

	public int m_BCondID;

	private NKMBattleConditionTemplet m_BCTemplet;

	public int PreConditionValue => m_PreconditionValue;

	public List<int> PreConditionList => m_lstPreCondition;

	public NKMBattleConditionTemplet BattleConditionTemplet => m_BCTemplet;

	public int Key => m_PreConditionGroup;

	public static NKMPreconditionBCGroupTemplet LoadFromLua(NKMLua lua)
	{
		NKMPreconditionBCGroupTemplet nKMPreconditionBCGroupTemplet = new NKMPreconditionBCGroupTemplet();
		lua.GetData("m_PreConditionGroup", ref nKMPreconditionBCGroupTemplet.m_PreConditionGroup);
		lua.GetData("m_PreConditionType", ref nKMPreconditionBCGroupTemplet.m_PreConditionType);
		lua.GetData("m_PreConditionValue", ref nKMPreconditionBCGroupTemplet.m_PreconditionValue);
		if (!lua.GetDataList("m_PreConditionList", out nKMPreconditionBCGroupTemplet.m_lstPreCondition, nullIfEmpty: false))
		{
			nKMPreconditionBCGroupTemplet.m_lstPreCondition = null;
		}
		lua.GetData("m_BCondID", ref nKMPreconditionBCGroupTemplet.m_BCondID);
		return nKMPreconditionBCGroupTemplet;
	}

	public void Join()
	{
		m_BCTemplet = NKMBattleConditionManager.GetTempletByID(m_BCondID);
	}

	public void Validate()
	{
		if (BattleConditionTemplet == null)
		{
			NKMTempletError.Add($"[NKMPreconditionBCGroupTemplet:{m_PreConditionGroup}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd BC\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPreconditionBCGroupTemplet.cs", 57);
		}
		switch (m_PreConditionType)
		{
		case PRE_COND.UNIT_ID:
			if (m_lstPreCondition == null || m_lstPreCondition.Count == 0)
			{
				NKMTempletError.Add($"[NKMPreconditionBCGroupTemplet:{m_PreConditionGroup}] UNIT_ID \ufffd\ufffd\ufffdǿ\ufffd m_PreConditionList\ufffd\ufffd \ufffdԷµ\ufffd\ufffd\ufffd \ufffdʾ\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPreconditionBCGroupTemplet.cs", 66);
			}
			else if (m_lstPreCondition.Count < m_PreconditionValue)
			{
				NKMTempletError.Add($"[NKMPreconditionBCGroupTemplet:{m_PreConditionGroup}] UNIT_ID \ufffd\ufffd\ufffd ũ\ufffd⺸\ufffd\ufffd m_PreConditionValue\ufffd\ufffd ŭ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPreconditionBCGroupTemplet.cs", 70);
			}
			break;
		case PRE_COND.UNITLEVEL_COUNT:
			if (m_lstPreCondition == null || m_lstPreCondition.Count != 1)
			{
				NKMTempletError.Add($"[NKMPreconditionBCGroupTemplet:{m_PreConditionGroup}] UNITLEVEL_COUNT \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd m_PreConditionList\ufffd\ufffd \ufffd\ufffdǥ \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdԷµ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPreconditionBCGroupTemplet.cs", 78);
			}
			break;
		}
		if (m_PreconditionValue < 0)
		{
			NKMTempletError.Add($"[NKMPreconditionBCGroupTemplet:{m_PreConditionGroup}] m_PreconditionValue\ufffd\ufffd \ufffdԷµ\ufffd\ufffd\ufffd \ufffdʾ\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPreconditionBCGroupTemplet.cs", 85);
		}
	}
}
