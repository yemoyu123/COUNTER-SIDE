using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupShipCommandModule : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_ship_info";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SHIP_MODULE";

	private static NKCPopupShipCommandModule m_Instance;

	public Animator m_Ani;

	public Image m_imgShip;

	public Image m_imgShip_02;

	public Text m_lbShipName;

	public Text m_lbShipName_02;

	public List<GameObject> m_lstModuleStep = new List<GameObject>();

	public List<NKCPopupShipCommandModuleSlot> m_lstModuleSlot = new List<NKCPopupShipCommandModuleSlot>();

	public GameObject m_objTargetSocket_01_ON;

	public GameObject m_objTargetSocket_01_OFF;

	public GameObject m_objTargetSocket_02_ON;

	public GameObject m_objTargetSocket_02_OFF;

	public List<NKCUIItemCostSlot> m_lstCostSlot = new List<NKCUIItemCostSlot>();

	public NKCUIComStateButton m_btnReroll;

	public NKCUIComStateButton m_btnConfirm;

	private NKMUnitData m_curShipData;

	private int m_curSelectedModuleIndex;

	private int m_curOpenModuleIndex;

	private bool bBackButton;

	private bool bHomeButton;

	private bool bSlotChange;

	private int m_targetSlotIndex = -1;

	public static NKCPopupShipCommandModule Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShipCommandModule>("ab_ui_nkm_ui_ship_info", "NKM_UI_POPUP_SHIP_MODULE", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupShipCommandModule>();
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

	public override string MenuName => "";

	public override string GuideTempletID => "ARTICLE_SHIP_COMMANDMODULE";

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (m_curShipData != null)
			{
				List<int> list = new List<int>();
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_curShipData.m_UnitID);
				if (unitTempletBase == null)
				{
					return base.UpsideMenuShowResourceList;
				}
				NKMShipCommandModuleTemplet nKMShipCommandModuleTemplet = NKMShipManager.GetNKMShipCommandModuleTemplet(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, unitTempletBase.m_NKM_UNIT_GRADE, 1);
				for (int i = 0; i < nKMShipCommandModuleTemplet.ModuleReqItems.Count; i++)
				{
					if (!list.Contains(nKMShipCommandModuleTemplet.ModuleReqItems[i].ItemId))
					{
						list.Add(nKMShipCommandModuleTemplet.ModuleReqItems[i].ItemId);
					}
				}
				for (int j = 0; j < nKMShipCommandModuleTemplet.ModuleLockItems.Count; j++)
				{
					if (!list.Contains(nKMShipCommandModuleTemplet.ModuleLockItems[j].ItemId))
					{
						list.Add(nKMShipCommandModuleTemplet.ModuleLockItems[j].ItemId);
					}
				}
				if (list.Count > 0)
				{
					return list;
				}
				return base.UpsideMenuShowResourceList;
			}
			return base.UpsideMenuShowResourceList;
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

	public NKMUnitData GetShipData()
	{
		return m_curShipData;
	}

	public long GetShipUID()
	{
		if (m_curShipData == null)
		{
			return 0L;
		}
		return m_curShipData.m_UnitUID;
	}

	public override void CloseInternal()
	{
		m_curShipData = null;
		m_curSelectedModuleIndex = -1;
		m_curOpenModuleIndex = -1;
		for (int i = 0; i < m_lstModuleSlot.Count; i++)
		{
			m_lstModuleSlot[i].CloseInternal();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		for (int i = 0; i < m_lstModuleSlot.Count; i++)
		{
			m_lstModuleSlot[i].Init(OnClickOpen, OnClickSetting);
		}
		for (int j = 0; j < m_lstCostSlot.Count; j++)
		{
			m_lstCostSlot[j].SetData(0, 0, 0L);
		}
		m_btnReroll.PointerClick.RemoveAllListeners();
		m_btnReroll.PointerClick.AddListener(OnClickReroll);
		m_btnReroll.m_bGetCallbackWhileLocked = true;
		m_btnConfirm.PointerClick.RemoveAllListeners();
		m_btnConfirm.PointerClick.AddListener(OnClickConfirm);
	}

	public override void Hide()
	{
		base.Hide();
		for (int i = 0; i < m_lstModuleSlot.Count; i++)
		{
			m_lstModuleSlot[i].DisableAllFx();
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		if (m_curSelectedModuleIndex < 0)
		{
			m_Ani.SetTrigger("01_IDLE");
		}
		else
		{
			m_Ani.SetTrigger("02_IDLE");
		}
		m_btnReroll.enabled = true;
	}

	public void Open(NKMUnitData shipData, int reservedModuleIndex = -1)
	{
		if (!NKMShipManager.HasNKMShipCommandModuleTemplet(shipData))
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_curShipData = shipData;
		m_curSelectedModuleIndex = reservedModuleIndex;
		m_curOpenModuleIndex = -1;
		m_btnReroll.enabled = true;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_curShipData.m_UnitID);
		NKCUtil.SetImageSprite(m_imgShip, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase));
		NKCUtil.SetImageSprite(m_imgShip_02, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase));
		NKCUtil.SetLabelText(m_lbShipName, unitTempletBase.GetUnitName());
		NKCUtil.SetLabelText(m_lbShipName_02, unitTempletBase.GetUnitName());
		UpdateUpsideMenu();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_curSelectedModuleIndex < 0)
		{
			m_Ani.SetTrigger("INTRO");
		}
		else
		{
			m_Ani.SetTrigger("02_IDLE");
		}
		SetData();
		SetModuleState();
		UIOpened();
	}

	private void SetData()
	{
		NKMShipModuleCandidate shipCandidateData = NKCScenManager.CurrentUserData().GetShipCandidateData();
		for (int i = 0; i < m_lstModuleSlot.Count; i++)
		{
			if (i < m_curShipData.ShipCommandModule.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstModuleStep[i], bValue: true);
				if (shipCandidateData.shipUid == m_curShipData.m_UnitUID && shipCandidateData.moduleId == i)
				{
					m_lstModuleSlot[i].SetData(i, m_curShipData.ShipCommandModule[i], shipCandidateData.slotCandidate);
				}
				else
				{
					m_lstModuleSlot[i].SetData(i, m_curShipData.ShipCommandModule[i]);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstModuleStep[i], bValue: false);
				m_lstModuleSlot[i].SetData(i, null);
			}
			if (m_curSelectedModuleIndex < 0)
			{
				m_lstModuleSlot[i].SetDisable(bValue: false);
			}
			else
			{
				m_lstModuleSlot[i].SetDisable(m_curSelectedModuleIndex != i);
			}
		}
		SetBottomInfo();
	}

	private void SetModuleState()
	{
		for (int i = 0; i < m_lstModuleSlot.Count; i++)
		{
			if (m_curSelectedModuleIndex == i)
			{
				m_lstModuleSlot[i].SetState(NKCPopupShipCommandModuleSlot.SHIP_MODULE_STATE.IDLE_02);
			}
			else
			{
				m_lstModuleSlot[i].SetState(NKCPopupShipCommandModuleSlot.SHIP_MODULE_STATE.IDLE_01);
			}
		}
	}

	private void OnClickOpen(int slotIndex)
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMShipManager.CanModuleOptionChange(m_curShipData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
			return;
		}
		m_curOpenModuleIndex = slotIndex;
		NKCPacketSender.Send_NKMPacket_SHIP_SLOT_FIRST_OPTION_REQ(m_curShipData.m_UnitUID, slotIndex);
	}

	private void OnClickSetting(int moduleIndex)
	{
		if (moduleIndex == m_curSelectedModuleIndex)
		{
			return;
		}
		if (NKCScenManager.CurrentUserData().GetShipCandidateData().shipUid > 0)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SHIP_COMMANDMODULE_EXIT_CONFIRM, delegate
			{
				OnSlotChange(moduleIndex);
			});
			return;
		}
		if (m_curSelectedModuleIndex < 0)
		{
			m_Ani.SetTrigger("01_TO_02");
		}
		else
		{
			m_lstModuleSlot[m_curSelectedModuleIndex].SetState(NKCPopupShipCommandModuleSlot.SHIP_MODULE_STATE.SET_02_TO_01);
			m_Ani.SetTrigger("02_IDLE");
		}
		for (int num = 0; num < m_lstModuleSlot.Count; num++)
		{
			m_lstModuleSlot[num].SetDisable(num != moduleIndex);
		}
		m_curSelectedModuleIndex = moduleIndex;
		m_lstModuleSlot[moduleIndex].SetState(NKCPopupShipCommandModuleSlot.SHIP_MODULE_STATE.SET_01_TO_02);
		SetBottomInfo();
	}

	private void OnSlotChange(int targetModuleIndex)
	{
		m_targetSlotIndex = targetModuleIndex;
		bSlotChange = true;
		NKCPacketSender.Send_NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ();
	}

	private void SetBottomInfo()
	{
		if (m_curSelectedModuleIndex < 0 || m_curSelectedModuleIndex >= m_curShipData.ShipCommandModule.Count)
		{
			SetTargetSocket(bSocket_01_locked: true, bSocket_02_locked: true);
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_curShipData.m_UnitID);
		if (unitTempletBase == null)
		{
			return;
		}
		NKMShipCommandModuleTemplet nKMShipCommandModuleTemplet = NKMShipManager.GetNKMShipCommandModuleTemplet(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, unitTempletBase.m_NKM_UNIT_GRADE, m_curSelectedModuleIndex + 1);
		if (nKMShipCommandModuleTemplet == null)
		{
			return;
		}
		for (int i = 0; i < m_lstCostSlot.Count; i++)
		{
			if (i < nKMShipCommandModuleTemplet.ModuleReqItems.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstCostSlot[i], bValue: true);
				m_lstCostSlot[i].SetData(nKMShipCommandModuleTemplet.ModuleReqItems[i].ItemId, nKMShipCommandModuleTemplet.ModuleReqItems[i].Count32, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(nKMShipCommandModuleTemplet.ModuleReqItems[i].ItemId));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstCostSlot[i], bValue: false);
			}
		}
		if (m_curShipData.ShipCommandModule[m_curSelectedModuleIndex].slots != null && m_curShipData.ShipCommandModule[m_curSelectedModuleIndex].slots.Length > 1)
		{
			SetTargetSocket(m_curShipData.ShipCommandModule[m_curSelectedModuleIndex].slots[0].isLock, m_curShipData.ShipCommandModule[m_curSelectedModuleIndex].slots[1].isLock);
		}
		bool flag = false;
		for (int j = 0; j < m_curShipData.ShipCommandModule[m_curSelectedModuleIndex].slots.Length; j++)
		{
			if (!m_curShipData.ShipCommandModule[m_curSelectedModuleIndex].slots[j].isLock)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			m_btnReroll.Lock();
		}
		else
		{
			m_btnReroll.UnLock();
		}
		if (NKCScenManager.CurrentUserData().GetShipCandidateData().shipUid == 0L)
		{
			m_btnConfirm.Lock();
		}
		else
		{
			m_btnConfirm.UnLock();
		}
	}

	private void SetTargetSocket(bool bSocket_01_locked, bool bSocket_02_locked)
	{
		NKCUtil.SetGameobjectActive(m_objTargetSocket_01_ON, !bSocket_01_locked);
		NKCUtil.SetGameobjectActive(m_objTargetSocket_01_OFF, bSocket_01_locked);
		NKCUtil.SetGameobjectActive(m_objTargetSocket_02_ON, !bSocket_02_locked);
		NKCUtil.SetGameobjectActive(m_objTargetSocket_02_OFF, bSocket_02_locked);
	}

	private void OnClickReroll()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_curShipData.m_UnitID);
		if (unitTempletBase == null)
		{
			return;
		}
		NKMShipCommandModuleTemplet nKMShipCommandModuleTemplet = NKMShipManager.GetNKMShipCommandModuleTemplet(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, unitTempletBase.m_NKM_UNIT_GRADE, m_curSelectedModuleIndex + 1);
		if (nKMShipCommandModuleTemplet == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < m_curShipData.ShipCommandModule[m_curSelectedModuleIndex].slots.Length; i++)
		{
			NKMShipCmdSlot obj = m_curShipData.ShipCommandModule[m_curSelectedModuleIndex].slots[i];
			if (obj != null && !obj.isLock)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_SHIP_COMMAND_MODULE_SLOT_ALL_LOCK);
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool flag2 = true;
		MiscItemUnit miscItemUnit = null;
		for (int j = 0; j < nKMShipCommandModuleTemplet.ModuleReqItems.Count; j++)
		{
			if (nKMUserData.m_InventoryData.GetCountMiscItem(nKMShipCommandModuleTemplet.ModuleReqItems[j].ItemId) < nKMShipCommandModuleTemplet.ModuleReqItems[j].Count32)
			{
				flag2 = false;
				miscItemUnit = nKMShipCommandModuleTemplet.ModuleReqItems[j];
				break;
			}
		}
		if (!flag2)
		{
			NKCPopupItemLack.Instance.OpenItemMiscLackPopup(miscItemUnit.ItemId, miscItemUnit.Count32);
			return;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMShipManager.CanModuleOptionChange(m_curShipData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
			return;
		}
		m_btnReroll.enabled = false;
		NKCPacketSender.Send_NKMPacket_SHIP_SLOT_OPTION_CHANGE_REQ(m_curShipData.m_UnitUID, m_curSelectedModuleIndex);
	}

	private void OnClickConfirm()
	{
		NKCPacketSender.Send_NKMPacket_SHIP_SLOT_OPTION_CONFIRM_REQ(m_curShipData.m_UnitUID, m_curSelectedModuleIndex);
	}

	public void CandidateChanged()
	{
		m_btnReroll.enabled = true;
		if (m_curSelectedModuleIndex < m_lstModuleSlot.Count)
		{
			if (NKCScenManager.CurrentUserData().GetShipCandidateData().shipUid > 0)
			{
				m_lstModuleSlot[m_curSelectedModuleIndex].ShowRerollFx();
			}
			else
			{
				m_lstModuleSlot[m_curSelectedModuleIndex].ShowConfirmFx();
			}
		}
		SetBottomInfo();
	}

	public void ShowModuleOpenFx()
	{
		if (m_curOpenModuleIndex >= 0)
		{
			m_lstModuleSlot[m_curOpenModuleIndex].ShowOpenFx();
			m_curOpenModuleIndex = -1;
		}
	}

	public override bool OnHomeButton()
	{
		if (NKCScenManager.CurrentUserData().GetShipCandidateData().shipUid > 0 && m_curSelectedModuleIndex >= 0)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SHIP_COMMANDMODULE_EXIT_CONFIRM, OnHomeConfirm);
			return false;
		}
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		return true;
	}

	private void OnHomeConfirm()
	{
		bHomeButton = true;
		NKCPacketSender.Send_NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ();
	}

	public override void OnBackButton()
	{
		if (NKCScenManager.CurrentUserData().GetShipCandidateData().shipUid > 0 && m_curSelectedModuleIndex >= 0)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SHIP_COMMANDMODULE_EXIT_CONFIRM, OnBackConfirm);
			return;
		}
		if (m_curSelectedModuleIndex < 0)
		{
			base.OnBackButton();
			return;
		}
		m_Ani.SetTrigger("02_TO_01");
		m_lstModuleSlot[m_curSelectedModuleIndex].SetState(NKCPopupShipCommandModuleSlot.SHIP_MODULE_STATE.SET_02_TO_01);
		m_curSelectedModuleIndex = -1;
		SetData();
	}

	private void OnBackConfirm()
	{
		bBackButton = true;
		NKCPacketSender.Send_NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ();
	}

	public void OnCandidateRemoved()
	{
		if (bBackButton)
		{
			bBackButton = false;
			m_lstModuleSlot[m_curSelectedModuleIndex].SetData(m_curSelectedModuleIndex, m_curShipData.ShipCommandModule[m_curSelectedModuleIndex]);
			OnBackButton();
		}
		else if (bHomeButton)
		{
			bHomeButton = false;
			m_curSelectedModuleIndex = -1;
			OnHomeButton();
		}
		else if (bSlotChange)
		{
			bSlotChange = false;
			SetData();
			OnClickSetting(m_targetSlotIndex);
			m_targetSlotIndex = -1;
		}
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		if (eEventType == NKMUserData.eChangeNotifyType.Update)
		{
			base.OnUnitUpdate(eEventType, eUnitType, uid, unitData);
			if (m_curShipData.m_UnitUID == uid)
			{
				m_curShipData = unitData;
			}
			SetData();
			SetModuleState();
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		base.OnInventoryChange(itemData);
		if (UpsideMenuShowResourceList.Contains(itemData.ItemID))
		{
			SetBottomInfo();
		}
	}
}
