using System.Globalization;
using System.Text.RegularExpressions;
using NKM;

namespace NKC;

public class NKCFilterManager
{
	public static CFilter cFilter = new CFilter();

	public static CFilter cGuildnameFilter = new CFilter();

	private const string EMOJI_REMOVE_REGEX = "\\p{Cs}|\\p{So}";

	public static bool LoadFromLua()
	{
		LoadFromLua("LUA_BAD_CHAT_FILTER_TEMPLET", "BAD_CHAT_FILTER_TEMPLET", ref cFilter);
		LoadFromLua("LUA_BAD_GUILD_NAME_FILTER_TEMPLET", "BAD_GUILD_NAME_FILTER_TEMPLET", ref cGuildnameFilter);
		return true;
	}

	private static void LoadFromLua(string scriptName, string tableName, ref CFilter targetFilter)
	{
		NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT", scriptName) || !nKMLua.OpenTable(tableName))
		{
			return;
		}
		int num = 1;
		while (nKMLua.OpenTable(num))
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCFilterManager.cs", 419))
			{
				num++;
				nKMLua.CloseTable();
				continue;
			}
			string rValue = "";
			nKMLua.GetData("WORD", ref rValue);
			if (!string.IsNullOrEmpty(rValue))
			{
				targetFilter.AddFilterString(rValue);
			}
			num++;
			nKMLua.CloseTable();
		}
	}

	public static string RemoveEmoji(string str)
	{
		return Regex.Replace(str, "\\p{Cs}|\\p{So}", "");
	}

	public static string CheckBadChat(string inputStr)
	{
		return RemoveEmoji(cFilter.Filter(inputStr));
	}

	public static bool CheckBadGuildname(string inputStr)
	{
		return cGuildnameFilter.CheckNickNameFilter(inputStr.ToCharArray());
	}

	public static bool CheckNickNameFilter(string data)
	{
		return cFilter.CheckNickNameFilter(data.ToCharArray());
	}

	public static char FilterEmojiInput(string text, int charIndex, char addedChar)
	{
		UnicodeCategory unicodeCategory = char.GetUnicodeCategory(addedChar);
		if (unicodeCategory == UnicodeCategory.Surrogate || unicodeCategory == UnicodeCategory.OtherSymbol)
		{
			return '\0';
		}
		return addedChar;
	}

	public static string RemoveNewLine(string inputStr)
	{
		return Regex.Replace(inputStr, "\\r\\n|\\r|\\n", " ");
	}
}
