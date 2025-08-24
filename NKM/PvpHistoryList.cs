using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class PvpHistoryList : ISerializable
{
	private List<PvpSingleHistory> list = new List<PvpSingleHistory>();

	public PvpSingleHistory GetData(int index)
	{
		if (list == null)
		{
			return null;
		}
		if (index < 0 || index >= list.Count)
		{
			return null;
		}
		return list[index];
	}

	public PvpSingleHistory GetDataByGameUID(long gameUID)
	{
		if (list == null)
		{
			return null;
		}
		return list.Find((PvpSingleHistory x) => x.gameUid == gameUID);
	}

	public int GetCount()
	{
		if (list == null)
		{
			return 0;
		}
		return list.Count;
	}

	public void Sort()
	{
		list.Sort((PvpSingleHistory a, PvpSingleHistory b) => b.RegdateTick.CompareTo(a.RegdateTick));
	}

	public void Add(List<PvpSingleHistory> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			Add(lstData[i]);
		}
	}

	public void Add(PvpSingleHistory data)
	{
		if (data != null)
		{
			list.Add(data);
			Sort();
			if (list.Count > NKMPvpCommonConst.Instance.MaxHistoryCount)
			{
				list.RemoveRange(NKMPvpCommonConst.Instance.MaxHistoryCount, list.Count - NKMPvpCommonConst.Instance.MaxHistoryCount);
			}
		}
	}

	public void FilterByGameType(NKM_GAME_TYPE gameType)
	{
		List<PvpSingleHistory> list = this.list.FindAll((PvpSingleHistory x) => x.GameType == gameType);
		this.list = list;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref list);
	}
}
