using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMTrimIntervalTemplet
{
	private const int MaxRestoreCount = 3;

	public const int MaxSlotCount = 3;

	public static readonly List<NKMTrimIntervalTemplet> IntervalList = new List<NKMTrimIntervalTemplet>();

	public string DateStrId;

	public int TrimIntervalID;

	public int WeeklyEnterLimit;

	public int ResultResetLimit;

	public int RestoreLimitCount;

	public int RestoreLimitReqItemId;

	public int MaxSkipCount = 99;

	public int[] TrimSlot = new int[3];

	public int[] RestoreLimitReqItemCount = new int[3];

	public bool IsWeeklyUnLimit => WeeklyEnterLimit == 0;

	public bool IsResetUnLimit => ResultResetLimit == 0;

	public bool IsRestoreUnLimit => RestoreLimitCount == 0;

	public NKMIntervalTemplet IntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public static NKMTrimIntervalTemplet Find(DateTime dateTime)
	{
		return IntervalList.FirstOrDefault((NKMTrimIntervalTemplet e) => e.IntervalTemplet.IsValidTime(ServiceTime.Recent));
	}

	public static NKMTrimIntervalTemplet Find(int trimIntervalId)
	{
		return IntervalList.FirstOrDefault((NKMTrimIntervalTemplet e) => e.TrimIntervalID == trimIntervalId);
	}

	public static bool Load(string assetName, string fileName)
	{
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath(assetName, fileName) || !nKMLua.OpenTable("TRIM_INTERVAL_TEMPLET"))
			{
				return false;
			}
			int num = 1;
			while (nKMLua.OpenTable(num++))
			{
				NKMTrimIntervalTemplet nKMTrimIntervalTemplet = new NKMTrimIntervalTemplet();
				if (!nKMTrimIntervalTemplet.LoadFromLua(nKMLua))
				{
					nKMLua.CloseTable();
					continue;
				}
				IntervalList.Add(nKMTrimIntervalTemplet);
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		return true;
	}

	private bool LoadFromLua(NKMLua cNKMLua)
	{
		bool flag = true;
		flag &= cNKMLua.GetData("m_DateStrID", ref DateStrId);
		flag &= cNKMLua.GetData("INDEX", ref TrimIntervalID);
		cNKMLua.GetData("WeeklyEnterLimit", ref WeeklyEnterLimit);
		cNKMLua.GetData("ResultResetLimit", ref ResultResetLimit);
		cNKMLua.GetData("RestoreLimitCount", ref RestoreLimitCount);
		cNKMLua.GetData("RestoreLimitReqItemID", ref RestoreLimitReqItemId);
		for (int i = 0; i < 3; i++)
		{
			cNKMLua.GetData($"RestoreLimitReqItemCount_{i + 1}", ref RestoreLimitReqItemCount[i]);
		}
		int rValue = 0;
		for (int j = 0; j < 3; j++)
		{
			if (cNKMLua.GetData($"TrimSlotID_{j + 1}", ref rValue))
			{
				TrimSlot[j] = rValue;
			}
		}
		if (WeeklyEnterLimit > 0)
		{
			MaxSkipCount = WeeklyEnterLimit;
		}
		return flag;
	}

	public static void Join()
	{
		foreach (NKMTrimIntervalTemplet interval in IntervalList)
		{
			interval.JoinIntervalTemplet();
		}
	}

	public void JoinIntervalTemplet()
	{
		IntervalTemplet = NKMIntervalTemplet.Find(DateStrId);
		if (IntervalTemplet == null)
		{
			NKMTempletError.Add("[NKMTrimIntervalTemplet] 잘못된 interval id:" + DateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimIntervalTemplet.cs", 109);
			IntervalTemplet = NKMIntervalTemplet.Unuseable;
		}
		else if (IntervalTemplet.IsRepeatDate)
		{
			NKMTempletError.Add("[NKMTrimIntervalTemplet] 반복 기간설정 사용 불가. id:" + DateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimIntervalTemplet.cs", 114);
		}
	}

	public bool IsValidTime(DateTime dateTime)
	{
		if (IntervalTemplet == null)
		{
			return false;
		}
		if (!IntervalTemplet.IsValidTime(dateTime))
		{
			return false;
		}
		return true;
	}

	public static void Validate()
	{
		if (Find(ServiceTime.Recent) == null)
		{
			NKMTempletError.Add("[NKMTrimIntervalTemplet] 현재 디멘션 트리밍을 진행할 수 있는 템플릿 정보 없음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimIntervalTemplet.cs", 140);
		}
	}
}
