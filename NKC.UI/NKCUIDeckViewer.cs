using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Community;
using ClientPacket.Raid;
using ClientPacket.User;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Protocol;
using NKC.PacketHandler;
using NKC.UI.Component;
using NKC.UI.Guide;
using NKC.UI.Guild;
using NKC.UI.Trim;
using NKM;
using NKM.Guild;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIDeckViewer : NKCUIBase
{
	public enum DeckViewerMode
	{
		DeckSetupOnly,
		PrepareBattle,
		PvPBattleFindTarget,
		AsyncPvPBattleStart,
		AsyncPvpDefenseDeck,
		PrepareDungeonBattle,
		WorldMapMissionDeckSelect,
		DeckSelect,
		WarfareBatch,
		WarfareBatch_Assault,
		WarfareRecovery,
		MainDeckSelect,
		PrepareDungeonBattle_Daily,
		PrepareDungeonBattleWithoutCost,
		PrepareDungeonBattle_CC,
		DeckMultiSelect,
		PrepareRaid,
		GuildCoopBoss,
		PrivatePvPReady,
		LeaguePvPMain,
		LeaguePvPGlobalBan,
		PrepareLocalDeck,
		TournamentApply,
		UnlimitedDeck
	}

	public struct DeckViewerOption
	{
		public delegate void OnBackButton();

		public delegate void OnDeckSideButtonConfirm(NKMDeckIndex selectedDeckIndex, long supportUserUID = 0L);

		public delegate void OnDeckSideButtonConfirmForMulti(List<NKMDeckIndex> lstSelectedDeckIndex);

		public delegate void OnDeckSideButtonConfirmForAsync(NKMDeckIndex selectedDeckIndex, NKMDeckData originalDeck);

		public delegate void OnDeckSideButtonConfirmForLeague(int selectedIndex);

		public delegate NKM_ERROR_CODE CheckDeckButtonConfirm(NKMDeckIndex selectedDeckIndex);

		public delegate void OnSelectSupporter(long friendCode);

		public delegate bool IsValidSupport(long friendCode);

		public delegate void OnChangeDeckUnit(NKMDeckIndex selectedDeckIndex, long newlyAddedUnitUID);

		public delegate void OnChangeDeckIndex(NKMDeckIndex selectedDeckIndex);

		public string MenuName;

		public DeckViewerMode eDeckviewerMode;

		public OnDeckSideButtonConfirm dOnSideMenuButtonConfirm;

		public OnDeckSideButtonConfirmForMulti dOnDeckSideButtonConfirmForMulti;

		public OnDeckSideButtonConfirmForAsync dOnDeckSideButtonConfirmForAsync;

		public int maxMultiSelectCount;

		public CheckDeckButtonConfirm dCheckSideMenuButton;

		public OnChangeDeckUnit dOnChangeDeckUnit;

		public OnChangeDeckIndex dOnChangeDeckIndex;

		public NKMDeckIndex DeckIndex;

		public List<NKMDeckIndex> lstMultiSelectedDeckIndex;

		public OnBackButton dOnBackButton;

		public bool SelectLeaderUnitOnOpen;

		public bool bEnableDefaultBackground;

		public bool bUpsideMenuHomeButton;

		public int WorldMapMissionID;

		public int WorldMapMissionCityID;

		public bool bOpenAlphaAni;

		public List<int> upsideMenuShowResourceList;

		public long raidUID;

		public int CostItemID;

		public int CostItemCount;

		public bool bUsableOperationSkip;

		public List<int> ShowDeckIndexList;

		public string DeckListButtonStateText;

		public string StageBattleStrID;

		public bool bUsableSupporter;

		public List<WarfareSupporterListData> lstSupporter;

		public OnSelectSupporter dOnSelectSupporter;

		public IsValidSupport dIsValidSupport;

		public bool bUseAsyncDeckSetting;

		public bool bSlot24Extend;

		public bool bNoUseLeaderBtn;

		public Action dOnHide;

		public Action dOnUnhide;
	}

	[Serializable]
	public class DeckTypeIconObject
	{
		public NKM_DECK_TYPE eDeckType;

		public GameObject objSquadIcon;
	}

	private struct AssistUserData
	{
		public NKMDeckIndex deckIndex;

		public long assistUserUID;

		public NKMAsyncUnitEquipData unitEquipData;

		public AssistUserData(NKMDeckIndex deck, NKMAsyncUnitEquipData equipData, long userUID)
		{
			deckIndex = deck;
			unitEquipData = equipData;
			assistUserUID = userUID;
		}
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_deck_view";

	private const string UI_ASSET_NAME = "NKM_UI_DECK_VIEW";

	private static NKCUIDeckViewer m_Instance;

	private DeckViewerOption m_ViewerOptions;

	private bool m_bUnitViewEnable = true;

	private NKMDeckIndex m_SelectDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, 0);

	private int m_SelectUnitSlotIndex = -1;

	private List<long> m_DeckUnitList = new List<long>();

	private bool m_bDeckInMission;

	private bool m_bDeckInWarfareBatch;

	private bool m_bDeckInDiveBatch;

	private bool m_bUnitListActivated;

	private NKMDeckData m_AsyncOriginalDeckData = new NKMDeckData();

	private NKCUIComSafeArea m_NKM_DECK_VIEW_ARMY_NKCUIComSafeArea;

	private NKM_UNIT_TYPE m_eCurrentSelectListType;

	public GameObject m_NKM_DECK_VIEW_BG;

	public GameObject m_objDeckViewSquadTitle;

	public InputField m_ifDeckName;

	public Text m_lbDeckNamePlaceholder;

	public NKCUIComStateButton m_csbtnDeckName;

	public List<DeckTypeIconObject> m_lstDecktypeIcon;

	public Text m_lbSquadNumber;

	public Text m_lbSupporterName;

	public NKCUIComStateButton m_csbtn_NKM_DECK_VIEW_HELP;

	public GameObject m_objDeckViewArmy;

	public GameObject m_objDeckViewSideRaidParent;

	public NKCDeckViewList m_NKCDeckViewList;

	public NKCDeckViewShip m_NKCDeckViewShip;

	public Vector3 m_vShipNormalAnchoredPos = new Vector3(1200f, -487f);

	public Vector3 m_vShipRaidAnchoredPos = new Vector3(1215f, -368f);

	public NKCDeckViewUnit m_NKCDeckViewUnit;

	public NKCUIDeckViewOperator m_NKCDeckViewOperator;

	public NKCDeckViewSide m_NKCDeckViewSide;

	public NKCDeckViewUnitSelectList m_NKCDeckViewUnitSelectList;

	private NKCDeckViewUnit m_NKCDeckViewUnit_24;

	private NKCUIRaidRightSide m_NKCUIRaidRightSide;

	private NKCUIGuildCoopRaidRightSide m_NKCUIGuildCoopRaidRightSide;

	public NKCDeckViewSupportList m_NKCDeckViewSupportList;

	public GameObject m_objNKCDeckViewTitle;

	[Header("지원 유닛")]
	public GameObject m_objUnitAssist;

	public NKCUIComStateButton m_csbtnUnitAssist;

	public GameObject m_objUnitAssistLock;

	public GameObject m_objUnitAssistEmpty;

	public NKCDeckViewUnitSelectListSlot m_UnitAssistSlot;

	[Header("스테이지 정보")]
	public GameObject m_NKM_DECK_VIEW_OPERATION_TITLE;

	public Text m_lbOperationEpisode;

	public Text m_lbOperationTitle;

	public NKCUIComStateButton m_csbtnEnemyList;

	[Header("소대작전능력")]
	public RectTransform m_rtDeckOperationPowerRoot;

	public Vector3 m_vDeckPowerNormalAnchoredPos = new Vector3(336.9f, 78f);

	public Vector3 m_vDeckPowerRaidAnchoredPos = new Vector3(336.9f, 78f);

	public Text m_lbDeckOperationPower;

	public GameObject m_objBanNotice;

	public GameObject m_objDescOrder;

	public Text m_lbAdditionalInfoTitle;

	public Text m_lbAdditionalInfoNumber;

	public CanvasGroup m_CanvasGroup;

	public GameObject m_objSlotUnlockEffect;

	[Header("덱 타입 안내")]
	public GameObject m_objDeckType;

	public Text m_lbDeckType;

	public NKCUIComStateButton m_csbtnDeckTypeGuide;

	[Header("덱 타입 트리밍")]
	public GameObject m_objDeckTypeTrim;

	public Text m_lbTrimLevel;

	public Text m_lbTrimName;

	public Text m_lbRecommendedPower;

	[Header("입장 제한")]
	public GameObject m_NKM_DECK_VIEW_SIDE_UNIT_OPERATION_EnterLimit;

	public Text m_EnterLimit_TEXT;

	public GameObject m_OPERATION_TITLE_BONUS;

	public Image m_BONUS_ICON;

	[Header("오퍼레이터")]
	public RectTransform m_NKM_DECK_VIEW_OPERATOR;

	public Vector2 m_vOperatorNormalAnchoredPos;

	public Vector2 m_vOperatorRaidAnchoredPos;

	public NKCUIOperatorDeckSlot m_NKM_UI_OPERATOR_DECK_SLOT;

	public GameObject m_EMPTY;

	public NKCUIComButton m_opereaterEmpty;

	public GameObject m_OperatorSkillInfo;

	public NKCUIOperatorSkill m_OperatorMainSkill;

	public NKCUIOperatorSkill m_OperatorSubSkill;

	public NKCUIOperatorTacticalSkillCombo m_OperatorSkillCombo;

	[Header("토너먼트 덱 변경 표시")]
	public GameObject m_objShipChanged;

	public GameObject m_objOperatorChanged;

	public GameObject m_objDeckOperationPowerChanged;

	public GameObject m_objDeckCostChanged;

	[Header("전투 환경")]
	public NKCUIComBattleEnvironmentList m_comBattleEnvironment;

	[Header("덱 복사")]
	public NKCUIComStateButton m_csbtnDeckCopy;

	public NKCUIComStateButton m_csbtnDeckPaste;

	private bool m_bReqSupportUnitList;

	private List<AssistUserData> m_lstAssistUnitDatas = new List<AssistUserData>();

	public static NKCUIDeckViewer Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIDeckViewer>("ab_ui_nkm_ui_deck_view", "NKM_UI_DECK_VIEW", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIDeckViewer>();
				m_Instance.InitUI();
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

	public override string GuideTempletID
	{
		get
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_TEAM)
			{
				return "ARTICLE_SYSTEM_TEAM_SETTING";
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
			{
				return "ARTICLE_WORLDMAP_MISSION";
			}
			return "";
		}
	}

	public override string MenuName => m_ViewerOptions.MenuName;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.ON_PLAY_GAME;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode
	{
		get
		{
			if (m_NKCDeckViewUnitSelectList.IsOpen)
			{
				return NKCUIUpsideMenu.eMode.LeftsideOnly;
			}
			if (m_NKCDeckViewSupportList.IsOpen)
			{
				return NKCUIUpsideMenu.eMode.LeftsideOnly;
			}
			if (m_ViewerOptions.bUpsideMenuHomeButton)
			{
				return NKCUIUpsideMenu.eMode.Normal;
			}
			return base.eUpsideMenuMode;
		}
	}

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (m_ViewerOptions.upsideMenuShowResourceList != null)
			{
				return m_ViewerOptions.upsideMenuShowResourceList;
			}
			return base.UpsideMenuShowResourceList;
		}
	}

	private NKMArmyData NKMArmyData => NKCScenManager.CurrentUserData()?.m_ArmyData;

	private bool IsSupportMenu => m_NKCDeckViewSupportList.IsOpen;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public static bool CheckInstance()
	{
		return m_Instance != null;
	}

	public bool IsPVPMode(DeckViewerMode eDeckViewerMode)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_NEW_MODE))
		{
			if (!IsPVPSyncMode(eDeckViewerMode))
			{
				return IsAsyncPvP();
			}
			return true;
		}
		return IsPVPSyncMode(eDeckViewerMode);
	}

	public static bool IsPVPSyncMode(DeckViewerMode eDeckViewerMode)
	{
		if (eDeckViewerMode == DeckViewerMode.PvPBattleFindTarget || eDeckViewerMode == DeckViewerMode.PrivatePvPReady || eDeckViewerMode == DeckViewerMode.UnlimitedDeck)
		{
			return true;
		}
		return false;
	}

	public static bool IsDungeonAtkReadyScen(DeckViewerMode eDeckViewerMode)
	{
		if (eDeckViewerMode == DeckViewerMode.PrepareDungeonBattle || (uint)(eDeckViewerMode - 12) <= 2u)
		{
			return true;
		}
		return false;
	}

	public DeckViewerMode GetDeckViewerMode()
	{
		return m_ViewerOptions.eDeckviewerMode;
	}

	public bool GetUnitViewEnable()
	{
		return m_bUnitViewEnable;
	}

	public NKMDeckIndex GetSelectDeckIndex()
	{
		return m_SelectDeckIndex;
	}

	public int GetSelectUnitSlotIndex()
	{
		return m_SelectUnitSlotIndex;
	}

	public NKCDeckViewUnit GetCurrDeckViewUnit()
	{
		if (IsUnitSlotExtention())
		{
			return GetDeckViewUnit24();
		}
		return m_NKCDeckViewUnit;
	}

	public void InitUI()
	{
		m_NKCDeckViewList.Init(SelectDeck, DeckUnlockRequestPopup, OnChangedMultiSelectedCount, OpenSupList);
		m_NKCDeckViewShip.Init(DeckViewShipClick);
		m_NKCDeckViewUnit.Init(OnUnitClicked, OnUnitDragEnd);
		m_NKCDeckViewOperator.Init();
		m_NKCDeckViewSide.Init(DeckViewUnitInfoClick, OpenDeckSelectList, OnLeaderChange, OnSideMenuButtonConfirm, OnClickCloseBtnOfDeckViewSide, CheckOperationMultiply);
		m_NKCDeckViewUnitSelectList.Init(this, OnDeckUnitChangeClicked, OnUnitSelectListClose, ClearDeck, AutoCompleteDeck);
		m_NKCDeckViewUnitSelectList.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup));
		m_NKCDeckViewSupportList.Init(this, UpdateSupporterUI, OnConfirmSuppoter);
		m_NKCDeckViewSupportList.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup));
		m_NKM_DECK_VIEW_ARMY_NKCUIComSafeArea = base.transform.Find("NKM_DECK_VIEW_ARMY").GetComponent<NKCUIComSafeArea>();
		if (null != m_opereaterEmpty)
		{
			m_opereaterEmpty.PointerClick.RemoveAllListeners();
			m_opereaterEmpty.PointerClick.AddListener(OnClickOperatorEmptySlot);
		}
		if (null != m_NKM_UI_OPERATOR_DECK_SLOT)
		{
			m_NKM_UI_OPERATOR_DECK_SLOT.Init(OnSelectOperator);
		}
		NKCUtil.SetGameobjectActive(m_NKM_DECK_VIEW_OPERATOR.gameObject, !NKCOperatorUtil.IsHide());
		NKCUtil.SetButtonClickDelegate(m_csbtnEnemyList, OnBtnEnemyList);
		NKCUtil.SetButtonClickDelegate(m_csbtnDeckTypeGuide, OnBtnDeckTypeGuide);
		NKCUtil.SetButtonClickDelegate(m_csbtnDeckCopy, OnClickDeckCopy);
		NKCUtil.SetButtonClickDelegate(m_csbtnDeckPaste, OnClickDeckPaste);
		NKCUtil.SetHotkey(m_csbtnDeckCopy, HotkeyEventType.Copy);
		NKCUtil.SetHotkey(m_csbtnDeckPaste, HotkeyEventType.Paste);
		if (m_ifDeckName != null)
		{
			m_ifDeckName.onValidateInput = NKCFilterManager.FilterEmojiInput;
			m_ifDeckName.onValueChanged.RemoveAllListeners();
			m_ifDeckName.onValueChanged.AddListener(OnDeckNameValueChanged);
			m_ifDeckName.onEndEdit.RemoveAllListeners();
			m_ifDeckName.onEndEdit.AddListener(OnEndEditDeckName);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnDeckName, OnChangeDeckName);
		base.gameObject.SetActive(value: false);
	}

	public void CloseResource()
	{
	}

	public override void OnScreenResolutionChanged()
	{
		base.OnScreenResolutionChanged();
		StartCoroutine(DelayedSelectCurrentDeck());
	}

	private IEnumerator DelayedSelectCurrentDeck()
	{
		yield return null;
		SelectCurrentDeck();
	}

	private void OnChangedMultiSelectedCount(int count)
	{
		m_NKCDeckViewSide.SetMultiSelectedCount(count, m_ViewerOptions.maxMultiSelectCount);
	}

	public void Load(NKMArmyData cNKMArmyData)
	{
	}

	private void LoadIcon(NKMUnitTempletBase cNKMUnitTempletBase)
	{
		NKCResourceUtility.PreloadUnitResource(NKCResourceUtility.eUnitResourceType.FACE_CARD, cNKMUnitTempletBase);
		if (cNKMUnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
		{
			NKCResourceUtility.PreloadUnitResource(NKCResourceUtility.eUnitResourceType.INVEN_ICON, cNKMUnitTempletBase);
		}
	}

	private void LoadSkillIcon(NKMUnitTempletBase cNKMUnitTempletBase)
	{
		if (cNKMUnitTempletBase != null)
		{
			for (int i = 0; i < cNKMUnitTempletBase.GetSkillCount(); i++)
			{
				NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(cNKMUnitTempletBase, i);
				if (shipSkillTempletByIndex != null)
				{
					NKCResourceUtility.LoadAssetResourceTemp<Sprite>("AB_UI_SHIP_SKILL_ICON", shipSkillTempletByIndex.m_ShipSkillIcon);
				}
				else
				{
					Debug.LogError($"ERROR - {cNKMUnitTempletBase.GetSkillStrID(i)}");
				}
			}
		}
		else
		{
			Debug.Log("NKCUIDeckViewer::LoadSkillIcon - cNKMUnitTempletBase is Null");
		}
	}

	private void LoadSpineIllust(NKMUnitTempletBase cNKMUnitTempletBase)
	{
		NKCResourceUtility.PreloadUnitResource(NKCResourceUtility.eUnitResourceType.SPINE_ILLUST, cNKMUnitTempletBase);
	}

	public void LoadComplete()
	{
	}

	public void Init()
	{
		m_SelectDeckIndex = m_ViewerOptions.DeckIndex;
		EnableUnitView(bEnable: true);
		m_SelectUnitSlotIndex = -1;
		if (null != m_csbtn_NKM_DECK_VIEW_HELP)
		{
			m_csbtn_NKM_DECK_VIEW_HELP.PointerClick.RemoveAllListeners();
			m_csbtn_NKM_DECK_VIEW_HELP.PointerClick.AddListener(OpenPopupUnitInfo);
		}
	}

	public void OpenPopupUnitInfo()
	{
		NKCPopupUnitRoleInfo.Instance.OpenDefaultPopup();
	}

	private bool IsMultiSelect()
	{
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.DeckMultiSelect)
		{
			return true;
		}
		if (m_ViewerOptions.dOnDeckSideButtonConfirmForMulti != null)
		{
			return true;
		}
		return false;
	}

	private NKCDeckViewUnit GetDeckViewUnit24()
	{
		if (m_NKCDeckViewUnit_24 == null)
		{
			m_NKCDeckViewUnit_24 = NKCDeckViewUnit.OpenInstance("AB_UI_NKM_UI_DECK_VIEW", "NKM_UI_DECK_VIEW_UNIT_24", m_objDeckViewArmy.transform, OnUnitClicked, OnUnitDragEnd);
		}
		return m_NKCDeckViewUnit_24;
	}

	private void SetDeckViewUnitUI()
	{
		if (IsUnitSlotExtention())
		{
			NKCUtil.SetGameobjectActive(GetDeckViewUnit24(), bValue: true);
			GetDeckViewUnit24()?.Open(NKMArmyData, m_SelectDeckIndex, m_ViewerOptions);
			m_NKCDeckViewUnit.Close();
		}
		else
		{
			NKCUtil.SetGameobjectActive(GetDeckViewUnit24(), bValue: false);
			m_NKCDeckViewUnit.Open(NKMArmyData, m_SelectDeckIndex, m_ViewerOptions);
		}
		GetCurrDeckViewUnit()?.Enable();
		m_NKCDeckViewOperator.Enable();
	}

	private void SetRightSideView(bool bInit = true)
	{
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareRaid)
		{
			NKCUtil.SetGameobjectActive(m_NKCUIGuildCoopRaidRightSide, bValue: false);
			if (m_NKCUIRaidRightSide == null)
			{
				m_NKCUIRaidRightSide = NKCUIRaidRightSide.OpenInstance(m_objDeckViewSideRaidParent.transform, OnClickRaidAttck);
			}
			NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_ViewerOptions.raidUID);
			if (nKMRaidDetailData == null)
			{
				Debug.LogError("raidData is null");
				return;
			}
			NKCUIRaidRightSide.NKC_RAID_SUB_BUTTON_TYPE eNKC_RAID_SUB_BUTTON_TYPE = NKCUIRaidRightSide.NKC_RAID_SUB_BUTTON_TYPE.NRSBT_ATTACK;
			if (nKMRaidDetailData.curHP == 0f || NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate))
			{
				eNKC_RAID_SUB_BUTTON_TYPE = NKCUIRaidRightSide.NKC_RAID_SUB_BUTTON_TYPE.NRSBT_EXIT;
			}
			NKCUtil.SetGameobjectActive(m_NKCUIRaidRightSide, bValue: true);
			m_NKCUIRaidRightSide?.SetUI(m_ViewerOptions.raidUID, NKCUIRaidRightSide.NKC_RAID_SUB_MENU_TYPE.NRSMT_SUPPORT_EQUIP, eNKC_RAID_SUB_BUTTON_TYPE);
			m_NKCDeckViewSide.Open(m_ViewerOptions, bInit, CheckUseCost());
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetShipOnlyMode(value: false);
		}
		else if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.GuildCoopBoss)
		{
			NKCUtil.SetGameobjectActive(m_NKCUIRaidRightSide, bValue: false);
			if (m_NKCUIGuildCoopRaidRightSide == null)
			{
				m_NKCUIGuildCoopRaidRightSide = NKCUIGuildCoopRaidRightSide.OpenInstance(m_objDeckViewSideRaidParent.transform, OnClickRaidAttck);
			}
			NKCUtil.SetGameobjectActive(m_NKCUIGuildCoopRaidRightSide, bValue: true);
			GuildRaidTemplet cGuildRaidTemplet = NKCGuildCoopManager.m_cGuildRaidTemplet;
			m_NKCUIGuildCoopRaidRightSide?.SetUI(cGuildRaidTemplet.GetSeasonRaidGrouop(), cGuildRaidTemplet.GetStageId());
			m_NKCDeckViewSide.Open(m_ViewerOptions, bInit, CheckUseCost());
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetShipOnlyMode(value: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKCUIRaidRightSide, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUIGuildCoopRaidRightSide, bValue: false);
			m_NKCDeckViewSide.Open(m_ViewerOptions, bInit, CheckUseCost());
		}
		m_NKCDeckViewSide.SetUnitData(null);
		m_NKCDeckViewSide.GetDeckViewSideUnitIllust().ResetObj();
		UpdateEnterLimitCount();
	}

	private bool CheckUseCost()
	{
		DeckViewerMode eDeckviewerMode = m_ViewerOptions.eDeckviewerMode;
		if ((uint)(eDeckviewerMode - 8) <= 1u)
		{
			return m_ViewerOptions.CostItemID == 2;
		}
		return m_ViewerOptions.CostItemID > 0;
	}

	private bool IsUnitSlotExtention()
	{
		if (m_ViewerOptions.bSlot24Extend)
		{
			return true;
		}
		return false;
	}

	public void Open(DeckViewerOption options, bool bInit = true)
	{
		m_ViewerOptions = options;
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
			m_NKM_DECK_VIEW_ARMY_NKCUIComSafeArea.SetSafeAreaBase();
		}
		if (bInit)
		{
			Init();
		}
		CloseDeckSelectList(bAnimate: false);
		if (m_ViewerOptions.bUseAsyncDeckSetting)
		{
			m_AsyncOriginalDeckData.DeepCopyFrom(NKMArmyData.GetDeckData(m_SelectDeckIndex));
		}
		SetDeckViewUnitUI();
		NKCUtil.SetGameobjectActive(m_objDeckViewSquadTitle, m_ViewerOptions.eDeckviewerMode != DeckViewerMode.PrepareLocalDeck && m_ViewerOptions.eDeckviewerMode != DeckViewerMode.TournamentApply);
		NKCUtil.SetGameobjectActive(m_objDescOrder, IsAsyncPvP());
		m_lstAssistUnitDatas.Clear();
		m_bReqSupportUnitList = false;
		NKCUtil.SetGameobjectActive(m_objUnitAssistLock, bValue: true);
		NKCUtil.SetGameobjectActive(m_UnitAssistSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUnitAssistEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnUnitAssist.gameObject, bValue: false);
		m_csbtnUnitAssist?.PointerClick.RemoveAllListeners();
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.USE_SUPPORT_UNIT) && !string.IsNullOrEmpty(m_ViewerOptions.StageBattleStrID))
		{
			NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(m_ViewerOptions.StageBattleStrID);
			if (nKMStageTempletV != null && nKMStageTempletV.m_bSupportUnit)
			{
				m_UnitAssistSlot.SetEmpty(bEnableLayoutElement: false, null);
				NKCUtil.SetGameobjectActive(m_objUnitAssistEmpty, bValue: true);
				NKCUtil.SetGameobjectActive(m_csbtnUnitAssist.gameObject, bValue: true);
				NKCUtil.SetBindFunction(m_csbtnUnitAssist, OnClickAssistUnit);
			}
		}
		m_NKCDeckViewList.Open(IsMultiSelect(), NKMArmyData, m_SelectDeckIndex.m_eDeckType, m_SelectDeckIndex.m_iIndex, m_ViewerOptions);
		UpdateDeckToggleUI();
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			long shipUId = NKCLocalDeckDataManager.GetShipUId(m_SelectDeckIndex.m_iIndex);
			NKMUnitData shipFromUID = NKMArmyData.GetShipFromUID(shipUId);
			m_NKCDeckViewShip.Open(shipFromUID, NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode));
		}
		else
		{
			m_NKCDeckViewShip.Open(NKMArmyData.GetDeckShip(m_SelectDeckIndex), NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode));
		}
		NKCUtil.SetGameobjectActive(m_objShipChanged, bValue: false);
		if (IsUnitSlotExtention())
		{
			m_NKCDeckViewShip.transform.localPosition = m_vShipRaidAnchoredPos;
			m_NKM_DECK_VIEW_OPERATOR.anchoredPosition = m_vOperatorRaidAnchoredPos;
		}
		else
		{
			m_NKCDeckViewShip.transform.localPosition = m_vShipNormalAnchoredPos;
			m_NKM_DECK_VIEW_OPERATOR.anchoredPosition = m_vOperatorNormalAnchoredPos;
		}
		UpdateOperator(NKMArmyData.GetDeckOperator(m_SelectDeckIndex));
		NKCUtil.SetGameobjectActive(m_csbtnDeckCopy, NKMOpenTagManager.IsOpened("COPY_SQUAD"));
		NKCUtil.SetGameobjectActive(m_csbtnDeckPaste, NKMOpenTagManager.IsOpened("COPY_SQUAD"));
		m_NKCDeckViewShip.Disable();
		SetRightSideView();
		if (IsMultiSelect())
		{
			OnChangedMultiSelectedCount(m_NKCDeckViewList.GetMultiSelectedCount());
		}
		if (m_rtDeckOperationPowerRoot != null)
		{
			if (!IsUnitSlotExtention())
			{
				m_rtDeckOperationPowerRoot.anchoredPosition = m_vDeckPowerNormalAnchoredPos;
			}
			else
			{
				m_rtDeckOperationPowerRoot.anchoredPosition = m_vDeckPowerRaidAnchoredPos;
			}
		}
		InitBattleEnvironment();
		if (!bInit && m_bUnitViewEnable)
		{
			SelectDeckViewUnit(m_SelectUnitSlotIndex, bForce: true);
		}
		SelectDeck(m_ViewerOptions.DeckIndex);
		if (m_ViewerOptions.SelectLeaderUnitOnOpen)
		{
			if (m_SelectUnitSlotIndex == -1 || bInit)
			{
				NKMDeckData deckData = NKMArmyData.GetDeckData(m_SelectDeckIndex);
				if (deckData != null)
				{
					SelectDeckViewUnit(deckData.m_LeaderIndex);
				}
				GetCurrDeckViewUnit()?.SelectDeckViewUnit(m_SelectUnitSlotIndex);
			}
			else
			{
				SelectDeckViewUnit(m_SelectUnitSlotIndex);
				GetCurrDeckViewUnit()?.SelectDeckViewUnit(m_SelectUnitSlotIndex);
			}
		}
		NKCUtil.SetGameobjectActive(m_NKM_DECK_VIEW_BG, m_ViewerOptions.bEnableDefaultBackground);
		if (m_ViewerOptions.bUpsideMenuHomeButton)
		{
			NKCUIFadeInOut.FadeIn(0.1f);
		}
		SetBotUI();
		UIOpened();
		if (m_ViewerOptions.bOpenAlphaAni)
		{
			m_CanvasGroup.alpha = 0f;
		}
		else
		{
			m_CanvasGroup.alpha = 1f;
		}
		CheckTutorial();
	}

	public void UpdateEnterLimitUI()
	{
		m_NKCDeckViewSide.Open(m_ViewerOptions, bInit: false, CheckUseCost());
		UpdateEnterLimitCount();
	}

	public void UpdateEnterLimitCount()
	{
		bool bValue = false;
		if (!string.IsNullOrEmpty(m_ViewerOptions.StageBattleStrID))
		{
			NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(m_ViewerOptions.StageBattleStrID);
			if (nKMStageTempletV != null && nKMStageTempletV.EnterLimit > 0)
			{
				bValue = true;
				int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(nKMStageTempletV.Key);
				string text = "";
				NKCUtil.SetLabelText(msg: nKMStageTempletV.EnterLimitCond switch
				{
					NKMStageTempletV2.RESET_TYPE.DAY => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02, nKMStageTempletV.EnterLimit - statePlayCnt, nKMStageTempletV.EnterLimit), 
					NKMStageTempletV2.RESET_TYPE.MONTH => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_MONTH_02, nKMStageTempletV.EnterLimit - statePlayCnt, nKMStageTempletV.EnterLimit), 
					NKMStageTempletV2.RESET_TYPE.WEEK => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_WEEK_02, nKMStageTempletV.EnterLimit - statePlayCnt, nKMStageTempletV.EnterLimit), 
					_ => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02, nKMStageTempletV.EnterLimit - statePlayCnt, nKMStageTempletV.EnterLimit), 
				}, label: m_EnterLimit_TEXT);
				if (nKMStageTempletV.EnterLimit - statePlayCnt <= 0)
				{
					NKCUtil.SetLabelTextColor(m_EnterLimit_TEXT, Color.red);
				}
				else
				{
					NKCUtil.SetLabelTextColor(m_EnterLimit_TEXT, Color.white);
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_NKM_DECK_VIEW_SIDE_UNIT_OPERATION_EnterLimit, bValue);
	}

	private void SetBotUI()
	{
		NKCUtil.SetGameobjectActive(m_objBanNotice, NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode));
		if (IsDungeonAtkReadyScen(m_ViewerOptions.eDeckviewerMode))
		{
			NKCUtil.SetGameobjectActive(m_NKM_DECK_VIEW_OPERATION_TITLE, bValue: true);
			NKC_SCEN_DUNGEON_ATK_READY sCEN_DUNGEON_ATK_READY = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY();
			NKMStageTempletV2 stageTemplet = sCEN_DUNGEON_ATK_READY.GetStageTemplet();
			NKMDungeonTempletBase dungeonTempletBase = sCEN_DUNGEON_ATK_READY.GetDungeonTempletBase();
			if (stageTemplet != null)
			{
				NKCUtil.SetGameobjectActive(m_csbtnEnemyList, bValue: true);
				NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(sCEN_DUNGEON_ATK_READY.GetEpisodeID(), sCEN_DUNGEON_ATK_READY.GetEpisodeDifficulty());
				if (nKMEpisodeTempletV != null)
				{
					m_lbOperationEpisode.text = nKMEpisodeTempletV.GetEpisodeTitle();
					if (nKMEpisodeTempletV.m_EPCategory == EPISODE_CATEGORY.EC_DAILY)
					{
						m_lbOperationTitle.text = stageTemplet.GetDungeonName() + " " + NKCUtilString.GetDailyDungeonLVDesc(sCEN_DUNGEON_ATK_READY.GetStageUIIndex());
					}
					else
					{
						m_lbOperationTitle.text = sCEN_DUNGEON_ATK_READY.GetActID() + "-" + sCEN_DUNGEON_ATK_READY.GetStageUIIndex() + " " + stageTemplet.GetDungeonName();
					}
				}
				if (stageTemplet.m_BuffType.Equals(RewardTuningType.None))
				{
					NKCUtil.SetGameobjectActive(m_OPERATION_TITLE_BONUS, bValue: false);
					return;
				}
				NKCUtil.SetGameobjectActive(m_OPERATION_TITLE_BONUS, bValue: true);
				NKCUtil.SetImageSprite(m_BONUS_ICON, NKCUtil.GetBounsTypeIcon(stageTemplet.m_BuffType, big: false));
			}
			else
			{
				if (dungeonTempletBase == null)
				{
					return;
				}
				NKCUtil.SetGameobjectActive(m_csbtnEnemyList, bValue: true);
				NKMEpisodeTempletV2 nKMEpisodeTempletV2 = NKMEpisodeTempletV2.Find(sCEN_DUNGEON_ATK_READY.GetEpisodeID(), sCEN_DUNGEON_ATK_READY.GetEpisodeDifficulty());
				if (nKMEpisodeTempletV2 != null)
				{
					m_lbOperationEpisode.text = nKMEpisodeTempletV2.GetEpisodeTitle();
					if (nKMEpisodeTempletV2.m_EPCategory == EPISODE_CATEGORY.EC_DAILY)
					{
						m_lbOperationTitle.text = dungeonTempletBase.GetDungeonName() + " " + NKCUtilString.GetDailyDungeonLVDesc(sCEN_DUNGEON_ATK_READY.GetStageUIIndex());
					}
					else
					{
						m_lbOperationTitle.text = sCEN_DUNGEON_ATK_READY.GetActID() + "-" + sCEN_DUNGEON_ATK_READY.GetStageUIIndex() + " " + dungeonTempletBase.GetDungeonName();
					}
				}
				if (dungeonTempletBase.StageTemplet != null)
				{
					if (dungeonTempletBase.StageTemplet.m_BuffType.Equals(RewardTuningType.None))
					{
						NKCUtil.SetGameobjectActive(m_OPERATION_TITLE_BONUS, bValue: false);
						return;
					}
					NKCUtil.SetGameobjectActive(m_OPERATION_TITLE_BONUS, bValue: true);
					NKCUtil.SetImageSprite(m_BONUS_ICON, NKCUtil.GetBounsTypeIcon(dungeonTempletBase.StageTemplet.m_BuffType, big: false));
				}
			}
			return;
		}
		DeckViewerMode eDeckviewerMode = m_ViewerOptions.eDeckviewerMode;
		if ((uint)(eDeckviewerMode - 8) <= 1u)
		{
			NKCUtil.SetGameobjectActive(m_NKM_DECK_VIEW_OPERATION_TITLE, bValue: true);
			NKC_SCEN_WARFARE_GAME nKC_SCEN_WARFARE_GAME = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME();
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(nKC_SCEN_WARFARE_GAME.GetWarfareStrID());
			NKCUtil.SetGameobjectActive(m_csbtnEnemyList, nKMWarfareTemplet != null);
			if (nKMWarfareTemplet == null)
			{
				return;
			}
			NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(nKC_SCEN_WARFARE_GAME.GetWarfareStrID());
			if (nKMStageTempletV != null)
			{
				NKMEpisodeTempletV2 episodeTemplet = nKMStageTempletV.EpisodeTemplet;
				if (episodeTemplet != null)
				{
					m_lbOperationEpisode.text = episodeTemplet.GetEpisodeTitle();
				}
				m_lbOperationTitle.text = nKMStageTempletV.ActId + "-" + nKMStageTempletV.m_StageUINum + " " + nKMWarfareTemplet.GetWarfareName();
			}
			if (nKMStageTempletV != null)
			{
				if (nKMStageTempletV.m_BuffType.Equals(RewardTuningType.None))
				{
					NKCUtil.SetGameobjectActive(m_OPERATION_TITLE_BONUS, bValue: false);
					return;
				}
				NKCUtil.SetGameobjectActive(m_OPERATION_TITLE_BONUS, bValue: true);
				NKCUtil.SetImageSprite(m_BONUS_ICON, NKCUtil.GetBounsTypeIcon(nKMStageTempletV.m_BuffType, big: false));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_DECK_VIEW_OPERATION_TITLE, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnEnemyList, bValue: false);
		}
	}

	private void OnDeckUpdate()
	{
		UpdateDeckOperationPower();
		UpdateBattleEnvironment();
	}

	private void UpdateDeckOperationPower()
	{
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			bool bPVP = IsPVPMode(m_ViewerOptions.eDeckviewerMode);
			bool bPossibleShowBan = NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode);
			bool bPossibleShowUp = NKCUtil.CheckPossibleShowUpUnit(m_ViewerOptions.eDeckviewerMode);
			int operationPower = NKCLocalDeckDataManager.GetOperationPower(m_SelectDeckIndex.m_iIndex, bPVP, bPossibleShowBan, bPossibleShowUp);
			m_lbDeckOperationPower.text = operationPower.ToString("N0");
			NKCUtil.SetLabelText(m_lbAdditionalInfoTitle, NKCUtilString.GET_STRING_DECK_AVG_SUMMON_COST);
			NKCUtil.SetLabelText(m_lbAdditionalInfoNumber, $"{CalculateLocalDeckAvgSummonCost(m_SelectDeckIndex.m_iIndex):0.00}");
		}
		else if (!IsSupportMenu)
		{
			bool bPVP2 = IsPVPMode(m_ViewerOptions.eDeckviewerMode);
			bool flag = NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode);
			bool flag2 = NKCUtil.CheckPossibleShowUpUnit(m_ViewerOptions.eDeckviewerMode);
			int armyAvarageOperationPower = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetArmyAvarageOperationPower(m_SelectDeckIndex, bPVP2, flag ? NKCBanManager.GetBanData() : null, flag2 ? NKCBanManager.m_dicNKMUpData : null);
			m_lbDeckOperationPower.text = armyAvarageOperationPower.ToString("N0");
			NKCUtil.SetLabelText(m_lbAdditionalInfoTitle, NKCUtilString.GET_STRING_DECK_AVG_SUMMON_COST);
			NKCUtil.SetLabelText(m_lbAdditionalInfoNumber, $"{CalculateDeckAvgSummonCost(m_SelectDeckIndex):0.00}");
			if (NeedShowChangedTag())
			{
				NKCUtil.SetGameobjectActive(m_objDeckOperationPowerChanged, NKCTournamentManager.m_TournamentApplyDeckData.operationPower != armyAvarageOperationPower);
				NKCUtil.SetGameobjectActive(m_objDeckCostChanged, NKCTournamentManager.CalculateDeckAvgSummonCost() != CalculateDeckAvgSummonCost(m_SelectDeckIndex));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objDeckOperationPowerChanged, bValue: false);
				NKCUtil.SetGameobjectActive(m_objDeckCostChanged, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objDeckOperationPowerChanged, bValue: false);
			NKCUtil.SetGameobjectActive(m_objDeckCostChanged, bValue: false);
			WarfareSupporterListData selectedData = m_NKCDeckViewSupportList.GetSelectedData();
			if (selectedData != null)
			{
				m_lbDeckOperationPower.text = selectedData.deckData.CalculateOperationPower().ToString("N0");
				NKCUtil.SetLabelText(m_lbAdditionalInfoTitle, NKCUtilString.GET_STRING_DECK_AVG_SUMMON_COST);
				NKCUtil.SetLabelText(m_lbAdditionalInfoNumber, $"{selectedData.deckData.CalculateSummonCost():0.00}");
			}
		}
	}

	private float CalculateDeckAvgSummonCost(NKMDeckIndex deckIndex)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (armyData == null)
		{
			return 0f;
		}
		bool flag = NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode);
		bool flag2 = NKCUtil.CheckPossibleShowUpUnit(m_ViewerOptions.eDeckviewerMode);
		return armyData.CalculateDeckAvgSummonCost(deckIndex, flag ? NKCBanManager.GetBanData() : null, flag2 ? NKCBanManager.m_dicNKMUpData : null);
	}

	public float CalculateLocalDeckAvgSummonCost(int deckIndex)
	{
		int num = 0;
		int num2 = 0;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return 0f;
		}
		bool num3 = NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode);
		bool flag = NKCUtil.CheckPossibleShowUpUnit(m_ViewerOptions.eDeckviewerMode);
		Dictionary<int, NKMBanData> dicNKMBanData = (num3 ? NKCBanManager.GetBanData() : null);
		Dictionary<int, NKMUnitUpData> dicNKMUpData = (flag ? NKCBanManager.m_dicNKMUpData : null);
		int localLeaderIndex = NKCLocalDeckDataManager.GetLocalLeaderIndex(deckIndex);
		List<long> localUnitDeckData = NKCLocalDeckDataManager.GetLocalUnitDeckData(deckIndex);
		int count = localUnitDeckData.Count;
		for (int i = 0; i < count; i++)
		{
			long unitUid = localUnitDeckData[i];
			NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(unitUid);
			if (unitFromUID != null)
			{
				num++;
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitFromUID.m_UnitID);
				if (unitStatTemplet == null)
				{
					Log.Error($"Cannot found UnitStatTemplet. UserUid:{nKMUserData.m_UserUID}, UnitId:{unitFromUID.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIDeckViewer.cs", 1190);
				}
				else
				{
					num2 += unitStatTemplet.GetRespawnCost(i == localLeaderIndex, dicNKMBanData, dicNKMUpData);
				}
			}
		}
		if (num == 0)
		{
			return 0f;
		}
		return (float)num2 / (float)num;
	}

	private void InitBattleEnvironment()
	{
		if (m_comBattleEnvironment == null)
		{
			return;
		}
		NKMDeckCondition deckCondition = null;
		List<NKMBattleConditionTemplet> lstBattleCondition = null;
		List<int> lstPreconditionBC = null;
		if (IsDungeonAtkReadyScen(m_ViewerOptions.eDeckviewerMode))
		{
			NKMDungeonTempletBase dungeonTempletBase = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetDungeonTempletBase();
			if (dungeonTempletBase != null)
			{
				lstBattleCondition = dungeonTempletBase.BattleConditions;
				deckCondition = dungeonTempletBase.m_DeckCondition;
				lstPreconditionBC = dungeonTempletBase.m_BCPreconditionGroups;
			}
		}
		else
		{
			switch (m_ViewerOptions.eDeckviewerMode)
			{
			case DeckViewerMode.PrepareRaid:
			{
				NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_ViewerOptions.raidUID);
				if (nKMRaidDetailData == null)
				{
					Debug.LogError("raidData is null");
					break;
				}
				NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
				if (nKMRaidTemplet?.DungeonTempletBase != null)
				{
					lstBattleCondition = nKMRaidTemplet.DungeonTempletBase.BattleConditions;
					deckCondition = nKMRaidTemplet.DungeonTempletBase.m_DeckCondition;
					lstPreconditionBC = nKMRaidTemplet.DungeonTempletBase.m_BCPreconditionGroups;
				}
				break;
			}
			case DeckViewerMode.GuildCoopBoss:
			{
				GuildRaidTemplet cGuildRaidTemplet = NKCGuildCoopManager.m_cGuildRaidTemplet;
				if (cGuildRaidTemplet == null)
				{
					Debug.LogError("guildRaidTempelt not found");
					break;
				}
				NKMDungeonTempletBase dungeonTempletBase2 = NKMDungeonManager.GetDungeonTempletBase(cGuildRaidTemplet.GetStageId());
				if (dungeonTempletBase2 != null)
				{
					lstBattleCondition = dungeonTempletBase2.BattleConditions;
					deckCondition = dungeonTempletBase2.m_DeckCondition;
					lstPreconditionBC = dungeonTempletBase2.m_BCPreconditionGroups;
				}
				break;
			}
			case DeckViewerMode.TournamentApply:
				if (NKCTournamentManager.m_TournamentTemplet != null)
				{
					deckCondition = NKCTournamentManager.m_TournamentTemplet.m_DeckCondition;
					lstBattleCondition = NKCTournamentManager.m_TournamentTemplet.BattleConditionTemplets;
					lstPreconditionBC = null;
				}
				break;
			}
		}
		m_comBattleEnvironment.InitData(deckCondition, lstBattleCondition, lstPreconditionBC);
		m_comBattleEnvironment.Close();
	}

	private void UpdateBattleEnvironment()
	{
		if (!(m_comBattleEnvironment == null))
		{
			if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
			{
				NKMEventDeckData localDeckData = NKCLocalDeckDataManager.GetLocalDeckData(m_SelectDeckIndex.m_iIndex);
				m_comBattleEnvironment.UpdateData(localDeckData, NKCLocalDeckDataManager.FreeEventDeckTemplet);
			}
			else
			{
				NKMDeckData deckData = NKMArmyData.GetDeckData(m_SelectDeckIndex);
				m_comBattleEnvironment.UpdateData(deckData);
			}
		}
	}

	public void SetUnitSlotData(NKMDeckIndex deckIndex, int unitSlotIndex, bool bEffect)
	{
		if (deckIndex == m_SelectDeckIndex)
		{
			GetCurrDeckViewUnit()?.SetUnitSlotData(NKMArmyData, m_SelectDeckIndex, unitSlotIndex, bEffect, m_ViewerOptions);
		}
	}

	public override void Hide()
	{
		GetCurrDeckViewUnit()?.CancelAllDrag();
		m_objSlotUnlockEffect.transform.SetParent(base.transform);
		NKCUtil.SetGameobjectActive(m_objSlotUnlockEffect, bValue: false);
		if (m_NKCDeckViewUnitSelectList.IsOpen)
		{
			m_bUnitListActivated = true;
			NKCUtil.SetGameobjectActive(m_NKCDeckViewUnitSelectList, bValue: false);
		}
		else
		{
			m_bUnitListActivated = false;
		}
		m_ViewerOptions.dOnHide?.Invoke();
		base.Hide();
	}

	public override void UnHide()
	{
		base.UnHide();
		if (m_bUnitListActivated)
		{
			NKCUtil.SetGameobjectActive(m_NKCDeckViewUnitSelectList, bValue: true);
		}
		m_bUnitListActivated = false;
		if (NKMArmyData.GetDeckUnitByIndex(m_SelectDeckIndex, m_SelectUnitSlotIndex) == null && !m_NKCDeckViewSide.GetDeckViewSideUnitIllust().hasUnitData() && m_ViewerOptions.eDeckviewerMode != DeckViewerMode.PrepareRaid && m_ViewerOptions.eDeckviewerMode != DeckViewerMode.GuildCoopBoss)
		{
			PlayLoadingAnim("BASE");
		}
		else
		{
			PlayLoadingAnim("CLOSE");
		}
		if (m_NKCDeckViewSide.GetDeckViewSideUnitIllust().hasUnitData())
		{
			if (m_NKCDeckViewSide.GetDeckViewSideUnitIllust().IsMatchedSideIllustToUnitType(NKM_UNIT_TYPE.NUT_SHIP))
			{
				EnableUnitView(bEnable: false, bForce: true);
			}
			else
			{
				EnableUnitView(bEnable: true, bForce: true);
			}
		}
		m_ViewerOptions.dOnUnhide?.Invoke();
		GetCurrDeckViewUnit()?.SlotResetPos(bImmediate: true);
	}

	public override void OnCloseInstance()
	{
		if (m_NKCDeckViewUnitSelectList != null)
		{
			UnityEngine.Object.Destroy(m_NKCDeckViewUnitSelectList.gameObject);
		}
		if (m_NKCDeckViewSupportList != null)
		{
			UnityEngine.Object.Destroy(m_NKCDeckViewSupportList.gameObject);
		}
		base.OnCloseInstance();
	}

	public override void CloseInternal()
	{
		if (m_ViewerOptions.bUseAsyncDeckSetting)
		{
			NKMArmyData.GetDeckData(m_SelectDeckIndex)?.DeepCopyFrom(m_AsyncOriginalDeckData);
		}
		m_NKCDeckViewList.Close();
		m_NKCDeckViewShip.Close();
		m_NKCDeckViewUnit.Close();
		m_NKCDeckViewOperator.Close();
		m_NKCDeckViewSide.Close();
		m_NKCDeckViewSupportList.Close();
		m_NKCDeckViewSupportList.Clear();
		NKCUtil.SetGameobjectActive(GetDeckViewUnit24(), bValue: false);
		m_objSlotUnlockEffect.transform.SetParent(base.transform);
		NKCUtil.SetGameobjectActive(m_objSlotUnlockEffect, bValue: false);
		m_NKCDeckViewUnitSelectList.Cleanup();
		m_NKCDeckViewUnitSelectList.Close(bAnimate: false);
		if (m_NKCUIRaidRightSide != null)
		{
			m_NKCUIRaidRightSide.CloseInstance();
			m_NKCUIRaidRightSide = null;
		}
		if (m_NKCUIGuildCoopRaidRightSide != null)
		{
			m_NKCUIGuildCoopRaidRightSide.CloseInstance();
			m_NKCUIGuildCoopRaidRightSide = null;
		}
		if (m_comBattleEnvironment != null)
		{
			m_comBattleEnvironment.Close();
			m_comBattleEnvironment.ClearData();
		}
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (m_NKCDeckViewUnit_24 != null)
		{
			m_NKCDeckViewUnit_24.CloseResource("AB_UI_NKM_UI_DECK_VIEW", "NKM_UI_DECK_VIEW_UNIT_24");
			m_NKCDeckViewUnit_24 = null;
		}
		if (m_NKCUIRaidRightSide != null)
		{
			m_NKCUIRaidRightSide.CloseInstance();
			m_NKCUIRaidRightSide = null;
		}
		if (m_NKCUIGuildCoopRaidRightSide != null)
		{
			m_NKCUIGuildCoopRaidRightSide.CloseInstance();
			m_NKCUIGuildCoopRaidRightSide = null;
		}
		m_Instance = null;
	}

	public override void OnBackButton()
	{
		if (m_NKCDeckViewUnitSelectList.IsOpen)
		{
			m_NKCDeckViewUnitSelectList.Close(bAnimate: true);
		}
		else if (m_ViewerOptions.dOnBackButton != null)
		{
			m_ViewerOptions.dOnBackButton();
		}
		else
		{
			base.OnBackButton();
		}
	}

	public void Update()
	{
		if (GetUnitViewEnable())
		{
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().Open(m_ViewerOptions.eDeckviewerMode, bInit: false);
		}
		if (base.IsOpen && m_CanvasGroup.alpha < 1f)
		{
			m_CanvasGroup.alpha += Time.deltaTime * 2.5f;
			if (m_CanvasGroup.alpha >= 1f)
			{
				m_CanvasGroup.alpha = 1f;
			}
		}
	}

	private bool IsAsyncPvP()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_NEW_MODE))
		{
			return m_ViewerOptions.eDeckviewerMode == DeckViewerMode.AsyncPvpDefenseDeck;
		}
		if (m_ViewerOptions.eDeckviewerMode != DeckViewerMode.AsyncPvPBattleStart)
		{
			return m_ViewerOptions.eDeckviewerMode == DeckViewerMode.AsyncPvpDefenseDeck;
		}
		return true;
	}

	public void DeckUnlockRequestPopup(NKMDeckIndex index)
	{
		NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_UNLOCK, NKCUtilString.GET_STRING_DECK_SLOT_UNLOCK, 101, 600, delegate
		{
			NKCPacketSender.Send_NKMPacket_DECK_UNLOCK_REQ(index.m_eDeckType);
		}, null, showResource: true);
	}

	public void SelectCurrentDeck()
	{
		SelectDeck(m_SelectDeckIndex);
	}

	public void SelectDeck(NKMDeckIndex index)
	{
		if (m_SelectDeckIndex != index)
		{
			m_ViewerOptions.dOnChangeDeckIndex?.Invoke(index);
		}
		m_SelectDeckIndex = index;
		m_NKCDeckViewSide.ChangeDeckIndex(m_SelectDeckIndex);
		UpdateAssistUnitUI();
		m_NKCDeckViewList.SetDeckListButton(IsMultiSelect(), NKMArmyData, m_ViewerOptions, m_SelectDeckIndex.m_iIndex);
		CloseSupList();
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			long shipUId = NKCLocalDeckDataManager.GetShipUId(m_SelectDeckIndex.m_iIndex);
			NKMUnitData shipFromUID = NKMArmyData.GetShipFromUID(shipUId);
			m_NKCDeckViewShip.SetShipSlotData(shipFromUID, NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode));
		}
		else
		{
			m_NKCDeckViewShip.SetShipSlotData(NKMArmyData.GetDeckShip(m_SelectDeckIndex), NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode));
		}
		GetCurrDeckViewUnit()?.SetDeckListButton(NKMArmyData, m_SelectDeckIndex, m_ViewerOptions);
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			long operatorUId = NKCLocalDeckDataManager.GetOperatorUId(m_SelectDeckIndex.m_iIndex);
			NKMOperator operatorFromUId = NKMArmyData.GetOperatorFromUId(operatorUId);
			UpdateOperator(operatorFromUId);
		}
		else
		{
			UpdateOperator(NKMArmyData.GetDeckOperator(m_SelectDeckIndex));
		}
		EnableUnitView(bEnable: true);
		m_NKCDeckViewShip.SetSelectEffect(value: false);
		m_NKM_UI_OPERATOR_DECK_SLOT.SetSelectEffect(bActive: false);
		SetTitleActive(isSupporter: false);
		NKCUtil.SetLabelText(m_lbDeckNamePlaceholder, GetDeckDefaultName(index));
		NKCUtil.SetLabelText(m_lbSquadNumber, NKCUtilString.GET_STRING_SQUAD_TWO_PARAM, index.m_iIndex + 1, NKCUtilString.GetRankNumber(index.m_iIndex + 1, bUpper: true));
		foreach (DeckTypeIconObject item in m_lstDecktypeIcon)
		{
			NKCUtil.SetGameobjectActive(item.objSquadIcon, index.m_eDeckType == item.eDeckType);
		}
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			m_bDeckInMission = false;
			m_bDeckInWarfareBatch = false;
			m_bDeckInDiveBatch = false;
			SetDeckNameInput(bEnable: false);
		}
		else
		{
			NKMDeckData deckData = NKMArmyData.GetDeckData(m_SelectDeckIndex);
			if (deckData != null)
			{
				m_bDeckInMission = deckData.GetState() == NKM_DECK_STATE.DECK_STATE_WORLDMAP_MISSION;
				m_bDeckInWarfareBatch = deckData.GetState() == NKM_DECK_STATE.DECK_STATE_WARFARE;
				m_bDeckInDiveBatch = deckData.GetState() == NKM_DECK_STATE.DECK_STATE_DIVE;
				NKM_DECK_TYPE eDeckType = m_SelectDeckIndex.m_eDeckType;
				bool bEnable = ((eDeckType - 1 <= NKM_DECK_TYPE.NDT_DAILY || eDeckType == NKM_DECK_TYPE.NDT_DIVE) ? true : false);
				SetDeckNameInput(bEnable, GetDeckName(m_SelectDeckIndex));
			}
			else
			{
				m_bDeckInMission = false;
				m_bDeckInWarfareBatch = false;
				m_bDeckInDiveBatch = false;
				SetDeckNameInput(bEnable: false);
			}
		}
		m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetUnitData(null, bLeader: false, IsUnitChangePossible());
		m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetOperatorData(null, IsUnitChangePossible());
		SelectDeckViewUnit(m_SelectUnitSlotIndex, bForce: true);
		OnDeckUpdate();
		UpdateDeckReadyState();
		UpdateDeckToggleUI();
		if ((m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareRaid || m_ViewerOptions.eDeckviewerMode == DeckViewerMode.GuildCoopBoss) && !m_NKCDeckViewSide.GetDeckViewSideUnitIllust().hasUnitData() && !m_NKCDeckViewUnitSelectList.IsOpen)
		{
			OnClickCloseBtnOfDeckViewSide();
		}
	}

	private void SetTitleActive(bool isSupporter)
	{
		NKCUtil.SetGameobjectActive(m_ifDeckName, !isSupporter);
		NKCUtil.SetGameobjectActive(m_csbtnDeckName, !isSupporter);
		NKCUtil.SetGameobjectActive(m_lbSquadNumber, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbSupporterName, isSupporter);
		NKCUtil.SetGameobjectActive(m_csbtn_NKM_DECK_VIEW_HELP, !isSupporter);
	}

	public void SetDeckScroll(int deckIndex)
	{
		m_NKCDeckViewList.SetScrollPosition(deckIndex);
	}

	public void DeckViewListClick(NKMDeckIndex index)
	{
		m_NKCDeckViewList.DeckViewListClick(index);
	}

	private void UpdateDeckReadyState()
	{
		if (m_ViewerOptions.dCheckSideMenuButton != null)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = m_ViewerOptions.dCheckSideMenuButton(m_SelectDeckIndex);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				m_NKCDeckViewSide.SetEnableButtons(nKM_ERROR_CODE);
				return;
			}
		}
		switch (m_ViewerOptions.eDeckviewerMode)
		{
		case DeckViewerMode.WorldMapMissionDeckSelect:
		{
			NKM_ERROR_CODE nKM_ERROR_CODE2 = NKMWorldMapManager.IsValidDeckForWorldMapMission(NKCScenManager.CurrentUserData(), m_SelectDeckIndex, m_ViewerOptions.WorldMapMissionCityID);
			bool bDeckReady = nKM_ERROR_CODE2 == NKM_ERROR_CODE.NEC_OK;
			m_NKCDeckViewSide.SetEnableButtons(nKM_ERROR_CODE2);
			int successRate = NKMWorldMapManager.GetMissionSuccessRate(NKMWorldMapManager.GetMissionTemplet(m_ViewerOptions.WorldMapMissionID), cityData: NKCScenManager.CurrentUserData().m_WorldmapData.GetCityData(m_ViewerOptions.WorldMapMissionCityID), armyData: NKMArmyData);
			NKCCompanyBuff.IncreaseMissioRateInWorldMap(NKMArmyData.Owner.m_companyBuffDataList, ref successRate);
			m_NKCDeckViewSide.SetSuccessRate(successRate, bDeckReady);
			break;
		}
		case DeckViewerMode.WarfareRecovery:
		case DeckViewerMode.DeckMultiSelect:
		case DeckViewerMode.PrepareLocalDeck:
		case DeckViewerMode.TournamentApply:
			m_NKCDeckViewSide.SetEnableButtons(NKM_ERROR_CODE.NEC_OK);
			break;
		case DeckViewerMode.WarfareBatch_Assault:
		{
			NKM_ERROR_CODE nKM_ERROR_CODE3 = NKMMain.IsValidDeck(NKMArmyData, m_SelectDeckIndex);
			if (nKM_ERROR_CODE3 == NKM_ERROR_CODE.NEC_OK && !NKCWarfareManager.CheckAssaultShip(NKCScenManager.CurrentUserData(), m_SelectDeckIndex))
			{
				nKM_ERROR_CODE3 = NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_ASSAULT_POSITION;
			}
			m_NKCDeckViewSide.SetEnableButtons(nKM_ERROR_CODE3);
			break;
		}
		case DeckViewerMode.WarfareBatch:
		{
			NKM_ERROR_CODE enableButtons5 = NKMMain.IsValidDeck(NKMArmyData, m_SelectDeckIndex);
			m_NKCDeckViewSide.SetEnableButtons(enableButtons5);
			break;
		}
		case DeckViewerMode.PvPBattleFindTarget:
		case DeckViewerMode.UnlimitedDeck:
		{
			NKM_ERROR_CODE enableButtons4 = NKMMain.IsValidDeck(NKMArmyData, m_SelectDeckIndex.m_eDeckType, m_SelectDeckIndex.m_iIndex, NKM_GAME_TYPE.NGT_PVP_RANK);
			m_NKCDeckViewSide.SetEnableButtons(enableButtons4);
			break;
		}
		case DeckViewerMode.AsyncPvPBattleStart:
		case DeckViewerMode.AsyncPvpDefenseDeck:
		{
			NKM_ERROR_CODE enableButtons3 = NKMMain.IsValidDeck(NKMArmyData, m_SelectDeckIndex.m_eDeckType, m_SelectDeckIndex.m_iIndex, NKM_GAME_TYPE.NGT_ASYNC_PVP);
			m_NKCDeckViewSide.SetEnableButtons(enableButtons3);
			break;
		}
		case DeckViewerMode.MainDeckSelect:
		{
			NKM_ERROR_CODE enableButtons2 = NKM_ERROR_CODE.NEC_FAIL_DECK_DATA_INVALID;
			NKMDeckData deckData = NKMArmyData.GetDeckData(m_SelectDeckIndex);
			if (deckData != null)
			{
				enableButtons2 = NKMMain.IsValidDeckCommon(NKMArmyData, deckData, m_SelectDeckIndex, NKM_GAME_TYPE.NGT_INVALID);
			}
			m_NKCDeckViewSide.SetEnableButtons(enableButtons2);
			break;
		}
		default:
		{
			NKM_ERROR_CODE enableButtons = NKMMain.IsValidDeck(NKMArmyData, m_SelectDeckIndex);
			m_NKCDeckViewSide.SetEnableButtons(enableButtons);
			break;
		}
		}
	}

	public void SetShipSlotData()
	{
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			long shipUId = NKCLocalDeckDataManager.GetShipUId(m_SelectDeckIndex.m_iIndex);
			m_NKCDeckViewShip.SetShipSlotData(NKMArmyData.GetShipFromUID(shipUId), NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode));
		}
		else
		{
			m_NKCDeckViewShip.SetShipSlotData(NKMArmyData.GetDeckShip(m_SelectDeckIndex), NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode));
		}
		if (NeedShowChangedTag())
		{
			NKCUtil.SetGameobjectActive(m_objShipChanged, NKCTournamentManager.IsShipChanged(NKMArmyData.GetDeckShip(m_SelectDeckIndex)));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objShipChanged, bValue: false);
		}
	}

	public void SetLeader(NKMDeckIndex deckIndex, int leaderIndex, bool bEffect)
	{
		GetCurrDeckViewUnit()?.SetLeader(leaderIndex, bEffect);
		if (leaderIndex == m_SelectUnitSlotIndex)
		{
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetLeader(bLeader: true);
		}
	}

	public void SelectDeckViewUnit(int index, bool bForce = false, bool bOpenUnitSelectListIfEmpty = false)
	{
		m_SelectUnitSlotIndex = index;
		m_NKCDeckViewShip.SetSelectEffect(value: false);
		m_NKM_UI_OPERATOR_DECK_SLOT.SetSelectEffect(bActive: false);
		GetCurrDeckViewUnit()?.SelectDeckViewUnit(index, bForce);
		if (index >= 0)
		{
			NKMUnitData nKMUnitData = null;
			bool flag = false;
			m_DeckUnitList.Clear();
			if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
			{
				NKCLocalDeckDataManager.GetLocalUnitDeckData(m_SelectDeckIndex.m_iIndex).ForEach(delegate(long e)
				{
					m_DeckUnitList.Add(e);
				});
				if (m_DeckUnitList.Count > index)
				{
					nKMUnitData = NKMArmyData.GetUnitFromUID(m_DeckUnitList[index]);
				}
				flag = index == NKCLocalDeckDataManager.GetLocalLeaderIndex(m_SelectDeckIndex.m_iIndex);
			}
			else
			{
				NKMArmyData.GetDeckList(m_SelectDeckIndex.m_eDeckType, m_SelectDeckIndex.m_iIndex, ref m_DeckUnitList);
				nKMUnitData = NKMArmyData.GetDeckUnitByIndex(m_SelectDeckIndex, m_SelectUnitSlotIndex);
				flag = NKMArmyData.GetDeckData(m_SelectDeckIndex)?.m_LeaderIndex == m_SelectUnitSlotIndex;
			}
			if (bOpenUnitSelectListIfEmpty && nKMUnitData == null)
			{
				OpenDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL, 0L);
			}
			else
			{
				UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL, nKMUnitData?.m_UnitUID ?? 0);
			}
			if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareRaid)
			{
				if (m_NKCUIRaidRightSide != null)
				{
					m_NKCUIRaidRightSide.Close();
				}
				m_NKCDeckViewSide.Open(m_ViewerOptions, bInit: false, bUseCost: false);
			}
			else if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.GuildCoopBoss)
			{
				if (m_NKCUIGuildCoopRaidRightSide != null)
				{
					m_NKCUIGuildCoopRaidRightSide.Close();
				}
				m_NKCDeckViewSide.Open(m_ViewerOptions, bInit: false, bUseCost: false);
			}
			m_NKCDeckViewSide.SetUnitData(nKMUnitData);
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetOperatorData(null, IsUnitChangePossible());
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetUnitData(nKMUnitData, flag, IsUnitChangePossible(), bForce);
			if (NeedShowChangedTag())
			{
				m_NKCDeckViewSide.CheckUnitChanged(GetSavedTournamentDeckUnit(index));
			}
		}
		else if (m_NKCDeckViewUnitSelectList.IsOpen)
		{
			switch (m_eCurrentSelectListType)
			{
			case NKM_UNIT_TYPE.NUT_SHIP:
				UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_SHIP, (m_NKCDeckViewShip.GetUnitData() != null) ? m_NKCDeckViewShip.GetUnitData().m_UnitUID : 0);
				break;
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
				{
					long operatorUId = NKCLocalDeckDataManager.GetOperatorUId(m_SelectDeckIndex.m_iIndex);
					UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_OPERATOR, NKMArmyData.GetOperatorFromUId(operatorUId)?.uid ?? 0);
				}
				else
				{
					UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_OPERATOR, NKMArmyData.GetDeckOperatorByIndex(m_SelectDeckIndex)?.uid ?? 0);
				}
				break;
			}
		}
		EnableUnitView(bEnable: true);
	}

	public void SetDeckViewShipUnit(long shipUID)
	{
		NKMUnitData shipFromUID = NKMArmyData.GetShipFromUID(shipUID);
		m_NKCDeckViewSide.SetUnitData(shipFromUID);
		m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetUnitData(shipFromUID, bLeader: false, IsUnitChangePossible());
		m_NKCDeckViewSide.GetDeckViewSideUnitIllust().PlayLoadingAnim("CLOSE");
		if (shipFromUID == null)
		{
			EnableUnitView(bEnable: true);
		}
		else
		{
			EnableUnitView(bEnable: false);
		}
	}

	public void DeckViewShipClick()
	{
		if (IsSupportMenu)
		{
			return;
		}
		EnableUnitView(bEnable: false);
		m_NKM_UI_OPERATOR_DECK_SLOT.SetSelectEffect(bActive: false);
		if (m_NKCDeckViewShip.GetUnitData() != null)
		{
			m_NKCDeckViewSide.SetUnitData(m_NKCDeckViewShip.GetUnitData());
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetUnitData(m_NKCDeckViewShip.GetUnitData(), bLeader: false, IsUnitChangePossible(), m_eCurrentSelectListType == NKM_UNIT_TYPE.NUT_OPERATOR);
			UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_SHIP, m_NKCDeckViewShip.GetUnitData().m_UnitUID);
			m_NKCDeckViewShip.SetSelectEffect(value: false);
		}
		else
		{
			m_NKCDeckViewShip.SetSelectEffect(value: true);
			OpenDeckSelectList(NKM_UNIT_TYPE.NUT_SHIP, 0L);
		}
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareRaid)
		{
			if (m_NKCUIRaidRightSide != null)
			{
				m_NKCUIRaidRightSide.Close();
			}
		}
		else if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.GuildCoopBoss && m_NKCUIGuildCoopRaidRightSide != null)
		{
			m_NKCUIGuildCoopRaidRightSide.Close();
		}
		m_SelectUnitSlotIndex = -1;
		GetCurrDeckViewUnit()?.SelectDeckViewUnit(-1);
	}

	public void DeckViewUnitInfoClick(NKMUnitData UnitData)
	{
		if (UnitData != null)
		{
			switch (NKMUnitManager.GetUnitTempletBase(UnitData.m_UnitID).m_NKM_UNIT_TYPE)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				NKCUIUnitInfo.Instance.Open(UnitData, OnRemoveUnit, new NKCUIUnitInfo.OpenOption(m_DeckUnitList, m_SelectUnitSlotIndex));
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
				NKCUIShipInfo.Instance.Open(UnitData, m_SelectDeckIndex);
				break;
			}
		}
	}

	public void EnableUnitView(bool bEnable, bool bForce = false)
	{
		if (m_bUnitViewEnable != bEnable || bForce)
		{
			bool flag = false;
			flag = ((m_ViewerOptions.eDeckviewerMode != DeckViewerMode.PrepareLocalDeck) ? (NKMArmyData.GetDeckShip(m_SelectDeckIndex) == null) : (NKCLocalDeckDataManager.GetShipUId(m_SelectDeckIndex.m_iIndex) <= 0));
			if (bEnable || flag)
			{
				PlayLoadingAnim("CHANGE");
				m_NKCDeckViewOperator.Enable();
				GetCurrDeckViewUnit()?.Enable();
				m_NKCDeckViewShip.Disable();
				m_bUnitViewEnable = true;
			}
			else
			{
				PlayLoadingAnim("CHANGE");
				m_NKCDeckViewOperator.Disable();
				GetCurrDeckViewUnit()?.Disable();
				m_NKCDeckViewShip.Enable(bInfoActive: false);
				m_bUnitViewEnable = false;
			}
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (itemData != null)
		{
			m_NKCDeckViewSide?.UpdateCostUI(itemData);
		}
	}

	public override void OnEquipChange(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipItem)
	{
		if (eType == NKMUserData.eChangeNotifyType.Update)
		{
			NKMUnitData unitData = m_NKCDeckViewSide.GetDeckViewSideUnitIllust().GetUnitData();
			if (unitData != null && (unitData.GetEquipItemWeaponUid() == equipUID || unitData.GetEquipItemDefenceUid() == equipUID || unitData.GetEquipItemAccessoryUid() == equipUID || unitData.GetEquipItemAccessory2Uid() == equipUID))
			{
				SelectCurrentDeck();
			}
		}
	}

	public void PlayLoadingAnim(string name)
	{
		m_NKCDeckViewSide.PlayLoadingAnim(name);
	}

	public void OnUnitDragEnd(int oldIndex, int newIndex)
	{
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			long localUnitData = NKCLocalDeckDataManager.GetLocalUnitData(m_SelectDeckIndex.m_iIndex, oldIndex);
			NKMUnitData unitFromUID = NKMArmyData.GetUnitFromUID(localUnitData);
			long localUnitData2 = NKCLocalDeckDataManager.GetLocalUnitData(m_SelectDeckIndex.m_iIndex, newIndex);
			NKMUnitData unitFromUID2 = NKMArmyData.GetUnitFromUID(localUnitData2);
			if (unitFromUID == null && unitFromUID2 == null)
			{
				SetUnitSlotData(m_SelectDeckIndex, oldIndex, bEffect: true);
				SetUnitSlotData(m_SelectDeckIndex, newIndex, bEffect: true);
				SelectDeckViewUnit(newIndex);
				return;
			}
			NKCLocalDeckDataManager.SwapSlotData(m_SelectDeckIndex.m_iIndex, oldIndex, newIndex);
			SetUnitSlotData(m_SelectDeckIndex, oldIndex, bEffect: true);
			SetUnitSlotData(m_SelectDeckIndex, newIndex, bEffect: true);
			SelectDeckViewUnit(newIndex);
			NKCSoundManager.PlaySound("FX_UI_DECK_UNIIT_SELECT", 1f, 0f, 0f);
			int localLeaderIndex = NKCLocalDeckDataManager.GetLocalLeaderIndex(m_SelectDeckIndex.m_iIndex);
			if (localLeaderIndex != -1)
			{
				SetLeader(m_SelectDeckIndex, localLeaderIndex, bEffect: false);
			}
		}
		else
		{
			NKMUnitData deckUnitByIndex = NKMArmyData.GetDeckUnitByIndex(m_SelectDeckIndex, oldIndex);
			NKMUnitData deckUnitByIndex2 = NKMArmyData.GetDeckUnitByIndex(m_SelectDeckIndex, newIndex);
			if (deckUnitByIndex == null && deckUnitByIndex2 == null)
			{
				SetUnitSlotData(m_SelectDeckIndex, oldIndex, bEffect: true);
				SetUnitSlotData(m_SelectDeckIndex, newIndex, bEffect: true);
				SelectDeckViewUnit(newIndex);
			}
			else
			{
				Send_NKMPacket_DECK_UNIT_SWAP_REQ(m_SelectDeckIndex, oldIndex, newIndex);
			}
		}
	}

	public void OnUnitClicked(int index)
	{
		SelectDeckViewUnit(index, m_eCurrentSelectListType == NKM_UNIT_TYPE.NUT_OPERATOR, bOpenUnitSelectListIfEmpty: true);
	}

	private NKCDeckViewSideUnitIllust.eUnitChangePossible IsUnitChangePossible()
	{
		if (m_bDeckInMission)
		{
			return NKCDeckViewSideUnitIllust.eUnitChangePossible.WORLDMAP_MISSION;
		}
		if (m_bDeckInWarfareBatch)
		{
			return NKCDeckViewSideUnitIllust.eUnitChangePossible.WARFARE;
		}
		if (m_bDeckInDiveBatch)
		{
			return NKCDeckViewSideUnitIllust.eUnitChangePossible.DIVE;
		}
		return NKCDeckViewSideUnitIllust.eUnitChangePossible.OK;
	}

	public void OnSideMenuButtonConfirm()
	{
		if (m_ViewerOptions.dOnSideMenuButtonConfirm != null)
		{
			m_ViewerOptions.dOnSideMenuButtonConfirm(m_SelectDeckIndex, GetSupportUserUID(m_SelectDeckIndex));
		}
		else if (m_ViewerOptions.dOnDeckSideButtonConfirmForMulti != null)
		{
			List<NKMDeckIndex> multiSelectedDeckIndexList = m_NKCDeckViewList.GetMultiSelectedDeckIndexList();
			m_ViewerOptions.dOnDeckSideButtonConfirmForMulti(multiSelectedDeckIndexList);
		}
		else if (m_ViewerOptions.dOnDeckSideButtonConfirmForAsync != null)
		{
			m_ViewerOptions.dOnDeckSideButtonConfirmForAsync(m_SelectDeckIndex, m_AsyncOriginalDeckData);
		}
	}

	private bool CheckOperationMultiply(bool bMsg)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_MULTIPLY))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(m_ViewerOptions.StageBattleStrID))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(m_ViewerOptions.StageBattleStrID);
			if (nKMStageTempletV != null)
			{
				if (!nKMUserData.IsSuperUser())
				{
					if (nKMUserData.CheckDungeonClear(nKMStageTempletV.m_StageBattleStrID))
					{
						NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(nKMStageTempletV.m_StageBattleStrID);
						if (dungeonTempletBase == null)
						{
							return false;
						}
						NKMDungeonClearData dungeonClearData = nKMUserData.GetDungeonClearData(dungeonTempletBase.m_DungeonStrID);
						if (nKMStageTempletV.EpisodeCategory != EPISODE_CATEGORY.EC_DAILY && (dungeonClearData == null || !dungeonClearData.missionResult1 || !dungeonClearData.missionResult2))
						{
							if (bMsg)
							{
								NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_MULTIPLY_OPERATION_MEDAL_COND);
							}
							return false;
						}
						if (dungeonTempletBase.m_RewardMultiplyMax <= 1)
						{
							return false;
						}
					}
					else
					{
						if (!nKMUserData.CheckWarfareClear(nKMStageTempletV.m_StageBattleStrID))
						{
							if (nKMStageTempletV.EpisodeCategory == EPISODE_CATEGORY.EC_DAILY)
							{
								if (bMsg)
								{
									NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_CONTENTS_UNLOCK_CLEAR_STAGE);
								}
								return false;
							}
							if (bMsg)
							{
								NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_MULTIPLY_OPERATION_MEDAL_COND);
							}
							return false;
						}
						NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(nKMStageTempletV.m_StageBattleStrID);
						if (nKMWarfareTemplet == null)
						{
							return false;
						}
						NKMWarfareClearData warfareClearData = nKMUserData.GetWarfareClearData(nKMWarfareTemplet.m_WarfareID);
						if (warfareClearData == null || !warfareClearData.m_mission_result_1 || !warfareClearData.m_mission_result_2)
						{
							if (bMsg)
							{
								NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_MULTIPLY_OPERATION_MEDAL_COND);
							}
							return false;
						}
						if (nKMWarfareTemplet.m_RewardMultiplyMax <= 1)
						{
							return false;
						}
					}
				}
				if (nKMStageTempletV.EnterLimit > 0)
				{
					int statePlayCnt = nKMUserData.GetStatePlayCnt(nKMStageTempletV.Key);
					if (m_ViewerOptions.bUsableOperationSkip && nKMStageTempletV.EnterLimit - statePlayCnt <= 0)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public int GetCurrMultiplyRewardCount()
	{
		return m_NKCDeckViewSide.GetCurrMultiplyRewardCount();
	}

	public bool GetOperationSkipState()
	{
		return m_NKCDeckViewSide.OperationSkip;
	}

	public void CloseOperationSkip()
	{
		if (m_NKCDeckViewSide.OperationSkip)
		{
			m_NKCDeckViewSide.m_tglSkip.Select(bSelect: false);
		}
	}

	private void OnClickCloseBtnOfDeckViewSide()
	{
		m_NKCDeckViewSide.GetDeckViewSideUnitIllust().ResetObj();
		if (m_NKCUIRaidRightSide != null)
		{
			m_NKCUIRaidRightSide.Open();
		}
		else if (m_NKCUIGuildCoopRaidRightSide != null)
		{
			m_NKCUIGuildCoopRaidRightSide.Open();
		}
		SelectDeckViewUnit(-1);
	}

	private void OnClickRaidAttck(long raidUID, List<int> _Buffs, int reqItemID, int reqItemCount, bool bIsTryAssist, bool isPracticeMode)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		if (!nKMUserData.CheckPrice(reqItemCount, reqItemID))
		{
			NKCShopManager.OpenItemLackPopup(reqItemID, reqItemCount);
		}
		else if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.GuildCoopBoss)
		{
			if (isPracticeMode || NKCPacketHandlers.Check_NKM_ERROR_CODE(NKCGuildCoopManager.CanStartBoss()))
			{
				if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_RAID_READY)
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID_READY().SetLastDeckIndex(m_SelectDeckIndex);
				}
				NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ(NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageId(), m_SelectDeckIndex.m_iIndex, isPracticeMode);
			}
		}
		else
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_RAID_READY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID_READY().SetLastDeckIndex(m_SelectDeckIndex);
			}
			NKCPacketSender.Send_NKMPacket_RAID_GAME_LOAD_REQ(raidUID, m_SelectDeckIndex.m_iIndex, _Buffs, bIsTryAssist);
		}
	}

	public void SetEnableUnlockEffect(NKCDeckListButton btn)
	{
		if (m_objSlotUnlockEffect != null && btn != null)
		{
			Transform parent = btn.transform;
			m_objSlotUnlockEffect.transform.SetParent(parent);
			m_objSlotUnlockEffect.transform.localPosition = Vector3.zero;
			m_objSlotUnlockEffect.transform.localScale = Vector3.one;
			NKCUtil.SetGameobjectActive(m_objSlotUnlockEffect, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSlotUnlockEffect, bValue: true);
			NKCSoundManager.PlaySound("FX_UI_DECK_SLOT_OPEN", 1f, 0f, 0f);
		}
	}

	private bool ContainEmptySlotCost(NKM_ERROR_CODE errorCode)
	{
		if (errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			return false;
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.DECKVIEW))
		{
			return false;
		}
		if (m_ViewerOptions.CostItemID != 2)
		{
			return false;
		}
		return NKMArmyData.GetDeckData(m_SelectDeckIndex)?.m_listDeckUnitUID.Contains(0L) ?? false;
	}

	private void CloseDeckSelectList(bool bAnimate)
	{
		m_eCurrentSelectListType = NKM_UNIT_TYPE.NUT_INVALID;
		m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetShipOnlyMode(value: false);
		m_NKCDeckViewUnitSelectList.Close(bAnimate);
		UpdateUpsideMenu();
	}

	private void OpenDeckSelectList(NKM_UNIT_TYPE eUnitType, long selectedUnitUID)
	{
		m_eCurrentSelectListType = eUnitType;
		if (m_NKCDeckViewUnitSelectList.IsOpen)
		{
			UpdateDeckSelectList(eUnitType, selectedUnitUID);
		}
		else
		{
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetShipOnlyMode(value: true);
			if (eUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
			{
				if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
				{
					m_NKCDeckViewUnitSelectList.Open(bAnimate: true, eUnitType, MakeLocalOperatorSortOptions(selectedUnitUID), m_ViewerOptions);
				}
				else
				{
					m_NKCDeckViewUnitSelectList.Open(bAnimate: true, eUnitType, MakeOperatorSortOptions(selectedUnitUID), m_ViewerOptions);
				}
			}
			else if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
			{
				m_NKCDeckViewUnitSelectList.Open(bAnimate: true, eUnitType, MakeLocalSortOptions(eUnitType, selectedUnitUID), m_ViewerOptions);
			}
			else
			{
				m_NKCDeckViewUnitSelectList.Open(bAnimate: true, eUnitType, MakeSortOptions(selectedUnitUID), m_ViewerOptions);
			}
		}
		UpdateUpsideMenu();
	}

	private void UpdateDeckSelectList(NKM_UNIT_TYPE eUnitType, long selectedUnitUID)
	{
		m_eCurrentSelectListType = eUnitType;
		if (m_eCurrentSelectListType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
			{
				m_NKCDeckViewUnitSelectList.UpdateLoopScrollList(eUnitType, MakeLocalOperatorSortOptions(selectedUnitUID));
			}
			else
			{
				m_NKCDeckViewUnitSelectList.UpdateLoopScrollList(eUnitType, MakeOperatorSortOptions(selectedUnitUID));
			}
		}
		else if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			m_NKCDeckViewUnitSelectList.UpdateLoopScrollList(eUnitType, MakeLocalSortOptions(eUnitType, selectedUnitUID));
		}
		else
		{
			m_NKCDeckViewUnitSelectList.UpdateLoopScrollList(eUnitType, MakeSortOptions(selectedUnitUID));
		}
	}

	private NKCUnitSortSystem.UnitListOptions MakeSortOptions(long selectedUnitUID)
	{
		NKCUnitSortSystem.UnitListOptions result = new NKCUnitSortSystem.UnitListOptions
		{
			eDeckType = m_SelectDeckIndex.m_eDeckType,
			setExcludeUnitID = null,
			setOnlyIncludeUnitID = null,
			setDuplicateUnitID = null,
			setExcludeUnitUID = null,
			bExcludeLockedUnit = false,
			bExcludeDeckedUnit = false,
			bIgnoreCityState = true,
			bIgnoreWorldMapLeader = true,
			setFilterOption = m_NKCDeckViewUnitSelectList.SortOptions.setFilterOption,
			lstSortOption = m_NKCDeckViewUnitSelectList.SortOptions.lstSortOption,
			bDescending = m_NKCDeckViewUnitSelectList.SortOptions.bDescending,
			bIncludeUndeckableUnit = false,
			bHideDeckedUnit = (m_SelectDeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_NORMAL || m_SelectDeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_TOURNAMENT),
			bPushBackUnselectable = true,
			bHideTokenFiltering = false
		};
		if (m_SelectDeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_TOURNAMENT)
		{
			if (m_eCurrentSelectListType == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				result.setExcludeUnitID = NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_NORMAL);
			}
			else if (m_eCurrentSelectListType == NKM_UNIT_TYPE.NUT_SHIP)
			{
				result.setExcludeUnitBaseID = NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_SHIP);
			}
		}
		result.MakeDuplicateUnitSet(m_SelectDeckIndex, selectedUnitUID, NKMArmyData);
		return result;
	}

	private NKCOperatorSortSystem.OperatorListOptions MakeOperatorSortOptions(long selectedUnitUID)
	{
		NKCOperatorSortSystem.OperatorListOptions result = new NKCOperatorSortSystem.OperatorListOptions
		{
			eDeckType = m_SelectDeckIndex.m_eDeckType,
			setExcludeOperatorID = null,
			setOnlyIncludeOperatorID = null,
			setDuplicateOperatorID = null,
			setExcludeOperatorUID = null,
			setFilterOption = m_NKCDeckViewUnitSelectList.SortOperatorOptions.setFilterOption,
			lstSortOption = m_NKCDeckViewUnitSelectList.SortOperatorOptions.lstSortOption
		};
		result.SetBuildOption(true, BUILD_OPTIONS.PUSHBACK_UNSELECTABLE);
		result.MakeDuplicateSetFromAllDeck(m_SelectDeckIndex, selectedUnitUID, NKMArmyData);
		return result;
	}

	private NKCUnitSortSystem.UnitListOptions MakeLocalSortOptions(NKM_UNIT_TYPE unitType, long selectedUnitUID)
	{
		NKCUnitSortSystem.UnitListOptions result = new NKCUnitSortSystem.UnitListOptions
		{
			eDeckType = m_SelectDeckIndex.m_eDeckType,
			setExcludeUnitID = null,
			setOnlyIncludeUnitID = null,
			setDuplicateUnitID = null,
			setExcludeUnitUID = null,
			bExcludeLockedUnit = false,
			bExcludeDeckedUnit = false,
			bIgnoreCityState = true,
			bIgnoreWorldMapLeader = true,
			setFilterOption = m_NKCDeckViewUnitSelectList.SortOptions.setFilterOption,
			lstSortOption = m_NKCDeckViewUnitSelectList.SortOptions.lstSortOption,
			bDescending = m_NKCDeckViewUnitSelectList.SortOptions.bDescending,
			bIncludeUndeckableUnit = false,
			bHideDeckedUnit = (m_SelectDeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_NORMAL),
			bPushBackUnselectable = true
		};
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			result.setExcludeUnitUID = new HashSet<long>();
			result.setDuplicateUnitID = new HashSet<int>();
			foreach (KeyValuePair<int, NKMEventDeckData> allLocalDeckDatum in NKCLocalDeckDataManager.GetAllLocalDeckData())
			{
				foreach (KeyValuePair<int, long> item in allLocalDeckDatum.Value.m_dicUnit)
				{
					long value = item.Value;
					if (value != 0L && selectedUnitUID != value)
					{
						NKMUnitData unitFromUID = NKMArmyData.GetUnitFromUID(value);
						if (unitFromUID != null)
						{
							result.setDuplicateUnitID.Add(unitFromUID.m_UnitID);
						}
					}
				}
			}
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
			result.setExcludeUnitUID = new HashSet<long>();
			result.setDuplicateUnitID = new HashSet<int>();
			foreach (KeyValuePair<int, NKMEventDeckData> allLocalDeckDatum2 in NKCLocalDeckDataManager.GetAllLocalDeckData())
			{
				long shipUID = allLocalDeckDatum2.Value.m_ShipUID;
				if (shipUID == 0L || selectedUnitUID == shipUID)
				{
					continue;
				}
				NKMUnitData shipFromUID = NKMArmyData.GetShipFromUID(shipUID);
				if (shipFromUID == null)
				{
					continue;
				}
				NKMUnitTempletBase shipTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
				if (shipTempletBase == null)
				{
					continue;
				}
				foreach (NKMUnitTempletBase item2 in NKMUnitTempletBase.Values.Where((NKMUnitTempletBase e) => e.m_ShipGroupID == shipTempletBase.m_ShipGroupID))
				{
					result.setDuplicateUnitID.Add(item2.m_UnitID);
				}
			}
			break;
		}
		return result;
	}

	private NKCOperatorSortSystem.OperatorListOptions MakeLocalOperatorSortOptions(long selectedUnitUID)
	{
		NKCOperatorSortSystem.OperatorListOptions result = new NKCOperatorSortSystem.OperatorListOptions
		{
			eDeckType = m_SelectDeckIndex.m_eDeckType,
			setExcludeOperatorID = null,
			setOnlyIncludeOperatorID = null,
			setDuplicateOperatorID = null,
			setExcludeOperatorUID = null,
			setFilterOption = m_NKCDeckViewUnitSelectList.SortOperatorOptions.setFilterOption,
			lstSortOption = m_NKCDeckViewUnitSelectList.SortOperatorOptions.lstSortOption
		};
		Dictionary<int, NKMEventDeckData> allLocalDeckData = NKCLocalDeckDataManager.GetAllLocalDeckData();
		if (allLocalDeckData.Count > 0)
		{
			result.setExcludeOperatorUID = new HashSet<long>();
			result.setDuplicateOperatorID = new HashSet<int>();
			foreach (KeyValuePair<int, NKMEventDeckData> item in allLocalDeckData)
			{
				long operatorUID = item.Value.m_OperatorUID;
				if (operatorUID != 0L && selectedUnitUID != operatorUID)
				{
					NKMOperator operatorFromUId = NKMArmyData.GetOperatorFromUId(operatorUID);
					if (operatorFromUId != null)
					{
						result.setDuplicateOperatorID.Add(operatorFromUId.id);
					}
				}
			}
		}
		return result;
	}

	public void OnDeckUnitChangeClicked(NKMDeckIndex unitDeckIndex, long unitUID, NKM_UNIT_TYPE eType)
	{
		if (!NKMArmyData.IsAllowedSameUnitInMultipleDeck(m_SelectDeckIndex.m_eDeckType) && m_ViewerOptions.eDeckviewerMode != DeckViewerMode.PrepareLocalDeck && m_ViewerOptions.eDeckviewerMode != DeckViewerMode.TournamentApply)
		{
			switch (NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitDeckState(unitUID))
			{
			case NKM_DECK_STATE.DECK_STATE_WARFARE:
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_DECK_BATCH_FAIL_STATE_WARFARE);
				return;
			case NKM_DECK_STATE.DECK_STATE_DIVE:
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_DECK_BATCH_FAIL_STATE_DIVE);
				return;
			}
		}
		switch (eType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
		{
			if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
			{
				long localUnitData = NKCLocalDeckDataManager.GetLocalUnitData(m_SelectDeckIndex.m_iIndex, m_SelectUnitSlotIndex);
				if (localUnitData > 0 && unitUID > 0)
				{
					NKMUnitData unitFromUID = NKMArmyData.GetUnitFromUID(localUnitData);
					NKMDeckIndex selectDeckIndex = m_SelectDeckIndex;
					NKMUnitData unitFromUID2 = NKMArmyData.GetUnitFromUID(unitUID);
					NKCUIUnitSelectListChangePopup.Instance.Open(unitFromUID, selectDeckIndex, unitFromUID2, unitDeckIndex, delegate
					{
						RegisterUnitToLocalUnitDeck(unitUID, prohibitSameUnitId: true);
					}, NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode), NKCUtil.CheckPossibleShowUpUnit(m_ViewerOptions.eDeckviewerMode));
				}
				else
				{
					RegisterUnitToLocalUnitDeck(unitUID, prohibitSameUnitId: true);
				}
				break;
			}
			NKMDeckData deckData2 = NKMArmyData.GetDeckData(m_SelectDeckIndex);
			if (deckData2 == null)
			{
				Debug.LogError("현재 덱 정보를 찾지 못함!");
				Send_NKMPacket_DECK_UNIT_SET_REQ(m_SelectDeckIndex, m_SelectUnitSlotIndex, unitUID);
				return;
			}
			long num = deckData2.m_listDeckUnitUID[m_SelectUnitSlotIndex];
			if (num != 0L && unitUID != 0L)
			{
				NKMUnitData unitFromUID3 = NKMArmyData.GetUnitFromUID(num);
				NKMDeckIndex selectDeckIndex2 = m_SelectDeckIndex;
				NKMUnitData unitFromUID4 = NKMArmyData.GetUnitFromUID(unitUID);
				NKCUIUnitSelectListChangePopup.Instance.Open(unitFromUID3, selectDeckIndex2, unitFromUID4, unitDeckIndex, delegate
				{
					Send_NKMPacket_DECK_UNIT_SET_REQ(m_SelectDeckIndex, m_SelectUnitSlotIndex, unitUID);
				}, NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode), NKCUtil.CheckPossibleShowUpUnit(m_ViewerOptions.eDeckviewerMode));
			}
			else
			{
				Send_NKMPacket_DECK_UNIT_SET_REQ(m_SelectDeckIndex, m_SelectUnitSlotIndex, unitUID);
			}
			break;
		}
		case NKM_UNIT_TYPE.NUT_OPERATOR:
		{
			if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
			{
				long operatorUId = NKCLocalDeckDataManager.GetOperatorUId(m_SelectDeckIndex.m_iIndex);
				if (operatorUId != 0L && unitUID != 0L)
				{
					NKCUIOperatorPopupChange.Instance.Open(m_SelectDeckIndex, operatorUId, unitUID, delegate
					{
						RegisterOperatorToLocalOperatorDeck(unitUID, prohibitSameUnitId: true);
					});
				}
				else
				{
					RegisterOperatorToLocalOperatorDeck(unitUID, prohibitSameUnitId: true);
				}
				break;
			}
			NKMDeckData deckData = NKMArmyData.GetDeckData(m_SelectDeckIndex);
			if (deckData == null)
			{
				Debug.LogError("현재 덱 정보를 찾지 못함!");
				Send_NKMPacket_DECK_UNIT_SET_REQ(m_SelectDeckIndex, m_SelectUnitSlotIndex, unitUID);
				return;
			}
			if (deckData.m_OperatorUID != 0L && unitUID != 0L && NKCOperatorUtil.IsMyOperator(deckData.m_OperatorUID) && NKCOperatorUtil.IsMyOperator(unitUID))
			{
				NKCUIOperatorPopupChange.Instance.Open(m_SelectDeckIndex, deckData.m_OperatorUID, unitUID, delegate
				{
					Send_NKMPacket_DECK_OPERATOR_SET_REQ(m_SelectDeckIndex, unitUID);
				});
			}
			else
			{
				Send_NKMPacket_DECK_OPERATOR_SET_REQ(m_SelectDeckIndex, unitUID);
			}
			break;
		}
		case NKM_UNIT_TYPE.NUT_SHIP:
			if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
			{
				NKCLocalDeckDataManager.SetLocalShipUId(m_SelectDeckIndex.m_iIndex, unitUID, prohibitSameUnitId: true);
				SetShipSlotData();
				long shipUId = NKCLocalDeckDataManager.GetShipUId(m_SelectDeckIndex.m_iIndex);
				SetDeckViewShipUnit(shipUId);
				UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_SHIP, shipUId);
				UpdateDeckReadyState();
				m_NKCDeckViewList.UpdateDeckState();
				OnDeckUpdate();
			}
			else
			{
				Send_NKMPacket_DECK_SHIP_SET_REQ(m_SelectDeckIndex, unitUID);
			}
			break;
		}
		m_ViewerOptions.dOnChangeDeckUnit?.Invoke(m_SelectDeckIndex, unitUID);
	}

	private void OnUnitSelectListClose(NKM_UNIT_TYPE eType)
	{
		m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetShipOnlyMode(value: false);
		m_NKCDeckViewShip.SetSelectEffect(value: false);
		switch (eType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			SelectDeckViewUnit(m_SelectUnitSlotIndex);
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
			if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
			{
				long shipUId = NKCLocalDeckDataManager.GetShipUId(m_SelectDeckIndex.m_iIndex);
				SetDeckViewShipUnit(shipUId);
			}
			else
			{
				SetDeckViewShipUnit(NKMArmyData.GetDeckShip(m_SelectDeckIndex)?.m_UnitUID ?? 0);
			}
			break;
		}
		UpdateUpsideMenu();
		if ((m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareRaid || m_ViewerOptions.eDeckviewerMode == DeckViewerMode.GuildCoopBoss) && !m_NKCDeckViewSide.GetDeckViewSideUnitIllust().hasUnitData())
		{
			OnClickCloseBtnOfDeckViewSide();
		}
	}

	public void UpdateUnitCount()
	{
		m_NKCDeckViewUnitSelectList.UpdateUnitCount();
	}

	private void RegisterUnitToLocalUnitDeck(long unitUId, bool prohibitSameUnitId)
	{
		NKCLocalDeckDataManager.SetLocalUnitUId(m_SelectDeckIndex.m_iIndex, m_SelectUnitSlotIndex, unitUId, prohibitSameUnitId);
		int localLeaderIndex = NKCLocalDeckDataManager.GetLocalLeaderIndex(m_SelectDeckIndex.m_iIndex);
		localLeaderIndex = NKCLocalDeckDataManager.SetLeaderIndex(m_SelectDeckIndex.m_iIndex, (localLeaderIndex < 0) ? m_SelectUnitSlotIndex : localLeaderIndex);
		int count = GetCurrDeckViewUnit().m_listNKCDeckViewUnitSlot.Count;
		for (int i = 0; i < count; i++)
		{
			SetUnitSlotData(m_SelectDeckIndex, i, i == m_SelectUnitSlotIndex);
		}
		if (unitUId > 0 && NKCLocalDeckDataManager.IsNextLocalSlotEmpty(m_SelectDeckIndex.m_iIndex, m_SelectUnitSlotIndex))
		{
			SelectDeckViewUnit(m_SelectUnitSlotIndex + 1);
			UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL, 0L);
		}
		else
		{
			SelectDeckViewUnit(m_SelectUnitSlotIndex);
			UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL, unitUId);
		}
		SetLeader(m_SelectDeckIndex, localLeaderIndex, bEffect: false);
		UpdateDeckReadyState();
		m_NKCDeckViewList.UpdateDeckState();
		OnDeckUpdate();
		if (unitUId != 0L)
		{
			NKMUnitData nKMUnitData = NKCScenManager.CurrentUserData()?.m_ArmyData.GetUnitFromUID(unitUId);
			if (nKMUnitData != null)
			{
				NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_SQUAD_ENTER, nKMUnitData);
			}
		}
	}

	private void RegisterOperatorToLocalOperatorDeck(long operatorUId, bool prohibitSameUnitId)
	{
		NKCLocalDeckDataManager.SetLocalOperatorUId(m_SelectDeckIndex.m_iIndex, operatorUId, prohibitSameUnitId);
		OnSelectOperator(operatorUId);
		UpdateOperator(NKCOperatorUtil.GetOperatorData(operatorUId));
		UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_OPERATOR, operatorUId);
		if (operatorUId == 0L)
		{
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetUnitData(null, bLeader: false, IsUnitChangePossible());
		}
		else
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_SQUAD_ENTER, NKCOperatorUtil.GetOperatorData(operatorUId));
		}
	}

	private void OpenSupList()
	{
		if (m_ViewerOptions.lstSupporter != null)
		{
			CloseDeckSelectList(bAnimate: false);
			m_SelectDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE, -1);
			m_SelectUnitSlotIndex = -1;
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetUnitData(null, bLeader: false, IsUnitChangePossible());
			SelectDeckViewUnit(m_SelectUnitSlotIndex);
			m_NKCDeckViewList.SetDeckListButton(IsMultiSelect(), NKMArmyData, m_ViewerOptions, m_SelectDeckIndex.m_iIndex);
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetShipOnlyMode(value: false);
			if (m_NKCDeckViewUnitSelectList.IsOpen)
			{
				m_eCurrentSelectListType = NKM_UNIT_TYPE.NUT_INVALID;
				m_NKCDeckViewUnitSelectList.Close(bAnimate: false);
			}
			m_NKCDeckViewSupportList.Open(m_ViewerOptions.lstSupporter, m_ViewerOptions.dIsValidSupport);
			UpdateSupporterUI();
			UpdateUpsideMenu();
		}
	}

	private void CloseSupList()
	{
		if (IsSupportMenu)
		{
			m_NKCDeckViewSupportList.Close();
			SetUIBySupport(bShow: true);
			UpdateUpsideMenu();
		}
	}

	private void UpdateSupporterUI()
	{
		WarfareSupporterListData selectedData = m_NKCDeckViewSupportList.GetSelectedData();
		SetUIBySupport(selectedData != null);
		m_NKCDeckViewSupportList.UpdateSelectUI();
		if (selectedData != null)
		{
			SetTitleActive(isSupporter: true);
			NKCUtil.SetLabelText(m_lbSupporterName, selectedData.commonProfile.nickname);
			NKMUnitData nKMUnitData = new NKMUnitData();
			nKMUnitData.FillDataFromDummy(selectedData.deckData.Ship);
			nKMUnitData.m_UnitID = selectedData.deckData.GetShipUnitId();
			m_NKCDeckViewShip.Open(nKMUnitData, NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode));
			m_NKCDeckViewUnit.OpenDummy(selectedData.deckData.List, selectedData.deckData.LeaderIndex);
			OnDeckUpdate();
		}
	}

	private void OnConfirmSuppoter(long selectedCode)
	{
		m_ViewerOptions.dOnSelectSupporter?.Invoke(selectedCode);
	}

	private void SetUIBySupport(bool bShow)
	{
		NKCUtil.SetGameobjectActive(m_NKCDeckViewUnit, bShow);
		NKCUtil.SetGameobjectActive(m_NKCDeckViewShip, bShow);
		NKCUtil.SetGameobjectActive(m_objNKCDeckViewTitle, bShow);
		NKCUtil.SetGameobjectActive(m_rtDeckOperationPowerRoot, bShow);
		NKCUtil.SetGameobjectActive(m_NKM_DECK_VIEW_OPERATOR, bValue: false);
	}

	private void UpdateOperator(NKMOperator operatorData = null)
	{
		if (NKCOperatorUtil.IsHide())
		{
			return;
		}
		if (!NKCOperatorUtil.IsActive())
		{
			NKCUtil.SetGameobjectActive(m_NKM_DECK_VIEW_OPERATOR, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_NKM_DECK_VIEW_OPERATOR, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_DECK_SLOT, operatorData != null);
		NKCUtil.SetGameobjectActive(m_EMPTY, operatorData == null);
		bool flag = NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode);
		m_NKM_UI_OPERATOR_DECK_SLOT.SetData(operatorData, flag);
		NKCUtil.SetGameobjectActive(m_OperatorSkillInfo, operatorData != null);
		NKCUtil.SetGameobjectActive(m_OperatorSkillCombo, operatorData != null);
		if (operatorData != null)
		{
			m_OperatorMainSkill.SetData(operatorData.mainSkill.id, operatorData.mainSkill.level, NKCBanManager.IsBanOperator(operatorData.id) && flag);
			m_OperatorSubSkill.SetData(operatorData.subSkill.id, operatorData.subSkill.level, NKCBanManager.IsBanOperator(operatorData.id) && flag);
			m_OperatorSkillCombo.SetData(operatorData.id);
		}
		if (NeedShowChangedTag())
		{
			NKCUtil.SetGameobjectActive(m_objOperatorChanged, NKCTournamentManager.IsOperatorChanged(operatorData));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objOperatorChanged, bValue: false);
		}
		OnDeckUpdate();
	}

	private void OnSelectOperator(long operatorUID)
	{
		m_SelectUnitSlotIndex = -1;
		m_NKCDeckViewShip.SetSelectEffect(value: false);
		m_NKM_UI_OPERATOR_DECK_SLOT.SetSelectEffect(bActive: true);
		GetCurrDeckViewUnit()?.SelectDeckViewUnit(m_SelectUnitSlotIndex);
		if (operatorUID != 0L && !NKCOperatorUtil.IsMyOperator(operatorUID))
		{
			return;
		}
		NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(operatorUID);
		if (operatorData == null)
		{
			OpenDeckSelectList(NKM_UNIT_TYPE.NUT_OPERATOR, 0L);
		}
		else
		{
			UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_OPERATOR, operatorData?.uid ?? 0);
		}
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareRaid)
		{
			if (m_NKCUIRaidRightSide != null)
			{
				m_NKCUIRaidRightSide.Close();
			}
			m_NKCDeckViewSide.Open(m_ViewerOptions, bInit: false, bUseCost: false);
		}
		else if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.GuildCoopBoss)
		{
			if (m_NKCUIGuildCoopRaidRightSide != null)
			{
				m_NKCUIGuildCoopRaidRightSide.Close();
			}
			m_NKCDeckViewSide.Open(m_ViewerOptions, bInit: false, bUseCost: false);
		}
		m_NKCDeckViewSide.SetOperatorData(operatorData, bForce: true);
		m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetOperatorData(operatorData, IsUnitChangePossible(), bForce: true);
		EnableUnitView(bEnable: true);
	}

	private void ClearDeck()
	{
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			List<bool> list = NKCLocalDeckDataManager.ClearDeck(m_SelectDeckIndex.m_iIndex);
			SetShipSlotData();
			UpdateOperator();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				SetUnitSlotData(m_SelectDeckIndex, i, list[i]);
			}
			SelectDeckViewUnit(m_SelectUnitSlotIndex, bForce: true);
			UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL, 0L);
			NKCLocalDeckDataManager.SetLeaderIndex(m_SelectDeckIndex.m_iIndex, -1);
			SetLeader(m_SelectDeckIndex, -1, bEffect: false);
			UpdateDeckReadyState();
			m_NKCDeckViewList.UpdateDeckState();
			OnDeckUpdate();
			m_ViewerOptions.dOnChangeDeckUnit?.Invoke(m_SelectDeckIndex, 0L);
			return;
		}
		NKMDeckData deckData = NKMArmyData.GetDeckData(m_SelectDeckIndex);
		if (deckData != null)
		{
			List<long> list2 = new List<long>();
			for (int j = 0; j < deckData.m_listDeckUnitUID.Count; j++)
			{
				list2.Add(0L);
			}
			Send_Packet_DECK_UNIT_AUTO_SET_REQ(m_SelectDeckIndex, list2, 0L, 0L);
			m_ViewerOptions.dOnChangeDeckUnit?.Invoke(m_SelectDeckIndex, 0L);
		}
	}

	private void AutoCompleteDeck()
	{
		NKMUserData userData = NKCScenManager.CurrentUserData();
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			List<long> localUnitDeckData = NKCLocalDeckDataManager.GetLocalUnitDeckData(m_SelectDeckIndex.m_iIndex);
			List<long> unitList = NKCUnitSortSystem.MakeLocalAutoCompleteDeck(userData, m_SelectDeckIndex, localUnitDeckData, prohibitSameUnitId: true);
			List<bool> list = NKCLocalDeckDataManager.SetLocalAutoDeckUnitUId(m_SelectDeckIndex.m_iIndex, unitList);
			long shipUId = NKCLocalDeckDataManager.GetShipUId(m_SelectDeckIndex.m_iIndex);
			long shipUId2 = NKCUnitSortSystem.LocalAutoSelectShip(userData, m_SelectDeckIndex, shipUId, prohibitSameUnitId: true);
			NKCLocalDeckDataManager.SetLocalAutoDeckShipUId(m_SelectDeckIndex.m_iIndex, shipUId2);
			SetShipSlotData();
			long operatorUId = NKCLocalDeckDataManager.GetOperatorUId(m_SelectDeckIndex.m_iIndex);
			long num = NKCUnitSortSystem.LocalAutoSelectOperator(userData, m_SelectDeckIndex, operatorUId, prohibitSameUnitId: true);
			NKCLocalDeckDataManager.SetLocalAutoDeckOperatorUId(m_SelectDeckIndex.m_iIndex, num);
			NKMOperator operatorFromUId = NKMArmyData.GetOperatorFromUId(num);
			UpdateOperator(operatorFromUId);
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				SetUnitSlotData(m_SelectDeckIndex, i, list[i]);
			}
			SelectDeckViewUnit(m_SelectUnitSlotIndex, bForce: true);
			UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL, 0L);
			int localLeaderIndex = NKCLocalDeckDataManager.GetLocalLeaderIndex(m_SelectDeckIndex.m_iIndex);
			localLeaderIndex = NKCLocalDeckDataManager.SetLeaderIndex(m_SelectDeckIndex.m_iIndex, (localLeaderIndex >= 0) ? localLeaderIndex : 0);
			SetLeader(m_SelectDeckIndex, localLeaderIndex, bEffect: false);
			UpdateDeckReadyState();
			m_NKCDeckViewList.UpdateDeckState();
			OnDeckUpdate();
			return;
		}
		NKMDeckData deckData = NKMArmyData.GetDeckData(m_SelectDeckIndex);
		if (deckData == null)
		{
			return;
		}
		bool bUseBanData = NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode);
		bool bUseUpData = NKCUtil.CheckPossibleShowUpUnit(m_ViewerOptions.eDeckviewerMode);
		List<long> unitUIDList = NKCUnitSortSystem.MakeAutoCompleteDeck(userData, m_SelectDeckIndex, deckData, bUseBanData, bUseUpData);
		long shipUID = 0L;
		if (deckData.m_ShipUID == 0L)
		{
			NKCUnitSortSystem.UnitListOptions options = new NKCUnitSortSystem.UnitListOptions
			{
				eDeckType = m_SelectDeckIndex.m_eDeckType,
				setExcludeUnitID = null,
				setOnlyIncludeUnitID = null,
				setDuplicateUnitID = null,
				setExcludeUnitUID = null,
				bExcludeLockedUnit = false,
				bExcludeDeckedUnit = false,
				setFilterOption = null,
				lstSortOption = new List<NKCUnitSortSystem.eSortOption>
				{
					NKCUnitSortSystem.eSortOption.Power_High,
					NKCUnitSortSystem.eSortOption.UID_First
				},
				bDescending = true,
				bHideDeckedUnit = !NKMArmyData.IsAllowedSameUnitInMultipleDeck(m_SelectDeckIndex.m_eDeckType),
				bPushBackUnselectable = true,
				bIncludeUndeckableUnit = false
			};
			if (m_SelectDeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_TOURNAMENT)
			{
				options.setExcludeUnitID = NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_NORMAL);
				options.setExcludeUnitBaseID = NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_SHIP);
			}
			options.MakeDuplicateUnitSet(m_SelectDeckIndex, 0L, NKMArmyData);
			NKMUnitData nKMUnitData = new NKCShipSort(userData, options).AutoSelect(null);
			if (nKMUnitData != null)
			{
				shipUID = nKMUnitData.m_UnitUID;
			}
		}
		else
		{
			shipUID = deckData.m_ShipUID;
		}
		long operatorUID = 0L;
		if (deckData.m_OperatorUID == 0L)
		{
			NKCOperatorSortSystem.OperatorListOptions options2 = new NKCOperatorSortSystem.OperatorListOptions
			{
				eDeckType = m_SelectDeckIndex.m_eDeckType,
				setExcludeOperatorID = null,
				setOnlyIncludeOperatorID = null,
				setDuplicateOperatorID = null,
				setExcludeOperatorUID = null,
				setFilterOption = null,
				lstSortOption = new List<NKCOperatorSortSystem.eSortOption>
				{
					NKCOperatorSortSystem.eSortOption.Power_High,
					NKCOperatorSortSystem.eSortOption.UID_First
				}
			};
			options2.SetBuildOption(true, BUILD_OPTIONS.DESCENDING, BUILD_OPTIONS.PUSHBACK_UNSELECTABLE);
			options2.MakeDuplicateSetFromAllDeck(m_SelectDeckIndex, 0L, NKMArmyData);
			NKMOperator nKMOperator = new NKCOperatorSort(userData, options2).AutoSelect(null);
			if (nKMOperator != null)
			{
				operatorUID = nKMOperator.uid;
			}
		}
		else
		{
			operatorUID = deckData.m_OperatorUID;
		}
		Send_Packet_DECK_UNIT_AUTO_SET_REQ(m_SelectDeckIndex, unitUIDList, shipUID, operatorUID);
	}

	public void OnRecv(NKMPacket_DECK_UNLOCK_ACK cPacket)
	{
		NKMDeckIndex index = new NKMDeckIndex(cPacket.deckType, cPacket.unlockedDeckSize - 1);
		SelectDeck(index);
		SetEnableUnlockEffect(m_NKCDeckViewList.GetDeckListButton(index.m_iIndex));
		UpdateDeckReadyState();
		m_NKCDeckViewList.UpdateDeckState();
		OnDeckUpdate();
	}

	public void OnRecv(NKMPacket_DECK_UNIT_SET_ACK cPacket, bool bWasEmptySlot)
	{
		SetUnitSlotData(cPacket.deckIndex, cPacket.slotIndex, bEffect: true);
		SetUnitSlotData(cPacket.oldDeckIndex, cPacket.oldSlotIndex, bEffect: false);
		int num = -1;
		if (bWasEmptySlot)
		{
			num = FindNextEmptySlot(cPacket.slotIndex);
		}
		if (num >= 0)
		{
			SelectDeckViewUnit(num, bForce: true);
			UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL, 0L);
		}
		else
		{
			SelectDeckViewUnit(cPacket.slotIndex, bForce: true);
			UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL, cPacket.slotUnitUID);
		}
		SetLeader(cPacket.deckIndex, cPacket.leaderSlotIndex, bEffect: false);
		UpdateDeckReadyState();
		m_NKCDeckViewList.UpdateDeckState();
		OnDeckUpdate();
		if (cPacket.slotUnitUID != 0L)
		{
			NKMUnitData deckUnitByIndex = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckUnitByIndex(cPacket.deckIndex, cPacket.slotIndex);
			if (deckUnitByIndex != null)
			{
				NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_SQUAD_ENTER, deckUnitByIndex);
			}
		}
	}

	private int FindNextEmptySlot(int currentIndex)
	{
		NKMDeckData deckData = NKMArmyData.GetDeckData(m_SelectDeckIndex);
		if (deckData == null)
		{
			return -1;
		}
		for (int i = 1; i < deckData.m_listDeckUnitUID.Count; i++)
		{
			int num = currentIndex + i;
			if (num >= deckData.m_listDeckUnitUID.Count)
			{
				num -= deckData.m_listDeckUnitUID.Count;
			}
			if (deckData.m_listDeckUnitUID[num] == 0L)
			{
				return num;
			}
		}
		return -1;
	}

	public void OnRecv(NKMPacket_DECK_SHIP_SET_ACK cPacket)
	{
		SetShipSlotData();
		SetDeckViewShipUnit(cPacket.shipUID);
		UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_SHIP, cPacket.shipUID);
		UpdateDeckReadyState();
		m_NKCDeckViewList.UpdateDeckState();
		OnDeckUpdate();
	}

	public void OnRecv(NKMPacket_DECK_UNIT_SWAP_ACK cPacket_DECK_UNIT_SWAP_ACK)
	{
		SetUnitSlotData(cPacket_DECK_UNIT_SWAP_ACK.deckIndex, cPacket_DECK_UNIT_SWAP_ACK.slotIndexFrom, bEffect: true);
		SetUnitSlotData(cPacket_DECK_UNIT_SWAP_ACK.deckIndex, cPacket_DECK_UNIT_SWAP_ACK.slotIndexTo, bEffect: true);
		SelectDeckViewUnit(cPacket_DECK_UNIT_SWAP_ACK.slotIndexTo);
		NKCSoundManager.PlaySound("FX_UI_DECK_UNIIT_SELECT", 1f, 0f, 0f);
		if (cPacket_DECK_UNIT_SWAP_ACK.leaderSlotIndex != -1)
		{
			SetLeader(cPacket_DECK_UNIT_SWAP_ACK.deckIndex, cPacket_DECK_UNIT_SWAP_ACK.leaderSlotIndex, bEffect: false);
		}
	}

	public void OnRecv(NKMPacket_DECK_UNIT_SET_LEADER_ACK cPacket_DECK_UNIT_SET_LEADER_ACK)
	{
		SetLeader(cPacket_DECK_UNIT_SET_LEADER_ACK.deckIndex, cPacket_DECK_UNIT_SET_LEADER_ACK.leaderSlotIndex, bEffect: true);
		NKMUnitData deckUnitByIndex = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckUnitByIndex(cPacket_DECK_UNIT_SET_LEADER_ACK.deckIndex, cPacket_DECK_UNIT_SET_LEADER_ACK.leaderSlotIndex);
		if (deckUnitByIndex != null)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_SQUAD_LEADER, deckUnitByIndex);
		}
	}

	public void OnRecv(NKMPacket_DECK_UNIT_AUTO_SET_ACK sPacket, HashSet<int> setChangedIndex)
	{
		SetShipSlotData();
		int num = sPacket.deckData.m_listDeckUnitUID.Count;
		for (int i = 0; i < sPacket.deckData.m_listDeckUnitUID.Count; i++)
		{
			bool flag = setChangedIndex?.Contains(i) ?? false;
			SetUnitSlotData(sPacket.deckIndex, i, flag);
			if (flag && num > i)
			{
				num = i;
			}
		}
		SelectDeckViewUnit(m_SelectUnitSlotIndex, bForce: true);
		if (m_SelectUnitSlotIndex != -1)
		{
			long selectedUnitUID = ((m_SelectUnitSlotIndex < sPacket.deckData.m_listDeckUnitUID.Count) ? sPacket.deckData.m_listDeckUnitUID[m_SelectUnitSlotIndex] : 0);
			UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL, selectedUnitUID);
		}
		UpdateOperator(NKCOperatorUtil.GetOperatorData(sPacket.deckData.m_OperatorUID));
		SetLeader(sPacket.deckIndex, sPacket.deckData.m_LeaderIndex, bEffect: false);
		UpdateDeckReadyState();
		m_NKCDeckViewList.UpdateDeckState();
		OnDeckUpdate();
		if (setChangedIndex.Count > 0)
		{
			m_ViewerOptions.dOnChangeDeckUnit?.Invoke(sPacket.deckIndex, 0L);
			NKMUnitData deckUnitByIndex = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckUnitByIndex(sPacket.deckIndex, num);
			if (deckUnitByIndex != null)
			{
				NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_SQUAD_ENTER, deckUnitByIndex);
			}
		}
	}

	public void OnRecv(NKMPacket_DECK_OPERATOR_SET_ACK sPacket)
	{
		if (m_SelectDeckIndex == sPacket.deckIndex)
		{
			OnSelectOperator(sPacket.operatorUid);
			UpdateOperator(NKCOperatorUtil.GetOperatorData(sPacket.operatorUid));
			UpdateDeckSelectList(NKM_UNIT_TYPE.NUT_OPERATOR, sPacket.operatorUid);
			if (sPacket.operatorUid == 0L)
			{
				m_NKCDeckViewSide.GetDeckViewSideUnitIllust().SetUnitData(null, bLeader: false, IsUnitChangePossible());
			}
			else
			{
				NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_SQUAD_ENTER, NKCOperatorUtil.GetOperatorData(sPacket.operatorUid));
			}
		}
	}

	private void Send_NKMPacket_DECK_UNIT_SWAP_REQ(NKMDeckIndex deckIndex, int slotFrom, int slotTo)
	{
		if (m_ViewerOptions.bUseAsyncDeckSetting)
		{
			AsyncDeckUnitSwap(deckIndex, slotFrom, slotTo);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_DECK_UNIT_SWAP_REQ(deckIndex, slotFrom, slotTo);
		}
	}

	public void Send_NKMPacket_DECK_UNIT_SET_REQ(NKMDeckIndex deckIndex, int slotIndex, long unitUID)
	{
		if (m_ViewerOptions.bUseAsyncDeckSetting)
		{
			AsyncDeckUnitSet(deckIndex, slotIndex, unitUID);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_DECK_UNIT_SET_REQ(deckIndex, slotIndex, unitUID);
		}
	}

	public void Send_NKMPacket_DECK_SHIP_SET_REQ(NKMDeckIndex deckIndex, long shipUID)
	{
		if (m_ViewerOptions.bUseAsyncDeckSetting)
		{
			AsyncDeckShipSet(deckIndex, shipUID);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_DECK_SHIP_SET_REQ(deckIndex, shipUID);
		}
	}

	public void Send_Packet_DECK_UNIT_SET_LEADER_REQ(NKMDeckIndex deckIndex, sbyte leaderIndex)
	{
		if (m_ViewerOptions.bUseAsyncDeckSetting)
		{
			AsyncDeckUnitSetLeader(deckIndex, leaderIndex);
		}
		else
		{
			NKCPacketSender.Send_Packet_DECK_UNIT_SET_LEADER_REQ(deckIndex, leaderIndex);
		}
	}

	public void Send_Packet_DECK_UNIT_AUTO_SET_REQ(NKMDeckIndex deckIndex, List<long> unitUIDList, long shipUID, long operatorUID)
	{
		if (m_ViewerOptions.bUseAsyncDeckSetting)
		{
			AsyncDeckUnitAutoSet(deckIndex, unitUIDList, shipUID, operatorUID);
		}
		else
		{
			NKCPacketSender.Send_Packet_DECK_UNIT_AUTO_SET_REQ(deckIndex, unitUIDList, shipUID, operatorUID);
		}
	}

	public void Send_NKMPacket_DECK_OPERATOR_SET_REQ(NKMDeckIndex deckIndex, long operatorUID)
	{
		if (m_ViewerOptions.bUseAsyncDeckSetting)
		{
			AsyncDeckOperatorSet(deckIndex, operatorUID);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_DECK_OPERATOR_SET_REQ(deckIndex, operatorUID);
		}
	}

	private void AsyncDeckUnitSwap(NKMDeckIndex deckIndex, int slotFrom, int slotTo)
	{
		NKMDeckData deckData = NKMArmyData.GetDeckData(deckIndex);
		if (deckData == null)
		{
			Debug.LogError($"deckData가 없음.. 말이됨? - {deckIndex.m_eDeckType.ToString()}, {deckIndex.m_iIndex}");
			return;
		}
		if (deckData.m_LeaderIndex == slotFrom)
		{
			deckData.m_LeaderIndex = (sbyte)slotTo;
		}
		else if (deckData.m_LeaderIndex == slotTo)
		{
			deckData.m_LeaderIndex = (sbyte)slotFrom;
		}
		long num = 0L;
		NKMUnitData deckUnitByIndex = NKMArmyData.GetDeckUnitByIndex(deckIndex, slotFrom);
		if (deckUnitByIndex != null)
		{
			num = deckUnitByIndex.m_UnitUID;
		}
		long num2 = 0L;
		NKMUnitData deckUnitByIndex2 = NKMArmyData.GetDeckUnitByIndex(deckIndex, slotTo);
		if (deckUnitByIndex2 != null)
		{
			num2 = deckUnitByIndex2.m_UnitUID;
		}
		deckData.m_listDeckUnitUID[slotFrom] = num2;
		deckData.m_listDeckUnitUID[slotTo] = num;
		NKMPacket_DECK_UNIT_SWAP_ACK nKMPacket_DECK_UNIT_SWAP_ACK = new NKMPacket_DECK_UNIT_SWAP_ACK();
		nKMPacket_DECK_UNIT_SWAP_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
		nKMPacket_DECK_UNIT_SWAP_ACK.deckIndex = deckIndex;
		nKMPacket_DECK_UNIT_SWAP_ACK.leaderSlotIndex = deckData.m_LeaderIndex;
		nKMPacket_DECK_UNIT_SWAP_ACK.slotIndexFrom = (byte)slotFrom;
		nKMPacket_DECK_UNIT_SWAP_ACK.slotIndexTo = (byte)slotTo;
		nKMPacket_DECK_UNIT_SWAP_ACK.slotUnitUIDFrom = num2;
		nKMPacket_DECK_UNIT_SWAP_ACK.slotUnitUIDTo = num;
		OnRecv(nKMPacket_DECK_UNIT_SWAP_ACK);
	}

	private void AsyncDeckUnitSet(NKMDeckIndex deckIndex, int slotIndex, long unitUID)
	{
		long num = 0L;
		NKMUnitData deckUnitByIndex = NKMArmyData.GetDeckUnitByIndex(deckIndex, slotIndex);
		if (deckUnitByIndex != null)
		{
			num = deckUnitByIndex.m_UnitUID;
		}
		NKMArmyData.SetDeckUnitByIndex(deckIndex, (byte)slotIndex, unitUID);
		NKMDeckData deckData = NKMArmyData.GetDeckData(deckIndex);
		if (deckData == null)
		{
			Debug.LogError($"deckData가 없음.. 말이됨? - {deckIndex.m_eDeckType.ToString()}, {deckIndex.m_iIndex}");
			return;
		}
		if (deckData.m_LeaderIndex == slotIndex && unitUID == 0L)
		{
			int num2 = deckData.m_listDeckUnitUID.FindIndex((long v) => v > 0);
			deckData.m_LeaderIndex = (sbyte)num2;
		}
		else if (deckData.m_LeaderIndex == -1)
		{
			deckData.m_LeaderIndex = (sbyte)slotIndex;
		}
		NKMPacket_DECK_UNIT_SET_ACK nKMPacket_DECK_UNIT_SET_ACK = new NKMPacket_DECK_UNIT_SET_ACK();
		nKMPacket_DECK_UNIT_SET_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
		nKMPacket_DECK_UNIT_SET_ACK.deckIndex = deckIndex;
		nKMPacket_DECK_UNIT_SET_ACK.slotIndex = (byte)slotIndex;
		nKMPacket_DECK_UNIT_SET_ACK.slotUnitUID = unitUID;
		nKMPacket_DECK_UNIT_SET_ACK.oldDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE, 0);
		nKMPacket_DECK_UNIT_SET_ACK.leaderSlotIndex = deckData.m_LeaderIndex;
		OnRecv(nKMPacket_DECK_UNIT_SET_ACK, deckUnitByIndex == null);
		if (NKCUIUnitSelectList.IsInstanceOpen)
		{
			if (num != 0L)
			{
				NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(num, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE));
			}
			NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(unitUID, deckIndex);
		}
	}

	private void AsyncDeckShipSet(NKMDeckIndex deckIndex, long shipUID)
	{
		long num = 0L;
		NKMUnitData deckShip = NKMArmyData.GetDeckShip(deckIndex);
		if (deckShip != null)
		{
			num = deckShip.m_UnitUID;
		}
		NKMDeckData deckData = NKMArmyData.GetDeckData(deckIndex);
		if (deckData == null)
		{
			Debug.LogError($"deckData가 없음.. 말이됨? - {deckIndex.m_eDeckType.ToString()}, {deckIndex.m_iIndex}");
			return;
		}
		deckData.m_ShipUID = shipUID;
		NKMPacket_DECK_SHIP_SET_ACK nKMPacket_DECK_SHIP_SET_ACK = new NKMPacket_DECK_SHIP_SET_ACK();
		nKMPacket_DECK_SHIP_SET_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
		nKMPacket_DECK_SHIP_SET_ACK.deckIndex = deckIndex;
		nKMPacket_DECK_SHIP_SET_ACK.shipUID = shipUID;
		nKMPacket_DECK_SHIP_SET_ACK.oldDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE, 0);
		OnRecv(nKMPacket_DECK_SHIP_SET_ACK);
		if (NKCUIUnitSelectList.IsInstanceOpen)
		{
			if (num != 0L)
			{
				NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(num, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE));
			}
			NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(shipUID, deckIndex);
		}
	}

	private void AsyncDeckUnitSetLeader(NKMDeckIndex deckIndex, sbyte leaderIndex)
	{
		NKMDeckData deckData = NKMArmyData.GetDeckData(deckIndex);
		if (deckData == null)
		{
			Debug.LogError($"deckData가 없음.. 말이됨? - {deckIndex.m_eDeckType.ToString()}, {deckIndex.m_iIndex}");
			return;
		}
		deckData.m_LeaderIndex = leaderIndex;
		NKMPacket_DECK_UNIT_SET_LEADER_ACK nKMPacket_DECK_UNIT_SET_LEADER_ACK = new NKMPacket_DECK_UNIT_SET_LEADER_ACK();
		nKMPacket_DECK_UNIT_SET_LEADER_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
		nKMPacket_DECK_UNIT_SET_LEADER_ACK.deckIndex = deckIndex;
		nKMPacket_DECK_UNIT_SET_LEADER_ACK.leaderSlotIndex = leaderIndex;
		OnRecv(nKMPacket_DECK_UNIT_SET_LEADER_ACK);
	}

	private void AsyncDeckUnitAutoSet(NKMDeckIndex deckIndex, List<long> unitUIDList, long shipUID, long operatorUID)
	{
		if (unitUIDList == null)
		{
			return;
		}
		NKMDeckData deckData = NKMArmyData.GetDeckData(deckIndex);
		if (deckData == null)
		{
			Debug.LogError($"deckData가 없음.. 말이됨? - {deckIndex.m_eDeckType.ToString()}, {deckIndex.m_iIndex}");
			return;
		}
		int num = -1;
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < deckData.m_listDeckUnitUID.Count; i++)
		{
			if (i < unitUIDList.Count)
			{
				if (unitUIDList[i] != deckData.m_listDeckUnitUID[i])
				{
					hashSet.Add(i);
					deckData.m_listDeckUnitUID[i] = unitUIDList[i];
				}
				if (unitUIDList[i] > 0 && num == -1)
				{
					num = i;
				}
			}
		}
		deckData.m_ShipUID = shipUID;
		deckData.m_OperatorUID = operatorUID;
		if (num == -1)
		{
			deckData.m_LeaderIndex = -1;
		}
		else if (deckData.m_LeaderIndex == -1)
		{
			deckData.m_LeaderIndex = 0;
		}
		NKMPacket_DECK_UNIT_AUTO_SET_ACK nKMPacket_DECK_UNIT_AUTO_SET_ACK = new NKMPacket_DECK_UNIT_AUTO_SET_ACK();
		nKMPacket_DECK_UNIT_AUTO_SET_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
		nKMPacket_DECK_UNIT_AUTO_SET_ACK.deckIndex = deckIndex;
		nKMPacket_DECK_UNIT_AUTO_SET_ACK.deckData = deckData;
		OnRecv(nKMPacket_DECK_UNIT_AUTO_SET_ACK, hashSet);
	}

	private void AsyncDeckOperatorSet(NKMDeckIndex deckIndex, long unitUID)
	{
		long num = 0L;
		NKMOperator deckOperator = NKMArmyData.GetDeckOperator(deckIndex);
		if (deckOperator != null)
		{
			num = deckOperator.uid;
		}
		NKMArmyData.SetDeckOperatorByIndex(deckIndex.m_eDeckType, deckIndex.m_iIndex, unitUID);
		if (NKMArmyData.GetDeckData(deckIndex) == null)
		{
			Debug.LogError($"deckData가 없음.. 말이됨? - {deckIndex.m_eDeckType.ToString()}, {deckIndex.m_iIndex}");
			return;
		}
		NKMPacket_DECK_OPERATOR_SET_ACK nKMPacket_DECK_OPERATOR_SET_ACK = new NKMPacket_DECK_OPERATOR_SET_ACK();
		nKMPacket_DECK_OPERATOR_SET_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
		nKMPacket_DECK_OPERATOR_SET_ACK.deckIndex = deckIndex;
		nKMPacket_DECK_OPERATOR_SET_ACK.operatorUid = unitUID;
		nKMPacket_DECK_OPERATOR_SET_ACK.deckIndex = deckIndex;
		OnRecv(nKMPacket_DECK_OPERATOR_SET_ACK);
		if (NKCUIUnitSelectList.IsInstanceOpen)
		{
			if (num != 0L)
			{
				NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(num, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE));
			}
			NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(unitUID, deckIndex);
		}
	}

	public void OnRemoveUnit(NKMUnitData UnitData)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY)
		{
			NKCUIUnitSelectList.CheckInstanceAndClose();
		}
		Send_NKMPacket_DECK_UNIT_SET_REQ(m_SelectDeckIndex, m_SelectUnitSlotIndex, 0L);
	}

	public void OnLeaderChange(NKMUnitData UnitData)
	{
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			int leaderIndex = NKCLocalDeckDataManager.SetLeaderIndex(m_SelectDeckIndex.m_iIndex, m_SelectUnitSlotIndex);
			SetLeader(m_SelectDeckIndex, leaderIndex, bEffect: true);
			long localUnitData = NKCLocalDeckDataManager.GetLocalUnitData(m_SelectDeckIndex.m_iIndex, m_SelectUnitSlotIndex);
			NKMUnitData nKMUnitData = NKCScenManager.CurrentUserData()?.m_ArmyData.GetUnitFromUID(localUnitData);
			if (nKMUnitData != null)
			{
				NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_SQUAD_LEADER, nKMUnitData);
			}
		}
		else
		{
			Send_Packet_DECK_UNIT_SET_LEADER_REQ(m_SelectDeckIndex, (sbyte)m_SelectUnitSlotIndex);
		}
	}

	private void OnBtnEnemyList()
	{
		if (IsDungeonAtkReadyScen(m_ViewerOptions.eDeckviewerMode))
		{
			NKC_SCEN_DUNGEON_ATK_READY sCEN_DUNGEON_ATK_READY = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY();
			NKMStageTempletV2 stageTemplet = sCEN_DUNGEON_ATK_READY.GetStageTemplet();
			NKMDungeonTempletBase dungeonTempletBase = sCEN_DUNGEON_ATK_READY.GetDungeonTempletBase();
			if (stageTemplet != null)
			{
				NKCPopupEnemyList.Instance.Open(stageTemplet);
			}
			else if (dungeonTempletBase != null)
			{
				NKCPopupEnemyList.Instance.Open(dungeonTempletBase);
			}
		}
		else
		{
			DeckViewerMode eDeckviewerMode = m_ViewerOptions.eDeckviewerMode;
			if ((uint)(eDeckviewerMode - 8) <= 1u)
			{
				NKMWarfareTemplet cNKMWarfareTemplet = NKMWarfareTemplet.Find(NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareStrID());
				NKCPopupEnemyList.Instance.Open(cNKMWarfareTemplet);
			}
		}
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		NKMUnitData unitData2 = eUnitType switch
		{
			NKM_UNIT_TYPE.NUT_NORMAL => NKMArmyData.GetUnitFromUID(uid), 
			NKM_UNIT_TYPE.NUT_SHIP => NKMArmyData.GetShipFromUID(uid), 
			_ => null, 
		};
		switch (eEventType)
		{
		case NKMUserData.eChangeNotifyType.Update:
			m_ViewerOptions.DeckIndex = m_SelectDeckIndex;
			GetCurrDeckViewUnit()?.UpdateUnit(unitData2, m_ViewerOptions);
			m_NKCDeckViewSide.UpdateUnitData(unitData);
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().UpdateUnit(unitData2);
			m_NKCDeckViewUnitSelectList.UpdateSlot(uid, unitData);
			if (eUnitType == NKM_UNIT_TYPE.NUT_SHIP)
			{
				m_NKCDeckViewShip.UpdateShipSlotData(unitData, NKCUtil.CheckPossibleShowBan(m_ViewerOptions.eDeckviewerMode));
			}
			else if (NeedShowChangedTag())
			{
				m_NKCDeckViewSide.CheckUnitChanged(NKCTournamentManager.GetAppliedAsyncUnitData(uid));
			}
			break;
		case NKMUserData.eChangeNotifyType.Add:
		case NKMUserData.eChangeNotifyType.Remove:
			m_NKCDeckViewUnitSelectList.InvalidateSortData(eUnitType);
			break;
		}
		OnDeckUpdate();
	}

	public override void OnOperatorUpdate(NKMUserData.eChangeNotifyType eEventType, long uid, NKMOperator operatorData)
	{
		switch (eEventType)
		{
		case NKMUserData.eChangeNotifyType.Update:
			UpdateOperator(operatorData);
			m_NKCDeckViewSide.SetOperatorData(operatorData, bForce: true);
			m_NKCDeckViewSide.GetDeckViewSideUnitIllust().UpdateOperator(operatorData);
			m_NKM_UI_OPERATOR_DECK_SLOT.UpdateData(operatorData);
			m_NKCDeckViewUnitSelectList.UpdateSlot(uid, operatorData);
			break;
		case NKMUserData.eChangeNotifyType.Add:
		case NKMUserData.eChangeNotifyType.Remove:
			m_NKCDeckViewUnitSelectList.InvalidateSortData(NKM_UNIT_TYPE.NUT_OPERATOR);
			break;
		}
		OnDeckUpdate();
	}

	public override void OnDeckUpdate(NKMDeckIndex deckIndex, NKMDeckData deckData)
	{
		m_NKCDeckViewList.UpdateDeckState();
		SelectCurrentDeck();
	}

	private void OnBtnDeckTypeGuide()
	{
		NKCUIPopUpGuide.Instance.Open("ARTICLE_SYSTEM_TEAM_SETTING");
	}

	private void UpdateDeckToggleUI()
	{
		NKCUtil.SetGameobjectActive(m_objDeckTypeTrim, bValue: false);
		if (m_ViewerOptions.eDeckviewerMode == DeckViewerMode.PrepareLocalDeck)
		{
			NKCUtil.SetGameobjectActive(m_objDeckType, bValue: false);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_TRIM)
			{
				NKCUtil.SetGameobjectActive(m_objDeckTypeTrim, bValue: true);
				if (NKCUIPopupTrimDungeon.IsInstanceOpen)
				{
					NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(NKCUIPopupTrimDungeon.Instance.TrimId);
					NKMTrimPointTemplet nKMTrimPointTemplet = NKMTrimPointTemplet.Find(NKCUIPopupTrimDungeon.Instance.SelectedGroup, NKCUIPopupTrimDungeon.Instance.SelectedLevel);
					string msg = ((nKMTrimTemplet != null) ? NKCStringTable.GetString(nKMTrimTemplet.TirmGroupName) : " - ");
					int num = nKMTrimPointTemplet?.RecommendCombatPoint ?? 0;
					NKCUtil.SetLabelText(m_lbTrimName, msg);
					NKCUtil.SetLabelText(m_lbTrimLevel, NKCUIPopupTrimDungeon.Instance.SelectedLevel.ToString());
					NKCUtil.SetLabelText(m_lbRecommendedPower, num.ToString());
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objDeckType, bValue: false);
		}
	}

	private void OnChangeDeckName()
	{
		if (m_ifDeckName != null && !m_ifDeckName.isFocused)
		{
			m_ifDeckName.Select();
			m_ifDeckName.ActivateInputField();
		}
	}

	private void OnDeckNameValueChanged(string text)
	{
		m_ifDeckName.text = text.Replace("\r", "").Replace("\n", "");
	}

	private void OnEndEditDeckName(string text)
	{
		NKCPacketSender.Send_NKMPacket_DECK_NAME_UPDATE_REQ(m_SelectDeckIndex, text);
	}

	private void SetDeckNameInput(bool bEnable, string name = "")
	{
		if (m_ifDeckName != null)
		{
			m_ifDeckName.SetTextWithoutNotify(name);
			m_ifDeckName.enabled = bEnable;
			m_ifDeckName.interactable = bEnable;
		}
		NKCUtil.SetGameobjectActive(m_csbtnDeckName, bEnable);
	}

	public void UpdateDeckName(NKMDeckIndex deckIndex, string name)
	{
		if (m_SelectDeckIndex == deckIndex)
		{
			NKCUtil.SetLabelText(m_lbDeckNamePlaceholder, GetDeckDefaultName(m_SelectDeckIndex));
			if (m_ifDeckName != null)
			{
				m_ifDeckName.SetTextWithoutNotify(name);
			}
		}
		m_NKCDeckViewList.UpdateDeckListButton(NKCScenManager.CurrentUserData().m_ArmyData, deckIndex);
	}

	public void ResetDeckName()
	{
		UpdateDeckName(m_SelectDeckIndex, GetDeckName(m_SelectDeckIndex));
	}

	public static string GetDeckName(NKMDeckIndex deckIndex)
	{
		switch (deckIndex.m_eDeckType)
		{
		case NKM_DECK_TYPE.NDT_NORMAL:
		case NKM_DECK_TYPE.NDT_PVP:
		case NKM_DECK_TYPE.NDT_DAILY:
		case NKM_DECK_TYPE.NDT_RAID:
		case NKM_DECK_TYPE.NDT_DIVE:
		{
			NKMDeckData deckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(deckIndex);
			if (deckData == null || string.IsNullOrEmpty(deckData.m_DeckName))
			{
				return GetDeckDefaultName(deckIndex);
			}
			return deckData.m_DeckName;
		}
		case NKM_DECK_TYPE.NDT_PVP_DEFENCE:
			return NKCStringTable.GetString("SI_PF_GAUNTLET_DEFENSEDECK");
		default:
			return GetDeckDefaultName(deckIndex);
		}
	}

	public static string GetDeckDefaultName(NKMDeckIndex deckIndex)
	{
		if (deckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_PVP_DEFENCE)
		{
			return NKCStringTable.GetString("SI_PF_GAUNTLET_DEFENSEDECK");
		}
		return string.Format(NKCUtilString.GET_STRING_SQUAD_ONE_PARAM, deckIndex.m_iIndex + 1);
	}

	public NKCUIComButton GetShipSelectButton()
	{
		return m_NKCDeckViewShip.m_cbtnShip;
	}

	public NKCUIUnitSelectListSlotBase SetTutorialSelectUnit(NKM_UNIT_TYPE type, int unitID)
	{
		OpenDeckSelectList(type, 0L);
		return m_NKCDeckViewUnitSelectList.GetAndScrollToTargetUnitSlot(unitID);
	}

	public NKCUIUnitSelectListSlotBase GetTutorialSelectSlotType(NKCDeckViewUnitSelectList.SlotType type)
	{
		OpenDeckSelectList(NKM_UNIT_TYPE.NUT_NORMAL, 0L);
		return m_NKCDeckViewUnitSelectList.GetAndScrollSlotBySlotType(type);
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.TeamSetting);
	}

	private void OnClickOperatorEmptySlot()
	{
		OpenDeckSelectList(NKM_UNIT_TYPE.NUT_OPERATOR, 0L);
	}

	private void OnClickDeckCopy()
	{
		NKMDeckData deckData = NKMArmyData.GetDeckData(m_SelectDeckIndex.m_eDeckType, m_SelectDeckIndex.m_iIndex);
		if (deckData == null)
		{
			return;
		}
		List<long> listDeckUnitUID = deckData.m_listDeckUnitUID;
		int shipID = NKMArmyData.GetDeckShip(m_SelectDeckIndex)?.m_UnitID ?? 0;
		int operID = NKMArmyData.GetDeckOperator(m_SelectDeckIndex)?.id ?? 0;
		List<int> list = new List<int>();
		foreach (long item in listDeckUnitUID)
		{
			NKMUnitData unitFromUID = NKMArmyData.GetUnitFromUID(item);
			if (unitFromUID == null)
			{
				list.Add(0);
			}
			else
			{
				list.Add(unitFromUID.m_UnitID);
			}
		}
		int leaderIndex = NKMArmyData.GetDeckData(m_SelectDeckIndex).m_LeaderIndex;
		NKCPopupDeckCopy.MakeDeckCopyCode(shipID, operID, list, leaderIndex);
	}

	private void OnClickDeckPaste()
	{
		NKCPopupDeckCopy.Instance.Open(m_SelectDeckIndex);
	}

	private void OnClickAssistUnit()
	{
		if (!m_bReqSupportUnitList)
		{
			NKCPacketSender.Send_NKMPacket_SUPPORT_UNIT_LIST_REQ();
			m_bReqSupportUnitList = true;
		}
	}

	public void OnRecv(NKMPacket_SUPPORT_UNIT_LIST_ACK sPacket)
	{
		m_bReqSupportUnitList = false;
		NKCUIPopupAssistSelect.Instance.Open(sPacket.supportUnitData, m_SelectDeckIndex, GetSupportUserUID(m_SelectDeckIndex));
	}

	private long GetSupportUserUID(NKMDeckIndex deckIdx)
	{
		foreach (AssistUserData lstAssistUnitData in m_lstAssistUnitDatas)
		{
			if (lstAssistUnitData.deckIndex == deckIdx)
			{
				return lstAssistUnitData.assistUserUID;
			}
		}
		return 0L;
	}

	private void UpdateAssistUnitData(NKMDeckIndex deckIdx, NKMAsyncUnitEquipData unitEquipData, long supportUserUID)
	{
		for (int i = 0; i < m_lstAssistUnitDatas.Count; i++)
		{
			if (m_lstAssistUnitDatas[i].deckIndex == deckIdx)
			{
				m_lstAssistUnitDatas.RemoveAt(i);
				break;
			}
		}
		m_lstAssistUnitDatas.Add(new AssistUserData(deckIdx, unitEquipData, supportUserUID));
	}

	public void OnRecv(NKMPacket_SET_DUNGEON_SUPPORT_UNIT_ACK sPacket)
	{
		if (sPacket.selectUnitData != null)
		{
			UpdateAssistUnitData(m_SelectDeckIndex, sPacket.selectUnitData.asyncUnitEquip, sPacket.selectUnitData.userUid);
			UpdateAssistUnitUI();
		}
	}

	private void UpdateAssistUnitUI()
	{
		foreach (AssistUserData lstAssistUnitData in m_lstAssistUnitDatas)
		{
			if (lstAssistUnitData.deckIndex == m_SelectDeckIndex)
			{
				NKMAsyncUnitData asyncUnit = lstAssistUnitData.unitEquipData.asyncUnit;
				NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(asyncUnit.unitId, asyncUnit.unitLevel, (short)asyncUnit.limitBreakLevel, asyncUnit.tacticLevel, asyncUnit.reactorLevel);
				nKMUnitData.m_SkinID = asyncUnit.skinId;
				m_UnitAssistSlot.SetData(nKMUnitData, m_SelectDeckIndex, bEnableLayoutElement: false, null);
				m_UnitAssistSlot.SetEquipData(NKCUIPopupAssistSelect.GetEquipSetData(lstAssistUnitData.unitEquipData));
				NKCUtil.SetGameobjectActive(m_objUnitAssistEmpty, bValue: false);
				NKCUtil.SetGameobjectActive(m_UnitAssistSlot, bValue: true);
				return;
			}
		}
		NKCUtil.SetGameobjectActive(m_objUnitAssistEmpty, bValue: true);
		NKCUtil.SetGameobjectActive(m_UnitAssistSlot, bValue: false);
		m_UnitAssistSlot.SetEmpty(bEnableLayoutElement: false, null);
	}

	private NKMAsyncUnitData GetSavedTournamentDeckUnit(int idx)
	{
		if (NKCTournamentManager.m_TournamentApplyDeckData.units.Count > idx)
		{
			return NKCTournamentManager.m_TournamentApplyDeckData.units[idx];
		}
		return null;
	}

	private bool NeedShowChangedTag()
	{
		if (m_ViewerOptions.eDeckviewerMode != DeckViewerMode.TournamentApply)
		{
			return false;
		}
		if (NKCTournamentManager.m_TournamentTemplet == null)
		{
			return false;
		}
		if (!NKCTournamentManager.m_TournamentTemplet.CanEnterDeckApply(ServiceTime.Now))
		{
			return false;
		}
		return true;
	}
}
