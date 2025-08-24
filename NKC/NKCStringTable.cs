using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AssetBundles;
using Cs.Logging;
using NKC.Templet;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCStringTable
{
	private static NKM_NATIONAL_CODE m_NationalCode;

	private static Dictionary<string, StringData> m_dicString;

	private static Dictionary<string, StringData> m_dicStringCustom;

	private const int DIC_STRING_SIZE = 35000;

	private const int DIC_STRING_CUSTOM_SIZE = 100;

	private const string USER_NICK_NAME = "<usernickname>";

	private static readonly Regex KeywordRegex;

	private static readonly Regex RankNumberRegex;

	public static NKM_NATIONAL_CODE NationalCode => m_NationalCode;

	static NKCStringTable()
	{
		KeywordRegex = new Regex("\\[#([a-zA-Z0-9_]*)#\\]");
		RankNumberRegex = new Regex("\\(th\\)|\\(TH\\)");
		Clear();
	}

	public static void Clear()
	{
		m_NationalCode = NKM_NATIONAL_CODE.NNC_KOREA;
		m_dicString = new Dictionary<string, StringData>(35000, StringComparer.OrdinalIgnoreCase);
		m_dicStringCustom = new Dictionary<string, StringData>(100, StringComparer.OrdinalIgnoreCase);
	}

	public static string GetNationalPostfix(NKM_NATIONAL_CODE eNKM_NATIONAL_CODE)
	{
		return eNKM_NATIONAL_CODE switch
		{
			NKM_NATIONAL_CODE.NNC_KOREA => "_KOREA", 
			NKM_NATIONAL_CODE.NNC_ENG => "_ENG", 
			NKM_NATIONAL_CODE.NNC_JAPAN => "_JPN", 
			NKM_NATIONAL_CODE.NNC_CENSORED_CHINESE => "_CHN", 
			NKM_NATIONAL_CODE.NNC_SIMPLIFIED_CHINESE => "_SCN", 
			NKM_NATIONAL_CODE.NNC_TRADITIONAL_CHINESE => "_TWN", 
			NKM_NATIONAL_CODE.NNC_THAILAND => "_THA", 
			NKM_NATIONAL_CODE.NNC_VIETNAM => "_VTN", 
			NKM_NATIONAL_CODE.NNC_DEUTSCH => "_DEU", 
			NKM_NATIONAL_CODE.NNC_FRENCH => "_FRA", 
			_ => "", 
		};
	}

	public static string GetCurrLanguageCode()
	{
		return GetLanguageCode(GetNationalCode());
	}

	public static string GetLanguageCode(NKM_NATIONAL_CODE eNKM_NATIONAL_CODE, bool bForTranslation = false)
	{
		switch (eNKM_NATIONAL_CODE)
		{
		case NKM_NATIONAL_CODE.NNC_KOREA:
			return "ko";
		case NKM_NATIONAL_CODE.NNC_ENG:
			return "en";
		case NKM_NATIONAL_CODE.NNC_JAPAN:
			return "ja";
		case NKM_NATIONAL_CODE.NNC_CENSORED_CHINESE:
		case NKM_NATIONAL_CODE.NNC_SIMPLIFIED_CHINESE:
			if (bForTranslation)
			{
				return "zh-CN";
			}
			return "zh-hans";
		case NKM_NATIONAL_CODE.NNC_TRADITIONAL_CHINESE:
			if (bForTranslation)
			{
				return "zh-TW";
			}
			return "zh-hant";
		case NKM_NATIONAL_CODE.NNC_THAILAND:
			return "th";
		case NKM_NATIONAL_CODE.NNC_VIETNAM:
			return "vi";
		case NKM_NATIONAL_CODE.NNC_DEUTSCH:
			return "de";
		case NKM_NATIONAL_CODE.NNC_FRENCH:
			return "fr";
		default:
			return "null";
		}
	}

	public static NKM_NATIONAL_CODE GetNationalCode()
	{
		return m_NationalCode;
	}

	public static bool LoadFromLUA(NKM_NATIONAL_CODE eNationalCode)
	{
		m_NationalCode = eNationalCode;
		HashSet<string> hashSet = new HashSet<string>();
		AssetBundleManager.LoadAssetBundle("ab_script_string_table", async: false);
		string[] allAssetNameInBundle = AssetBundleManager.GetAllAssetNameInBundle("ab_script_string_table");
		if (allAssetNameInBundle == null)
		{
			Log.Error("load fail, String Table Asset Bundle", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCStringTable.cs", 157);
			return false;
		}
		string text = "";
		text = "_C";
		for (int i = 0; i < allAssetNameInBundle.Length; i++)
		{
			string text2 = allAssetNameInBundle[i];
			string text3 = text2;
			if (NKCDefineManager.DEFINE_USE_CONVERTED_FILENAME() && text2.Contains("_C"))
			{
				string decryptedFileName = NKMLua.GetDecryptedFileName(text2.Substring(0, text2.Length - 2));
				text3 = decryptedFileName + "_C";
				text2 = decryptedFileName;
			}
			if (text2.ToUpper().EndsWith("_DEV"))
			{
				continue;
			}
			string value = GetNationalPostfix(m_NationalCode) + text;
			if (text3.ToUpper().EndsWith(value))
			{
				if (hashSet.Contains(text2))
				{
					Log.Error($"Duplicate String Table File Name: {m_NationalCode}, {text2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCStringTable.cs", 200);
				}
				else
				{
					hashSet.Add(text2);
				}
			}
		}
		foreach (string item in hashSet)
		{
			if (NKCDefineManager.DEFINE_USE_CONVERTED_FILENAME())
			{
				Debug.Log("[stringTableFile] => " + item);
			}
			if (!LoadFromLUA(item))
			{
				return false;
			}
		}
		return true;
	}

	public static void AddString(NKM_NATIONAL_CODE nationalCode, string strID, string value, bool bOverwriteDuplicate)
	{
		if (m_NationalCode != nationalCode)
		{
			return;
		}
		if (m_dicString.ContainsKey(strID))
		{
			StringData stringData = m_dicString[strID];
			if (bOverwriteDuplicate)
			{
				stringData.m_StringValue = value;
				return;
			}
			Log.Error($"Duplicate String National[{m_NationalCode}] StrID[{strID}]  Prev[{stringData.m_StringValue}] New[{value}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCStringTable.cs", 266);
		}
		else
		{
			StringData stringData2 = new StringData();
			stringData2.m_StringID = strID;
			stringData2.m_StringValue = value;
			m_dicString.Add(stringData2.m_StringID, stringData2);
		}
	}

	public static bool LoadFromLUA(string fileName, string bundleName = "AB_SCRIPT_STRING_TABLE")
	{
		NKMLua nKMLua = new NKMLua();
		bool bAddCompiledLuaPostFix = NKCDefineManager.DEFINE_USE_CONVERTED_FILENAME();
		if (nKMLua.LoadCommonPath(bundleName, fileName, bAddCompiledLuaPostFix))
		{
			if (nKMLua.OpenTable("m_dicString"))
			{
				int num = 1;
				while (nKMLua.OpenTable(num))
				{
					string rValue = "";
					nKMLua.GetData(1, ref rValue);
					StringData stringData = null;
					if (m_dicString.ContainsKey(rValue))
					{
						stringData = m_dicString[rValue];
						Log.Error($"Duplicate String: {m_NationalCode}, {fileName} - {rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCStringTable.cs", 298);
					}
					else
					{
						stringData = new StringData();
						stringData.m_StringID = rValue;
						m_dicString.Add(stringData.m_StringID, stringData);
					}
					string rValue2 = "";
					nKMLua.GetData(2, ref rValue2);
					stringData.m_StringValue = rValue2;
					num++;
					nKMLua.CloseTable();
				}
			}
			else
			{
				Log.Error("StringTable can't find m_dicString table, fileName : " + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCStringTable.cs", 318);
			}
		}
		nKMLua.LuaClose();
		return true;
	}

	public static void SetNationalCode(NKM_NATIONAL_CODE eNKM_NATIONAL_CODE)
	{
		m_NationalCode = eNKM_NATIONAL_CODE;
	}

	public static bool CheckExistString(string strID)
	{
		if (string.IsNullOrEmpty(strID))
		{
			return false;
		}
		if (m_dicStringCustom.TryGetValue(strID, out var value))
		{
			return true;
		}
		if (m_dicString.TryGetValue(strID, out value))
		{
			return true;
		}
		return false;
	}

	private static string ProcessParameteredString(string stringValue, object[] param)
	{
		string text = ((param == null) ? stringValue : string.Format(stringValue, param));
		if (NKCScenManager.CurrentUserData() != null)
		{
			text = text.Replace("<usernickname>", NKCScenManager.CurrentUserData().m_UserNickName);
		}
		text = ProcessRankNumber(text);
		if (m_NationalCode == NKM_NATIONAL_CODE.NNC_KOREA)
		{
			return Korean.ReplaceJosa(text);
		}
		return text;
	}

	public static string GetString(string strID, bool bSkipErrorCheck = false)
	{
		if (string.IsNullOrEmpty(strID))
		{
			return "";
		}
		string[] array = null;
		if (strID.Contains("@@"))
		{
			string[] array2 = strID.Split(NKCServerStringFormatter.Seperators, StringSplitOptions.None);
			if (array2.Length > 1)
			{
				strID = array2[0];
				array = new string[array2.Length - 1];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = GetString(array2[i + 1], bSkipErrorCheck: true, null);
				}
			}
		}
		string strID2 = strID;
		object[] param = array;
		return GetString(strID2, bSkipErrorCheck, param);
	}

	public static string GetString(string strID, bool bSkipErrorCheck = false, params object[] param)
	{
		if (NKCScenManager.GetScenManager() != null)
		{
			NKCScenManager.GetScenManager().SetLanguage();
		}
		if (string.IsNullOrEmpty(strID))
		{
			return "";
		}
		bool flag = bSkipErrorCheck;
		bSkipErrorCheck = true;
		if (m_dicStringCustom.TryGetValue(strID, out var value))
		{
			return ReplaceKeyword(value.m_StringValue, param);
		}
		if (m_dicString.TryGetValue(strID, out value))
		{
			return ReplaceKeyword(value.m_StringValue, param);
		}
		if (bSkipErrorCheck)
		{
			if (!flag && bSkipErrorCheck)
			{
				Log.Debug("No Define String: " + strID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCStringTable.cs", 453);
			}
			return strID;
		}
		Log.Error("No Define String: " + strID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCStringTable.cs", 463);
		return "";
	}

	private static string ReplaceKeyword(string src, params object[] param)
	{
		if (KeywordRegex.Matches(src).Count == 0)
		{
			if (param != null && param.Length != 0)
			{
				return ProcessParameteredString(src, param);
			}
			return src;
		}
		string text = KeywordRegex.Replace(src, KeywordEvaluator);
		if (param != null && param.Length != 0)
		{
			return ProcessParameteredString(text, param);
		}
		return text;
	}

	private static string KeywordEvaluator(Match match)
	{
		if (match.Groups.Count <= 1)
		{
			return match.Value;
		}
		NKCKeywordTemplet nKCKeywordTemplet = NKCKeywordTemplet.Find(match.Groups[1].Value);
		if (nKCKeywordTemplet == null)
		{
			return match.Value;
		}
		return nKCKeywordTemplet.GetTMPLinkString();
	}

	public static string GetString(string strID, params object[] param)
	{
		return GetString(strID, bSkipErrorCheck: false, param);
	}

	public static string GetString(NKM_ERROR_CODE error_code)
	{
		return GetString(error_code.ToString());
	}

	public static void ChangeString(string strID, string newData)
	{
		if (m_dicString.ContainsKey(strID))
		{
			StringData stringData = new StringData();
			stringData.m_StringID = strID;
			stringData.m_StringValue = newData;
			m_dicString[strID] = stringData;
		}
	}

	public static string ProcessRankNumber(string src)
	{
		MatchCollection matchCollection = RankNumberRegex.Matches(src);
		if (matchCollection.Count == 0)
		{
			return src;
		}
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder(src.Length);
		foreach (Match item in matchCollection)
		{
			bool bUpper = item.Value == "(TH)";
			stringBuilder.Append(src, num, item.Index - num);
			if (item.Index > 0)
			{
				int num2 = item.Index - 1;
				char c = src[num2];
				if (c == '>')
				{
					bool flag = false;
					for (int num3 = item.Index - 1; num3 > 0; num3--)
					{
						if (src[num3] == '<')
						{
							num2 = num3 - 1;
							c = src[num2];
							if (c != '>')
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						Debug.LogError($"can't find start of tag from string {src}");
						return src;
					}
				}
				if (char.IsNumber(c))
				{
					int num4 = ToNumber(c);
					if (num2 > 0)
					{
						char c2 = src[num2 - 1];
						if (char.IsNumber(c2))
						{
							num4 = ToNumber(c2) * 10 + num4;
						}
					}
					string rankNumber = NKCUtilString.GetRankNumber(num4, bUpper);
					stringBuilder.Append(rankNumber);
				}
				else
				{
					stringBuilder.Append(item.Value);
				}
			}
			num = item.Index + item.Length;
		}
		stringBuilder.Append(src, num, src.Length - num);
		return stringBuilder.ToString();
	}

	private static int ToNumber(char inChar)
	{
		return (int)char.GetNumericValue(inChar);
	}
}
