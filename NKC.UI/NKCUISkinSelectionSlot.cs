using Cs.Logging;
using NKC.UI.Shop;
using NKM;
using UnityEngine;

namespace NKC.UI;

[RequireComponent(typeof(NKCUIShopSlotSkin))]
public class NKCUISkinSelectionSlot : MonoBehaviour
{
	public delegate void OnClick(int skinId);

	public GameObject m_objHaveSkin;

	public NKCUIComButton m_cbtnSlotButton;

	private NKCUIShopSlotSkin m_NKCUIShopSlotSkin;

	private int m_skinId;

	private OnClick m_dOnClickSlot;

	public void Init()
	{
		TryGetComponent<NKCUIShopSlotSkin>(out m_NKCUIShopSlotSkin);
		NKCUtil.SetButtonClickDelegate(m_cbtnSlotButton, OnClickSlot);
	}

	public void SetData(NKMSkinTemplet skinTemplet, bool haveSkin, OnClick onClickSlot)
	{
		m_skinId = 0;
		m_dOnClickSlot = onClickSlot;
		if (m_NKCUIShopSlotSkin == null)
		{
			Log.Error("NKCUISkinSelectionSlot needs NKCUIShopSlotSkin", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUISkinSelectionSlot.cs", 33);
			return;
		}
		if (skinTemplet == null)
		{
			Log.Error("SkinTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUISkinSelectionSlot.cs", 39);
			return;
		}
		m_skinId = skinTemplet.m_SkinID;
		m_NKCUIShopSlotSkin.SetNameText(skinTemplet.GetTitle());
		Sprite slotCardItemImage = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, skinTemplet);
		m_NKCUIShopSlotSkin.SetSlotCardItemImage(slotCardItemImage);
		m_NKCUIShopSlotSkin.SetGoodImageFromSkinData(skinTemplet);
		NKCUtil.SetGameobjectActive(m_objHaveSkin, haveSkin);
	}

	private void OnClickSlot()
	{
		if (m_dOnClickSlot != null)
		{
			m_dOnClickSlot(m_skinId);
		}
	}
}
