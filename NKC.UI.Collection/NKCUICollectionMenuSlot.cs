using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionMenuSlot : MonoBehaviour, ICollectionMenuButton
{
	public Text m_nameOn;

	public Image m_iconOn;

	public Text m_nameOff;

	public Image m_iconOff;

	public NKCUIComToggle m_Toggle;

	public GameObject m_RedDot;

	private List<NKCUICollectionMenuSubSlot> m_lstSubSlot = new List<NKCUICollectionMenuSubSlot>();

	private UnityAction m_dOnCallBack;

	public List<NKCUICollectionMenuSubSlot> SubSlotList => m_lstSubSlot;

	public void Init(NKCUIComToggleGroup toggleGroup, string name, Sprite icon)
	{
		NKCUtil.SetLabelText(m_nameOn, name);
		NKCUtil.SetLabelText(m_nameOff, name);
		NKCUtil.SetImageSprite(m_iconOn, icon);
		NKCUtil.SetImageSprite(m_iconOff, icon);
		NKCUtil.SetGameobjectActive(m_RedDot, bValue: false);
		m_Toggle.OnValueChanged.AddListener(OnValueChange);
		m_Toggle.SetToggleGroup(toggleGroup);
	}

	public void SetCallBackFunction(UnityAction callback)
	{
		m_dOnCallBack = callback;
	}

	public void AddSubSlot(NKCUICollectionMenuSubSlot child)
	{
		child.m_Toggle.SetToggleGroup(base.gameObject.GetComponent<NKCUIComToggleGroup>());
		child.SetOnRedDotChangedFunc(OnSubSlotRedDotChanged);
		m_lstSubSlot.Add(child);
	}

	public void SetRedDotActive(bool value)
	{
		NKCUtil.SetGameobjectActive(m_RedDot, value);
	}

	public NKCUIComToggle GetToggle()
	{
		return m_Toggle;
	}

	public void OnActiveChild(bool bActive, bool skipSubButtonAni = false)
	{
		foreach (NKCUICollectionMenuSubSlot item in m_lstSubSlot)
		{
			if (!(item == null))
			{
				item.SetActive(bActive, skipSubButtonAni);
			}
		}
	}

	private void OnValueChange(bool bVal)
	{
		if (bVal && m_dOnCallBack != null)
		{
			m_dOnCallBack();
			return;
		}
		OnActiveChild(bVal);
		bool flag = false;
		foreach (NKCUICollectionMenuSubSlot item in m_lstSubSlot)
		{
			if (!(item == null) && bVal && item.m_Toggle.IsSelected)
			{
				item.m_Toggle.Select(bSelect: false, bForce: true);
				item.m_Toggle.Select(bSelect: true);
				flag = true;
			}
		}
		if (bVal && !flag && m_lstSubSlot.Count > 0)
		{
			m_lstSubSlot[0].m_Toggle.Select(bSelect: true);
		}
	}

	private void OnSubSlotRedDotChanged()
	{
		bool bValue = false;
		if (m_lstSubSlot != null)
		{
			int count = m_lstSubSlot.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_lstSubSlot[i].IsRedDotOn)
				{
					bValue = true;
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_RedDot, bValue);
	}

	private void OnDestroy()
	{
		m_lstSubSlot?.Clear();
		m_lstSubSlot = null;
		m_dOnCallBack = null;
	}
}
