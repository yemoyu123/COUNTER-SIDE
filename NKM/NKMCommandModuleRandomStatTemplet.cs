using System;
using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMCommandModuleRandomStatTemplet : INKMTemplet
{
	private int statGroupId;

	private NKM_STAT_TYPE statType;

	private float minStatValue;

	private float maxStatValue;

	private float statValueControl;

	private List<float> CandidateValues = new List<float>();

	public int Key => statGroupId;

	public int StatGroupId => statGroupId;

	public NKM_STAT_TYPE StatType => statType;

	public float MinStatValue => minStatValue;

	public float MaxStatValue => maxStatValue;

	public float StatValueControl => statValueControl;

	public static IEnumerable<NKMCommandModuleRandomStatTemplet> Values => NKMTempletContainer<NKMCommandModuleRandomStatTemplet>.Values;

	public IReadOnlyList<float> GetCandidateValues => CandidateValues;

	public static NKMCommandModuleRandomStatTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 758))
		{
			return null;
		}
		NKMCommandModuleRandomStatTemplet nKMCommandModuleRandomStatTemplet = new NKMCommandModuleRandomStatTemplet
		{
			statGroupId = lua.GetInt32("StatGroupID")
		};
		nKMCommandModuleRandomStatTemplet.minStatValue = lua.GetFloat("MinStatValue", 0f);
		nKMCommandModuleRandomStatTemplet.maxStatValue = lua.GetFloat("MaxStatValue", 0f);
		nKMCommandModuleRandomStatTemplet.statValueControl = lua.GetFloat("StatValueControl", 0f);
		lua.GetDataEnum<NKM_STAT_TYPE>("StatType", out nKMCommandModuleRandomStatTemplet.statType);
		float num = lua.GetFloat("MinStatFactor", 0f);
		float num2 = lua.GetFloat("MaxStatFactor", 0f);
		if (num != 0f || num2 != 0f)
		{
			NKM_STAT_TYPE factorStat = NKMUnitStatManager.GetFactorStat(nKMCommandModuleRandomStatTemplet.statType);
			if (factorStat == NKM_STAT_TYPE.NST_END)
			{
				Log.ErrorAndExit($"[NKMCommandModuleRandomStatTemplet] factorStat \ufffd\ufffd\ufffd\ufffdȣȯ ó\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. statType = {nKMCommandModuleRandomStatTemplet.statType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 782);
				return null;
			}
			nKMCommandModuleRandomStatTemplet.statType = factorStat;
			nKMCommandModuleRandomStatTemplet.minStatValue = num;
			nKMCommandModuleRandomStatTemplet.maxStatValue = num2;
			float num3 = lua.GetFloat("StatFactorControl", 0f);
			nKMCommandModuleRandomStatTemplet.statValueControl = num3;
		}
		return nKMCommandModuleRandomStatTemplet;
	}

	public void Join()
	{
		if (minStatValue != 0f && maxStatValue != 0f && statValueControl > 0f)
		{
			for (float num = minStatValue; num <= maxStatValue; num = (float)(Math.Round((num + statValueControl) * 10000f) / 10000.0))
			{
				CandidateValues.Add(num);
			}
		}
	}

	public void Validate()
	{
		if (minStatValue != 0f && maxStatValue != 0f && statValueControl > 0f)
		{
			if (StatType != NKM_STAT_TYPE.NST_HEAL_REDUCE_RATE && (minStatValue < 0f || maxStatValue < 0f))
			{
				NKMTempletError.Add($"[NKMCommandModuleRandomStatTemplet:{statGroupId}] \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd statType\ufffd\ufffd \ufffdƴѵ\ufffd Value\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd. minStatValue:{minStatValue} minStatValue:{minStatValue} maxStatValue:{maxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 820);
			}
			if (maxStatValue < minStatValue)
			{
				NKMTempletError.Add($"[NKMCommandModuleRandomStatTemplet:{statGroupId}] statValue\ufffd\ufffd\ufffd\ufffd \ufffd\u033b\ufffd\ufffd\ufffd. minStatValue:{minStatValue} maxStatValue:{maxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 825);
			}
			if (minStatValue == maxStatValue && statValueControl != 0f)
			{
				NKMTempletError.Add($"[NKMCommandModuleRandomStatTemplet:{statGroupId}] statValue\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd Control\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. statValueControl:{statValueControl}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 830);
			}
			if (StatType == NKM_STAT_TYPE.NST_HEAL_REDUCE_RATE && minStatValue < 0f && maxStatValue < 0f)
			{
				if (maxStatValue * -1f < statValueControl)
				{
					NKMTempletError.Add($"[NKMCommandModuleRandomStatTemplet:{statGroupId}] statValueControl\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd maxStatValue:{maxStatValue} < statValueControl:{statValueControl}.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 838);
				}
			}
			else if (maxStatValue < statValueControl)
			{
				NKMTempletError.Add($"[NKMCommandModuleRandomStatTemplet:{statGroupId}] statValueControl\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd maxStatValue:{maxStatValue} < statValueControl:{statValueControl}.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 843);
			}
			if (statValueControl <= 0f)
			{
				NKMTempletError.Add($"[NKMCommandModuleRandomStatTemplet:{statGroupId}] statValueControl\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd statValueControl:{statValueControl} <= 0", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 848);
			}
		}
		else
		{
			string arg = $"minStatValue:{minStatValue} maxStatValue:{maxStatValue} statValueControl:{statValueControl}";
			NKMTempletError.Add($"[NKMCommandModuleRandomStatTemplet:{statGroupId}] \ufffd\ufffdü\ufffd\ufffd\ufffd\ufffd \ufffdԷ°\ufffd\ufffd\ufffd \ufffd\u033b\ufffd\ufffd\ufffd. {arg}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 854);
		}
	}
}
