using ClientPacket.Common;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIComKillCountRewardSlot : MonoBehaviour
{
	public Text m_lbStep;

	public Text m_lbDesc;

	public NKCUISlot m_Slot;

	private int m_iEventId;

	private int m_iStep;

	private int m_iServerCompleteStep;

	public void Init()
	{
		m_Slot?.Init();
	}

	public void SetData(int eventId, NKMKillCountStepTemplet stepTemplet, NKMKillCountData killCountData, long currentServerKillCount)
	{
		if (stepTemplet == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_lbStep, string.Format(NKCUtilString.GET_STRING_KILLCOUNT_SERVER_REWARD_STEP, stepTemplet.StepId));
		NKCUtil.SetLabelText(m_lbDesc, string.Format(NKCUtilString.GET_STRING_KILLCOUNT_SERVER_REWARD_DESC, stepTemplet.KillCount));
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(stepTemplet.RewardInfo);
		m_iEventId = eventId;
		m_iStep = stepTemplet.StepId;
		m_iServerCompleteStep = 0;
		if (killCountData != null)
		{
			m_iServerCompleteStep = killCountData.serverCompleteStep;
		}
		if (m_iServerCompleteStep >= stepTemplet.StepId)
		{
			m_Slot.SetData(data);
			m_Slot.SetDisable(disable: true);
			m_Slot.SetEventGet(bActive: true);
			m_Slot.SetFirstGetMark(bValue: false);
			return;
		}
		if (stepTemplet.KillCount <= currentServerKillCount)
		{
			m_Slot.SetData(data, bEnableLayoutElement: true, OnClickSlot);
			m_Slot.SetFirstGetMark(bValue: true);
			m_Slot.SetArrowBGText(NKCUtilString.GET_STRING_KILLCOUNT_SERVER_REWARD_GET, NKCUtil.GetColor("#4E4F52"));
		}
		else
		{
			m_Slot.SetData(data);
			m_Slot.SetFirstGetMark(bValue: false);
		}
		m_Slot.SetDisable(disable: false);
		m_Slot.SetEventGet(bActive: false);
	}

	private void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCPacketSender.Send_NKMPacket_KILL_COUNT_SERVER_REWARD_REQ(m_iEventId, m_iServerCompleteStep + 1);
	}
}
