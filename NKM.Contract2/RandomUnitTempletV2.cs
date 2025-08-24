using System;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Contract2;

public sealed class RandomUnitTempletV2
{
	public static readonly string TextTableHeader;

	private readonly bool pickUpTarget;

	private readonly bool ratioUpTarget;

	private readonly bool customPickupTarget;

	public string UnitStringId { get; }

	public int Ratio { get; }

	public float FinalRatePercent { get; private set; }

	public NKMUnitTempletBase UnitTemplet { get; private set; }

	public NKM_UNIT_PICK_GRADE PickGrade { get; private set; }

	public bool PickUpTarget => pickUpTarget;

	public bool RatioUpTarget => ratioUpTarget;

	public bool CustomPickupTarget => customPickupTarget;

	static RandomUnitTempletV2()
	{
		TextTableHeader = string.Join(" | ", "No".PadRight(5), "Name".PadRight(30), "Grade".PadRight(5), "Ratio".PadRight(5), "FinalRate".PadRight(10));
	}

	public RandomUnitTempletV2(string unitStringId, int ratio, bool pickUpTaget, bool ratioUpTarget, bool customPickupTarget)
	{
		UnitStringId = unitStringId;
		Ratio = ratio;
		pickUpTarget = pickUpTaget;
		this.ratioUpTarget = ratioUpTarget;
		this.customPickupTarget = customPickupTarget;
	}

	public void Join()
	{
		UnitTemplet = NKMUnitManager.GetUnitTempletBase(UnitStringId);
		if (UnitTemplet == null)
		{
			NKMTempletError.Add("[RandomUnitTemplet] invalid unitId:" + UnitStringId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomUnitTempletV2.cs", 49);
		}
		else
		{
			PickGrade = ToPickGrade(UnitTemplet.m_NKM_UNIT_GRADE, pickUpTarget);
		}
	}

	public void Validate()
	{
		if (PickUpTarget && CustomPickupTarget)
		{
			NKMTempletError.Add($"[RandomUnitTemplet] PickupTarget and CustomPickupTarget opened unitType:{UnitTemplet.m_NKM_UNIT_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomUnitTempletV2.cs", 60);
		}
		else if (UnitTemplet.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL && UnitTemplet.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			NKMTempletError.Add($"[RandomUnitTemplet] invalid unitType:{UnitTemplet.m_NKM_UNIT_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomUnitTempletV2.cs", 67);
		}
	}

	public void CalcFinalRate(float gradeRatePercent, int unitTotalRatio)
	{
		float num = (float)Ratio * 100f / (float)unitTotalRatio;
		FinalRatePercent = gradeRatePercent * num * 0.01f;
	}

	private static NKM_UNIT_PICK_GRADE ToPickGrade(NKM_UNIT_GRADE unitGrade, bool pickup)
	{
		switch (unitGrade)
		{
		case NKM_UNIT_GRADE.NUG_N:
			if (!pickup)
			{
				return NKM_UNIT_PICK_GRADE.NUPG_N;
			}
			return NKM_UNIT_PICK_GRADE.NUPG_N_PICK;
		case NKM_UNIT_GRADE.NUG_R:
			if (!pickup)
			{
				return NKM_UNIT_PICK_GRADE.NUPG_R;
			}
			return NKM_UNIT_PICK_GRADE.NUPG_R_PICK;
		case NKM_UNIT_GRADE.NUG_SR:
			if (!pickup)
			{
				return NKM_UNIT_PICK_GRADE.NUPG_SR;
			}
			return NKM_UNIT_PICK_GRADE.NUPG_SR_PICK;
		case NKM_UNIT_GRADE.NUG_SSR:
			if (!pickup)
			{
				return NKM_UNIT_PICK_GRADE.NUPG_SSR;
			}
			return NKM_UNIT_PICK_GRADE.NUPG_SSR_PICK;
		default:
			throw new Exception($"[RandomUnitPool] invalid unitGrade:{unitGrade}");
		}
	}

	public string ToReadableString(int order)
	{
		NKMUnitTempletBase unitTemplet = UnitTemplet;
		string text = string.Format("[{0}] {1}", unitTemplet.Key, unitTemplet.m_UnitStrID.Replace("NKM_UNIT_", string.Empty));
		return $"{order,5} | {text,-30} | {unitTemplet.m_NKM_UNIT_GRADE.ToString().Substring(4),5} | {Ratio,5} | {FinalRatePercent,9:0.000}%";
	}

	internal RandomUnitTempletV2 Clone()
	{
		return MemberwiseClone() as RandomUnitTempletV2;
	}
}
