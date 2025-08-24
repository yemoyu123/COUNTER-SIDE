using ClientPacket.Common;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopSeasonRewardSlot : MonoBehaviour
{
	public delegate void OnClickSlot();

	public NKCUISlot m_slot;

	public GameObject m_objNormal;

	public GameObject m_objClear;

	public GameObject m_objComplete;

	public Text m_lbScore;

	public GameObject m_objGauge;

	public Image m_imgGauge;

	private bool m_bBtnLocked;

	private OnClickSlot m_dOnClickSlot;

	public void InitUI()
	{
		m_slot.Init();
	}

	public void SetData(GuildSeasonRewardTemplet rewardTemplet, bool bCanReward, bool bIsRewarded, bool bIsFinalSlot, OnClickSlot onClickSlot)
	{
		m_dOnClickSlot = onClickSlot;
		if (rewardTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_slot, bValue: false);
			NKCUtil.SetGameobjectActive(m_lbScore, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNormal, bValue: false);
			NKCUtil.SetGameobjectActive(m_objClear, bValue: false);
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
			NKCUtil.SetGameobjectActive(m_objGauge, bValue: true);
			m_bBtnLocked = true;
			return;
		}
		m_bBtnLocked = false;
		NKCUtil.SetGameobjectActive(m_slot, bValue: true);
		m_slot.SetData(NKCUISlot.SlotData.MakeRewardTypeData(rewardTemplet.GetRewardItemType(), rewardTemplet.GetRewardItemId(), rewardTemplet.GetRewardItemValue()), bEnableLayoutElement: true, OnClickBtn);
		m_slot.SetEventGet(bIsRewarded);
		m_slot.SetDisable(!bCanReward || bIsRewarded);
		m_slot.SetRewardFx(bCanReward);
		NKCUtil.SetGameobjectActive(m_lbScore, bValue: true);
		int rewardCountValue = rewardTemplet.GetRewardCountValue();
		NKCUtil.SetLabelText(m_lbScore, rewardCountValue.ToString("N0"));
		NKCUtil.SetGameobjectActive(m_objGauge, !bIsFinalSlot);
		switch (rewardTemplet.GetRewardCategory())
		{
		case GuildDungeonRewardCategory.DUNGEON_TRY:
			NKCUtil.SetGameobjectActive(m_objNormal, !bCanReward);
			NKCUtil.SetGameobjectActive(m_objClear, NKCGuildCoopManager.m_TryCount >= rewardCountValue && !bIsRewarded);
			NKCUtil.SetGameobjectActive(m_objComplete, bIsRewarded);
			break;
		case GuildDungeonRewardCategory.RANK:
			NKCUtil.SetGameobjectActive(m_objNormal, NKCGuildCoopManager.m_KillPoint < rewardCountValue);
			NKCUtil.SetGameobjectActive(m_objClear, NKCGuildCoopManager.m_KillPoint >= rewardCountValue && !bIsRewarded);
			NKCUtil.SetGameobjectActive(m_objComplete, bIsRewarded);
			break;
		}
	}

	public void SetGaugeProgress(float progressPercent)
	{
		if (m_imgGauge != null)
		{
			m_imgGauge.fillAmount = progressPercent;
		}
	}

	public void OnClickBtn(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (m_objClear.activeSelf && !m_bBtnLocked)
		{
			m_bBtnLocked = true;
			m_dOnClickSlot?.Invoke();
		}
		else
		{
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData);
		}
	}
}
