using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleContentGame : NKCUIDevConsoleContentBase2
{
	public NKCUIDevConsoleTutorial m_NKM_DEV_CONSOLE_TUTORIAL;

	private bool isMooJuckMode;

	private bool isWarfareUnbreakableMode;

	[Header("unit debug info toggle")]
	public NKCUIComToggle m_NKM_DEV_CONSOLE_MENU_SHOW_UNIT_DEBUG_INFO_TOGGLE;

	public Dropdown m_ddUnitDebugInfoType;

	public NKCUIComToggle m_TglSaveReplay;

	public Text m_lbTeam;

	private static bool m_ShowDebugDEInfo = false;

	private static NKC_DEV_UNIT_INFO m_DebugInfoType = NKC_DEV_UNIT_INFO.STAT;

	private static bool m_ShowDebugCollisionBox = false;

	private static bool m_ShowDebugAttackBox = false;

	private static bool m_ShowDebugUnitInfo = false;

	private static bool m_SaveReplay = false;

	public static bool GetShowDebugUnitInfo()
	{
		return m_ShowDebugUnitInfo;
	}

	public static NKC_DEV_UNIT_INFO GetUnitInfoType()
	{
		return m_DebugInfoType;
	}

	public static bool GetShowDebugDEInfo()
	{
		return m_ShowDebugDEInfo;
	}

	public static bool GetShowDebugCollisionBox()
	{
		return m_ShowDebugCollisionBox;
	}

	public static bool GetShowDebugAttackBox()
	{
		return m_ShowDebugAttackBox;
	}

	public static bool GetForceSaveReplay()
	{
		return m_SaveReplay;
	}
}
