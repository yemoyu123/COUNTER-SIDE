using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupChangeBirthdaySlot : MonoBehaviour
{
	public Text m_lbNum;

	private int m_Data;

	public void SetData(int data)
	{
		m_Data = data;
		NKCUtil.SetLabelText(m_lbNum, data.ToString());
	}

	public int GetData()
	{
		return m_Data;
	}
}
