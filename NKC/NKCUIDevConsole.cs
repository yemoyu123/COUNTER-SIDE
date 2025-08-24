using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsole : MonoBehaviour
{
	private static NKCUIDevConsole instance;

	private bool m_bOpened;

	public NKCUIComStateButton m_NKM_DEV_CONSOLE_MENU_CLOSE_BUTTON;

	public NKCUIComStateButton m_NKM_DEV_CONSOLE_MENU_LOG_BUTTON;

	public NKCUIComStateButton m_NKM_DEV_CONSOLE_MENU_CHEAT_BUTTON;

	public NKCUIComStateButton m_NKM_DEV_CONSOLE_MENU_POST_BUTTON;

	public NKCUIComStateButton m_NKM_DEV_CONSOLE_MENU_TUTORIAL_BUTTON;

	public NKCUIComStateButton m_NKM_DEV_CONSOLE_MENU_ATTENDACE_BUTTON;

	public NKCUIComToggle m_NKM_DEV_CONSOLE_MENU_SHOW_UID_BUTTON;

	public NKCUIDevConsoleLog m_NKM_DEV_CONSOLE_LOG;

	public NKCUIDevConsoleCheat m_NKM_DEV_CONSOLE_CHEAT;

	public NKCUIDevConsoleTutorial m_NKM_DEV_CONSOLE_TUTORIAL;

	public NKCUIDevConsoleMail m_NKM_DEV_CONSOLE_MAIL;

	[Header("인게임 디버깅")]
	public GameObject m_NKM_DEV_CONSOLE_MENU_DEBUG_TOGGLES;

	public NKCUIComToggle m_NKM_DEV_CONSOLE_MENU_SHOW_FRAME_RATE_TOGGLE;

	public NKCUIComToggle m_NKM_DEV_CONSOLE_MENU_SHOW_UNIT_DEBUG_INFO_TOGGLE;

	public NKCUIComToggle m_NKM_DEV_CONSOLE_MENU_SHOW_DE_DEBUG_INFO_TOGGLE;

	public NKCUIComToggle m_NKM_DEV_CONSOLE_MENU_SHOW_UNIT_COLLISION_BOX_TOGGLE;

	public NKCUIComToggle m_NKM_DEV_CONSOLE_MENU_SHOW_ATTACK_BOX_TOGGLE;

	public NKCUIComToggle m_NKM_DEV_CONSOLE_MENU_SHOW_STRING_ID_TOGGLE;

	public NKCUIComToggle m_NKM_DEV_CONSOLE_MENU_HIDE_GAME_HUD_TOGGLE;

	public Dropdown m_ddUnitDebugInfoType;

	public Text m_PatchVersionText;

	public Text m_lbServerTime;
}
