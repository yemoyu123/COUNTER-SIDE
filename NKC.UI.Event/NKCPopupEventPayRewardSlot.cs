using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventPayRewardSlot : MonoBehaviour
{
	public delegate void OnMissionComplete();

	public delegate void OnSetMissionState(bool cleared);

	public List<NKCUISlot> m_lstRewardSlot;

	public Text m_missionDesc;

	public NKCUIComStateButton m_slotButton;

	public GameObject m_objClearRoot;

	public GameObject m_objCompleteRoot;

	public Image m_progress;

	public Image m_payIcon;

	private NKCAssetInstanceData m_InstanceData;

	private OnMissionComplete m_dOnMissionComplete;

	private OnSetMissionState m_dOnSetMissionState;

	public static NKCPopupEventPayRewardSlot GetNewInstance(Transform parent, string bundleName, string assetName)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCPopupEventPayRewardSlot nKCPopupEventPayRewardSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCPopupEventPayRewardSlot>();
		if (nKCPopupEventPayRewardSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCPopupEventPayRewardSlot Prefab null!");
			return null;
		}
		nKCPopupEventPayRewardSlot.m_InstanceData = nKCAssetInstanceData;
		nKCPopupEventPayRewardSlot.Init();
		if (parent != null)
		{
			nKCPopupEventPayRewardSlot.transform.SetParent(parent);
		}
		nKCPopupEventPayRewardSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCPopupEventPayRewardSlot.gameObject.SetActive(value: false);
		return nKCPopupEventPayRewardSlot;
	}

	public void DestoryInstance()
	{
		m_dOnMissionComplete = null;
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	public void Init()
	{
		foreach (NKCUISlot item in m_lstRewardSlot)
		{
			if (!(item == null))
			{
				item.Init();
			}
		}
		NKCUtil.SetButtonClickDelegate(m_slotButton, OnClickSlot);
	}

	public void SetData(NKMMissionTemplet missionTemplet, float progress, OnSetMissionState onSetMissionState, OnMissionComplete onMissionComplete, NKCUISlot.OnClick onRewardIconClick)
	{
		m_dOnMissionComplete = onMissionComplete;
		m_dOnSetMissionState = onSetMissionState;
		NKMMissionManager.MissionState state = NKMMissionManager.GetMissionStateData(missionTemplet).state;
		bool flag = state == NKMMissionManager.MissionState.CAN_COMPLETE || state == NKMMissionManager.MissionState.REPEAT_CAN_COMPLETE;
		bool flag2 = state == NKMMissionManager.MissionState.REPEAT_COMPLETED || state == NKMMissionManager.MissionState.COMPLETED;
		m_slotButton.SetLock(!flag);
		NKCUtil.SetGameobjectActive(m_objClearRoot, flag && !flag2);
		NKCUtil.SetGameobjectActive(m_objCompleteRoot, flag2);
		string text = null;
		if (missionTemplet != null)
		{
			text = missionTemplet.GetDesc();
			for (int i = 0; i < m_lstRewardSlot.Count; i++)
			{
				NKCUISlot nKCUISlot = m_lstRewardSlot[i];
				if (!(nKCUISlot == null))
				{
					if (missionTemplet.m_MissionReward.Count > i && missionTemplet.m_MissionReward[i] != null)
					{
						NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
						MissionReward missionReward = missionTemplet.m_MissionReward[i];
						NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(missionReward.reward_type, missionReward.reward_id, missionReward.reward_value);
						nKCUISlot.SetData(data, bEnableLayoutElement: true, onRewardIconClick);
					}
					else
					{
						NKCUtil.SetGameobjectActive(nKCUISlot, bValue: false);
					}
				}
			}
			if (missionTemplet.m_MissionCond.value1 != null && missionTemplet.m_MissionCond.value1.Count > 0)
			{
				Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(missionTemplet.m_MissionCond.value1[0]);
				NKCUtil.SetImageSprite(m_payIcon, orLoadMiscItemSmallIcon);
			}
		}
		NKCUtil.SetLabelText(m_missionDesc, string.IsNullOrEmpty(text) ? "" : text);
		NKCUtil.SetImageFillAmount(m_progress, progress);
		if (m_dOnSetMissionState != null)
		{
			m_dOnSetMissionState(flag2);
		}
	}

	private void OnClickSlot()
	{
		if (m_dOnMissionComplete != null)
		{
			m_dOnMissionComplete();
		}
	}
}
