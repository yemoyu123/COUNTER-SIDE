using System.Collections.Generic;
using NKC.Dev;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleNew : MonoBehaviour
{
	private NKCUIDevConsoleContentBase[] m_Contents = new NKCUIDevConsoleContentBase[9];

	[Header("Dummy Object")]
	public GameObject m_pfbButton;

	public GameObject m_pfbCheckBox;

	[Header("첫번째 메뉴")]
	public List<ConsoleMainMenu> m_lstFirstMenu;

	[Header("Contents")]
	public NKCUIDevConsoleMenu m_ContentMenu;

	[Space]
	public NKCUIDevConsoleLog m_NKM_DEV_CONSOLE_LOG;

	public NKCUIDevConsoleContentSystem m_NKM_DEV_CONSOLE_SYSTEM;

	public NKCUIDevConsoleContentGame m_NKM_UI_DEV_CONSOLE_GAME;

	public NKCUIDevConsoleContentTest m_NKM_UI_DEV_CONSOLE_TEST;

	public NKCUIDevConsoleContentShop m_NKM_UI_DEV_CONSOLE_SHOP;

	public NKCUIDevConsoleContentItem m_NKM_UI_DEV_CONSOLE_ITEM;

	public NKCUIDevConsoleContentPVP m_NKM_UI_DEV_CONSOLE_PVP;

	public NKCUIDevConsoleContentCheat m_NKM_UI_DEV_CONSOLE_CHEAT;

	public NKCUIDevConsoleContentMemory m_MemoryStatConsole;

	[Header("button")]
	public NKCUIComStateButton m_NKM_DEV_CONSOLE_MENU_CLOSE_BUTTON;

	[Header("etc")]
	public Text m_PatchVersionText;

	public Text m_lbServerTime;
}
