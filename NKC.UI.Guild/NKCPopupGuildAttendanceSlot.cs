using NKM;
using NKM.Contract2;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildAttendanceSlot : MonoBehaviour
{
	public Text m_lbNeedMemberCount;

	public Image m_imgRewardIcon;

	public Text m_lbReward;

	public GameObject m_objComplete;

	public void SetData(int needMemberCount, RewardUnit reward, bool bComplete)
	{
		if (needMemberCount > 0)
		{
			NKCUtil.SetLabelText(m_lbNeedMemberCount, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_ATTENDANCE_REWARD_CONDITION, needMemberCount));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbNeedMemberCount, NKCUtilString.GET_STRING_CONSORTIUM_POPUP_ATTENDANCE_REWARD_BASIC);
		}
		NKCUtil.SetImageSprite(m_imgRewardIcon, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(reward.ItemID));
		NKCUtil.SetLabelText(m_lbReward, $"{GetRewardName(reward)} {reward.Count}");
		NKCUtil.SetGameobjectActive(m_objComplete, bComplete);
	}

	private string GetRewardName(RewardUnit reward)
	{
		if (reward.RewardType == NKM_REWARD_TYPE.RT_MISC)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(reward.ItemID);
			if (itemMiscTempletByID != null)
			{
				return itemMiscTempletByID.GetItemName();
			}
		}
		return "";
	}
}
