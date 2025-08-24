using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMLimitBreakItemTemplet : INKMTemplet
{
	public struct ItemRequirement
	{
		public int itemID;

		public int count;
	}

	private const int ITEM_MAX_COUNT = 2;

	public NKM_UNIT_STYLE_TYPE m_NKM_UNIT_STYLE_TYPE;

	public NKM_UNIT_GRADE m_NKM_UNIT_GRADE;

	public int m_TargetLimitbreakLevel;

	public int m_CreditReq;

	public List<ItemRequirement> m_lstRequiredItem = new List<ItemRequirement>(2);

	public int Key => NKMUnitLimitBreakManager.MakeKey(m_NKM_UNIT_STYLE_TYPE, m_NKM_UNIT_GRADE, m_TargetLimitbreakLevel);

	public static NKMLimitBreakItemTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitLimitBreakManager.cs", 76))
		{
			return null;
		}
		NKMLimitBreakItemTemplet nKMLimitBreakItemTemplet = new NKMLimitBreakItemTemplet();
		bool flag = true;
		flag &= lua.GetData("m_NKM_UNIT_STYLE_TYPE", ref nKMLimitBreakItemTemplet.m_NKM_UNIT_STYLE_TYPE);
		flag &= lua.GetData("m_NKM_UNIT_GRADE", ref nKMLimitBreakItemTemplet.m_NKM_UNIT_GRADE);
		flag &= lua.GetData("m_TargetLimitbreakLevel", ref nKMLimitBreakItemTemplet.m_TargetLimitbreakLevel);
		flag &= lua.GetData("m_CreditReq", ref nKMLimitBreakItemTemplet.m_CreditReq);
		for (int i = 0; i < 2; i++)
		{
			int rValue = 0;
			lua.GetData("m_ItemID_" + (i + 1), ref rValue);
			if (rValue != 0)
			{
				int rValue2 = 0;
				lua.GetData("m_ItemCount_" + (i + 1), ref rValue2);
				nKMLimitBreakItemTemplet.m_lstRequiredItem.Add(new ItemRequirement
				{
					itemID = rValue,
					count = rValue2
				});
			}
		}
		if (!flag)
		{
			Log.Error("NKMLimitBreakItemTemplet Load Fail", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitLimitBreakManager.cs", 101);
			return null;
		}
		return nKMLimitBreakItemTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
