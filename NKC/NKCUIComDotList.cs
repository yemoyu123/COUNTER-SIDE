using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIComDotList : MonoBehaviour
{
	public List<Image> m_lstDots;

	public Color m_colSelected;

	public Color m_colBase;

	private int m_MaxCount = 1;

	public void SetIndex(int index)
	{
		for (int i = 0; i < m_lstDots.Count; i++)
		{
			Image image = m_lstDots[i];
			if (!(image == null))
			{
				image.color = ((i == index) ? m_colSelected : m_colBase);
			}
		}
	}

	public void SetMaxCount(int value)
	{
		if (value < 1)
		{
			return;
		}
		if (value > m_lstDots.Count)
		{
			while (value > m_lstDots.Count)
			{
				GameObject obj = Object.Instantiate(m_lstDots[0].gameObject);
				obj.transform.SetParent(m_lstDots[0].transform.parent);
				Image component = obj.GetComponent<Image>();
				m_lstDots.Add(component);
			}
			m_MaxCount = m_lstDots.Count;
		}
		else
		{
			m_MaxCount = value;
		}
		for (int i = 0; i < m_lstDots.Count; i++)
		{
			Image image = m_lstDots[i];
			if (!(image == null))
			{
				NKCUtil.SetGameobjectActive(image, i < m_MaxCount);
			}
		}
	}
}
