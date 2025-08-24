using System;
using System.Collections.Generic;
using Cs.Math.Lottery;
using NKM.Contract2;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMUnitExtractBonuseTemplet
{
	private readonly RatioLottery<MiscItemUnit> lottery = new RatioLottery<MiscItemUnit>();

	private List<Tuple<int, MiscItemUnit>> ItemData = new List<Tuple<int, MiscItemUnit>>();

	public static NKMUnitExtractBonuseTemplet Instance { get; private set; }

	public IEnumerable<MiscItemUnit> Items => lottery;

	public IEnumerable<Tuple<int, MiscItemUnit>> Datas => ItemData;

	public static void LoadFromLua()
	{
		string bundleName = "AB_SCRIPT";
		string text = "LUA_EXTRACT_BONUS_TEMPLET";
		string text2 = "ECTRACT_BONUS_TEMPLET";
		Instance = new NKMUnitExtractBonuseTemplet();
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath(bundleName, text) || !nKMLua.OpenTable(text2))
		{
			NKMTempletError.Add("[ExtractTemplet] loading file failed. fileName:" + text + " tablName:" + text2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitExtractBonuseTemplet.cs", 31);
			return;
		}
		for (int i = 1; nKMLua.OpenTable(i); i++)
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitExtractBonuseTemplet.cs", 38))
			{
				break;
			}
			int @int = nKMLua.GetInt32("m_ExtractBonusItemID");
			int int2 = nKMLua.GetInt32("m_ExtractBonusItemCount");
			int int3 = nKMLua.GetInt32("m_Ratio");
			Instance.lottery.AddCase(int3, new MiscItemUnit(@int, int2));
			Instance.ItemData.Add(new Tuple<int, MiscItemUnit>(int3, new MiscItemUnit(@int, int2)));
			nKMLua.CloseTable();
		}
	}

	public void Join()
	{
		foreach (MiscItemUnit item in lottery)
		{
			item.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitExtractBonuseTemplet.cs", 61);
		}
	}

	public MiscItemUnit GetRandom()
	{
		return lottery.Decide();
	}
}
