using System.Collections.Generic;
using NKC.UI;
using UnityEngine;

namespace NKC;

public class NKCUIComToggleGroup : MonoBehaviour
{
	public bool m_bAllowSwitchOff;

	private List<NKCUIComToggle> m_lstComToggles = new List<NKCUIComToggle>();

	public bool m_bCallbackOnUnSelect;

	public bool m_bMultiSelect;

	public int m_MaxMultiCount = 1;

	public HotkeyEventType m_hotKeyPrev;

	public HotkeyEventType m_hotkeyNext;

	private NKCUIBase m_uiRoot;

	private void Start()
	{
		m_uiRoot = NKCUIManager.FindRootUIBase(base.transform);
	}

	public void RegisterToggle(NKCUIComToggle toggle)
	{
		if (!m_lstComToggles.Contains(toggle))
		{
			m_lstComToggles.Add(toggle);
		}
	}

	public void DeregisterToggle(NKCUIComToggle toggle)
	{
		if (m_lstComToggles.Contains(toggle))
		{
			m_lstComToggles.Remove(toggle);
		}
	}

	public void OnCheckOneToggle(NKCUIComToggle SelectedToggle)
	{
		if (!m_bMultiSelect)
		{
			foreach (NKCUIComToggle lstComToggle in m_lstComToggles)
			{
				if (lstComToggle != SelectedToggle)
				{
					lstComToggle.Select(bSelect: false, !m_bCallbackOnUnSelect);
				}
			}
			return;
		}
		int num = 0;
		foreach (NKCUIComToggle lstComToggle2 in m_lstComToggles)
		{
			if (lstComToggle2.m_bChecked)
			{
				num++;
			}
		}
		if (num > m_MaxMultiCount)
		{
			SelectedToggle.Select(bSelect: false, bForce: true);
		}
	}

	public void SetAllToggleUnselected()
	{
		foreach (NKCUIComToggle lstComToggle in m_lstComToggles)
		{
			lstComToggle.Select(bSelect: false, !m_bCallbackOnUnSelect);
		}
	}

	public void SetHotkey(HotkeyEventType prev, HotkeyEventType next)
	{
		m_hotKeyPrev = prev;
		m_hotkeyNext = next;
	}

	private void Update()
	{
		if (m_lstComToggles.Count <= 1 || (m_hotKeyPrev == HotkeyEventType.None && m_hotkeyNext == HotkeyEventType.None) || !NKCUIManager.IsTopmostUI(m_uiRoot))
		{
			return;
		}
		if (NKCInputManager.CheckHotKeyEvent(m_hotKeyPrev))
		{
			if (MoveSelection(-1))
			{
				NKCInputManager.ConsumeHotKeyEvent(m_hotKeyPrev);
			}
		}
		else if (NKCInputManager.CheckHotKeyEvent(m_hotkeyNext) && MoveSelection(1))
		{
			NKCInputManager.ConsumeHotKeyEvent(m_hotkeyNext);
		}
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey))
		{
			NKCUIComHotkeyDisplay.OpenInstance(base.transform, m_hotKeyPrev, m_hotkeyNext);
		}
	}

	private bool MoveSelection(int delta)
	{
		int num = m_lstComToggles.FindIndex((NKCUIComToggle x) => x.m_bSelect);
		if (num < 0)
		{
			return false;
		}
		int index = (num + m_lstComToggles.Count + delta) % m_lstComToggles.Count;
		NKCUIComToggle nKCUIComToggle = m_lstComToggles[index];
		if (!nKCUIComToggle.CanCastRaycast(out var _))
		{
			return false;
		}
		nKCUIComToggle.Select(bSelect: true);
		return true;
	}
}
