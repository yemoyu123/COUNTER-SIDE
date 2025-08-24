using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupMatchTenSlot : MonoBehaviour
{
	public Animator m_Ani;

	public Image m_imgIcon;

	public Image m_imgNum;

	public GameObject m_objNum;

	public GameObject m_objSelected;

	private int m_col;

	private int m_row;

	private int m_Num;

	public void SetData(int col, int row, int num, Sprite sprIcon)
	{
		NKCUtil.SetGameobjectActive(m_objNum, bValue: true);
		NKCUtil.SetGameobjectActive(m_objSelected, bValue: false);
		m_col = col;
		m_row = row;
		if (sprIcon != null)
		{
			NKCUtil.SetImageSprite(m_imgIcon, sprIcon);
		}
		NKCUtil.SetImageSprite(m_imgNum, GetNumberImg(num));
		m_Num = num;
		NKCUtil.SetGameobjectActive(m_objNum, num != 0);
		m_Ani.SetTrigger("IDLE");
	}

	private Sprite GetNumberImg(int num)
	{
		if (num == 0 || (uint)(num - 1) > 8u)
		{
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("UI_SINGLE_MATCHTEN_Sprite", $"UI_SINGLE_MATCHTEN_NUM_0{num}");
	}

	public int GetNumber()
	{
		return m_Num;
	}

	public int GetCol()
	{
		return m_col;
	}

	public int GetRow()
	{
		return m_row;
	}

	public void SetNumberOff()
	{
		NKCUtil.SetGameobjectActive(m_objSelected, bValue: false);
		m_Ani.SetTrigger("CLEAR");
		m_Num = 0;
	}

	public void SetSelected(bool bValue)
	{
		if (m_objNum.activeInHierarchy)
		{
			NKCUtil.SetGameobjectActive(m_objSelected, bValue);
		}
	}
}
