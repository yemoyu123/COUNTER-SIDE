using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMCollectionV2MiscTemplet : INKMTemplet
{
	private int id;

	private string openTag;

	private string miscType;

	private int sortIndex;

	private NKM_REWARD_TYPE collectionItemType;

	private int collectionItemId;

	private bool exclude;

	private NKM_REWARD_TYPE rewardType;

	private int rewardId;

	private int rewardValue;

	private bool defaultCollection;

	public NKM_REWARD_TYPE RewardType => rewardType;

	public int RewardId => rewardId;

	public int RewardValue => rewardValue;

	public bool DefaultCollection => defaultCollection;

	public int Key => collectionItemId;

	public int CollectionItemId => collectionItemId;

	public string MiscType => miscType;

	public int SortIndex => sortIndex;

	public bool Exclude => exclude;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public static IEnumerable<NKMCollectionV2MiscTemplet> Values => NKMTempletContainer<NKMCollectionV2MiscTemplet>.Values;

	public NKMItemMiscTemplet CollectionMiscTemplet { get; private set; }

	public static NKMCollectionV2MiscTemplet Find(int collectionItemId)
	{
		return NKMTempletContainer<NKMCollectionV2MiscTemplet>.Find(collectionItemId);
	}

	public static NKMCollectionV2MiscTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 42))
		{
			return null;
		}
		NKMCollectionV2MiscTemplet nKMCollectionV2MiscTemplet = new NKMCollectionV2MiscTemplet();
		lua.GetData("ID", ref nKMCollectionV2MiscTemplet.id);
		lua.GetData("OpenTag", ref nKMCollectionV2MiscTemplet.openTag);
		lua.GetData("MiscType", ref nKMCollectionV2MiscTemplet.miscType);
		lua.GetDataEnum<NKM_REWARD_TYPE>("CollectionItemType", out nKMCollectionV2MiscTemplet.collectionItemType);
		lua.GetData("CollectionItemID", ref nKMCollectionV2MiscTemplet.collectionItemId);
		lua.GetData("SortIndex", ref nKMCollectionV2MiscTemplet.sortIndex);
		lua.GetData("DefaultCollection", ref nKMCollectionV2MiscTemplet.defaultCollection);
		lua.GetDataEnum<NKM_REWARD_TYPE>("CollectionRewardType", out nKMCollectionV2MiscTemplet.rewardType);
		lua.GetData("CollectionRewardID", ref nKMCollectionV2MiscTemplet.rewardId);
		lua.GetData("CollectionRewardValue", ref nKMCollectionV2MiscTemplet.rewardValue);
		lua.GetData("bExclude", ref nKMCollectionV2MiscTemplet.exclude);
		return nKMCollectionV2MiscTemplet;
	}

	public void Join()
	{
		CollectionMiscTemplet = NKMItemMiscTemplet.Find(CollectionItemId);
		if (CollectionMiscTemplet == null)
		{
			NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] NKMItemMiscTemplet을 찾지 못함. CollectionItemId:{CollectionItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 66);
		}
	}

	public void Validate()
	{
		if (id <= 0)
		{
			NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] ID 값이 비정상. id:{id}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 75);
		}
		if (!NKMOpenTagManager.IsOpened(openTag))
		{
			return;
		}
		if (sortIndex <= 0)
		{
			NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] 정렬 값이 비정상. sortIndex:{sortIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 85);
		}
		if (collectionItemType != NKM_REWARD_TYPE.RT_MISC)
		{
			NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] 수집 아이템의 Type값이 비정상. collectionItemType:{collectionItemType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 90);
		}
		if (collectionItemId <= 0)
		{
			NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] 수집 아이템 Id가 비정상. collectionItemId:{collectionItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 95);
		}
		else if (NKMItemMiscTemplet.Find(collectionItemId) == null)
		{
			NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] 수집 아이템 Templet을 찾지 못함. collectionItemId:{collectionItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 99);
		}
		if (CollectionMiscTemplet.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME && CollectionMiscTemplet.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_EMBLEM && CollectionMiscTemplet.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_EMBLEM_RANK && CollectionMiscTemplet.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_BACKGROUND)
		{
			NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] 수집 아이템의 miscType값이 비정상. itemId:{collectionItemId} ItemMiscType:{CollectionMiscTemplet.m_ItemMiscType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 107);
		}
		if (!DefaultCollection)
		{
			if (!NKMRewardTemplet.IsOpenedReward(RewardType, RewardId, useRandomContract: false))
			{
				NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] Reward 가 오픈되어 있지 않습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 118);
			}
			if (RewardType == NKM_REWARD_TYPE.RT_NONE)
			{
				NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] 보상 아이템 타입이 비정상. rewardType:{RewardType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 123);
			}
			if (RewardId <= 0)
			{
				NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] 보상 아이템 id값이 비정상. rewardId:{RewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 128);
			}
			else if (NKMItemMiscTemplet.Find(RewardId) == null)
			{
				NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] 보상 아이템 Templet을 찾지 못함. rewardId:{RewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 132);
			}
			if (RewardValue <= 0)
			{
				NKMTempletError.Add($"[NKMCollectionV2MiscTemplet:{Key}] 보상 아이템의 개수가 비정상. rewardValue:{RewardValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionV2MiscTemplet.cs", 137);
			}
		}
	}
}
