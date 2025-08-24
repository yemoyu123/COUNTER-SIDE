using NKM;
using NKM.Shop;
using UnityEngine;

namespace NKC.UI.Shop;

public class NKCUIShopSlotPreview : NKCUIShopSlotCard
{
	[Header("슬롯")]
	public NKCUISlot m_Slot;

	protected override bool IsProductAvailable(ShopItemTemplet shopTemplet, out bool bAdmin, bool bIncludeLockedItemWithReason)
	{
		bAdmin = false;
		return true;
	}

	protected override void SetGoodsImage(ShopItemTemplet shopTemplet, bool bFirstBuy)
	{
		string assetName = (string.IsNullOrEmpty(m_OverrideImageAsset) ? shopTemplet.m_CardImage : m_OverrideImageAsset);
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("AB_UI_NKM_UI_SHOP_IMG", assetName));
		NKCUtil.SetGameobjectActive(m_imgItem, orLoadAssetResource != null);
		NKCUtil.SetGameobjectActive(m_Slot, orLoadAssetResource == null);
		if (orLoadAssetResource != null)
		{
			m_imgItem.sprite = orLoadAssetResource;
		}
		else
		{
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeShopItemData(shopTemplet, bFirstBuy);
			bool bShowNumber = slotData.eType == NKCUISlot.eSlotMode.ItemMisc || slotData.eType == NKCUISlot.eSlotMode.Mold;
			m_Slot.SetData(slotData, bShowName: false, bShowNumber, bEnableLayoutElement: false, OnSlotClick);
		}
		if (m_lbDescription != null)
		{
			m_lbDescription.text = NKCUtilString.GetShopDescriptionText(shopTemplet.GetItemDesc(), bFirstBuy);
		}
	}

	private void OnSlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnBtnBuy();
	}
}
