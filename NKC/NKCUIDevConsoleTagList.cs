using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleTagList : NKCUIDevConsoleContentBase
{
	private Queue<NKCUIDevConsoleLogScrollItem> m_LogScrollItemPool = new Queue<NKCUIDevConsoleLogScrollItem>();

	private List<string> m_lstTags = new List<string>();

	public NKCUIDevConsoleLogScrollItem m_LogScrollItem;

	public InputField m_ifSearch;

	public LoopScrollRect m_LSLog;

	public RectTransform m_LogPoolObject;

	public RectTransform m_LogFilePoolObject;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_OPEN_TAG_LIST_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CONTENT_TAG_LIST_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_INTERVAL_LIST_BUTTON;

	public NKCUIComToggle m_IntervalToggle;
}
