namespace NKM;

public class NKMEventConsume : NKMUnitStateEventOneTime
{
	public enum eConsumeType
	{
		MAX_HP_RATE,
		CURRENT_HP_RATE,
		FLAT_HP,
		SKILL_COOLTIME,
		SKILL_COOLTIME_RATE,
		HYPER_COOLTIME,
		HYPER_COOLTIME_RATE
	}

	public eConsumeType m_eConsumeType;

	public float m_fValue;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventConsume source)
	{
		DeepCopy(source);
		m_eConsumeType = source.m_eConsumeType;
		m_fValue = source.m_fValue;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_eConsumeType", ref m_eConsumeType);
		cNKMLua.GetData("m_fValue", ref m_fValue);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		if (cNKMUnit.Get_NKM_UNIT_CLASS_TYPE() != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			return;
		}
		switch (m_eConsumeType)
		{
		case eConsumeType.MAX_HP_RATE:
		{
			float incomingDamage2 = cNKMUnit.GetMaxHP() * m_fValue;
			float expectedHPAfterDamage2 = cNKMUnit.GetExpectedHPAfterDamage(incomingDamage2);
			cNKMUnit.SetHP(expectedHPAfterDamage2);
			break;
		}
		case eConsumeType.CURRENT_HP_RATE:
		{
			float incomingDamage = cNKMUnit.GetHP() * m_fValue;
			float expectedHPAfterDamage = cNKMUnit.GetExpectedHPAfterDamage(incomingDamage);
			cNKMUnit.SetHP(expectedHPAfterDamage);
			break;
		}
		case eConsumeType.FLAT_HP:
		{
			float expectedHPAfterDamage3 = cNKMUnit.GetExpectedHPAfterDamage(m_fValue);
			cNKMUnit.SetHP(expectedHPAfterDamage3);
			break;
		}
		case eConsumeType.SKILL_COOLTIME:
			if (cNKMUnit.GetUnitTemplet().m_listSkillStateData.Count > 0)
			{
				NKMAttackStateData nKMAttackStateData2 = cNKMUnit.GetUnitTemplet().m_listSkillStateData[0];
				NKMUnitState unitState = cNKMUnit.GetUnitState(nKMAttackStateData2.m_StateName);
				if (unitState != null)
				{
					cNKMUnit.SetStateCoolTimeAdd(unitState, m_fValue);
				}
			}
			break;
		case eConsumeType.SKILL_COOLTIME_RATE:
			if (cNKMUnit.GetUnitTemplet().m_listSkillStateData.Count > 0)
			{
				NKMAttackStateData nKMAttackStateData4 = cNKMUnit.GetUnitTemplet().m_listSkillStateData[0];
				ConsumeStateCooltimeRate(cNKMUnit, nKMAttackStateData4.m_StateName);
			}
			break;
		case eConsumeType.HYPER_COOLTIME:
			if (cNKMUnit.GetUnitTemplet().m_listHyperSkillStateData.Count > 0)
			{
				NKMAttackStateData nKMAttackStateData3 = cNKMUnit.GetUnitTemplet().m_listHyperSkillStateData[0];
				NKMUnitState unitState2 = cNKMUnit.GetUnitState(nKMAttackStateData3.m_StateName);
				if (unitState2 != null)
				{
					cNKMUnit.SetStateCoolTimeAdd(unitState2, m_fValue);
				}
			}
			break;
		case eConsumeType.HYPER_COOLTIME_RATE:
			if (cNKMUnit.GetUnitTemplet().m_listHyperSkillStateData.Count > 0)
			{
				NKMAttackStateData nKMAttackStateData = cNKMUnit.GetUnitTemplet().m_listHyperSkillStateData[0];
				ConsumeStateCooltimeRate(cNKMUnit, nKMAttackStateData.m_StateName);
			}
			break;
		}
		cNKMUnit.SetPushSync();
	}

	private void ConsumeStateCooltimeRate(NKMUnit cNKMUnit, string stateName)
	{
		NKMUnitState unitState = cNKMUnit.GetUnitState(stateName);
		if (unitState != null)
		{
			float fAddCool = cNKMUnit.GetStateMaxCoolTime(unitState.m_StateName) * m_fValue;
			cNKMUnit.SetStateCoolTimeAdd(unitState, fAddCool);
		}
	}
}
