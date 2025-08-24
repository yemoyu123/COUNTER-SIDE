using System.Collections.Generic;
using NKM;
using NKM.Shop;
using UnityEngine;

namespace NKC.UI.Shop;

public class NKCUIComShopPackageCommon : MonoBehaviour, IShopPrefab
{
	[Header("NKCUIComSlotDataInjector의 설정을 무시하고 패키지의 데이터를 집어넣습니다")]
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
		for (int i = 0; i < m_lstDataSlot.Count; i++)
		{
			NKCUIComSlotDataInjector nKCUIComSlotDataInjector = m_lstDataSlot[i];
			if (!(nKCUIComSlotDataInjector == null))
			{
				if (i < randomBoxItemTempletList.Count)
				{
					NKMRandomBoxItemTemplet data = randomBoxItemTempletList[i];
					NKCUtil.SetGameobjectActive(nKCUIComSlotDataInjector, bValue: true);
					nKCUIComSlotDataInjector.SetData(data);
				}
				else
				{
					NKCUtil.SetGameobjectActive(nKCUIComSlotDataInjector, bValue: false);
				}
			}
		}
	}
}
