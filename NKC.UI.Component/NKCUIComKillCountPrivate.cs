using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIComKillCountPrivate : MonoBehaviour
{
	public Text m_lbPrivateCurrentKillCnt;

	public Text[] m_lbPrivateRewardKillCnt;

	public NKCUISlot[] m_privateRewardSlot;

	public GameObject m_objComplete;

	public GameObject[] m_objSlotRewardOn;

	public Image[] m_KillCountGauge;

	public NKCUIComStateButton m_csbtnPrivateRewardComplete;

	public NKCUIComStateButton m_csbtnPrivateRewardProgress;

	private int m_iEventId;

	private int m_iMaxUserStep;

	public void Init()
	{
		if (m_privateRewardSlot != null)
		{
			int num = m_privateRewardSlot.Length;
			for (int i = 0; i < num; i++)
			{
				m_privateRewardSlot[i]?.Init();
			}
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnPrivateRewardProgress, OnClickProgress);
		NKCUtil.SetButtonClickDelegate(m_csbtnPrivateRewardComplete, OnClickComplete);
		NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
	}

	public void SetData(int eventId)
	{
		m_iEventId = eventId;
		m_iMaxUserStep = 0;
		long num = 0L;
		int userCompleteStep = 0;
		NKMKillCountData killCountData = NKCKillCountManager.GetKillCountData(eventId);
		if (killCountData != null)
		{
			num = killCountData.killCount;
			userCompleteStep = killCountData.userCompleteStep;
		}
		NKCUtil.SetLabelText(m_lbPrivateCurrentKillCnt, $"{num:#,0}");
		NKMKillCountTemplet nKMKillCountTemplet = NKMKillCountTemplet.Find(eventId);
		if (nKMKillCountTemplet != null)
		{
			m_iMaxUserStep = nKMKillCountTemplet.GetMaxUserStep();
			SetRewardKillCount(nKMKillCountTemplet);
			SetRewardSlot(nKMKillCountTemplet, userCompleteStep, num);
			SetRewardGetButtonState(nKMKillCountTemplet, userCompleteStep, num);
			SetKillCountGauge(nKMKillCountTemplet, num, userCompleteStep);
		}
	}

	private void SetRewardKillCount(NKMKillCountTemplet killCountTemplet)
	{
		if (m_lbPrivateRewardKillCnt == null)
		{
			return;
		}
		int num = m_lbPrivateRewardKillCnt.Length;
		for (int i = 0; i < num; i++)
		{
			NKMKillCountStepTemplet result = null;
			killCountTemplet.TryGetUserStep(i + 1, out result);
			if (result != null)
			{
				NKCUtil.SetLabelText(m_lbPrivateRewardKillCnt[i], $"{result.KillCount:#,0}");
			}
		}
	}

	private void SetRewardSlot(NKMKillCountTemplet killCountTemplet, int userCompleteStep, long killCount)
	{
		if (m_privateRewardSlot == null)
		{
			return;
		}
		int num = m_privateRewardSlot.Length;
		for (int i = 0; i < num; i++)
		{
			NKMKillCountStepTemplet result = null;
			killCountTemplet.TryGetUserStep(i + 1, out result);
			if (result != null && !(m_privateRewardSlot[i] == null))
			{
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(result.RewardInfo);
				m_privateRewardSlot[i].SetData(data);
				bool flag = false;
				bool bValue = false;
				if (i + 1 <= userCompleteStep)
				{
					flag = true;
				}
				else if (killCount >= result.KillCount)
				{
					bValue = true;
				}
				if (m_objSlotRewardOn != null && m_objSlotRewardOn[i] != null)
				{
					NKCUtil.SetGameobjectActive(m_objSlotRewardOn[i], bValue);
				}
				m_privateRewardSlot[i].SetDisable(flag);
				m_privateRewardSlot[i].SetEventGet(flag);
			}
		}
	}

	private void SetRewardGetButtonState(NKMKillCountTemplet killCountTemplet, int userCompleteStep, long killCount)
	{
		if (m_iMaxUserStep > userCompleteStep)
		{
			NKMKillCountStepTemplet result = null;
			killCountTemplet.TryGetUserStep(userCompleteStep + 1, out result);
			if (result != null)
			{
				NKCUtil.SetGameobjectActive(m_csbtnPrivateRewardProgress.gameObject, killCount < result.KillCount);
				NKCUtil.SetGameobjectActive(m_csbtnPrivateRewardComplete.gameObject, killCount >= result.KillCount);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnPrivateRewardProgress.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtnPrivateRewardComplete.gameObject, bValue: false);
		}
	}

	private void SetKillCountGauge(NKMKillCountTemplet killCountTemplet, long killCount, int userCompleteStep)
	{
		if (m_KillCountGauge == null)
		{
			return;
		}
		int num = m_KillCountGauge.Length;
		for (int i = 0; i < num; i++)
		{
			if (!(m_KillCountGauge[i] == null))
			{
				int num2 = 0;
				NKMKillCountStepTemplet result = null;
				killCountTemplet.TryGetUserStep(i + 1, out result);
				if (result != null)
				{
					num2 = result.KillCount;
				}
				int num3 = 0;
				NKMKillCountStepTemplet result2 = null;
				killCountTemplet.TryGetUserStep(i, out result2);
				if (result2 != null)
				{
					num3 = result2.KillCount;
				}
				float num4 = 0f;
				NKCUtil.SetImageFillAmount(value: (killCount >= num2) ? 1f : ((killCount >= num3) ? ((float)(killCount - num3) / (float)(num2 - num3)) : 0f), image: m_KillCountGauge[i]);
			}
		}
	}

	private void OnClickProgress()
	{
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OPERATION, "EC_EVENT");
	}

	private void OnClickComplete()
	{
		NKMKillCountData killCountData = NKCKillCountManager.GetKillCountData(m_iEventId);
		long num = 0L;
		int num2 = 0;
		if (killCountData != null)
		{
			num = killCountData.killCount;
			num2 = killCountData.userCompleteStep;
		}
		if (m_iMaxUserStep <= num2)
		{
			return;
		}
		NKMKillCountTemplet nKMKillCountTemplet = NKMKillCountTemplet.Find(m_iEventId);
		if (nKMKillCountTemplet != null)
		{
			NKMKillCountStepTemplet result = null;
			nKMKillCountTemplet.TryGetUserStep(num2 + 1, out result);
			if (result != null && result.KillCount <= num)
			{
				NKCPacketSender.Send_NKMPacket_KILL_COUNT_USER_REWARD_REQ(m_iEventId, num2 + 1);
			}
		}
	}
}
