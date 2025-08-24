using TMPro;
using UnityEngine;

namespace NKC.UI.Collection;

public class NKCUICollectionInfoTab : MonoBehaviour
{
	public NKCUIComToggle m_tglToggle;

	public TMP_Text[] m_lbTabNumber;

	public TMP_Text[] m_lbTabName;

	public void SetTabNumber(int number)
	{
		if (m_lbTabNumber != null)
		{
			TMP_Text[] lbTabNumber = m_lbTabNumber;
			for (int i = 0; i < lbTabNumber.Length; i++)
			{
				NKCUtil.SetLabelText(lbTabNumber[i], number.ToString("D2"));
			}
		}
	}

	public void Select(bool select, bool force = false)
	{
		if (base.gameObject.activeSelf)
		{
			m_tglToggle?.Select(select, force);
		}
	}
}
