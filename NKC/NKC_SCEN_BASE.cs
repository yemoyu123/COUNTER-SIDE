using System.Collections.Generic;
using ClientPacket.Unit;
using NKC.UI;
using NKC.UI.NPC;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_BASE : NKC_SCEN_BASIC
{
	public enum eUIOpenReserve
	{
		Nothing,
		LAB_Begin,
		LAB_Train,
		LAB_Enchant,
		LAB_Transcendence,
		LAB_End,
		Factory_Begin,
		Factory_Craft,
		Factory_Enchant,
		Factory_Tunning,
		Factory_End,
		Hangar_Begin,
		Hangar_Build,
		Hangar_Shipyard,
		Hangar_End,
		Personnel_Begin,
		Personnel_Negotiate,
		Personnel_Lifetime,
		Personnel_Scout,
		Personnel_End,
		Base_Main,
		FORGE_Enchant
	}

	private enum LoadingUIType
	{
		Nothing,
		Base,
		Lab,
		Hangar,
		Personnel
	}

	private NKCUIManager.LoadedUIData m_UIDataBaseSceneMenu;

	private NKCUIBaseSceneMenu m_NKCUIBaseSceneMenu;

	private NKCUIManager.LoadedUIData m_UIDataLab;

	private NKCUILab m_NKCUILab;

	private NKMTrackingFloat m_BloomIntensity = new NKMTrackingFloat();

	public GameObject m_objNUF_BASE_Panel;

	private bool m_bBaseSceneMenuInitComplete;

	private eUIOpenReserve m_eUIOpenReserve;

	private long m_reserveUnitUID;

	private LoadingUIType eLoadingUIType;

	private NKCUILab.LAB_DETAIL_STATE m_SelectLabDetailState;

	private RectTransform m_NUF_COMMON_Panel;

	private NKMUserData UserData => NKCScenManager.CurrentUserData();

	public void SetOpenReserve(eUIOpenReserve UIToOpen, long unitUID = 0L, bool bForce = false)
	{
		if (bForce || !CheckIgnoreReservedUI(UIToOpen))
		{
			m_eUIOpenReserve = UIToOpen;
			m_reserveUnitUID = unitUID;
		}
	}

	private bool CheckIgnoreReservedUI(eUIOpenReserve UIToOpen)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_BASE)
		{
			return false;
		}
		switch (UIToOpen)
		{
		case eUIOpenReserve.Factory_Craft:
		case eUIOpenReserve.Factory_Enchant:
		case eUIOpenReserve.Factory_Tunning:
			if (NKCUIForgeCraft.IsInstanceOpen)
			{
				return true;
			}
			if (NKCUIForge.IsInstanceOpen)
			{
				return true;
			}
			break;
		default:
			return false;
		case eUIOpenReserve.LAB_Train:
		case eUIOpenReserve.LAB_Enchant:
		case eUIOpenReserve.LAB_Transcendence:
		case eUIOpenReserve.Personnel_Negotiate:
		case eUIOpenReserve.Personnel_Lifetime:
		case eUIOpenReserve.Personnel_Scout:
			break;
		}
		return false;
	}

	public NKC_SCEN_BASE()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_BASE;
		eLoadingUIType = LoadingUIType.Base;
	}

	private void BeginUILoading(LoadingUIType type)
	{
		if (type == LoadingUIType.Nothing)
		{
			return;
		}
		eLoadingUIType = type;
		switch (eLoadingUIType)
		{
		case LoadingUIType.Base:
			if (!NKCUIManager.IsValid(m_UIDataBaseSceneMenu))
			{
				m_UIDataBaseSceneMenu = NKCUIManager.OpenNewInstanceAsync<NKCUIBaseSceneMenu>("ab_ui_nuf_base", "NKM_UI_BASE", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), null);
			}
			break;
		case LoadingUIType.Lab:
			if (!NKCUIManager.IsValid(m_UIDataLab))
			{
				m_UIDataLab = NKCUIManager.OpenNewInstanceAsync<NKCUILab>("ab_ui_nkm_ui_lab", "NKM_UI_LAB", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), null);
			}
			break;
		}
		NKMPopUpBox.OpenWaitBox();
	}

	private NKCUIManager.LoadedUIData GetLoadedUIData(LoadingUIType uiType)
	{
		return uiType switch
		{
			LoadingUIType.Base => m_UIDataBaseSceneMenu, 
			LoadingUIType.Lab => m_UIDataLab, 
			_ => null, 
		};
	}

	private void ProcessUILoading(bool bOpenUIOnComplete)
	{
		if (eLoadingUIType == LoadingUIType.Nothing && m_eUIOpenReserve == eUIOpenReserve.Nothing)
		{
			return;
		}
		if (eLoadingUIType != LoadingUIType.Nothing && GetLoadedUIData(eLoadingUIType) == null)
		{
			eLoadingUIType = LoadingUIType.Nothing;
			Debug.LogError("Logic Error : UILoadResourceData not loaded");
		}
		switch (eLoadingUIType)
		{
		case LoadingUIType.Base:
			if (m_UIDataBaseSceneMenu.CheckLoadAndGetInstance<NKCUIBaseSceneMenu>(out m_NKCUIBaseSceneMenu))
			{
				m_NKCUIBaseSceneMenu.Init(BeginUILoading, OpenSubMenu);
				eLoadingUIType = LoadingUIType.Nothing;
				NKMPopUpBox.CloseWaitBox();
				if (bOpenUIOnComplete)
				{
					m_NKCUIBaseSceneMenu.Open();
				}
				m_bBaseSceneMenuInitComplete = true;
			}
			break;
		case LoadingUIType.Lab:
			if (m_UIDataLab.CheckLoadAndGetInstance<NKCUILab>(out m_NKCUILab))
			{
				m_NKCUILab.InitUI(NKCPacketSender.Send_Packet_NKMPacket_LIMIT_BREAK_UNIT_REQ, NKCPacketSender.Send_Packet_NKMPacket_UNIT_SKILL_UPGRADE_REQ, m_NKCUIBaseSceneMenu.m_objNPCLab_Professor_TouchArea?.GetComponent<NKCUINPCProfessorOlivia>());
				eLoadingUIType = LoadingUIType.Nothing;
				NKMPopUpBox.CloseWaitBox();
				if (bOpenUIOnComplete)
				{
					OpenLab();
				}
			}
			break;
		}
		if (bOpenUIOnComplete && m_bBaseSceneMenuInitComplete && m_eUIOpenReserve != eUIOpenReserve.Nothing)
		{
			OpenSubMenu();
		}
	}

	private void BeginUILoading(eUIOpenReserve openBtnType)
	{
		m_eUIOpenReserve = openBtnType;
	}

	private void OpenSubMenu()
	{
		if (m_eUIOpenReserve != eUIOpenReserve.Nothing)
		{
			CloseOpenedUI(m_eUIOpenReserve);
			switch (m_eUIOpenReserve)
			{
			case eUIOpenReserve.LAB_Train:
			case eUIOpenReserve.LAB_Enchant:
			case eUIOpenReserve.LAB_Transcendence:
				OpenLab();
				break;
			case eUIOpenReserve.Factory_Craft:
			case eUIOpenReserve.Factory_Enchant:
			case eUIOpenReserve.Factory_Tunning:
				OpenFactory();
				break;
			case eUIOpenReserve.Hangar_Build:
			case eUIOpenReserve.Hangar_Shipyard:
				OpenHanger(m_eUIOpenReserve);
				break;
			case eUIOpenReserve.Personnel_Negotiate:
			case eUIOpenReserve.Personnel_Lifetime:
			case eUIOpenReserve.Personnel_Scout:
				OpenHR();
				break;
			case eUIOpenReserve.Base_Main:
				m_NKCUIBaseSceneMenu.ChangeMenu(NKCUIBaseSceneMenu.BaseSceneMenuType.Base, bReturnIfSameMenu: true);
				m_eUIOpenReserve = eUIOpenReserve.Nothing;
				break;
			default:
				Debug.LogError($"NKC_SCEN_BASE::OpenSubMenu - type not matched! : {m_eUIOpenReserve}");
				m_eUIOpenReserve = eUIOpenReserve.Nothing;
				break;
			}
		}
	}

	private void OpenLab()
	{
		if (m_NKCUILab != null)
		{
			if (m_eUIOpenReserve == eUIOpenReserve.LAB_Transcendence)
			{
				m_SelectLabDetailState = NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK;
			}
			else if (m_eUIOpenReserve == eUIOpenReserve.LAB_Enchant)
			{
				m_SelectLabDetailState = NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_ENHANCE;
			}
			else if (m_eUIOpenReserve == eUIOpenReserve.LAB_Train)
			{
				m_SelectLabDetailState = NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN;
			}
			m_eUIOpenReserve = eUIOpenReserve.Nothing;
			if (m_SelectLabDetailState != NKCUILab.LAB_DETAIL_STATE.LDS_INVALID)
			{
				m_NKCUIBaseSceneMenu.ChangeMenu(NKCUIBaseSceneMenu.BaseSceneMenuType.Lab, bReturnIfSameMenu: true);
				m_NKCUILab.Open(m_SelectLabDetailState, m_reserveUnitUID);
				m_reserveUnitUID = 0L;
			}
		}
		else
		{
			BeginUILoading(LoadingUIType.Lab);
		}
	}

	private void OpenHR()
	{
		if (m_eUIOpenReserve == eUIOpenReserve.Personnel_Scout)
		{
			m_NKCUIBaseSceneMenu.ChangeMenu(NKCUIBaseSceneMenu.BaseSceneMenuType.Personnel, bReturnIfSameMenu: true);
			NKCUIScout.Instance.Open();
			m_eUIOpenReserve = eUIOpenReserve.Nothing;
		}
		else if (m_eUIOpenReserve == eUIOpenReserve.Personnel_Lifetime)
		{
			m_NKCUIBaseSceneMenu.ChangeMenu(NKCUIBaseSceneMenu.BaseSceneMenuType.Personnel, bReturnIfSameMenu: true);
			NKCUIPersonnel.Instance.Open();
			if (FindReserveUnit(out var result))
			{
				NKCUIPersonnel.Instance.ReserveUnitData(result);
			}
			m_eUIOpenReserve = eUIOpenReserve.Nothing;
		}
	}

	public void OpenFactory()
	{
		m_NKCUIBaseSceneMenu.ChangeMenu(NKCUIBaseSceneMenu.BaseSceneMenuType.Factory, bReturnIfSameMenu: true);
		switch (m_eUIOpenReserve)
		{
		case eUIOpenReserve.Factory_Craft:
			NKCUIForgeCraft.Instance.Open();
			break;
		case eUIOpenReserve.Factory_Enchant:
			NKCUIForge.Instance.Open(NKCUIForge.NKC_FORGE_TAB.NFT_ENCHANT, 0L);
			break;
		case eUIOpenReserve.Factory_Tunning:
			NKCUIForge.Instance.Open(NKCUIForge.NKC_FORGE_TAB.NFT_TUNING, 0L);
			break;
		}
		m_eUIOpenReserve = eUIOpenReserve.Nothing;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (eLoadingUIType == LoadingUIType.Nothing && !NKCUIManager.IsValid(m_UIDataBaseSceneMenu))
		{
			eLoadingUIType = LoadingUIType.Base;
		}
		BeginUILoading(eLoadingUIType);
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		ProcessUILoading(bOpenUIOnComplete: false);
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		Open();
		NKCCamera.EnableBloom(bEnable: false);
		NKCCamera.GetCamera().orthographic = false;
		NKCCamera.GetTrackingPos().SetNowValue(0f, 0f, -1000f);
	}

	public override void ScenEnd()
	{
		NKCUIGameResultGetUnit.CheckInstanceAndClose();
		if (m_NKCUILab != null && m_NKCUILab.IsOpen)
		{
			m_NKCUILab.Close();
		}
		m_NKCUILab = null;
		if (m_NKCUIBaseSceneMenu != null && m_NKCUIBaseSceneMenu.IsOpen)
		{
			m_NKCUIBaseSceneMenu.Close();
		}
		m_NKCUIBaseSceneMenu = null;
		if (m_UIDataBaseSceneMenu != null)
		{
			m_UIDataBaseSceneMenu.CloseInstance();
		}
		m_UIDataBaseSceneMenu = null;
		if (m_UIDataLab != null)
		{
			m_UIDataLab.CloseInstance();
		}
		m_UIDataLab = null;
		base.ScenEnd();
	}

	public void Open()
	{
		if (m_NKCUIBaseSceneMenu != null)
		{
			m_NKCUIBaseSceneMenu.Open(m_eUIOpenReserve != eUIOpenReserve.Nothing);
		}
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
		ProcessUILoading(bOpenUIOnComplete: true);
		if (!NKCCamera.IsTrackingCameraPos())
		{
			NKCCamera.TrackingPos(10f, NKMRandom.Range(-30f, 30f), NKMRandom.Range(-30f, 30f), NKMRandom.Range(-1000f, -950f));
		}
		m_BloomIntensity.Update(Time.deltaTime);
		if (!m_BloomIntensity.IsTracking())
		{
			m_BloomIntensity.SetTracking(NKMRandom.Range(1f, 2f), 4f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		NKCCamera.SetBloomIntensity(m_BloomIntensity.GetNowValue());
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void OnRecv(NKMPacket_LIMIT_BREAK_UNIT_ACK sPacket)
	{
		if (!(m_NKCUILab != null) || !m_NKCUILab.IsOpen)
		{
			return;
		}
		NKCUIGameResultGetUnit.ShowUnitTranscendence(sPacket.unitData, delegate
		{
			if (NKCGameEventManager.IsEventPlaying())
			{
				NKCGameEventManager.WaitFinished();
			}
			else
			{
				m_NKCUILab.TutorialCheckUnit();
			}
		});
	}

	public void OnRecv(NKMPacket_ENHANCE_UNIT_ACK sPacket)
	{
		if (m_NKCUILab != null && m_NKCUILab.IsOpen)
		{
			m_NKCUILab.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_UNIT_SKILL_UPGRADE_ACK sPacket)
	{
		if (m_NKCUILab != null && m_NKCUILab.IsOpen)
		{
			m_NKCUILab.OnRecv(sPacket);
		}
	}

	private void OnFinishMultiSelectionToRemoveEquip(List<long> listEquipSlot)
	{
		if (listEquipSlot == null || listEquipSlot.Count <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_SELECTED_EQUIP);
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData;
		for (int i = 0; i < listEquipSlot.Count; i++)
		{
			NKMEquipItemData itemEquip = inventoryData.GetItemEquip(listEquipSlot[i]);
			if (itemEquip == null)
			{
				continue;
			}
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet != null && equipTemplet.m_NKM_ITEM_GRADE >= NKM_ITEM_GRADE.NIG_SR)
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EQUIP_BREAK_UP_WARNING, delegate
				{
					NKCPacketSender.Send_NKMPacket_REMOVE_EQUIP_ITEM_REQ(listEquipSlot);
				});
				return;
			}
		}
		NKCPacketSender.Send_NKMPacket_REMOVE_EQUIP_ITEM_REQ(listEquipSlot);
	}

	private void InitAnchor()
	{
		if (m_NUF_COMMON_Panel == null)
		{
			m_NUF_COMMON_Panel = NKCUIManager.OpenUI("NUF_COMMON_Panel").GetComponent<RectTransform>();
		}
	}

	private void OpenHanger(eUIOpenReserve newState)
	{
		InitAnchor();
		m_NKCUIBaseSceneMenu.ChangeMenu(NKCUIBaseSceneMenu.BaseSceneMenuType.Hangar, bReturnIfSameMenu: true);
		eUIOpenReserve eUIOpenReserve = m_eUIOpenReserve;
		if ((uint)(eUIOpenReserve - 12) <= 1u)
		{
			m_reserveUnitUID = 0L;
		}
		m_eUIOpenReserve = eUIOpenReserve.Nothing;
	}

	private bool FindReserveUnit(out NKMUnitData result)
	{
		result = null;
		if (m_reserveUnitUID > 0)
		{
			result = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(m_reserveUnitUID);
			m_reserveUnitUID = 0L;
		}
		return result != null;
	}

	public void CloseOpenedUI(eUIOpenReserve reservedUI)
	{
		switch (reservedUI)
		{
		case eUIOpenReserve.Nothing:
			return;
		default:
			if (m_NKCUILab != null && m_NKCUILab.IsOpen)
			{
				m_NKCUILab.Close();
			}
			break;
		case eUIOpenReserve.LAB_Train:
		case eUIOpenReserve.LAB_Enchant:
		case eUIOpenReserve.LAB_Transcendence:
			break;
		}
		if (reservedUI != eUIOpenReserve.Factory_Craft && reservedUI != eUIOpenReserve.Factory_Enchant && reservedUI != eUIOpenReserve.Factory_Tunning)
		{
			NKCUIForge.CheckInstanceAndClose();
			NKCUIForgeCraft.CheckInstanceAndClose();
			NKCUIInventory.CheckInstanceAndClose();
		}
		if (reservedUI != eUIOpenReserve.Personnel_Negotiate && NKCUIUnitSelectList.Instance != null && NKCUIUnitSelectList.Instance.isActiveAndEnabled)
		{
			NKCUIUnitSelectList.Instance.Close();
		}
		if (reservedUI != eUIOpenReserve.Personnel_Scout && NKCUIScout.IsInstanceOpen)
		{
			NKCUIScout.Instance.Close();
		}
	}

	public void TutorialCheck(eUIOpenReserve openUi)
	{
	}

	public void SetBaseMenuType(NKCUIBaseSceneMenu.BaseSceneMenuType menu)
	{
		m_NKCUIBaseSceneMenu.ChangeMenu(menu);
	}

	public NKCUILab GetUILab()
	{
		return m_NKCUILab;
	}
}
