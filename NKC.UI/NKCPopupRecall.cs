using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs.Logging;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using NKM.Templet.Recall;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupRecall : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_RECALL";

	private static NKCPopupRecall m_Instance;

	public NKCPopupRecallSlot m_pfbSlot;

	public Text m_lbTitle;

	public Text m_lbDesc;

	[Header("유닛/함선 텍스트")]
	public Text m_lbSourceType;

	[Header("유닛")]
	public GameObject m_objRecallUnit;

	public GameObject m_objUnit;

	public NKCUIUnitSelectListSlot m_slotSourceUnit;

	public NKCUIUnitSelectListSlot m_slotTargetUnit;

	[Header("함선")]
	public GameObject m_objShip;

	public NKCUIShipSelectListSlot m_slotSourceShip;

	public NKCUIShipSelectListSlot m_slotTargetShip;

	public NKCUIShipModule m_ShipModuleSource;

	public NKCUIShipModule m_ShipModuleTarget;

	[Header("보상들")]
	public GameObject m_objResource;

	public Transform m_trRewardSlotParent;

	[Header("하단버튼")]
	public NKCUIComStateButton m_btnCancel;

	public NKCUIComStateButton m_btnOK;

	public Text m_lbOk;

	[Header("임시 리콜")]
	public GameObject m_objRefoundUnit;

	public NKCUIUnitSelectListSlot m_slotRefoundUnit;

	public GameObject m_objRefoundLimitCnt;

	public Text m_lbRefoundLimitCount;

	[Header("전술업데이트, 리액터 리콜")]
	public NKCPopupRecallMultiUnitSlot m_pfbMultiUnitSlot;

	public GameObject m_objUnitTacticRecall;

	public NKCUIUnitSelectListSlot m_slotSourceUnitTactic;

	public ScrollRect m_ScrollRect;

	public NKCUIComStateButton m_csbtnClear;

	public NKCComTMPUIText m_lbSelectCnt;

	private List<NKCPopupRecallSlot> m_lstRewardSlot = new List<NKCPopupRecallSlot>();

	private NKMRecallTemplet m_RecallTemplet;

	private NKMUnitData m_NKMUnitData;

	private int m_TargetUnitID;

	private NKM_UNIT_TYPE m_CurUnitType;

	private UnityAction m_dOK;

	private List<NKCPopupRecallMultiUnitSlot> m_lstMultiUnitSlots = new List<NKCPopupRecallMultiUnitSlot>();

	private int m_iMaxSelectRecallUnitCnt;

	private int m_iCurUnitSelectSlotIndex;

	private NKCUIUnitSelectList m_UIUnitSelectList;

	public static NKCPopupRecall Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupRecall>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_RECALL", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupRecall>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private bool IsTacticUnitRecallMode => m_iMaxSelectRecallUnitCnt > 1;

	private NKCUIUnitSelectList UnitSelectList
	{
		get
		{
			if (m_UIUnitSelectList == null)
			{
				m_UIUnitSelectList = NKCUIUnitSelectList.OpenNewInstance(bWillCloseUnderPopupOnOpen: false);
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
		if (m_UIUnitSelectList != null && NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_UNIT_LIST)
		{
			UnitSelectList.Close();
		}
		m_UIUnitSelectList = null;
		ClearMultiUnitSlot();
		base.gameObject.SetActive(value: false);
	}

	private void Init()
	{
		m_slotSourceUnit.Init();
		m_slotTargetUnit.Init();
		m_slotSourceShip.Init();
		m_slotTargetShip.Init();
		m_slotRefoundUnit.Init();
		NKCUtil.SetBindFunction(m_btnCancel, base.Close);
		NKCUtil.SetBindFunction(m_btnOK, OnClickOK);
		NKCUtil.SetBindFunction(m_csbtnClear, OnClickClear);
		m_btnOK.m_bGetCallbackWhileLocked = true;
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void UnHide()
	{
		base.UnHide();
		SetButtonState();
	}

	public void Open(NKMUnitData sourceUnitData)
	{
		if (sourceUnitData == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_NKMUnitData = sourceUnitData;
		m_TargetUnitID = 0;
		m_iMaxSelectRecallUnitCnt = 0;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(sourceUnitData);
		if (unitTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			Log.Error($"UnitTempletBase is null - ID : {sourceUnitData.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupRecall.cs", 172);
			return;
		}
		NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_PF_RECALL_POPUP_TITLE"));
		NKCUtil.SetGameobjectActive(m_objRefoundUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRecallUnit, bValue: true);
		m_CurUnitType = unitTempletBase.m_NKM_UNIT_TYPE;
		if (m_CurUnitType == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			m_RecallTemplet = NKMRecallTemplet.Find(sourceUnitData.m_UnitID, NKMTime.UTCtoLocal(NKCSynchronizedTime.GetServerUTCTime()));
			if (m_RecallTemplet.RecallItemCondition == Recall_Condition.TACTIC_UPDATE)
			{
				NKCUtil.SetLabelText(m_lbDesc, NKCUtilString.GET_STRING_RECALL_FINAL_CHECK_POPUP_DESC3);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString("SI_PF_RECALL_POPUP_SUBTITLE"));
			}
		}
		else if (m_CurUnitType == NKM_UNIT_TYPE.NUT_SHIP)
		{
			m_RecallTemplet = NKMRecallTemplet.Find(NKCRecallManager.GetFirstLevelShipID(sourceUnitData.m_UnitID), NKMTime.UTCtoLocal(NKCSynchronizedTime.GetServerUTCTime()));
			NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString("SI_PF_RECALL_POPUP_SHIP_SUBTITLE"));
		}
		if (m_RecallTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			Log.Error($"RecallTemplet is null - ID : {sourceUnitData.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupRecall.cs", 203);
		}
		else
		{
			UpdateRecallUI();
			UIOpened();
		}
	}

	public void Open(NKMUnitData refoundUnitData, UnityAction callBack)
	{
		if (refoundUnitData == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_NKMUnitData = refoundUnitData;
		m_TargetUnitID = 0;
		m_dOK = callBack;
		NKCUtil.SetLabelText(m_lbSourceType, NKCUtilString.GET_STRING_COLLECTION_UNIT);
		NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_TACTIC_UPDATE_REFOUND_POPUP_TITLE);
		NKCUtil.SetLabelText(m_lbDesc, NKCUtilString.GET_STRING_TACTIC_UPDATE_REFOUND_POPUP_DESC);
		NKCUtil.SetLabelText(m_lbOk, NKCUtilString.GET_STRING_DECK_BUTTON_OK);
		NKCUtil.SetBindFunction(m_btnOK, OnClickRefoundOK);
		NKCUtil.SetGameobjectActive(m_objRefoundUnit, bValue: true);
		NKCUtil.SetGameobjectActive(m_objRecallUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRefoundLimitCnt, bValue: false);
		SetUnitRefoundReward();
		SetButtonState();
		UIOpened();
	}

	private void UpdateRecallUI()
	{
		switch (m_CurUnitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			NKCUtil.SetLabelText(m_lbSourceType, NKCUtilString.GET_STRING_COLLECTION_UNIT);
			SetUnitRecallReward();
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
			NKCUtil.SetLabelText(m_lbSourceType, NKCUtilString.GET_STRING_COLLECTION_SHIP);
			SetShipRecallUI();
			break;
		default:
			Log.Debug("유닛 / 함선 외에는 리콜 대상이 아님", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupRecall.cs", 265);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetBindFunction(m_btnOK, OnClickOK);
		NKCUtil.SetGameobjectActive(m_objRefoundLimitCnt, bValue: false);
		SetButtonState();
	}

	private void ClearMultiUnitSlot()
	{
		for (int i = 0; i < m_lstMultiUnitSlots.Count; i++)
		{
			if (null != m_lstMultiUnitSlots[i])
			{
				Object.DestroyImmediate(m_lstMultiUnitSlots[i].gameObject);
				m_lstMultiUnitSlots[i] = null;
				m_lstMultiUnitSlots.RemoveAt(i);
				i--;
			}
		}
		m_lstMultiUnitSlots.Clear();
	}

	private NKCPopupRecallSlot GetRecallRewardSlot()
	{
		NKCPopupRecallSlot nKCPopupRecallSlot = Object.Instantiate(m_pfbSlot);
		if (nKCPopupRecallSlot != null)
		{
			nKCPopupRecallSlot.transform.SetParent(m_trRewardSlotParent);
			nKCPopupRecallSlot.transform.localPosition = Vector3.zero;
			nKCPopupRecallSlot.transform.localScale = Vector3.one;
		}
		return nKCPopupRecallSlot;
	}

	private NKCPopupRecallMultiUnitSlot GetRecallMultiUnitSlot()
	{
		NKCPopupRecallMultiUnitSlot nKCPopupRecallMultiUnitSlot = Object.Instantiate(m_pfbMultiUnitSlot);
		if (nKCPopupRecallMultiUnitSlot != null)
		{
			nKCPopupRecallMultiUnitSlot.transform.SetParent(m_trRewardSlotParent);
			nKCPopupRecallMultiUnitSlot.transform.localPosition = Vector3.zero;
			nKCPopupRecallMultiUnitSlot.transform.localScale = Vector3.one;
		}
		return nKCPopupRecallMultiUnitSlot;
	}

	private void SetButtonState()
	{
		if (IsTacticUnitRecallMode && m_CurUnitType == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			if (IsCanAddRecallUnit())
			{
				m_btnOK.Lock();
				NKCUtil.SetLabelTextColor(m_lbOk, NKCUtil.GetColor("#212122"));
			}
			else
			{
				m_btnOK.UnLock();
				NKCUtil.SetLabelTextColor(m_lbOk, NKCUtil.GetColor("#582817"));
			}
		}
		else if (m_CurUnitType == NKM_UNIT_TYPE.NUT_NORMAL && m_TargetUnitID == 0)
		{
			m_btnOK.Lock();
			NKCUtil.SetLabelTextColor(m_lbOk, NKCUtil.GetColor("#212122"));
		}
		else
		{
			m_btnOK.UnLock();
			NKCUtil.SetLabelTextColor(m_lbOk, NKCUtil.GetColor("#582817"));
		}
	}

	private void SetRewardSlot(string title, List<NKMItemMiscData> lstReward, int slotIdx)
	{
		if (slotIdx >= m_lstRewardSlot.Count)
		{
			m_lstRewardSlot.Add(GetRecallRewardSlot());
		}
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		for (int i = 0; i < lstReward.Count; i++)
		{
			list.Add(NKCUISlot.SlotData.MakeMiscItemData(lstReward[i]));
		}
		NKCUtil.SetGameobjectActive(m_lstRewardSlot[slotIdx], bValue: true);
		m_lstRewardSlot[slotIdx].SetData(title, list);
	}

	private void SetUnitRecallReward()
	{
		m_iMaxSelectRecallUnitCnt = ((m_RecallTemplet.RecallItemCondition == Recall_Condition.TACTIC_UPDATE) ? 1 : (m_NKMUnitData.tacticLevel + 1));
		if (IsTacticUnitRecallMode)
		{
			NKCUtil.SetGameobjectActive(m_objRecallUnit, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitTacticRecall, bValue: true);
			NKCUtil.SetGameobjectActive(m_objUnit, bValue: false);
			NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString("SI_PF_RECALL_POPUP_SUBTITLE_2"));
			m_slotSourceUnitTactic.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
			m_slotSourceUnitTactic.SetData(m_NKMUnitData, NKMDeckIndex.None, bEnableLayoutElement: false, null);
			ReadyToMultiUnitSelectUI();
			UpdateRecallUnitCnt();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objUnitTacticRecall, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnit, bValue: true);
			m_slotSourceUnit.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
			m_slotSourceUnit.SetData(m_NKMUnitData, NKMDeckIndex.None, bEnableLayoutElement: false, null);
			m_slotTargetUnit.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
			m_slotTargetUnit.SetMode(NKCUIUnitSelectListSlotBase.eUnitSlotMode.Add, bEnableLayoutElement: false, OnClickTargetUnitSlot);
		}
		NKCUtil.SetGameobjectActive(m_objShip, bValue: false);
		NKCUtil.SetGameobjectActive(m_objResource, bValue: true);
		SetUnitRewardSlot();
	}

	private void SetUnitRewardSlot()
	{
		int num = 0;
		for (int i = 0; i < m_lstRewardSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstRewardSlot[i], bValue: false);
		}
		List<NKMItemMiscData> list = NKCRecallManager.ConvertUnitExpToResources(m_NKMUnitData).Values.ToList();
		if (list.Count > 0)
		{
			list.Sort(SortByID);
		}
		SetRewardSlot(NKCUtilString.GET_STRING_SORT_LEVEL, list, num++);
		List<NKMItemMiscData> list2 = NKCRecallManager.ConvertLimitBreakToResources(m_NKMUnitData).Values.ToList();
		if (list2.Count > 0)
		{
			list2.Sort(SortByID);
		}
		SetRewardSlot(NKCStringTable.GetString("SI_DP_LAB_MENU_NAME_LDS_UNIT_LIMITBREAK"), list2, num++);
		List<NKMItemMiscData> list3 = NKCRecallManager.ConvertSkillToResources(m_NKMUnitData).Values.ToList();
		if (list3.Count > 0)
		{
			list3.Sort(SortByID);
		}
		SetRewardSlot(NKCStringTable.GetString("SI_DP_LAB_MENU_NAME_LDS_UNIT_SKILL_TRAIN"), list3, num++);
		List<NKMItemMiscData> list4 = NKCRecallManager.ConvertUnitLifeTimeToResource(m_NKMUnitData).Values.ToList();
		if (list4.Count > 0)
		{
			list4.Sort(SortByID);
		}
		SetRewardSlot(NKCStringTable.GetString("SI_PF_BASE_MENU_PERSONNEL_LIFETIME_TEXT"), list4, num++);
		List<NKMItemMiscData> list5 = NKCRecallManager.ConvertUnitReactorToResource(m_NKMUnitData).Values.ToList();
		if (list5.Count > 0)
		{
			list5.Sort(SortByID);
			SetRewardSlot(NKCStringTable.GetString("SI_DP_EQUIP_POSITION_REACTOR"), list5, num++);
		}
		List<NKMItemMiscData> list6 = NKCRecallManager.ConvertUnitRearmToResource(m_NKMUnitData).Values.ToList();
		if (list6.Count > 0)
		{
			list6.Sort(SortByID);
			SetRewardSlot(NKCStringTable.GetString("SI_PF_REARM_COST_TEXT"), list6, num++);
		}
		List<NKMItemMiscData> list7 = NKCRecallManager.ConvertRecallConditionToResources(m_RecallTemplet, m_NKMUnitData).Values.ToList();
		if (list7.Count > 0)
		{
			list7.Sort(SortByID);
			SetRewardSlot(NKCStringTable.GetString(m_RecallTemplet.RecallItemSlotName), list7, num++);
		}
	}

	private void SetShipRecallUI()
	{
		NKCUtil.SetGameobjectActive(m_objUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRecallUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUnitTacticRecall, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShip, bValue: true);
		NKCUtil.SetGameobjectActive(m_objResource, bValue: false);
		m_ShipModuleSource.SetData(m_NKMUnitData);
		m_slotSourceShip.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		m_slotSourceShip.SetData(m_NKMUnitData, NKMDeckIndex.None, bEnableLayoutElement: false, null);
		m_ShipModuleTarget.SetData(m_NKMUnitData);
		m_slotTargetShip.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		m_slotTargetShip.SetMode(NKCUIUnitSelectListSlotBase.eUnitSlotMode.Add, bEnableLayoutElement: false, OnClickTargetUnitSlot);
	}

	private void SetUnitRefoundReward()
	{
		NKCUtil.SetGameobjectActive(m_objUnit, bValue: true);
		NKCUtil.SetGameobjectActive(m_objShip, bValue: false);
		m_slotRefoundUnit.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		m_slotRefoundUnit.SetData(m_NKMUnitData, NKMDeckIndex.None, bEnableLayoutElement: false, null);
		SetUnitRewardSlot();
	}

	private int SortByID(NKMItemMiscData lItem, NKMItemMiscData rItem)
	{
		return lItem.ItemID.CompareTo(rItem.ItemID);
	}

	public void OnClickTargetUnitSlot(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		UnitSelectList.Open(GetUnitSelectListOption(), OnUnitSelected);
	}

	public void OnClickTargetOperatorSlot(NKMOperator operatorData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
	}

	private void OnUnitSelected(List<long> lstUnitUID)
	{
		m_TargetUnitID = (int)lstUnitUID.FirstOrDefault();
		UnitSelectList.Close();
		if (IsTacticUnitRecallMode)
		{
			NKCPopupRecallMultiUnitSlot nKCPopupRecallMultiUnitSlot = m_lstMultiUnitSlots[m_iCurUnitSelectSlotIndex];
			if (nKCPopupRecallMultiUnitSlot == null)
			{
				return;
			}
			if (nKCPopupRecallMultiUnitSlot.IsEmpleySlot && m_TargetUnitID != 0)
			{
				nKCPopupRecallMultiUnitSlot.SetUnitData(m_TargetUnitID);
			}
			else if (!nKCPopupRecallMultiUnitSlot.IsEmpleySlot)
			{
				if (m_TargetUnitID != 0)
				{
					nKCPopupRecallMultiUnitSlot.SetUnitData(m_TargetUnitID);
				}
				else
				{
					if (IsHasEmptySlot())
					{
						RemoveEmptyMultiSlot();
					}
					nKCPopupRecallMultiUnitSlot.SetEmpty();
				}
			}
			UpdateRecallUnitCnt();
			if (!IsHasEmptySlot() && IsCanAddRecallUnit())
			{
				AddMultiUnitslot();
			}
			SetButtonState();
			return;
		}
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(m_TargetUnitID);
		if (nKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(m_TargetUnitID, m_NKMUnitData.m_UnitLevel, m_NKMUnitData.m_LimitBreakLevel);
			if (nKMUnitData != null)
			{
				m_slotTargetShip.SetData(nKMUnitData, NKMDeckIndex.None, bEnableLayoutElement: false, OnClickTargetUnitSlot);
			}
		}
		else
		{
			m_slotTargetUnit.SetData(nKMUnitTempletBase, 1, 0, bEnableLayoutElement: false, OnClickTargetUnitSlot);
		}
	}

	private void OnClickClear()
	{
		ReadyToMultiUnitSelectUI();
		SetButtonState();
		UpdateRecallUnitCnt();
	}

	private void ReadyToMultiUnitSelectUI()
	{
		ClearMultiUnitSlot();
		AddMultiUnitslot();
	}

	private void AddMultiUnitslot()
	{
		NKCPopupRecallMultiUnitSlot recallMultiUnitSlot = GetRecallMultiUnitSlot();
		if (null != recallMultiUnitSlot)
		{
			recallMultiUnitSlot.Init(m_lstMultiUnitSlots.Count, OnClickMutiUnit, OnClickPlusUnitCnt, OnClickMinusUnitCnt);
			recallMultiUnitSlot.SetEmpty();
			recallMultiUnitSlot.transform.SetParent(m_ScrollRect.content);
			m_lstMultiUnitSlots.Add(recallMultiUnitSlot);
		}
	}

	private void OnClickMutiUnit(int slotIndex)
	{
		m_iCurUnitSelectSlotIndex = slotIndex;
		UnitSelectList.Open(GetUnitSelectListOption(), OnUnitSelected);
	}

	private bool IsHasEmptySlot()
	{
		foreach (NKCPopupRecallMultiUnitSlot lstMultiUnitSlot in m_lstMultiUnitSlots)
		{
			if (lstMultiUnitSlot.IsEmpleySlot)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsCanAddRecallUnit()
	{
		return GetCurRecallUnitTotalCount() < m_iMaxSelectRecallUnitCnt;
	}

	private int GetCurRecallUnitTotalCount()
	{
		int num = 0;
		foreach (NKCPopupRecallMultiUnitSlot lstMultiUnitSlot in m_lstMultiUnitSlots)
		{
			num += lstMultiUnitSlot.UnitCount;
		}
		return num;
	}

	private void OnClickPlusUnitCnt(int slotIndex)
	{
		if (!IsCanAddRecallUnit())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_PF_RECALL_ERROR_ALT_UNIT_SELECT_MAX"));
			return;
		}
		m_lstMultiUnitSlots[slotIndex].SetUnitCount(m_lstMultiUnitSlots[slotIndex].UnitCount + 1);
		UpdateRecallUnitCnt();
		if (!IsCanAddRecallUnit())
		{
			RemoveEmptyMultiSlot();
			SetButtonState();
		}
	}

	private void RemoveEmptyMultiSlot()
	{
		for (int i = 0; i < m_lstMultiUnitSlots.Count; i++)
		{
			if (m_lstMultiUnitSlots[i].IsEmpleySlot)
			{
				Object.DestroyImmediate(m_lstMultiUnitSlots[i].gameObject);
				m_lstMultiUnitSlots[i] = null;
				m_lstMultiUnitSlots.RemoveAt(i);
				break;
			}
		}
	}

	private void OnClickMinusUnitCnt(int slotIndex)
	{
		m_lstMultiUnitSlots[slotIndex].SetUnitCount(m_lstMultiUnitSlots[slotIndex].UnitCount - 1);
		UpdateRecallUnitCnt();
		if (IsCanAddRecallUnit() && !IsHasEmptySlot())
		{
			AddMultiUnitslot();
		}
		SetButtonState();
	}

	private void UpdateRecallUnitCnt()
	{
		NKCUtil.SetLabelText(m_lbSelectCnt, $"{GetCurRecallUnitTotalCount()}/{m_iMaxSelectRecallUnitCnt}");
	}

	public void OnClickOK()
	{
		if (IsTacticUnitRecallMode && m_CurUnitType == NKM_UNIT_TYPE.NUT_NORMAL && IsCanAddRecallUnit())
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_RECALL_ERROR_ALT_UNIT_SELECT);
		}
		else if (!IsTacticUnitRecallMode && m_CurUnitType == NKM_UNIT_TYPE.NUT_NORMAL && m_TargetUnitID <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_RECALL_ERROR_ALT_UNIT_SELECT);
		}
		else
		{
			if (m_RecallTemplet == null)
			{
				return;
			}
			if (!NKCRecallManager.IsValidTime(m_RecallTemplet, NKCSynchronizedTime.GetServerUTCTime()))
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_RECALL_PERIOD_EXPIRED);
			}
			else if (m_CurUnitType == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(m_NKMUnitData.m_UnitID);
				if (nKMUnitTempletBase != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine("[" + nKMUnitTempletBase.GetUnitTitle() + "] [" + nKMUnitTempletBase.GetUnitName() + "]");
					if (m_RecallTemplet.RecallItemCondition == Recall_Condition.TACTIC_UPDATE)
					{
						stringBuilder.AppendLine(NKCUtilString.GET_STRING_RECALL_FINAL_CHECK_POPUP_DESC3);
					}
					else
					{
						stringBuilder.AppendLine(NKCUtilString.GET_STRING_RECALL_FINAL_CHECK_POPUP_DESC2);
					}
					stringBuilder.Append(string.Format(NKCUtilString.GET_STRING_RECALL_FINAL_CHECK_POPUP_DATE, m_RecallTemplet.IntervalTemplet.GetStartDate(), m_RecallTemplet.IntervalTemplet.GetEndDate(), NKCSynchronizedTime.GetTimeLeftString(m_RecallTemplet.IntervalTemplet.GetEndDateUtc().Ticks)));
					NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, stringBuilder.ToString(), OnConfirm);
					stringBuilder.Clear();
				}
			}
			else if (m_CurUnitType == NKM_UNIT_TYPE.NUT_SHIP)
			{
				NKMUnitTempletBase nKMUnitTempletBase2 = NKMUnitTempletBase.Find(m_NKMUnitData.m_UnitID);
				if (nKMUnitTempletBase2 != null)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					stringBuilder2.AppendLine("[" + nKMUnitTempletBase2.GetUnitName() + "]");
					stringBuilder2.AppendLine(NKCUtilString.GET_STRING_RECALL_FINAL_CHECK_POPUP_DESC);
					stringBuilder2.Append(string.Format(NKCUtilString.GET_STRING_RECALL_FINAL_CHECK_POPUP_DATE, m_RecallTemplet.IntervalTemplet.GetStartDate(), m_RecallTemplet.IntervalTemplet.GetEndDate(), NKCSynchronizedTime.GetTimeLeftString(m_RecallTemplet.IntervalTemplet.GetEndDateUtc().Ticks)));
					NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, stringBuilder2.ToString(), OnConfirm);
					stringBuilder2.Clear();
				}
			}
		}
	}

	private void OnClickRefoundOK()
	{
		m_dOK?.Invoke();
		CheckInstanceAndClose();
	}

	public void OnConfirm()
	{
		if (IsTacticUnitRecallMode)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (NKCPopupRecallMultiUnitSlot lstMultiUnitSlot in m_lstMultiUnitSlots)
			{
				if (!lstMultiUnitSlot.IsEmpleySlot)
				{
					dictionary.Add(lstMultiUnitSlot.UnitID, lstMultiUnitSlot.UnitCount);
				}
			}
			NKCRecallManager.UnEquipAndRecall(m_NKMUnitData, dictionary);
		}
		else
		{
			NKCRecallManager.UnEquipAndRecall(m_NKMUnitData, new Dictionary<int, int> { { m_TargetUnitID, 1 } });
		}
	}

	public NKCUIUnitSelectList.UnitSelectListOptions GetUnitSelectListOption()
	{
		NKCUIUnitSelectList.UnitSelectListOptions result = new NKCUIUnitSelectList.UnitSelectListOptions(m_CurUnitType, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL, NKCUIUnitSelectList.eUnitSelectListMode.CUSTOM_LIST);
		result.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(m_CurUnitType, bIsCollection: false);
		result.bShowRemoveSlot = false;
		result.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		result.setExcludeUnitUID = new HashSet<long>();
		if (IsTacticUnitRecallMode && m_CurUnitType == NKM_UNIT_TYPE.NUT_NORMAL && m_lstMultiUnitSlots.Count > 1)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (NKCPopupRecallMultiUnitSlot lstMultiUnitSlot in m_lstMultiUnitSlots)
			{
				if (!lstMultiUnitSlot.IsEmpleySlot)
				{
					hashSet.Add(lstMultiUnitSlot.UnitID);
					result.setExcludeUnitID = hashSet;
				}
			}
		}
		result.m_bHideUnitCount = true;
		result.bDescending = true;
		result.bShowRemoveSlot = (IsTacticUnitRecallMode ? true : false);
		string strUpsideMenuName = "";
		result.strUpsideMenuName = strUpsideMenuName;
		result.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		result.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		result.setShipFilterCategory = NKCUnitSortSystem.setDefaultShipFilterCategory;
		result.setShipSortCategory = NKCUnitSortSystem.setDefaultShipSortCategory;
		result.bEnableLockUnitSystem = false;
		result.setOnlyIncludeUnitID = GetUnitIDs();
		result.m_bUseFavorite = true;
		return result;
	}

	private HashSet<int> GetUnitIDs()
	{
		List<NKMRecallUnitExchangeTemplet> list = NKMRecallUnitExchangeTemplet.GetUnitGroupTemplet(m_RecallTemplet.UnitExchangeGroupId).ToList();
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < list.Count; i++)
		{
			if (m_CurUnitType == NKM_UNIT_TYPE.NUT_SHIP)
			{
				char c = m_NKMUnitData.m_UnitID.ToString()[1];
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(list[i].UnitId);
				if (unitTempletBase != null && unitTempletBase.PickupEnableByTag)
				{
					char[] array = unitTempletBase.m_UnitID.ToString().ToCharArray();
					if (array.Length >= 1)
					{
						array[1] = c;
						int item = int.Parse(new string(array));
						hashSet.Add(item);
					}
				}
			}
			else
			{
				hashSet.Add(list[i].UnitId);
			}
		}
		return hashSet;
	}

	public static int GetRecallRewardCnt(NKMUnitData unitData)
	{
		List<NKMItemMiscData> list = NKCRecallManager.ConvertUnitExpToResources(unitData).Values.ToList();
		List<NKMItemMiscData> list2 = NKCRecallManager.ConvertLimitBreakToResources(unitData).Values.ToList();
		List<NKMItemMiscData> list3 = NKCRecallManager.ConvertSkillToResources(unitData).Values.ToList();
		List<NKMItemMiscData> list4 = NKCRecallManager.ConvertUnitLifeTimeToResource(unitData).Values.ToList();
		return list.Count + list2.Count + list3.Count + list4.Count;
	}
}
