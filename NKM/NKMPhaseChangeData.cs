using System.Collections.Generic;
using NKM.Unit;

namespace NKM;

public class NKMPhaseChangeData : IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public int m_TargetPhase;

	public float m_fChangeConditionHPRate;

	public float m_fChangeConditionHPValue;

	public bool m_bCutHpDamage;

	public float m_fChangeConditionTime;

	public int m_ChangeConditionMyKill;

	public bool m_bChangeConditionImmortalStart;

	public string m_ChangeStateName = "";

	public List<NKMPhaseChangeCoolTime> m_listChangeCoolTime = new List<NKMPhaseChangeCoolTime>();

	public NKMEventCondition Condition => m_Condition;

	public float GetTargetHP(float maxHP)
	{
		if (m_fChangeConditionHPRate > 0f)
		{
			return maxHP * m_fChangeConditionHPRate;
		}
		if (m_fChangeConditionHPValue > 0f)
		{
			return m_fChangeConditionHPValue;
		}
		return 0f;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_TargetPhase", ref m_TargetPhase);
		cNKMLua.GetData("m_fChangeConditionHPRate", ref m_fChangeConditionHPRate);
		cNKMLua.GetData("m_fChangeConditionHPValue", ref m_fChangeConditionHPValue);
		cNKMLua.GetData("m_bCutHpDamage", ref m_bCutHpDamage);
		cNKMLua.GetData("m_fChangeConditionTime", ref m_fChangeConditionTime);
		cNKMLua.GetData("m_ChangeConditionMyKill", ref m_ChangeConditionMyKill);
		cNKMLua.GetData("m_bChangeConditionImmortalStart", ref m_bChangeConditionImmortalStart);
		cNKMLua.GetData("m_ChangeStateName", ref m_ChangeStateName);
		if (cNKMLua.OpenTable("m_listChangeCoolTime"))
		{
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				NKMPhaseChangeCoolTime nKMPhaseChangeCoolTime = null;
				if (m_listChangeCoolTime.Count < num)
				{
					nKMPhaseChangeCoolTime = new NKMPhaseChangeCoolTime();
					m_listChangeCoolTime.Add(nKMPhaseChangeCoolTime);
				}
				else
				{
					nKMPhaseChangeCoolTime = m_listChangeCoolTime[num - 1];
				}
				nKMPhaseChangeCoolTime.LoadFromLUA(cNKMLua);
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		return true;
	}

	public void DeepCopyFromSource(NKMPhaseChangeData source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_TargetPhase = source.m_TargetPhase;
		m_fChangeConditionHPRate = source.m_fChangeConditionHPRate;
		m_fChangeConditionHPValue = source.m_fChangeConditionHPValue;
		m_bCutHpDamage = source.m_bCutHpDamage;
		m_fChangeConditionTime = source.m_fChangeConditionTime;
		m_ChangeConditionMyKill = source.m_ChangeConditionMyKill;
		m_bChangeConditionImmortalStart = source.m_bChangeConditionImmortalStart;
		m_ChangeStateName = source.m_ChangeStateName;
		m_listChangeCoolTime.Clear();
		for (int i = 0; i < source.m_listChangeCoolTime.Count; i++)
		{
			NKMPhaseChangeCoolTime nKMPhaseChangeCoolTime = new NKMPhaseChangeCoolTime();
			nKMPhaseChangeCoolTime.DeepCopyFromSource(source.m_listChangeCoolTime[i]);
			m_listChangeCoolTime.Add(nKMPhaseChangeCoolTime);
		}
	}
}
