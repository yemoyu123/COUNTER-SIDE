using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Unit;
using ClientPacket.User;
using ClientPacket.WorldMap;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUITacticUpdate : NKCUIBase
{
	private enum TACTIC_UPDATE_STEP
	{
		INTRO,
		READY,
		BACK
	}

	public delegate void dLastSelectedUnitData(NKMUnitData unit);

	[Serializable]
	public class NodeSlot
	{
		public List<Image> lstIcon;

		public List<Text> lbDesc;

		public GameObject objON;

		public GameObject objOFF;

		public GameObject objFX;

		public GameObject objMaxLevel;

		public GameObject objMaxLevelFX;
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_info";

	private const string UI_ASSET_NAME = "AB_UI_TACTIC_UPDATE";

	private static NKCUITacticUpdate m_Instance;

	public Animator m_Ani;

	public Text m_lbName;

	public Text m_lbTitle;

	public GameObject m_objReadyBtnON;

	public GameObject m_objReadyBtnOFF;

	public NKCUIComStateButton m_csbtnReady;

	public NKCUITacticUpdateLevelSlot m_TacticUpdateLvSlot;

	public NKCUIComStateButton m_csbtnRun;

	public NKCUIComDragSelectablePanel m_DragCharacterView;

	public NKCUIComRaycastTarget m_DragCharacterViewRayCastTarget;

	public NKCUIComStateButton m_csbtnUnitSelect;

	[Header("right ui")]
	public NKCUITacticUpdateLevelSlot m_TargetTacticUpdateLvSlot;

	public Image m_imgTargetUpdateIcon;

	public Text m_lbTargetUpdateDesc;

	public NKCUIUnitSelectListSlot m_UnitSelectListSlot;

	[Header("FX")]
	public GameObject m_objFX;

	public Animator m_FXAni;

	public GameObject m_objMaxLevelFX;

	[Space]
	public List<GameObject> m_lstCenterNodeCompleteFX;

	public List<GameObject> m_lstBottomNodeReadyFX;

	private bool m_bPlayingFX;

	[Header("Update Ani Setting")]
	public float m_fNodeUpdateDelay = 1f;

	public float m_fChangeAniDelayWhenMaxLevel = 1f;

	public float m_fCenterMaxFXDelay = 1f;

	public float m_fChangeNodeColorDelay = 0.4f;

	private TACTIC_UPDATE_STEP m_curTacticUpdateStep;

	private List<NKMUnitData> m_lstSelectedUnitData;

	private dLastSelectedUnitData m_lastSelectedUnitCallBack;

	private NKMUnitData m_targetUnitData;

	private List<NKMUnitData> m_lstConsumeUnitData;

	private int m_iUnEquipCnt;

	private NKMUnitData m_UnEquipTargetUnit;

	public List<NodeSlot> m_lstTacticUpdateNodeCenter = new List<NodeSlot>();

	public List<NodeSlot> m_lstTacticUpdateNodeBottom = new List<NodeSlot>();

	public List<NodeSlot> m_lstTacticUpdateNodeRight = new List<NodeSlot>();

	private NKCUIUnitSelectList m_UIUnitSelectList;

	private const string ANI_TRIGGER_INTRO = "INTRO";

	private const string ANI_TRIGGER_READY = "READY";

	private const string ANI_TRIGGER_BACK = "BACK";

	private const string ANI_TRIGGER_READY_IDLE = "READY_IDLE";

	private const string ACTIVE = "ACTIVE";

	public static NKCUITacticUpdate Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUITacticUpdate>("ab_ui_nkm_ui_unit_info", "AB_UI_TACTIC_UPDATE", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUITacticUpdate>();
			}
			m_Instance.Initialize();
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

	public override string GuideTempletID => "ARTICLE_UNIT_TACTIC_UPDATE";

	public override string MenuName => NKCUtilString.GET_STRING_TACTIC_UPDATE;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.LeftsideOnly;

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
		m_UIUnitSelectList?.Close();
		m_UIUnitSelectList = null;
		m_lastSelectedUnitCallBack?.Invoke(m_targetUnitData);
		BannerCleanUp();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void Initialize()
	{
		NKCUtil.SetBindFunction(m_csbtnReady, OnClickTacticReady);
		NKCUtil.SetBindFunction(m_csbtnRun, OnClickTacticUpdate);
		m_UnitSelectListSlot.Init(resetLocalScale: true);
		NKCUtil.SetBindFunction(m_csbtnUnitSelect, OnClickUnitSelect);
		if (m_DragCharacterView != null)
		{
			m_DragCharacterView.Init(rotation: true);
			m_DragCharacterView.dOnGetObject += MakeMainBannerListSlot;
			m_DragCharacterView.dOnReturnObject += ReturnMainBannerListSlot;
			m_DragCharacterView.dOnProvideData += ProvideMainBannerListSlotData;
			m_DragCharacterView.dOnIndexChangeListener += SelectCharacter;
			m_DragCharacterView.dOnFocus += Focus;
		}
	}

	public override void OnBackButton()
	{
		if (m_bPlayingFX)
		{
			return;
		}
		if (m_curTacticUpdateStep == TACTIC_UPDATE_STEP.READY)
		{
			UpdateTacticUpdateUI(TACTIC_UPDATE_STEP.BACK);
			return;
		}
		foreach (GameObject item in m_lstBottomNodeReadyFX)
		{
			NKCUtil.SetGameobjectActive(item, bValue: false);
		}
		m_bPlayingFX = false;
		base.OnBackButton();
	}

	public override void Hide()
	{
		base.Hide();
		NKCUtil.SetGameobjectActive(m_objMaxLevelFX, bValue: false);
	}

	public override void UnHide()
	{
		base.UnHide();
		if (m_curTacticUpdateStep == TACTIC_UPDATE_STEP.READY)
		{
			SetAni("READY_IDLE");
		}
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		base.OnUnitUpdate(eEventType, eUnitType, uid, unitData);
		switch (eEventType)
		{
		case NKMUserData.eChangeNotifyType.Update:
		{
			for (int num2 = 0; num2 < m_lstSelectedUnitData.Count; num2++)
			{
				if (m_lstSelectedUnitData[num2] != null && m_lstSelectedUnitData[num2].m_UnitUID == uid)
				{
					m_lstSelectedUnitData[num2] = unitData;
					break;
				}
			}
			break;
		}
		case NKMUserData.eChangeNotifyType.Remove:
		{
			int num = m_lstSelectedUnitData.FindIndex((NKMUnitData v) => v.m_UnitUID == uid);
			if (num >= 0)
			{
				m_lstSelectedUnitData.RemoveAt(num);
				UpdateChangeUnitUI();
			}
			break;
		}
		}
	}

	public void Open(NKMUnitData unitData, List<NKMUnitData> lstUnitData, dLastSelectedUnitData callBack)
	{
		Open(unitData);
		if (lstUnitData.Count <= 0)
		{
			lstUnitData.Add(unitData);
		}
		m_lstSelectedUnitData = lstUnitData;
		UpdateChangeUnitUI();
		m_lastSelectedUnitCallBack = callBack;
	}

	public void Open(long tacticUpdateUnitUID)
	{
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(tacticUpdateUnitUID);
		Open(unitFromUID);
	}

	public void Open(NKMUnitData unitData)
	{
		if (unitData != null)
		{
			m_lstSelectedUnitData = null;
			NKCUtil.SetGameobjectActive(m_objFX, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMaxLevelFX, bValue: false);
			base.gameObject.SetActive(value: true);
			m_lstConsumeUnitData = new List<NKMUnitData>();
			m_targetUnitData = unitData;
			UpdateUnitUI();
			UpdateTacticUpdateUI(TACTIC_UPDATE_STEP.INTRO, bRefresh: true);
			UpdateNode(ref m_lstTacticUpdateNodeRight);
			UpdateRightNodeUI();
			UIOpened();
		}
	}

	private void UpdateUnitUI()
	{
		m_UnitSelectListSlot.SetEmpty(bEnableLayoutElement: true, OnSelectUnitSlot);
		NKCUtil.SetLabelText(m_lbName, m_targetUnitData.GetUnitTempletBase().GetUnitName());
		NKCUtil.SetLabelText(m_lbTitle, m_targetUnitData.GetUnitTempletBase().GetUnitTitle());
		NKCUtil.SetGameobjectActive(m_objReadyBtnON, m_targetUnitData.tacticLevel != 6);
		NKCUtil.SetGameobjectActive(m_objReadyBtnOFF, m_targetUnitData.tacticLevel == 6);
	}

	private void OnClickTacticReady()
	{
		if (m_targetUnitData.tacticLevel == 6)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_TACTIC_UPDATE_MAX_UNIT);
		}
		else
		{
			UpdateTacticUpdateUI(TACTIC_UPDATE_STEP.READY);
		}
	}

	private void OnClickTacticUpdate()
	{
		switch (m_curTacticUpdateStep)
		{
		case TACTIC_UPDATE_STEP.INTRO:
			UpdateTacticUpdateUI(TACTIC_UPDATE_STEP.READY);
			break;
		case TACTIC_UPDATE_STEP.READY:
			if (m_iUnEquipCnt <= 0)
			{
				OnTryTacticUpdate();
			}
			break;
		case TACTIC_UPDATE_STEP.BACK:
			UpdateTacticUpdateUI(TACTIC_UPDATE_STEP.READY);
			break;
		default:
			Debug.Log($"<color=red>OnClickTacticUpdate : {m_curTacticUpdateStep}</color>");
			break;
		}
	}

	private bool IsPocessibleStatusConsumeUnit()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (m_lstConsumeUnitData.Count <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TACTIC_UPDATE_UNIT_SELECT_NONE);
			return false;
		}
		List<NKMUnitData> _lstEquipedUnits = new List<NKMUnitData>();
		foreach (NKMUnitData lstConsumeUnitDatum in m_lstConsumeUnitData)
		{
			NKMEquipmentSet equipmentSet = lstConsumeUnitDatum.GetEquipmentSet(nKMUserData.m_InventoryData);
			if (equipmentSet.Weapon != null || equipmentSet.Defence != null || equipmentSet.Accessory != null || equipmentSet.Accessory2 != null)
			{
				_lstEquipedUnits.Add(lstConsumeUnitDatum);
			}
		}
		if (_lstEquipedUnits.Count > 0)
		{
			m_iUnEquipCnt = 0;
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TACTIC_UPDATE_BLOCK_MESSAGE_EQUIPED, delegate
			{
				foreach (NKMUnitData item in _lstEquipedUnits)
				{
					UnEquipAllItems(item);
				}
			});
			return false;
		}
		foreach (NKMUnitData consumeUnit in m_lstConsumeUnitData)
		{
			if (consumeUnit.m_bLock || nKMUserData.m_ArmyData.IsUnitInAnyDeck(consumeUnit.m_UnitUID))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_TACTIC_UPDATE_UNIT_WARNING);
				return false;
			}
			if (nKMUserData.backGroundInfo.unitInfoList.Find((NKMBackgroundUnitInfo e) => e.unitUid == consumeUnit.m_UnitUID) != null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_TACTIC_UPDATE_UNIT_WARNING);
				return false;
			}
			foreach (NKMWorldMapCityData value in nKMUserData.m_WorldmapData.worldMapCityDataMap.Values)
			{
				if (value.leaderUnitUID == consumeUnit.m_UnitUID)
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_TACTIC_UPDATE_UNIT_WARNING);
					return false;
				}
			}
			if (consumeUnit.OfficeRoomId > 0)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_TACTIC_UPDATE_UNIT_WARNING);
				return false;
			}
		}
		return true;
	}

	private void OnTryTacticUpdate()
	{
		if (!IsPocessibleStatusConsumeUnit())
		{
			return;
		}
		string consumeUnitChceckDesc = GetConsumeUnitChceckDesc();
		NKCPopupUnitConfirm.Instance.Open(m_lstConsumeUnitData, NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TACTIC_UPDATE_DESC, consumeUnitChceckDesc, delegate
		{
			List<long> list = new List<long>();
			foreach (NKMUnitData lstConsumeUnitDatum in m_lstConsumeUnitData)
			{
				list.Add(lstConsumeUnitDatum.m_UnitUID);
			}
			NKCPacketSender.Send_NKMPacket_UNIT_TACTIC_UPDATE_REQ(m_targetUnitData.m_UnitUID, list);
		});
	}

	private string GetConsumeUnitChceckDesc()
	{
		foreach (NKMUnitData lstConsumeUnitDatum in m_lstConsumeUnitData)
		{
			if (lstConsumeUnitDatum.reactorLevel > 0)
			{
				return NKCUtilString.GET_STRING_UNIT_REACTOR_REMOVE_WARNING_DESC;
			}
		}
		foreach (NKMUnitData lstConsumeUnitDatum2 in m_lstConsumeUnitData)
		{
			if (NKCRearmamentUtil.IsHasLeaderSkill(lstConsumeUnitDatum2))
			{
				return NKCUtilString.GET_STRING_TACTIC_UPDATE_DESC_REARM;
			}
		}
		foreach (NKMUnitData lstConsumeUnitDatum3 in m_lstConsumeUnitData)
		{
			if (lstConsumeUnitDatum3.tacticLevel > 0)
			{
				return NKCUtilString.GET_STRING_TACTIC_UPDATE_DESC_TACTIC;
			}
		}
		return "";
	}

	private void UnEquipAllItems(NKMUnitData targetUnit)
	{
		if (targetUnit != null)
		{
			m_UnEquipTargetUnit = targetUnit;
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			NKMEquipmentSet equipmentSet = m_UnEquipTargetUnit.GetEquipmentSet(nKMUserData.m_InventoryData);
			SendUnEquipItemREQ(m_UnEquipTargetUnit.m_UnitUID, equipmentSet.Weapon, ITEM_EQUIP_POSITION.IEP_WEAPON);
			SendUnEquipItemREQ(m_UnEquipTargetUnit.m_UnitUID, equipmentSet.Defence, ITEM_EQUIP_POSITION.IEP_DEFENCE);
			SendUnEquipItemREQ(m_UnEquipTargetUnit.m_UnitUID, equipmentSet.Accessory, ITEM_EQUIP_POSITION.IEP_ACC);
			SendUnEquipItemREQ(m_UnEquipTargetUnit.m_UnitUID, equipmentSet.Accessory2, ITEM_EQUIP_POSITION.IEP_ACC2);
			if (m_iUnEquipCnt == 0)
			{
				OnCompletUnEquip();
			}
		}
	}

	private void SendUnEquipItemREQ(long unitUID, NKMEquipItemData equipData, ITEM_EQUIP_POSITION position)
	{
		if (equipData != null)
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: false, unitUID, equipData.m_ItemUid, position);
			m_iUnEquipCnt++;
		}
	}

	private void OnCompletUnEquip()
	{
		if (m_UnEquipTargetUnit != null && m_UnitSelectListSlot.NKMUnitData.m_UnitUID == m_UnEquipTargetUnit.m_UnitUID)
		{
			m_UnitSelectListSlot.SetData(m_UnEquipTargetUnit, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), bEnableLayoutElement: false, OnSelectUnitSlot);
		}
		OnTryTacticUpdate();
	}

	public void OnUnEquipComplete()
	{
		m_iUnEquipCnt--;
		if (m_iUnEquipCnt == 0)
		{
			if (m_lstConsumeUnitData[0] != null)
			{
				m_UnitSelectListSlot.SetData(m_lstConsumeUnitData[0], new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), bEnableLayoutElement: false, OnSelectUnitSlot);
				m_UnitSelectListSlot.SetTacticSelectUnitCnt(m_lstConsumeUnitData.Count);
			}
			OnCompletUnEquip();
		}
	}

	private void UpdateTacticUpdateUI(TACTIC_UPDATE_STEP step, bool bRefresh = false)
	{
		if (bRefresh)
		{
			UpdateNodeUI(bRefresh);
		}
		switch (step)
		{
		case TACTIC_UPDATE_STEP.INTRO:
			SetAni("INTRO");
			break;
		case TACTIC_UPDATE_STEP.READY:
			SetAni("READY");
			break;
		case TACTIC_UPDATE_STEP.BACK:
			SetAni("BACK");
			break;
		}
		m_curTacticUpdateStep = step;
		m_DragCharacterViewRayCastTarget.enabled = step != TACTIC_UPDATE_STEP.READY;
	}

	private void UpdateNodeUI(bool bRefreshCenter = false)
	{
		UpdateNode(ref m_lstTacticUpdateNodeCenter, bRefreshCenter);
		UpdateNode(ref m_lstTacticUpdateNodeBottom);
		m_TacticUpdateLvSlot.SetLevel(m_targetUnitData.tacticLevel);
		m_TargetTacticUpdateLvSlot.SetLevel(m_targetUnitData.tacticLevel);
	}

	private string GetTacticUpdateDesc(NKMTacticUpdateTemplet.TacticUpdateData targetTacticUpdateTemplet)
	{
		string arg = (targetTacticUpdateTemplet.m_StatValue * 0.01f).ToString("N2");
		return string.Format(NKCStringTable.GetString(targetTacticUpdateTemplet.m_StringKey), arg);
	}

	private void UpdateNode(ref List<NodeSlot> lstNodeSlots, bool bRefreshCenter = false)
	{
		NKMUnitTempletBase unitTempletBase = m_targetUnitData.GetUnitTempletBase();
		int tacticLevel = m_targetUnitData.tacticLevel;
		List<NKMTacticUpdateTemplet.TacticUpdateData> list = NKMTacticUpdateTemplet.Find(unitTempletBase).m_dicTacticData.Values.OrderBy((NKMTacticUpdateTemplet.TacticUpdateData e) => e.m_TacticPhase).ToList();
		for (int num = 0; num < lstNodeSlots.Count; num++)
		{
			if (list[num] == null)
			{
				NKCUtil.SetGameobjectActive(lstNodeSlots[num].objOFF, bValue: true);
				NKCUtil.SetGameobjectActive(lstNodeSlots[num].objFX, bValue: false);
				NKCUtil.SetGameobjectActive(lstNodeSlots[num].objON, bValue: false);
				continue;
			}
			Sprite nodeStatIcon = GetNodeStatIcon(list[num].m_StatIcon);
			foreach (Image item in lstNodeSlots[num].lstIcon)
			{
				NKCUtil.SetImageSprite(item, nodeStatIcon);
			}
			string tacticUpdateDesc = GetTacticUpdateDesc(list[num]);
			foreach (Text item2 in lstNodeSlots[num].lbDesc)
			{
				NKCUtil.SetLabelText(item2, tacticUpdateDesc);
			}
			if (tacticLevel == 6 && bRefreshCenter)
			{
				NKCUtil.SetGameobjectActive(lstNodeSlots[num].objMaxLevel, bValue: true);
				NKCUtil.SetGameobjectActive(lstNodeSlots[num].objMaxLevelFX, bValue: true);
				NKCUtil.SetGameobjectActive(lstNodeSlots[num].objON, bValue: false);
				NKCUtil.SetGameobjectActive(lstNodeSlots[num].objOFF, bValue: false);
				NKCUtil.SetGameobjectActive(lstNodeSlots[num].objFX, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(lstNodeSlots[num].objMaxLevel, bValue: false);
			NKCUtil.SetGameobjectActive(lstNodeSlots[num].objMaxLevelFX, bValue: false);
			NKCUtil.SetGameobjectActive(lstNodeSlots[num].objON, num < tacticLevel);
			NKCUtil.SetGameobjectActive(lstNodeSlots[num].objOFF, num >= tacticLevel);
			if (bRefreshCenter)
			{
				NKCUtil.SetGameobjectActive(lstNodeSlots[num].objFX, num < tacticLevel);
			}
		}
	}

	private Sprite GetNodeStatIcon(string assetName)
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_unit_info_texture", assetName);
	}

	private void OnClickSelectResourceUnit()
	{
		NKCUtil.SetGameobjectActive(m_objFX, bValue: false);
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
		options.bDescending = false;
		options.bShowRemoveSlot = false;
		options.bExcludeLockedUnit = false;
		options.bExcludeDeckedUnit = false;
		options.m_SortOptions.bUseDeckedState = false;
		options.m_SortOptions.bUseLockedState = false;
		options.m_SortOptions.bUseDormInState = false;
		options.dOnSelectedUnitWarning = null;
		options.m_SortOptions.bIgnoreCityState = true;
		options.m_SortOptions.bIgnoreWorldMapLeader = true;
		options.bShowHideDeckedUnitMenu = false;
		options.bHideDeckedUnit = false;
		options.setOnlyIncludeUnitID = new HashSet<int> { m_targetUnitData.GetUnitTempletBase().m_UnitID };
		foreach (int sameBaseUnitID in NKCRearmamentUtil.GetSameBaseUnitIDList(m_targetUnitData))
		{
			options.setOnlyIncludeUnitID.Add(sameBaseUnitID);
		}
		options.setSelectedUnitUID = new HashSet<long>();
		if (m_lstConsumeUnitData.Count > 0)
		{
			foreach (NKMUnitData lstConsumeUnitDatum in m_lstConsumeUnitData)
			{
				if (NKCScenManager.CurrentArmyData().GetUnitFromUID(lstConsumeUnitDatum.m_UnitUID) != null)
				{
					options.setSelectedUnitUID.Add(lstConsumeUnitDatum.m_UnitUID);
				}
			}
		}
		options.setExcludeUnitUID = new HashSet<long> { m_targetUnitData.m_UnitUID };
		options.setExcludeUnitID = NKCUnitSortSystem.GetDefaultExcludeUnitIDs();
		options.strEmptyMessage = NKCUtilString.GET_STRING_TACTIC_UPDATE_UNIT_SELECT_LIST_EMPTY_MESSAGE;
		options.dOnSlotSetData = null;
		options.setUnitFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
		{
			NKCUnitSortSystem.eFilterCategory.Level,
			NKCUnitSortSystem.eFilterCategory.Locked,
			NKCUnitSortSystem.eFilterCategory.Decked
		};
		options.setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>
		{
			NKCUnitSortSystem.eSortCategory.Level,
			NKCUnitSortSystem.eSortCategory.UnitLoyalty,
			NKCUnitSortSystem.eSortCategory.Deploy_Status
		};
		options.m_bHideUnitCount = true;
		options.m_bUseFavorite = false;
		options.bMultipleSelect = true;
		options.iMaxMultipleSelect = 6 - m_targetUnitData.tacticLevel;
		options.bShowFromContractUnit = true;
		UnitSelectList.Open(options, OnConsumeUnitSelected);
	}

	public void OnConsumeUnitSelected(List<long> selectedList)
	{
		if (m_UIUnitSelectList != null && m_UIUnitSelectList.IsOpen)
		{
			m_UIUnitSelectList.Close();
		}
		if (selectedList.Count <= 0)
		{
			return;
		}
		m_lstConsumeUnitData.Clear();
		foreach (long selected in selectedList)
		{
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(selected);
			if (unitFromUID != null)
			{
				m_lstConsumeUnitData.Add(unitFromUID);
			}
		}
		if (m_lstConsumeUnitData.Count <= 0)
		{
			return;
		}
		if (m_lstConsumeUnitData[0] != null)
		{
			m_UnitSelectListSlot.SetData(m_lstConsumeUnitData[0], new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), bEnableLayoutElement: false, OnSelectUnitSlot);
			m_UnitSelectListSlot.SetTacticSelectUnitCnt(selectedList.Count);
		}
		int tacticLevel = m_targetUnitData.tacticLevel;
		int count = m_lstConsumeUnitData.Count;
		for (int i = 0; i < m_lstBottomNodeReadyFX.Count; i++)
		{
			if (i >= tacticLevel && i < tacticLevel + count)
			{
				NKCUtil.SetGameobjectActive(m_lstBottomNodeReadyFX[i], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstBottomNodeReadyFX[i], bValue: false);
			}
		}
		UpdateRightNodeUI();
		m_TargetTacticUpdateLvSlot.SetActiveLevel(m_targetUnitData.tacticLevel, m_targetUnitData.tacticLevel + m_lstConsumeUnitData.Count);
	}

	private void UpdateRightNodeUI()
	{
		for (int i = 0; i < m_lstTacticUpdateNodeRight.Count; i++)
		{
			if (i < m_targetUnitData.tacticLevel)
			{
				NKCUtil.SetGameobjectActive(m_lstTacticUpdateNodeRight[i].objON, bValue: false);
			}
			else if (i + 1 > m_targetUnitData.tacticLevel + m_lstConsumeUnitData.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstTacticUpdateNodeRight[i].objON, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstTacticUpdateNodeRight[i].objON, bValue: true);
			}
		}
	}

	public void OnSelectUnitSlot(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		if (m_curTacticUpdateStep != TACTIC_UPDATE_STEP.BACK)
		{
			OnClickSelectResourceUnit();
		}
	}

	private void SetAni(string aniName)
	{
		m_Ani.SetTrigger(aniName);
	}

	private IEnumerator PlayCompleteFX(int iStart, int iEnd)
	{
		m_bPlayingFX = true;
		int iStartLv = iStart;
		while (iStartLv != iEnd)
		{
			NKCUtil.SetGameobjectActive(m_lstCenterNodeCompleteFX[iStartLv], bValue: true);
			NKCUtil.SetGameobjectActive(m_lstTacticUpdateNodeCenter[iStartLv].objON, bValue: true);
			NKCUtil.SetGameobjectActive(m_lstTacticUpdateNodeCenter[iStartLv].objFX, bValue: true);
			NKCUtil.SetGameobjectActive(m_lstTacticUpdateNodeBottom[iStartLv].objON, bValue: true);
			int num = iStartLv + 1;
			iStartLv = num;
			yield return new WaitForSeconds(m_fNodeUpdateDelay);
		}
		if (iStartLv == 6)
		{
			yield return new WaitForSeconds(m_fChangeAniDelayWhenMaxLevel);
			UpdateTacticUpdateUI(TACTIC_UPDATE_STEP.BACK);
			yield return new WaitForSeconds(m_fCenterMaxFXDelay);
			NKCUtil.SetGameobjectActive(m_objMaxLevelFX, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMaxLevelFX, bValue: true);
			yield return new WaitForSeconds(m_fChangeNodeColorDelay);
			UpdateNodeUI(bRefreshCenter: true);
		}
		else
		{
			UpdateNodeUI(bRefreshCenter: true);
		}
		m_bPlayingFX = false;
	}

	public void OnRecv(NKMPacket_UNIT_TACTIC_UPDATE_ACK sPacket)
	{
		if (sPacket.unitData.m_UnitUID == m_targetUnitData.m_UnitUID)
		{
			StartCoroutine(PlayCompleteFX(m_targetUnitData.tacticLevel, sPacket.unitData.tacticLevel));
			m_targetUnitData = sPacket.unitData;
			NKCUtil.SetGameobjectActive(m_objFX, bValue: true);
			m_FXAni.SetTrigger("ACTIVE");
			foreach (GameObject item in m_lstBottomNodeReadyFX)
			{
				NKCUtil.SetGameobjectActive(item, bValue: false);
			}
		}
		m_lstConsumeUnitData.Clear();
		UpdateRightNodeUI();
		m_UnitSelectListSlot.SetEmpty(bEnableLayoutElement: true, OnSelectUnitSlot);
	}

	public static int CanThisUnitTactocUpdateNow(NKMUnitData unitData, NKMUserData userData)
	{
		if (unitData == null || userData == null)
		{
			return -1;
		}
		if (unitData.tacticLevel == 6)
		{
			return -1;
		}
		HashSet<int> hashSet = new HashSet<int>();
		hashSet.Add(unitData.GetUnitTempletBase().m_UnitID);
		foreach (int sameBaseUnitID in NKCRearmamentUtil.GetSameBaseUnitIDList(unitData))
		{
			hashSet.Add(sameBaseUnitID);
		}
		int num = 0;
		foreach (KeyValuePair<long, NKMUnitData> item in userData.m_ArmyData.m_dicMyUnit)
		{
			if (item.Key != unitData.m_UnitUID && !item.Value.m_bLock && !unitData.IsSeized && hashSet.Contains(item.Value.m_UnitID))
			{
				num++;
			}
		}
		return num;
	}

	private void UpdateChangeUnitUI()
	{
		int index = m_lstSelectedUnitData.FindIndex((NKMUnitData v) => v.m_UnitUID == m_targetUnitData.m_UnitUID);
		m_DragCharacterView.TotalCount = m_lstSelectedUnitData.Count;
		m_DragCharacterView.SetIndex(index);
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
		if (m_lstSelectedUnitData == null || m_lstSelectedUnitData.Count <= idx)
		{
			return;
		}
		NKMUnitData nKMUnitData = m_lstSelectedUnitData[idx];
		if (nKMUnitData != null)
		{
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
		UnityEngine.Object.Destroy(go.gameObject);
	}

	private void Focus(RectTransform rect, bool bFocus)
	{
		NKCUtil.SetGameobjectActive(rect.gameObject, bFocus);
	}

	public void SelectCharacter(int idx)
	{
		if (m_bPlayingFX)
		{
			return;
		}
		if (m_lstSelectedUnitData.Count <= idx || idx < 0)
		{
			Debug.LogWarning($"Error - Count : {m_lstSelectedUnitData.Count}, Index : {idx}");
			return;
		}
		NKMUnitData nKMUnitData = m_lstSelectedUnitData[idx];
		if (nKMUnitData != null)
		{
			m_targetUnitData = nKMUnitData;
			UpdateUnitUI();
			UpdateNodeUI(bRefreshCenter: true);
			m_lstConsumeUnitData.Clear();
			UpdateRightNodeUI();
		}
	}

	private void BannerCleanUp()
	{
		if (!(m_DragCharacterView != null))
		{
			return;
		}
		NKCUICharacterView[] componentsInChildren = m_DragCharacterView.gameObject.GetComponentsInChildren<NKCUICharacterView>(includeInactive: true);
		if (componentsInChildren != null)
		{
			NKCUICharacterView[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].CloseImmediatelyIllust();
			}
		}
	}

	private void OnClickUnitSelect()
	{
		if (m_curTacticUpdateStep == TACTIC_UPDATE_STEP.READY)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objFX, bValue: false);
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
		options.bDescending = false;
		options.bShowRemoveSlot = false;
		options.bExcludeLockedUnit = false;
		options.bExcludeDeckedUnit = false;
		options.m_SortOptions.bUseDeckedState = false;
		options.m_SortOptions.bUseLockedState = false;
		options.m_SortOptions.bUseDormInState = false;
		options.dOnSelectedUnitWarning = null;
		options.m_SortOptions.bIgnoreCityState = true;
		options.m_SortOptions.bIgnoreWorldMapLeader = true;
		options.bShowHideDeckedUnitMenu = false;
		options.bHideDeckedUnit = false;
		options.setOnlyIncludeUnitID = new HashSet<int>();
		options.setExcludeUnitUID = new HashSet<long>();
		options.setOnlyIncludeUnitID.Add(m_targetUnitData.GetUnitTempletBase().m_UnitID);
		foreach (KeyValuePair<long, NKMUnitData> item in NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyUnit)
		{
			int num = CanThisUnitTactocUpdateNow(item.Value, NKCScenManager.CurrentUserData());
			if (0 < num)
			{
				options.setOnlyIncludeUnitID.Add(item.Value.m_UnitID);
				Debug.Log($"<color=red>OnClickUnitSelect : {num} : {item.Value.GetUnitTempletBase().GetUnitName()}</color>");
			}
			if (item.Value.tacticLevel == 6)
			{
				options.setExcludeUnitUID.Add(item.Value.m_UnitUID);
			}
		}
		foreach (int sameBaseUnitID in NKCRearmamentUtil.GetSameBaseUnitIDList(m_targetUnitData))
		{
			options.setOnlyIncludeUnitID.Add(sameBaseUnitID);
		}
		options.setExcludeUnitID = NKCUnitSortSystem.GetDefaultExcludeUnitIDs();
		options.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		options.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		options.strEmptyMessage = NKCUtilString.GET_STRING_TACTIC_UPDATE_UNIT_SELECT_LIST_EMPTY_MESSAGE;
		options.dOnSlotSetData = null;
		options.m_bHideUnitCount = true;
		options.m_bUseFavorite = false;
		options.bMultipleSelect = false;
		options.iMaxMultipleSelect = 1;
		UnitSelectList.Open(options, null, OnUnitSortList);
	}

	private void OnUnitSortList(long UID, List<NKMUnitData> unitUIDList)
	{
		if (m_UIUnitSelectList != null && m_UIUnitSelectList.IsOpen)
		{
			m_UIUnitSelectList.Close();
		}
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(UID);
		if (unitFromUID != null)
		{
			m_lstSelectedUnitData = unitUIDList;
			m_targetUnitData = unitFromUID;
			UpdateChangeUnitUI();
		}
	}
}
