using System.Collections.Generic;
using NKC.UI.NPC;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupHangarBuildConfirm : NKCUIBase
{
	public delegate void OnTryBuildShip(int id);

	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_hangar";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_HANGAR_BUILD_CONFIRM";

	private static NKCUIPopupHangarBuildConfirm m_Instance;

	[Header("아이템 슬롯 리스트")]
	public NKCUIItemCostSlot[] m_lstCostItems;

	public Text m_NKM_UI_POPUP_HANGAR_BUILD_CONFIRM_TOP_TEXT;

	[Header("npc")]
	public GameObject m_AB_NPC_NA_HEE_RIN_TOUCH;

	private NKCUINPCHangarNaHeeRin m_NKCUINPCHangarNaHeeRin;

	[Header("버튼")]
	public NKCUIComStateButton m_NKM_UI_POPUP_OK_CANCEL_BOX_OK;

	public NKCUIComStateButton m_NKM_UI_POPUP_OK_CANCEL_BOX_CANCEL;

	public NKCUIComStateButton m_NKM_UI_POPUP_CLOSEBUTTON;

	private OnTryBuildShip dOnTryBuildShip;

	private int m_targetBuildShipID;

	private UnityAction dCloseAction;

	public static NKCUIPopupHangarBuildConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupHangarBuildConfirm>("ab_ui_nkm_ui_hangar", "NKM_UI_POPUP_HANGAR_BUILD_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupHangarBuildConfirm>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_POPUP_MENU_NAME_BUILD_CONFIRM;

	public override List<int> UpsideMenuShowResourceList => base.UpsideMenuShowResourceList;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.ResourceOnly;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		if (dCloseAction != null)
		{
			dCloseAction();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void InitUI()
	{
		if (m_NKCUINPCHangarNaHeeRin == null)
		{
			m_NKCUINPCHangarNaHeeRin = m_AB_NPC_NA_HEE_RIN_TOUCH.GetComponent<NKCUINPCHangarNaHeeRin>();
			m_NKCUINPCHangarNaHeeRin.Init();
		}
		else
		{
			m_NKCUINPCHangarNaHeeRin.PlayAni(NPC_ACTION_TYPE.START);
		}
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_OK_CANCEL_BOX_OK, OnTryBuild);
		NKCUtil.SetHotkey(m_NKM_UI_POPUP_OK_CANCEL_BOX_OK, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_OK_CANCEL_BOX_CANCEL, base.Close);
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_CLOSEBUTTON, base.Close);
	}

	public void Open(int shipID, OnTryBuildShip tryBuildShip, UnityAction closeAction = null)
	{
		NKMShipBuildTemplet shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(shipID);
		if (shipBuildTemplet != null)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			for (int i = 0; i < m_lstCostItems.Length; i++)
			{
				if (i + 1 > shipBuildTemplet.BuildMaterialList.Count)
				{
					m_lstCostItems[i].SetData(0, 0, 0L);
					continue;
				}
				BuildMaterial buildMaterial = shipBuildTemplet.BuildMaterialList[i];
				if (nKMUserData != null)
				{
					m_lstCostItems[i].SetData(buildMaterial.m_ShipBuildMaterialID, buildMaterial.m_ShipBuildMaterialCount, nKMUserData.m_InventoryData.GetCountMiscItem(buildMaterial.m_ShipBuildMaterialID));
				}
			}
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipID);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_HANGAR_BUILD_CONFIRM_TOP_TEXT, string.Format(NKCUtilString.GET_STRING_HANGAR_CONFIRM, unitTempletBase.GetUnitName()));
		}
		dOnTryBuildShip = tryBuildShip;
		m_targetBuildShipID = shipID;
		dCloseAction = closeAction;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
	}

	private void OnTryBuild()
	{
		if (dOnTryBuildShip != null)
		{
			dOnTryBuildShip(m_targetBuildShipID);
			Close();
		}
	}
}
