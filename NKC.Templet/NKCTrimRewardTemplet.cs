using System.Collections.Generic;
using NKM;
using NKM.Templet;

namespace NKC.Templet;

public class NKCTrimRewardTemplet
{
	public static readonly Dictionary<int, Dictionary<int, NKCTrimRewardTemplet>> TrimRewardList = new Dictionary<int, Dictionary<int, NKCTrimRewardTemplet>>();

	private int trimID;

	private int trimLevel;

	private NKM_REWARD_TYPE firstClearRewardType;

	private int firstClearRewardID;

	private int firstClearValue;

	private int rewardCountPoint;

	private NKM_REWARD_TYPE fixRewardType;

	private int fixRewardID;

	private int rewardCreditMin;

	private List<int> rewardGroupID = new List<int>();

	private List<int> rewardUnitExp = new List<int>();

	private int rewardUserExp;

	private List<int> eventDropIndex = new List<int>();

	public NKM_REWARD_TYPE FirstClearRewardType => firstClearRewardType;

	public int FirstClearRewardID => firstClearRewardID;

	public int FirstClearValue => firstClearValue;

	public NKM_REWARD_TYPE FixRewardType => fixRewardType;

	public int FixRewardID => fixRewardID;

	public int RewardCreditMin => rewardCreditMin;

	public List<int> RewardGroupID => rewardGroupID;

	public List<int> RewardUnitExp => rewardUnitExp;

	public int RewardUserExp => rewardUserExp;

	public List<int> EventDropIndex => eventDropIndex;

	public static NKCTrimRewardTemplet Find(int trimId, int trimLevel)
	{
		if (!TrimRewardList.ContainsKey(trimId))
		{
			return null;
		}
		if (!TrimRewardList[trimId].ContainsKey(trimLevel))
		{
			return null;
		}
		return TrimRewardList[trimId][trimLevel];
	}

	public static bool Load(string assetName, string fileName)
	{
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath(assetName, fileName))
			{
				return false;
			}
			if (!nKMLua.OpenTable("TRIM_REWARD_TEMPLET"))
			{
				return false;
			}
			int num = 1;
			while (nKMLua.OpenTable(num++))
			{
				NKCTrimRewardTemplet nKCTrimRewardTemplet = new NKCTrimRewardTemplet();
				if (!nKCTrimRewardTemplet.LoadFromLua(nKMLua))
				{
					nKMLua.CloseTable();
					continue;
				}
				if (!TrimRewardList.ContainsKey(nKCTrimRewardTemplet.trimID))
				{
					TrimRewardList.Add(nKCTrimRewardTemplet.trimID, new Dictionary<int, NKCTrimRewardTemplet>());
				}
				if (!TrimRewardList[nKCTrimRewardTemplet.trimID].ContainsKey(nKCTrimRewardTemplet.trimLevel))
				{
					TrimRewardList[nKCTrimRewardTemplet.trimID].Add(nKCTrimRewardTemplet.trimLevel, nKCTrimRewardTemplet);
				}
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		return true;
	}

	private bool LoadFromLua(NKMLua cNKMLua)
	{
		bool flag = true;
		flag &= cNKMLua.GetData("TrimID", ref trimID);
		flag &= cNKMLua.GetData("TrimLevel", ref trimLevel);
		cNKMLua.GetData("FirstClearRewardType", ref firstClearRewardType);
		cNKMLua.GetData("FirstClearRewardID", ref firstClearRewardID);
		cNKMLua.GetData("FirstClearRewardValue", ref firstClearValue);
		cNKMLua.GetData("RewardCountPoint", ref rewardCountPoint);
		cNKMLua.GetData("FirstClearRewardType", ref fixRewardType);
		cNKMLua.GetData("FixRewardID", ref fixRewardID);
		cNKMLua.GetData("m_RewardCredit_Min", ref rewardCreditMin);
		int num = 1;
		int rValue = 0;
		while (cNKMLua.GetData($"m_RewardGroupID_{num++}", ref rValue))
		{
			rewardGroupID.Add(rValue);
		}
		int num2 = 1;
		int rValue2 = 0;
		while (cNKMLua.GetData($"m_RewardUnitExp{num2++}", ref rValue2))
		{
			rewardUnitExp.Add(rValue2);
		}
		cNKMLua.GetData("m_RewardUserEXP", ref rewardUserExp);
		int num3 = 1;
		int rValue3 = 0;
		while (cNKMLua.GetData($"m_EventDropIndex_{num3++}", ref rValue3))
		{
			eventDropIndex.Add(rValue3);
		}
		return flag;
	}
}
