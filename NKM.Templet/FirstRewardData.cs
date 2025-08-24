using Cs.Logging;

namespace NKM.Templet;

public sealed class FirstRewardData
{
	public static readonly FirstRewardData Empty = new FirstRewardData();

	private NKM_REWARD_TYPE type;

	private int rewardId;

	private string strId;

	private int rewardQuantity;

	public NKM_REWARD_TYPE Type => type;

	public int RewardId => rewardId;

	public string StrId => strId;

	public int RewardQuantity => rewardQuantity;

	public static FirstRewardData Load(NKMLua lua)
	{
		FirstRewardData firstRewardData = new FirstRewardData();
		lua.GetData("m_FirstReward_Type", ref firstRewardData.type);
		lua.GetData("m_FirstReward_ID", ref firstRewardData.rewardId);
		lua.GetData("m_FirstReward_StrID", out firstRewardData.strId, string.Empty);
		lua.GetData("m_FirstRewardQuantity", ref firstRewardData.rewardQuantity);
		return firstRewardData;
	}

	public void Validate()
	{
		if (Type != NKM_REWARD_TYPE.RT_NONE && RewardId > 0 && !NKMRewardTemplet.IsValidReward(Type, RewardId))
		{
			Log.ErrorAndExit($"[WarfareTemplet] 첫 클리어 보상 정보가 존재하지 않음 m_FirstReward_Type:{Type} m_FirstReward_ID:{RewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 103);
		}
	}
}
