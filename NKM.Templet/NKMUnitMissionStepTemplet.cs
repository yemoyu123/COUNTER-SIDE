using System;
using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMUnitMissionStepTemplet : IComparable<NKMUnitMissionStepTemplet>
{
	private static readonly Dictionary<int, NKMUnitMissionStepTemplet> StepTemplets = new Dictionary<int, NKMUnitMissionStepTemplet>();

	public int StepId { get; private set; }

	public int StepIndex { get; internal set; }

	public string DebugName => $"[id:{StepId} index:{StepIndex}]";

	public string MissionDesc { get; private set; }

	public int MissionValue { get; private set; }

	public NKMRewardInfo RewardInfo { get; private set; }

	public NKMUnitMissionTemplet Owner { get; }

	public NKMUnitMissionStepTemplet(NKMUnitMissionTemplet missionTemplet)
	{
		Owner = missionTemplet;
	}

	public static NKMUnitMissionStepTemplet Find(int stepId)
	{
		StepTemplets.TryGetValue(stepId, out var value);
		return value;
	}

	public int CompareTo(NKMUnitMissionStepTemplet other)
	{
		return MissionValue.CompareTo(other.MissionValue);
	}

	public void Load(NKMLua lua)
	{
		StepId = lua.GetInt32("StepID");
		MissionDesc = lua.GetString("Mission_Desc");
		MissionValue = lua.GetInt32("Mission_Value");
		RewardInfo = new NKMRewardInfo
		{
			rewardType = lua.GetEnum<NKM_REWARD_TYPE>("m_RewardType"),
			ID = lua.GetInt32("m_RewardID"),
			Count = lua.GetInt32("m_RewardValue")
		};
		if (StepTemplets.ContainsKey(StepId))
		{
			NKMTempletError.Add($"[UnitMission] StepId 중복:{StepId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitMissionTemplet.cs", 161);
		}
		else
		{
			StepTemplets.Add(StepId, this);
		}
	}

	public static void Drop()
	{
		StepTemplets.Clear();
	}

	public void Validate(NKMUnitMissionTemplet owner)
	{
		if (string.IsNullOrEmpty(MissionDesc))
		{
			NKMTempletError.Add($"[UnitMission] 미션 설명이 올바르지 않음. MissionId:{owner.MissionId}, StepId:{StepId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitMissionTemplet.cs", 178);
		}
		if (MissionValue <= 0)
		{
			NKMTempletError.Add($"[UnitMission] 미션 달성에 필요한 값이 올바르지 않음. MissionId:{owner.MissionId}, StepId:{StepId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitMissionTemplet.cs", 183);
		}
		if (!NKMRewardTemplet.IsValidReward(RewardInfo.rewardType, RewardInfo.ID))
		{
			NKMTempletError.Add($"[UnitMission] 보상 정보가 올바르지 않음. MissionId:{owner.MissionId} StepId:{StepId} rewardType:{RewardInfo.rewardType} rewardId:{RewardInfo.ID} count:{RewardInfo.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitMissionTemplet.cs", 188);
		}
		if (RewardInfo.Count <= 0)
		{
			NKMTempletError.Add($"[UnitMission] 보상 개수가 올바르지 않음. MissionId:{owner.MissionId} StepId:{StepId} rewardType:{RewardInfo.rewardType} rewardId:{RewardInfo.ID} count:{RewardInfo.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitMissionTemplet.cs", 193);
		}
	}

	public NKMUnitMissionStepTemplet GetNextStep()
	{
		int num = StepIndex + 1;
		if (num >= Owner.Steps.Count)
		{
			return null;
		}
		return Owner.Steps[num];
	}
}
