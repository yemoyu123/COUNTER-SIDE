namespace NKC.UI;

public class NKCUIGame : NKCUIBase
{
	private int m_frameStep = 2;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override string MenuName => "인게임";

	public void Open()
	{
		UIOpened();
	}

	public override void CloseInternal()
	{
	}

	public override void UnHide()
	{
	}

	public override void Hide()
	{
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().Get_SCEN_GAME().OnBackButton();
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		return false;
	}

	public override void OnScreenResolutionChanged()
	{
		if (NKCScenManager.GetScenManager().GetGameClient() != null)
		{
			NKCScenManager.GetScenManager().GetGameClient().SetCamera(bResetPosition: false);
		}
	}
}
