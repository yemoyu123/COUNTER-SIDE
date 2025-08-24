using System.Collections.Generic;
using Cs.Core.Util;
using Cs.Math;
using Cs.Math.Lottery;
using NKM.Templet.Base;

namespace NKM.Contract2;

public sealed class MiscContractTemplet : INKMTemplet
{
	public int m_RandomGradeId;

	public int m_UnitPoolId;

	public int m_UnitCount;

	public int Key { get; }

	public static IEnumerable<MiscContractTemplet> Values => NKMTempletContainer<MiscContractTemplet>.Values;

	public RandomGradeTempletV2 RandomGradeTemplet { get; private set; }

	public RandomUnitPoolTempletV2 UnitPoolTemplet { get; private set; }

	private MiscContractTemplet(int key)
	{
		Key = key;
	}

	public static MiscContractTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/MiscContractTemplet.cs", 29))
		{
			return null;
		}
		if (!lua.GetData("m_ContractID", out var rValue, -1))
		{
			NKMTempletError.Add("[Contract] m_ContractID column loading failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/MiscContractTemplet.cs", 36);
			return null;
		}
		MiscContractTemplet miscContractTemplet = new MiscContractTemplet(rValue);
		lua.GetData("m_UnitCount", ref miscContractTemplet.m_UnitCount);
		if ((1u & (lua.GetData("m_RandomGradeID", ref miscContractTemplet.m_RandomGradeId) ? 1u : 0u) & (lua.GetData("m_UnitPoolID", ref miscContractTemplet.m_UnitPoolId) ? 1u : 0u)) == 0)
		{
			NKMTempletError.Add($"[{miscContractTemplet.Key}] templet loading failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/MiscContractTemplet.cs", 49);
			return null;
		}
		return miscContractTemplet;
	}

	public static MiscContractTemplet Find(int key)
	{
		return NKMTempletContainer<MiscContractTemplet>.Find(key);
	}

	public void Join()
	{
		RandomGradeTemplet = RandomGradeTempletV2.Find(m_RandomGradeId);
		if (RandomGradeTemplet == null)
		{
			NKMTempletError.Add($"[Contract] invalid randomGrade id:{m_RandomGradeId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/MiscContractTemplet.cs", 64);
		}
		UnitPoolTemplet = RandomUnitPoolTempletV2.Find(m_UnitPoolId);
		if (UnitPoolTemplet == null)
		{
			NKMTempletError.Add($"[Contract] invalid UnitPool id:{m_UnitPoolId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/MiscContractTemplet.cs", 70);
		}
	}

	public void Validate()
	{
	}

	public void ValidateMiscContract()
	{
		if (m_UnitCount <= 0)
		{
			NKMTempletError.Add($"MiscContract[{Key}] 유효하지 않은 유닛 수. m_UnitCount:{m_UnitCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/MiscContractTemplet.cs", 82);
		}
		foreach (NKM_UNIT_PICK_GRADE value in EnumUtil<NKM_UNIT_PICK_GRADE>.GetValues())
		{
			RatioLottery<RandomUnitTempletV2> lottery = UnitPoolTemplet.GetLottery(value);
			if (!RandomGradeTemplet.Lottery.TryGetRatePercent(value, out var ratePercent) && lottery.Count == 0)
			{
				continue;
			}
			if (lottery.Count == 0)
			{
				NKMTempletError.Add($"MiscContract[{Key}] 등급 확률이 있으나 유닛 풀이 비어있음. pickGrade:{value} gradeRate:{ratePercent:0.00}% gradePoolId:{m_RandomGradeId} unitPoolId:{m_UnitPoolId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/MiscContractTemplet.cs", 96);
			}
			if (ratePercent.IsNearlyZero())
			{
				NKMTempletError.Add($"MiscContract[{Key}] 유닛이 있는데 등급 확률이 설정되지 않음. pickGrade:{value} unitCount:{lottery.Count} gradePoolId:{m_RandomGradeId} unitPoolId:{m_UnitPoolId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/MiscContractTemplet.cs", 101);
			}
			foreach (RandomUnitTempletV2 caseValue in lottery.CaseValues)
			{
				caseValue.CalcFinalRate(ratePercent, lottery.TotalRatio);
			}
		}
		foreach (RandomUnitTempletV2 unitTemplet in UnitPoolTemplet.UnitTemplets)
		{
			if (!unitTemplet.UnitTemplet.IsUnitStyleType())
			{
				NKMTempletError.Add($"잘못된 유닛 풀 정보. unitType:{unitTemplet.UnitTemplet.m_NKM_UNIT_TYPE} unitId:{unitTemplet.UnitTemplet.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/MiscContractTemplet.cs", 120);
			}
		}
	}
}
