using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMRaidSeasonTemplet : INKMTemplet, INKMTempletEx
{
	private string dateStrId;

	private int raidSeasonId;

	private int raidBoardId;

	private int raidBossId;

	public int RaidSeasonId => raidSeasonId;

	public int RaidBoardId => raidBoardId;

	public int RaidBossId => raidBossId;

	public NKMIntervalTemplet IntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public IReadOnlyList<NKMRaidSeasonRewardTemplet> RaidSeasonRewardTemplet { get; private set; }

	public int Key => raidSeasonId;

	public static IEnumerable<NKMRaidSeasonTemplet> Values => NKMTempletContainer<NKMRaidSeasonTemplet>.Values;

	public static NKMRaidSeasonTemplet Find(int key)
	{
		return NKMTempletContainer<NKMRaidSeasonTemplet>.Find((NKMRaidSeasonTemplet x) => x.Key == key);
	}

	public static NKMRaidSeasonTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidSeasonTemplet.cs", 29))
		{
			return null;
		}
		NKMRaidSeasonTemplet nKMRaidSeasonTemplet = new NKMRaidSeasonTemplet();
		if ((1u & (cNKMLua.GetData("m_DateStrID", ref nKMRaidSeasonTemplet.dateStrId) ? 1u : 0u) & (cNKMLua.GetData("Raid_Season_ID", ref nKMRaidSeasonTemplet.raidSeasonId) ? 1u : 0u) & (cNKMLua.GetData("Reward_Board_ID", ref nKMRaidSeasonTemplet.raidBoardId) ? 1u : 0u) & (cNKMLua.GetData("Raid_Boss_ID", ref nKMRaidSeasonTemplet.raidBossId) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMRaidSeasonTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void JoinIntervalTemplet()
	{
		IntervalTemplet = NKMIntervalTemplet.Find(dateStrId);
		if (IntervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMRaidSeasonTemplet:{Key}] 잘못된 interval id:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidSeasonTemplet.cs", 56);
			IntervalTemplet = NKMIntervalTemplet.Unuseable;
		}
		else if (IntervalTemplet.IsRepeatDate)
		{
			NKMTempletError.Add($"[NKMRaidSeasonTemplet:{Key}] 반복 기간설정 사용 불가. id:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidSeasonTemplet.cs", 61);
		}
		RaidSeasonRewardTemplet = NKMRaidSeasonRewardTemplet.Values.Where((NKMRaidSeasonRewardTemplet t) => t.RewardBoardId == raidBoardId).ToList();
		if (RaidSeasonRewardTemplet.Count == 0)
		{
			NKMTempletError.Add($"[NKMRaidSeasonTemplet:{Key}] 해당시즌에 맞는 보상정보가 없음. raidBoardId:{raidBoardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidSeasonTemplet.cs", 67);
		}
	}

	public void Validate()
	{
		NKMRaidSeasonRewardTemplet nKMRaidSeasonRewardTemplet = (from e in NKMRaidSeasonRewardTemplet.Values
			where RaidBoardId == e.RewardBoardId
			orderby e.RaidPoint descending
			select e).FirstOrDefault();
		if (nKMRaidSeasonRewardTemplet.ExtraRewardId > 0)
		{
			List<NKMRaidSeasonRewardTemplet> list = RaidSeasonRewardTemplet.Where((NKMRaidSeasonRewardTemplet e) => e.ExtraRewardId > 0).ToList();
			if (list.Count > 1)
			{
				NKMTempletError.Add($"[NKMRaidSeasonTemplet:{Key}] 해당 시즌에 추가 보상이 여러개 존재함. raidBoardId:{raidBoardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidSeasonTemplet.cs", 80);
			}
			NKMRaidSeasonRewardTemplet nKMRaidSeasonRewardTemplet2 = list.FirstOrDefault();
			if (nKMRaidSeasonRewardTemplet.RaidPoint != nKMRaidSeasonRewardTemplet2.RaidPoint)
			{
				NKMTempletError.Add($"[NKMRaidSeasonTemplet:{Key}] 해당 시즌에 확장 보상 값이 최댓값에 존재하지 않음. raidBoardId:{raidBoardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidSeasonTemplet.cs", 86);
			}
		}
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
