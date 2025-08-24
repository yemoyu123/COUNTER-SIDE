using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class ConsumerPackageGroupData : INKMTemplet
{
	private const int MaxRewardCount = 3;

	private int m_PackageId;

	private int consumeRequireItemId;

	private long consumeRequireItemValue;

	private List<NKMRewardInfo> m_RewardInfos = new List<NKMRewardInfo>();

	private string m_MailTitle;

	private string m_MailDesc;

	public int Key => m_PackageId;

	public int ConsumeRequireItemId => consumeRequireItemId;

	public long ConsumeRequireItemValue => consumeRequireItemValue;

	public List<NKMRewardInfo> RewardInfos => m_RewardInfos;

	public string MailTitle => m_MailTitle;

	public string MailDesc => m_MailDesc;

	public static ConsumerPackageGroupData LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupData.cs", 29))
		{
			return null;
		}
		ConsumerPackageGroupData consumerPackageGroupData = new ConsumerPackageGroupData
		{
			m_PackageId = lua.GetInt32("m_PackageID"),
			consumeRequireItemId = lua.GetInt32("ConsumeRequireItemID"),
			consumeRequireItemValue = lua.GetInt64("ConsumeRequireItemValue"),
			m_MailTitle = lua.GetString("m_MailTitle"),
			m_MailDesc = lua.GetString("m_MailDesc")
		};
		for (int i = 0; i < 3; i++)
		{
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
			lua.GetDataEnum<NKM_REWARD_TYPE>($"m_RewardType_{i + 1}", out nKMRewardInfo.rewardType);
			lua.GetData($"m_RewardID_{i + 1}", ref nKMRewardInfo.ID);
			lua.GetData($"m_RewardValue_{i + 1}", ref nKMRewardInfo.Count);
			int rValue = 0;
			lua.GetData($"m_PaidValue_{i + 1}", ref rValue);
			if (nKMRewardInfo.rewardType != NKM_REWARD_TYPE.RT_NONE && nKMRewardInfo.ID > 0)
			{
				if (nKMRewardInfo.Count > 0)
				{
					nKMRewardInfo.paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE;
					consumerPackageGroupData.m_RewardInfos.Add(nKMRewardInfo);
				}
				if (rValue > 0)
				{
					NKMRewardInfo item = new NKMRewardInfo
					{
						rewardType = nKMRewardInfo.rewardType,
						ID = nKMRewardInfo.ID,
						Count = rValue,
						paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_PAID
					};
					consumerPackageGroupData.m_RewardInfos.Add(item);
				}
			}
		}
		return consumerPackageGroupData;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_RewardInfos.Count <= 0)
		{
			NKMTempletError.Add($"[ConsumerPackageTemplet:{m_PackageId}] 패키지 보상 정보가 하나 이상 존재하지 않음 m_RewardCount:{m_RewardInfos.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupData.cs", 86);
		}
		if (NKMItemManager.GetItemMiscTempletByID(consumeRequireItemId) == null)
		{
			NKMTempletError.Add($"[ItemEquipUpgradeTemple:{m_PackageId}] 소비 아이템 정보가 없음. m_ConsumeRequireItemID: {consumeRequireItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupData.cs", 91);
		}
		if (consumeRequireItemValue < 0)
		{
			NKMTempletError.Add($"[ItemEquipUpgradeTemple:{m_PackageId}] 소비 아이템 목표 개수가 비정상. m_ConsumeRequireItemValue: {consumeRequireItemValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupData.cs", 96);
		}
		foreach (NKMRewardInfo rewardInfo in m_RewardInfos)
		{
			if (!NKMRewardTemplet.IsValidReward(rewardInfo.rewardType, rewardInfo.ID))
			{
				NKMTempletError.Add($"[ConsumerPackageTemplet:{m_PackageId}] 패키지 보상 정보가 존재하지 않음 m_RewardType:{rewardInfo.rewardType}, m_RewardID:{rewardInfo.ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupData.cs", 103);
			}
			else if (rewardInfo.rewardType == NKM_REWARD_TYPE.RT_MISC && rewardInfo.ID != 101 && rewardInfo.ID != 102 && rewardInfo.paymentType == NKM_ITEM_PAYMENT_TYPE.NIPT_PAID)
			{
				NKMTempletError.Add($"[ConsumerPackageTemplet:{m_PackageId}] 쿼츠, 주화 외에 유료 보상이 설정되어 있음. m_RewardType:{rewardInfo.rewardType} m_RewardID:{rewardInfo.ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupData.cs", 112);
			}
			if (rewardInfo.Count <= 0)
			{
				NKMTempletError.Add($"[ConsumerPackageTemplet:{m_PackageId}] 올바르지 못한 보상 개수 설정. Id:{rewardInfo.ID} Count:{rewardInfo.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupData.cs", 118);
			}
		}
	}
}
