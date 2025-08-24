using System;
using System.Collections.Generic;
using DG.Tweening;
using NKC.UI.Event;
using NKM;
using NKM.Shop;
using NKM.Templet;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShopSkinPopup : NKCUIBase
{
	private enum Mode
	{
		ForShop,
		ForUnitInfo,
		ForSkinInfo
	}

	public delegate void OnApplySkin(int skinId);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_shop_skin";

	private const string UI_ASSET_NAME = "NKM_UI_SHOP_SKIN";

	private static NKCUIShopSkinPopup m_Instance;

	private List<int> DEFAULT_RESOURCE_LIST = new List<int> { 1, 2, 101, 102 };

	private List<int> m_lstUpsideMenuShowResource = new List<int>();

	private bool m_bShowUpsideMenu = true;

	private Mode m_eMode;

	[Header("오른쪽 UI 루트")]
	public RectTransform m_rtRightUI;

	[Header("캐릭터 뷰")]
	public NKCUICharacterView m_CharacterView;

	private int m_ProductID;

	private int m_SelectedSkinID;

	private int m_SelectedUnitID;

	private long m_SelectedUnitUID;

	[Header("스킨 목록")]
	public GameObject m_objMenuSkinList;

	public NKCUISkinSlot m_pfbSkinSlot;

	public RectTransform m_rtSlotRoot;

	public ScrollRect m_srScrollRect;

	public GameObject m_objNoSkin;

	private List<NKCUISkinSlot> m_lstSlot = new List<NKCUISkinSlot>();

	[Header("스킨 기본정보")]
	public GameObject m_objMenuSkinTitle;

	public Text m_lbSkinName;

	public GameObject m_objLimited;

	public Text m_lbSkinGrade;

	public Image m_imgSkinGradeLine;

	public Sprite m_spGradeLineN;

	public Sprite m_spGradeLineR;

	public Sprite m_spGradeLineSR;

	public Sprite m_spGradeLineSSR;

	public Sprite m_spGradeLineSpecial;

	[Header("스킨 설명")]
	public GameObject m_objMenuSkinDesc;

	public Text m_lbDesc;

	[Header("착용 가능 사원")]
	public GameObject m_objEquippableUnit;

	public GameObject m_objEquppableUnitNotOwned;

	public Image m_imgEquippableUnit;

	public Text m_lbEquippableUnitName;

	public Text m_lbEquippableUnitTitle;

	[Header("스킨 전체 구성요소")]
	public GameObject m_objMenuSkinComponents;

	public NKCUIComStateButton m_csbtnComponentVoice;

	public NKCUIComStateButton m_csbtnComponentCutin;

	public Image m_imgComponentCutin;

	public GameObject m_objComponentCutinEffect;

	public NKCUIComStateButton m_csbtnComponentStory;

	public GameObject m_NKM_UI_SHOP_SKIN_POPUP_INFO_COMPONENT_CONTENT_EFFECT;

	public GameObject m_objComponentConversion;

	public GameObject m_objComponentLobbyFace;

	public GameObject m_objComponentCollab;

	public GameObject m_objComponentGauntlet;

	public NKCUIComStateButton m_csbtnLoginAnim;

	public GameObject m_objComponentLoginAnim;

	[Header("유닛정보용 버튼")]
	public GameObject m_objMenuSkinUnitInfoBuy;

	public NKCUIComButton m_cbtnUnitInfoEquip;

	public NKCUIComButton m_cbtnUnitInfoBuy;

	[Header("상점용 버튼")]
	public GameObject m_objMenuSkinShopBuy;

	public GameObject m_objSalePriceRoot;

	public Text m_lbOldPrice;

	public NKCUIPriceTag m_PriceTag;

	public NKCUIComButton m_cbtnShopTry;

	public NKCUIComButton m_cbtnShopBuy;

	[Header("스킨정보용 버튼")]
	public GameObject m_objMenuSkinInfoOnly;

	public NKCUIComButton m_cbtnSkinInfoTry;

	public NKCUIComButton m_cbtnSKinInfoClose;

	[Header("왼쪽 구매 가능 시간")]
	public GameObject m_objLimitedTime;

	public Text m_lbLimitedTime;

	[Header("할인 관련")]
	public GameObject m_objDiscountDay;

	public Text m_txtDiscountDay;

	public GameObject m_objDiscountDayForUnitInfo;

	public Text m_txtDiscountDayForUnitInfo;

	public GameObject m_objDiscountRate;

	public Text m_txtDiscountRate;

	[Header("SD/인게임 뷰")]
	public NKCUIComStateButton m_sbtnIngameSD;

	private bool m_bGameUnitView;

	public NKCUIInGameCharacterViewer m_InGameUnitViewer;

	[Header("SD캐릭터 관련 설정")]
	public RectTransform m_rtSDRoot;

	public float m_fSDScale = 1.2f;

	private NKCASUIUnitIllust m_spineSD;

	[Header("기타")]
	public NKCUIComStateButton m_cbtnIllustViewMode;

	private long m_lBuyTimeLimit;

	private DateTime m_tEndDateDiscountTime = DateTime.MinValue;

	private bool m_bUseUpdate;

	private Color CUTIN_PRIVATE_COLOR = new Color(0.9490196f, 0.81960785f, 0.101960786f);

	private NKCASUIUnitIllust m_NKCASUIUnitCutinIllust;

	private NKCUICharacterView.eMode m_eCurrentViewMode;

	private OnApplySkin m_dOnApplySkin;

	private bool m_bUnitCollection;

	private const float ONE_SECOND = 1f;

	private float m_fUpdateGap = 1f;

	public static NKCUIShopSkinPopup Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIShopSkinPopup>("ab_ui_nkm_ui_shop_skin", "NKM_UI_SHOP_SKIN", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIShopSkinPopup>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override bool WillCloseUnderPopupOnOpen => false;

	public override string MenuName => NKCUtilString.GET_STRING_SHOP_SKIN_INFO;

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (m_lstUpsideMenuShowResource != null && m_lstUpsideMenuShowResource.Count > 0)
			{
				return m_lstUpsideMenuShowResource;
			}
			return DEFAULT_RESOURCE_LIST;
		}
	}

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode
	{
		get
		{
			if (!m_bShowUpsideMenu)
			{
				return NKCUIUpsideMenu.eMode.BackButtonOnly;
			}
			return base.eUpsideMenuMode;
		}
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		CleanUp();
	}

	public override void Hide()
	{
		base.Hide();
		if (m_NKCASUIUnitCutinIllust != null)
		{
			NKCUtil.SetGameobjectActive(m_NKCASUIUnitCutinIllust.GetRectTransform().gameObject, bValue: false);
		}
	}

	internal void Init()
	{
		m_CharacterView.Init();
		m_cbtnIllustViewMode.PointerClick.RemoveAllListeners();
		m_cbtnIllustViewMode.PointerClick.AddListener(OnBtnIllust);
		m_cbtnShopBuy.PointerClick.RemoveAllListeners();
		m_cbtnShopBuy.PointerClick.AddListener(OnBtnShopBuy);
		NKCUtil.SetButtonClickDelegate(m_cbtnSkinInfoTry, OnBtnTryButton);
		NKCUtil.SetButtonClickDelegate(m_cbtnSKinInfoClose, base.Close);
		m_cbtnShopTry.PointerClick.RemoveAllListeners();
		m_cbtnShopTry.PointerClick.AddListener(OnBtnTryButton);
		m_cbtnUnitInfoEquip.PointerClick.RemoveAllListeners();
		m_cbtnUnitInfoEquip.PointerClick.AddListener(OnBtnUnitInfoEquip);
		m_cbtnUnitInfoBuy.PointerClick.RemoveAllListeners();
		m_cbtnUnitInfoBuy.PointerClick.AddListener(OnBtnUnitInfoBuy);
		m_sbtnIngameSD.PointerClick.RemoveAllListeners();
		m_sbtnIngameSD.PointerClick.AddListener(OnBtnGameUnit);
		NKCUtil.SetButtonClickDelegate(m_csbtnComponentVoice, OnUnitVoice);
		NKCUtil.SetButtonClickDelegate(m_csbtnComponentCutin, OnUnitCutin);
		SetBindSkinFunction();
		NKCUtil.SetButtonClickDelegate(m_csbtnLoginAnim, OnClickLoginAnim);
	}

	private void SetBindSkinFunction(bool bActive = false)
	{
		if (bActive)
		{
			NKCUtil.SetBindFunction(m_csbtnComponentStory, OnClickStoryPlayable);
		}
		else
		{
			NKCUtil.SetBindFunction(m_csbtnComponentStory, OnClickStoryNotOwned);
		}
	}

	private void OnClickStoryPlayable()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SKIN_STORY_REPLAY_CONFIRM, delegate
		{
			PlayCutScene(m_SelectedSkinID);
		});
	}

	private void OnClickStoryNotOwned()
	{
		NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_SHOP_SKIN_STORY_MSG);
	}

	private void OnClickLoginAnim()
	{
		if (!NKCScenManager.CurrentUserData().m_InventoryData.HasItemSkin(m_SelectedSkinID))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_SHOP_SKIN_LOGIN_CUTIN_MSG);
			return;
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_SelectedSkinID);
		if (skinTemplet != null && skinTemplet.HasLoginCutin)
		{
			NKCUIEventSequence.PlaySkinCutin(skinTemplet, null);
		}
	}

	private void PlayCutScene(int skinID)
	{
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
		if (skinTemplet != null && !string.IsNullOrEmpty(skinTemplet.m_CutscenePurchase))
		{
			NKCUICutScenPlayer.Instance.LoadAndPlay(skinTemplet.m_CutscenePurchase, 0);
		}
	}

	public void CleanUp()
	{
		m_CharacterView.CleanUp();
		m_lstUpsideMenuShowResource.Clear();
		m_InGameUnitViewer.CleanUp();
		if (m_spineSD != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
			m_spineSD = null;
		}
		m_SelectedSkinID = 0;
		m_SelectedUnitID = 0;
		m_SelectedUnitUID = 0L;
		m_ProductID = 0;
		m_bUseUpdate = false;
		m_lBuyTimeLimit = 0L;
		m_tEndDateDiscountTime = DateTime.MinValue;
		if (m_NKCASUIUnitCutinIllust != null)
		{
			m_NKCASUIUnitCutinIllust.Unload();
			m_NKCASUIUnitCutinIllust = null;
		}
		NKCUIPopupIllustView.CheckInstanceAndClose();
		NKCScenManager.GetScenManager().m_NKCMemoryCleaner.UnloadObjectPool();
	}

	private void OnBtnShopBuy()
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(m_ProductID);
		if (shopItemTemplet == null)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_SKIN_LOCK);
		}
		else
		{
			NKCPopupShopBuyConfirm.Instance.Open(shopItemTemplet, NKCShopManager.TryProductBuy);
		}
	}

	private void OnBtnIllust()
	{
		if (m_SelectedSkinID == 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_SelectedUnitID);
			NKCUIPopupIllustView.Instance.Open(unitTempletBase);
		}
		else
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_SelectedSkinID);
			NKCUIPopupIllustView.Instance.Open(skinTemplet);
		}
	}

	private void OnBtnGameUnit()
	{
		ToggleGameUnit(!m_bGameUnitView);
	}

	private void ToggleGameUnit(bool value)
	{
		m_bGameUnitView = value;
		NKCUtil.SetGameobjectActive(m_InGameUnitViewer, m_bGameUnitView);
		NKCUtil.SetGameobjectActive(m_rtSDRoot, !m_bGameUnitView);
	}

	public void OpenForShop(ShopItemTemplet shopTemplet)
	{
		if (shopTemplet == null)
		{
			return;
		}
		long skinLimitedBuy = (shopTemplet.HasDateLimit ? shopTemplet.EventDateEndUtc.Ticks : 0);
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(shopTemplet.m_ItemID);
		int priceItemID = shopTemplet.m_PriceItemID;
		int realPrice = NKCScenManager.CurrentUserData().m_ShopData.GetRealPrice(shopTemplet);
		int price = shopTemplet.m_Price;
		m_ProductID = shopTemplet.m_ProductID;
		if (skinTemplet == null || NKMUnitManager.GetUnitTempletBase(skinTemplet.m_SkinEquipUnitID) == null)
		{
			return;
		}
		UpdateUpsideMenuResource(skinTemplet.m_SkinID);
		m_bUseUpdate = false;
		m_eMode = Mode.ForShop;
		m_bShowUpsideMenu = true;
		NKCUtil.SetGameobjectActive(m_objMenuSkinList, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMenuSkinTitle, bValue: true);
		SetSkinTitleData(skinTemplet);
		SetEquippableUnit(skinTemplet);
		NKCUtil.SetGameobjectActive(m_objMenuSkinDesc, bValue: true);
		SetSkinDesc(skinTemplet);
		NKCUtil.SetGameobjectActive(m_objMenuSkinUnitInfoBuy, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMenuSkinInfoOnly, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMenuSkinShopBuy, bValue: true);
		SetSkinShopBuyButtons(skinTemplet.m_SkinID, priceItemID, realPrice, price);
		SetSkinCommon();
		SetSkinLimitedBuy(skinLimitedBuy);
		bool flag = false;
		if (shopTemplet != null)
		{
			if (shopTemplet.m_DiscountRate > 0f && NKCSynchronizedTime.IsEventTime(shopTemplet.discountIntervalId, shopTemplet.DiscountStartDateUtc, shopTemplet.DiscountEndDateUtc) && shopTemplet.DiscountEndDateUtc != DateTime.MaxValue)
			{
				m_bUseUpdate = true;
				flag = true;
				m_tEndDateDiscountTime = shopTemplet.DiscountEndDateUtc;
				UpdateDiscountTime(m_tEndDateDiscountTime);
			}
			else
			{
				m_tEndDateDiscountTime = DateTime.MinValue;
			}
			if (!shopTemplet.HasDiscountDateLimit)
			{
				NKCUtil.SetGameobjectActive(m_objDiscountRate, shopTemplet.m_DiscountRate > 0f);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objDiscountRate, shopTemplet.m_DiscountRate > 0f && flag);
			}
			NKCUtil.SetLabelText(m_txtDiscountRate, $"-{(int)shopTemplet.m_DiscountRate}%");
		}
		else
		{
			m_tEndDateDiscountTime = DateTime.MinValue;
		}
		SetShowDiscountTime(flag);
		m_SelectedSkinID = skinTemplet.m_SkinID;
		m_SelectedUnitID = skinTemplet.m_SkinEquipUnitID;
		m_SelectedUnitUID = 0L;
		SelectSkin(skinTemplet.m_SkinID);
		SetViewMode(NKCUICharacterView.eMode.Normal, bAnimate: false);
		UIOpened();
	}

	public void OpenForSkinInfo(NKMSkinTemplet skinTemplet, int productID)
	{
		if (skinTemplet != null && NKMUnitManager.GetUnitTempletBase(skinTemplet.m_SkinEquipUnitID) != null)
		{
			UpdateUpsideMenuResource(skinTemplet.m_SkinID);
			m_bUseUpdate = false;
			m_eMode = Mode.ForSkinInfo;
			m_bShowUpsideMenu = true;
			NKCUtil.SetGameobjectActive(m_objMenuSkinList, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMenuSkinTitle, bValue: true);
			SetSkinTitleData(skinTemplet);
			SetEquippableUnit(skinTemplet);
			NKCUtil.SetGameobjectActive(m_objMenuSkinDesc, bValue: true);
			SetSkinDesc(skinTemplet);
			NKCUtil.SetGameobjectActive(m_objMenuSkinUnitInfoBuy, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMenuSkinShopBuy, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMenuSkinInfoOnly, bValue: true);
			SetSkinCommon();
			SetSkinLimitedBuy(0L);
			m_tEndDateDiscountTime = DateTime.MinValue;
			SetShowDiscountTime(bValue: false);
			m_ProductID = productID;
			m_SelectedSkinID = skinTemplet.m_SkinID;
			m_SelectedUnitID = skinTemplet.m_SkinEquipUnitID;
			m_SelectedUnitUID = 0L;
			SelectSkin(skinTemplet.m_SkinID);
			SetViewMode(NKCUICharacterView.eMode.Normal, bAnimate: false);
			UIOpened();
		}
	}

	private void UpdateUpsideMenuResource(int targetSkinID)
	{
		m_lstUpsideMenuShowResource.Clear();
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(m_ProductID);
		if (shopItemTemplet != null)
		{
			ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(shopItemTemplet.m_TabID, shopItemTemplet.m_TabSubIndex);
			if (shopTabTemplet != null)
			{
				AddUpsideMenuResource(shopTabTemplet.m_ResourceTypeID_1);
				AddUpsideMenuResource(shopTabTemplet.m_ResourceTypeID_2);
				AddUpsideMenuResource(shopTabTemplet.m_ResourceTypeID_3);
				AddUpsideMenuResource(shopTabTemplet.m_ResourceTypeID_4);
				AddUpsideMenuResource(shopTabTemplet.m_ResourceTypeID_5);
			}
		}
	}

	private void AddUpsideMenuResource(int resourceID)
	{
		if (resourceID > 0 && !m_lstUpsideMenuShowResource.Contains(resourceID))
		{
			m_lstUpsideMenuShowResource.Add(resourceID);
		}
	}

	public void OpenForUnitInfo(NKMUnitData unitData, bool bShowUpsideMenu = true, bool bUnitCollection = false, OnApplySkin dOnApplySkin = null)
	{
		if (unitData == null)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase != null)
		{
			m_eMode = Mode.ForUnitInfo;
			m_bShowUpsideMenu = bShowUpsideMenu;
			m_bUnitCollection = bUnitCollection;
			m_dOnApplySkin = dOnApplySkin;
			m_SelectedSkinID = unitData.m_SkinID;
			m_SelectedUnitID = unitData.m_UnitID;
			m_SelectedUnitUID = unitData.m_UnitUID;
			m_ProductID = NKCShopManager.GetShopTempletBySkinID(m_SelectedSkinID)?.m_ProductID ?? 0;
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(unitData);
			NKCUtil.SetGameobjectActive(m_objMenuSkinList, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMenuSkinTitle, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMenuSkinDesc, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMenuSkinUnitInfoBuy, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMenuSkinInfoOnly, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMenuSkinShopBuy, bValue: false);
			NKCUtil.SetGameobjectActive(m_objDiscountDay, bValue: false);
			NKCUtil.SetGameobjectActive(m_objDiscountRate, bValue: false);
			NKCUtil.SetGameobjectActive(m_objEquippableUnit, bValue: false);
			SetSkinListData(unitData);
			if (skinTemplet != null)
			{
				SetSkinTitleData(skinTemplet);
				SetSkinDesc(skinTemplet);
			}
			else
			{
				SetSkinTitleData(unitTempletBase);
				SetSkinDesc(unitTempletBase);
			}
			SetSkinCommon();
			SelectSkin(unitData.m_SkinID);
			SetViewMode(NKCUICharacterView.eMode.Normal, bAnimate: false);
			UIOpened();
		}
	}

	private void SetSkinCommon()
	{
		m_InGameUnitViewer.Prepare();
	}

	private void SetSkinListData(NKMUnitData unitData)
	{
		if (NKMSkinManager.IsCharacterHasSkin(unitData.m_UnitID))
		{
			NKCUtil.SetGameobjectActive(m_objNoSkin, bValue: false);
			NKCUtil.SetGameobjectActive(m_srScrollRect, bValue: true);
			List<NKMSkinTemplet> skinlistForCharacter = NKMSkinManager.GetSkinlistForCharacter(unitData.m_UnitID, NKCScenManager.CurrentUserData().m_InventoryData);
			SetSkinList(unitData, skinlistForCharacter);
		}
		else
		{
			SetDefaultSkin(unitData);
		}
	}

	private void SetEquippableUnit(NKMSkinTemplet skinTemplet)
	{
		if (skinTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_objEquippableUnit, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objEquippableUnit, bValue: true);
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(skinTemplet.m_SkinEquipUnitID);
		NKCUtil.SetGameobjectActive(m_objEquppableUnitNotOwned, !NKCScenManager.CurrentUserData().m_ArmyData.HaveUnit(skinTemplet.m_SkinEquipUnitID, bIncludeRearm: true));
		NKCUtil.SetImageSprite(m_imgEquippableUnit, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, skinTemplet.m_SkinEquipUnitID, 0));
		NKCUtil.SetLabelText(m_lbEquippableUnitName, nKMUnitTempletBase.GetUnitName());
		NKCUtil.SetLabelText(m_lbEquippableUnitTitle, nKMUnitTempletBase.GetUnitTitle());
	}

	private void SetSkinComponents(NKMSkinTemplet skinTemplet)
	{
		NKCUtil.SetGameobjectActive(m_csbtnComponentVoice, skinTemplet.ChangesVoice());
		NKCUtil.SetGameobjectActive(m_csbtnComponentCutin, skinTemplet.ChangesHyperCutin());
		NKCUtil.SetGameobjectActive(m_objComponentCutinEffect, skinTemplet.m_SkinSkillCutIn == NKMSkinTemplet.SKIN_CUTIN.CUTIN_PRIVATE);
		NKCUtil.SetGameobjectActive(m_csbtnComponentStory, !string.IsNullOrEmpty(skinTemplet.m_CutscenePurchase));
		NKCUtil.SetGameobjectActive(m_NKM_UI_SHOP_SKIN_POPUP_INFO_COMPONENT_CONTENT_EFFECT, skinTemplet.m_bEffect);
		NKCUtil.SetGameobjectActive(m_objComponentConversion, skinTemplet.m_Conversion);
		NKCUtil.SetGameobjectActive(m_objComponentLobbyFace, skinTemplet.m_LobbyFace);
		NKCUtil.SetGameobjectActive(m_objComponentCollab, skinTemplet.m_Collabo);
		NKCUtil.SetGameobjectActive(m_objComponentGauntlet, skinTemplet.m_Gauntlet);
		NKCUtil.SetGameobjectActive(m_objComponentLoginAnim, skinTemplet.HasLoginCutin);
		if (skinTemplet.m_SkinSkillCutIn == NKMSkinTemplet.SKIN_CUTIN.CUTIN_PRIVATE)
		{
			NKCUtil.SetImageSprite(m_imgComponentCutin, NKCUtil.GetShopSprite("NKM_UI_SHOP_SKIN_ICON_CUTIN_SPECIAL"));
			NKCUtil.SetImageColor(m_imgComponentCutin, CUTIN_PRIVATE_COLOR);
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgComponentCutin, NKCUtil.GetShopSprite("NKM_UI_SHOP_SKIN_ICON_CUTIN"));
			NKCUtil.SetImageColor(m_imgComponentCutin, Color.white);
		}
	}

	private void SetSkinTitleData(NKMUnitTempletBase unitTempletBase)
	{
		NKCUtil.SetGameobjectActive(m_objLimited, bValue: false);
		NKCUtil.SetLabelText(m_lbSkinName, unitTempletBase.GetUnitTitle());
		NKCUtil.SetLabelTextColor(m_lbSkinGrade, NKCUtil.GetColorForGrade(NKMSkinTemplet.SKIN_GRADE.SG_VARIATION));
		NKCUtil.SetImageSprite(m_imgSkinGradeLine, m_spGradeLineN);
		NKCUtil.SetLabelText(m_lbSkinGrade, NKCUtilString.GET_STRING_BASE_SKIN);
	}

	private void SetSkinTitleData(NKMSkinTemplet skinTemplet)
	{
		NKCUtil.SetGameobjectActive(m_objLimited, skinTemplet.m_bLimited);
		NKCUtil.SetLabelText(m_lbSkinName, skinTemplet.GetTitle());
		NKCUtil.SetLabelTextColor(m_lbSkinGrade, NKCUtil.GetColorForGrade(skinTemplet.m_SkinGrade));
		NKCUtil.SetLabelText(m_lbSkinGrade, NKCUtil.GetStringForGrade(skinTemplet.m_SkinGrade));
		switch (skinTemplet.m_SkinGrade)
		{
		case NKMSkinTemplet.SKIN_GRADE.SG_VARIATION:
			NKCUtil.SetImageSprite(m_imgSkinGradeLine, m_spGradeLineN);
			break;
		case NKMSkinTemplet.SKIN_GRADE.SG_NORMAL:
			NKCUtil.SetImageSprite(m_imgSkinGradeLine, m_spGradeLineR);
			break;
		case NKMSkinTemplet.SKIN_GRADE.SG_RARE:
			NKCUtil.SetImageSprite(m_imgSkinGradeLine, m_spGradeLineSR);
			break;
		case NKMSkinTemplet.SKIN_GRADE.SG_PREMIUM:
			NKCUtil.SetImageSprite(m_imgSkinGradeLine, m_spGradeLineSSR);
			break;
		case NKMSkinTemplet.SKIN_GRADE.SG_SPECIAL:
			NKCUtil.SetImageSprite(m_imgSkinGradeLine, m_spGradeLineSpecial);
			break;
		}
	}

	private void SetSkinDesc(NKMSkinTemplet skinTemplet)
	{
		NKCUtil.SetLabelText(m_lbDesc, skinTemplet.GetSkinDesc());
	}

	private void SetSkinDesc(NKMUnitTempletBase unitTempletBase)
	{
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(m_SelectedUnitID);
		if (unitTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbDesc, unitTemplet.GetUnitIntro());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbDesc, NKCUtilString.GET_STRING_BASE_SKIN);
		}
	}

	private void SetSkinUnitInfoBuyButtons(bool skinOwned, bool equipped)
	{
		if (m_bUnitCollection)
		{
			NKCUtil.SetGameobjectActive(m_cbtnUnitInfoEquip, bValue: true);
			NKCUtil.SetGameobjectActive(m_cbtnUnitInfoBuy, bValue: false);
			if (!m_cbtnUnitInfoEquip.m_bLock)
			{
				m_cbtnUnitInfoEquip?.Lock();
			}
			m_cbtnUnitInfoEquip?.UnLock();
			return;
		}
		NKCUtil.SetGameobjectActive(m_cbtnUnitInfoEquip, skinOwned);
		NKCUtil.SetGameobjectActive(m_cbtnUnitInfoBuy, !skinOwned);
		if (skinOwned)
		{
			if (equipped)
			{
				m_cbtnUnitInfoEquip?.Lock();
			}
			else
			{
				m_cbtnUnitInfoEquip?.UnLock();
			}
			return;
		}
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(m_ProductID);
		if (shopItemTemplet != null)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (NKCShopManager.CanBuyFixShop(myUserData, shopItemTemplet, out var _, out var _) == NKM_ERROR_CODE.NEC_OK)
			{
				m_cbtnUnitInfoBuy?.UnLock();
			}
			else
			{
				m_cbtnUnitInfoBuy?.Lock();
			}
		}
		else
		{
			m_cbtnUnitInfoBuy?.Lock();
		}
	}

	private void SetSkinShopBuyButtons(int skinID, int priceItemID, int price, int oldPrice)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(m_ProductID);
		if (shopItemTemplet != null)
		{
			if (!shopItemTemplet.HasDiscountDateLimit)
			{
				NKCUtil.SetGameobjectActive(m_objDiscountRate, shopItemTemplet.m_DiscountRate > 0f);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objDiscountRate, shopItemTemplet.m_DiscountRate > 0f && NKCSynchronizedTime.IsEventTime(shopItemTemplet.discountIntervalId, shopItemTemplet.DiscountStartDateUtc, shopItemTemplet.DiscountEndDateUtc));
			}
			m_PriceTag.SetData(priceItemID, price, showMinus: false, changeColor: false, bHidePriceIcon: true);
			if (oldPrice > price)
			{
				NKCResourceUtility.GetOrLoadMiscItemSmallIcon(priceItemID);
				NKCUtil.SetLabelText(m_lbOldPrice, oldPrice.ToString());
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objSalePriceRoot, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSalePriceRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objDiscountRate, bValue: false);
		}
	}

	private void SetSkinLimitedBuy(long buyEndTick)
	{
		m_lBuyTimeLimit = buyEndTick;
		if (buyEndTick > 0)
		{
			m_bUseUpdate = true;
			NKCUtil.SetGameobjectActive(m_objLimitedTime, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objLimitedTime, bValue: false);
		}
	}

	public void SetShowDiscountTime(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objDiscountDay, bValue);
	}

	public void UpdateDiscountTime(DateTime endTime)
	{
		string msg = ((!NKCSynchronizedTime.IsFinished(endTime)) ? NKCUtilString.GetRemainTimeStringOneParam(endTime) : NKCUtilString.GET_STRING_QUIT);
		if (m_eMode == Mode.ForShop)
		{
			NKCUtil.SetLabelText(m_txtDiscountDay, msg);
		}
		else if (m_eMode == Mode.ForUnitInfo)
		{
			NKCUtil.SetLabelText(m_txtDiscountDayForUnitInfo, msg);
		}
	}

	private void UpdateBuyTime()
	{
		NKCUtil.SetLabelText(m_lbLimitedTime, NKCSynchronizedTime.GetTimeLeftString(m_lBuyTimeLimit));
	}

	public void Update()
	{
		if (!m_bUseUpdate)
		{
			return;
		}
		m_fUpdateGap -= Time.unscaledDeltaTime;
		if (m_fUpdateGap <= 0f)
		{
			if (m_lBuyTimeLimit > 0)
			{
				UpdateBuyTime();
			}
			if (m_tEndDateDiscountTime != DateTime.MinValue)
			{
				UpdateDiscountTime(m_tEndDateDiscountTime);
			}
			m_fUpdateGap = 1f;
		}
	}

	private void SelectSkin(int skinID)
	{
		m_SelectedSkinID = skinID;
		foreach (NKCUISkinSlot item in m_lstSlot)
		{
			item.SetSelected(item.SkinID == skinID);
		}
		if (m_eMode == Mode.ForUnitInfo)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(m_SelectedUnitUID);
			bool equipped = unitFromUID != null && unitFromUID.m_SkinID == skinID;
			bool flag = myUserData.m_InventoryData.HasItemSkin(skinID);
			ShopItemTemplet shopTempletBySkinID = NKCShopManager.GetShopTempletBySkinID(skinID);
			m_ProductID = shopTempletBySkinID?.m_ProductID ?? 9;
			SetSkinUnitInfoBuyButtons(flag, equipped);
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
			if (skinTemplet != null)
			{
				bool bValue = !string.IsNullOrEmpty(skinTemplet.m_CutscenePurchase);
				NKCUtil.SetGameobjectActive(m_csbtnComponentStory, bValue);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_csbtnComponentStory, bValue: false);
			}
			SetBindSkinFunction(flag);
			if (!flag)
			{
				if (shopTempletBySkinID != null)
				{
					bool flag2 = false;
					if (shopTempletBySkinID.m_DiscountRate > 0f && NKCSynchronizedTime.IsEventTime(shopTempletBySkinID.discountIntervalId, shopTempletBySkinID.DiscountStartDateUtc, shopTempletBySkinID.DiscountEndDateUtc) && shopTempletBySkinID.DiscountEndDateUtc != DateTime.MaxValue)
					{
						flag2 = true;
						m_tEndDateDiscountTime = shopTempletBySkinID.DiscountEndDateUtc;
						UpdateDiscountTime(m_tEndDateDiscountTime);
					}
					else
					{
						m_tEndDateDiscountTime = DateTime.MinValue;
					}
					NKCUtil.SetGameobjectActive(m_objDiscountDayForUnitInfo, flag2);
					if (!shopTempletBySkinID.HasDiscountDateLimit)
					{
						NKCUtil.SetGameobjectActive(m_objDiscountRate, shopTempletBySkinID.m_DiscountRate > 0f);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_objDiscountRate, shopTempletBySkinID.m_DiscountRate > 0f && flag2);
					}
					NKCUtil.SetLabelText(m_txtDiscountRate, $"-{(int)shopTempletBySkinID.m_DiscountRate}%");
					bool flag3 = false;
					if (shopTempletBySkinID.HasDateLimit && NKCSynchronizedTime.IsEventTime(shopTempletBySkinID.eventIntervalId, shopTempletBySkinID.EventDateStartUtc, shopTempletBySkinID.EventDateEndUtc))
					{
						flag3 = true;
						m_lBuyTimeLimit = shopTempletBySkinID.EventDateEndUtc.Ticks;
						UpdateBuyTime();
					}
					else
					{
						m_lBuyTimeLimit = 0L;
					}
					NKCUtil.SetGameobjectActive(m_objLimited, flag3);
					SetSkinLimitedBuy(m_lBuyTimeLimit);
					m_bUseUpdate = flag2 || flag3;
				}
				else
				{
					m_bUseUpdate = false;
					NKCUtil.SetGameobjectActive(m_objDiscountDayForUnitInfo, bValue: false);
					NKCUtil.SetGameobjectActive(m_objDiscountRate, bValue: false);
					NKCUtil.SetGameobjectActive(m_objLimitedTime, bValue: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objLimitedTime, bValue: false);
				NKCUtil.SetGameobjectActive(m_objDiscountDay, bValue: false);
				NKCUtil.SetGameobjectActive(m_objDiscountRate, bValue: false);
			}
		}
		else
		{
			SetBindSkinFunction();
		}
		if (skinID == 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_SelectedUnitID);
			if (unitTempletBase != null)
			{
				SetSkinTitleData(unitTempletBase);
				NKCUtil.SetGameobjectActive(m_objMenuSkinComponents, bValue: false);
				SetSkinDesc(unitTempletBase);
			}
			m_CharacterView.SetCharacterIllust(unitTempletBase);
			OpenSDIllust(unitTempletBase);
		}
		else
		{
			NKMSkinTemplet skinTemplet2 = NKMSkinManager.GetSkinTemplet(skinID);
			if (skinTemplet2 != null)
			{
				SetSkinTitleData(skinTemplet2);
				NKCUtil.SetGameobjectActive(m_objMenuSkinComponents, bValue: true);
				SetSkinComponents(skinTemplet2);
				SetSkinDesc(skinTemplet2);
			}
			m_CharacterView.SetCharacterIllust(skinTemplet2);
			OpenSDIllust(skinTemplet2);
		}
		m_InGameUnitViewer.SetPreviewBattleUnit(m_SelectedUnitID, skinID);
		ToggleGameUnit(m_bGameUnitView);
	}

	private void SetViewMode(NKCUICharacterView.eMode mode, bool bAnimate = true)
	{
		m_eCurrentViewMode = mode;
		m_CharacterView.SetMode(mode, bAnimate);
		m_rtRightUI.DOComplete();
		switch (mode)
		{
		case NKCUICharacterView.eMode.Normal:
			if (bAnimate)
			{
				m_rtRightUI.DOAnchorMin(new Vector2(0f, 0f), 0.4f).SetEase(Ease.OutCubic);
				m_rtRightUI.DOAnchorMax(new Vector2(1f, 1f), 0.4f).SetEase(Ease.OutCubic);
			}
			else
			{
				m_rtRightUI.anchorMin = new Vector2(0f, 0f);
				m_rtRightUI.anchorMax = new Vector2(1f, 1f);
			}
			break;
		case NKCUICharacterView.eMode.CharacterView:
			if (bAnimate)
			{
				m_rtRightUI.DOAnchorMin(new Vector2(1f, 0f), 0.4f).SetEase(Ease.OutCubic);
				m_rtRightUI.DOAnchorMax(new Vector2(2f, 1f), 0.4f).SetEase(Ease.OutCubic);
			}
			else
			{
				m_rtRightUI.anchorMin = new Vector2(1f, 0f);
				m_rtRightUI.anchorMax = new Vector2(2f, 1f);
			}
			break;
		}
	}

	private void OnUnitVoice()
	{
		bool bLifetime = false;
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (armyData.IsCollectedUnit(m_SelectedUnitID))
		{
			bLifetime = armyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, m_SelectedUnitID, NKMArmyData.UNIT_SEARCH_OPTION.Devotion, 0);
		}
		if (m_SelectedSkinID == 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_SelectedUnitID);
			NKCUIPopupVoice.Instance.Open(unitTempletBase, bLifetime);
		}
		else
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_SelectedSkinID);
			NKCUIPopupVoice.Instance.Open(skinTemplet, bLifetime);
		}
	}

	private void OnUnitCutin()
	{
		if (m_SelectedSkinID == 0)
		{
			return;
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_SelectedSkinID);
		if (skinTemplet != null && !string.IsNullOrEmpty(skinTemplet.m_HyperSkillCutin))
		{
			if (m_NKCASUIUnitCutinIllust != null)
			{
				m_NKCASUIUnitCutinIllust.Unload();
				m_NKCASUIUnitCutinIllust = null;
			}
			m_NKCASUIUnitCutinIllust = NKCResourceUtility.OpenSpineIllust(skinTemplet.m_HyperSkillCutin, skinTemplet.m_HyperSkillCutin);
			NKCUtil.SetGameobjectActive(m_NKCASUIUnitCutinIllust.GetRectTransform().Find("VFX").gameObject, bValue: false);
			m_NKCASUIUnitCutinIllust.SetParent(base.transform, worldPositionStays: true);
			NKCUtil.SetGameobjectActive(m_NKCASUIUnitCutinIllust.GetRectTransform().gameObject, bValue: true);
			m_NKCASUIUnitCutinIllust.GetRectTransform().GetComponentInChildren<SkeletonGraphic>(includeInactive: true).AnimationState.SetAnimation(0, "BASE", loop: false);
		}
	}

	private void SetSlotCount(int count)
	{
		while (m_lstSlot.Count < count)
		{
			NKCUISkinSlot nKCUISkinSlot = UnityEngine.Object.Instantiate(m_pfbSkinSlot);
			nKCUISkinSlot.transform.SetParent(m_rtSlotRoot, worldPositionStays: false);
			nKCUISkinSlot.Init(SelectSkin);
			m_lstSlot.Add(nKCUISkinSlot);
		}
		for (int i = count; i < m_lstSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstSlot[i], bValue: false);
		}
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
		if (lstSkinTemplet != null)
		{
			SetSlotCount(lstSkinTemplet.Count + 1);
			m_lstSlot[0].SetData(NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID), !m_bUnitCollection && unitData.m_SkinID == 0);
			NKCUtil.SetGameobjectActive(m_lstSlot[0], bValue: true);
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			for (int i = 0; i < lstSkinTemplet.Count; i++)
			{
				NKMSkinTemplet nKMSkinTemplet = lstSkinTemplet[i];
				NKCUISkinSlot nKCUISkinSlot = m_lstSlot[i + 1];
				nKCUISkinSlot.SetData(nKMSkinTemplet, myUserData.m_InventoryData.HasItemSkin(nKMSkinTemplet.m_SkinID), !m_bUnitCollection && unitData.m_SkinID == nKMSkinTemplet.m_SkinID);
				NKCUtil.SetGameobjectActive(nKCUISkinSlot, bValue: true);
			}
			NKCUtil.SetLabelText(m_lstSlot[0].m_lbName, NKCUtilString.GET_STRING_BASE);
		}
	}

	private void OnBtnUnitInfoBuy()
	{
		if (!(null == m_cbtnUnitInfoBuy) && !m_cbtnUnitInfoBuy.m_bLock)
		{
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(m_ProductID);
			if (shopItemTemplet == null)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_SKIN_LOCK);
			}
			else
			{
				NKCPopupShopBuyConfirm.Instance.Open(shopItemTemplet, NKCShopManager.TryProductBuy);
			}
		}
	}

	private void OnBtnUnitInfoEquip()
	{
		if (m_bUnitCollection)
		{
			if (m_dOnApplySkin != null)
			{
				m_dOnApplySkin(m_SelectedSkinID);
			}
			Close();
		}
		else if (!m_cbtnUnitInfoEquip.m_bLock)
		{
			NKCPacketSender.Send_NKMPacket_SET_UNIT_SKIN_REQ(m_SelectedUnitUID, m_SelectedSkinID);
		}
	}

	private void OnBtnTryButton()
	{
		NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(m_SelectedUnitID, 100, 3);
		if (nKMUnitData != null)
		{
			nKMUnitData.m_SkinID = m_SelectedSkinID;
		}
		if (NKCUIShop.IsInstanceOpen)
		{
			string returnUIShortcutParam = $"{NKCUIShop.Instance.GetShortcutParam()},{m_ProductID}";
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OpenPracticeGameComfirmPopup(nKMUnitData, NKM_SHORTCUT_TYPE.SHORTCUT_SHOP_SCENE, returnUIShortcutParam);
		}
		else if (NKCUIEvent.IsInstanceOpen)
		{
			string returnUIShortcutParam2 = NKCUIEvent.Instance.SelectedTabId.ToString();
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OpenPracticeGameComfirmPopup(nKMUnitData, NKM_SHORTCUT_TYPE.SHORTCUT_HOME_EVENT_BANNER, returnUIShortcutParam2);
		}
		else
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OpenPracticeGameComfirmPopup(nKMUnitData);
		}
	}

	private void OpenSDIllust(NKMSkinTemplet skinTemplet)
	{
		if (skinTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
			return;
		}
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
		m_spineSD = NKCResourceUtility.OpenSpineSD(skinTemplet);
		if (m_spineSD != null)
		{
			m_spineSD.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true);
			m_spineSD.SetParent(m_rtSDRoot, worldPositionStays: false);
			RectTransform rectTransform = m_spineSD.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector3.zero;
				rectTransform.localScale = Vector3.one * m_fSDScale;
			}
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
		}
	}

	private void OpenSDIllust(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
			return;
		}
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
		m_spineSD = NKCResourceUtility.OpenSpineSD(unitTempletBase);
		if (m_spineSD != null)
		{
			m_spineSD.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true);
			m_spineSD.SetParent(m_rtSDRoot, worldPositionStays: false);
			RectTransform rectTransform = m_spineSD.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector3.zero;
				rectTransform.localScale = Vector3.one * m_fSDScale;
			}
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
		}
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		if (m_eMode == Mode.ForUnitInfo && unitData.m_UnitUID == m_SelectedUnitUID)
		{
			SetSkinListData(unitData);
			SelectSkin(unitData.m_SkinID);
		}
	}
}
