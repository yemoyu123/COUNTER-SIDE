using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIRearmamentExtractSynergySlot : MonoBehaviour
{
	public NKCUISlot m_Slot;

	public Text m_lbName;

	public Text m_lbCount;

	public Text m_lbPercent;

	public void SetData(int itemID, int itemCnt, int percent)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
		nKMRewardInfo.rewardType = NKM_REWARD_TYPE.RT_MISC;
		nKMRewardInfo.ID = itemID;
		nKMRewardInfo.Count = itemCnt;
		m_Slot.SetData(NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardInfo));
		NKCUtil.SetLabelText(m_lbName, itemMiscTempletByID.GetItemName());
		NKCUtil.SetLabelText(m_lbCount, itemCnt.ToString());
		NKCUtil.SetLabelText(m_lbPercent, string.Format("{0}%", ((double)percent * 0.01).ToString("N2")));
	}
}
