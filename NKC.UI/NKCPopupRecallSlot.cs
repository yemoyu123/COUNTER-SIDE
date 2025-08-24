using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupRecallSlot : MonoBehaviour
{
	public Text m_lbTitle;

	public GameObject m_objEmpty;

	public List<NKCUISlot> m_lstSlot = new List<NKCUISlot>();

	public void SetData(string title, List<NKCUISlot.SlotData> lstSlotData)
	{
		NKCUtil.SetLabelText(m_lbTitle, title);
		if (lstSlotData == null || lstSlotData.Count <= 0)
		{
			for (int i = 0; i < m_lstSlot.Count; i++)
			{
				NKCUtil.SetGameobjectActive(m_lstSlot[i].gameObject, bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: true);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
		for (int j = 0; j < m_lstSlot.Count; j++)
		{
			if (j < lstSlotData.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstSlot[j], bValue: true);
				m_lstSlot[j].SetMiscItemData(lstSlotData[j], bShowName: false, bShowCount: true, bEnableLayoutElement: false, null);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstSlot[j], bValue: false);
			}
		}
	}
}
