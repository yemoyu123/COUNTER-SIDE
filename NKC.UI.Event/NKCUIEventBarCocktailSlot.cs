using NKC.UI.Tooltip;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventBarCocktailSlot : MonoBehaviour
{
	public delegate void OnSelectSlot(GameObject slot);

	public Text m_lbCockTailName;

	public Text m_lbCockTailCount;

	public Image m_imgCockTailIcon;

	public GameObject m_objNone;

	public GameObject m_objSelect;

	public string m_strIconColorNone;

	public NKCUIComStateButton m_csbtnSlot;

	private int m_cockTailID;

	private float timer;

	private OnSelectSlot m_dOnSelectSlot;

	public int CockTailID => m_cockTailID;

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnSlot, OnClickSlot);
	}

	public void SetData(int cocktailID, OnSelectSlot onSelectSlot)
	{
		m_cockTailID = cocktailID;
		m_dOnSelectSlot = onSelectSlot;
		long num = 0L;
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(cocktailID);
		if (itemMiscTempletByID != null)
		{
			NKCUtil.SetLabelText(m_lbCockTailName, itemMiscTempletByID.GetItemName());
			Sprite orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(itemMiscTempletByID);
			NKCUtil.SetImageSprite(m_imgCockTailIcon, orLoadMiscItemIcon);
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				num = nKMUserData.m_InventoryData.GetCountMiscItem(itemMiscTempletByID.m_ItemMiscID);
			}
		}
		NKCUtil.SetLabelText(m_lbCockTailCount, num.ToString());
		NKCUtil.SetGameobjectActive(m_objNone, num <= 0);
		Color color = Color.white;
		if (num <= 0 && !string.IsNullOrEmpty(m_strIconColorNone))
		{
			ColorUtility.TryParseHtmlString(m_strIconColorNone, out color);
		}
		NKCUtil.SetImageColor(m_imgCockTailIcon, color);
		m_csbtnSlot?.Select(bSelect: false);
	}

	public void SetSelected(bool value)
	{
		m_csbtnSlot?.Select(value);
	}

	public bool Toggle()
	{
		if (m_csbtnSlot == null)
		{
			return false;
		}
		m_csbtnSlot.Select(!m_csbtnSlot.m_bSelect);
		return m_csbtnSlot.m_bSelect;
	}

	private void OnClickSlot()
	{
		if (m_dOnSelectSlot != null)
		{
			m_dOnSelectSlot(base.gameObject);
		}
	}

	private void OnClickPress(PointerEventData eventData)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_cockTailID);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		long count = 0L;
		if (nKMUserData != null && itemMiscTempletByID != null)
		{
			count = nKMUserData.m_InventoryData.GetCountMiscItem(itemMiscTempletByID.m_ItemMiscID);
		}
		NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(m_cockTailID, count);
		NKCUITooltip.Instance.Open(slotData, eventData.position);
	}
}
