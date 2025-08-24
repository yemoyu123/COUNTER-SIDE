using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Event;

public sealed class NKMEventWechatCouponTemplet : INKMTemplet
{
	private const int MaxRewardCount = 3;

	private readonly List<NKMRewardInfo> rewardList = new List<NKMRewardInfo>();

	public int Key => EventId;

	public int EventId { get; private set; }

	public string ZlongCardCode { get; private set; }

	public int ZlongActivityInstanceId { get; private set; }

	public IReadOnlyList<NKMRewardInfo> RewardList => rewardList;

	public NKMEventTabTemplet EventTabTemplet { get; private set; }

	public static NKMEventWechatCouponTemplet LoadFromLua(NKMLua lua)
	{
		NKMEventWechatCouponTemplet nKMEventWechatCouponTemplet = new NKMEventWechatCouponTemplet
		{
			EventId = lua.GetInt32("m_EventID"),
			ZlongCardCode = lua.GetString("Zlong_CardCode"),
			ZlongActivityInstanceId = lua.GetInt32("Zlong_ActivityInstanceId")
		};
		for (int i = 0; i < 3; i++)
		{
			int num = i + 1;
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo
			{
				paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
			};
			if ((1u & (lua.GetData($"RewardType_{num}", ref nKMRewardInfo.rewardType) ? 1u : 0u) & (lua.GetData($"RewardId_{num}", ref nKMRewardInfo.ID) ? 1u : 0u) & (lua.GetData($"RewardValue_{num}", ref nKMRewardInfo.Count) ? 1u : 0u)) != 0 && nKMRewardInfo.Count > 0)
			{
				nKMEventWechatCouponTemplet.rewardList.Add(nKMRewardInfo);
			}
		}
		return nKMEventWechatCouponTemplet;
	}

	public static NKMEventWechatCouponTemplet Find(int eventId)
	{
		return NKMTempletContainer<NKMEventWechatCouponTemplet>.Find(eventId);
	}

	public void Join()
	{
		EventTabTemplet = NKMEventTabTemplet.Find(EventId);
		if (EventTabTemplet == null)
		{
			NKMTempletError.Add($"[ZlongCouponTemplet:{Key}] 연결된 EventTabTemplet을 찾을 수 없음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventWechatCouponTemplet.cs", 56);
		}
	}

	public void Validate()
	{
		if (!rewardList.Any())
		{
			NKMTempletError.Add($"[ZlongCouponTemplet:{Key}] 보상 데이터가 비어있음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventWechatCouponTemplet.cs", 64);
		}
		foreach (NKMRewardInfo reward in rewardList)
		{
			if (!NKMRewardTemplet.IsValidReward(reward.rewardType, reward.ID))
			{
				NKMTempletError.Add($"[ZlongCouponTemplet:{Key}] 보상 정보가 올바르지 않음:{reward.DebugName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventWechatCouponTemplet.cs", 71);
			}
			if (!NKMRewardTemplet.IsOpenedReward(reward.rewardType, reward.ID, useRandomContract: false))
			{
				NKMTempletError.Add($"[ZlongCouponTemplet:{Key}] 쿠폰 보상이 태그에 의해 비활성 처리됨:{reward.DebugName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventWechatCouponTemplet.cs", 76);
			}
		}
	}
}
