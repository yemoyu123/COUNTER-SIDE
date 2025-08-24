using System.Collections.Generic;
using System.IO;
using NKM;
using NKM.Guild;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public static class NKCResourceUtility
{
	public enum eUnitResourceType
	{
		FACE_CARD,
		INVEN_ICON,
		INVEN_ICON_GRAY,
		SPINE_ILLUST,
		SPINE_SD
	}

	public const string UNIT_FACE_CARD_BUNDLE_NAME = "AB_UNIT_FACE_CARD";

	public const string UNIT_INVEN_ICON_BUNDLE_NAME = "AB_INVEN_ICON_UNIT";

	public const string UNIT_INVEN_ICON_NKM_UNIT_EMPTY = "AB_INVEN_ICON_NKM_UNIT_EMPTY";

	public const string UNIT_MINI_MAP_FACE_BUNDLE_NAME = "AB_UNIT_MINI_MAP_FACE";

	public const string MISC_ITEM_ICON_BUNDLE_NAME = "AB_INVEN_ICON_ITEM_MISC";

	public const string MISC_ITEM_EMBLEM_ICON_BUNDLE_NAME = "AB_INVEN_ICON_EMBLEM";

	public const string MISC_ITEM_PIECE_ICON_BUNDLE_NAME = "AB_INVEN_ICON_UNIT_PIECE";

	public const string MISC_ITEM_BG_ICON_BUNDLE_NAME = "AB_INVEN_ICON_BG";

	public const string MISC_ITEM_SELFIE_FRAME_BUNDLE_NAME = "AB_INVEN_ICON_BORDER";

	public const string MISC_ITEM_INTERIOR_BUNDLE_NAME = "AB_INVEN_ICON_FNC";

	public const string MISC_ITEM_USERTITLE_BUNDLE_NAME = "AB_INVEN_ICON_USERTITLE";

	public const string COMMON_ICON_BUNDLE_NAME = "ab_ui_nkm_ui_common_icon";

	public const string INGAME_ICON_BUNDLE_NAME = "AB_UNIT_GAME_NKM_UNIT_SPRITE";

	public const string MISC_SMALL_ITEM_ICON_BUNDLE_NAME = "AB_INVEN_ICON_ITEM_MISC_SMALL";

	public const string EQUIP_ICON_BUNDLE_NAME = "AB_INVEN_ICON_ITEM_EQUIP";

	public const string MOLD_ICON_BUNDLE_NAME = "AB_INVEN_ICON_ITEM_MISC";

	public const string BUFF_ICON_BUNDLE_NAME = "AB_INVEN_ICON_ITEM_MISC";

	public const string DIVE_ARTIFACT_ICON_BUNDLE_NAME = "AB_UI_NKM_UI_WORLD_MAP_DIVE_ARTIFACT";

	public const string DIVE_ARTIFACT_BIG_ICON_BUNDLE_NAME = "AB_UI_NKM_UI_WORLD_MAP_DIVE_ARTIFACT_BIG";

	public const string GUILD_ARTIFACT_ICON_BUNDLE_NAME = "AB_UI_NKM_UI_WORLD_MAP_DIVE_ARTIFACT";

	public const string GUILD_ARTIFACT_BIG_ICON_BUNDLE_NAME = "AB_UI_NKM_UI_WORLD_MAP_DIVE_ARTIFACT_BIG";

	public const string EMOTICON_ICON_BUNDLE_NAME = "AB_UI_NKM_UI_EMOTICON_ICON";

	private static Dictionary<string, NKCAssetResourceBundle> m_dicAssetResourceBundle = new Dictionary<string, NKCAssetResourceBundle>();

	private static Dictionary<string, NKCAssetResourceBundle> m_dicAssetResourceBundleTemp = new Dictionary<string, NKCAssetResourceBundle>();

	public static T GetOrLoadAssetResource<T>(NKMAssetName cNKMAssetName) where T : Object
	{
		return GetOrLoadAssetResource<T>(cNKMAssetName.m_BundleName, cNKMAssetName.m_AssetName);
	}

	public static T GetOrLoadAssetResource<T>(string bundleName, string assetName, bool tryParseAssetName = false) where T : Object
	{
		if (string.IsNullOrEmpty(bundleName))
		{
			return null;
		}
		if (string.IsNullOrEmpty(assetName))
		{
			return null;
		}
		if (tryParseAssetName)
		{
			NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(bundleName, assetName);
			bundleName = nKMAssetName.m_BundleName;
			assetName = nKMAssetName.m_AssetName;
		}
		if (!HasAssetResourceBundle(bundleName, assetName))
		{
			LoadAssetResourceTemp<T>(bundleName, assetName, bAsync: false);
		}
		NKCAssetResourceData assetResource = GetAssetResource(bundleName, assetName);
		if (assetResource == null)
		{
			return null;
		}
		return assetResource.GetAsset<T>();
	}

	private static bool HasAssetResourceBundle(string bundleName, string assetName)
	{
		if (string.IsNullOrEmpty(assetName))
		{
			return false;
		}
		bundleName = NKCAssetResourceManager.RemapLocBundle(bundleName, assetName);
		if (m_dicAssetResourceBundle.ContainsKey(bundleName) && m_dicAssetResourceBundle[bundleName].m_dicNKCResourceData.ContainsKey(assetName))
		{
			return true;
		}
		return false;
	}

	public static void LoadAssetResourceTemp<T>(NKMAssetName cNKMAssetName, bool bAsync = true) where T : Object
	{
		LoadAssetResourceTemp<T>(cNKMAssetName.m_BundleName, cNKMAssetName.m_AssetName, bAsync);
	}

	public static void LoadAssetResourceTemp<T>(string assetName, bool bAsync = true) where T : Object
	{
		LoadAssetResourceTemp<T>(NKCAssetResourceManager.GetBundleName(assetName), assetName, bAsync);
	}

	public static void LoadAssetResourceTemp<T>(string bundleName, string assetName, bool bAsync = true) where T : Object
	{
		bundleName = NKCAssetResourceManager.RemapLocBundle(bundleName, assetName);
		NKCAssetResourceBundle nKCAssetResourceBundle = null;
		nKCAssetResourceBundle = ((!bAsync) ? GetAssetResourceBundle(bundleName, m_dicAssetResourceBundle) : GetAssetResourceBundle(bundleName, m_dicAssetResourceBundleTemp));
		if (!nKCAssetResourceBundle.m_dicNKCResourceData.ContainsKey(assetName))
		{
			NKCAssetResourceData value = NKCAssetResourceManager.OpenResource<T>(bundleName, assetName, bAsync);
			nKCAssetResourceBundle.m_dicNKCResourceData.Add(assetName, value);
		}
	}

	public static Sprite GetRewardInvenIcon(NKMRewardInfo rewardInfo, bool bSmall = false)
	{
		return GetRewardInvenIcon(rewardInfo.rewardType, rewardInfo.ID, bSmall);
	}

	public static Sprite GetRewardInvenIcon(NKM_REWARD_TYPE rewardType, int id, bool bSmall = false)
	{
		switch (rewardType)
		{
		default:
			return null;
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMUnitTempletBase cNKMUnitTempletBase = NKMUnitTempletBase.Find(id);
			return GetorLoadUnitSprite(eUnitResourceType.INVEN_ICON, cNKMUnitTempletBase);
		}
		case NKM_REWARD_TYPE.RT_MISC:
		case NKM_REWARD_TYPE.RT_MISSION_POINT:
		case NKM_REWARD_TYPE.RT_PASS_EXP:
			NKMItemMiscTemplet.Find(id);
			if (bSmall)
			{
				return GetOrLoadMiscItemSmallIcon(id);
			}
			return GetOrLoadMiscItemIcon(id);
		case NKM_REWARD_TYPE.RT_USER_EXP:
			if (bSmall)
			{
				return GetOrLoadMiscItemSmallIcon(501);
			}
			return GetOrLoadMiscItemIcon(501);
		case NKM_REWARD_TYPE.RT_EQUIP:
			return GetOrLoadEquipIcon(NKMItemManager.GetEquipTemplet(id));
		case NKM_REWARD_TYPE.RT_MOLD:
			return GetOrLoadMoldIcon(NKMItemMoldTemplet.Find(id));
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(id);
			return GetorLoadUnitSprite(eUnitResourceType.INVEN_ICON, skinTemplet);
		}
		case NKM_REWARD_TYPE.RT_BUFF:
			return GetOrLoadBuffIconForItemPopup(NKMCompanyBuffTemplet.Find(id));
		case NKM_REWARD_TYPE.RT_EMOTICON:
			return GetOrLoadEmoticonIcon(NKMEmoticonTemplet.Find(id));
		}
	}

	public static void PreloadUnitResource(eUnitResourceType type, NKMUnitData cNKMUnitData, bool bAsync = true)
	{
		if (cNKMUnitData != null)
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(cNKMUnitData);
			if (skinTemplet != null)
			{
				PreloadUnitResource(type, skinTemplet, bAsync);
				return;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData.m_UnitID);
			PreloadUnitResource(type, unitTempletBase, bAsync);
		}
	}

	public static void PreloadUnitResource(eUnitResourceType type, NKMSkinTemplet cNKMSkinTemplet, bool bAsync = true)
	{
		if (cNKMSkinTemplet != null)
		{
			switch (type)
			{
			case eUnitResourceType.FACE_CARD:
				LoadAssetResourceTemp<Sprite>("AB_UNIT_FACE_CARD", cNKMSkinTemplet.m_FaceCardName, bAsync);
				break;
			case eUnitResourceType.INVEN_ICON:
				LoadAssetResourceTemp<Sprite>("AB_INVEN_ICON_UNIT", cNKMSkinTemplet.m_InvenIconName, bAsync);
				break;
			case eUnitResourceType.INVEN_ICON_GRAY:
				LoadAssetResourceTemp<Sprite>("AB_INVEN_ICON_UNIT", cNKMSkinTemplet.m_InvenIconName + "_GRAY", bAsync);
				break;
			case eUnitResourceType.SPINE_ILLUST:
				LoadAssetResourceTemp<GameObject>(cNKMSkinTemplet.m_SpineIllustName, cNKMSkinTemplet.m_SpineIllustName);
				break;
			case eUnitResourceType.SPINE_SD:
				LoadAssetResourceTemp<GameObject>(cNKMSkinTemplet.m_SpineSDName, cNKMSkinTemplet.m_SpineSDName);
				break;
			}
		}
	}

	public static void PreloadUnitResource(eUnitResourceType type, NKMUnitTempletBase cNKMUnitTempletBase, bool bAsync = true)
	{
		if (cNKMUnitTempletBase != null)
		{
			switch (type)
			{
			case eUnitResourceType.FACE_CARD:
				LoadAssetResourceTemp<Sprite>("AB_UNIT_FACE_CARD", cNKMUnitTempletBase.m_FaceCardName, bAsync);
				break;
			case eUnitResourceType.INVEN_ICON:
				LoadAssetResourceTemp<Sprite>("AB_INVEN_ICON_UNIT", cNKMUnitTempletBase.m_InvenIconName, bAsync);
				break;
			case eUnitResourceType.INVEN_ICON_GRAY:
				LoadAssetResourceTemp<Sprite>("AB_INVEN_ICON_UNIT", cNKMUnitTempletBase.m_InvenIconName + "_GRAY", bAsync);
				break;
			case eUnitResourceType.SPINE_ILLUST:
				LoadAssetResourceTemp<GameObject>(cNKMUnitTempletBase.m_SpineIllustName, cNKMUnitTempletBase.m_SpineIllustName);
				break;
			case eUnitResourceType.SPINE_SD:
				LoadAssetResourceTemp<GameObject>(cNKMUnitTempletBase.m_SpineSDName, cNKMUnitTempletBase.m_SpineSDName);
				break;
			}
		}
	}

	public static NKCAssetResourceData GetUnitResource(eUnitResourceType type, NKMUnitData cNKMUnitData)
	{
		if (cNKMUnitData == null)
		{
			return null;
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(cNKMUnitData);
		if (skinTemplet != null)
		{
			NKCAssetResourceData unitResource = GetUnitResource(type, skinTemplet);
			if (unitResource != null)
			{
				return unitResource;
			}
			Debug.LogError("Skin Unitresource get failed. fallback to base resource");
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData.m_UnitID);
		return GetUnitResource(type, unitTempletBase);
	}

	public static NKCAssetResourceData GetUnitResource(eUnitResourceType type, NKMSkinTemplet cNKMSkinTemplet)
	{
		if (cNKMSkinTemplet == null)
		{
			return null;
		}
		return type switch
		{
			eUnitResourceType.FACE_CARD => GetAssetResource("AB_UNIT_FACE_CARD", cNKMSkinTemplet.m_FaceCardName), 
			eUnitResourceType.INVEN_ICON => GetAssetResource("AB_INVEN_ICON_UNIT", cNKMSkinTemplet.m_InvenIconName), 
			eUnitResourceType.INVEN_ICON_GRAY => GetAssetResource("AB_INVEN_ICON_UNIT", cNKMSkinTemplet.m_InvenIconName + "_GRAY"), 
			eUnitResourceType.SPINE_ILLUST => GetAssetResource(cNKMSkinTemplet.m_SpineIllustName, cNKMSkinTemplet.m_SpineIllustName), 
			eUnitResourceType.SPINE_SD => GetAssetResource(cNKMSkinTemplet.m_SpineSDName, cNKMSkinTemplet.m_SpineSDName), 
			_ => null, 
		};
	}

	public static NKCAssetResourceData GetUnitResource(eUnitResourceType type, NKMUnitTempletBase cNKMUnitTempletBase)
	{
		if (cNKMUnitTempletBase == null)
		{
			return null;
		}
		return type switch
		{
			eUnitResourceType.FACE_CARD => GetAssetResource("AB_UNIT_FACE_CARD", cNKMUnitTempletBase.m_FaceCardName), 
			eUnitResourceType.INVEN_ICON => GetAssetResource("AB_INVEN_ICON_UNIT", cNKMUnitTempletBase.m_InvenIconName), 
			eUnitResourceType.INVEN_ICON_GRAY => GetAssetResource("AB_INVEN_ICON_UNIT", cNKMUnitTempletBase.m_InvenIconName + "_GRAY"), 
			eUnitResourceType.SPINE_ILLUST => GetAssetResource(cNKMUnitTempletBase.m_SpineIllustName, cNKMUnitTempletBase.m_SpineIllustName), 
			eUnitResourceType.SPINE_SD => GetAssetResource(cNKMUnitTempletBase.m_SpineSDName, cNKMUnitTempletBase.m_SpineSDName), 
			_ => null, 
		};
	}

	public static Sprite GetorLoadUnitSprite(eUnitResourceType type, NKMOperator cNKMOperator)
	{
		if (cNKMOperator == null)
		{
			return null;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMOperator.id);
		return GetorLoadUnitSprite(type, unitTempletBase);
	}

	public static Sprite GetorLoadUnitSprite(eUnitResourceType type, NKMUnitData cNKMUnitData)
	{
		if (cNKMUnitData == null)
		{
			return null;
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(cNKMUnitData);
		if (skinTemplet != null)
		{
			Sprite sprite = GetorLoadUnitSprite(type, skinTemplet);
			if (sprite != null)
			{
				return sprite;
			}
			Debug.LogError("Skin Sprite load failed. fallback to base sprite");
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData.m_UnitID);
		return GetorLoadUnitSprite(type, unitTempletBase);
	}

	public static Sprite GetorLoadUnitSprite(eUnitResourceType type, int unitID, int skinID)
	{
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
		if (skinTemplet != null && skinTemplet.m_SkinEquipUnitID == unitID)
		{
			Sprite sprite = GetorLoadUnitSprite(type, skinTemplet);
			if (sprite != null)
			{
				return sprite;
			}
			Debug.LogError("Skin Sprite load failed. fallback to base sprite");
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		return GetorLoadUnitSprite(type, unitTempletBase);
	}

	public static Sprite GetorLoadUnitSprite(eUnitResourceType type, NKMUnitTempletBase cNKMUnitTempletBase, int skinID)
	{
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
		if (skinTemplet != null && skinTemplet.m_SkinEquipUnitID == cNKMUnitTempletBase.m_UnitID)
		{
			Sprite sprite = GetorLoadUnitSprite(type, skinTemplet);
			if (sprite != null)
			{
				return sprite;
			}
			Debug.LogError("Skin Sprite load failed. fallback to base sprite");
		}
		return GetorLoadUnitSprite(type, cNKMUnitTempletBase);
	}

	public static Sprite GetorLoadUnitSprite(eUnitResourceType type, NKMSkinTemplet cNKMSkinTemplet)
	{
		if (cNKMSkinTemplet == null)
		{
			return null;
		}
		switch (type)
		{
		case eUnitResourceType.FACE_CARD:
			return GetOrLoadAssetResource<Sprite>("AB_UNIT_FACE_CARD", cNKMSkinTemplet.m_FaceCardName);
		case eUnitResourceType.INVEN_ICON:
			return GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", cNKMSkinTemplet.m_InvenIconName);
		case eUnitResourceType.INVEN_ICON_GRAY:
			return GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", cNKMSkinTemplet.m_InvenIconName + "_GRAY");
		case eUnitResourceType.SPINE_ILLUST:
		case eUnitResourceType.SPINE_SD:
			Debug.LogWarning("Wrong type");
			return null;
		default:
			return null;
		}
	}

	public static Sprite GetorLoadUnitSprite(eUnitResourceType type, NKMUnitTempletBase cNKMUnitTempletBase)
	{
		if (cNKMUnitTempletBase == null)
		{
			return null;
		}
		switch (type)
		{
		case eUnitResourceType.FACE_CARD:
			return GetOrLoadAssetResource<Sprite>("AB_UNIT_FACE_CARD", cNKMUnitTempletBase.m_FaceCardName);
		case eUnitResourceType.INVEN_ICON:
		{
			Sprite orLoadAssetResource2 = GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", cNKMUnitTempletBase.m_InvenIconName);
			if (orLoadAssetResource2 == null)
			{
				Debug.LogError($"UnitIconSprite {cNKMUnitTempletBase.m_InvenIconName}(From UnitStrID {cNKMUnitTempletBase.m_UnitStrID}) not found");
			}
			return orLoadAssetResource2;
		}
		case eUnitResourceType.INVEN_ICON_GRAY:
		{
			Sprite orLoadAssetResource = GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", cNKMUnitTempletBase.m_InvenIconName + "_GRAY");
			if (orLoadAssetResource == null)
			{
				Debug.LogError($"UnitIconSprite {cNKMUnitTempletBase.m_InvenIconName}(From UnitStrID {cNKMUnitTempletBase.m_UnitStrID}) not found");
			}
			return orLoadAssetResource;
		}
		case eUnitResourceType.SPINE_ILLUST:
		case eUnitResourceType.SPINE_SD:
			Debug.LogWarning("Wrong type");
			return null;
		default:
			return null;
		}
	}

	public static void PreloadUnitInvenIconEmpty()
	{
		LoadAssetResourceTemp<Sprite>("AB_INVEN_ICON_UNIT", "AB_INVEN_ICON_NKM_UNIT_EMPTY");
	}

	public static NKCAssetResourceData GetAssetResourceUnitInvenIconEmpty()
	{
		return GetAssetResource("AB_INVEN_ICON_UNIT", "AB_INVEN_ICON_NKM_UNIT_EMPTY");
	}

	public static Sprite GetShipRandomFaceCard()
	{
		return GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_deck_view_texture", "NKM_DECK_VIEW_SHIP_RANDOM");
	}

	public static Sprite GetReactorIcon(string assetName)
	{
		return GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_ITEM_EQUIP", assetName);
	}

	public static NKCASUIUnitIllust OpenSpineIllust(NKMUnitData cNKMUnitData, bool bAsync = false)
	{
		if (cNKMUnitData == null)
		{
			return null;
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(cNKMUnitData);
		if (skinTemplet != null)
		{
			NKCASUIUnitIllust nKCASUIUnitIllust = OpenSpineIllust(skinTemplet, bAsync);
			if (nKCASUIUnitIllust != null)
			{
				return nKCASUIUnitIllust;
			}
			Debug.LogError("Skin Spineillust load failed. fallback to base sprite");
		}
		return OpenSpineIllust(NKMUnitManager.GetUnitTempletBase(cNKMUnitData.m_UnitID), bAsync);
	}

	public static NKCASUIUnitIllust OpenSpineIllust(NKMSkinTemplet skinTemplet, bool bAsync = false)
	{
		if (skinTemplet == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(skinTemplet.m_SpineIllustName))
		{
			return null;
		}
		return OpenSpineIllust(skinTemplet.m_SpineIllustName, skinTemplet.m_SpineIllustName, bAsync);
	}

	public static NKCASUIUnitIllust OpenSpineIllust(NKMUnitTempletBase unitTempletBase, int skinID, bool bAsync = false)
	{
		if (unitTempletBase == null)
		{
			return null;
		}
		if (skinID == 0)
		{
			return OpenSpineIllust(unitTempletBase, bAsync);
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
		if (NKMSkinManager.IsSkinForCharacter(unitTempletBase.m_UnitID, skinTemplet))
		{
			return OpenSpineIllust(skinTemplet);
		}
		Debug.LogError("Skin Spineillust load failed, or not a skin for target unit. fallback to base sprite");
		return OpenSpineIllust(unitTempletBase, bAsync);
	}

	public static NKCASUIUnitIllust OpenSpineIllust(NKMUnitTempletBase unitTempletBase, bool bAsync = false)
	{
		if (unitTempletBase == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(unitTempletBase.m_SpineIllustName))
		{
			return null;
		}
		return OpenSpineIllust(unitTempletBase.m_SpineIllustName, unitTempletBase.m_SpineIllustName, bAsync);
	}

	public static void CloseSpineIllust(NKCASUIUnitIllust spineIllust)
	{
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(spineIllust);
	}

	public static NKCASUIUnitIllust OpenSpineIllustWithManualNaming(string unitStrID, bool bAsync = false)
	{
		return OpenSpineIllust("AB_UNIT_ILLUST_" + unitStrID, "AB_UNIT_ILLUST_" + unitStrID, bAsync);
	}

	public static NKCASUIUnitIllust OpenSpineIllust(string bundleName, string assetName, bool bAsync = false)
	{
		return (NKCASUISpineIllust)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUISpineIllust, bundleName, assetName, bAsync);
	}

	public static NKCASUIUnitIllust OpenSpineSD(NKMUnitData cNKMUnitData, bool bAsync = false)
	{
		if (cNKMUnitData == null)
		{
			return null;
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(cNKMUnitData);
		if (skinTemplet != null)
		{
			NKCASUIUnitIllust nKCASUIUnitIllust = OpenSpineSD(skinTemplet, bAsync);
			if (nKCASUIUnitIllust != null)
			{
				return nKCASUIUnitIllust;
			}
			Debug.LogError("Skin SD load failed. fallback to base sprite");
		}
		return OpenSpineSD(NKMUnitManager.GetUnitTempletBase(cNKMUnitData.m_UnitID), bAsync);
	}

	public static NKCASUIUnitIllust OpenSpineSD(int unitID, int skinID, bool bAsync = false)
	{
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID, unitID);
		if (skinTemplet != null)
		{
			NKCASUIUnitIllust nKCASUIUnitIllust = OpenSpineSD(skinTemplet, bAsync);
			if (nKCASUIUnitIllust != null)
			{
				return nKCASUIUnitIllust;
			}
			Debug.LogError("Skin SD load failed. fallback to base sprite");
		}
		return OpenSpineSD(NKMUnitManager.GetUnitTempletBase(unitID), bAsync);
	}

	public static NKCASUIUnitIllust OpenSpineSD(NKMSkinTemplet skinTemplet, bool bAsync = false)
	{
		if (skinTemplet == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(skinTemplet.m_SpineSDName))
		{
			return null;
		}
		return (NKCASUISpineIllust)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUISpineIllust, skinTemplet.m_SpineSDName, skinTemplet.m_SpineSDName, bAsync);
	}

	public static NKCASUIUnitIllust OpenSpineSD(NKMUnitTempletBase unitTempletBase, bool bAsync = false)
	{
		if (unitTempletBase == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(unitTempletBase.m_SpineSDName))
		{
			return null;
		}
		return (NKCASUISpineIllust)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUISpineIllust, unitTempletBase.m_SpineSDName, unitTempletBase.m_SpineSDName, bAsync);
	}

	public static NKCASUISpineIllust OpenSpineSD(NKMWorldMapEventTemplet cNKMWorldMapEventTemplet, bool bAsync = false)
	{
		if (cNKMWorldMapEventTemplet == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(cNKMWorldMapEventTemplet.spineSDName))
		{
			return null;
		}
		return (NKCASUISpineIllust)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUISpineIllust, cNKMWorldMapEventTemplet.spineSDName, cNKMWorldMapEventTemplet.spineSDName, bAsync);
	}

	public static NKCASUISpineIllust OpenSpineSD(string spineSDName, bool bAsync = false)
	{
		if (string.IsNullOrEmpty(spineSDName))
		{
			return null;
		}
		return (NKCASUISpineIllust)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUISpineIllust, spineSDName, spineSDName, bAsync);
	}

	public static NKCASUISpineIllust OpenSpineSD(string bundleName, string spineSDName, bool bAsync = false)
	{
		if (string.IsNullOrEmpty(spineSDName))
		{
			return null;
		}
		return (NKCASUISpineIllust)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUISpineIllust, bundleName, spineSDName, bAsync);
	}

	public static Sprite GetOrLoadMinimapFaceIcon(NKMUnitTempletBase cNKMUnitTempletBase, bool bSub = false)
	{
		if (cNKMUnitTempletBase == null)
		{
			return null;
		}
		if (bSub && !string.IsNullOrEmpty(cNKMUnitTempletBase.m_MiniMapFaceNameSub))
		{
			return GetOrLoadMinimapFaceIcon(cNKMUnitTempletBase.m_MiniMapFaceNameSub);
		}
		return GetOrLoadMinimapFaceIcon(cNKMUnitTempletBase.m_MiniMapFaceName);
	}

	public static Sprite GetOrLoadMinimapFaceIcon(string minimapFaceName)
	{
		return GetOrLoadAssetResource<Sprite>("AB_UNIT_MINI_MAP_FACE", minimapFaceName);
	}

	public static string GetMiscItemIconBundleName(NKMItemMiscTemplet itemMiscTemplet)
	{
		if (itemMiscTemplet == null)
		{
			return "AB_INVEN_ICON_ITEM_MISC";
		}
		switch (itemMiscTemplet.m_ItemMiscType)
		{
		case NKM_ITEM_MISC_TYPE.IMT_EMBLEM:
		case NKM_ITEM_MISC_TYPE.IMT_EMBLEM_RANK:
			return "AB_INVEN_ICON_EMBLEM";
		case NKM_ITEM_MISC_TYPE.IMT_PIECE:
			return "AB_INVEN_ICON_UNIT_PIECE";
		case NKM_ITEM_MISC_TYPE.IMT_BACKGROUND:
			return "AB_INVEN_ICON_BG";
		case NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME:
			return "AB_INVEN_ICON_BORDER";
		case NKM_ITEM_MISC_TYPE.IMT_INTERIOR:
			return "AB_INVEN_ICON_FNC";
		default:
			return "AB_INVEN_ICON_ITEM_MISC";
		}
	}

	public static void PreloadMiscItemIcon(NKMItemMiscTemplet itemMiscTemplet, bool bAsync = true)
	{
		if (itemMiscTemplet != null)
		{
			LoadAssetResourceTemp<Sprite>(NKMAssetName.ParseBundleName(GetMiscItemIconBundleName(itemMiscTemplet), itemMiscTemplet.m_ItemMiscIconName, "AB_INVEN_"));
		}
	}

	public static NKCAssetResourceData GetAssetResourceMiscItemIcon(NKMItemMiscTemplet itemMiscTemplet)
	{
		if (itemMiscTemplet == null)
		{
			return null;
		}
		return GetAssetResource(NKMAssetName.ParseBundleName(GetMiscItemIconBundleName(itemMiscTemplet), itemMiscTemplet.m_ItemMiscIconName, "AB_INVEN_"));
	}

	public static Sprite GetOrLoadMiscItemIcon(NKMItemMiscTemplet itemMiscTemplet)
	{
		if (itemMiscTemplet == null)
		{
			return null;
		}
		return GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName(GetMiscItemIconBundleName(itemMiscTemplet), itemMiscTemplet.m_ItemMiscIconName, "AB_INVEN_"));
	}

	public static Sprite GetOrLoadMiscItemIcon(int miscItemID)
	{
		return GetOrLoadMiscItemIcon(NKMItemManager.GetItemMiscTempletByID(miscItemID));
	}

	public static void PreloadMiscItemSmallIcon(NKMItemMiscTemplet itemMiscTemplet, bool bAsync = true)
	{
		if (itemMiscTemplet != null)
		{
			LoadAssetResourceTemp<Sprite>(NKMAssetName.ParseBundleName("AB_INVEN_ICON_ITEM_MISC_SMALL", itemMiscTemplet.m_ItemMiscIconName, "AB_INVEN_"));
		}
	}

	public static NKCAssetResourceData GetAssetResourceMiscItemSmallIcon(NKMItemMiscTemplet itemMiscTemplet)
	{
		if (itemMiscTemplet == null)
		{
			return null;
		}
		return GetAssetResource(NKMAssetName.ParseBundleName("AB_INVEN_ICON_ITEM_MISC_SMALL", itemMiscTemplet.m_ItemMiscIconName, "AB_INVEN_"));
	}

	public static Sprite GetOrLoadMiscItemSmallIcon(NKMItemMiscTemplet itemMiscTemplet)
	{
		if (itemMiscTemplet == null)
		{
			return null;
		}
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName("AB_INVEN_ICON_ITEM_MISC_SMALL", itemMiscTemplet.m_ItemMiscIconName, "AB_INVEN_");
		if (!NKCAssetResourceManager.IsAssetExists(nKMAssetName.m_BundleName, nKMAssetName.m_AssetName, loadUnloadedBundle: true))
		{
			Debug.LogWarning($"ItemID {itemMiscTemplet.m_ItemMiscID} : Small icon does not exist. defaulting to normal icon");
			return GetOrLoadMiscItemIcon(itemMiscTemplet);
		}
		Sprite orLoadAssetResource = GetOrLoadAssetResource<Sprite>(nKMAssetName);
		if (orLoadAssetResource == null)
		{
			Debug.LogWarning($"ItemID {itemMiscTemplet.m_ItemMiscID} : Small icon does not exist. defaulting to normal icon");
			return GetOrLoadMiscItemIcon(itemMiscTemplet);
		}
		return orLoadAssetResource;
	}

	public static Sprite GetOrLoadMiscItemSmallIcon(int miscItemID)
	{
		return GetOrLoadMiscItemSmallIcon(NKMItemManager.GetItemMiscTempletByID(miscItemID));
	}

	public static void PreloadEquipIcon(NKMEquipTemplet equipTemplet, bool bAsync = true)
	{
		if (equipTemplet != null)
		{
			LoadAssetResourceTemp<Sprite>("AB_INVEN_ICON_ITEM_EQUIP", equipTemplet.m_ItemEquipIconName);
		}
	}

	public static NKCAssetResourceData GetAssetResourceEquipIcon(NKMEquipTemplet equipTemplet)
	{
		if (equipTemplet == null)
		{
			return null;
		}
		return GetAssetResource("AB_INVEN_ICON_ITEM_EQUIP", equipTemplet.m_ItemEquipIconName);
	}

	public static Sprite GetOrLoadEquipIcon(NKMEquipTemplet equipTemplet)
	{
		if (equipTemplet == null)
		{
			return null;
		}
		return GetOrLoadEquipIcon(equipTemplet.m_ItemEquipIconName);
	}

	public static Sprite GetOrLoadEquipIcon(string equipIconName)
	{
		return GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_ITEM_EQUIP", "AB_INVEN_ICON_IQI_EQUIP_" + equipIconName);
	}

	public static void PreloadUnitRoleIcon(NKM_UNIT_ROLE_TYPE roleType, bool bAwaken, bool bSmall = false, bool bAsync = true)
	{
		LoadAssetResourceTemp<Sprite>("ab_ui_nkm_ui_common_icon", GetUnitRoleIconAssetName(roleType, bAwaken, bSmall), bAsync);
	}

	public static NKCAssetResourceData GetAssetResourceUnitRoleIcon(NKM_UNIT_ROLE_TYPE roleType, bool bAwaken, bool bSmall = false)
	{
		return GetAssetResource("ab_ui_nkm_ui_common_icon", GetUnitRoleIconAssetName(roleType, bAwaken, bSmall));
	}

	public static Sprite GetOrLoadUnitTypeIcon(NKMUnitTempletBase templetBase, bool bSmall = false)
	{
		if (templetBase == null)
		{
			return null;
		}
		if (templetBase.IsTrophy)
		{
			return null;
		}
		if (templetBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_INVALID && templetBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_INVALID)
		{
			return GetOrLoadUnitStyleIcon(templetBase.m_NKM_UNIT_STYLE_TYPE, bSmall);
		}
		return GetOrLoadUnitRoleIcon(templetBase, bSmall);
	}

	public static Sprite GetOrLoadUnitRoleIcon(NKMUnitTempletBase templetBase, bool bSmall = false)
	{
		if (templetBase == null)
		{
			return null;
		}
		if (templetBase.IsTrophy)
		{
			return null;
		}
		return GetOrLoadUnitRoleIcon(templetBase.m_NKM_UNIT_ROLE_TYPE, templetBase.m_bAwaken, bSmall);
	}

	public static Sprite GetOrLoadUnitRoleIcon(NKM_UNIT_ROLE_TYPE roleType, bool bAwaken, bool bSmall = false)
	{
		return GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_common_icon", GetUnitRoleIconAssetName(roleType, bAwaken, bSmall));
	}

	private static string GetUnitRoleIconAssetName(NKM_UNIT_ROLE_TYPE roleType, bool bAwaken, bool bSmall)
	{
		if (bAwaken)
		{
			if (bSmall)
			{
				return roleType switch
				{
					NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "NKM_UI_COMMON_UNIT_CLASS_ICON_STRIKER_AWAKEN_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_RANGER => "NKM_UI_COMMON_UNIT_CLASS_ICON_RANGER_AWAKEN_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "NKM_UI_COMMON_UNIT_CLASS_ICON_DEFENDER_AWAKEN_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "NKM_UI_COMMON_UNIT_CLASS_ICON_SNIPER_AWAKEN_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "NKM_UI_COMMON_UNIT_CLASS_ICON_SUPPORTER_AWAKEN_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "NKM_UI_COMMON_UNIT_CLASS_ICON_SIEGE_AWAKEN_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_TOWER => "NKM_UI_COMMON_UNIT_CLASS_ICON_TOWER_AWAKEN_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_INVALID => "NKM_UI_COMMON_UNIT_CLASS_ICON_NONE_SMALL", 
					_ => "", 
				};
			}
			return roleType switch
			{
				NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "NKM_UI_COMMON_UNIT_CLASS_ICON_STRIKER_AWAKEN", 
				NKM_UNIT_ROLE_TYPE.NURT_RANGER => "NKM_UI_COMMON_UNIT_CLASS_ICON_RANGER_AWAKEN", 
				NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "NKM_UI_COMMON_UNIT_CLASS_ICON_DEFENDER_AWAKEN", 
				NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "NKM_UI_COMMON_UNIT_CLASS_ICON_SNIPER_AWAKEN", 
				NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "NKM_UI_COMMON_UNIT_CLASS_ICON_SUPPORTER_AWAKEN", 
				NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "NKM_UI_COMMON_UNIT_CLASS_ICON_SIEGE_AWAKEN", 
				NKM_UNIT_ROLE_TYPE.NURT_TOWER => "NKM_UI_COMMON_UNIT_CLASS_ICON_TOWER_AWAKEN", 
				NKM_UNIT_ROLE_TYPE.NURT_INVALID => "NKM_UI_COMMON_UNIT_CLASS_ICON_NONE_SMALL", 
				_ => "", 
			};
		}
		if (bSmall)
		{
			return roleType switch
			{
				NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "NKM_UI_COMMON_UNIT_CLASS_ICON_STRIKER_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_RANGER => "NKM_UI_COMMON_UNIT_CLASS_ICON_RANGER_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "NKM_UI_COMMON_UNIT_CLASS_ICON_DEFENDER_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "NKM_UI_COMMON_UNIT_CLASS_ICON_SNIPER_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "NKM_UI_COMMON_UNIT_CLASS_ICON_SUPPORTER_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "NKM_UI_COMMON_UNIT_CLASS_ICON_SIEGE_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_TOWER => "NKM_UI_COMMON_UNIT_CLASS_ICON_TOWER_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_INVALID => "NKM_UI_COMMON_UNIT_CLASS_ICON_NONE_SMALL", 
				_ => "", 
			};
		}
		return roleType switch
		{
			NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "NKM_UI_COMMON_UNIT_CLASS_ICON_STRIKER", 
			NKM_UNIT_ROLE_TYPE.NURT_RANGER => "NKM_UI_COMMON_UNIT_CLASS_ICON_RANGER", 
			NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "NKM_UI_COMMON_UNIT_CLASS_ICON_DEFENDER", 
			NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "NKM_UI_COMMON_UNIT_CLASS_ICON_SNIPER", 
			NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "NKM_UI_COMMON_UNIT_CLASS_ICON_SUPPORTER", 
			NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "NKM_UI_COMMON_UNIT_CLASS_ICON_SIEGE", 
			NKM_UNIT_ROLE_TYPE.NURT_TOWER => "NKM_UI_COMMON_UNIT_CLASS_ICON_TOWER", 
			NKM_UNIT_ROLE_TYPE.NURT_INVALID => "NKM_UI_COMMON_UNIT_CLASS_ICON_NONE_SMALL", 
			_ => "", 
		};
	}

	public static Sprite GetOrLoadUnitRoleIconInGame(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase == null)
		{
			return null;
		}
		return GetOrLoadUnitRoleIconInGame(unitTempletBase.m_NKM_UNIT_ROLE_TYPE, unitTempletBase.m_bAwaken);
	}

	public static Sprite GetOrLoadUnitRoleIconInGame(NKM_UNIT_ROLE_TYPE roleType, bool bAwaken)
	{
		return GetOrLoadAssetResource<Sprite>("AB_UNIT_GAME_NKM_UNIT_SPRITE", GetUnitRoleIconForInGameAssetName(roleType, bAwaken));
	}

	private static string GetUnitRoleIconForInGameAssetName(NKM_UNIT_ROLE_TYPE roleType, bool bAwaken)
	{
		if (bAwaken)
		{
			return roleType switch
			{
				NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_STRIKER_AWAKEN_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_RANGER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_RANGER_AWAKEN_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_DEFENDER_AWAKEN_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_SNIPER_AWAKEN_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_SUPPORTER_AWAKEN_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_SIEGE_AWAKEN_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_TOWER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_TOWER_AWAKEN_SMALL", 
				NKM_UNIT_ROLE_TYPE.NURT_INVALID => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_NONE_SMALL", 
				_ => "", 
			};
		}
		return roleType switch
		{
			NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_STRIKER_SMALL", 
			NKM_UNIT_ROLE_TYPE.NURT_RANGER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_RANGER_SMALL", 
			NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_DEFENDER_SMALL", 
			NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_SNIPER_SMALL", 
			NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_SUPPORTER_SMALL", 
			NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_SIEGE_SMALL", 
			NKM_UNIT_ROLE_TYPE.NURT_TOWER => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_TOWER_SMALL", 
			NKM_UNIT_ROLE_TYPE.NURT_INVALID => "AB_UNIT_GAME_COMMON_UNIT_CLASS_ICON_NONE_SMALL", 
			_ => "", 
		};
	}

	public static void PreloadUnitAttackTypeIcon(NKM_FIND_TARGET_TYPE attackType, bool bSmall = false, bool bAsync = true)
	{
		LoadAssetResourceTemp<Sprite>("ab_ui_nkm_ui_common_icon", GetUnitAttackTypeIconAssetName(attackType, bSmall), bAsync);
	}

	public static NKCAssetResourceData GetAssetResourceUnitAttackTypeIcon(NKM_FIND_TARGET_TYPE attackType, bool bSmall = false)
	{
		return GetAssetResource("ab_ui_nkm_ui_common_icon", GetUnitAttackTypeIconAssetName(attackType, bSmall));
	}

	public static Sprite GetOrLoadUnitAttackTypeIcon(NKMUnitTempletBase unitTempletBase, bool bSmall = false)
	{
		if (unitTempletBase == null)
		{
			return null;
		}
		if (unitTempletBase.IsTrophy)
		{
			return null;
		}
		if (unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc == NKM_FIND_TARGET_TYPE.NFTT_INVALID)
		{
			return GetOrLoadUnitAttackTypeIcon(unitTempletBase.m_NKM_FIND_TARGET_TYPE, bSmall);
		}
		return GetOrLoadUnitAttackTypeIcon(unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc, bSmall);
	}

	public static Sprite GetOrLoadUnitAttackTypeIcon(NKMUnitData unitData, bool bSmall = false)
	{
		if (unitData == null)
		{
			return null;
		}
		return GetOrLoadUnitAttackTypeIcon(NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID), bSmall);
	}

	public static Sprite GetOrLoadUnitAttackTypeIcon(NKM_FIND_TARGET_TYPE attackType, bool bSmall = false)
	{
		return GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_common_icon", GetUnitAttackTypeIconAssetName(attackType, bSmall));
	}

	private static string GetUnitAttackTypeIconAssetName(NKM_FIND_TARGET_TYPE attackType, bool bSmall)
	{
		if (bSmall)
		{
			switch (attackType)
			{
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR_BOSS_LAST:
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_AIR:
				return "UI_COMMON_UNIT_ATTACK_TYPE_AIR_SMALL";
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND_BOSS_LAST:
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_LAND:
				return "UI_COMMON_UNIT_ATTACK_TYPE_LAND_SMALL";
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR_FIRST:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_BOSS_LAST:
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM:
				return "UI_COMMON_UNIT_ATTACK_TYPE_ALL_SMALL";
			case NKM_FIND_TARGET_TYPE.NFTT_NO:
				return "NKM_UI_COMMON_UNIT_BATTLE_TYPE_ICON_NONE_SMALL";
			default:
				return "";
			}
		}
		switch (attackType)
		{
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_AIR:
			return "UI_COMMON_UNIT_ATTACK_TYPE_AIR";
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_LAND:
			return "UI_COMMON_UNIT_ATTACK_TYPE_LAND";
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM:
			return "UI_COMMON_UNIT_ATTACK_TYPE_ALL";
		case NKM_FIND_TARGET_TYPE.NFTT_NO:
			return "NKM_UI_COMMON_UNIT_BATTLE_TYPE_ICON_NONE_SMALL";
		default:
			return "";
		}
	}

	public static void PreloadUnitRoldAttackTypeIcon(NKM_UNIT_ROLE_TYPE roleType, NKM_FIND_TARGET_TYPE attackType, bool bSmall = false, bool bAsync = true)
	{
		LoadAssetResourceTemp<Sprite>("ab_ui_nkm_ui_common_icon", GetUnitRoleAttackTypeIconAssetName(roleType, attackType, bSmall), bAsync);
	}

	public static NKCAssetResourceData GetAssetResourceUnitRoleAttackTypeIcon(NKM_UNIT_ROLE_TYPE roleType, NKM_FIND_TARGET_TYPE attackType, bool bSmall = false)
	{
		return GetAssetResource("ab_ui_nkm_ui_common_icon", GetUnitRoleAttackTypeIconAssetName(roleType, attackType, bSmall));
	}

	public static Sprite GetOrLoadUnitRoleAttackTypeIcon(NKMUnitTempletBase unitTempletBase, bool bSmall = false)
	{
		if (unitTempletBase == null)
		{
			return null;
		}
		if (unitTempletBase.IsTrophy)
		{
			return null;
		}
		if (unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc == NKM_FIND_TARGET_TYPE.NFTT_INVALID)
		{
			return GetOrLoadUnitRoleAttackTypeIcon(unitTempletBase.m_NKM_UNIT_ROLE_TYPE, unitTempletBase.m_NKM_FIND_TARGET_TYPE, bSmall);
		}
		return GetOrLoadUnitRoleAttackTypeIcon(unitTempletBase.m_NKM_UNIT_ROLE_TYPE, unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc, bSmall);
	}

	public static Sprite GetOrLoadUnitRoleAttackTypeIcon(NKMUnitData unitData, bool bSmall = false)
	{
		if (unitData == null)
		{
			return null;
		}
		return GetOrLoadUnitRoleAttackTypeIcon(NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID), bSmall);
	}

	public static Sprite GetOrLoadUnitRoleAttackTypeIcon(NKM_UNIT_ROLE_TYPE roleType, NKM_FIND_TARGET_TYPE attackType, bool bSmall = false)
	{
		return GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_common_icon", GetUnitRoleAttackTypeIconAssetName(roleType, attackType, bSmall));
	}

	public static string GetUnitRoleAttackTypeIconAssetName(NKM_UNIT_ROLE_TYPE roleType, NKM_FIND_TARGET_TYPE attackType, bool bSmall)
	{
		if (bSmall)
		{
			switch (attackType)
			{
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR_BOSS_LAST:
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_AIR:
				return roleType switch
				{
					NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "UI_COMMON_UNIT_CLASS_STRIKER_AIR_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_RANGER => "UI_COMMON_UNIT_CLASS_RANGER_AIR_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "UI_COMMON_UNIT_CLASS_DEFENDER_AIR_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "UI_COMMON_UNIT_CLASS_SNIPER_AIR_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "UI_COMMON_UNIT_CLASS_SUPPORTER_AIR_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "UI_COMMON_UNIT_CLASS_SIEGE_AIR_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_TOWER => "UI_COMMON_UNIT_CLASS_TOWER_AIR_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_INVALID => "UI_COMMON_UNIT_ATTACK_TYPE_AIR_SMALL", 
					_ => "", 
				};
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND_BOSS_LAST:
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_LAND:
				return roleType switch
				{
					NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "UI_COMMON_UNIT_CLASS_STRIKER_LAND_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_RANGER => "UI_COMMON_UNIT_CLASS_RANGER_LAND_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "UI_COMMON_UNIT_CLASS_DEFENDER_LAND_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "UI_COMMON_UNIT_CLASS_SNIPER_LAND_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "UI_COMMON_UNIT_CLASS_SUPPORTER_LAND_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "UI_COMMON_UNIT_CLASS_SIEGE_LAND_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_TOWER => "UI_COMMON_UNIT_CLASS_TOWER_LAND_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_INVALID => "UI_COMMON_UNIT_ATTACK_TYPE_LAND_SMALL", 
					_ => "", 
				};
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR_FIRST:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_BOSS_LAST:
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM:
				return roleType switch
				{
					NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "UI_COMMON_UNIT_CLASS_STRIKER_ALL_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_RANGER => "UI_COMMON_UNIT_CLASS_RANGER_ALL_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "UI_COMMON_UNIT_CLASS_DEFENDER_ALL_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "UI_COMMON_UNIT_CLASS_SNIPER_ALL_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "UI_COMMON_UNIT_CLASS_SUPPORTER_ALL_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "UI_COMMON_UNIT_CLASS_SIEGE_ALL_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_TOWER => "UI_COMMON_UNIT_CLASS_TOWER_ALL_SMALL", 
					NKM_UNIT_ROLE_TYPE.NURT_INVALID => "UI_COMMON_UNIT_ATTACK_TYPE_ALL_SMALL", 
					_ => "", 
				};
			case NKM_FIND_TARGET_TYPE.NFTT_NO:
				return "NKM_UI_COMMON_UNIT_BATTLE_TYPE_ICON_NONE_SMALL";
			default:
				return "";
			}
		}
		switch (attackType)
		{
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_AIR:
			return roleType switch
			{
				NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "UI_COMMON_UNIT_CLASS_STRIKER_AIR", 
				NKM_UNIT_ROLE_TYPE.NURT_RANGER => "UI_COMMON_UNIT_CLASS_RANGER_AIR", 
				NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "UI_COMMON_UNIT_CLASS_DEFENDER_AIR", 
				NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "UI_COMMON_UNIT_CLASS_SNIPER_AIR", 
				NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "UI_COMMON_UNIT_CLASS_SUPPORTER_AIR", 
				NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "UI_COMMON_UNIT_CLASS_SIEGE_AIR", 
				NKM_UNIT_ROLE_TYPE.NURT_TOWER => "UI_COMMON_UNIT_CLASS_TOWER_AIR", 
				NKM_UNIT_ROLE_TYPE.NURT_INVALID => "UI_COMMON_UNIT_ATTACK_TYPE_AIR", 
				_ => "", 
			};
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_LAND:
			return roleType switch
			{
				NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "UI_COMMON_UNIT_CLASS_STRIKER_LAND", 
				NKM_UNIT_ROLE_TYPE.NURT_RANGER => "UI_COMMON_UNIT_CLASS_RANGER_LAND", 
				NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "UI_COMMON_UNIT_CLASS_DEFENDER_LAND", 
				NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "UI_COMMON_UNIT_CLASS_SNIPER_LAND", 
				NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "UI_COMMON_UNIT_CLASS_SUPPORTER_LAND", 
				NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "UI_COMMON_UNIT_CLASS_SIEGE_LAND", 
				NKM_UNIT_ROLE_TYPE.NURT_TOWER => "UI_COMMON_UNIT_CLASS_TOWER_LAND", 
				NKM_UNIT_ROLE_TYPE.NURT_INVALID => "UI_COMMON_UNIT_ATTACK_TYPE_LAND", 
				_ => "", 
			};
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM:
			return roleType switch
			{
				NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "UI_COMMON_UNIT_CLASS_STRIKER_ALL", 
				NKM_UNIT_ROLE_TYPE.NURT_RANGER => "UI_COMMON_UNIT_CLASS_RANGER_ALL", 
				NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "UI_COMMON_UNIT_CLASS_DEFENDER_ALL", 
				NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "UI_COMMON_UNIT_CLASS_SNIPER_ALL", 
				NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "UI_COMMON_UNIT_CLASS_SUPPORTER_ALL", 
				NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "UI_COMMON_UNIT_CLASS_SIEGE_ALL", 
				NKM_UNIT_ROLE_TYPE.NURT_TOWER => "UI_COMMON_UNIT_CLASS_TOWER_ALL", 
				NKM_UNIT_ROLE_TYPE.NURT_INVALID => "UI_COMMON_UNIT_ATTACK_TYPE_ALL", 
				_ => "", 
			};
		case NKM_FIND_TARGET_TYPE.NFTT_NO:
			return "NKM_UI_COMMON_UNIT_BATTLE_TYPE_ICON_NONE_SMALL";
		default:
			return "";
		}
	}

	public static Sprite GetOrLoadUnitSourceTypeIcon(NKMUnitTempletBase unitTempletBase, bool bSmall = false)
	{
		if (unitTempletBase == null)
		{
			return null;
		}
		if (unitTempletBase.IsTrophy)
		{
			return null;
		}
		if (unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
		{
			return GetOrLoadUnitSourceTypeIcon(unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB, bSmall);
		}
		return GetOrLoadUnitSourceTypeIcon(unitTempletBase.m_NKM_UNIT_SOURCE_TYPE, bSmall);
	}

	public static Sprite GetOrLoadUnitSourceTypeIcon(NKMUnitData unitData, bool bSmall = false)
	{
		if (unitData == null)
		{
			return null;
		}
		return GetOrLoadUnitSourceTypeIcon(NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID), bSmall);
	}

	public static Sprite GetOrLoadUnitSourceTypeIcon(NKM_UNIT_SOURCE_TYPE sourceType, bool bSmall = false)
	{
		if (NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE"))
		{
			return GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_common_icon", GetUnitSourceTypeIconAssetName(sourceType, bSmall));
		}
		return null;
	}

	public static string GetUnitSourceTypeIconAssetName(NKM_UNIT_SOURCE_TYPE sourceType, bool bSmall = false)
	{
		if (bSmall)
		{
			return sourceType switch
			{
				NKM_UNIT_SOURCE_TYPE.NUST_CONFLICT => "NKM_UI_COMMON_WEAK_TAG_SYMBOL_CONFLICT_SMALL", 
				NKM_UNIT_SOURCE_TYPE.NUST_STABLE => "NKM_UI_COMMON_WEAK_TAG_SYMBOL_STABLE_SMALL", 
				NKM_UNIT_SOURCE_TYPE.NUST_LIBERAL => "NKM_UI_COMMON_WEAK_TAG_SYMBOL_LIBERAL_SMALL", 
				_ => "", 
			};
		}
		return sourceType switch
		{
			NKM_UNIT_SOURCE_TYPE.NUST_CONFLICT => "NKM_UI_COMMON_WEAK_TAG_SYMBOL_CONFLICT", 
			NKM_UNIT_SOURCE_TYPE.NUST_STABLE => "NKM_UI_COMMON_WEAK_TAG_SYMBOL_STABLE", 
			NKM_UNIT_SOURCE_TYPE.NUST_LIBERAL => "NKM_UI_COMMON_WEAK_TAG_SYMBOL_LIBERAL", 
			_ => "", 
		};
	}

	public static Sprite GetOrLoadDiveArtifactIcon(NKMDiveArtifactTemplet cNKMDiveArtifactTemplet)
	{
		if (cNKMDiveArtifactTemplet == null)
		{
			return null;
		}
		return GetOrLoadDiveArtifactIcon(cNKMDiveArtifactTemplet.ArtifactMiscIconName);
	}

	public static Sprite GetOrLoadDiveArtifactIcon(string artifactMiscIconName)
	{
		return GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_ARTIFACT", artifactMiscIconName);
	}

	public static Sprite GetOrLoadDiveArtifactIconBig(NKMDiveArtifactTemplet cNKMDiveArtifactTemplet)
	{
		if (cNKMDiveArtifactTemplet == null)
		{
			return null;
		}
		return GetOrLoadDiveArtifactIconBig(cNKMDiveArtifactTemplet.ArtifactMiscIconName);
	}

	public static Sprite GetOrLoadDiveArtifactIconBig(string artifactMiscIconName)
	{
		return GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_ARTIFACT_BIG", artifactMiscIconName);
	}

	public static Sprite GetOrLoadGuildArtifactIcon(GuildDungeonArtifactTemplet cGuildDungeonArtifactTemplet)
	{
		if (cGuildDungeonArtifactTemplet == null)
		{
			return null;
		}
		return GetOrLoadGuildArtifactIcon(cGuildDungeonArtifactTemplet.GetIconName());
	}

	public static Sprite GetOrLoadGuildArtifactIcon(string iconName)
	{
		return GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_ARTIFACT", iconName);
	}

	public static Sprite GetOrLoadGuildArtifactIconBig(GuildDungeonArtifactTemplet cGuildDungeonArtifactTemplet)
	{
		if (cGuildDungeonArtifactTemplet == null)
		{
			return null;
		}
		return GetOrLoadGuildArtifactIconBig(cGuildDungeonArtifactTemplet.GetIconName());
	}

	public static Sprite GetOrLoadGuildArtifactIconBig(string iconName)
	{
		return GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_ARTIFACT_BIG", iconName);
	}

	public static Sprite GetOrLoadEmoticonIcon(NKMEmoticonTemplet cNKMEmoticonTemplet)
	{
		if (cNKMEmoticonTemplet == null)
		{
			return null;
		}
		return GetOrLoadEmoticonIcon(cNKMEmoticonTemplet.m_EmoticonaIconName);
	}

	public static Sprite GetOrLoadEmoticonIcon(string iconName)
	{
		return GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EMOTICON_ICON", "AB_UI_NKM_UI_EMOTICON_ICON_" + iconName);
	}

	public static Sprite GetOrLoadMoldIcon(NKMItemMoldTemplet moldTemplet)
	{
		if (moldTemplet == null)
		{
			return null;
		}
		return GetOrLoadMoldIcon(moldTemplet.m_MoldIconName);
	}

	public static Sprite GetOrLoadMoldIcon(string moldIconName)
	{
		return GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_ITEM_MISC", "AB_INVEN_" + moldIconName);
	}

	public static Sprite GetOrLoadBuffIconForItemPopup(NKMCompanyBuffTemplet buffTemplet)
	{
		if (buffTemplet == null)
		{
			return null;
		}
		return GetOrLoadBuffIconForItemPopup(buffTemplet.m_CompanyBuffItemIcon);
	}

	public static Sprite GetOrLoadBuffIconForItemPopup(string buffIconName)
	{
		if (!string.IsNullOrEmpty(buffIconName))
		{
			return GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_ITEM_MISC", "AB_INVEN_" + buffIconName);
		}
		return GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_ITEM_MISC", "AB_INVEN_ICON_IMI_MISC_BUFF_BASIC_CREDIT");
	}

	public static void PreloadUnitStyleIcon(NKM_UNIT_STYLE_TYPE StyleType, bool bSmall = false, bool bAsync = true)
	{
		if (StyleType != NKM_UNIT_STYLE_TYPE.NUST_INVALID)
		{
			LoadAssetResourceTemp<Sprite>("ab_ui_nkm_ui_common_icon", GetUnitStyleIconAssetName(StyleType, bSmall), bAsync);
		}
	}

	public static NKCAssetResourceData GetAssetResourceUnitStyleIcon(NKM_UNIT_STYLE_TYPE StyleType, bool bSmall = false)
	{
		if (StyleType == NKM_UNIT_STYLE_TYPE.NUST_INVALID)
		{
			return null;
		}
		return GetAssetResource("ab_ui_nkm_ui_common_icon", GetUnitStyleIconAssetName(StyleType, bSmall));
	}

	public static Sprite GetOrLoadUnitStyleIcon(NKM_UNIT_STYLE_TYPE StyleType, bool bSmall = false)
	{
		if (StyleType == NKM_UNIT_STYLE_TYPE.NUST_INVALID)
		{
			return null;
		}
		return GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_common_icon", GetUnitStyleIconAssetName(StyleType, bSmall));
	}

	private static string GetUnitStyleIconAssetName(NKM_UNIT_STYLE_TYPE styleType, bool bSmall)
	{
		if (bSmall)
		{
			switch (styleType)
			{
			case NKM_UNIT_STYLE_TYPE.NUST_COUNTER:
				return "NKM_UI_COMMON_UNIT_TYPE_COUNTER_SMALL";
			case NKM_UNIT_STYLE_TYPE.NUST_MECHANIC:
				return "NKM_UI_COMMON_UNIT_TYPE_MECHANIC_SMALL";
			case NKM_UNIT_STYLE_TYPE.NUST_SOLDIER:
				return "NKM_UI_COMMON_UNIT_TYPE_SOLDIER_SMALL";
			case NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT:
				return "NKM_UI_COMMON_SHIP_TYPE_ASSAULT_SMALL";
			case NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER:
				return "NKM_UI_COMMON_SHIP_TYPE_CRUISER_SMALL";
			case NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY:
				return "NKM_UI_COMMON_SHIP_TYPE_HEAVY_SMALL";
			case NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL:
				return "NKM_UI_COMMON_SHIP_TYPE_SPECIAL_SMALL";
			case NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL:
				return "NKM_UI_COMMON_SHIP_TYPE_PATROL_VEHCLE_SMALL";
			case NKM_UNIT_STYLE_TYPE.NUST_OPERATOR:
				return "NKM_UI_COMMON_UNIT_TYPE_OPERATOR";
			case NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED:
			case NKM_UNIT_STYLE_TYPE.NUST_REPLACER:
			case NKM_UNIT_STYLE_TYPE.NUST_TRAINER:
			case NKM_UNIT_STYLE_TYPE.NUST_SHIP_ETC:
			case NKM_UNIT_STYLE_TYPE.NUST_ENV:
			case NKM_UNIT_STYLE_TYPE.NUST_ETC:
				return "NKM_UI_COMMON_UNIT_TYPE_ETC_SMALL";
			default:
				return "";
			}
		}
		switch (styleType)
		{
		case NKM_UNIT_STYLE_TYPE.NUST_COUNTER:
			return "NKM_UI_COMMON_UNIT_TYPE_COUNTER";
		case NKM_UNIT_STYLE_TYPE.NUST_MECHANIC:
			return "NKM_UI_COMMON_UNIT_TYPE_MECHANIC";
		case NKM_UNIT_STYLE_TYPE.NUST_SOLDIER:
			return "NKM_UI_COMMON_UNIT_TYPE_SOLDIER";
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT:
			return "NKM_UI_COMMON_SHIP_TYPE_ASSAULT";
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER:
			return "NKM_UI_COMMON_SHIP_TYPE_CRUISER";
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY:
			return "NKM_UI_COMMON_SHIP_TYPE_HEAVY";
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL:
			return "NKM_UI_COMMON_SHIP_TYPE_SPECIAL";
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL:
			return "NKM_UI_COMMON_SHIP_TYPE_PATROL_VEHCLE";
		case NKM_UNIT_STYLE_TYPE.NUST_OPERATOR:
			return "NKM_UI_COMMON_UNIT_TYPE_OPERATOR";
		case NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED:
		case NKM_UNIT_STYLE_TYPE.NUST_REPLACER:
		case NKM_UNIT_STYLE_TYPE.NUST_TRAINER:
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_ETC:
		case NKM_UNIT_STYLE_TYPE.NUST_ENV:
		case NKM_UNIT_STYLE_TYPE.NUST_ETC:
			return "NKM_UI_COMMON_UNIT_TYPE_ETC";
		default:
			return "";
		}
	}

	private static NKCAssetResourceBundle GetAssetResourceBundle(string bundleName, Dictionary<string, NKCAssetResourceBundle> dicBundle)
	{
		bundleName = bundleName.ToLower();
		NKCAssetResourceBundle nKCAssetResourceBundle = null;
		if (!dicBundle.ContainsKey(bundleName))
		{
			nKCAssetResourceBundle = new NKCAssetResourceBundle();
			dicBundle.Add(bundleName, nKCAssetResourceBundle);
		}
		else
		{
			nKCAssetResourceBundle = dicBundle[bundleName];
		}
		return nKCAssetResourceBundle;
	}

	public static void ClearResource()
	{
		Dictionary<string, NKCAssetResourceBundle>.Enumerator enumerator = m_dicAssetResourceBundle.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKCAssetResourceBundle value = enumerator.Current.Value;
			Dictionary<string, NKCAssetResourceData>.Enumerator enumerator2 = value.m_dicNKCResourceData.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				NKCAssetResourceManager.CloseResource(enumerator2.Current.Value);
			}
			value.m_dicNKCResourceData.Clear();
		}
	}

	public static void SwapResource()
	{
		ClearResource();
		Dictionary<string, NKCAssetResourceBundle>.Enumerator enumerator = m_dicAssetResourceBundleTemp.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKCAssetResourceBundle value = enumerator.Current.Value;
			Dictionary<string, NKCAssetResourceData>.Enumerator enumerator2 = value.m_dicNKCResourceData.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				NKCAssetResourceData value2 = enumerator2.Current.Value;
				NKCAssetResourceBundle assetResourceBundle = GetAssetResourceBundle(value2.m_NKMAssetName.m_BundleName, m_dicAssetResourceBundle);
				if (!assetResourceBundle.m_dicNKCResourceData.ContainsKey(value2.m_NKMAssetName.m_AssetName))
				{
					assetResourceBundle.m_dicNKCResourceData.Add(value2.m_NKMAssetName.m_AssetName, value2);
				}
				else
				{
					NKCAssetResourceManager.CloseResource(value2);
				}
			}
			value.m_dicNKCResourceData.Clear();
		}
	}

	public static NKCAssetResourceData GetAssetResource(NKMAssetName cNKMAssetName)
	{
		return GetAssetResource(cNKMAssetName.m_BundleName, cNKMAssetName.m_AssetName);
	}

	public static NKCAssetResourceData GetAssetResource(string bundleName, string assetName)
	{
		bundleName = NKCAssetResourceManager.RemapLocBundle(bundleName, assetName);
		NKCAssetResourceBundle assetResourceBundle = GetAssetResourceBundle(bundleName, m_dicAssetResourceBundle);
		if (assetResourceBundle.m_dicNKCResourceData.ContainsKey(assetName))
		{
			return assetResourceBundle.m_dicNKCResourceData[assetName];
		}
		return null;
	}

	public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100f)
	{
		Texture2D texture2D = LoadTexture(FilePath);
		if (texture2D != null)
		{
			return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), PixelsPerUnit);
		}
		return null;
	}

	public static Texture2D LoadTexture(string FilePath)
	{
		if (File.Exists(FilePath))
		{
			byte[] data = File.ReadAllBytes(FilePath);
			Texture2D texture2D = new Texture2D(2, 2);
			if (texture2D.LoadImage(data))
			{
				return texture2D;
			}
		}
		return null;
	}
}
