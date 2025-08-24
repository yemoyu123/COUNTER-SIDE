using System.Collections.Generic;
using NKM;

namespace NKC;

public static class StringValidSet
{
	private static HashSet<int> ignoreSet = new HashSet<int>();

	public static void Init()
	{
		for (int i = 0; i <= 64; i++)
		{
			ignoreSet.Add(i);
		}
		for (int j = 91; j <= 96; j++)
		{
			ignoreSet.Add(j);
		}
		for (int k = 123; k <= 127; k++)
		{
			ignoreSet.Add(k);
		}
	}

	public static bool Ignore(char ch)
	{
		return ignoreSet.Contains(ch);
	}

	public static bool Valid(char ch)
	{
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.KOR))
		{
			return ValidKorea(ch);
		}
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.GLOBAL))
		{
			return ValidGlobal(ch);
		}
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.CHN))
		{
			return ValidChina(ch);
		}
		return true;
	}

	private static bool ValidKorea(char ch)
	{
		if (ValidEnglish(ch))
		{
			return true;
		}
		if (ValidHangle(ch))
		{
			return true;
		}
		if (ValidArabicNumerals(ch))
		{
			return true;
		}
		return false;
	}

	private static bool ValidChina(char ch)
	{
		if (ValidEnglish(ch))
		{
			return true;
		}
		if (ValidArabicNumerals(ch))
		{
			return true;
		}
		if (ValidChineseChar(ch))
		{
			return true;
		}
		return false;
	}

	private static bool ValidGlobal(char ch)
	{
		if (ValidEnglish(ch))
		{
			return true;
		}
		if (ValidHangle(ch))
		{
			return true;
		}
		if (ValidChineseChar(ch))
		{
			return true;
		}
		if (ValidJapaneseChar(ch))
		{
			return true;
		}
		if (ValidArabicNumerals(ch))
		{
			return true;
		}
		return false;
	}

	private static bool ValidJapaneseChar(char ch)
	{
		if (ch >= '\u3040' && ch <= 'ヿ')
		{
			return true;
		}
		return false;
	}

	private static bool ValidChineseChar(char ch)
	{
		if (ch >= '一')
		{
			return ch <= '龥';
		}
		return false;
	}

	private static bool ValidHangle(char ch)
	{
		if ('가' <= ch && ch <= '힣')
		{
			return true;
		}
		return false;
	}

	private static bool ValidEnglish(char ch)
	{
		if ('A' <= ch && ch <= 'Z')
		{
			return true;
		}
		if ('a' <= ch && ch <= 'z')
		{
			return true;
		}
		return false;
	}

	private static bool ValidArabicNumerals(char ch)
	{
		if ('0' <= ch && ch <= '9')
		{
			return true;
		}
		return false;
	}

	public static bool CheckIgnoreSet(char ch)
	{
		return Ignore(ch);
	}
}
