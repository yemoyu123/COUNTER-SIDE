using System.Collections.Generic;
using Cs.Core.Util;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMResetCounterGroupTemplet : INKMTemplet, INKMTempletEx
{
	private int groupId;

	private int maxCount;

	private COUNT_RESET_TYPE type;

	private string intervalStrId;

	private string openTag;

	public int GroupId => groupId;

	public int MaxCount => maxCount;

	public COUNT_RESET_TYPE Type => type;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public NKMIntervalTemplet IntervalTemplet { get; private set; }

	public int Key => groupId;

	public static IEnumerable<NKMResetCounterGroupTemplet> Values => NKMTempletContainer<NKMResetCounterGroupTemplet>.Values;

	public static NKMResetCounterGroupTemplet Find(int key)
	{
		return NKMTempletContainer<NKMResetCounterGroupTemplet>.Find(key);
	}

	public static NKMResetCounterGroupTemplet LoadFromLua(NKMLua lua)
	{
		NKMResetCounterGroupTemplet nKMResetCounterGroupTemplet = new NKMResetCounterGroupTemplet();
		_ = 1u & (lua.GetData("GroupID", ref nKMResetCounterGroupTemplet.groupId) ? 1u : 0u) & (lua.GetData("MaxCount", ref nKMResetCounterGroupTemplet.maxCount) ? 1u : 0u) & (lua.GetData("ResetType", ref nKMResetCounterGroupTemplet.type) ? 1u : 0u);
		lua.GetData("OpenTag", ref nKMResetCounterGroupTemplet.openTag);
		lua.GetData("IntervalStrId", ref nKMResetCounterGroupTemplet.intervalStrId);
		return nKMResetCounterGroupTemplet;
	}

	public void Join()
	{
		if (intervalStrId != null && NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void Validate()
	{
	}

	private void JoinIntervalTemplet()
	{
		IntervalTemplet = NKMIntervalTemplet.Find(intervalStrId);
		if (IntervalTemplet == null)
		{
			IntervalTemplet = NKMIntervalTemplet.Invalid;
			NKMTempletError.Add($"[NKMResetCountGroupTemplet:{groupId}] invalid seasonDateStrId:{intervalStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMResetCounterGroupTemplet.cs", 73);
		}
	}

	public void PostJoin()
	{
		if (intervalStrId != null)
		{
			JoinIntervalTemplet();
		}
	}

	public bool IsValid()
	{
		bool flag = true;
		if (!EnableByTag)
		{
			return false;
		}
		if (intervalStrId != null)
		{
			flag &= IntervalTemplet.IsValidTime(ServiceTime.Now);
		}
		return flag;
	}
}
