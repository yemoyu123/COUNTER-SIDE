using System;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public class NKMLimitBreakTemplet : INKMTemplet
{
	public int m_iLBRank;

	public int m_iRequiredLevel;

	public int m_iMaxLevel;

	public int m_iUnitRequirement;

	public int m_Tier;

	public int Key => m_iLBRank;

	public static NKMLimitBreakTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitLimitBreakManager.cs", 28))
		{
			return null;
		}
		NKMLimitBreakTemplet nKMLimitBreakTemplet = new NKMLimitBreakTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_iLBRank", ref nKMLimitBreakTemplet.m_iLBRank) ? 1u : 0u) & (cNKMLua.GetData("m_iRequiredLevel", ref nKMLimitBreakTemplet.m_iRequiredLevel) ? 1u : 0u) & (cNKMLua.GetData("m_iMaxLevel", ref nKMLimitBreakTemplet.m_iMaxLevel) ? 1u : 0u) & (cNKMLua.GetData("m_iUnitRequirement", ref nKMLimitBreakTemplet.m_iUnitRequirement) ? 1u : 0u)) & (cNKMLua.GetData("m_Tier", ref nKMLimitBreakTemplet.m_Tier) ? 1 : 0);
		NKMUnitLimitBreakManager.LIMITBREAK_TIER_MAX = Math.Max(nKMLimitBreakTemplet.m_Tier, NKMUnitLimitBreakManager.LIMITBREAK_TIER_MAX);
		if (num == 0)
		{
			Log.Error($"NKMLimitBreakTemplet Load - {nKMLimitBreakTemplet.m_iLBRank}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitLimitBreakManager.cs", 44);
			return null;
		}
		return nKMLimitBreakTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
