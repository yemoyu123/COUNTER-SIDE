using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guide;

public class NKCUIPopupGuideStatusInfo : MonoBehaviour, IGuideSubPage
{
	public enum Type
	{
		Buff,
		Debuff,
		CC
	}

	public Type m_eType;

	public LoopScrollRect m_LoopScroll;

	public NKCUIStatInfoSlot m_pfbSlot;

	private bool m_bInit;

	private List<NKMUnitStatusTemplet> m_lstData;

	private List<NKCUIStatInfoSlot> m_lstVisibleSlot = new List<NKCUIStatInfoSlot>();

	private Stack<NKCUIStatInfoSlot> m_stkSlotPool = new Stack<NKCUIStatInfoSlot>();

	public void SetData()
	{
		if (!m_bInit)
		{
			Init();
			if (m_lstData != null && m_lstData.Count > 0)
			{
				m_LoopScroll.TotalCount = m_lstData.Count;
				m_LoopScroll.PrepareCells();
				m_LoopScroll.SetIndexPosition(0);
				m_LoopScroll.RefreshCells(bForce: true);
			}
		}
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
		switch (m_eType)
		{
		case Type.Buff:
			m_lstData = NKMTempletContainer<NKMUnitStatusTemplet>.Values.Where((NKMUnitStatusTemplet x) => x.m_bShowGuide && !x.m_bCrowdControl && !x.m_bDebuff).ToList();
			break;
		case Type.Debuff:
			m_lstData = NKMTempletContainer<NKMUnitStatusTemplet>.Values.Where((NKMUnitStatusTemplet x) => x.m_bShowGuide && !x.m_bCrowdControl && x.m_bDebuff).ToList();
			break;
		case Type.CC:
			m_lstData = NKMTempletContainer<NKMUnitStatusTemplet>.Values.Where((NKMUnitStatusTemplet x) => x.m_bShowGuide && x.m_bCrowdControl).ToList();
			break;
		}
		m_lstData.Sort(Comparer);
	}

	private int Comparer(NKMUnitStatusTemplet a, NKMUnitStatusTemplet b)
	{
		return a.m_sortIndex.CompareTo(b.m_sortIndex);
	}

	public void Clear()
	{
		foreach (NKCUIStatInfoSlot item in m_lstVisibleSlot)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (NKCUIStatInfoSlot item2 in m_stkSlotPool)
		{
			Object.Destroy(item2.gameObject);
		}
		m_lstVisibleSlot.Clear();
		m_stkSlotPool.Clear();
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
		NKCUIStatInfoSlot nKCUIStatInfoSlot2 = Object.Instantiate(m_pfbSlot);
		nKCUIStatInfoSlot2.transform.SetParent(m_LoopScroll.content);
		m_lstVisibleSlot.Add(nKCUIStatInfoSlot2);
		NKCUtil.SetGameobjectActive(nKCUIStatInfoSlot2, bValue: false);
		return nKCUIStatInfoSlot2.GetComponent<RectTransform>();
	}

	public void ReturnContentObject(Transform tr)
	{
		if (tr.TryGetComponent<NKCUIStatInfoSlot>(out var component))
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			m_lstVisibleSlot.Remove(component);
			m_stkSlotPool.Push(component);
			tr.SetParent(base.transform, worldPositionStays: false);
		}
	}

	public void ProvideContentData(Transform tr, int idx)
	{
		if (tr.TryGetComponent<NKCUIStatInfoSlot>(out var component) && m_lstData.Count > idx)
		{
			NKMUnitStatusTemplet nKMUnitStatusTemplet = m_lstData[idx];
			component.SetData(NKCStringTable.GetString(nKMUnitStatusTemplet.m_StatusStrID), NKCStringTable.GetString(nKMUnitStatusTemplet.m_DescStrID));
		}
	}
}
