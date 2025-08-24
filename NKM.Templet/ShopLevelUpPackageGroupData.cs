using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class ShopLevelUpPackageGroupData : INKMTemplet
{
	private const int MaxRewardCount = 3;

	private int m_PackageID;

	private int m_LevelRequire;

	private List<NKMRewardInfo> m_RewardInfos = new List<NKMRewardInfo>();

	private string m_MailTitle;

	private string m_MailDesc;

	public int Key => m_PackageID;

	public int LevelRequire => m_LevelRequire;

	public List<NKMRewardInfo> RewardInfos => m_RewardInfos;

	public string MailTitle => m_MailTitle;

	public string MailDesc => m_MailDesc;

	public static ShopLevelUpPackageGroupData LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopLevelUpPackageGroupData.cs", 27))
		{
			return null;
		}
		ShopLevelUpPackageGroupData shopLevelUpPackageGroupData = new ShopLevelUpPackageGroupData();
		bool flag = true;
		flag &= cNKMLua.GetData("m_PackageID", ref shopLevelUpPackageGroupData.m_PackageID);
		flag &= cNKMLua.GetData("m_LevelRequire", ref shopLevelUpPackageGroupData.m_LevelRequire);
		for (int i = 0; i < 3; i++)
		{
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
			cNKMLua.GetDataEnum<NKM_REWARD_TYPE>($"m_RewardType_{i + 1}", out nKMRewardInfo.rewardType);
			cNKMLua.GetData($"m_RewardID_{i + 1}", ref nKMRewardInfo.ID);
			cNKMLua.GetData($"m_FreeValue_{i + 1}", ref nKMRewardInfo.Count);
			int rValue = 0;
			cNKMLua.GetData($"m_PaidValue_{i + 1}", ref rValue);
			if (nKMRewardInfo.rewardType != NKM_REWARD_TYPE.RT_NONE && nKMRewardInfo.ID > 0)
			{
				if (nKMRewardInfo.Count > 0)
				{
					nKMRewardInfo.paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE;
					shopLevelUpPackageGroupData.m_RewardInfos.Add(nKMRewardInfo);
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
					shopLevelUpPackageGroupData.m_RewardInfos.Add(item);
				}
			}
		}
		flag &= cNKMLua.GetData("m_MailTitle", ref shopLevelUpPackageGroupData.m_MailTitle);
		if (!(flag & cNKMLua.GetData("m_MailDesc", ref shopLevelUpPackageGroupData.m_MailDesc)))
		{
			return null;
		}
		return shopLevelUpPackageGroupData;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_RewardInfos.Count <= 0)
		{
			NKMTempletError.Add($"[ShopLevelUpPackageTemplet] 레벨업 패키지 보상 정보가 하나 이상 존재하지 않음 m_PackageID : {m_PackageID}, m_LevelRequire : {m_LevelRequire}, m_RewardCount : {m_RewardInfos.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopLevelUpPackageGroupData.cs", 83);
		}
		foreach (NKMRewardInfo rewardInfo in m_RewardInfos)
		{
			if (!NKMRewardTemplet.IsValidReward(rewardInfo.rewardType, rewardInfo.ID))
			{
				NKMTempletError.Add($"[ShopLevelUpPackageTemplet] 레벨업 패키지 보상 정보가 존재하지 않음 m_PackageID : {m_PackageID}, m_LevelRequire : {m_LevelRequire}, m_RewardType : {rewardInfo.rewardType}, m_RewardID : {rewardInfo.ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopLevelUpPackageGroupData.cs", 90);
			}
			else if (rewardInfo.rewardType == NKM_REWARD_TYPE.RT_MISC && rewardInfo.ID != 101 && rewardInfo.ID != 102 && rewardInfo.paymentType == NKM_ITEM_PAYMENT_TYPE.NIPT_PAID)
			{
				NKMTempletError.Add($"[ShopLevelUpPackageTemplet] 유료 재화 설정 오류. m_RewardType:{rewardInfo.rewardType} m_RewardID:{rewardInfo.ID} m_PaidRewardValue", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopLevelUpPackageGroupData.cs", 99);
			}
		}
	}
}
