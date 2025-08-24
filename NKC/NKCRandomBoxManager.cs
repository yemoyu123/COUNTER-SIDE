using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKC;

public class NKCRandomBoxManager
{
	public static Dictionary<int, List<NKMRandomBoxItemTemplet>> _dic;

	public static bool LoadFromLUA()
	{
		_dic = NKMTempletLoader<NKMRandomBoxItemTemplet>.LoadGroup("ab_script", "LUA_RANDOM_ITEM_BOX", "RANDOM_ITEM_BOX", NKMRandomBoxItemTemplet.LoadFromLUA);
		return _dic != null;
	}

	public static List<NKMRandomBoxItemTemplet> GetRandomBoxItemTempletList(int groupID)
	{
		if (!_dic.TryGetValue(groupID, out var value))
		{
			return null;
		}
		List<NKMRandomBoxItemTemplet> list = null;
		foreach (NKMRandomBoxItemTemplet item in value)
		{
			if (!item.EnableByTag)
			{
				continue;
			}
			if (item.m_RewardGroupID == 33028 || item.m_RewardGroupID == 33027)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item.m_RewardID);
				if (unitTempletBase != null && (unitTempletBase.PickupEnableByTag || unitTempletBase.ContractEnableByTag))
				{
					if (list == null)
					{
						list = new List<NKMRandomBoxItemTemplet>();
					}
					list.Add(item);
				}
			}
			else if (NKMRewardTemplet.IsOpenedReward(item.m_reward_type, item.m_RewardID, useRandomContract: true))
			{
				if (list == null)
				{
					list = new List<NKMRandomBoxItemTemplet>();
				}
				list.Add(item);
			}
		}
		return list;
	}
}
