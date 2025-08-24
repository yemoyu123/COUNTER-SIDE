using System;
using ClientPacket.Pvp;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletEventRewardSlot : MonoBehaviour
{
	public NKCUISlot[] m_iconSlot;

	public Text m_lbDesc;

	public Text m_lbProgress;

	public Text m_lbResetTime;

	public GameObject m_objClear;

	public GameObject m_objComplete;

	private NKCAssetInstanceData m_InstanceData;

	private bool m_bHasCooltime;

	private DateTime m_NextActivateTimeUTC;

	private NKMMissionTemplet m_MissionTemplet;

	private bool m_bNeedRefresh;

	private float m_deltaTime;

	public static NKCUIGauntletEventRewardSlot GetNewInstance(Transform parent, string bundleName, string assetName)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCUIGauntletEventRewardSlot nKCUIGauntletEventRewardSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUIGauntletEventRewardSlot>();
		if (nKCUIGauntletEventRewardSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIGauntletEventRewardSlot Prefab null!");
			return null;
		}
		nKCUIGauntletEventRewardSlot.m_InstanceData = nKCAssetInstanceData;
		nKCUIGauntletEventRewardSlot.Init();
		if (parent != null)
		{
			nKCUIGauntletEventRewardSlot.transform.SetParent(parent);
		}
		nKCUIGauntletEventRewardSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUIGauntletEventRewardSlot.gameObject.SetActive(value: false);
		return nKCUIGauntletEventRewardSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Init()
	{
		for (int i = 0; i < m_iconSlot.Length; i++)
		{
			m_iconSlot[i].Init();
		}
	}

	public void SetEmpty()
	{
		m_bHasCooltime = false;
		m_MissionTemplet = null;
		NKCUtil.SetLabelText(m_lbDesc, "");
		NKCUtil.SetLabelText(m_lbProgress, "");
		NKCUtil.SetLabelText(m_lbResetTime, "");
		NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
		NKCUtil.SetGameobjectActive(m_objClear, bValue: false);
	}

	public void SetData(NKMEventPvpRewardTemplet eventPvpRewardTemplet)
	{
		m_bHasCooltime = false;
		m_MissionTemplet = null;
		if (eventPvpRewardTemplet == null)
		{
			NKCUtil.SetLabelText(m_lbDesc, "");
			NKCUtil.SetLabelText(m_lbProgress, "");
			NKCUtil.SetLabelText(m_lbResetTime, "");
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
			NKCUtil.SetGameobjectActive(m_objClear, bValue: false);
			return;
		}
		NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString(eventPvpRewardTemplet.DescStrID));
		NKCUtil.SetLabelText(m_lbResetTime, "");
		for (int i = 0; i < m_iconSlot.Length; i++)
		{
			if (eventPvpRewardTemplet.RewardInfos[i].ID <= 0)
			{
				m_iconSlot[i].SetActive(bSet: false);
				continue;
			}
			m_iconSlot[i].SetActive(bSet: true);
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(eventPvpRewardTemplet.RewardInfos[i]);
			m_iconSlot[i].SetData(data);
		}
		EventPvpReward eventPvpReward = NKCEventPvpMgr.EventPvpRewardInfo.Find((EventPvpReward e) => e.groupId == eventPvpRewardTemplet.RewardGroupId);
		if (eventPvpReward == null)
		{
			return;
		}
		int num = Mathf.Min(eventPvpReward.playCount, eventPvpRewardTemplet.PlayTimes);
		NKCUtil.SetLabelText(m_lbProgress, $"{num}/{eventPvpRewardTemplet.PlayTimes}");
		if (eventPvpRewardTemplet.PlayTimes <= eventPvpReward.playCount)
		{
			if (eventPvpRewardTemplet.Step < eventPvpReward.step)
			{
				NKCUtil.SetGameobjectActive(m_objComplete, bValue: true);
				NKCUtil.SetGameobjectActive(m_objClear, bValue: false);
			}
			else if (eventPvpRewardTemplet.Step == eventPvpReward.step)
			{
				NKCUtil.SetGameobjectActive(m_objComplete, eventPvpReward.isReward);
				NKCUtil.SetGameobjectActive(m_objClear, !eventPvpReward.isReward);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
				NKCUtil.SetGameobjectActive(m_objClear, bValue: true);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
			NKCUtil.SetGameobjectActive(m_objClear, bValue: false);
		}
	}

	public void SetData(NKMMissionTemplet missionTemplet)
	{
		NKCUtil.SetLabelText(m_lbDesc, missionTemplet.GetDesc());
		m_MissionTemplet = missionTemplet;
		for (int i = 0; i < m_iconSlot.Length; i++)
		{
			if (i >= missionTemplet.m_MissionReward.Count || missionTemplet.m_MissionReward[i].reward_id <= 0)
			{
				m_iconSlot[i].SetActive(bSet: false);
				continue;
			}
			m_iconSlot[i].SetActive(bSet: true);
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(missionTemplet.m_MissionReward[i].reward_type, missionTemplet.m_MissionReward[i].reward_id, missionTemplet.m_MissionReward[i].reward_value);
			m_iconSlot[i].SetData(data);
		}
		NKMMissionManager.MissionStateData missionStateData = NKMMissionManager.GetMissionStateData(missionTemplet);
		NKCUtil.SetGameobjectActive(m_objClear, missionStateData.state == NKMMissionManager.MissionState.CAN_COMPLETE || missionStateData.state == NKMMissionManager.MissionState.REPEAT_CAN_COMPLETE);
		NKCUtil.SetGameobjectActive(m_objComplete, missionStateData.state == NKMMissionManager.MissionState.COMPLETED || missionStateData.state == NKMMissionManager.MissionState.REPEAT_COMPLETED);
		NKCUtil.SetLabelText(m_lbProgress, $"{missionStateData.progressCount}/{missionTemplet.m_Times}");
		m_bHasCooltime = missionTemplet.m_ResetInterval != NKM_MISSION_RESET_INTERVAL.NONE;
		NKCUtil.SetGameobjectActive(m_lbResetTime, m_bHasCooltime);
		if (m_bHasCooltime && m_objComplete.activeInHierarchy)
		{
			m_NextActivateTimeUTC = GetNextResetTimeUTC();
			UpdateRemainCoolTimeTextUI();
		}
	}

	private DateTime GetNextResetTimeUTC()
	{
		DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime(-1.0);
		NKMTime.TimePeriod timePeriod = NKMTime.TimePeriod.Day;
		if (m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.DAILY)
		{
			timePeriod = NKMTime.TimePeriod.Day;
		}
		else if (m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.WEEKLY)
		{
			timePeriod = NKMTime.TimePeriod.Week;
		}
		else if (m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.MONTHLY)
		{
			timePeriod = NKMTime.TimePeriod.Month;
		}
		return NKMTime.GetNextResetTime(serverUTCTime, timePeriod);
	}

	private void UpdateRemainCoolTimeTextUI()
	{
		if (m_bHasCooltime && m_objComplete.activeInHierarchy)
		{
			if (NKCSynchronizedTime.GetTimeLeft(m_NextActivateTimeUTC).TotalMilliseconds <= 0.0 && !m_bNeedRefresh)
			{
				m_bNeedRefresh = true;
				SetData(m_MissionTemplet);
			}
			NKCUtil.SetLabelText(m_lbResetTime, NKCUtilString.GetRemainTimeStringOneParam(m_NextActivateTimeUTC));
		}
	}

	private void Update()
	{
		if (m_bHasCooltime && base.gameObject.activeSelf && base.gameObject.activeInHierarchy && m_objComplete.activeInHierarchy)
		{
			m_deltaTime += Time.deltaTime;
			if (m_deltaTime > 1f)
			{
				m_deltaTime = 0f;
				UpdateRemainCoolTimeTextUI();
			}
		}
	}
}
