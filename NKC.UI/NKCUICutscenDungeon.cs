namespace NKC.UI;

public class NKCUICutscenDungeon : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_cutscen";

	private const string UI_ASSET_NAME = "NKM_CUTSCEN_DUNGEON_Panel";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public override string MenuName => "컷신 던전";

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public static NKCUIManager.LoadedUIData OpenNewInstance()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstance<NKCUICutscenDungeon>("ab_ui_nkm_ui_cutscen", "NKM_CUTSCEN_DUNGEON_Panel", NKCUIManager.eUIBaseRect.UIFrontCommon, null);
		}
		return s_LoadedUIData;
	}

	private void OnDestroy()
	{
		s_LoadedUIData = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (NKCUICutScenPlayer.HasInstance)
		{
			NKCUICutScenPlayer.Instance.StopWithCallBack();
		}
	}

	public void Open()
	{
		UIOpened();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		if (NKCUICutScenPlayer.IsInstanceOpen && NKCUICutScenPlayer.Instance.IsPlaying())
		{
			NKCUICutScenPlayer.Instance.StopWithCallBack();
		}
	}
}
