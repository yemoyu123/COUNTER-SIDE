using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC;

public class NKCSideMenuSlot : MonoBehaviour
{
	public Text m_TEXT;

	public NKCUIComToggle m_Toggle;

	public GameObject m_LOCK;

	public GameObject m_CLEAR;

	private List<NKCSideMenuSlotChild> m_lstSubSlot = new List<NKCSideMenuSlotChild>();

	private List<NKCUISkinSlot> m_lstSubSlotSkin = new List<NKCUISkinSlot>();

	private UnityAction dOnCallBack;

	public void Init(string title, NKCUIComToggleGroup toggleGroup, RectTransform rtParent)
	{
		base.gameObject.GetComponent<RectTransform>().SetParent(rtParent);
		m_Toggle.OnValueChanged.AddListener(OnValueChange);
		NKCUtil.SetLabelText(m_TEXT, title);
		m_Toggle.SetToggleGroup(toggleGroup);
	}

	public void SetCallBackFunction(UnityAction callback)
	{
		dOnCallBack = callback;
	}

	public void OnValueChange(bool bVal)
	{
		Color col = (bVal ? NKCUtil.GetColor("#011B3B") : NKCUtil.GetColor("#FFFFFF"));
		NKCUtil.SetLabelTextColor(m_TEXT, col);
		OnActiveChild(bVal);
	}

	public void AddSubSlot(NKCSideMenuSlotChild child)
	{
		child.m_Toggle.SetToggleGroup(base.gameObject.GetComponent<NKCUIComToggleGroup>());
		m_lstSubSlot.Add(child);
	}

	public void AddSubSlot(NKCUISkinSlot child)
	{
		m_lstSubSlotSkin.Add(child);
	}

	public bool SelectSubSlot(string ARTICLE_ID)
	{
		bool flag = false;
		foreach (NKCSideMenuSlotChild item in m_lstSubSlot)
		{
			bool flag2 = item.OnSelected(ARTICLE_ID);
			if (!flag)
			{
				flag = flag2;
			}
		}
		m_Toggle.Select(flag);
		return flag;
	}

	private void OnActiveChild(bool bActive)
	{
		if (bActive && dOnCallBack != null)
		{
			dOnCallBack();
			return;
		}
		foreach (NKCSideMenuSlotChild item in m_lstSubSlot)
		{
			item.OnActive(bActive);
		}
	}

	public bool HasChild(string key)
	{
		for (int i = 0; i < m_lstSubSlot.Count; i++)
		{
			if (string.Equals(key, m_lstSubSlot[i].KEY))
			{
				return true;
			}
		}
		return false;
	}

	public void NotifySelectID(string key)
	{
		for (int i = 0; i < m_lstSubSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstSubSlot[i].m_SELECTED, string.Equals(key, m_lstSubSlot[i].KEY));
			m_lstSubSlot[i].m_Toggle.Select(string.Equals(key, m_lstSubSlot[i].KEY), bForce: true);
		}
	}

	public void Clear()
	{
		foreach (NKCSideMenuSlotChild item in m_lstSubSlot)
		{
			Object.Destroy(item.gameObject);
		}
		m_lstSubSlot.Clear();
		foreach (NKCUISkinSlot item2 in m_lstSubSlotSkin)
		{
			Object.Destroy(item2.gameObject);
		}
		m_lstSubSlotSkin.Clear();
	}

	public void Lock()
	{
		if (m_Toggle != null)
		{
			m_Toggle.Lock();
		}
		NKCUtil.SetLabelTextColor(m_TEXT, NKCUtil.GetColor("#676767"));
	}

	public void UnLock()
	{
		if (m_Toggle != null)
		{
			m_Toggle.UnLock();
		}
		NKCUtil.SetLabelTextColor(m_TEXT, NKCUtil.GetColor("#FFFFFF"));
	}

	public void ForceSelect(bool select)
	{
		if (m_Toggle != null)
		{
			m_Toggle.Select(select);
		}
	}

	public void SetClear(bool value)
	{
		NKCUtil.SetGameobjectActive(m_CLEAR, value);
	}
}
