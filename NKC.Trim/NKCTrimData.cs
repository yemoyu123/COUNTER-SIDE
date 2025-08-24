using System.Collections.Generic;
using ClientPacket.Common;

namespace NKC.Trim;

public class NKCTrimData
{
	private NKMTrimIntervalData trimIntervalData;

	private List<NKMTrimClearData> trimClearList;

	public NKMTrimIntervalData TrimIntervalData
	{
		get
		{
			if (trimIntervalData == null)
			{
				return new NKMTrimIntervalData();
			}
			return trimIntervalData;
		}
	}

	public List<NKMTrimClearData> TrimClearList
	{
		get
		{
			if (trimClearList == null)
			{
				return new List<NKMTrimClearData>();
			}
			return trimClearList;
		}
	}

	public void SetTrimIntervalData(NKMTrimIntervalData _trimIntervalData)
	{
		if (_trimIntervalData != null)
		{
			trimIntervalData = _trimIntervalData;
		}
	}

	public void SetTrimClearList(List<NKMTrimClearData> _trimClearList)
	{
		if (_trimClearList != null)
		{
			trimClearList = _trimClearList;
		}
	}

	public void SetTrimClearData(NKMTrimClearData trimClearData)
	{
		if (trimClearData != null && trimClearList != null)
		{
			int num = trimClearList.FindIndex((NKMTrimClearData e) => e.trimId == trimClearData.trimId && e.trimLevel == trimClearData.trimLevel);
			if (num >= 0)
			{
				trimClearList[num] = trimClearData;
			}
			else
			{
				trimClearList.Add(trimClearData);
			}
		}
	}

	public int GetClearedTrimLevel(int trimId)
	{
		int clearedLevel = 0;
		if (trimClearList != null)
		{
			List<NKMTrimClearData> list = trimClearList.FindAll((NKMTrimClearData e) => e.trimId == trimId);
			if (list != null && list.Count > 0)
			{
				list.ForEach(delegate(NKMTrimClearData e)
				{
					if (e.isWin && clearedLevel < e.trimLevel)
					{
						clearedLevel = e.trimLevel;
					}
				});
			}
		}
		return clearedLevel;
	}

	public NKMTrimClearData GetTrimClearData(int trimId, int trimLevel)
	{
		return trimClearList?.Find((NKMTrimClearData e) => e.trimId == trimId && e.trimLevel == trimLevel);
	}
}
