using System.Collections.Generic;
using ClientPacket.Unit;
using Cs.Math;
using DG.Tweening;
using NKC.UI.Component;
using NKC.UI.NPC;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILab : NKCUIBase
{
	public enum LAB_DETAIL_STATE
	{
		LDS_INVALID,
		LDS_MENU,
		LDS_UNIT_ENHANCE,
		LDS_UNIT_LIMITBREAK,
		LDS_UNIT_SKILL_TRAIN
	}

	private enum UI_ANIM_TYPE
	{
		UAT_INVALID = -1,
		UAT_IN,
		UAT_OUT
	}

	public delegate void OnSelectUnitPlayVoice(NKM_UNIT_STYLE_TYPE unitStyleType, LAB_DETAIL_STATE labState);

	public enum MenuTransitionType
	{
		Start,
		MenuToDetail,
		DetailToMenu,
		CharacterChange,
		DetailToDetail,
		MenuDirectOpen
	}

	public struct strSpineInfo
	{
		public int index;

		public GameObject objSpine;

		public strSpineInfo(int idx, GameObject obj)
		{
			index = idx;
			objSpine = obj;
		}
	}

	public class ContractBannerInfo
	{
		public Dictionary<int, strSpineInfo> m_LstBanner = new Dictionary<int, strSpineInfo>();
	}

	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_lab";

	public const string UI_ASSET_NAME = "NKM_UI_LAB";

	[Header("왼쪽 캐릭터 정보")]
	public RectTransform m_rtCharacterInfo;

	public NKCUILabCharacterInfo m_NKCUILabCharacterInfo;

	private NKCUIComButton m_NKM_UI_LAB_UNIT_CHANGE_btn;

	private GameObject m_objNKM_UI_LAB_UNIT_CHANGE;

	[Header("숏컷 메뉴")]
	public GameObject m_objRootShortcutMenu;

	public RectTransform m_rtShortcutMenu;

	public NKCUIComButton m_cbtnShortcutEnhance;

	public NKCUIComButton m_cbtnShortcutLimitBreak;

	public NKCUIComButton m_cbtnShortcutTraining;

	[Header("캐릭터 스파인 일러스트")]
	public RectTransform m_rtCharacterIllust;

	[Header("초월")]
	public NKCUILabLimitBreak m_NKCUILabLimitBreak;

	private RectTransform m_rtLimitBreak;

	[Header("강화")]
	public NKCUILabUnitEnhance m_NKCUILabUnitEnhance;

	private RectTransform m_rtUnitEnhance;

	[Header("훈련")]
	public NKCUILabSkillTrain m_NKCUILabSkillTrain;

	private RectTransform m_rtSkillTrain;

	[Header("이펙트 관련")]
	public Animator m_animEffect;

	public RectTransform m_rtEffect;

	public GameObject m_objNKM_UI_LAB_TRAINING_EMPTY;

	public GameObject m_objNKM_UI_LAB_LIMITBREAK_EMPTY_ROOT;

	public NKCUIUnitSelect m_NKCUIUnitSelect;

	[Header("캐릭터 판넬")]
	public NKCUIComDragSelectablePanel m_DragCharacterView;

	public EventTrigger m_evtPanel;

	private OnSelectUnitPlayVoice dOnSelectUnitPlayVoice;

	private LAB_DETAIL_STATE m_LAB_DETAIL_STATE;

	private LAB_DETAIL_STATE m_targetState;

	private NKMUnitData m_CurrentUnitData;

	private NKCASUIUnitIllust m_NKCASUISpineIllust;

	private bool m_bForceReserveOpen;

	private const float UI_TRANSITION_TIME = 0.4f;

	private Dictionary<long, int> dicUnitLimitBreak = new Dictionary<long, int>();

	private NKCUnitSortSystem.UnitListOptions m_preUnitListOption;

	private NKCUnitSortSystem.eFilterOption m_preFilterOption;

	private int m_SelectUnitIndex;

	private List<NKMUnitData> m_UnitSortList = new List<NKMUnitData>();

	private int m_iCheckUnitCnt;

	private int m_iMaxUnitCnt;

	private bool bNextClick;

	protected ContractBannerInfo m_BannerSet = new ContractBannerInfo();

	public override string MenuName => NKCUtilString.GetLabMenuName(m_LAB_DETAIL_STATE);

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (m_LAB_DETAIL_STATE == LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN)
			{
				return new List<int> { 3, 1, 2, 101 };
			}
			return base.UpsideMenuShowResourceList;
		}
	}

	public override string GuideTempletID => m_LAB_DETAIL_STATE switch
	{
		LAB_DETAIL_STATE.LDS_UNIT_ENHANCE => "ARTICLE_UNIT_ENCHANT", 
		LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK => "ARTICLE_UNIT_LIMITBREAK", 
		LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN => "ARTICLE_UNIT_TRAINING", 
		_ => "", 
	};

	public static NKCAssetResourceData OpenInstanceAsync()
	{
		return NKCUIBase.OpenInstanceAsync<NKCUILab>("ab_ui_nkm_ui_lab", "NKM_UI_LAB");
	}

	public static bool CheckInstanceLoaded(NKCAssetResourceData loadResourceData, out NKCUILab retVal)
	{
		return NKCUIBase.CheckInstanceLoaded<NKCUILab>(loadResourceData, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), out retVal);
	}

	public void InitUI(NKCUILabLimitBreak.OnTryLimitBreak onTryLimitBreak, NKCUILabSkillTrain.OnTrySkillTrain onTrySkillTrain, NKCUINPCProfessorOlivia npcLabProfessorOlivia)
	{
		m_NKCUILabUnitEnhance.Init(npcLabProfessorOlivia, onGetUnitList);
		m_NKCUILabLimitBreak.Init(onTryLimitBreak);
		m_NKCUILabSkillTrain.Init(onTrySkillTrain);
		InitButton();
		m_NKCUILabCharacterInfo.Init(OnClickCharacterChange);
		m_NKCUIUnitSelect.Init(OnClickCharacterChange);
		m_rtLimitBreak = m_NKCUILabLimitBreak.GetComponent<RectTransform>();
		m_rtUnitEnhance = m_NKCUILabUnitEnhance.GetComponent<RectTransform>();
		m_rtSkillTrain = m_NKCUILabSkillTrain.GetComponent<RectTransform>();
		if (m_DragCharacterView != null)
		{
			m_DragCharacterView.Init(rotation: true);
			m_DragCharacterView.dOnGetObject += MakeMainBannerListSlot;
			m_DragCharacterView.dOnReturnObject += ReturnMainBannerListSlot;
			m_DragCharacterView.dOnProvideData += ProvideMainBannerListSlotData;
			m_DragCharacterView.dOnFocus += Focus;
		}
		if (m_evtPanel != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnEventPanelClick);
			m_evtPanel.triggers.Add(entry);
		}
		if (npcLabProfessorOlivia != null)
		{
			dOnSelectUnitPlayVoice = npcLabProfessorOlivia.PlayVoice;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitButton()
	{
		m_cbtnShortcutEnhance.PointerClick.RemoveAllListeners();
		m_cbtnShortcutEnhance.PointerClick.AddListener(delegate
		{
			SetState(LAB_DETAIL_STATE.LDS_UNIT_ENHANCE);
		});
		m_cbtnShortcutLimitBreak.PointerClick.RemoveAllListeners();
		m_cbtnShortcutLimitBreak.PointerClick.AddListener(delegate
		{
			SetState(LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK);
		});
		m_cbtnShortcutTraining.PointerClick.RemoveAllListeners();
		m_cbtnShortcutTraining.PointerClick.AddListener(delegate
		{
			SetState(LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN);
		});
		m_objNKM_UI_LAB_UNIT_CHANGE = base.transform.Find("NKM_UI_LAB_UNIT_CHANGE").gameObject;
		m_NKM_UI_LAB_UNIT_CHANGE_btn = m_objNKM_UI_LAB_UNIT_CHANGE.GetComponent<NKCUIComButton>();
		m_NKM_UI_LAB_UNIT_CHANGE_btn.PointerClick.RemoveAllListeners();
		m_NKM_UI_LAB_UNIT_CHANGE_btn.PointerClick.AddListener(OnClickCharacterChange);
		NKCUtil.SetGameobjectActive(m_objNKM_UI_LAB_UNIT_CHANGE, bValue: false);
	}

	public void TouchIllust()
	{
		m_NKCASUISpineIllust.SetAnimation(NKCASUIUnitIllust.eAnimation.UNIT_TOUCH, loop: false);
	}

	public override void Hide()
	{
		base.Hide();
		NKCUtil.SetGameobjectActive(m_rtCharacterIllust.gameObject, bValue: true);
		m_NKCUIUnitSelect.Close();
	}

	public override void UnHide()
	{
		m_bHide = false;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_rtCharacterIllust.gameObject, bValue: true);
		if (m_CurrentUnitData == null)
		{
			OpenUnitSelect();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objNKM_UI_LAB_UNIT_CHANGE, bValue: true);
		}
		TutorialCheck();
	}

	public void Open(LAB_DETAIL_STATE _LAB_DETAIL_STATE, long lReserveUID = 0L)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_cbtnShortcutTraining.gameObject, NKCContentManager.IsContentsUnlocked(ContentsType.LAB_TRAINING));
		NKCUtil.SetGameobjectActive(m_cbtnShortcutLimitBreak.gameObject, NKCContentManager.IsContentsUnlocked(ContentsType.LAB_LIMITBREAK));
		CurrentUnitDataCheck();
		SetState(_LAB_DETAIL_STATE);
		UIOpened();
		TutorialCheck();
		if (lReserveUID != 0L)
		{
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(lReserveUID);
			if (unitFromUID != null)
			{
				List<NKMUnitData> list = new List<NKMUnitData>();
				list.Add(unitFromUID);
				OnUnitSortList(lReserveUID, list);
			}
			else
			{
				Debug.LogError("유닛 정보를 찾을 수 없습니다. lReserveUID : " + lReserveUID);
			}
			m_bForceReserveOpen = true;
			OnSelectUnit(new List<long> { lReserveUID });
		}
		else if (null != m_DragCharacterView)
		{
			m_DragCharacterView.SetArrow();
		}
		m_bForceReserveOpen = false;
	}

	private void CurrentUnitDataCheck()
	{
		bool flag = false;
		if (m_CurrentUnitData != null)
		{
			flag = NKCScenManager.CurrentUserData().m_ArmyData.IsHaveUnitFromUID(m_CurrentUnitData.m_UnitUID);
		}
		if (!flag)
		{
			m_CurrentUnitData = null;
			OpenUnitSelect();
			SwitchCharacterInfo(bActive: false);
		}
		else
		{
			SwitchCharacterInfo(bActive: true);
		}
	}

	private void SetState(LAB_DETAIL_STATE state)
	{
		m_targetState = state;
		MenuTransitionType currentTransition = ((!base.IsOpen) ? MenuTransitionType.MenuDirectOpen : ((m_LAB_DETAIL_STATE != LAB_DETAIL_STATE.LDS_INVALID) ? ((m_LAB_DETAIL_STATE == LAB_DETAIL_STATE.LDS_MENU) ? MenuTransitionType.MenuToDetail : ((state == LAB_DETAIL_STATE.LDS_MENU) ? MenuTransitionType.DetailToMenu : ((m_LAB_DETAIL_STATE != state) ? MenuTransitionType.DetailToDetail : MenuTransitionType.CharacterChange))) : MenuTransitionType.Start));
		m_cbtnShortcutEnhance.Select(state == LAB_DETAIL_STATE.LDS_UNIT_ENHANCE);
		m_cbtnShortcutLimitBreak.Select(state == LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK);
		m_cbtnShortcutTraining.Select(state == LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN);
		ProcessChangeState(currentTransition, m_LAB_DETAIL_STATE, state);
	}

	public void ProcessChangeState(MenuTransitionType currentTransition, LAB_DETAIL_STATE oldState, LAB_DETAIL_STATE newState)
	{
		m_LAB_DETAIL_STATE = newState;
		UpdateUpsideMenu();
		switch (currentTransition)
		{
		case MenuTransitionType.Start:
			NKCUtil.SetGameobjectActive(m_NKCUILabCharacterInfo, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNKM_UI_LAB_UNIT_CHANGE, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRootShortcutMenu, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUILabLimitBreak, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUILabUnitEnhance, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUILabSkillTrain, bValue: false);
			break;
		case MenuTransitionType.MenuToDetail:
		case MenuTransitionType.CharacterChange:
			if (m_CurrentUnitData != null)
			{
				SwitchCharacterInfo(bActive: true);
				if (oldState != newState)
				{
					SetLeftUIIn(ref m_rtCharacterInfo);
				}
			}
			NKCUtil.SetGameobjectActive(m_objRootShortcutMenu, bValue: true);
			ChangeFunctionByState(newState);
			ChangeEmptyPanel(newState);
			SetDetailData(newState);
			if (oldState != newState)
			{
				PlaySubUIAnimation(newState, UI_ANIM_TYPE.UAT_IN);
			}
			break;
		case MenuTransitionType.DetailToDetail:
			SetDetailData(newState);
			ChangeEmptyPanel(newState);
			if (oldState != newState)
			{
				PlaySubUIAnimation(oldState, UI_ANIM_TYPE.UAT_OUT);
				PlaySubUIAnimation(newState, UI_ANIM_TYPE.UAT_IN);
			}
			if (m_CurrentUnitData == null)
			{
				OpenUnitSelect();
			}
			break;
		case MenuTransitionType.DetailToMenu:
			if (oldState != newState)
			{
				PlaySubUIAnimation(oldState, UI_ANIM_TYPE.UAT_OUT);
			}
			break;
		case MenuTransitionType.MenuDirectOpen:
			if (m_CurrentUnitData != null)
			{
				SwitchCharacterInfo(bActive: true);
				SetLeftUIIn(ref m_rtCharacterInfo);
				SetLeftUIIn(ref m_rtShortcutMenu);
			}
			NKCUtil.SetGameobjectActive(m_objRootShortcutMenu, bValue: true);
			ChangeFunctionByState(newState);
			ChangeEmptyPanel(newState);
			SetDetailData(newState);
			if (oldState != newState)
			{
				PlaySubUIAnimation(newState, UI_ANIM_TYPE.UAT_IN);
			}
			m_NKCUILabCharacterInfo.SetData(m_CurrentUnitData);
			break;
		}
		if ((currentTransition == MenuTransitionType.MenuDirectOpen || currentTransition == MenuTransitionType.DetailToDetail) && (oldState != newState || oldState == LAB_DETAIL_STATE.LDS_INVALID))
		{
			MakeDummyList(newState);
		}
	}

	private void SwitchCharacterInfo(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_NKCUILabCharacterInfo, bActive);
		NKCUtil.SetGameobjectActive(m_objNKM_UI_LAB_UNIT_CHANGE, bActive);
	}

	private void ChangeFunctionByState(LAB_DETAIL_STATE newState)
	{
		NKCUtil.SetGameobjectActive(m_NKCUILabLimitBreak, newState == LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK);
		NKCUtil.SetGameobjectActive(m_NKCUILabUnitEnhance, newState == LAB_DETAIL_STATE.LDS_UNIT_ENHANCE);
		NKCUtil.SetGameobjectActive(m_NKCUILabSkillTrain, newState == LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN);
	}

	private void ChangeEmptyPanel(LAB_DETAIL_STATE newState)
	{
		NKCUtil.SetGameobjectActive(m_objNKM_UI_LAB_LIMITBREAK_EMPTY_ROOT, m_CurrentUnitData == null && newState == LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK);
		NKCUtil.SetGameobjectActive(m_objNKM_UI_LAB_TRAINING_EMPTY, m_CurrentUnitData == null && newState == LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN);
	}

	private void PlaySubUIAnimation(LAB_DETAIL_STATE state, UI_ANIM_TYPE ani)
	{
		GetUIRectTransform(state, out var rt);
		if (!(null == rt))
		{
			switch (ani)
			{
			case UI_ANIM_TYPE.UAT_OUT:
				SetSubUIOut(rt);
				break;
			case UI_ANIM_TYPE.UAT_IN:
				SetSubUIIn(ref rt);
				break;
			}
		}
	}

	private void GetUIRectTransform(LAB_DETAIL_STATE state, out RectTransform rt)
	{
		rt = null;
		switch (state)
		{
		case LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
			rt = m_rtUnitEnhance;
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
			rt = m_rtLimitBreak;
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN:
			rt = m_rtSkillTrain;
			break;
		}
	}

	private void SetSubUIIn(ref RectTransform rect)
	{
		rect.DOKill();
		rect.anchorMin = new Vector2(0.5f, 0f);
		rect.anchorMax = new Vector2(1.5f, 1f);
		NKCUtil.SetGameobjectActive(rect, bValue: true);
		rect.DOAnchorMin(Vector2.zero, 0.4f).SetEase(Ease.OutCubic);
		rect.DOAnchorMax(Vector2.one, 0.4f).SetEase(Ease.OutCubic);
	}

	private void SetSubUIOut(RectTransform rect)
	{
		rect.DOKill();
		rect.DOAnchorMin(new Vector2(0.5f, 0f), 0.4f).SetEase(Ease.OutCubic).OnComplete(delegate
		{
			NKCUtil.SetGameobjectActive(rect, bValue: false);
		});
		rect.DOAnchorMax(new Vector2(1.5f, 1f), 0.4f).SetEase(Ease.OutCubic);
	}

	private void SetLeftUIIn(ref RectTransform rect)
	{
		rect.anchorMin = Vector2.left;
		rect.anchorMax = Vector2.up;
		rect.DOAnchorPosX(-1000f, 0f);
		rect.DOKill();
		rect.DOAnchorMin(Vector2.zero, 0.4f).SetEase(Ease.OutCubic);
		rect.DOAnchorMax(Vector2.one, 0.4f).SetEase(Ease.OutCubic);
	}

	private void SetDetailData(LAB_DETAIL_STATE state)
	{
		switch (state)
		{
		case LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
			m_NKCUILabUnitEnhance.SetData(m_CurrentUnitData);
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
			m_NKCUILabLimitBreak.SetData(m_CurrentUnitData, NKCScenManager.GetScenManager().GetMyUserData());
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN:
			m_NKCUILabSkillTrain.SetData(NKCScenManager.GetScenManager().GetMyUserData(), m_CurrentUnitData);
			break;
		case LAB_DETAIL_STATE.LDS_MENU:
			break;
		}
	}

	private void OpenUnitSelect()
	{
		if (m_LAB_DETAIL_STATE == LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN)
		{
			m_NKCUILabSkillTrain.SwitchSkillBack(bActive: false);
		}
		m_NKCUIUnitSelect?.Open();
	}

	private NKCUIUnitSelectList.UnitSelectListOptions GetLabUnitSelectOption(LAB_DETAIL_STATE targetState, bool bIncludeCurrnetUnit = false)
	{
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
		options.bDescending = true;
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
		options.bShowRemoveSlot = false;
		options.bShowHideDeckedUnitMenu = false;
		options.bHideDeckedUnit = false;
		options.bCanSelectUnitInMission = true;
		options.eDeckType = NKM_DECK_TYPE.NDT_NORMAL;
		options.setExcludeUnitUID = new HashSet<long>();
		options.strUpsideMenuName = NKCUtilString.GetLabSelectUnitMenuName(targetState);
		options.strEmptyMessage = NKCUtilString.GetLabSelectUnitEmptyMsg(targetState);
		options.bPushBackUnselectable = false;
		options.m_SortOptions.bIgnoreCityState = true;
		options.m_SortOptions.bIgnoreWorldMapLeader = true;
		options.m_bUseFavorite = true;
		options.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		options.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		options.setShipFilterCategory = NKCUnitSortSystem.setDefaultShipFilterCategory;
		options.setShipSortCategory = NKCUnitSortSystem.setDefaultShipSortCategory;
		if (m_CurrentUnitData != null)
		{
			options.m_IncludeUnitUID = m_CurrentUnitData.m_UnitUID;
		}
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		switch (targetState)
		{
		case LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
		{
			List<NKMUnitData> list = new List<NKMUnitData>(armyData.m_dicMyUnit.Values);
			for (int i = 0; i < list.Count; i++)
			{
				if (options.m_IncludeUnitUID == list[i].m_UnitUID)
				{
					options.setExcludeUnitUID.Add(list[i].m_UnitUID);
				}
				else if (NKMEnhanceManager.CheckUnitFullEnhance(list[i]))
				{
					options.setExcludeUnitUID.Add(list[i].m_UnitUID);
				}
			}
			options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>
			{
				NKCUnitSortSystem.eSortOption.Level_High,
				NKCUnitSortSystem.eSortOption.Rarity_High,
				NKCUnitSortSystem.eSortOption.Unit_SummonCost_High,
				NKCUnitSortSystem.eSortOption.ID_First,
				NKCUnitSortSystem.eSortOption.UID_Last
			};
			options.m_SortOptions.AdditionalExcludeFilterFunc = CheckUnitCanEnhance;
			break;
		}
		case LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
		{
			dicUnitLimitBreak.Clear();
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				foreach (KeyValuePair<long, NKMUnitData> item in armyData.m_dicMyUnit)
				{
					int num = NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(item.Value, nKMUserData);
					if (item.Value.m_UnitUID == options.m_IncludeUnitUID && num < 0)
					{
						num = 0;
					}
					dicUnitLimitBreak[item.Key] = num;
				}
			}
			options.m_SortOptions.AdditionalExcludeFilterFunc = CheckUnitCanLimitBreak;
			options.setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>();
			options.setUnitSortCategory.Add(NKCUnitSortSystem.eSortCategory.Custom1);
			foreach (NKCUnitSortSystem.eSortCategory item2 in NKCUnitSortSystem.setDefaultUnitSortCategory)
			{
				options.setUnitSortCategory.Add(item2);
			}
			string key = NKCStringTable.GetString("SI_GUIDE_MANUAL_TEMPLET_ARTICLE_UNIT_LIMITBREAK");
			options.m_SortOptions.lstCustomSortFunc.Add(NKCUnitSortSystem.eSortCategory.Custom1, new KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>(key, SortByUnitCanLimitBreakNow));
			options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>();
			options.lstSortOption.Add(NKCUnitSortSystem.eSortOption.CustomDescend1);
			NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false).ForEach(delegate(NKCUnitSortSystem.eSortOption e)
			{
				options.lstSortOption.Add(e);
			});
			options.dOnSlotSetData = OnLimitBreakSlotSetData;
			break;
		}
		case LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN:
			foreach (KeyValuePair<long, NKMUnitData> item3 in armyData.m_dicMyUnit)
			{
				if (item3.Value.m_UnitUID == options.m_IncludeUnitUID)
				{
					options.setExcludeUnitUID.Add(item3.Key);
				}
				else if (!NKMUnitSkillManager.CheckHaveUpgradableSkill(item3.Value))
				{
					options.setExcludeUnitUID.Add(item3.Key);
				}
			}
			options.m_SortOptions.AdditionalExcludeFilterFunc = CheckUnitCanSkillTrain;
			break;
		}
		if (!bIncludeCurrnetUnit && m_CurrentUnitData != null && !options.setExcludeUnitUID.Contains(m_CurrentUnitData.m_UnitUID))
		{
			options.setExcludeUnitUID.Add(m_CurrentUnitData.m_UnitUID);
		}
		return options;
	}

	private void OpenUnitSelect(LAB_DETAIL_STATE targetState)
	{
		NKCUIUnitSelectList.Instance.Open(GetLabUnitSelectOption(targetState), OnSelectUnit, OnUnitSortList, null, OnUnitSortOption);
	}

	private bool CheckUnitCanEnhance(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase == null || unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return false;
		}
		return true;
	}

	private bool CheckUnitCanLimitBreak(NKMUnitData unitData)
	{
		return NKMUnitLimitBreakManager.CanThisUnitLimitBreak(unitData);
	}

	private int SortByUnitCanLimitBreakNow(NKMUnitData lhs, NKMUnitData rhs)
	{
		if (!dicUnitLimitBreak.TryGetValue(lhs.m_UnitUID, out var value))
		{
			value = NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(lhs, NKCScenManager.CurrentUserData());
			dicUnitLimitBreak[lhs.m_UnitUID] = value;
		}
		if (!dicUnitLimitBreak.TryGetValue(rhs.m_UnitUID, out var value2))
		{
			value2 = NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(rhs, NKCScenManager.CurrentUserData());
			dicUnitLimitBreak[rhs.m_UnitUID] = value2;
		}
		return value2.CompareTo(value);
	}

	private void OnLimitBreakSlotSetData(NKCUIUnitSelectListSlotBase cUnitSlot, NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex)
	{
		if (cNKMUnitData != null && dicUnitLimitBreak.TryGetValue(cNKMUnitData.m_UnitUID, out var value))
		{
			NKCUIUnitSelectListSlot nKCUIUnitSelectListSlot = cUnitSlot as NKCUIUnitSelectListSlot;
			if (nKCUIUnitSelectListSlot != null)
			{
				nKCUIUnitSelectListSlot.SetLimitPossibleMark(value >= 0, NKMUnitLimitBreakManager.IsMaxLimitBreak(cNKMUnitData, 0));
			}
		}
	}

	private bool CheckUnitCanSkillTrain(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase == null || unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return false;
		}
		return true;
	}

	private void OnSelectUnit(List<long> unitUID)
	{
		if (unitUID.Count != 1)
		{
			Debug.LogError("NKCUILab.OpenUnitEnhance, Fatal Error : UnitSelectList returned wrong list");
			OpenUnitSelect();
			return;
		}
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		NKMUnitData unitFromUID = armyData.GetUnitFromUID(unitUID[0]);
		if (unitFromUID == null)
		{
			Debug.Log("NKCUILab.OpenUnitEnhance, Fatal Error : wrong uid, newUnitData is null");
			return;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = CanSelectUnit(unitFromUID, armyData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
			return;
		}
		switch (m_targetState)
		{
		case LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
			if (NKMEnhanceManager.CheckUnitFullEnhance(unitFromUID) && !m_bForceReserveOpen)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ALREADY_ENHANCE_MAX);
				OpenUnitSelect();
				return;
			}
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
			if (NKMUnitLimitBreakManager.IsMaxLimitBreak(unitFromUID) && !m_bForceReserveOpen)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ALREADY_LIMITBREAK_MAX);
				OpenUnitSelect();
				return;
			}
			break;
		}
		NKCUIUnitSelectList.CheckInstanceAndClose();
		if (m_CurrentUnitData == null || m_CurrentUnitData.m_UnitUID != unitFromUID.m_UnitUID)
		{
			m_NKCUILabCharacterInfo.SetData(unitFromUID);
		}
		m_CurrentUnitData = unitFromUID;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
		if (dOnSelectUnitPlayVoice != null)
		{
			dOnSelectUnitPlayVoice(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, m_LAB_DETAIL_STATE);
		}
		SetState(m_targetState);
		m_NKCUIUnitSelect.Close();
	}

	private NKM_ERROR_CODE CanSelectUnit(NKMUnitData unitData, NKMArmyData armyData)
	{
		if (unitData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		return armyData.GetUnitDeckState(unitData) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKM_ERROR_CODE.NEC_OK, 
		};
	}

	public void OnUnitSortOption(NKCUnitSortSystem.UnitListOptions unitOption)
	{
		m_preUnitListOption = unitOption;
	}

	private void MakeDummyList(LAB_DETAIL_STATE targetState)
	{
		if (!(m_DragCharacterView != null))
		{
			return;
		}
		BannerCleanUp();
		if (m_CurrentUnitData == null)
		{
			return;
		}
		NKCUIUnitSelectList.UnitSelectListOptions labUnitSelectOption = GetLabUnitSelectOption(targetState);
		labUnitSelectOption.lstSortOption = m_preUnitListOption.lstSortOption;
		labUnitSelectOption.setFilterOption = m_preUnitListOption.setFilterOption;
		NKCUnitSortSystem unitDummySortSystem = NKCUIUnitSelectList.GetUnitDummySortSystem(labUnitSelectOption);
		if (unitDummySortSystem.SortedUnitList.Count > 0)
		{
			bool flag = false;
			foreach (NKMUnitData sortedUnit in unitDummySortSystem.SortedUnitList)
			{
				if (sortedUnit.m_UnitUID == m_CurrentUnitData.m_UnitUID)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				unitDummySortSystem.SortedUnitList.Add(m_CurrentUnitData);
			}
		}
		else
		{
			unitDummySortSystem.SortedUnitList.Add(m_CurrentUnitData);
		}
		OnUnitSortList(m_CurrentUnitData.m_UnitUID, unitDummySortSystem.SortedUnitList);
	}

	private void OnUnitSortList(long UID, List<NKMUnitData> unitUIDList)
	{
		if (unitUIDList.Count > 0)
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			if (armyData == null || CanSelectUnit(armyData.GetUnitFromUID(UID), armyData) != NKM_ERROR_CODE.NEC_OK)
			{
				return;
			}
			for (int i = 0; i < unitUIDList.Count; i++)
			{
				if (CanSelectUnit(unitUIDList[i], armyData) != NKM_ERROR_CODE.NEC_OK)
				{
					unitUIDList.RemoveAt(i);
					i--;
				}
			}
		}
		BannerCleanUp();
		m_UnitSortList = unitUIDList;
		m_DragCharacterView.TotalCount = m_UnitSortList.Count;
		if (m_UnitSortList.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < m_UnitSortList.Count; j++)
		{
			if (m_UnitSortList[j].m_UnitUID == UID)
			{
				m_DragCharacterView.SetIndex(j);
				break;
			}
		}
	}

	private void ClearCheckCount()
	{
		m_iCheckUnitCnt = 0;
		m_iMaxUnitCnt = m_UnitSortList.Count;
	}

	private void ChangeCharacter(bool bIsNext = false)
	{
		if (m_UnitSortList.Count > 1)
		{
			m_SelectUnitIndex = (bIsNext ? (m_SelectUnitIndex + 1) : (m_SelectUnitIndex - 1));
			if (m_SelectUnitIndex >= m_UnitSortList.Count)
			{
				m_SelectUnitIndex = 0;
			}
			if (m_SelectUnitIndex < 0)
			{
				m_SelectUnitIndex = m_UnitSortList.Count - 1;
			}
			bNextClick = bIsNext;
			OpenSelectUnit(m_SelectUnitIndex, bIsNext);
		}
	}

	private void OpenSelectUnit(int idx = 0, bool bNext = false)
	{
		if (m_iCheckUnitCnt >= m_iMaxUnitCnt)
		{
			return;
		}
		NKMUnitData nKMUnitData = m_UnitSortList[idx];
		bool flag = false;
		switch (m_targetState)
		{
		case LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
			if (NKMEnhanceManager.CheckUnitFullEnhance(nKMUnitData))
			{
				flag = true;
			}
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
			if (NKMUnitLimitBreakManager.IsMaxLimitBreak(nKMUnitData))
			{
				flag = true;
			}
			break;
		}
		if (flag)
		{
			m_iCheckUnitCnt++;
			ChangeCharacter(bNext);
			return;
		}
		if (m_CurrentUnitData == null || m_CurrentUnitData.m_UnitUID != nKMUnitData.m_UnitUID)
		{
			m_NKCUILabCharacterInfo.SetData(nKMUnitData);
		}
		m_CurrentUnitData = nKMUnitData;
		SetState(m_targetState);
		TutorialCheckUnit();
	}

	public List<long> onGetUnitList(ref int curIdx)
	{
		curIdx = m_SelectUnitIndex;
		List<long> list = new List<long>();
		for (int i = 0; i < m_UnitSortList.Count; i++)
		{
			list.Add(m_UnitSortList[i].m_UnitUID);
		}
		return list;
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		switch (m_LAB_DETAIL_STATE)
		{
		case LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
			if (itemData.ItemID == 1)
			{
				m_NKCUILabUnitEnhance.UpdateRequiredCredit();
			}
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
			m_NKCUILabLimitBreak.OnInventoryChange(itemData);
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN:
			m_NKCUILabSkillTrain.OnInventoryChange(itemData);
			break;
		}
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		if (eEventType == NKMUserData.eChangeNotifyType.Update && m_CurrentUnitData != null && m_CurrentUnitData.m_UnitUID == uid)
		{
			m_NKCUILabCharacterInfo.SetData(unitData);
			m_CurrentUnitData = unitData;
			int num = m_UnitSortList.FindIndex((NKMUnitData v) => v.m_UnitUID == uid);
			if (num >= 0)
			{
				m_UnitSortList[num] = unitData;
			}
			if (m_DragCharacterView != null)
			{
				RectTransform currentItem = m_DragCharacterView.GetCurrentItem();
				if (currentItem != null)
				{
					NKCUICharacterView componentInChildren = currentItem.gameObject.GetComponentInChildren<NKCUICharacterView>();
					if (componentInChildren != null)
					{
						componentInChildren.SetCharacterIllust(unitData, bAsync: false, unitData.m_SkinID == 0);
					}
				}
			}
		}
		switch (m_LAB_DETAIL_STATE)
		{
		case LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
			m_NKCUILabUnitEnhance.UnitUpdated(uid, unitData);
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
			m_NKCUILabLimitBreak.UnitUpdated(uid, unitData);
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN:
			m_NKCUILabSkillTrain.UnitUpdated(uid, unitData);
			break;
		}
	}

	public override void OnBackButton()
	{
		base.OnBackButton();
		m_NKCUIUnitSelect.Close();
	}

	public void OnRecv(NKMPacket_ENHANCE_UNIT_ACK sPacket)
	{
		if (m_LAB_DETAIL_STATE == LAB_DETAIL_STATE.LDS_UNIT_ENHANCE && sPacket.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			NKCSoundManager.PlaySound("FX_UI_UNIT_ENCHANT_START", 1f, 0f, 0f);
			m_animEffect.SetTrigger("ENHANCE");
			m_rtEffect.position = m_rtCharacterIllust.position;
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_GROWTH_STATUS, NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(sPacket.unitUID));
		}
	}

	public void OnRecv(NKMPacket_UNIT_SKILL_UPGRADE_ACK sPacket)
	{
		if (m_LAB_DETAIL_STATE == LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN && sPacket.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			m_NKCUILabSkillTrain.OnSkillLevelUp(sPacket.skillID);
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_GROWTH_SKILL, NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(sPacket.unitUID));
		}
	}

	public void OnClickCharacterChange()
	{
		bNextClick = true;
		m_NKCUIUnitSelect.Outro();
		OpenUnitSelect(m_LAB_DETAIL_STATE);
	}

	public override void CloseInternal()
	{
		m_LAB_DETAIL_STATE = LAB_DETAIL_STATE.LDS_INVALID;
		m_NKCUILabLimitBreak.Cleanup();
		m_NKCUILabSkillTrain.Cleanup();
		m_NKCUILabUnitEnhance.Cleanup();
		if (m_NKCASUISpineIllust != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllust);
			m_NKCASUISpineIllust = null;
		}
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		m_NKCUIUnitSelect.Close();
		BannerCleanUp();
	}

	public void ClearAllFeedUnitSlots()
	{
		m_NKCUILabUnitEnhance.ClearAllFeedUnitSlots();
	}

	private void ClearFeedUnitSlot(int index)
	{
		m_NKCUILabUnitEnhance.ClearFeedUnitSlot(index);
	}

	private void OnEventPanelClick(BaseEventData e)
	{
		if (!(m_DragCharacterView != null) || !m_DragCharacterView.GetDragOffset().IsNearlyZero())
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

	private void ReturnMainBannerListSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		Object.Destroy(go.gameObject);
	}

	private void ProvideMainBannerListSlotData(Transform tr, int idx)
	{
		NKMUnitData nKMUnitData = m_UnitSortList[idx];
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

	private void FocusColor(RectTransform rect, Color ApplyColor)
	{
		NKCUICharacterView componentInChildren = rect.gameObject.GetComponentInChildren<NKCUICharacterView>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(ApplyColor);
		}
	}

	private void Focus(RectTransform rect, bool bFocus)
	{
		NKCUtil.SetGameobjectActive(rect.gameObject, bFocus);
		if (bFocus)
		{
			NKCUICharacterView componentInChildren = rect.gameObject.GetComponentInChildren<NKCUICharacterView>();
			if (componentInChildren != null)
			{
				SetCharacterInfo(componentInChildren.GetCurrentUnitData());
			}
		}
	}

	private void BannerCleanUp()
	{
		NKCUICharacterView[] componentsInChildren = m_rtCharacterIllust.gameObject.GetComponentsInChildren<NKCUICharacterView>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].CloseImmediatelyIllust();
		}
	}

	private void SetCharacterInfo(NKMUnitData newUnitData)
	{
		if (newUnitData != null)
		{
			if (m_CurrentUnitData == null || m_CurrentUnitData.m_UnitUID != newUnitData.m_UnitUID)
			{
				m_NKCUILabCharacterInfo.SetData(newUnitData);
			}
			m_CurrentUnitData = newUnitData;
			SetState(m_targetState);
			TutorialCheckUnit();
		}
	}

	public void SelectCharacter(int idx)
	{
		if (m_UnitSortList.Count < idx || idx < 0)
		{
			Debug.LogWarning($"잘못된 인덱스 총 갯수 : {m_UnitSortList.Count}, 목표 인덱스 : {idx}");
			return;
		}
		NKMUnitData nKMUnitData = m_UnitSortList[idx];
		if (m_CurrentUnitData == null || m_CurrentUnitData.m_UnitUID != nKMUnitData.m_UnitUID)
		{
			m_NKCUILabCharacterInfo.SetData(nKMUnitData);
		}
		m_CurrentUnitData = nKMUnitData;
		SetState(m_targetState);
		TutorialCheckUnit();
	}

	public void TouchCharacter(RectTransform rt, PointerEventData eventData)
	{
		if (m_DragCharacterView.GetDragOffset().IsNearlyZero())
		{
			NKCUICharacterView componentInChildren = rt.GetComponentInChildren<NKCUICharacterView>();
			if (componentInChildren != null)
			{
				componentInChildren.OnPointerDown(eventData);
				componentInChildren.OnPointerUp(eventData);
			}
		}
	}

	private void TutorialCheck()
	{
		switch (m_LAB_DETAIL_STATE)
		{
		case LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
			NKCTutorialManager.TutorialRequired(TutorialPoint.LabEnhance);
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
			NKCTutorialManager.TutorialRequired(TutorialPoint.LabLimitBreak);
			break;
		}
	}

	public void TutorialCheckUnit()
	{
		switch (m_LAB_DETAIL_STATE)
		{
		case LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
			if (m_CurrentUnitData != null)
			{
				NKMUnitLimitBreakManager.UnitLimitBreakStatusData unitLimitbreakStatus = NKMUnitLimitBreakManager.GetUnitLimitbreakStatus(m_CurrentUnitData);
				Debug.LogWarning($"TutorialCheckUnit - T{unitLimitbreakStatus.Tier} {unitLimitbreakStatus.Status}");
				if (NKMUnitLimitBreakManager.IsMaxLimitBreak(unitLimitbreakStatus, 0) && unitLimitbreakStatus.Status != NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max)
				{
					NKCTutorialManager.TutorialRequired(TutorialPoint.LabLimitBreakUnit);
				}
			}
			break;
		case LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
		case LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN:
			break;
		}
	}

	public RectTransform GetEnhanceItemSlotRect(int index)
	{
		if (index < m_NKCUILabUnitEnhance.m_lstObjUnitSlot.Count)
		{
			return m_NKCUILabUnitEnhance.m_lstObjUnitSlot[index].m_Slot?.GetComponent<RectTransform>();
		}
		return null;
	}

	public RectTransform GetSkillLevelSlotRect(int index)
	{
		if (index < m_NKCUILabSkillTrain.m_lstSkillSlot.Count)
		{
			return m_NKCUILabSkillTrain.m_lstSkillSlot[index].m_slot.m_cbtnSlot?.GetComponent<RectTransform>();
		}
		return null;
	}
}
