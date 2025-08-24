using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Unit;
using ClientPacket.User;
using Cs.Logging;
using DG.Tweening;
using NKC.UI.Component;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using NKM.Templet.Recall;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShipInfo : NKCUIBase
{
	private enum State
	{
		INFO,
		REPAIR,
		MODULE
	}

	private enum ShipViewState
	{
		Normal,
		IllustView
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_ship_info";

	private const string UI_ASSET_NAME = "NKM_UI_SHIP_INFO";

	private static NKCUIShipInfo m_Instance;

	[Header("함선 정보")]
	public NKCUIShipInfoRepair m_ShipInfoRepair;

	[Header("지휘 모듈")]
	public NKCUIShipInfoCommandModule m_ShipInfoCommandModule;

	[Header("왼쪽 위 함선 기본정보")]
	public NKCUIShipInfoSummary m_uiSummary;

	public GameObject m_objModuleStepMini;

	public List<GameObject> m_lstModuleStepMini;

	[Header("우측 토글 탭")]
	public NKCUIComToggle m_ctglInfo;

	public NKCUIComToggle m_ctglRepair;

	public GameObject m_objRepairLock;

	public NKCUIComToggle m_ctglModule;

	[Header("우측 오브젝트")]
	public GameObject m_objInfo;

	public GameObject m_objRepair;

	public GameObject m_objModule;

	[Header("오른쪽 함선 정보")]
	public Text m_lbPower;

	public Text m_lbHP;

	public Text m_lbAttack;

	public Text m_lbDefence;

	public Text m_lbCritical;

	public Text m_lbHit;

	public Text m_lbEvade;

	[Header("모듈 정보")]
	public NKCUIShipModule m_ShipModule;

	[Header("스킬")]
	public NKCUIShipInfoSkillPanel m_SkillPanel;

	[Header("UI State 관련")]
	public RectTransform m_rtLeftRect;

	public RectTransform m_rtRightRect;

	public NKCUIRectMove m_rmLock;

	[Header("스파인 일러스트")]
	public ScrollRect m_srScrollRect;

	public RectTransform m_rectSpineIllustPanel;

	public RectTransform m_rectIllustRoot;

	public Vector2 m_vIllustRootAnchorMinNormal;

	public Vector2 m_vIllustRootAnchorMaxNormal;

	public Vector2 m_vIllustRootAnchorMinIllustView;

	public Vector2 m_vIllustRootAnchorMaxIllustView;

	[Header("기타 버튼")]
	public NKCUIComStateButton m_ChangeShip;

	public NKCUIComStateButton m_cbtnChangeIllust;

	public NKCUIComToggle m_ctglLock;

	public NKCUIComToggle m_tglMoveRange;

	public NKCUIShipInfoMoveType m_ShipInfoMoveType;

	public NKCUIComStateButton m_cbtnPractice;

	[Space]
	public NKCUIComStateButton m_GuideBtn;

	public string m_GuideStrID;

	public NKCComStatInfoToolTip m_ToolTipHP;

	public NKCComStatInfoToolTip m_ToolTipATK;

	public NKCComStatInfoToolTip m_ToolTipDEF;

	public NKCComStatInfoToolTip m_ToolTipCritical;

	public NKCComStatInfoToolTip m_ToolTipHit;

	public NKCComStatInfoToolTip m_ToolTipEvade;

	[Space]
	public GameObject m_objSeized;

	[Header("임시빤짝이")]
	public GameObject TempChangeFX;

	public Image m_imgBG;

	[Header("리콜")]
	public GameObject m_objRecall;

	public NKCUIComStateButton m_btnRecall;

	public Text m_lbRecallTime;

	public GameObject m_objModuleUnlockFx;

	private State m_curUIState;

	private ShipViewState m_eShipViewState;

	private NKMUnitData m_curShipData;

	private float m_fDeltaTime;

	private const float MIN_ZOOM_SCALE = 0.5f;

	private const float MAX_ZOOM_SCALE = 2f;

	private int m_CurShipID;

	private long m_CurShipUID;

	private List<NKMShipSkillTemplet> m_lstShipSkills = new List<NKMShipSkillTemplet>();

	private string m_shipName;

	[Header("함선 변경")]
	public NKCUIComDragSelectablePanel m_DragUnitView;

	private Dictionary<int, NKCASUIUnitIllust> m_dicUnitIllust = new Dictionary<int, NKCASUIUnitIllust>();

	private int m_iBannerSlotCnt;

	private List<NKMUnitData> m_lstUnitData = new List<NKMUnitData>();

	private NKCUIUnitSelectList m_UIUnitSelectList;

	public static NKCUIShipInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIShipInfo>("ab_ui_nkm_ui_ship_info", "NKM_UI_SHIP_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIShipInfo>();
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

	public override string GuideTempletID
	{
		get
		{
			if (m_curUIState == State.INFO)
			{
				return "ARTICLE_SHIP_INFO";
			}
			return "ARTICLE_SHIP_LEVELUP";
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName
	{
		get
		{
			if (m_curUIState == State.INFO)
			{
				return NKCUtilString.GET_STRING_SHIP_INFO;
			}
			return NKCUtilString.GET_STRING_HANGAR_SHIPYARD;
		}
	}

	private NKMArmyData NKMArmyData => NKCScenManager.CurrentUserData()?.m_ArmyData;

	private NKCUIUnitSelectList UnitSelectList
	{
		get
		{
			if (m_UIUnitSelectList == null)
			{
				m_UIUnitSelectList = NKCUIUnitSelectList.OpenNewInstance();
			}
			return m_UIUnitSelectList;
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
		NKCUIPopupIllustView.CheckInstanceAndClose();
		if (null != m_UIUnitSelectList && NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_UNIT_LIST)
		{
			UnitSelectList.Close();
		}
		m_UIUnitSelectList = null;
		NKCUtil.SetGameobjectActive(m_objModuleUnlockFx, bValue: false);
		BannerCleanUp();
		base.gameObject.SetActive(value: false);
	}

	public override void OnBackButton()
	{
		if (m_curUIState == State.INFO && m_eShipViewState == ShipViewState.IllustView)
		{
			SetState(ShipViewState.Normal);
		}
		else
		{
			base.OnBackButton();
		}
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		if (eEventType == NKMUserData.eChangeNotifyType.Update && uid == m_curShipData.m_UnitUID)
		{
			if (m_curShipData.m_LimitBreakLevel == 0 && unitData.m_LimitBreakLevel == 1)
			{
				NKCUtil.SetGameobjectActive(m_objModuleUnlockFx, bValue: false);
				NKCUtil.SetGameobjectActive(m_objModuleUnlockFx, bValue: true);
			}
			for (int i = 0; i < m_lstUnitData.Count; i++)
			{
				if (m_lstUnitData[i].m_UnitUID == unitData.m_UnitUID)
				{
					m_lstUnitData[i] = unitData;
					break;
				}
			}
			SetData(unitData);
		}
		else if (eEventType == NKMUserData.eChangeNotifyType.Remove)
		{
			NKMUnitData nKMUnitData = m_lstUnitData.Find((NKMUnitData x) => x.m_UnitUID == uid);
			if (nKMUnitData != null)
			{
				m_lstUnitData.Remove(nKMUnitData);
			}
			SetBannerUnit(m_curShipData.m_UnitUID);
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (m_curUIState == State.REPAIR)
		{
			m_ShipInfoRepair.SetData(m_curShipData);
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		CheckTabLock();
		TutorialCheck();
	}

	public override void Hide()
	{
		base.Hide();
		NKCUtil.SetGameobjectActive(m_objModuleUnlockFx, bValue: false);
		TempChangeFX.SetActive(value: false);
	}

	public void Init()
	{
		NKCUtil.SetBindFunction(m_cbtnChangeIllust, OnClickChangeIllust);
		NKCUtil.SetBindFunction(m_cbtnPractice, OnClickPractice);
		NKCUtil.SetBindFunction(m_GuideBtn, delegate
		{
			NKCUIPopUpGuide.Instance.Open(m_GuideStrID);
		});
		if (m_ctglLock != null)
		{
			m_ctglLock.OnValueChanged.RemoveAllListeners();
			m_ctglLock.OnValueChanged.AddListener(OnLockToggle);
		}
		if ((bool)m_ctglInfo)
		{
			m_ctglInfo.OnValueChanged.RemoveAllListeners();
			m_ctglInfo.OnValueChanged.AddListener(delegate(bool b)
			{
				if (b)
				{
					ChangeState(State.INFO);
				}
			});
		}
		if ((bool)m_ctglRepair)
		{
			m_ctglRepair.OnValueChanged.RemoveAllListeners();
			m_ctglRepair.OnValueChanged.AddListener(OnChangeRepairTab);
			m_ctglRepair.m_bGetCallbackWhileLocked = true;
		}
		if ((bool)m_ctglModule)
		{
			m_ctglModule.OnValueChanged.RemoveAllListeners();
			m_ctglModule.OnValueChanged.AddListener(OnChangeModuleTab);
			m_ctglModule.m_bGetCallbackWhileLocked = true;
		}
		if (m_btnRecall != null)
		{
			m_btnRecall.PointerClick.RemoveAllListeners();
			m_btnRecall.PointerClick.AddListener(OnClickRecall);
		}
		if ((bool)m_tglMoveRange)
		{
			m_tglMoveRange.OnValueChanged.RemoveAllListeners();
			m_tglMoveRange.OnValueChanged.AddListener(OnChangeMoveRange);
		}
		NKCUtil.SetBindFunction(m_ChangeShip, OnSelectShip);
		if (m_ToolTipHP != null)
		{
			m_ToolTipHP.SetType(NKM_STAT_TYPE.NST_HP);
		}
		if (m_ToolTipATK != null)
		{
			m_ToolTipATK.SetType(NKM_STAT_TYPE.NST_ATK);
		}
		if (m_ToolTipDEF != null)
		{
			m_ToolTipDEF.SetType(NKM_STAT_TYPE.NST_DEF);
		}
		if (m_ToolTipCritical != null)
		{
			m_ToolTipCritical.SetType(NKM_STAT_TYPE.NST_CRITICAL);
		}
		if (m_ToolTipHit != null)
		{
			m_ToolTipHit.SetType(NKM_STAT_TYPE.NST_HIT);
		}
		if (m_ToolTipEvade != null)
		{
			m_ToolTipEvade.SetType(NKM_STAT_TYPE.NST_EVADE);
		}
		InitDragSelectablePanel();
		m_SkillPanel.Init(OpenPopupSkillFullInfoForShip);
		m_ShipInfoRepair.Init(OpenPopupSkillFullInfoForShip);
		m_ShipInfoCommandModule.Init(OpenCommandModuleUI);
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMUnitData shipData, NKMDeckIndex deckIndex, NKCUIUnitInfo.OpenOption openOption = null, NKC_SCEN_UNIT_LIST.eUIOpenReserve ReserveUI = NKC_SCEN_UNIT_LIST.eUIOpenReserve.Nothing)
	{
		SetShipData(shipData, deckIndex);
		base.gameObject.SetActive(value: true);
		SetData(shipData);
		SetState(ShipViewState.Normal, bAnimate: false);
		if (openOption == null)
		{
			openOption = new NKCUIUnitInfo.OpenOption(new List<long> { shipData.m_UnitUID });
		}
		if (openOption.m_lstUnitData.Count <= 0 && openOption.m_UnitUIDList.Count <= 0)
		{
			Debug.Log("Can not found ship list info");
		}
		TempChangeFX.SetActive(value: false);
		NKCUtil.SetGameobjectActive(m_objModuleUnlockFx, bValue: false);
		m_ctglInfo.Select(bSelect: true, bForce: true);
		m_lstUnitData = openOption.m_lstUnitData;
		SetBannerUnit(shipData.m_UnitUID);
		switch (ReserveUI)
		{
		case NKC_SCEN_UNIT_LIST.eUIOpenReserve.ShipRepair:
			m_ctglRepair.Select(bSelect: true, bForce: true, bImmediate: true);
			ChangeState(State.REPAIR);
			break;
		case NKC_SCEN_UNIT_LIST.eUIOpenReserve.ShipModule:
			if (m_ctglModule.m_bLock)
			{
				m_ctglInfo.Select(bSelect: true, bForce: true, bImmediate: true);
				ChangeState(State.INFO);
			}
			else
			{
				m_ctglModule.Select(bSelect: true, bForce: true, bImmediate: true);
				ChangeState(State.MODULE);
			}
			break;
		default:
			m_ctglInfo.Select(bSelect: true, bForce: true, bImmediate: true);
			ChangeState(State.INFO);
			break;
		}
		NKCUtil.SetGameobjectActive(m_ShipInfoMoveType, bValue: false);
		UIOpened();
	}

	private void CheckTabLock()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPYARD))
		{
			m_ctglRepair.Lock();
		}
		else
		{
			m_ctglRepair.UnLock();
		}
		NKCUtil.SetGameobjectActive(m_objRepairLock, !NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPYARD));
		if (!NKMShipManager.IsModuleUnlocked(m_curShipData))
		{
			m_ctglModule.Lock();
		}
		else
		{
			m_ctglModule.UnLock();
		}
	}

	private void TutorialCheck()
	{
		switch (m_curUIState)
		{
		case State.INFO:
			if (m_curShipData.m_UnitLevel >= 100 && m_curShipData.m_LimitBreakLevel == 0 && NKMShipManager.GetShipLimitBreakTemplet(m_curShipData.m_UnitID, 1) != null)
			{
				NKCTutorialManager.TutorialRequired(TutorialPoint.ShipInfoMaxLevel);
			}
			else
			{
				NKCTutorialManager.TutorialRequired(TutorialPoint.ShipInfo);
			}
			break;
		case State.REPAIR:
			if (m_curShipData.m_UnitLevel >= 100 && m_curShipData.m_LimitBreakLevel == 0 && NKMShipManager.GetShipLimitBreakTemplet(m_curShipData.m_UnitID, 1) != null)
			{
				NKCTutorialManager.TutorialRequired(TutorialPoint.ShipLimitBreak);
			}
			else
			{
				NKCTutorialManager.TutorialRequired(TutorialPoint.ShipOverhaul);
			}
			break;
		case State.MODULE:
			if (NKMShipManager.IsModuleUnlocked(m_curShipData))
			{
				NKCTutorialManager.TutorialRequired(TutorialPoint.ShipModule);
			}
			break;
		}
	}

	private void OnClickPractice()
	{
		if (m_curShipData.IsSeized)
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_SHIP_IS_SEIZED);
		}
		else
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OpenPracticeGameComfirmPopup(m_curShipData);
		}
	}

	private void SetData(NKMUnitData shipData)
	{
		if (m_ShipInfoRepair.Status == NKCUIShipInfoRepair.RepairState.LevelUp && m_curShipData.m_UnitUID == shipData.m_UnitUID && m_curShipData.m_UnitLevel < shipData.m_UnitLevel)
		{
			TempChangeFX.SetActive(value: false);
			TempChangeFX.SetActive(value: true);
			NKCSoundManager.PlaySound("FX_UI_SHIP_LEVEL_UP", 1f, 0f, 0f);
		}
		m_curShipData = shipData;
		NKMDeckIndex shipDeckIndex = NKMArmyData.GetShipDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, m_curShipData.m_UnitUID);
		SetShipData(m_curShipData, shipDeckIndex);
		m_fDeltaTime = 0f;
		NKCUtil.SetGameobjectActive(m_objRecall, NKCRecallManager.IsRecallTargetUnit(m_curShipData, NKCSynchronizedTime.GetServerUTCTime()));
		if (m_objRecall.activeSelf)
		{
			SetRecallRemainTime();
		}
		m_ShipInfoRepair.SetData(m_curShipData);
		m_ShipInfoCommandModule.SetData(m_curShipData);
		CheckTabLock();
		if (m_curUIState == State.REPAIR)
		{
			GameObject objRepairLock = m_objRepairLock;
			if ((object)objRepairLock != null && objRepairLock.activeSelf)
			{
				goto IL_013b;
			}
		}
		if (m_curUIState == State.MODULE && m_ctglModule.m_bLock)
		{
			goto IL_013b;
		}
		goto IL_0151;
		IL_0151:
		if (base.gameObject.activeSelf)
		{
			TutorialCheck();
		}
		return;
		IL_013b:
		m_ctglInfo.Select(bSelect: true, bForce: true, bImmediate: true);
		ChangeState(State.INFO);
		goto IL_0151;
	}

	private void SetShipData(NKMUnitData shipData, NKMDeckIndex deckIndex)
	{
		m_curShipData = shipData;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipData.m_UnitID);
		m_uiSummary.SetShipData(shipData, unitTempletBase, deckIndex);
		NKCUtil.SetGameobjectActive(m_objModuleStepMini, shipData.ShipCommandModule.Count > 0);
		if (shipData.ShipCommandModule.Count > 0)
		{
			for (int i = 0; i < m_lstModuleStepMini.Count; i++)
			{
				NKCUtil.SetGameobjectActive(m_lstModuleStepMini[i], i < shipData.ShipCommandModule.Count);
			}
		}
		NKCUtil.SetGameobjectActive(m_tglMoveRange, unitTempletBase != null && m_curUIState == State.INFO);
		if (unitTempletBase != null)
		{
			m_ShipInfoMoveType.SetData(unitTempletBase.m_NKM_UNIT_STYLE_TYPE);
		}
		NKMUnitManager.GetUnitStatTemplet(shipData.m_UnitID);
		NKCUtil.SetLabelText(m_lbHP, $"{NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_HP, shipData):#;-#;0}");
		NKCUtil.SetLabelText(m_lbAttack, $"{NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_ATK, shipData):#;-#;0}");
		NKCUtil.SetLabelText(m_lbDefence, $"{NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_DEF, shipData):#;-#;0}");
		NKCUtil.SetLabelText(m_lbCritical, $"{NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_CRITICAL, shipData):#;-#;0}");
		NKCUtil.SetLabelText(m_lbHit, $"{NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_HIT, shipData):#;-#;0}");
		NKCUtil.SetLabelText(m_lbEvade, $"{NKMUnitStatManager.CalculateStat(NKM_STAT_TYPE.NST_EVADE, shipData):#;-#;0}");
		NKCUtil.SetLabelText(m_lbPower, shipData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData).ToString("N0"));
		m_ShipModule.SetData(shipData);
		m_SkillPanel?.SetData(unitTempletBase);
		m_CurShipID = shipData.m_UnitID;
		m_CurShipUID = shipData.m_UnitUID;
		if (m_ctglLock != null)
		{
			m_ctglLock.Select(shipData.m_bLock, bForce: true, bImmediate: true);
		}
		NKCUtil.SetGameobjectActive(m_objSeized, shipData.IsSeized);
	}

	private void AddSameBuff(ref List<NKMShipCmdSlot> lstSocket, NKMShipCmdSlot targetSocket)
	{
		bool flag = false;
		for (int i = 0; i < lstSocket.Count; i++)
		{
			NKMShipCmdSlot nKMShipCmdSlot = lstSocket[i];
			if (nKMShipCmdSlot.statType == targetSocket.statType && nKMShipCmdSlot.targetStyleType.SetEquals(targetSocket.targetStyleType) && nKMShipCmdSlot.targetRoleType.SetEquals(targetSocket.targetRoleType))
			{
				flag = true;
				nKMShipCmdSlot.statValue += targetSocket.statValue;
				break;
			}
		}
		if (!flag)
		{
			NKMShipCmdSlot item = new NKMShipCmdSlot(targetSocket.targetStyleType, targetSocket.targetRoleType, targetSocket.statType, targetSocket.statValue, targetSocket.isLock);
			lstSocket.Add(item);
		}
	}

	private void SetState(ShipViewState state, bool bAnimate = true)
	{
		m_eShipViewState = state;
		m_rtLeftRect.DOKill();
		m_rtRightRect.DOKill();
		m_rectSpineIllustPanel.DOKill();
		m_rectIllustRoot.DOKill();
		if (m_srScrollRect != null)
		{
			m_srScrollRect.enabled = state == ShipViewState.IllustView;
		}
		switch (state)
		{
		case ShipViewState.IllustView:
			m_tglMoveRange.Select(bSelect: false);
			if (bAnimate)
			{
				m_rtLeftRect.DOAnchorMin(new Vector2(-1.5f, 0f), 0.4f).SetEase(Ease.OutCubic);
				m_rtLeftRect.DOAnchorMax(new Vector2(-0.5f, 1f), 0.4f).SetEase(Ease.OutCubic);
				m_rtRightRect.DOAnchorMin(new Vector2(1.5f, 0f), 0.4f).SetEase(Ease.OutCubic);
				m_rtRightRect.DOAnchorMax(new Vector2(2.5f, 1f), 0.4f).SetEase(Ease.OutCubic);
				m_rectIllustRoot.DOAnchorMin(m_vIllustRootAnchorMinIllustView, 0.4f).SetEase(Ease.OutCubic);
				m_rectIllustRoot.DOAnchorMax(m_vIllustRootAnchorMaxIllustView, 0.4f).SetEase(Ease.OutCubic);
				m_rmLock.Transit("Out");
				NKCUIManager.NKCUIUpsideMenu.Move(bOutsideScreen: true, bAnimate: true);
			}
			else
			{
				m_rtLeftRect.anchorMin = new Vector2(-1.5f, 0f);
				m_rtLeftRect.anchorMax = new Vector2(-0.5f, 1f);
				m_rtRightRect.anchorMin = new Vector2(1.5f, 0f);
				m_rtRightRect.anchorMax = new Vector2(2.5f, 1f);
				m_rectIllustRoot.anchorMin = m_vIllustRootAnchorMinIllustView;
				m_rectIllustRoot.anchorMax = m_vIllustRootAnchorMaxIllustView;
				m_rmLock.Set("Out");
				NKCUIManager.NKCUIUpsideMenu.Move(bOutsideScreen: true, bAnimate: false);
			}
			break;
		case ShipViewState.Normal:
			if (bAnimate)
			{
				m_rtLeftRect.DOAnchorMin(Vector2.zero, 0.4f).SetEase(Ease.OutCubic);
				m_rtLeftRect.DOAnchorMax(Vector2.one, 0.4f).SetEase(Ease.OutCubic);
				m_rtRightRect.DOAnchorMin(Vector2.zero, 0.4f).SetEase(Ease.OutCubic);
				m_rtRightRect.DOAnchorMax(Vector2.one, 0.4f).SetEase(Ease.OutCubic);
				m_rectSpineIllustPanel.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutCubic);
				m_rectSpineIllustPanel.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
				m_rectIllustRoot.DOAnchorMin(m_vIllustRootAnchorMinNormal, 0.4f).SetEase(Ease.OutCubic);
				m_rectIllustRoot.DOAnchorMax(m_vIllustRootAnchorMaxNormal, 0.4f).SetEase(Ease.OutCubic);
				m_rmLock.Transit("Base");
				NKCUIManager.NKCUIUpsideMenu.Move(bOutsideScreen: false, bAnimate: true);
			}
			else
			{
				m_rtLeftRect.anchorMin = Vector2.zero;
				m_rtLeftRect.anchorMax = Vector2.one;
				m_rtRightRect.anchorMin = Vector2.zero;
				m_rtRightRect.anchorMax = Vector2.one;
				m_rectSpineIllustPanel.localScale = Vector3.one;
				m_rectSpineIllustPanel.localPosition = Vector3.zero;
				m_rectIllustRoot.anchorMin = m_vIllustRootAnchorMinNormal;
				m_rectIllustRoot.anchorMax = m_vIllustRootAnchorMaxNormal;
				NKCUIManager.NKCUIUpsideMenu.Move(bOutsideScreen: false, bAnimate: false);
			}
			break;
		}
	}

	private void SetDeckNumber(NKMDeckIndex deckIndex)
	{
	}

	public void OnRecv(NKMPacket_DECK_SHIP_SET_ACK sPacket)
	{
		if (sPacket.shipUID == m_curShipData.m_UnitUID)
		{
			SetDeckNumber(sPacket.deckIndex);
		}
	}

	public void OnRecv(NKMPacket_SHIP_UPGRADE_ACK sPacket)
	{
		if (m_curUIState == State.REPAIR)
		{
			m_ShipInfoRepair.OnRecv(sPacket);
		}
	}

	private void OnLockToggle(bool bValue)
	{
		if (bValue != m_curShipData.m_bLock)
		{
			NKCPacketSender.Send_NKMPacket_LOCK_UNIT_REQ(m_curShipData.m_UnitUID, !m_curShipData.m_bLock);
		}
	}

	private void OnClickRecall()
	{
		if (m_curShipData == null)
		{
			return;
		}
		DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
		NKMRecallTemplet nKMRecallTemplet = NKMRecallTemplet.Find(NKCRecallManager.GetFirstLevelShipID(m_curShipData.m_UnitID), NKMTime.UTCtoLocal(serverUTCTime));
		if (nKMRecallTemplet == null)
		{
			return;
		}
		if (NKCScenManager.CurrentUserData().m_RecallHistoryData.ContainsKey(m_curShipData.m_UnitID))
		{
			RecallHistoryInfo recallHistoryInfo = NKCScenManager.CurrentUserData().m_RecallHistoryData[m_curShipData.m_UnitID];
			if (NKCRecallManager.IsValidTime(nKMRecallTemplet, recallHistoryInfo.lastUpdateDate))
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_RECALL_ALREADY_USED);
				return;
			}
		}
		if (!NKCRecallManager.IsValidTime(nKMRecallTemplet, serverUTCTime))
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_RECALL_PERIOD_EXPIRED);
		}
		else if (!NKCRecallManager.IsValidRegTime(nKMRecallTemplet, m_curShipData.m_regDate))
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_RECALL_INVALID_ACCQUIRE_TIME);
		}
		else if (m_curShipData.m_bLock || NKCScenManager.CurrentUserData().m_ArmyData.IsUnitInAnyDeck(m_curShipData.m_UnitUID))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_RECALL_ERROR_ALT_USING_UNIT);
		}
		else
		{
			NKCPopupRecall.Instance.Open(m_curShipData);
		}
	}

	private void SetRecallRemainTime()
	{
		NKMRecallTemplet nKMRecallTemplet = NKMRecallTemplet.Find(NKCRecallManager.GetFirstLevelShipID(m_curShipData.m_UnitID), NKMTime.UTCtoLocal(NKCSynchronizedTime.GetServerUTCTime()));
		if (nKMRecallTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbRecallTime, string.Format(NKCUtilString.GET_STRING_RECALL_DESC_END_DATE, NKCUtilString.GetRemainTimeStringEx(nKMRecallTemplet.IntervalTemplet.GetEndDateUtc())));
		}
	}

	private void OnChangeMoveRange(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_ShipInfoMoveType, bValue);
	}

	private void Update()
	{
		if (m_curUIState == State.INFO && m_eShipViewState == ShipViewState.IllustView)
		{
			if (NKCScenManager.GetScenManager().GetHasPinch())
			{
				m_srScrollRect.enabled = false;
				OnPinchZoom(NKCScenManager.GetScenManager().GetPinchCenter(), NKCScenManager.GetScenManager().GetPinchDeltaMagnitude());
			}
			else
			{
				m_srScrollRect.enabled = true;
			}
			float y = Input.mouseScrollDelta.y;
			if (y != 0f)
			{
				OnPinchZoom(Input.mousePosition, y);
			}
		}
		if (m_objRecall.activeSelf)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRecallRemainTime();
			}
		}
	}

	private void OnClickChangeIllust()
	{
		NKCUIPopupIllustView.Instance.Open(m_curShipData);
	}

	public void OnPinchZoom(Vector2 PinchCenter, float pinchMagnitude)
	{
		float num = m_rectSpineIllustPanel.localScale.x * Mathf.Pow(4f, pinchMagnitude);
		if (num < 0.5f)
		{
			num = 0.5f;
		}
		if (num > 2f)
		{
			num = 2f;
		}
		m_rectSpineIllustPanel.localScale = new Vector3(num, num, 1f);
	}

	private void OpenPopupSkillFullInfoForShip()
	{
		if (m_CurShipID == 0)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_CurShipID);
		if (unitTempletBase == null)
		{
			return;
		}
		m_lstShipSkills.Clear();
		m_shipName = unitTempletBase.GetUnitName();
		for (int i = 0; i < 4; i++)
		{
			NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(unitTempletBase, i);
			if (shipSkillTempletByIndex != null)
			{
				m_lstShipSkills.Add(shipSkillTempletByIndex);
			}
		}
		NKCPopupSkillFullInfo.ShipInstance.OpenForShip(m_CurShipID, m_CurShipUID);
	}

	private void InitDragSelectablePanel()
	{
		if (m_DragUnitView != null)
		{
			m_DragUnitView.Init(rotation: true);
			m_DragUnitView.dOnGetObject += MakeMainBannerListSlot;
			m_DragUnitView.dOnReturnObject += ReturnMainBannerListSlot;
			m_DragUnitView.dOnProvideData += ProvideMainBannerListSlotData;
			m_DragUnitView.dOnIndexChangeListener += SelectCharacter;
			m_DragUnitView.dOnFocus += Focus;
			m_iBannerSlotCnt = 0;
		}
	}

	private void SetBannerUnit(long unitUID)
	{
		if (!(m_DragUnitView != null))
		{
			return;
		}
		if (m_lstUnitData.Count <= 0)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				NKMArmyData armyData = nKMUserData.m_ArmyData;
				if (armyData != null)
				{
					NKMUnitData shipFromUID = armyData.GetShipFromUID(unitUID);
					if (shipFromUID != null)
					{
						m_lstUnitData.Add(shipFromUID);
					}
				}
			}
		}
		for (int i = 0; i < m_lstUnitData.Count; i++)
		{
			if (m_lstUnitData[i].m_UnitUID == unitUID)
			{
				m_DragUnitView.TotalCount = m_lstUnitData.Count;
				m_DragUnitView.SetIndex(i);
				break;
			}
		}
	}

	private RectTransform MakeMainBannerListSlot()
	{
		GameObject obj = new GameObject($"Banner{m_iBannerSlotCnt}", typeof(RectTransform), typeof(LayoutElement));
		LayoutElement component = obj.GetComponent<LayoutElement>();
		component.ignoreLayout = false;
		component.preferredWidth = m_DragUnitView.m_rtContentRect.GetWidth();
		component.preferredHeight = m_DragUnitView.m_rtContentRect.GetHeight();
		component.flexibleWidth = 2f;
		component.flexibleHeight = 2f;
		m_iBannerSlotCnt++;
		return obj.GetComponent<RectTransform>();
	}

	private void ProvideMainBannerListSlotData(Transform tr, int idx)
	{
		if (m_lstUnitData == null)
		{
			return;
		}
		NKMUnitData nKMUnitData = m_lstUnitData[idx];
		if (nKMUnitData == null || !(tr != null))
		{
			return;
		}
		string text = tr.gameObject.name;
		string s = text.Substring(text.Length - 1);
		int result = 0;
		int.TryParse(s, out result);
		if (!m_dicUnitIllust.ContainsKey(result))
		{
			NKCASUIUnitIllust nKCASUIUnitIllust = NKCResourceUtility.OpenSpineIllust(nKMUnitData);
			if (nKCASUIUnitIllust != null)
			{
				RectTransform rectTransform = nKCASUIUnitIllust.GetRectTransform();
				if (rectTransform != null)
				{
					rectTransform.localScale = new Vector3(-1f, rectTransform.localScale.y, rectTransform.localScale.z);
				}
				nKCASUIUnitIllust.SetParent(tr.transform, worldPositionStays: false);
				nKCASUIUnitIllust.SetAnchoredPosition(Vector2.zero);
				nKCASUIUnitIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SHIP_IDLE);
			}
			m_dicUnitIllust.Add(result, nKCASUIUnitIllust);
		}
		else
		{
			if (m_dicUnitIllust[result] == null)
			{
				return;
			}
			m_dicUnitIllust[result].Unload();
			m_dicUnitIllust[result] = null;
			m_dicUnitIllust[result] = NKCResourceUtility.OpenSpineIllust(nKMUnitData);
			if (m_dicUnitIllust[result] != null)
			{
				RectTransform rectTransform2 = m_dicUnitIllust[result].GetRectTransform();
				if (rectTransform2 != null)
				{
					rectTransform2.localScale = new Vector3(-1f, rectTransform2.localScale.y, rectTransform2.localScale.z);
				}
				m_dicUnitIllust[result].SetParent(tr.transform, worldPositionStays: false);
				m_dicUnitIllust[result].SetAnchoredPosition(Vector2.zero);
				m_dicUnitIllust[result].SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SHIP_IDLE);
			}
		}
	}

	private void ReturnMainBannerListSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		UnityEngine.Object.Destroy(go.gameObject);
	}

	private void Focus(RectTransform rect, bool bFocus)
	{
		NKCUtil.SetGameobjectActive(rect.gameObject, bFocus);
	}

	public void SelectCharacter(int idx)
	{
		if (m_lstUnitData.Count < idx || idx < 0)
		{
			Debug.LogWarning($"Error - Count : {m_lstUnitData.Count}, Index : {idx}");
			return;
		}
		NKMUnitData nKMUnitData = m_lstUnitData[idx];
		if (nKMUnitData != null)
		{
			ChangeUnit(nKMUnitData);
		}
	}

	private void BannerCleanUp()
	{
		foreach (KeyValuePair<int, NKCASUIUnitIllust> item in m_dicUnitIllust)
		{
			if (item.Value != null)
			{
				item.Value.Unload();
			}
		}
		m_dicUnitIllust.Clear();
	}

	private void ChangeUnit(NKMUnitData shipData)
	{
		if (shipData != m_curShipData)
		{
			SetData(shipData);
		}
	}

	private void ChangeState(State newState)
	{
		if (newState == State.REPAIR && !NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPYARD))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.HANGER_SHIPYARD);
			return;
		}
		if ((newState == State.REPAIR || newState == State.MODULE) && m_eShipViewState == ShipViewState.IllustView)
		{
			SetState(ShipViewState.Normal);
		}
		if (newState == State.REPAIR)
		{
			m_ShipInfoRepair.SetData(m_curShipData);
		}
		if (newState == State.MODULE)
		{
			m_ShipInfoCommandModule.SetData(m_curShipData);
		}
		m_curUIState = newState;
		NKCUtil.SetGameobjectActive(m_objInfo, m_curUIState == State.INFO);
		NKCUtil.SetGameobjectActive(m_objRepair, m_curUIState == State.REPAIR);
		NKCUtil.SetGameobjectActive(m_objModule, m_curUIState == State.MODULE);
		NKCUtil.SetGameobjectActive(m_cbtnChangeIllust, m_curUIState == State.INFO);
		NKCUtil.SetGameobjectActive(m_ctglLock, m_curUIState == State.INFO);
		NKCUtil.SetGameobjectActive(m_cbtnPractice, m_curUIState == State.INFO);
		NKCUtil.SetGameobjectActive(m_ChangeShip, m_curUIState == State.REPAIR || m_curUIState == State.MODULE);
		NKCUtil.SetGameobjectActive(m_tglMoveRange, m_curUIState == State.INFO);
		if (m_tglMoveRange.m_bSelect)
		{
			m_tglMoveRange.Select(bSelect: false);
		}
		NKCUtil.SetImageSprite(m_imgBG, GetBackgroundSprite(m_curUIState));
		TutorialCheck();
		NKCUIManager.UpdateUpsideMenu();
	}

	private Sprite GetBackgroundSprite(State type)
	{
		string text = "";
		string text2 = "";
		if (type == State.INFO)
		{
			text2 = "AB_UI_BG_SPRITE";
			text = "BG";
		}
		else
		{
			text2 = "AB_UI_NUF_BASE_BG";
			text = "NKM_UI_BASE_HANGAR_BG";
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(text2, text);
		if (orLoadAssetResource == null)
		{
			Debug.LogError("Error - NKCUIUnitInfo::GetBackgroundSprite - path:" + text2 + ", name:" + text);
		}
		return orLoadAssetResource;
	}

	private void OnSelectShip()
	{
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_SHIP, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
		options.bDescending = true;
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_SHIP, bIsCollection: false);
		options.bShowRemoveSlot = false;
		options.bShowHideDeckedUnitMenu = false;
		options.bHideDeckedUnit = false;
		options.bCanSelectUnitInMission = true;
		options.strUpsideMenuName = NKCUtilString.GET_STRING_SELECT_SHIP;
		options.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_SELECT_SHIP;
		options.setShipFilterCategory = NKCUnitSortSystem.setDefaultShipFilterCategory;
		options.setShipSortCategory = NKCUnitSortSystem.setDefaultShipSortCategory;
		options.m_bHideUnitCount = true;
		options.m_bUseFavorite = true;
		UnitSelectList.Open(options, null, OnUnitSortList);
	}

	private void OnUnitSortList(long UID, List<NKMUnitData> unitDataList)
	{
		UnitSelectList.Close();
		BannerCleanUp();
		m_lstUnitData = unitDataList;
		if (m_lstUnitData.Count <= 0)
		{
			return;
		}
		m_DragUnitView.TotalCount = m_lstUnitData.Count;
		for (int i = 0; i < m_lstUnitData.Count; i++)
		{
			if (m_lstUnitData[i].m_UnitUID == UID)
			{
				m_DragUnitView.SetIndex(i);
				break;
			}
		}
	}

	private void OnChangeRepairTab(bool bValue)
	{
		if (m_ctglRepair.m_bLock && !NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPYARD))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.HANGER_SHIPYARD);
		}
		else if (bValue)
		{
			ChangeState(State.REPAIR);
		}
	}

	private void OnChangeModuleTab(bool bValue)
	{
		if (m_ctglModule.m_bLock)
		{
			string text = "";
			if (!NKMOpenTagManager.IsOpened("SHIP_LIMITBREAK"))
			{
				text = NKCUtilString.GET_STRING_COMING_SOON_SYSTEM;
			}
			else if (!NKMOpenTagManager.IsOpened("SHIP_COMMANDMODULE"))
			{
				text = NKCUtilString.GET_STRING_COMING_SOON_SYSTEM;
			}
			else if (m_curShipData.m_LimitBreakLevel <= 0)
			{
				text = ((NKMShipManager.GetShipLimitBreakTemplet(m_curShipData.m_UnitID, 1) != null) ? NKCUtilString.GET_STRING_SHIP_COMMAND_MODULE_NOT_LIMITBREAK : NKCUtilString.GET_STRING_SHIP_INFO_COMMAND_MODULE_NO_LIMITBREAK);
			}
			if (!string.IsNullOrEmpty(text))
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(text, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
			switch (m_curUIState)
			{
			case State.INFO:
				m_ctglInfo.Select(bSelect: true, bForce: true, bImmediate: true);
				break;
			case State.MODULE:
				m_ctglInfo.Select(bSelect: true, bForce: true, bImmediate: true);
				ChangeState(State.INFO);
				break;
			case State.REPAIR:
				m_ctglRepair.Select(bSelect: true, bForce: true, bImmediate: true);
				break;
			}
		}
		else if (bValue)
		{
			ChangeState(State.MODULE);
		}
	}

	private void OpenCommandModuleUI()
	{
		if (NKCScenManager.CurrentUserData() == null)
		{
			return;
		}
		NKMShipModuleCandidate shipCandidateData = NKCScenManager.CurrentUserData().GetShipCandidateData();
		if (shipCandidateData.shipUid > 0)
		{
			if (NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(shipCandidateData.shipUid) != null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SHIP_COMMAND_MODULE_SLOT_HAS_RESERVED, OpenCommandModulePopup);
				return;
			}
			Log.Warn($"TargetShipData is null - UID : {shipCandidateData.shipUid}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIShipInfo.cs", 1195);
			NKCPacketSender.Send_NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ();
		}
		NKCPopupShipCommandModule.Instance.Open(m_curShipData);
	}

	private void OpenCommandModulePopup()
	{
		NKMShipModuleCandidate shipCandidateData = NKCScenManager.CurrentUserData().GetShipCandidateData();
		long shipUid = shipCandidateData.shipUid;
		SetBannerUnit(shipUid);
		NKCPopupShipCommandModule.Instance.Open(m_curShipData, shipCandidateData.moduleId);
	}
}
