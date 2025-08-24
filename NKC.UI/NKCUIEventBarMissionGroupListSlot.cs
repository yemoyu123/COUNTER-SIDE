using System;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEventBarMissionGroupListSlot : MonoBehaviour
{
	public Text m_lbMissionTitle;

	public Text m_lbMissionDesc;

	public Text m_lbProgess;

	public Text m_lbResetTime;

	public NKCUISlot[] m_RewardSlot;

	public GameObject m_objComplete;

	public NKCUIComStateButton m_csbtnSlot;

	private NKM_MISSION_RESET_INTERVAL m_resetInterval;

	private NKCAssetInstanceData m_InstanceData;

	private float m_updateTimer;

	public static NKCUIEventBarMissionGroupListSlot GetNewInstance(Transform parent, string bundleName, string assetName)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCUIEventBarMissionGroupListSlot nKCUIEventBarMissionGroupListSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUIEventBarMissionGroupListSlot>();
		if (nKCUIEventBarMissionGroupListSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIComMissionGroupListSlot Prefab null!");
			return null;
		}
		nKCUIEventBarMissionGroupListSlot.m_InstanceData = nKCAssetInstanceData;
		nKCUIEventBarMissionGroupListSlot.Init();
		if (parent != null)
		{
			nKCUIEventBarMissionGroupListSlot.transform.SetParent(parent);
		}
		nKCUIEventBarMissionGroupListSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUIEventBarMissionGroupListSlot.gameObject.SetActive(value: false);
		return nKCUIEventBarMissionGroupListSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (!(m_objComplete == null) && m_objComplete.activeSelf && !(m_lbResetTime == null) && m_lbResetTime.gameObject.activeSelf)
		{
			if (m_updateTimer > 1f)
			{
				NKMUserData userData = NKCScenManager.CurrentUserData();
				DateTime nextResetTime = NKMTime.GetNextResetTime(GetLoginMissionLastUpdateDateUTC(userData), m_resetInterval);
				NKCUtil.SetLabelText(m_lbResetTime, NKCUtilString.GetRemainTimeStringOneParam(nextResetTime));
				m_updateTimer = 0f;
			}
			m_updateTimer += Time.deltaTime;
		}
	}

	private void Init()
	{
		if (m_RewardSlot != null)
		{
			int num = m_RewardSlot.Length;
			for (int i = 0; i < num; i++)
			{
				m_RewardSlot[i].Init();
			}
		}
	}

	public void SetData(NKMMissionTemplet missionTemplet)
	{
		if (missionTemplet == null)
		{
			return;
		}
		m_resetInterval = missionTemplet.m_ResetInterval;
		NKCUtil.SetLabelText(m_lbMissionTitle, missionTemplet.GetTitle());
		NKCUtil.SetLabelText(m_lbMissionDesc, missionTemplet.GetDesc());
		int count = missionTemplet.m_MissionReward.Count;
		int num = m_RewardSlot.Length;
		for (int i = 0; i < num; i++)
		{
			if (i >= count)
			{
				NKCUtil.SetGameobjectActive(m_RewardSlot[i], bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_RewardSlot[i], bValue: true);
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(missionTemplet.m_MissionReward[i].reward_id, missionTemplet.m_MissionReward[i].reward_value);
			m_RewardSlot[i].SetData(data);
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKMMissionData nKMMissionData = null;
		if (nKMUserData != null)
		{
			nKMMissionData = NKCScenManager.CurrentUserData().m_MissionData.GetMissionData(missionTemplet);
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId);
		bool flag = missionTabTemplet != null && missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.POINT_EXCHANGE;
		bool flag2 = false;
		if (nKMMissionData != null)
		{
			flag2 = nKMMissionData.mission_id > missionTemplet.m_MissionID || (nKMMissionData.mission_id == missionTemplet.m_MissionID && nKMMissionData.IsComplete);
			NKCUtil.SetGameobjectActive(m_lbResetTime, m_resetInterval == NKM_MISSION_RESET_INTERVAL.DAILY || m_resetInterval == NKM_MISSION_RESET_INTERVAL.WEEKLY || m_resetInterval == NKM_MISSION_RESET_INTERVAL.MONTHLY);
			if (flag)
			{
				bool flag3 = missionTemplet.m_MissionCond.mission_cond == NKM_MISSION_COND.LOGIN_DAYS && (nKMMissionData.mission_id > missionTemplet.m_MissionID || nKMMissionData.times >= missionTemplet.m_Times);
				flag2 = flag2 || (flag && flag3);
			}
			if (flag2 && m_lbResetTime != null && m_lbResetTime.gameObject.activeSelf)
			{
				DateTime nextResetTime = NKMTime.GetNextResetTime(GetLoginMissionLastUpdateDateUTC(nKMUserData), missionTemplet.m_ResetInterval);
				NKCUtil.SetLabelText(m_lbResetTime, NKCUtilString.GetRemainTimeStringOneParam(nextResetTime));
			}
			NKCUtil.SetGameobjectActive(m_objComplete, flag2);
			if (flag && missionTemplet.m_MissionCond.mission_cond != NKM_MISSION_COND.LOGIN_DAYS)
			{
				NKCUtil.SetLabelText(m_lbProgess, $"{nKMMissionData.times % missionTemplet.m_Times}/{missionTemplet.m_Times}");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbProgess, $"{nKMMissionData.times}/{missionTemplet.m_Times}");
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
			NKCUtil.SetLabelText(m_lbProgess, $"{0}/{missionTemplet.m_Times}");
		}
	}

	private DateTime GetLoginMissionLastUpdateDateUTC(NKMUserData userData)
	{
		if (userData == null)
		{
			return NKCSynchronizedTime.GetServerUTCTime();
		}
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(10210);
		return userData.m_MissionData.GetMissionData(missionTemplet)?.LastUpdateDate ?? NKCSynchronizedTime.GetServerUTCTime();
	}
}
