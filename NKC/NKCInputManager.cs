using System.Collections.Generic;
using NKC.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC;

public class NKCInputManager : MonoBehaviour
{
	private static NKCInputManager m_InputManager;

	private HashSet<HotkeyEventType> m_hsCurrentHotKeys = new HashSet<HotkeyEventType>();

	private HashSet<HotkeyEventType> m_hsCurrentUpHotKeys = new HashSet<HotkeyEventType>();

	private HashSet<InGamehotkeyEventType> m_hsCurrentIngameHotKeys = new HashSet<InGamehotkeyEventType>();

	private bool m_bShift;

	private bool m_bCtrl;

	private Dictionary<(SubHotKey, KeyCode), HotkeyEventType> m_dicHotkeyPair = new Dictionary<(SubHotKey, KeyCode), HotkeyEventType>();

	private Dictionary<KeyCode, InGamehotkeyEventType> m_dicIngameHotkeyPair = new Dictionary<KeyCode, InGamehotkeyEventType>();

	private Dictionary<HotkeyEventType, (SubHotKey, KeyCode)> m_dicKeycodeForHotkeyHelp = new Dictionary<HotkeyEventType, (SubHotKey, KeyCode)>();

	private Dictionary<InGamehotkeyEventType, KeyCode> m_dicKeycodeForIngameHotkeyHelp = new Dictionary<InGamehotkeyEventType, KeyCode>();

	private HashSet<KeyCode> m_hsIgnoreKey = new HashSet<KeyCode>();

	public static float ScrollSensibility => 150f;

	public static NKCInputManager GetInputManager()
	{
		return m_InputManager;
	}

	private void Awake()
	{
		if (m_InputManager == null)
		{
			m_InputManager = this;
			SetDefaultHotkeyEvent();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		m_bShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		m_bCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
		SubHotKey subHotKey = SubHotKey.None;
		if (m_bShift)
		{
			subHotKey = SubHotKey.Shift;
		}
		else if (m_bCtrl)
		{
			subHotKey = SubHotKey.Ctrl;
		}
		m_hsCurrentHotKeys.Clear();
		m_hsCurrentIngameHotKeys.Clear();
		m_hsCurrentUpHotKeys.Clear();
		if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null && (EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null || EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null))
		{
			return;
		}
		if (Input.anyKeyDown)
		{
			foreach (KeyValuePair<(SubHotKey, KeyCode), HotkeyEventType> item in m_dicHotkeyPair)
			{
				if ((item.Key.Item1 == subHotKey || item.Key.Item1 == SubHotKey.All) && !m_hsIgnoreKey.Contains(item.Key.Item2))
				{
					HotkeyEventType value = item.Value;
					if (Input.GetKeyDown(item.Key.Item2) && !NKCScenManager.GetScenManager().ProcessGlobalHotkey(value) && !NKCUIManager.OnHotkey(value))
					{
						m_hsCurrentHotKeys.Add(value);
					}
				}
			}
			foreach (KeyValuePair<KeyCode, InGamehotkeyEventType> item2 in m_dicIngameHotkeyPair)
			{
				if (Input.GetKey(item2.Key))
				{
					m_hsCurrentIngameHotKeys.Add(item2.Value);
				}
			}
		}
		if (Input.anyKey)
		{
			foreach (KeyValuePair<(SubHotKey, KeyCode), HotkeyEventType> item3 in m_dicHotkeyPair)
			{
				if (item3.Key.Item1 == subHotKey && !m_hsIgnoreKey.Contains(item3.Key.Item2) && Input.GetKey(item3.Key.Item2))
				{
					NKCScenManager.GetScenManager().ProcessGlobalHotkeyHold(item3.Value);
					NKCUIManager.OnHotkeyHold(item3.Value);
				}
			}
		}
		foreach (KeyValuePair<(SubHotKey, KeyCode), HotkeyEventType> item4 in m_dicHotkeyPair)
		{
			if (item4.Key.Item1 == subHotKey && !m_hsIgnoreKey.Contains(item4.Key.Item2) && Input.GetKeyUp(item4.Key.Item2))
			{
				NKCUIManager.OnHotKeyRelease(item4.Value);
				m_hsCurrentUpHotKeys.Add(item4.Value);
			}
		}
	}

	private void SetDefaultHotkeyEvent()
	{
		AddHotkeyPair(KeyCode.UpArrow, HotkeyEventType.Up);
		AddHotkeyPair(KeyCode.DownArrow, HotkeyEventType.Down);
		AddHotkeyPair(KeyCode.LeftArrow, HotkeyEventType.Left);
		AddHotkeyPair(KeyCode.RightArrow, HotkeyEventType.Right);
		AddHotkeyPair(KeyCode.Q, HotkeyEventType.RotateLeft);
		AddHotkeyPair(KeyCode.E, HotkeyEventType.RotateRight);
		AddHotkeyPair(KeyCode.W, HotkeyEventType.Up);
		AddHotkeyPair(KeyCode.S, HotkeyEventType.Down);
		AddHotkeyPair(KeyCode.A, HotkeyEventType.Left);
		AddHotkeyPair(KeyCode.D, HotkeyEventType.Right);
		AddHotkeyPair(KeyCode.Keypad8, HotkeyEventType.Up);
		AddHotkeyPair(KeyCode.Keypad2, HotkeyEventType.Down);
		AddHotkeyPair(KeyCode.Keypad4, HotkeyEventType.Left);
		AddHotkeyPair(KeyCode.Keypad6, HotkeyEventType.Right);
		AddHotkeyPair(KeyCode.Tab, HotkeyEventType.NextTab);
		AddHotkeyPair(KeyCode.Tab, HotkeyEventType.PrevTab, SubHotKey.Shift);
		AddHotkeyPair(KeyCode.Return, HotkeyEventType.Confirm, SubHotKey.All);
		AddHotkeyPair(KeyCode.KeypadEnter, HotkeyEventType.Confirm, SubHotKey.All);
		AddHotkeyPair(KeyCode.Space, HotkeyEventType.Confirm, SubHotKey.All);
		AddHotkeyPair(KeyCode.Keypad5, HotkeyEventType.Confirm, SubHotKey.All);
		AddHotkeyPair(KeyCode.F1, HotkeyEventType.Help);
		AddHotkeyPair(KeyCode.LeftAlt, HotkeyEventType.ShowHotkey);
		AddHotkeyPair(KeyCode.Plus, HotkeyEventType.Plus);
		AddHotkeyPair(KeyCode.Minus, HotkeyEventType.Minus);
		AddHotkeyPair(KeyCode.KeypadPlus, HotkeyEventType.Plus);
		AddHotkeyPair(KeyCode.KeypadMinus, HotkeyEventType.Minus);
		AddHotkeyPair(KeyCode.LeftControl, HotkeyEventType.Skip, SubHotKey.Ctrl);
		AddHotkeyPair(KeyCode.Escape, HotkeyEventType.Cancel, SubHotKey.All);
		AddHotkeyPair(KeyCode.Mouse3, HotkeyEventType.Cancel, SubHotKey.All);
		AddHotkeyPair(KeyCode.Alpha4, InGamehotkeyEventType.Unit0);
		AddHotkeyPair(KeyCode.Alpha3, InGamehotkeyEventType.Unit1);
		AddHotkeyPair(KeyCode.Alpha2, InGamehotkeyEventType.Unit2);
		AddHotkeyPair(KeyCode.Alpha1, InGamehotkeyEventType.Unit3);
		AddHotkeyPair(KeyCode.Alpha5, InGamehotkeyEventType.UnitAssist);
		AddHotkeyPair(KeyCode.E, InGamehotkeyEventType.ShipSkill0);
		AddHotkeyPair(KeyCode.R, InGamehotkeyEventType.ShipSkill1);
		AddHotkeyPair(KeyCode.F12, HotkeyEventType.ScreenCapture);
		AddHotkeyPair(KeyCode.M, HotkeyEventType.Mute);
		AddHotkeyPair(KeyCode.LeftBracket, HotkeyEventType.MasterVolumeDown);
		AddHotkeyPair(KeyCode.RightBracket, HotkeyEventType.MasterVolumeUp);
		AddHotkeyPair(KeyCode.Tab, InGamehotkeyEventType.Emoticon);
		AddHotkeyPair(KeyCode.Equals, HotkeyEventType.HamburgerMenu);
		AddHotkeyPair(KeyCode.C, HotkeyEventType.Copy, SubHotKey.Ctrl);
		AddHotkeyPair(KeyCode.V, HotkeyEventType.Paste, SubHotKey.Ctrl);
	}

	private void AddHotkeyPair(KeyCode keyCode, HotkeyEventType eventType, SubHotKey subKey = SubHotKey.None)
	{
		m_dicHotkeyPair.Add((subKey, keyCode), eventType);
		if (!m_dicKeycodeForHotkeyHelp.ContainsKey(eventType))
		{
			m_dicKeycodeForHotkeyHelp.Add(eventType, (subKey, keyCode));
		}
	}

	private void AddHotkeyPair(KeyCode keyCode, InGamehotkeyEventType eventType)
	{
		m_dicIngameHotkeyPair.Add(keyCode, eventType);
		if (!m_dicKeycodeForIngameHotkeyHelp.ContainsKey(eventType))
		{
			m_dicKeycodeForIngameHotkeyHelp.Add(eventType, keyCode);
		}
	}

	public static bool IsHotkeyPressed(HotkeyEventType eventType)
	{
		if (eventType == HotkeyEventType.None)
		{
			return false;
		}
		if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
		{
			if (EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
			{
				return false;
			}
			if (EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null)
			{
				return false;
			}
		}
		if (m_InputManager == null)
		{
			return false;
		}
		return m_InputManager._IsHotkeyPressed(eventType);
	}

	private bool _IsHotkeyPressed(HotkeyEventType eventType)
	{
		if (eventType == HotkeyEventType.None)
		{
			return false;
		}
		foreach (KeyValuePair<(SubHotKey, KeyCode), HotkeyEventType> item in m_dicHotkeyPair)
		{
			if (item.Value == eventType && CheckSubHotKey(item.Key.Item1) && !m_hsIgnoreKey.Contains(item.Key.Item2) && Input.GetKey(item.Key.Item2))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckHotKeyEvent(HotkeyEventType type)
	{
		if (type == HotkeyEventType.None)
		{
			return false;
		}
		if (m_InputManager == null)
		{
			return false;
		}
		return m_InputManager._CheckHotKeyEvent(type);
	}

	private bool CheckSubHotKey(SubHotKey subHotKey)
	{
		return subHotKey switch
		{
			SubHotKey.Shift => m_bShift, 
			SubHotKey.Ctrl => m_bCtrl, 
			_ => true, 
		};
	}

	private bool _CheckHotKeyEvent(HotkeyEventType type)
	{
		if (m_hsCurrentHotKeys.Contains(type))
		{
			return true;
		}
		return false;
	}

	public static bool CheckHotKeyUp(HotkeyEventType type)
	{
		if (type == HotkeyEventType.None)
		{
			return false;
		}
		if (m_InputManager == null)
		{
			return false;
		}
		return m_InputManager._CheckHotKeyUp(type);
	}

	private bool _CheckHotKeyUp(HotkeyEventType type)
	{
		return m_hsCurrentUpHotKeys.Contains(type);
	}

	public static bool CheckHotKeyEvent(InGamehotkeyEventType type)
	{
		if (m_InputManager == null)
		{
			return false;
		}
		return m_InputManager._CheckHotKeyEvent(type);
	}

	private bool _CheckHotKeyEvent(InGamehotkeyEventType type)
	{
		return m_hsCurrentIngameHotKeys.Contains(type);
	}

	public static void ConsumeHotKeyEvent(HotkeyEventType type)
	{
		if (!(m_InputManager == null) && type != HotkeyEventType.ShowHotkey)
		{
			m_InputManager._ConsumeHotKeyEvent(type);
		}
	}

	private void _ConsumeHotKeyEvent(HotkeyEventType type)
	{
		m_hsCurrentHotKeys.Remove(type);
	}

	public static void ConsumeHotKeyEvent(InGamehotkeyEventType type)
	{
		if (!(m_InputManager == null))
		{
			m_InputManager._ConsumeHotKeyEvent(type);
		}
	}

	private void _ConsumeHotKeyEvent(InGamehotkeyEventType type)
	{
		m_hsCurrentIngameHotKeys.Remove(type);
	}

	public string GetHotkeyString(KeyCode keyCode)
	{
		switch (keyCode)
		{
		case KeyCode.UpArrow:
			return "↑";
		case KeyCode.DownArrow:
			return "↓";
		case KeyCode.LeftArrow:
			return "←";
		case KeyCode.RightArrow:
			return "→";
		case KeyCode.Tab:
			return "Tab";
		case KeyCode.Return:
		case KeyCode.KeypadEnter:
			return '⏎'.ToString();
		case KeyCode.Escape:
			return "Esc";
		case KeyCode.Plus:
		case KeyCode.KeypadPlus:
			return "+";
		case KeyCode.Minus:
		case KeyCode.KeypadMinus:
			return "-";
		case KeyCode.Alpha1:
			return "1";
		case KeyCode.Alpha2:
			return "2";
		case KeyCode.Alpha3:
			return "3";
		case KeyCode.Alpha4:
			return "4";
		case KeyCode.Alpha5:
			return "5";
		case KeyCode.Alpha6:
			return "6";
		case KeyCode.Alpha7:
			return "7";
		case KeyCode.Alpha8:
			return "8";
		case KeyCode.Alpha9:
			return "9";
		case KeyCode.Alpha0:
			return "0";
		case KeyCode.Backspace:
			return "BackSpace";
		case KeyCode.Space:
			return "Space";
		case KeyCode.LeftControl:
			return "Ctrl";
		case KeyCode.RightControl:
			return "RCtrl";
		case KeyCode.LeftBracket:
			return "[";
		case KeyCode.RightBracket:
			return "]";
		case KeyCode.Equals:
			return "=";
		default:
			if (KeyCode.A <= keyCode && keyCode <= KeyCode.Z)
			{
				return keyCode.ToString();
			}
			if (KeyCode.F1 <= keyCode && keyCode <= KeyCode.F15)
			{
				return keyCode.ToString();
			}
			Debug.LogError($"Keycode {keyCode} not defined!");
			return string.Empty;
		}
	}

	public string GetHotkeyString(HotkeyEventType type)
	{
		if (m_dicKeycodeForHotkeyHelp.TryGetValue(type, out var value))
		{
			string hotkeyString = GetHotkeyString(value.Item2);
			switch (value.Item1)
			{
			case SubHotKey.None:
			case SubHotKey.All:
				return hotkeyString;
			case SubHotKey.Shift:
				return "⇧" + hotkeyString;
			case SubHotKey.Ctrl:
				return "^" + hotkeyString;
			}
		}
		return string.Empty;
	}

	public string GetHotkeyString(InGamehotkeyEventType type)
	{
		if (m_dicKeycodeForIngameHotkeyHelp.TryGetValue(type, out var value))
		{
			return GetHotkeyString(value);
		}
		return string.Empty;
	}

	public static HotkeyEventType GetDirection(Vector2 direction)
	{
		if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
		{
			if (!(direction.x > 0f))
			{
				return HotkeyEventType.Left;
			}
			return HotkeyEventType.Right;
		}
		if (!(direction.y > 0f))
		{
			return HotkeyEventType.Down;
		}
		return HotkeyEventType.Up;
	}

	public static Vector2 GetMoveVector()
	{
		Vector2 zero = Vector2.zero;
		if (IsHotkeyPressed(HotkeyEventType.Left))
		{
			zero.x -= 1f;
		}
		if (IsHotkeyPressed(HotkeyEventType.Right))
		{
			zero.x += 1f;
		}
		if (IsHotkeyPressed(HotkeyEventType.Up))
		{
			zero.y += 1f;
		}
		if (IsHotkeyPressed(HotkeyEventType.Down))
		{
			zero.y -= 1f;
		}
		return zero.normalized;
	}

	public static void AddIgnoreKey(params KeyCode[] keys)
	{
		if (m_InputManager != null)
		{
			m_InputManager.m_hsIgnoreKey.UnionWith(keys);
		}
	}

	public static void ClearIgnoreKey(params KeyCode[] keys)
	{
		if (m_InputManager != null)
		{
			m_InputManager.m_hsIgnoreKey.Clear();
		}
	}

	public static bool IsChatSubmitEnter()
	{
		if (!Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKeyDown(KeyCode.Return))
		{
			return Input.GetButtonDown("Submit");
		}
		return true;
	}
}
