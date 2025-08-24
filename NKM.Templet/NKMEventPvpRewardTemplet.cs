using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMEventPvpRewardTemplet : INKMTemplet
{
	public enum ResetType
	{
		Season,
		Daily
	}

	public enum PlayCountCondition
	{
		Win,
		Lose,
		Draw,
		Play
	}

	public const int RewardCount = 3;

	private int rewardGroupId;

	private int rewardId;

	private int step;

	private int playTimes;

	private ResetType resetType;

	private PlayCountCondition countCondition = PlayCountCondition.Play;

	private string eventGauntletDescStrID;

	public NKMRewardInfo[] RewardInfos = new NKMRewardInfo[3];

	public int Key => rewardId;

	public int RewardGroupId => rewardGroupId;

	public int RewardId => rewardId;

	public int Step => step;

	public int PlayTimes => playTimes;

	public ResetType Type => resetType;

	public PlayCountCondition CountCondition => countCondition;

	public string DescStrID => eventGauntletDescStrID;

	public static IEnumerable<NKMEventPvpRewardTemplet> Values => NKMTempletContainer<NKMEventPvpRewardTemplet>.Values;

	public static NKMEventPvpRewardTemplet Find(int key)
	{
		return NKMTempletContainer<NKMEventPvpRewardTemplet>.Find(key);
	}

	public static NKMEventPvpRewardTemplet FindStep(int groupId, int step)
	{
		return Values.FirstOrDefault((NKMEventPvpRewardTemplet e) => e.RewardGroupId == groupId && e.Step == step);
	}

	public static NKMEventPvpRewardTemplet LoadFromLua(NKMLua lua)
	{
		NKMEventPvpRewardTemplet nKMEventPvpRewardTemplet = new NKMEventPvpRewardTemplet();
		int num = (int)(1u & (lua.GetData("RewardGroupID", ref nKMEventPvpRewardTemplet.rewardGroupId) ? 1u : 0u) & (lua.GetData("RewardID", ref nKMEventPvpRewardTemplet.rewardId) ? 1u : 0u)) & (lua.GetData("ResetType", ref nKMEventPvpRewardTemplet.resetType) ? 1 : 0);
		if (!lua.GetData("PlayCountCondition", ref nKMEventPvpRewardTemplet.countCondition))
		{
			nKMEventPvpRewardTemplet.countCondition = PlayCountCondition.Play;
		}
		_ = (uint)num & (lua.GetData("Step", ref nKMEventPvpRewardTemplet.step) ? 1u : 0u);
		lua.GetData("PlayTimes", ref nKMEventPvpRewardTemplet.playTimes);
		lua.GetData("EventGauntletDescStrID", ref nKMEventPvpRewardTemplet.eventGauntletDescStrID);
		for (int i = 0; i < nKMEventPvpRewardTemplet.RewardInfos.Length; i++)
		{
			nKMEventPvpRewardTemplet.RewardInfos[i] = new NKMRewardInfo();
			lua.GetData($"EventRewardType_{i + 1}", ref nKMEventPvpRewardTemplet.RewardInfos[i].rewardType);
			lua.GetData($"EventRewardID_{i + 1}", ref nKMEventPvpRewardTemplet.RewardInfos[i].ID);
			lua.GetData($"EventRewardValue_{i + 1}", ref nKMEventPvpRewardTemplet.RewardInfos[i].Count);
			nKMEventPvpRewardTemplet.RewardInfos[i].paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE;
		}
		return nKMEventPvpRewardTemplet;
	}

	public static PlayCountCondition GetPlayCountCondition(PVP_RESULT pvpResult)
	{
		return pvpResult switch
		{
			PVP_RESULT.WIN => PlayCountCondition.Win, 
			PVP_RESULT.LOSE => PlayCountCondition.Lose, 
			PVP_RESULT.DRAW => PlayCountCondition.Draw, 
			_ => PlayCountCondition.Play, 
		};
	}

	public IEnumerable<NKMEventPvpRewardTemplet> FindNextSteps()
	{
		return Values.Where((NKMEventPvpRewardTemplet e) => e.step >= step && e.RewardGroupId == RewardGroupId);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public NKMEventPvpRewardTemplet NextStep()
	{
		int num = step + 1;
		NKMEventPvpRewardTemplet nKMEventPvpRewardTemplet = FindStep(rewardGroupId, num);
		if (nKMEventPvpRewardTemplet == null)
		{
			return this;
		}
		return nKMEventPvpRewardTemplet;
	}
}
