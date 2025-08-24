using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCUIReplayLobby : NKCUIBase
{
	public Animator m_amtorLeft;

	[Header("Fallback BG")]
	public GameObject m_objBGFallBack;

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_LOBBY";

	private bool m_bInit;

	public override string MenuName => NKCUtilString.GET_STRING_REPLAY;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override string GuideTempletID => "ARTICLE_REPLAY";

	public override List<int> UpsideMenuShowResourceList => new List<int> { 101 };

	public static NKCAssetResourceData OpenInstanceAsync()
	{
		return NKCUIBase.OpenInstanceAsync<NKCUIBaseSceneMenu>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LOBBY");
	}

	public static bool CheckInstanceLoaded(NKCAssetResourceData loadResourceData, out NKCUIReplayLobby retVal)
	{
		return NKCUIBase.CheckInstanceLoaded<NKCUIReplayLobby>(loadResourceData, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), out retVal);
	}

	public void CloseInstance()
	{
		NKCAssetResourceManager.CloseResource("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LOBBY");
		Object.Destroy(base.gameObject);
	}

	public void InitUI()
	{
		if (!m_bInit)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			m_bInit = true;
		}
	}

	private void ResetUIByCurrTab()
	{
		UpdateUpsideMenu();
	}

	public void Open()
	{
		CheckTutorial();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_INTRO);
	}

	private void CheckTutorial()
	{
	}
}
