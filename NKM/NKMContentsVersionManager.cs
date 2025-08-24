using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cs.Core.Util;
using Cs.Logging;
using NKM.Templet.Base;
using UnityEngine;

namespace NKM;

public static class NKMContentsVersionManager
{
	public const string m_LOCAL_SAVE_CONTENTS_VERSION_KEY = "LOCAL_SAVE_CONTENTS_VERSION_KEY";

	public const string m_LOCAL_SAVE_CONTENTS_TAG_KEY = "LOCAL_SAVE_CONTENTS_TAG_KEY";

	public const string m_LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_IP = "LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_IP";

	public const string m_LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_PORT = "LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_PORT";

	public const string FileName = "LUA_CONTENTS_VERSION";

	public const string VersionVariableName = "ContentsVersion";

	private static readonly string[] CountryTag;

	private static readonly string[] DataFormatChangeTag;

	private static HashSet<string> currentContentsTag;

	public static NKMContentsVersion CurrentVersion { get; private set; }

	public static IEnumerable<string> CurrentTagList => currentContentsTag;

	internal static HashSet<string> CurrentContentsTag => currentContentsTag;

	static NKMContentsVersionManager()
	{
		CountryTag = new string[EnumUtil<CountryTagType>.Count];
		DataFormatChangeTag = new string[EnumUtil<DataFormatChangeTagType>.Count];
		currentContentsTag = new HashSet<string>();
		for (int i = 0; i < CountryTag.Length; i++)
		{
			string[] countryTag = CountryTag;
			int num = i;
			CountryTagType countryTagType = (CountryTagType)i;
			countryTag[num] = countryTagType.ToString();
		}
		for (int j = 0; j < DataFormatChangeTag.Length; j++)
		{
			string[] dataFormatChangeTag = DataFormatChangeTag;
			int num2 = j;
			DataFormatChangeTagType dataFormatChangeTagType = (DataFormatChangeTagType)j;
			dataFormatChangeTag[num2] = dataFormatChangeTagType.ToString();
		}
	}

	public static void LoadDefaultVersion()
	{
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_CONTENTS_VERSION"))
		{
			Log.ErrorAndExit("[ContentsVersion] loading file failed:LUA_CONTENTS_VERSION", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentsVersionManager.cs", 68);
		}
		string rValue = null;
		if (!nKMLua.GetData("ContentsVersion", ref rValue))
		{
			Log.ErrorAndExit("[ContentsVersion] loading default version failed:ContentsVersion", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentsVersionManager.cs", 74);
		}
		if (!SetCurrent(rValue))
		{
			Log.ErrorAndExit("[ContentsVersion] parsing version value failed:" + rValue, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentsVersionManager.cs", 79);
		}
	}

	public static bool HasTag(string tag)
	{
		if (string.IsNullOrWhiteSpace(tag))
		{
			throw new ArgumentException("[ContentsTag] param:" + tag);
		}
		return currentContentsTag.Contains(tag);
	}

	public static bool CheckIfValid(string tag)
	{
		if (!string.IsNullOrWhiteSpace(tag))
		{
			return currentContentsTag.Contains(tag);
		}
		return true;
	}

	public static bool HasDFChangeTagType(DataFormatChangeTagType tagType)
	{
		return HasTag(DataFormatChangeTag[(int)tagType]);
	}

	public static bool HasCountryTag(CountryTagType countryTagType)
	{
		return HasTag(CountryTag[(int)countryTagType]);
	}

	public static bool SetCurrent(string literal)
	{
		CurrentVersion = NKMContentsVersion.Create(literal);
		if (CurrentVersion == null)
		{
			return false;
		}
		Log.Info($"[ContentsVersion] ContentsVersion:{CurrentVersion}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentsVersionManager.cs", 119);
		return true;
	}

	public static void AddTag(string tag)
	{
		if (!string.IsNullOrEmpty(tag) && !currentContentsTag.Contains(tag))
		{
			currentContentsTag.Add(tag);
		}
	}

	public static bool CheckContentsVersion(IList<string> listContentsTagIgnore, IList<string> listContentsTagAllow)
	{
		if (listContentsTagAllow.Count > 0 && listContentsTagAllow.All((string e) => !currentContentsTag.Contains(e)))
		{
			return false;
		}
		if (listContentsTagIgnore.Any((string e) => currentContentsTag.Contains(e)))
		{
			return false;
		}
		return true;
	}

	public static bool CheckContentsVersion(NKMLua cNKMLua, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		string text = "listContentsTagAllow";
		string rValue2;
		if (cNKMLua.OpenTable(text))
		{
			string rValue;
			for (int i = 1; cNKMLua.GetData(i, out rValue, string.Empty); i++)
			{
				list.Add(rValue);
			}
			cNKMLua.CloseTable();
		}
		else if (cNKMLua.GetData(text, out rValue2, string.Empty))
		{
			NKMTempletError.Add("[ContentsVersion] 데이터 포맷 오류. 리스트 타입이어야 함. keyName:" + text + " value:" + rValue2, file, line);
		}
		text = "listContentsTagIgnore";
		string rValue4;
		if (cNKMLua.OpenTable(text))
		{
			string rValue3;
			for (int j = 1; cNKMLua.GetData(j, out rValue3, string.Empty); j++)
			{
				list2.Add(rValue3);
			}
			cNKMLua.CloseTable();
		}
		else if (cNKMLua.GetData(text, out rValue4, string.Empty))
		{
			NKMTempletError.Add("[ContentsVersion] 데이터 포맷 오류. 리스트 타입이어야 함. keyName:" + text + " value:" + rValue4, file, line);
		}
		return CheckContentsVersion(list2, list);
	}

	public static string Dump()
	{
		return $"version:{CurrentVersion} #tags:{currentContentsTag.Count}";
	}

	public static void PrintCurrentTagSet()
	{
		Log.Debug("[ContentsVersion] Print tag: " + string.Join(", ", currentContentsTag), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentsVersionManager.cs", 198);
	}

	public static void Drop()
	{
		currentContentsTag.Clear();
	}

	public static string GetCountryTag()
	{
		for (int i = 0; i < CountryTag.Length; i++)
		{
			string text = CountryTag[i];
			if (!string.IsNullOrWhiteSpace(text) && HasTag(text))
			{
				return text;
			}
		}
		return string.Empty;
	}

	public static void DeleteLocalTag()
	{
		Log.Info("DeleteLocalTags", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMContentsVersionManagerEx.cs", 27);
		PlayerPrefs.DeleteKey("LOCAL_SAVE_CONTENTS_TAG_KEY");
		PlayerPrefs.DeleteKey("LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_IP");
		PlayerPrefs.DeleteKey("LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_PORT");
	}
}
