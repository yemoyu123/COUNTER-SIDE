using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventBarIngradientSlot : MonoBehaviour
{
	public delegate bool OnSelectIngradient(int itemID, bool select);

	public Image m_imgIcon;

	public Text m_lbName;

	public Text m_lbDesc;

	public GameObject m_objSelect;

	public NKCUIComStateButton m_csbtnSlot;

	private int m_itemID;

	private OnSelectIngradient m_dOnSelect;

	public int ItemID => m_itemID;

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnSlot, OnClickSlot);
	}

	public void SetData(int itemID, OnSelectIngradient onSelect)
	{
		m_itemID = itemID;
		m_dOnSelect = onSelect;
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID != null)
		{
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadMiscItemIcon(itemMiscTempletByID));
			NKCUtil.SetLabelText(m_lbName, itemMiscTempletByID.GetItemName());
			NKCUtil.SetLabelText(m_lbDesc, itemMiscTempletByID.GetItemDesc());
			m_csbtnSlot?.Select(bSelect: false);
		}
	}

	public bool IsSelected()
	{
		if (m_csbtnSlot == null)
		{
			return false;
		}
		return m_csbtnSlot.m_bSelect;
	}

	private void OnClickSlot()
	{
		if (m_dOnSelect != null)
		{
			bool bSelect = m_dOnSelect(m_itemID, !m_csbtnSlot.m_bSelect);
			m_csbtnSlot.Select(bSelect);
		}
	}
}
