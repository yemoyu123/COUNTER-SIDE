using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMMiniGameTemplet : INKMTemplet, INKMTempletEx
{
	private NKMIntervalTemplet intervalTemplet;

	public int m_Id;

	public string m_OpenTag;

	public string m_DateStrID;

	public string m_BannerTitle;

	public string m_BannerDesc;

	public int m_ScoreRewardGroupID;

	public NKM_MINI_GAME_TYPE m_MiniGameType;

	public int Key => m_Id;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public NKMIntervalTemplet IntervalTemplet => intervalTemplet;

	public static IEnumerable<NKMMiniGameTemplet> Values => NKMTempletContainer<NKMMiniGameTemplet>.Values;

	public static NKMMiniGameTemplet Find(int key)
	{
		return NKMTempletContainer<NKMMiniGameTemplet>.Find((NKMMiniGameTemplet x) => x.m_Id == key);
	}

	public static NKMMiniGameTemplet GetByTime(DateTime time)
	{
		return (from e in Values
			where e.IsOn(time)
			orderby e.m_Id
			select e).FirstOrDefault();
	}

	public static NKMMiniGameTemplet LoadFromLua(NKMLua lua)
	{
		NKMMiniGameTemplet nKMMiniGameTemplet = new NKMMiniGameTemplet();
		if (!nKMMiniGameTemplet.Load(lua))
		{
			return null;
		}
		return nKMMiniGameTemplet;
	}

	protected virtual bool Load(NKMLua lua)
	{
		int num = (int)(1u & (lua.GetData("m_Id", ref m_Id) ? 1u : 0u) & (lua.GetData("m_OpenTag", ref m_OpenTag) ? 1u : 0u)) & (lua.GetData("m_DateStrID", ref m_DateStrID) ? 1 : 0);
		lua.GetData("m_BannerTitle", ref m_BannerTitle);
		lua.GetData("m_BannerDesc", ref m_BannerDesc);
		return (byte)((uint)num & (lua.GetData("m_ScoreRewardGroupID", ref m_ScoreRewardGroupID) ? 1u : 0u) & (lua.GetData("m_GameType", ref m_MiniGameType) ? 1u : 0u)) != 0;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void Validate()
	{
		intervalTemplet?.Validate();
		if (!NKMScoreRewardTemplet.Groups.ContainsKey(m_ScoreRewardGroupID))
		{
			Log.Warn($"NKMMiniGameTemplet(m_Id {m_Id}) : 보상 목록 중 해당 groupId로 된 목록이 존재하지 않음. m_ScoreRewardGroupID:{m_ScoreRewardGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMiniGameTemplet.cs", 60);
		}
	}

	public bool IsOn(DateTime current)
	{
		if (!EnableByTag)
		{
			return false;
		}
		if (!intervalTemplet.IsValidTime(current))
		{
			return false;
		}
		return true;
	}

	private void JoinIntervalTemplet()
	{
		intervalTemplet = NKMIntervalTemplet.Find(m_DateStrID);
		if (intervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMMiniGameTemplet] interval templet을 찾을 수 없음. m_Id:{m_Id} dateStrId:{m_DateStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMiniGameTemplet.cs", 84);
		}
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
