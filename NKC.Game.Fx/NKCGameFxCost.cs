using NKC.UI.Component;
using UnityEngine;

namespace NKC.Game.Fx;

public class NKCGameFxCost : MonoBehaviour
{
	public NKCComTMPUIText m_lbCost;

	public GameObject m_objPlus;

	public GameObject m_objMinus;

	public Animator m_animator;

	public void SetData(float value)
	{
		NKCUtil.SetGameobjectActive(m_objMinus, value < 0f);
		NKCUtil.SetGameobjectActive(m_objPlus, value > 0f);
		int value2 = (int)value;
		NKCUtil.SetLabelText(m_lbCost, Mathf.Abs(value2).ToString());
		if (m_animator != null)
		{
			m_animator.ResetTrigger("PLUS");
			m_animator.ResetTrigger("MINUS");
			if (value > 0f)
			{
				m_animator.SetTrigger("PLUS");
			}
			else if (value < 0f)
			{
				m_animator.SetTrigger("MINUS");
			}
		}
	}
}
