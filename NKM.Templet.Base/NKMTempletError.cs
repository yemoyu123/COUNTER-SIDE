using System.Runtime.CompilerServices;
using Cs.Logging;

namespace NKM.Templet.Base;

public static class NKMTempletError
{
	private static int errorCount;

	public static bool HasError => errorCount > 0;

	public static void Add(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		Log.Error(message, file, line);
		errorCount++;
	}

	public static void Validate()
	{
		if (errorCount > 0)
		{
			Log.ErrorAndExit($"[Templet] 로딩 중 {errorCount}개의 오류 확인", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletError.cs", 22);
		}
	}
}
