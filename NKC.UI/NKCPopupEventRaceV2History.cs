using System.Collections.Generic;
using ClientPacket.Event;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEventRaceV2History : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnClose;

	public LoopScrollRect m_LoopScrollRect;

	public NKCPopupEventRaceV2HistorySlot m_pfbHistorySlot;

	private List<NKMEventBetPrivateResult> m_lstBetPrivateResult = new List<NKMEventBetPrivateResult>();

	public void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnClose, OnClickClose);
		if (null != m_LoopScrollRect)
		{
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnSlot;
			m_LoopScrollRect.dOnProvideData += ProvideSlotData;
			m_LoopScrollRect.ContentConstraintCount = 1;
			m_LoopScrollRect.PrepareCells();
			m_LoopScrollRect.TotalCount = 0;
			m_LoopScrollRect.RefreshCells();
		}
	}

	public void Open()
	{
		List<NKMEventBetPrivateResult> listUserJoinHistory = NKCScenManager.CurrentUserData().GetRaceData().ListUserJoinHistory;
		if (listUserJoinHistory == null)
		{
			return;
		}
		m_lstBetPrivateResult.Clear();
		foreach (NKMEventBetPrivateResult item in listUserJoinHistory)
		{
			if (item != null)
			{
				m_lstBetPrivateResult.Add(item);
			}
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_LoopScrollRect.TotalCount = m_lstBetPrivateResult.Count;
		m_LoopScrollRect.SetIndexPosition(0);
	}

	private RectTransform GetSlot(int index)
	{
		NKCPopupEventRaceV2HistorySlot nKCPopupEventRaceV2HistorySlot = Object.Instantiate(m_pfbHistorySlot);
		if (null == nKCPopupEventRaceV2HistorySlot)
		{
			return null;
		}
		NKCUtil.SetGameobjectActive(nKCPopupEventRaceV2HistorySlot, bValue: true);
		nKCPopupEventRaceV2HistorySlot.transform.localScale = Vector3.one;
		return nKCPopupEventRaceV2HistorySlot.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		go.GetComponent<NKCPopupEventRaceV2HistorySlot>();
		Object.Destroy(go);
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		if (m_lstBetPrivateResult.Count > idx)
		{
			NKCPopupEventRaceV2HistorySlot component = tr.GetComponent<NKCPopupEventRaceV2HistorySlot>();
			if (!(component == null))
			{
				component.SetData(m_lstBetPrivateResult[idx]);
			}
		}
	}

	private void OnClickClose()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Close()
	{
		OnClickClose();
	}
}
