using NKC;
using UnityEngine;
using UnityEngine.UI;

public class NKCPopupEquipOptionListSlot : MonoBehaviour
{
	public Text m_OPTION_NAME;

	public Text m_OPTION_TEXT;

	public void SetData(string name, string value)
	{
		NKCUtil.SetLabelText(m_OPTION_NAME, name);
		NKCUtil.SetLabelText(m_OPTION_TEXT, value);
	}
}
