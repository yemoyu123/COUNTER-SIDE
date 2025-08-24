using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCWarfareGameLabelMgr
{
	private Transform m_labelParent;

	private List<NKCWarfareGameLabel> m_labelList = new List<NKCWarfareGameLabel>();

	public NKCWarfareGameLabelMgr(Transform labelParent)
	{
		m_labelParent = labelParent;
	}

	public void SetLabel(int tileIndex, WARFARE_LABEL_TYPE labelType, Vector3 pos)
	{
		NKCWarfareGameLabel nKCWarfareGameLabel = getLabel(tileIndex);
		if (nKCWarfareGameLabel == null)
		{
			for (int i = 0; i < m_labelList.Count; i++)
			{
				if (!m_labelList[i].gameObject.activeSelf)
				{
					nKCWarfareGameLabel = m_labelList[i];
					break;
				}
			}
		}
		if (nKCWarfareGameLabel == null)
		{
			nKCWarfareGameLabel = NKCWarfareGameLabel.GetNewInstance(m_labelParent);
			m_labelList.Add(nKCWarfareGameLabel);
		}
		nKCWarfareGameLabel.SetWFLabelType(tileIndex, labelType);
		nKCWarfareGameLabel.transform.localPosition = pos;
	}

	public void SetText(int index, int count)
	{
		NKCWarfareGameLabel label = getLabel(index);
		if (label != null)
		{
			label.SetWFLabelCount(count);
		}
	}

	public void HideAll()
	{
		for (int i = 0; i < m_labelList.Count; i++)
		{
			m_labelList[i].Hide();
		}
	}

	private NKCWarfareGameLabel getLabel(int index)
	{
		for (int i = 0; i < m_labelList.Count; i++)
		{
			if (m_labelList[i].Index == index)
			{
				return m_labelList[i];
			}
		}
		return null;
	}
}
