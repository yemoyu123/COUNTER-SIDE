using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMADTemplet : INKMTemplet
{
	private static readonly string internalOpenTag = "AD_REWARD";

	private int idx;

	private string openTag;

	private string adRewardStrId;

	private int adItemId;

	private int dayLimit;

	private int watchCoolTime;

	private NKM_REWARD_TYPE rewardItemType;

	private int rewardItemId;

	private int rewarditemValue;

	public int Idx => idx;

	public string OpenTag => openTag;

	public string AdRewardStrId => adRewardStrId;

	public int AdItemId => adItemId;

	public int DayLimit => dayLimit;

	public int WatchCoolTime => watchCoolTime;

	public NKM_REWARD_TYPE RewardItemType => rewardItemType;

	public int RewardItemId => rewardItemId;

	public int RewarditemValue => rewarditemValue;

	public int Key => AdItemId;

	public NKMItemMiscTemplet AdItemTemplet { get; private set; }

	public static bool EnableByTag => NKMOpenTagManager.IsOpened(internalOpenTag);

	public static NKMADTemplet Find(int key)
	{
		return NKMTempletContainer<NKMADTemplet>.Find((NKMADTemplet x) => x.Key == key);
	}

	public static NKMADTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMADTemplet.cs", 38))
		{
			return null;
		}
		NKMADTemplet nKMADTemplet = new NKMADTemplet();
		if ((1u & (lua.GetData("IDX", ref nKMADTemplet.idx) ? 1u : 0u) & (lua.GetData("OpenTag", ref nKMADTemplet.openTag) ? 1u : 0u) & (lua.GetData("AdRewardStrID", ref nKMADTemplet.adRewardStrId) ? 1u : 0u) & (lua.GetData("AdItemID", ref nKMADTemplet.adItemId) ? 1u : 0u) & (lua.GetData("DayLimit", ref nKMADTemplet.dayLimit) ? 1u : 0u) & (lua.GetData("WatchCoolTime", ref nKMADTemplet.watchCoolTime) ? 1u : 0u) & (lua.GetData("RewardItemType", ref nKMADTemplet.rewardItemType) ? 1u : 0u) & (lua.GetData("RewardItemID", ref nKMADTemplet.rewardItemId) ? 1u : 0u) & (lua.GetData("RewardItemValue", ref nKMADTemplet.rewarditemValue) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMADTemplet;
	}

	public void Join()
	{
		AdItemTemplet = NKMItemManager.GetItemMiscTempletByID(AdItemId);
		if (AdItemTemplet == null)
		{
			NKMTempletError.Add($"[NKMADTemplet] AdItemId 의 NKMItemMiscTemplet 없음. AdItemId:{AdItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMADTemplet.cs", 65);
		}
	}

	public void Validate()
	{
		if (!NKMRewardTemplet.IsValidReward(RewardItemType, RewardItemId))
		{
			NKMTempletError.Add($"[NKMADTemplet] AD 보상 정보가 없음. AdItemId:{AdItemId}, RewardItemType:{RewardItemType}, RewardItemId:{RewardItemId} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMADTemplet.cs", 73);
		}
	}
}
