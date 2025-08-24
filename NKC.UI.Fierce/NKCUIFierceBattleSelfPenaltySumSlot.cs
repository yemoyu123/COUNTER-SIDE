using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCUIFierceBattleSelfPenaltySumSlot : MonoBehaviour
{
	public Text m_lbDesc;

	public Image m_imgIcon;

	private NKMFiercePenaltyTemplet m_iPenaltyTemplet;

	public NKMFiercePenaltyTemplet PenaltyTemplet => m_iPenaltyTemplet;

	public void SetData(NKMFiercePenaltyTemplet templet)
	{
		if (templet == null)
		{
			return;
		}
		m_iPenaltyTemplet = templet;
		if (templet.battleCondition != null)
		{
			if (!string.IsNullOrEmpty(templet.battleCondition.BattleCondIngameIcon))
			{
				Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_fierce_battle_texture", templet.battleCondition.BattleCondIngameIcon);
				NKCUtil.SetImageSprite(m_imgIcon, orLoadAssetResource);
			}
			string msg = NKCStringTable.GetString(templet.battleCondition.BattleCondDesc);
			NKCUtil.SetLabelText(m_lbDesc, msg);
		}
	}
}
