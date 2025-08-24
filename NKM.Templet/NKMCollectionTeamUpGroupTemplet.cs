using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMCollectionTeamUpGroupTemplet : INKMTemplet
{
	private readonly int teamID;

	private readonly string teamName;

	private readonly List<int> unitIDList = new List<int>();

	private readonly List<string> unitStrIDList = new List<string>();

	private readonly int rewardCriteria;

	private readonly NKM_REWARD_TYPE rewardType;

	private readonly int rewardID;

	private readonly int rewardValue;

	private static string OpenTag = "TAG_COLLECTION_TEAMUP_REWARD";

	public int Key => teamID;

	public int TeamID => teamID;

	public string TeamName => teamName;

	public List<int> UnitIDList => unitIDList;

	public List<string> UnitStrIDList => unitStrIDList;

	public int RewardCriteria => rewardCriteria;

	public NKM_REWARD_TYPE RewardType => rewardType;

	public int RewardID => rewardID;

	public int RewardValue => rewardValue;

	public static bool EnableByTag => NKMOpenTagManager.IsOpened(OpenTag);

	public NKMCollectionTeamUpGroupTemplet(int teamID, List<NKMCollectionTeamUpTemplet> list)
	{
		if (list == null || list.Count == 0)
		{
			Log.ErrorAndExit($"invalid list data. teamID:{teamID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionTeamUpGroupTemplet.cs", 35);
			return;
		}
		this.teamID = teamID;
		foreach (NKMCollectionTeamUpTemplet item in list)
		{
			unitIDList.Add(item.UnitID);
			unitStrIDList.Add(item.UnitStrID);
		}
		NKMCollectionTeamUpTemplet nKMCollectionTeamUpTemplet = list[0];
		teamName = nKMCollectionTeamUpTemplet.TeamName;
		rewardCriteria = nKMCollectionTeamUpTemplet.RewardCriteria;
		rewardType = nKMCollectionTeamUpTemplet.RewardType;
		rewardID = nKMCollectionTeamUpTemplet.RewardID;
		rewardValue = nKMCollectionTeamUpTemplet.RewardValue;
	}

	public static NKMCollectionTeamUpGroupTemplet Find(int key)
	{
		return NKMTempletContainer<NKMCollectionTeamUpGroupTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		foreach (int unitID in unitIDList)
		{
			if (NKMUnitManager.GetUnitTempletBase(unitID) == null)
			{
				Log.ErrorAndExit($"[NKMCollectionTeamUpGroupTemplet] 유닛 정보가 존재하지 않음 teamID:{teamID}, unitID:{unitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionTeamUpGroupTemplet.cs", 67);
			}
		}
		if (!NKMRewardTemplet.IsValidReward(rewardType, rewardID))
		{
			Log.ErrorAndExit($"[CollectionTeamUpTemplet] 보상 정보가 존재하지 않음 teamID:{teamID}, rewardType:{rewardType}, rewardID:{rewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionTeamUpGroupTemplet.cs", 73);
		}
	}
}
