using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

[RequireComponent(typeof(LoopScrollRect))]
public class NKCUIComLoopScrollHotkey : MonoBehaviour
{
	public float Speed = 4000f;

	private LoopScrollRect m_srTarget;

	private NKCUIBase m_uiRoot;

	public static void AddHotkey(LoopScrollRect sr, NKCUIBase uiRoot = null)
	{
		if (!(sr == null) && sr.gameObject.GetComponent<NKCUIComLoopScrollHotkey>() == null)
		{
			sr.gameObject.AddComponent<NKCUIComLoopScrollHotkey>().m_uiRoot = uiRoot;
		}
	}

	public static void EnableHotkey(LoopScrollRect sr, bool value)
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
		m_srTarget = GetComponent<LoopScrollRect>();
	}

	private void Update()
	{
		if (m_srTarget == null || !NKCUIManager.IsTopmostUI(m_uiRoot))
		{
			return;
		}
		if (m_srTarget.horizontal)
		{
			if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Right))
			{
				m_srTarget.MovePosition(new Vector2((0f - Speed) * Time.deltaTime, 0f));
			}
			else if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Left))
			{
				m_srTarget.MovePosition(new Vector2(Speed * Time.deltaTime, 0f));
			}
		}
		if (m_srTarget.vertical)
		{
			if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Up))
			{
				m_srTarget.MovePosition(new Vector2(0f, (0f - Speed) * Time.deltaTime));
			}
			else if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Down))
			{
				m_srTarget.MovePosition(new Vector2(0f, Speed * Time.deltaTime));
			}
		}
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey))
		{
			NKCUIComHotkeyDisplay.OpenInstance(m_srTarget);
		}
	}
}
