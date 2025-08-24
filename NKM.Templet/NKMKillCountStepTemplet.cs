using System;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMKillCountStepTemplet : IComparable<NKMKillCountStepTemplet>
{
	public string KillCountStrId { get; private set; }

	public int KillCount { get; private set; }

	public bool IsUserStep { get; private set; }

	public int StepId { get; private set; }

	public NKMRewardInfo RewardInfo { get; private set; }

	public int CompareTo(NKMKillCountStepTemplet other)
	{
		return StepId.CompareTo(other.StepId);
	}

	public void Load(NKMLua lua)
	{
		KillCountStrId = lua.GetString("m_KillCountStrID");
		KillCount = lua.GetInt32("m_KillCountValue");
		IsUserStep = lua.GetBoolean("m_bIndividual");
		StepId = lua.GetInt32("Step");
		RewardInfo = new NKMRewardInfo();
		RewardInfo.rewardType = lua.GetEnum<NKM_REWARD_TYPE>("m_KillCountRewardType");
		RewardInfo.ID = lua.GetInt32("m_KillCountRewardID");
		RewardInfo.Count = lua.GetInt32("m_KillCountRewardQuantity");
	}

	public void Validate(NKMKillCountTemplet owner)
	{
		if (string.IsNullOrEmpty(KillCountStrId))
		{
			NKMTempletError.Add($"[KillCount] 보상 설명 Id가 올바르지 않음. eventId:{owner.EventId} step:{StepId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 194);
		}
		if (KillCount <= 0)
		{
			NKMTempletError.Add($"[KillCount] 보상 달성에 필요한 KillCount가 올바르지 않음. eventId:{owner.EventId} step:{StepId} killCount:{KillCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 199);
		}
		if (!NKMRewardTemplet.IsValidReward(RewardInfo.rewardType, RewardInfo.ID))
		{
			NKMTempletError.Add($"[KillCount] 보상 정보가 올바르지 않음. eventId:{owner.EventId} step:{StepId} rewardType:{RewardInfo.rewardType} rewardId:{RewardInfo.ID} count:{RewardInfo.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 204);
		}
		if (RewardInfo.Count <= 0)
		{
			NKMTempletError.Add($"[KillCount] 보상 개수가 올바르지 않음.  eventId:{owner.EventId} step:{StepId} count:{RewardInfo.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 209);
		}
	}
}
