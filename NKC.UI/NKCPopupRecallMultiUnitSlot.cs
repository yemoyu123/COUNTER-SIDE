using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupRecallMultiUnitSlot : MonoBehaviour
{
	public delegate void CallBackFunc(int slotIdx);

	public GameObject m_objEmplty;

	public GameObject m_objOn;

	public NKCUIComStateButton m_csbtnEmplty;

	public NKCUISlot m_TargetUnitSlot;

	public NKCComTMPUIText m_lbSelectUnitCnt;

	public NKCUIComStateButton m_csbtnPlus;

	public NKCUIComStateButton m_csbtnMinus;

	public bool IsEmpleySlot;

	private int m_iSlotIndex;

	private int m_iUnitID;

	private int m_iUnitCnt;

	private CallBackFunc m_CallBackUnitSelect;

	private CallBackFunc m_CallBackUnitPlusCnt;

	private CallBackFunc m_CallBackUnitMinusCnt;

	public int UnitID => m_iUnitID;

	public int UnitCount => m_iUnitCnt;

	public void Init(int SlotIndex, CallBackFunc UnitSelect, CallBackFunc PlusUnitCnt, CallBackFunc MinusUnitCnt)
	{
		NKCUtil.SetBindFunction(m_csbtnPlus, OnClickPlus);
		NKCUtil.SetBindFunction(m_csbtnMinus, OnClickMinus);
		NKCUtil.SetBindFunction(m_csbtnEmplty, OnClickUnitSelect);
		m_TargetUnitSlot.Init();
		m_iSlotIndex = SlotIndex;
		m_CallBackUnitSelect = UnitSelect;
		m_CallBackUnitPlusCnt = PlusUnitCnt;
		m_CallBackUnitMinusCnt = MinusUnitCnt;
		IsEmpleySlot = true;
	}

	public void SetEmpty()
	{
		IsEmpleySlot = true;
		NKCUtil.SetGameobjectActive(m_objOn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEmplty, bValue: true);
	}

	public void SetUnitData(int UnitID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(UnitID);
		if (unitTempletBase != null)
		{
			NKCUtil.SetGameobjectActive(m_objOn, bValue: true);
			NKCUtil.SetGameobjectActive(m_objEmplty, bValue: false);
			IsEmpleySlot = false;
			m_TargetUnitSlot.SetUnitData(UnitID, 1, 0, bShowName: false, bShowLevel: false, bEnableLayoutElement: false, OnClickSlot);
			m_iUnitID = UnitID;
			m_iUnitCnt = 1;
			NKCUtil.SetLabelText(m_lbSelectUnitCnt, m_iUnitCnt.ToString());
		}
	}

	public void SetUnitCount(int iUnitCnt)
	{
		m_iUnitCnt = iUnitCnt;
		NKCUtil.SetLabelText(m_lbSelectUnitCnt, m_iUnitCnt.ToString());
	}

	public void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnClickUnitSelect();
	}

	private void OnClickUnitSelect()
	{
		m_CallBackUnitSelect?.Invoke(m_iSlotIndex);
	}

	private void OnClickPlus()
	{
		m_CallBackUnitPlusCnt?.Invoke(m_iSlotIndex);
	}

	private void OnClickMinus()
	{
		if (m_iUnitCnt != 1)
		{
			m_CallBackUnitMinusCnt?.Invoke(m_iSlotIndex);
		}
	}
}
