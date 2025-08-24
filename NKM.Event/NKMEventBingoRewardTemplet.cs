using System.Collections.Generic;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Event;

public class NKMEventBingoRewardTemplet : INKMTemplet
{
	public int m_index;

	public int m_BingoCompletRewardGroupID;

	public BingoCompleteType m_BingoCompletType;

	public int m_BingoCompletTypeValue;

	public List<NKMRewardInfo> rewards = new List<NKMRewardInfo>();

	public int Key => m_BingoCompletRewardGroupID;

	public int ZeroBaseTileIndex => m_index - 1;

	public static NKMEventBingoRewardTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBingoTemplet.cs", 116))
		{
			return null;
		}
		NKMEventBingoRewardTemplet nKMEventBingoRewardTemplet = new NKMEventBingoRewardTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_index", ref nKMEventBingoRewardTemplet.m_index);
		flag &= cNKMLua.GetData("m_BingoCompletRewardGroupID", ref nKMEventBingoRewardTemplet.m_BingoCompletRewardGroupID);
		flag &= cNKMLua.GetData("m_BingoCompletType", ref nKMEventBingoRewardTemplet.m_BingoCompletType);
		flag &= cNKMLua.GetData("m_BingoCompletTypeValue", ref nKMEventBingoRewardTemplet.m_BingoCompletTypeValue);
		for (int i = 1; i <= 3; i++)
		{
			NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
			int rValue = 0;
			int rValue2 = 0;
			cNKMLua.GetData($"m_BingoCompletRewardType_{i}", ref result);
			cNKMLua.GetData($"m_BingoCompletRewardID_{i}", ref rValue);
			cNKMLua.GetData($"m_BingoCompletRewardValue_{i}", ref rValue2);
			if (rValue != 0)
			{
				nKMEventBingoRewardTemplet.rewards.Add(new NKMRewardInfo
				{
					rewardType = result,
					ID = rValue,
					Count = rValue2,
					paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
				});
			}
		}
		if (!flag)
		{
			return null;
		}
		return nKMEventBingoRewardTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_BingoCompletType == BingoCompleteType.LINE_SINGLE && rewards.Count > 1)
		{
			NKMTempletError.Add($"빙고이벤트 LINE_SINGLE 보상은 2개 이상이 될 수 없음. Group:{Key} Index:{m_index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBingoTemplet.cs", 163);
		}
	}
}
