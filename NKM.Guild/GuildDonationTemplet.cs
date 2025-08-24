using System.Collections.Generic;
using NKM.Contract2;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildDonationTemplet : INKMTemplet
{
	public int ID;

	public string DonateImgFileName;

	public string DonateText;

	public MiscItemUnit reqItemUnit;

	public long RewardGuildExp;

	public long RewardUnionPoint;

	public readonly List<DonationReward> m_DonationReward = new List<DonationReward>();

	public int Key => ID;

	public static GuildDonationTemplet Find(int key)
	{
		return NKMTempletContainer<GuildDonationTemplet>.Find(key);
	}

	public static GuildDonationTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 33))
		{
			return null;
		}
		GuildDonationTemplet guildDonationTemplet = new GuildDonationTemplet();
		lua.GetData("ID", ref guildDonationTemplet.ID);
		lua.GetData("m_DonateImgFileName", ref guildDonationTemplet.DonateImgFileName);
		lua.GetData("m_DonateText", ref guildDonationTemplet.DonateText);
		guildDonationTemplet.reqItemUnit = new MiscItemUnit(lua.GetInt32("m_DonateRequireItemID"), lua.GetInt64("m_DonateRequireItemValue"));
		int num = 0;
		while (true)
		{
			DonationReward donationReward = new DonationReward();
			lua.GetData($"m_RewardID_{num + 1}", ref donationReward.RewardID);
			lua.GetData($"m_RewardValue_{num + 1}", ref donationReward.RewardValue);
			if (!lua.GetData($"m_RewardType_{num + 1}", ref donationReward.RewardType))
			{
				break;
			}
			guildDonationTemplet.m_DonationReward.Add(donationReward);
			num++;
		}
		return guildDonationTemplet;
	}

	public void Join()
	{
		reqItemUnit.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 66);
		DonationReward donationReward = m_DonationReward.Find((DonationReward e) => e.RewardID == 24);
		if (donationReward == null)
		{
			NKMTempletError.Add($"[GuildDonation] 연합포인트 보상 누락. donationId:{ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 71);
		}
		else
		{
			RewardUnionPoint = donationReward.RewardValue;
		}
		DonationReward donationReward2 = m_DonationReward.Find((DonationReward e) => e.RewardID == 503);
		if (donationReward2 == null)
		{
			NKMTempletError.Add($"[GuildDonation] 길드경험치 보상 누락. donationId:{ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 81);
		}
		else
		{
			RewardGuildExp = donationReward2.RewardValue;
		}
	}

	public void Validate()
	{
		if (ID <= 0)
		{
			NKMTempletError.Add($"[Guild] invalid DonateID :{ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 93);
		}
		if (RewardGuildExp <= 0)
		{
			NKMTempletError.Add($"[Guild] invalid RewardGuildExp:{RewardGuildExp}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 98);
		}
		if (RewardUnionPoint <= 0)
		{
			NKMTempletError.Add($"[Guild] invalid RewardUnionPoint:{RewardUnionPoint}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 103);
		}
		if (m_DonationReward.Count <= 0)
		{
			NKMTempletError.Add($"[Guild] invalid RewardCount:{m_DonationReward.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 108);
		}
		for (int i = 0; i < m_DonationReward.Count; i++)
		{
			DonationReward donationReward = m_DonationReward[i];
			if (!NKMRewardTemplet.IsValidReward(donationReward.RewardType, donationReward.RewardID))
			{
				NKMTempletError.Add($"[Guild] invalid Reward. type:{donationReward.RewardType} id:{donationReward.RewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 117);
			}
			if (donationReward.RewardID <= 0)
			{
				NKMTempletError.Add($"[Guild] invalid RewardID:{donationReward.RewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 122);
			}
			if (donationReward.RewardValue <= 0)
			{
				NKMTempletError.Add($"[Guild] RewardValue:{donationReward.RewardValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDonationTemplet.cs", 127);
			}
		}
	}
}
