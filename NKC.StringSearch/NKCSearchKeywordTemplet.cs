using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;

namespace NKC.StringSearch;

public class NKCSearchKeywordTemplet
{
	private static Dictionary<string, string> s_dicKeywords = new Dictionary<string, string>();

	public static string GetKeyword(string key)
	{
		if (s_dicKeywords.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public static void LoadFromLua()
	{
		s_dicKeywords.Clear();
		foreach (var item in NKMTempletLoader.LoadCommonPath("AB_SCRIPT", "LUA_SEARCH_KEYWORD_TEMPLET", "SEARCH_KEYWORD_TEMPLET", LoadFromLua))
		{
			if (!string.IsNullOrEmpty(item.Item1) && !string.IsNullOrEmpty(item.Item2))
			{
				s_dicKeywords.Add(item.Item1, item.Item2);
			}
		}
	}

	private static (string, string) LoadFromLua(NKMLua cNKMLua)
	{
		string rValue = string.Empty;
		string rValue2 = string.Empty;
		cNKMLua.GetData("Keyword_StringKey", ref rValue);
		cNKMLua.GetData("StringValue" + NKCStringTable.GetNationalPostfix(NKCStringTable.GetNationalCode()), ref rValue2);
		return (rValue, rValue2);
	}
}
