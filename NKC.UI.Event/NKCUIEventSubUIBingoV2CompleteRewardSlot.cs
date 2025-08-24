using NKM;
using NKM.Event;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventSubUIBingoV2CompleteRewardSlot : MonoBehaviour
{
	public delegate void OnComplete(int reward_index);

	public NKCUISlot m_Slot;

	public Text m_lbDesc;

	public GameObject m_objComplete;

	public GameObject m_objFinalRewardFX;

	public NKCUIComStateButton m_csbtnComplete;

	private NKMEventBingoRewardTemplet m_rewardTemplet;

	private OnComplete dOnComplete;

	public void Init(OnComplete onComplete)
	{
		if (m_Slot != null)
		{
			m_Slot.Init();
		}
		dOnComplete = onComplete;
		NKCUtil.SetButtonClickDelegate(m_csbtnComplete, OnBtnComplete);
	}

	public void SetData(NKMEventBingoRewardTemplet rewardTemplet, EventBingo bingoData, bool bLastReward)
	{
		m_rewardTemplet = rewardTemplet;
		if (rewardTemplet.rewards.Count == 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		if (m_Slot != null)
		{
			NKMRewardInfo nKMRewardInfo = rewardTemplet.rewards[0];
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardInfo.rewardType, nKMRewardInfo.ID, nKMRewardInfo.Count);
			m_Slot.SetData(data);
		}
		NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString("SI_PF_EVENT_BINGO_LINE_REWARD_CLEAR", rewardTemplet.m_BingoCompletTypeValue), true, true);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		int count = bingoData.GetBingoLine().Count;
		bool flag = bingoData.m_bingoInfo.rewardList.Contains(rewardTemplet.ZeroBaseTileIndex);
		bool rewardFx = !flag && count >= rewardTemplet.m_BingoCompletTypeValue;
		m_Slot.SetRewardFx(rewardFx);
		NKCUtil.SetGameobjectActive(m_objComplete, flag);
		if (m_csbtnComplete != null)
		{
			m_csbtnComplete.enabled = rewardFx;
		}
		NKCUtil.SetGameobjectActive(m_objFinalRewardFX, bLastReward);
	}

	public void OnBtnComplete()
	{
		if (m_rewardTemplet != null)
		{
			dOnComplete?.Invoke(m_rewardTemplet.ZeroBaseTileIndex);
		}
	}
}
