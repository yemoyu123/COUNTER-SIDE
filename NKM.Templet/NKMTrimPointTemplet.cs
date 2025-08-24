using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMTrimPointTemplet
{
	private const int TrimStageCount = 3;

	private static readonly Dictionary<int, List<NKMTrimPointTemplet>> TrimPointGroups = new Dictionary<int, List<NKMTrimPointTemplet>>();

	public int TrimPointGroup;

	public int TrimLevel;

	public int RecommendCombatPoint;

	public NKMTrimPoint[] Point = new NKMTrimPoint[3];

	public static NKMTrimPointTemplet Find(int group, int level)
	{
		return TrimPointGroups[group].FirstOrDefault((NKMTrimPointTemplet e) => e.TrimLevel == level);
	}

	public NKMTrimPointTemplet()
	{
		for (int i = 0; i < Point.Length; i++)
		{
			Point[i] = new NKMTrimPoint();
		}
	}

	public static bool Load(string assetName, string fileName)
	{
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath(assetName, fileName) || !nKMLua.OpenTable("TRIM_POINT_TEMPLET"))
			{
				return false;
			}
			int num = 1;
			while (nKMLua.OpenTable(num++))
			{
				NKMTrimPointTemplet nKMTrimPointTemplet = new NKMTrimPointTemplet();
				if (!nKMTrimPointTemplet.LoadFromLua(nKMLua))
				{
					nKMLua.CloseTable();
					continue;
				}
				if (!TrimPointGroups.ContainsKey(nKMTrimPointTemplet.TrimPointGroup))
				{
					List<NKMTrimPointTemplet> value = new List<NKMTrimPointTemplet>();
					TrimPointGroups[nKMTrimPointTemplet.TrimPointGroup] = value;
				}
				TrimPointGroups[nKMTrimPointTemplet.TrimPointGroup].Add(nKMTrimPointTemplet);
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		return true;
	}

	private bool LoadFromLua(NKMLua cNKMLua)
	{
		bool flag = true;
		flag &= cNKMLua.GetData("TrimPointGroup", ref TrimPointGroup);
		flag &= cNKMLua.GetData("TrimLevel", ref TrimLevel);
		flag &= cNKMLua.GetData("RecommendCombatPoint", ref RecommendCombatPoint);
		for (int i = 0; i < 3; i++)
		{
			flag &= cNKMLua.GetData($"TrimMaxDamagePoint_{i + 1}", ref Point[i].MaxDamagePoint);
			flag &= cNKMLua.GetData($"TrimMaxTimePoint_{i + 1}", ref Point[i].MaxTimePoint);
			flag &= cNKMLua.GetData($"TrimStageClearPoint_{i + 1}", ref Point[i].StageClearPoint);
		}
		return flag;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		NKMTrimPoint[] point = Point;
		foreach (NKMTrimPoint obj in point)
		{
			if (obj == null)
			{
				NKMTempletError.Add($"[NKMTrimPoint] Trim에 점수 기준이 존재하지 않음 trimLevel :{TrimLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimPointTemplet.cs", 97);
			}
			if (obj.StageClearPoint == 0)
			{
				NKMTempletError.Add($"[NKMTrimPoint] TrimPoint에 클리어 점수가 존재하지 않음 trimLevel :{TrimLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimPointTemplet.cs", 102);
			}
			if (obj.MaxDamagePoint == 0)
			{
				NKMTempletError.Add($"[NKMTrimPoint] TrimPoint에 데미지 누적 점수가 올바르지 않음 trimLevel :{TrimLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimPointTemplet.cs", 107);
			}
			if (obj.MaxTimePoint == 0)
			{
				NKMTempletError.Add($"[NKMTrimPoint] TrimPoint에 시간 누적 점수가 올바르지 않음 trimLevel :{TrimLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimPointTemplet.cs", 112);
			}
		}
	}
}
