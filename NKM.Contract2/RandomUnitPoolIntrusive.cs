using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Math.Lottery;

namespace NKM.Contract2;

public class RandomUnitPoolIntrusive : IRandomUnitPool
{
	private readonly RandomUnitPoolTempletV2 sourceTemplet;

	private readonly RandomUnitTempletV2 addUnit;

	private readonly Dictionary<string, RandomUnitTempletV2> unitTemplets = new Dictionary<string, RandomUnitTempletV2>();

	private readonly RatioLottery<RandomUnitTempletV2>[] lotteries = new RatioLottery<RandomUnitTempletV2>[EnumUtil<NKM_UNIT_PICK_GRADE>.Count];

	public string DebugName => "[" + sourceTemplet.DebugName + " + " + addUnit.UnitTemplet.DebugName + "]";

	public int TotalUnitCount => unitTemplets.Count;

	public IEnumerable<RandomUnitTempletV2> UnitTemplets => unitTemplets.Values;

	public IReadOnlyList<IEnumerable<RandomUnitTempletV2>> UnitsByPickGrade => lotteries;

	public RandomUnitPoolIntrusive(RandomUnitPoolTempletV2 sourceTemplet, RandomUnitTempletV2 addUnit)
	{
		this.sourceTemplet = sourceTemplet;
		this.addUnit = addUnit;
		sourceTemplet.Join();
		unitTemplets = sourceTemplet.UnitTemplets.Select((RandomUnitTempletV2 e) => e.Clone()).ToDictionary((RandomUnitTempletV2 e) => e.UnitTemplet.m_UnitStrID);
		if (IsValidUnitByTag(this.addUnit))
		{
			unitTemplets[addUnit.UnitStringId] = addUnit;
		}
		for (int num = 0; num < lotteries.Length; num++)
		{
			lotteries[num] = new RatioLottery<RandomUnitTempletV2>();
		}
		foreach (RandomUnitTempletV2 value in unitTemplets.Values)
		{
			value.Join();
			int pickGrade = (int)value.PickGrade;
			lotteries[pickGrade].AddCase(value.Ratio, value);
		}
	}

	public RandomUnitTempletV2 Decide(NKM_UNIT_PICK_GRADE pickGrade)
	{
		return lotteries[(int)pickGrade].Decide();
	}

	public IEnumerable<RandomUnitTempletV2> GetBonusCandidates()
	{
		RatioLottery<RandomUnitTempletV2> ratioLottery = lotteries[7];
		if (ratioLottery == null)
		{
			return Array.Empty<RandomUnitTempletV2>();
		}
		return ratioLottery;
	}

	public RatioLottery<RandomUnitTempletV2> GetLottery(NKM_UNIT_PICK_GRADE pickGrade)
	{
		return lotteries[(int)pickGrade];
	}

	public void Recalculate()
	{
		for (int i = 0; i < lotteries.Length; i++)
		{
			lotteries[i] = new RatioLottery<RandomUnitTempletV2>();
		}
		unitTemplets.Clear();
		foreach (RandomUnitTempletV2 unitTemplet in sourceTemplet.UnitTemplets)
		{
			unitTemplets.Add(unitTemplet.UnitStringId, unitTemplet.Clone());
		}
		if (IsValidUnitByTag(addUnit))
		{
			unitTemplets[addUnit.UnitStringId] = addUnit;
		}
		foreach (RandomUnitTempletV2 value in unitTemplets.Values)
		{
			value.Join();
			int pickGrade = (int)value.PickGrade;
			lotteries[pickGrade].AddCase(value.Ratio, value);
		}
	}

	private static bool IsValidUnitByTag(RandomUnitTempletV2 unit)
	{
		if ((!unit.PickUpTarget || !unit.UnitTemplet.PickupEnableByTag) && (!unit.RatioUpTarget || !unit.UnitTemplet.PickupEnableByTag))
		{
			if (!unit.PickUpTarget && !unit.RatioUpTarget)
			{
				return unit.UnitTemplet.ContractEnableByTag;
			}
			return false;
		}
		return true;
	}
}
