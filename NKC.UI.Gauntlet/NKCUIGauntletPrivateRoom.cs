using System.Collections.Generic;
using ClientPacket.Pvp;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletPrivateRoom : NKCUIBase
{
	public enum ROOM_STATE
	{
		NORMAL,
		CHANGE_ROLE_1,
		CHANGE_ROLE_2,
		KICK
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_PRIVATE_ROOM";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public NKCUIComStateButton m_btnChat;

	public TMP_Text m_lbRoomCode;

	public NKCUIComToggle m_tglRevealRoomCode;

	public NKCUIComStateButton m_btnCopyRoomCode;

	public NKCUIComStateButton m_btnGameConfigChange;

	public TMP_Text m_lbObserverCount;

	public LoopScrollRect m_loopScrollRect;

	[Header("버튼")]
	public NKCUIComStateButton m_btnLeaveRoom;

	public NKCUIComStateButton m_btnInvite;

	public NKCUIComStateButton m_btnKick;

	public NKCUIComStateButton m_btnGlobalBan;

	[Header("대전 준비 버튼")]
	public NKCUIComToggle m_tglReadyPlayer1;

	public NKCComTMPUIText m_lbLoadingPlayer1;

	public NKCUIComToggle m_tglReadyPlayer2;

	public NKCComTMPUIText m_lbLoadingPlayer2;

	public GameObject m_objReadyBlockPlayer1;

	public GameObject m_objReadyBlockPlayer2;

	public Sprite m_sprReadyEnable;

	public Sprite m_sprReadyGray;

	public Color m_readyEnableTxtColor;

	public Color m_readyGrayTxtColor;

	[Header("대전 시작 버튼-호스트")]
	public GameObject m_objStartButtonRoot;

	public NKCUIComStateButton m_btnStartButtonOff;

	public NKCUIComStateButton m_btnStartButton;

	[Header("대전 시작 버튼-관전자")]
	public GameObject m_objStartDescRoot;

	public GameObject m_objStartPreparing;

	public GameObject m_objStartReady;

	[Header("대전 상대 설정 버튼")]
	public NKCUIComStateButton m_btnSetBattlePlayer1;

	public NKCUIComStateButton m_btnLeaveBattlePlayer1;

	public NKCUIComStateButton m_btnSetBattlePlayer2;

	public NKCUIComStateButton m_btnLeaveBattlePlayer2;

	public GameObject m_objSelectPlayer1Fx;

	public GameObject m_objSelectPlayer2Fx;

	[Header("유저 슬롯")]
	private const int PLAYER_COUNT_MAX = 2;

	public NKCUIGauntletPrivateRoomUserSlot[] m_slotPlayers = new NKCUIGauntletPrivateRoomUserSlot[2];

	public NKCUIGauntletPrivateRoomUserSlot m_slotHost;

	[Header("방 옵션")]
	public NKCUIGauntletPrivateRoomCustomOption m_customOption;

	public NKCComTMPUIText m_lbGameMode;

	[Space]
	public GameObject m_objButtonRoot;

	public GameObject m_objKickOkCancelRoot;

	public NKCUIComStateButton m_btnOK;

	public NKCUIComStateButton m_btnCancel;

	private static ROOM_STATE m_RoomState;

	private bool m_revealRoomCode;

	private List<NKMPvpGameLobbyUserState> m_playerList = new List<NKMPvpGameLobbyUserState>();

	private long m_kickUserUId;

	private NKCUIGauntletPrivateRoomInvite m_NKCUIGauntletPrivateRoomInvite;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override List<int> UpsideMenuShowResourceList => new List<int>();

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET;

	public static bool IsInstanceLoaded
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsLoadComplete;
			}
			return false;
		}
	}

	public NKCUIGauntletPrivateRoomInvite UIGauntletPrivateRoomInvite
	{
		get
		{
			if (m_NKCUIGauntletPrivateRoomInvite == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCUIGauntletPrivateRoomInvite>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_PRIVATE_ROOM_INVITE_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCUIGauntletPrivateRoomInvite = loadedUIData.GetInstance<NKCUIGauntletPrivateRoomInvite>();
				m_NKCUIGauntletPrivateRoomInvite.Init();
				NKCUtil.SetGameobjectActive(m_NKCUIGauntletPrivateRoomInvite, bValue: false);
			}
			return m_NKCUIGauntletPrivateRoomInvite;
		}
	}

	public static ROOM_STATE RoomState => m_RoomState;

	public override void CloseInternal()
	{
	}

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIGauntletPrivateRoom>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_PRIVATE_ROOM", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIGauntletPrivateRoom GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIGauntletPrivateRoom>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public void Init()
	{
		if (m_loopScrollRect != null)
		{
			m_loopScrollRect.dOnGetObject += GetSlot;
			m_loopScrollRect.dOnReturnObject += ReturnSlot;
			m_loopScrollRect.dOnProvideData += ProvideSlotData;
			m_loopScrollRect.dOnRepopulate += CalculateContentRectSize;
			CalculateContentRectSize();
			m_loopScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_loopScrollRect);
		}
		NKCUtil.SetButtonClickDelegate(m_btnLeaveRoom, OnTouchLeaveRoom);
		NKCUtil.SetButtonClickDelegate(m_btnCopyRoomCode, OnTouchCopyRoomCode);
		NKCUtil.SetButtonClickDelegate(m_btnGameConfigChange, OnTouchGameConfigChange);
		NKCUtil.SetToggleValueChangedDelegate(m_tglRevealRoomCode, OnToggleRevealRoomCode);
		NKCUtil.SetButtonClickDelegate(m_btnInvite, OnTouchInvite);
		NKCUtil.SetButtonClickDelegate(m_btnKick, OnTouchKick);
		NKCUtil.SetButtonClickDelegate(m_btnOK, OnTouchOK);
		NKCUtil.SetButtonClickDelegate(m_btnCancel, OnTouchCancel);
		NKCUtil.SetButtonClickDelegate(m_btnSetBattlePlayer1, OnClickSetBattlePlayer1);
		NKCUtil.SetButtonClickDelegate(m_btnLeaveBattlePlayer1, OnClickLeaveBattlePlayer1);
		NKCUtil.SetButtonClickDelegate(m_btnSetBattlePlayer2, OnClickSetBattlePlayer2);
		NKCUtil.SetButtonClickDelegate(m_btnLeaveBattlePlayer2, OnClickLeaveBattlePlayer2);
		NKCUtil.SetToggleValueChangedDelegate(m_tglReadyPlayer1, OnToggleReadyBattle1);
		NKCUtil.SetToggleValueChangedDelegate(m_tglReadyPlayer2, OnToggleReadyBattle2);
		NKCUtil.SetButtonClickDelegate(m_btnStartButtonOff, OnClickOnClickStartBattleOff);
		NKCUtil.SetButtonClickDelegate(m_btnStartButton, OnClickStartBattle);
		NKCUtil.SetButtonClickDelegate(m_btnChat, OnClickChat);
		NKCUtil.SetButtonClickDelegate(m_btnGlobalBan, OnClickGlobalBan);
		NKCUIGauntletPrivateRoomUserSlot[] slotPlayers = m_slotPlayers;
		for (int i = 0; i < slotPlayers.Length; i++)
		{
			slotPlayers[i].Init(OnSelectUserSlot);
		}
		m_slotPlayers[0].SetPlayerRole(PvpPlayerRole.PlayerA);
		m_slotPlayers[1].SetPlayerRole(PvpPlayerRole.PlayerB);
		m_slotHost.Init(OnSelectUserSlot);
		m_slotHost.SetPlayerRole(PvpPlayerRole.Observer);
		m_customOption?.Init();
	}

	public override void UnHide()
	{
		NKMPvpGameLobbyUserState myPvpGameLobbyUserState = NKCPrivatePVPRoomMgr.GetMyPvpGameLobbyUserState();
		if (myPvpGameLobbyUserState != null && myPvpGameLobbyUserState.playerState != LobbyPlayerState.Lobby)
		{
			NKCPacketSender.Send_NKMPacket_PRIVATE_PVP_STATE_REQ(LobbyPlayerState.Lobby);
		}
		base.UnHide();
		RefreshUI();
	}

	public void Open()
	{
		m_revealRoomCode = false;
		m_tglRevealRoomCode?.SetLock(value: false);
		m_tglRevealRoomCode?.Select(m_revealRoomCode, bForce: true);
		m_tglReadyPlayer1?.Select(bSelect: false, bForce: true);
		m_tglReadyPlayer2?.Select(bSelect: false, bForce: true);
		NKCUtil.SetGameobjectActive(m_objReadyBlockPlayer1, bValue: false);
		NKCUtil.SetGameobjectActive(m_objReadyBlockPlayer2, bValue: false);
		RefreshUI(initScrollPosition: true);
		if (!base.IsOpen)
		{
			UIOpened();
			NKMPvpGameLobbyUserState myPvpGameLobbyUserState = NKCPrivatePVPRoomMgr.GetMyPvpGameLobbyUserState();
			if (myPvpGameLobbyUserState != null && myPvpGameLobbyUserState.playerState != LobbyPlayerState.Lobby)
			{
				NKCPacketSender.Send_NKMPacket_PRIVATE_PVP_STATE_REQ(LobbyPlayerState.Lobby);
			}
		}
		if (NKCPrivatePVPRoomMgr.HasInviteData)
		{
			NKCPopupGauntletInvite.ClosePopupBox();
			NKCPopupGauntletInvite.OpenOKTimerBox(NKCUtilString.GET_STRING_FRIEND_PVP, NKCUtilString.GET_STRING_PRIVATE_PVP_INVITE_REQ, NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_REQ, 10f, NKCUtilString.GET_STRING_CANCEL, NKCUtilString.GET_STRING_PRIVATE_PVP_AUTO_CANCEL_ID, NKCPrivatePVPRoomMgr.GetTargetFriendListData(), NKCPrivatePVPRoomMgr.PrivateGameConfig);
		}
		else if (NKCPrivatePVPRoomMgr.GetInviteErrorCode() != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPrivatePVPRoomMgr.ShowInviteErrorPopup();
		}
	}

	public void SetUI()
	{
		RefreshUI();
	}

	public void RefreshUI(bool initScrollPosition = false, bool initRoomState = true)
	{
		bool flag = NKCPrivatePVPRoomMgr.IsHost(NKCPrivatePVPRoomMgr.GetMyPvpGameLobbyUserState());
		NKCUtil.SetGameobjectActive(m_btnCopyRoomCode, flag);
		m_tglRevealRoomCode?.SetLock(!flag);
		NKCUtil.SetGameobjectActive(m_objButtonRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_btnGlobalBan, bValue: true);
		NKCUtil.SetGameobjectActive(m_btnInvite, flag);
		NKCUtil.SetGameobjectActive(m_btnKick, flag);
		NKCUtil.SetGameobjectActive(m_btnSetBattlePlayer1, flag);
		NKCUtil.SetGameobjectActive(m_btnLeaveBattlePlayer1, flag);
		NKCUtil.SetGameobjectActive(m_btnSetBattlePlayer2, flag);
		NKCUtil.SetGameobjectActive(m_btnLeaveBattlePlayer2, flag);
		RefreshRoomCode();
		RefreshPlayer();
		RefreshObserver(initScrollPosition);
		RefreshCustomOption();
		RefreshReadyState(flag);
		m_slotHost?.SetUI(NKCPrivatePVPRoomMgr.GetHostGameLobbyUserState());
		if (initRoomState)
		{
			SetRoomState(ROOM_STATE.NORMAL);
		}
		if (UIGauntletPrivateRoomInvite != null && UIGauntletPrivateRoomInvite.IsOpen)
		{
			UIGauntletPrivateRoomInvite.RefreshObserveCount();
		}
	}

	private void RefreshRoomCode()
	{
		if (NKCPrivatePVPRoomMgr.IsHost(NKCPrivatePVPRoomMgr.GetMyPvpGameLobbyUserState()))
		{
			if (m_revealRoomCode)
			{
				NKCUtil.SetLabelText(m_lbRoomCode, NKCPrivatePVPRoomMgr.LobbyData.lobbyCode);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbRoomCode, "********");
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRoomCode, "********");
		}
	}

	private void RefreshPlayer()
	{
		List<NKMPvpGameLobbyUserState> users = NKCPrivatePVPRoomMgr.LobbyData.users;
		for (int i = 0; i < users.Count && i < 2; i++)
		{
			if (users[i] == null)
			{
				m_slotPlayers[i].SetEmptyUI();
				continue;
			}
			m_slotPlayers[i].SetUI(users[i]);
			switch (users[i].playerState)
			{
			case LobbyPlayerState.Setting:
				NKCUtil.SetLabelText((i == 0) ? m_lbLoadingPlayer1 : m_lbLoadingPlayer2, NKCUtilString.GET_STRING_FRIENDLY_MATCH_READY_DECK);
				break;
			case LobbyPlayerState.Lobby:
			case LobbyPlayerState.InGame:
			case LobbyPlayerState.Result:
				NKCUtil.SetLabelText((i == 0) ? m_lbLoadingPlayer1 : m_lbLoadingPlayer2, NKCUtilString.GET_STRING_READYING);
				break;
			}
		}
	}

	private void RefreshObserver(bool initScrollPosition)
	{
		m_playerList = NKCPrivatePVPRoomMgr.GetPlayerList();
		m_loopScrollRect.TotalCount = m_playerList.Count;
		if (initScrollPosition)
		{
			m_loopScrollRect.SetIndexPosition(0);
		}
		else
		{
			m_loopScrollRect.RefreshCells();
		}
		NKCUtil.SetLabelText(m_lbObserverCount, $"{m_playerList.Count + 1}/{NKMPvpCommonConst.Instance.PvpRoomMaxPlayerCount}");
	}

	private void RefreshCustomOption()
	{
		m_customOption?.SetOption(NKCPrivatePVPRoomMgr.LobbyData.config);
		NKCUtil.SetLabelText(m_lbGameMode, GetGameModeName(NKCPrivatePVPRoomMgr.LobbyData.config));
	}

	private string GetGameModeName(NKMPrivateGameConfig config)
	{
		if (config.draftBanMode)
		{
			return NKCStringTable.GetString("PVP_FRIENDLY_OPTION_DRAFT_BAN");
		}
		return NKCStringTable.GetString("PVP_FRIENDLY_OPTION_NORMAL");
	}

	private void RefreshReadyState(bool isHost)
	{
		long num = NKCScenManager.CurrentUserData()?.m_UserUID ?? 0;
		bool flag = false;
		bool flag2 = false;
		if (NKCPrivatePVPRoomMgr.LobbyData.users[0] != null)
		{
			flag = NKCPrivatePVPRoomMgr.LobbyData.users[0].isReady;
			m_tglReadyPlayer1.Select(flag, bForce: true);
			bool flag3 = num == NKCPrivatePVPRoomMgr.LobbyData.users[0].profileData.commonProfile.userUid;
			SetReadyButtonColor(m_tglReadyPlayer1, flag3);
			NKCUtil.SetGameobjectActive(m_objReadyBlockPlayer1, !flag3);
		}
		if (NKCPrivatePVPRoomMgr.LobbyData.users[1] != null)
		{
			flag2 = NKCPrivatePVPRoomMgr.LobbyData.users[1].isReady;
			m_tglReadyPlayer2.Select(flag2, bForce: true);
			bool flag4 = num == NKCPrivatePVPRoomMgr.LobbyData.users[1].profileData.commonProfile.userUid;
			SetReadyButtonColor(m_tglReadyPlayer2, flag4);
			NKCUtil.SetGameobjectActive(m_objReadyBlockPlayer2, !flag4);
		}
		NKCUtil.SetGameobjectActive(m_objStartButtonRoot, isHost);
		NKCUtil.SetGameobjectActive(m_objStartDescRoot, !isHost);
		bool flag5 = flag && flag2;
		bool flag6 = true;
		for (int i = 0; i < NKCPrivatePVPRoomMgr.LobbyData.observers.Count; i++)
		{
			if (NKCPrivatePVPRoomMgr.LobbyData.observers[i].playerState != LobbyPlayerState.Lobby)
			{
				flag6 = false;
				break;
			}
		}
		if (isHost)
		{
			NKCUtil.SetGameobjectActive(m_btnStartButtonOff, !flag5 || !flag6);
			NKCUtil.SetGameobjectActive(m_btnStartButton, flag5 && flag6);
			NKCUtil.SetGameobjectActive(m_btnSetBattlePlayer1, !flag);
			NKCUtil.SetGameobjectActive(m_btnLeaveBattlePlayer1, !flag);
			NKCUtil.SetGameobjectActive(m_btnSetBattlePlayer2, !flag2);
			NKCUtil.SetGameobjectActive(m_btnLeaveBattlePlayer2, !flag2);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objStartPreparing, !flag5);
			NKCUtil.SetGameobjectActive(m_objStartReady, flag5);
		}
	}

	private void SetReadyButtonColor(NKCUIComToggle m_tglReadyPlayer, bool isMySlot)
	{
		NKCUtil.SetImageSprite(m_tglReadyPlayer.m_ButtonBG_Normal.GetComponent<Image>(), isMySlot ? m_sprReadyEnable : m_sprReadyGray);
		NKCUtil.SetLabelTextColor(m_tglReadyPlayer.m_ButtonBG_Normal.GetComponentInChildren<TMP_Text>(), isMySlot ? m_readyEnableTxtColor : m_readyGrayTxtColor);
	}

	private void SetRoomState(ROOM_STATE roomState)
	{
		m_RoomState = roomState;
		switch (m_RoomState)
		{
		case ROOM_STATE.NORMAL:
			m_slotHost.SetSelectableActive(value: false);
			m_loopScrollRect.RefreshCells();
			NKCUtil.SetGameobjectActive(m_objButtonRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnKick, NKCPrivatePVPRoomMgr.IsHost(NKCPrivatePVPRoomMgr.GetMyPvpGameLobbyUserState()));
			NKCUtil.SetGameobjectActive(m_btnInvite, NKCPrivatePVPRoomMgr.IsHost(NKCPrivatePVPRoomMgr.GetMyPvpGameLobbyUserState()));
			NKCUtil.SetGameobjectActive(m_objKickOkCancelRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectPlayer1Fx, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectPlayer2Fx, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectPlayer1Fx, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSelectPlayer2Fx, bValue: true);
			m_kickUserUId = 0L;
			break;
		case ROOM_STATE.KICK:
			NKCUtil.SetGameobjectActive(m_objButtonRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objKickOkCancelRoot, bValue: true);
			break;
		case ROOM_STATE.CHANGE_ROLE_1:
			m_slotHost.SetSelectableActive(value: true);
			m_loopScrollRect.RefreshCells();
			NKCUtil.SetGameobjectActive(m_objSelectPlayer1Fx, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectPlayer2Fx, bValue: false);
			break;
		case ROOM_STATE.CHANGE_ROLE_2:
			m_slotHost.SetSelectableActive(value: true);
			m_loopScrollRect.RefreshCells();
			NKCUtil.SetGameobjectActive(m_objSelectPlayer1Fx, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectPlayer2Fx, bValue: false);
			break;
		}
	}

	private void CalculateContentRectSize()
	{
		m_loopScrollRect.SetAutoResize(1);
	}

	public RectTransform GetSlot(int index)
	{
		NKCUIGauntletPrivateRoomUserSlot newInstance = NKCUIGauntletPrivateRoomUserSlot.GetNewInstance(null, null);
		if (newInstance != null)
		{
			newInstance.Init(OnSelectUserSlot);
			newInstance.SetPlayerRole(PvpPlayerRole.Observer);
		}
		return newInstance?.GetComponent<RectTransform>();
	}

	public void ReturnSlot(Transform tr)
	{
		NKCUIGauntletPrivateRoomUserSlot component = tr.GetComponent<NKCUIGauntletPrivateRoomUserSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideSlotData(Transform tr, int index)
	{
		NKCUIGauntletPrivateRoomUserSlot component = tr.GetComponent<NKCUIGauntletPrivateRoomUserSlot>();
		if (!(component != null))
		{
			return;
		}
		if (index < m_playerList.Count && m_playerList[index] != null)
		{
			component.SetUI(m_playerList[index]);
			switch (m_RoomState)
			{
			case ROOM_STATE.CHANGE_ROLE_1:
			case ROOM_STATE.CHANGE_ROLE_2:
				component.SetSelectableActive(value: true);
				break;
			case ROOM_STATE.KICK:
				component.SetKickSelectActive(m_kickUserUId == m_playerList[index].profileData.commonProfile.userUid);
				break;
			}
		}
		else
		{
			component.SetEmptyUI();
		}
	}

	private void OnTouchLeaveRoom()
	{
		if (m_RoomState != ROOM_STATE.NORMAL)
		{
			SetRoomState(ROOM_STATE.NORMAL);
		}
		else if (NKCPrivatePVPRoomMgr.IsHost(NKCPrivatePVPRoomMgr.GetMyPvpGameLobbyUserState()))
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_PRIVATE_PVP_READY_CANCEL_TITLE, NKCUtilString.GET_STRING_PRIVATE_PVP_HOST_TERMINATE_ROOM, NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_EXIT_REQ);
		}
		else
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_PRIVATE_PVP_READY_CANCEL_TITLE, NKCUtilString.GET_STRING_PRIVATE_PVP_OBSERVE_LEAVE_ROOM, NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_EXIT_REQ);
		}
	}

	private void OnTouchStartDeckSelect()
	{
		NKCPopupOKCancel.OpenOKCancelBox("대전 시작", "덱 구성 단계로 넘어가시겠습니까?", NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_LOBBY_START_GAME_SETTING_REQ);
	}

	public override void OnBackButton()
	{
		OnTouchLeaveRoom();
	}

	public void ProcessBackButton()
	{
		base.OnBackButton();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	private void OnTouchCopyRoomCode()
	{
		TextEditor textEditor = new TextEditor();
		textEditor.text = NKCPrivatePVPRoomMgr.LobbyData.lobbyCode;
		textEditor.OnFocus();
		textEditor.Copy();
		NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_TOAST_PVP_FRIENDLY_CODE_COPY"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
	}

	private bool OnSelectUserSlot(long userUId)
	{
		switch (m_RoomState)
		{
		case ROOM_STATE.KICK:
			m_kickUserUId = userUId;
			m_loopScrollRect.RefreshCells();
			return true;
		case ROOM_STATE.CHANGE_ROLE_1:
			if (m_slotPlayers[0].UserUID != userUId)
			{
				NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ(userUId, PvpPlayerRole.PlayerA);
			}
			else
			{
				SetRoomState(ROOM_STATE.NORMAL);
				RefreshPlayer();
			}
			return true;
		case ROOM_STATE.CHANGE_ROLE_2:
			if (m_slotPlayers[1].UserUID != userUId)
			{
				NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ(userUId, PvpPlayerRole.PlayerB);
			}
			else
			{
				SetRoomState(ROOM_STATE.NORMAL);
				RefreshPlayer();
			}
			return true;
		default:
			return false;
		}
	}

	private void OnTouchGameConfigChange()
	{
		NKCPopupGauntletPrivateRoomOption.Instance.Open();
	}

	private void OnToggleRevealRoomCode(bool value)
	{
		m_revealRoomCode = value;
		RefreshRoomCode();
	}

	private void OnTouchInvite()
	{
		if (m_RoomState != ROOM_STATE.NORMAL)
		{
			SetRoomState(ROOM_STATE.NORMAL);
		}
		NKCPrivatePVPRoomMgr.ShowInvitePopup();
	}

	private void OnTouchKick()
	{
		if (m_RoomState != ROOM_STATE.NORMAL)
		{
			SetRoomState(ROOM_STATE.NORMAL);
		}
		SetRoomState(ROOM_STATE.KICK);
	}

	private void OnTouchOK()
	{
		if (m_RoomState == ROOM_STATE.KICK && m_kickUserUId > 0)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_PRIVATE_PVP_USER_KICK, delegate
			{
				NKCPacketSender.Send_NKMPacket_PVP_ROOM_KICK_REQ(m_kickUserUId);
			});
		}
	}

	private void OnTouchCancel()
	{
		SetRoomState(ROOM_STATE.NORMAL);
	}

	private void OnClickSetBattlePlayer1()
	{
		if (m_RoomState != ROOM_STATE.KICK)
		{
			SetRoomState(ROOM_STATE.CHANGE_ROLE_1);
		}
	}

	private void OnClickSetBattlePlayer2()
	{
		if (m_RoomState != ROOM_STATE.KICK)
		{
			SetRoomState(ROOM_STATE.CHANGE_ROLE_2);
		}
	}

	private void OnClickLeaveBattlePlayer1()
	{
		if (m_RoomState == ROOM_STATE.KICK)
		{
			return;
		}
		if (m_slotPlayers[0].UserUID <= 0)
		{
			if (m_RoomState != ROOM_STATE.NORMAL)
			{
				SetRoomState(ROOM_STATE.NORMAL);
			}
		}
		else
		{
			NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ(m_slotPlayers[0].UserUID, PvpPlayerRole.Observer);
		}
	}

	private void OnClickLeaveBattlePlayer2()
	{
		if (m_RoomState == ROOM_STATE.KICK)
		{
			return;
		}
		if (m_slotPlayers[1].UserUID <= 0)
		{
			if (m_RoomState != ROOM_STATE.NORMAL)
			{
				SetRoomState(ROOM_STATE.NORMAL);
			}
		}
		else
		{
			NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ(m_slotPlayers[1].UserUID, PvpPlayerRole.Observer);
		}
	}

	private void OnToggleReadyBattle1(bool value)
	{
		if (NKCPrivatePVPRoomMgr.LobbyData.users[0] == null)
		{
			m_tglReadyPlayer1.Select(value, bForce: true);
			return;
		}
		NKMDeckIndex deckIndex = NKCPrivatePVPRoomMgr.LobbyData.users[0].deckIndex;
		if (value)
		{
			if (NKCPrivatePVPRoomMgr.LobbyData.config.draftBanMode)
			{
				NKM_ERROR_CODE nKM_ERROR_CODE = NKCPrivatePVPRoomMgr.CanPlayPVPDraftGame(NKCScenManager.CurrentUserData());
				if (NKCPrivatePVPRoomMgr.LobbyData.config.draftBanMode && nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
				{
					m_tglReadyPlayer1.Select(bSelect: false, bForce: true);
					NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
					return;
				}
			}
			else
			{
				NKM_ERROR_CODE nKM_ERROR_CODE2 = NKMMain.IsValidDeck(NKCScenManager.CurrentUserData().m_ArmyData, deckIndex.m_eDeckType, deckIndex.m_iIndex, NKM_GAME_TYPE.NGT_PVP_PRIVATE);
				if (nKM_ERROR_CODE2 != NKM_ERROR_CODE.NEC_OK)
				{
					m_tglReadyPlayer1.Select(bSelect: false, bForce: true);
					NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE2);
					return;
				}
			}
		}
		NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_READY_REQ(deckIndex, value);
	}

	private void OnToggleReadyBattle2(bool value)
	{
		if (NKCPrivatePVPRoomMgr.LobbyData.users[1] == null)
		{
			m_tglReadyPlayer2.Select(value, bForce: true);
			return;
		}
		NKMDeckIndex deckIndex = NKCPrivatePVPRoomMgr.LobbyData.users[1].deckIndex;
		if (value)
		{
			if (NKCPrivatePVPRoomMgr.LobbyData.config.draftBanMode)
			{
				NKM_ERROR_CODE nKM_ERROR_CODE = NKCPrivatePVPRoomMgr.CanPlayPVPDraftGame(NKCScenManager.CurrentUserData());
				if (NKCPrivatePVPRoomMgr.LobbyData.config.draftBanMode && nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
				{
					m_tglReadyPlayer2.Select(bSelect: false, bForce: true);
					NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
					return;
				}
			}
			else
			{
				NKM_ERROR_CODE nKM_ERROR_CODE2 = NKMMain.IsValidDeck(NKCScenManager.CurrentUserData().m_ArmyData, deckIndex.m_eDeckType, deckIndex.m_iIndex, NKM_GAME_TYPE.NGT_PVP_PRIVATE);
				if (nKM_ERROR_CODE2 != NKM_ERROR_CODE.NEC_OK)
				{
					m_tglReadyPlayer2.Select(bSelect: false, bForce: true);
					NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE2);
					return;
				}
			}
		}
		NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_READY_REQ(deckIndex, value);
	}

	private void OnClickStartBattle()
	{
		if (NKCPrivatePVPRoomMgr.LobbyData.users[0] == null || NKCPrivatePVPRoomMgr.LobbyData.users[1] == null || !NKCPrivatePVPRoomMgr.LobbyData.users[0].isReady || !NKCPrivatePVPRoomMgr.LobbyData.users[1].isReady)
		{
			return;
		}
		for (int i = 0; i < NKCPrivatePVPRoomMgr.LobbyData.observers.Count; i++)
		{
			if (NKCPrivatePVPRoomMgr.LobbyData.observers[i].playerState != LobbyPlayerState.Lobby)
			{
				return;
			}
		}
		NKCPacketSender.Send_NKMPacket_PRIVATE_PVP_START_REQ();
	}

	private void OnClickOnClickStartBattleOff()
	{
		if (!NKCPrivatePVPRoomMgr.LobbyData.users[0].isReady || !NKCPrivatePVPRoomMgr.LobbyData.users[1].isReady)
		{
			return;
		}
		for (int i = 0; i < NKCPrivatePVPRoomMgr.LobbyData.observers.Count; i++)
		{
			if (NKCPrivatePVPRoomMgr.LobbyData.observers[i].playerState != LobbyPlayerState.Lobby)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_POPUP_NOTICE_FRIENDLY_MATCH_AFTER_LOADING);
				break;
			}
		}
	}

	private void OnClickChat()
	{
		if (NKMOpenTagManager.IsOpened("CHAT_PRIVATE"))
		{
			bool bAdmin;
			switch (NKCContentManager.CheckContentStatus(ContentsType.FRIENDS, out bAdmin))
			{
			case NKCContentManager.eContentStatus.Open:
				if (NKCScenManager.GetScenManager().GetGameOptionData().UseChatContent)
				{
					NKCPopupPrivateChatLobby.Instance.Open(0L);
				}
				else
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_OPTION_GAME_CHAT_NOTICE);
				}
				break;
			case NKCContentManager.eContentStatus.Lock:
				NKCContentManager.ShowLockedMessagePopup(ContentsType.FRIENDS);
				break;
			}
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_COMING_SOON_SYSTEM);
		}
	}

	private void OnClickGlobalBan()
	{
		NKCPopupGauntletBan.Instance.Open();
	}
}
