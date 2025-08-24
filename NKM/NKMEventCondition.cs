using System;
using System.Collections.Generic;
using System.Linq;
using NKM.Game;

namespace NKM;

public class NKMEventCondition
{
	public enum UnitCountCond
	{
		INVALID,
		ALL,
		ANY,
		NOT_ANY,
		NOT_ALL
	}

	public NKMEventConditionV2 m_EventConditionV2;

	public NKMMinMaxInt m_Phase = new NKMMinMaxInt(-1, -1);

	public bool m_bShipSkill;

	public string m_SkillStrID = "";

	public int m_SkillID = -1;

	public NKMMinMaxInt m_SkillLevel = new NKMMinMaxInt(-1, -1);

	public bool m_bMasterShipSkill;

	public string m_MasterSkillStrID = "";

	public int m_MasterSkillID = -1;

	public NKMMinMaxInt m_MasterSkillLevel = new NKMMinMaxInt(-1, -1);

	public bool m_bLeaderUnit;

	public string m_NeedBuffStrID = "";

	public short m_NeedBuffID;

	public NKMMinMaxInt m_NeedBuffLevel = new NKMMinMaxInt(-1, -1);

	public NKMMinMaxInt m_NeedBuffOverlapCount = new NKMMinMaxInt(-1, -1);

	public string m_IgnoreBuffStrID = "";

	public short m_IgnoreBuffID;

	public NKMMinMaxInt m_IgnoreBuffLevel = new NKMMinMaxInt(-1, -1);

	public NKMMinMaxInt m_IgnoreBuffOverlapCount = new NKMMinMaxInt(-1, -1);

	public NKMMinMaxFloat m_MapPositon = new NKMMinMaxFloat(-1f, -1f);

	public NKMMinMaxFloat m_fHPRate = new NKMMinMaxFloat(-1f, -1f);

	public NKMMinMaxInt m_LevelRange = new NKMMinMaxInt(-1, -1);

	public NKMMinMaxInt m_BuffCount = new NKMMinMaxInt(-1, -1);

	public NKMMinMaxInt m_DebuffCount = new NKMMinMaxInt(-1, -1);

	public bool m_bUsePVE = true;

	public bool m_bUsePVP = true;

	public NKM_UNIT_STATUS_EFFECT m_IgnoreStatusEffect;

	public NKM_UNIT_STATUS_EFFECT m_NeedStatusEffect;

	public List<int> m_listReqUnit;

	public UnitCountCond m_ReqUnitCond;

	public bool m_bReqUnitEnemy;

	public NKMMinMaxInt m_ReactorLevel = new NKMMinMaxInt(-1, -1);

	public string m_EventVariableName = string.Empty;

	public NKMMinMaxInt m_EventVariableValue = new NKMMinMaxInt(-1, -1);

	public void DeepCopyFromSource(NKMEventCondition source)
	{
		if (source.m_EventConditionV2 != null)
		{
			m_EventConditionV2 = new NKMEventConditionV2();
			m_EventConditionV2.DeepCopyFromSource(source.m_EventConditionV2);
		}
		else
		{
			m_EventConditionV2 = null;
		}
		m_Phase.DeepCopyFromSource(source.m_Phase);
		m_bLeaderUnit = source.m_bLeaderUnit;
		m_SkillStrID = source.m_SkillStrID;
		m_SkillLevel.DeepCopyFromSource(source.m_SkillLevel);
		m_ReactorLevel.DeepCopyFromSource(source.m_ReactorLevel);
		m_MasterSkillStrID = source.m_MasterSkillStrID;
		m_MasterSkillLevel.DeepCopyFromSource(source.m_MasterSkillLevel);
		m_NeedBuffStrID = source.m_NeedBuffStrID;
		m_NeedBuffID = source.m_NeedBuffID;
		m_NeedBuffLevel.DeepCopyFromSource(source.m_NeedBuffLevel);
		m_NeedBuffOverlapCount.DeepCopyFromSource(source.m_NeedBuffOverlapCount);
		m_IgnoreBuffStrID = source.m_IgnoreBuffStrID;
		m_IgnoreBuffID = source.m_IgnoreBuffID;
		m_IgnoreBuffLevel.DeepCopyFromSource(source.m_IgnoreBuffLevel);
		m_IgnoreBuffOverlapCount.DeepCopyFromSource(source.m_IgnoreBuffOverlapCount);
		m_MapPositon.DeepCopyFromSource(source.m_MapPositon);
		m_fHPRate.DeepCopyFromSource(source.m_fHPRate);
		m_LevelRange.DeepCopyFromSource(source.m_LevelRange);
		m_BuffCount.DeepCopyFromSource(source.m_BuffCount);
		m_DebuffCount.DeepCopyFromSource(source.m_DebuffCount);
		m_bUsePVE = source.m_bUsePVE;
		m_bUsePVP = source.m_bUsePVP;
		m_IgnoreStatusEffect = source.m_IgnoreStatusEffect;
		m_NeedStatusEffect = source.m_NeedStatusEffect;
		m_ReqUnitCond = source.m_ReqUnitCond;
		m_bReqUnitEnemy = source.m_bReqUnitEnemy;
		if (source.m_listReqUnit != null)
		{
			m_listReqUnit = new List<int>();
			m_listReqUnit.AddRange(source.m_listReqUnit);
		}
		else
		{
			m_listReqUnit = null;
		}
		m_EventVariableName = source.m_EventVariableName;
		m_EventVariableValue.DeepCopyFromSource(source.m_EventVariableValue);
	}

	public bool LoadFromLUA(NKMLua cNKMLua, string tableName = "m_Condition")
	{
		if (tableName == "m_Condition")
		{
			m_EventConditionV2 = NKMEventConditionV2.LoadFromLUA(cNKMLua);
			if (m_EventConditionV2 != null)
			{
				return true;
			}
		}
		else
		{
			m_EventConditionV2 = NKMEventConditionV2.LoadFromLUA(cNKMLua, tableName + "V2");
			if (m_EventConditionV2 != null)
			{
				return true;
			}
		}
		if (!cNKMLua.OpenTable(tableName))
		{
			return false;
		}
		m_Phase.LoadFromLua(cNKMLua, "m_Phase");
		cNKMLua.GetData("m_bLeaderUnit", ref m_bLeaderUnit);
		cNKMLua.GetData("m_SkillStrID", ref m_SkillStrID);
		m_SkillLevel.LoadFromLua(cNKMLua, "m_SkillLevel");
		cNKMLua.GetData("m_MasterSkillStrID", ref m_MasterSkillStrID);
		m_MasterSkillLevel.LoadFromLua(cNKMLua, "m_MasterSkillLevel");
		cNKMLua.GetData("m_NeedBuffStrID", ref m_NeedBuffStrID);
		m_NeedBuffLevel.LoadFromLua(cNKMLua, "m_NeedBuffLevel");
		m_NeedBuffOverlapCount.LoadFromLua(cNKMLua, "m_NeedBuffOverlapCount");
		cNKMLua.GetData("m_IgnoreBuffStrID", ref m_IgnoreBuffStrID);
		m_IgnoreBuffLevel.LoadFromLua(cNKMLua, "m_IgnoreBuffLevel");
		m_IgnoreBuffOverlapCount.LoadFromLua(cNKMLua, "m_IgnoreBuffOverlapCount");
		m_MapPositon.LoadFromLua(cNKMLua, "m_MapPositon");
		m_fHPRate.LoadFromLua(cNKMLua, "m_fHPRate");
		m_LevelRange.LoadFromLua(cNKMLua, "m_LevelRange");
		m_BuffCount.LoadFromLua(cNKMLua, "m_BuffCount");
		m_DebuffCount.LoadFromLua(cNKMLua, "m_DebuffCount");
		cNKMLua.GetData("m_bUsePVE", ref m_bUsePVE);
		cNKMLua.GetData("m_bUsePVP", ref m_bUsePVP);
		cNKMLua.GetData("m_IgnoreStatusEffect", ref m_IgnoreStatusEffect);
		cNKMLua.GetData("m_NeedStatusEffect", ref m_NeedStatusEffect);
		cNKMLua.GetData("m_ReqUnitCond", ref m_ReqUnitCond);
		cNKMLua.GetData("m_bReqUnitEnemy", ref m_bReqUnitEnemy);
		m_ReactorLevel.LoadFromLua(cNKMLua, "m_ReactorLevel");
		if (!cNKMLua.GetDataList("m_listReqUnit", out m_listReqUnit, nullIfEmpty: false))
		{
			m_listReqUnit = null;
		}
		cNKMLua.GetData("m_EventVariableName", ref m_EventVariableName);
		m_EventVariableValue.LoadFromLua(cNKMLua, "m_EventVariableValue");
		cNKMLua.CloseTable();
		return true;
	}

	public void CheckSkillID()
	{
		if (m_SkillID == -1 && m_SkillStrID.Length > 1)
		{
			m_SkillID = NKMUnitSkillManager.GetSkillID(m_SkillStrID);
			if (m_SkillID == -1)
			{
				m_SkillID = NKMShipSkillManager.GetSkillID(m_SkillStrID);
				m_bShipSkill = true;
			}
			else
			{
				m_bShipSkill = false;
			}
		}
		if (m_MasterSkillID == -1 && m_MasterSkillStrID.Length > 1)
		{
			m_MasterSkillID = NKMUnitSkillManager.GetSkillID(m_MasterSkillStrID);
			if (m_MasterSkillID == -1)
			{
				m_MasterSkillID = NKMShipSkillManager.GetSkillID(m_MasterSkillStrID);
				m_bMasterShipSkill = true;
			}
			else
			{
				m_bMasterShipSkill = false;
			}
		}
	}

	public bool CanUseSkill(int skillLevel)
	{
		if (m_SkillID != -1 && skillLevel != -1)
		{
			if (m_bShipSkill && skillLevel == 0)
			{
				return false;
			}
			if (m_SkillLevel.m_Min != -1 && m_SkillLevel.m_Min > skillLevel)
			{
				return false;
			}
			if (m_SkillLevel.m_Max != -1 && m_SkillLevel.m_Max < skillLevel)
			{
				return false;
			}
		}
		return true;
	}

	public bool CanUseMasterSkill(int masterSkillLevel)
	{
		if (m_MasterSkillID != -1 && masterSkillLevel != -1)
		{
			if (m_bMasterShipSkill && masterSkillLevel == 0)
			{
				return false;
			}
			if (m_MasterSkillLevel.m_Min != -1 && m_MasterSkillLevel.m_Min > masterSkillLevel)
			{
				return false;
			}
			if (m_MasterSkillLevel.m_Max != -1 && m_MasterSkillLevel.m_Max < masterSkillLevel)
			{
				return false;
			}
		}
		return true;
	}

	public bool CanUsePhase(int phaseNow)
	{
		if (m_Phase.m_Min != -1 && m_Phase.m_Min > phaseNow)
		{
			return false;
		}
		if (m_Phase.m_Max != -1 && m_Phase.m_Max < phaseNow)
		{
			return false;
		}
		return true;
	}

	public bool CheckHPRate(NKMUnit masterUnit)
	{
		if (m_fHPRate.m_Min < 0f || m_fHPRate.m_Max < 0f)
		{
			return true;
		}
		if (masterUnit == null)
		{
			return false;
		}
		if (masterUnit.GetHPRate() < m_fHPRate.m_Min)
		{
			return false;
		}
		if (masterUnit.GetHPRate() > m_fHPRate.m_Max)
		{
			return false;
		}
		return true;
	}

	public bool CanUseBuff(Dictionary<short, NKMBuffSyncData> dicBuffData)
	{
		if (m_IgnoreBuffStrID.Length > 1 && m_IgnoreBuffID <= 0)
		{
			NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(m_IgnoreBuffStrID);
			if (buffTempletByStrID != null)
			{
				m_IgnoreBuffID = buffTempletByStrID.m_BuffID;
			}
		}
		if (m_NeedBuffStrID.Length > 1 && m_NeedBuffID <= 0)
		{
			NKMBuffTemplet buffTempletByStrID2 = NKMBuffManager.GetBuffTempletByStrID(m_NeedBuffStrID);
			if (buffTempletByStrID2 != null)
			{
				m_NeedBuffID = buffTempletByStrID2.m_BuffID;
			}
		}
		if (m_IgnoreBuffID > 0)
		{
			foreach (KeyValuePair<short, NKMBuffSyncData> dicBuffDatum in dicBuffData)
			{
				NKMBuffSyncData value = dicBuffDatum.Value;
				if (Math.Abs(value.m_BuffID) == m_IgnoreBuffID)
				{
					bool flag = true;
					if (m_IgnoreBuffLevel.m_Min != -1 && m_IgnoreBuffLevel.m_Min > value.m_BuffStatLevel)
					{
						return true;
					}
					if (m_IgnoreBuffLevel.m_Max != -1 && m_IgnoreBuffLevel.m_Max < value.m_BuffStatLevel)
					{
						return true;
					}
					flag = false;
					if (m_IgnoreBuffOverlapCount.m_Min != -1 && m_IgnoreBuffOverlapCount.m_Min > value.m_OverlapCount)
					{
						return true;
					}
					if (m_IgnoreBuffOverlapCount.m_Max != -1 && m_IgnoreBuffOverlapCount.m_Max < value.m_OverlapCount)
					{
						return true;
					}
					return false;
				}
			}
		}
		if (m_NeedBuffID > 0)
		{
			foreach (KeyValuePair<short, NKMBuffSyncData> dicBuffDatum2 in dicBuffData)
			{
				NKMBuffSyncData value2 = dicBuffDatum2.Value;
				if (Math.Abs(value2.m_BuffID) == m_NeedBuffID)
				{
					bool flag2 = false;
					if (m_NeedBuffLevel.m_Min != -1 && m_NeedBuffLevel.m_Min > value2.m_BuffStatLevel)
					{
						return false;
					}
					if (m_NeedBuffLevel.m_Max != -1 && m_NeedBuffLevel.m_Max < value2.m_BuffStatLevel)
					{
						return false;
					}
					flag2 = true;
					if (m_NeedBuffOverlapCount.m_Min != -1 && m_NeedBuffOverlapCount.m_Min > value2.m_OverlapCount)
					{
						return false;
					}
					if (m_NeedBuffOverlapCount.m_Max != -1 && m_NeedBuffOverlapCount.m_Max < value2.m_OverlapCount)
					{
						return false;
					}
					return true;
				}
			}
			return false;
		}
		return true;
	}

	public bool CanUseBuffCount(NKMUnit unit)
	{
		if (m_BuffCount.m_Min < 0 && m_BuffCount.m_Max < 0 && m_DebuffCount.m_Min < 0 && m_DebuffCount.m_Max < 0)
		{
			return true;
		}
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in unit.GetUnitFrameData().m_dicBuffData)
		{
			NKMBuffData value = dicBuffDatum.Value;
			NKMBuffTemplet nKMBuffTemplet = value.m_NKMBuffTemplet;
			if (nKMBuffTemplet.m_bBuffCount && value.m_BuffSyncData.m_bAffect)
			{
				if (nKMBuffTemplet.m_bDebuff)
				{
					num2++;
				}
				else
				{
					num++;
				}
			}
		}
		if (!m_BuffCount.IsBetween(num, negativeIsTrue: true))
		{
			return false;
		}
		if (!m_DebuffCount.IsBetween(num2, negativeIsTrue: true))
		{
			return false;
		}
		return true;
	}

	public bool CanUseStatus(NKMUnit unit)
	{
		if (m_NeedStatusEffect != NKM_UNIT_STATUS_EFFECT.NUSE_NONE && !unit.HasStatus(m_NeedStatusEffect))
		{
			return false;
		}
		if (m_IgnoreStatusEffect != NKM_UNIT_STATUS_EFFECT.NUSE_NONE && unit.HasStatus(m_IgnoreStatusEffect))
		{
			return false;
		}
		return true;
	}

	public bool CanUseMapPosition(float currentPosition)
	{
		if (m_MapPositon.m_Min != -1f && currentPosition < m_MapPositon.m_Min)
		{
			return false;
		}
		if (m_MapPositon.m_Max != -1f && currentPosition > m_MapPositon.m_Max)
		{
			return false;
		}
		return true;
	}

	public bool CanUseLevelRange(NKMUnit unit)
	{
		if (m_LevelRange.m_Min < 0 && m_LevelRange.m_Max < 0)
		{
			return true;
		}
		if (unit == null)
		{
			return false;
		}
		if (unit.GetUnitData() == null)
		{
			return false;
		}
		int unitLevel = unit.GetUnitData().m_UnitLevel;
		if (m_LevelRange.m_Min >= 0 && unitLevel < m_LevelRange.m_Min)
		{
			return false;
		}
		if (m_LevelRange.m_Max >= 0 && unitLevel > m_LevelRange.m_Max)
		{
			return false;
		}
		return true;
	}

	public bool CanUseUnitExist(NKMGame game, NKM_TEAM_TYPE myTeam)
	{
		return CanUseUnitExist(game, m_listReqUnit, m_bReqUnitEnemy, myTeam, m_ReqUnitCond);
	}

	private bool CanUseUnitExist(NKMGame game, List<int> lstUnitID, bool bEnemy, NKM_TEAM_TYPE myTeam, UnitCountCond cond)
	{
		if (lstUnitID == null)
		{
			return true;
		}
		return cond switch
		{
			UnitCountCond.ALL => lstUnitID.All(UnitExist), 
			UnitCountCond.ANY => lstUnitID.Exists(UnitExist), 
			UnitCountCond.NOT_ALL => lstUnitID.All(UnitNotExist), 
			UnitCountCond.NOT_ANY => lstUnitID.Exists(UnitNotExist), 
			_ => true, 
		};
		bool UnitExist(int unitID)
		{
			return (bEnemy ? game.GetEnemyUnitByUnitID(unitID, myTeam) : game.GetUnitByUnitID(unitID, myTeam)) != null;
		}
		bool UnitNotExist(int unitID)
		{
			return (bEnemy ? game.GetEnemyUnitByUnitID(unitID, myTeam) : game.GetUnitByUnitID(unitID, myTeam)) == null;
		}
	}

	public bool CanUseReactorSkill(NKMUnit unit)
	{
		if (m_ReactorLevel.m_Min != -1 && m_ReactorLevel.m_Min > unit.GetUnitData().reactorLevel)
		{
			return false;
		}
		if (m_ReactorLevel.m_Max != -1 && m_ReactorLevel.m_Max < unit.GetUnitData().reactorLevel)
		{
			return false;
		}
		return true;
	}

	public bool CheckEventVariable(NKMUnit unit)
	{
		if (string.IsNullOrEmpty(m_EventVariableName))
		{
			return true;
		}
		int eventVariable = unit.GetEventVariable(m_EventVariableName);
		return m_EventVariableValue.IsBetween(eventVariable, negativeIsTrue: false);
	}
}
