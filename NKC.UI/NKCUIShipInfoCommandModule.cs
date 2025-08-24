using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.Events;

namespace NKC.UI;

public class NKCUIShipInfoCommandModule : MonoBehaviour
{
	public NKCUIComStateButton m_btnModuleSetting;

	public List<GameObject> m_lstModuleStep = new List<GameObject>();

	private UnityAction m_dOnClickModuleSetting;

	public void Init(UnityAction dOnClickModuleSetting)
	{
		m_btnModuleSetting.PointerClick.RemoveAllListeners();
		m_btnModuleSetting.PointerClick.AddListener(OnClickModuleSetting);
		m_dOnClickModuleSetting = dOnClickModuleSetting;
	}

	public void SetData(NKMUnitData shipData)
	{
		for (int i = 0; i < m_lstModuleStep.Count; i++)
		{
			if (i < shipData.m_LimitBreakLevel)
			{
				NKCUtil.SetGameobjectActive(m_lstModuleStep[i], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstModuleStep[i], bValue: false);
			}
		}
	}

	private void OnClickModuleSetting()
	{
		m_dOnClickModuleSetting?.Invoke();
	}
}
