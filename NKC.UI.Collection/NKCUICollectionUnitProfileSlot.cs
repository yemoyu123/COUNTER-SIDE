using TMPro;
using UnityEngine;

namespace NKC.UI.Collection;

public class NKCUICollectionUnitProfileSlot : MonoBehaviour
{
	public TMP_Text m_TypeText;

	public TMP_Text m_ValueText;

	public NKCUICollectionProfileToolTip m_profileToopTip;

	public void Init()
	{
		m_profileToopTip?.Init();
	}

	public void SetData(string type, string value)
	{
		string msg = NKCStringTable.GetString(type);
		string msg2 = NKCStringTable.GetString(value);
		NKCUtil.SetLabelText(m_TypeText, msg);
		NKCUtil.SetLabelText(m_ValueText, msg2);
		m_profileToopTip?.SetDescData(type);
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
	}
}
