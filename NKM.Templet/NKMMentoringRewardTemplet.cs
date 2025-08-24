using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMMentoringRewardTemplet : INKMTemplet
{
	private static Dictionary<int, List<NKMMentoringRewardTemplet>> RewardGroupTemplet = new Dictionary<int, List<NKMMentoringRewardTemplet>>();

	public int RewardGroupId { get; private set; }

	public NKM_REWARD_TYPE RewardType { get; private set; }

	public int RewardId { get; private set; }

	public int RewardCount { get; private set; }

	public int InviteSuccessRequireCnt { get; private set; }

	public int Key => GetHashCode();

	public static NKMMentoringRewardTemplet GetRewardTempletBy(int rewardGroupId, int inviteRequireCount)
	{
		if (!RewardGroupTemplet.TryGetValue(rewardGroupId, out var value))
		{
			return null;
		}
		foreach (NKMMentoringRewardTemplet item in value)
		{
			if (item.InviteSuccessRequireCnt == inviteRequireCount)
			{
				return item;
			}
		}
		return null;
	}

	public static IReadOnlyList<NKMMentoringRewardTemplet> GetRewardGroupList(int rewardGroupId)
	{
		RewardGroupTemplet.TryGetValue(rewardGroupId, out var value);
		return value;
	}

	public static NKMMentoringRewardTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringRewardTemplet.cs", 45))
		{
			return null;
		}
		int rValue = 0;
		bool data = lua.GetData("m_RewardGroupID", ref rValue);
		NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
		bool num = data & lua.GetData("m_RewardType", ref result);
		int rValue2 = 0;
		bool num2 = num & lua.GetData("m_RewardID", ref rValue2);
		int rValue3 = 0;
		bool num3 = num2 & lua.GetData("m_RewardValue", ref rValue3);
		int rValue4 = 0;
		if (!(num3 & lua.GetData("m_InviteSuccessRequireCnt", ref rValue4)))
		{
			return null;
		}
		NKMMentoringRewardTemplet nKMMentoringRewardTemplet = new NKMMentoringRewardTemplet
		{
			RewardGroupId = rValue,
			RewardType = result,
			RewardId = rValue2,
			RewardCount = rValue3,
			InviteSuccessRequireCnt = rValue4
		};
		if (!RewardGroupTemplet.ContainsKey(nKMMentoringRewardTemplet.RewardGroupId))
		{
			RewardGroupTemplet.Add(nKMMentoringRewardTemplet.RewardGroupId, new List<NKMMentoringRewardTemplet>());
		}
		RewardGroupTemplet[nKMMentoringRewardTemplet.RewardGroupId].Add(nKMMentoringRewardTemplet);
		return nKMMentoringRewardTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (RewardCount <= 0)
		{
			Log.ErrorAndExit($"해당 reward count가 유효하지 않습니다. reward group id: {RewardGroupId}, id: {RewardId}, count: {RewardCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringRewardTemplet.cs", 97);
		}
		else if (RewardType == NKM_REWARD_TYPE.RT_MISC)
		{
			if (NKMItemManager.GetItemMiscTempletByID(RewardId) == null)
			{
				Log.ErrorAndExit($"해당 reward id을 MiscTemplet에서 찾을 수 없습니다. reward type: {RewardType}, reward id: {RewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringRewardTemplet.cs", 106);
			}
		}
		else if (RewardType == NKM_REWARD_TYPE.RT_BUFF)
		{
			if (NKMCompanyBuffTemplet.Find(RewardId) == null)
			{
				Log.ErrorAndExit($"해당 reward id을 CompanyBuffTemplet에서 찾을 수 없습니다. reward type: {RewardType}, reward id: {RewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringRewardTemplet.cs", 115);
			}
		}
		else if (RewardType == NKM_REWARD_TYPE.RT_EQUIP)
		{
			if (NKMItemManager.GetEquipTemplet(RewardId) == null)
			{
				Log.ErrorAndExit($"해당 reward id을 EquipTemplet에서 찾을 수 없습니다. reward type: {RewardType}, reward id: {RewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringRewardTemplet.cs", 124);
			}
		}
		else if (RewardType == NKM_REWARD_TYPE.RT_MOLD && NKMItemManager.GetItemMoldTempletByID(RewardId) == null)
		{
			Log.ErrorAndExit($"해당 reward id을 MoldTemplet에서 찾을 수 없습니다. reward type: {RewardType}, reward id: {RewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMMentoringRewardTemplet.cs", 133);
		}
	}
}
