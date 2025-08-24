using ClientPacket.Event;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventRaceTeamSelect : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_PF_RACE";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_EVENT_RACE_TEAM_SELECT";

	private static NKCPopupEventRaceTeamSelect m_Instance;

	public Image m_imgRedTeamUnit;

	public Image m_imgBlueTeamUnit;

	public GameObject m_objRedSelected;

	public GameObject m_objBlueSelected;

	public EventTrigger m_eventTriggerBg;

	public NKCUIComStateButton m_csbtnRedTeamSelect;

	public NKCUIComStateButton m_csbtnBlueTeamSelect;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	private RaceTeam m_selectedTeam;

	private int m_EventId;

	public static NKCPopupEventRaceTeamSelect Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventRaceTeamSelect>("AB_UI_NKM_UI_EVENT_PF_RACE", "NKM_UI_POPUP_EVENT_RACE_TEAM_SELECT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventRaceTeamSelect>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

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

	public override string MenuName => "RACE TEAM SELECT";

	public override eMenutype eUIType => eMenutype.Popup;

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

	public void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnRedTeamSelect, OnClickRedTeamSelect);
		NKCUtil.SetButtonClickDelegate(m_csbtnBlueTeamSelect, OnClickBlueTeamSelect);
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, OnClickOK);
		NKCUtil.SetButtonClickDelegate(m_csbtnCancel, base.Close);
		if (m_eventTriggerBg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_eventTriggerBg.triggers.Add(entry);
		}
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		NKCUtil.SetHotkey(m_csbtnRedTeamSelect, HotkeyEventType.Left);
		NKCUtil.SetHotkey(m_csbtnBlueTeamSelect, HotkeyEventType.Right);
	}

	public void Open(int eventId)
	{
		m_EventId = eventId;
		NKCUtil.SetGameobjectActive(m_objRedSelected, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBlueSelected, bValue: false);
		m_csbtnOK.SetLock(value: true);
		SetCharacter(eventId);
		base.gameObject.SetActive(value: true);
		UIOpened();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void SetCharacter(int eventId)
	{
		NKMEventRaceTemplet.Find(eventId);
	}

	private void SetCharacterImage(string type, int id, Image characterImage)
	{
		if (type != null && type == "SKIN")
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(id);
			if (skinTemplet != null)
			{
				NKCUtil.SetImageSprite(characterImage, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, skinTemplet), bDisableIfSpriteNull: true);
			}
		}
		else
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(id);
			if (unitTempletBase != null)
			{
				NKCUtil.SetImageSprite(characterImage, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase), bDisableIfSpriteNull: true);
			}
		}
	}

	private void OnClickRedTeamSelect()
	{
		m_selectedTeam = RaceTeam.TeamA;
		NKCUtil.SetGameobjectActive(m_objRedSelected, bValue: true);
		NKCUtil.SetGameobjectActive(m_objBlueSelected, bValue: false);
		m_csbtnOK.SetLock(value: false);
	}

	private void OnClickBlueTeamSelect()
	{
		m_selectedTeam = RaceTeam.TeamB;
		NKCUtil.SetGameobjectActive(m_objRedSelected, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBlueSelected, bValue: true);
		m_csbtnOK.SetLock(value: false);
	}

	private void OnClickOK()
	{
		Close();
		NKCPacketSender.Send_NKMPACKET_RACE_TEAM_SELECT_REQ(m_selectedTeam);
	}

	private void OnDestroy()
	{
		m_imgRedTeamUnit = null;
		m_imgBlueTeamUnit = null;
		m_objRedSelected = null;
		m_objBlueSelected = null;
		m_csbtnRedTeamSelect = null;
		m_csbtnBlueTeamSelect = null;
		m_csbtnOK = null;
		m_csbtnCancel = null;
	}
}
