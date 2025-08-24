using System;
using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISeasonPoint : MonoBehaviour
{
	public Text m_lbSeasonDate;

	public Text m_lbPointName;

	public Text m_lbCurPoint;

	public Text m_lbDesc;

	public LoopScrollRect m_loop;

	public Transform m_trContent;

	public Transform m_trObjPool;

	public NKCUISeasonPointSlot m_pfbSlot;

	private List<NKCUISeasonPointSlot.SeasonPointSlotData> m_lstSlotData = new List<NKCUISeasonPointSlot.SeasonPointSlotData>();

	private Stack<NKCUISeasonPointSlot> m_stkSlot = new Stack<NKCUISeasonPointSlot>();

	private int m_myScore;

	private int m_receivedPoint;

	private NKCUISeasonPointSlot.OnClickSlot m_dOnClickSlot;

	private DateTime m_endDateUTC;

	private bool m_bUseFixedDuration;

	private float m_fDeltaTime;

	public void Init(bool bUseFixedDuration)
	{
		m_bUseFixedDuration = bUseFixedDuration;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private RectTransform GetObject(int idx)
	{
		NKCUISeasonPointSlot nKCUISeasonPointSlot = null;
		nKCUISeasonPointSlot = ((m_stkSlot.Count <= 0) ? UnityEngine.Object.Instantiate(m_pfbSlot) : m_stkSlot.Pop());
		if (nKCUISeasonPointSlot == null)
		{
			return null;
		}
		NKCUtil.SetGameobjectActive(nKCUISeasonPointSlot, bValue: true);
		nKCUISeasonPointSlot.transform.SetParent(m_trContent);
		return nKCUISeasonPointSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUISeasonPointSlot component = tr.GetComponent<NKCUISeasonPointSlot>();
		if (!(component == null))
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			component.transform.SetParent(m_trObjPool);
			m_stkSlot.Push(component);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUISeasonPointSlot component = tr.GetComponent<NKCUISeasonPointSlot>();
		if (component == null || m_lstSlotData.Count <= idx)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		float gaugeProgress = 0f;
		bool flag = m_lstSlotData.Count == idx + 1;
		if (idx == 0)
		{
			gaugeProgress = (float)m_myScore / (float)m_lstSlotData[idx + 1].SlotPoint;
			component.SetData(m_lstSlotData[idx], m_myScore, m_receivedPoint, flag, OnClickSlot);
			component.SetGaugeProgress(gaugeProgress);
			return;
		}
		component.SetData(m_lstSlotData[idx], m_myScore, m_receivedPoint, flag, OnClickSlot);
		if (!flag)
		{
			if (idx == 0)
			{
				gaugeProgress = (float)m_myScore / (float)m_lstSlotData[idx].SlotPoint;
			}
			else if (m_lstSlotData.Count > idx && m_myScore >= m_lstSlotData[idx].SlotPoint)
			{
				gaugeProgress = (float)(m_myScore - m_lstSlotData[idx].SlotPoint) / (float)(m_lstSlotData[idx + 1].SlotPoint - m_lstSlotData[idx].SlotPoint);
			}
		}
		component.SetGaugeProgress(gaugeProgress);
	}

	public void Open(List<NKCUISeasonPointSlot.SeasonPointSlotData> lstSlotData, string pointName, int myScore, int receivedPoint, NKMIntervalTemplet intervalTemplet, NKCUISeasonPointSlot.OnClickSlot dOnClickSlot)
	{
		if (lstSlotData == null || lstSlotData.Count == 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_myScore = myScore;
		m_lstSlotData = lstSlotData;
		m_receivedPoint = receivedPoint;
		m_dOnClickSlot = dOnClickSlot;
		m_endDateUTC = intervalTemplet.GetEndDateUtc();
		NKCUtil.SetLabelText(m_lbPointName, pointName);
		NKCUtil.SetLabelText(m_lbCurPoint, myScore.ToString("N0"));
		if (!m_bUseFixedDuration)
		{
			SetRemainTime();
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSeasonDate, NKCUtilString.GetTimeIntervalString(intervalTemplet.StartDate, intervalTemplet.EndDate, NKMTime.INTERVAL_FROM_UTC, bDateOnly: true));
		}
		m_loop.TotalCount = m_lstSlotData.Count;
		m_loop.SetIndexPosition(GetCurrentIndex());
	}

	private void SetRemainTime()
	{
		NKCUtil.SetLabelText(m_lbSeasonDate, NKCUtilString.GetRemainTimeStringEx(m_endDateUTC));
	}

	private int GetCurrentIndex()
	{
		int num = m_lstSlotData.FindIndex(0, (NKCUISeasonPointSlot.SeasonPointSlotData x) => x.SlotPoint > m_receivedPoint);
		if (num < 0 && m_receivedPoint > 0)
		{
			num = m_lstSlotData.Count - 1;
		}
		return num;
	}

	public void Refresh(int myScore, int receivedPoint)
	{
		m_myScore = myScore;
		m_receivedPoint = receivedPoint;
		NKCUtil.SetLabelText(m_lbCurPoint, myScore.ToString("N0"));
		m_loop.TotalCount = m_lstSlotData.Count;
		m_loop.SetIndexPosition(GetCurrentIndex());
	}

	private void OnClickSlot(NKCUISeasonPointSlot.SeasonPointSlotData seasonSlotData)
	{
		m_dOnClickSlot?.Invoke(seasonSlotData);
	}

	private void Update()
	{
		if (!m_bUseFixedDuration)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRemainTime();
			}
		}
	}
}
