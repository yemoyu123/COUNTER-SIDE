using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Shop;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIComShopConsumerPackageInjector : MonoBehaviour, IShopDataInjector
{
	[Serializable]
	public class SlotSet
	{
		public float m_fMarkPosition;

		public GameObject m_objCompleteMark;

		public NKCUIComShopConsumerPackageSlot slot;
	}

	[Header("컨슈머 패키지 관련")]
	public Text m_lbUseCount;

	public Image m_imgGauge;

	public GameObject m_objAllCompleteMark;

	[Multiline]
	public string useCountFormat = "{0}/{1}";

	public List<SlotSet> m_lstSlots;

	private void LoadConsumerPackageData()
	{
		if (!NKMTempletContainer<ConsumerPackageGroupTemplet>.HasValue())
		{
			NKMTempletContainer<ConsumerPackageGroupTemplet>.Load(from e in NKMTempletLoader<ConsumerPackageGroupData>.LoadGroup("AB_SCRIPT", "LUA_ACQ_PACKAGE_TEMPLET", "ACQ_PACKAGE_TEMPLET", ConsumerPackageGroupData.LoadFromLUA)
				select new ConsumerPackageGroupTemplet(e.Key, e.Value), null);
		}
	}

	public void TriggerInjectData(ShopItemTemplet productTemplet)
	{
		LoadConsumerPackageData();
		ConsumerPackageGroupTemplet consumerPackageGroupTemplet = ConsumerPackageGroupTemplet.Find(productTemplet.m_PurchaseEventValue);
		if (productTemplet.m_PurchaseEventType != PURCHASE_EVENT_REWARD_TYPE.CONSUMER_PACKAGE || consumerPackageGroupTemplet == null)
		{
			Debug.LogError($"[ShopTemplet] 소비자 패키지 정보가 존재하지 않음 m_ProductId:{productTemplet.m_ProductID}, m_PurchaseEventValue:{productTemplet.m_PurchaseEventValue}");
			return;
		}
		if (consumerPackageGroupTemplet.MaxLevel > m_lstSlots.Count)
		{
			Debug.LogError($"[ShopTemplet] 소비자 패키지 정보 프리팹에 슬롯 수가 부족함 m_ProductID {productTemplet.m_ProductID}");
			return;
		}
		if (!NKCScenManager.CurrentUserData().GetConsumerPackageData(productTemplet.m_ProductID, out var data))
		{
			data = null;
		}
		long num = data?.spendCount ?? 0;
		long maxLevelRequireValue = consumerPackageGroupTemplet.MaxLevelRequireValue;
		NKCUtil.SetLabelText(m_lbUseCount, string.Format(useCountFormat, num, maxLevelRequireValue));
		NKCUtil.SetImageFillAmount(m_imgGauge, GetCurrentProgress(data, consumerPackageGroupTemplet));
		for (int i = 0; i < m_lstSlots.Count; i++)
		{
			if (m_lstSlots[i] != null)
			{
				int num2 = i + 1;
				bool bValue = data != null && data.rewardedLevel >= num2;
				NKCUtil.SetGameobjectActive(m_lstSlots[i].m_objCompleteMark, bValue);
				m_lstSlots[i].slot.SetData(data, consumerPackageGroupTemplet, num2);
			}
		}
		bool bValue2 = data != null && data.rewardedLevel >= consumerPackageGroupTemplet.MaxLevel;
		NKCUtil.SetGameobjectActive(m_objAllCompleteMark, bValue2);
	}

	private float GetCurrentProgress(NKMConsumerPackageData data, ConsumerPackageGroupTemplet templet)
	{
		if (data == null)
		{
			return 0f;
		}
		if (data.rewardedLevel == templet.MaxLevel)
		{
			return 1f;
		}
		long num = templet.GetRewardData(data.rewardedLevel)?.ConsumeRequireItemValue ?? 0;
		long num2 = templet.GetRewardData(data.rewardedLevel + 1)?.ConsumeRequireItemValue ?? templet.MaxLevelRequireValue;
		float num3 = (float)(data.spendCount - num) / (float)(num2 - num);
		float levelProgress = GetLevelProgress(data.rewardedLevel, 0f);
		float levelProgress2 = GetLevelProgress(data.rewardedLevel + 1, 1f);
		return levelProgress + (levelProgress2 - levelProgress) * num3;
	}

	private float GetLevelProgress(int level, float defaultValue)
	{
		return GetSlot(level)?.m_fMarkPosition ?? defaultValue;
	}

	private SlotSet GetSlot(int level)
	{
		int num = level - 1;
		if (num < 0)
		{
			return null;
		}
		if (num < m_lstSlots.Count)
		{
			return m_lstSlots[num];
		}
		return null;
	}
}
