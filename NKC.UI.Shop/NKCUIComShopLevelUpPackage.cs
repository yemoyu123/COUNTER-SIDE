using System.Collections.Generic;
using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Shop;

public class NKCUIComShopLevelUpPackage : MonoBehaviour, IShopPrefab
{
	[Header("레벨업 보상들이 들어갈 SlotDataInjector. Desc에 목표 레벨이 출력됨")]
	public List<NKCUIComSlotDataInjector> m_lstLevelUpReward;

	[Header("목표 레벨 표기에 사용할 스트링 키. 없으면 그냥 숫자만")]
	public string m_strkeyLevel;

	public bool m_bHideLockObject;

	public bool IsHideLockObject()
	{
		return m_bHideLockObject;
	}

	void IShopPrefab.SetData(ShopItemTemplet productTemplet)
	{
		if (productTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (productTemplet.m_PurchaseEventType != PURCHASE_EVENT_REWARD_TYPE.LEVELUP_PACKAGE)
		{
			Debug.LogError("Levelup Package가 아닌 아이템이 사용됨");
		}
		ShopLevelUpPackageGroupTemplet levelUpPackageGroupTemplet = NKCShopManager.GetLevelUpPackageGroupTemplet(productTemplet.m_PurchaseEventValue);
		if (levelUpPackageGroupTemplet == null)
		{
			Debug.LogError($"레벨업 패키지 정보 찾지 못함. id : {productTemplet.m_PurchaseEventValue}");
		}
		IEnumerable<ShopLevelUpPackageGroupData> groupDatas = levelUpPackageGroupTemplet.GetGroupDatas(0, levelUpPackageGroupTemplet.MaxLevelRequire);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		int userLevel = nKMUserData.m_UserLevel;
		int i = 0;
		bool flag = nKMUserData.m_ShopData.GetPurchasedCount(productTemplet) > 0;
		foreach (ShopLevelUpPackageGroupData item in groupDatas)
		{
			foreach (NKMRewardInfo rewardInfo in item.RewardInfos)
			{
				if (rewardInfo.rewardType != NKM_REWARD_TYPE.RT_NONE)
				{
					if (i >= m_lstLevelUpReward.Count)
					{
						Debug.LogError("SlotInjector가 리워드 전체를 표기하기에 부족함!");
						break;
					}
					NKCUIComSlotDataInjector nKCUIComSlotDataInjector = m_lstLevelUpReward[i];
					nKCUIComSlotDataInjector.SetData(rewardInfo);
					NKCUtil.SetGameobjectActive(nKCUIComSlotDataInjector, bValue: true);
					i++;
					if (string.IsNullOrEmpty(m_strkeyLevel))
					{
						NKCUtil.SetLabelText(nKCUIComSlotDataInjector.m_lbDesc, item.LevelRequire.ToString());
					}
					else
					{
						NKCUtil.SetLabelText(nKCUIComSlotDataInjector.m_lbDesc, NKCStringTable.GetString(m_strkeyLevel, item.LevelRequire));
					}
					if (flag && userLevel >= item.LevelRequire)
					{
						nKCUIComSlotDataInjector.m_itemSlot?.SetCompleteMark(bValue: true);
					}
				}
			}
		}
		for (; i < m_lstLevelUpReward.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstLevelUpReward[i], bValue: false);
		}
	}
}
