using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupSelectionConfirmSkin : MonoBehaviour
{
	public NKCUISlot m_iconSlot;

	public Text m_lbName;

	public Text m_lbDesc;

	public Text m_lbOwn;

	public Text m_lbOwnCount;

	public Text m_lbSelectItemOwnAmount;

	public Text m_lbSelectItemCount;

	public void Init()
	{
		m_iconSlot?.Init();
	}

	public void SetData(int skinId, int selectMiscItemId)
	{
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinId);
		if (skinTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbName, skinTemplet.GetTitle());
			NKCUtil.SetLabelText(m_lbDesc, skinTemplet.GetSkinDesc());
		}
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeSkinData(skinId);
		m_iconSlot?.SetData(data);
		NKCUtil.SetLabelText(m_lbOwn, NKCStringTable.GetString("SI_DP_POPUP_USE_COUNT_TEXT_MISC_HAVE"));
		NKCUtil.SetLabelText(m_lbSelectItemOwnAmount, NKCStringTable.GetString("SI_DP_POPUP_USE_COUNT_HAVE_TEXT_MISC_CHOICE"));
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		int num = 0;
		long num2 = 0L;
		if (nKMUserData != null)
		{
			num = (nKMUserData.m_InventoryData.HasItemSkin(skinId) ? 1 : 0);
			num2 = nKMUserData.m_InventoryData.GetCountMiscItem(selectMiscItemId);
		}
		NKCUtil.SetLabelText(m_lbOwnCount, num.ToString());
		NKCUtil.SetLabelText(m_lbSelectItemCount, num2.ToString());
	}

	private string GetUnitName(int unitId)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitId);
		string result = "";
		if (unitTempletBase != null)
		{
			result = ((!unitTempletBase.m_bAwaken) ? unitTempletBase.GetUnitName() : NKCStringTable.GetString("SI_PF_SHOP_SKIN_AWAKEN", unitTempletBase.GetUnitName()));
		}
		return result;
	}
}
