using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.Templet;

public class NKCVoiceActorNameTemplet
{
	private string voiceActorNameStrID;

	private string actorNameVKOR;

	private string actorNameVJPN;

	private string actorNameVCHN;

	private static Dictionary<string, Dictionary<NKC_VOICE_CODE, string>> voiceActorList_v2 = new Dictionary<string, Dictionary<NKC_VOICE_CODE, string>>();

	public static string NoVoiceActorString => NKCStringTable.GetString("SI_PF_COLLECTION_PROFILE_NO_VOICE_ACTOR");

	public static void LoadFromLua()
	{
		Load("LUA_VOICE_ACTOR_NAME_TEMPLET_V2", voiceActorList_v2);
	}

	private static void Load(string fileName, Dictionary<string, Dictionary<NKC_VOICE_CODE, string>> voiceActorList)
	{
		IEnumerable<NKCVoiceActorNameTemplet> enumerable = NKMTempletLoader.LoadCommonPath("AB_SCRIPT", fileName, "VOICE_ACTOR_NAME_TEMPLET", LoadFromLua);
		if (enumerable == null)
		{
			return;
		}
		voiceActorList.Clear();
		foreach (NKCVoiceActorNameTemplet item in enumerable)
		{
			if (item != null)
			{
				if (!voiceActorList.ContainsKey(item.voiceActorNameStrID))
				{
					voiceActorList.Add(item.voiceActorNameStrID, new Dictionary<NKC_VOICE_CODE, string>());
					voiceActorList[item.voiceActorNameStrID].Add(NKC_VOICE_CODE.NVC_KOR, item.actorNameVKOR);
					voiceActorList[item.voiceActorNameStrID].Add(NKC_VOICE_CODE.NVC_JPN, item.actorNameVJPN);
					voiceActorList[item.voiceActorNameStrID].Add(NKC_VOICE_CODE.NVC_CHN, item.actorNameVCHN);
				}
				else
				{
					Debug.LogError("voiceActorNameStrID: " + item.voiceActorNameStrID + " already exist in LUA_VOICE_ACTOR_NAME_TEMPLET");
				}
			}
		}
	}

	private static NKCVoiceActorNameTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCVoiceActorNameTemplet.cs", 58))
		{
			return null;
		}
		NKCVoiceActorNameTemplet nKCVoiceActorNameTemplet = new NKCVoiceActorNameTemplet();
		int num = 1 & (cNKMLua.GetData("VOICE_ACTOR_NAME_StrID", ref nKCVoiceActorNameTemplet.voiceActorNameStrID) ? 1 : 0);
		cNKMLua.GetData("Actor_Name_VKOR", ref nKCVoiceActorNameTemplet.actorNameVKOR);
		cNKMLua.GetData("Actor_Name_VJPN", ref nKCVoiceActorNameTemplet.actorNameVJPN);
		cNKMLua.GetData("Actor_Name_VCHN", ref nKCVoiceActorNameTemplet.actorNameVCHN);
		if (num == 0)
		{
			return null;
		}
		return nKCVoiceActorNameTemplet;
	}

	public static string FindActorName(string unitStrId, int skinId)
	{
		string text = null;
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinId);
		if (skinTemplet != null)
		{
			text = FindVoiceActorKey(skinTemplet.m_SkinStrID);
		}
		if (!string.IsNullOrEmpty(text))
		{
			return NKCVoiceActorStringTemplet.FindVoiceActorName(text);
		}
		text = FindVoiceActorKey(unitStrId);
		if (string.IsNullOrEmpty(text))
		{
			text = FindVoiceActorKeyFromBaseUnit(NKMUnitManager.GetUnitTempletBase(unitStrId));
		}
		if (string.IsNullOrEmpty(text))
		{
			return NoVoiceActorString;
		}
		return NKCVoiceActorStringTemplet.FindVoiceActorName(text);
	}

	public static string FindActorName(NKCCollectionUnitTemplet collectionUnitTemplet)
	{
		if (collectionUnitTemplet == null)
		{
			return NoVoiceActorString;
		}
		string text = FindVoiceActorKey(collectionUnitTemplet.m_UnitStrID);
		if (string.IsNullOrEmpty(text))
		{
			text = FindVoiceActorKeyFromBaseUnit(NKMUnitManager.GetUnitTempletBase(collectionUnitTemplet.m_UnitStrID));
		}
		if (string.IsNullOrEmpty(text))
		{
			return NoVoiceActorString;
		}
		return NKCVoiceActorStringTemplet.FindVoiceActorName(text);
	}

	public static string FindActorName(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase == null)
		{
			return NoVoiceActorString;
		}
		string text = FindVoiceActorKey(unitTempletBase.m_UnitStrID);
		if (string.IsNullOrEmpty(text))
		{
			text = FindVoiceActorKeyFromBaseUnit(unitTempletBase);
		}
		if (string.IsNullOrEmpty(text))
		{
			return NoVoiceActorString;
		}
		return NKCVoiceActorStringTemplet.FindVoiceActorName(text);
	}

	public static string FindActorName(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return NoVoiceActorString;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase == null)
		{
			return NoVoiceActorString;
		}
		string text = null;
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(unitData.m_SkinID);
		if (skinTemplet != null)
		{
			text = FindVoiceActorKey(skinTemplet.m_SkinStrID);
		}
		if (!string.IsNullOrEmpty(text))
		{
			return NKCVoiceActorStringTemplet.FindVoiceActorName(text);
		}
		text = FindVoiceActorKey(unitTempletBase.m_UnitStrID);
		if (string.IsNullOrEmpty(text))
		{
			text = FindVoiceActorKeyFromBaseUnit(unitTempletBase);
		}
		if (string.IsNullOrEmpty(text))
		{
			return NoVoiceActorString;
		}
		return NKCVoiceActorStringTemplet.FindVoiceActorName(text);
	}

	private static string FindVoiceActorKey(string unitStrID)
	{
		if (!voiceActorList_v2.ContainsKey(unitStrID))
		{
			return null;
		}
		if (!voiceActorList_v2[unitStrID].ContainsKey(NKCUIVoiceManager.CurrentVoiceCode))
		{
			return null;
		}
		return voiceActorList_v2[unitStrID][NKCUIVoiceManager.CurrentVoiceCode];
	}

	private static string FindVoiceActorKeyFromBaseUnit(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase != null && unitTempletBase.m_BaseUnitID > 0)
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(unitTempletBase.m_BaseUnitID);
			if (unitTempletBase2 != null)
			{
				return FindVoiceActorKey(unitTempletBase2.m_UnitStrID);
			}
		}
		return null;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
