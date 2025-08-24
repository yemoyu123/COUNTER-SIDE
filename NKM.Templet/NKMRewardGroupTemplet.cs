using System;
using System.Collections.Generic;
using Cs.Core.Util;
using Cs.Math.Lottery;

namespace NKM.Templet;

public sealed class NKMRewardGroupTemplet
{
	private readonly List<NKMRewardTemplet> rewardList = new List<NKMRewardTemplet>();

	public int GroupId { get; }

	public IReadOnlyList<NKMRewardTemplet> List => rewardList;

	public NKMRewardGroupTemplet(int groupId)
	{
		GroupId = groupId;
	}

	public void Add(NKMRewardTemplet templet)
	{
		rewardList.Add(templet);
	}

	public NKMRewardTemplet Decide()
	{
		RatioLottery<NKMRewardTemplet> ratioLottery = new RatioLottery<NKMRewardTemplet>();
		DateTime recent = ServiceTime.Recent;
		foreach (NKMRewardTemplet reward in rewardList)
		{
			if (reward.intervalTemplet.IsValidTime(recent))
			{
				ratioLottery.AddCase(reward.m_Ratio, reward);
			}
		}
		if (ratioLottery.Count <= 0)
		{
			return null;
		}
		return ratioLottery.Decide();
	}
}
