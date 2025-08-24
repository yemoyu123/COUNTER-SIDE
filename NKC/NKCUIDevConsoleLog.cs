using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleLog : NKCUIDevConsoleContentBase
{
	private Queue<NKCUIDevConsoleLogScrollItem> m_LogScrollItemPool = new Queue<NKCUIDevConsoleLogScrollItem>();

	private Queue<NKCUIDevConsoleLogFileScrollItem> m_LogFileScrollItemPool = new Queue<NKCUIDevConsoleLogFileScrollItem>();

	private List<string> m_lstLog = new List<string>();

	private List<string> m_lstOldLog = new List<string>();

	private List<string> m_lstLogfile = new List<string>();

	public NKCUIDevConsoleLogScrollItem m_LogScrollItem;

	public NKCUIDevConsoleLogFileScrollItem m_LogFileScrollItem;

	public LoopScrollRect m_LSLog;

	public LoopScrollRect m_LSOldLog;

	public LoopScrollRect m_LSFile;

	public RectTransform m_LogPoolObject;

	public RectTransform m_LogFilePoolObject;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_MENU_CLEAR_LOG_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_MENU_CURRENT_LOG_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_MENU_PREV_LOG_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_MENU_LATEST_LOG_BUTTON;
}
