using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Fierce;

public class NKCUIFierceBattleSelfPenaltyContent : MonoBehaviour
{
	public NKCUIFierceBattleSelfPenaltySlot m_pfbSlot;

	public NKCComText m_lbTitle;

	public NKCComText m_lbDesc;

	public RectTransform m_rtParent;

	private OnClickPenalty m_OnClick;

	private List<NKCUIFierceBattleSelfPenaltySlot> m_lstChildSlots = new List<NKCUIFierceBattleSelfPenaltySlot>();

	private int m_iPerSelectedPenaltyID;

	private int m_iPenaltyGroupID;

	public int PenaltyGroupID => m_iPenaltyGroupID;

	public void Init(RectTransform rtParent)
	{
		base.gameObject.transform.SetParent(rtParent);
	}

	public void SetData(List<NKMFiercePenaltyTemplet> lstPenaltys, OnClickPenalty dSlotClick)
	{
		if (lstPenaltys.Count <= 0)
		{
			return;
		}
		for (int i = 0; lstPenaltys.Count > i; i++)
		{
			if (i == 0)
			{
				string msg = NKCStringTable.GetString(lstPenaltys[i].PenaltyGroupName);
				NKCUtil.SetLabelText(m_lbTitle, msg);
				string msg2 = NKCStringTable.GetString(lstPenaltys[i].PenaltyGroupDesc);
				NKCUtil.SetLabelText(m_lbDesc, msg2);
				m_iPenaltyGroupID = lstPenaltys[i].PenaltyGroupID;
			}
			if (null != m_pfbSlot)
			{
				NKCUIFierceBattleSelfPenaltySlot nKCUIFierceBattleSelfPenaltySlot = Object.Instantiate(m_pfbSlot);
				if (null != nKCUIFierceBattleSelfPenaltySlot)
				{
					nKCUIFierceBattleSelfPenaltySlot.gameObject.transform.SetParent(m_rtParent);
					nKCUIFierceBattleSelfPenaltySlot.Init();
					nKCUIFierceBattleSelfPenaltySlot.SetData(lstPenaltys[i], OnClickChildSlot);
					m_lstChildSlots.Add(nKCUIFierceBattleSelfPenaltySlot);
				}
			}
		}
		m_OnClick = dSlotClick;
	}

	private void OnClickChildSlot(NKMFiercePenaltyTemplet penaltyTempet)
	{
		if (m_iPerSelectedPenaltyID == penaltyTempet.Key)
		{
			foreach (NKCUIFierceBattleSelfPenaltySlot lstChildSlot in m_lstChildSlots)
			{
				lstChildSlot.Select(bSelect: false);
				lstChildSlot.Disable(bDisable: false);
			}
			m_iPerSelectedPenaltyID = 0;
		}
		else
		{
			foreach (NKCUIFierceBattleSelfPenaltySlot lstChildSlot2 in m_lstChildSlots)
			{
				if (lstChildSlot2.TempletData.Key == penaltyTempet.Key)
				{
					lstChildSlot2.Select(bSelect: true);
					lstChildSlot2.Disable(bDisable: false);
				}
				else
				{
					lstChildSlot2.Select(bSelect: false);
					lstChildSlot2.Disable(bDisable: true);
				}
			}
			m_iPerSelectedPenaltyID = penaltyTempet.Key;
		}
		m_OnClick?.Invoke(penaltyTempet);
	}

	public void SelectChildSlot(int PenaltyKey)
	{
		foreach (NKCUIFierceBattleSelfPenaltySlot lstChildSlot in m_lstChildSlots)
		{
			if (lstChildSlot.TempletData.Key == PenaltyKey)
			{
				lstChildSlot.Select(bSelect: true);
				lstChildSlot.Disable(bDisable: false);
				m_iPerSelectedPenaltyID = PenaltyKey;
			}
			else
			{
				lstChildSlot.Select(bSelect: false);
				lstChildSlot.Disable(bDisable: true);
			}
		}
	}

	public void UnCheckChildSlots()
	{
		foreach (NKCUIFierceBattleSelfPenaltySlot lstChildSlot in m_lstChildSlots)
		{
			lstChildSlot.Select(bSelect: false);
			lstChildSlot.Disable(bDisable: false);
		}
		m_iPerSelectedPenaltyID = 0;
	}

	public void Clear()
	{
		for (int i = 0; i < m_lstChildSlots.Count; i++)
		{
			Object.Destroy(m_lstChildSlots[i]);
			m_lstChildSlots[i] = null;
		}
		m_lstChildSlots.Clear();
	}
}
