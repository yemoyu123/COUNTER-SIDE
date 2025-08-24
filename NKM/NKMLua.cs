using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using AssetBundles;
using Cs.Logging;
using Cs.Memory;
using KeraLua;
using NKC;
using NKC.Converter;
using NKC.Patcher;
using NLua;
using UnityEngine;

namespace NKM;

public sealed class NKMLua : IDisposable
{
	private readonly NLua.Lua m_LuaSvr = new NLua.Lua();

	private readonly KeraLua.Lua state;

	private int m_TableDepth;

	private bool disposed;

	private string fileNameForDebug;

	private static int m_LUA_STATIC_BUF_SIZE = 2097152;

	private static byte[] m_LUA_STATIC_BUF = new byte[m_LUA_STATIC_BUF_SIZE];

	private static IStrConverter _converter = new EasyStrConverter();

	public NKMLua()
	{
		m_TableDepth = 0;
		state = m_LuaSvr.State;
		state.Encoding = Encoding.UTF8;
	}

	public bool DoString(string str)
	{
		m_LuaSvr.DoString(str);
		return true;
	}

	public void LuaClose()
	{
		if (!disposed)
		{
			m_LuaSvr.Dispose();
			disposed = true;
		}
	}

	public void Dispose()
	{
		LuaClose();
	}

	private bool Get(string name)
	{
		if (m_TableDepth > 0)
		{
			state.PushString(name);
			return state.GetTable(-2) != LuaType.Nil;
		}
		return state.GetGlobal(name) != LuaType.Nil;
	}

	private bool Get(int iIndex)
	{
		if (m_TableDepth <= 0)
		{
			return false;
		}
		state.PushNumber(iIndex);
		state.GetTable(-2);
		return true;
	}

	public bool OpenTable(string tableName)
	{
		if (Get(tableName) && state.IsTable(-1))
		{
			m_TableDepth++;
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool OpenTable(int iIndex)
	{
		if (Get(iIndex) && state.IsTable(-1))
		{
			m_TableDepth++;
			return true;
		}
		state.Pop(1);
		return false;
	}

	public IDisposable OpenTable(string tableName, string errorMessage, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (!OpenTable(tableName))
		{
			Log.ErrorAndExit(errorMessage, file, line);
			return null;
		}
		return new NKMLuaTableOpener(this);
	}

	public IDisposable OpenTable(int index, string errorMessage, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (!OpenTable(index))
		{
			Log.ErrorAndExit(errorMessage, file, line);
			return null;
		}
		return new NKMLuaTableOpener(this);
	}

	public bool CloseTable()
	{
		if (m_TableDepth > 0)
		{
			state.Pop(1);
			m_TableDepth--;
			return true;
		}
		return false;
	}

	public Dictionary<string, float> OpenTableAsDictionary(string tableName, string errorMessage, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (!OpenTable(tableName))
		{
			Log.ErrorAndExit(errorMessage, file, line);
			return null;
		}
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		int top = state.GetTop();
		state.PushNil();
		while (state.Next(-2))
		{
			if (!state.IsString(-2) || !state.IsNumber(-1))
			{
				Log.ErrorAndExit(errorMessage, file, line);
				return null;
			}
			string key = state.ToString(-2);
			float value = (float)state.ToNumber(-1);
			dictionary[key] = value;
			state.SetTop(-2);
		}
		state.SetTop(top);
		CloseTable();
		return dictionary;
	}

	public Dictionary<T, float> OpenTableAsDictionary<T>(string tableName, string errorMessage, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, bool bHaltOnFail = true) where T : Enum
	{
		if (!OpenTable(tableName))
		{
			if (bHaltOnFail)
			{
				Log.ErrorAndExit(errorMessage, file, line);
			}
			return null;
		}
		Dictionary<T, float> dictionary = new Dictionary<T, float>();
		int top = state.GetTop();
		state.PushNil();
		while (state.Next(-2))
		{
			if (!state.IsString(-2) || !state.IsNumber(-1))
			{
				Log.ErrorAndExit(errorMessage + " : table " + tableName + " has type mismatch", file, line);
				return null;
			}
			string text = state.ToString(-2);
			float value = (float)state.ToNumber(-1);
			if (!text.TryParse<T>(out var @enum))
			{
				Log.ErrorAndExit(errorMessage + " enum " + text + " parse failed", file, line);
				return null;
			}
			dictionary[@enum] = value;
			state.SetTop(-2);
		}
		state.SetTop(top);
		CloseTable();
		return dictionary;
	}

	public DateTime GetDateTime(string keyName)
	{
		if (!GetData(keyName, out var rValue, DateTime.MinValue))
		{
			Log.ErrorAndExit("get lua value failed. keyName:" + keyName + " filename:" + fileNameForDebug, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 225);
		}
		return rValue;
	}

	public DateTime GetDateTime(string keyName, DateTime defaultValue)
	{
		if (!GetData(keyName, out var rValue, DateTime.MinValue))
		{
			return defaultValue;
		}
		return rValue;
	}

	public TimeSpan GetTimeSpan(string keyName)
	{
		if (!GetData(keyName, out var rValue, TimeSpan.MinValue))
		{
			Log.ErrorAndExit("get lua value failed. keyName:" + keyName + " filename:" + fileNameForDebug, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 245);
		}
		return rValue;
	}

	public TimeSpan GetTimeSpan(string keyName, TimeSpan defaultValue)
	{
		if (!GetData(keyName, out var rValue, TimeSpan.MinValue))
		{
			return defaultValue;
		}
		return rValue;
	}

	public string GetString(string keyName)
	{
		if (!GetData(keyName, out var rValue, null))
		{
			Log.ErrorAndExit("get lua value failed. keyName:" + keyName + " filename:" + fileNameForDebug, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 265);
		}
		return rValue;
	}

	public bool GetBoolean(string keyName)
	{
		bool rbValue = false;
		if (!GetData(keyName, ref rbValue))
		{
			Log.ErrorAndExit("get lua value failed. keyName:" + keyName + " filename:" + fileNameForDebug, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 276);
		}
		return rbValue;
	}

	public int GetInt32(string keyName)
	{
		int rValue = 0;
		if (!GetData(keyName, ref rValue))
		{
			Log.ErrorAndExit("get lua value failed. keyName:" + keyName + " filename:" + fileNameForDebug, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 287);
		}
		return rValue;
	}

	public int GetInt32(string keyName, int defaultValue)
	{
		GetData(keyName, out var rValue, defaultValue);
		return rValue;
	}

	public string GetString(string keyName, string defaultValue)
	{
		GetData(keyName, out var rValue, defaultValue);
		return rValue;
	}

	public float GetFloat(string keyName, float defaultValue)
	{
		GetData(keyName, out var rValue, defaultValue);
		return rValue;
	}

	public bool GetBoolean(string keyName, bool defaultValue)
	{
		GetData(keyName, out var rbValue, defaultValue);
		return rbValue;
	}

	public long GetInt64(string keyName)
	{
		long rValue = 0L;
		if (!GetData(keyName, ref rValue))
		{
			Log.ErrorAndExit("get lua value failed. keyName:" + keyName + " filename:" + fileNameForDebug, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 322);
		}
		return rValue;
	}

	public T GetEnum<T>(string keyName) where T : Enum
	{
		if (!GetDataEnum<T>(keyName, out var result))
		{
			Log.ErrorAndExit("get lua value failed. keyName:" + keyName + " filename:" + fileNameForDebug, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 332);
		}
		return result;
	}

	public T GetEnum<T>(string keyName, T defaultValue) where T : Enum
	{
		if (!GetDataEnum<T>(keyName, out var result))
		{
			return defaultValue;
		}
		return result;
	}

	public float GetFloat(string keyName)
	{
		float rValue = 0f;
		if (!GetData(keyName, ref rValue))
		{
			Log.ErrorAndExit("get lua value failed. keyName:" + keyName + " filename:" + fileNameForDebug, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 353);
		}
		return rValue;
	}

	public bool GetData(string keyName, out bool rbValue, bool defValue)
	{
		rbValue = defValue;
		if (Get(keyName) && state.IsBoolean(-1))
		{
			rbValue = state.ToBoolean(-1);
			state.Pop(1);
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, out bool rbValue, bool defValue)
	{
		rbValue = defValue;
		if (Get(iIndex) && state.IsBoolean(-1))
		{
			rbValue = state.ToBoolean(-1);
			state.Pop(1);
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, out int rValue, int defValue)
	{
		rValue = defValue;
		if (Get(pszName) && state.IsNumber(-1))
		{
			rValue = (int)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 404);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, out int rValue, int defValue)
	{
		rValue = defValue;
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = (int)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 424);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, out short rValue, short defValue)
	{
		rValue = defValue;
		if (Get(pszName) && state.IsNumber(-1))
		{
			rValue = (short)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 444);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, out short rValue, short defValue)
	{
		rValue = defValue;
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = (short)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 464);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, out byte rValue, byte defValue)
	{
		rValue = defValue;
		if (Get(pszName) && state.IsNumber(-1))
		{
			rValue = (byte)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 484);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, out byte rValue, byte defValue)
	{
		rValue = defValue;
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = (byte)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 504);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, out long rValue, long defValue)
	{
		rValue = defValue;
		if (Get(pszName) && state.IsNumber(-1))
		{
			rValue = (long)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 524);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, out long rValue, long defValue)
	{
		rValue = defValue;
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = (long)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 544);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, out float rValue, float defValue)
	{
		rValue = defValue;
		if (Get(pszName) && state.IsNumber(-1))
		{
			rValue = (float)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 564);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, out float rValue, float defValue)
	{
		rValue = defValue;
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = (float)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 584);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, out double rValue, double defValue)
	{
		rValue = defValue;
		if (Get(pszName) && state.IsNumber(-1))
		{
			rValue = state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 604);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, out double rValue, double defValue)
	{
		rValue = defValue;
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 624);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, List<float> listFloat, int index)
	{
		if (Get(pszName) && state.IsNumber(-1))
		{
			listFloat[index] = (float)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 643);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, List<float> listFloat, int index)
	{
		if (Get(iIndex) && state.IsNumber(-1))
		{
			listFloat[index] = (float)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 662);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, out string rValue, string defValue)
	{
		rValue = defValue;
		if (Get(pszName) && state.IsString(-1))
		{
			rValue = string.Intern(state.ToString(-1));
			state.Pop(1);
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, out string rValue, string defValue)
	{
		rValue = defValue;
		if (Get(iIndex) && state.IsString(-1))
		{
			rValue = string.Intern(state.ToString(-1));
			state.Pop(1);
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, out DateTime rValue, DateTime defValue)
	{
		rValue = defValue;
		return GetData(pszName, ref rValue);
	}

	public bool GetData(string pszName, out TimeSpan rValue, TimeSpan defValue)
	{
		rValue = defValue;
		return GetData(pszName, ref rValue);
	}

	public bool GetData(string pszName, ICollection<int> listInt)
	{
		if (listInt == null)
		{
			return false;
		}
		if (OpenTable(pszName))
		{
			int i = 1;
			for (int rValue = 0; GetData(i, ref rValue); i++)
			{
				listInt.Add(rValue);
			}
			CloseTable();
			return true;
		}
		return false;
	}

	public bool GetData(string pszName, ICollection<string> listString)
	{
		if (listString == null)
		{
			return false;
		}
		if (OpenTable(pszName))
		{
			int i = 1;
			for (string rValue = ""; GetData(i, ref rValue); i++)
			{
				listString.Add(rValue);
			}
			CloseTable();
			return true;
		}
		return false;
	}

	public bool GetDataEnum<T>(string pszName, out T result) where T : Enum
	{
		Get(pszName);
		if (!state.IsString(-1))
		{
			state.Pop(1);
			result = default(T);
			return false;
		}
		string data = string.Intern(state.ToString(-1));
		state.Pop(1);
		return data.TryParse<T>(out result);
	}

	public bool GetDataEnum<T>(int iIndex, out T result) where T : Enum
	{
		if (Get(iIndex) && !state.IsString(-1))
		{
			state.Pop(1);
			result = default(T);
			return false;
		}
		string data = string.Intern(state.ToString(-1));
		state.Pop(1);
		return data.TryParse<T>(out result);
	}

	public bool GetData(string keyName, ref bool rbValue)
	{
		Get(keyName);
		if (state.IsBoolean(-1))
		{
			rbValue = state.ToBoolean(-1);
			state.Pop(1);
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, ref bool rbValue)
	{
		if (Get(iIndex) && state.IsBoolean(-1))
		{
			rbValue = state.ToBoolean(-1);
			state.Pop(1);
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, ref int rValue)
	{
		Get(pszName);
		if (state.IsNumber(-1))
		{
			rValue = (int)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 839);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, ref int rValue)
	{
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = (int)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 858);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, ref short rValue)
	{
		Get(pszName);
		if (state.IsNumber(-1))
		{
			rValue = (short)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 878);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, ref short rValue)
	{
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = (short)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 897);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, ref byte rValue)
	{
		Get(pszName);
		if (state.IsNumber(-1))
		{
			rValue = (byte)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 917);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, ref byte rValue)
	{
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = (byte)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 936);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, ref long rValue)
	{
		Get(pszName);
		if (state.IsNumber(-1))
		{
			rValue = (long)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 956);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, ref long rValue)
	{
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = (long)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 975);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, ref float rValue)
	{
		Get(pszName);
		if (state.IsNumber(-1))
		{
			rValue = (float)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 995);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, ref float rValue)
	{
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = (float)state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 1014);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, ref double rValue)
	{
		Get(pszName);
		if (state.IsNumber(-1))
		{
			rValue = state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + pszName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 1034);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, ref double rValue)
	{
		if (Get(iIndex) && state.IsNumber(-1))
		{
			rValue = state.ToNumber(-1);
			state.Pop(1);
			return true;
		}
		if (state.IsString(-1))
		{
			Log.Error("WrongType: " + iIndex, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 1053);
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, ref string rValue)
	{
		Get(pszName);
		if (state.IsStringOrNumber(-1))
		{
			rValue = string.Intern(state.ToString(-1));
			state.Pop(1);
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(int iIndex, ref string rValue)
	{
		if (Get(iIndex) && state.IsStringOrNumber(-1))
		{
			rValue = string.Intern(state.ToString(-1));
			state.Pop(1);
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, ref DateTime rValue)
	{
		Get(pszName);
		if (state.IsString(-1))
		{
			string text = string.Intern(state.ToString(-1));
			state.Pop(1);
			if (!DateTime.TryParse(text, out rValue))
			{
				Log.Error("invalid date format:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 1102);
				return false;
			}
			return true;
		}
		if (state.IsNumber(-1))
		{
			double d = state.ToNumber(-1);
			state.Pop(1);
			rValue = DateTime.FromOADate(d);
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool GetData(string pszName, ref TimeSpan rValue)
	{
		Get(pszName);
		if (state.IsString(-1))
		{
			string text = string.Intern(state.ToString(-1));
			state.Pop(1);
			if (!TimeSpan.TryParse(text, out rValue))
			{
				Log.Error("invalid date format:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 1133);
				return false;
			}
			return true;
		}
		state.Pop(1);
		return false;
	}

	public bool GetData<T>(string pszName, ref T result) where T : struct, Enum
	{
		Get(pszName);
		if (!state.IsString(-1))
		{
			state.Pop(1);
			return false;
		}
		string text = string.Intern(state.ToString(-1));
		state.Pop(1);
		if (!Enum.TryParse<T>(text, out var result2))
		{
			Log.Error("[" + typeof(T).Name + "] undefined type. key:" + pszName + " value:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMLua.cs", 1157);
			return false;
		}
		result = result2;
		return true;
	}

	public bool GetDataList(string pszName, out List<int> result, bool nullIfEmpty = false)
	{
		if (OpenTable(pszName))
		{
			result = new List<int>();
			int i = 1;
			for (int rValue = 0; GetData(i, ref rValue); i++)
			{
				result.Add(rValue);
			}
			CloseTable();
			return true;
		}
		if (nullIfEmpty)
		{
			result = null;
		}
		else
		{
			result = new List<int>();
		}
		return false;
	}

	public bool GetDataList(string pszName, out List<float> result, bool nullIfEmpty = false)
	{
		if (OpenTable(pszName))
		{
			result = new List<float>();
			int i = 1;
			for (float rValue = 0f; GetData(i, ref rValue); i++)
			{
				result.Add(rValue);
			}
			CloseTable();
			return true;
		}
		if (nullIfEmpty)
		{
			result = null;
		}
		else
		{
			result = new List<float>();
		}
		return false;
	}

	public bool GetDataList(string pszName, out List<string> result, bool nullIfEmpty = false)
	{
		if (OpenTable(pszName))
		{
			result = new List<string>();
			int i = 1;
			for (string rValue = ""; GetData(i, ref rValue); i++)
			{
				result.Add(rValue);
			}
			CloseTable();
			return true;
		}
		if (nullIfEmpty)
		{
			result = null;
		}
		else
		{
			result = new List<string>();
		}
		return false;
	}

	public bool GetDataList(string pszName, out HashSet<int> result, bool nullIfEmpty = false)
	{
		if (OpenTable(pszName))
		{
			result = new HashSet<int>();
			int i = 1;
			for (int rValue = 0; GetData(i, ref rValue); i++)
			{
				result.Add(rValue);
			}
			CloseTable();
			return true;
		}
		if (nullIfEmpty)
		{
			result = null;
		}
		else
		{
			result = new HashSet<int>();
		}
		return false;
	}

	public bool GetDataListEnum<T>(string pszName, ICollection<T> result, bool bClearList = true) where T : Enum
	{
		if (result == null)
		{
			return false;
		}
		if (bClearList)
		{
			result.Clear();
		}
		if (OpenTable(pszName))
		{
			T result2;
			for (int i = 1; GetDataEnum<T>(i, out result2); i++)
			{
				result.Add(result2);
			}
			CloseTable();
			return true;
		}
		return false;
	}

	public bool GetDataListEnum<T>(string pszName, out HashSet<T> result, bool nullIfEmpty = true) where T : Enum
	{
		if (OpenTable(pszName))
		{
			result = new HashSet<T>();
			T result2;
			for (int i = 1; GetDataEnum<T>(i, out result2); i++)
			{
				result.Add(result2);
			}
			CloseTable();
			return true;
		}
		if (nullIfEmpty)
		{
			result = null;
		}
		else
		{
			result = new HashSet<T>();
		}
		return false;
	}

	public bool GetDataListEnum<T>(string pszName, out List<T> result, bool nullIfEmpty = true) where T : Enum
	{
		if (OpenTable(pszName))
		{
			result = new List<T>();
			T result2;
			for (int i = 1; GetDataEnum<T>(i, out result2); i++)
			{
				result.Add(result2);
			}
			CloseTable();
			return true;
		}
		if (nullIfEmpty)
		{
			result = null;
		}
		else
		{
			result = new List<T>();
		}
		return false;
	}

	public bool GetExplicitEnum<T>(string pszName, ref T? result) where T : struct, Enum
	{
		Get(pszName);
		if (!state.IsString(-1))
		{
			state.Pop(1);
			return true;
		}
		string data = string.Intern(state.ToString(-1));
		state.Pop(1);
		if (!data.TryParse<T>(out var @enum))
		{
			return false;
		}
		result = @enum;
		return true;
	}

	public bool GetExplicitEnum<T>(int iIndex, ref T? result) where T : struct, Enum
	{
		if (!Get(iIndex) || !state.IsString(-1))
		{
			state.Pop(1);
			return false;
		}
		if (!state.IsString(-1))
		{
			state.Pop(1);
			return true;
		}
		string data = string.Intern(state.ToString(-1));
		state.Pop(1);
		if (!data.TryParse<T>(out var @enum))
		{
			return false;
		}
		result = @enum;
		return true;
	}

	public bool GetData<T>(string pszName, out T result, T defValue) where T : Enum
	{
		Get(pszName);
		if (!state.IsString(-1))
		{
			state.Pop(1);
			result = defValue;
			return false;
		}
		string data = string.Intern(state.ToString(-1));
		state.Pop(1);
		if (!data.TryParse<T>(out var @enum))
		{
			result = defValue;
			return false;
		}
		result = @enum;
		return true;
	}

	public bool GetData<T>(int iIndex, ref T result) where T : Enum
	{
		if (Get(iIndex) && !state.IsString(-1))
		{
			state.Pop(1);
			return false;
		}
		string data = string.Intern(state.ToString(-1));
		state.Pop(1);
		if (!data.TryParse<T>(out var @enum))
		{
			return false;
		}
		result = @enum;
		return true;
	}

	public bool GetData<T>(int iIndex, out T result, T defValue) where T : Enum
	{
		if (Get(iIndex) && !state.IsString(-1))
		{
			state.Pop(1);
			result = defValue;
			return false;
		}
		string data = string.Intern(state.ToString(-1));
		state.Pop(1);
		if (!data.TryParse<T>(out var @enum))
		{
			result = defValue;
			return false;
		}
		result = @enum;
		return true;
	}

	public bool LoadCommonPath(string bundleName, string fileName, bool bAddCompiledLuaPostFix = true)
	{
		string errorMessage = "";
		if (!LoadCommonPathBase(bundleName, fileName, bAddCompiledLuaPostFix, NKCDefineManager.DEFINE_USE_DEV_SCRIPT(), ref errorMessage))
		{
			Log.ErrorAndExit("LUA Loading Error. FileName:" + fileName + " BundleName:" + bundleName + " error:" + errorMessage, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMLuaEx.cs", 24);
			Log.ErrorAndExit(errorMessage, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMLuaEx.cs", 25);
			return false;
		}
		return true;
	}

	public bool LoadCommonPathBase(string bundleName, string fileName, bool bAddCompiledLuaPostFix, bool bUseDevScript, ref string errorMessage)
	{
		string text = GetEncryptedFileName(fileName);
		if (NKCDefineManager.DEFINE_EXTRA_ASSET())
		{
			string text2 = ".bytes";
			if (bAddCompiledLuaPostFix)
			{
				text2 = "_C.bytes";
			}
			string text3 = Path.Combine(Path.Combine(NKCUtil.GetExtraDownloadPath(), bundleName.ToUpper()), text + text2);
			if (NKCPatchUtility.IsFileExists(text3))
			{
				byte[] array = null;
				array = ((!text3.Contains("jar:")) ? File.ReadAllBytes(text3) : BetterStreamingAssets.ReadAllBytes(NKCAssetbundleInnerStream.GetJarRelativePath(text3)));
				Crypto2.Decrypt(array, array.Length);
				string chunk;
				using (MemoryStream stream = new MemoryStream(array, 0, array.Length, writable: false))
				{
					using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
					chunk = streamReader.ReadToEnd();
				}
				if (NKCDefineManager.DEFINE_USE_COMPILED_LUA())
				{
					bool flag = true;
					if (text.Contains("LUA_ASSET_BUNDLE_FILE_LIST"))
					{
						flag = false;
					}
					if (flag)
					{
						m_LuaSvr.DoString(array, text, "b");
					}
					else
					{
						m_LuaSvr.DoString(chunk, text);
					}
				}
				else
				{
					m_LuaSvr.DoString(chunk, text);
				}
				return true;
			}
		}
		NKCAssetResourceData nKCAssetResourceData = null;
		try
		{
			if (bAddCompiledLuaPostFix)
			{
				text += "_c";
			}
			nKCAssetResourceData = NKCAssetResourceManager.OpenResource<TextAsset>(bundleName, text);
			TextAsset asset = nKCAssetResourceData.GetAsset<TextAsset>();
			if (asset == null)
			{
				Log.Error("Resources.Load null: " + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMLuaEx.cs", 144);
			}
			if (bUseDevScript)
			{
				string fileName2 = fileName + "_DEV";
				if (CheckCommonFileExist(bundleName, fileName2, bAddCompiledLuaPostFix))
				{
					NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
					return LoadCommonPathBase(bundleName, fileName2, bAddCompiledLuaPostFix, bUseDevScript: false, ref errorMessage);
				}
			}
			if (m_LUA_STATIC_BUF_SIZE < asset.bytes.Length)
			{
				m_LUA_STATIC_BUF_SIZE = asset.bytes.Length * 2 + 20;
				m_LUA_STATIC_BUF = new byte[m_LUA_STATIC_BUF_SIZE];
				Log.Debug($"루아 버퍼 확장 : {m_LUA_STATIC_BUF_SIZE / 1024 / 1024} Mb", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMLuaEx.cs", 172);
			}
			Array.Clear(m_LUA_STATIC_BUF, 0, asset.bytes.Length + 10);
			Buffer.BlockCopy(asset.bytes, 0, m_LUA_STATIC_BUF, 0, asset.bytes.Length);
			Crypto2.Decrypt(m_LUA_STATIC_BUF, asset.bytes.Length);
			string chunk2;
			using (MemoryStream stream2 = new MemoryStream(m_LUA_STATIC_BUF, 0, asset.bytes.Length, writable: false))
			{
				using StreamReader streamReader2 = new StreamReader(stream2, Encoding.UTF8);
				chunk2 = streamReader2.ReadToEnd();
			}
			if (NKCDefineManager.DEFINE_USE_COMPILED_LUA())
			{
				bool flag2 = true;
				if (text.Contains("LUA_ASSET_BUNDLE_FILE_LIST"))
				{
					flag2 = false;
				}
				if (flag2)
				{
					m_LuaSvr.DoString(m_LUA_STATIC_BUF, text, "b");
				}
				else
				{
					m_LuaSvr.DoString(chunk2, text);
				}
			}
			else
			{
				m_LuaSvr.DoString(chunk2, text);
			}
			NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
			return true;
		}
		catch (Exception ex)
		{
			errorMessage = ex.Message;
			Debug.LogError("Lua parse Error from " + fileName + " : " + errorMessage);
			NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
			return false;
		}
	}

	public bool LoadServerPath(string assetName, string fileName)
	{
		throw new Exception("trying to load server path sript. fileName:" + fileName);
	}

	public static bool CheckCommonFileExist(string bundleName, string fileName, bool bAddCompiledLuaPostFix)
	{
		string text = GetEncryptedFileName(fileName);
		if (bAddCompiledLuaPostFix)
		{
			text += "_c";
		}
		return AssetBundleManager.IsAssetExists(bundleName, text);
	}

	public static string GetDecryptedFileName(string fileName)
	{
		if (!NKCDefineManager.DEFINE_USE_CONVERTED_FILENAME())
		{
			return fileName;
		}
		if (fileName.Contains("LUA_ASSET_BUNDLE_FILE_LIST"))
		{
			return fileName;
		}
		return _converter.Decryption(fileName);
	}

	private static string GetEncryptedFileName(string fileName)
	{
		if (!NKCDefineManager.DEFINE_USE_CONVERTED_FILENAME())
		{
			return fileName;
		}
		if (fileName.Contains("LUA_ASSET_BUNDLE_FILE_LIST"))
		{
			return fileName;
		}
		if (NKCDefineManager.DEINFE_USE_CONVERTED_FILENAME_TO_UPPERCASE())
		{
			Log.Info("[GetEncryptedFileName FileName convert to uppercase] : " + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMLuaEx.cs", 284);
			fileName = fileName.ToUpper();
		}
		return _converter.Encryption(fileName);
	}
}
