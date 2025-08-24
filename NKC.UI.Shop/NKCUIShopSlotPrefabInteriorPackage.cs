using System.Collections.Generic;
using NKC.UI.Component.Office;
using NKC.UI.Office;
using NKM;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;

namespace NKC.UI.Shop;

public class NKCUIShopSlotPrefabInteriorPackage : NKCUIShopSlotPrefab
{
	[Header("가구 관련")]
	public NKCUIComOfficeInteriorDetail m_OfficeInteriorDetail;

	public NKCUIComStateButton m_cstbnDetail;

	private ShopItemTemplet m_shopTemplet;

	private Dictionary<int, int> m_dicPackageInterior;

	public override void Init(OnBuy onBuy, OnRefreshRequired onRefreshRequired)
	{
		base.Init(onBuy, onRefreshRequired);
		NKCUtil.SetButtonClickDelegate(m_cstbnDetail, OnClickDetail);
	}

	protected override void PostSetData(ShopItemTemplet shopTemplet)
	{
		base.PostSetData(shopTemplet);
		if (shopTemplet.m_ItemType != NKM_REWARD_TYPE.RT_MISC)
		{
			NKCUtil.SetGameobjectActive(m_OfficeInteriorDetail, bValue: false);
			NKCUtil.SetGameobjectActive(m_cstbnDetail, bValue: false);
			return;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(shopTemplet.m_ItemID);
		if (!itemMiscTempletByID.IsPackageItem)
		{
			NKCUtil.SetGameobjectActive(m_OfficeInteriorDetail, bValue: false);
			NKCUtil.SetGameobjectActive(m_cstbnDetail, bValue: false);
			return;
		}
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(itemMiscTempletByID.m_RewardGroupID);
		m_dicPackageInterior = new Dictionary<int, int>();
		foreach (NKMRandomBoxItemTemplet item in randomBoxItemTempletList)
		{
			if (item.m_reward_type == NKM_REWARD_TYPE.RT_MISC && NKMOfficeInteriorTemplet.Find(item.m_RewardID) != null)
			{
				if (m_dicPackageInterior.ContainsKey(item.m_RewardID))
				{
					m_dicPackageInterior[item.m_RewardID] += item.TotalQuantity_Max;
				}
				else
				{
					m_dicPackageInterior[item.m_RewardID] = item.TotalQuantity_Max;
				}
			}
		}
		m_OfficeInteriorDetail.SetData(m_dicPackageInterior.Keys);
		NKCUtil.SetGameobjectActive(m_cstbnDetail, m_dicPackageInterior.Count > 0);
	}

	private void OnClickDetail()
	{
		ShopItemTemplet.Find(base.ProductID);
		NKCUIPopupOfficeInteriorSelect.Instance.OpenForListView(m_dicPackageInterior);
	}
}
