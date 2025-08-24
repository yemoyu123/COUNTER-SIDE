using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitSelectListRemovePopup : MonoBehaviour
{
	public delegate void OnRemoveUnits(HashSet<NKM_UNIT_GRADE> setUnitGrade, bool bSmart);

	[Header("희귀도 옵션")]
	public NKCUIComToggle m_ctglSSR;

	public NKCUIComToggle m_ctglSR;

	public NKCUIComToggle m_ctglR;

	public NKCUIComToggle m_ctglN;

	[Header("해고 옵션")]
	public GameObject m_ENABLE;

	public NKCUIComToggle m_ctglSmart;

	public NKCUIComToggle m_ctglAll;

	[Header("결정")]
	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	private OnRemoveUnits dOnRemoveUnits;

	private bool m_bInit;

	public RectTransform m_NKM_UI_POPUP_REMOVE_PANEL;

	public Text m_lbTitle;

	private float m_fRemovePanelOriginalPosY;

	private float m_fRemovePanelOriginalHeight;

	private float m_fRemovePanelPosYGap = 104.7f;

	private float m_fRemovePanelHeightForOperator = 490f;

	private bool m_bOperatorOption;

	public bool IsOpen => base.gameObject.activeSelf;

	private void Init()
	{
		if (!m_bInit)
		{
			m_ctglSSR.Select(bSelect: false, bForce: true);
			m_ctglSR.Select(bSelect: false, bForce: true);
			m_ctglR.Select(bSelect: true, bForce: true);
			m_ctglN.Select(bSelect: true, bForce: true);
			m_ctglSmart.Select(bSelect: true, bForce: true);
			NKCUtil.SetButtonClickDelegate(m_csbtnOK, OnOK);
			NKCUtil.SetButtonClickDelegate(m_csbtnCancel, Close);
			m_bInit = true;
			m_fRemovePanelOriginalPosY = m_NKM_UI_POPUP_REMOVE_PANEL.transform.position.y;
			m_fRemovePanelOriginalHeight = m_NKM_UI_POPUP_REMOVE_PANEL.GetHeight();
		}
	}

	public void Open(OnRemoveUnits onRemoveUnits, bool bOperator = false, bool bOperatorExtract = false)
	{
		Init();
		dOnRemoveUnits = onRemoveUnits;
		base.gameObject.SetActive(value: true);
		m_bOperatorOption = bOperator;
		NKCUtil.SetLabelText(m_lbTitle, (m_bOperatorOption && bOperatorExtract) ? NKCStringTable.GetString("SI_FILTER_SMART_EXTRACT_TITLE") : NKCStringTable.GetString("SI_FILTER_SMART_REMOVE_TITLE"));
		NKCUtil.SetGameobjectActive(m_ENABLE, !m_bOperatorOption);
		ChangePanelSize();
	}

	private void OnOK()
	{
		HashSet<NKM_UNIT_GRADE> hashSet = new HashSet<NKM_UNIT_GRADE>();
		if (m_ctglSSR != null && m_ctglSSR.m_bChecked)
		{
			hashSet.Add(NKM_UNIT_GRADE.NUG_SSR);
		}
		if (m_ctglSR != null && m_ctglSR.m_bChecked)
		{
			hashSet.Add(NKM_UNIT_GRADE.NUG_SR);
		}
		if (m_ctglR != null && m_ctglR.m_bChecked)
		{
			hashSet.Add(NKM_UNIT_GRADE.NUG_R);
		}
		if (m_ctglN != null && m_ctglN.m_bChecked)
		{
			hashSet.Add(NKM_UNIT_GRADE.NUG_N);
		}
		if (!m_bOperatorOption)
		{
			dOnRemoveUnits?.Invoke(hashSet, m_ctglSmart != null && m_ctglSmart.m_bChecked);
		}
		else
		{
			dOnRemoveUnits?.Invoke(hashSet, bSmart: false);
		}
		Close();
	}

	public void Close()
	{
		dOnRemoveUnits = null;
		base.gameObject.SetActive(value: false);
	}

	private void ChangePanelSize()
	{
		if (m_bOperatorOption)
		{
			m_NKM_UI_POPUP_REMOVE_PANEL.transform.position = new Vector3(m_NKM_UI_POPUP_REMOVE_PANEL.transform.position.x, m_fRemovePanelOriginalPosY - m_fRemovePanelPosYGap, m_NKM_UI_POPUP_REMOVE_PANEL.transform.position.z);
			m_NKM_UI_POPUP_REMOVE_PANEL.SetHeight(m_fRemovePanelHeightForOperator);
		}
		else
		{
			m_NKM_UI_POPUP_REMOVE_PANEL.transform.position = new Vector3(m_NKM_UI_POPUP_REMOVE_PANEL.transform.position.x, m_fRemovePanelOriginalPosY, m_NKM_UI_POPUP_REMOVE_PANEL.transform.position.z);
			m_NKM_UI_POPUP_REMOVE_PANEL.SetHeight(m_fRemovePanelOriginalHeight);
		}
	}
}
