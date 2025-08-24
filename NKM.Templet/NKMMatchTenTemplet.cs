using System;
using System.Collections.Generic;
using System.Linq;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMMatchTenTemplet : INKMTemplet, INKMTempletEx
{
	public int m_Id;

	public string m_OpenTag;

	public string m_DateStrID;

	public string m_BannerTitle;

	public string m_BannerDesc;

	public int m_PlayTimeSec;

	public int m_ScoreRewardGroupID;

	public int m_PlayScoreMid;

	public int m_PlayScoreHigh;

	public int m_BoardSizeX;

	public int m_BoardSizeY;

	private NKMIntervalTemplet intervalTemplet;

	public int Key => m_Id;

	public int m_PerfectScoreValue => m_BoardSizeX * m_BoardSizeY;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public NKMIntervalTemplet IntervalTemplet => intervalTemplet;

	public static IEnumerable<NKMMatchTenTemplet> Values => NKMTempletContainer<NKMMatchTenTemplet>.Values;

	public static NKMMatchTenTemplet Find(int key)
	{
		return NKMTempletContainer<NKMMatchTenTemplet>.Find((NKMMatchTenTemplet x) => x.m_Id == key);
	}

	public static NKMMatchTenTemplet GetByTime(DateTime time)
	{
		return (from e in Values
			where e.IsOn(time)
			orderby e.m_Id
			select e).FirstOrDefault();
	}

	public static NKMMatchTenTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMMatchTenTemplet nKMMatchTenTemplet = new NKMMatchTenTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_Id", ref nKMMatchTenTemplet.m_Id) ? 1u : 0u) & (cNKMLua.GetData("m_OpenTag", ref nKMMatchTenTemplet.m_OpenTag) ? 1u : 0u)) & (cNKMLua.GetData("m_DateStrID", ref nKMMatchTenTemplet.m_DateStrID) ? 1 : 0);
		cNKMLua.GetData("m_BannerTitle", ref nKMMatchTenTemplet.m_BannerTitle);
		cNKMLua.GetData("m_BannerDesc", ref nKMMatchTenTemplet.m_BannerDesc);
		int num2 = (int)((uint)num & (cNKMLua.GetData("m_PlayTimeSec", ref nKMMatchTenTemplet.m_PlayTimeSec) ? 1u : 0u)) & (cNKMLua.GetData("m_ScoreRewardGroupID", ref nKMMatchTenTemplet.m_ScoreRewardGroupID) ? 1 : 0);
		cNKMLua.GetData("m_PlayScoreMid", ref nKMMatchTenTemplet.m_PlayScoreMid);
		cNKMLua.GetData("m_PlayScoreHigh", ref nKMMatchTenTemplet.m_PlayScoreHigh);
		if (((uint)num2 & (cNKMLua.GetData("m_BoardSizeX", ref nKMMatchTenTemplet.m_BoardSizeX) ? 1u : 0u) & (cNKMLua.GetData("m_BoardSizeY", ref nKMMatchTenTemplet.m_BoardSizeY) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMMatchTenTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	private void JoinIntervalTemplet()
	{
		intervalTemplet = NKMIntervalTemplet.Find(m_DateStrID);
		if (intervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMMatchTenTemplet] interval templet\ufffd\ufffd ã\ufffd\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. m_Id:{m_Id} dateStrId:{m_DateStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMatchTenTemplet.cs", 71);
		}
	}

	public void Validate()
	{
		intervalTemplet?.Validate();
		if (m_BoardSizeX <= 0 || m_BoardSizeY <= 0)
		{
			NKMTempletError.Add($"NKMMatchTenTemplet(m_Id {m_Id}) : \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\uec21 \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd. m_BoardSIzeX:{m_BoardSizeX}, m_BoardSIzeY:{m_BoardSizeY}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMatchTenTemplet.cs", 81);
		}
		if (m_PerfectScoreValue <= 0)
		{
			NKMTempletError.Add($"NKMMatchTenTemplet(m_Id {m_Id}) : \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd \ufffd\u05b0\ufffd ȹ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd. m_PerfectScoreValue:{m_PerfectScoreValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMatchTenTemplet.cs", 86);
		}
		if (m_PlayTimeSec <= 0)
		{
			NKMTempletError.Add($"NKMMatchTenTemplet(m_Id {m_Id}) : \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdð\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd. m_PlayTimeSec:{m_PlayTimeSec}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMatchTenTemplet.cs", 91);
		}
		if (!NKMScoreRewardTemplet.Groups.ContainsKey(m_ScoreRewardGroupID))
		{
			NKMTempletError.Add($"NKMMatchTenTemplet(m_Id {m_Id}) : \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd \ufffd\ufffd \ufffdش\ufffd groupId\ufffd\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. m_ScoreRewardGroupID:{m_ScoreRewardGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMatchTenTemplet.cs", 96);
		}
	}

	public bool IsOn(DateTime current)
	{
		if (!EnableByTag)
		{
			return false;
		}
		if (intervalTemplet.IsValidTime(current))
		{
			return true;
		}
		return false;
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}

	public string GetRuleTitle()
	{
		return NKCStringTable.GetString(m_BannerTitle);
	}

	public string GetRuleDesc()
	{
		return NKCStringTable.GetString(m_BannerDesc);
	}
}
