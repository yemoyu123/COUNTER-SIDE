using Cs.Logging;
using NKM;

namespace NKC;

public static class NKCClientConst
{
	public static float DiveAutoSpeed { get; private set; } = 1f;

	public static float NextTalkChangeSpeedWhenAuto_Fast { get; private set; } = 1f;

	public static float NextTalkChangeSpeedWhenAuto_Normal { get; private set; } = 1.2f;

	public static float NextTalkChangeSpeedWhenAuto_Slow { get; private set; } = 1.4f;

	public static void LoadFromLUA(string fileName)
	{
		bool flag = true;
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath("AB_SCRIPT", fileName))
			{
				Log.ErrorAndExit("fail loading lua file:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCClientConst.cs", 24);
				return;
			}
			using (nKMLua.OpenTable("Dive", "[ClientConst] loading Dive table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCClientConst.cs", 28))
			{
				float rValue = 1f;
				flag &= nKMLua.GetData("fAutoSpeed", ref rValue);
				DiveAutoSpeed = rValue;
			}
			using (nKMLua.OpenTable("Cutscen", "[ClientConst] loading Cutscen table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCClientConst.cs", 36))
			{
				float rValue2 = 1f;
				flag &= nKMLua.GetData("fNextTalkChangeSpeedWhenAuto_Fast", ref rValue2);
				NextTalkChangeSpeedWhenAuto_Fast = rValue2;
				flag &= nKMLua.GetData("fNextTalkChangeSpeedWhenAuto_Normal", ref rValue2);
				NextTalkChangeSpeedWhenAuto_Normal = rValue2;
				flag &= nKMLua.GetData("fNextTalkChangeSpeedWhenAuto_Slow", ref rValue2);
				NextTalkChangeSpeedWhenAuto_Slow = rValue2;
			}
		}
		if (!flag)
		{
			Log.ErrorAndExit("fail loading lua file:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCClientConst.cs", 53);
		}
	}
}
