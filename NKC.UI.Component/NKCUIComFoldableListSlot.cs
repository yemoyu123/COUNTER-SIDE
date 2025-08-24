using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI.Component;

public class NKCUIComFoldableListSlot : MonoBehaviour
{
	public delegate void OnSelectSlot(int majorKey, int minorKey, bool bToggleValue, bool isMajor);

	public NKCUIComToggle m_Toggle;

	private bool isMajor;

	private OnSelectSlot dOnSelectSlot;

	private List<NKCUIComFoldableListSlot> m_lstChild;

	public int MajorKey { get; private set; }

	public int MinorKey { get; private set; }

	public void SetData(NKCUIComFoldableList.Element element, OnSelectSlot onSelectSlot)
	{
		MajorKey = element.MajorKey;
		MinorKey = element.MinorKey;
		isMajor = element.isMajor;
		if (m_Toggle != null)
		{
			NKCUtil.SetToggleValueChangedDelegate(m_Toggle, OnToggle);
		}
		SetData(element);
		dOnSelectSlot = onSelectSlot;
	}

	protected virtual void SetData(NKCUIComFoldableList.Element element)
	{
	}

	public void SetToggleGroup(NKCUIComToggleGroup tglGroup)
	{
		m_Toggle?.SetToggleGroup(tglGroup);
	}

	public void Select(bool bSelect, bool bForce = false)
	{
		m_Toggle.Select(bSelect, bForce);
	}

	private void OnToggle(bool value)
	{
		dOnSelectSlot?.Invoke(MajorKey, MinorKey, value, isMajor);
	}

	public void ClearChild()
	{
		m_lstChild?.Clear();
		m_lstChild = null;
	}

	public void AddChild(NKCUIComFoldableListSlot child)
	{
		if (m_lstChild == null)
		{
			m_lstChild = new List<NKCUIComFoldableListSlot>();
		}
		m_lstChild.Add(child);
	}

	public void ActivateChild(bool value)
	{
		if (m_lstChild == null)
		{
			return;
		}
		foreach (NKCUIComFoldableListSlot item in m_lstChild)
		{
			NKCUtil.SetGameobjectActive(item, value);
		}
	}
}
