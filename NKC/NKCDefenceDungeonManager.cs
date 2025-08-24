using System.Collections.Generic;
using ClientPacket.Defence;
using ClientPacket.LeaderBoard;
using Cs.Core.Util;
using Cs.Logging;
using NKM;
using NKM.Templet;

namespace NKC;

public static class NKCDefenceDungeonManager
{
	private static HashSet<int> m_hsScoreReward = new HashSet<int>();

	public static int m_DefenceTempletId { get; private set; }

	public static int m_BestClearScore { get; private set; }

	public static bool m_bMissionResult1 { get; private set; }

	public static bool m_bMissionResult2 { get; private set; }

	public static int m_MyRankNum { get; private set; }

	public static int m_MyRankPercent { get; private set; }

	public static bool m_bCanReceiveRankReward { get; private set; }

	public static NKMDefenceRankData m_topRankData { get; private set; }

	public static void Init()
	{
		m_DefenceTempletId = 0;
		m_BestClearScore = 0;
		m_bMissionResult1 = false;
		m_bMissionResult2 = false;
		m_topRankData = null;
		m_bCanReceiveRankReward = false;
		m_MyRankNum = 0;
		m_MyRankPercent = 0;
		m_hsScoreReward = new HashSet<int>();
	}

	public static void SetTopRankData(NKMDefenceRankData topRankData)
	{
		m_topRankData = topRankData;
	}

	public static void SetData(NKMPacket_DEFENCE_INFO_ACK sPacket)
	{
		m_DefenceTempletId = sPacket.defenceTempletId;
		m_BestClearScore = sPacket.bestScore;
		if (m_BestClearScore <= 0)
		{
			m_BestClearScore = 0;
		}
		m_bMissionResult1 = sPacket.missionResult1;
		m_bMissionResult2 = sPacket.missionResult2;
		m_MyRankNum = sPacket.rank;
		m_MyRankPercent = sPacket.rankPercent;
		m_topRankData = sPacket.topRankProfile;
		m_bCanReceiveRankReward = sPacket.canReceiveRankReward;
		m_hsScoreReward.Clear();
		for (int i = 0; i < sPacket.scoreRewardIds.Count; i++)
		{
			if (!m_hsScoreReward.Contains(sPacket.scoreRewardIds[i]))
			{
				m_hsScoreReward.Add(sPacket.scoreRewardIds[i]);
			}
			else
			{
				Log.Error($"NKMPacket_DEFENCE_INFO_ACK.scoreRewardIds - {sPacket.scoreRewardIds[i]} is duplicated", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCDefenceDungeonManager.cs", 65);
			}
		}
	}

	public static void SetMyRank(int rank)
	{
		if (rank == 0 || rank > 100)
		{
			m_MyRankPercent = rank;
			m_MyRankNum = 0;
		}
		else
		{
			m_MyRankNum = rank;
		}
	}

	public static void SetRankRewardReceived()
	{
		m_bCanReceiveRankReward = false;
	}

	public static void SetData(NKMDefenceClearData defenceClearData, bool bMissionResult1, bool bMissionResult2)
	{
		m_DefenceTempletId = defenceClearData.defenceTempletId;
		m_BestClearScore = defenceClearData.bestScore;
		m_bMissionResult1 |= bMissionResult1;
		m_bMissionResult2 |= bMissionResult2;
	}

	public static string GetRankingTotalDesc()
	{
		if (m_MyRankNum == 0 && m_MyRankPercent == 0)
		{
			return string.Format(NKCUtilString.GET_FIERCE_RANK_DESC_01, 100);
		}
		if (m_MyRankNum != 0 && m_MyRankNum <= 100)
		{
			return string.Format(NKCUtilString.GET_FIERCE_RANK_IN_TOP_100_DESC_01, m_MyRankNum);
		}
		return string.Format(NKCUtilString.GET_FIERCE_RANK_DESC_01, m_MyRankPercent);
	}

	public static bool NeedHideDeckInfo()
	{
		NKMDefenceTemplet nKMDefenceTemplet = NKMDefenceTemplet.Find(m_DefenceTempletId);
		NKM_ERROR_CODE resultCode;
		if (nKMDefenceTemplet != null && nKMDefenceTemplet.m_PrivateRankInfo)
		{
			return nKMDefenceTemplet.CheckGameEnable(ServiceTime.Now, out resultCode);
		}
		return false;
	}

	public static bool IsReceivedPointReward(int rewardId)
	{
		return m_hsScoreReward.Contains(rewardId);
	}

	public static bool IsCanReceivePointReward()
	{
		NKMDefenceTemplet nKMDefenceTemplet = NKMDefenceTemplet.Find(m_DefenceTempletId);
		if (NKMDefenceScoreRewardTemplet.Groups.ContainsKey(nKMDefenceTemplet.m_ScoreRewardGroupID))
		{
			List<NKMDefenceScoreRewardTemplet> list = NKMDefenceScoreRewardTemplet.Groups[nKMDefenceTemplet.m_ScoreRewardGroupID];
			int bestClearScore = m_BestClearScore;
			foreach (NKMDefenceScoreRewardTemplet item in list)
			{
				if (item.Score <= bestClearScore && !IsReceivedPointReward(item.DefenceScoreRewardID))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void UpdateReceveScoreRewardID(int receivedScoreRewardID)
	{
		if (m_hsScoreReward != null && !m_hsScoreReward.Contains(receivedScoreRewardID))
		{
			m_hsScoreReward.Add(receivedScoreRewardID);
		}
	}
}
