using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCWarfareGameItemMgr
{
	private Transform m_Parent;

	private List<NKCWarfareGameItem> m_itemList = new List<NKCWarfareGameItem>();

	public NKCWarfareGameItemMgr(Transform parent)
	{
		m_Parent = parent;
	}

	public void Set(int index, WARFARE_ITEM_STATE state, Vector3 pos, bool bWithEnemy)
	{
		NKCWarfareGameItem nKCWarfareGameItem = getItem(index);
		if (nKCWarfareGameItem == null)
		{
			if (state == WARFARE_ITEM_STATE.None)
			{
				return;
			}
			nKCWarfareGameItem = NKCWarfareGameItem.GetNewInstance(m_Parent, index);
			m_itemList.Add(nKCWarfareGameItem);
		}
		nKCWarfareGameItem.transform.localPosition = pos;
		nKCWarfareGameItem.Set(state, bWithEnemy);
	}

	public void SetPos(int index, bool bWithEnemy)
	{
		NKCWarfareGameItem item = getItem(index);
		if (!(item == null))
		{
			item.SetPos(bWithEnemy);
		}
	}

	public bool IsItem(int index)
	{
		NKCWarfareGameItem item = getItem(index);
		if (item == null)
		{
			return false;
		}
		return item.State == WARFARE_ITEM_STATE.Item;
	}

	private NKCWarfareGameItem getItem(int index)
	{
		for (int i = 0; i < m_itemList.Count; i++)
		{
			if (m_itemList[i].Index == index)
			{
				return m_itemList[i];
			}
		}
		return null;
	}

	public Vector3 GetWorldPos(int index)
	{
		NKCWarfareGameItem item = getItem(index);
		if (item == null)
		{
			return Vector3.zero;
		}
		return item.GetWorldPos();
	}

	public void Close()
	{
		for (int i = 0; i < m_itemList.Count; i++)
		{
			m_itemList[i].Close();
		}
		m_itemList.Clear();
	}

	public void HideAll()
	{
		for (int i = 0; i < m_itemList.Count; i++)
		{
			_ = m_itemList[i];
			m_itemList[i].Set(WARFARE_ITEM_STATE.None, bWithEnemy: false);
		}
	}
}
