using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Game;

public class NKMEventConditionSkillLevel : NKMEventConditionDetail
{
	public string m_SkillStrID = "";

	public NKMMinMaxInt m_SkillLevel = new NKMMinMaxInt(-1, -1);

	private bool m_bShipSkill;

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		int skillLevel;
		if (cNKMUnit.HasMasterUnit())
		{
			NKMUnit masterUnit = cNKMUnit.GetMasterUnit();
			if (masterUnit == null)
			{
				return false;
			}
			skillLevel = masterUnit.GetUnitData().GetSkillLevel(m_SkillStrID);
		}
		else
		{
			skillLevel = cNKMUnit.GetUnitData().GetSkillLevel(m_SkillStrID);
		}
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
		return true;
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionSkillLevel nKMEventConditionSkillLevel = new NKMEventConditionSkillLevel();
		nKMEventConditionSkillLevel.m_SkillStrID = m_SkillStrID;
		nKMEventConditionSkillLevel.m_SkillLevel.DeepCopyFromSource(m_SkillLevel);
		nKMEventConditionSkillLevel.m_bShipSkill = m_bShipSkill;
		return nKMEventConditionSkillLevel;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		int result = (int)(1u & (cNKMLua.GetData("m_SkillStrID", ref m_SkillStrID) ? 1u : 0u)) & (m_SkillLevel.LoadFromLua(cNKMLua, "m_SkillLevel") ? 1 : 0);
		int skillID = NKMShipSkillManager.GetSkillID(m_SkillStrID);
		m_bShipSkill = skillID != -1;
		return (byte)result != 0;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		if (string.IsNullOrEmpty(m_SkillStrID) || unitTemplet.m_UnitTempletBase.m_bMonster || m_SkillStrID.Equals("-1"))
		{
			return true;
		}
		NKMUnitTempletBase ownerTemplet = ((masterTemplet != null) ? masterTemplet : unitTemplet.m_UnitTempletBase);
		if (ownerTemplet.IsShip())
		{
			IEnumerable<NKMUnitTempletBase> enumerable = NKMUnitTempletBase.Values.Where((NKMUnitTempletBase e) => e.m_ShipGroupID == ownerTemplet.m_ShipGroupID);
			bool flag = false;
			foreach (NKMUnitTempletBase item in enumerable)
			{
				if (item.GetSkillIndex(m_SkillStrID) >= 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (masterTemplet != null)
				{
					NKMTempletError.Add("[EventConditionSkill] " + masterTemplet.DebugName + " \ufffdԼ\ufffd\ufffd\ufffd \ufffd\ufffdȯ\ufffdϴ\ufffd " + unitTemplet.m_UnitTempletBase.DebugName + "\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffdų\ufffd\ufffd\ufffd\u0335\ufffd (" + m_SkillStrID + ")\ufffd\ufffd \ufffd߸\ufffd\ufffdǾ\ufffd\ufffd\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 664);
				}
				else
				{
					NKMTempletError.Add("[EventConditionSkill] " + unitTemplet.m_UnitTempletBase.DebugName + " \ufffdԼ\ufffd\ufffd\ufffd \ufffd\ufffdų\ufffd\ufffd\ufffd\u0335\ufffd (" + m_SkillStrID + ")\ufffd\ufffd \ufffd߸\ufffd\ufffdǾ\ufffd\ufffd\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 668);
				}
				return false;
			}
		}
		else if (ownerTemplet.GetSkillIndex(m_SkillStrID) < 0)
		{
			if (masterTemplet != null)
			{
				NKMTempletError.Add("[EventConditionSkill] " + masterTemplet.DebugName + " \ufffdԼ\ufffd\ufffd\ufffd \ufffd\ufffdȯ\ufffdϴ\ufffd " + unitTemplet.m_UnitTempletBase.DebugName + "\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffdų\ufffd\ufffd\ufffd\u0335\ufffd (" + m_SkillStrID + ")\ufffd\ufffd \ufffd߸\ufffd\ufffdǾ\ufffd\ufffd\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 679);
			}
			else
			{
				NKMTempletError.Add("[EventConditionSkill] " + unitTemplet.m_UnitTempletBase.DebugName + " \ufffdԼ\ufffd\ufffd\ufffd \ufffd\ufffdų\ufffd\ufffd\ufffd\u0335\ufffd (" + m_SkillStrID + ")\ufffd\ufffd \ufffd߸\ufffd\ufffdǾ\ufffd\ufffd\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 683);
			}
			return false;
		}
		if (NKMUnitSkillManager.GetSkillID(m_SkillStrID) != -1)
		{
			return true;
		}
		if (NKMShipSkillManager.GetSkillID(m_SkillStrID) != -1)
		{
			return true;
		}
		NKMTempletError.Add("[EventConditionSkill] " + m_SkillStrID + " \ufffd\ufffdų\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 699);
		return false;
	}
}
