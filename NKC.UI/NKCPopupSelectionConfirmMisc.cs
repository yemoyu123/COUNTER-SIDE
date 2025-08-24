using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupSelectionConfirmMisc : MonoBehaviour
{
	public NKCUISlot m_slot;

	public Text m_lbName;

	public Text m_lbHaveCount;

	public Text m_lbDesc;

	public void SetData(int itemID, long count)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID != null)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(itemID, count);
			m_slot.SetData(data, bShowName: false, bShowNumber: true, bEnableLayoutElement: false, null);
			NKCUtil.SetLabelText(m_lbName, itemMiscTempletByID.GetItemName());
			NKCUtil.SetLabelText(m_lbHaveCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemID).ToString("N0"));
			NKCUtil.SetLabelText(m_lbDesc, itemMiscTempletByID.GetItemDesc());
		}
	}
}
