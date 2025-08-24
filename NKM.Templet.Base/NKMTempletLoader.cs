using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;

namespace NKM.Templet.Base;

public static class NKMTempletLoader<T> where T : INKMTemplet
{
	private static readonly string templetTypeName = typeof(T).Name;

	public static Dictionary<int, List<T>> LoadGroup(string assetName, string fileName, string tableName, Func<NKMLua, T> factory)
	{
		IEnumerable<T> source = NKMTempletLoader.LoadCommonPath(assetName, fileName, tableName, factory);
		try
		{
			return (from e in source
				group e by e.Key).ToDictionary((IGrouping<int, T> e) => e.Key, (IGrouping<int, T> e) => e.ToList());
		}
		catch (Exception ex)
		{
			Log.ErrorAndExit("[" + templetTypeName + "] tableName:" + tableName + " exception:" + ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletLoader.cs", 22);
		}
		return null;
	}

	public static Dictionary<int, IReadOnlyList<T>> LoadReadOnlyGroup(string assetName, string fileName, string tableName, Func<NKMLua, T> factory)
	{
		IEnumerable<T> source = NKMTempletLoader.LoadCommonPath(assetName, fileName, tableName, factory);
		try
		{
			return (from e in source
				group e by e.Key).ToDictionary((Func<IGrouping<int, T>, int>)((IGrouping<int, T> e) => e.Key), (Func<IGrouping<int, T>, IReadOnlyList<T>>)((IGrouping<int, T> e) => e.ToList()));
		}
		catch (Exception ex)
		{
			Log.ErrorAndExit("[" + templetTypeName + "] tableName:" + tableName + " exception:" + ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletLoader.cs", 38);
		}
		return null;
	}
}
public static class NKMTempletLoader
{
	public static Dictionary<int, T> LoadDictionary<T>(string assetName, string fileName, string tableName, Func<NKMLua, T> factory) where T : INKMTemplet
	{
		string name = typeof(T).Name;
		IEnumerable<T> enumerable = LoadCommonPath(assetName, fileName, tableName, factory);
		T[] array = (enumerable as T[]) ?? enumerable.ToArray();
		Dictionary<int, T> dictionary = new Dictionary<int, T>(array.Length);
		try
		{
			T[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				T value = array2[i];
				dictionary.Add(value.Key, value);
			}
			return dictionary;
		}
		catch (Exception ex)
		{
			IEnumerable<int> values = from e in array
				group e by e.Key into e
				where e.Count() > 1
				select e.Key;
			string text = string.Join(", ", values);
			Log.ErrorAndExit("[" + name + "] tableName:" + tableName + " exception:" + ex.Message + " 중복키목록:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletLoader.cs", 68);
		}
		return null;
	}

	public static Dictionary<int, T> LoadDictionary<T>(string assetName, string[] fileNames, string tableName, Func<NKMLua, T> factory) where T : INKMTemplet
	{
		string name = typeof(T).Name;
		Dictionary<int, T> dictionary = new Dictionary<int, T>(500);
		foreach (string text in fileNames)
		{
			foreach (T item in LoadCommonPath(assetName, text, tableName, factory))
			{
				if (dictionary.ContainsKey(item.Key))
				{
					Log.ErrorAndExit("[" + name + "] table cotnains duplicate key. fileName:" + text + " tableName:" + tableName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletLoader.cs", 87);
				}
				dictionary.Add(item.Key, item);
			}
		}
		return dictionary;
	}

	public static IEnumerable<T> LoadCommonPath<T>(string assetName, string fileName, string tableName, Func<NKMLua, T> factory)
	{
		string name = typeof(T).Name;
		using NKMLua lua = new NKMLua();
		if (!lua.LoadCommonPath(assetName, fileName))
		{
			Log.ErrorAndExit("[" + name + "] lua file loading fail. assetName:" + assetName + " fileName:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletLoader.cs", 105);
			yield break;
		}
		if (!lua.OpenTable(tableName))
		{
			Log.ErrorAndExit("[" + name + "] lua table open fail. fileName:" + fileName + " tableName:" + tableName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletLoader.cs", 111);
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

	public static IEnumerable<T> LoadCommonPath<T>(string assetName, List<string> lstFileName, string tableName, Func<NKMLua, T> factory) where T : class
	{
		string templetTypeName = typeof(T).Name;
		foreach (string item in lstFileName)
		{
			using NKMLua lua = new NKMLua();
			if (!lua.LoadCommonPath(assetName, item))
			{
				Log.ErrorAndExit("[" + templetTypeName + "] lua file loading fail. assetName:" + assetName + " fileName:" + item, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletLoader.cs", 142);
				yield break;
			}
			if (!lua.OpenTable(tableName))
			{
				Log.ErrorAndExit("[" + templetTypeName + "] lua table open fail. fileName:" + item + " tableName:" + tableName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletLoader.cs", 148);
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
	}

	public static IEnumerable<T> LoadServerPath<T>(string assetName, string fileName, string tableName, Func<NKMLua, T> factory)
	{
		string name = typeof(T).Name;
		using NKMLua lua = new NKMLua();
		if (!lua.LoadServerPath(assetName, fileName))
		{
			Log.ErrorAndExit("[" + name + "] lua file loading fail. assetName:" + assetName + " fileName:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletLoader.cs", 177);
			yield break;
		}
		if (!lua.OpenTable(tableName))
		{
			Log.ErrorAndExit("[" + name + "] lua table open fail. fileName:" + fileName + " tableName:" + tableName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Base/NKMTempletLoader.cs", 183);
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

	public static void Load<T>(string assetName, string tableName, Func<NKMLua, T> factory, params string[] fileNames) where T : class, INKMTemplet
	{
		foreach (string fileName in fileNames)
		{
			NKMTempletContainer<T>.TryAppend(assetName, fileName, tableName, factory);
		}
	}
}
