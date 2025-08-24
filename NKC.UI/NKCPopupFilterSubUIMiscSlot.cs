using UnityEngine;

namespace NKC.UI;

public class NKCPopupFilterSubUIMiscSlot : MonoBehaviour
{
	public NKCUIComToggle m_cTgl;

	public NKCComText[] m_lbName;

	private int m_iTitleCategoryID;

	public int TitleCategoryID => m_iTitleCategoryID;

	public void SetData(int titleCategoryID, string titleName)
	{
		NKCComText[] lbName = m_lbName;
		for (int i = 0; i < lbName.Length; i++)
		{
			NKCUtil.SetLabelText(lbName[i], titleName);
		}
		m_iTitleCategoryID = titleCategoryID;
	}
}
