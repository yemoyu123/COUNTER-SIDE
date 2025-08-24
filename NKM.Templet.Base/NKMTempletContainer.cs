using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;

namespace NKM.Templet.Base;

public static class NKMTempletContainer<T> where T : class, INKMTemplet
{
	private static readonly string templetTypeName = typeof(T).Name;

	private static Dictionary<int, T> data = new Dictionary<int, T>();

	private static Dictionary<string, T> strData = new Dictionary<string, T>();

	public static IEnumerable<T> Values => data.Values;

	public static IEnumerable<int> Keys => data.Keys;

	public static IEnumerable<string> StrKeys => strData.Keys;

	public static void Load(string assetName, string fileName, string tableName, Func<NKMLua, T> factory)
	{
		data = NKMTempletLoader.LoadDictionary(assetName, fileName, tableName, factory);
	}

	public static void TryAppend(string assetName, string fileName, string tableName, Func<NKMLua, T> factory)
	{
		string name = typeof(T).Name;
		Log.Debug("[" + name + "] append additional data file:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletContainer.cs", 28);
		foreach (KeyValuePair<int, T> item in NKMTempletLoader.LoadDictionary(assetName, fileName, tableName, factory))
		{
			if (data.ContainsKey(item.Key))
			{
				NKMTempletError.Add($"[{name}] 추가 로딩 파일에 중복키 정의가 존재. key:{item.Key} filename:{fileName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletContainer.cs", 35);
			}
			else
			{
				data.Add(item.Key, item.Value);
			}
		}
	}

	public static void Load(string assetName, string[] fileNames, string tableName, Func<NKMLua, T> factory)
	{
		data = NKMTempletLoader.LoadDictionary(assetName, fileNames, tableName, factory);
	}

	public static void Load(string assetName, string[] fileNames, string tableName, Func<NKMLua, T> factory, Func<T, string> strKeySelector)
	{
		data = NKMTempletLoader.LoadDictionary(assetName, fileNames, tableName, factory);
		try
		{
			strData.Clear();
			strData = data.Values.ToDictionary(strKeySelector);
		}
		catch (Exception ex)
		{
			NKMTempletError.Add("[" + templetTypeName + "] Table contains duplicate string key. tableName:" + tableName + " exception:" + ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletContainer.cs", 59);
		}
	}

	public static void Load(string assetName, string fileName, string tableName, Func<NKMLua, T> factory, Func<T, string> strKeySelector)
	{
		Load(assetName, fileName, tableName, factory);
		try
		{
			strData.Clear();
			strData = data.Values.ToDictionary(strKeySelector);
		}
		catch (Exception ex)
		{
			NKMTempletError.Add("[" + templetTypeName + "] Table contains duplicate string key. tableName:" + tableName + " exception:" + ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletContainer.cs", 74);
		}
	}

	public static void Load(IEnumerable<T> list, Func<T, string> strKeySelector)
	{
		try
		{
			data.Clear();
			data = list.ToDictionary((T e) => e.Key);
			if (strKeySelector != null)
			{
				strData.Clear();
				strData = list.ToDictionary(strKeySelector);
			}
		}
		catch (Exception ex)
		{
			NKMTempletError.Add(ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletContainer.cs", 93);
		}
	}

	public static void Add(T templet, Func<T, string> strKeySelector)
	{
		try
		{
			if (data.ContainsKey(templet.Key))
			{
				NKMTempletError.Add($"[{typeof(T).Name}] 중복된 키가 존재. key:{templet.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletContainer.cs", 103);
			}
			data.Add(templet.Key, templet);
			if (strKeySelector != null)
			{
				string text = strKeySelector(templet);
				if (strData.ContainsKey(text))
				{
					NKMTempletError.Add($"[{typeof(T).Name}] 중복된 텍스트키가 존재. key:{templet.Key} strKey:{text}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletContainer.cs", 113);
				}
				strData.Add(text, templet);
			}
		}
		catch (Exception ex)
		{
			NKMTempletError.Add("[" + typeof(T).Name + "] " + ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletContainer.cs", 121);
		}
	}

	public static void AddRange(IEnumerable<T> list, Func<T, string> strKeySelector)
	{
		try
		{
			foreach (T item in list)
			{
				data.Add(item.Key, item);
			}
			if (strKeySelector == null)
			{
				return;
			}
			foreach (T item2 in list)
			{
				strData.Add(strKeySelector(item2), item2);
			}
		}
		catch (Exception ex)
		{
			NKMTempletError.Add(ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletContainer.cs", 144);
		}
	}

	public static bool TryGetValue(int key, out T result)
	{
		return data.TryGetValue(key, out result);
	}

	public static T Find(int key)
	{
		data.TryGetValue(key, out var value);
		return value;
	}

	public static T Find(string key)
	{
		strData.TryGetValue(key, out var value);
		return value;
	}

	public static T Find(Func<T, bool> predicate)
	{
		return data.Values.FirstOrDefault(predicate);
	}

	public static void Join()
	{
		foreach (T value in Values)
		{
			value.Join();
		}
	}

	public static void Validate()
	{
		foreach (T value in Values)
		{
			value.Validate();
		}
	}

	public static void SetForTest(int key, T value)
	{
		data[key] = value;
	}

	public static void Drop()
	{
		data.Clear();
		strData.Clear();
	}

	public static bool HasValue()
	{
		if (data != null)
		{
			return data.Count > 0;
		}
		return false;
	}

	public static void PostJoin()
	{
		foreach (INKMTempletEx value in Values)
		{
			value.PostJoin();
		}
	}
}
