using Cs.Math.Lottery;
using NKM.Templet.Base;

namespace NKM.Contract2;

public sealed class RandomGradeTempletV2 : INKMTemplet
{
	private readonly RateLottery<NKM_UNIT_PICK_GRADE> lottery = new RateLottery<NKM_UNIT_PICK_GRADE>(NKM_UNIT_PICK_GRADE.NUPG_N);

	public int Key { get; }

	public string StringId { get; }

	public IReadOnlyRateLottery<NKM_UNIT_PICK_GRADE> Lottery => lottery;

	public float FinalRateSsrPercent { get; private set; }

	public float FinalRateSrPercent { get; private set; }

	public float FinalRateRPercent { get; private set; }

	public float FinalRateNPercent { get; private set; }

	public RandomGradeTempletV2(int key, string stringId)
	{
		Key = key;
		StringId = stringId;
	}

	public static RandomGradeTempletV2 Find(string key)
	{
		return NKMTempletContainer<RandomGradeTempletV2>.Find(key);
	}

	public static RandomGradeTempletV2 Find(int key)
	{
		return NKMTempletContainer<RandomGradeTempletV2>.Find(key);
	}

	public static RandomGradeTempletV2 LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomGradeTempletV2.cs", 43))
		{
			return null;
		}
		if (!lua.GetData("m_RandomGradeID", out var rValue, 0))
		{
			NKMTempletError.Add($"[RandomGrade] loading key failed. id:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomGradeTempletV2.cs", 50);
			return null;
		}
		if (!lua.GetData("m_RandomGradeStrID", out var rValue2, null))
		{
			NKMTempletError.Add($"[RandomGrade] loading key failed. id:{rValue} strId:{rValue2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomGradeTempletV2.cs", 56);
			return null;
		}
		RandomGradeTempletV2 randomGradeTempletV = new RandomGradeTempletV2(rValue, rValue2);
		lua.GetData("Rate_SSR", out var rValue3, 0);
		lua.GetData("Rate_SR", out var rValue4, 0);
		lua.GetData("Rate_R", out var rValue5, 0);
		lua.GetData("Rate_N", out var rValue6, 0);
		lua.GetData("Rate_Pick_SSR", out var rValue7, 0);
		lua.GetData("Rate_Pick_SR", out var rValue8, 0);
		lua.GetData("Rate_Pick_R", out var rValue9, 0);
		lua.GetData("Rate_Pick_N", out var rValue10, 0);
		randomGradeTempletV.lottery.AddCase(rValue3, NKM_UNIT_PICK_GRADE.NUPG_SSR);
		randomGradeTempletV.lottery.AddCase(rValue4, NKM_UNIT_PICK_GRADE.NUPG_SR);
		randomGradeTempletV.lottery.AddCase(rValue5, NKM_UNIT_PICK_GRADE.NUPG_R);
		randomGradeTempletV.lottery.AddCase(rValue6, NKM_UNIT_PICK_GRADE.NUPG_N);
		randomGradeTempletV.lottery.AddCase(rValue7, NKM_UNIT_PICK_GRADE.NUPG_SSR_PICK);
		randomGradeTempletV.lottery.AddCase(rValue8, NKM_UNIT_PICK_GRADE.NUPG_SR_PICK);
		randomGradeTempletV.lottery.AddCase(rValue9, NKM_UNIT_PICK_GRADE.NUPG_R_PICK);
		randomGradeTempletV.lottery.AddCase(rValue10, NKM_UNIT_PICK_GRADE.NUPG_N_PICK);
		randomGradeTempletV.FinalRateSsrPercent = (float)(rValue3 + rValue7) * 0.01f;
		randomGradeTempletV.FinalRateSrPercent = (float)(rValue4 + rValue8) * 0.01f;
		randomGradeTempletV.FinalRateRPercent = (float)(rValue5 + rValue9) * 0.01f;
		randomGradeTempletV.FinalRateNPercent = (float)(rValue6 + rValue10) * 0.01f;
		return randomGradeTempletV;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (!lottery.HasFullRate)
		{
			NKMTempletError.Add($"[{Key}]{StringId} 모든 등급 확률의 합이 10000이어야 함. 확률합:{lottery.TotalRate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomGradeTempletV2.cs", 97);
		}
	}
}
