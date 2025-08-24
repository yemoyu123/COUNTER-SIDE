using System.Collections.Generic;
using NKM;
using NKM.Event;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCUIEventSubUI : MonoBehaviour
{
	private List<NKCUIEventSubUIBase> m_listSubUI = new List<NKCUIEventSubUIBase>();

	public void Init()
	{
		NKCUIEventSubUIBase[] components = base.gameObject.GetComponents<NKCUIEventSubUIBase>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Init();
			m_listSubUI.Add(components[i]);
		}
	}

	public void Open(NKMEventTabTemplet tabTemplet)
	{
		for (int i = 0; i < m_listSubUI.Count; i++)
		{
			m_listSubUI[i].Open(tabTemplet);
		}
	}

	public void Refresh()
	{
		for (int i = 0; i < m_listSubUI.Count; i++)
		{
			m_listSubUI[i].Refresh();
		}
	}

	public void Close()
	{
		for (int i = 0; i < m_listSubUI.Count; i++)
		{
			m_listSubUI[i].Close();
		}
	}

	public void Hide()
	{
		for (int i = 0; i < m_listSubUI.Count; i++)
		{
			m_listSubUI[i].Hide();
		}
	}

	public void UnHide()
	{
		for (int i = 0; i < m_listSubUI.Count; i++)
		{
			m_listSubUI[i].UnHide();
		}
	}

	public bool OnBackButton()
	{
		bool flag = false;
		for (int i = 0; i < m_listSubUI.Count; i++)
		{
			flag |= m_listSubUI[i].OnBackButton();
		}
		return flag;
	}

	public void OnInventoryChange(NKMItemMiscData itemData)
	{
		for (int i = 0; i < m_listSubUI.Count; i++)
		{
			m_listSubUI[i].OnInventoryChange(itemData);
		}
	}

	public IEnumerable<T> GetSubUIs<T>() where T : NKCUIEventSubUIBase
	{
		for (int i = 0; i < m_listSubUI.Count; i++)
		{
			if (m_listSubUI[i] is T)
			{
				yield return m_listSubUI[i] as T;
			}
		}
	}
}
