using System.Collections.Generic;
using Cs.Logging;

namespace NKM.Game;

public class NKMUnitReaction
{
	public enum ReactionEventType
	{
		HIT,
		HIT_CRITICAL,
		HIT_EVADE,
		ATTACK,
		ATTACK_CRITICAL,
		ATTACK_MISS,
		KILL,
		DEAD,
		UNIT_DEAD,
		RESPAWN,
		HP_RATE,
		UNIT_HP_RATE,
		TAKE_CC,
		UNIT_TAKE_CC,
		TAKE_HITSTUN,
		UNIT_TAKE_HITSTUN
	}

	public enum TargetType
	{
		MASTER,
		LAST_INVOKER,
		REACTION_UNIT,
		MASTER_TARGET,
		MASTER_SUB_TARGET,
		REACTION_UNIT_TARGET,
		REACTION_UNIT_SUB_TARGET
	}

	public string m_Name;

	public ReactionEventType m_EventType;

	public float m_Param;

	public int m_RequireCount;

	public NKMEventConditionV2 m_ReactionCondition;

	public string m_Trigger;

	public TargetType m_TriggerTarget;

	public bool m_bImmediate;

	public static void LoadFromLua(NKMLua cNKMLua, ref Dictionary<int, NKMUnitReaction> dicReaction, ref Dictionary<string, int> dicReactionID)
	{
		if (cNKMLua.OpenTable("m_dicReaction"))
		{
			dicReactionID = new Dictionary<string, int>();
			dicReaction = new Dictionary<int, NKMUnitReaction>();
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				NKMUnitReaction nKMUnitReaction = new NKMUnitReaction();
				if (!nKMUnitReaction.LoadReactionFromLua(cNKMLua))
				{
					Log.Error("Reaction m_Name not found!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Game/NKMUnitReaction.cs", 103);
					num++;
					continue;
				}
				dicReaction.Add(num, nKMUnitReaction);
				dicReactionID.Add(nKMUnitReaction.m_Name, num);
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		else
		{
			dicReaction = null;
			dicReactionID = null;
		}
	}

	private bool LoadReactionFromLua(NKMLua cNKMLua)
	{
		if (!cNKMLua.GetData("m_Name", ref m_Name))
		{
			return false;
		}
		cNKMLua.GetData("m_EventType", ref m_EventType);
		cNKMLua.GetData("m_RequireCount", ref m_RequireCount);
		cNKMLua.GetData("m_Param", ref m_Param);
		cNKMLua.GetData("m_bImmediate", ref m_bImmediate);
		cNKMLua.GetData("m_TriggerTarget", ref m_TriggerTarget);
		cNKMLua.GetData("m_Trigger", ref m_Trigger);
		m_ReactionCondition = NKMEventConditionV2.LoadFromLUA(cNKMLua, "m_ReactionCondition");
		return true;
	}

	public bool CheckParam(float[] param)
	{
		ReactionEventType eventType = m_EventType;
		if ((uint)(eventType - 10) > 1u)
		{
			return true;
		}
		if (param == null || param.Length < 2)
		{
			return false;
		}
		float num = param[0];
		if (param[1] <= m_Param)
		{
			return m_Param < num;
		}
		return false;
	}

	public bool UsingParam()
	{
		ReactionEventType eventType = m_EventType;
		if ((uint)(eventType - 10) <= 1u)
		{
			return true;
		}
		return false;
	}
}
