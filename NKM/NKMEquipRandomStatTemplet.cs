using System;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public class NKMEquipRandomStatTemplet : INKMTemplet
{
	public int m_StatGroupID;

	public NKM_STAT_TYPE m_StatType;

	public float m_MinStatValue;

	public float m_MaxStatValue;

	public int Key => m_StatGroupID;

	private string DebugName => $"[EquipStat {m_StatGroupID} {m_StatType}]";

	public static NKMEquipRandomStatTemplet LoadFromLUA(NKMLua lua)
	{
		NKMEquipRandomStatTemplet nKMEquipRandomStatTemplet = new NKMEquipRandomStatTemplet();
		nKMEquipRandomStatTemplet.m_StatGroupID = lua.GetInt32("m_StatGroupID");
		nKMEquipRandomStatTemplet.m_StatType = lua.GetEnum<NKM_STAT_TYPE>("m_StatType");
		lua.GetData("m_MinStatValue", ref nKMEquipRandomStatTemplet.m_MinStatValue);
		lua.GetData("m_MaxStatValue", ref nKMEquipRandomStatTemplet.m_MaxStatValue);
		float rValue = 0f;
		float rValue2 = 0f;
		if ((0u | (lua.GetData("m_MinStatRate", ref rValue) ? 1u : 0u) | (lua.GetData("m_MaxStatRate", ref rValue2) ? 1u : 0u)) != 0)
		{
			NKM_STAT_TYPE factorStat = NKMUnitStatManager.GetFactorStat(nKMEquipRandomStatTemplet.m_StatType);
			if (factorStat == NKM_STAT_TYPE.NST_END)
			{
				Log.ErrorAndExit($"non-factor stat {nKMEquipRandomStatTemplet.m_StatType} has stat factor value", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEquipRandomStatTemplet.cs", 42);
			}
			nKMEquipRandomStatTemplet.m_StatType = factorStat;
			nKMEquipRandomStatTemplet.m_MinStatValue = rValue;
			nKMEquipRandomStatTemplet.m_MaxStatValue = rValue2;
		}
		return nKMEquipRandomStatTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (!NKMUnitStatManager.IsMainFactorStat(m_StatType))
		{
			if (NKMUnitStatManager.IsMainStat(m_StatType))
			{
				if (m_MinStatValue < 1f || m_MaxStatValue < 1f)
				{
					NKMTempletError.Add($"{DebugName} 1차스탯 min~max 덧셈(value) 수치 1.0 미만. value:{m_MinStatValue}~{m_MaxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEquipRandomStatTemplet.cs", 67);
				}
			}
			else if (m_MinStatValue >= 1f || m_MaxStatValue >= 1f)
			{
				NKMTempletError.Add($"{DebugName} 2차스탯 min~max 덧셈(value) 수치 1.0 이상. value:{m_MinStatValue}~{m_MaxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEquipRandomStatTemplet.cs", 72);
			}
			else if (m_MinStatValue <= -1f || m_MaxStatValue <= -1f)
			{
				NKMTempletError.Add($"{DebugName} 2차스탯(음수) min~max 덧셈(value) 수치 -1.0 이하. value:{m_MinStatValue}~{m_MaxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEquipRandomStatTemplet.cs", 76);
			}
			if (m_MinStatValue > m_MaxStatValue)
			{
				NKMTempletError.Add($"{DebugName} addable min~max 수치이상. value:{m_MinStatValue}~{m_MaxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEquipRandomStatTemplet.cs", 81);
			}
			if (m_MinStatValue * m_MaxStatValue < 0f)
			{
				NKMTempletError.Add($"{DebugName} addable min~max값의 부호가 다름. value:{m_MinStatValue}~{m_MaxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEquipRandomStatTemplet.cs", 86);
			}
		}
		else
		{
			if (m_MinStatValue > m_MaxStatValue)
			{
				NKMTempletError.Add($"{DebugName} multipliable min~max 수치이상. value:{m_MinStatValue}~{m_MaxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEquipRandomStatTemplet.cs", 93);
			}
			if (m_MinStatValue >= 1f || m_MaxStatValue >= 1f)
			{
				NKMTempletError.Add($"{DebugName} multipliable min~max 수치 1.0 초과. value:{m_MinStatValue}~{m_MaxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEquipRandomStatTemplet.cs", 98);
			}
		}
	}

	public EQUIP_ITEM_STAT GenerateSubStat(int precision)
	{
		return new EQUIP_ITEM_STAT
		{
			type = m_StatType,
			stat_value = CalcStatValue(precision)
		};
	}

	public float CalcResultStat(int precision)
	{
		return CalcStatValue(precision);
	}

	public bool IsInitialPrecisionMax()
	{
		return m_MinStatValue == m_MaxStatValue;
	}

	private float CalcStatValue(int precision)
	{
		float num = (float)precision / 100f;
		float num2 = ((!(m_MaxStatValue < 0f) || !(m_MinStatValue < 0f)) ? ((m_MaxStatValue - m_MinStatValue) * num + m_MinStatValue) : ((m_MinStatValue - m_MaxStatValue) * num + m_MaxStatValue));
		if (NKMUnitStatManager.IsPercentStat(m_StatType))
		{
			return (float)(Math.Truncate(num2 * 10000f) / 10000.0);
		}
		return (float)Math.Truncate(num2);
	}
}
