using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Lobby;
using ClientPacket.Pvp;
using Cs.Logging;
using NKC.PacketHandler;
using NKC.UI;
using NKC.UI.Gauntlet;
using NKC.UI.Shop;
using NKM;

namespace NKC;

public static class NKCPrivatePVPRoomMgr
{
	private static NKMLobbyData m_LobbyData = null;

	private static bool isInviteSender = false;

	private static NKM_ERROR_CODE m_inviteErrorCode = NKM_ERROR_CODE.NEC_OK;

	private static NKMUserProfileData m_targetProfile;

	private static FriendListData m_targetFriendListData;

	private static long m_currentInviteUserUID = 0L;

	private static float WaitTimeSenderInviteReq = 10f;

	private static bool isCancelNotPopupOpened = false;

	private static List<FriendListData> m_searchResult = new List<FriendListData>();

	private static NKMPrivateGameConfig m_privateGameConfig = null;

	public static NKMLobbyData LobbyData => m_LobbyData;

	public static bool HasInviteData => isInviteSender;

	public static bool CancelNotPopupOpened => isCancelNotPopupOpened;

	public static List<FriendListData> SearchResult => m_searchResult;

	public static bool PrivatePVPLobbyBanUpState
	{
		get
		{
			if (m_LobbyData != null)
			{
				return m_LobbyData.config.applyBanUpSystem;
			}
			return false;
		}
	}

	public static bool IsDraftBanMode
	{
		get
		{
			if (m_privateGameConfig != null)
			{
				return m_privateGameConfig.draftBanMode;
			}
			return false;
		}
	}

	public static NKMPrivateGameConfig PrivateGameConfig
	{
		get
		{
			if (m_privateGameConfig == null)
			{
				m_privateGameConfig = new NKMPrivateGameConfig();
				m_privateGameConfig.applyEquipStat = true;
			}
			return m_privateGameConfig;
		}
	}

	public static NKM_ERROR_CODE GetInviteErrorCode()
	{
		return m_inviteErrorCode;
	}

	public static FriendListData GetTargetFriendListData()
	{
		return m_targetFriendListData;
	}

	public static void SetLobbyData(NKMLobbyData lobbyData)
	{
		if (lobbyData == null)
		{
			Log.Error("NKMLobbyData is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPrivatePVPRoomMgr.cs", 70);
		}
		else
		{
			m_LobbyData = lobbyData;
		}
	}

	public static void SetLocalConfig(NKMPrivateGameConfig config)
	{
		m_privateGameConfig = new NKMPrivateGameConfig();
		m_privateGameConfig.applyAllUnitMaxLevel = config.applyAllUnitMaxLevel;
		m_privateGameConfig.applyEquipStat = config.applyEquipStat;
		m_privateGameConfig.applyBanUpSystem = config.applyBanUpSystem;
		m_privateGameConfig.draftBanMode = config.draftBanMode;
	}

	public static void ResetData()
	{
		m_LobbyData = null;
		m_privateGameConfig = null;
		isCancelNotPopupOpened = false;
		ResetInviteData();
		ResetSearchData();
	}

	public static void ResetSearchData()
	{
		m_searchResult.Clear();
	}

	public static void ResetInviteData()
	{
		isInviteSender = false;
		m_targetProfile = null;
		m_targetFriendListData = null;
	}

	private static void ChangeScene(NKM_SCEN_ID scenID)
	{
		NKCScenManager.GetScenManager().ScenChangeFade(scenID);
	}

	public static void CancelInviteProcess()
	{
		ResetInviteData();
	}

	public static void CancelAllProcess()
	{
		ResetData();
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM().OnCancelAllProcess();
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME_RESULT)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
		}
	}

	public static void OnClickAcceptInvite()
	{
		Send_NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ(bAccept: true, m_currentInviteUserUID);
	}

	public static void OnClickRefuseInvite()
	{
		Send_NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ(bAccept: false, m_currentInviteUserUID);
		CancelAllProcess();
	}

	public static void ShowInvitePopup()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM()?.GauntletPrivateRoom?.UIGauntletPrivateRoomInvite.Open();
	}

	private static void RefreshLobbyData(NKMLobbyData lobbyState)
	{
		SetLobbyData(lobbyState);
		if (m_LobbyData != null)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM()?.GauntletPrivateRoom?.SetUI();
		}
	}

	public static NKMPvpGameLobbyUserState GetMyPvpGameLobbyUserState()
	{
		if (m_LobbyData == null)
		{
			return null;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (m_LobbyData.users != null)
		{
			foreach (NKMPvpGameLobbyUserState user in m_LobbyData.users)
			{
				if (user != null && user.profileData.commonProfile.userUid == myUserData.m_UserUID)
				{
					return user;
				}
			}
		}
		if (m_LobbyData.observers != null)
		{
			foreach (NKMPvpGameLobbyUserState observer in m_LobbyData.observers)
			{
				if (observer.profileData.commonProfile.userUid == myUserData.m_UserUID)
				{
					return observer;
				}
			}
		}
		return null;
	}

	public static NKMPvpGameLobbyUserState GetTargetPvpGameLobbyUserState()
	{
		if (m_LobbyData == null)
		{
			return null;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		foreach (NKMPvpGameLobbyUserState user in m_LobbyData.users)
		{
			if (user != null && user.profileData.commonProfile.userUid != myUserData.m_UserUID)
			{
				return user;
			}
		}
		return null;
	}

	public static NKMPvpGameLobbyUserState GetLeftPvpGameLobbyUserState()
	{
		if (IsPlayer(GetMyPvpGameLobbyUserState()))
		{
			return GetMyPvpGameLobbyUserState();
		}
		return m_LobbyData.users[0];
	}

	public static NKMPvpGameLobbyUserState GetRightPvpGameLobbyUserState()
	{
		if (IsPlayer(GetMyPvpGameLobbyUserState()))
		{
			return GetTargetPvpGameLobbyUserState();
		}
		return m_LobbyData.users[1];
	}

	public static NKMPvpGameLobbyUserState GetHostGameLobbyUserState()
	{
		if (m_LobbyData == null)
		{
			return null;
		}
		foreach (NKMPvpGameLobbyUserState user in m_LobbyData.users)
		{
			if (user != null && user.isHost)
			{
				return user;
			}
		}
		foreach (NKMPvpGameLobbyUserState observer in m_LobbyData.observers)
		{
			if (observer.isHost)
			{
				return observer;
			}
		}
		return null;
	}

	public static List<NKMPvpGameLobbyUserState> GetPlayerList()
	{
		List<NKMPvpGameLobbyUserState> playerList = new List<NKMPvpGameLobbyUserState>();
		if (m_LobbyData.users != null)
		{
			m_LobbyData.users.ForEach(delegate(NKMPvpGameLobbyUserState e)
			{
				if (e != null)
				{
					playerList.Add(e);
				}
			});
		}
		if (m_LobbyData.observers != null)
		{
			playerList.AddRange(m_LobbyData.observers);
		}
		int num = playerList.FindIndex((NKMPvpGameLobbyUserState e) => e.isHost);
		if (num >= 0)
		{
			playerList.RemoveAt(num);
		}
		return playerList;
	}

	public static bool CanEditDeck()
	{
		if (!IsPlayer(GetMyPvpGameLobbyUserState()))
		{
			return false;
		}
		return true;
	}

	public static bool IsHost(NKMPvpGameLobbyUserState userState)
	{
		if (userState == null)
		{
			return false;
		}
		if (userState.isHost)
		{
			return true;
		}
		return false;
	}

	public static bool IsPlayer(long userUid)
	{
		if (m_LobbyData == null)
		{
			return false;
		}
		foreach (NKMPvpGameLobbyUserState user in m_LobbyData.users)
		{
			if (user != null && user.profileData.commonProfile.userUid == userUid)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsObserver(long userUid)
	{
		if (m_LobbyData == null)
		{
			return false;
		}
		foreach (NKMPvpGameLobbyUserState observer in m_LobbyData.observers)
		{
			if (observer != null && observer.profileData.commonProfile.userUid == userUid)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsPlayer(NKMPvpGameLobbyUserState userState)
	{
		if (userState == null)
		{
			return false;
		}
		if (m_LobbyData == null)
		{
			return false;
		}
		foreach (NKMPvpGameLobbyUserState user in m_LobbyData.users)
		{
			if (user != null && user.profileData.commonProfile.userUid == userState.profileData.commonProfile.userUid)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsObserver(NKMPvpGameLobbyUserState userState)
	{
		if (userState == null)
		{
			return false;
		}
		if (m_LobbyData == null)
		{
			return false;
		}
		foreach (NKMPvpGameLobbyUserState observer in m_LobbyData.observers)
		{
			if (observer.profileData.commonProfile.userUid == userState.profileData.commonProfile.userUid)
			{
				return true;
			}
		}
		return false;
	}

	public static int GetPlayerSlotIndex(NKMPvpGameLobbyUserState userState)
	{
		if (userState == null)
		{
			return -1;
		}
		if (m_LobbyData == null)
		{
			return -1;
		}
		for (int i = 0; i < m_LobbyData.users.Count; i++)
		{
			if (m_LobbyData.users[i] != null && m_LobbyData.users[i] == userState)
			{
				return i;
			}
		}
		return -1;
	}

	public static bool IsInProgress()
	{
		if (isInviteSender)
		{
			return true;
		}
		if (m_targetProfile != null)
		{
			return true;
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
		{
			return true;
		}
		return false;
	}

	public static NKM_ERROR_CODE CanPlayPVPDraftGame(NKMUserData userData)
	{
		if (userData.m_ArmyData.GetUnitTypeCount() < NKMPvpCommonConst.Instance.DraftBan.MinUnitCount)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_NOT_ENOUGH_UNIT_COUNT;
		}
		if (userData.m_ArmyData.GetShipTypeCount() < NKMPvpCommonConst.Instance.DraftBan.MinShipCount)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_NOT_ENOUGH_SHIP_COUNT;
		}
		if (!IsValidGlobalBanUnit())
		{
			return NKM_ERROR_CODE.NEC_FAIL_LOBBY_DRAFT_INVALID_BAN_UNIT;
		}
		if (!IsValidGlobalBanShip())
		{
			return NKM_ERROR_CODE.NEC_FAIL_LOBBY_DRAFT_INVALID_BAN_SHIP;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	private static bool IsValidGlobalBanUnit()
	{
		List<int> unitIdList = NKCBanManager.m_GlobalVoteData.unitIdList;
		if (unitIdList == null || unitIdList.Count < 2)
		{
			return false;
		}
		for (int i = 0; i < 2; i++)
		{
			if (unitIdList[i] < 0)
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsValidGlobalBanShip()
	{
		List<int> shipGroupIdList = NKCBanManager.m_GlobalVoteData.shipGroupIdList;
		if (shipGroupIdList == null || shipGroupIdList.Count < 1)
		{
			return false;
		}
		for (int i = 0; i < 1; i++)
		{
			if (shipGroupIdList[i] < 0)
			{
				return false;
			}
		}
		return true;
	}

	public static void Send_NKMPacket_PRIVATE_PVP_LOBBY_CREATE_REQ(FriendListData friendListData = null)
	{
		long num = 0L;
		if (friendListData != null)
		{
			isInviteSender = true;
			m_targetFriendListData = friendListData;
			num = friendListData.commonProfile.friendCode;
			m_currentInviteUserUID = friendListData.commonProfile.userUid;
		}
		NKMPacket_PRIVATE_PVP_LOBBY_CREATE_REQ nKMPacket_PRIVATE_PVP_LOBBY_CREATE_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_CREATE_REQ();
		nKMPacket_PRIVATE_PVP_LOBBY_CREATE_REQ.config = PrivateGameConfig;
		nKMPacket_PRIVATE_PVP_LOBBY_CREATE_REQ.inviteFriendCode = num;
		if (num == 0L)
		{
			ResetData();
		}
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_CREATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CREATE_ACK sPacket)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM))
		{
			SetLocalConfig(sPacket.lobbyState.config);
			SetLobbyData(sPacket.lobbyState);
			ChangeScene(NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM);
		}
	}

	public static void Send_NKMPacket_PRIVATE_PVP_LOBBY_INVITE_REQ(FriendListData friendListData)
	{
		isInviteSender = true;
		m_targetProfile = null;
		m_targetFriendListData = friendListData;
		m_currentInviteUserUID = friendListData.commonProfile.userUid;
		NKMPacket_PRIVATE_PVP_LOBBY_INVITE_REQ nKMPacket_PRIVATE_PVP_LOBBY_INVITE_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_INVITE_REQ();
		nKMPacket_PRIVATE_PVP_LOBBY_INVITE_REQ.friendCode = friendListData.commonProfile.friendCode;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_INVITE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_INVITE_ACK sPacket)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM))
		{
			m_inviteErrorCode = sPacket.errorCode;
			if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
			{
				ResetInviteData();
				return;
			}
			isInviteSender = true;
			NKCPopupGauntletInvite.ClosePopupBox();
			NKCPopupGauntletInvite.OpenOKTimerBox(NKCUtilString.GET_STRING_FRIEND_PVP, NKCUtilString.GET_STRING_PRIVATE_PVP_INVITE_REQ, Send_NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_REQ, WaitTimeSenderInviteReq, NKCUtilString.GET_STRING_CANCEL, NKCUtilString.GET_STRING_PRIVATE_PVP_AUTO_CANCEL_ID, m_targetFriendListData, PrivateGameConfig);
		}
	}

	public static void ShowInviteErrorPopup()
	{
		if (m_inviteErrorCode != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupGauntletInvite.ClosePopupBox();
			NKCPacketHandlers.Check_NKM_ERROR_CODE(m_inviteErrorCode);
			m_inviteErrorCode = NKM_ERROR_CODE.NEC_OK;
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_INVITE_NOT sPacket)
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM))
		{
			return;
		}
		if (isInviteSender)
		{
			Send_NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ(bAccept: false, sPacket.senderProfile.commonProfile.userUid);
			return;
		}
		if (NKCUIShop.IsInstanceOpen)
		{
			Send_NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ(bAccept: false, sPacket.senderProfile.commonProfile.userUid);
			return;
		}
		switch (NKCScenManager.GetScenManager().GetNowScenID())
		{
		default:
			Send_NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ(bAccept: false, sPacket.senderProfile.commonProfile.userUid);
			break;
		case NKM_SCEN_ID.NSI_HOME:
		case NKM_SCEN_ID.NSI_COLLECTION:
		case NKM_SCEN_ID.NSI_FRIEND:
		case NKM_SCEN_ID.NSI_GAUNTLET_INTRO:
		case NKM_SCEN_ID.NSI_GAUNTLET_LOBBY:
		case NKM_SCEN_ID.NSI_GUILD_LOBBY:
			isInviteSender = false;
			m_targetProfile = sPacket.senderProfile;
			m_currentInviteUserUID = m_targetProfile.commonProfile.userUid;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				NKCScenManager.GetScenManager().Get_SCEN_HOME()?.UnhideLobbyUI();
			}
			NKCPopupGauntletInvite.OpenOKCancelTimerBox(NKCUtilString.GET_STRING_FRIEND_PVP, NKCUtilString.GET_STRING_PRIVATE_PVP_INVITE_NOT, OnClickAcceptInvite, OnClickRefuseInvite, sPacket.timeoutDurationSec, NKCUtilString.GET_STRING_PRIVATE_PVP_AUTO_CANCEL_ID, NKCUtilString.GET_STRING_ACCEPT, NKCUtilString.GET_STRING_REFUSE, m_targetProfile, sPacket.config);
			break;
		}
	}

	public static void Send_NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_REQ()
	{
		NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_REQ nKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_REQ();
		nKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_REQ.targetUserUid = m_currentInviteUserUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_ACK sPacket)
	{
		NKCPopupOKCancel.ClosePopupBox();
		NKMPopUpBox.CloseWaitBox();
		if (isInviteSender)
		{
			CancelInviteProcess();
		}
		else
		{
			CancelAllProcess();
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_NOT sPacket)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM))
		{
			if (m_currentInviteUserUID != 0L && sPacket.targetUserUid != m_currentInviteUserUID)
			{
				Log.Debug($"[PrivatePvp] TargetUser[{sPacket.targetUserUid}] CancelType[{sPacket.cancelType.ToString()}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPrivatePVPRoomMgr.cs", 673);
				return;
			}
			NKCPopupGauntletInvite.ClosePopupBox();
			NKCPopupOKCancel.ClosePopupBox();
			ShowCancelNoticePopup(sPacket.cancelType);
		}
	}

	public static void ShowCancelNoticePopup(PrivatePvpCancelType cancelType)
	{
		string text = "SI_DP_PRIVATE_PVP_CANCEL_NOT_";
		string text2 = "";
		switch (cancelType)
		{
		case PrivatePvpCancelType.IRejectInvitation:
			return;
		case PrivatePvpCancelType.OtherPlayerCancelGame:
			text2 = NKCUtilString.GET_STRING_PRIVATE_PVP_HOST_LEFT_ROOM;
			break;
		default:
		{
			int num = (int)cancelType;
			text2 = NKCStringTable.GetString(text + num);
			break;
		}
		}
		if (isInviteSender)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, text2, CancelInviteProcess);
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, text2, CancelAllProcess);
		}
	}

	public static void Send_NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ(bool bAccept, long targetUserUID)
	{
		NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ nKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ();
		nKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ.targetUserUid = targetUserUID;
		nKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ.accept = bAccept;
		if (!bAccept && targetUserUID != m_currentInviteUserUID)
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID, bAccept);
		}
		else
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL, bAccept);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_ACK sPacket)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM) && sPacket.cancelType != PrivatePvpCancelType.IRejectInvitation)
		{
			NKCConnectGame connectGame = NKCScenManager.GetScenManager().GetConnectGame();
			if (!string.IsNullOrEmpty(sPacket.serverIp))
			{
				connectGame.SetRemoteAddress(sPacket.serverIp, sPacket.port);
				connectGame.SetAccessToken(sPacket.accessToken);
				connectGame.ResetConnection();
				connectGame.ConnectToLobbyServer();
			}
			else
			{
				ChangeScene(NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM);
			}
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_NOT sPacket)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM))
		{
			NKCPopupGauntletInvite.ClosePopupBox();
			NKCUIGauntletPrivateRoom.GetInstance()?.UIGauntletPrivateRoomInvite?.Close();
			ResetInviteData();
			SetLobbyData(sPacket.lobbyState);
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM()?.GauntletPrivateRoom?.SetUI();
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_ACK sPacket)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM) && sPacket.cancelType != PrivatePvpCancelType.IRejectInvitation)
		{
			NKCConnectGame connectGame = NKCScenManager.GetScenManager().GetConnectGame();
			if (!string.IsNullOrEmpty(sPacket.serverIp))
			{
				connectGame.SetRemoteAddress(sPacket.serverIp, sPacket.port);
				connectGame.SetAccessToken(sPacket.accessToken);
				connectGame.ResetConnection();
				connectGame.ConnectToLobbyServer();
			}
			else
			{
				ChangeScene(NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM);
			}
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_SEARCH_USER_ACK sPacket)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
		{
			if (m_LobbyData != null && m_LobbyData.lobbyState == NKM_GAME_STATE.NGS_LOBBY_MATCHING)
			{
				m_searchResult = sPacket.list;
				if (m_searchResult.Count == 0)
				{
					NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_FRIEND_SEARCH_RESULT_EMPTY);
				}
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM()?.GauntletPrivateRoom?.UIGauntletPrivateRoomInvite?.SetUI();
			}
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			m_searchResult = sPacket.list;
			if (m_searchResult.Count == 0)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_FRIEND_SEARCH_RESULT_EMPTY);
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY()?.GauntletLobbyCustom?.SetUI();
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM()?.GauntletPrivateRoom?.UIGauntletPrivateRoomInvite?.SetUI();
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_STATE_ACK sPacket)
	{
		NKMPvpGameLobbyUserState myPvpGameLobbyUserState = GetMyPvpGameLobbyUserState();
		if (myPvpGameLobbyUserState != null && myPvpGameLobbyUserState.playerState != sPacket.playerState)
		{
			myPvpGameLobbyUserState.playerState = sPacket.playerState;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM()?.GauntletPrivateRoom?.RefreshUI();
	}

	public static void Send_NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ(long targetUserUid, PvpPlayerRole newRole)
	{
		NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ nKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ();
		nKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ.targetUserUid = targetUserUid;
		nKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ.changeRole = newRole;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_ACK sPacket)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM))
		{
			SetLobbyData(sPacket.lobbyState);
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM()?.GauntletPrivateRoom?.SetUI();
		}
	}

	public static void Send_NKMPacket_PRIVATE_PVP_EXIT_REQ()
	{
		NKMPacket_PRIVATE_PVP_LOBBY_EXIT_REQ packet = new NKMPacket_PRIVATE_PVP_LOBBY_EXIT_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecvExit()
	{
		NKCPopupGauntletInvite.ClosePopupBox();
		NKCPopupOKCancel.ClosePopupBox();
		CancelAllProcess();
	}

	public static void OnRecv(NKMPacket_EVENT_PVP_EXIT_ACK sNKMPacket_EVENT_PVP_EXIT_ACK)
	{
		NKCPopupGauntletInvite.ClosePopupBox();
		NKCPopupOKCancel.ClosePopupBox();
		CancelAllProcess();
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_NOT sPacket)
	{
		if (!IsPlayer(sPacket.targetUserUid))
		{
			Log.Debug($"[PrivatePvpRoom] TargetUser[{sPacket.targetUserUid}] CancelType[{sPacket.cancelType.ToString()}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPrivatePVPRoomMgr.cs", 912);
		}
		NKCPopupGauntletInvite.ClosePopupBox();
		NKCPopupOKCancel.ClosePopupBox();
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM)
		{
			ShowCancelNoticePopup(sPacket.cancelType);
		}
		else if (isInviteSender)
		{
			CancelInviteProcess();
		}
		else
		{
			CancelAllProcess();
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_STATE_NOT cNKMPacket_PRIVATE_PVP_LOBBY_STATE_NOT)
	{
		SetLobbyData(cNKMPacket_PRIVATE_PVP_LOBBY_STATE_NOT.lobbyData);
		NKM_GAME_STATE lobbyState = m_LobbyData.lobbyState;
		if (lobbyState != NKM_GAME_STATE.NGS_LOBBY_MATCHING)
		{
			_ = 7;
			return;
		}
		NKM_SCEN_ID nowScenID = NKCScenManager.GetScenManager().GetNowScenID();
		if (nowScenID != NKM_SCEN_ID.NSI_GAME && nowScenID != NKM_SCEN_ID.NSI_GAME_RESULT && nowScenID != NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM)
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
			{
				ChangeScene(NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM);
			}
			else if (NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM() != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM().GauntletPrivateRoom?.RefreshUI(initScrollPosition: false, initRoomState: false);
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PVP_CANCEL_NOT sPacket)
	{
		if (m_LobbyData != null)
		{
			if (!IsPlayer(sPacket.targetUserUid))
			{
				Log.Debug($"[PrivatePvpRoom] TargetUser[{sPacket.targetUserUid}] CancelType[{sPacket.cancelType.ToString()}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPrivatePVPRoomMgr.cs", 962);
				return;
			}
			NKCPopupGauntletInvite.ClosePopupBox();
			NKCPopupOKCancel.ClosePopupBox();
			ShowCancelNoticePopup(sPacket.cancelType);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_KICK_ACK sPacket)
	{
		RefreshLobbyData(sPacket.lobbyState);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_KICK_NOT sPacket)
	{
		NKCPopupGauntletInvite.ClosePopupBox();
		NKCPopupOKCancel.ClosePopupBox();
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_PRIVATE_PVP_KICKED_BY_HOST, CancelAllProcess);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_OPTION_ACK sPacket)
	{
		NKCPopupGauntletPrivateRoomOption.CheckInstanceAndClose();
		m_LobbyData = sPacket.lobbyState;
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CONFIG_NOT sPacket)
	{
		if (m_LobbyData != null)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_POPUP_NOTICE_FRIENDLY_MATCH_CHANGE_OPTION);
			m_LobbyData.config = sPacket.lobbyConfig;
			RefreshLobbyData(m_LobbyData);
		}
	}

	public static void SetApplyEquipSet(bool value)
	{
		PrivateGameConfig.applyEquipStat = value;
	}

	public static void SetApplyAllUnitMaxLevel(bool value)
	{
		PrivateGameConfig.applyAllUnitMaxLevel = value;
	}

	public static void SetApplyBanUp(bool value)
	{
		PrivateGameConfig.applyBanUpSystem = value;
	}

	public static void SetDraftBanMode(bool value)
	{
		PrivateGameConfig.draftBanMode = value;
	}

	public static void Send_NKMPacket_PRIVATE_PVP_LOBBY_START_GAME_SETTING_REQ()
	{
		NKMPacket_PRIVATE_PVP_LOBBY_START_GAME_SETTING_REQ packet = new NKMPacket_PRIVATE_PVP_LOBBY_START_GAME_SETTING_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_START_GAME_SETTING_ACK sPacket)
	{
		NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM);
	}

	public static void Send_NKMPacket_PRIVATE_PVP_SYNC_DECK_INDEX_REQ(NKMDeckIndex selectedDeckIndex)
	{
		GetMyPvpGameLobbyUserState().deckIndex = selectedDeckIndex;
		NKMPacket_PRIVATE_PVP_LOBBY_SYNC_DECK_INDEX_REQ nKMPacket_PRIVATE_PVP_LOBBY_SYNC_DECK_INDEX_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_SYNC_DECK_INDEX_REQ();
		nKMPacket_PRIVATE_PVP_LOBBY_SYNC_DECK_INDEX_REQ.deckIndex = selectedDeckIndex;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_SYNC_DECK_INDEX_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void Send_NKMPacket_PRIVATE_PVP_READY_REQ(NKMDeckIndex selectedDeckIndex, bool value)
	{
		NKMPacket_PRIVATE_PVP_LOBBY_READY_REQ nKMPacket_PRIVATE_PVP_LOBBY_READY_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_READY_REQ();
		nKMPacket_PRIVATE_PVP_LOBBY_READY_REQ.deckIndex = selectedDeckIndex;
		nKMPacket_PRIVATE_PVP_LOBBY_READY_REQ.isReady = value;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_READY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_READY_ACK packet)
	{
		if (m_LobbyData != null && NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM() != null)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM().GauntletPrivateRoom.RefreshUI();
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_ACCEPT_NOT sPacket)
	{
		if (m_LobbyData != null)
		{
			ResetData();
		}
	}

	public static void OnRecv(NKMPacket_PVP_GAME_MATCH_COMPLETE_NOT cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT)
	{
		ResetInviteData();
	}

	public static void ProcessReLogin(NKMGameData gameData, NKMLobbyData lobbyData)
	{
		if (lobbyData.lobbyState == NKM_GAME_STATE.NGS_LOBBY_MATCHING)
		{
			NKCCollectionManager.Init();
			SetLobbyData(lobbyData);
			NKM_GAME_STATE lobbyState = m_LobbyData.lobbyState;
			if (lobbyState != NKM_GAME_STATE.NGS_LOBBY_MATCHING)
			{
				_ = 7;
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
			{
				ChangeScene(NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM);
			}
			else if (NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM() != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM().GauntletPrivateRoom?.RefreshUI();
			}
		}
	}
}
