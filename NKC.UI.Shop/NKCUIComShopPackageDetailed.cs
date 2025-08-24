using System.Collections.Generic;
using NKM;
using NKM.Shop;
using UnityEngine;

namespace NKC.UI.Shop;

public class NKCUIComShopPackageDetailed : MonoBehaviour, IShopPrefab
{
	[Header("패키지에 들어가는 아이템 숫자만큼 NKCUIComSlotDataInjector가 프리팹 안에 있어야 합니다!")]
	public List<NKCUIComSlotDataInjector> m_lstDataSlot;

	public bool m_bHideLockObject;

	public bool bShowError = true;

	public bool IsHideLockObject()
	{
		return m_bHideLockObject;
	}

	public void SetData(ShopItemTemplet productTemplet)
	{
		if (productTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (productTemplet.MiscProductTemplet == null || !productTemplet.MiscProductTemplet.IsPackageItem)
		{
			Debug.LogError($"Product {productTemplet.m_ProductID} is not a Package Item!");
			return;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(productTemplet.m_ItemID);
		if (itemMiscTempletByID.m_RewardGroupID == 0)
		{
			Debug.LogError("no rewardgroup! ID : " + productTemplet.m_ItemID);
			return;
		}
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(itemMiscTempletByID.m_RewardGroupID);
		if (itemMiscTempletByID.m_RewardGroupID != 0 && randomBoxItemTempletList == null)
		{
			Debug.LogError("rewardgroup null! ID : " + itemMiscTempletByID.m_RewardGroupID);
			return;
		}
		NKCUIComSlotDataInjector[] componentsInChildren = GetComponentsInChildren<NKCUIComSlotDataInjector>(includeInactive: true);
		int num = 0;
		NKCUIComSlotDataInjector[] array = componentsInChildren;
		foreach (NKCUIComSlotDataInjector injector in array)
		{
			NKMRandomBoxItemTemplet nKMRandomBoxItemTemplet = randomBoxItemTempletList.Find((NKMRandomBoxItemTemplet x) => x.m_reward_type == injector.m_rewardType && x.m_RewardID == injector.m_rewardID);
			NKCUtil.SetGameobjectActive(injector, nKMRandomBoxItemTemplet != null);
			if (bShowError && nKMRandomBoxItemTemplet == null)
			{
				Debug.LogError($"reward_type {injector.m_rewardType} / reward_id {injector.m_rewardID} 리워드가 패키지 {itemMiscTempletByID.m_ItemMiscID} 안에 없음");
				continue;
			}
			injector.SetData(nKMRandomBoxItemTemplet);
			num++;
		}
		if (bShowError && num != randomBoxItemTempletList.Count)
		{
			Debug.LogError("패키지 내의 아이템 전체를 표기하지 못했음");
		}
	}
}
