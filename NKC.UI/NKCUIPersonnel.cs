using System.Collections.Generic;
using Cs.Math;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPersonnel : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_personnel";

	public const string UI_ASSET_NAME = "NKM_UI_PERSONNEL_LIFETIME";

	private static NKCUIPersonnel m_Instance;

	public NKCUIPersonnelShortCutMenu m_NKCUIPersonnelShortCutMenu;

	public NKCUICharInfoSummary m_unitInfoSummary;

	public NKCUIComDragSelectablePanel m_UnitDragSelectView;

	public EventTrigger m_evtPanel;

	public NKCComUITalkBox m_talkBox;

	public RectTransform m_rtUnitSelectAnchor;

	public NKCUIComStateButton m_btnSelectUnit;

	[Header("종신계약")]
	public NKCUIItemCostSlot m_costLifetime;

	public NKCUIComStateButton m_btnLifetimeOK;

	public GameObject m_objLifetimeOK_off;

	public Text m_txtLifetimeOK;

	public Image m_imgLifetimeOK;

	public GameObject m_objLifetimeReady;

	public NKCUIComStateButton m_btnLifetimeInfo;

	public Text m_txtLoyaltyGauge;

	public Image m_imgLoyaltyGauge;

	public GameObject m_objLifetimeDefault;

	public NKCUIComStateButton m_btnLifetimeDefaultInfo;

	public NKCUIComStateButton m_btnLifetimeDefaultInfo2;

	private const int LIFETIME_ITEM_REQUIRE_COUNT = 1;

	private NKMUnitData m_targetUnit;

	private List<NKMUnitData> m_UnitSortList = new List<NKMUnitData>();

	private static NKCAssetInstanceData m_AssetInstanceData;

	private const string ASSET_SELECT_BUNDLE_NAME = "ab_ui_nuf_base";

	private const string UI_SELECT_ASSET_NAME = "NKM_UI_BASE_UNIT_SELECT";

	private NKCUIUnitSelect m_NKCUIUnitSelect;

	public static NKCUIPersonnel Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPersonnel>("ab_ui_nkm_ui_personnel", "NKM_UI_PERSONNEL_LIFETIME", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPersonnel>();
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

	public override string GuideTempletID => "ARTICLE_UNIT_WEDDING_CONTRACT";

	public override string MenuName => NKCUtilString.GET_STRING_LIFETIME;

	public override eMenutype eUIType => eMenutype.FullScreen;

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

	private void Init()
	{
		m_btnSelectUnit.PointerClick.RemoveAllListeners();
		m_btnSelectUnit.PointerClick.AddListener(OnSelectUnit);
		m_unitInfoSummary.Init();
		m_btnLifetimeOK.PointerClick.RemoveAllListeners();
		m_btnLifetimeOK.PointerClick.AddListener(OnTouchLifetime);
		m_btnLifetimeOK.m_ButtonBG_Locked = m_objLifetimeOK_off;
		m_btnLifetimeInfo.PointerClick.RemoveAllListeners();
		m_btnLifetimeInfo.PointerClick.AddListener(OnTouchLifetimeInfo);
		m_btnLifetimeDefaultInfo.PointerClick.RemoveAllListeners();
		m_btnLifetimeDefaultInfo.PointerClick.AddListener(OnTouchLifetimeInfo);
		m_btnLifetimeDefaultInfo2.PointerClick.RemoveAllListeners();
		m_btnLifetimeDefaultInfo2.PointerClick.AddListener(OnTouchLifetimeInfo);
		if (m_UnitDragSelectView != null)
		{
			m_UnitDragSelectView.Init(rotation: true);
			m_UnitDragSelectView.dOnGetObject += MakeMainBannerListSlot;
			m_UnitDragSelectView.dOnReturnObject += ReturnMainBannerListSlot;
			m_UnitDragSelectView.dOnProvideData += ProvideMainBannerListSlotData;
			m_UnitDragSelectView.dOnIndexChangeListener += SelectCharacter;
			m_UnitDragSelectView.dOnFocus += Focus;
		}
		if (m_evtPanel != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnEventPanelClick);
			m_evtPanel.triggers.Add(entry);
		}
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UpdateUI();
		UIOpened();
		NKCTutorialManager.TutorialRequired(TutorialPoint.Lifetime);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		BannerCleanUp();
		m_targetUnit = null;
		CloseSelectInstance();
	}

	public override void UnHide()
	{
		base.UnHide();
		UpdateUI();
	}

	public void ReserveUnitData(NKMUnitData unitData)
	{
		List<NKMUnitData> unitUIDList = new List<NKMUnitData> { unitData };
		if (unitData != null)
		{
			OnUnitSortList(unitData.m_UnitUID, unitUIDList);
		}
		m_targetUnit = unitData;
		if (unitData != null)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_NEGOTITATE_READY, unitData);
		}
		UpdateUI();
	}

	private void SetData(NKMUnitData unitData)
	{
		m_targetUnit = unitData;
		if (unitData != null)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_NEGOTITATE_READY, unitData);
		}
		UpdateUI();
	}

	private void UpdateUI()
	{
		NKCUtil.SetGameobjectActive(m_btnSelectUnit, m_targetUnit != null);
		OpenUnitSelect();
		SetUnit(m_targetUnit);
		SetTalk(m_targetUnit);
		SetInfo(m_targetUnit);
		SetCost();
		SetButton(m_targetUnit);
		m_NKCUIPersonnelShortCutMenu.SetData(NKC_SCEN_BASE.eUIOpenReserve.Personnel_Lifetime);
	}

	private void SetUnit(NKMUnitData unitData)
	{
		SetUICharInfo(unitData);
		if (m_UnitDragSelectView != null)
		{
			m_UnitDragSelectView.SetArrow(unitData == null);
		}
	}

	private void SetUICharInfo(NKMUnitData unitData)
	{
		NKCUtil.SetGameobjectActive(m_unitInfoSummary, unitData != null);
		if (unitData != null)
		{
			m_unitInfoSummary.SetData(unitData);
		}
	}

	private void SetTalk(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			NKCUtil.SetGameobjectActive(m_talkBox, bValue: false);
			return;
		}
		string speech = NKCNegotiateManager.GetSpeech(unitData, NKCNegotiateManager.SpeechType.Ready, bCheckVoiceBundle: true);
		if (string.IsNullOrEmpty(speech))
		{
			NKCUtil.SetGameobjectActive(m_talkBox, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_talkBox, bValue: true);
		m_talkBox.SetText(speech);
	}

	private void SetInfo(NKMUnitData unitData)
	{
		bool flag = unitData != null;
		NKCUtil.SetGameobjectActive(m_objLifetimeDefault, !flag);
		NKCUtil.SetGameobjectActive(m_objLifetimeReady, flag);
		if (flag)
		{
			int loyalty = unitData.loyalty;
			int num = 10000;
			NKCUtil.SetLabelText(m_txtLoyaltyGauge, $"{loyalty / 100}/{num / 100}");
			m_imgLoyaltyGauge.fillAmount = (float)loyalty / (float)num;
		}
	}

	private void SetCost()
	{
		long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(1024);
		m_costLifetime.SetData(1024, 1, countMiscItem);
	}

	private void SetButton(NKMUnitData unitData)
	{
		bool flag = CanLifetime(unitData) == NKM_ERROR_CODE.NEC_OK;
		if (flag)
		{
			m_btnLifetimeOK.UnLock();
		}
		else
		{
			m_btnLifetimeOK.Lock();
		}
		m_txtLifetimeOK.color = NKCUtil.GetButtonUIColor(flag);
		m_imgLifetimeOK.color = NKCUtil.GetButtonUIColor(flag);
	}

	private void OnTouchLifetime()
	{
		if (m_targetUnit != null)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = CanLifetime(m_targetUnit);
			switch (nKM_ERROR_CODE)
			{
			case NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM:
				NKCShopManager.OpenItemLackPopup(1024, 1);
				break;
			default:
				NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
				break;
			case NKM_ERROR_CODE.NEC_OK:
				NKCUILifetime.Instance.Open(m_targetUnit, replay: false);
				break;
			}
		}
	}

	private void OnTouchLifetimeInfo()
	{
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_INFORMATION, NKCUtilString.GET_STRING_LIFETIME_REWARD_INFO);
	}

	private NKM_ERROR_CODE CanLifetime(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(unitData.m_UnitID);
		if (unitTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (string.IsNullOrEmpty(unitTemplet.m_CutsceneLifetime_Start))
		{
			return NKM_ERROR_CODE.NEC_FAIL_PERMANENT_CONTRACT_INVALID_CONDITION;
		}
		if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(1024) < 1)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM;
		}
		if (!unitData.IsPermanentContractEnable())
		{
			return NKM_ERROR_CODE.NEC_FAIL_PERMANENT_CONTRACT_INVALID_CONDITION;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	private void OnSelectUnit()
	{
		NKCUIUnitSelectList.Instance.Open(MakeSelectUnitOption(), OnUnitSelected, OnUnitSortList);
		if (m_NKCUIUnitSelect != null)
		{
			m_NKCUIUnitSelect.Close();
		}
	}

	private NKCUIUnitSelectList.UnitSelectListOptions MakeSelectUnitOption()
	{
		NKCUIUnitSelectList.UnitSelectListOptions result = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
		result.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
		result.bDescending = true;
		result.bExcludeLockedUnit = false;
		result.bExcludeDeckedUnit = false;
		result.bCanSelectUnitInMission = true;
		result.bHideDeckedUnit = false;
		result.strUpsideMenuName = NKCUtilString.GET_STRING_LIFETIME;
		result.strEmptyMessage = NKCUtilString.GET_STRING_LIFETIME_NO_EXIST_UNIT;
		result.bShowHideDeckedUnitMenu = false;
		result.bEnableLockUnitSystem = false;
		result.m_SortOptions.AdditionalExcludeFilterFunc = CheckUnitCanLifetime;
		result.m_SortOptions.bIgnoreCityState = true;
		result.m_SortOptions.bIgnoreWorldMapLeader = true;
		result.bPushBackUnselectable = false;
		result.m_bUseFavorite = true;
		result.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		result.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		return result;
	}

	private bool CheckUnitCanLifetime(NKMUnitData unitData)
	{
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(unitData.m_UnitID);
		if (unitTemplet == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(unitTemplet.m_CutsceneLifetime_Start))
		{
			return false;
		}
		return unitData.IsPermanentContractEnable();
	}

	private void OnUnitSelected(List<long> lstUnitUID)
	{
		if (lstUnitUID != null && lstUnitUID.Count >= 1)
		{
			NKCUIUnitSelectList.CheckInstanceAndClose();
			long unitUid = lstUnitUID[0];
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(unitUid);
			SetData(unitFromUID);
		}
	}

	private void OnEventPanelClick(BaseEventData e)
	{
		if (!(m_UnitDragSelectView != null) || !m_UnitDragSelectView.GetDragOffset().IsNearlyZero())
		{
			return;
		}
		RectTransform currentItem = m_UnitDragSelectView.GetCurrentItem();
		if (currentItem != null)
		{
			NKCUICharacterView componentInChildren = currentItem.GetComponentInChildren<NKCUICharacterView>();
			if (componentInChildren != null && componentInChildren.HasCharacterIllust())
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				componentInChildren.OnPointerDown(eventData);
				componentInChildren.OnPointerUp(eventData);
			}
		}
	}

	private RectTransform MakeMainBannerListSlot()
	{
		GameObject obj = new GameObject("Banner", typeof(RectTransform), typeof(LayoutElement));
		LayoutElement component = obj.GetComponent<LayoutElement>();
		component.ignoreLayout = false;
		component.preferredWidth = m_UnitDragSelectView.m_rtContentRect.GetWidth();
		component.preferredHeight = m_UnitDragSelectView.m_rtContentRect.GetHeight();
		component.flexibleWidth = 2f;
		component.flexibleHeight = 2f;
		return obj.GetComponent<RectTransform>();
	}

	private void ProvideMainBannerListSlotData(Transform tr, int idx)
	{
		if (m_UnitSortList == null)
		{
			return;
		}
		NKMUnitData nKMUnitData = m_UnitSortList[idx];
		Debug.Log($"<color=yellow>target : {tr.name}, idx : {idx}, </color>");
		if (nKMUnitData != null)
		{
			NKCUICharacterView component = tr.GetComponent<NKCUICharacterView>();
			if (component != null)
			{
				component.SetCharacterIllust(nKMUnitData, bAsync: false, nKMUnitData.m_SkinID == 0);
				return;
			}
			NKCUICharacterView nKCUICharacterView = tr.gameObject.AddComponent<NKCUICharacterView>();
			nKCUICharacterView.m_rectIllustRoot = tr.GetComponent<RectTransform>();
			nKCUICharacterView.SetCharacterIllust(nKMUnitData, bAsync: false, nKMUnitData.m_SkinID == 0);
		}
	}

	private void ReturnMainBannerListSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		Object.Destroy(go.gameObject);
	}

	public void TouchCharacter(RectTransform rt, PointerEventData eventData)
	{
		if (m_UnitDragSelectView.GetDragOffset().IsNearlyZero())
		{
			NKCUICharacterView componentInChildren = rt.GetComponentInChildren<NKCUICharacterView>();
			if (componentInChildren != null)
			{
				componentInChildren.OnPointerDown(eventData);
				componentInChildren.OnPointerUp(eventData);
			}
		}
	}

	private void Focus(RectTransform rect, bool bFocus)
	{
		NKCUtil.SetGameobjectActive(rect.gameObject, bFocus);
	}

	public void SelectCharacter(int idx)
	{
		Debug.Log($"<color=yellow>SelectCharacter {idx}</color>");
		if (m_UnitSortList.Count < idx || idx < 0)
		{
			Debug.LogWarning($"무언가 잘못된 인덱스 인디? 총 갯수 : {m_UnitSortList.Count}, 목표 인덱스 : {idx}");
			return;
		}
		NKMUnitData nKMUnitData = m_UnitSortList[idx];
		if (nKMUnitData != null)
		{
			SetData(nKMUnitData);
		}
	}

	private void BannerCleanUp()
	{
		if (m_UnitDragSelectView != null)
		{
			NKCUICharacterView[] componentsInChildren = m_UnitDragSelectView.gameObject.GetComponentsInChildren<NKCUICharacterView>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CloseImmediatelyIllust();
			}
		}
	}

	private void OnUnitSortList(long UID, List<NKMUnitData> unitUIDList)
	{
		m_UnitSortList = unitUIDList;
		if (!(m_UnitDragSelectView != null))
		{
			return;
		}
		m_UnitDragSelectView.TotalCount = m_UnitSortList.Count;
		if (m_UnitSortList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < m_UnitSortList.Count; i++)
		{
			if (m_UnitSortList[i].m_UnitUID == UID)
			{
				m_UnitDragSelectView.SetIndex(i);
				break;
			}
		}
	}

	private void OpenSelectInstance()
	{
		m_AssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nuf_base", "NKM_UI_BASE_UNIT_SELECT");
		if (m_AssetInstanceData.m_Instant != null)
		{
			m_NKCUIUnitSelect = m_AssetInstanceData.m_Instant.GetComponent<NKCUIUnitSelect>();
			m_NKCUIUnitSelect.Init(OnClickCharacterChange);
			m_NKCUIUnitSelect.transform.SetParent(m_rtUnitSelectAnchor.transform, worldPositionStays: false);
			m_NKCUIUnitSelect.Open();
		}
	}

	private void CloseSelectInstance()
	{
		if (m_AssetInstanceData != null)
		{
			m_AssetInstanceData.Unload();
		}
	}

	private void OnClickCharacterChange()
	{
		if (m_NKCUIUnitSelect != null)
		{
			m_NKCUIUnitSelect.Outro();
		}
		OnSelectUnit();
	}

	private void OpenUnitSelect()
	{
		if (m_targetUnit == null)
		{
			if (m_NKCUIUnitSelect != null)
			{
				m_NKCUIUnitSelect.Open();
			}
			else
			{
				OpenSelectInstance();
			}
		}
		else if (m_NKCUIUnitSelect != null)
		{
			m_NKCUIUnitSelect.Close();
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (IsInstanceOpen)
		{
			UpdateUI();
		}
	}
}
