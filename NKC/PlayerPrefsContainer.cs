using UnityEngine;

namespace NKC;

public static class PlayerPrefsContainer
{
	public static void Set(string key, int value)
	{
		PlayerPrefs.SetInt(key, value);
		PlayerPrefs.Save();
	}

	public static void Set(string key, float value)
	{
		PlayerPrefs.SetFloat(key, value);
		PlayerPrefs.Save();
	}

	public static void Set(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
		PlayerPrefs.Save();
	}

	public static void Set(string key, bool value)
	{
		PlayerPrefs.SetInt(key, value ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static int GetInt(string key)
	{
		return PlayerPrefs.GetInt(key);
	}

	public static string GetString(string key)
	{
		return PlayerPrefs.GetString(key);
	}

	public static bool GetBoolean(string key)
	{
		return PlayerPrefs.GetInt(key) > 0;
	}
}
