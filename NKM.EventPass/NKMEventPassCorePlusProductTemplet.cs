using NKM.Templet;

namespace NKM.EventPass;

public class NKMEventPassCorePlusProductTemplet
{
	public string StrId { get; private set; }

	public string DescStrId { get; private set; }

	public NKM_REWARD_TYPE PriceType { get; private set; }

	public int PriceId { get; private set; }

	public int PriceCount { get; private set; }

	public int PassExp { get; private set; }

	public static NKMEventPassCorePlusProductTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		string rValue = string.Empty;
		cNKMLua.GetData("CorePassPlusStrID", ref rValue);
		string rValue2 = string.Empty;
		bool data = cNKMLua.GetData("CorePassPlusDescStrID", ref rValue2);
		NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
		bool num = data & cNKMLua.GetData("CorePassPlusPriceType", ref result);
		int rValue3 = 0;
		bool num2 = num & cNKMLua.GetData("CorePassPlusPriceID", ref rValue3);
		int rValue4 = 0;
		bool num3 = num2 & cNKMLua.GetData("CorePassPlusPriceCount", ref rValue4);
		int rValue5 = 0;
		if (!(num3 & cNKMLua.GetData("CorePassPlusExp", ref rValue5)))
		{
			return null;
		}
		return new NKMEventPassCorePlusProductTemplet
		{
			StrId = rValue,
			DescStrId = rValue2,
			PriceType = result,
			PriceId = rValue3,
			PriceCount = rValue4,
			PassExp = rValue5
		};
	}
}
