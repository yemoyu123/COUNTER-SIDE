using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guide;

public class NKCUIPopupGuideStatInfo : MonoBehaviour, IGuideSubPage
{
	public LoopVerticalScrollFlexibleRect m_LoopScroll;

	public STAT_TYPE m_Type;

	public Text m_Title;

	private bool m_bInit;

	private List<(NKCStatInfoTemplet, bool)> m_lstData;

	private List<NKCUIStatInfoSlot> m_lstVisibleSlot = new List<NKCUIStatInfoSlot>();

	private Stack<NKCUIStatInfoSlot> m_stkSlotPool = new Stack<NKCUIStatInfoSlot>();

	public void SetData()
	{
		if (!m_bInit)
		{
			Init();
			if (m_lstData != null && m_lstData.Count > 0)
			{
				NKCUtil.SetLabelText(m_Title, NKCStringTable.GetString(m_lstData[0].Item1.Stat_Category_Name));
				m_LoopScroll.TotalCount = m_lstData.Count;
				m_LoopScroll.PrepareCells();
				m_LoopScroll.SetIndexPosition(0);
				m_LoopScroll.RefreshCells(bForce: true);
			}
		}
	}

	public void Clear()
	{
		foreach (NKCUIStatInfoSlot item in m_lstVisibleSlot)
		{
			item?.DestoryInstance();
		}
		foreach (NKCUIStatInfoSlot item2 in m_stkSlotPool)
		{
			item2?.DestoryInstance();
		}
		m_lstVisibleSlot.Clear();
		m_stkSlotPool.Clear();
	}

	private void Init()
	{
		m_bInit = true;
		if (m_LoopScroll != null)
		{
			m_LoopScroll.dOnGetObject += GetContentObject;
			m_LoopScroll.dOnReturnObject += ReturnContentObject;
			m_LoopScroll.dOnProvideData += ProvideContentData;
		}
		m_lstData = new List<(NKCStatInfoTemplet, bool)>();
		foreach (List<NKCStatInfoTemplet> value in NKCStatInfoTemplet.Groups.Values)
		{
			if (value.Count <= 0 || value[0].Category_Type != m_Type)
			{
				continue;
			}
			{
				foreach (NKCStatInfoTemplet item in value)
				{
					if (item.m_bShowGuide)
					{
						m_lstData.Add((item, true));
						if (!string.IsNullOrEmpty(item.Stat_Negative_Name) && !string.IsNullOrEmpty(item.Stat_Negative_DESC))
						{
							m_lstData.Add((item, false));
						}
					}
				}
				break;
			}
		}
	}

	public RectTransform GetContentObject(int index)
	{
		if (m_stkSlotPool.Count > 0)
		{
			NKCUIStatInfoSlot nKCUIStatInfoSlot = m_stkSlotPool.Pop();
			nKCUIStatInfoSlot.transform.SetParent(m_LoopScroll.content);
			m_lstVisibleSlot.Add(nKCUIStatInfoSlot);
			NKCUtil.SetGameobjectActive(nKCUIStatInfoSlot, bValue: false);
			return nKCUIStatInfoSlot.GetComponent<RectTransform>();
		}
		NKCUIStatInfoSlot newInstance = NKCUIStatInfoSlot.GetNewInstance(m_LoopScroll.content.transform);
		newInstance.transform.SetParent(m_LoopScroll.content);
		m_lstVisibleSlot.Add(newInstance);
		NKCUtil.SetGameobjectActive(newInstance, bValue: false);
		return newInstance.GetComponent<RectTransform>();
	}

	public void ReturnContentObject(Transform tr)
	{
		NKCUIStatInfoSlot component = tr.GetComponent<NKCUIStatInfoSlot>();
		if (component != null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			m_lstVisibleSlot.Remove(component);
			m_stkSlotPool.Push(component);
			tr.SetParent(base.transform, worldPositionStays: false);
		}
	}

	public void ProvideContentData(Transform tr, int idx)
	{
		NKCUIStatInfoSlot component = tr.GetComponent<NKCUIStatInfoSlot>();
		if (!(component == null) && m_lstData.Count > idx)
		{
			NKCStatInfoTemplet item = m_lstData[idx].Item1;
			if (m_lstData[idx].Item2)
			{
				component.SetData(NKCStringTable.GetString(item.Stat_Name), NKCStringTable.GetString(item.Stat_Desc));
			}
			else
			{
				component.SetData(NKCStringTable.GetString(item.Stat_Negative_Name), NKCStringTable.GetString(item.Stat_Negative_DESC));
			}
		}
	}
}
