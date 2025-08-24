using System.Collections.Generic;
using System.Text;
using ClientPacket.User;
using NKC.UI.Component;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionShipInfo : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_ship_info";

	private const string UI_ASSET_NAME = "NKM_UI_SHIP_INFO_COLLECTION";

	private static NKCUICollectionShipInfo m_Instance;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	[Header("왼쪽 위 함선 기본정보")]
	public NKCUIShipInfoSummary m_uiSummary;

	public GameObject m_objModuleStepMini;

	public List<GameObject> m_lstModuleStepMini;

	[Header("오른쪽 함선 정보")]
	public Text m_lbPower;

	public Text m_lbHP;

	public Text m_lbAttack;

	public Text m_lbDefence;

	public Text m_lbCritical;

	public Text m_lbHit;

	public Text m_lbEvade;

	public NKCUIShipInfoSkillPanel m_SkillPanel;

	public GameObject m_objNoModule;

	public GameObject m_objLockedModule;

	public GameObject m_objEnabledModule;

	public List<GameObject> m_lstModuleStep = new List<GameObject>();

	public Text m_lbModuleStep;

	public NKCUIComToggle m_tglSocket_01;

	public NKCUIComToggle m_tglSocket_02;

	public ScrollRect m_srSocketOptions;

	public Text m_lbSocketOptions;

	[Header("UI State 관련")]
	public RectTransform m_rtLeftRect;

	public RectTransform m_rtRightRect;

	public NKCUIRectMove m_rmLock;

	public Animator m_Ani_NKM_UI_SHIP_INFO_COLLECTION;

	[Header("스파인 일러스트")]
	public ScrollRect m_srScrollRect;

	public RectTransform m_rectSpineIllustPanel;

	public RectTransform m_rectIllustRoot;

	public Vector2 m_vIllustRootAnchorMinNormal;

	public Vector2 m_vIllustRootAnchorMaxNormal;

	public Vector2 m_vIllustRootAnchorMinIllustView;

	public Vector2 m_vIllustRootAnchorMaxIllustView;

	[Header("기타 버튼")]
	public NKCUIComStateButton m_cbtnChangeIllust;

	public NKCUIComStateButton m_cbtnPractice;

	public NKCUIComToggle m_tglMoveRange;

	public NKCUIShipInfoMoveType m_ShipInfoMoveType;

	public NKCUIComStateButton m_GuideBtn;

	public string m_GuideStrID;

	public NKCComStatInfoToolTip m_ToolTipHP;

	public NKCComStatInfoToolTip m_ToolTipATK;

	public NKCComStatInfoToolTip m_ToolTipDEF;

	public NKCComStatInfoToolTip m_ToolTipCritical;

	public NKCComStatInfoToolTip m_ToolTipHit;

	public NKCComStatInfoToolTip m_ToolTipEvade;

	[Header("유닛 획득 표시")]
	public GameObject m_NKM_UI_SHIP_INFO_NOTICE_NOTGET;

	private NKMDeckIndex m_deckIndex;

	private NKMUnitData m_NKMUnitData;

	private List<NKMEquipItemData> m_listNKMEquipItemData;

	private bool m_isGauntlet;

	private bool m_bNonePlatoon;

	private bool m_bIllustView;

	private const float MIN_ZOOM_SCALE = 0.5f;

	private const float MAX_ZOOM_SCALE = 2f;

	private int m_CurShipID;

	[Header("함선 변경")]
	public NKCUIComDragSelectablePanel m_DragUnitView;

	private Dictionary<int, NKCASUIUnitIllust> m_dicUnitIllust = new Dictionary<int, NKCASUIUnitIllust>();

	private int m_iBannerSlotCnt;

	private NKCUIUnitInfo.OpenOption m_OpenOption;

	public static NKCUICollectionShipInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_loadedUIData = NKCUIManager.OpenNewInstance<NKCUICollectionShipInfo>("ab_ui_nkm_ui_ship_info", "NKM_UI_SHIP_INFO_COLLECTION", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
				m_Instance = m_loadedUIData.GetInstance<NKCUICollectionShipInfo>();
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

	public override string MenuName => NKCUtilString.GET_STRING_SHIP_INFO;

	public override bool WillCloseUnderPopupOnOpen => false;

	private NKMArmyData NKMArmyData => NKCScenManager.CurrentUserData()?.m_ArmyData;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_loadedUIData != null)
		{
			m_loadedUIData.CloseInstance();
		}
	}

	private void OnDestroy()
	{
	}

	public override void CloseInternal()
	{
		BannerCleanUp();
		base.gameObject.SetActive(value: false);
		NKCUIPopupIllustView.CheckInstanceAndClose();
	}

	public override void OnBackButton()
	{
		if (m_bIllustView)
		{
			OnClickChangeIllust();
		}
		else
		{
			base.OnBackButton();
		}
	}

	private void Init()
	{
		if (m_cbtnChangeIllust != null)
		{
			m_cbtnChangeIllust.PointerClick.RemoveAllListeners();
			m_cbtnChangeIllust.PointerClick.AddListener(OnClickChangeIllust);
		}
		if (m_cbtnPractice != null)
		{
			m_cbtnPractice.PointerClick.RemoveAllListeners();
			m_cbtnPractice.PointerClick.AddListener(OnClickPractice);
		}
		if (null != m_GuideBtn)
		{
			m_GuideBtn.PointerClick.RemoveAllListeners();
			m_GuideBtn.PointerClick.AddListener(delegate
			{
				NKCUIPopUpGuide.Instance.Open(m_GuideStrID);
			});
		}
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
		if (m_tglSocket_01 != null)
		{
			m_tglSocket_01.OnValueChanged.RemoveAllListeners();
			m_tglSocket_01.OnValueChanged.AddListener(OnValueChangedSocket_01);
		}
		if (m_tglSocket_02 != null)
		{
			m_tglSocket_02.OnValueChanged.RemoveAllListeners();
			m_tglSocket_02.OnValueChanged.AddListener(OnValueChangedSocket_02);
		}
		if ((bool)m_tglMoveRange)
		{
			m_tglMoveRange.OnValueChanged.RemoveAllListeners();
			m_tglMoveRange.OnValueChanged.AddListener(OnChangeMoveRange);
		}
		InitDragSelectablePanel();
		m_SkillPanel.Init(OpenPopupSkillFullInfoForShip);
		base.gameObject.SetActive(value: false);
	}

	public static void CheckInstanceAndOpen(NKMUnitData shipData, NKMDeckIndex deckIndex, NKCUIUnitInfo.OpenOption openOption = null, List<NKMEquipItemData> listNKMEquipItemData = null, bool isGauntlet = false)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipData);
		if (unitTempletBase == null || unitTempletBase.IsUnitDescNullOrEmplty())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENT_UNIT_DETAIL_INFO_NOT_POSSIBLE);
			return;
		}
		openOption?.FilterUnitlist();
		Instance.Open(shipData, deckIndex, openOption, listNKMEquipItemData, isGauntlet);
	}

	private void Open(NKMUnitData shipData, NKMDeckIndex deckIndex, NKCUIUnitInfo.OpenOption openOption = null, List<NKMEquipItemData> listNKMEquipItemData = null, bool isGauntlet = false)
	{
		m_bNonePlatoon = deckIndex == NKMDeckIndex.None;
		m_listNKMEquipItemData = listNKMEquipItemData;
		m_isGauntlet = isGauntlet;
		m_tglMoveRange.Select(bSelect: false);
		NKCUtil.SetGameobjectActive(m_ShipInfoMoveType, bValue: false);
		if (m_listNKMEquipItemData != null || m_isGauntlet)
		{
			NKCUtil.SetGameobjectActive(m_cbtnPractice, bValue: false);
		}
		SetShipData(shipData, deckIndex);
		base.gameObject.SetActive(value: true);
		if (openOption == null)
		{
			openOption = new NKCUIUnitInfo.OpenOption(new List<long>());
			openOption.m_lstUnitData.Add(shipData);
		}
		if (openOption.m_lstUnitData.Count <= 0 && openOption.m_UnitUIDList.Count <= 0)
		{
			Debug.Log("Can not found ship list info");
		}
		m_OpenOption = openOption;
		m_iBannerSlotCnt = 0;
		SetBannerUnit(shipData.m_UnitUID);
		UIOpened();
	}

	private void OnClickPractice()
	{
		NKM_SHORTCUT_TYPE returnUIShortcut = ((NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION) ? NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().GetCurrentShortcutType() : NKM_SHORTCUT_TYPE.SHORTCUT_NONE);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_NKMUnitData);
		string returnUIShortcutParam = ((unitTempletBase != null) ? unitTempletBase.m_UnitStrID : "");
		NKCScenManager.GetScenManager().Get_SCEN_GAME().OpenPracticeGameComfirmPopup(m_NKMUnitData, returnUIShortcut, returnUIShortcutParam);
	}

	private void SetShipData(NKMUnitData shipData, NKMDeckIndex deckIndex)
	{
		m_NKMUnitData = shipData;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipData.m_UnitID);
		m_deckIndex = deckIndex;
		m_uiSummary.SetShipData(shipData, unitTempletBase, deckIndex);
		NKCUtil.SetGameobjectActive(m_objModuleStepMini, shipData.m_LimitBreakLevel > 0);
		for (int i = 0; i < m_lstModuleStepMini.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstModuleStepMini[i], i < shipData.m_LimitBreakLevel);
		}
		NKCUtil.SetGameobjectActive(m_tglMoveRange, unitTempletBase != null);
		if (unitTempletBase != null)
		{
			m_ShipInfoMoveType.SetData(unitTempletBase.m_NKM_UNIT_STYLE_TYPE);
		}
		bool bPvP = false;
		NKMStatData nKMStatData = new NKMStatData();
		nKMStatData.Init();
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(shipData.m_UnitID);
		if (unitStatTemplet != null)
		{
			nKMStatData.MakeBaseStat(null, bPvP, shipData, unitStatTemplet.m_StatData);
		}
		if (m_listNKMEquipItemData != null)
		{
			nKMStatData.MakeBaseBonusFactor(shipData, NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.EquipItems, null, null);
		}
		else
		{
			nKMStatData.MakeBaseBonusFactor(shipData, null, null, null);
		}
		m_lbHP.text = $"{nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_HP):#;-#;0}";
		m_lbAttack.text = $"{nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_ATK):#;-#;0}";
		m_lbDefence.text = $"{nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_DEF):#;-#;0}";
		m_lbCritical.text = $"{nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_CRITICAL):#;-#;0}";
		m_lbHit.text = $"{nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_HIT):#;-#;0}";
		m_lbEvade.text = $"{nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_EVADE):#;-#;0}";
		m_lbPower.text = shipData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData).ToString("N0");
		NKMShipLimitBreakTemplet shipLimitBreakTemplet = NKMShipManager.GetShipLimitBreakTemplet(NKMShipManager.GetMaxLevelShipID(shipData.m_UnitID), 1);
		NKCUtil.SetGameobjectActive(m_objNoModule, shipLimitBreakTemplet == null);
		NKCUtil.SetGameobjectActive(m_objLockedModule, shipData.m_LimitBreakLevel == 0 && shipLimitBreakTemplet != null);
		NKCUtil.SetGameobjectActive(m_objEnabledModule, shipData.m_LimitBreakLevel > 0);
		if (m_objEnabledModule != null && m_objEnabledModule.activeSelf)
		{
			for (int j = 0; j < m_lstModuleStep.Count; j++)
			{
				NKCUtil.SetGameobjectActive(m_lstModuleStep[j], j < shipData.m_LimitBreakLevel);
			}
			NKCUtil.SetLabelText(m_lbModuleStep, string.Format(NKCUtilString.GET_STRING_SHIP_INFO_MODULE_STEP_TEXT, shipData.m_LimitBreakLevel));
			m_tglSocket_01.Select(bSelect: true, bForce: true, bImmediate: true);
			ShowTotalSocketOptions(0);
		}
		m_SkillPanel.SetData(unitTempletBase);
		m_CurShipID = shipData.m_UnitID;
		if (m_listNKMEquipItemData == null)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_SHIP_INFO_NOTICE_NOTGET, !NKCUICollectionUnitList.IsHasUnit(NKM_UNIT_TYPE.NUT_SHIP, shipData.m_UnitID));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_SHIP_INFO_NOTICE_NOTGET, bValue: false);
		}
	}

	private void SetDeckNumber(NKMDeckIndex deckIndex)
	{
		m_deckIndex = deckIndex;
	}

	public void OnRecv(NKMPacket_DECK_SHIP_SET_ACK sPacket)
	{
		if (sPacket.shipUID == m_NKMUnitData.m_UnitUID)
		{
			SetDeckNumber(sPacket.deckIndex);
		}
	}

	private void OnClickChangeIllust()
	{
		NKCUIPopupIllustView.Instance.Open(m_NKMUnitData);
	}

	private void Update()
	{
		if (m_bIllustView)
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
		if (m_CurShipID != 0)
		{
			NKCPopupSkillFullInfo.ShipInstance.OpenForShip(m_CurShipID, -1L);
		}
	}

	private void OnValueChangedSocket_01(bool bValue)
	{
		if (bValue)
		{
			ShowTotalSocketOptions(0);
		}
	}

	private void OnValueChangedSocket_02(bool bValue)
	{
		if (bValue)
		{
			ShowTotalSocketOptions(1);
		}
	}

	private void ShowTotalSocketOptions(int socketIndex)
	{
		List<NKMShipCmdSlot> lstSocket = new List<NKMShipCmdSlot>();
		for (int i = 0; i < m_NKMUnitData.ShipCommandModule.Count; i++)
		{
			if (m_NKMUnitData.ShipCommandModule[i] == null || m_NKMUnitData.ShipCommandModule[i].slots == null)
			{
				continue;
			}
			for (int j = 0; j < m_NKMUnitData.ShipCommandModule[i].slots.Length; j++)
			{
				if (socketIndex == j)
				{
					NKMShipCmdSlot nKMShipCmdSlot = m_NKMUnitData.ShipCommandModule[i].slots[j];
					if (nKMShipCmdSlot != null && nKMShipCmdSlot.statType != NKM_STAT_TYPE.NST_RANDOM)
					{
						AddSameBuff(ref lstSocket, nKMShipCmdSlot);
					}
				}
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int k = 0; k < lstSocket.Count; k++)
		{
			stringBuilder.AppendLine(NKCUtilString.GetSlotOptionString(lstSocket[k]));
		}
		NKCUtil.SetLabelText(m_lbSocketOptions, stringBuilder.ToString());
		m_srSocketOptions.normalizedPosition = Vector2.zero;
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
		if (m_OpenOption.m_lstUnitData.Count <= 0)
		{
			if (m_OpenOption.m_UnitUIDList.Count <= 0 && unitUID != 0L)
			{
				m_OpenOption.m_UnitUIDList.Add(unitUID);
			}
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				NKMArmyData armyData = nKMUserData.m_ArmyData;
				if (armyData != null)
				{
					for (int i = 0; i < m_OpenOption.m_UnitUIDList.Count; i++)
					{
						NKMUnitData shipFromUID = armyData.GetShipFromUID(m_OpenOption.m_UnitUIDList[i]);
						if (shipFromUID != null)
						{
							m_OpenOption.m_lstUnitData.Add(shipFromUID);
						}
					}
				}
			}
		}
		for (int j = 0; j < m_OpenOption.m_lstUnitData.Count; j++)
		{
			if (m_OpenOption.m_lstUnitData[j].m_UnitUID == unitUID)
			{
				m_DragUnitView.TotalCount = m_OpenOption.m_lstUnitData.Count;
				m_DragUnitView.SetIndex(j);
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
		if (m_OpenOption == null || m_OpenOption.m_lstUnitData == null)
		{
			return;
		}
		NKMUnitData nKMUnitData = m_OpenOption.m_lstUnitData[idx];
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
		Object.Destroy(go.gameObject);
	}

	private void Focus(RectTransform rect, bool bFocus)
	{
		NKCUtil.SetGameobjectActive(rect.gameObject, bFocus);
	}

	public void SelectCharacter(int idx)
	{
		if (m_OpenOption.m_lstUnitData.Count < idx || idx < 0)
		{
			Debug.LogWarning($"Error - Count : {m_OpenOption.m_lstUnitData.Count}, Index : {idx}");
			return;
		}
		NKMUnitData nKMUnitData = m_OpenOption.m_lstUnitData[idx];
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

	private void ChangeUnit(NKMUnitData cNKMUnitData)
	{
		NKMDeckIndex shipDeckIndex = NKMArmyData.GetShipDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, cNKMUnitData.m_UnitUID);
		SetShipData(cNKMUnitData, m_bNonePlatoon ? NKMDeckIndex.None : shipDeckIndex);
	}

	private void OnChangeMoveRange(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_ShipInfoMoveType, bValue);
	}

	private Sprite GetSpriteMoveType(NKM_UNIT_STYLE_TYPE type)
	{
		string stringMoveType = GetStringMoveType(type);
		if (string.IsNullOrEmpty(stringMoveType))
		{
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_SHIP_INFO_TEXTURE", stringMoveType);
	}

	private string GetStringMoveType(NKM_UNIT_STYLE_TYPE type)
	{
		string result = string.Empty;
		switch (type)
		{
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT:
			result = "NKM_UI_SHIP_INFO_TEXTURE_MOVETYPE_1";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY:
			result = "NKM_UI_SHIP_INFO_TEXTURE_MOVETYPE_4";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER:
			result = "NKM_UI_SHIP_INFO_TEXTURE_MOVETYPE_2";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL:
			result = "NKM_UI_SHIP_INFO_TEXTURE_MOVETYPE_3";
			break;
		}
		return result;
	}
}
