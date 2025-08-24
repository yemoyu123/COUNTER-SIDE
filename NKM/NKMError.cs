using System.Runtime.CompilerServices;
using Cs.Logging;

namespace NKM;

public static class NKMError
{
	public static NKM_ERROR_CODE Build(NKM_ERROR_CODE code, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (code == NKM_ERROR_CODE.NEC_OK)
		{
			return code;
		}
		Log.Error($"[{code}] {message}", file, line);
		return code;
	}

	public static NKM_ERROR_CODE Build(NKM_ERROR_CODE code, long userUid, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (code == NKM_ERROR_CODE.NEC_OK)
		{
			return code;
		}
		Log.Error($"[{code}] [userUid:{userUid}] {message}", file, line);
		return code;
	}

	public static NKM_ERROR_CODE Build(NKM_ERROR_CODE code, long userUid, long gameUid, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (code == NKM_ERROR_CODE.NEC_OK)
		{
			return code;
		}
		Log.Error($"[{code}] [userUid:{userUid}] [gameUid:{gameUid} {message}", file, line);
		return code;
	}

	public static NKM_ERROR_CODE Build(NKM_ERROR_CODE code, NKMUserData userData, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (code == NKM_ERROR_CODE.NEC_OK)
		{
			return code;
		}
		Log.Error($"[{code}] [userUid:{userData?.m_UserUID}] {message}", file, line);
		return code;
	}

	public static NKM_ERROR_CODE Build(NKM_ERROR_CODE code, NKMUserData userData, NKMGameData gameData, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (code == NKM_ERROR_CODE.NEC_OK)
		{
			return code;
		}
		Log.Error($"[{code}] [userUid:{userData?.m_UserUID}] [gameUid:{gameData?.m_GameUID}] {message}", file, line);
		return code;
	}

	public static NKM_ERROR_CODE Warning(NKM_ERROR_CODE code, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (code == NKM_ERROR_CODE.NEC_OK)
		{
			return code;
		}
		Log.Warn($"[{code}] {message}", file, line);
		return code;
	}

	public static NKM_ERROR_CODE Warning(NKM_ERROR_CODE code, long userUid, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (code == NKM_ERROR_CODE.NEC_OK)
		{
			return code;
		}
		Log.Warn($"[{code}] [userUid:{userUid}] {message}", file, line);
		return code;
	}

	public static NKM_ERROR_CODE Warning(NKM_ERROR_CODE code, long userUid, long gameUid, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (code == NKM_ERROR_CODE.NEC_OK)
		{
			return code;
		}
		Log.Warn($"[{code}] [userUid:{userUid}] [gameUid:{gameUid} {message}", file, line);
		return code;
	}

	public static NKM_ERROR_CODE Warning(NKM_ERROR_CODE code, NKMUserData userData, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (code == NKM_ERROR_CODE.NEC_OK)
		{
			return code;
		}
		Log.Warn($"[{code}] [userUid:{userData?.m_UserUID}] {message}", file, line);
		return code;
	}

	public static NKM_ERROR_CODE Warning(NKM_ERROR_CODE code, NKMUserData userData, NKMGameData gameData, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (code == NKM_ERROR_CODE.NEC_OK)
		{
			return code;
		}
		Log.Warn($"[{code}] [userUid:{userData?.m_UserUID}] [gameUid:{gameData?.m_GameUID}] {message}", file, line);
		return code;
	}
}
