using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NKC.StringSearch;

public class NKCStringSearchTool
{
	private static readonly Dictionary<char, char> dicFullSymbol = new Dictionary<char, char>
	{
		{ '\u3000', ' ' },
		{ '、', ',' },
		{ '，', ',' },
		{ '．', '.' },
		{ '＜', '<' },
		{ '＞', '>' },
		{ '／', '/' }
	};

	private const CompareOptions CompareFlag = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;

	private const CompareOptions JapaneseCompareFlag = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;

	private static readonly List<char> KoreanConsonants = new List<char>
	{
		'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ',
		'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
	};

	private const int FirstHangulLetter = 44032;

	private const int LastHangulLetter = 55203;

	private const int numberOfSameInitialConsonant = 588;

	private const int HiraganaBlockStart = 12353;

	private const int HiraganaBlockEnd = 12438;

	private const int KatakanaBlockStart = 12449;

	private const int KatakanaBlockEnd = 12538;

	private const int KanjiBlockStart = 19968;

	private const int KanjiBlockEnd = 40895;

	private const int GanaBlockDiff = 96;

	private static readonly IDictionary<string, string> GanaToRomajiTable = new Dictionary<string, string>
	{
		{ "１", "1" },
		{ "２", "2" },
		{ "３", "3" },
		{ "４", "4" },
		{ "５", "5" },
		{ "６", "6" },
		{ "７", "7" },
		{ "８", "8" },
		{ "９", "9" },
		{ "０", "0" },
		{ "！", "!" },
		{ "“", "\"" },
		{ "”", "\"" },
		{ "＃", "#" },
		{ "＄", "$" },
		{ "％", "%" },
		{ "＆", "&" },
		{ "’", "\"" },
		{ "（", "(" },
		{ "）", ")" },
		{ "＝", "=" },
		{ "～", "~" },
		{ "｜", "|" },
		{ "＠", "@" },
		{ "‘", "`" },
		{ "＋", "+" },
		{ "＊", "*" },
		{ "；", ";" },
		{ "：", ":" },
		{ "＜", "<" },
		{ "＞", ">" },
		{ "、", "," },
		{ "。", "." },
		{ "／", "/" },
		{ "？", "?" },
		{ "\uff3f", "_" },
		{ "・", "･" },
		{ "「", "\"" },
		{ "」", "\"" },
		{ "｛", "{" },
		{ "｝", "}" },
		{ "￥", "\\" },
		{ "\uff3e", "^" },
		{ "あ", "a" },
		{ "い", "i" },
		{ "う", "u" },
		{ "え", "e" },
		{ "お", "o" },
		{ "ア", "a" },
		{ "イ", "i" },
		{ "ウ", "u" },
		{ "エ", "e" },
		{ "オ", "o" },
		{ "か", "ka" },
		{ "き", "ki" },
		{ "く", "ku" },
		{ "け", "ke" },
		{ "こ", "ko" },
		{ "カ", "ka" },
		{ "キ", "ki" },
		{ "ク", "ku" },
		{ "ケ", "ke" },
		{ "コ", "ko" },
		{ "さ", "sa" },
		{ "し", "si" },
		{ "す", "su" },
		{ "せ", "se" },
		{ "そ", "so" },
		{ "サ", "sa" },
		{ "シ", "si" },
		{ "ス", "su" },
		{ "セ", "se" },
		{ "ソ", "so" },
		{ "た", "ta" },
		{ "ち", "ti" },
		{ "つ", "tu" },
		{ "て", "te" },
		{ "と", "to" },
		{ "タ", "ta" },
		{ "チ", "ti" },
		{ "ツ", "tu" },
		{ "テ", "te" },
		{ "ト", "to" },
		{ "な", "na" },
		{ "に", "ni" },
		{ "ぬ", "nu" },
		{ "ね", "ne" },
		{ "の", "no" },
		{ "ナ", "na" },
		{ "ニ", "ni" },
		{ "ヌ", "nu" },
		{ "ネ", "ne" },
		{ "ノ", "no" },
		{ "は", "ha" },
		{ "ひ", "hi" },
		{ "ふ", "hu" },
		{ "へ", "he" },
		{ "ほ", "ho" },
		{ "ハ", "ha" },
		{ "ヒ", "hi" },
		{ "フ", "hu" },
		{ "ヘ", "he" },
		{ "ホ", "ho" },
		{ "ま", "ma" },
		{ "み", "mi" },
		{ "む", "mu" },
		{ "め", "me" },
		{ "も", "mo" },
		{ "マ", "ma" },
		{ "ミ", "mi" },
		{ "ム", "mu" },
		{ "メ", "me" },
		{ "モ", "mo" },
		{ "や", "ya" },
		{ "ゆ", "yu" },
		{ "よ", "yo" },
		{ "ヤ", "ya" },
		{ "ユ", "yu" },
		{ "ヨ", "yo" },
		{ "ら", "ra" },
		{ "り", "ri" },
		{ "る", "ru" },
		{ "れ", "re" },
		{ "ろ", "ro" },
		{ "ラ", "ra" },
		{ "リ", "ri" },
		{ "ル", "ru" },
		{ "レ", "re" },
		{ "ロ", "ro" },
		{ "わ", "wa" },
		{ "ゐ", "wi" },
		{ "ゑ", "we" },
		{ "を", "wo" },
		{ "ワ", "wa" },
		{ "ヰ", "wi" },
		{ "ヱ", "we" },
		{ "ヲ", "wo" },
		{ "が", "ga" },
		{ "ぎ", "gi" },
		{ "ぐ", "gu" },
		{ "げ", "ge" },
		{ "ご", "go" },
		{ "ガ", "ga" },
		{ "ギ", "gi" },
		{ "グ", "gu" },
		{ "ゲ", "ge" },
		{ "ゴ", "go" },
		{ "ざ", "za" },
		{ "じ", "zi" },
		{ "ず", "zu" },
		{ "ぜ", "ze" },
		{ "ぞ", "zo" },
		{ "ザ", "za" },
		{ "ジ", "zi" },
		{ "ズ", "zu" },
		{ "ゼ", "ze" },
		{ "ゾ", "zo" },
		{ "だ", "da" },
		{ "ぢ", "di" },
		{ "づ", "du" },
		{ "で", "de" },
		{ "ど", "do" },
		{ "ダ", "da" },
		{ "ヂ", "di" },
		{ "ヅ", "du" },
		{ "デ", "de" },
		{ "ド", "do" },
		{ "ば", "ba" },
		{ "び", "bi" },
		{ "ぶ", "bu" },
		{ "べ", "be" },
		{ "ぼ", "bo" },
		{ "バ", "ba" },
		{ "ビ", "bi" },
		{ "ブ", "bu" },
		{ "ベ", "be" },
		{ "ボ", "bo" },
		{ "ぱ", "pa" },
		{ "ぴ", "pi" },
		{ "ぷ", "pu" },
		{ "ぺ", "pe" },
		{ "ぽ", "po" },
		{ "パ", "pa" },
		{ "ピ", "pi" },
		{ "プ", "pu" },
		{ "ペ", "pe" },
		{ "ポ", "po" },
		{ "きゃ", "kya" },
		{ "きゅ", "kyu" },
		{ "きょ", "kyo" },
		{ "しゃ", "sya" },
		{ "しゅ", "syu" },
		{ "しょ", "syo" },
		{ "ちゃ", "tya" },
		{ "ちゅ", "tyu" },
		{ "ちょ", "tyo" },
		{ "にゃ", "nya" },
		{ "にゅ", "nyu" },
		{ "にょ", "nyo" },
		{ "ひゃ", "hya" },
		{ "ひゅ", "hyu" },
		{ "ひょ", "hyo" },
		{ "みゃ", "mya" },
		{ "みゅ", "myu" },
		{ "みょ", "myo" },
		{ "りゃ", "rya" },
		{ "りゅ", "ryu" },
		{ "りょ", "ryo" },
		{ "キャ", "kya" },
		{ "キュ", "kyu" },
		{ "キョ", "kyo" },
		{ "シャ", "sya" },
		{ "シュ", "syu" },
		{ "ショ", "syo" },
		{ "チャ", "tya" },
		{ "チュ", "tyu" },
		{ "チョ", "tyo" },
		{ "ニャ", "nya" },
		{ "ニュ", "nyu" },
		{ "ニョ", "nyo" },
		{ "ヒャ", "hya" },
		{ "ヒュ", "hyu" },
		{ "ヒョ", "hyo" },
		{ "ミャ", "mya" },
		{ "ミュ", "myu" },
		{ "ミョ", "myo" },
		{ "リャ", "rya" },
		{ "リュ", "ryu" },
		{ "リョ", "ryo" },
		{ "ぎゃ", "gya" },
		{ "ぎゅ", "gyu" },
		{ "ぎょ", "gyo" },
		{ "じゃ", "zya" },
		{ "じゅ", "zyu" },
		{ "じょ", "zyo" },
		{ "ぢゃ", "dya" },
		{ "ぢゅ", "dyu" },
		{ "ぢょ", "dyo" },
		{ "びゃ", "bya" },
		{ "びゅ", "byu" },
		{ "びょ", "byo" },
		{ "ぴゃ", "pya" },
		{ "ぴゅ", "pyu" },
		{ "ぴょ", "pyo" },
		{ "くゎ", "kwa" },
		{ "ぐゎ", "gwa" },
		{ "ギャ", "gya" },
		{ "ギュ", "gyu" },
		{ "ギョ", "gyo" },
		{ "ジャ", "zya" },
		{ "ジュ", "zyu" },
		{ "ジョ", "zyo" },
		{ "ヂャ", "dya" },
		{ "ヂュ", "dyu" },
		{ "ヂョ", "dyo" },
		{ "ビャ", "bya" },
		{ "ビュ", "byu" },
		{ "ビョ", "byo" },
		{ "ピャ", "pya" },
		{ "ピュ", "pyu" },
		{ "ピョ", "pyo" },
		{ "クヮ", "kwa" },
		{ "グヮ", "gwa" },
		{ "ぁ", "a" },
		{ "ぃ", "i" },
		{ "ぅ", "u" },
		{ "ぇ", "e" },
		{ "ぉ", "o" },
		{ "ゃ", "ya" },
		{ "ゅ", "yu" },
		{ "ょ", "yo" },
		{ "ゎ", "wa" },
		{ "ァ", "a" },
		{ "ィ", "i" },
		{ "ゥ", "u" },
		{ "ェ", "e" },
		{ "ォ", "o" },
		{ "ャ", "ya" },
		{ "ュ", "yu" },
		{ "ョ", "yo" },
		{ "ヮ", "wa" },
		{ "ヵ", "ka" },
		{ "ヶ", "ke" },
		{ "ん", "n" },
		{ "ン", "n" },
		{ "\u3000", " " },
		{ "いぇ", "ye" },
		{ "きぇ", "kye" },
		{ "くぃ", "kwi" },
		{ "くぇ", "kwe" },
		{ "くぉ", "kwo" },
		{ "ぐぃ", "gwi" },
		{ "ぐぇ", "gwe" },
		{ "ぐぉ", "gwo" },
		{ "イェ", "ye" },
		{ "キェ", "kya" },
		{ "クィ", "kwi" },
		{ "クェ", "kwe" },
		{ "クォ", "kwo" },
		{ "グィ", "gwi" },
		{ "グェ", "gwe" },
		{ "グォ", "gwo" },
		{ "しぇ", "sye" },
		{ "じぇ", "zye" },
		{ "すぃ", "swi" },
		{ "ずぃ", "zwi" },
		{ "ちぇ", "tye" },
		{ "つぁ", "twa" },
		{ "つぃ", "twi" },
		{ "つぇ", "twe" },
		{ "つぉ", "two" },
		{ "にぇ", "nye" },
		{ "ひぇ", "hye" },
		{ "ふぁ", "hwa" },
		{ "ふぃ", "hwi" },
		{ "ふぇ", "hwe" },
		{ "ふぉ", "hwo" },
		{ "ふゅ", "hwyu" },
		{ "ふょ", "hwyo" },
		{ "シェ", "sye" },
		{ "ジェ", "zye" },
		{ "スィ", "swi" },
		{ "ズィ", "zwi" },
		{ "チェ", "tye" },
		{ "ツァ", "twa" },
		{ "ツィ", "twi" },
		{ "ツェ", "twe" },
		{ "ツォ", "two" },
		{ "ニェ", "nye" },
		{ "ヒェ", "hye" },
		{ "ファ", "hwa" },
		{ "フィ", "hwi" },
		{ "フェ", "hwe" },
		{ "フォ", "hwo" },
		{ "フュ", "hwyu" },
		{ "フョ", "hwyo" }
	};

	public static bool SplitPrefixNumber(string value, out int number, out string subString)
	{
		number = 0;
		for (int i = 0; i < value.Length; i++)
		{
			if ('0' <= value[i] && value[i] <= '9')
			{
				number = number * 10 + CharUnicodeInfo.GetDecimalDigitValue(value[i]);
				continue;
			}
			if (i == 0)
			{
				subString = value;
				return false;
			}
			subString = value.Substring(i);
			return true;
		}
		subString = string.Empty;
		return false;
	}

	public static string RemoveDiacritics(string text)
	{
		string text2 = text.Normalize(NormalizationForm.FormD);
		StringBuilder stringBuilder = new StringBuilder(text2.Length);
		bool flag = false;
		foreach (char c in text2)
		{
			if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
			{
				stringBuilder.Append(c);
				flag = IsKatagana(c) || IsHiragana(c);
			}
			else if (flag)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
	}

	public static string NormalizeSearchInput(string input)
	{
		if (NKCStringTable.NationalCode == NKM_NATIONAL_CODE.NNC_KOREA)
		{
			input = NormalizeHangulConsonant(input);
		}
		StringBuilder stringBuilder = new StringBuilder();
		string text = input;
		foreach (char c in text)
		{
			if (dicFullSymbol.TryGetValue(c, out var value))
			{
				stringBuilder.Append(value);
			}
			else
			{
				stringBuilder.Append(char.ToLower(c));
			}
		}
		return stringBuilder.ToString();
	}

	public static bool StartsWith(string baseValue, string startsWith)
	{
		if (string.IsNullOrWhiteSpace(baseValue))
		{
			return false;
		}
		baseValue = baseValue.Replace(" ", "");
		switch (NKCStringTable.NationalCode)
		{
		case NKM_NATIONAL_CODE.NNC_KOREA:
		{
			if (baseValue.Length < startsWith.Length)
			{
				return false;
			}
			for (int i = 0; i < startsWith.Length; i++)
			{
				if (!HangulConsonantMatch(baseValue[i], startsWith[i]))
				{
					return false;
				}
			}
			return true;
		}
		case NKM_NATIONAL_CODE.NNC_JAPAN:
			if (CultureInfo.CurrentCulture.CompareInfo.IsPrefix(baseValue, startsWith, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
			{
				return true;
			}
			if (!HasKanji(baseValue) && IsAlphabetOrNumber(startsWith))
			{
				string source = GanaToRomaji(baseValue);
				if (CultureInfo.CurrentCulture.CompareInfo.IsPrefix(source, startsWith, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
				{
					return true;
				}
			}
			return false;
		default:
			return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(baseValue, startsWith, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
		}
	}

	public static bool Contains(string baseValue, string contain)
	{
		if (string.IsNullOrWhiteSpace(baseValue))
		{
			return false;
		}
		baseValue = baseValue.Replace(" ", "");
		switch (NKCStringTable.NationalCode)
		{
		case NKM_NATIONAL_CODE.NNC_KOREA:
			return IndexOfHangulConsonant(baseValue, contain) >= 0;
		case NKM_NATIONAL_CODE.NNC_JAPAN:
			if (baseValue.IndexOf(contain) >= 0 || CultureInfo.CurrentCulture.CompareInfo.IndexOf(baseValue, contain, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) >= 0)
			{
				return true;
			}
			if (!HasKanji(baseValue) && IsAlphabetOrNumber(contain))
			{
				string source = GanaToRomaji(baseValue);
				if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(source, contain, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) >= 0)
				{
					return true;
				}
			}
			return false;
		default:
			if (baseValue.IndexOf(contain) < 0)
			{
				return CultureInfo.CurrentCulture.CompareInfo.IndexOf(baseValue, contain, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) >= 0;
			}
			return true;
		}
	}

	public static bool IsAbbreviation(string baseString, string userInput, bool bFirstLetterMatch = false)
	{
		if (string.IsNullOrWhiteSpace(baseString) || string.IsNullOrWhiteSpace(userInput))
		{
			return false;
		}
		if (userInput.Length == 1)
		{
			bFirstLetterMatch = true;
		}
		return NKCStringTable.NationalCode switch
		{
			NKM_NATIONAL_CODE.NNC_KOREA => IsHangulAbbreviation(baseString, userInput, bFirstLetterMatch), 
			NKM_NATIONAL_CODE.NNC_JAPAN => Contains(baseString, userInput), 
			_ => Contains(baseString, userInput), 
		};
	}

	private static bool IsAbbreviationInner(string baseString, string userInput, bool bFirstLetterMatch, CompareOptions compareFlag)
	{
		int num = 0;
		if (baseString.Length < userInput.Length)
		{
			return false;
		}
		for (int i = 0; i < userInput.Length; i++)
		{
			if (num >= baseString.Length)
			{
				return false;
			}
			int startIndex = num;
			num = baseString.IndexOf(userInput[i], num);
			if (num < 0)
			{
				num = CultureInfo.CurrentCulture.CompareInfo.IndexOf(baseString, userInput[i], startIndex, compareFlag);
			}
			if (num < 0)
			{
				return false;
			}
			if (i == 0 && bFirstLetterMatch && num != 0)
			{
				return false;
			}
			num++;
		}
		return true;
	}

	private static bool HangulConsonantMatch(char letter, char consonant)
	{
		if (letter == consonant)
		{
			return true;
		}
		if ('가' <= letter && letter <= '힣')
		{
			int index = (letter - 44032) / 588;
			return KoreanConsonants[index] == consonant;
		}
		return char.ToLower(letter) == consonant;
	}

	private static int IndexOfHangulConsonant(string baseString, char consonant, int startIndex)
	{
		for (int i = startIndex; i < baseString.Length; i++)
		{
			if (HangulConsonantMatch(baseString[i], consonant))
			{
				return i;
			}
		}
		return -1;
	}

	private static int IndexOfHangulConsonant(string baseString, string target)
	{
		int num = baseString.Length - target.Length;
		if (num < 0)
		{
			return -1;
		}
		for (int i = 0; i <= num; i++)
		{
			for (int j = 0; j < target.Length && HangulConsonantMatch(baseString[i + j], target[j]); j++)
			{
				if (j == target.Length - 1)
				{
					return i;
				}
			}
		}
		return -1;
	}

	private static bool IsHangulAbbreviation(string baseString, string userInput, bool bFirstLetterMatch)
	{
		int num = 0;
		for (int i = 0; i < userInput.Length; i++)
		{
			if (num >= baseString.Length)
			{
				return false;
			}
			num = IndexOfHangulConsonant(baseString, userInput[i], num);
			if (num < 0)
			{
				return false;
			}
			if (i == 0 && bFirstLetterMatch && num != 0)
			{
				return false;
			}
			num++;
		}
		return true;
	}

	private static string NormalizeHangulConsonant(string input)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in input)
		{
			switch (c)
			{
			case 'ㄳ':
				stringBuilder.Append("ㄱㅅ");
				break;
			case 'ㄵ':
				stringBuilder.Append("ㄴㅈ");
				break;
			case 'ㄶ':
				stringBuilder.Append("ㄴㅎ");
				break;
			case 'ㄺ':
				stringBuilder.Append("ㄹㄱ");
				break;
			case 'ㄻ':
				stringBuilder.Append("ㄹㅁ");
				break;
			case 'ㄼ':
				stringBuilder.Append("ㄹㅂ");
				break;
			case 'ㄽ':
				stringBuilder.Append("ㄹㅅ");
				break;
			case 'ㄾ':
				stringBuilder.Append("ㄹㅌ");
				break;
			case 'ㄿ':
				stringBuilder.Append("ㄹㅍ");
				break;
			case 'ㅀ':
				stringBuilder.Append("ㄹㅎ");
				break;
			default:
				stringBuilder.Append(c);
				break;
			}
		}
		return stringBuilder.ToString();
	}

	public static string ToKatakana(string input)
	{
		StringBuilder stringBuilder = new StringBuilder(input.Length);
		foreach (char c in input)
		{
			if (IsHiragana(c))
			{
				stringBuilder.Append((char)(c + 96));
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	private static bool IsKatagana(char c)
	{
		if ('ァ' <= c)
		{
			return c <= 'ヺ';
		}
		return false;
	}

	private static bool IsHiragana(char c)
	{
		if ('ぁ' <= c)
		{
			return c <= 'ゖ';
		}
		return false;
	}

	private static bool IsKanji(char c)
	{
		if ('一' <= c)
		{
			return c <= '龿';
		}
		return false;
	}

	private static bool HasGana(string hira)
	{
		foreach (char c in hira)
		{
			if (IsKatagana(c) || IsHiragana(c))
			{
				return true;
			}
		}
		return false;
	}

	private static bool HasKanji(string str)
	{
		for (int i = 0; i < str.Length; i++)
		{
			if (IsKanji(str[i]))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsAlphabetOrNumber(string str)
	{
		foreach (char c in str)
		{
			if (!IsAlphabet(c) && !char.IsNumber(c))
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsAlphabet(char c)
	{
		c = Full2Half(c);
		if ('a' <= c && c <= 'z')
		{
			return true;
		}
		if ('A' <= c && c <= 'Z')
		{
			return true;
		}
		return false;
	}

	private static char Full2Half(char c)
	{
		if (c > '\uff00' && c <= '～')
		{
			return (char)(c - 65248);
		}
		if (c == '\u3000')
		{
			return ' ';
		}
		return c;
	}

	public static string Full2Half(string str)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in str)
		{
			stringBuilder.Append(Full2Half(c));
		}
		return stringBuilder.ToString();
	}

	public static string GanaToRomaji(string hira)
	{
		if (!HasGana(hira))
		{
			return hira;
		}
		string text = "";
		int i = 0;
		int length = hira.Length;
		bool flag = false;
		string text2 = "";
		string value = null;
		int num;
		for (; i < length; i += ((num <= 0) ? 1 : num))
		{
			for (num = Math.Min(2, length - i); num > 0; num--)
			{
				text = hira.Substring(i, num);
				if (Convert.ToString(text[0]).Equals("っ") && num == 1 && i < length - 1)
				{
					flag = true;
					value = "";
					break;
				}
				value = null;
				GanaToRomajiTable.TryGetValue(text, out value);
				if (value != null && flag)
				{
					value = value[0] + value;
					flag = false;
				}
				if (value != null)
				{
					break;
				}
			}
			if (value == null)
			{
				value = text;
			}
			text2 += value;
		}
		return text2;
	}

	public static bool AbbreviationKeywordList(IEnumerable<string> keywords, string userInput, bool bFirstLetterMatch = false)
	{
		if (keywords == null)
		{
			return false;
		}
		foreach (string keyword in keywords)
		{
			if (IsAbbreviation(NKCSearchKeywordTemplet.GetKeyword(keyword), userInput, bFirstLetterMatch))
			{
				return true;
			}
		}
		return false;
	}

	public static bool ContainKeywordList(IEnumerable<string> keywords, string userInput, bool bFirstLetterMatch = false)
	{
		if (keywords == null)
		{
			return false;
		}
		foreach (string keyword in keywords)
		{
			if (Contains(NKCSearchKeywordTemplet.GetKeyword(keyword), userInput))
			{
				return true;
			}
		}
		return false;
	}
}
