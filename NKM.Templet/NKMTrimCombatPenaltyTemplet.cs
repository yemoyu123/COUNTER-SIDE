using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMTrimCombatPenaltyTemplet
{
	public static readonly List<NKMTrimCombatPenaltyTemplet> PenaltyTempletList = new List<NKMTrimCombatPenaltyTemplet>();

	public int PenaltyIndex;

	public int TrimId;

	public string BattleConditionId;

	public int LowCombatRate;

	public static bool Load(string assetName, string fileName)
	{
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath(assetName, fileName) || !nKMLua.OpenTable("TRIM_COMBAT_PENALTY"))
			{
				return false;
			}
			int num = 1;
			while (nKMLua.OpenTable(num++))
			{
				NKMTrimCombatPenaltyTemplet nKMTrimCombatPenaltyTemplet = new NKMTrimCombatPenaltyTemplet();
				if (!nKMTrimCombatPenaltyTemplet.LoadFromLua(nKMLua))
				{
					nKMLua.CloseTable();
					continue;
				}
				PenaltyTempletList.Add(nKMTrimCombatPenaltyTemplet);
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		return true;
	}

	private bool LoadFromLua(NKMLua cNKMLua)
	{
		return (byte)(1u & (cNKMLua.GetData("INDEX", ref PenaltyIndex) ? 1u : 0u) & (cNKMLua.GetData("TrimID", ref TrimId) ? 1u : 0u) & (cNKMLua.GetData("CombatPenaltyBC", ref BattleConditionId) ? 1u : 0u) & (cNKMLua.GetData("Low_Combat_Rate", ref LowCombatRate) ? 1u : 0u)) != 0;
	}

	public static void Join()
	{
	}

	public static void Validate()
	{
		foreach (NKMTrimCombatPenaltyTemplet penaltyTemplet in PenaltyTempletList)
		{
			if (NKMBattleConditionManager.GetTempletByStrID(penaltyTemplet.BattleConditionId) == null)
			{
				NKMTempletError.Add("[NKMTrimCombatPenaltyTemplet] 배틀 컨디션 데이터가 없음. BattleCondition :" + penaltyTemplet.BattleConditionId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimCombatPenaltyTemplet.cs", 66);
			}
		}
	}
}
