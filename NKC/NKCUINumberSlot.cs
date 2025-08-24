using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUINumberSlot : MonoBehaviour
{
	[Serializable]
	public class Number
	{
		public Text m_lbCredit;

		public GameObject m_objEnable;

		public GameObject m_objDisable;

		public void SetEnable(bool value)
		{
			NKCUtil.SetGameobjectActive(m_objEnable, value);
			NKCUtil.SetGameobjectActive(m_objDisable, !value);
		}

		public void SetNumber(int value)
		{
			NKCUtil.SetLabelText(m_lbCredit, value.ToString());
		}
	}

	[Header("번호 오브젝트, 낮은 자리수부터 높은 자리수 순으로")]
	public List<Number> m_lstNumber;

	public Animator m_Animator;

	private int currentValue = -1;

	public void SetValue(int value, bool bForceAni = false)
	{
		if (bForceAni || currentValue != value)
		{
			m_Animator.SetTrigger("Play");
			int num = 1;
			for (int i = 0; i < m_lstNumber.Count; i++)
			{
				if (m_lstNumber[i] != null)
				{
					m_lstNumber[i].SetEnable(value >= num);
					m_lstNumber[i].SetNumber(value / num % 10);
				}
				num *= 10;
			}
		}
		currentValue = value;
	}
}
