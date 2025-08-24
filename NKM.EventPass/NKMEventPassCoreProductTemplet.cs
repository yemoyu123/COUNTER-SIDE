using NKM.Templet;

namespace NKM.EventPass;

public sealed class NKMEventPassCoreProductTemplet
{
	public string StrId { get; private set; }

	public string DescStrId { get; private set; }

	public NKM_REWARD_TYPE PriceType { get; private set; }

	public int PriceId { get; private set; }

	public int PriceCount { get; private set; }

	public static NKMEventPassCoreProductTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		string rValue = string.Empty;
		cNKMLua.GetData("CorePassStrID", ref rValue);
		string rValue2 = string.Empty;
		bool data = cNKMLua.GetData("CorePassDescStrID", ref rValue2);
		NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
		bool num = data & cNKMLua.GetData("CorePassPriceType", ref result);
		int rValue3 = 0;
		bool num2 = num & cNKMLua.GetData("CorePassPriceID", ref rValue3);
		int rValue4 = 0;
		if (!(num2 & cNKMLua.GetData("CorePassPriceCount", ref rValue4)))
		{
			return null;
		}
		return new NKMEventPassCoreProductTemplet
		{
			StrId = rValue,
			DescStrId = rValue2,
			PriceType = result,
			PriceId = rValue3,
			PriceCount = rValue4
		};
	}
}
