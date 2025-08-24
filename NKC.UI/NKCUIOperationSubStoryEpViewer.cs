using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI;

public class NKCUIOperationSubStoryEpViewer : MonoBehaviour
{
	public List<NKCUIOperationSubStorySlot> m_lstSlot = new List<NKCUIOperationSubStorySlot>();

	public List<NKCUIOperationSubStorySlot> GetEpList()
	{
		return m_lstSlot;
	}

	public void InitUI()
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].InitUI();
		}
	}

	public void SetData()
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].SetData();
		}
	}

	public void Refresh()
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].Refresh();
		}
	}
}
