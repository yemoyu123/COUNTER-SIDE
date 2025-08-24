using System.Collections.Generic;
using NKM.Templet;
using NKM.Unit;

namespace NKM;

public class NKMEventStun : NKMUnitStateEventOneTime, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public float m_fStunTime;

	public float m_fRange;

	public bool m_bUseUnitSize;

	public int m_MaxCount;

	public float m_fStunTimePerSkillLevel;

	public int m_StunCountPerSkillLevel;

	public HashSet<NKM_UNIT_STYLE_TYPE> m_IgnoreStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventStun source)
	{
		DeepCopy(source);
		m_fStunTime = source.m_fStunTime;
		m_fRange = source.m_fRange;
		m_bUseUnitSize = source.m_bUseUnitSize;
		m_MaxCount = source.m_MaxCount;
		m_fStunTimePerSkillLevel = source.m_fStunTimePerSkillLevel;
		m_StunCountPerSkillLevel = source.m_StunCountPerSkillLevel;
		m_IgnoreStyleType.Clear();
		foreach (NKM_UNIT_STYLE_TYPE item in source.m_IgnoreStyleType)
		{
			m_IgnoreStyleType.Add(item);
		}
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_fStunTime", ref m_fStunTime);
		cNKMLua.GetData("m_fRange", ref m_fRange);
		cNKMLua.GetData("m_bUseUnitSize", ref m_bUseUnitSize);
		cNKMLua.GetData("m_MaxCount", ref m_MaxCount);
		cNKMLua.GetData("m_fStunTimePerSkillLevel", ref m_fStunTimePerSkillLevel);
		cNKMLua.GetData("m_StunCountPerSkillLevel", ref m_StunCountPerSkillLevel);
		m_IgnoreStyleType.Clear();
		if (cNKMLua.OpenTable("m_IgnoreStyleType"))
		{
			bool flag = true;
			int num = 1;
			NKM_UNIT_STYLE_TYPE result = NKM_UNIT_STYLE_TYPE.NUST_INVALID;
			while (flag)
			{
				flag = cNKMLua.GetData(num, ref result);
				if (flag)
				{
					m_IgnoreStyleType.Add(result);
				}
				num++;
			}
			cNKMLua.CloseTable();
		}
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.SetStun(cNKMUnit, m_fStunTime, m_fRange, m_bUseUnitSize, m_MaxCount, m_fStunTimePerSkillLevel, m_StunCountPerSkillLevel, m_IgnoreStyleType);
	}

	public override void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		cNKMUnit.SetStun(cNKMUnit, m_fStunTime, m_fRange, m_bUseUnitSize, m_MaxCount, m_fStunTimePerSkillLevel, m_StunCountPerSkillLevel, m_IgnoreStyleType);
	}
}
