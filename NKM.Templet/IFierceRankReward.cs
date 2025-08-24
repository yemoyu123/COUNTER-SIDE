using System.Collections.Generic;

namespace NKM.Templet;

public interface IFierceRankReward
{
	int RankValue { get; }

	int FierceRankRewardID { get; }

	List<NKM_REWARD_DATA> Rewards { get; }
}
