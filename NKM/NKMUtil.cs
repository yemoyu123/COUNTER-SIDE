using System;
using System.Collections.Generic;
using Cs.Logging;

namespace NKM;

public static class NKMUtil
{
	public static bool IsServer => false;

	public static bool TryParse<T>(this string data, out T @enum, bool bSkipError = false) where T : Enum
	{
		Type typeFromHandle = typeof(T);
		try
		{
			object obj = Enum.Parse(typeFromHandle, data);
			@enum = (T)obj;
			return true;
		}
		catch (Exception ex)
		{
			if (!bSkipError)
			{
				Log.Error($"GetStringToEnum Fail. enumType:{typeFromHandle} data:{data} message:{ex.Message}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUtil.cs", 130);
			}
		}
		@enum = default(T);
		return false;
	}

	public static List<T> LoadListFromLua<T>(NKMLua lua, string tableName, Func<NKMLua, T> factory, bool bNullIfEmpty)
	{
		if (!lua.OpenTable(tableName))
		{
			if (bNullIfEmpty)
			{
				return null;
			}
			return new List<T>();
		}
		List<T> list = new List<T>();
		int num = 1;
		while (lua.OpenTable(num))
		{
			T val = factory(lua);
			if (val != null)
			{
				list.Add(val);
			}
			num++;
			lua.CloseTable();
		}
		lua.CloseTable();
		return list;
	}

	public static IEnumerable<T> LoadTableFromLua<T>(NKMLua lua, string tableName, Func<NKMLua, T> factory)
	{
		if (!lua.OpenTable(tableName))
		{
			yield break;
		}
		int index = 1;
		while (lua.OpenTable(index))
		{
			T val = factory(lua);
			if (val != null)
			{
				yield return val;
			}
			index++;
			lua.CloseTable();
		}
		lua.CloseTable();
	}

	public static float NKMToRadian(float fDegree)
	{
		return fDegree * ((float)Math.PI / 180f);
	}

	public static float NKMToDegree(float fRadian)
	{
		return fRadian * (180f / (float)Math.PI);
	}

	public static ushort FloatToHalf(float fValue)
	{
		if (fValue > 50000f)
		{
			fValue = 50000f;
		}
		if (fValue < -50000f)
		{
			fValue = -50000f;
		}
		return HalfHelper.SingleToHalf(fValue).value;
	}

	public static void SimpleEncrypt(byte encryptSeed, ref long target, long value)
	{
		target = value + encryptSeed;
	}

	public static long SimpleDecrypt(byte encryptSeed, long target)
	{
		return target - encryptSeed;
	}

	public static void SimpleEncrypt(byte encryptSeed, ref int target, int value)
	{
		target = value + encryptSeed;
	}

	public static int SimpleDecrypt(byte encryptSeed, int target)
	{
		return target - encryptSeed;
	}

	public static Dictionary<string, bool> ParseStringTable(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return new Dictionary<string, bool>();
		}
		char[] separator = new char[1] { ';' };
		char[] separator2 = new char[1] { '=' };
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		string[] array = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			string[] array2 = text.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length != 2)
			{
				Log.Error("Table parse error : 2 or more = in single statement", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUtil.cs", 254);
				continue;
			}
			string key = array2[0].Trim();
			string text2 = array2[1].Trim();
			if (!bool.TryParse(text2, out var result))
			{
				Log.Error("TryParse Fail. parameter:" + text2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUtil.cs", 263);
			}
			else
			{
				dictionary.Add(key, result);
			}
		}
		return dictionary;
	}
}
