using System.Collections.Generic;
using Cs.Logging;
using Cs.Math;
using NKM.Game;
using NKM.Templet;
using NKM.Unit;

namespace NKM;

public class NKMEventBuff : NKMUnitEffectStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public string m_BuffStrID = "";

	public bool m_BuffRemove;

	public bool m_StateEndRemove;

	public byte m_BuffStatLevel = 1;

	public byte m_BuffStatLevelPerSkillLevel;

	public byte m_BuffTimeLevel = 1;

	public byte m_BuffTimeLevelPerSkillLevel;

	public float m_fRange;

	public bool m_bUseUnitSize;

	public bool m_bUseTriggerTargetRange;

	public NKMEventConditionV2 m_ConditionTarget;

	public int m_MaxCount;

	public bool m_bMyTeam;

	public bool m_bEnemy;

	public bool m_bReflection;

	public int m_Overlap = 1;

	public float m_fMinTargetHP;

	public float m_fMaxTargetHP;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public bool Validate()
	{
		if (!string.IsNullOrEmpty(m_BuffStrID) && NKMBuffManager.GetBuffTempletByStrID(m_BuffStrID) == null)
		{
			return false;
		}
		return true;
	}

	public void DeepCopyFromSource(NKMEventBuff source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTime = source.m_fEventTime;
		m_bStateEndTime = source.m_bStateEndTime;
		m_BuffStrID = source.m_BuffStrID;
		m_BuffStatLevel = source.m_BuffStatLevel;
		m_BuffRemove = source.m_BuffRemove;
		m_StateEndRemove = source.m_StateEndRemove;
		m_BuffStatLevelPerSkillLevel = source.m_BuffStatLevelPerSkillLevel;
		m_BuffTimeLevel = source.m_BuffTimeLevel;
		m_BuffTimeLevelPerSkillLevel = source.m_BuffTimeLevelPerSkillLevel;
		m_fRange = source.m_fRange;
		m_bMyTeam = source.m_bMyTeam;
		m_bEnemy = source.m_bEnemy;
		m_bReflection = source.m_bReflection;
		m_Overlap = source.m_Overlap;
		m_fMinTargetHP = source.m_fMinTargetHP;
		m_fMaxTargetHP = source.m_fMaxTargetHP;
		m_bUseUnitSize = source.m_bUseUnitSize;
		m_bUseTriggerTargetRange = source.m_bUseTriggerTargetRange;
		m_MaxCount = source.m_MaxCount;
		m_ConditionTarget = NKMEventConditionV2.Clone(source.m_ConditionTarget);
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_BuffStrID", ref m_BuffStrID);
		cNKMLua.GetData("m_BuffRemove", ref m_BuffRemove);
		cNKMLua.GetData("m_StateEndRemove", ref m_StateEndRemove);
		byte rValue = 0;
		if (cNKMLua.GetData("m_BuffLevel", ref rValue))
		{
			m_BuffStatLevel = rValue;
			m_BuffTimeLevel = rValue;
		}
		byte rValue2 = 0;
		if (cNKMLua.GetData("m_BuffLevelPerSkillLevel", ref rValue2))
		{
			m_BuffStatLevel = rValue2;
			m_BuffTimeLevel = rValue2;
		}
		cNKMLua.GetData("m_BuffStatLevel", ref m_BuffStatLevel);
		cNKMLua.GetData("m_BuffStatLevelPerSkillLevel", ref m_BuffStatLevelPerSkillLevel);
		cNKMLua.GetData("m_BuffTimeLevel", ref m_BuffTimeLevel);
		cNKMLua.GetData("m_BuffTimeLevelPerSkillLevel", ref m_BuffTimeLevelPerSkillLevel);
		cNKMLua.GetData("m_fRange", ref m_fRange);
		cNKMLua.GetData("m_bMyTeam", ref m_bMyTeam);
		cNKMLua.GetData("m_bEnemy", ref m_bEnemy);
		cNKMLua.GetData("m_bReflection", ref m_bReflection);
		cNKMLua.GetData("m_bUseUnitSize", ref m_bUseUnitSize);
		cNKMLua.GetData("m_bUseTriggerTargetRange", ref m_bUseTriggerTargetRange);
		cNKMLua.GetData("m_MaxCount", ref m_MaxCount);
		int rValue3 = 0;
		if (cNKMLua.GetData("m_AddOverlap", ref rValue3))
		{
			if (rValue3 >= 0)
			{
				m_Overlap = rValue3 + 1;
			}
			else
			{
				m_Overlap = rValue3;
			}
		}
		cNKMLua.GetData("m_Overlap", ref m_Overlap);
		if (m_Overlap >= 255)
		{
			Log.ErrorAndExit($"[NKMEventBuff] Overlap is to big [{m_Overlap}/{byte.MaxValue}] BuffID[{m_BuffStrID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 1420);
			return false;
		}
		m_ConditionTarget = NKMEventConditionV2.LoadFromLUA(cNKMLua, "m_ConditionTarget");
		cNKMLua.GetData("m_fMinTargetHP", ref m_fMinTargetHP);
		cNKMLua.GetData("m_fMaxTargetHP", ref m_fMaxTargetHP);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		int num = m_BuffStatLevel;
		int num2 = m_BuffTimeLevel;
		NKMUnitTemplet unitTemplet = cNKMUnit.GetUnitData().GetUnitTemplet();
		if (unitTemplet != null && unitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP && cNKMUnit.GetUnitStateNow() != null)
		{
			NKMUnitSkillTemplet unitSkillTempletByType = cNKMUnit.GetUnitData().GetUnitSkillTempletByType(cNKMUnit.GetUnitStateNow().m_NKM_SKILL_TYPE);
			if (unitSkillTempletByType != null && unitSkillTempletByType.m_Level > 0)
			{
				if (m_BuffStatLevelPerSkillLevel > 0)
				{
					num += (unitSkillTempletByType.m_Level - 1) * m_BuffStatLevelPerSkillLevel;
				}
				if (m_BuffTimeLevelPerSkillLevel > 0)
				{
					num2 = (unitSkillTempletByType.m_Level - 1) * m_BuffTimeLevelPerSkillLevel;
				}
			}
		}
		if (m_fRange.IsNearlyZero())
		{
			if (!(m_fMinTargetHP > cNKMUnit.GetHPRate()) && (!(m_fMaxTargetHP > 0f) || !(m_fMaxTargetHP <= cNKMUnit.GetHPRate())))
			{
				NKMUnit triggerTargetUnit = cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange);
				if (m_BuffRemove)
				{
					triggerTargetUnit.DeleteBuff(m_BuffStrID);
				}
				else
				{
					triggerTargetUnit.AddBuffByStrID(m_BuffStrID, (byte)num, (byte)num2, cNKMUnit.GetUnitSyncData().m_GameUnitUID, bUseMasterStat: true, bRangeSon: false, m_StateEndRemove, m_Overlap);
				}
			}
		}
		else
		{
			List<NKMUnit> sortUnitListByNearDist = cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange).GetSortUnitListByNearDist(m_bUseUnitSize);
			ProcessRangeBuff(cNKMGame, cNKMUnit, (byte)num, (byte)num2, cNKMUnit.GetUnitSyncData().m_PosX, sortUnitListByNearDist);
		}
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect)
	{
		if (cNKMDamageEffect == null)
		{
			return;
		}
		int num = m_BuffStatLevel;
		int num2 = m_BuffTimeLevel;
		NKMUnit masterUnit = cNKMDamageEffect.GetMasterUnit();
		if (masterUnit == null)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = masterUnit.GetUnitTempletBase();
		if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP && masterUnit.GetUnitStateNow() != null)
		{
			NKMUnitSkillTemplet unitSkillTempletByType = masterUnit.GetUnitData().GetUnitSkillTempletByType(masterUnit.GetUnitStateNow().m_NKM_SKILL_TYPE);
			if (unitSkillTempletByType != null && unitSkillTempletByType.m_Level > 0)
			{
				if (m_BuffStatLevelPerSkillLevel > 0)
				{
					num += (unitSkillTempletByType.m_Level - 1) * m_BuffStatLevelPerSkillLevel;
				}
				if (m_BuffTimeLevelPerSkillLevel > 0)
				{
					num2 = (unitSkillTempletByType.m_Level - 1) * m_BuffTimeLevelPerSkillLevel;
				}
			}
		}
		List<NKMUnit> sortUnitListByNearDist = cNKMDamageEffect.GetSortUnitListByNearDist(m_bUseUnitSize);
		ProcessRangeBuff(cNKMGame, masterUnit, (byte)num, (byte)num2, cNKMDamageEffect.GetDEData().m_PosX, sortUnitListByNearDist);
	}

	private void ProcessRangeBuff(NKMGame cNKMGame, NKMUnit cNKMUnit, byte buffLevel, byte buffTimeLevel, float posX, List<NKMUnit> cSortedUnitList)
	{
		int num = 0;
		for (int i = 0; i < cSortedUnitList.Count; i++)
		{
			NKMUnit nKMUnit = cSortedUnitList[i];
			if (nKMUnit.GetUnitSyncData().m_GameUnitUID == cNKMUnit.GetUnitSyncData().m_GameUnitUID)
			{
				continue;
			}
			if (!nKMUnit.IsInRange(posX, m_fRange, m_bUseUnitSize))
			{
				break;
			}
			if (!nKMUnit.WillInteractWithGameUnits() || nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE || (!m_bMyTeam && !cNKMGame.IsEnemy(cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE)) || (!m_bEnemy && cNKMGame.IsEnemy(cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE)) || !nKMUnit.CheckEventCondition(m_ConditionTarget, cNKMUnit) || m_fMinTargetHP > nKMUnit.GetHPRate() || (m_fMaxTargetHP > 0f && m_fMaxTargetHP <= nKMUnit.GetHPRate()))
			{
				continue;
			}
			if (m_BuffRemove)
			{
				bool bFromEnemy = cNKMGame.IsEnemy(cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE);
				nKMUnit.DeleteBuff(m_BuffStrID, bFromEnemy);
			}
			else
			{
				nKMUnit.AddBuffByStrID(m_BuffStrID, buffLevel, buffTimeLevel, cNKMUnit.GetUnitSyncData().m_GameUnitUID, bUseMasterStat: true, bRangeSon: false, m_StateEndRemove, (byte)m_Overlap);
			}
			if (m_MaxCount > 0)
			{
				num++;
				if (num >= m_MaxCount)
				{
					break;
				}
			}
		}
	}
}
