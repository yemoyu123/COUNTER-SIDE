using System.Collections.Generic;
using NKM.Event;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventBingoRewardSlot : MonoBehaviour
{
	public delegate void OnTouchComplete(NKMEventBingoRewardTemplet rewardTemplet);

	public Text m_txtTitle;

	public List<NKCUISlot> m_listSlot;

	public GameObject m_objClear;

	public GameObject m_objComplete;

	public NKCUIComButton m_btnComplete;

	public NKCUIComButton m_btnDisable;

	public Text m_txtButton;

	private OnTouchComplete dOnTouchComplete;

	private NKMEventBingoRewardTemplet m_rewardTemplet;

	public void Init(OnTouchComplete onTouchComplete)
	{
		dOnTouchComplete = onTouchComplete;
		if (m_listSlot != null)
		{
			for (int i = 0; i < m_listSlot.Count; i++)
			{
				m_listSlot[i].Init();
			}
		}
		if (m_btnComplete != null)
		{
			m_btnComplete.PointerClick.RemoveAllListeners();
			m_btnComplete.PointerClick.AddListener(OnTouchCompleteBtn);
		}
	}

	public void SetData(NKMEventBingoRewardTemplet rewardTemplet, int currentBingoCount, bool bComplete)
	{
		m_rewardTemplet = rewardTemplet;
		NKCUtil.SetLabelText(m_txtTitle, NKCUtilString.GET_STRING_EVENT_BINGO_REWARD_TITLE, rewardTemplet.m_BingoCompletTypeValue);
		if (m_listSlot != null)
		{
			for (int i = 0; i < m_listSlot.Count; i++)
			{
				NKCUISlot nKCUISlot = m_listSlot[i];
				if (i < rewardTemplet.rewards.Count)
				{
					NKMRewardInfo nKMRewardInfo = rewardTemplet.rewards[i];
					NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardInfo.rewardType, nKMRewardInfo.ID, nKMRewardInfo.Count);
					nKCUISlot.SetData(data);
					NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
				}
				else
				{
					NKCUtil.SetGameobjectActive(nKCUISlot, bValue: false);
				}
			}
		}
		bool flag = !bComplete && currentBingoCount >= rewardTemplet.m_BingoCompletTypeValue;
		NKCUtil.SetGameobjectActive(m_objClear, flag);
		NKCUtil.SetGameobjectActive(m_objComplete, bComplete);
		NKCUtil.SetGameobjectActive(m_btnComplete, flag);
		NKCUtil.SetGameobjectActive(m_btnDisable, !flag);
		NKCUtil.SetLabelText(m_txtButton, bComplete ? NKCUtilString.GET_STRING_EVENT_BINGO_REWARD_SLOT_COMPLETE : NKCUtilString.GET_STRING_EVENT_BINGO_REWARD_SLOT_PROGRESS);
	}

	private void OnTouchCompleteBtn()
	{
		dOnTouchComplete?.Invoke(m_rewardTemplet);
	}
}
