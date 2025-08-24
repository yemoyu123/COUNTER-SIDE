using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM;

public class NKMSkinTemplet : INKMTemplet
{
	public enum SKIN_GRADE
	{
		SG_VARIATION,
		SG_NORMAL,
		SG_RARE,
		SG_PREMIUM,
		SG_SPECIAL
	}

	public enum SKIN_CUTIN
	{
		CUTIN_EMPTY,
		CUTIN_NORMAL,
		CUTIN_PRIVATE
	}

	public int m_SkinID;

	public int m_SkinEquipUnitID;

	public string m_SkinDesc;

	public string m_Title;

	public int m_ReturnItemId;

	public int m_ReturnItemCount;

	private string m_OpenTag;

	public string m_SkinStrID;

	public SKIN_GRADE m_SkinGrade;

	public bool m_bLimited;

	public bool m_bEffect;

	public bool m_Conversion;

	public bool m_LobbyFace;

	public bool m_Collabo;

	public bool m_Gauntlet;

	public SKIN_CUTIN m_SkinSkillCutIn;

	public string m_VoiceBundleName;

	public string m_SpriteBundleName;

	public string m_SpriteName;

	public string m_SpriteMaterialName = "";

	public string m_SpriteBundleNameSub;

	public string m_SpriteNameSub;

	public string m_SpriteMaterialNameSub = "";

	public string m_FaceCardName;

	public string m_SpineIllustName;

	public string m_SpineSDName;

	public string m_InvenIconName;

	public string m_HyperSkillCutin;

	public string m_CutscenePurchase;

	public string m_CutsceneLifetime_start;

	public string m_CutsceneLifetime_end;

	public string m_CutsceneLifetime_BG;

	public string m_LoginCutin;

	public bool m_bExclude;

	public int Key => m_SkinID;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public bool HasLoginCutin => !string.IsNullOrEmpty(m_LoginCutin);

	public static NKMSkinTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		return LoadFromLUA(cNKMLua, bIgnoreContentVersion: false);
	}

	private static NKMSkinTemplet LoadFromLUA(NKMLua cNKMLua, bool bIgnoreContentVersion)
	{
		if (!bIgnoreContentVersion && !NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMSkinManagerEx.cs", 83))
		{
			return null;
		}
		NKMSkinTemplet nKMSkinTemplet = new NKMSkinTemplet();
		cNKMLua.GetData("m_OpenTag", ref nKMSkinTemplet.m_OpenTag);
		int num = (int)(1u & (cNKMLua.GetData("m_SkinID", ref nKMSkinTemplet.m_SkinID) ? 1u : 0u) & (cNKMLua.GetData("m_SkinStrID", ref nKMSkinTemplet.m_SkinStrID) ? 1u : 0u) & (cNKMLua.GetData("m_SkinEquipUnitID", ref nKMSkinTemplet.m_SkinEquipUnitID) ? 1u : 0u) & (cNKMLua.GetData("m_Title", ref nKMSkinTemplet.m_Title) ? 1u : 0u)) & (cNKMLua.GetData("m_SkinDesc", ref nKMSkinTemplet.m_SkinDesc) ? 1 : 0);
		cNKMLua.GetData("m_ReturnItemID", ref nKMSkinTemplet.m_ReturnItemId);
		cNKMLua.GetData("m_ReturnItemCount", ref nKMSkinTemplet.m_ReturnItemCount);
		int num2 = (int)((uint)num & (cNKMLua.GetDataEnum<SKIN_GRADE>("m_SkinGrade", out nKMSkinTemplet.m_SkinGrade) ? 1u : 0u) & (cNKMLua.GetData("m_bLimited", ref nKMSkinTemplet.m_bLimited) ? 1u : 0u) & (cNKMLua.GetData("m_bEffect", ref nKMSkinTemplet.m_bEffect) ? 1u : 0u)) & (cNKMLua.GetData("m_SkinSkillCutIn", ref nKMSkinTemplet.m_SkinSkillCutIn) ? 1 : 0);
		cNKMLua.GetData("m_Conversion", ref nKMSkinTemplet.m_Conversion);
		cNKMLua.GetData("m_LobbyFace", ref nKMSkinTemplet.m_LobbyFace);
		cNKMLua.GetData("m_Collabo", ref nKMSkinTemplet.m_Collabo);
		cNKMLua.GetData("m_Gauntlet", ref nKMSkinTemplet.m_Gauntlet);
		cNKMLua.GetData("m_VoiceBundleName", ref nKMSkinTemplet.m_VoiceBundleName);
		int num3 = (int)((uint)num2 & (cNKMLua.GetData("m_SpriteBundleName", ref nKMSkinTemplet.m_SpriteBundleName) ? 1u : 0u)) & (cNKMLua.GetData("m_SpriteName", ref nKMSkinTemplet.m_SpriteName) ? 1 : 0);
		cNKMLua.GetData("m_SpriteMaterialName", ref nKMSkinTemplet.m_SpriteMaterialName);
		cNKMLua.GetData("m_SpriteBundleNameSub", ref nKMSkinTemplet.m_SpriteBundleNameSub);
		cNKMLua.GetData("m_SpriteNameSub", ref nKMSkinTemplet.m_SpriteNameSub);
		cNKMLua.GetData("m_SpriteMaterialNameSub", ref nKMSkinTemplet.m_SpriteMaterialNameSub);
		int num4 = (int)((uint)num3 & (cNKMLua.GetData("m_FaceCardName", ref nKMSkinTemplet.m_FaceCardName) ? 1u : 0u) & (cNKMLua.GetData("m_SpineIllustName", ref nKMSkinTemplet.m_SpineIllustName) ? 1u : 0u) & (cNKMLua.GetData("m_SpineSDName", ref nKMSkinTemplet.m_SpineSDName) ? 1u : 0u)) & (cNKMLua.GetData("m_InvenIconName", ref nKMSkinTemplet.m_InvenIconName) ? 1 : 0);
		cNKMLua.GetData("m_HyperSkillCutin", ref nKMSkinTemplet.m_HyperSkillCutin);
		cNKMLua.GetData("m_CutscenePurchase", ref nKMSkinTemplet.m_CutscenePurchase);
		cNKMLua.GetData("m_CutsceneLifetime_start", ref nKMSkinTemplet.m_CutsceneLifetime_start);
		cNKMLua.GetData("m_CutsceneLifetime_end", ref nKMSkinTemplet.m_CutsceneLifetime_end);
		cNKMLua.GetData("m_CutsceneLifetime_BG", ref nKMSkinTemplet.m_CutsceneLifetime_BG);
		cNKMLua.GetData("m_bExclude", ref nKMSkinTemplet.m_bExclude);
		cNKMLua.GetData("m_LoginCutin", ref nKMSkinTemplet.m_LoginCutin);
		if (num4 == 0)
		{
			Log.Error($"NKMSkinTemplet Load Fail - {nKMSkinTemplet.m_SkinID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMSkinManagerEx.cs", 141);
			return null;
		}
		return nKMSkinTemplet;
	}

	public string GetTitle()
	{
		return NKCStringTable.GetString(m_Title);
	}

	public string GetSkinDesc()
	{
		return NKCStringTable.GetString(m_SkinDesc);
	}

	public bool ChangesHyperCutin()
	{
		return !string.IsNullOrEmpty(m_HyperSkillCutin);
	}

	public bool ChangesVoice()
	{
		return !string.IsNullOrEmpty(m_VoiceBundleName);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
