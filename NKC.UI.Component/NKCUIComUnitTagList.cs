using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIComUnitTagList : MonoBehaviour
{
	public NKCUIComUnitTagListSlot m_pfbSlot;

	public RectTransform m_rtSlotParent;

	public ScrollRect m_ScrollRect;

	public bool m_bOpenUnitRoleInfoOnClick = true;

	private List<NKCUIComUnitTagListSlot> m_lstSlot = new List<NKCUIComUnitTagListSlot>();

	private NKMUnitTempletBase m_Templet;

	public void SetData(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			Hide();
		}
		else
		{
			SetData(NKMUnitManager.GetUnitTempletBase(unitData));
		}
	}

	public void SetData(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase == null || unitTempletBase.m_lstUnitTag == null || unitTempletBase.m_lstUnitTag.Count == 0)
		{
			Hide();
		}
		else
		{
			if (m_rtSlotParent == null)
			{
				return;
			}
			m_Templet = unitTempletBase;
			int num = 0;
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			if (m_ScrollRect != null)
			{
				m_ScrollRect.horizontalNormalizedPosition = 0f;
				m_ScrollRect.verticalNormalizedPosition = 0f;
			}
			if (unitTempletBase.m_lstUnitTag != null)
			{
				while (m_lstSlot.Count < unitTempletBase.m_lstUnitTag.Count)
				{
					m_lstSlot.Add(GetNewSlot());
				}
				for (num = 0; num < unitTempletBase.m_lstUnitTag.Count; num++)
				{
					m_lstSlot[num].SetData(unitTempletBase.m_lstUnitTag[num], OpenUnitRoleInfo);
					NKCUtil.SetGameobjectActive(m_lstSlot[num], bValue: true);
				}
				for (; num < m_lstSlot.Count; num++)
				{
					NKCUtil.SetGameobjectActive(m_lstSlot[num], bValue: false);
				}
			}
		}
	}

	private NKCUIComUnitTagListSlot GetNewSlot()
	{
		NKCUIComUnitTagListSlot nKCUIComUnitTagListSlot = Object.Instantiate(m_pfbSlot);
		nKCUIComUnitTagListSlot.transform.SetParent(m_rtSlotParent);
		return nKCUIComUnitTagListSlot;
	}

	private void Hide()
	{
		m_Templet = null;
		foreach (NKCUIComUnitTagListSlot item in m_lstSlot)
		{
			NKCUtil.SetGameobjectActive(item, bValue: false);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OpenUnitRoleInfo()
	{
		if (m_bOpenUnitRoleInfoOnClick)
		{
			NKCPopupUnitRoleInfo.Instance.OpenPopup(m_Templet, NKCPopupUnitRoleInfo.Page.TAG);
		}
	}
}
