using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEquipChangeStatSlot : MonoBehaviour
{
	public Text m_lbName;

	public Text m_lbValue;

	public Text m_lbChangeValue;

	public void SetData(string statShortName, string statValue, string changedValueStr, string changedValueColor)
	{
		NKCUtil.SetLabelText(m_lbName, statShortName);
		NKCUtil.SetLabelText(m_lbValue, statValue);
		NKCUtil.SetLabelText(m_lbChangeValue, changedValueStr);
		NKCUtil.SetLabelTextColor(m_lbChangeValue, NKCUtil.GetColor(changedValueColor));
	}
}
