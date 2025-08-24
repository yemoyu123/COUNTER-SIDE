using System;
using System.Collections.Generic;
using ClientPacket.Guild;
using Cs.Core.Util;
using NKC.UI.Shop;
using NKM;
using NKM.Guild;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildLobbyInfo : MonoBehaviour
{
	public delegate void OnMoveToTab(NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE lobbyUiType, bool bForce = false);

	public List<NKCUIInGameCharacterViewer> m_lstCharacterViewer = new List<NKCUIInGameCharacterViewer>();

	public List<int> m_lstUnitIDs = new List<int>();

	[Space]
	public TMP_InputField m_IFNotice;

	public NKCUIComStateButton m_btnEditGuildNotice;

	public NKCUIComStateButton m_btnConsortiumCoop;

	public NKCUIComStateButton m_btnShop;

	public NKCUIComStateButton m_btnMission;

	public NKCUIComStateButton m_btnManage;

	public ScrollRect m_scMain;

	[Header("협력전 알림")]
	public GameObject m_objCoopNotice;

	public Text m_lbCoopStatus;

	public Text m_lbCoopRemainTime;

	public GameObject m_objGuildCoopRedDot;

	private OnMoveToTab m_dOnMoveToTab;

	private float m_fDeltaTime;

	private bool m_bCoopStatusChanged;

	private DateTime m_tLastPacketSendTime = DateTime.MinValue;

	private NKCUIShopSingle m_ConsortiumShop;

	private NKCUIShopSingle ConsortiumShop
	{
		get
		{
			if (m_ConsortiumShop == null)
			{
				m_ConsortiumShop = NKCUIShopSingle.GetInstance("ab_ui_nkm_ui_consortium_shop", "NKM_UI_CONSORTIUM_SHOP");
			}
			return m_ConsortiumShop;
		}
	}

	public void InitUI()
	{
		m_IFNotice.onValidateInput = NKCFilterManager.FilterEmojiInput;
		m_IFNotice.onEndEdit.RemoveAllListeners();
		m_IFNotice.onEndEdit.AddListener(OnIFChanged);
		m_IFNotice.onValueChanged.RemoveAllListeners();
		m_IFNotice.onValueChanged.AddListener(OnIFChanged);
		m_IFNotice.characterLimit = 36;
		NKCUtil.SetGameobjectActive(m_btnEditGuildNotice, bValue: false);
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.HIDE_GUILD_NOTICE))
		{
			m_btnEditGuildNotice.PointerClick.RemoveAllListeners();
			m_btnEditGuildNotice.PointerClick.AddListener(OnClickEditGuildNotice);
			m_btnEditGuildNotice.m_bGetCallbackWhileLocked = true;
		}
		if (m_btnConsortiumCoop != null)
		{
			m_btnConsortiumCoop.PointerClick.RemoveAllListeners();
			m_btnConsortiumCoop.PointerClick.AddListener(OnClickCoop);
			m_btnConsortiumCoop.m_bGetCallbackWhileLocked = true;
		}
		m_btnShop?.PointerClick.RemoveAllListeners();
		m_btnShop?.PointerClick.AddListener(OnClickShop);
		m_btnMission?.PointerClick.RemoveAllListeners();
		m_btnMission?.PointerClick.AddListener(OnClickMission);
		m_btnManage.PointerClick.RemoveAllListeners();
		m_btnManage.PointerClick.AddListener(OnClickManage);
		m_tLastPacketSendTime = DateTime.MinValue;
	}

	public void SetData(GuildMemberGrade myGrade, OnMoveToTab onMoveToTab)
	{
		m_dOnMoveToTab = onMoveToTab;
		ResetPosition();
		m_IFNotice.text = NKCFilterManager.CheckBadChat(NKCGuildManager.MyGuildData.notice);
		m_IFNotice.interactable = myGrade != GuildMemberGrade.Member;
		NKCUtil.SetGameobjectActive(m_btnManage, myGrade == GuildMemberGrade.Master);
		NKCUtil.SetGameobjectActive(m_btnEditGuildNotice, myGrade != GuildMemberGrade.Member);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.HIDE_GUILD_NOTICE))
		{
			m_IFNotice.interactable = false;
			NKCUtil.SetGameobjectActive(m_btnEditGuildNotice, bValue: false);
		}
		m_bCoopStatusChanged = false;
		switch (NKCGuildCoopManager.m_GuildDungeonState)
		{
		case GuildDungeonState.PlayableGuildDungeon:
			NKCUtil.SetLabelText(m_lbCoopStatus, NKCUtilString.GET_STRING_CONSORTIUM_LOBBY_MESSAGE_IN_PROGRESS);
			break;
		case GuildDungeonState.SeasonOut:
		case GuildDungeonState.SessionOut:
			NKCUtil.SetLabelText(m_lbCoopStatus, NKCUtilString.GET_STRING_CONSORTIUM_LOBBY_MESSAGE_NOT_IN_PROGRESS);
			break;
		case GuildDungeonState.Adjust:
			NKCUtil.SetLabelText(m_lbCoopStatus, NKCUtilString.GET_STRING_CONSORTIUM_LOBBY_MESSAGE_CALCULATE);
			break;
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_SHOP))
		{
			m_btnShop.Lock();
		}
		else
		{
			m_btnShop.UnLock();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_DUNGEON) || !NKCGuildCoopManager.CheckFirstSeasonStarted())
		{
			NKCUtil.SetGameobjectActive(m_objCoopNotice, bValue: false);
			m_btnConsortiumCoop.Lock();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objCoopNotice, NKCGuildCoopManager.m_GuildDungeonState != GuildDungeonState.Invalid);
			if (m_objCoopNotice.activeSelf)
			{
				SetCoopStatusTime();
			}
			m_btnConsortiumCoop.UnLock();
		}
		NKCUtil.SetGameobjectActive(m_objGuildCoopRedDot, NKCGuildCoopManager.CheckSeasonRewardEnable());
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_MISSION))
		{
			m_btnMission.Lock();
		}
		else
		{
			m_btnMission.UnLock();
		}
	}

	private void SetUnitIllust()
	{
		for (int i = 0; i < m_lstCharacterViewer.Count; i++)
		{
			m_lstCharacterViewer[i].CleanUp();
		}
		for (int j = 0; j < m_lstCharacterViewer.Count; j++)
		{
			m_lstCharacterViewer[j].Prepare();
			if (m_lstUnitIDs.Count > j)
			{
				m_lstCharacterViewer[j].SetPreviewBattleUnit(m_lstUnitIDs[j], 0);
			}
			else
			{
				m_lstCharacterViewer[j].SetPreviewBattleUnit(1001 + j, 0);
			}
		}
	}

	private void SetCoopStatusTime()
	{
		switch (NKCGuildCoopManager.m_GuildDungeonState)
		{
		case GuildDungeonState.Invalid:
			if (NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_NextSessionStartDateUTC) && NKCGuildCoopManager.HasNextSessionData(NKCGuildCoopManager.m_NextSessionStartDateUTC))
			{
				if (!m_bCoopStatusChanged)
				{
					m_bCoopStatusChanged = true;
					SendPacket();
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objCoopNotice, bValue: false);
			}
			break;
		case GuildDungeonState.PlayableGuildDungeon:
			if (NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_SessionEndDateUTC))
			{
				if (!m_bCoopStatusChanged)
				{
					m_bCoopStatusChanged = true;
					SendPacket();
				}
			}
			else
			{
				NKCUtil.SetLabelText(m_lbCoopRemainTime, NKCUtilString.GetTimeSpanStringDay(NKCGuildCoopManager.m_SessionEndDateUTC - NKCSynchronizedTime.GetServerUTCTime()));
			}
			break;
		case GuildDungeonState.SessionOut:
			if (NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_NextSessionStartDateUTC))
			{
				if (!m_bCoopStatusChanged)
				{
					m_bCoopStatusChanged = true;
					SendPacket();
				}
			}
			else
			{
				NKCUtil.SetLabelText(m_lbCoopRemainTime, NKCUtilString.GetTimeSpanStringDay(NKCGuildCoopManager.m_NextSessionStartDateUTC - NKCSynchronizedTime.GetServerUTCTime()));
			}
			break;
		case GuildDungeonState.SeasonOut:
			NKCUtil.SetGameobjectActive(m_objCoopNotice, bValue: true);
			if (NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_NextSessionStartDateUTC))
			{
				if (NKCGuildCoopManager.HasNextSessionData(NKCGuildCoopManager.m_NextSessionStartDateUTC))
				{
					if (!m_bCoopStatusChanged)
					{
						m_bCoopStatusChanged = true;
						SendPacket();
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_objCoopNotice, bValue: false);
				}
			}
			else
			{
				NKCUtil.SetLabelText(m_lbCoopRemainTime, NKCUtilString.GetTimeSpanStringDay(NKCGuildCoopManager.m_NextSessionStartDateUTC - NKCSynchronizedTime.GetServerUTCTime()));
			}
			break;
		case GuildDungeonState.Adjust:
			if (NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_NextSessionStartDateUTC) && !m_bCoopStatusChanged)
			{
				m_bCoopStatusChanged = true;
				SendPacket();
			}
			NKCUtil.SetLabelText(m_lbCoopRemainTime, NKCUtilString.GetTimeSpanStringDay(NKCGuildCoopManager.m_NextSessionStartDateUTC - NKCSynchronizedTime.GetServerUTCTime()));
			break;
		}
	}

	private void SendPacket()
	{
		if ((NKCSynchronizedTime.ServiceTime - m_tLastPacketSendTime).TotalSeconds > 10.0)
		{
			m_tLastPacketSendTime = NKCSynchronizedTime.ServiceTime;
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_INFO_REQ(NKCGuildManager.MyData.guildUid);
		}
	}

	public void ResetPosition()
	{
		m_scMain.normalizedPosition = new Vector2(0.5f, 0.5f);
	}

	private void OnIFChanged(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			str = NKCFilterManager.RemoveNewLine(str);
			m_IFNotice.text = NKCFilterManager.CheckBadChat(str);
		}
	}

	private void OnIFEndEdit(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			m_IFNotice.text = NKCFilterManager.CheckBadChat(str);
			if (NKCInputManager.IsChatSubmitEnter())
			{
				OnClickEditGuildNotice();
				EventSystem.current.SetSelectedGameObject(null);
			}
		}
	}

	private void OnClickEditGuildNotice()
	{
		if (m_btnEditGuildNotice.m_bLock)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_NOTICE_CHANGE_COOLTIME_TOAST_TEXT"));
		}
		else if (!string.IsNullOrWhiteSpace(m_IFNotice.text) && !string.Equals(m_IFNotice.text, NKCGuildManager.MyGuildData.notice))
		{
			NKCPacketSender.Send_NKMPacket_GUILD_UPDATE_NOTICE_REQ(NKCGuildManager.MyData.guildUid, m_IFNotice.text);
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_NOTICE_NOT_CHANGE_TOAST_TEXT"));
		}
	}

	private void OnClickCoop()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_DUNGEON) || NKCGuildCoopManager.GetFirstSeasonTemplet() == null)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.GUILD_DUNGEON);
		}
		else if (NKCGuildCoopManager.m_GuildDungeonState == GuildDungeonState.Invalid && !NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_NextSessionStartDateUTC))
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_CONSORTIUM_SEASON_OPEN_BEFORE_TOAST_TEXT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_NextSessionStartDateUTC) && NKCGuildCoopManager.HasNextSessionData(NKCGuildCoopManager.m_NextSessionStartDateUTC))
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().SetReserveMoveToCoopScen(bValue: true);
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_INFO_REQ(NKCGuildManager.MyData.guildUid);
		}
		else if (NKCGuildCoopManager.m_GuildDungeonState != GuildDungeonState.Invalid && GuildDungeonTempletManager.GetCurrentSeasonTemplet(ServiceTime.Recent) != null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_COOP);
		}
	}

	private void OnClickShop()
	{
		OpenShop();
	}

	public void OpenShop()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_SHOP))
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_NONE_SYSTEM"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else
		{
			NKCUIShop.ShopShortcut("TAB_EXCHANGE_GUILD_COIN");
		}
	}

	private void OnClickMission()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_MISSION))
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_NONE_SYSTEM"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else
		{
			m_dOnMoveToTab?.Invoke(NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Mission);
		}
	}

	private void OnClickManage()
	{
		m_dOnMoveToTab?.Invoke(NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Manage);
	}

	private void Update()
	{
		m_fDeltaTime += Time.deltaTime;
		if (m_fDeltaTime > 1f)
		{
			m_fDeltaTime -= 1f;
			if (NKCGuildManager.LastGuildNoticeChangedTimeUTC.Add(GuildTemplet.NoticeCooltime) > NKCSynchronizedTime.GetServerUTCTime())
			{
				m_IFNotice.enabled = false;
				m_btnEditGuildNotice.Lock();
			}
			else
			{
				m_IFNotice.enabled = true;
				m_btnEditGuildNotice.UnLock();
			}
			if (NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_DUNGEON) && NKCGuildCoopManager.GetFirstSeasonTemplet() != null)
			{
				SetCoopStatusTime();
			}
		}
	}
}
