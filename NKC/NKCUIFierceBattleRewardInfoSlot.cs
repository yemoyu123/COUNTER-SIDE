using System.Collections.Generic;
using NKC.UI;
using NKC.UI.Fierce;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIFierceBattleRewardInfoSlot : MonoBehaviour
{
	private NKCAssetInstanceData m_InstanceData;

	public Text m_Desc;

	public List<NKCUISlot> m_lstReward;

	public static NKCUIFierceBattleRewardInfoSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_LEADER_BOARD_DETAIL", "AB_UI_LEADER_BOARD_DETAIL_REWARD_INFO_SLOT");
		NKCUIFierceBattleRewardInfoSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIFierceBattleRewardInfoSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIFierceBattleRewardInfoSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.GetComponent<RectTransform>().localScale = Vector3.one;
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

	public void SetData(NKCUIPopupFierceBattleRewardInfo.RankUIRewardData data)
	{
		if (data == null || data.RankRewardID == 0)
		{
			return;
		}
		NKCUtil.SetLabelText(m_Desc, NKCStringTable.GetString(data.RankDescStrID));
		for (int i = 0; i < m_lstReward.Count; i++)
		{
			if (!(m_lstReward[i] == null))
			{
				if (data.Rewards.Count <= i)
				{
					NKCUtil.SetGameobjectActive(m_lstReward[i], bValue: false);
					continue;
				}
				if (data.Rewards[i] == null)
				{
					NKCUtil.SetGameobjectActive(m_lstReward[i], bValue: false);
					continue;
				}
				NKCUtil.SetGameobjectActive(m_lstReward[i], bValue: true);
				m_lstReward[i].Init();
				NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
				nKMRewardInfo.rewardType = data.Rewards[i].RewardType;
				nKMRewardInfo.ID = data.Rewards[i].RewardID;
				nKMRewardInfo.Count = data.Rewards[i].RewardQuantity;
				m_lstReward[i].SetData(NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardInfo));
			}
		}
	}
}
