using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guide;

public class NKCUIPopupGuideSlot : MonoBehaviour
{
	public Text m_TEXT;

	public NKCUIComToggle m_Toggle;

	private List<NKCUIPopupGuideSubSlot> m_lstSubSlot = new List<NKCUIPopupGuideSubSlot>();

	public void Init(string title, NKCUIComToggleGroup toggleGroup)
	{
		m_Toggle.OnValueChanged.AddListener(OnValueChange);
		NKCUtil.SetLabelText(m_TEXT, title);
		m_Toggle.SetToggleGroup(toggleGroup);
	}

	public void OnValueChange(bool bVal)
	{
		Color col = (bVal ? NKCUtil.GetColor("#011B3B") : NKCUtil.GetColor("#FFFFFF"));
		NKCUtil.SetLabelTextColor(m_TEXT, col);
		OnActiveChild(bVal);
	}

	public void AddSubSlot(NKCUIPopupGuideSubSlot child)
	{
		m_lstSubSlot.Add(child);
	}

	public bool SelectSubSlot(string ARTICLE_ID)
	{
		bool flag = false;
		foreach (NKCUIPopupGuideSubSlot item in m_lstSubSlot)
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
		foreach (NKCUIPopupGuideSubSlot item in m_lstSubSlot)
		{
			item.OnActive(bActive);
		}
	}

	public bool HasChild(string ARTICLE_ID)
	{
		for (int i = 0; i < m_lstSubSlot.Count; i++)
		{
			if (string.Equals(ARTICLE_ID, m_lstSubSlot[i].ARTICLE_ID))
			{
				return true;
			}
		}
		return false;
	}

	public void Clear()
	{
		foreach (NKCUIPopupGuideSubSlot item in m_lstSubSlot)
		{
			Object.Destroy(item.gameObject);
		}
		m_lstSubSlot.Clear();
	}
}
