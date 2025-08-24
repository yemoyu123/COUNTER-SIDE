using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMAttendanceRewardTemplet
{
	private int rewardGroup;

	private int loginDate;

	private NKM_REWARD_TYPE rewardType;

	private int rewardID;

	private string rewardStrID;

	private int rewardValue;

	private string mailTitle;

	private string mailDesc;

	public int RewardGroup => rewardGroup;

	public int LoginDate => loginDate;

	public NKM_REWARD_TYPE RewardType => rewardType;

	public int RewardID => rewardID;

	public string RewardStrID => rewardStrID;

	public int RewardValue => rewardValue;

	public string MailTitle => mailTitle;

	public string MailDesc => mailDesc;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		return (byte)(1u & (cNKMLua.GetData("m_RewardGroup", ref rewardGroup) ? 1u : 0u) & (cNKMLua.GetData("m_LoginDate", ref loginDate) ? 1u : 0u) & (cNKMLua.GetData("m_RewardType", ref rewardType) ? 1u : 0u) & (cNKMLua.GetData("m_RewardID", ref rewardID) ? 1u : 0u) & (cNKMLua.GetData("m_RewardStrID", ref rewardStrID) ? 1u : 0u) & (cNKMLua.GetData("m_RewardValue", ref rewardValue) ? 1u : 0u) & (cNKMLua.GetData("m_MailTitle", ref mailTitle) ? 1u : 0u) & (cNKMLua.GetData("m_MailDesc", ref mailDesc) ? 1u : 0u)) != 0;
	}

	public void Validate()
	{
		if (!NKMRewardTemplet.IsValidReward(rewardType, rewardID))
		{
			NKMTempletError.Add($"Invalid reward data. type:{rewardType} id:{rewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 201);
		}
	}
}
