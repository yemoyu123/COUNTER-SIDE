using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

[RequireComponent(typeof(ScrollRect))]
public class NKCUIComScrollRectHotkey : MonoBehaviour
{
	public float Speed = 4000f;

	private ScrollRect m_srTarget;

	private NKCUIBase m_uiRoot;

	public static void AddHotkey(ScrollRect sr, NKCUIBase uiRoot = null)
	{
		if (!(sr == null) && sr.gameObject.GetComponent<NKCUIComScrollRectHotkey>() == null)
		{
			sr.gameObject.AddComponent<NKCUIComScrollRectHotkey>().m_uiRoot = uiRoot;
		}
	}

	public static void EnableHotkey(ScrollRect sr, bool value)
	{
		if (!(sr == null) && sr.gameObject.TryGetComponent<NKCUIComLoopScrollHotkey>(out var component))
		{
			component.enabled = value;
		}
	}

	private void Start()
	{
		if (m_uiRoot == null)
		{
			m_uiRoot = NKCUIManager.FindRootUIBase(base.transform);
		}
		m_srTarget = GetComponent<ScrollRect>();
	}

	private void Update()
	{
		if (m_srTarget == null || m_srTarget.content == null || !NKCUIManager.IsTopmostUI(m_uiRoot))
		{
			return;
		}
		Vector2 anchoredPosition = m_srTarget.content.anchoredPosition;
		if (m_srTarget.horizontal)
		{
			if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Right))
			{
				anchoredPosition.x -= Speed * Time.deltaTime;
				m_srTarget.content.anchoredPosition = anchoredPosition;
			}
			else if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Left))
			{
				anchoredPosition.x += Speed * Time.deltaTime;
				m_srTarget.content.anchoredPosition = anchoredPosition;
			}
		}
		if (m_srTarget.vertical)
		{
			if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Up))
			{
				anchoredPosition.y -= Speed * Time.deltaTime;
				m_srTarget.content.anchoredPosition = anchoredPosition;
			}
			else if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Down))
			{
				anchoredPosition.y += Speed * Time.deltaTime;
				m_srTarget.content.anchoredPosition = anchoredPosition;
			}
		}
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey))
		{
			NKCUIComHotkeyDisplay.OpenInstance(m_srTarget);
		}
	}
}
