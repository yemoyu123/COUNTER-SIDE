using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleCheatUserInfo : NKCUIDevConsoleContentBase
{
	private enum ChangeResourceOp
	{
		Plus,
		Minus
	}

	private enum ResourceType
	{
		Credit,
		Eternium,
		Information,
		CashPaid,
		CashFree,
		Max
	}

	public NKCUIDevConsoleCheatUserInfoController m_NKM_UI_DEV_CONSOLE_CHEAT_USER_INFO_LEVEL;

	public Toggle m_BeforeLevelUpTogglePaid;

	public Dropdown m_NKM_UI_DEV_CONSOLE_CHEAT_RESOURCE_TYPE;

	public NKCUIDevConsoleCheatUserInfoController m_NKM_UI_DEV_CONSOLE_CHEAT_USER_INFO_RESOURCE;

	public InputField m_ifRecourceID;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_ADD_USER_BUFF;

	public InputField m_NKM_UI_DEV_CONSOLE_CHEAT_USER_BUFF_ID_INPUT_FIELD;

	public Toggle m_TogglePaid;
}
