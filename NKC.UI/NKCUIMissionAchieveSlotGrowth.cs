using System;
using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIMissionAchieveSlotGrowth : MonoBehaviour
{
	public delegate void OnClickMASlot(NKCUIMissionAchieveSlotGrowth cNKCUIMissionAchieveSlotGrowth);

	public GameObject m_objComplete;

	public GameObject m_objLock;

	public Image m_imgMissionThumbnail;

	public Text m_lbMissionNum;

	public Text m_lbMissionTitle;

	public Text m_lbMissionDesc;

	public Text m_lbProgress;

	public Slider m_sliderProgress;

	public GameObject m_objMissionTip;

	public Text m_lbMissionTip;

	public List<NKCUISlot> m_lstRewardSlot;

	public NKCUIComStateButton m_btnProgress;

	public NKCUIComStateButton m_btnComplete;

	public NKCUIComStateButton m_btnDisable;

	public GameObject m_objOutline;

	private NKMMissionTemplet m_MissionTemplet;

	private NKMMissionData m_MissionData;

	private NKMMissionManager.MissionStateData m_MissionUIData;

	private OnClickMASlot m_OnClickMASlotMove;

	private OnClickMASlot m_OnClickMASlotComplete;

	private NKCAssetInstanceData m_InstanceData;

	public NKMMissionTemplet GetNKMMissionTemplet()
	{
		return m_MissionTemplet;
	}

	public static NKCUIMissionAchieveSlotGrowth GetNewInstance(Transform parent, OnClickMASlot OnClickMASlotMove = null, OnClickMASlot OnClickMASlotComplete = null)
	{
		return GetNewInstance(parent, "AB_UI_NKM_UI_MISSION", "NKM_UI_MISSION_GROWTH_LIST_SLOT", OnClickMASlotMove, OnClickMASlotComplete);
	}

	public static NKCUIMissionAchieveSlotGrowth GetNewInstance(Transform parent, string BundleName, string AssetName, OnClickMASlot OnClickMASlotMove = null, OnClickMASlot OnClickMASlotComplete = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(BundleName, AssetName);
		NKCUIMissionAchieveSlotGrowth component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIMissionAchieveSlotGrowth>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIMissionAchieveSlotGrowth Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		component.m_btnProgress.PointerClick.RemoveAllListeners();
		component.m_btnProgress.PointerClick.AddListener(component.OnClickMove);
		component.m_btnComplete.PointerClick.RemoveAllListeners();
		component.m_btnComplete.PointerClick.AddListener(component.OnClickComplete);
		component.m_btnDisable.PointerClick.RemoveAllListeners();
		component.m_btnDisable.PointerClick.AddListener(component.OnClickMove);
		component.m_OnClickMASlotMove = OnClickMASlotMove;
		component.m_OnClickMASlotComplete = OnClickMASlotComplete;
		for (int i = 0; i < component.m_lstRewardSlot.Count; i++)
		{
			if (component.m_lstRewardSlot[i] != null)
			{
				component.m_lstRewardSlot[i].Init();
			}
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SetData(NKMMissionTemplet cNKMMissionTemplet)
	{
		if (cNKMMissionTemplet == null)
		{
			return;
		}
		bool flag = false;
		if (m_MissionTemplet == cNKMMissionTemplet)
		{
			flag = true;
		}
		m_MissionTemplet = cNKMMissionTemplet;
		m_MissionData = NKMMissionManager.GetMissionData(cNKMMissionTemplet);
		m_MissionUIData = NKMMissionManager.GetMissionStateData(cNKMMissionTemplet);
		NKCUtil.SetImageSprite(m_imgMissionThumbnail, NKCUtil.GetMissionThumbnailSprite(m_MissionTemplet.m_MissionTabId));
		NKCUtil.SetLabelText(m_lbMissionTitle, m_MissionTemplet.GetTitle());
		NKCUtil.SetLabelText(m_lbMissionDesc, m_MissionTemplet.GetDesc());
		if (!string.IsNullOrEmpty(m_MissionTemplet.m_MissionTip))
		{
			NKCUtil.SetLabelText(m_lbMissionTip, m_MissionTemplet.GetTip());
		}
		int num = NKMMissionManager.GetMissionTempletListByType(m_MissionTemplet.m_MissionTabId).FindIndex((NKMMissionTemplet x) => x == m_MissionTemplet);
		NKCUtil.SetLabelText(m_lbMissionNum, NKCUtilString.GET_STRING_MISSION_ONE_PARAM, num + 1);
		if (!flag)
		{
			for (int num2 = 0; num2 < m_MissionTemplet.m_MissionReward.Count; num2++)
			{
				MissionReward missionReward = m_MissionTemplet.m_MissionReward[num2];
				m_lstRewardSlot[num2].SetData(NKCUISlot.SlotData.MakeRewardTypeData(missionReward.reward_type, missionReward.reward_id, missionReward.reward_value));
				m_lstRewardSlot[num2].SetActive(bSet: true);
			}
			for (int num3 = m_MissionTemplet.m_MissionReward.Count; num3 < m_lstRewardSlot.Count; num3++)
			{
				m_lstRewardSlot[num3].SetActive(bSet: false);
			}
		}
		NKMMissionManager.CheckCanReset(m_MissionTemplet.m_ResetInterval, m_MissionData);
		bool isMissionCanClear = m_MissionUIData.IsMissionCanClear;
		NKCUtil.SetGameobjectActive(m_objMissionTip, m_MissionUIData.IsMissionOngoing && !string.IsNullOrEmpty(m_MissionTemplet.m_MissionTip));
		NKCUtil.SetGameobjectActive(m_objOutline, m_MissionUIData.IsMissionOngoing);
		NKCUtil.SetGameobjectActive(m_btnProgress, m_MissionTemplet.m_ShortCutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE && m_MissionUIData.IsMissionOngoing && !isMissionCanClear);
		NKCUtil.SetGameobjectActive(m_objComplete, m_MissionUIData.IsMissionCompleted);
		NKCUtil.SetGameobjectActive(m_btnComplete, isMissionCanClear);
		NKCUtil.SetGameobjectActive(m_objLock, m_MissionUIData.IsLocked);
		NKCUtil.SetGameobjectActive(m_btnDisable, m_MissionUIData.IsLocked);
		long progressCount = m_MissionUIData.progressCount;
		NKCUtil.SetLabelText(m_lbProgress, $"{Math.Min(m_MissionTemplet.m_Times, progressCount)} / {m_MissionTemplet.m_Times}");
		m_sliderProgress.value = (float)progressCount / (float)m_MissionTemplet.m_Times;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void OnClickMove()
	{
		if (m_btnDisable.gameObject.activeSelf)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_MISSION_UNAVAILABLE);
		}
		else
		{
			m_OnClickMASlotMove?.Invoke(this);
		}
	}

	public void OnClickComplete()
	{
		m_OnClickMASlotComplete?.Invoke(this);
	}
}
