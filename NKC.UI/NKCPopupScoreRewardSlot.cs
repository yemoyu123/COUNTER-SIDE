using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupScoreRewardSlot : MonoBehaviour
{
	public enum POINT_REWARD_SLOT_TYPE
	{
		DISABLE,
		CAN_RECEVIE,
		COMPLETE
	}

	public delegate void OnClickRewardSlot(int rewardId);

	public NKCComText m_Desc;

	public List<NKCUISlot> m_lstRewardSlot;

	public NKCUIComStateButton m_BUTTON_COMPLETE;

	public GameObject m_COMPLETE_LINE;

	public GameObject m_BUTTON_COMPLETE_GET;

	public GameObject m_BUTTON_COMPLETE_DISABLE;

	public OnClickRewardSlot m_dOnClickReward;

	private int m_iTargetPointRewardID;

	private NKCAssetInstanceData m_InstanceData;

	public static NKCPopupScoreRewardSlot GetNewInstance(string bundleName, string assetName, Transform parent, OnClickRewardSlot dOnClickReward)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCPopupScoreRewardSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCPopupScoreRewardSlot>();
		if (component == null)
		{
			Debug.LogError("NKCPopupScoreRewardSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.GetComponent<RectTransform>().localScale = Vector3.one;
			component.Init(dOnClickReward);
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	private void Init(OnClickRewardSlot dOnClickReward)
	{
		m_dOnClickReward = dOnClickReward;
		NKCUtil.SetBindFunction(m_BUTTON_COMPLETE, OnClickReward);
		foreach (NKCUISlot item in m_lstRewardSlot)
		{
			item?.Init();
		}
	}

	public void SetData(int pointRewardID, string pointDescStrID, List<NKCUISlot.SlotData> lstRewardSlotData, POINT_REWARD_SLOT_TYPE type)
	{
		if (pointRewardID <= 0)
		{
			return;
		}
		m_iTargetPointRewardID = pointRewardID;
		string msg = NKCStringTable.GetString(pointDescStrID);
		NKCUtil.SetLabelText(m_Desc, msg);
		for (int i = 0; i < m_lstRewardSlot.Count; i++)
		{
			if (lstRewardSlotData != null && lstRewardSlotData.Count > i)
			{
				m_lstRewardSlot[i].SetData(lstRewardSlotData[i]);
				NKCUtil.SetGameobjectActive(m_lstRewardSlot[i], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstRewardSlot[i], bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_BUTTON_COMPLETE, type == POINT_REWARD_SLOT_TYPE.CAN_RECEVIE);
		NKCUtil.SetGameobjectActive(m_COMPLETE_LINE, type == POINT_REWARD_SLOT_TYPE.CAN_RECEVIE);
		NKCUtil.SetGameobjectActive(m_BUTTON_COMPLETE_GET, type == POINT_REWARD_SLOT_TYPE.COMPLETE);
		NKCUtil.SetGameobjectActive(m_BUTTON_COMPLETE_DISABLE, type == POINT_REWARD_SLOT_TYPE.DISABLE);
	}

	private void OnClickReward()
	{
		m_dOnClickReward?.Invoke(m_iTargetPointRewardID);
	}
}
