using System;
using ClientPacket.Mode;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCPopupShadowRecordSlot : MonoBehaviour
{
	public Text m_txtNum;

	public Text m_txtTime;

	public GameObject m_objNormal;

	public GameObject m_objBoss;

	public void SetData(NKMShadowBattleTemplet templet, NKMPalaceDungeonData data)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(data.recentTime);
		NKCUtil.SetLabelText(m_txtTime, NKCUtilString.GetTimeSpanString(timeSpan));
		NKCUtil.SetGameobjectActive(m_objNormal, templet.PALACE_BATTLE_TYPE != PALACE_BATTLE_TYPE.PBT_BOSS);
		NKCUtil.SetGameobjectActive(m_objBoss, templet.PALACE_BATTLE_TYPE == PALACE_BATTLE_TYPE.PBT_BOSS);
		if (templet.PALACE_BATTLE_TYPE == PALACE_BATTLE_TYPE.PBT_BOSS)
		{
			NKCUtil.SetLabelText(m_txtNum, NKCUtilString.GET_SHADOW_RECORD_POPUP_SLOT_BOSS);
			return;
		}
		NKCUtil.SetLabelText(m_txtNum, NKCUtilString.GET_SHADOW_RECORD_POPUP_SLOT_NORMAL, templet.BATTLE_ORDER.ToString("D2"));
	}
}
