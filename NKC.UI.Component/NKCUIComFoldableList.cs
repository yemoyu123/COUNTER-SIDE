using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIComFoldableList : MonoBehaviour
{
	public struct Element : IComparable<Element>
	{
		public int MajorKey;

		public int MinorKey;

		public int MinorSortKey;

		public bool isMajor;

		public Tuple<int, int> KeyPair => new Tuple<int, int>(MajorKey, MinorKey);

		public bool isMinor => !isMajor;

		public int CompareTo(Element other)
		{
			if (MajorKey != other.MajorKey)
			{
				return MajorKey.CompareTo(other.MajorKey);
			}
			if (isMajor != other.isMajor)
			{
				return other.isMajor.CompareTo(isMajor);
			}
			if (MinorSortKey != other.MinorSortKey)
			{
				return MinorSortKey.CompareTo(other.MinorSortKey);
			}
			return MinorKey.CompareTo(other.MinorKey);
		}
	}

	public enum Mode
	{
		SingleOpenOnly,
		AllOpened,
		MultipleOpenClose
	}

	public delegate void OnSelectList(int major, int minor);

	[Header("스크롤")]
	public ScrollRect m_scrollRect;

	[Header("모드")]
	public Mode m_eMode;

	[Header("대분류의 선택이 가능한지? false라면 대분류 선택시 첫 소분류가 자동 선택된다.")]
	public bool m_bCanSelectMajor;

	[Header("대분류")]
	public NKCUIComFoldableListSlot m_pfbMajor;

	public NKCUIComToggleGroup m_MajorToggleGroup;

	[Header("소분류")]
	public NKCUIComFoldableListSlot m_pfbMinor;

	public NKCUIComToggleGroup m_MinorToggleGroup;

	private Dictionary<int, NKCUIComFoldableListSlot> m_dicMajorSlot = new Dictionary<int, NKCUIComFoldableListSlot>();

	private Dictionary<int, NKCUIComFoldableListSlot> m_dicFirstMinorSlot = new Dictionary<int, NKCUIComFoldableListSlot>();

	private Dictionary<Tuple<int, int>, NKCUIComFoldableListSlot> m_dicMinorSlot = new Dictionary<Tuple<int, int>, NKCUIComFoldableListSlot>();

	private OnSelectList dOnSelectList;

	private NKCUIBase m_uiBase;

	private int m_SelectedMajorIndex = -1;

	private int m_SelectedMinorIndex = -1;

	private List<Element> m_lstSelectableElements = new List<Element>();

	private void Start()
	{
		m_uiBase = NKCUIManager.FindRootUIBase(base.transform);
	}

	public void BuildList(List<Element> lstElement, OnSelectList onSelect)
	{
		if (m_scrollRect == null)
		{
			Debug.LogError("ScrollRect null!!");
			return;
		}
		if (m_scrollRect.content == null)
		{
			Debug.LogError("ScrollRect content null!!");
			return;
		}
		ValidateInput(lstElement);
		dOnSelectList = onSelect;
		_BuildList(lstElement);
	}

	public void SelectMajorSlot(int majorKey)
	{
		if (m_dicMajorSlot.TryGetValue(majorKey, out var value))
		{
			value.Select(bSelect: true, bForce: true);
			FoldSlot(majorKey, bOpen: true);
			m_SelectedMajorIndex = majorKey;
			m_SelectedMinorIndex = 0;
			if (!m_bCanSelectMajor && m_dicFirstMinorSlot.TryGetValue(majorKey, out var value2))
			{
				value2.Select(bSelect: true, bForce: true);
			}
		}
	}

	public void SelectMinorSlot(int majorKey, int minorKey)
	{
		if (m_SelectedMajorIndex != majorKey)
		{
			SelectMajorSlot(majorKey);
		}
		if (m_dicMinorSlot.TryGetValue(new Tuple<int, int>(majorKey, minorKey), out var value))
		{
			m_SelectedMinorIndex = minorKey;
			value.Select(bSelect: true, bForce: true);
		}
	}

	public NKCUIComFoldableListSlot GetSlot(bool bMajor, int majorKey, int minorKey)
	{
		if (bMajor)
		{
			return GetMajorSlot(majorKey);
		}
		return GetMinorSlot(majorKey, minorKey);
	}

	public NKCUIComFoldableListSlot GetMajorSlot(int majorKey)
	{
		if (m_dicMajorSlot.TryGetValue(majorKey, out var value))
		{
			return value;
		}
		return null;
	}

	public NKCUIComFoldableListSlot GetMinorSlot(int majorKey, int minorKey)
	{
		if (m_dicMinorSlot.TryGetValue(new Tuple<int, int>(majorKey, minorKey), out var value))
		{
			return value;
		}
		return null;
	}

	public void UnselectAll()
	{
		m_SelectedMajorIndex = -1;
		m_SelectedMinorIndex = -1;
		if (m_eMode == Mode.AllOpened)
		{
			foreach (KeyValuePair<int, NKCUIComFoldableListSlot> item in m_dicMajorSlot)
			{
				item.Value?.Select(bSelect: false, bForce: true);
			}
			{
				foreach (KeyValuePair<Tuple<int, int>, NKCUIComFoldableListSlot> item2 in m_dicMinorSlot)
				{
					item2.Value?.Select(bSelect: false, bForce: true);
				}
				return;
			}
		}
		foreach (KeyValuePair<int, NKCUIComFoldableListSlot> item3 in m_dicMajorSlot)
		{
			FoldSlot(item3.Key, bOpen: false);
			item3.Value?.Select(bSelect: false, bForce: true);
		}
	}

	private void _BuildList(List<Element> lstElement)
	{
		m_SelectedMajorIndex = -1;
		m_SelectedMinorIndex = -1;
		m_lstSelectableElements.Clear();
		Stack<NKCUIComFoldableListSlot> stack = new Stack<NKCUIComFoldableListSlot>(m_dicMajorSlot.Values);
		Stack<NKCUIComFoldableListSlot> stack2 = new Stack<NKCUIComFoldableListSlot>(m_dicMinorSlot.Values);
		foreach (NKCUIComFoldableListSlot value2 in m_dicMajorSlot.Values)
		{
			value2.ClearChild();
		}
		m_dicFirstMinorSlot.Clear();
		lstElement.Sort();
		Dictionary<int, NKCUIComFoldableListSlot> dictionary = new Dictionary<int, NKCUIComFoldableListSlot>();
		Dictionary<Tuple<int, int>, NKCUIComFoldableListSlot> dictionary2 = new Dictionary<Tuple<int, int>, NKCUIComFoldableListSlot>();
		List<NKCUIComFoldableListSlot> list = new List<NKCUIComFoldableListSlot>();
		foreach (Element item in lstElement)
		{
			if (item.isMajor)
			{
				NKCUIComFoldableListSlot nKCUIComFoldableListSlot = ((stack.Count <= 0) ? GetNewSlot(bMajor: true) : stack.Pop());
				nKCUIComFoldableListSlot.SetData(item, OnSelectSlot);
				nKCUIComFoldableListSlot.SetToggleGroup(m_MajorToggleGroup);
				dictionary.Add(item.MajorKey, nKCUIComFoldableListSlot);
				list.Add(nKCUIComFoldableListSlot);
				if (m_bCanSelectMajor)
				{
					m_lstSelectableElements.Add(item);
				}
				continue;
			}
			if (!dictionary.TryGetValue(item.MajorKey, out var value))
			{
				Debug.LogError($"Logic Error : MajorKey {item.MajorKey} slot not created! ");
				continue;
			}
			NKCUIComFoldableListSlot nKCUIComFoldableListSlot2 = ((stack2.Count <= 0) ? GetNewSlot(bMajor: false) : stack2.Pop());
			nKCUIComFoldableListSlot2.SetData(item, OnSelectSlot);
			nKCUIComFoldableListSlot2.SetToggleGroup(m_MinorToggleGroup);
			value.AddChild(nKCUIComFoldableListSlot2);
			if (!m_dicFirstMinorSlot.ContainsKey(item.MajorKey))
			{
				m_dicFirstMinorSlot.Add(item.MajorKey, nKCUIComFoldableListSlot2);
			}
			dictionary2.Add(item.KeyPair, nKCUIComFoldableListSlot2);
			list.Add(nKCUIComFoldableListSlot2);
			m_lstSelectableElements.Add(item);
		}
		while (stack.Count > 0)
		{
			UnityEngine.Object.Destroy(stack.Pop().gameObject);
		}
		while (stack2.Count > 0)
		{
			UnityEngine.Object.Destroy(stack2.Pop().gameObject);
		}
		foreach (NKCUIComFoldableListSlot item2 in list)
		{
			item2.transform.SetParent(m_scrollRect.content);
			item2.transform.SetAsLastSibling();
		}
		m_dicMajorSlot = dictionary;
		m_dicMinorSlot = dictionary2;
		foreach (KeyValuePair<int, NKCUIComFoldableListSlot> item3 in dictionary)
		{
			FoldSlot(item3.Key, bOpen: false);
		}
	}

	private void FoldSlot(int majorKey, bool bOpen)
	{
		switch (m_eMode)
		{
		case Mode.MultipleOpenClose:
			m_dicMajorSlot[majorKey]?.ActivateChild(bOpen);
			break;
		case Mode.SingleOpenOnly:
			if (!bOpen)
			{
				m_dicMajorSlot[majorKey]?.ActivateChild(value: false);
				break;
			}
			{
				foreach (KeyValuePair<int, NKCUIComFoldableListSlot> item in m_dicMajorSlot)
				{
					item.Value?.ActivateChild(majorKey == item.Key);
				}
				break;
			}
		case Mode.AllOpened:
			m_dicMajorSlot[majorKey]?.ActivateChild(value: true);
			break;
		}
	}

	private void OnSelectSlot(int majorKey, int minorKey, bool bToggleValue, bool isMajor)
	{
		if (isMajor)
		{
			if (bToggleValue)
			{
				m_SelectedMajorIndex = majorKey;
				m_SelectedMinorIndex = minorKey;
			}
			FoldSlot(majorKey, bToggleValue);
			if (bToggleValue)
			{
				NKCUIComFoldableListSlot value;
				if (m_bCanSelectMajor)
				{
					dOnSelectList?.Invoke(majorKey, minorKey);
				}
				else if (m_dicFirstMinorSlot.TryGetValue(majorKey, out value))
				{
					value.Select(bSelect: true, bForce: true);
					dOnSelectList?.Invoke(value.MajorKey, value.MinorKey);
				}
			}
		}
		else if (bToggleValue)
		{
			dOnSelectList?.Invoke(majorKey, minorKey);
		}
	}

	private NKCUIComFoldableListSlot GetNewSlot(bool bMajor)
	{
		if (bMajor)
		{
			return UnityEngine.Object.Instantiate(m_pfbMajor);
		}
		return UnityEngine.Object.Instantiate(m_pfbMinor);
	}

	private bool ValidateInput(List<Element> lstElement)
	{
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<Tuple<int, int>> hashSet2 = new HashSet<Tuple<int, int>>();
		foreach (Element item in lstElement)
		{
			if (item.isMajor)
			{
				if (hashSet.Contains(item.MajorKey))
				{
					Debug.LogError("Major key duplicate!");
					return false;
				}
				hashSet.Add(item.MajorKey);
			}
		}
		foreach (Element item2 in lstElement)
		{
			if (item2.isMinor)
			{
				if (!hashSet.Contains(item2.MajorKey))
				{
					Debug.LogError($"There is no Major Key {item2.MajorKey}");
					return false;
				}
				Tuple<int, int> keyPair = item2.KeyPair;
				if (hashSet2.Contains(keyPair))
				{
					Debug.LogError($"Minor Key Duplicate : {item2.MajorKey}, {item2.MinorKey}");
					return false;
				}
				hashSet2.Add(keyPair);
			}
		}
		return true;
	}

	private void Update()
	{
		ProcessHotkey();
	}

	public void ProcessHotkey()
	{
		if (!NKCUIManager.IsTopmostUI(m_uiBase) || !(m_scrollRect != null))
		{
			return;
		}
		if (m_scrollRect.vertical)
		{
			if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.Up) && MoveSelection(-1))
			{
				NKCInputManager.ConsumeHotKeyEvent(HotkeyEventType.Up);
			}
			if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.Down) && MoveSelection(1))
			{
				NKCInputManager.ConsumeHotKeyEvent(HotkeyEventType.Down);
			}
		}
		if (m_scrollRect.horizontal)
		{
			if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.Left) && MoveSelection(-1))
			{
				NKCInputManager.ConsumeHotKeyEvent(HotkeyEventType.Left);
			}
			if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.Right) && MoveSelection(1))
			{
				NKCInputManager.ConsumeHotKeyEvent(HotkeyEventType.Right);
			}
		}
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey))
		{
			NKCUIComHotkeyDisplay.OpenInstance(m_scrollRect, base.transform);
		}
	}

	private bool MoveSelection(int delta)
	{
		int num = m_lstSelectableElements.FindIndex((Element x) => x.MajorKey == m_SelectedMajorIndex && x.MinorKey == m_SelectedMinorIndex) + delta;
		if (num < 0)
		{
			return false;
		}
		if (num >= m_lstSelectableElements.Count)
		{
			return false;
		}
		Element element = m_lstSelectableElements[num];
		OnSelectSlot(element.MajorKey, element.MinorKey, bToggleValue: true, element.isMajor);
		return true;
	}
}
