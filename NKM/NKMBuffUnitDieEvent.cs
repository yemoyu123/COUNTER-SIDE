using System.Collections.Generic;
using Cs.Logging;
using NKM.Unit;

namespace NKM;

public class NKMBuffUnitDieEvent : IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public string m_BuffStrID = "";

	public float m_fSkillCoolTime;

	public float m_fHyperSkillCoolTime;

	public float m_fSkillCoolTimeAdd;

	public float m_fHyperSkillCoolTimeAdd;

	public float m_fHPRate;

	public string m_OutBuffStrID = "";

	public byte m_OutBuffStatLevel = 1;

	public byte m_OutBuffTimeLevel = 1;

	public int m_Overlap = 1;

	public List<NKMEventRespawn> m_listNKMEventRespawn = new List<NKMEventRespawn>();

	public List<NKMEventDamageEffect> m_listNKMEventDamageEffect = new List<NKMEventDamageEffect>();

	public List<NKMEventCost> m_listNKMEventCost = new List<NKMEventCost>();

	public NKMEventCondition Condition => m_Condition;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_BuffStrID", ref m_BuffStrID);
		cNKMLua.GetData("m_fSkillCoolTime", ref m_fSkillCoolTime);
		cNKMLua.GetData("m_fHyperSkillCoolTime", ref m_fHyperSkillCoolTime);
		cNKMLua.GetData("m_fSkillCoolTimeAdd", ref m_fSkillCoolTimeAdd);
		cNKMLua.GetData("m_fHyperSkillCoolTimeAdd", ref m_fHyperSkillCoolTimeAdd);
		cNKMLua.GetData("m_fHPRate", ref m_fHPRate);
		cNKMLua.GetData("m_OutBuffStrID", ref m_OutBuffStrID);
		byte rValue = 0;
		if (cNKMLua.GetData("m_OutBuffLevel", ref rValue))
		{
			m_OutBuffStatLevel = rValue;
			m_OutBuffTimeLevel = rValue;
		}
		cNKMLua.GetData("m_OutBuffStatLevel", ref m_OutBuffStatLevel);
		cNKMLua.GetData("m_OutBuffTimeLevel", ref m_OutBuffTimeLevel);
		int rValue2 = 0;
		if (cNKMLua.GetData("m_AddOverlap", ref rValue2))
		{
			if (rValue2 >= 0)
			{
				m_Overlap = rValue2 + 1;
			}
			else
			{
				m_Overlap = rValue2;
			}
		}
		cNKMLua.GetData("m_Overlap", ref m_Overlap);
		if (m_Overlap >= 255)
		{
			Log.ErrorAndExit($"[NKMBuffUnitDieEvent] Overlap is to big [{m_Overlap}/{byte.MaxValue}] BuffID[{m_BuffStrID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 4171);
			return false;
		}
		NKMUnitState.LoadAndMergeEventList(cNKMLua, "m_listNKMEventRespawn", ref m_listNKMEventRespawn);
		NKMUnitState.LoadAndMergeEventList(cNKMLua, "m_listNKMEventDamageEffect", ref m_listNKMEventDamageEffect);
		NKMUnitState.LoadAndMergeEventList(cNKMLua, "m_listNKMEventCost", ref m_listNKMEventCost);
		return true;
	}

	public void DeepCopyFromSource(NKMBuffUnitDieEvent source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_BuffStrID = source.m_BuffStrID;
		m_fSkillCoolTime = source.m_fSkillCoolTime;
		m_fHyperSkillCoolTime = source.m_fHyperSkillCoolTime;
		m_fSkillCoolTimeAdd = source.m_fSkillCoolTimeAdd;
		m_fHyperSkillCoolTimeAdd = source.m_fHyperSkillCoolTimeAdd;
		m_fHPRate = source.m_fHPRate;
		m_OutBuffStrID = source.m_OutBuffStrID;
		m_OutBuffStatLevel = source.m_OutBuffStatLevel;
		m_OutBuffTimeLevel = source.m_OutBuffTimeLevel;
		m_Overlap = source.m_Overlap;
		NKMUnitState.DeepCopy(source.m_listNKMEventRespawn, ref m_listNKMEventRespawn, delegate(NKMEventRespawn t, NKMEventRespawn s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventDamageEffect, ref m_listNKMEventDamageEffect, delegate(NKMEventDamageEffect t, NKMEventDamageEffect s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventCost, ref m_listNKMEventCost, delegate(NKMEventCost t, NKMEventCost s)
		{
			t.DeepCopyFromSource(s);
		});
	}
}
