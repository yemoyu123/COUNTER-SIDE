using System;
using System.Collections.Generic;
using ClientPacket.Event;
using Cs.Math;
using UnityEngine;

public static class NKCMatchTenManager
{
	public static int TOTAL_COL_COUNT = 20;

	public static int TOTAL_ROW_COUNT = 8;

	private const int TARGET_NUM = 10;

	private static int[,] m_BoardData;

	private static bool m_bInitComplete = false;

	private static List<Vector2> m_lstHint = new List<Vector2>();

	private static int TOTAL_TIME_SEC = 120;

	private static int m_TempletId = 0;

	private static int m_MyBestScore = 0;

	private static int m_MyBestRemainTime = 0;

	private static int m_PerfectScoreValue = 0;

	private static HashSet<int> m_hsScoreReward = new HashSet<int>();

	public static int GetTargetNum()
	{
		return 10;
	}

	public static int[,] GetBoardData()
	{
		if (!m_bInitComplete)
		{
			SetBoard();
		}
		return m_BoardData;
	}

	public static int GetTotalTimeSec()
	{
		return TOTAL_TIME_SEC;
	}

	public static void SetTotalTimeSec(int totalTimeSec)
	{
		TOTAL_TIME_SEC = totalTimeSec;
	}

	public static int GetTempletId()
	{
		return m_TempletId;
	}

	public static int GetBestScore()
	{
		return m_MyBestScore;
	}

	public static int GetBestRemainTime()
	{
		return m_MyBestRemainTime;
	}

	public static int GetPerfectScoreValue()
	{
		return m_PerfectScoreValue;
	}

	public static void SetPerfectScoreValue(int score)
	{
		m_PerfectScoreValue = score;
	}

	public static void DoAfterLogout()
	{
		m_TempletId = 0;
		m_MyBestRemainTime = 0;
		m_MyBestScore = 0;
		m_hsScoreReward.Clear();
	}

	public static void SetBoardSize(int col, int row)
	{
		TOTAL_COL_COUNT = col;
		TOTAL_ROW_COUNT = row;
		m_BoardData = new int[TOTAL_COL_COUNT, TOTAL_ROW_COUNT];
	}

	public static void SetBoard(bool bIsReroll = false)
	{
		for (int i = 0; i < TOTAL_COL_COUNT; i++)
		{
			for (int j = 0; j < TOTAL_ROW_COUNT; j++)
			{
				if (!bIsReroll || m_BoardData[i, j] != 0)
				{
					m_BoardData[i, j] = RandomGenerator.Range(1, 10);
				}
			}
		}
		if (IsNeedReroll())
		{
			SetBoard(bIsReroll);
		}
		else
		{
			m_bInitComplete = true;
		}
	}

	public static bool IsNeedReroll()
	{
		return !CheckRemainData();
	}

	public static List<Vector2> GetHint()
	{
		return m_lstHint;
	}

	public static bool CheckRemainData()
	{
		m_lstHint.Clear();
		for (int i = 0; i < TOTAL_COL_COUNT; i++)
		{
			for (int j = 0; j < TOTAL_ROW_COUNT; j++)
			{
				for (int k = i; k < TOTAL_COL_COUNT; k++)
				{
					for (int l = j; l < TOTAL_ROW_COUNT; l++)
					{
						bool overTargetNum;
						int sum = GetSum(i, j, k, l, out overTargetNum);
						if (overTargetNum)
						{
							break;
						}
						if (sum == 10)
						{
							m_lstHint.Add(new Vector2(i, j));
							m_lstHint.Add(new Vector2(k, l));
							return true;
						}
					}
				}
			}
		}
		m_lstHint.Clear();
		return false;
	}

	public static void RemoveMatched(int minCol, int minRow, int maxCol, int maxRow)
	{
		for (int i = minCol; i <= maxCol; i++)
		{
			for (int j = minRow; j <= maxRow; j++)
			{
				m_BoardData[i, j] = 0;
			}
		}
	}

	public static int GetSum(int minCol, int minRow, int maxCol, int maxRow, out bool overTargetNum)
	{
		int num = 0;
		overTargetNum = false;
		for (int i = minCol; i <= maxCol; i++)
		{
			for (int j = minRow; j <= maxRow; j++)
			{
				num += m_BoardData[i, j];
				if (num > 10)
				{
					overTargetNum = true;
					return num;
				}
			}
		}
		return num;
	}

	public static int GetZeroCount()
	{
		int num = 0;
		for (int i = 0; i < TOTAL_COL_COUNT; i++)
		{
			for (int j = 0; j < TOTAL_ROW_COUNT; j++)
			{
				if (m_BoardData[i, j] == 0)
				{
					num++;
				}
			}
		}
		return num;
	}

	public static bool IsRewardReceived(int rewardId)
	{
		return m_hsScoreReward.Contains(rewardId);
	}

	public static void SetMyScore(int score, int remainTime)
	{
		m_MyBestScore = Math.Max(m_MyBestScore, score);
		m_MyBestRemainTime = Math.Max(m_MyBestRemainTime, remainTime);
	}

	public static void OnRecv(NKMPACKET_EVENT_TEN_RECORD_NOT sPacket)
	{
		m_TempletId = sPacket.templetId;
		m_MyBestScore = sPacket.score;
		m_MyBestRemainTime = sPacket.remainTime;
		m_hsScoreReward.Clear();
		for (int i = 0; i < sPacket.scoreRewardIds.Count; i++)
		{
			m_hsScoreReward.Add(sPacket.scoreRewardIds[i]);
		}
	}

	public static void OnRecv(NKMPACKET_EVENT_TEN_RESULT_ACK sPacket)
	{
	}

	public static void SetReceivedIds(List<int> rewardIds)
	{
		for (int i = 0; i < rewardIds.Count; i++)
		{
			m_hsScoreReward.Add(rewardIds[i]);
		}
	}
}
