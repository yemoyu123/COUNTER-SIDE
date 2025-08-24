using System;
using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.Templet;

public class NKCVoiceActorStringTemplet
{
	private string strKey;

	private string strValueKOR;

	private string strValueENG;

	private string strValueJPN;

	private string strValueCHN;

	private static Dictionary<string, Dictionary<NKM_NATIONAL_CODE, string>> voiceActorStringList = new Dictionary<string, Dictionary<NKM_NATIONAL_CODE, string>>();

	public static void LoadFromLua()
	{
		string[] array = new string[3] { "LUA_STRING_VOICE_ACTOR_NAME_KOR", "LUA_STRING_VOICE_ACTOR_NAME_JPN", "LUA_STRING_VOICE_ACTOR_NAME_CHN" };
		voiceActorStringList.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			IEnumerable<NKCVoiceActorStringTemplet> enumerable = NKMTempletLoader.LoadCommonPath("AB_SCRIPT", array[i], "STRING_VOICE_ACTOR_NAME", LoadFromLua);
			if (enumerable == null)
			{
				continue;
			}
			foreach (NKCVoiceActorStringTemplet item in enumerable)
			{
				if (item == null)
				{
					continue;
				}
				if (!voiceActorStringList.ContainsKey(item.strKey))
				{
					voiceActorStringList.Add(item.strKey, new Dictionary<NKM_NATIONAL_CODE, string>());
					foreach (NKM_NATIONAL_CODE value in Enum.GetValues(typeof(NKM_NATIONAL_CODE)))
					{
						switch (value)
						{
						case NKM_NATIONAL_CODE.NNC_KOREA:
							voiceActorStringList[item.strKey].Add(value, item.strValueKOR);
							break;
						case NKM_NATIONAL_CODE.NNC_JAPAN:
							voiceActorStringList[item.strKey].Add(value, item.strValueJPN);
							break;
						case NKM_NATIONAL_CODE.NNC_CENSORED_CHINESE:
						case NKM_NATIONAL_CODE.NNC_SIMPLIFIED_CHINESE:
							voiceActorStringList[item.strKey].Add(value, item.strValueCHN);
							break;
						default:
							voiceActorStringList[item.strKey].Add(value, item.strValueENG);
							break;
						}
					}
				}
				else
				{
					Debug.LogError("StringKey: " + item.strKey + " already exist in LUA_STRING_VOICE_ACTOR_NAME_TEMPLET");
				}
			}
		}
	}

	private static NKCVoiceActorStringTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCVoiceActorNameTemplet.cs", 261))
		{
			return null;
		}
		NKCVoiceActorStringTemplet nKCVoiceActorStringTemplet = new NKCVoiceActorStringTemplet();
		int num = 1 & (cNKMLua.GetData("StringKey", ref nKCVoiceActorStringTemplet.strKey) ? 1 : 0);
		cNKMLua.GetData("StringValue_KOR", ref nKCVoiceActorStringTemplet.strValueKOR);
		cNKMLua.GetData("StringValue_ENG", ref nKCVoiceActorStringTemplet.strValueENG);
		cNKMLua.GetData("StringValue_JPN", ref nKCVoiceActorStringTemplet.strValueJPN);
		cNKMLua.GetData("StringValue_CHN", ref nKCVoiceActorStringTemplet.strValueCHN);
		if (num == 0)
		{
			return null;
		}
		return nKCVoiceActorStringTemplet;
	}

	public static string FindVoiceActorName(string actorKey)
	{
		if (!voiceActorStringList.ContainsKey(actorKey))
		{
			return NKCVoiceActorNameTemplet.NoVoiceActorString;
		}
		NKM_NATIONAL_CODE key = NKCGameOptionData.LoadLanguageCode(NKM_NATIONAL_CODE.NNC_KOREA);
		if (!voiceActorStringList[actorKey].ContainsKey(key))
		{
			return NKCVoiceActorNameTemplet.NoVoiceActorString;
		}
		if (string.IsNullOrEmpty(voiceActorStringList[actorKey][key]))
		{
			return NKCVoiceActorNameTemplet.NoVoiceActorString;
		}
		return voiceActorStringList[actorKey][key];
	}
}
