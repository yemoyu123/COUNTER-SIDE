using System.Collections.Generic;
using ClientPacket.Event;
using Cs.Logging;
using NKM.Event;

namespace NKM;

public class EventBingo
{
	public class BingoLine
	{
		private List<int> list = new List<int>();

		public NKM_ERROR_CODE Mark(int index)
		{
			if (list.Exists((int e) => e.Equals(index)))
			{
				Log.Error("Fatal. Bingo tile Already marked.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMEventManagerEx.cs", 445);
				return NKM_ERROR_CODE.NEC_FAIL_EVENT_BINGO_ALREADY_MARKED;
			}
			list.Add(index);
			return NKM_ERROR_CODE.NEC_OK;
		}

		public int MarkedTileCount()
		{
			return list.Count;
		}
	}

	public int m_eventID;

	public int m_size;

	public BingoLine[] m_bingoLines;

	public NKMEventBingoTemplet m_bingoTemplet;

	public BingoInfo m_bingoInfo;

	public EventBingo(int eventID, BingoInfo bingoInfo)
	{
		m_eventID = eventID;
		m_bingoInfo = bingoInfo;
		m_bingoTemplet = NKMEventManager.GetBingoTemplet(eventID);
		m_size = m_bingoTemplet.m_BingoSize;
		int num = m_size * 2 + 2;
		m_bingoLines = new BingoLine[num];
		for (int i = 0; i < num; i++)
		{
			m_bingoLines[i] = new BingoLine();
		}
		for (int j = 0; j < bingoInfo.markTileIndexList.Count; j++)
		{
			int num2 = bingoInfo.markTileIndexList[j];
			foreach (int item in GetLineIndex(num2))
			{
				m_bingoLines[item].Mark(num2);
			}
		}
	}

	public void MarkToLine(int tileIndex)
	{
		if (m_bingoInfo.markTileIndexList.Exists((int e) => e.Equals(tileIndex)))
		{
			return;
		}
		foreach (int item in GetLineIndex(tileIndex))
		{
			m_bingoLines[item].Mark(tileIndex);
		}
		m_bingoInfo.markTileIndexList.Add(tileIndex);
	}

	public bool IsRemainNum()
	{
		int num = 0;
		for (int i = 0; i < m_bingoTemplet.MissionTiles.Count; i++)
		{
			if (m_bingoInfo.markTileIndexList.Contains(m_bingoTemplet.MissionTiles[i]))
			{
				num++;
			}
		}
		int num2 = m_bingoInfo.markTileIndexList.Count - num;
		int tileRange = m_bingoTemplet.TileRange;
		return num2 < tileRange;
	}

	public void SetMileage(int mileage)
	{
		if (m_bingoInfo != null)
		{
			m_bingoInfo.mileage = mileage;
		}
	}

	public List<int> GetBingoLine()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < m_bingoLines.Length; i++)
		{
			if (m_bingoLines[i].MarkedTileCount() == m_size)
			{
				list.Add(i);
			}
		}
		return list;
	}

	public void RecvReward(int index)
	{
		if (!m_bingoInfo.rewardList.Contains(index))
		{
			m_bingoInfo.rewardList.Add(index);
		}
	}

	private List<int> GetLineIndex(int tileIndex)
	{
		List<int> list = new List<int>();
		list.Add(tileIndex / m_size);
		list.Add(tileIndex % m_size + m_size);
		if (RightDiagonal(tileIndex))
		{
			list.Add(m_size * 2);
		}
		if (LeftDiagonal(tileIndex))
		{
			list.Add(m_size * 2 + 1);
		}
		return list;
	}

	private bool RightDiagonal(int index)
	{
		int num = index / m_size;
		return (m_size + 1) * num == index;
	}

	private bool LeftDiagonal(int index)
	{
		int num = index / m_size + 1;
		return (m_size - 1) * num == index;
	}

	public int GetTileValue(int tileIndex)
	{
		if (tileIndex < m_bingoInfo.tileValueList.Count)
		{
			return m_bingoInfo.tileValueList[tileIndex];
		}
		return 0;
	}

	public bool Completed()
	{
		if (IsRemainNum())
		{
			return false;
		}
		List<NKMEventBingoRewardTemplet> bingoRewardTempletList = NKMEventManager.GetBingoRewardTempletList(m_bingoTemplet.m_EventID);
		if (bingoRewardTempletList != null)
		{
			foreach (NKMEventBingoRewardTemplet item in bingoRewardTempletList)
			{
				if (item != null && NKMEventManager.IsReceiveableBingoReward(m_bingoTemplet.m_EventID, item.ZeroBaseTileIndex))
				{
					return false;
				}
			}
		}
		return true;
	}
}
