using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM;

public class NKMShipSkillTemplet : INKMTemplet
{
	public int m_ShipSkillID;

	public string m_ShipSkillStrID = "";

	public string m_ShipSkillIcon = "";

	public string m_SkillName = "";

	public string m_SkillDesc = "";

	public string m_SkillBuildDesc = "";

	public NKM_SKILL_TYPE m_NKM_SKILL_TYPE;

	public NKM_SHIP_SKILL_USE_TYPE m_NKM_SHIP_SKILL_USE_TYPE = NKM_SHIP_SKILL_USE_TYPE.NSSUT_ENEMY;

	public string m_UnitStateName = "";

	public bool m_bFullMap;

	public float m_fRange;

	public bool m_bEnemy = true;

	public bool m_bAir = true;

	public float m_fCooltimeSecond = 1f;

	public List<SkillStatData> m_lstSkillStatData = new List<SkillStatData>(5);

	public int Key => m_ShipSkillID;

	public static NKMShipSkillTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMShipSkillTemplet nKMShipSkillTemplet = new NKMShipSkillTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_ShipSkillID", ref nKMShipSkillTemplet.m_ShipSkillID);
		flag &= cNKMLua.GetData("m_ShipSkillStrID", ref nKMShipSkillTemplet.m_ShipSkillStrID);
		flag &= cNKMLua.GetData("m_ShipSkillIcon", ref nKMShipSkillTemplet.m_ShipSkillIcon);
		flag &= cNKMLua.GetData("m_SkillName", ref nKMShipSkillTemplet.m_SkillName);
		cNKMLua.GetData("m_SkillDesc", ref nKMShipSkillTemplet.m_SkillDesc);
		cNKMLua.GetData("m_SkillBuildDesc", ref nKMShipSkillTemplet.m_SkillBuildDesc);
		flag &= cNKMLua.GetData("m_NKM_SKILL_TYPE", ref nKMShipSkillTemplet.m_NKM_SKILL_TYPE);
		flag &= cNKMLua.GetData("m_NKM_SHIP_SKILL_USE_TYPE", ref nKMShipSkillTemplet.m_NKM_SHIP_SKILL_USE_TYPE);
		switch (nKMShipSkillTemplet.m_NKM_SKILL_TYPE)
		{
		case NKM_SKILL_TYPE.NST_SHIP_ACTIVE:
			flag &= cNKMLua.GetData("m_UnitStateName", ref nKMShipSkillTemplet.m_UnitStateName);
			flag &= cNKMLua.GetData("m_bFullMap", ref nKMShipSkillTemplet.m_bFullMap);
			flag &= cNKMLua.GetData("m_fRange", ref nKMShipSkillTemplet.m_fRange);
			flag &= cNKMLua.GetData("m_bEnemy", ref nKMShipSkillTemplet.m_bEnemy);
			flag &= cNKMLua.GetData("m_bAir", ref nKMShipSkillTemplet.m_bAir);
			flag &= cNKMLua.GetData("m_fCooltimeSecond", ref nKMShipSkillTemplet.m_fCooltimeSecond);
			break;
		case NKM_SKILL_TYPE.NST_PASSIVE:
		case NKM_SKILL_TYPE.NST_LEADER:
		{
			nKMShipSkillTemplet.m_lstSkillStatData.Clear();
			for (int i = 0; i < 5; i++)
			{
				string text = (i + 1).ToString();
				NKM_STAT_TYPE result = NKM_STAT_TYPE.NST_END;
				if (!cNKMLua.GetData("m_NKM_STAT_TYPE" + text, ref result) || result == NKM_STAT_TYPE.NST_END)
				{
					break;
				}
				float statValue = 0f;
				NKMUnitStatManager.LoadStat(cNKMLua, "m_fStatValue" + text, "m_fStatRate" + text, ref result, ref statValue);
				nKMShipSkillTemplet.m_lstSkillStatData.Add(new SkillStatData(result, statValue));
			}
			break;
		}
		default:
			Log.Error("ShipSkill Can't have skilltype " + nKMShipSkillTemplet.m_NKM_SKILL_TYPE, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipSkillManager.cs", 100);
			return null;
		}
		if (!flag)
		{
			Log.Error($"NKMShipSkillTemplet Load - {nKMShipSkillTemplet.m_ShipSkillID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipSkillManager.cs", 106);
			return null;
		}
		return nKMShipSkillTemplet;
	}

	public void Validate()
	{
	}

	public void Join()
	{
	}

	public string GetName()
	{
		return NKCStringTable.GetString(m_SkillName);
	}

	public string GetDesc()
	{
		return NKCStringTable.GetString(m_SkillDesc);
	}

	public string GetBuildDesc()
	{
		return NKCStringTable.GetString(m_SkillBuildDesc);
	}
}
