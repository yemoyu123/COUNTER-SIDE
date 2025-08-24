using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComMissionGroup : MonoBehaviour
{
	public delegate void OnRewardGet(NKMMissionTemplet missionTemplet);

	public delegate void OnRewardLocked(bool allMissionCompleted);

	public Text m_lbDesc;

	public Text m_lbCurrentProgress;

	public Text m_lbDestProgress;

	public Image m_imgProgressGauge;

	public NKCUISlot[] m_RewardSlot;

	public NKCUIComStateButton m_csbtnRewardGet;

	public NKCUIComStateButton m_csbtnRewardLocked;

	public GameObject m_objErrandEnableFx;

	private NKMMissionTemplet m_currentMissionTemplet;

	private bool m_allMissionCompleted;

	private OnRewardGet m_dOnRewardGet;

	private OnRewardLocked m_dOnRewardLocked;

	public void Init()
	{
		if (m_RewardSlot != null)
		{
			int num = m_RewardSlot.Length;
			for (int i = 0; i < num; i++)
			{
				m_RewardSlot[i]?.Init();
			}
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnRewardGet, OnClickGet);
		NKCUtil.SetButtonClickDelegate(m_csbtnRewardLocked, OnClickRewardLock);
	}

	public void SetData(int missionGroup, OnRewardGet onRewardGet, OnRewardLocked onRewardLocked = null)
	{
		m_dOnRewardGet = onRewardGet;
		m_dOnRewardLocked = onRewardLocked;
		List<NKMMissionTemplet> missionTempletListByGroupID = NKMMissionManager.GetMissionTempletListByGroupID(missionGroup);
		if (missionTempletListByGroupID.Count <= 0)
		{
			SetEmptyDataUI();
			return;
		}
		NKMMissionData missionData = NKCScenManager.CurrentUserData().m_MissionData.GetMissionData(missionTempletListByGroupID[0]);
		m_currentMissionTemplet = null;
		m_allMissionCompleted = false;
		long num = 0L;
		bool flag = false;
		long num2 = 0L;
		if (missionData == null)
		{
			m_currentMissionTemplet = missionTempletListByGroupID[0];
		}
		else
		{
			int count = missionTempletListByGroupID.Count;
			for (int i = 0; i < count; i++)
			{
				if (missionTempletListByGroupID[i].m_MissionID < missionData.mission_id)
				{
					num2 = missionTempletListByGroupID[i].m_Times;
					continue;
				}
				if (missionData.mission_id == missionTempletListByGroupID[i].m_MissionID && missionData.IsComplete)
				{
					if (i < count - 1)
					{
						num2 = missionTempletListByGroupID[i].m_Times;
					}
					m_allMissionCompleted = true;
					m_currentMissionTemplet = missionTempletListByGroupID[i];
					num = missionData.times;
					flag = missionData.IsComplete;
					continue;
				}
				m_currentMissionTemplet = missionTempletListByGroupID[i];
				m_allMissionCompleted = false;
				num = missionData.times;
				flag = missionData.IsComplete;
				break;
			}
		}
		if (m_currentMissionTemplet == null)
		{
			SetEmptyDataUI();
			return;
		}
		NKCUtil.SetLabelText(m_lbDesc, m_currentMissionTemplet.GetDesc());
		NKCUtil.SetLabelText(m_lbCurrentProgress, num.ToString());
		NKCUtil.SetLabelText(m_lbDestProgress, m_currentMissionTemplet.m_Times.ToString());
		NKCUtil.SetImageFillAmount(m_imgProgressGauge, (float)(num - num2) / (float)(m_currentMissionTemplet.m_Times - num2));
		bool rewardGetButtonState = num >= m_currentMissionTemplet.m_Times && !flag;
		SetRewardGetButtonState(rewardGetButtonState);
		int count2 = m_currentMissionTemplet.m_MissionReward.Count;
		if (m_RewardSlot == null)
		{
			return;
		}
		int num3 = m_RewardSlot.Length;
		for (int j = 0; j < num3; j++)
		{
			if (j >= count2)
			{
				NKCUtil.SetGameobjectActive(m_RewardSlot[j], bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_RewardSlot[j], bValue: true);
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(m_currentMissionTemplet.m_MissionReward[j].reward_id, m_currentMissionTemplet.m_MissionReward[j].reward_value);
			m_RewardSlot[j]?.SetData(data);
		}
	}

	private void SetRewardGetButtonState(bool rewardGetEnable)
	{
		NKCUtil.SetGameobjectActive(m_csbtnRewardGet, rewardGetEnable);
		NKCUtil.SetGameobjectActive(m_csbtnRewardLocked, !rewardGetEnable);
		NKCUtil.SetGameobjectActive(m_objErrandEnableFx, rewardGetEnable);
	}

	private void SetEmptyDataUI()
	{
		NKCUtil.SetLabelText(m_lbDesc, "-");
		NKCUtil.SetLabelText(m_lbCurrentProgress, "-");
		NKCUtil.SetLabelText(m_lbDestProgress, "-");
		NKCUtil.SetImageFillAmount(m_imgProgressGauge, 0f);
		if (m_RewardSlot != null)
		{
			int num = m_RewardSlot.Length;
			for (int i = 0; i < num; i++)
			{
				NKCUtil.SetGameobjectActive(m_RewardSlot[i], bValue: false);
			}
		}
	}

	private void OnClickGet()
	{
		if (m_dOnRewardGet != null)
		{
			m_dOnRewardGet(m_currentMissionTemplet);
		}
	}

	private void OnClickRewardLock()
	{
		if (m_dOnRewardLocked != null)
		{
			m_dOnRewardLocked(m_allMissionCompleted);
		}
	}
}
