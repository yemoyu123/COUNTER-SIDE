using Cs.Core.Util;
using NKM.Templet.Base;

namespace NKM.Templet;

public static class NKMRewardMultiplyTemplet
{
	public enum ScopeType
	{
		General,
		ShadowPalace
	}

	public class RewardMultiplyItem
	{
		public int MiscItemId { get; set; }

		public int MiscItemCount { get; set; }

		public int GetMultiplyItemCount(int rewardMultiply)
		{
			if (rewardMultiply <= 1)
			{
				return MiscItemCount;
			}
			return MiscItemCount * (rewardMultiply - 1);
		}
	}

	private static RewardMultiplyItem[] items = new RewardMultiplyItem[2];

	public static RewardMultiplyItem GetCostItem(ScopeType scopeType)
	{
		return items[(int)scopeType];
	}

	public static bool LoadFromLUA(NKMLua lua, ScopeType scopeType)
	{
		string text = scopeType.ToString();
		using (lua.OpenTable(text, "lua rewardMultiply:" + text + " loading failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRewardMultiplyTemplet.cs", 39))
		{
			int rValue = 0;
			bool data = lua.GetData("MiscItemId", ref rValue);
			int rValue2 = 0;
			if (!(data & lua.GetData("MiscItemCount", ref rValue2)))
			{
				return false;
			}
			items[(int)scopeType] = new RewardMultiplyItem
			{
				MiscItemId = rValue,
				MiscItemCount = rValue2
			};
		}
		return true;
	}

	public static void Validate()
	{
		foreach (ScopeType value in EnumUtil<ScopeType>.GetValues())
		{
			if (NKMItemManager.GetItemMiscTempletByID(items[(int)value].MiscItemId) == null)
			{
				NKMTempletError.Add($"[NKMRewardMultiplyTemplet:{value}] 중첩에 필요한 재화의 종류가 잘못됨. miscItemId:{items[(int)value].MiscItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRewardMultiplyTemplet.cs", 68);
			}
			if (items[(int)value].MiscItemCount != 0)
			{
				NKMTempletError.Add($"[NKMRewardMultiplyTemplet:{value}] 중첩에 필요한 재화의 개수가 0이 아님. miscItemCount:{items[(int)value].MiscItemCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRewardMultiplyTemplet.cs", 73);
			}
		}
	}
}
