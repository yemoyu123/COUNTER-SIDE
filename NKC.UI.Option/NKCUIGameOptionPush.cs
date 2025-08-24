using NKC.Publisher;

namespace NKC.UI.Option;

public class NKCUIGameOptionPush : NKCUIGameOptionContentBase
{
	private NKCUIComToggle[] m_AllowPushToggles = new NKCUIComToggle[1];

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_PUSH_SLOT1_BTN;

	public override void Init()
	{
		m_AllowPushToggles[0] = m_NKM_UI_GAME_OPTION_PUSH_SLOT1_BTN;
		for (int i = 0; i < 1; i++)
		{
			NKC_GAME_OPTION_PUSH_GROUP pushGroup = (NKC_GAME_OPTION_PUSH_GROUP)i;
			m_AllowPushToggles[i]?.OnValueChanged.AddListener(delegate(bool allow)
			{
				OnClickAllowPushButton(pushGroup, allow);
			});
		}
	}

	public override void SetContent()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			for (int i = 0; i < 1; i++)
			{
				NKC_GAME_OPTION_PUSH_GROUP type = (NKC_GAME_OPTION_PUSH_GROUP)i;
				m_AllowPushToggles[i]?.Select(gameOptionData.GetAllowPush(type), bForce: true);
			}
		}
	}

	private void OnClickAllowPushButton(NKC_GAME_OPTION_PUSH_GROUP pushGroup, bool allow)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.SetAllowPush(pushGroup, allow);
			NKCPublisherModule.Push.ReRegisterPush();
			SetContent();
		}
	}
}
