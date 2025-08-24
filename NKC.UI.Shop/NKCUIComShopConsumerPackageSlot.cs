using System;
using System.Collections.Generic;
using ClientPacket.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIComShopConsumerPackageSlot : MonoBehaviour
{
	[Serializable]
	public class RewardData
	{
		public GameObject m_objRoot;

		public Image m_imgReward;

		public Text m_lbRewardCount;
	}

	public Image m_imgReq;

	public Text m_lbReqCount;

	public List<RewardData> m_lstRewardDatas;

	public GameObject m_objCompleted;

	public void SetData(NKMConsumerPackageData data, ConsumerPackageGroupTemplet templet, int level)
	{
		bool bValue = data != null && data.rewardedLevel >= level;
		NKCUtil.SetGameobjectActive(m_objCompleted, bValue);
		ConsumerPackageGroupData rewardData = templet.GetRewardData(level);
		if (m_imgReq != null)
		{
			NKCUtil.SetImageSprite(m_imgReq, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(rewardData.ConsumeRequireItemId));
		}
		NKCUtil.SetLabelText(m_lbReqCount, rewardData.ConsumeRequireItemValue.ToString("#,##0"));
		if (rewardData.RewardInfos.Count > m_lstRewardDatas.Count)
		{
			Debug.LogError($"Reward 종류가 슬롯이 가진 표시 수보다 많음! ConsumerPackageGroupTemplet : {templet.ShopTemplet.m_ProductID}, level : {level}");
		}
		for (int i = 0; i < m_lstRewardDatas.Count; i++)
		{
			if (rewardData.RewardInfos.Count <= i)
			{
				NKCUtil.SetGameobjectActive(m_lstRewardDatas[i].m_objRoot, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_lstRewardDatas[i].m_objRoot, bValue: true);
			NKCUtil.SetImageSprite(m_lstRewardDatas[i].m_imgReward, NKCResourceUtility.GetRewardInvenIcon(rewardData.RewardInfos[i]));
			NKCUtil.SetLabelText(m_lstRewardDatas[i].m_lbRewardCount, rewardData.RewardInfos[i].Count.ToString());
		}
	}
}
