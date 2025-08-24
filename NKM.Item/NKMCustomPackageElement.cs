using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Item;

public sealed class NKMCustomPackageElement : INKMTemplet
{
	public int Key => GroupId;

	public int Index { get; private set; }

	public int GroupId { get; private set; }

	public NKM_REWARD_TYPE RewardType { get; private set; }

	public int RewardId { get; private set; }

	public int FreeRewardCount { get; private set; }

	public int PaidRewardCount { get; private set; }

	public int TotalRewardCount => FreeRewardCount + PaidRewardCount;

	public string OpenTag { get; private set; }

	public bool EnableByTag => NKMOpenTagManager.IsOpened(OpenTag);

	public static NKMCustomPackageElement LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMCustomPackageElement.cs", 22))
		{
			return null;
		}
		return new NKMCustomPackageElement
		{
			Index = lua.GetInt32("m_Index"),
			GroupId = lua.GetInt32("m_CustomRewardGroupID"),
			RewardType = lua.GetEnum<NKM_REWARD_TYPE>("m_RewardType"),
			RewardId = lua.GetInt32("m_RewardID"),
			FreeRewardCount = lua.GetInt32("m_FreeValue", 0),
			PaidRewardCount = lua.GetInt32("m_PaidValue", 0),
			OpenTag = lua.GetString("m_OpenTag", null)
		};
	}

	public override string ToString()
	{
		return $"rewardType:{RewardType} rewardId:{RewardId} FreeRewardCount:{FreeRewardCount} PaidRewardCount:{PaidRewardCount}";
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (Index < 0)
		{
			NKMTempletError.Add($"[CustomPackageElement] index 범위가 유효하지 않음. groupId:{GroupId} index:{Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMCustomPackageElement.cs", 52);
		}
		if (!NKMRewardTemplet.IsValidReward(RewardType, RewardId) || (FreeRewardCount <= 0 && PaidRewardCount <= 0))
		{
			NKMTempletError.Add($"[CustomPackageElement] 보상 설정이 유효하지 않음. groupId:{GroupId} rewardType:{RewardType} rewardId:{RewardId} freeRewardCount:{FreeRewardCount} paidRewardCount:{PaidRewardCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMCustomPackageElement.cs", 57);
		}
		if (RewardType == NKM_REWARD_TYPE.RT_MISC)
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(RewardId);
			if (nKMItemMiscTemplet != null && nKMItemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE)
			{
				NKMTempletError.Add($"[CustomPackageElement] 커스텀패키지에 포함 불가한 항목. groupId:{GroupId} rewardType:{RewardType} rewardId:{RewardId} freeRewardCount:{FreeRewardCount} paidRewardCount:{PaidRewardCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMCustomPackageElement.cs", 65);
			}
			if (RewardId != 101 && RewardId != 102 && PaidRewardCount != 0)
			{
				NKMTempletError.Add($"[CustomPackageElement] 유료 재화 설정 오류.. groupId:{GroupId} rewardType:{RewardType} rewardId:{RewardId} freeRewardCount:{FreeRewardCount} paidRewardCount:{PaidRewardCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMCustomPackageElement.cs", 70);
			}
		}
	}
}
