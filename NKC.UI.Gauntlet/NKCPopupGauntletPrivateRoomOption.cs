using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletPrivateRoomOption : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_gauntlet";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_PRIVATE_ROOM_OPTION_POPUP";

	private static NKCPopupGauntletPrivateRoomOption m_Instance;

	public NKCUIGauntletPrivateRoomCustomOption m_prevOption;

	public NKCUIGauntletPrivateRoomCustomOption m_newOption;

	public EventTrigger m_evtBG;

	public NKCUIComStateButton m_btnOK;

	public NKCUIComStateButton m_btnCancel;

	public GameObject m_objArrow;

	public Text m_lbOkText;

	public Text m_lbContent;

	public static NKCPopupGauntletPrivateRoomOption Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGauntletPrivateRoomOption>("ab_ui_nkm_ui_gauntlet", "NKM_UI_GAUNTLET_PRIVATE_ROOM_OPTION_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupGauntletPrivateRoomOption>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "Gautlet Private Room Option";

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

	private void InitUI()
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			OnClickCancel();
		});
		m_evtBG.triggers.Add(entry);
		m_prevOption.Init();
		m_newOption.Init();
		NKCUtil.SetButtonClickDelegate(m_btnOK, OnClickOK);
		NKCUtil.SetButtonClickDelegate(m_btnCancel, OnClickCancel);
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open()
	{
		bool flag = NKCPrivatePVPRoomMgr.IsHost(NKCPrivatePVPRoomMgr.GetMyPvpGameLobbyUserState());
		m_prevOption.SetOption(NKCPrivatePVPRoomMgr.LobbyData.config);
		m_prevOption.ProhibitToggle = !flag;
		if (flag)
		{
			NKCUtil.SetLabelText(m_lbContent, NKCUtilString.GET_STRING_PRIVATE_PVP_OPTION_SETUP_ENABLE);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbContent, NKCUtilString.GET_STRING_PRIVATE_PVP_OPTION_SETUP_VIEW);
		}
		NKCUtil.SetGameobjectActive(m_btnCancel, flag);
		NKCUtil.SetLabelText(m_lbOkText, NKCStringTable.GetString("SI_DP_DECK_BUTTON_OK"));
		NKCUtil.SetGameobjectActive(m_objArrow, bValue: false);
		NKCUtil.SetGameobjectActive(m_newOption, bValue: false);
		UIOpened();
	}

	private bool ComparePrevOption()
	{
		if (NKCPrivatePVPRoomMgr.LobbyData.config.applyAllUnitMaxLevel != NKCPrivatePVPRoomMgr.PrivateGameConfig.applyAllUnitMaxLevel || NKCPrivatePVPRoomMgr.LobbyData.config.applyBanUpSystem != NKCPrivatePVPRoomMgr.PrivateGameConfig.applyBanUpSystem || NKCPrivatePVPRoomMgr.LobbyData.config.applyEquipStat != NKCPrivatePVPRoomMgr.PrivateGameConfig.applyEquipStat || NKCPrivatePVPRoomMgr.LobbyData.config.draftBanMode != NKCPrivatePVPRoomMgr.PrivateGameConfig.draftBanMode)
		{
			return false;
		}
		return true;
	}

	private void RevertOption()
	{
		NKCPrivatePVPRoomMgr.PrivateGameConfig.applyAllUnitMaxLevel = NKCPrivatePVPRoomMgr.LobbyData.config.applyAllUnitMaxLevel;
		NKCPrivatePVPRoomMgr.PrivateGameConfig.applyBanUpSystem = NKCPrivatePVPRoomMgr.LobbyData.config.applyBanUpSystem;
		NKCPrivatePVPRoomMgr.PrivateGameConfig.applyEquipStat = NKCPrivatePVPRoomMgr.LobbyData.config.applyEquipStat;
		NKCPrivatePVPRoomMgr.PrivateGameConfig.draftBanMode = NKCPrivatePVPRoomMgr.LobbyData.config.draftBanMode;
	}

	private void OnClickOK()
	{
		if (!NKCPrivatePVPRoomMgr.IsHost(NKCPrivatePVPRoomMgr.GetMyPvpGameLobbyUserState()))
		{
			Close();
		}
		else if (!m_newOption.gameObject.activeSelf)
		{
			if (!ComparePrevOption())
			{
				m_prevOption.SetOption(NKCPrivatePVPRoomMgr.LobbyData.config);
				m_prevOption.ProhibitToggle = true;
				NKCUtil.SetGameobjectActive(m_objArrow, bValue: true);
				NKCUtil.SetGameobjectActive(m_newOption, bValue: true);
				NKCUtil.SetLabelText(m_lbOkText, NKCStringTable.GetString("SI_USER_INFO_CORP_CHNAGE_BUTTON_TEXT"));
				NKCUtil.SetLabelText(m_lbContent, NKCUtilString.GET_STRING_PRIVATE_PVP_OPTION_SETUP_CONFIRM);
				m_newOption.SetOption(NKCPrivatePVPRoomMgr.PrivateGameConfig);
				m_newOption.ProhibitToggle = true;
			}
			else
			{
				OnClickCancel();
			}
		}
		else if (!ComparePrevOption())
		{
			NKCPacketSender.Send_NKMPacket_PVP_ROOM_CHANGE_OPTION_REQ(NKCPrivatePVPRoomMgr.PrivateGameConfig);
		}
		else
		{
			OnClickCancel();
		}
	}

	private void OnClickCancel()
	{
		RevertOption();
		Close();
	}
}
