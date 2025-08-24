using NKM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIUnitInfoSummary : MonoBehaviour
{
	private NKCUICharInfoSummary m_NKCUICharInfoSummary;

	private NKCUIComButton m_NKM_UI_UNIT_CHANGE;

	private bool IsSet;

	public void InitUI()
	{
		m_NKCUICharInfoSummary = base.gameObject.GetComponent<NKCUICharInfoSummary>();
		m_NKCUICharInfoSummary.Init();
		m_NKM_UI_UNIT_CHANGE = base.transform.Find("NKM_UI_LAB_UNIT_CHANGE").GetComponent<NKCUIComButton>();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void FindLabObject()
	{
		if (!IsSet)
		{
			m_NKCUICharInfoSummary.m_lbPowerSummary = GameObject.Find("NKM_UI_LAB_UNIT_SUMMARY_UNIT_POWER_TEXT").GetComponent<Text>();
			m_NKCUICharInfoSummary.m_lbPowerSummary.text = "";
			IsSet = true;
		}
	}

	public void LinkLab(UnityAction addListener)
	{
		m_NKM_UI_UNIT_CHANGE.PointerClick.RemoveAllListeners();
		m_NKM_UI_UNIT_CHANGE.PointerClick.AddListener(addListener);
	}

	public void SetData(NKMUnitData unitData)
	{
		m_NKCUICharInfoSummary.SetData(unitData);
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
