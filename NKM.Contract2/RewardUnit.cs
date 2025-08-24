using System.Runtime.CompilerServices;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Contract2;

public sealed class RewardUnit
{
	public NKM_REWARD_TYPE RewardType { get; }

	public int ItemID { get; }

	public int Count { get; }

	public RewardUnit(NKM_REWARD_TYPE rewardType, int itemId, int count)
	{
		RewardType = rewardType;
		ItemID = itemId;
		Count = count;
	}

	public void Validate([CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (!NKMRewardTemplet.IsValidReward(RewardType, ItemID))
		{
			NKMTempletError.Add($"[RewardUnit] invalid data. type:{RewardType} itemId:{ItemID}", file, line);
		}
		if (Count <= 0)
		{
			NKMTempletError.Add($"[RewardUnit] invalid itemCount:{Count}", file, line);
		}
	}

	public override string ToString()
	{
		return $"[{RewardType}] id:{ItemID} count:{Count}";
	}
}
