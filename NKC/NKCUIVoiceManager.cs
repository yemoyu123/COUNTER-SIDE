using System.Collections.Generic;
using System.Text;
using AssetBundles;
using Cs.Logging;
using NKC.Localization;
using NKC.Templet;
using NKC.UI;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public static class NKCUIVoiceManager
{
	public class VoiceData
	{
		public int idx;

		public VOICE_TYPE voiceType;

		public VOICE_BUNDLE voiceBundle;
	}

	private static StringBuilder m_BundleName = new StringBuilder();

	private static StringBuilder m_AssetName = new StringBuilder();

	private static Dictionary<VOICE_TYPE, List<NKCVoiceTemplet>> m_dicTempletByType = new Dictionary<VOICE_TYPE, List<NKCVoiceTemplet>>();

	private static int m_CurrentSoundUID = 0;

	private static NKCVoiceTemplet m_CurrentVoiceTemplet = null;

	public const string VOICE_BUNDLE_FORMAT = "AB_UI_UNIT_VOICE_{0}";

	private const string VOICE_PREF_KEY = "LOCAL_VOICE_CODE";

	private const string DEFAULT_LANGUAGE_TAG = "DEFAULT_VOICE_{0}";

	private static HashSet<string> s_setLoadedAssetBundleNames = new HashSet<string>();

	public static NKC_VOICE_CODE CurrentVoiceCode { get; private set; }

	public static bool NeedSelectVoice()
	{
		NKC_VOICE_CODE item = (NKC_VOICE_CODE)PlayerPrefs.GetInt("LOCAL_VOICE_CODE", 0);
		List<NKC_VOICE_CODE> availableVoiceCode = GetAvailableVoiceCode();
		if (PlayerPrefs.HasKey("LOCAL_VOICE_CODE") && availableVoiceCode.Contains(item))
		{
			Debug.Log("[VoiceManager] Local voice code is exist.");
			return false;
		}
		if (availableVoiceCode.Count <= 1)
		{
			Debug.Log($"[VoiceManager] Available voice count : {availableVoiceCode.Count}.");
			return false;
		}
		return true;
	}

	public static void DeleteLocalVoiceCode()
	{
		PlayerPrefs.DeleteKey("LOCAL_VOICE_CODE");
	}

	public static NKC_VOICE_CODE LoadLocalVoiceCode()
	{
		NKC_VOICE_CODE nKC_VOICE_CODE = (NKC_VOICE_CODE)PlayerPrefs.GetInt("LOCAL_VOICE_CODE", 0);
		List<NKC_VOICE_CODE> availableVoiceCode = GetAvailableVoiceCode();
		if (availableVoiceCode.Contains(nKC_VOICE_CODE))
		{
			return nKC_VOICE_CODE;
		}
		foreach (NKC_VOICE_CODE item in availableVoiceCode)
		{
			if (NKMContentsVersionManager.HasTag($"DEFAULT_VOICE_{NKCLocalization.GetVariant(item).ToUpper()}"))
			{
				return item;
			}
		}
		if (availableVoiceCode.Count > 0)
		{
			return availableVoiceCode[0];
		}
		return NKC_VOICE_CODE.NVC_KOR;
	}

	public static List<NKC_VOICE_CODE> GetAvailableVoiceCode()
	{
		List<NKC_VOICE_CODE> list = new List<NKC_VOICE_CODE>();
		foreach (KeyValuePair<string, NKC_VOICE_CODE> item in NKCLocalization.s_dicVoiceTag)
		{
			if (NKMContentsVersionManager.HasTag(item.Key))
			{
				list.Add(item.Value);
			}
		}
		return list;
	}

	public static void SetVoiceCode(NKC_VOICE_CODE code)
	{
		CurrentVoiceCode = code;
		PlayerPrefs.SetInt("LOCAL_VOICE_CODE", (int)code);
		PlayerPrefs.Save();
	}

	public static string GetVoiceLanguageName(NKC_VOICE_CODE code)
	{
		return NKCStringTable.GetString("SI_PF_OPTION_VOICE_" + code);
	}

	public static int GetCurrentSoundUID()
	{
		return m_CurrentSoundUID;
	}

	public static void Init()
	{
		LoadLua("LUA_VOICE_TEMPLET");
	}

	private static string GetAssetName(string unitStrID, string voicePostID)
	{
		m_AssetName.Remove(0, m_AssetName.Length);
		m_AssetName.AppendFormat("AB_UI_UNIT_VOICE_{0}_{1}", unitStrID, voicePostID);
		return m_AssetName.ToString();
	}

	public static bool CheckAsset(string unitStrID, int skinID, string postID, VOICE_BUNDLE bundleType, bool IgnoreVoiceBundleCheck = true)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitStrID);
		if (unitTempletBase == null || !unitTempletBase.m_bExistVoiceBundle)
		{
			return false;
		}
		string text = ((unitTempletBase.BaseUnit == null) ? "" : unitTempletBase.BaseUnit.m_UnitStrID);
		if (bundleType.HasFlag(VOICE_BUNDLE.SKIN))
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
			if (skinTemplet != null && !string.IsNullOrEmpty(skinTemplet.m_VoiceBundleName))
			{
				if (CheckAsset(skinTemplet.m_VoiceBundleName, GetAssetName(unitStrID, postID), IgnoreVoiceBundleCheck))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(text) && CheckAsset(skinTemplet.m_VoiceBundleName, GetAssetName(text, postID), IgnoreVoiceBundleCheck))
				{
					return true;
				}
			}
		}
		if (bundleType.HasFlag(VOICE_BUNDLE.UNIT))
		{
			m_BundleName.Clear();
			m_BundleName.AppendFormat("AB_UI_UNIT_VOICE_{0}", unitStrID);
			if (CheckAsset(m_BundleName.ToString(), GetAssetName(unitStrID, postID), IgnoreVoiceBundleCheck))
			{
				return true;
			}
			if (!string.IsNullOrEmpty(text))
			{
				m_BundleName.Clear();
				m_BundleName.AppendFormat("AB_UI_UNIT_VOICE_{0}", text);
				if (CheckAsset(m_BundleName.ToString(), GetAssetName(text, postID), IgnoreVoiceBundleCheck))
				{
					return true;
				}
			}
		}
		if (bundleType.HasFlag(VOICE_BUNDLE.COMMON))
		{
			m_BundleName.Clear();
			if (!string.IsNullOrEmpty(unitTempletBase.m_CommonVoiceBundle))
			{
				m_BundleName.AppendFormat("AB_UI_UNIT_VOICE_{0}", unitTempletBase.m_CommonVoiceBundle);
				if (CheckAsset(m_BundleName.ToString(), GetAssetName(unitTempletBase.m_CommonVoiceBundle, postID), IgnoreVoiceBundleCheck))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static List<NKMAssetName> GetAssetList(string unitStrID, int skinID, string postID, VOICE_BUNDLE bundleType)
	{
		List<NKMAssetName> list = new List<NKMAssetName>();
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitStrID);
		if (unitTempletBase == null || !unitTempletBase.m_bExistVoiceBundle)
		{
			return list;
		}
		string text = "";
		if (unitTempletBase.BaseUnit != null)
		{
			text = unitTempletBase.BaseUnit.m_UnitStrID;
		}
		if (bundleType.HasFlag(VOICE_BUNDLE.SKIN))
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
			if (skinTemplet != null)
			{
				if (CheckAsset(skinTemplet.m_VoiceBundleName, GetAssetName(unitStrID, postID)))
				{
					list.Add(new NKMAssetName(skinTemplet.m_VoiceBundleName, GetAssetName(unitStrID, postID)));
				}
				else if (!string.IsNullOrEmpty(text) && CheckAsset(skinTemplet.m_VoiceBundleName, GetAssetName(text, postID)))
				{
					list.Add(new NKMAssetName(skinTemplet.m_VoiceBundleName, GetAssetName(text, postID)));
				}
			}
		}
		if (bundleType.HasFlag(VOICE_BUNDLE.UNIT))
		{
			m_BundleName.Remove(0, m_BundleName.Length);
			m_BundleName.AppendFormat("AB_UI_UNIT_VOICE_{0}", unitStrID);
			if (CheckAsset(m_BundleName.ToString(), GetAssetName(unitStrID, postID)))
			{
				list.Add(new NKMAssetName(m_BundleName.ToString(), GetAssetName(unitStrID, postID)));
			}
			else if (!string.IsNullOrEmpty(text))
			{
				m_BundleName.Clear();
				m_BundleName.AppendFormat("AB_UI_UNIT_VOICE_{0}", text);
				if (CheckAsset(m_BundleName.ToString(), GetAssetName(text, postID)))
				{
					list.Add(new NKMAssetName(m_BundleName.ToString(), GetAssetName(text, postID)));
				}
			}
		}
		if (bundleType.HasFlag(VOICE_BUNDLE.COMMON))
		{
			m_BundleName.Remove(0, m_BundleName.Length);
			if (!string.IsNullOrEmpty(unitTempletBase.m_CommonVoiceBundle))
			{
				m_BundleName.AppendFormat("AB_UI_UNIT_VOICE_{0}", unitTempletBase.m_CommonVoiceBundle);
				if (CheckAsset(m_BundleName.ToString(), GetAssetName(unitTempletBase.m_CommonVoiceBundle, postID)))
				{
					list.Add(new NKMAssetName(m_BundleName.ToString(), GetAssetName(unitTempletBase.m_CommonVoiceBundle, postID)));
				}
			}
		}
		return list;
	}

	public static int PlayVoice(VOICE_TYPE type, NKMUnitData unitData, bool bIgnoreShowNormalAfterLifeTimeOption = false, bool bShowCaption = false)
	{
		if (unitData == null)
		{
			return 0;
		}
		if (!m_dicTempletByType.ContainsKey(type))
		{
			return 0;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase == null)
		{
			return 0;
		}
		if (NKMRandom.Range(0, 101) > m_dicTempletByType[type][0].Rate)
		{
			return 0;
		}
		List<NKCVoiceTemplet> list = m_dicTempletByType[type].FindAll((NKCVoiceTemplet v) => CheckCondition(unitData, v.Condition, v.ConditionValue));
		if (list.Count == 0)
		{
			return 0;
		}
		return PlayVoice(type, unitTempletBase, unitData.m_SkinID, list, bIgnoreShowNormalAfterLifeTimeOption, bShowCaption);
	}

	public static int PlayVoice(VOICE_TYPE type, int unitID, int skinID = 0, bool bIgnoreShowNormalAfterLifeTimeOption = false, bool bShowCaption = false)
	{
		string unitStrID = NKMUnitManager.GetUnitStrID(unitID);
		if (string.IsNullOrEmpty(unitStrID))
		{
			Log.Error($"VoiceManager : unitID -> unitStrID Error ({unitID})", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCUIVoiceManager.cs", 403);
			return 0;
		}
		return PlayVoice(type, unitStrID, skinID, bIgnoreShowNormalAfterLifeTimeOption, bShowCaption);
	}

	public static int PlayVoice(VOICE_TYPE type, string unitStrID, int skinID = 0, bool bIgnoreShowNormalAfterLifeTimeOption = false, bool bShowCaption = false)
	{
		if (!m_dicTempletByType.ContainsKey(type))
		{
			return 0;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitStrID);
		if (unitTempletBase == null)
		{
			return 0;
		}
		if (NKMRandom.Range(0, 101) > m_dicTempletByType[type][0].Rate)
		{
			return 0;
		}
		List<NKCVoiceTemplet> list = m_dicTempletByType[type].FindAll((NKCVoiceTemplet v) => v.Condition == VOICE_CONDITION.VC_NONE);
		if (list.Count == 0)
		{
			return 0;
		}
		return PlayVoice(type, unitTempletBase, skinID, list, bIgnoreShowNormalAfterLifeTimeOption, bShowCaption);
	}

	public static int PlayOperatorVoice(VOICE_TYPE type, NKMOperator operatorData, bool bShowCaption = false)
	{
		return PlayVoice(type, operatorData, bShowCaption);
	}

	public static int PlayVoice(VOICE_TYPE type, NKMOperator operatorData, bool bShowCaption = false, bool bStopCurrentVoice = true)
	{
		if (operatorData == null)
		{
			return 0;
		}
		if (!m_dicTempletByType.ContainsKey(type))
		{
			return 0;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData);
		if (unitTempletBase == null)
		{
			return 0;
		}
		if (NKMRandom.Range(0, 101) > m_dicTempletByType[type][0].Rate)
		{
			return 0;
		}
		List<NKCVoiceTemplet> list = m_dicTempletByType[type].FindAll((NKCVoiceTemplet v) => CheckCondition(operatorData, v.Condition, v.ConditionValue));
		if (list.Count == 0)
		{
			return 0;
		}
		return PlayVoice(type, unitTempletBase, 0, list, bIgnoreShowNormalAfterLifeTimeOption: false, bShowCaption, bStopCurrentVoice);
	}

	public static int PlayRandomVoiceInBundle(string bundleName)
	{
		if (!IsAssetBundleLoaded(bundleName))
		{
			LoadAssetBundle(bundleName);
		}
		string[] allAssetNameInBundle = AssetBundleManager.GetAllAssetNameInBundle(bundleName);
		if (allAssetNameInBundle != null && allAssetNameInBundle.Length != 0)
		{
			int num = NKMRandom.Range(0, allAssetNameInBundle.Length);
			return NKCSoundManager.PlayVoice(bundleName, allAssetNameInBundle[num], 0, bClearVoice: false, bIgnoreSameVoice: false, 1f, 0f, 0f);
		}
		Debug.LogWarning("Failed to load asset. bundleName : " + bundleName);
		return 0;
	}

	public static void StopVoice()
	{
		if (m_CurrentSoundUID != 0)
		{
			NKCSoundManager.StopSound(m_CurrentSoundUID);
			NKCUIManager.NKCUIOverlayCaption.CloseCaption(m_CurrentSoundUID);
		}
	}

	public static List<NKCVoiceTemplet> GetVoiceList(VOICE_TYPE type, NKMUnitTempletBase unitTempletBase, int skinID, List<NKCVoiceTemplet> templetList, bool bIncludeIfStringExist, out string targetUnitStrID, out VOICE_BUNDLE voiceFlag)
	{
		string unitStrID = unitTempletBase.m_UnitStrID;
		string baseUnitStrID = "";
		if (unitTempletBase.BaseUnit != null)
		{
			baseUnitStrID = unitTempletBase.BaseUnit.m_UnitStrID;
		}
		List<NKCVoiceTemplet> list = new List<NKCVoiceTemplet>();
		VOICE_BUNDLE flag = VOICE_BUNDLE.NONE;
		if (skinID > 0)
		{
			flag = VOICE_BUNDLE.SKIN;
			list = templetList.FindAll((NKCVoiceTemplet v) => CheckAsset(unitStrID, skinID, v.FileName, flag, bIncludeIfStringExist));
			if (list.Count == 0 && !string.IsNullOrEmpty(baseUnitStrID))
			{
				list = templetList.FindAll((NKCVoiceTemplet v) => CheckAsset(baseUnitStrID, skinID, v.FileName, flag, bIncludeIfStringExist));
				if (list.Count > 0)
				{
					unitStrID = baseUnitStrID;
				}
			}
		}
		if (list.Count == 0)
		{
			flag = VOICE_BUNDLE.UNIT | VOICE_BUNDLE.COMMON;
			list = templetList.FindAll((NKCVoiceTemplet v) => CheckAsset(unitStrID, skinID, v.FileName, flag, bIncludeIfStringExist));
			if (list.Count == 0 && !string.IsNullOrEmpty(baseUnitStrID))
			{
				list = templetList.FindAll((NKCVoiceTemplet v) => CheckAsset(baseUnitStrID, skinID, v.FileName, flag, bIncludeIfStringExist));
				if (list.Count > 0)
				{
					unitStrID = baseUnitStrID;
				}
			}
		}
		voiceFlag = flag;
		targetUnitStrID = unitStrID;
		return list;
	}

	private static int PlayVoice(VOICE_TYPE type, NKMUnitTempletBase unitTempletBase, int skinID, List<NKCVoiceTemplet> templetList, bool bIgnoreShowNormalAfterLifeTimeOption = false, bool bShowCaption = false, bool bStopCurrentVoice = true)
	{
		if (unitTempletBase == null)
		{
			return 0;
		}
		string targetUnitStrID;
		VOICE_BUNDLE voiceFlag;
		List<NKCVoiceTemplet> list = GetVoiceList(type, unitTempletBase, skinID, templetList, bIncludeIfStringExist: false, out targetUnitStrID, out voiceFlag);
		if (list.Count == 0 && bShowCaption)
		{
			list = GetVoiceList(type, unitTempletBase, skinID, templetList, bIncludeIfStringExist: true, out targetUnitStrID, out voiceFlag);
		}
		if (list.Count == 0)
		{
			templetList.Exists((NKCVoiceTemplet v) => !string.IsNullOrEmpty(v.Npc));
			return 0;
		}
		bool flag = false;
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			flag = gameOptionData.UseShowNormalSubtitleAfterLifeTime;
		}
		if ((!(!bIgnoreShowNormalAfterLifeTimeOption && flag) || !list.Exists((NKCVoiceTemplet v) => v.Condition == VOICE_CONDITION.VC_LIFETIME)) && list.Exists((NKCVoiceTemplet v) => v.Condition != VOICE_CONDITION.VC_NONE))
		{
			list = list.FindAll((NKCVoiceTemplet v) => v.Condition != VOICE_CONDITION.VC_NONE);
		}
		List<NKMAssetName> list2 = new List<NKMAssetName>();
		Dictionary<int, List<NKMAssetName>> dictionary = new Dictionary<int, List<NKMAssetName>>();
		for (int num = 0; num < list.Count; num++)
		{
			List<NKMAssetName> assetList = GetAssetList(targetUnitStrID, skinID, list[num].FileName, voiceFlag);
			list2.AddRange(assetList);
			dictionary.Add(num, assetList);
		}
		int index = NKMRandom.Range(0, list2.Count);
		NKMAssetName nKMAssetName = list2[index];
		int index2 = 0;
		foreach (KeyValuePair<int, List<NKMAssetName>> item in dictionary)
		{
			if (item.Value.Contains(nKMAssetName))
			{
				index2 = item.Key;
				break;
			}
		}
		NKCVoiceTemplet nKCVoiceTemplet = list[index2];
		if (NKCSoundManager.IsPlayingVoice(m_CurrentSoundUID) && m_CurrentVoiceTemplet != null && m_CurrentVoiceTemplet.Priority < nKCVoiceTemplet.Priority)
		{
			return m_CurrentSoundUID;
		}
		if (m_CurrentSoundUID != 0 && bStopCurrentVoice)
		{
			NKCSoundManager.StopSound(m_CurrentSoundUID);
		}
		Log.Debug($"[Voice] play {nKMAssetName.m_BundleName} - {nKMAssetName.m_AssetName}(voiceTemplet:{nKCVoiceTemplet.Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCUIVoiceManager.cs", 625);
		float delayTime = NKCVoiceTimingManager.GetDelayTime(unitTempletBase.m_UnitID, skinID, nKCVoiceTemplet);
		m_CurrentSoundUID = NKCSoundManager.PlayVoice(nKMAssetName.m_BundleName, nKMAssetName.m_AssetName, 0, bStopCurrentVoice, bIgnoreSameVoice: false, (float)nKCVoiceTemplet.Volume * 0.01f, 0f, 0f, bLoop: false, 0f, bShowCaption, 0f, delayTime);
		if (m_CurrentSoundUID > 0)
		{
			m_CurrentVoiceTemplet = nKCVoiceTemplet;
		}
		return m_CurrentSoundUID;
	}

	public static int ForcePlayVoice(NKMAssetName assetName, float delayTime = 0f, float volume = 1f, bool bShowCaption = false, bool bStopCurrentVoice = true)
	{
		if (m_CurrentSoundUID != 0 && bStopCurrentVoice)
		{
			NKCSoundManager.StopSound(m_CurrentSoundUID);
		}
		Log.Debug("[Voice] ForcePlay " + assetName.m_BundleName + " - " + assetName.m_AssetName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCUIVoiceManager.cs", 646);
		m_CurrentSoundUID = NKCSoundManager.PlayVoice(assetName.m_BundleName, assetName.m_AssetName, 0, bStopCurrentVoice, bIgnoreSameVoice: false, volume, 0f, 0f, bLoop: false, 0f, bShowCaption, 0f, delayTime);
		if (m_CurrentSoundUID > 0)
		{
			m_CurrentVoiceTemplet = null;
		}
		return m_CurrentSoundUID;
	}

	public static bool GetVoiceBundleInfo(NKMUnitData unitData, string audioClipName, out NKMAssetName assetName)
	{
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(unitData);
		if (skinTemplet != null && CheckAsset(skinTemplet.m_VoiceBundleName, audioClipName))
		{
			assetName = new NKMAssetName(skinTemplet.m_VoiceBundleName, audioClipName);
			return true;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase != null)
		{
			string bundleName = $"AB_UI_UNIT_VOICE_{unitTempletBase.m_UnitStrID}";
			if (CheckAsset(bundleName, audioClipName))
			{
				assetName = new NKMAssetName(bundleName, audioClipName);
				return true;
			}
			if (unitTempletBase.BaseUnit != null)
			{
				bundleName = $"AB_UI_UNIT_VOICE_{unitTempletBase.BaseUnit.m_UnitStrID}";
				if (CheckAsset(bundleName, audioClipName))
				{
					assetName = new NKMAssetName(bundleName, audioClipName);
					return true;
				}
			}
		}
		assetName = new NKMAssetName();
		return false;
	}

	private static bool CheckCondition(NKMUnitData unitData, VOICE_CONDITION condition, int value)
	{
		switch (condition)
		{
		case VOICE_CONDITION.VC_NONE:
			return true;
		case VOICE_CONDITION.VC_LEVEL:
			if (unitData.m_UnitLevel >= value)
			{
				return true;
			}
			break;
		case VOICE_CONDITION.VC_LIFETIME:
			if (unitData.IsPermanentContract)
			{
				return true;
			}
			break;
		case VOICE_CONDITION.VC_DEVOTION:
			if (unitData.loyalty >= value)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private static bool CheckCondition(NKMOperator operatorData, VOICE_CONDITION condition, int value)
	{
		switch (condition)
		{
		case VOICE_CONDITION.VC_NONE:
			return true;
		case VOICE_CONDITION.VC_LEVEL:
			if (operatorData.level >= value)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private static void LoadLua(string fileName)
	{
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT", fileName) && nKMLua.OpenTable("m_voiceTemplet"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKCVoiceTemplet nKCVoiceTemplet = new NKCVoiceTemplet();
				nKCVoiceTemplet.LoadLUA(nKMLua);
				if (!m_dicTempletByType.ContainsKey(nKCVoiceTemplet.Type))
				{
					m_dicTempletByType.Add(nKCVoiceTemplet.Type, new List<NKCVoiceTemplet>());
				}
				m_dicTempletByType[nKCVoiceTemplet.Type].Add(nKCVoiceTemplet);
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
	}

	public static void CleanUp()
	{
		foreach (string s_setLoadedAssetBundleName in s_setLoadedAssetBundleNames)
		{
			AssetBundleManager.UnloadAssetBundle(s_setLoadedAssetBundleName);
		}
		s_setLoadedAssetBundleNames.Clear();
	}

	private static void LoadAssetBundle(string bundleName)
	{
		bundleName = bundleName.ToLower();
		s_setLoadedAssetBundleNames.Add(bundleName);
		AssetBundleManager.LoadAssetBundle(bundleName, async: false);
	}

	private static bool IsAssetBundleLoaded(string bundleName)
	{
		bundleName = bundleName.ToLower();
		if (s_setLoadedAssetBundleNames.Contains(bundleName))
		{
			return true;
		}
		return AssetBundleManager.IsAssetBundleLoaded(bundleName);
	}

	private static bool CheckAsset(string bundleName, string assetName, bool IgnoreVoiceBundleCheck = true)
	{
		bundleName = bundleName.ToLower();
		if (!AssetBundleManager.IsBundleExists(bundleName))
		{
			if (IgnoreVoiceBundleCheck && NKCStringTable.CheckExistString(NKCUtilString.GetVoiceKey(bundleName, assetName)))
			{
				return true;
			}
			return false;
		}
		if (!IsAssetBundleLoaded(bundleName))
		{
			LoadAssetBundle(bundleName);
		}
		return AssetBundleManager.IsAssetExists(bundleName, assetName);
	}

	public static List<NKCVoiceTemplet> GetTemplets()
	{
		List<NKCVoiceTemplet> list = new List<NKCVoiceTemplet>();
		foreach (List<NKCVoiceTemplet> value in m_dicTempletByType.Values)
		{
			list.AddRange(value);
		}
		return list;
	}

	public static List<NKCVoiceTemplet> GetVoiceTempletByType(VOICE_TYPE type)
	{
		if (!m_dicTempletByType.ContainsKey(type))
		{
			return new List<NKCVoiceTemplet>();
		}
		return m_dicTempletByType[type];
	}

	public static NKMAssetName PlayOnUI(string unitStrID, int skinID, string fileName, float vol, VOICE_BUNDLE bundleType, bool bShowCaption = false)
	{
		if (CheckAsset(unitStrID, skinID, fileName, bundleType))
		{
			List<NKMAssetName> assetList = GetAssetList(unitStrID, skinID, fileName, bundleType);
			if (assetList.Count == 0)
			{
				return null;
			}
			if (m_CurrentSoundUID != 0)
			{
				NKCSoundManager.StopSound(m_CurrentSoundUID);
			}
			m_CurrentSoundUID = NKCSoundManager.PlayVoice(assetList[0].m_BundleName, assetList[0].m_AssetName, 0, bClearVoice: true, bIgnoreSameVoice: false, vol * 0.01f, 0f, 0f, bLoop: false, 0f, bShowCaption);
			return assetList[0];
		}
		return null;
	}

	public static bool UnitHasVoice(NKMUnitTempletBase unitTemplet)
	{
		if (unitTemplet == null)
		{
			return false;
		}
		if (!unitTemplet.m_bExistVoiceBundle)
		{
			return false;
		}
		string text = "";
		text = ((unitTemplet.BaseUnit == null) ? $"AB_UI_UNIT_VOICE_{unitTemplet.m_UnitStrID}" : $"AB_UI_UNIT_VOICE_{unitTemplet.BaseUnit.m_UnitStrID}");
		text = text.ToLower();
		return AssetBundleManager.IsBundleExists(text);
	}

	public static bool IsPlayingVoice(int soundUID = -1)
	{
		return NKCSoundManager.IsPlayingVoice(soundUID);
	}

	public static List<VoiceData> GetListVoice(string unitStrID, int skinID, bool bLifetime)
	{
		List<VoiceData> list = new List<VoiceData>();
		foreach (NKCCollectionVoiceTemplet voiceTemplet in NKMTempletContainer<NKCCollectionVoiceTemplet>.Values)
		{
			if (voiceTemplet.m_bVoiceCondLifetime && !bLifetime)
			{
				continue;
			}
			if (CheckVoice(unitStrID, skinID, voiceTemplet, VOICE_BUNDLE.SKIN, IgnoreVoiceBundleCheck: false))
			{
				if (voiceTemplet.m_VoiceType != VOICE_TYPE.VT_NONE)
				{
					list.RemoveAll((VoiceData v) => v.voiceType == voiceTemplet.m_VoiceType && v.voiceBundle != VOICE_BUNDLE.SKIN);
				}
				VoiceData voiceData = new VoiceData();
				voiceData.idx = voiceTemplet.IDX;
				voiceData.voiceBundle = VOICE_BUNDLE.SKIN;
				voiceData.voiceType = voiceTemplet.m_VoiceType;
				list.Add(voiceData);
			}
			else
			{
				if (voiceTemplet.m_VoiceType != VOICE_TYPE.VT_NONE && list.Exists((VoiceData v) => v.voiceType == voiceTemplet.m_VoiceType && v.voiceBundle == VOICE_BUNDLE.SKIN))
				{
					continue;
				}
				if (CheckVoice(unitStrID, skinID, voiceTemplet, VOICE_BUNDLE.UNIT, IgnoreVoiceBundleCheck: false))
				{
					VoiceData voiceData2 = new VoiceData();
					voiceData2.idx = voiceTemplet.IDX;
					voiceData2.voiceBundle = VOICE_BUNDLE.UNIT;
					voiceData2.voiceType = voiceTemplet.m_VoiceType;
					list.Add(voiceData2);
				}
				else if (CheckVoice(unitStrID, skinID, voiceTemplet, VOICE_BUNDLE.SKIN, IgnoreVoiceBundleCheck: true))
				{
					if (voiceTemplet.m_VoiceType != VOICE_TYPE.VT_NONE)
					{
						list.RemoveAll((VoiceData v) => v.voiceType == voiceTemplet.m_VoiceType && v.voiceBundle != VOICE_BUNDLE.SKIN);
					}
					VoiceData voiceData3 = new VoiceData();
					voiceData3.idx = voiceTemplet.IDX;
					voiceData3.voiceBundle = VOICE_BUNDLE.SKIN;
					voiceData3.voiceType = voiceTemplet.m_VoiceType;
					list.Add(voiceData3);
				}
				else if (CheckVoice(unitStrID, skinID, voiceTemplet, VOICE_BUNDLE.UNIT, IgnoreVoiceBundleCheck: true))
				{
					VoiceData voiceData4 = new VoiceData();
					voiceData4.idx = voiceTemplet.IDX;
					voiceData4.voiceBundle = VOICE_BUNDLE.UNIT;
					voiceData4.voiceType = voiceTemplet.m_VoiceType;
					list.Add(voiceData4);
				}
				else if (CheckVoice(unitStrID, skinID, voiceTemplet, VOICE_BUNDLE.COMMON, IgnoreVoiceBundleCheck: true))
				{
					VoiceData voiceData5 = new VoiceData();
					voiceData5.idx = voiceTemplet.IDX;
					voiceData5.voiceBundle = VOICE_BUNDLE.COMMON;
					voiceData5.voiceType = voiceTemplet.m_VoiceType;
					list.Add(voiceData5);
				}
			}
		}
		return list;
	}

	private static bool CheckVoice(string unitStrID, int skinID, NKCCollectionVoiceTemplet voiceTemplet, VOICE_BUNDLE bundleType, bool IgnoreVoiceBundleCheck)
	{
		return CheckAsset(unitStrID, skinID, voiceTemplet.m_VoicePostID, bundleType, IgnoreVoiceBundleCheck);
	}
}
