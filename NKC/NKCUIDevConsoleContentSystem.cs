namespace NKC;

public class NKCUIDevConsoleContentSystem : NKCUIDevConsoleContentBase2
{
	public NKCUIDevConsoleMail m_NKM_DEV_CONSOLE_MAIL;

	public NKCUIDevConsoleTagList m_NKM_DEV_CONSOLE_TAG_LIST;

	private static bool m_ShowDebugFrame;

	public static bool GetShowDebugFrame()
	{
		return m_ShowDebugFrame;
	}
}
