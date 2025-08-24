using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMUnitSkillTemplet : INKMTemplet
{
	public struct NKMUpgradeReqItem
	{
		public int ItemID;

		public int ItemCount;
	}

	public int m_ID;

	public string m_strID = "";

	public int m_Level = 1;

	public string m_UnitSkillIcon = "";

	public string m_SkillName = "";

	public string m_SkillDesc = "";

	public int m_AttackCount;

	public NKM_SKILL_TYPE m_NKM_SKILL_TYPE;

	public int m_UnlockReqUpgrade;

	public List<NKMUpgradeReqItem> m_lstUpgradeReqItem = new List<NKMUpgradeReqItem>(4);

	public float m_fCooltimeSecond;

	public float m_fEmpowerFactor = 1f;

	public List<SkillStatData> m_lstSkillStatData = new List<SkillStatData>(5);

	private const int MAX_REQ_ITEM_TYPE_COUNT = 4;

	public int Key => m_ID;

	public static NKMUnitSkillTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMUnitSkillTemplet nKMUnitSkillTemplet = new NKMUnitSkillTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_UnitSkillID", ref nKMUnitSkillTemplet.m_ID);
		flag &= cNKMLua.GetData("m_UnitSkillStrID", ref nKMUnitSkillTemplet.m_strID);
		flag &= cNKMLua.GetData("m_Level", ref nKMUnitSkillTemplet.m_Level);
		flag &= cNKMLua.GetData("m_UnitSkillIcon", ref nKMUnitSkillTemplet.m_UnitSkillIcon);
		flag &= cNKMLua.GetData("m_SkillName", ref nKMUnitSkillTemplet.m_SkillName);
		flag &= cNKMLua.GetData("m_SkillDesc", ref nKMUnitSkillTemplet.m_SkillDesc);
		cNKMLua.GetData("m_AttackCount", ref nKMUnitSkillTemplet.m_AttackCount);
		flag &= cNKMLua.GetData("m_NKM_SKILL_TYPE", ref nKMUnitSkillTemplet.m_NKM_SKILL_TYPE);
		if (nKMUnitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_INVALID)
		{
			Log.Error("Skill Templet with INVALID skill type, table error!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitSkillManager.cs", 133);
			flag = false;
		}
		cNKMLua.GetData("m_UnlockReqUpgrade", ref nKMUnitSkillTemplet.m_UnlockReqUpgrade);
		if (!flag)
		{
			Log.Error("UnitSkillTemplet parsing failed, id = " + nKMUnitSkillTemplet.m_strID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitSkillManager.cs", 141);
			return null;
		}
		for (int i = 0; i < 4; i++)
		{
			int rValue = 0;
			int rValue2 = 0;
			cNKMLua.GetData("m_UpgradeReqtemID_" + (i + 1), ref rValue);
			cNKMLua.GetData("m_UpgradeReqtemValue_" + (i + 1), ref rValue2);
			if (rValue != 0 && rValue2 != 0)
			{
				nKMUnitSkillTemplet.m_lstUpgradeReqItem.Add(new NKMUpgradeReqItem
				{
					ItemCount = rValue2,
					ItemID = rValue
				});
			}
		}
		cNKMLua.GetData("m_fCooltimeSecond", ref nKMUnitSkillTemplet.m_fCooltimeSecond);
		cNKMLua.GetData("m_fEmpowerFactor", ref nKMUnitSkillTemplet.m_fEmpowerFactor);
		nKMUnitSkillTemplet.m_lstSkillStatData.Clear();
		for (int j = 0; j < 5; j++)
		{
			string text = (j + 1).ToString();
			NKM_STAT_TYPE result = NKM_STAT_TYPE.NST_END;
			if (!cNKMLua.GetData("m_NKM_STAT_TYPE" + text, ref result) || result == NKM_STAT_TYPE.NST_END)
			{
				break;
			}
			float statValue = 0f;
			NKMUnitStatManager.LoadStat(cNKMLua, "m_fStatValue" + text, "m_fStatRate" + text, ref result, ref statValue);
			nKMUnitSkillTemplet.m_lstSkillStatData.Add(new SkillStatData(result, statValue));
		}
		return nKMUnitSkillTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public string GetSkillDesc()
	{
		return NKCStringTable.GetString(m_SkillDesc);
	}

	public string GetSkillName()
	{
		return NKCStringTable.GetString(m_SkillName);
	}
}
