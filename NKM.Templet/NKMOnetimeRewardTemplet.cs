using Cs.Logging;
using Cs.Math;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMOnetimeRewardTemplet
{
	public const int MAX_PROBABILITY = 10000;

	private readonly NKMRewardInfo rewardInfo;

	private readonly int probability;

	private NKMOnetimeRewardTemplet(NKMRewardInfo rewardInfo, int probability)
	{
		this.rewardInfo = rewardInfo;
		this.probability = probability;
	}

	public static NKMOnetimeRewardTemplet Load(NKMLua lua, int index)
	{
		NKMRewardInfo nKMRewardInfo = new NKMRewardInfo
		{
			paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
		};
		int rValue = 0;
		int num = index + 1;
		bool num2 = lua.GetData($"m_OneTimeRewardType_{num}", ref nKMRewardInfo.rewardType) & lua.GetData($"m_OneTimeRewardId_{num}", ref nKMRewardInfo.ID) & lua.GetData($"m_OneTimeRewardCount_{num}", ref nKMRewardInfo.Count);
		lua.GetData($"m_OneTimeRewardProbability_{num}", ref rValue);
		if (!num2)
		{
			return null;
		}
		return new NKMOnetimeRewardTemplet(nKMRewardInfo, rValue);
	}

	public bool IsValid()
	{
		if (!NKMRewardTemplet.IsValidReward(rewardInfo.rewardType, rewardInfo.ID))
		{
			Log.Error($"[OneTimeReward] 보상 정보가 유효하지 않음 Type:{rewardInfo.rewardType} Id:{rewardInfo.ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 144);
			return false;
		}
		if (rewardInfo.Count < 0)
		{
			Log.Error($"[OneTimeReward] 보상 갯수는 0보다 작을 수 없음 Type:{rewardInfo.rewardType} Id:{rewardInfo.ID} Count:{rewardInfo.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 150);
			return false;
		}
		return true;
	}

	public void ValidateServerOnly()
	{
		if (probability < 0 || probability > 10000)
		{
			NKMTempletError.Add($"[OneTimeReward] 보상 확률은 0보다 작거나 {10000}보다 클 수 없음 Type:{rewardInfo.rewardType} Id:{rewardInfo.ID} Probability:{probability}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 162);
		}
	}

	public bool MakeDecision(out NKMRewardInfo rewardInfo)
	{
		if (RandomGenerator.Next(10000) < probability)
		{
			rewardInfo = this.rewardInfo;
			return true;
		}
		rewardInfo = null;
		return false;
	}

	public NKMRewardInfo GetRewardInfo()
	{
		return rewardInfo;
	}
}
