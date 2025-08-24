using System;
using System.Collections.Generic;
using Cs.Logging;
using NKC.Publisher;
using NKM;
using UnityEngine;

namespace NKC.Localization;

public static class NKCLocalization
{
	public const string LOCALIZED_PATH_POSTFIX = "_loc";

	public const string VARIANT_CENSORED = "cn";

	public const string VARIANT_NATION_CN = "zlong";

	public const string VARIANT_DEFAULT = "asset";

	public const string CENSOR_TAG = "CENSOR_ASSET";

	public const string VARIANT_NATION_CHN = "zchn";

	public const string VARIANT_NATION_TWN = "gbtwn";

	public const string VARIANT_NATION_SEA = "zsea";

	public const string VARIANT_NATION_JPN = "njpn";

	public const string VARIANT_NATION_NAEU = "naeu";

	public const string VARIANT_NATION_GLOBAL = "gbl";

	public const string LANGUAGE_TAG_KOR = "LANGUAGE_KOR";

	public const string LANGUAGE_TAG_JPN = "LANGUAGE_JPN";

	public const string LANGUAGE_TAG_ENG = "LANGUAGE_ENG";

	public const string LANGUAGE_TAG_CENSORED_CHN = "LANGUAGE_CENSORED_CHN";

	public const string LANGUAGE_TAG_SIMPLIFIED_CHN = "LANGUAGE_SIMPLIFIED_CHN";

	public const string LANGUAGE_TAG_TRADITIONAL_CHN = "LANGUAGE_TRADITIONAL_CHN";

	public const string LANGUAGE_TAG_THAILAND = "LANGUAGE_THA";

	public const string LANGUAGE_TAG_VIETNAM = "LANGUAGE_VTN";

	public const string LANGUAGE_TAG_DEUTSCH = "LANGUAGE_DEU";

	public const string LANGUAGE_TAG_FRENCH = "LANGUAGE_FRA";

	public const string LANGUAGE_CODE_KOR = "ko";

	public const string LANGUAGE_CODE_JPN = "ja";

	public const string LANGUAGE_CODE_ENG = "en";

	public const string LANGUAGE_CODE_SIMPLIFIED_CHN = "zh-hans";

	public const string LANGUAGE_CODE_TRADITIONAL_CHN = "zh-hant";

	public const string LANGUAGE_CODE_THAILAND = "th";

	public const string LANGUAGE_CODE_VIETNAM = "vi";

	public const string LANGUAGE_CODE_DEUTSCH = "de";

	public const string LANGUAGE_CODE_FRENCH = "fr";

	public const string VOICE_VARIANT_KOR = "vkor";

	public const string VOICE_TAG_KOR = "VOICE_KOR";

	public const string VOICE_VARIANT_CHN = "vchn";

	public const string VOICE_TAG_CHN = "VOICE_CHN";

	public const string VOICE_VARIANT_JPN = "vjpn";

	public const string VOICE_TAG_JPN = "VOICE_JPN";

	public static readonly Dictionary<string, NKM_NATIONAL_CODE> s_dicLanguageTag = new Dictionary<string, NKM_NATIONAL_CODE>
	{
		{
			"LANGUAGE_KOR",
			NKM_NATIONAL_CODE.NNC_KOREA
		},
		{
			"LANGUAGE_JPN",
			NKM_NATIONAL_CODE.NNC_JAPAN
		},
		{
			"LANGUAGE_ENG",
			NKM_NATIONAL_CODE.NNC_ENG
		},
		{
			"LANGUAGE_SIMPLIFIED_CHN",
			NKM_NATIONAL_CODE.NNC_SIMPLIFIED_CHINESE
		},
		{
			"LANGUAGE_CENSORED_CHN",
			NKM_NATIONAL_CODE.NNC_CENSORED_CHINESE
		},
		{
			"LANGUAGE_TRADITIONAL_CHN",
			NKM_NATIONAL_CODE.NNC_TRADITIONAL_CHINESE
		},
		{
			"LANGUAGE_THA",
			NKM_NATIONAL_CODE.NNC_THAILAND
		},
		{
			"LANGUAGE_VTN",
			NKM_NATIONAL_CODE.NNC_VIETNAM
		},
		{
			"LANGUAGE_DEU",
			NKM_NATIONAL_CODE.NNC_DEUTSCH
		},
		{
			"LANGUAGE_FRA",
			NKM_NATIONAL_CODE.NNC_FRENCH
		}
	};

	public static readonly Dictionary<string, string> s_dicLangTagByLangCode = new Dictionary<string, string>
	{
		{ "ko", "LANGUAGE_KOR" },
		{ "ja", "LANGUAGE_JPN" },
		{ "en", "LANGUAGE_ENG" },
		{ "zh-hans", "LANGUAGE_SIMPLIFIED_CHN" },
		{ "zh-hant", "LANGUAGE_TRADITIONAL_CHN" },
		{ "th", "LANGUAGE_THA" },
		{ "vi", "LANGUAGE_VTN" },
		{ "de", "LANGUAGE_DEU" },
		{ "fr", "LANGUAGE_FRA" }
	};

	public static readonly Dictionary<string, NKC_VOICE_CODE> s_dicVoiceTag = new Dictionary<string, NKC_VOICE_CODE>
	{
		{
			"VOICE_KOR",
			NKC_VOICE_CODE.NVC_KOR
		},
		{
			"VOICE_CHN",
			NKC_VOICE_CODE.NVC_CHN
		},
		{
			"VOICE_JPN",
			NKC_VOICE_CODE.NVC_JPN
		}
	};

	public static readonly Dictionary<NKM_NATIONAL_CODE, string> s_dicLanguageVariant = new Dictionary<NKM_NATIONAL_CODE, string>
	{
		{
			NKM_NATIONAL_CODE.NNC_KOREA,
			"kor"
		},
		{
			NKM_NATIONAL_CODE.NNC_JAPAN,
			"jpn"
		},
		{
			NKM_NATIONAL_CODE.NNC_ENG,
			"eng"
		},
		{
			NKM_NATIONAL_CODE.NNC_CENSORED_CHINESE,
			"chn"
		},
		{
			NKM_NATIONAL_CODE.NNC_SIMPLIFIED_CHINESE,
			"scn"
		},
		{
			NKM_NATIONAL_CODE.NNC_TRADITIONAL_CHINESE,
			"twn"
		},
		{
			NKM_NATIONAL_CODE.NNC_THAILAND,
			"tha"
		},
		{
			NKM_NATIONAL_CODE.NNC_VIETNAM,
			"vtn"
		},
		{
			NKM_NATIONAL_CODE.NNC_DEUTSCH,
			"deu"
		},
		{
			NKM_NATIONAL_CODE.NNC_FRENCH,
			"fra"
		}
	};

	public static readonly Dictionary<NKC_VOICE_CODE, string> s_dicVoiceVariant = new Dictionary<NKC_VOICE_CODE, string>
	{
		{
			NKC_VOICE_CODE.NVC_KOR,
			"vkor"
		},
		{
			NKC_VOICE_CODE.NVC_CHN,
			"vchn"
		},
		{
			NKC_VOICE_CODE.NVC_JPN,
			"vjpn"
		}
	};

	public static bool CensoredVersion => NKMContentsVersionManager.HasTag("CENSOR_ASSET");

	public static string GetLangTagByLangCode(string langCode)
	{
		if (s_dicLangTagByLangCode.TryGetValue(langCode, out var value))
		{
			return value;
		}
		return "";
	}

	public static string GetBySystemLanguageCode()
	{
		Log.Debug("GetLangTagByApplicationSystemLanguage [" + Application.systemLanguage.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Localization/NKCLocalization.cs", 158);
		switch (Application.systemLanguage)
		{
		case SystemLanguage.Chinese:
			return "zh-hant";
		case SystemLanguage.ChineseSimplified:
			return "zh-hans";
		case SystemLanguage.Japanese:
			return "ja";
		case SystemLanguage.Vietnamese:
			return "vi";
		case SystemLanguage.Thai:
			return "th";
		case SystemLanguage.Korean:
			return "ko";
		case SystemLanguage.Danish:
		case SystemLanguage.French:
			return "fr";
		case SystemLanguage.Czech:
		case SystemLanguage.Dutch:
		case SystemLanguage.German:
			return "de";
		case SystemLanguage.English:
			return "en";
		default:
			return "";
		}
	}

	public static HashSet<NKM_NATIONAL_CODE> GetSelectLanguageSet()
	{
		HashSet<NKM_NATIONAL_CODE> hashSet = new HashSet<NKM_NATIONAL_CODE>();
		foreach (KeyValuePair<string, NKM_NATIONAL_CODE> item in s_dicLanguageTag)
		{
			if (NKMContentsVersionManager.HasTag(item.Key))
			{
				hashSet.Add(item.Value);
			}
		}
		if (hashSet.Count > 0)
		{
			return hashSet;
		}
		if (NKCDefineManager.DEFINE_CLIENT_KOR())
		{
			hashSet.Add(NKM_NATIONAL_CODE.NNC_KOREA);
		}
		else if (NKCDefineManager.DEFINE_CLIENT_JPN())
		{
			hashSet.Add(NKM_NATIONAL_CODE.NNC_JAPAN);
		}
		else if (NKCDefineManager.DEFINE_CLIENT_CHN())
		{
			hashSet.Add(NKM_NATIONAL_CODE.NNC_SIMPLIFIED_CHINESE);
		}
		else if (NKCDefineManager.DEFINE_CLIENT_TWN())
		{
			hashSet.Add(NKM_NATIONAL_CODE.NNC_TRADITIONAL_CHINESE);
		}
		else if (NKCDefineManager.DEFINE_CLIENT_SEA())
		{
			hashSet.Add(NKM_NATIONAL_CODE.NNC_VIETNAM);
			hashSet.Add(NKM_NATIONAL_CODE.NNC_THAILAND);
			hashSet.Add(NKM_NATIONAL_CODE.NNC_ENG);
		}
		else if (NKCDefineManager.DEFINE_CLIENT_GBL())
		{
			hashSet.Add(NKM_NATIONAL_CODE.NNC_ENG);
			hashSet.Add(NKM_NATIONAL_CODE.NNC_DEUTSCH);
			hashSet.Add(NKM_NATIONAL_CODE.NNC_FRENCH);
			hashSet.Add(NKM_NATIONAL_CODE.NNC_KOREA);
		}
		if (hashSet.Count == 0)
		{
			hashSet.Add(NKCPublisherModule.Localization.GetDefaultLanguage());
		}
		return hashSet;
	}

	public static bool IsVoiceVariant(string variant)
	{
		return s_dicVoiceVariant.ContainsValue(variant);
	}

	public static string GetVariant(NKM_NATIONAL_CODE eCode)
	{
		if (s_dicLanguageVariant.ContainsKey(eCode))
		{
			return s_dicLanguageVariant[eCode];
		}
		return "";
	}

	public static string GetVariant(NKC_VOICE_CODE eCode)
	{
		if (s_dicVoiceVariant.ContainsKey(eCode))
		{
			return s_dicVoiceVariant[eCode];
		}
		return "";
	}

	public static string[] GetVariants(NKM_NATIONAL_CODE currentNationalCode, NKC_VOICE_CODE targetVoiceCode)
	{
		List<string> list = new List<string>();
		if (!NKCDefineManager.DEFINE_SERVICE())
		{
			list.Add("dev");
		}
		if (CensoredVersion)
		{
			list.Add("cn");
			list.Add("zlong");
		}
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.CHN))
		{
			list.Add("zchn");
		}
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.TWN))
		{
			list.Add("gbtwn");
		}
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.SEA))
		{
			list.Add("zsea");
		}
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.JPN))
		{
			list.Add("njpn");
		}
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.NAEU))
		{
			list.Add("naeu");
		}
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.GLOBAL))
		{
			list.Add("gbl");
		}
		string variant = GetVariant(currentNationalCode);
		if (!string.IsNullOrEmpty(variant))
		{
			list.Add(variant);
		}
		string variant2 = GetVariant(targetVoiceCode);
		if (!string.IsNullOrEmpty(variant2))
		{
			list.Add(variant2);
		}
		else
		{
			list.Add("vkor");
		}
		list.Add("asset");
		foreach (string item in list)
		{
			Log.Info("<color=#00ff00> Variant added : [" + item + "]</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Localization/NKCLocalization.cs", 366);
		}
		return list.ToArray();
	}

	public static List<string> GetAllVariants()
	{
		List<string> list = new List<string>();
		list.Add("dev");
		list.Add("cn");
		list.Add("zlong");
		list.Add("zchn");
		list.Add("gbtwn");
		list.Add("zsea");
		list.Add("njpn");
		list.Add("naeu");
		list.Add("gbl");
		foreach (NKM_NATIONAL_CODE value2 in Enum.GetValues(typeof(NKM_NATIONAL_CODE)))
		{
			string variant = GetVariant(value2);
			if (!string.IsNullOrEmpty(variant))
			{
				list.Add(variant);
			}
		}
		foreach (KeyValuePair<NKC_VOICE_CODE, string> item in s_dicVoiceVariant)
		{
			string value = item.Value;
			if (!string.IsNullOrEmpty(value))
			{
				list.Add(value);
			}
		}
		list.Add("asset");
		return list;
	}
}
