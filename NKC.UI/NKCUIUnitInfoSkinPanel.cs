using System.Collections.Generic;
using NKM;
using NKM.Shop;
using NKM.Templet;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitInfoSkinPanel : MonoBehaviour
{
	public delegate void ChangeSkin(int skinID);

	public NKCUISkinSlot m_pfbSkinSlot;

	public RectTransform m_rtSlotRoot;

	public ScrollRect m_srScrollRect;

	public GameObject m_objNoSkin;

	public Text m_lbSkinTitle;

	public Text m_lbSkinDescription;

	public NKCUIComButton m_cbtnEquip;

	public NKCUIComButton m_cbtnBuy;

	public NKCUIComModelTextureRenderer m_TextureRenderer;

	public GameObject m_objLoading;

	private NKCASUnitSpineSprite m_UnitPreview;

	private int m_UnitPreviewOrigLayer;

	private List<NKCUISkinSlot> m_lstSlot = new List<NKCUISkinSlot>();

	private int m_SelectedSkinID;

	private int m_SelectedUnitID;

	private long m_SelectedUnitUID;

	private ChangeSkin dChangeSkin;

	private bool bWaitingForLoading;

	public void Init(ChangeSkin changeSkinIllust)
	{
		dChangeSkin = changeSkinIllust;
		if (null != m_cbtnEquip)
		{
			m_cbtnEquip.PointerClick.RemoveAllListeners();
			m_cbtnEquip.PointerClick.AddListener(OnBtnEquip);
		}
		if (null != m_cbtnBuy)
		{
			m_cbtnBuy.PointerClick.RemoveAllListeners();
			m_cbtnBuy.PointerClick.AddListener(OnBtnBuy);
		}
	}

	public void CleanUp()
	{
		CloseCurrentPreviewModel();
		m_TextureRenderer.CleanUp();
		m_SelectedSkinID = 0;
		m_SelectedUnitID = 0;
		m_SelectedUnitUID = 0L;
	}

	private void CloseCurrentPreviewModel()
	{
		if (m_UnitPreview != null && m_UnitPreview.m_UnitSpineSpriteInstant != null)
		{
			m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform.localScale = Vector3.one;
			NKCUtil.SetLayer(m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform, m_UnitPreviewOrigLayer);
		}
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_UnitPreview);
		m_UnitPreview = null;
	}

	public void SetData(NKMUnitData unitData, bool resetSkin)
	{
		if (unitData == null)
		{
			return;
		}
		m_TextureRenderer.PrepareTexture(null);
		m_SelectedUnitID = unitData.m_UnitID;
		m_SelectedUnitUID = unitData.m_UnitUID;
		if (NKMSkinManager.IsCharacterHasSkin(unitData.m_UnitID))
		{
			NKCUtil.SetGameobjectActive(m_objNoSkin, bValue: false);
			NKCUtil.SetGameobjectActive(m_srScrollRect, bValue: true);
			List<NKMSkinTemplet> skinlistForCharacter = NKMSkinManager.GetSkinlistForCharacter(unitData.m_UnitID, NKCScenManager.CurrentUserData().m_InventoryData);
			SetSkinList(unitData, skinlistForCharacter);
			if (resetSkin)
			{
				SelectSkin(unitData.m_SkinID);
			}
			else
			{
				SelectSkin(m_SelectedSkinID);
			}
		}
		else
		{
			SetDefaultSkin(unitData);
		}
	}

	private void SetDefaultSkin(NKMUnitData unitData = null)
	{
		if (unitData != null)
		{
			SetSlotCount(1);
			if (null != m_lstSlot[0])
			{
				m_lstSlot[0].SetData(NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID), unitData.m_SkinID == 0);
				NKCUtil.SetGameobjectActive(m_lstSlot[0], bValue: true);
				SelectSkin(0);
				NKCUtil.SetLabelText(m_lstSlot[0].m_lbName, NKCUtilString.GET_STRING_BASE);
			}
		}
	}

	private void SetSkinList(NKMUnitData unitData, List<NKMSkinTemplet> lstSkinTemplet)
	{
		SetSlotCount(lstSkinTemplet.Count + 1);
		m_lstSlot[0].SetData(NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID), unitData.m_SkinID == 0);
		NKCUtil.SetGameobjectActive(m_lstSlot[0], bValue: true);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		for (int i = 0; i < lstSkinTemplet.Count; i++)
		{
			NKMSkinTemplet nKMSkinTemplet = lstSkinTemplet[i];
			NKCUISkinSlot nKCUISkinSlot = m_lstSlot[i + 1];
			nKCUISkinSlot.SetData(nKMSkinTemplet, myUserData.m_InventoryData.HasItemSkin(nKMSkinTemplet.m_SkinID), unitData.m_SkinID == nKMSkinTemplet.m_SkinID);
			NKCUtil.SetGameobjectActive(nKCUISkinSlot, bValue: true);
		}
		NKCUtil.SetLabelText(m_lstSlot[0].m_lbName, NKCUtilString.GET_STRING_BASE);
	}

	private void SetBottomButton(bool skinOwned, bool equipped)
	{
		NKCUtil.SetGameobjectActive(m_cbtnEquip, skinOwned);
		NKCUtil.SetGameobjectActive(m_cbtnBuy, !skinOwned);
		if (skinOwned)
		{
			if (equipped)
			{
				m_cbtnEquip?.Lock();
			}
			else
			{
				m_cbtnEquip?.UnLock();
			}
			return;
		}
		ShopItemTemplet shopTempletBySkinID = NKCShopManager.GetShopTempletBySkinID(m_SelectedSkinID);
		if (shopTempletBySkinID != null)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (NKCShopManager.CanBuyFixShop(myUserData, shopTempletBySkinID, out var _, out var _) == NKM_ERROR_CODE.NEC_OK)
			{
				m_cbtnBuy?.UnLock();
			}
			else
			{
				m_cbtnBuy?.Lock();
			}
		}
		else
		{
			m_cbtnBuy?.Lock();
		}
	}

	private void SelectSkin(int skinID)
	{
		m_SelectedSkinID = skinID;
		foreach (NKCUISkinSlot item in m_lstSlot)
		{
			item.SetSelected(item.SkinID == skinID);
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(m_SelectedUnitUID);
		bool equipped = unitFromUID != null && unitFromUID.m_SkinID == skinID;
		bool skinOwned = myUserData.m_InventoryData.HasItemSkin(skinID);
		SetBottomButton(skinOwned, equipped);
		if (skinID == 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_SelectedUnitID);
			if (unitTempletBase != null)
			{
				NKCUtil.SetLabelText(m_lbSkinTitle, unitTempletBase.GetUnitTitle());
			}
			NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(m_SelectedUnitID);
			if (unitTemplet != null)
			{
				NKCUtil.SetLabelText(m_lbSkinDescription, unitTemplet.GetUnitIntro());
			}
			else
			{
				NKCUtil.SetLabelText(m_lbSkinDescription, NKCUtilString.GET_STRING_BASE_SKIN);
			}
		}
		else
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
			if (skinTemplet != null)
			{
				NKCUtil.SetLabelText(m_lbSkinTitle, skinTemplet.GetTitle());
				NKCUtil.SetLabelText(m_lbSkinDescription, skinTemplet.GetSkinDesc());
			}
		}
		if (unitFromUID != null)
		{
			SetPreviewBattleUnit(unitFromUID.m_UnitID, skinID);
		}
		if (dChangeSkin != null)
		{
			dChangeSkin(skinID);
		}
	}

	private void SetPreviewBattleUnit(int unitID, int skinID)
	{
		CloseCurrentPreviewModel();
		if (skinID == 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
			m_UnitPreview = NKCUnitViewer.OpenUnitViewerSpineSprite(unitTempletBase, bSub: false, bAsync: true);
			bWaitingForLoading = true;
		}
		else
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
			m_UnitPreview = NKCUnitViewer.OpenUnitViewerSpineSprite(skinTemplet, bSub: false, bAsync: true);
			bWaitingForLoading = true;
		}
		NKCUtil.SetGameobjectActive(m_TextureRenderer, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLoading, bWaitingForLoading);
	}

	private void Update()
	{
		if (bWaitingForLoading && m_UnitPreview != null && m_UnitPreview.m_bIsLoaded)
		{
			AfterUnitLoadComplete();
		}
	}

	private void AfterUnitLoadComplete()
	{
		bWaitingForLoading = false;
		if (m_UnitPreview != null && m_UnitPreview.m_UnitSpineSpriteInstant != null)
		{
			m_UnitPreviewOrigLayer = m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.layer;
			NKCUtil.SetLayer(m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform, 31);
			m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform.SetParent(m_TextureRenderer.transform, worldPositionStays: false);
			m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform.localPosition = new Vector3(0f, 30f, 0f);
			float num = m_TextureRenderer.m_rtImage.GetHeight() / 300f;
			m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform.localScale = Vector3.one * num;
			Transform transform = m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform.Find("SPINE_SkeletonAnimation");
			if (transform != null)
			{
				SkeletonAnimation component = transform.GetComponent<SkeletonAnimation>();
				if (component != null)
				{
					component.AnimationState.SetAnimation(0, "ASTAND", loop: true);
				}
				NKCUtil.SetGameobjectActive(m_TextureRenderer, bValue: true);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_TextureRenderer, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objLoading, bValue: false);
	}

	public void OnSkinEquip(long unitUID, int equippedSkinID)
	{
		if (unitUID == m_SelectedUnitUID)
		{
			foreach (NKCUISkinSlot item in m_lstSlot)
			{
				item.SetEquipped(item.SkinID == equippedSkinID);
			}
		}
		SelectSkin(equippedSkinID);
	}

	public void OnSkinBuy(int skinID)
	{
		NKCUISkinSlot nKCUISkinSlot = m_lstSlot.Find((NKCUISkinSlot x) => x.SkinID == skinID);
		if (nKCUISkinSlot != null)
		{
			nKCUISkinSlot.SetOwned(bValue: true);
		}
		SelectSkin(skinID);
	}

	private void SetSlotCount(int count)
	{
		while (m_lstSlot.Count < count)
		{
			NKCUISkinSlot nKCUISkinSlot = Object.Instantiate(m_pfbSkinSlot);
			nKCUISkinSlot.transform.SetParent(m_rtSlotRoot, worldPositionStays: false);
			nKCUISkinSlot.Init(SelectSkin);
			m_lstSlot.Add(nKCUISkinSlot);
		}
		for (int i = count; i < m_lstSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstSlot[i], bValue: false);
		}
	}

	private void OnBtnEquip()
	{
		if (!(null == m_cbtnEquip) && !m_cbtnEquip.m_bLock)
		{
			NKCPacketSender.Send_NKMPacket_SET_UNIT_SKIN_REQ(m_SelectedUnitUID, m_SelectedSkinID);
		}
	}

	private void OnBtnBuy()
	{
		if (null == m_cbtnBuy || m_cbtnBuy.m_bLock)
		{
			return;
		}
		ShopItemTemplet shopTemplet = NKCShopManager.GetShopTempletBySkinID(m_SelectedSkinID);
		if (shopTemplet == null)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_SKIN_LOCK);
			return;
		}
		NKCPopupItemBox.Instance.Open(shopTemplet, delegate
		{
			TryProductBuy(shopTemplet.m_ProductID);
		});
	}

	private void TryProductBuy(int ProductID)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(ProductID);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData.HaveEnoughResourceToBuy(shopItemTemplet, 1))
		{
			if (shopItemTemplet.m_PriceItemID == 0)
			{
				NKCPacketSender.Send_NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ(shopItemTemplet.m_MarketID);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_SHOP_FIX_SHOP_BUY_REQ(ProductID, 1);
			}
		}
		else
		{
			NKCShopManager.OpenItemLackPopup(shopItemTemplet.m_PriceItemID, myUserData.m_ShopData.GetRealPrice(shopItemTemplet));
		}
	}
}
