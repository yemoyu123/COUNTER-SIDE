using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.EventPass;

public sealed class NKMEventPassRewardTemplet : INKMTemplet
{
	private static Dictionary<int, List<NKMEventPassRewardTemplet>> rewardGroupTemplet = new Dictionary<int, List<NKMEventPassRewardTemplet>>();

	private static int index;

	private int key;

	public int Key => key;

	public int PassRewardGroupId { get; private set; }

	public int PassLevel { get; private set; }

	public NKM_REWARD_TYPE NormalRewardItemType { get; private set; }

	public int NormalRewardItemId { get; private set; }

	public int NormalRewardItemCount { get; private set; }

	public NKM_REWARD_TYPE CoreRewardItemType { get; private set; }

	public int CoreRewardItemId { get; private set; }

	public int CoreRewardItemCount { get; private set; }

	public static NKMEventPassRewardTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 27))
		{
			return null;
		}
		int rValue = 0;
		bool data = cNKMLua.GetData("PassRewardGroupID", ref rValue);
		int rValue2 = 0;
		bool num = data & cNKMLua.GetData("PassLevel", ref rValue2);
		NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
		bool num2 = num & cNKMLua.GetData("NormalRewardItemType", ref result);
		int rValue3 = 0;
		bool num3 = num2 & cNKMLua.GetData("NormalRewardItemID", ref rValue3);
		int rValue4 = 0;
		bool num4 = num3 & cNKMLua.GetData("NormalRewardItemCount", ref rValue4);
		NKM_REWARD_TYPE result2 = NKM_REWARD_TYPE.RT_NONE;
		bool num5 = num4 & cNKMLua.GetData("CoreRewardItemType", ref result2);
		int rValue5 = 0;
		bool num6 = num5 & cNKMLua.GetData("CoreRewardItemID", ref rValue5);
		int rValue6 = 0;
		if (!(num6 & cNKMLua.GetData("CoreRewardItemCount", ref rValue6)))
		{
			Log.ErrorAndExit($"[EventPassRewardTemplet] data is invalid, passRewardGroupID: {rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 56);
			return null;
		}
		NKMEventPassRewardTemplet nKMEventPassRewardTemplet = new NKMEventPassRewardTemplet
		{
			key = ++index,
			PassLevel = rValue2,
			PassRewardGroupId = rValue,
			NormalRewardItemType = result,
			NormalRewardItemId = rValue3,
			NormalRewardItemCount = rValue4,
			CoreRewardItemType = result2,
			CoreRewardItemId = rValue5,
			CoreRewardItemCount = rValue6
		};
		if (!rewardGroupTemplet.ContainsKey(nKMEventPassRewardTemplet.PassRewardGroupId))
		{
			rewardGroupTemplet.Add(nKMEventPassRewardTemplet.PassRewardGroupId, new List<NKMEventPassRewardTemplet>());
		}
		rewardGroupTemplet[nKMEventPassRewardTemplet.PassRewardGroupId].Add(nKMEventPassRewardTemplet);
		return nKMEventPassRewardTemplet;
	}

	public static List<NKMEventPassRewardTemplet> GetRewardGroupTemplet(int groupId)
	{
		if (!rewardGroupTemplet.ContainsKey(groupId))
		{
			return null;
		}
		return rewardGroupTemplet[groupId];
	}

	public static NKMEventPassRewardTemplet GetRewardTemplet(int groupId, int passLevel)
	{
		List<NKMEventPassRewardTemplet> list = GetRewardGroupTemplet(groupId);
		if (list == null)
		{
			return null;
		}
		int num = passLevel - 1;
		if (num < 0)
		{
			return null;
		}
		if (list.Count <= num)
		{
			return null;
		}
		return list[num];
	}

	public static IEnumerable<NKMEventPassRewardTemplet> GetRangeOfRewardTemplet(int groupId, int fromLevel, int toLevel)
	{
		List<NKMEventPassRewardTemplet> list = GetRewardGroupTemplet(groupId);
		if (list == null)
		{
			yield break;
		}
		foreach (NKMEventPassRewardTemplet item in list)
		{
			if (item.PassLevel >= fromLevel)
			{
				if (item.PassLevel > toLevel)
				{
					yield break;
				}
				yield return item;
			}
		}
	}

	public static void Drop()
	{
		rewardGroupTemplet.Clear();
	}

	public void Join()
	{
	}

	public void Validate()
	{
		switch (NormalRewardItemType)
		{
		case NKM_REWARD_TYPE.RT_MISC:
			if (NKMItemManager.GetItemMiscTempletByID(NormalRewardItemId) == null)
			{
				NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid NormalRewardItemId. inputType:{NormalRewardItemType} InputItemId:{NormalRewardItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 159);
			}
			break;
		case NKM_REWARD_TYPE.RT_SKIN:
			if (NKMSkinManager.GetSkinTemplet(NormalRewardItemId) == null)
			{
				NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid NormalRewardItemId. inputType:{NormalRewardItemType} InputItemId:{NormalRewardItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 167);
			}
			if (NormalRewardItemCount > 1)
			{
				NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid NormalRewardItemCount. inputType:{NormalRewardItemType} inputId:{NormalRewardItemId} inputCount:{NormalRewardItemCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 172);
			}
			break;
		case NKM_REWARD_TYPE.RT_UNIT:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(NormalRewardItemId);
			if (unitTempletBase == null || unitTempletBase.m_bMonster || !unitTempletBase.CollectionEnableByTag)
			{
				NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid NormalRewardItemId. inputType:{NormalRewardItemType} InputItemId:{NormalRewardItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 182);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_EQUIP:
			if (NKMItemManager.GetEquipTemplet(NormalRewardItemId) == null)
			{
				NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid NormalRewardItemId. inputType:{NormalRewardItemType} InputItemId:{NormalRewardItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 190);
			}
			break;
		default:
			NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid NormalRewardItemType. inputType:{NormalRewardItemType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 195);
			break;
		}
		switch (CoreRewardItemType)
		{
		case NKM_REWARD_TYPE.RT_MISC:
			if (NKMItemManager.GetItemMiscTempletByID(CoreRewardItemId) == null)
			{
				NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid CoreRewardItemId. inputType:{CoreRewardItemType} InputItemId:{CoreRewardItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 206);
			}
			break;
		case NKM_REWARD_TYPE.RT_SKIN:
			if (NKMSkinManager.GetSkinTemplet(CoreRewardItemId) == null)
			{
				NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid CoreRewardItemId. inputType:{CoreRewardItemType} InputItemId:{CoreRewardItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 214);
			}
			if (CoreRewardItemCount > 1)
			{
				NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid CoreRewardItemCount. inputType:{CoreRewardItemType} InputItemId:{CoreRewardItemId} inputCount:{CoreRewardItemCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 219);
			}
			break;
		case NKM_REWARD_TYPE.RT_EQUIP:
			if (NKMItemManager.GetEquipTemplet(CoreRewardItemId) == null)
			{
				NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid CoreRewardItemId. inputType:{CoreRewardItemType} InputItemId:{CoreRewardItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 236);
			}
			break;
		case NKM_REWARD_TYPE.RT_EMOTICON:
			if (NKMEmoticonTemplet.Find(CoreRewardItemId) == null)
			{
				NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid CoreRewardItemId. inputType:{CoreRewardItemType} InputItemId:{CoreRewardItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 244);
			}
			break;
		default:
			NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid CoreRewardItemType. inputType:{CoreRewardItemType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 248);
			break;
		case NKM_REWARD_TYPE.RT_UNIT:
			break;
		}
		if (NormalRewardItemCount <= 0)
		{
			NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid NormalRewardItemCount. inputCount:{NormalRewardItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 255);
		}
		if (CoreRewardItemCount <= 0)
		{
			NKMTempletError.Add($"[NKMEventPassRewardTemplet] Invalid CoreRewardItemCount. inputCount:{CoreRewardItemCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassRewardTemplet.cs", 261);
		}
	}
}
