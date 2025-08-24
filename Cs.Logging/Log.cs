using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Cs.Logging;

public static class Log
{
	public static void Info(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		UnityEngine.Debug.Log(message + " (" + Path.GetFileName(file) + ":" + line + ")");
	}

	public static void Info(string message, Object context, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		UnityEngine.Debug.Log(message + " (" + Path.GetFileName(file) + ":" + line + ")", context);
	}

	public static void Debug(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		UnityEngine.Debug.Log(message + " (" + Path.GetFileName(file) + ":" + line + ")");
	}

	public static void Debug(string message, Object context, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		UnityEngine.Debug.Log(message + " (" + Path.GetFileName(file) + ":" + line + ")", context);
	}

	public static void DebugBold(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		Debug(message, file, line);
	}

	public static void Warn(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		UnityEngine.Debug.LogWarning(message + " (" + Path.GetFileName(file) + ":" + line + ")");
	}

	public static void Warn(string message, Object context, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		UnityEngine.Debug.LogWarning(message + " (" + Path.GetFileName(file) + ":" + line + ")", context);
	}

	public static void Error(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		UnityEngine.Debug.LogError(message + " (" + Path.GetFileName(file) + ":" + line + ")");
	}

	public static void Error(string message, Object context, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		UnityEngine.Debug.LogError(message + " (" + Path.GetFileName(file) + ":" + line + ")", context);
	}

	public static void ErrorFormat(string message, params object[] args)
	{
		UnityEngine.Debug.LogErrorFormat(message ?? "", args);
	}

	public static void ErrorAndExit(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		UnityEngine.Debug.LogError(message + " (" + Path.GetFileName(file) + ":" + line + ")");
	}
}
