using System.Collections.Generic;
using Cs.Protocol;
using NKC.Templet;
using NKC.UI.Component;
using NKC.UI.Guide;
using NKC.UI.Shop;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionUnitInfoV2 : NKCUIBase
{
	public enum eCollectionState
	{
		CS_NONE,
		CS_PROFILE,
		CS_INFOMATION,
		CS_VOICE,
		CS_STATUS
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_collection";

	private const string UI_ASSET_NAME = "AB_UI_COLLECTION_INFO";

	private const string UI_ASSET_NAME_OTHER = "NKM_UI_COLLECTION_UNIT_INFO_OTHER";

	private static bool m_isGauntlet;

	private static bool m_bWillCloseUnderPopupOnOpen = true;

	private static NKCUICollectionUnitInfoV2 m_Instance;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	private static bool m_bIsDummyUnit = false;

	private NKCUIUpsideMenu.eMode m_UpsideMenuMode = NKCUIUpsideMenu.eMode.Normal;

	private int m_UnitID;

	private int m_SkinID;

	private static int m_SkinIDReserved;

	private NKMUnitData m_NKMUnitData;

	private List<NKMEquipItemData> m_listNKMEquipItemData;

	public GameObject m_objUnitIdRoot;

	public Text m_lbUnitId;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdâ")]
	public NKCUICharInfoSummary m_NKCUICharInfoSummary;

	public NKCUIOperatorSummary m_NKCUIOperInfoSummary;

	public NKCUIShipInfoSummary m_NKCUIShipInfoSummary;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdâ")]
	public GameObject m_objEquipSlotParent;

	public NKCUISlot m_slotEquipWeapon;

	public NKCUISlot m_slotEquipDefense;

	public NKCUISlot m_slotEquipAcc;

	public NKCUISlot m_slotEquipAcc_2;

	public NKCUISlot m_slotEquipReactor;

	[Header("\ufffd\ufffdƮ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffdƮ")]
	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON;

	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE;

	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC;

	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public ScrollRect m_srUnitIntroduce;

	public Text m_NKM_UI_COLLECTION_UNIT_PROFILE_UNIT_INTRODUCE_TEXT;

	[Header("\ufffdɷ\ufffdġ")]
	public GameObject m_objUnitStatsList;

	public GameObject m_objShipStatsList;

	public GameObject m_objOperatorStatsList;

	public TMP_Text m_lbUnitPowerText;

	public NKCUIComStateButton m_GuideBtn;

	public string m_GuideStrID;

	public NKCUIUnitStatSlot m_slotHP;

	public NKCUIUnitStatSlot m_slotAttack;

	public NKCUIUnitStatSlot m_slotDefense;

	public NKCUIUnitStatSlot m_slotHitRate;

	public NKCUIUnitStatSlot m_slotCritHitRate;

	public NKCUIUnitStatSlot m_slotEvade;

	[Header("\ufffd\ufffd")]
	public NKCUICollectionInfoTab m_UnitProfileTab;

	public NKCUICollectionUnitProfile m_UnitProfile;

	public NKCUICollectionInfoTab m_UnitInformationTab;

	public NKCUICollectionUnitDesc m_UnitInfomation;

	public NKCUICollectionInfoTab m_UnitVoiceTab;

	public NKCUICollectionUnitVoice m_UnitVoice;

	public NKCUICollectionInfoTab m_UnitStatusTab;

	public GameObject m_objUnitStatus;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffdϷ\ufffd\ufffd\ufffdƮ")]
	public NKCUICharacterView m_CharacterView;

	[Header("\ufffdϷ\ufffd\ufffd\ufffdƮ \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd忡\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\u0334\ufffd Rect\ufffd\ufffd. Base/ViewMode \ufffd\ufffd \ufffd\u0338\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public Animator m_ani_NKM_UI_COLLECTION_UNIT_INFO_CONTENT;

	[Header("\ufffd\ufffdŸ \ufffd\ufffdư")]
	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_PRACTICE;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY;

	public NKCUIComToggle m_tglUnitInfoDetailPopup;

	[Space]
	public GameObject m_NKM_UI_COLLECTION_UNIT_INFO_NOTICE_NOTGET;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON;

	public GameObject m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON_NEW;

	public GameObject m_UNIT_ACHIEVEMENT_COMPLETE;

	[Header("\ufffd\ufffdų \ufffdг\ufffd")]
	public NKCUIUnitInfoSkillPanel m_UISkillPanel;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objVoiceActor;

	public Text m_lbVoiceActorName;

	[Header("\ufffd\ufffdŲ \ufffd\ufffdư")]
	public NKCUIComStateButton m_SKIN_BUTTON;

	public GameObject m_SKIN_COUNT_ROOT;

	public Text m_SKIN_COUNT;

	public GameObject m_SKIN_COMPLETE;

	[Header("ĳ\ufffd\ufffd\ufffd\ufffd \ufffdǳ\ufffd")]
	public NKCUIComDragSelectablePanel m_DragCharacterView;

	public EventTrigger m_evtPanel;

	[Header("\ufffd±\ufffd")]
	public NKCUIComUnitTagList m_UnitTagList;

	private eCollectionState m_eCurrentState = eCollectionState.CS_PROFILE;

	private NKCUIUnitInfo.OpenOption m_OpenOption;

	private static NKCUIUnitInfo.OpenOption m_OpenOptionReserved = null;

	private bool m_bAppraisal;

	private bool m_bViewMode;

	[Header("SDĳ\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public RectTransform m_rtSDRoot;

	private NKCASUIUnitIllust m_spineSD;

	public float m_fSDScale = 1.2f;

	private int m_CurUnitID = -1;

	public override bool WillCloseUnderPopupOnOpen => m_bWillCloseUnderPopupOnOpen;

	public static NKCUICollectionUnitInfoV2 Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_loadedUIData = NKCUIManager.OpenNewInstance<NKCUICollectionUnitInfoV2>("ab_ui_collection", "AB_UI_COLLECTION_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
				m_Instance = m_loadedUIData.GetInstance<NKCUICollectionUnitInfoV2>();
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

	public override string MenuName => NKCUtilString.GET_STRING_UNIT_INFO;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => m_UpsideMenuMode;

	private static void CleanupInstance()
	{
		m_isGauntlet = false;
		m_bIsDummyUnit = false;
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_loadedUIData != null)
		{
			m_loadedUIData.CloseInstance();
			m_loadedUIData = null;
		}
	}

	private void OnDestroy()
	{
		CheckInstanceAndClose();
	}

	public void Init()
	{
		InitUI();
	}

	public void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE, OnClickChangeIllust);
		NKCUtil.SetButtonClickDelegate(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_PRACTICE, OnUnitTestButton);
		NKCUtil.SetButtonClickDelegate(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL, OnUnitAppraisal);
		NKCUtil.SetButtonClickDelegate(m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY, OnReplayLifetime);
		NKCUtil.SetButtonClickDelegate(m_SKIN_BUTTON, OnSkinButton);
		NKCUtil.SetGameobjectActive(m_tglUnitInfoDetailPopup, bValue: false);
		NKCUtil.SetToggleValueChangedDelegate(m_tglUnitInfoDetailPopup, OpenUnitInfoDetailPopup);
		if (m_DragCharacterView != null)
		{
			m_DragCharacterView.Init(rotation: true);
			m_DragCharacterView.dOnGetObject += MakeMainBannerListSlot;
			m_DragCharacterView.dOnReturnObject += ReturnMainBannerListSlot;
			m_DragCharacterView.dOnProvideData += ProvideMainBannerListSlotData;
			m_DragCharacterView.dOnIndexChangeListener += SelectCharacter;
			m_DragCharacterView.dOnFocus += Focus;
		}
		if (m_evtPanel != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnEventPanelClick);
			m_evtPanel.triggers.Add(entry);
		}
		if (m_UISkillPanel != null)
		{
			m_UISkillPanel.Init();
			m_UISkillPanel.SetOpenPopupWhenSelected();
		}
		m_slotEquipWeapon.Init();
		m_slotEquipWeapon.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		m_slotEquipDefense.Init();
		m_slotEquipDefense.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		m_slotEquipAcc.Init();
		m_slotEquipAcc.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		m_slotEquipAcc_2.Init();
		m_slotEquipAcc_2.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		m_slotEquipReactor?.Init();
		if (null != m_UnitProfileTab)
		{
			NKCUtil.SetToggleValueChangedDelegate(m_UnitProfileTab.m_tglToggle, delegate
			{
				ChangeState(eCollectionState.CS_PROFILE);
			});
		}
		if (null != m_UnitInformationTab)
		{
			NKCUtil.SetToggleValueChangedDelegate(m_UnitInformationTab.m_tglToggle, delegate
			{
				ChangeState(eCollectionState.CS_INFOMATION);
			});
		}
		if (null != m_UnitVoiceTab)
		{
			NKCUtil.SetToggleValueChangedDelegate(m_UnitVoiceTab.m_tglToggle, delegate
			{
				ChangeState(eCollectionState.CS_VOICE);
			});
		}
		if (null != m_UnitStatusTab)
		{
			NKCUtil.SetToggleValueChangedDelegate(m_UnitStatusTab.m_tglToggle, delegate
			{
				ChangeState(eCollectionState.CS_STATUS);
			});
		}
		NKCUtil.SetButtonClickDelegate(m_GuideBtn, (UnityAction)delegate
		{
			NKCUIPopUpGuide.Instance.Open(m_GuideStrID);
		});
		NKCUtil.SetButtonClickDelegate(m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON, OnClickUnitAchievement);
		m_NKCUICharInfoSummary.SetUnitClassRootActive(value: false);
		m_NKCUICharInfoSummary.Init(bShowLevel: false);
		m_UnitProfile?.Init();
		m_UnitInfomation?.Init();
		m_UnitVoice?.InitUI();
		base.gameObject.SetActive(value: false);
		NKCUtil.SetGameobjectActive(m_slotEquipReactor, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REACTOR));
	}

	private void OnEventPanelClick(BaseEventData e)
	{
		if (m_eCurrentState == eCollectionState.CS_VOICE)
		{
			m_UnitVoice?.StopVoice();
		}
		if (!(m_DragCharacterView != null) || m_DragCharacterView.GetDragOffset() != 0f)
		{
			return;
		}
		RectTransform currentItem = m_DragCharacterView.GetCurrentItem();
		if (currentItem != null)
		{
			NKCUICharacterView componentInChildren = currentItem.GetComponentInChildren<NKCUICharacterView>();
			if (componentInChildren != null)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				componentInChildren.OnPointerDown(eventData);
				componentInChildren.OnPointerUp(eventData);
			}
		}
	}

	private void ChangeUnit(NKMUnitData cNKMUnitData)
	{
		m_NKCUICharInfoSummary.SetData(cNKMUnitData);
		if (m_UnitTagList != null)
		{
			m_UnitTagList.SetData(cNKMUnitData);
		}
		SetData(cNKMUnitData);
		NKCUICharacterView componentInChildren = m_DragCharacterView.GetCurrentItem().GetComponentInChildren<NKCUICharacterView>();
		if (!m_isGauntlet && componentInChildren != null && componentInChildren.IsDiffrentCharacter(cNKMUnitData.m_UnitID, m_SkinID))
		{
			ChangeSkin(m_SkinID);
		}
		if (NKCPopupUnitInfoDetail.IsInstanceOpen)
		{
			NKCPopupUnitInfoDetail.InstanceOpen(m_NKMUnitData, NKCPopupUnitInfoDetail.UnitInfoDetailType.gauntlet_collection_v2, m_listNKMEquipItemData, delegate
			{
				m_tglUnitInfoDetailPopup.Select(bSelect: false, bForce: true);
			});
		}
	}

	private RectTransform MakeMainBannerListSlot()
	{
		GameObject obj = new GameObject("Banner", typeof(RectTransform), typeof(LayoutElement));
		LayoutElement component = obj.GetComponent<LayoutElement>();
		component.ignoreLayout = false;
		component.preferredWidth = m_DragCharacterView.m_rtContentRect.GetWidth();
		component.preferredHeight = m_DragCharacterView.m_rtContentRect.GetHeight();
		component.flexibleWidth = 2f;
		component.flexibleHeight = 2f;
		return obj.GetComponent<RectTransform>();
	}

	private void ProvideMainBannerListSlotData(Transform tr, int idx)
	{
		if (m_OpenOption == null || m_OpenOption.m_lstUnitData == null)
		{
			return;
		}
		NKMUnitData nKMUnitData = m_OpenOption.m_lstUnitData[idx];
		Debug.Log($"<color=yellow>target : {tr.name}, idx : {idx}, </color>");
		if (nKMUnitData != null)
		{
			m_SkinID = 0;
			NKCUICharacterView component = tr.GetComponent<NKCUICharacterView>();
			if (component != null)
			{
				component.SetCharacterIllust(nKMUnitData);
				return;
			}
			NKCUICharacterView nKCUICharacterView = tr.gameObject.AddComponent<NKCUICharacterView>();
			nKCUICharacterView.m_rectIllustRoot = tr.GetComponent<RectTransform>();
			nKCUICharacterView.SetCharacterIllust(nKMUnitData);
		}
	}

	private void ReturnMainBannerListSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		Object.Destroy(go.gameObject);
	}

	public void TouchCharacter(RectTransform rt, PointerEventData eventData)
	{
		if (m_DragCharacterView.GetDragOffset() == 0f)
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

	private void FocusColor(RectTransform rect, Color ApplyColor)
	{
		NKCUICharacterView componentInChildren = rect.gameObject.GetComponentInChildren<NKCUICharacterView>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(ApplyColor);
		}
	}

	public void SelectCharacter(int idx)
	{
		if (m_OpenOption.m_lstUnitData.Count >= idx && idx >= 0)
		{
			NKMUnitData nKMUnitData = m_OpenOption.m_lstUnitData[idx];
			if (nKMUnitData != null)
			{
				ChangeUnit(nKMUnitData);
			}
		}
	}

	private void BannerCleanUp()
	{
		if (m_DragCharacterView != null)
		{
			NKCUICharacterView[] componentsInChildren = m_DragCharacterView.gameObject.GetComponentsInChildren<NKCUICharacterView>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CloseImmediatelyIllust();
			}
		}
	}

	public static void CheckInstanceAndOpen(NKMUnitData cNKMUnitData, NKCUIUnitInfo.OpenOption openOption, List<NKMEquipItemData> listNKMEquipItemData = null, eCollectionState eStartingState = eCollectionState.CS_PROFILE, bool isGauntlet = false, NKCUIUpsideMenu.eMode upsideMenuMode = NKCUIUpsideMenu.eMode.Normal, bool bWillCloseUnderPopupOnOpen = true, bool bDummyUnit = false)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData);
		if (unitTempletBase == null || unitTempletBase.IsUnitDescNullOrEmplty())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENT_UNIT_DETAIL_INFO_NOT_POSSIBLE);
			return;
		}
		m_isGauntlet = isGauntlet;
		m_bIsDummyUnit = bDummyUnit;
		Instance.Open(cNKMUnitData, openOption, listNKMEquipItemData, eStartingState, upsideMenuMode, bWillCloseUnderPopupOnOpen);
	}

	private void Open(NKMUnitData cNKMUnitData, NKCUIUnitInfo.OpenOption openOption, List<NKMEquipItemData> listNKMEquipItemData = null, eCollectionState eStartingState = eCollectionState.CS_PROFILE, NKCUIUpsideMenu.eMode upsideMenuMode = NKCUIUpsideMenu.eMode.Normal, bool bWillCloseUnderPopupOnOpen = true)
	{
		m_eCurrentState = eCollectionState.CS_NONE;
		m_UpsideMenuMode = upsideMenuMode;
		m_bWillCloseUnderPopupOnOpen = bWillCloseUnderPopupOnOpen;
		m_listNKMEquipItemData = listNKMEquipItemData;
		m_OpenOption = openOption;
		if (m_OpenOption == null)
		{
			if (m_OpenOptionReserved != null)
			{
				m_OpenOption = m_OpenOptionReserved;
			}
			else
			{
				m_OpenOption = new NKCUIUnitInfo.OpenOption(new List<long>());
				m_OpenOption.m_lstUnitData.Add(cNKMUnitData);
			}
		}
		m_OpenOptionReserved = null;
		if (m_DragCharacterView != null)
		{
			if (m_OpenOption.m_lstUnitData.Count == 0)
			{
				m_OpenOption.m_lstUnitData.Add(cNKMUnitData);
			}
			m_DragCharacterView.TotalCount = m_OpenOption.m_lstUnitData.Count;
			for (int i = 0; i < m_OpenOption.m_lstUnitData.Count; i++)
			{
				if (m_OpenOption.m_lstUnitData[i].m_UnitUID == cNKMUnitData.m_UnitUID)
				{
					m_DragCharacterView.SetIndex(i);
					break;
				}
			}
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		SetData(cNKMUnitData);
		ChangeSkin(cNKMUnitData.m_SkinID);
		if (m_SkinIDReserved != 0)
		{
			NKCUICharacterView nKCUICharacterView = m_DragCharacterView.GetCurrentItem()?.GetComponentInChildren<NKCUICharacterView>();
			if (nKCUICharacterView != null && nKCUICharacterView.IsDiffrentCharacter(cNKMUnitData.m_UnitID, m_SkinIDReserved))
			{
				ChangeSkin(m_SkinIDReserved);
			}
			m_SkinIDReserved = 0;
		}
		int num = 0;
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		if (m_isGauntlet)
		{
			flag = false;
			flag2 = false;
			flag3 = false;
			eStartingState = eCollectionState.CS_STATUS;
			m_tglUnitInfoDetailPopup.Select(bSelect: false);
		}
		NKCUtil.SetGameobjectActive(m_UnitProfileTab, flag);
		NKCUtil.SetGameobjectActive(m_UnitInformationTab, flag2);
		NKCUtil.SetGameobjectActive(m_UnitVoiceTab, flag3);
		NKCUtil.SetGameobjectActive(m_UnitStatusTab, flag4);
		if (flag)
		{
			m_UnitProfileTab.SetTabNumber(++num);
		}
		if (flag2)
		{
			m_UnitInformationTab.SetTabNumber(++num);
		}
		if (flag3)
		{
			m_UnitVoiceTab.SetTabNumber(++num);
		}
		if (flag4)
		{
			m_UnitStatusTab.SetTabNumber(++num);
		}
		m_UnitProfileTab.Select(select: false, force: true);
		m_UnitInformationTab.Select(select: false, force: true);
		m_UnitVoiceTab.Select(select: false, force: true);
		m_UnitStatusTab.Select(select: false, force: true);
		switch (eStartingState)
		{
		case eCollectionState.CS_PROFILE:
			m_UnitProfileTab.Select(select: true);
			break;
		case eCollectionState.CS_INFOMATION:
			m_UnitInformationTab.Select(select: true);
			break;
		case eCollectionState.CS_VOICE:
			m_UnitVoiceTab.Select(select: true);
			break;
		case eCollectionState.CS_STATUS:
			m_UnitStatusTab.Select(select: true);
			break;
		}
		NKCUtil.SetGameobjectActive(m_NKCUICharInfoSummary.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKCUIShipInfoSummary.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUIOperInfoSummary.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUnitStatsList, bValue: true);
		NKCUtil.SetGameobjectActive(m_objShipStatsList, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOperatorStatsList, bValue: false);
		UIOpened();
	}

	private void ChangeState(eCollectionState newStat)
	{
		if (m_eCurrentState != newStat && !m_bViewMode)
		{
			m_eCurrentState = newStat;
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		switch (m_eCurrentState)
		{
		case eCollectionState.CS_PROFILE:
			NKCUtil.SetGameobjectActive(m_UnitProfile, bValue: true);
			NKCUtil.SetGameobjectActive(m_UnitInfomation, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitVoice, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitStatus, bValue: false);
			m_UnitProfile?.ResetCRFGauge();
			m_UnitProfile?.SetData(m_UnitID);
			break;
		case eCollectionState.CS_INFOMATION:
			NKCUtil.SetGameobjectActive(m_UnitProfile, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitInfomation, bValue: true);
			NKCUtil.SetGameobjectActive(m_UnitVoice, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitStatus, bValue: false);
			m_UnitInfomation?.SetData(m_UnitID);
			break;
		case eCollectionState.CS_VOICE:
			NKCUtil.SetGameobjectActive(m_UnitProfile, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitInfomation, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitVoice, bValue: true);
			NKCUtil.SetGameobjectActive(m_objUnitStatus, bValue: false);
			m_UnitVoice?.SetData(m_NKMUnitData);
			break;
		case eCollectionState.CS_STATUS:
			NKCUtil.SetGameobjectActive(m_UnitProfile, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitInfomation, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitVoice, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitStatus, bValue: true);
			break;
		}
	}

	private void SetData(NKMUnitData unitData)
	{
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(unitData.m_UnitID);
		NKCUtil.SetGameobjectActive(m_objUnitIdRoot, unitTemplet != null && !unitTemplet.m_bExclude);
		string employeeNumber = NKCCollectionManager.GetEmployeeNumber(unitData.m_UnitID);
		NKCUtil.SetLabelText(m_lbUnitId, employeeNumber);
		m_UISkillPanel.SetData(unitData, bDisplayEmptySlot: true);
		if (m_NKMUnitData == null || m_UnitID != unitData.m_UnitID)
		{
			if (m_NKMUnitData == null)
			{
				m_NKMUnitData = new NKMUnitData();
			}
			m_NKMUnitData.DeepCopyFrom(unitData);
			m_UnitID = unitData.m_UnitID;
			m_SkinID = 0;
			SetUnitDiscription(m_UnitID);
			SetDetailedStat(m_NKMUnitData);
			m_NKCUICharInfoSummary.SetData(m_NKMUnitData);
			if (m_UnitTagList != null)
			{
				m_UnitTagList.SetData(m_NKMUnitData);
			}
			CheckHasUnit(m_UnitID);
			SetVoiceData();
			SetLifetimeButtonUI(m_UnitID);
			if (m_listNKMEquipItemData != null || m_isGauntlet)
			{
				NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: true);
				UpdateEquipSlots();
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_NOTICE_NOTGET, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_PRACTICE, !m_isGauntlet);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REVIEW_SYSTEM) && !m_isGauntlet);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON, NKCUnitMissionManager.GetOpenTagCollectionMission() && !m_isGauntlet);
			NKCUtil.SetGameobjectActive(m_objVoiceActor, !m_isGauntlet);
			NKCUtil.SetGameobjectActive(m_tglUnitInfoDetailPopup, m_isGauntlet);
			NKCUtil.SetGameobjectActive(m_SKIN_BUTTON, !m_isGauntlet);
			UpdateReactorSlot();
			UpdateUnitMission();
			NKCUtil.SetLabelText(m_lbVoiceActorName, NKCVoiceActorNameTemplet.FindActorName(unitData));
			UpdateSkinButton();
		}
	}

	private void CheckHasUnit(int iUnitID)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_NOTICE_NOTGET, armyData.IsFirstGetUnit(iUnitID));
	}

	private void SetUnitDiscription(int unitID)
	{
		if (m_eCurrentState == eCollectionState.CS_PROFILE)
		{
			m_UnitProfile?.SetData(unitID);
		}
		else if (m_eCurrentState == eCollectionState.CS_INFOMATION)
		{
			m_UnitInfomation?.SetData(unitID);
		}
	}

	private void SetDetailedStat(NKMUnitData unitData)
	{
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		bool bPvP = false;
		NKMStatData nKMStatData = new NKMStatData();
		nKMStatData.Init();
		nKMStatData.MakeBaseStat(null, bPvP, unitData, unitStatTemplet.m_StatData);
		if (m_listNKMEquipItemData != null)
		{
			NKMInventoryData nKMInventoryData = new NKMInventoryData();
			nKMInventoryData.AddItemEquip(m_listNKMEquipItemData);
			nKMStatData.MakeBaseBonusFactor(unitData, nKMInventoryData.EquipItems, null, null);
			NKCUtil.SetLabelText(m_lbUnitPowerText, unitData.CalculateOperationPower(nKMInventoryData).ToString("N0"));
		}
		else
		{
			nKMStatData.MakeBaseBonusFactor(unitData, null, null, null);
			NKCUtil.SetLabelText(m_lbUnitPowerText, unitData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData).ToString("N0"));
		}
		m_slotHP.SetStat(NKM_STAT_TYPE.NST_HP, nKMStatData, unitData);
		m_slotAttack.SetStat(NKM_STAT_TYPE.NST_ATK, nKMStatData, unitData);
		m_slotDefense.SetStat(NKM_STAT_TYPE.NST_DEF, nKMStatData, unitData);
		m_slotHitRate.SetStat(NKM_STAT_TYPE.NST_HIT, nKMStatData, unitData);
		m_slotCritHitRate.SetStat(NKM_STAT_TYPE.NST_CRITICAL, nKMStatData, unitData);
		m_slotEvade.SetStat(NKM_STAT_TYPE.NST_EVADE, nKMStatData, unitData);
	}

	public override void OnBackButton()
	{
		if (m_bViewMode)
		{
			OnClickChangeIllust();
		}
		else
		{
			base.OnBackButton();
		}
	}

	public override void UnHide()
	{
		m_bAppraisal = false;
		base.UnHide();
	}

	public override void CloseInternal()
	{
		BannerCleanUp();
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		if (m_spineSD != null)
		{
			m_spineSD.Unload();
			m_spineSD = null;
		}
		NKCPopupUnitInfoDetail.CheckInstanceAndClose();
		NKCUIPopupIllustView.CheckInstanceAndClose();
		NKCUIPopupCollectionAchievement.CheckInstanceAndClose();
		m_UnitProfile?.ResetCRFGauge();
		m_NKMUnitData = null;
		m_UnitID = 0;
		m_SkinID = 0;
	}

	private void OnUnitTestButton()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_PRIVATE_PVP_UNUSABLE_FUNCTION, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (!m_bViewMode)
		{
			if (m_eCurrentState == eCollectionState.CS_VOICE)
			{
				m_UnitVoice?.StopVoice();
			}
			NKM_SHORTCUT_TYPE returnUIShortcut = ((NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION) ? NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().GetCurrentShortcutType() : NKM_SHORTCUT_TYPE.SHORTCUT_NONE);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
			string returnUIShortcutParam = ((unitTempletBase != null) ? unitTempletBase.m_UnitStrID : "");
			if (unitTempletBase != null)
			{
				m_OpenOptionReserved = m_OpenOption;
				m_SkinIDReserved = m_SkinID;
			}
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OpenPracticeGameComfirmPopup(m_NKMUnitData, returnUIShortcut, returnUIShortcutParam);
		}
	}

	private void OnUnitAppraisal()
	{
		if (!m_bViewMode)
		{
			NKCUIUnitReview.Instance.OpenUI(m_UnitID);
			m_bAppraisal = true;
		}
	}

	private void OnClickChangeIllust()
	{
		if (!m_bAppraisal)
		{
			if (m_eCurrentState == eCollectionState.CS_VOICE)
			{
				m_UnitVoice?.StopVoice();
			}
			NKCUIPopupIllustView.Instance.Open(m_NKMUnitData);
		}
	}

	public void ChangeSkin(int skinID)
	{
		if (m_OpenOption != null && m_DragCharacterView != null && m_DragCharacterView.CurrentIndex >= 0 && m_OpenOption.m_lstUnitData.Count > m_DragCharacterView.CurrentIndex)
		{
			NKMUnitData nKMUnitData = m_OpenOption.m_lstUnitData[m_DragCharacterView.CurrentIndex];
			if (nKMUnitData != null)
			{
				if (skinID != 0)
				{
					NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
					if (!NKMSkinManager.IsSkinForCharacter(nKMUnitData.m_UnitID, skinTemplet))
					{
						return;
					}
				}
				if (m_DragCharacterView.GetCurrentItem() != null)
				{
					NKCUICharacterView componentInChildren = m_DragCharacterView.GetCurrentItem().GetComponentInChildren<NKCUICharacterView>();
					if (componentInChildren != null)
					{
						componentInChildren.CloseImmediatelyIllust();
						m_SkinID = skinID;
						m_NKMUnitData.m_SkinID = skinID;
						componentInChildren.SetCharacterIllust(nKMUnitData.m_UnitID, skinID);
					}
				}
				NKCUtil.SetLabelText(m_lbVoiceActorName, NKCVoiceActorNameTemplet.FindActorName(m_NKMUnitData));
			}
		}
		if (m_eCurrentState == eCollectionState.CS_VOICE)
		{
			m_UnitVoice?.SetData(m_NKMUnitData);
		}
	}

	private void OpenSDIllust(NKMUnitData unitData, int skinID)
	{
		if (unitData == null)
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
			return;
		}
		if (m_spineSD != null)
		{
			m_spineSD.Unload();
			m_spineSD = null;
		}
		m_spineSD = NKCResourceUtility.OpenSpineSD(unitData.m_UnitID, skinID);
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

	private void SetVoiceData()
	{
		if (m_eCurrentState == eCollectionState.CS_VOICE)
		{
			m_UnitVoice?.SetData(m_NKMUnitData);
		}
	}

	private void SetLifetimeButtonUI(int unitID)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		bool bValue = false;
		if (armyData.IsCollectedUnit(unitID))
		{
			bValue = armyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unitID, NKMArmyData.UNIT_SEARCH_OPTION.Devotion, 0);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY, bValue);
	}

	private void OnReplayLifetime()
	{
		if (NKMSkinManager.IsCharacterHasSkin(m_UnitID))
		{
			NKCUIPopupCollectionSkinSelect.Instance.Open(m_NKMUnitData);
		}
		else
		{
			NKCUILifetime.Instance.Open(m_NKMUnitData, replay: true);
		}
	}

	public void OpenUnitInfoDetailPopup(bool value)
	{
		if (!value)
		{
			NKCPopupUnitInfoDetail.CheckInstanceAndClose();
			return;
		}
		NKCPopupUnitInfoDetail.InstanceOpen(m_NKMUnitData, NKCPopupUnitInfoDetail.UnitInfoDetailType.gauntlet_collection_v2, m_listNKMEquipItemData, delegate
		{
			m_tglUnitInfoDetailPopup.Select(bSelect: false, bForce: true);
		});
	}

	public void UpdateEquipSlots()
	{
		if (m_NKMUnitData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
			if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
			{
				NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: true);
				if (m_bIsDummyUnit && m_listNKMEquipItemData != null)
				{
					UpdateEquipItemSlot(m_listNKMEquipItemData[0], ref m_slotEquipWeapon, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON, ref m_listNKMEquipItemData);
					UpdateEquipItemSlot(m_listNKMEquipItemData[1], ref m_slotEquipDefense, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE, ref m_listNKMEquipItemData);
					UpdateEquipItemSlot(m_listNKMEquipItemData[2], ref m_slotEquipAcc, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC, ref m_listNKMEquipItemData);
					UpdateEquipItemSlot(m_listNKMEquipItemData[3], ref m_slotEquipAcc_2, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02, ref m_listNKMEquipItemData);
				}
				else
				{
					UpdateEquipItemSlot(m_NKMUnitData.GetEquipItemWeaponUid(), ref m_slotEquipWeapon, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON);
					UpdateEquipItemSlot(m_NKMUnitData.GetEquipItemDefenceUid(), ref m_slotEquipDefense, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE);
					UpdateEquipItemSlot(m_NKMUnitData.GetEquipItemAccessoryUid(), ref m_slotEquipAcc, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC);
					UpdateEquipItemSlot(m_NKMUnitData.GetEquipItemAccessory2Uid(), ref m_slotEquipAcc_2, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02);
				}
				UpdateReactorSlot();
			}
			if (!m_NKMUnitData.IsUnlockAccessory2())
			{
				m_slotEquipAcc_2.SetLock(OnSetLockMessage);
				m_slotEquipAcc_2.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
			}
			SetDetailedStat(m_NKMUnitData);
		}
		else
		{
			m_slotEquipAcc_2.SetLock(OnSetLockMessage);
			m_slotEquipAcc_2.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
	}

	private void OnSetLockMessage(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EQUIP_ACC_2_LOCKED_DESC);
	}

	private NKMEquipItemData GetItemEquip(long itemUID)
	{
		if (m_listNKMEquipItemData != null)
		{
			return m_listNKMEquipItemData.Find((NKMEquipItemData x) => x.m_ItemUid == itemUID);
		}
		return null;
	}

	private void UpdateEquipItemSlot(long equipItemUID, ref NKCUISlot slot, NKCUISlot.OnClick func, GameObject effObj, Animator effAni)
	{
		bool flag = false;
		NKCUtil.SetGameobjectActive(effObj, bValue: true);
		if (equipItemUID > 0)
		{
			NKMEquipItemData itemEquip = GetItemEquip(equipItemUID);
			if (itemEquip != null)
			{
				slot.SetData(NKCUISlot.SlotData.MakeEquipData(itemEquip), bShowName: false, bShowNumber: true, bEnableLayoutElement: false, OpenEquipBoxForInspection);
				flag = true;
				if (NKMItemManager.IsActiveSetOptionItem(itemEquip) && effAni != null)
				{
					NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(itemEquip.m_SetOptionId);
					if (equipSetOptionTemplet != null)
					{
						effAni.SetTrigger(equipSetOptionTemplet.m_EquipSetIconEffect);
					}
				}
			}
		}
		NKCUtil.SetGameobjectActive(effObj, bValue: false);
		if (!flag)
		{
			slot.SetCustomizedEmptySP(GetCustomizedEquipEmptySP());
			slot.SetEmpty(func);
		}
		slot.SetUsedMark(bVal: false);
	}

	private void UpdateEquipItemSlot(NKMEquipItemData equipItemData, ref NKCUISlot slot, NKCUISlot.OnClick func, GameObject effObj, Animator effAni, ref List<NKMEquipItemData> lstEquipDatas)
	{
		bool flag = false;
		NKCUtil.SetGameobjectActive(effObj, bValue: true);
		if (equipItemData != null)
		{
			slot.SetData(NKCUISlot.SlotData.MakeEquipData(equipItemData), bShowName: false, bShowNumber: true, bEnableLayoutElement: false, OpenEquipBoxForInspection);
			flag = true;
			NKMEquipmentSet equipSet = new NKMEquipmentSet(lstEquipDatas[0], lstEquipDatas[1], lstEquipDatas[2], lstEquipDatas[3]);
			if (NKMItemManager.IsActiveSetOptionItem(equipItemData, equipSet) && effAni != null)
			{
				NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(equipItemData.m_SetOptionId);
				if (equipSetOptionTemplet != null)
				{
					effAni.SetTrigger(equipSetOptionTemplet.m_EquipSetIconEffect);
				}
			}
		}
		NKCUtil.SetGameobjectActive(effObj, bValue: false);
		if (!flag)
		{
			slot.SetCustomizedEmptySP(GetCustomizedEquipEmptySP());
			slot.SetEmpty(func);
		}
		slot.SetUsedMark(bVal: false);
	}

	private void UpdateReactorSlot()
	{
		if (m_NKMUnitData != null && NKCReactorUtil.IsReactorUnit(m_NKMUnitData.GetUnitTempletBase()))
		{
			if (!NKCReactorUtil.CheckCanTryLevelUp(m_NKMUnitData))
			{
				return;
			}
			if (m_NKMUnitData.reactorLevel == 0)
			{
				m_slotEquipReactor.SetEmpty();
				return;
			}
			NKMUnitReactorTemplet reactorTemplet = NKCReactorUtil.GetReactorTemplet(m_NKMUnitData.GetUnitTempletBase());
			if (reactorTemplet != null)
			{
				NKCUISlot.SlotData slotData = new NKCUISlot.SlotData();
				slotData.eType = NKCUISlot.eSlotMode.UnitReactor;
				slotData.ID = m_NKMUnitData.m_UnitID;
				slotData.UID = m_NKMUnitData.m_UnitUID;
				slotData.Count = m_NKMUnitData.reactorLevel;
				m_slotEquipReactor.SetData(slotData, bEnableLayoutElement: true, OnReactorSlotClick);
			}
		}
		else
		{
			m_slotEquipReactor.SetLockReactorNotHasReactor();
		}
	}

	private void OnReactorSlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKMUnitReactorTemplet reactorTemplet = NKCReactorUtil.GetReactorTemplet(m_NKMUnitData.GetUnitTempletBase());
		if (reactorTemplet != null)
		{
			NKCPopupItemEquipBox.Open(reactorTemplet, m_NKMUnitData.reactorLevel);
		}
	}

	private Sprite GetCustomizedEquipEmptySP()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_NKMUnitData.m_UnitID);
		if (unitTempletBase == null)
		{
			return null;
		}
		NKM_UNIT_STYLE_TYPE nKM_UNIT_STYLE_TYPE = unitTempletBase.m_NKM_UNIT_STYLE_TYPE;
		if ((uint)(nKM_UNIT_STYLE_TYPE - 1) <= 2u)
		{
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_inven_icon_common", "AB_INVEN_ICON_FRAME_EMPTY");
		}
		return null;
	}

	private void OpenEquipBoxForInspection(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKMEquipItemData itemEquip = GetItemEquip(slotData.UID);
		if (itemEquip != null)
		{
			NKCPopupItemEquipBox.Open(itemEquip, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
	}

	private void UpdateUnitMission()
	{
		UpdateUnitMissionState();
		if (NKCUIPopupCollectionAchievement.IsInstanceOpen)
		{
			NKCUIPopupCollectionAchievement.Instance.Open(m_UnitID);
		}
	}

	public void UpdateUnitMissionState()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON_NEW, NKCUnitMissionManager.HasRewardEnableMission(m_UnitID));
		int total = 0;
		int achieved = 0;
		NKCUnitMissionManager.GetUnitMissionAchievedCount(m_UnitID, ref total, ref achieved);
		NKCUtil.SetGameobjectActive(m_UNIT_ACHIEVEMENT_COMPLETE, total > 0 && total <= achieved);
	}

	private void UpdateSkinButton()
	{
		if (m_NKMUnitData != null && NKMSkinManager.IsCharacterHasSkin(m_NKMUnitData.m_UnitID))
		{
			List<NKMSkinTemplet> skinlistForCharacter = NKMSkinManager.GetSkinlistForCharacter(m_NKMUnitData.m_UnitID, NKCScenManager.CurrentUserData().m_InventoryData);
			if (skinlistForCharacter == null)
			{
				NKCUtil.SetGameobjectActive(m_SKIN_COMPLETE, bValue: false);
				NKCUtil.SetGameobjectActive(m_SKIN_COUNT_ROOT, bValue: false);
				return;
			}
			int num = 0;
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			foreach (NKMSkinTemplet item in skinlistForCharacter)
			{
				if (item != null && nKMUserData != null && nKMUserData.m_InventoryData.HasItemSkin(item.m_SkinID))
				{
					num++;
				}
			}
			bool flag = num >= skinlistForCharacter.Count;
			NKCUtil.SetGameobjectActive(m_SKIN_COMPLETE, flag);
			NKCUtil.SetGameobjectActive(m_SKIN_COUNT_ROOT, !flag);
			if (!flag)
			{
				NKCUtil.SetLabelText(m_SKIN_COUNT, $"{num + 1}/{skinlistForCharacter.Count + 1}");
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_SKIN_COMPLETE, bValue: false);
			NKCUtil.SetGameobjectActive(m_SKIN_COUNT_ROOT, bValue: false);
		}
	}

	private bool IsSeizedUnit(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return true;
		}
		if (unitData.IsSeized)
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED);
			return true;
		}
		return false;
	}

	private void OnClickUnitAchievement()
	{
		if (m_eCurrentState == eCollectionState.CS_VOICE)
		{
			m_UnitVoice?.StopVoice();
		}
		NKCUIPopupCollectionAchievement.Instance.Open(m_UnitID);
	}

	private void OnSkinButton()
	{
		if (!IsSeizedUnit(m_NKMUnitData))
		{
			if (m_eCurrentState == eCollectionState.CS_VOICE)
			{
				m_UnitVoice?.StopVoice();
			}
			NKCUIShopSkinPopup.Instance.OpenForUnitInfo(m_NKMUnitData, bShowUpsideMenu: false, bUnitCollection: true, ChangeSkin);
		}
	}
}
