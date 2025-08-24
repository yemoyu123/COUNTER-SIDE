using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMTrimDungeonTemplet
{
	public static readonly List<NKMTrimDungeonTemplet> TrimDungeonList = new List<NKMTrimDungeonTemplet>();

	public int TrimId;

	public int TrimDungeonId;

	public int TrimLevelLow;

	public int TrimLevelHigh;

	public int DungeonId;

	public string TimeBossFaceCard;

	public bool ShowCutScene;

	public string TrimLevelBattleCondition;

	public List<string> TrimDungeonBattleCondition;

	public bool m_bShowCutScene;

	public NKMDungeonTempletBase DungeonTempletBase;

	public static bool Load(string assetName, string fileName)
	{
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath(assetName, fileName) || !nKMLua.OpenTable("TRIM_DUNGEON_TEMPLET"))
			{
				return false;
			}
			int num = 1;
			while (nKMLua.OpenTable(num++))
			{
				NKMTrimDungeonTemplet nKMTrimDungeonTemplet = new NKMTrimDungeonTemplet();
				if (!nKMTrimDungeonTemplet.LoadFromLua(nKMLua))
				{
					nKMLua.CloseTable();
					continue;
				}
				TrimDungeonList.Add(nKMTrimDungeonTemplet);
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		return true;
	}

	private bool LoadFromLua(NKMLua cNKMLua)
	{
		int result = (int)(1u & (cNKMLua.GetData("TrimID", ref TrimId) ? 1u : 0u) & (cNKMLua.GetData("TrimDungeonID", ref TrimDungeonId) ? 1u : 0u) & (cNKMLua.GetData("DungeonID", ref DungeonId) ? 1u : 0u) & (cNKMLua.GetData("TrimLevel_Low", ref TrimLevelLow) ? 1u : 0u) & (cNKMLua.GetData("TrimLevel_High", ref TrimLevelHigh) ? 1u : 0u)) & (cNKMLua.GetData("TrimLevelBC", ref TrimLevelBattleCondition) ? 1 : 0);
		cNKMLua.GetData("m_bShowCutScene", ref m_bShowCutScene);
		cNKMLua.GetDataList("TrimDungeonBC", out TrimDungeonBattleCondition, nullIfEmpty: false);
		cNKMLua.GetData("TrimBossFaceCard", ref TimeBossFaceCard);
		cNKMLua.GetData("m_bShowCutScene", ref ShowCutScene);
		return (byte)result != 0;
	}

	public static void Join()
	{
	}

	public bool IsValidateLevel(int level)
	{
		if (TrimLevelLow <= level)
		{
			return level <= TrimLevelHigh;
		}
		return false;
	}

	public static void Validate()
	{
		foreach (NKMTrimDungeonTemplet trimDungeon in TrimDungeonList)
		{
			trimDungeon.DungeonTempletBase = NKMTempletContainer<NKMDungeonTempletBase>.Find(trimDungeon.DungeonId);
			if (trimDungeon.DungeonTempletBase == null)
			{
				NKMTempletError.Add($"[NKMTrimDungeonTemplet] DungeonTempletBase가 존재하지 않음 Trim ID :{trimDungeon.TrimId} Dungeon ID : {trimDungeon.DungeonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimDungeonTemplet.cs", 88);
			}
			else if (NKMBattleConditionManager.GetTempletByStrID(trimDungeon.TrimLevelBattleCondition) == null)
			{
				NKMTempletError.Add($"[NKMTrimDungeonTemplet] Level BattleConditon 데이터가 존재하지 않음 Trim ID :{trimDungeon.TrimId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimDungeonTemplet.cs", 94);
			}
		}
	}
}
