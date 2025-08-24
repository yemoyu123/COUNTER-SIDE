using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationRelateEpList : MonoBehaviour
{
	public NKCUIComToggle m_tglDropDown;

	public ScrollRect m_srDropDown;

	public NKCUIOperationRelateEpListSlot m_pfbDropDownSlot;

	private Stack<NKCUIOperationRelateEpListSlot> m_stkRelateSlot = new Stack<NKCUIOperationRelateEpListSlot>();

	private List<NKCUIOperationRelateEpListSlot> m_lstVisibleRelateSlot = new List<NKCUIOperationRelateEpListSlot>();

	private List<NKMEpisodeTempletV2> m_lstEpTempletDropDown = new List<NKMEpisodeTempletV2>();

	public void InitUI()
	{
		m_tglDropDown.OnValueChanged.RemoveAllListeners();
		m_tglDropDown.OnValueChanged.AddListener(OnClickDropDown);
	}

	public void SetData(NKMEpisodeTempletV2 epTemplet)
	{
		if (epTemplet == null || epTemplet.m_lstConnect_EpisodeID.Count <= 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_tglDropDown.Select(bSelect: false, bForce: true);
		ResetRelateSlot();
		List<int> lstConnect_EpisodeID = epTemplet.m_lstConnect_EpisodeID;
		NKCUtil.SetGameobjectActive(m_tglDropDown, lstConnect_EpisodeID.Count > 0);
		if (!m_tglDropDown.gameObject.activeSelf)
		{
			return;
		}
		lstConnect_EpisodeID.Sort();
		int num = 0;
		for (int i = 0; i < lstConnect_EpisodeID.Count; i++)
		{
			NKCUIOperationRelateEpListSlot relateSlot = GetRelateSlot();
			NKCUtil.SetGameobjectActive(relateSlot, bValue: true);
			m_lstVisibleRelateSlot.Add(relateSlot);
			if (relateSlot.SetData(lstConnect_EpisodeID[i]))
			{
				num++;
			}
			relateSlot.transform.SetAsLastSibling();
		}
		if (num > 0)
		{
			m_srDropDown.normalizedPosition = Vector2.zero;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_tglDropDown, bValue: false);
		}
	}

	private NKCUIOperationRelateEpListSlot GetRelateSlot()
	{
		if (m_stkRelateSlot.Count > 0)
		{
			return m_stkRelateSlot.Pop();
		}
		return Object.Instantiate(m_pfbDropDownSlot, m_srDropDown.content);
	}

	private void ResetRelateSlot()
	{
		for (int i = 0; i < m_lstVisibleRelateSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstVisibleRelateSlot[i], bValue: false);
			m_stkRelateSlot.Push(m_lstVisibleRelateSlot[i]);
		}
		m_lstVisibleRelateSlot.Clear();
	}

	private void OnClickDropDown(bool bValue)
	{
	}
}
