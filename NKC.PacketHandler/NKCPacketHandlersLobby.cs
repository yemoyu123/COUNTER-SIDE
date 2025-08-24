using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Account;
using ClientPacket.Chat;
using ClientPacket.Common;
using ClientPacket.Community;
using ClientPacket.Contract;
using ClientPacket.Defence;
using ClientPacket.Event;
using ClientPacket.Game;
using ClientPacket.Guild;
using ClientPacket.Item;
using ClientPacket.LeaderBoard;
using ClientPacket.Lobby;
using ClientPacket.Mode;
using ClientPacket.Negotiation;
using ClientPacket.Office;
using ClientPacket.Pvp;
using ClientPacket.Raid;
using ClientPacket.Service;
using ClientPacket.Shop;
using ClientPacket.Unit;
using ClientPacket.User;
using ClientPacket.Warfare;
using ClientPacket.WorldMap;
using Cs.GameServer.Replay;
using Cs.Logging;
using Cs.Protocol;
using NKC.Office;
using NKC.Patcher;
using NKC.Publisher;
using NKC.Trim;
using NKC.UI;
using NKC.UI.Collection;
using NKC.UI.Event;
using NKC.UI.Fierce;
using NKC.UI.Friend;
using NKC.UI.Gauntlet;
using NKC.UI.Guild;
using NKC.UI.Lobby;
using NKC.UI.Module;
using NKC.UI.NPC;
using NKC.UI.Office;
using NKC.UI.Option;
using NKC.UI.Result;
using NKC.UI.Shop;
using NKC.UI.Trim;
using NKC.Util;
using NKM;
using NKM.Contract2;
using NKM.Event;
using NKM.Guild;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;

namespace NKC.PacketHandler;

public static class NKCPacketHandlersLobby
{
	public static void OnRecv(NKMPacket_HEART_BIT_ACK res)
	{
		NKCScenManager.GetScenManager().GetConnectGame().ResetHeartbitTimeout();
		NKCScenManager.GetScenManager().GetConnectGame().SetPingTime(res.time);
		NKCPacketObjectPool.CloseObject(res);
	}

	public static void OnRecv(NKMPacket_CONNECT_CHECK_ACK res)
	{
		NKMPopUpBox.CloseWaitBox();
		NKCScenManager.GetScenManager().SetAppEnableConnectCheckTime(-1f);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME().OnReturnApp();
		}
	}

	public static void OnRecv(NKMPacket_JOIN_LOBBY_ACK res)
	{
		NKMPopUpBox.CloseWaitBox();
		NKCPopupOKCancel.OnButton onOK_Button = null;
		if (NKCPublisherModule.PublisherType == NKCPublisherModule.ePublisherType.NexonPC && (res.errorCode == NKM_ERROR_CODE.NEC_FAIL_NEXON_PC_FORCE_SHUTDOWN || res.errorCode == NKM_ERROR_CODE.NEC_FAIL_NEXON_PC_INVALID_AGE || res.errorCode == NKM_ERROR_CODE.NEC_FAIL_NEXON_PC_OPTIONAL_SHUTDOWN || res.errorCode == NKM_ERROR_CODE.NEC_FAIL_NEXON_PC_INVALID_AUTH_LEVEL || res.errorCode == NKM_ERROR_CODE.NEC_FAIL_UNDER_MAINTENANCE))
		{
			NKCScenManager.GetScenManager().GetConnectLogin().SetEnable(bSet: false);
			NKCScenManager.GetScenManager().GetConnectGame().SetEnable(bSet: false);
			onOK_Button = delegate
			{
				NKCMain.QuitGame();
			};
		}
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(res.errorCode, bCloseWaitBox: true, onOK_Button))
		{
			Debug.LogWarningFormat("Login failed. result:{0}", res.errorCode);
			if (res.errorCode == NKM_ERROR_CODE.kLoginFailure_LoggedInJustBefore)
			{
				NKCScenManager.GetScenManager().SetAppEnableConnectCheckTime(4f, bForce: true);
			}
			return;
		}
		NKCSynchronizedTime.OnRecv(res.utcTime, res.utcOffset);
		NKCStringTable.AddString(NKCStringTable.GetNationalCode(), "SI_SYSTEM_UTC_OFFSET", NKMTime.INTERVAL_FROM_UTC.ToString("+0;-#"), bOverwriteDuplicate: true);
		NKMTempletContainer<NKMIntervalTemplet>.Load(new List<NKMIntervalTemplet>(((IEnumerable<NKMIntervalData>)res.intervalData).Select((Func<NKMIntervalData, NKMIntervalTemplet>)((NKMIntervalData e) => e))), (NKMIntervalTemplet e) => e.StrKey);
		try
		{
			NKCTempletUtility.PostJoin();
			foreach (ContractTempletV2 value in ContractTempletV2.Values)
			{
				value.Validate();
			}
			foreach (MiscContractTemplet value2 in MiscContractTemplet.Values)
			{
				value2.ValidateMiscContract();
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex.Message + "\n" + ex.StackTrace, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketHandlersLobby.cs", 148);
			if (!NKCPatchDownloader.Instance.ProloguePlay)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_INTERVAL_JOIN_FAILED, Application.Quit);
			}
		}
		NKCScenManager.GetScenManager().SetAppEnableConnectCheckTime(-1f, bForce: true);
		NKCScenManager.GetScenManager().GetConnectGame().SetReconnectKey(res.reconnectKey);
		NKCScenManager.GetScenManager().GetConnectGame().LoginComplete();
		Debug.Log("Login succeed.");
		NKCScenManager.GetScenManager().SetMyUserData(res.userData);
		NKCScenManager.GetScenManager().SetWarfareGameData(res.warfareGameData);
		NKCScenManager.GetScenManager().GetMyUserData().m_AsyncData = res.asyncPvpState;
		NKCScenManager.GetScenManager().GetMyUserData().m_LeagueData = res.leaguePvpState;
		NKCScenManager.GetScenManager().GetMyUserData().LastPvpPointChargeTimeUTC = NKCSynchronizedTime.ToUtcTime(res.pvpPointChargeTime);
		NKCScenManager.GetScenManager().GetMyUserData().m_LeaguePvpHistory = new PvpHistoryList();
		NKCScenManager.GetScenManager().GetMyUserData().m_LeaguePvpHistory.Add(res.leaguePvpHistories);
		NKCScenManager.GetScenManager().GetMyUserData().m_PrivatePvpHistory = new PvpHistoryList();
		NKCScenManager.GetScenManager().GetMyUserData().m_PrivatePvpHistory.Add(res.privatePvpHistories);
		NKCScenManager.GetScenManager().GetMyUserData().m_RankOpen = res.rankPvpOpen;
		NKCScenManager.GetScenManager().GetMyUserData().m_LeagueOpen = res.leaguePvpOpen;
		NKCScenManager.GetScenManager().GetMyUserData().m_LastPlayInfo = res.lastPlayInfo;
		Log.Debug($"[LastPlayInfo][JoinLobbyAck] GameType[{(NKM_GAME_TYPE)res.lastPlayInfo.gameType}] StageId[{res.lastPlayInfo.stageId}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketHandlersLobby.cs", 180);
		NKCScenManager.GetScenManager().GetMyUserData().UpdateConsumerPackageData(res.consumerPackages);
		NKCScenManager.GetScenManager().GetMyUserData().m_NpcData = res.npcPvpData;
		NKCScenManager.GetScenManager().GetMyUserData().m_enableAccountLink = res.enableAccountLink;
		Log.Debug($"[SteamLink] JoinLobbyAck - enableAccountLink[{res.enableAccountLink}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketHandlersLobby.cs", 186);
		NKCScenManager.CurrentUserData().m_ShopData.SetTotalPayment(res.totalPaidAmount);
		NKCScenManager.CurrentUserData().m_ShopData.SetChainTabResetData(res.shopChainTabNestResetList);
		NKCScenManager.CurrentUserData().SetReturningUserStates(res.ReturningUserStates);
		NKCScenManager.CurrentUserData().SetShipCandidateData(res.shipSlotCandidate);
		NKCScenManager.CurrentUserData().OfficeData.SetData(res.officeState);
		NKCScenManager.CurrentUserData().kakaoMissionData = res.kakaoMissionData;
		NKCScenManager.CurrentUserData().TrimData.SetTrimIntervalData(res.trimIntervalData);
		NKCScenManager.CurrentUserData().TrimData.SetTrimClearList(res.trimClearList);
		NKCUIManager.NKCUIUpsideMenu.UpdateTimeContents();
		NKCScenManager.GetScenManager().GetNKCContractDataMgr().SetContractState(res.contractState);
		NKCScenManager.GetScenManager().GetNKCContractDataMgr().SetContractBonusState(res.contractBonusState);
		NKCScenManager.GetScenManager().GetNKCContractDataMgr().SetSelectableContractState(res.selectableContractState);
		NKCScenManager.CurrentUserData().SetMyUserProfileInfo(res.userProfileData);
		NKMEventManager.SetEventInfo(res.eventInfo);
		NKCScenManager.CurrentUserData().SetStagePlayData(res.stagePlayDataList);
		if (res.marketReviewCompletion)
		{
			NKCPublisherModule.Marketing.SetMarketReviewCompleted();
		}
		NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().SetDailyRewardReceived(res.fierceDailyRewardReceived);
		NKCPublisherModule.InAppPurchase.RequestBillingProductList(null);
		NKCBanManager.UpdatePVPBanData(res.pvpBanResult);
		NKCPVPManager.Init(NKCSynchronizedTime.GetServerUTCTime());
		NKCContentManager.AddUnlockableContents();
		NKCContentManager.AddUnlockableCounterCase();
		NKCScenManager.CurrentUserData().SetEquipTuningData(res.equipTuningCandidate);
		NKCScenManager.CurrentUserData().SetEquipPotentialData(res.potentialOptionCandidate);
		NKCGuildManager.SetMyData(res.privateGuildData);
		NKCGuildCoopManager.SetMyData(res.guildDungeonRewardInfo);
		NKCChatManager.SetMuteEndDate(res.blockMuteEndDate);
		NKMEpisodeMgr.SetUnlockedStage(res.unlockedStageIds);
		NKCScenManager.GetScenManager().GetGameOptionData()?.LoadAccountLocal(res.userData.m_UserOption);
		NKMMissionManager.SetDefaultTrackingMissionToGrowthMission();
		NKCScenManager.GetScenManager().Get_SCEN_LOGIN().OnLoginSuccess(res);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_FRIENDLY_MODE) && NKCPrivatePVPRoomMgr.IsInProgress() && res.gameData == null && res.lobbyData == null)
		{
			NKCPrivatePVPRoomMgr.CancelAllProcess();
		}
		NKCPhaseManager.SetPhaseModeState(res.phaseModeState);
		NKCPhaseManager.SetPhaseClearDataList(res.phaseClearDataList);
		NKCTrimManager.SetTrimModeState(res.trimModeState);
		NKCKillCountManager.SetServerKillCountData(res.serverKillCountDataList);
		NKCKillCountManager.SetKillCountData(res.killCountDataList);
		NKCUnitMissionManager.SetCompletedUnitMissionData(res.completedUnitMissions);
		NKCUnitMissionManager.SetRewardUnitMissionData(res.rewardEnableUnitMissions);
		NKCBanManager.UpdatePVPCastingVoteData(res.pvpCastingVoteData);
		NKCBanManager.UpdatePVPGlobalVoteData(res.pvpDraftVoteData);
		NKCScenManager.CurrentUserData().SetMyUserProfileInfo(res.supportUnitProfileData);
		NKCScenManager.CurrentUserData().EventCollectionInfo = res.eventCollectionInfo;
		NKCScenManager.CurrentUserData().m_eventPvpData = res.eventPvpState;
		NKCScenManager.GetScenManager().GetNKCContractDataMgr().UpdateCustomPickUpContract(res.customPickupContracts);
		if (!NKCPublisherModule.Auth.OnLoginSuccessToCS())
		{
			return;
		}
		if (!NKCPatchUtility.BackgroundPatchEnabled())
		{
			NKCPatchUtility.SaveTutorialClearedStatus();
		}
		if (!NKCTutorialManager.CheckTutoGameCondAtLogin(res.userData) || res.userData.CheckDungeonClear(1007))
		{
			NKCPatchUtility.SaveTutorialClearedStatus();
			if (NKCPatchDownloader.Instance != null && (NKCPatchDownloader.Instance.VersionCheckStatus != NKCPatchDownloader.VersionStatus.UpToDate || NKCPatchDownloader.Instance.ProloguePlay))
			{
				NKCScenManager.GetScenManager().ShowBundleUpdate(bCallFromTutorial: false);
				return;
			}
		}
		Debug.Log($"[NKMPacket_JOIN_LOBBY_ACK] Is gameData null : {res.gameData == null}");
		Debug.Log($"[NKMPacket_JOIN_LOBBY_ACK] Is leaguePvpRoomData null : {res.leaguePvpRoomData == null}");
		Debug.Log($"[NKMPacket_JOIN_LOBBY_ACK] Is lobbyData null : {res.lobbyData == null}");
		if (res.gameData == null)
		{
			if (NKCTutorialManager.CheckTutoGameCondAtLogin(res.userData))
			{
				if (!res.userData.CheckDungeonClear(1004))
				{
					NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(new NKMEventDeckData(), 11211, 0, 1004, 0, bLocal: false, 1, 0, 0L);
					return;
				}
				if (!res.userData.CheckDungeonClear(1005))
				{
					NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(new NKMEventDeckData(), 11212, 0, 1005, 0, bLocal: false, 1, 0, 0L);
					return;
				}
				if (!res.userData.CheckDungeonClear(1006))
				{
					NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(new NKMEventDeckData(), 11213, 0, 1006, 0, bLocal: false, 1, 0, 0L);
					return;
				}
				if (!res.userData.CheckDungeonClear(1007))
				{
					NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(new NKMEventDeckData(), 11214, 0, 1007, 0, bLocal: false, 1, 0, 0L);
					return;
				}
			}
			if (NKCPhaseManager.PlayNextPhase() || NKCTrimManager.ProcessTrim())
			{
				return;
			}
			switch (NKCScenManager.GetScenManager().GetNowScenID())
			{
			case NKM_SCEN_ID.NSI_LOGIN:
				if (res.lobbyData != null)
				{
					NKCPrivatePVPRoomMgr.ProcessReLogin(res.gameData, res.lobbyData);
				}
				else
				{
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
				}
				return;
			case NKM_SCEN_ID.NSI_GAME:
				if (NKCScenManager.GetScenManager().GetGameClient() != null && NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData() != null && (NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData()
					.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH || NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData()
					.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_END))
				{
					if (NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_BattleResultData() != null)
					{
						NKCScenManager.GetScenManager().Get_SCEN_GAME().EndGameWithReservedGameData();
					}
					else if (NKCScenManager.GetScenManager().GetGameClient().GetGameData() != null)
					{
						NKCScenManager.GetScenManager().Get_SCEN_GAME().DoAfterGiveUp();
					}
					else
					{
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
					}
				}
				else if (NKCScenManager.GetScenManager().GetGameClient() == null || NKCScenManager.GetScenManager().GetGameClient().GetGameData() == null || NKCScenManager.GetScenManager().GetGameClient().GetGameData()
					.GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE)
				{
					if (NKCScenManager.GetScenManager().GetGameClient() != null && NKCScenManager.GetScenManager().GetGameClient().GetGameData() != null)
					{
						NKCScenManager.GetScenManager().Get_SCEN_GAME().DoAfterGiveUp();
					}
					else
					{
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
					}
				}
				return;
			case NKM_SCEN_ID.NSI_WARFARE_GAME:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().ProcessReLogin();
				return;
			case NKM_SCEN_ID.NSI_BASE:
				if (NKCUIUnitInfo.IsInstanceOpen)
				{
					NKCUIUnitInfo.Instance.RefreshUIForReconnect();
				}
				return;
			case NKM_SCEN_ID.NSI_GAUNTLET_MATCH:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().ProcessReLogin();
				return;
			case NKM_SCEN_ID.NSI_OPERATION:
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().ProcessRelogin();
				return;
			case NKM_SCEN_ID.NSI_WORLDMAP:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().ProcessReLogin();
				return;
			case NKM_SCEN_ID.NSI_COLLECTION:
			case NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON:
			case NKM_SCEN_ID.NSI_GAME_RESULT:
				return;
			}
			if (res.leaguePvpRoomData != null)
			{
				if (res.lobbyData != null)
				{
					NKCPrivatePVPRoomMgr.SetLobbyData(res.lobbyData);
				}
				NKCLeaguePVPMgr.ProcessReLogin(res.gameData, res.leaguePvpRoomData);
			}
			else if (res.lobbyData != null)
			{
				NKCPrivatePVPRoomMgr.ProcessReLogin(res.gameData, res.lobbyData);
			}
			else
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKCScenManager.GetScenManager().GetNowScenID());
			}
			return;
		}
		Debug.Log($"[NKMPacket_JOIN_LOBBY_ACK] {NKCScenManager.GetScenManager().GetNowScenID()} ProcessReLogin");
		switch (NKCScenManager.GetScenManager().GetNowScenID())
		{
		case NKM_SCEN_ID.NSI_GAUNTLET_MATCH:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().ProcessReLogin(res.gameData);
			return;
		case NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON:
			return;
		}
		if (res.leaguePvpRoomData != null)
		{
			if (res.lobbyData != null)
			{
				NKCPrivatePVPRoomMgr.SetLobbyData(res.lobbyData);
			}
			NKCLeaguePVPMgr.ProcessReLogin(res.gameData, res.leaguePvpRoomData);
		}
		else
		{
			NormalProcessReLoginWhenGameExist(res.gameData);
		}
	}

	public static void NormalProcessReLoginWhenGameExist(NKMGameData _NKMGameData)
	{
		if (_NKMGameData == null)
		{
			return;
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.IsSameData(_NKMGameData))
		{
			if (NKCScenManager.GetScenManager().GetNowScenState() == NKC_SCEN_STATE.NSS_START)
			{
				NKCScenManager.GetScenManager().GetGameClient().SetIntrude(value: true);
				NKCScenManager.GetScenManager().GetGameClient().SetMyTeam();
				NKCScenManager.GetScenManager().Get_SCEN_GAME().OnGameScenStart();
			}
		}
		else
		{
			NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(_NKMGameData, bIntrude: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
		}
	}

	public static void OnRecv(NKMPacket_GAME_LAW_SHUTDOWN_NOT cNKMPacket_GAME_LAW_SHUTDOWN_NOT)
	{
		if (cNKMPacket_GAME_LAW_SHUTDOWN_NOT.remainSpan <= TimeSpan.Zero)
		{
			NKCScenManager.GetScenManager().GetConnectLogin().SetEnable(bSet: false);
			NKCScenManager.GetScenManager().GetConnectGame().SetEnable(bSet: false);
			NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
			NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
			NKCScenManager.GetScenManager().Get_SCEN_LOGIN().SetShutdownPopup();
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
		}
		else
		{
			NKCUIPopupMessageServer.Instance.Open(NKCUIPopupMessageServer.eMessageStyle.Slide, NKCUtilString.GET_STRING_SHUTDOWN_ALARM);
		}
	}

	public static void OnRecv(NKMPacket_DUPLICATED_CONNECTED_NOT cNKMPacket_DUPLICATED_CONNECTED_NOT)
	{
		NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
		NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
		NKCScenManager.GetScenManager().Get_SCEN_LOGIN().SetDuplicateConnectPopup();
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
	}

	public static void OnRecv(NKMPacket_WEEKLY_REFRESH_NOT cNKMPacket_WEEKLY_REFRESH_NOT)
	{
		NKCScenManager.CurrentUserData()?.m_InventoryData.UpdateItemInfo(cNKMPacket_WEEKLY_REFRESH_NOT.refreshItemDataList);
	}

	public static void OnRecv(NKMPacket_CHANGE_NICKNAME_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_UserNickName = sPacket.nickname;
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
			NKCPopupNickname.CheckInstanceAndClose();
			if (NKCUIUserInfo.IsInstanceOpen)
			{
				NKCUIUserInfo.Instance.RefreshNickname();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				NKCScenManager.GetScenManager().Get_SCEN_HOME().RefreshNickname();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().RefreshNickname();
			}
			if (NKCUIUserInfoV2.IsInstanceOpen)
			{
				NKCUIUserInfoV2.Instance.RefreshNickname();
			}
			NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.PlayerNameChanged);
		}
	}

	public static void OnRecv(NKMPacket_DECK_UNLOCK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_ArmyData.UnlockDeck(sPacket.deckType, sPacket.unlockedDeckSize);
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
			if (NKCUIDeckViewer.IsInstanceOpen)
			{
				NKCUIDeckViewer.Instance.OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_DECK_UNIT_SET_ACK cPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			return;
		}
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		if (cPacket.oldDeckIndex.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
		{
			armyData.SetDeckUnitByIndex(cPacket.oldDeckIndex, (byte)cPacket.oldSlotIndex, 0L);
			armyData.SetDeckLeader(cPacket.oldDeckIndex, cPacket.oldLeaderSlotIndex);
		}
		long num = 0L;
		NKMUnitData deckUnitByIndex = armyData.GetDeckUnitByIndex(cPacket.deckIndex, cPacket.slotIndex);
		if (deckUnitByIndex != null)
		{
			num = deckUnitByIndex.m_UnitUID;
		}
		armyData.SetDeckUnitByIndex(cPacket.deckIndex, cPacket.slotIndex, cPacket.slotUnitUID);
		NKMDeckData deckData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(cPacket.deckIndex);
		if (deckData != null)
		{
			deckData.m_LeaderIndex = cPacket.leaderSlotIndex;
		}
		if (NKCUIDeckViewer.IsInstanceOpen)
		{
			NKCUIDeckViewer.Instance.OnRecv(cPacket, deckUnitByIndex == null);
		}
		if (NKCUIUnitSelectList.IsInstanceOpen)
		{
			if (num != 0L)
			{
				NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(num, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE));
			}
			NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(cPacket.slotUnitUID, cPacket.deckIndex);
		}
	}

	public static void OnRecv(NKMPacket_DECK_SHIP_SET_ACK cPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			return;
		}
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		NKMDeckData deckData = armyData.GetDeckData(cPacket.deckIndex);
		long num = 0L;
		NKMUnitData deckShip = armyData.GetDeckShip(cPacket.deckIndex);
		if (deckShip != null)
		{
			num = deckShip.m_UnitUID;
		}
		if (cPacket.oldDeckIndex.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
		{
			NKMDeckData deckData2 = armyData.GetDeckData(cPacket.oldDeckIndex);
			if (deckData2 != null)
			{
				deckData2.m_ShipUID = 0L;
			}
		}
		if (deckData != null)
		{
			deckData.m_ShipUID = cPacket.shipUID;
		}
		if (NKCUIDeckViewer.IsInstanceOpen)
		{
			NKCUIDeckViewer.Instance.OnRecv(cPacket);
		}
		if (NKCUIShipInfo.IsInstanceOpen)
		{
			NKCUIShipInfo.Instance.OnRecv(cPacket);
		}
		if (NKCUIUnitSelectList.IsInstanceOpen)
		{
			if (num != 0L)
			{
				NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(num, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE));
			}
			NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(cPacket.shipUID, cPacket.deckIndex);
		}
	}

	public static void OnRecv(NKMPacket_DECK_UNIT_SWAP_ACK cPacket_DECK_UNIT_SWAP_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket_DECK_UNIT_SWAP_ACK.errorCode))
		{
			if (NKCUIDeckViewer.IsInstanceOpen)
			{
				NKCUIDeckViewer.Instance.SelectCurrentDeck();
			}
			return;
		}
		NKMDeckData deckData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(cPacket_DECK_UNIT_SWAP_ACK.deckIndex);
		if (deckData != null)
		{
			if (cPacket_DECK_UNIT_SWAP_ACK.leaderSlotIndex != -1)
			{
				deckData.m_LeaderIndex = cPacket_DECK_UNIT_SWAP_ACK.leaderSlotIndex;
			}
			deckData.m_listDeckUnitUID[cPacket_DECK_UNIT_SWAP_ACK.slotIndexFrom] = cPacket_DECK_UNIT_SWAP_ACK.slotUnitUIDFrom;
			deckData.m_listDeckUnitUID[cPacket_DECK_UNIT_SWAP_ACK.slotIndexTo] = cPacket_DECK_UNIT_SWAP_ACK.slotUnitUIDTo;
		}
		if (NKCUIDeckViewer.IsInstanceOpen)
		{
			NKCUIDeckViewer.Instance.OnRecv(cPacket_DECK_UNIT_SWAP_ACK);
		}
	}

	public static void OnRecv(NKMPacket_DECK_UNIT_SET_LEADER_ACK cPacket_DECK_UNIT_SET_LEADER_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket_DECK_UNIT_SET_LEADER_ACK.errorCode))
		{
			NKMDeckData deckData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(cPacket_DECK_UNIT_SET_LEADER_ACK.deckIndex);
			if (deckData != null)
			{
				deckData.m_LeaderIndex = cPacket_DECK_UNIT_SET_LEADER_ACK.leaderSlotIndex;
			}
			if (NKCUIDeckViewer.IsInstanceOpen)
			{
				NKCUIDeckViewer.Instance.OnRecv(cPacket_DECK_UNIT_SET_LEADER_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_DECK_UNIT_AUTO_SET_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMDeckData nKMDeckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(sPacket.deckIndex);
		if (nKMDeckData == null)
		{
			nKMDeckData = new NKMDeckData();
		}
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < nKMDeckData.m_listDeckUnitUID.Count; i++)
		{
			if (i < sPacket.deckData.m_listDeckUnitUID.Count && sPacket.deckData.m_listDeckUnitUID[i] != nKMDeckData.m_listDeckUnitUID[i])
			{
				hashSet.Add(i);
			}
		}
		nKMDeckData.DeepCopyFrom(sPacket.deckData);
		if (NKCUIDeckViewer.IsInstanceOpen)
		{
			NKCUIDeckViewer.Instance.OnRecv(sPacket, hashSet);
		}
	}

	public static void OnRecv(NKMPacket_DECK_NAME_UPDATE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCUIDeckViewer.IsInstanceOpen)
			{
				NKCUIDeckViewer.Instance.ResetDeckName();
			}
			return;
		}
		NKMDeckData deckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(sPacket.deckIndex);
		if (deckData != null)
		{
			deckData.m_DeckName = sPacket.name;
		}
		if (NKCUIDeckViewer.IsInstanceOpen)
		{
			NKCUIDeckViewer.Instance.UpdateDeckName(sPacket.deckIndex, sPacket.name);
		}
	}

	public static void OnRecv(NKMPacket_PVP_GAME_MATCH_ACK cNKMPacket_PVP_GAME_MATCH_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_PVP_GAME_MATCH_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME || NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKMPopUpBox.OpenWaitBox();
		}
	}

	public static void OnRecv(NKMPacket_PVP_GAME_MATCH_CANCEL_ACK cNKMPacket_PVP_GAME_MATCH_CANCEL_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_PVP_GAME_MATCH_CANCEL_ACK.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_MATCH)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().NKCUIGuantletMatch.OnRecv(cNKMPacket_PVP_GAME_MATCH_CANCEL_ACK);
		}
	}

	public static void OnRecv(NKMPacket_PVP_GAME_MATCH_COMPLETE_NOT cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT)
	{
		NKCSoundManager.SetMute(NKCScenManager.GetScenManager().GetGameOptionData().SoundMute, bIgnoreApplicationFocus: true);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM) || NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_FRIENDLY_MODE))
		{
			NKCPrivatePVPRoomMgr.OnRecv(cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT);
		}
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_LEAGUE_MODE))
		{
			NKCLeaguePVPMgr.OnRecv(cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			if (NKCScenManager.GetScenManager().GetGameClient().GetGameData()
				.IsSameData(cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT.gameData))
			{
				if (NKCScenManager.GetScenManager().GetNowScenState() == NKC_SCEN_STATE.NSS_START)
				{
					NKCScenManager.GetScenManager().Get_SCEN_GAME().OnGameScenStart();
				}
			}
			else
			{
				NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT.gameData);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
			}
			return;
		}
		NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT.gameData);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_MATCH)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().OnRecv(cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT);
		}
		else
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_ASYNC_READY)
			{
				return;
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY)
			{
				NKCScenManager.GetScenManager().Get_SCEN_GAME().ReserveGameEndData(null);
				if (cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT.gameData == null)
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR_SERVER_GAME_DATA, delegate
					{
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
					});
				}
				else
				{
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
				}
			}
			else
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
			}
		}
	}

	public static void OnRecv(NKMPacket_PVP_GAME_MATCH_FAIL_NOT cNKMPacket_PVP_GAME_MATCH_FAIL_NOT)
	{
		NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_MATCHING_FAIL_ALARM);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	public static void OnRecv(NKMPacket_GAME_SURRENDER_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetGameClient()?.GetGameHud().GetNKCGameHudPause().Close();
		}
	}

	public static void OnRecv(NKMPacket_GAME_SURRENDER_NOT sPacket)
	{
		NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_PF_PVP_SURRENDER_SELECT_NOTICE"));
	}

	public static void OnRecv(NKMPacket_CONTRACT_ACK sNKMPacket_CONTRACT_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sNKMPacket_CONTRACT_ACK.errorCode))
		{
			if ((sNKMPacket_CONTRACT_ACK.errorCode == NKM_ERROR_CODE.NEC_FAIL_CANNOT_USE_TICKET_WHEN_FREE_CHANCE_REMAINED || sNKMPacket_CONTRACT_ACK.errorCode == NKM_ERROR_CODE.NEC_FAIL_CANNOT_USE_MONEY_WHEN_FREE_CHANCE_REMAINED || sNKMPacket_CONTRACT_ACK.errorCode == NKM_ERROR_CODE.NEC_FAIL_CANNOT_USE_MONEY_WHEN_TICKET_REMAINED) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_CONTRACT)
			{
				NKCScenManager.GetScenManager().GET_SCEN_CONTRACT().OnUIForceRefresh();
			}
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData;
		if (inventoryData != null)
		{
			inventoryData.UpdateItemInfo(sNKMPacket_CONTRACT_ACK.costItems);
			if (sNKMPacket_CONTRACT_ACK.rewardData != null && sNKMPacket_CONTRACT_ACK.rewardData.MiscItemDataList != null)
			{
				inventoryData.AddItemMisc(sNKMPacket_CONTRACT_ACK.rewardData.MiscItemDataList);
			}
		}
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		for (int i = 0; i < sNKMPacket_CONTRACT_ACK.units.Count; i++)
		{
			NKMUnitData nKMUnitData = sNKMPacket_CONTRACT_ACK.units[i];
			if (armyData.IsFirstGetUnit(nKMUnitData.m_UnitID))
			{
				NKCUIGameResultGetUnit.AddFirstGetUnit(nKMUnitData.m_UnitID);
			}
			armyData.AddNewUnit(nKMUnitData);
		}
		foreach (NKMOperator @operator in sNKMPacket_CONTRACT_ACK.operators)
		{
			if (armyData.IsFirstGetUnit(@operator.id))
			{
				NKCUIGameResultGetUnit.AddFirstGetUnit(@operator.id);
			}
			armyData.AddNewOperator(@operator);
		}
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		if (nKCContractDataMgr != null)
		{
			nKCContractDataMgr.UpdateContractBonusState(sNKMPacket_CONTRACT_ACK.contractBonusState);
			nKCContractDataMgr.UpdateContractState(sNKMPacket_CONTRACT_ACK.contractState);
		}
		NKCScenManager.GetScenManager().GET_SCEN_CONTRACT().OnRecv(sNKMPacket_CONTRACT_ACK);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCUIModuleHome.UpdateAllModule();
		}
	}

	public static void OnRecv(NKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_ACK sNKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sNKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetNKCContractDataMgr().SetSelectableContractState(sNKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_ACK.selectableContractState);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_CONTRACT)
			{
				NKCScenManager.GetScenManager().GET_SCEN_CONTRACT().OnRecv(sNKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_SELECTABLE_CONTRACT_CONFIRM_ACK sNKMPacket_SELECTABLE_CONTRACT_CONFIRM_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sNKMPacket_SELECTABLE_CONTRACT_CONFIRM_ACK.errorCode))
		{
			return;
		}
		NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(sNKMPacket_SELECTABLE_CONTRACT_CONFIRM_ACK.costItems);
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		List<NKMUnitData> units = sNKMPacket_SELECTABLE_CONTRACT_CONFIRM_ACK.units;
		for (int i = 0; i < units.Count; i++)
		{
			if (armyData.IsFirstGetUnit(units[i].m_UnitID))
			{
				NKCUIGameResultGetUnit.AddFirstGetUnit(units[i].m_UnitID);
			}
			armyData.AddNewUnit(units[i]);
		}
		NKCScenManager.GetScenManager().GetNKCContractDataMgr().SetSelectableContractState(sNKMPacket_SELECTABLE_CONTRACT_CONFIRM_ACK.selectableContractState);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_CONTRACT)
		{
			NKCScenManager.GetScenManager().GET_SCEN_CONTRACT().OnRecv(sNKMPacket_SELECTABLE_CONTRACT_CONFIRM_ACK);
		}
	}

	public static void OnRecv(NKMPacket_CONTRACT_STATE_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (sPacket.contractState != null)
			{
				NKCScenManager.GetScenManager().GetNKCContractDataMgr().SetContractState(sPacket.contractState);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_CONTRACT)
			{
				NKCScenManager.GetScenManager().GET_SCEN_CONTRACT().OnUIForceRefresh();
			}
		}
	}

	public static void OnRecv(NKMPacket_INSTANT_CONTRACT_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetNKCContractDataMgr().UpdateInstantContract(sPacket.InstantContract);
			if (sPacket.InstantContract != null && sPacket.InstantContract.Count > 0 && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_CONTRACT)
			{
				NKCScenManager.GetScenManager().GET_SCEN_CONTRACT().OnUIForceRefresh(bForce: true);
			}
		}
	}

	public static void OnRecv(NKMPacket_CUSTOM_PICKUP_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData;
		if (inventoryData != null)
		{
			inventoryData.UpdateItemInfo(sPacket.costItems);
			if (sPacket.rewardData != null && sPacket.rewardData.MiscItemDataList != null)
			{
				inventoryData.AddItemMisc(sPacket.rewardData.MiscItemDataList);
			}
		}
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		for (int i = 0; i < sPacket.units.Count; i++)
		{
			NKMUnitData nKMUnitData = sPacket.units[i];
			if (armyData.IsFirstGetUnit(nKMUnitData.m_UnitID))
			{
				NKCUIGameResultGetUnit.AddFirstGetUnit(nKMUnitData.m_UnitID);
			}
			armyData.AddNewUnit(nKMUnitData);
		}
		foreach (NKMOperator @operator in sPacket.operators)
		{
			if (armyData.IsFirstGetUnit(@operator.id))
			{
				NKCUIGameResultGetUnit.AddFirstGetUnit(@operator.id);
			}
			armyData.AddNewOperator(@operator);
		}
		CustomPickupContractTemplet.Find(sPacket.customPickupId)?.UpdateData(sPacket.customPickupContractData);
		NKCScenManager.GetScenManager().GetNKCContractDataMgr()?.UpdateContractBonusState(sPacket.contractBonusState);
		NKCScenManager.GetScenManager().GET_SCEN_CONTRACT().OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_CUSTOM_PICUP_SELECT_TARGET_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetNKCContractDataMgr().UpdateCustomPickUpContract(sPacket.customPickupContractData);
			NKCScenManager.GetScenManager().GetNKCContractDataMgr()?.UpdateContractBonusState(sPacket.contractBonusState);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_CONTRACT)
			{
				NKCScenManager.GetScenManager().GET_SCEN_CONTRACT().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_BACKGROUND_CHANGE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData().backGroundInfo = sPacket.backgroundInfo;
			NKCUIChangeLobby.CheckInstanceAndClose();
			NKCUIUserInfo.CheckInstanceAndClose();
		}
	}

	public static void OnRecv(NKMPacket_JUKEBOX_CHANGE_BGM_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData().m_JukeboxData = sPacket.jukeboxData;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
			}
			else if (NKCUIModuleHome.IsAnyInstanceOpen())
			{
				NKCUIModuleHome.UpdateAllModule();
			}
		}
	}

	public static void OnRecv(NKMPacket_SERVER_TIME_ACK sPacket)
	{
		NKCSynchronizedTime.OnRecv(sPacket);
		NKCPacketObjectPool.CloseObject(sPacket);
	}

	public static void OnRecv(NKMPacket_ENHANCE_UNIT_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMArmyData armyData = myUserData.m_ArmyData;
		for (int i = 0; i < sPacket.consumeUnitUIDList.Count; i++)
		{
			armyData.RemoveUnit(sPacket.consumeUnitUIDList[i]);
		}
		myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
		NKMUnitData unitFromUID = armyData.GetUnitFromUID(sPacket.unitUID);
		if (unitFromUID != null)
		{
			for (int j = 0; j < sPacket.statExpList.Count; j++)
			{
				unitFromUID.m_listStatEXP[j] = sPacket.statExpList[j];
			}
		}
		armyData.UpdateUnitData(unitFromUID);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_BASE)
		{
			NKCScenManager.GetScenManager().Get_SCEN_BASE().OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_LOCK_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			NKMUnitData unitOrShipFromUID = armyData.GetUnitOrShipFromUID(sPacket.unitUID);
			if (unitOrShipFromUID != null)
			{
				unitOrShipFromUID.m_bLock = sPacket.isLock;
			}
			armyData.UpdateData(unitOrShipFromUID);
		}
	}

	public static void OnRecv(NKMPacket_FAVORITE_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			NKMUnitData unitOrShipFromUID = armyData.GetUnitOrShipFromUID(sPacket.unitUid);
			if (unitOrShipFromUID != null)
			{
				unitOrShipFromUID.isFavorite = sPacket.isFavorite;
			}
			armyData.UpdateData(unitOrShipFromUID);
		}
	}

	public static void OnRecv(NKMPacket_INVENTORY_EXPAND_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMInventoryManager.UpdateInventoryCount(sPacket.inventoryExpandType, sPacket.expandedCount, myUserData);
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			if (NKCUIUnitSelectList.IsInstanceOpen)
			{
				NKCUIUnitSelectList.Instance.UpdateUnitCount();
				NKCUIUnitSelectList.Instance.OnExpandInventory();
			}
			if (NKCUIInventory.IsInstanceOpen)
			{
				NKCUIInventory.Instance.SetCurrentEquipCountUI();
				NKCUIInventory.Instance.OnInventoryAdd();
			}
			if (NKCUIForge.IsInstanceOpen && NKCUIForge.Instance.IsInventoryInstanceOpen())
			{
				NKCUIForge.Instance.Inventory.SetCurrentEquipCountUI();
				NKCUIForge.Instance.Inventory.OnInventoryAdd();
			}
			if (NKCUIDeckViewer.IsInstanceOpen)
			{
				NKCUIDeckViewer.Instance.UpdateUnitCount();
			}
		}
	}

	public static void OnRecv(NKMPacket_REMOVE_UNIT_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMArmyData armyData = myUserData.m_ArmyData;
		myUserData.m_InventoryData.AddItemMisc(sPacket.rewardItemDataList);
		armyData.RemoveUnitList(sPacket.removeUnitUIDList);
		armyData.AddUnitDeleteRewardList(sPacket.rewardItemDataList);
		if (!armyData.IsEmptyUnitDeleteList)
		{
			NKCPacketSender.Send_NKMPacket_REMOVE_UNIT_REQ();
			return;
		}
		if (NKCUIUnitSelectList.IsInstanceOpen)
		{
			NKCUIUnitSelectList.Instance.CloseRemoveMode();
			NKCUIUnitSelectList.Instance.ClearMultipleSelect();
			NKCUINPCMachineGap.PlayVoice(NPC_TYPE.MACHINE_GAP, NPC_ACTION_TYPE.DISMISSAL_RESULT);
		}
		if (armyData.GetUnitDeleteReward().Count > 0)
		{
			NKCUIResult.Instance.OpenItemGain(armyData.GetUnitDeleteReward(), NKCUtilString.GET_STRING_ITEM_GAIN, NKCUtilString.GET_STRING_REMOVE_UNIT);
		}
	}

	public static void OnRecv(NKMPacket_LIMIT_BREAK_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMArmyData armyData = myUserData.m_ArmyData;
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			armyData.UpdateUnitData(sPacket.unitData);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_BASE)
			{
				NKCScenManager.GetScenManager().Get_SCEN_BASE().OnRecv(sPacket);
			}
			if (NKCUIUnitInfo.IsInstanceOpen)
			{
				NKCUIUnitInfo.Instance.OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_SKILL_UPGRADE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(sPacket.unitUID);
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			int unitSkillIndex = unitFromUID.GetUnitSkillIndex(sPacket.skillID);
			if (unitSkillIndex >= 0)
			{
				unitFromUID.m_aUnitSkillLevel[unitSkillIndex] = sPacket.skillLevel;
				myUserData.m_ArmyData.UpdateUnitData(unitFromUID);
			}
			else
			{
				Debug.LogError($"Can't Find Skill {sPacket.skillID} from Unit {unitFromUID.m_UnitID}(Uid {sPacket.unitUID})");
			}
			if (NKCUIUnitInfo.IsInstanceOpen)
			{
				NKCUIUnitInfo.Instance.OnRecv(sPacket);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_BASE)
			{
				NKCScenManager.GetScenManager().Get_SCEN_BASE().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_CONTENTS_DAILY_REFRESH_NOT cNKMPacket_CONTENTS_DAILY_REFRESH_NOT)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_CONTENTS_DAILY_REFRESH_NOT.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_CONTENTS_DAILY_REFRESH_NOT.refreshItemDataList);
				myUserData.m_InventoryData.RefreshDailyContens();
			}
		}
	}

	public static void OnRecv(NKMPacket_COUNTERCASE_UNLOCK_ACK cNKMPacket_COUNTERCASE_UNLOCK_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_COUNTERCASE_UNLOCK_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_COUNTERCASE_UNLOCK_ACK.costItemData);
				myUserData.AddCounterCaseData(cNKMPacket_COUNTERCASE_UNLOCK_ACK.dungeonID, unlocked: true);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OPERATION)
			{
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().OnRecv(cNKMPacket_COUNTERCASE_UNLOCK_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null && cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK.dungeonClearData != null && cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK.dungeonClearData.dungeonId != 0)
			{
				myUserData.SetDungeonClearData(cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK.dungeonClearData);
				myUserData.GetReward(cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK.dungeonClearData.rewardData);
				myUserData.UpdateEpisodeCompleteData(cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK.episodeCompleteData);
				NKCContentManager.SetUnlockedContent(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON, cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK.dungeonClearData.dungeonId);
			}
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().OnRecv(cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK);
		}
	}

	public static void OnRecv(NKMPacket_EPISODE_COMPLETE_REWARD_ACK cNKMPacket_EPISODE_COMPLETE_REWARD_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EPISODE_COMPLETE_REWARD_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				myUserData.UpdateEpisodeCompleteData(cNKMPacket_EPISODE_COMPLETE_REWARD_ACK.episodeCompleteData);
				myUserData.GetReward(cNKMPacket_EPISODE_COMPLETE_REWARD_ACK.rewardData);
			}
			if (myUserData != null)
			{
				NKCUIResult.Instance.OpenComplexResult(myUserData.m_ArmyData, cNKMPacket_EPISODE_COMPLETE_REWARD_ACK.rewardData, null, 0L);
			}
			if (NKCUIOperationNodeViewer.isOpen())
			{
				NKCUIOperationNodeViewer.Instance.Refresh();
			}
		}
	}

	public static void OnRecv(NKMPacket_EPISODE_COMPLETE_REWARD_ALL_ACK cNKMPacket_EPISODE_COMPLETE_REWARD_ALL_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EPISODE_COMPLETE_REWARD_ALL_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			foreach (NKMEpisodeCompleteData episodeCompleteDatum in cNKMPacket_EPISODE_COMPLETE_REWARD_ALL_ACK.episodeCompleteData)
			{
				myUserData.UpdateEpisodeCompleteData(episodeCompleteDatum);
			}
			myUserData.GetReward(cNKMPacket_EPISODE_COMPLETE_REWARD_ALL_ACK.rewardDate);
		}
		if (myUserData != null)
		{
			NKCUIResult.Instance.OpenComplexResult(myUserData.m_ArmyData, cNKMPacket_EPISODE_COMPLETE_REWARD_ALL_ACK.rewardDate, null, 0L);
		}
		if (NKCUIOperationNodeViewer.isOpen())
		{
			NKCUIOperationNodeViewer.Instance.Refresh();
		}
	}

	public static void OnRecv(NKMPacket_REFRESH_COMPANY_BUFF_ACK cNKMPacket_REFRESH_COMPANY_BUFF_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_REFRESH_COMPANY_BUFF_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				myUserData.m_companyBuffDataList = cNKMPacket_REFRESH_COMPANY_BUFF_ACK.companyBuffDataList;
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				NKCScenManager.GetScenManager().Get_SCEN_HOME().RefreshBuff();
			}
		}
	}

	public static void OnRecv(NKMPacket_SHOP_SUBSCRIPTION_NOT sPacket)
	{
		if (NKCScenManager.CurrentUserData()?.m_ShopData?.subscriptions == null)
		{
			Debug.LogWarning("[NKMPacket_SHOP_SUBSCRIPTION_NOT] Invalid Current User ShopData");
		}
		else if (NKCScenManager.CurrentUserData().m_ShopData.subscriptions.ContainsKey(sPacket.productId))
		{
			NKCScenManager.CurrentUserData().m_ShopData.subscriptions[sPacket.productId].lastUpdateDate = sPacket.lastUpdateDate;
		}
	}

	public static void OnRecv(NKMPacket_GAME_OPTION_PLAY_CUTSCENE_ACK cNKMPacket_GAME_OPTION_PLAY_CUTSCENE_ACK)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GAME_OPTION_PLAY_CUTSCENE_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bPlayCutscene = cNKMPacket_GAME_OPTION_PLAY_CUTSCENE_ACK.isPlayCutscene;
			if (NKCPopupDungeonInfo.IsInstanceOpen)
			{
				NKCPopupDungeonInfo.Instance.OnRecv(cNKMPacket_GAME_OPTION_PLAY_CUTSCENE_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_PVP_RANK_WEEK_REWARD_ACK cNKMPacket_PVP_RANK_WEEK_REWARD_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_PVP_RANK_WEEK_REWARD_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		if (cNKMPacket_PVP_RANK_WEEK_REWARD_ACK.pvpData != null)
		{
			myUserData.m_PvpData.WeekID = cNKMPacket_PVP_RANK_WEEK_REWARD_ACK.pvpData.WeekID;
		}
		if (cNKMPacket_PVP_RANK_WEEK_REWARD_ACK.rewardData == null)
		{
			return;
		}
		myUserData.GetReward(cNKMPacket_PVP_RANK_WEEK_REWARD_ACK.rewardData);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			if (cNKMPacket_PVP_RANK_WEEK_REWARD_ACK.isScoreChanged)
			{
				NKCPopupGauntletOutgameReward.SetNKMPVPData(cNKMPacket_PVP_RANK_WEEK_REWARD_ACK.pvpData);
				myUserData.m_PvpData = cNKMPacket_PVP_RANK_WEEK_REWARD_ACK.pvpData;
			}
			else
			{
				NKCPopupGauntletOutgameReward.SetNKMPVPData(myUserData.m_PvpData);
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(cNKMPacket_PVP_RANK_WEEK_REWARD_ACK);
		}
	}

	public static void OnRecv(NKMPacket_PVP_RANK_SEASON_REWARD_ACK cNKMPacket_PVP_RANK_SEASON_REWARD_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			if (cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.isScoreChanged)
			{
				NKCPopupGauntletNewSeasonAlarm.SetPrevNKMPVPData(cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.reducedPvpData);
				NKCPopupGauntletOutgameReward.SetNKMPVPData(cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.reducedPvpData);
				myUserData.m_PvpData = cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.pvpData;
			}
			else
			{
				NKCPopupGauntletNewSeasonAlarm.SetPrevNKMPVPData(myUserData.m_PvpData);
				NKCPopupGauntletOutgameReward.SetNKMPVPData(myUserData.m_PvpData);
				myUserData.m_PvpData = cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.pvpData;
			}
			NKCPopupGauntletOutgameReward.SetPrevSeasonPVPData(cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.reducedPvpData);
			if (cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.rewardData != null)
			{
				myUserData.GetReward(cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.rewardData);
			}
			if (cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.rankRewardData != null)
			{
				myUserData.GetReward(cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.rankRewardData);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(cNKMPacket_PVP_RANK_SEASON_REWARD_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_ASYNC_PVP_RANK_WEEK_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		myUserData.m_AsyncData.WeekID = sPacket.weekID;
		if (sPacket.rewardData != null)
		{
			myUserData.GetReward(sPacket.rewardData);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCPopupGauntletOutgameReward.SetNKMPVPData(myUserData.m_AsyncData);
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().NKCPopupGauntletOutgameReward.Open(bWeeklyReward: true, sPacket.rewardData, bRank: false);
			}
		}
	}

	public static void OnRecv(NKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKCPopupGauntletNewSeasonAlarm.SetPrevNKMPVPData(myUserData.m_AsyncData);
			NKCPopupGauntletOutgameReward.SetNKMPVPData(myUserData.m_AsyncData);
			myUserData.m_AsyncData = sPacket.pvpState;
			myUserData.m_NpcData = sPacket.npcPvpData;
			if (sPacket.rewardData != null)
			{
				myUserData.GetReward(sPacket.rewardData);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_PVP_BAN_LIST_UPDATED_NOT not)
	{
		if (NKCScenManager.CurrentUserData() == null || NKCScenManager.GetScenManager() == null)
		{
			return;
		}
		NKCBanManager.UpdatePVPBanData(not.pvpBanResult);
		NKM_SCEN_ID nowScenID = NKCScenManager.GetScenManager().GetNowScenID();
		bool flag = false;
		switch (nowScenID)
		{
		case NKM_SCEN_ID.NSI_GAUNTLET_LOBBY:
			if (NKCPopupGauntletBanList.IsInstanceOpen)
			{
				flag = true;
				NKCPopupGauntletBanList.Instance.OnChangedBanList();
			}
			break;
		case NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY:
			if (NKCUIDeckViewer.IsInstanceOpen && NKCUIDeckViewer.IsPVPSyncMode(NKCUIDeckViewer.Instance.GetDeckViewerMode()))
			{
				flag = true;
				NKCScenManager.GetScenManager().ScenChangeFade(nowScenID);
			}
			break;
		}
		if (flag)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_TOAST_PVP_BANLIST_RENEWED"));
		}
	}

	public static void OnRecv(NKMPacket_ASYNC_PVP_TARGET_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_ASYNC_PVP_RANK_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_REVENGE_PVP_TARGET_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_NPC_PVP_TARGET_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_ASYNC_PVP_START_GAME_ACK sPacket)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_ASYNC_READY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_ASYNC_READY().OnRecv(sPacket);
		}
		else
		{
			NKMPopUpBox.CloseWaitBox();
		}
	}

	public static void OnRecv(NKMPacket_UPDATE_DEFENCE_DECK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP_DEFENCE, 0))?.DeepCopyFrom(sPacket.deckData);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_INVITE_ACK sPacket)
	{
		NKCPrivatePVPRoomMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_INVITE_NOT sPacket)
	{
		NKCPrivatePVPRoomMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_NOT sPacket)
	{
		if (sPacket.cancelType == PrivatePvpCancelType.OtherPlayerCancelGame)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_MATCH_OTHER_PLAYER_CANCEL);
		}
		NKCPrivatePVPRoomMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_READY_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_PRIVATE_PVP_NOT_IN_GAME_ROOM)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
			}
		}
		else
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_STATE_NOT sPacket)
	{
		if (sPacket != null)
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_EXIT_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode, bCloseWaitBox: true, delegate
		{
			NKCPrivatePVPRoomMgr.OnRecvExit();
		}))
		{
			switch (sPacket.errorCode)
			{
			case NKM_ERROR_CODE.NEC_FAIL_PRIVATE_PVP_STATE_NOT_JOINED:
			case NKM_ERROR_CODE.NEC_FAIL_PRIVATE_PVP_GAME_ALREADY_JOINED:
				return;
			case NKM_ERROR_CODE.NEC_FAIL_PRIVATE_PVP_NOT_IN_GAME_ROOM:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
				return;
			}
		}
		NKCPrivatePVPRoomMgr.OnRecvExit();
		if (!NKCLeaguePVPMgr.OnRecvExit() && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PVP_EXIT_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		Debug.Log($"<color=red>NKMPacket_EVENT_PVP_EXIT_ACK : {NKCScenManager.GetScenManager().GetNowScenID()}</color>");
		if (!NKCLeaguePVPMgr.OnRecv(sPacket))
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PVP_CANCEL_NOT sPacket)
	{
		if (!NKCLeaguePVPMgr.OnRecv(sPacket))
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_UPDATE_PVP_INVITATION_OPTION_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			if (gameOptionData.ePrivatePvpInviteOption != sPacket.value)
			{
				gameOptionData.ePrivatePvpInviteOption = sPacket.value;
			}
			gameOptionData.Save();
			NKCUIGameOption.CheckInstanceAndClose();
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_SYNC_DECK_INDEX_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_SEARCH_USER_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CREATE_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_NOT sPacket)
	{
		NKCPrivatePVPRoomMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if ((sPacket.errorCode != NKM_ERROR_CODE.NEC_FAIL_PRIVATE_PVP_INVALID_TARGET_USER_UID || !NKCPrivatePVPRoomMgr.CancelNotPopupOpened) && NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_NOT sPacket)
	{
		NKCPrivatePVPRoomMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_CASTING_VOTE_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCBanManager.UpdatePVPGlobalVoteData(sPacket.pvpCastingVoteData);
			if (NKCPopupGauntletBan.IsInstanceOpen)
			{
				NKCPopupGauntletBan.Instance.UpdateUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_CASTING_VOTE_SHIP_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCBanManager.UpdatePVPGlobalVoteData(sPacket.pvpCastingVoteData);
			if (NKCPopupGauntletBan.IsInstanceOpen)
			{
				NKCPopupGauntletBan.Instance.UpdateUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_PVP_PICK_RATE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		if (sPacket.gameType == NKM_GAME_TYPE.NGT_PVP_RANK)
		{
			NKCRankPVPMgr.SetPickRateData(sPacket.pickRates);
			NKCPopupGauntletUnitUsage.Instance.Open();
		}
		else if (sPacket.gameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE || sPacket.gameType == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			if (sPacket.pickRates == null || sPacket.pickRates.Count == 0)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_POPUP_LEAGUE_NO_MAG);
				return;
			}
			NKCLeaguePVPMgr.SetPickRateData(sPacket.pickRates);
			NKCUIPopupDraftMagazine.Instance.Open();
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (sPacket.errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_PVP_ROOM_OBSERVE_CODE_NOT_EXISTS || sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_PVP_ROOM_OBSERVE_CODE_INVALID)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_PRIVATE_PVP_OBSERVE_CODE_NOT_EXIST, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				return;
			}
			if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_PVP_ROOM_INVALID_GAME_STATE || sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_PVP_ROOM_MAX_COUNT_EXCEED)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_PRIVATE_PVP_OBSERVE_CODE_CANNOT_ENTER, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				return;
			}
			if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_REQUEST_COOLDOWN_TIME)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString(sPacket.errorCode), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				return;
			}
			if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
			{
				return;
			}
		}
		NKCPrivatePVPRoomMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_KICK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_KICK_NOT sPacket)
	{
		NKCPrivatePVPRoomMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_OPTION_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_CONFIG_NOT sPacket)
	{
		NKCPrivatePVPRoomMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_LOBBY_START_GAME_SETTING_ACK sPacket)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_DRAFT_GIVEUP_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && !NKCLeaguePVPMgr.OnRecvExit())
		{
			NKCPrivatePVPRoomMgr.OnRecvExit();
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
			}
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_DRAFT_GIVEUP_NOT sPacket)
	{
		NKCLeaguePVPMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_STATE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPrivatePVPRoomMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_MATCH_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME || NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKMPopUpBox.OpenWaitBox();
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_MATCH_CANCEL_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_MATCH)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_MATCH_FAIL_NOT sPacket)
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_ACCEPT_NOT sPacket)
	{
		NKCLeaguePVPMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_UPDATED_NOT sPacket)
	{
		NKCLeaguePVPMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_GLOBAL_BAN_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaguePVPMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_GIVEUP_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaguePVPMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_PICK_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaguePVPMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_OPPONENT_BAN_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaguePVPMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_PICK_SHIP_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaguePVPMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_PICK_OPERATOR_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaguePVPMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_PICK_LEADER_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaguePVPMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_SELECT_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaguePVPMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_WEEKLY_RANKER_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME().OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_WEEKLY_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_SEASON_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKCPVPManager.m_bLeagueSeasonRewardReceived = true;
			myUserData.m_LeagueData = sPacket.pvpData;
			if (sPacket.rewardData != null)
			{
				myUserData.GetReward(sPacket.rewardData);
				myUserData.GetReward(sPacket.rankRewardData);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
			}
			else if (NKCUIModuleHome.IsAnyInstanceOpen())
			{
				NKCUIModuleHome.SendMessage(new NKCUIModuleSubUIDraft.EventModuleMessageDataDraft
				{
					m_bOpenRewardPopup = true
				});
			}
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_SEASON_INFO_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().m_LeagueData = sPacket.leaguePvpState;
			NKCPVPManager.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PVP_GAME_MATCH_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode, bCloseWaitBox: true, delegate
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
		}) && (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME || NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME))
		{
			NKMPopUpBox.OpenWaitBox();
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PVP_GAME_MATCH_CANCEL_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_MATCH)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().NKCUIGuantletMatch.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PVP_GAME_MATCH_FAIL_NOT sPacket)
	{
		NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_MATCHING_FAIL_ALARM);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	public static void OnRecv(NKMPacket_EVENT_PVP_SEASON_INFO_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.m_eventPvpData = sPacket.eventPvpState;
			}
			NKCEventPvpMgr.EventPvpRewardInfo = sPacket.eventPvpRewardInfos;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecvEventPvpSeasonInfo();
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PVP_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			for (int i = 0; i < sPacket.rewardDatas.Count; i++)
			{
				nKMUserData.GetReward(sPacket.rewardDatas[i]);
			}
		}
		NKCEventPvpMgr.EventPvpRewardInfo = sPacket.eventPvpRewardInfos;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecvEventPvpReward();
		}
		NKMRewardData nKMRewardData = new NKMRewardData();
		for (int j = 0; j < sPacket.rewardDatas.Count; j++)
		{
			nKMRewardData.AddRewardDataForRepeatOperation(sPacket.rewardDatas[j]);
		}
		NKCUIResult.Instance.OpenRewardGain(nKMUserData.m_ArmyData, nKMRewardData, null, NKCUtilString.GET_STRING_RESULT_MISSION, "", null);
	}

	public static void OnRecv(NKMPacket_PHASE_START_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPhaseManager.SetPhaseModeState(sPacket.state);
			NKCPhaseManager.PlayNextPhase();
		}
	}

	public static void OnRecv(NKMPacket_GAME_LOAD_ACK cNKMPacket_GAME_LOAD_ACK)
	{
		NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GAME_LOAD_ACK.errorCode, bCloseWaitBox: true, OnLoadAckFail))
		{
			return;
		}
		NKCScenManager.GetScenManager().Get_SCEN_GAME().ReserveGameEndData(null);
		if (cNKMPacket_GAME_LOAD_ACK.gameData == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR_SERVER_GAME_DATA, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
		}
		if (nKCRepeatOperaion != null && nKCRepeatOperaion.GetIsOnGoing() && cNKMPacket_GAME_LOAD_ACK.costItemDataList != null && cNKMPacket_GAME_LOAD_ACK.costItemDataList.Count > 0)
		{
			nKCRepeatOperaion.SetCostIncreaseCount(nKCRepeatOperaion.GetCostIncreaseCount() + 1);
		}
		switch (NKCScenManager.GetScenManager().GetNowScenID())
		{
		case NKM_SCEN_ID.NSI_DUNGEON_ATK_READY:
		case NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON:
		{
			int multiply = 1;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_DUNGEON_ATK_READY)
			{
				multiply = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastMultiplyRewardCount();
			}
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(cNKMPacket_GAME_LOAD_ACK.gameData.m_DungeonID);
			if (dungeonTempletBase != null && dungeonTempletBase.StageTemplet != null)
			{
				NKCScenManager.CurrentUserData().SetLastPlayInfo(NKM_GAME_TYPE.NGT_DUNGEON, dungeonTempletBase.StageTemplet.Key);
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().OnRecv(cNKMPacket_GAME_LOAD_ACK, multiply);
			break;
		}
		case NKM_SCEN_ID.NSI_LOGIN:
		case NKM_SCEN_ID.NSI_HOME:
		case NKM_SCEN_ID.NSI_WARFARE_GAME:
		case NKM_SCEN_ID.NSI_WORLDMAP:
		case NKM_SCEN_ID.NSI_GAME_RESULT:
		case NKM_SCEN_ID.NSI_DIVE:
		case NKM_SCEN_ID.NSI_RAID_READY:
		case NKM_SCEN_ID.NSI_TRIM:
		{
			int multiply2 = 1;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				multiply2 = NKCScenManager.GetScenManager().WarfareGameData.rewardMultiply;
			}
			NKCScenManager.GetScenManager().Get_SCEN_HOME().OnRecv(cNKMPacket_GAME_LOAD_ACK, multiply2);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_TRIM)
			{
				NKCScenManager.CurrentUserData().SetLastPlayInfo(NKM_GAME_TYPE.NGT_TRIM, 0);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OpenWaitBox();
			}
			break;
		}
		case NKM_SCEN_ID.NSI_GAME:
			if (cNKMPacket_GAME_LOAD_ACK.gameData == null)
			{
				Debug.LogError("Dungeon Loaded from Game Scene!");
			}
			else if (cNKMPacket_GAME_LOAD_ACK.gameData.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_TUTORIAL && !NKMDungeonManager.IsRestartAllowed(cNKMPacket_GAME_LOAD_ACK.gameData.m_NKM_GAME_TYPE))
			{
				Debug.LogError($"Dungeon Loaded from Game Scene. GameType : {cNKMPacket_GAME_LOAD_ACK.gameData.m_NKM_GAME_TYPE}");
			}
			NKCScenManager.GetScenManager().Get_SCEN_HOME().OnRecv(cNKMPacket_GAME_LOAD_ACK);
			break;
		}
		void OnLoadAckFail()
		{
			NKCRepeatOperaion nKCRepeatOperaion2 = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
			if (nKCRepeatOperaion2 != null && nKCRepeatOperaion2.GetIsOnGoing())
			{
				NKCPopupOKCancel.ClosePopupBox();
				nKCRepeatOperaion2.Init();
				nKCRepeatOperaion2.SetStopReason(NKCStringTable.GetString(cNKMPacket_GAME_LOAD_ACK.errorCode.ToString()));
				if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME_RESULT)
				{
					NKCPopupRepeatOperation.Instance.OpenForResult(delegate
					{
						NKMDungeonTempletBase dungeonTempletBase2 = NKMDungeonManager.GetDungeonTempletBase(cNKMPacket_GAME_LOAD_ACK.gameData.m_DungeonID);
						if (dungeonTempletBase2 != null && dungeonTempletBase2.StageTemplet != null)
						{
							if (!NKCScenManager.GetScenManager().Get_SCEN_OPERATION().PlayByFavorite)
							{
								NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedEpisodeTemplet(dungeonTempletBase2.StageTemplet.EpisodeTemplet);
							}
							NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
						}
					});
				}
				else
				{
					NKCPopupRepeatOperation.Instance.OpenForResult();
				}
			}
			nKCRepeatOperaion2?.Init();
			switch (NKCScenManager.GetScenManager().GetNowScenID())
			{
			case NKM_SCEN_ID.NSI_LOGIN:
			case NKM_SCEN_ID.NSI_GAME:
			case NKM_SCEN_ID.NSI_DUNGEON_ATK_READY:
			case NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON:
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
				NKCPopupMessageManager.AddPopupMessage(cNKMPacket_GAME_LOAD_ACK.errorCode);
				break;
			case NKM_SCEN_ID.NSI_WARFARE_GAME:
			{
				bool activeRepeatOperationOnOff = false;
				if (nKCRepeatOperaion2 != null)
				{
					activeRepeatOperationOnOff = nKCRepeatOperaion2.GetIsOnGoing();
				}
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
					.m_NKCWarfareGameHUD.SetActiveRepeatOperationOnOff(activeRepeatOperationOnOff);
				break;
			}
			}
		}
	}

	public static void OnRecv(NKMPacket_GAME_RESTART_ACK sPacket)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode);
	}

	public static void OnRecv(NKMPacket_GAME_GIVEUP_ACK cNKMPacket_GAME_GIVEUP_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GAME_GIVEUP_ACK.errorCode) && NKCTrimManager.TrimModeState != null)
		{
			NKCTrimManager.SetGiveUpState(value: true);
		}
	}

	public static void UpdateLeagueGiveupUserData(NKMPVPResultDataForClient _NKMPVPResultData)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (_NKMPVPResultData.myInfo != null)
		{
			PvpState.SetPrevScore(myUserData.m_LeagueData.Score);
			myUserData.m_LeagueData = _NKMPVPResultData.myInfo;
			myUserData.LastPvpPointChargeTimeUTC = NKCSynchronizedTime.ToUtcTime(_NKMPVPResultData.pvpPointChargeTime);
		}
		myUserData.m_InventoryData.AddItemMisc(_NKMPVPResultData.pvpPoint);
		myUserData.m_InventoryData.UpdateItemInfo(_NKMPVPResultData.pvpChargePoint);
		if (_NKMPVPResultData.history != null)
		{
			myUserData.m_LeaguePvpHistory.Add(_NKMPVPResultData.history);
		}
		NKCUIGauntletLobbyLeague.SetAlertDemotion(NKCUtil.IsPVPDemotionAlert(NKM_GAME_TYPE.NGT_PVP_LEAGUE, myUserData.m_PvpData));
		NKCPublisherModule.Push.UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP.PVP_POINT_COMPLETE, bForce: true);
	}

	public static void UpdateUserData(bool bWin, NKMDungeonClearData _NKMDungeonClearData, NKMEpisodeCompleteData _NKMEpisodeCompleteData = null, WarfareSyncData _NKMWarfareGameSyncDataPack = null, NKMPVPResultDataForClient _NKMPVPResultData = null, NKMDiveSyncData _NKMDiveSyncData = null, NKMRaidBossResultData _NKMRaidBossResultData = null, List<UnitLoyaltyUpdateData> lstUnitUpdateData = null, NKMShadowGameResult _NKMShadowGameResult = null, NKMFierceResultData _NKMFierceResultData = null, NKMPhaseClearData _NKMPhaseClearData = null)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (_NKMDungeonClearData != null)
		{
			if (bWin)
			{
				if (!myUserData.CheckDungeonClear(_NKMDungeonClearData.dungeonId))
				{
					if (myUserData.m_dicNKMDungeonClearData.Count == 0)
					{
						NKCPublisherModule.Statistics.OnFirstEpisodeClear();
					}
					NKCUtil.m_sHsFirstClearDungeon.Add(_NKMDungeonClearData.dungeonId);
				}
				myUserData.SetDungeonClearDataOnlyTrue(_NKMDungeonClearData);
			}
			myUserData.GetReward(_NKMDungeonClearData.rewardData);
			myUserData.GetReward(_NKMDungeonClearData.missionReward);
			myUserData.GetReward(_NKMDungeonClearData.oneTimeRewards);
			NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
			if (nKCRepeatOperaion != null && nKCRepeatOperaion.GetIsOnGoing())
			{
				if (_NKMDungeonClearData != null)
				{
					nKCRepeatOperaion.AddReward(_NKMDungeonClearData.rewardData);
					nKCRepeatOperaion.AddReward(_NKMDungeonClearData.missionReward);
					nKCRepeatOperaion.AddReward(_NKMDungeonClearData.oneTimeRewards);
				}
				if (_NKMPhaseClearData != null)
				{
					nKCRepeatOperaion.AddReward(_NKMPhaseClearData.rewardData);
					nKCRepeatOperaion.AddReward(_NKMPhaseClearData.missionReward);
					nKCRepeatOperaion.AddReward(_NKMPhaseClearData.oneTimeRewards);
				}
			}
		}
		NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
		if (gameData != null)
		{
			switch (gameData.m_NKM_GAME_TYPE)
			{
			case NKM_GAME_TYPE.NGT_DIVE:
				if (_NKMDungeonClearData == null)
				{
				}
				break;
			case NKM_GAME_TYPE.NGT_RAID:
			case NKM_GAME_TYPE.NGT_RAID_SOLO:
			{
				if (_NKMRaidBossResultData == null)
				{
					break;
				}
				NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(gameData.m_RaidUID);
				if (nKMRaidDetailData != null)
				{
					nKMRaidDetailData.curHP = _NKMRaidBossResultData.curHP;
					NKMRaidJoinData nKMRaidJoinData = nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID);
					if (nKMRaidJoinData != null)
					{
						nKMRaidJoinData.tryCount++;
					}
				}
				break;
			}
			case NKM_GAME_TYPE.NGT_PHASE:
			{
				NKCRepeatOperaion nKCRepeatOperaion3 = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
				if (!bWin)
				{
					if (nKCRepeatOperaion3.GetIsOnGoing())
					{
						NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetCurrRepeatCount(nKCRepeatOperaion3.GetCurrRepeatCount() + 1);
						NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetAlarmRepeatOperationQuitByDefeat(bSet: true);
					}
					nKCRepeatOperaion3.Init();
					if (NKCScenManager.GetScenManager().GetGameClient() != null && NKCScenManager.GetScenManager().GetGameClient().GetGameHud() != null)
					{
						NKCScenManager.GetScenManager().GetGameClient().GetGameHud()
							.GetNKCGameHUDRepeatOperation()
							.ResetBtnOnOffUI();
					}
				}
				if (_NKMPhaseClearData != null && bWin && nKCRepeatOperaion3.GetIsOnGoing())
				{
					nKCRepeatOperaion3.SetCurrRepeatCount(nKCRepeatOperaion3.GetCurrRepeatCount() + 1);
					if (nKCRepeatOperaion3.GetCurrRepeatCount() >= nKCRepeatOperaion3.GetMaxRepeatCount())
					{
						nKCRepeatOperaion3.Init();
						nKCRepeatOperaion3.SetStopReason(NKCUtilString.GET_STRING_REPEAT_OPERATION_IS_TERMINATED);
						nKCRepeatOperaion3.SetAlarmRepeatOperationSuccess(bSet: true);
					}
				}
				break;
			}
			case NKM_GAME_TYPE.NGT_DUNGEON:
			{
				NKCRepeatOperaion nKCRepeatOperaion2 = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
				if (_NKMDungeonClearData == null || !bWin)
				{
					if (nKCRepeatOperaion2.GetIsOnGoing())
					{
						NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetCurrRepeatCount(nKCRepeatOperaion2.GetCurrRepeatCount() + 1);
						NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetAlarmRepeatOperationQuitByDefeat(bSet: true);
					}
					nKCRepeatOperaion2.Init();
					if (NKCScenManager.GetScenManager().GetGameClient() != null && NKCScenManager.GetScenManager().GetGameClient().GetGameHud() != null)
					{
						NKCScenManager.GetScenManager().GetGameClient().GetGameHud()
							.GetNKCGameHUDRepeatOperation()
							.ResetBtnOnOffUI();
					}
				}
				else if (nKCRepeatOperaion2.GetIsOnGoing())
				{
					nKCRepeatOperaion2.SetCurrRepeatCount(nKCRepeatOperaion2.GetCurrRepeatCount() + 1);
					if (nKCRepeatOperaion2.GetCurrRepeatCount() >= nKCRepeatOperaion2.GetMaxRepeatCount())
					{
						nKCRepeatOperaion2.Init();
						nKCRepeatOperaion2.SetStopReason(NKCUtilString.GET_STRING_REPEAT_OPERATION_IS_TERMINATED);
						nKCRepeatOperaion2.SetAlarmRepeatOperationSuccess(bSet: true);
					}
				}
				break;
			}
			case NKM_GAME_TYPE.NGT_SHADOW_PALACE:
			{
				if (_NKMShadowGameResult == null)
				{
					break;
				}
				NKMShadowPalace shadowPalace = myUserData.m_ShadowPalace;
				shadowPalace.life = _NKMShadowGameResult.life;
				NKMPalaceData nKMPalaceData = shadowPalace.palaceDataList.Find((NKMPalaceData v) => v.palaceId == _NKMShadowGameResult.palaceId);
				if (nKMPalaceData == null)
				{
					nKMPalaceData = new NKMPalaceData();
					nKMPalaceData.palaceId = _NKMShadowGameResult.palaceId;
					shadowPalace.palaceDataList.Add(nKMPalaceData);
				}
				nKMPalaceData.currentDungeonId = _NKMShadowGameResult.currentDungeonId;
				if (_NKMShadowGameResult.dungeonData != null)
				{
					NKMPalaceDungeonData nKMPalaceDungeonData = nKMPalaceData.dungeonDataList.Find((NKMPalaceDungeonData v) => v.dungeonId == _NKMShadowGameResult.dungeonData.dungeonId);
					if (nKMPalaceDungeonData == null)
					{
						nKMPalaceDungeonData = new NKMPalaceDungeonData();
						nKMPalaceDungeonData.dungeonId = _NKMShadowGameResult.dungeonData.dungeonId;
						nKMPalaceData.dungeonDataList.Add(nKMPalaceDungeonData);
					}
					nKMPalaceDungeonData.recentTime = _NKMShadowGameResult.dungeonData.recentTime;
				}
				if (_NKMShadowGameResult.rewardData == null && _NKMShadowGameResult.life != 0)
				{
					break;
				}
				shadowPalace.currentPalaceId = 0;
				if (_NKMShadowGameResult.rewardData != null)
				{
					NKMShadowPalaceManager.SaveLastClearedPalace(_NKMShadowGameResult.palaceId);
					myUserData.GetReward(_NKMShadowGameResult.rewardData);
				}
				List<int> list = new List<int>();
				for (int num = 0; num < nKMPalaceData.dungeonDataList.Count; num++)
				{
					NKMPalaceDungeonData nKMPalaceDungeonData2 = nKMPalaceData.dungeonDataList[num];
					list.Add(nKMPalaceDungeonData2.bestTime);
					if (_NKMShadowGameResult.newRecord)
					{
						nKMPalaceDungeonData2.bestTime = nKMPalaceDungeonData2.recentTime;
					}
				}
				NKCScenManager.GetScenManager().Get_NKC_SCEN_SHADOW_RESULT().SetData(_NKMShadowGameResult, list);
				break;
			}
			}
		}
		NKMDiveGameData diveGameData = myUserData.m_DiveGameData;
		if (diveGameData != null && _NKMDiveSyncData != null)
		{
			diveGameData.UpdateData(_NKMDungeonClearData != null && bWin, _NKMDiveSyncData);
			if (NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_BattleResultData() == null || NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_BattleResultData()
				.m_BATTLE_RESULT_TYPE != BATTLE_RESULT_TYPE.BRT_WIN)
			{
				NKCDiveGame.SetReservedUnitDieShow(bSet: true, diveGameData.Player.PlayerBase.ReservedDeckIndex);
			}
			if (_NKMDiveSyncData.AddedSlotSets.Count > 0)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().SetSectorAddEvent();
			}
			if (diveGameData.Player.PlayerBase.State == NKMDivePlayerState.Clear || diveGameData.Player.PlayerBase.State == NKMDivePlayerState.Annihilation)
			{
				if (diveGameData.Player.PlayerBase.State == NKMDivePlayerState.Clear)
				{
					if (myUserData.m_DiveClearData == null)
					{
						myUserData.m_DiveClearData = new HashSet<int>();
					}
					if (!diveGameData.Floor.Templet.IsEventDive && !myUserData.m_DiveClearData.Contains(diveGameData.Floor.Templet.StageID))
					{
						myUserData.m_DiveClearData.Add(diveGameData.Floor.Templet.StageID);
					}
					if (_NKMDiveSyncData.RewardData != null)
					{
						myUserData.GetReward(_NKMDiveSyncData.RewardData);
					}
					if (_NKMDiveSyncData.ArtifactRewardData != null)
					{
						myUserData.GetReward(_NKMDiveSyncData.ArtifactRewardData);
					}
					if (_NKMDiveSyncData.StormMiscReward != null)
					{
						NKMRewardData nKMRewardData = new NKMRewardData();
						nKMRewardData.SetMiscItemData(new List<NKMItemMiscData> { _NKMDiveSyncData.StormMiscReward });
						myUserData.GetReward(nKMRewardData);
					}
					if (!myUserData.CheckDiveHistory(diveGameData.Floor.Templet.StageID))
					{
						myUserData.m_LastDiveHistoryData = new HashSet<int>(myUserData.m_DiveHistoryData);
						myUserData.m_DiveHistoryData.Add(diveGameData.Floor.Templet.StageID);
					}
					else if (!myUserData.m_LastDiveHistoryData.Contains(diveGameData.Floor.Templet.StageID))
					{
						myUserData.m_LastDiveHistoryData.Add(diveGameData.Floor.Templet.StageID);
					}
				}
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE_RESULT()?.SetData(diveGameData.Player.PlayerBase.State == NKMDivePlayerState.Clear, diveGameData.Floor.Templet.IsEventDive, _NKMDiveSyncData.RewardData, _NKMDiveSyncData.ArtifactRewardData, _NKMDiveSyncData.StormMiscReward, diveGameData.Player.PlayerBase.Artifacts, new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, diveGameData.Player.PlayerBase.LeaderDeckIndex), diveGameData.Floor.Templet);
				ProcessWorldmapContentsAfterDiveEnd();
				myUserData.ClearDiveGameData();
			}
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData != null && _NKMWarfareGameSyncDataPack != null)
		{
			if (NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME() != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().SetBattleInfo(warfareGameData, _NKMWarfareGameSyncDataPack);
			}
			warfareGameData.UpdateData(_NKMWarfareGameSyncDataPack);
			Debug.Log("Warfare Game State : " + warfareGameData.warfareGameState);
			if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_RESULT && warfareGameData.isWinTeamA && !NKCScenManager.GetScenManager().GetMyUserData().CheckWarfareClear(warfareGameData.warfareTempletID))
			{
				NKCUtil.m_sHsFirstClearWarfare.Add(warfareGameData.warfareTempletID);
			}
		}
		myUserData.UpdateEpisodeCompleteData(_NKMEpisodeCompleteData);
		if (_NKMPhaseClearData != null)
		{
			NKCPhaseManager.UpdateClearData(_NKMPhaseClearData);
			myUserData.GetReward(_NKMPhaseClearData.rewardData);
			myUserData.GetReward(_NKMPhaseClearData.missionReward);
			myUserData.GetReward(_NKMPhaseClearData.oneTimeRewards);
		}
		if (_NKMPVPResultData != null && gameData != null)
		{
			if (gameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || gameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
			{
				if (_NKMPVPResultData.myInfo != null)
				{
					PvpState.SetPrevScore(myUserData.m_LeagueData.Score);
					myUserData.m_LeagueData = _NKMPVPResultData.myInfo;
					myUserData.LastPvpPointChargeTimeUTC = NKCSynchronizedTime.ToUtcTime(_NKMPVPResultData.pvpPointChargeTime);
				}
				myUserData.m_InventoryData.AddItemMisc(_NKMPVPResultData.pvpPoint);
				myUserData.m_InventoryData.UpdateItemInfo(_NKMPVPResultData.pvpChargePoint);
				if (_NKMPVPResultData.history != null)
				{
					myUserData.m_LeaguePvpHistory.Add(_NKMPVPResultData.history);
				}
				NKCUIGauntletLobbyLeague.SetAlertDemotion(NKCUtil.IsPVPDemotionAlert(gameData.m_NKM_GAME_TYPE, myUserData.m_LeagueData));
			}
			else if (gameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_PRIVATE)
			{
				if (_NKMPVPResultData.history != null)
				{
					myUserData.m_PrivatePvpHistory.Add(_NKMPVPResultData.history);
				}
			}
			else if (gameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_RANK)
			{
				if (_NKMPVPResultData.myInfo != null)
				{
					PvpState.SetPrevScore(myUserData.m_PvpData.Score);
					myUserData.m_PvpData = _NKMPVPResultData.myInfo;
					myUserData.LastPvpPointChargeTimeUTC = NKCSynchronizedTime.ToUtcTime(_NKMPVPResultData.pvpPointChargeTime);
					myUserData.m_LeagueOpen = _NKMPVPResultData.leaguePvpOpen;
				}
				myUserData.m_InventoryData.AddItemMisc(_NKMPVPResultData.pvpPoint);
				myUserData.m_InventoryData.UpdateItemInfo(_NKMPVPResultData.pvpChargePoint);
				if (_NKMPVPResultData.history != null)
				{
					myUserData.m_SyncPvpHistory.Add(_NKMPVPResultData.history);
				}
				NKCUIGauntletLobbyRank.SetAlertDemotion(NKCUtil.IsPVPDemotionAlert(gameData.m_NKM_GAME_TYPE, myUserData.m_PvpData));
			}
			else if (gameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT)
			{
				if (_NKMPVPResultData.myInfo != null)
				{
					myUserData.m_eventPvpData = _NKMPVPResultData.myInfo;
					myUserData.LastPvpPointChargeTimeUTC = NKCSynchronizedTime.ToUtcTime(_NKMPVPResultData.pvpPointChargeTime);
				}
				myUserData.m_InventoryData.AddItemMisc(_NKMPVPResultData.pvpPoint);
				myUserData.m_InventoryData.UpdateItemInfo(_NKMPVPResultData.pvpChargePoint);
				if (_NKMPVPResultData.history != null)
				{
					myUserData.m_EventPvpHistory.Add(_NKMPVPResultData.history);
				}
			}
			NKCPublisherModule.Push.UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP.PVP_POINT_COMPLETE, bForce: true);
		}
		if (lstUnitUpdateData == null)
		{
			return;
		}
		foreach (UnitLoyaltyUpdateData lstUnitUpdateDatum in lstUnitUpdateData)
		{
			NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(lstUnitUpdateDatum.unitUid);
			if (unitFromUID != null)
			{
				unitFromUID.loyalty = lstUnitUpdateDatum.loyalty;
				unitFromUID.SetOfficeRoomId(lstUnitUpdateDatum.officeRoomId, lstUnitUpdateDatum.officeGrade, lstUnitUpdateDatum.heartGaugeStartTime);
			}
		}
	}

	public static void UpdateUserData(NKMGameEndData gameEndData)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (gameEndData.dungeonClearData != null)
		{
			if (gameEndData.win)
			{
				if (!nKMUserData.CheckDungeonClear(gameEndData.dungeonClearData.dungeonId))
				{
					if (nKMUserData.m_dicNKMDungeonClearData.Count == 0)
					{
						NKCPublisherModule.Statistics.OnFirstEpisodeClear();
					}
					NKCUtil.m_sHsFirstClearDungeon.Add(gameEndData.dungeonClearData.dungeonId);
				}
				nKMUserData.SetDungeonClearDataOnlyTrue(gameEndData.dungeonClearData);
			}
			nKMUserData.GetReward(gameEndData.dungeonClearData.rewardData);
			nKMUserData.GetReward(gameEndData.dungeonClearData.missionReward);
			nKMUserData.GetReward(gameEndData.dungeonClearData.oneTimeRewards);
			NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
			if (nKCRepeatOperaion != null && nKCRepeatOperaion.GetIsOnGoing() && gameEndData.dungeonClearData != null)
			{
				nKCRepeatOperaion.AddReward(gameEndData.dungeonClearData.rewardData);
				nKCRepeatOperaion.AddReward(gameEndData.dungeonClearData.missionReward);
				nKCRepeatOperaion.AddReward(gameEndData.dungeonClearData.oneTimeRewards);
			}
		}
		if (gameEndData.updatedUnits == null)
		{
			return;
		}
		foreach (UnitLoyaltyUpdateData updatedUnit in gameEndData.updatedUnits)
		{
			NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(updatedUnit.unitUid);
			if (unitFromUID != null)
			{
				unitFromUID.loyalty = updatedUnit.loyalty;
				unitFromUID.SetOfficeRoomId(updatedUnit.officeRoomId, updatedUnit.officeGrade, updatedUnit.heartGaugeStartTime);
			}
		}
	}

	public static void OnRecv(NKMPacket_GAME_END_NOT cPacket_GAME_END_NOT)
	{
		if (NKCReplayMgr.IsRecording())
		{
			NKCScenManager.GetScenManager().GetNKCReplayMgr().FillReplayData(cPacket_GAME_END_NOT);
		}
		if (NKCLeaguePVPMgr.OnRecv(cPacket_GAME_END_NOT))
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME_RESULT);
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
		int num = 0;
		int num2 = 0;
		NKM_GAME_TYPE gameType = NKM_GAME_TYPE.NGT_INVALID;
		if (gameData != null)
		{
			num2 = ((cPacket_GAME_END_NOT.dungeonClearData != null) ? cPacket_GAME_END_NOT.dungeonClearData.dungeonId : gameData.m_DungeonID);
			gameType = gameData.m_NKM_GAME_TYPE;
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(num2);
			switch (gameData.m_NKM_GAME_TYPE)
			{
			case NKM_GAME_TYPE.NGT_PHASE:
				num = NKCPhaseManager.PhaseModeState.stageId;
				break;
			case NKM_GAME_TYPE.NGT_WARFARE:
			{
				NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(gameData.m_WarfareID);
				num = ((nKMWarfareTemplet != null && nKMWarfareTemplet.StageTemplet != null) ? nKMWarfareTemplet.StageTemplet.Key : 0);
				break;
			}
			default:
				num = ((dungeonTempletBase != null && dungeonTempletBase.StageTemplet != null) ? dungeonTempletBase.StageTemplet.Key : 0);
				break;
			}
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && !cPacket_GAME_END_NOT.giveup)
		{
			Debug.Log("NKMPacket_GAME_END_NOT making resultUIData");
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (gameData != null)
			{
				NKCUIResult.BattleResultData battleResultData;
				if (gameData.IsPVE())
				{
					NKMDungeonTempletBase dungeonTempletBase2 = NKMDungeonManager.GetDungeonTempletBase(num2);
					if (num2 == 1007)
					{
						Debug.Log("Tutorial final stage cleared!");
						NKCPatchUtility.SaveTutorialClearedStatus();
					}
					NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.DungeonGameClear, num2);
					battleResultData = NKCUIResult.MakePvEBattleResultData(gameData.m_NKM_GAME_TYPE, gameClient, cPacket_GAME_END_NOT, num2, num);
					if (gameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_FIERCE)
					{
						NKCScenManager.CurrentUserData().SetLastPlayInfo(NKM_GAME_TYPE.NGT_FIERCE, 0);
					}
					else if (gameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_TRIM)
					{
						NKCScenManager.CurrentUserData().SetLastPlayInfo(NKM_GAME_TYPE.NGT_TRIM, 0);
					}
					else if (gameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_SHADOW_PALACE)
					{
						NKCScenManager.CurrentUserData().SetLastPlayInfo(NKM_GAME_TYPE.NGT_SHADOW_PALACE, 0);
					}
					else if (num > 0)
					{
						NKCScenManager.CurrentUserData().SetLastPlayInfo(NKM_GAME_TYPE.NGT_DUNGEON, num);
					}
					if (dungeonTempletBase2 != null)
					{
						string key = $"{myUserData.m_UserUID}_{dungeonTempletBase2.m_DungeonStrID}";
						if (battleResultData != null && battleResultData.IsWin && PlayerPrefs.HasKey(key))
						{
							PlayerPrefs.DeleteKey(key);
						}
					}
					myUserData.UpdateStagePlayData(cPacket_GAME_END_NOT.stagePlayData);
				}
				else if (gameData.IsPVP())
				{
					BATTLE_RESULT_TYPE bATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;
					if (cPacket_GAME_END_NOT.pvpResultData != null)
					{
						bATTLE_RESULT_TYPE = ((cPacket_GAME_END_NOT.pvpResultData.result != PVP_RESULT.WIN) ? ((cPacket_GAME_END_NOT.pvpResultData.result == PVP_RESULT.LOSE) ? BATTLE_RESULT_TYPE.BRT_LOSE : BATTLE_RESULT_TYPE.BRT_DRAW) : BATTLE_RESULT_TYPE.BRT_WIN);
					}
					NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.PvPGameFinished, (int)bATTLE_RESULT_TYPE);
					NKMItemMiscData cNKMItemMiscData = null;
					if (cPacket_GAME_END_NOT.pvpResultData != null)
					{
						cNKMItemMiscData = cPacket_GAME_END_NOT.pvpResultData.pvpPoint;
					}
					battleResultData = NKCUIResult.MakePvPResultData(bATTLE_RESULT_TYPE, cNKMItemMiscData, NKCUIBattleStatistics.MakeBattleData(gameClient, cPacket_GAME_END_NOT), gameData.GetGameType());
				}
				else
				{
					Debug.LogError("Undefined GameType");
					battleResultData = NKCUIResult.MakeMissionResultData(myUserData.m_ArmyData, num2, 0, cPacket_GAME_END_NOT.win, cPacket_GAME_END_NOT.dungeonClearData, cPacket_GAME_END_NOT.deckIndex, NKCUIBattleStatistics.MakeBattleData(gameClient, cPacket_GAME_END_NOT), cPacket_GAME_END_NOT.updatedUnits);
				}
				NKCScenManager.GetScenManager().Get_SCEN_GAME().ReserveGameEndData(battleResultData);
			}
			else
			{
				Debug.LogError("FATAL : GAMEDATA NULL");
			}
		}
		NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
		if (nKCRepeatOperaion != null && nKCRepeatOperaion.GetIsOnGoing() && cPacket_GAME_END_NOT.costItemDataList != null && cPacket_GAME_END_NOT.costItemDataList.Count > 0)
		{
			nKCRepeatOperaion.SetCostIncreaseCount(nKCRepeatOperaion.GetCostIncreaseCount() + 1);
		}
		UpdateUserData(cPacket_GAME_END_NOT.win, cPacket_GAME_END_NOT.dungeonClearData, cPacket_GAME_END_NOT.episodeCompleteData, cPacket_GAME_END_NOT.warfareSyncData, cPacket_GAME_END_NOT.pvpResultData, cPacket_GAME_END_NOT.diveSyncData, cPacket_GAME_END_NOT.raidBossResultData, cPacket_GAME_END_NOT.updatedUnits, cPacket_GAME_END_NOT.shadowGameResult, cPacket_GAME_END_NOT.fierceResultData, cPacket_GAME_END_NOT.phaseClearData);
		myUserData.m_InventoryData.UpdateItemInfo(cPacket_GAME_END_NOT.costItemDataList);
		NKCPhaseManager.SetPhaseModeState(cPacket_GAME_END_NOT.phaseModeState);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_TRIM()?.SetReservedTrim(NKCTrimManager.TrimModeState);
		if (NKCTrimManager.GiveUpState)
		{
			NKCTrimManager.ClearTrimModeState();
		}
		else
		{
			NKCTrimManager.SetTrimModeState(cPacket_GAME_END_NOT.trimModeState);
		}
		if (!NKCPhaseManager.IsPhaseOnGoing())
		{
			NKCKillCountManager.CurrentStageKillCount = 0L;
			NKCPhaseManager.ClearTempUnitData();
		}
		else
		{
			NKCPhaseManager.SaveTempUnitData(NKCScenManager.GetScenManager().GetGameClient(), cPacket_GAME_END_NOT.gameRecord);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && cPacket_GAME_END_NOT.giveup)
		{
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetAlarmRepeatOperationQuitByDefeat(bSet: false);
			if (cPacket_GAME_END_NOT.restart)
			{
				NKCScenManager.GetScenManager().Get_SCEN_GAME().DoAfterRestart(gameType, num, num2, cPacket_GAME_END_NOT.deckIndex);
			}
			else
			{
				NKCScenManager.GetScenManager().Get_SCEN_GAME().DoAfterGiveUp();
			}
		}
		NKCKillCountManager.UpdateKillCountData(cPacket_GAME_END_NOT.killCountData);
	}

	public static void OnRecv(NKMPacket_ASYNC_PVP_GAME_END_NOT sPacket)
	{
		if (NKCReplayMgr.IsRecording())
		{
			NKCScenManager.GetScenManager().GetNKCReplayMgr().FillReplayData(sPacket);
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
			if ((gameData != null && gameData.GetGameType() == NKM_GAME_TYPE.NGT_ASYNC_PVP) || gameData.GetGameType() == NKM_GAME_TYPE.NGT_PVP_STRATEGY || gameData.GetGameType() == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC || gameData.GetGameType() == NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE)
			{
				Debug.Log("[AsyncPvpGameEndNot] CorrectGameType");
				BATTLE_RESULT_TYPE battleResultType = BATTLE_RESULT_TYPE.BRT_LOSE;
				Debug.Log($"[AsyncPvpGameEndNot] Result : {sPacket.result}");
				switch (sPacket.result)
				{
				case PVP_RESULT.WIN:
					battleResultType = BATTLE_RESULT_TYPE.BRT_WIN;
					break;
				case PVP_RESULT.LOSE:
					battleResultType = BATTLE_RESULT_TYPE.BRT_LOSE;
					break;
				case PVP_RESULT.DRAW:
					battleResultType = BATTLE_RESULT_TYPE.BRT_DRAW;
					break;
				}
				NKCUIResult.BattleResultData resultData = NKCUIResult.MakePvPResultData(battleResultType, sPacket.gainPointItem, NKCUIBattleStatistics.MakeBattleData(gameClient, sPacket.gameRecord, gameData.GetGameType()), gameData.GetGameType());
				NKCScenManager.GetScenManager().Get_SCEN_GAME().ReserveGameEndData(resultData);
				_ = myUserData.m_AsyncData.MaxScore;
				myUserData.m_AsyncData = sPacket.pvpState;
				Debug.Log($"[AsyncPvpGameEndNot] PvpState is null? : {sPacket.pvpState == null}");
				myUserData.m_AsyncPvpHistory.Add(sPacket.history);
				Debug.Log($"[AsyncPvpGameEndNot] History is null? : {sPacket.history == null}");
				myUserData.LastPvpPointChargeTimeUTC = NKCSynchronizedTime.ToUtcTime(sPacket.pointChargeTime);
				myUserData.m_RankOpen = sPacket.rankPvpOpen;
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetAsyncTargetList(sPacket.targetList);
				Debug.Log($"[AsyncPvpGameEndNot] targetList is null? : {sPacket.targetList == null}");
				if (gameData.GetGameType() == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC && NKCScenManager.CurrentUserData().m_NpcData.MaxOpenedTier < sPacket.npcMaxOpenedTier)
				{
					NKCScenManager.CurrentUserData().m_NpcData.MaxOpenedTier = sPacket.npcMaxOpenedTier;
					NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReserveOpenNpcBotTier(sPacket.npcMaxOpenedTier);
				}
			}
		}
		myUserData.m_InventoryData.AddItemMisc(sPacket.gainPointItem);
		myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItem);
		NKCPublisherModule.Push.UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP.PVP_POINT_COMPLETE, bForce: true);
	}

	public static void OnRecv(NKMPacket_STRATEGY_PVP_REFRESH_NOT sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().m_AsyncData = sPacket.data;
		}
	}

	public static void OnRecv(NKMPacket_PVP_RANK_LIST_ACK cNKMPacket_PVP_RANK_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_PVP_RANK_LIST_ACK.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(cNKMPacket_PVP_RANK_LIST_ACK);
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_RANK_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaderBoardManager.OnRecv(sPacket);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_PVP_CHARGE_POINT_REFRESH_ACK cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK.itemData);
		if (cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK.itemData != null)
		{
			if (cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK.itemData.ItemID == 9)
			{
				myUserData.LastPvpPointChargeTimeUTC = NKCSynchronizedTime.ToUtcTime(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK.chrageTime);
			}
			else if (cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK.itemData.ItemID == 6)
			{
				myUserData.LastPvpPointChargeTimeUTC = NKCSynchronizedTime.ToUtcTime(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK.chrageTime);
			}
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME().OnRecv(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK);
		}
	}

	public static void OnGameEndCommon(NKMGameEndData gameEndData)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
		int stageID = 0;
		int dungeonID = 0;
		NKM_GAME_TYPE gameType = NKM_GAME_TYPE.NGT_INVALID;
		if (gameData != null)
		{
			dungeonID = ((gameEndData.dungeonClearData != null) ? gameEndData.dungeonClearData.dungeonId : gameData.m_DungeonID);
			gameType = gameData.m_NKM_GAME_TYPE;
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
			switch (gameData.m_NKM_GAME_TYPE)
			{
			case NKM_GAME_TYPE.NGT_PHASE:
				stageID = NKCPhaseManager.PhaseModeState.stageId;
				break;
			case NKM_GAME_TYPE.NGT_WARFARE:
			{
				NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(gameData.m_WarfareID);
				stageID = ((nKMWarfareTemplet != null && nKMWarfareTemplet.StageTemplet != null) ? nKMWarfareTemplet.StageTemplet.Key : 0);
				break;
			}
			default:
				stageID = ((dungeonTempletBase != null && dungeonTempletBase.StageTemplet != null) ? dungeonTempletBase.StageTemplet.Key : 0);
				break;
			}
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && !gameEndData.giveup)
		{
			Debug.Log("NKMPacket_GAME_END_NOT making resultUIData");
			NKCScenManager.GetScenManager().GetGameClient();
			if (gameData == null)
			{
				Debug.LogError("FATAL : GAMEDATA NULL");
			}
		}
		NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
		if (nKCRepeatOperaion != null && nKCRepeatOperaion.GetIsOnGoing() && gameEndData.costItemDataList != null && gameEndData.costItemDataList.Count > 0)
		{
			nKCRepeatOperaion.SetCostIncreaseCount(nKCRepeatOperaion.GetCostIncreaseCount() + 1);
		}
		myUserData.m_InventoryData.UpdateItemInfo(gameEndData.costItemDataList);
		if (!NKCPhaseManager.IsPhaseOnGoing())
		{
			NKCKillCountManager.CurrentStageKillCount = 0L;
			NKCPhaseManager.ClearTempUnitData();
		}
		else
		{
			NKCPhaseManager.SaveTempUnitData(NKCScenManager.GetScenManager().GetGameClient(), gameEndData.gameRecord);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && gameEndData.giveup)
		{
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetAlarmRepeatOperationQuitByDefeat(bSet: false);
			if (gameEndData.restart)
			{
				NKCScenManager.GetScenManager().Get_SCEN_GAME().DoAfterRestart(gameType, stageID, dungeonID, gameEndData.deckIndex);
			}
			else
			{
				NKCScenManager.GetScenManager().Get_SCEN_GAME().DoAfterGiveUp();
			}
		}
		NKCKillCountManager.UpdateKillCountData(gameEndData.killCountData);
	}

	public static void OnRecv(NKMPacket_WARFARE_GAME_START_ACK cNKMPacket_WARFARE_GAME_START_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WARFARE_GAME_START_ACK.errorCode))
		{
			if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
			{
				NKCPopupOKCancel.ClosePopupBox();
				NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
				NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetStopReason(NKCStringTable.GetString(cNKMPacket_WARFARE_GAME_START_ACK.errorCode.ToString()));
				NKCPopupRepeatOperation.Instance.OpenForResult();
			}
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				bool isOnGoing = NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing();
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
					.m_NKCWarfareGameHUD.SetActiveRepeatOperationOnOff(isOnGoing);
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
					.SetUserUnitDeckWarfareState();
			}
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_WARFARE_GAME_START_ACK.costItemDataList);
		NKCScenManager.GetScenManager().SetWarfareGameData(cNKMPacket_WARFARE_GAME_START_ACK.warfareGameData);
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_WARFARE_GAME)
		{
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OnRecv(cNKMPacket_WARFARE_GAME_START_ACK);
		if (cNKMPacket_WARFARE_GAME_START_ACK.warfareGameData != null)
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(cNKMPacket_WARFARE_GAME_START_ACK.warfareGameData.warfareTempletID);
			string key = $"{myUserData.m_UserUID}_{nKMWarfareTemplet.m_WarfareStrID}";
			if (!PlayerPrefs.HasKey(key) && !NKCScenManager.CurrentUserData().CheckWarfareClear(nKMWarfareTemplet.m_WarfareStrID))
			{
				PlayerPrefs.SetInt(key, 0);
			}
		}
	}

	public static void OnRecv(NKMPacket_WARFARE_GAME_MOVE_ACK cNKMPacket_WARFARE_GAME_MOVE_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME && NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame() != null)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
				.SetPause(bSet: false);
		}
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WARFARE_GAME_MOVE_ACK.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OnRecv(cNKMPacket_WARFARE_GAME_MOVE_ACK);
		}
	}

	public static void OnRecv(NKMPacket_WARFARE_GAME_TURN_FINISH_ACK cNKMPacket_WARFARE_GAME_TURN_FINISH_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME && NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame() != null)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
				.SetPause(bSet: false);
		}
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WARFARE_GAME_TURN_FINISH_ACK.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OnRecv(cNKMPacket_WARFARE_GAME_TURN_FINISH_ACK);
		}
	}

	public static void OnRecv(NKMPacket_WARFARE_GAME_NEXT_ORDER_ACK cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().InitWaitNextOrder();
			if (NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame() != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
					.SetPause(bSet: false);
			}
		}
		Debug.Log("NKMPacket_WARFARE_GAME_NEXT_ORDER_ACK - CurrentScenID : " + NKCScenManager.GetScenManager().GetNowScenID().ToString() + ", errorCode : " + cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.errorCode);
		if (cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.errorCode == NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_GET_NEXT_ORDER_AT_TURN_A || cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.errorCode == NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_NEXT_ORDER_WARFARE_STATE_INGAME_PLAYING)
		{
			Debug.LogWarning("NKMPacket_WARFARE_GAME_NEXT_ORDER_ACK - errorCode : " + cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.errorCode);
			NKMPopUpBox.CloseWaitBox();
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME && cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.errorCode == NKM_ERROR_CODE.NEC_FAIL_WARFARE_NOT_ENOUGH_SUPPLEMENT && NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
		{
			NKMPopUpBox.CloseWaitBox();
			if (NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame() != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
					.SetPause(bSet: true);
			}
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetStopReason(NKCStringTable.GetString(cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.errorCode.ToString()));
			NKCPopupRepeatOperation.Instance.OpenForResult(delegate
			{
				if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME && NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame() != null)
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
						.SetPause(bSet: false);
				}
			});
		}
		else if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.errorCode))
		{
			NKCUIGameOption.CheckInstanceAndClose();
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OnRecv(cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_WARFARE_GAME_GIVE_UP_ACK cNKMPacket_WARFARE_GAME_GIVE_UP_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WARFARE_GAME_GIVE_UP_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				NKCScenManager.GetScenManager().WarfareGameData.warfareGameState = NKM_WARFARE_GAME_STATE.NWGS_STOP;
				myUserData.m_ArmyData.ResetDeckStateOf(NKM_DECK_STATE.DECK_STATE_WARFARE);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OnRecv(cNKMPacket_WARFARE_GAME_GIVE_UP_ACK);
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_OPERATION && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_SHADOW_BATTLE)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_SHADOW_BATTLE().OnRecv(cNKMPacket_WARFARE_GAME_GIVE_UP_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_WARFARE_GAME_AUTO_ACK cNKMPacket_WARFARE_GAME_AUTO_ACK)
	{
		if (NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().WaitAutoPacekt)
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
					.SetPause(bSet: false);
			}
			if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WARFARE_GAME_AUTO_ACK.errorCode))
			{
				return;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().WaitAutoPacekt = false;
		}
		else if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WARFARE_GAME_AUTO_ACK.errorCode, bCloseWaitBox: false))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			myUserData.m_UserOption.m_bAutoWarfare = cNKMPacket_WARFARE_GAME_AUTO_ACK.isAuto;
			myUserData.m_UserOption.m_bAutoWarfareRepair = cNKMPacket_WARFARE_GAME_AUTO_ACK.isAutoRepair;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OnRecv(cNKMPacket_WARFARE_GAME_AUTO_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_WARFARE_GAME_USE_SERVICE_ACK cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
				.SetPause(bSet: false);
		}
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				NKCWarfareManager.UseService(myUserData, NKCScenManager.GetScenManager().WarfareGameData, cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK);
				myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK.costItemData);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OnRecv(cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_WARFARE_EXPIRED_NOT cNKMPacket_WARFARE_EXPIRED_NOT)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKCScenManager.GetScenManager().WarfareGameData.warfareGameState = NKM_WARFARE_GAME_STATE.NWGS_STOP;
			myUserData.m_ArmyData.ResetDeckStateOf(NKM_DECK_STATE.DECK_STATE_WARFARE);
			NKCUtil.ProcessWFExpireTime();
		}
		switch (NKCScenManager.GetScenManager().GetNowScenID())
		{
		case NKM_SCEN_ID.NSI_HOME:
			NKCScenManager.GetScenManager().Get_SCEN_HOME().OnRecv(cNKMPacket_WARFARE_EXPIRED_NOT);
			break;
		case NKM_SCEN_ID.NSI_WARFARE_GAME:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OnRecv(cNKMPacket_WARFARE_EXPIRED_NOT);
			break;
		}
	}

	public static void OnRecv(NKMPacket_WARFARE_FRIEND_LIST_ACK cNKMPacket_WARFARE_FRIEND_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WARFARE_FRIEND_LIST_ACK.errorCode))
		{
			NKCWarfareManager.SetSupportList(cNKMPacket_WARFARE_FRIEND_LIST_ACK.friends, cNKMPacket_WARFARE_FRIEND_LIST_ACK.guests);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OnRecv(cNKMPacket_WARFARE_FRIEND_LIST_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_WARFARE_RECOVER_ACK cNKMPacket_WARFARE_RECOVER_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
				.SetPause(bSet: false);
		}
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WARFARE_RECOVER_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.UpdateItemInfo(cNKMPacket_WARFARE_RECOVER_ACK.costItemDataList);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().OnRecv(cNKMPacket_WARFARE_RECOVER_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_STAGE_UNLOCK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(sPacket.stageId);
			string text = "";
			NKMDungeonTempletBase dungeonTempletBase = nKMStageTempletV.DungeonTempletBase;
			text = ((dungeonTempletBase == null || dungeonTempletBase.m_DungeonType != NKM_DUNGEON_TYPE.NDT_CUTSCENE) ? string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_COMPLETE_EC_SIDESTORY"), nKMStageTempletV.EpisodeTemplet.GetEpisodeTitle(), nKMStageTempletV.ActId, nKMStageTempletV.m_StageUINum) : string.Format(NKCStringTable.GetString("SI_DP_UNLOCK_CUTSCENE_COMPLETE_EC_SIDESTORY"), nKMStageTempletV.EpisodeTemplet.GetEpisodeTitle(), nKMStageTempletV.GetDungeonName()));
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(text, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			NKCScenManager.CurrentUserData().m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			NKMEpisodeMgr.SetUnlockedStage(sPacket.stageId);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OPERATION)
			{
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_FAVORITES_STAGE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMEpisodeMgr.SetFavoriteStage(sPacket.favoritesStage);
		}
	}

	public static void OnRecv(NKMPacket_FAVORITES_STAGE_UPDATE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMEpisodeMgr.SetFavoriteStage(sPacket.favoritesStage);
			if (NKCPopupFavorite.isOpen())
			{
				NKCPopupFavorite.Instance.CancelEditMode();
				NKCPopupFavorite.Instance.RefreshList();
			}
		}
	}

	public static void OnRecv(NKMPacket_FAVORITES_STAGE_ADD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMEpisodeMgr.SetFavoriteStage(sPacket.favoritesStage);
		}
	}

	public static void OnRecv(NKMPacket_FAVORITE_STAGE_DELETE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMEpisodeMgr.SetFavoriteStage(sPacket.favoritesStage);
		}
	}

	public static void OnRecv(NKMPacket_SET_EMBLEM_ACK cNKMPacket_SET_EMBLEM_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_SET_EMBLEM_ACK.errorCode))
		{
			NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
			if (userProfileData != null && cNKMPacket_SET_EMBLEM_ACK.index >= 0 && cNKMPacket_SET_EMBLEM_ACK.index < userProfileData.emblems.Count)
			{
				int index = cNKMPacket_SET_EMBLEM_ACK.index;
				userProfileData.emblems[index].id = cNKMPacket_SET_EMBLEM_ACK.itemId;
				userProfileData.emblems[index].count = cNKMPacket_SET_EMBLEM_ACK.count;
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_SET_EMBLEM_ACK);
			}
			if (NKCUIUserInfoV2.IsInstanceOpen)
			{
				NKCUIUserInfoV2.Instance.OnRecv(cNKMPacket_SET_EMBLEM_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_MY_USER_PROFILE_INFO_ACK cNKMPacket_MY_USER_PROFILE_INFO_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_MY_USER_PROFILE_INFO_ACK.errorCode))
		{
			NKCScenManager.CurrentUserData().SetMyUserProfileInfo(cNKMPacket_MY_USER_PROFILE_INFO_ACK.userProfileData);
			if (NKCUIPopupOfficeInteract.IsInstanceOpen)
			{
				NKCUIPopupOfficeInteract.Instance.UpdateMyBizCard();
			}
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_LIST_ACK cNKMPacket_FRIEND_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_LIST_ACK.errorCode))
		{
			if (cNKMPacket_FRIEND_LIST_ACK.friendListType == NKM_FRIEND_LIST_TYPE.FRIEND)
			{
				NKCFriendManager.SetFriendList(cNKMPacket_FRIEND_LIST_ACK.list);
			}
			else if (cNKMPacket_FRIEND_LIST_ACK.friendListType == NKM_FRIEND_LIST_TYPE.BLOCKER)
			{
				NKCFriendManager.SetBlockList(cNKMPacket_FRIEND_LIST_ACK.list);
			}
			else if (cNKMPacket_FRIEND_LIST_ACK.friendListType == NKM_FRIEND_LIST_TYPE.RECEIVE_REQUEST)
			{
				NKCFriendManager.SetReceivedREQList(cNKMPacket_FRIEND_LIST_ACK.list);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_LIST_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_DELETE_NOT not)
	{
		NKCFriendManager.DeleteFriend(not.friendCode);
		NKCChatManager.RemoveFriendByFriendCode(not.friendCode);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME()?.UpdateRightSide3DButton(NKCUILobbyV2.eUIMenu.Friends);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(not);
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_CANCEL_REQUEST_NOT not)
	{
		NKCFriendManager.RemoveReceivedREQ(not.friendCode);
		NKC_SCEN_HOME sCEN_HOME = NKCScenManager.GetScenManager().Get_SCEN_HOME();
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME && sCEN_HOME != null)
		{
			sCEN_HOME.UpdateRightSide3DButton(NKCUILobbyV2.eUIMenu.Friends);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(not);
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_ACCEPT_NOT cNKMPacket_FRIEND_ACCEPT_NOT)
	{
		if (cNKMPacket_FRIEND_ACCEPT_NOT.isAllow)
		{
			NKCFriendManager.AddFriend(cNKMPacket_FRIEND_ACCEPT_NOT.friendProfileData);
			NKCChatManager.AddFriend(cNKMPacket_FRIEND_ACCEPT_NOT.friendProfileData.commonProfile);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_ACCEPT_NOT);
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_SEARCH_ACK cNKMPacket_FRIEND_SEARCH_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_SEARCH_ACK.errorCode))
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_SEARCH_ACK);
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().OnRecv(cNKMPacket_FRIEND_SEARCH_ACK.list);
			}
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_RECOMMEND_ACK cNKMPacket_FRIEND_RECOMMEND_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_RECOMMEND_ACK.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_RECOMMEND_ACK);
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_REQUEST_ACK cNKMPacket_FRIEND_ADD_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_ADD_ACK.errorCode))
		{
			return;
		}
		NKCPopupFriendInfo.CheckInstanceAndClose();
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_FRIEND_ADD_REQ_COMPLETE);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OFFICE)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FRIEND_ADD_REQ_COMPLETE, delegate
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_OFFICE_BIZ_CARD_SENT"));
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FRIEND_ADD_REQ_COMPLETE);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_ADD_ACK);
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_REQUEST_NOT cNKMPacket_FRIEND_REQUEST_NOT)
	{
		NKCFriendManager.AddReceivedREQ(cNKMPacket_FRIEND_REQUEST_NOT.friendProfileData.commonProfile.friendCode);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME()?.UpdateRightSide3DButton(NKCUILobbyV2.eUIMenu.Friends);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_REQUEST_NOT);
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK cNKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK)
	{
		bool bCloseWaitBox = true;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
		{
			bCloseWaitBox = NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().DecreaseWaitingRespondCount() <= 0;
		}
		if (NKCUIUserInfoV2.IsInstanceOpen)
		{
			bCloseWaitBox = --NKCUIUserInfoV2.Instance.WaitingRespondCount <= 0;
		}
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK.errorCode, bCloseWaitBox))
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().CloseImageChange();
			NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
			if (userProfileData != null)
			{
				userProfileData.commonProfile.mainUnitId = cNKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK.mainCharId;
				userProfileData.commonProfile.mainUnitSkinId = cNKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK.mainCharSkinId;
				userProfileData.commonProfile.mainUnitTacticLevel = cNKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK.mainUnitTacticLevel;
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK);
			}
			if (NKCUIUserInfoV2.IsInstanceOpen)
			{
				NKCUIUserInfoV2.Instance.UpdateMainCharUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK cNKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK.errorCode))
		{
			NKCUIDeckViewer.CheckInstanceAndClose();
			NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
			if (userProfileData != null)
			{
				userProfileData.profileDeck = cNKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK.deckData;
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK);
			}
			if (NKCUIUserInfoV2.IsInstanceOpen)
			{
				NKCUIUserInfoV2.Instance.UpdateDeckUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK.errorCode))
		{
			cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK.intro = NKCFilterManager.CheckBadChat(cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK.intro);
			NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
			if (userProfileData != null)
			{
				userProfileData.friendIntro = cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK.intro;
			}
			NKCUIUserInfo.SetComment(cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK.intro);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK);
			}
			if (NKCUIUserInfoV2.IsInstanceOpen)
			{
				NKCUIUserInfoV2.Instance.UpdateCommentUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_GREETING_MESSAGE_ACK cNKMPacket_GREETING_MESSAGE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GREETING_MESSAGE_ACK.errorCode))
		{
			NKCUIUserInfo.SetComment(cNKMPacket_GREETING_MESSAGE_ACK.message);
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_ACCEPT_ACK cNKMPacket_FRIEND_ACCEPT_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_ACCEPT_ACK.errorCode))
		{
			NKCPopupFriendInfo.CheckInstanceAndClose();
			if (cNKMPacket_FRIEND_ACCEPT_ACK.isAllow)
			{
				Log.Info("<color=#ffffff>AddFriend</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketHandlersLobby.cs", 4470);
				NKCFriendManager.AddFriend(cNKMPacket_FRIEND_ACCEPT_ACK.friendCode);
			}
			NKCFriendManager.RemoveReceivedREQ(cNKMPacket_FRIEND_ACCEPT_ACK.friendCode);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_ACCEPT_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_CANCEL_REQUEST_ACK cNKMPacket_FRIEND_ADD_CANCEL_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_ADD_CANCEL_ACK.errorCode))
		{
			NKCPopupFriendInfo.CheckInstanceAndClose();
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_ADD_CANCEL_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_BLOCK_ACK cNKMPacket_FRIEND_BLOCK_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_BLOCK_ACK.errorCode))
		{
			NKCPopupFriendInfo.CheckInstanceAndClose();
			if (cNKMPacket_FRIEND_BLOCK_ACK.isCancel)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FRIEND_BLOCK_CANCEL_NOTICE);
				NKCFriendManager.RemoveBlockUser(cNKMPacket_FRIEND_BLOCK_ACK.friendCode);
			}
			else if (NKCFriendManager.IsFriend(cNKMPacket_FRIEND_BLOCK_ACK.friendCode))
			{
				NKCFriendManager.DeleteFriend(cNKMPacket_FRIEND_BLOCK_ACK.friendCode);
				NKCFriendManager.AddBlockUser(cNKMPacket_FRIEND_BLOCK_ACK.friendCode);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_BLOCK_ACK);
			}
			if (NKCPopupPrivateChatLobby.IsInstanceOpen)
			{
				NKCPopupPrivateChatLobby.Instance.OnRecvFriendBlock(cNKMPacket_FRIEND_BLOCK_ACK.friendCode);
			}
		}
	}

	public static void OnRecv(NKMPacket_FRIEND_DELETE_ACK cNKMPacket_FRIEND_DEL_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_FRIEND_DEL_ACK.errorCode))
		{
			NKCPopupFriendInfo.CheckInstanceAndClose();
			NKCFriendManager.DeleteFriend(cNKMPacket_FRIEND_DEL_ACK.friendCode);
			NKCChatManager.RemoveFriendByFriendCode(cNKMPacket_FRIEND_DEL_ACK.friendCode);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(cNKMPacket_FRIEND_DEL_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_USER_PROFILE_CHANGE_FRAME_ACK sPacket)
	{
		bool bCloseWaitBox = true;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
		{
			bCloseWaitBox = NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().DecreaseWaitingRespondCount() <= 0;
		}
		if (NKCUIUserInfoV2.IsInstanceOpen)
		{
			bCloseWaitBox = --NKCUIUserInfoV2.Instance.WaitingRespondCount <= 0;
		}
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode, bCloseWaitBox))
		{
			NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
			if (userProfileData != null)
			{
				userProfileData.commonProfile.frameId = sPacket.selfiFrameId;
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FRIEND)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().OnRecv(sPacket);
			}
			if (NKCUIUserInfoV2.IsInstanceOpen)
			{
				NKCUIUserInfoV2.Instance.UpdateMainCharUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_RANDOM_MISSION_REFRESH_NOT cNKMPacket_RANDOM_MISSION_REFRESH_NOT)
	{
		if (NKCScenManager.CurrentUserData() == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		nKMUserData.m_MissionData.RemoveAllRandomMissionInTab(cNKMPacket_RANDOM_MISSION_REFRESH_NOT.tabId);
		foreach (NKMMissionData missionData in cNKMPacket_RANDOM_MISSION_REFRESH_NOT.missionDataList)
		{
			nKMUserData.m_MissionData.AddMission(missionData);
		}
		nKMUserData.m_MissionData.OnRandomMissionRefresh();
	}

	public static void OnRecv(NKMPacket_RANDOM_MISSION_CHANGE_ACK cNKMPacket_RANDOM_MISSION_CHANGE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_RANDOM_MISSION_CHANGE_ACK.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_RANDOM_MISSION_CHANGE_ACK.costItemData);
				nKMUserData.m_MissionData.RemoveMission(cNKMPacket_RANDOM_MISSION_CHANGE_ACK.beforeGroupId);
				nKMUserData.m_MissionData.AddMission(cNKMPacket_RANDOM_MISSION_CHANGE_ACK.afterMissionData);
				NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(cNKMPacket_RANDOM_MISSION_CHANGE_ACK.afterMissionData.mission_id);
				nKMUserData.m_MissionData.SetRandomMissionRefreshCount(missionTemplet.m_MissionTabId, cNKMPacket_RANDOM_MISSION_CHANGE_ACK.remainRefreshCount);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_MISSION_GIVE_ITEM_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(sPacket.costItems);
		}
	}

	public static void OnRecv(NKMPacket_MISSION_COMPLETE_ALL_ACK cNKMPacket_MISSION_COMPLETE_ALL_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_MISSION_COMPLETE_ALL_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.GetReward(cNKMPacket_MISSION_COMPLETE_ALL_ACK.rewardDate);
		for (int i = 0; i < cNKMPacket_MISSION_COMPLETE_ALL_ACK.missionIDList.Count; i++)
		{
			myUserData.m_MissionData.SetCompleteMissionData(cNKMPacket_MISSION_COMPLETE_ALL_ACK.missionIDList[i]);
		}
		NKMMissionManager.SetHaveClearedMission(myUserData.m_MissionData.CheckCompletableMission(myUserData));
		NKMMissionManager.SetHaveClearedMissionGuide(myUserData.m_MissionData.CheckCompletableGuideMission(myUserData));
		bool flag = false;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME && NKCUIMissionAchievement.IsInstanceOpen)
		{
			flag = true;
			NKCUIMissionAchievement.Instance.OnRecv(cNKMPacket_MISSION_COMPLETE_ALL_ACK);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME && NKCUIMissionGuide.IsInstanceOpen)
		{
			flag = true;
			NKCUIMissionGuide.Instance.OnRecv(cNKMPacket_MISSION_COMPLETE_ALL_ACK);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
		}
		if (NKCUIEvent.IsInstanceOpen)
		{
			NKCUIEvent.Instance.RefreshUI();
		}
		if (NKCUIEventPass.IsInstanceOpen)
		{
			flag = true;
			if (cNKMPacket_MISSION_COMPLETE_ALL_ACK.additionalReward != null)
			{
				NKCUIEventPass.Instance.RefreshPassAdditionalExpRelatedInfo(cNKMPacket_MISSION_COMPLETE_ALL_ACK.additionalReward.eventPassExpDelta);
			}
			NKCPopupMessageToastSimple.Instance.Open(cNKMPacket_MISSION_COMPLETE_ALL_ACK.rewardDate, cNKMPacket_MISSION_COMPLETE_ALL_ACK.additionalReward, delegate
			{
				NKCUIEventPass.Instance.PlayExpDoTween();
			});
		}
		NKMEventCollectionIndexTemplet nKMEventCollectionIndexTemplet = null;
		foreach (NKCUIModuleHome item in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
		{
			if (item.IsOpen)
			{
				item.UpdateUI();
				nKMEventCollectionIndexTemplet = item.EventCollectionIndexTemplet;
				if (nKMEventCollectionIndexTemplet != null && !string.IsNullOrEmpty(nKMEventCollectionIndexTemplet.EventMergeResultPrefabID))
				{
					flag = true;
				}
			}
		}
		if (nKMEventCollectionIndexTemplet != null && flag)
		{
			NKCUIPopupModuleResult popupResultUIData = NKCUIPopupModuleResult.MakeInstance(nKMEventCollectionIndexTemplet.EventResultPrefabID, nKMEventCollectionIndexTemplet.EventResultPrefabID);
			if (null != popupResultUIData)
			{
				popupResultUIData.Init();
				popupResultUIData.Open(cNKMPacket_MISSION_COMPLETE_ALL_ACK.rewardDate, cNKMPacket_MISSION_COMPLETE_ALL_ACK.additionalReward, delegate
				{
					if (popupResultUIData.IsOpen)
					{
						popupResultUIData.Close();
						popupResultUIData = null;
					}
				});
			}
		}
		NKCUIManager.ForAllUI(delegate(NKCPopupEventPayReward ui)
		{
			ui.Refresh();
		});
		if (!flag)
		{
			NKCUIResult.Instance.OpenRewardGain(myUserData.m_ArmyData, cNKMPacket_MISSION_COMPLETE_ALL_ACK.rewardDate, cNKMPacket_MISSION_COMPLETE_ALL_ACK.additionalReward, NKCUtilString.GET_STRING_RESULT_MISSION);
		}
	}

	public static void OnRecv(NKMPacket_MISSION_COMPLETE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.GetReward(sPacket.rewardData);
			myUserData.m_MissionData.SetCompleteMissionData(sPacket.missionID);
			NKMMissionManager.SetHaveClearedMission(myUserData.m_MissionData.CheckCompletableMission(myUserData));
			NKMMissionManager.SetHaveClearedMissionGuide(myUserData.m_MissionData.CheckCompletableGuideMission(myUserData));
			MissionCompleteCommonProcess(sPacket.missionID, sPacket.rewardData, sPacket.additionalReward);
		}
	}

	public static void OnRecv(NKMPacket_MISSION_GET_COMPLETE_REWARD_ACK cPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.GetReward(cPacket.rewardData);
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(cPacket.updatedMissionData.mission_id);
			myUserData.m_MissionData.GetMissionData(missionTemplet).UpdateMissionData(cPacket.updatedMissionData.mission_id, cPacket.updatedMissionData.times, cPacket.updatedMissionData.IsComplete, cPacket.updatedMissionData.last_update_date);
			MissionCompleteCommonProcess(cPacket.updatedMissionData.mission_id, cPacket.rewardData, null);
		}
	}

	private static void MissionCompleteCommonProcess(int missionID, NKMRewardData rewardData, NKMAdditionalReward additionalReward)
	{
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionID);
		if (missionTemplet == null)
		{
			return;
		}
		if (missionTemplet.m_TrackingEvent != string.Empty)
		{
			NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.MissionClear, 0, missionTemplet.m_TrackingEvent);
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.TUTORIAL)
			{
				Debug.Log($"Tutorial mission {missionID} Completed!");
				NKCGameEventManager.TutorialCompletePacketSent(missionID);
				return;
			}
			if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.BINGO)
			{
				if (NKCPopupEventMission.IsInstanceOpen)
				{
					NKCPopupEventMission.Instance.OnCompleteMision(missionID);
				}
				return;
			}
		}
		bool flag = false;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME && NKCUIMissionAchievement.IsInstanceOpen)
		{
			flag = true;
			NKCUIMissionAchievement.Instance.OnMissionComplete(missionID, rewardData, additionalReward);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME && NKCUIMissionGuide.IsInstanceOpen)
		{
			flag = true;
			NKCUIMissionGuide.Instance.OnMissionComplete(missionID, rewardData, additionalReward);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
		}
		if (NKCPopupHamburgerMenu.IsInstanceOpen)
		{
			if (!flag)
			{
				flag = true;
				NKCPopupHamburgerMenu.instance.OnMissionComplete(rewardData);
			}
			NKCPopupHamburgerMenu.instance.Refresh();
		}
		if (NKCUIEvent.IsInstanceOpen)
		{
			NKCUIEvent.Instance.RefreshUI();
		}
		if (NKCUIEventPass.IsInstanceOpen)
		{
			flag = true;
			if (additionalReward != null)
			{
				NKCUIEventPass.Instance.RefreshPassAdditionalExpRelatedInfo(additionalReward.eventPassExpDelta);
			}
			NKCPopupMessageToastSimple.Instance.Open(rewardData, additionalReward, delegate
			{
				NKCUIEventPass.Instance.PlayExpDoTween();
			});
		}
		NKMEventCollectionIndexTemplet nKMEventCollectionIndexTemplet = null;
		foreach (NKCUIModuleHome item in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
		{
			if (item.IsOpen)
			{
				item.UpdateUI();
				nKMEventCollectionIndexTemplet = item.EventCollectionIndexTemplet;
				if (nKMEventCollectionIndexTemplet != null && !string.IsNullOrEmpty(nKMEventCollectionIndexTemplet.EventMergeResultPrefabID))
				{
					flag = true;
				}
			}
		}
		if (nKMEventCollectionIndexTemplet != null && flag)
		{
			NKCUIPopupModuleResult popupResult = NKCUIPopupModuleResult.MakeInstance(nKMEventCollectionIndexTemplet.EventResultPrefabID, nKMEventCollectionIndexTemplet.EventResultPrefabID);
			if (null != popupResult)
			{
				popupResult.Init();
				popupResult.Open(rewardData, additionalReward, delegate
				{
					if (popupResult.IsOpen)
					{
						popupResult.Close();
						popupResult = null;
					}
				});
			}
		}
		if (!flag)
		{
			NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentArmyData(), rewardData, additionalReward, NKCUtilString.GET_STRING_RESULT_MISSION);
		}
	}

	public static void OnRecv(NKMPacket_MISSION_UPDATE_NOT cNKMPacket_MISSION_UPDATE_NOT)
	{
		if (cNKMPacket_MISSION_UPDATE_NOT.missionDataList == null)
		{
			return;
		}
		foreach (NKMMissionData missionData in cNKMPacket_MISSION_UPDATE_NOT.missionDataList)
		{
			if (missionData == null)
			{
				continue;
			}
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionData.mission_id);
			if (missionTemplet == null)
			{
				continue;
			}
			NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId);
			if (missionTabTemplet == null || missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.TUTORIAL)
			{
				continue;
			}
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			bool flag = false;
			bool flag2 = false;
			if (myUserData != null)
			{
				if (NKMMissionManager.CanComplete(missionTemplet, myUserData, missionData) == NKM_ERROR_CODE.NEC_OK)
				{
					flag = true;
					if (NKMMissionManager.IsCumulativeCondition(missionTemplet.m_MissionCond.mission_cond))
					{
						bool flag3 = false;
						NKMMissionData missionDataByMissionId = myUserData.m_MissionData.GetMissionDataByMissionId(missionData.mission_id);
						if (missionDataByMissionId != null && !NKMMissionManager.CheckCanReset(missionTemplet.m_ResetInterval, missionDataByMissionId))
						{
							flag3 = NKMMissionManager.CanComplete(missionTemplet, myUserData, missionDataByMissionId) == NKM_ERROR_CODE.NEC_OK;
						}
						if (!flag3)
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				myUserData.m_MissionData.AddOrUpdateMission(missionData);
			}
			if (flag)
			{
				if (flag2)
				{
					NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_MISSION_COMPLETE_ONE_PARAM, missionTemplet.GetTitle()), NKCPopupMessage.eMessagePosition.Top, bShowFX: true, bPreemptive: false, 0f, bWaitForGameEnd: true);
					NKCSoundManager.PlaySound("FX_UI_DECK_SLOT_OPEN", 1f, 0f, 0f);
				}
				if (missionTabTemplet.EnableByTag)
				{
					if (missionTabTemplet.m_MissionType != NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
					{
						NKMMissionManager.SetHaveClearedMission(bSet: true, missionTabTemplet.m_Visible);
					}
					if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
					{
						NKMMissionManager.SetHaveClearedMissionGuide(bSet: true, missionTabTemplet.m_Visible);
					}
				}
			}
			if (NKCUIMissionAchievement.IsInstanceOpen)
			{
				if (NKCUIMissionAchievement.Instance.gameObject.activeInHierarchy)
				{
					NKCUIMissionAchievement.Instance.SetUIByCurrTab();
				}
				else
				{
					NKCUIMissionAchievement.Instance.ReservedRefresh(cNKMPacket_MISSION_UPDATE_NOT);
				}
				NKCUIMissionAchievement.Instance.SetCompletableMissionAlarm();
			}
			if (NKCUIMissionGuide.IsInstanceOpen)
			{
				if (NKCUIMissionGuide.Instance.gameObject.activeInHierarchy)
				{
					NKCUIMissionGuide.Instance.SetUIByCurrTab();
				}
				else
				{
					NKCUIMissionGuide.Instance.ReservedRefresh(cNKMPacket_MISSION_UPDATE_NOT);
				}
				NKCUIMissionGuide.Instance.SetCompletableMissionAlarm();
			}
		}
		if (cNKMPacket_MISSION_UPDATE_NOT.missionDataList.Count > 0)
		{
			if (NKCUIEvent.IsInstanceOpen)
			{
				NKCUIEvent.Instance.RefreshUI();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
			}
			if (NKCUIEventPass.IsEventTime(activateAlarm: false))
			{
				NKCUIEventPass.RefreshMissionState(cNKMPacket_MISSION_UPDATE_NOT.missionDataList);
			}
			NKCUIModuleHome.UpdateAllModule();
		}
	}

	public static void OnRecv(NKMPacket_SURVEY_UPSERT_NOT cNKMPacket_SURVEY_UPSERT_NOT)
	{
		if (cNKMPacket_SURVEY_UPSERT_NOT.surveyInfos != null)
		{
			for (int i = 0; i < cNKMPacket_SURVEY_UPSERT_NOT.surveyInfos.Count; i++)
			{
				NKCScenManager.GetScenManager().GetNKCSurveyMgr().UpdaterOrAdd(cNKMPacket_SURVEY_UPSERT_NOT.surveyInfos[i]);
			}
		}
	}

	public static void OnRecv(NKMPacket_SURVEY_RESET_NOT cNKMPacket_SURVEY_RESET_NOT)
	{
		NKCScenManager.GetScenManager().GetNKCSurveyMgr().Clear();
	}

	public static void OnRecv(NKMPacket_SURVEY_COMPLETE_ACK cNKMPacket_SURVEY_COMPLETE_ACK)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_SURVEY_COMPLETE_ACK.errorCode);
	}

	public static void OnRecv(NKMPacket_WORLDMAP_EVENT_CANCEL_ACK cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMWorldMapCityData cityData = nKMUserData.m_WorldmapData.GetCityData(cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK.cityID);
		if (cityData == null)
		{
			return;
		}
		NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(cityData.worldMapEventGroup.worldmapEventID);
		if (nKMWorldMapEventTemplet != null)
		{
			if (nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_DIVE)
			{
				NKMUserData nKMUserData2 = NKCScenManager.CurrentUserData();
				if (nKMUserData2 != null && nKMUserData2.m_DiveGameData != null && nKMUserData2.m_DiveGameData.DiveUid == cityData.worldMapEventGroup.eventUid)
				{
					nKMUserData2.ClearDiveGameData();
				}
			}
			else if (nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
			{
				NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Remove(cityData.worldMapEventGroup.eventUid);
			}
		}
		cityData.worldMapEventGroup.Clear();
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK);
		}
	}

	public static void OnRecv(NKMPacket_WORLDMAP_SET_CITY_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
		if (myUserData.m_WorldmapData.IsCityUnlocked(sPacket.worldMapCityData.cityID))
		{
			Debug.LogError("FATAL : City already opened, Client-server data sync off");
			return;
		}
		myUserData.m_WorldmapData.worldMapCityDataMap.Add(sPacket.worldMapCityData.cityID, sPacket.worldMapCityData);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnWorldManCitySet(sPacket.worldMapCityData.cityID, sPacket.worldMapCityData);
		}
	}

	public static void OnRecv(NKMPacket_WORLDMAP_SET_LEADER_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMWorldMapCityData cityData = myUserData.m_WorldmapData.GetCityData(sPacket.cityID);
		if (cityData == null)
		{
			Debug.LogError("FATAL : City/Area Does not exist, Cilent-Server Templet info sync off");
			return;
		}
		foreach (KeyValuePair<int, NKMWorldMapCityData> item in myUserData.m_WorldmapData.worldMapCityDataMap)
		{
			if (item.Value.leaderUnitUID != 0L && item.Value.leaderUnitUID == sPacket.leaderUID)
			{
				item.Value.leaderUnitUID = 0L;
				break;
			}
		}
		cityData.leaderUnitUID = sPacket.leaderUID;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnCityLeaderChanged(cityData);
		}
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(cityData.leaderUnitUID);
		if (unitFromUID != null)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_WORLDMAP_CHECK, unitFromUID);
		}
	}

	public static void OnRecv(NKMPacket_WORLDMAP_CITY_MISSION_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMWorldMapCityData cityData = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.GetCityData(sPacket.cityID);
		if (cityData == null)
		{
			Debug.LogError("FATAL : City/Area Does not exist, Cilent-Server Templet info sync off");
			return;
		}
		cityData.worldMapMission.completeTime = sPacket.completeTime;
		cityData.worldMapMission.currentMissionID = sPacket.missionID;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().CityDataUpdated(cityData);
		}
		long leaderUnitUID = cityData.leaderUnitUID;
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(leaderUnitUID);
		if (unitFromUID != null)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_WORLDMAP_MISSION_START, unitFromUID);
		}
	}

	public static void OnRecv(NKMPacket_WORLDMAP_CITY_MISSION_CANCEL_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMWorldMapCityData cityData = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.GetCityData(sPacket.cityID);
		if (cityData == null)
		{
			Debug.LogError("FATAL : City/Area Does not exist, Cilent-Server Templet info sync off");
			return;
		}
		cityData.worldMapMission.completeTime = 0L;
		cityData.worldMapMission.currentMissionID = 0;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().CityDataUpdated(cityData);
		}
		NKCPublisherModule.Push.UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP.WORLD_MAP_MISSION_COMPLETE, bForce: true);
	}

	public static void OnRecv(NKMPacket_WORLDMAP_MISSION_REFRESH_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMWorldMapCityData cityData = myUserData.m_WorldmapData.GetCityData(sPacket.cityID);
		if (cityData == null)
		{
			Debug.LogError("FATAL : City/Area Does not exist, Cilent-Server Templet info sync off");
			return;
		}
		myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
		cityData.worldMapMission.stMissionIDList = sPacket.stMissionIDList;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().CityDataUpdated(cityData);
		}
	}

	public static void OnRecv(NKMPacket_WORLDMAP_MISSION_COMPLETE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMWorldMapCityData cityData = myUserData.m_WorldmapData.GetCityData(sPacket.cityID);
		if (cityData == null)
		{
			Debug.LogError("FATAL : City/Area Does not exist, Cilent-Server Templet info sync off");
			return;
		}
		NKCUIResult.CityMissionResultData cityUIData = NKCUIResult.MakeCityMissionCompleteUIData(myUserData.m_ArmyData, cityData, sPacket.rewardData, sPacket.exp, sPacket.level, sPacket.isSuccess);
		bool bGotNewEvent = cityData.worldMapEventGroup.worldmapEventID == 0 && sPacket.worldMapEventGroup.worldmapEventID > 0;
		myUserData.GetReward(sPacket.rewardData);
		cityData.worldMapMission.stMissionIDList = sPacket.stMissionIDList;
		cityData.worldMapMission.completeTime = 0L;
		cityData.worldMapMission.currentMissionID = 0;
		_ = cityData.exp;
		cityData.exp = sPacket.exp;
		cityData.level = sPacket.level;
		cityData.worldMapEventGroup = sPacket.worldMapEventGroup;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().CityDataUpdated(cityData);
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(sPacket, cityData, cityUIData, bGotNewEvent);
		}
	}

	public static void OnRecv(NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMWorldMapCityData cityData = nKMUserData.m_WorldmapData.GetCityData(sPacket.cityID);
		if (cityData != null)
		{
			cityData.worldMapEventGroup.Clear();
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_WORLDMAP_COLLECT_ACK sPacket)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode);
	}

	public static void OnRecv(NKMPacket_WORLDMAP_BUILD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMWorldMapCityData cityData = nKMUserData.m_WorldmapData.GetCityData(sPacket.cityID);
		if (cityData == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_CITY_ID);
			Debug.LogError("FATAL : City not exist. Client-Server data sync off");
			return;
		}
		nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
		NKMWorldmapCityBuildingData nKMWorldmapCityBuildingData = new NKMWorldmapCityBuildingData();
		nKMWorldmapCityBuildingData.id = sPacket.buildID;
		nKMWorldmapCityBuildingData.level = 1;
		cityData.AddBuild(nKMWorldmapCityBuildingData);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().CityBuildingChanged(cityData, nKMWorldmapCityBuildingData.id);
		}
	}

	public static void OnRecv(NKMPacket_WORLDMAP_BUILD_LEVELUP_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMWorldMapCityData cityData = nKMUserData.m_WorldmapData.GetCityData(sPacket.cityID);
		if (cityData == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_CITY_ID);
			Debug.LogError("FATAL : City not exist. Client-Server data sync off");
			return;
		}
		cityData.UpdateBuildingData(sPacket.worldMapCityBuildingData);
		nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			int changedBuildingID = sPacket.worldMapCityBuildingData?.id ?? 0;
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().CityBuildingChanged(cityData, changedBuildingID);
		}
	}

	public static void OnRecv(NKMPacket_WORLDMAP_BUILD_EXPIRE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMWorldMapCityData cityData = nKMUserData.m_WorldmapData.GetCityData(sPacket.cityID);
		if (cityData == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_CITY_ID);
			Debug.LogError("FATAL : City not exist. Client-Server data sync off");
			return;
		}
		cityData.RemoveBuild(sPacket.buildID);
		nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.itemData);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().CityBuildingChanged(cityData);
		}
	}

	public static void OnRecv(NKMPacket_POST_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCMailManager.AddMail(sPacket.postDataList, sPacket.postCount);
		}
	}

	public static void OnRecv(NKMPacket_POST_LIST_NOT sPacket)
	{
		NKCMailManager.AddMail(sPacket.postDataList, sPacket.postCount);
	}

	public static void OnRecv(NKMPacket_POST_RECEIVE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) || (sPacket.postIndex == 0L && sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_POST_RECV_ITEM_FULL))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.GetReward(sPacket.rewardDate);
			NKCMailManager.OnPostReceive(sPacket);
			NKCUIResult.Instance.OpenMailResult(myUserData.m_ArmyData, sPacket.rewardDate);
		}
	}

	public static void OnRecv(NKMPacket_POST_ARRIVE_NOT sPacket)
	{
		NKCMailManager.OnNewMailNotify(sPacket.count);
	}

	public static void OnRecv(NKMPacket_SHIP_BUILD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(sPacket.costItemList);
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		if (armyData.IsFirstGetUnit(sPacket.shipData.m_UnitID))
		{
			NKCUIGameResultGetUnit.AddFirstGetUnit(sPacket.shipData.m_UnitID);
		}
		armyData.AddNewShip(sPacket.shipData);
		if (NKCUIHangarBuild.IsInstanceOpen)
		{
			NKCUIHangarBuild.Instance.UpdateUI(bSlotUpdate: true);
		}
		NKMRewardData nKMRewardData = new NKMRewardData();
		nKMRewardData.UnitDataList.Add(sPacket.shipData);
		if (NKCGameEventManager.IsWaiting())
		{
			NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentUserData().m_ArmyData, nKMRewardData, NKCUtilString.GET_STRING_HANGAR_BUILD, "", NKCGameEventManager.WaitFinished);
		}
		else
		{
			NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentUserData().m_ArmyData, nKMRewardData, NKCUtilString.GET_STRING_HANGAR_BUILD);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(sPacket.shipData.m_UnitID);
		if (unitTempletBase != null)
		{
			switch (unitTempletBase.m_NKM_UNIT_STYLE_TYPE)
			{
			case NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT:
				NKCUINPCHangarNaHeeRin.PlayVoice(NPC_TYPE.HANGAR_NAHEERIN, NPC_ACTION_TYPE.SHIP_GET_ASSAULT);
				break;
			case NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER:
				NKCUINPCHangarNaHeeRin.PlayVoice(NPC_TYPE.HANGAR_NAHEERIN, NPC_ACTION_TYPE.SHIP_GET_CRUISER);
				break;
			case NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY:
				NKCUINPCHangarNaHeeRin.PlayVoice(NPC_TYPE.HANGAR_NAHEERIN, NPC_ACTION_TYPE.SHIP_GET_HEAVY);
				break;
			case NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL:
				NKCUINPCHangarNaHeeRin.PlayVoice(NPC_TYPE.HANGAR_NAHEERIN, NPC_ACTION_TYPE.SHIP_GET_SPECIAL);
				break;
			}
		}
	}

	public static void OnRecv(NKMPacket_SHIP_LEVELUP_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_ArmyData.UpdateShipData(sPacket.shipUnitData);
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
		}
	}

	public static void OnRecv(NKMPacket_SHIP_DIVISION_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMArmyData armyData = myUserData.m_ArmyData;
			myUserData.m_InventoryData.AddItemMisc(sPacket.rewardItemDataList);
			armyData.RemoveShip(sPacket.removeShipUIDList);
			if (sPacket.rewardItemDataList.Count > 0)
			{
				NKCUIResult.Instance.OpenItemGain(sPacket.rewardItemDataList, NKCUtilString.GET_STRING_ITEM_GAIN, NKCUtilString.GET_STRING_REMOVE_SHIP);
			}
			if (NKCUIUnitSelectList.IsInstanceOpen)
			{
				NKCUIUnitSelectList.Instance.CloseRemoveMode();
				NKCUIUnitSelectList.Instance.ClearMultipleSelect();
			}
			NKCUINPCHangarNaHeeRin.PlayVoice(NPC_TYPE.HANGAR_NAHEERIN, NPC_ACTION_TYPE.SHIP_DIVISION);
		}
	}

	public static void OnRecv(NKMPacket_SHIP_UPGRADE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			NKMArmyData armyData = myUserData.m_ArmyData;
			armyData.UpdateShipData(sPacket.shipUnitData);
			armyData.TryCollectUnit(sPacket.shipUnitData.m_UnitID);
			if (NKCUIShipInfo.IsInstanceOpen)
			{
				NKCUIShipInfo.Instance.OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_LIMIT_BREAK_SHIP_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCUIGameResultGetUnit.Instance.Open(new NKCUIGameResultGetUnit.GetUnitResultData(sPacket.shipData), null, bEnableAutoSkip: false, bUseDefaultSort: false, bSkipDuplicateNormalUnit: false, NKCUIGameResultGetUnit.Type.Ship);
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			myUserData.m_ArmyData.RemoveShip(sPacket.consumeUnitUid);
			myUserData.m_ArmyData.UpdateShipData(sPacket.shipData);
		}
	}

	public static void OnRecv(NKMPacket_SHIP_SLOT_FIRST_OPTION_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.UpdateShipData(sPacket.shipData);
			if (NKCPopupShipCommandModule.IsInstanceOpen)
			{
				NKCPopupShipCommandModule.Instance.ShowModuleOpenFx();
			}
		}
	}

	public static void OnRecv(NKMPacket_SHIP_SLOT_LOCK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			myUserData.m_ArmyData.UpdateShipData(sPacket.shipData);
		}
	}

	public static void OnRecv(NKMPacket_SHIP_SLOT_OPTION_CHANGE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			myUserData.SetShipCandidateData(sPacket.candidateOption);
			if (NKCPopupShipCommandModule.IsInstanceOpen)
			{
				NKCPopupShipCommandModule.Instance.CandidateChanged();
			}
			myUserData.m_ArmyData.UpdateShipData(sPacket.shipData);
		}
	}

	public static void OnRecv(NKMPacket_SHIP_SLOT_OPTION_CONFIRM_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.SetShipCandidateData(new NKMShipModuleCandidate());
			myUserData.m_ArmyData.UpdateShipData(sPacket.shipData);
			if (NKCPopupShipCommandModule.IsInstanceOpen)
			{
				NKCPopupShipCommandModule.Instance.CandidateChanged();
			}
		}
	}

	public static void OnRecv(NKMPacket_SHIP_SLOT_OPTION_CANCEL_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData().SetShipCandidateData(new NKMShipModuleCandidate());
			if (NKCPopupShipCommandModule.IsInstanceOpen)
			{
				NKCPopupShipCommandModule.Instance.OnCandidateRemoved();
			}
		}
	}

	public static void OnRecv(NKMPacket_CRAFT_UNLOCK_SLOT_ACK cNKMPacket_CREATION_UNLOCK_SLOT_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_CREATION_UNLOCK_SLOT_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_CREATION_UNLOCK_SLOT_ACK.costItemDataList);
			myUserData.m_CraftData.AddSlotData(cNKMPacket_CREATION_UNLOCK_SLOT_ACK.craftSlotData);
			if (NKCUIForgeCraft.IsInstanceOpen)
			{
				NKCUIForgeCraft.Instance.ResetUI();
				NKCUIForgeCraft.Instance.OnRecvSlotOpen(cNKMPacket_CREATION_UNLOCK_SLOT_ACK.craftSlotData.Index);
			}
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_TUNING_REFINE_ACK cNKMPacket_EQUIP_TUNING_REFINE_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EQUIP_TUNING_REFINE_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMEquipItemData nKMEquipItemData = new NKMEquipItemData();
		int changedSlotNum = -1;
		NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(cNKMPacket_EQUIP_TUNING_REFINE_ACK.equipItemData.m_ItemUid);
		if (itemEquip != null)
		{
			for (int i = 1; i < itemEquip.m_Stat.Count && cNKMPacket_EQUIP_TUNING_REFINE_ACK.equipItemData.m_Stat.Count > i; i++)
			{
				if (itemEquip.m_Stat[i].stat_value != cNKMPacket_EQUIP_TUNING_REFINE_ACK.equipItemData.m_Stat[i].stat_value && ((i == 1 && cNKMPacket_EQUIP_TUNING_REFINE_ACK.equipItemData.m_Precision >= 100) || (i == 2 && cNKMPacket_EQUIP_TUNING_REFINE_ACK.equipItemData.m_Precision2 >= 100)))
				{
					changedSlotNum = i;
				}
			}
			nKMEquipItemData.DeepCopyFrom(itemEquip);
			itemEquip.DeepCopyFrom(cNKMPacket_EQUIP_TUNING_REFINE_ACK.equipItemData);
			myUserData.m_InventoryData.UpdateItemEquip(itemEquip);
		}
		myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_EQUIP_TUNING_REFINE_ACK.costItemDataList);
		if (NKCUIForge.IsInstanceOpen)
		{
			NKCUIForge.Instance.DoAfterRefine(nKMEquipItemData, changedSlotNum);
		}
		NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GetRefineResultMsg(cNKMPacket_EQUIP_TUNING_REFINE_ACK.equipRefineResult), NKCPopupMessage.eMessagePosition.Top, bShowFX: true);
		NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_TUNING, bStopCurrentSound: true);
	}

	public static void OnRecv(NKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK cNKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK.equipItemData.m_ItemUid);
			if (itemEquip != null)
			{
				itemEquip.DeepCopyFrom(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK.equipItemData);
				myUserData.m_InventoryData.UpdateItemEquip(itemEquip);
			}
			NKMItemManager.SetResetCount(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK.resetCount);
			myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK.costItemDataList);
			NKCScenManager.CurrentUserData().SetEquipTuningData(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK.equipTuningCandidate);
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.ResetUI();
				NKCUIForge.Instance.DoAfterOptionChanged(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK.equipOptionID);
			}
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_ACK cNKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_ACK.equipItemData.m_ItemUid);
			if (itemEquip != null)
			{
				itemEquip.DeepCopyFrom(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_ACK.equipItemData);
				myUserData.m_InventoryData.UpdateItemEquip(itemEquip);
			}
			NKCScenManager.CurrentUserData().SetEquipTuningData(cNKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_ACK.equipTuningCandidate);
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.ResetUI();
				NKCUIForge.Instance.DoAfterOptionChangedConfirm();
			}
			NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_TUNING, bStopCurrentSound: true);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMItemManager.SetResetCount(sPacket.resetCount);
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(sPacket.equipItemData.m_ItemUid);
			if (itemEquip != null)
			{
				itemEquip.DeepCopyFrom(sPacket.equipItemData);
				myUserData.m_InventoryData.UpdateItemEquip(itemEquip);
			}
			if (NKCUISelectionEquipDetail.IsInstanceOpen)
			{
				NKCUISelectionEquipDetail.Instance.Close();
			}
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.ResetUI();
				NKCUIForge.Instance.DoAfterOptionChangedConfirm();
			}
			NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_TUNING, bStopCurrentSound: true);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		if (inventoryData != null)
		{
			NKMItemManager.SetResetCount(sPacket.resetCount);
			NKMEquipItemData itemEquip = inventoryData.GetItemEquip(sPacket.equipUid);
			if (itemEquip != null)
			{
				itemEquip.m_SetOptionId = sPacket.setOptionId;
			}
			inventoryData.UpdateItemEquip(itemEquip);
			if (NKCUISelectionEquipDetail.IsInstanceOpen)
			{
				NKCUISelectionEquipDetail.Instance.Close();
			}
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.ResetUI();
				NKCUIForge.Instance.DoAfterSetOptionChangeConfirm();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_UNIT_LIST && NKCUIUnitInfo.IsInstanceOpen)
			{
				NKCUIUnitInfo.Instance.UpdateEquipSlots();
			}
			if (NKCUIInventory.IsInstanceOpen)
			{
				NKCUIInventory.Instance.ResetEquipSlotList();
			}
		}
	}

	public static void OnRecv(NKMPacket_CRAFT_INSTANT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPopupForgeCraft.CheckInstanceAndClose();
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(sPacket.moldId);
			if (itemMoldTempletByID != null && !itemMoldTempletByID.m_bPermanent)
			{
				NKCScenManager.GetScenManager().GetMyUserData().m_CraftData.DecMoldItem(sPacket.moldId, sPacket.moldCount);
			}
			NKMItemManager.SetResetCount(sPacket.resetCount);
			NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(sPacket.materialItemDataList);
			myUserData.GetReward(sPacket.createdRewardData);
			NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_CRAFT_COMPLETE, bStopCurrentSound: true);
			if (NKCUIForgeCraft.IsInstanceOpen)
			{
				NKCUIForgeCraft.Instance.ResetUI();
			}
			NKCUIResult.Instance.OpenComplexResult(myUserData.m_ArmyData, sPacket.createdRewardData, null, 0L, null, bIgnoreAutoClose: true);
		}
	}

	public static void OnRecv(NKMPacket_CRAFT_START_ACK cNKMPacket_CREATION_START_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_CREATION_START_ACK.errorCode))
		{
			NKCUIForgeCraftMold.CheckInstanceAndClose();
			NKCScenManager.GetScenManager().GetMyUserData().m_CraftData.UpdateSlotData(cNKMPacket_CREATION_START_ACK.craftSlotData);
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(cNKMPacket_CREATION_START_ACK.craftSlotData.MoldID);
			if (itemMoldTempletByID != null && !itemMoldTempletByID.m_bPermanent)
			{
				NKCScenManager.GetScenManager().GetMyUserData().m_CraftData.DecMoldItem(cNKMPacket_CREATION_START_ACK.craftSlotData.MoldID, cNKMPacket_CREATION_START_ACK.craftSlotData.Count);
			}
			NKMItemManager.SetResetCount(cNKMPacket_CREATION_START_ACK.resetCount);
			NKCPublisherModule.Push.UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP.CRAFT_COMPLETE);
			NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(cNKMPacket_CREATION_START_ACK.materialItemDataList);
			NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_CRAFT_START, bStopCurrentSound: true);
			if (NKCUIForgeCraft.IsInstanceOpen)
			{
				NKCUIForgeCraft.Instance.ResetUI();
			}
			if (NKCUIOfficeMapFront.IsInstanceOpen)
			{
				NKCUIOfficeMapFront.GetInstance().UpdateFactoryState();
			}
			NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.THANKS, bStopCurrentSound: true);
		}
	}

	public static void OnRecv(NKMPacket_CRAFT_COMPLETE_ACK cNKMPacket_CREATION_COMPLETE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_CREATION_COMPLETE_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData().m_CraftData.UpdateSlotData(cNKMPacket_CREATION_COMPLETE_ACK.craftSlotData);
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.GetReward(cNKMPacket_CREATION_COMPLETE_ACK.createdRewardData);
			NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_CRAFT_COMPLETE, bStopCurrentSound: true);
			if (NKCUIForgeCraft.IsInstanceOpen)
			{
				NKCUIForgeCraft.Instance.ResetUI();
			}
			if (NKCUIOfficeMapFront.IsInstanceOpen)
			{
				NKCUIOfficeMapFront.GetInstance().UpdateFactoryState();
			}
			NKCUIResult.Instance.OpenComplexResult(myUserData.m_ArmyData, cNKMPacket_CREATION_COMPLETE_ACK.createdRewardData, null, 0L, null, bIgnoreAutoClose: true);
		}
	}

	public static void OnRecv(NKMPacket_CRAFT_INSTANT_COMPLETE_ACK cNKMPacket_CREATION_INSTANT_COMPLETE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_CREATION_INSTANT_COMPLETE_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData().m_CraftData.UpdateSlotData(cNKMPacket_CREATION_INSTANT_COMPLETE_ACK.craftSlotData);
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.GetReward(cNKMPacket_CREATION_INSTANT_COMPLETE_ACK.createdRewardData);
			myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_CREATION_INSTANT_COMPLETE_ACK.extraCostItemData);
			NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_CRAFT_COMPLETE, bStopCurrentSound: true);
			if (NKCUIForgeCraft.IsInstanceOpen)
			{
				NKCUIForgeCraft.Instance.ResetUI();
			}
			if (NKCUIOfficeMapFront.IsInstanceOpen)
			{
				NKCUIOfficeMapFront.GetInstance().UpdateFactoryState();
			}
			NKCUIResult.Instance.OpenComplexResult(myUserData.m_ArmyData, cNKMPacket_CREATION_INSTANT_COMPLETE_ACK.createdRewardData, null, 0L, null, bIgnoreAutoClose: true);
			NKCPublisherModule.Push.UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP.CRAFT_COMPLETE, bForce: true);
		}
	}

	public static void OnRecv(NKMPacket_REMOVE_EQUIP_ITEM_ACK cNKMPacket_REMOVE_EQUIP_ITEM_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_REMOVE_EQUIP_ITEM_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.m_InventoryData.RemoveItemEquip(cNKMPacket_REMOVE_EQUIP_ITEM_ACK.removeEquipItemUIDList);
		NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_RECYCLE, bStopCurrentSound: false);
		if (cNKMPacket_REMOVE_EQUIP_ITEM_ACK.rewardItemDataList.Count > 0)
		{
			myUserData.m_InventoryData.AddItemMisc(cNKMPacket_REMOVE_EQUIP_ITEM_ACK.rewardItemDataList);
			Log.Info("OpenItemGain", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketHandlersLobby.cs", 7302);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_INVENTORY || NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OFFICE)
			{
				NKCScenManager.GetScenManager().Get_SCEN_INVENTORY().OnRemoveEquipItemAck();
			}
			else
			{
				NKCUIInventory.CheckInstanceAndClose();
			}
			NKCUIResult.Instance.OpenItemGain(cNKMPacket_REMOVE_EQUIP_ITEM_ACK.rewardItemDataList, NKCUtilString.GET_STRING_ITEM_GAIN, NKCUtilString.GET_STRING_EQUIP_BREAK_UP);
		}
	}

	public static void OnRecv(NKMPAcket_EQUIP_ITEM_ENCHANT_ACK cNKMPAcket_EQUIP_ITEM_ENCHANT_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPAcket_EQUIP_ITEM_ENCHANT_ACK.errorCode))
		{
			NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(cNKMPAcket_EQUIP_ITEM_ENCHANT_ACK.equipItemUID);
			itemEquip.m_EnchantLevel = cNKMPAcket_EQUIP_ITEM_ENCHANT_ACK.enchantLevel;
			itemEquip.m_EnchantExp = cNKMPAcket_EQUIP_ITEM_ENCHANT_ACK.enchantExp;
			NKCScenManager.CurrentUserData().m_InventoryData.UpdateItemEquip(itemEquip);
			for (int i = 0; i < cNKMPAcket_EQUIP_ITEM_ENCHANT_ACK.consumeEquipItemUIDList.Count; i++)
			{
				NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.RemoveItemEquip(cNKMPAcket_EQUIP_ITEM_ENCHANT_ACK.consumeEquipItemUIDList[i]);
			}
			NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(cNKMPAcket_EQUIP_ITEM_ENCHANT_ACK.costItemDataList);
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.PlayEnhanceEffect();
			}
			if (NKCUIInventory.IsInstanceOpen)
			{
				NKCUIInventory.Instance.ResetEquipSlotList();
			}
			NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_ENHANCE, bStopCurrentSound: false);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(sPacket.equipItemUID);
			itemEquip.m_EnchantLevel = sPacket.enchantLevel;
			itemEquip.m_EnchantExp = sPacket.enchantExp;
			NKCScenManager.CurrentUserData().m_InventoryData.UpdateItemEquip(itemEquip);
			NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.PlayEnhanceEffect();
			}
			if (NKCUIInventory.IsInstanceOpen)
			{
				NKCUIInventory.Instance.ResetEquipSlotList();
			}
			NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_ENHANCE, bStopCurrentSound: false);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_ITEM_EQUIP_ACK cNKMPacket_EQUIP_ITEM_EQUIP_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EQUIP_ITEM_EQUIP_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(cNKMPacket_EQUIP_ITEM_EQUIP_ACK.unitUID);
		NKMUnitData nKMUnitData = null;
		if (unitFromUID != null)
		{
			if (cNKMPacket_EQUIP_ITEM_EQUIP_ACK.unequipItemUID > 0)
			{
				NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(cNKMPacket_EQUIP_ITEM_EQUIP_ACK.unequipItemUID);
				if (itemEquip != null && unitFromUID.m_UnitUID != itemEquip.m_OwnerUnitUID)
				{
					nKMUnitData = myUserData.m_ArmyData.GetUnitFromUID(itemEquip.m_OwnerUnitUID);
				}
				if (!unitFromUID.UnEquipItem(myUserData.m_InventoryData, cNKMPacket_EQUIP_ITEM_EQUIP_ACK.unequipItemUID, cNKMPacket_EQUIP_ITEM_EQUIP_ACK.equipPosition))
				{
					Debug.LogError("UnitData.UnEquipItem failed");
				}
				NKMEquipItemData itemEquip2 = myUserData.m_InventoryData.GetItemEquip(cNKMPacket_EQUIP_ITEM_EQUIP_ACK.unequipItemUID);
				myUserData.m_InventoryData.UpdateItemEquip(itemEquip2);
				if (NKCUIInventory.IsInstanceOpen)
				{
					if (NKCUIInventory.Instance.GetNKCUIInventoryOption().m_dOnClickEmptySlot != null)
					{
						NKCUIInventory.Instance.Close();
					}
					else
					{
						NKCUIInventory.Instance.UpdateEquipSlot(cNKMPacket_EQUIP_ITEM_EQUIP_ACK.unequipItemUID);
					}
				}
			}
			if (cNKMPacket_EQUIP_ITEM_EQUIP_ACK.equipItemUID > 0)
			{
				long unequip_item_uid = 0L;
				if (!unitFromUID.EquipItem(myUserData.m_InventoryData, cNKMPacket_EQUIP_ITEM_EQUIP_ACK.equipItemUID, out unequip_item_uid, cNKMPacket_EQUIP_ITEM_EQUIP_ACK.equipPosition))
				{
					Debug.LogError("UnitData.EquipItem failed");
				}
				NKMEquipItemData itemEquip3 = myUserData.m_InventoryData.GetItemEquip(cNKMPacket_EQUIP_ITEM_EQUIP_ACK.equipItemUID);
				myUserData.m_InventoryData.UpdateItemEquip(itemEquip3);
				if (NKCUIUnitSelectList.IsInstanceOpen && !NKCUIUnitInfo.IsInstanceOpen)
				{
					NKCUIUnitSelectList.Instance.Close();
					if (NKCUIInventory.IsInstanceOpen)
					{
						NKCUIInventory.Instance.UpdateEquipSlot(cNKMPacket_EQUIP_ITEM_EQUIP_ACK.equipItemUID);
					}
				}
				else if (NKCUIInventory.IsInstanceOpen)
				{
					NKCUIInventory.Instance.Close();
				}
				NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_GROWTH_EQUIP, unitFromUID);
			}
		}
		if (nKMUnitData != null)
		{
			myUserData.m_ArmyData.UpdateUnitData(nKMUnitData);
		}
		myUserData.m_ArmyData.UpdateUnitData(unitFromUID);
		NKCPopupItemEquipBox.CloseItemBox();
		if (NKCUIUnitInfo.IsInstanceOpen)
		{
			NKCUIUnitInfo.Instance.EquipPreset?.UpdatePresetData(null, setScrollPositon: false);
		}
		if (NKCRecallManager.m_bWaitingRecallProcess)
		{
			NKCRecallManager.OnUnequipComplete();
		}
		if (NKCUITacticUpdate.IsInstanceOpen)
		{
			NKCUITacticUpdate.Instance.OnUnEquipComplete();
		}
	}

	public static void OnRecv(NKMPacket_LOCK_EQUIP_ITEM_ACK cNKMPacket_LOCK_EQUIP_ITEM_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_LOCK_EQUIP_ITEM_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(cNKMPacket_LOCK_EQUIP_ITEM_ACK.equipItemUID);
			itemEquip.m_bLock = cNKMPacket_LOCK_EQUIP_ITEM_ACK.isLock;
			if (NKCUIInventory.IsInstanceOpen)
			{
				NKCUIInventory.Instance.UpdateEquipSlot(cNKMPacket_LOCK_EQUIP_ITEM_ACK.equipItemUID);
			}
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.ResetUI();
			}
			myUserData.m_InventoryData.UpdateItemEquip(itemEquip);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_UPGRADE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			myUserData.m_InventoryData.RemoveItemEquip(sPacket.consumeEquipItemUidList);
			myUserData.m_InventoryData.UpdateItemEquip(sPacket.equipItemData);
			if (NKCUIForgeUpgrade.IsInstanceOpen)
			{
				NKCUIForgeUpgrade.Instance.UpgradeFinished(sPacket.equipItemData);
			}
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_OPEN_SOCKET_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			myUserData.m_InventoryData.UpdateItemEquip(sPacket.equipItemData);
		}
		if (!NKCUIForge.IsInstanceOpen || !(NKCUIForge.Instance.m_NKCUIForgeHiddenOption != null))
		{
			return;
		}
		NKCUIForge.Instance.m_NKCUIForgeHiddenOption.SetUI();
		if (sPacket.equipItemData.potentialOptions.Count <= 0)
		{
			return;
		}
		int socketIndex = -1;
		int num = sPacket.equipItemData.potentialOptions[0].sockets.Length;
		for (int i = 0; i < num; i++)
		{
			if (sPacket.equipItemData.potentialOptions[0].sockets[i] != null)
			{
				socketIndex = i;
			}
		}
		NKCUIForge.Instance.m_NKCUIForgeHiddenOption.UnlockingSocket = true;
		NKCUIForge.Instance.m_NKCUIForgeHiddenOption.ActivateUnlockFx(socketIndex, delegate
		{
			NKCUIForge.Instance.ResetUI();
		});
	}

	public static void OnRecv(NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		if (inventoryData != null)
		{
			inventoryData.UpdateItemInfo(sPacket.costItemDataList);
			NKCScenManager.CurrentUserData().SetEquipPotentialData(sPacket.potentialOptionCandidate);
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.m_NKCUIForgeHiddenOption.SetUI();
				NKCUIForge.Instance.DoAfterPotentialChanged();
			}
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CONFIRM_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(sPacket.equipItemData.m_ItemUid);
			if (itemEquip != null)
			{
				itemEquip.DeepCopyFrom(sPacket.equipItemData);
				myUserData.m_InventoryData.UpdateItemEquip(itemEquip);
			}
			NKCScenManager.CurrentUserData().SetEquipPotentialData(new NKMPotentialOptionChangeCandidate());
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.ResetUI();
				NKCUIForge.Instance.DoAfterPotentialChangedConfirm();
			}
			NKCUINPCFactoryAnastasia.PlayVoice(NPC_TYPE.FACTORY_ANASTASIA, NPC_ACTION_TYPE.ITEM_TUNING, bStopCurrentSound: true);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().m_InventoryData.UpdateItemEquip(sPacket.equipItemData);
			NKCScenManager.CurrentUserData().SetEquipPotentialData(new NKMPotentialOptionChangeCandidate());
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.ResetUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_RESET_GROUP_COUNT_NOT sPacket)
	{
		NKMItemManager.SetResetCount(sPacket.resetCountList);
	}

	public static void OnRecv(NKMPacket_RANDOM_ITEM_BOX_OPEN_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCGameEventManager.IsEventPlaying())
			{
				NKCGameEventManager.CollectResultData(null);
			}
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKCUIGameResultGetUnit.AddFirstGetUnit(sPacket.rewardData);
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
			myUserData.GetReward(sPacket.rewardData);
			if (NKCGameEventManager.RandomBoxDataCollecting)
			{
				NKCGameEventManager.CollectResultData(sPacket.rewardData);
			}
			else
			{
				NKCUIResult.Instance.OpenBoxGain(myUserData.m_ArmyData, sPacket.rewardData, sPacket.costItemData.ItemID);
			}
		}
	}

	public static void OnRecv(NKMPacket_CHOICE_ITEM_USE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCGameEventManager.IsEventPlaying())
			{
				NKCGameEventManager.CollectResultData(null);
			}
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
		myUserData.GetReward(sPacket.rewardData);
		if (NKCUISelection.IsInstanceOpen)
		{
			NKCUISelection.Instance.Close();
		}
		if (NKCUISelectionEquip.IsInstanceOpen)
		{
			NKCUISelectionEquip.Instance.Close();
		}
		if (NKCUISelectionMisc.IsInstanceOpen)
		{
			NKCUISelectionMisc.Instance.Close();
		}
		if (NKCUISelectionSkin.IsInstanceOpen)
		{
			NKCUISelectionSkin.Instance.Close();
		}
		NKCUISelectionOperator.CheckInstanceAndClose();
		NKCUIResult.Instance.OpenComplexResult(NKCScenManager.CurrentUserData().m_ArmyData, sPacket.rewardData, null, 0L);
	}

	public static void OnRecv(NKMPacket_MISC_CONTRACT_OPEN_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null || sPacket.result == null)
		{
			return;
		}
		int boxItemID = 0;
		foreach (MiscContractResult item in sPacket.result)
		{
			if (item == null)
			{
				continue;
			}
			boxItemID = item.miscItemId;
			foreach (NKMUnitData unit in item.units)
			{
				if (unit != null)
				{
					NKCUIGameResultGetUnit.AddFirstGetUnit(unit.m_UnitID);
				}
			}
		}
		myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItems);
		NKMRewardData nKMRewardData = new NKMRewardData();
		nKMRewardData.contractList = new List<MiscContractResult>(sPacket.result);
		myUserData.GetReward(nKMRewardData);
		NKCUIResult.Instance.OpenBoxGain(myUserData.m_ArmyData, nKMRewardData, boxItemID);
	}

	public static void OnRecv(NKMPacket_ZLONG_PAYMENT_NOTIFY sPacket)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME()?.UnhideLobbyUI();
		}
		CommonProcessBuy(sPacket.rewardData, sPacket.productID, sPacket.history, sPacket.subScriptionData);
		if (sPacket.totalPaidAmount > 0.0)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null && myUserData.m_ShopData != null)
			{
				myUserData.m_ShopData.SetTotalPayment(sPacket.totalPaidAmount);
			}
		}
		if (NKCUIEvent.IsInstanceOpen)
		{
			NKCUIEvent.Instance.RefreshUI();
		}
	}

	public static void OnRecv(NKMPacket_SHOP_FIXED_LIST_ACK sPacket)
	{
		if (sPacket.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			NKCShopManager.SetShopItemList(sPacket.shopList, sPacket.InstantProductList);
		}
		else
		{
			Debug.LogError("Server Error Code : " + sPacket.errorCode);
		}
	}

	private static void CommonProcessBuy(NKMRewardData rewardData, int productID, NKMShopPurchaseHistory history, NKMShopSubscriptionData subScriptionData)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		myUserData.GetReward(rewardData);
		if (subScriptionData != null)
		{
			if (myUserData.m_ShopData.subscriptions.ContainsKey(subScriptionData.productId))
			{
				myUserData.m_ShopData.subscriptions[subScriptionData.productId].endDate = subScriptionData.endDate;
			}
			else
			{
				subScriptionData.startDate = subScriptionData.startDate.AddMinutes(-1.0);
				myUserData.m_ShopData.subscriptions.Add(subScriptionData.productId, subScriptionData);
			}
		}
		if (history != null)
		{
			myUserData.m_ShopData.histories[history.shopId] = history;
		}
		ShopItemTemplet productTemplet = ShopItemTemplet.Find(productID);
		if (productTemplet.m_PriceItemID == 0)
		{
			NKCPublisherModule.Statistics.TrackPurchase(productTemplet.m_ProductID);
		}
		NKCMMPManager.OnTrackPurchase(productTemplet);
		foreach (NKCUIShop item in NKCUIManager.GetOpenedUIsByType<NKCUIShop>())
		{
			item?.RefreshShopItem(productID);
			if (history != null)
			{
				item?.RefreshShopItem(history.shopId);
			}
			item?.RefreshShopRedDot();
			item?.OnProductBuy(productTemplet);
		}
		if (NKCPopupPointExchange.IsInstanceOpen)
		{
			NKCPopupPointExchange.Instance.RefreshProduct();
		}
		if (rewardData != null)
		{
			if (rewardData.CompanyBuffDataList.Count > 0)
			{
				for (int i = 0; i < rewardData.CompanyBuffDataList.Count; i++)
				{
					NKCCompanyBuff.UpsertCompanyBuffData(myUserData.m_companyBuffDataList, rewardData.CompanyBuffDataList[i]);
				}
			}
			if (NKCShopManager.GetBundleCount() > 0)
			{
				NKCShopManager.RemoveBundleItemId(productID, rewardData);
			}
			else if (productTemplet.m_ItemType == NKM_REWARD_TYPE.RT_UNIT || productTemplet.m_ItemType == NKM_REWARD_TYPE.RT_OPERATOR)
			{
				NKCUIGameResultGetUnit.ShowNewUnitGetUI(rewardData, null);
			}
			else if (NKCShopManager.IsPackageItem(productID) || NKCShopManager.IsCustomPackageItem(productID))
			{
				NKCUIResult.Instance.OpenBoxGain(myUserData.m_ArmyData, new List<NKMRewardData> { rewardData }, productTemplet.GetItemName(), "", null, bDisplayUnitGet: false);
			}
			else
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GetShopItemBuyMessage(productTemplet), NKCPopupMessage.eMessagePosition.Top, bShowFX: true);
			}
		}
		else
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GetShopItemBuyMessage(productTemplet, bItemToMail: true), NKCPopupMessage.eMessagePosition.Top, bShowFX: true);
		}
		if (productTemplet.m_ItemType != NKM_REWARD_TYPE.RT_SKIN)
		{
			return;
		}
		if (NKCUIShopSkinPopup.IsInstanceOpen)
		{
			NKCUIShopSkinPopup.Instance.OnSkinBuy(productTemplet.m_ItemID);
		}
		if (rewardData == null)
		{
			return;
		}
		NKCUIGameResultGetUnit.ShowNewSkinGetUI(rewardData.SkinIdList, delegate
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(productTemplet.m_ItemID);
			if (skinTemplet != null && !string.IsNullOrEmpty(skinTemplet.m_CutscenePurchase))
			{
				NKCUICutScenPlayer.Instance.LoadAndPlay(skinTemplet.m_CutscenePurchase, 0);
			}
		});
	}

	public static void OnRecv(NKMPacket_SHOP_FIX_SHOP_BUY_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		Log.Debug($"[Inapp] NKMPacket_SHOP_FIX_SHOP_BUY_ACK - productID[{sPacket.productID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketHandlersLobby.cs", 7871);
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCPopupShopBuyShortcut.IsInstanceOpen)
			{
				NKCPopupShopBuyShortcut.Instance.Close();
			}
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			CommonProcessBuy(sPacket.rewardData, sPacket.productID, sPacket.histroy, sPacket.subScriptionData);
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
			if (sPacket.totalPaidAmount > 0.0)
			{
				myUserData.m_ShopData.SetTotalPayment(sPacket.totalPaidAmount);
			}
			if (NKCUIPopupTrimDungeon.IsInstanceOpen)
			{
				NKCUIPopupTrimDungeon.Instance.RefreshUI(resetLevelTab: false);
			}
			if (NKCPopupShopBannerNotice.IsInstanceOpen)
			{
				NKCPopupShopBannerNotice.Instance.RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_GAMEBASE_BUY_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCPopupShopBuyShortcut.IsInstanceOpen)
			{
				NKCPopupShopBuyShortcut.Instance.Close();
			}
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			CommonProcessBuy(sPacket.rewardData, sPacket.productId, sPacket.histroy, sPacket.subScriptionData);
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
			if (sPacket.totalPaidAmount > 0.0)
			{
				myUserData.m_ShopData.SetTotalPayment(sPacket.totalPaidAmount);
			}
		}
	}

	public static void OnRecv(NKMPacket_SHOP_REFRESH_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.m_ShopData.randomShop = sPacket.randomShopData;
		myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
		foreach (NKCUIShop item in NKCUIManager.GetOpenedUIsByType<NKCUIShop>())
		{
			item?.RandomShopItemUpdateComplete();
		}
	}

	public static void OnRecv(NKMPacket_SHOP_CHAIN_TAB_RESET_TIME_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.GetScenManager().GetMyUserData().m_ShopData.SetChainTabResetData(sPacket.list);
		foreach (NKCUIShop item in NKCUIManager.GetOpenedUIsByType<NKCUIShop>())
		{
			item?.ChainRefreshComplete(sPacket.list);
		}
	}

	public static void OnRecv(NKMPacket_SHOP_RANDOM_SHOP_BUY_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (!myUserData.m_ShopData.randomShop.datas.TryGetValue(sPacket.slotIndex, out var value))
		{
			Debug.LogError("Bad random shop index from server");
		}
		value.isBuy = true;
		myUserData.GetReward(sPacket.rewardData);
		myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
		if (value.itemType == NKM_REWARD_TYPE.RT_UNIT || value.itemType == NKM_REWARD_TYPE.RT_OPERATOR)
		{
			NKCUIGameResultGetUnit.ShowNewUnitGetUI(sPacket.rewardData, null);
		}
		else
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GetShopItemBuyMessage(value), NKCPopupMessage.eMessagePosition.Top, bShowFX: true);
		}
		foreach (NKCUIShop item in NKCUIManager.GetOpenedUIsByType<NKCUIShop>())
		{
			item.RefreshRandomShopItem(sPacket.slotIndex);
			item.OnProductBuy(null);
		}
	}

	public static void OnRecv(NKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMShopRandomData randomShop = myUserData.m_ShopData.randomShop;
		myUserData.GetReward(sPacket.rewardData);
		myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDatas);
		NKCPopupMessageToastSimple.Instance.Open(sPacket.rewardData, null);
		foreach (int slotIndex in sPacket.slotIndexes)
		{
			if (!randomShop.datas.TryGetValue(slotIndex, out var value))
			{
				Debug.LogError("Bad random shop index from server");
			}
			value.isBuy = true;
		}
		foreach (NKCUIShop item in NKCUIManager.GetOpenedUIsByType<NKCUIShop>())
		{
			item.ClearMultibuySelection();
			item.RefreshRandomShopItem(-1);
			item.OnProductBuy(null);
		}
	}

	public static void OnRecv(NKMPacket_FIRST_CASH_PURCHASE_NOT sPacket)
	{
		NKCMMPManager.OnCustomEvent("00_first_purchase");
	}

	public static void OnRecv(NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_ACK sPacket)
	{
		NKMShopData shopData = NKCScenManager.CurrentUserData().m_ShopData;
		if (sPacket == null)
		{
			return;
		}
		if (sPacket.histroy != null)
		{
			if (shopData.histories.ContainsKey(sPacket.histroy.shopId))
			{
				shopData.histories[sPacket.histroy.shopId] = sPacket.histroy;
			}
			else
			{
				shopData.histories.Add(sPacket.histroy.shopId, sPacket.histroy);
			}
		}
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_SHOP().OnRecvProductBuyCheck(sPacket.productMarketID, sPacket.selectIndices);
			if (NKCPopupShopBannerNotice.IsInstanceOpen)
			{
				NKCPopupShopBannerNotice.Instance.RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_CONSUMER_PACKAGE_UPDATED_NOT sPacket)
	{
		NKCScenManager.CurrentUserData().UpdateConsumerPackageData(sPacket.list);
	}

	public static void OnRecv(NKMPacket_CONSUMER_PACKAGE_REMOVED_NOT sPacket)
	{
		NKCScenManager.CurrentUserData().RemoveConsumerPackageData(sPacket.productIds);
	}

	private static void ProcessWorldmapContentsAfterDiveEnd()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null || myUserData.m_DiveGameData == null || !myUserData.m_DiveGameData.Floor.Templet.IsEventDive)
		{
			return;
		}
		if (myUserData.m_DiveGameData.Player.PlayerBase.State == NKMDivePlayerState.Clear)
		{
			myUserData.m_WorldmapData.RemoveEvent(NKM_WORLDMAP_EVENT_TYPE.WET_DIVE, myUserData.m_DiveGameData.DiveUid, out var _);
		}
		else if (myUserData.m_DiveGameData.Player.PlayerBase.State == NKMDivePlayerState.Annihilation)
		{
			int cityID2 = myUserData.m_WorldmapData.GetCityID(myUserData.m_DiveGameData.DiveUid);
			NKMWorldMapCityData cityData = myUserData.m_WorldmapData.GetCityData(cityID2);
			if (cityData != null)
			{
				cityData.worldMapEventGroup.eventUid = 0L;
			}
		}
	}

	public static void OnRecv(NKMPacket_DIVE_SUICIDE_ACK cNKMPacket_DIVE_SUICIDE_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_DIVE_SUICIDE_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null && myUserData.m_DiveGameData != null)
		{
			NKCDiveGame.SetReservedUnitDieShow(bSet: true, myUserData.m_DiveGameData.Player.PlayerBase.ReservedDeckIndex, NKC_DIVE_GAME_UNIT_DIE_TYPE.NDGUDT_WARP);
			myUserData.m_DiveGameData.UpdateData(isWin: false, cNKMPacket_DIVE_SUICIDE_ACK.diveSyncData);
			if (myUserData.m_DiveGameData.Player.PlayerBase.State == NKMDivePlayerState.Annihilation)
			{
				NKCDiveGame.SetReservedUnitDieShow(bSet: false);
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE_RESULT()?.SetData(bDiveClear: false, myUserData.m_DiveGameData.Floor.Templet.IsEventDive, cNKMPacket_DIVE_SUICIDE_ACK.diveSyncData.RewardData, cNKMPacket_DIVE_SUICIDE_ACK.diveSyncData.ArtifactRewardData, cNKMPacket_DIVE_SUICIDE_ACK.diveSyncData.StormMiscReward, null, new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, myUserData.m_DiveGameData.Player.PlayerBase.LeaderDeckIndex), null);
				ProcessWorldmapContentsAfterDiveEnd();
				myUserData.ClearDiveGameData();
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DIVE_RESULT);
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_DIVE)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().OnRecv(cNKMPacket_DIVE_SUICIDE_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_DIVE_SELECT_ARTIFACT_ACK cNKMPacket_DIVE_SELECT_ARTIFACT_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_DIVE_SELECT_ARTIFACT_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null && myUserData.m_DiveGameData != null)
		{
			myUserData.m_DiveGameData.UpdateData(isWin: false, cNKMPacket_DIVE_SELECT_ARTIFACT_ACK.diveSyncData);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_DIVE)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().OnRecv(cNKMPacket_DIVE_SELECT_ARTIFACT_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_DIVE_START_ACK cNKMPacket_DIVE_START_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_DIVE_START_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.m_DiveGameData = cNKMPacket_DIVE_START_ACK.diveGameData;
		myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_DIVE_START_ACK.costItemDataList);
		if (cNKMPacket_DIVE_START_ACK.cityID != 0)
		{
			NKMWorldMapCityData cityData = myUserData.m_WorldmapData.GetCityData(cNKMPacket_DIVE_START_ACK.cityID);
			if (cityData != null)
			{
				cityData.worldMapEventGroup.eventUid = cNKMPacket_DIVE_START_ACK.diveGameData.DiveUid;
				NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(cityData.worldMapEventGroup.worldmapEventID);
				if (nKMWorldMapEventTemplet.eventType != NKM_WORLDMAP_EVENT_TYPE.WET_DIVE)
				{
					Debug.LogError("FATAL : Dive event가 없는 도시의 dive에 진입 성공. 서버 로직 에러");
				}
				if (nKMWorldMapEventTemplet.stageID != cNKMPacket_DIVE_START_ACK.diveGameData.Floor.Templet.StageID)
				{
					Debug.LogError("FATAL : Dive event가 지정하는 dive stage와 다른 dive에 진입. 서버 로직 에러");
				}
			}
		}
		NKMDiveGameManager.UpdateAllDiveDeckState(NKM_DECK_STATE.DECK_STATE_DIVE, myUserData);
		NKCDiveGame.SetReservedUnitDieShow(bSet: false);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().SetIntro();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().SetSectorAddEventWhenStart();
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DIVE);
	}

	public static void OnRecv(NKMPacket_DIVE_MOVE_FORWARD_ACK cNKMPacket_DIVE_MOVE_FORWARD_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_DIVE_MOVE_FORWARD_ACK.errorCode))
		{
			return;
		}
		if (cNKMPacket_DIVE_MOVE_FORWARD_ACK.diveSyncData != null && cNKMPacket_DIVE_MOVE_FORWARD_ACK.diveSyncData.RewardData != null)
		{
			NKCScenManager.GetScenManager().GetMyUserData()?.GetReward(cNKMPacket_DIVE_MOVE_FORWARD_ACK.diveSyncData.RewardData);
		}
		if (cNKMPacket_DIVE_MOVE_FORWARD_ACK.diveSyncData == null)
		{
			return;
		}
		NKMDiveGameData nKMDiveGameData = NKCScenManager.CurrentUserData()?.m_DiveGameData;
		if (nKMDiveGameData == null)
		{
			return;
		}
		NKMDiveSyncData diveSyncData = cNKMPacket_DIVE_MOVE_FORWARD_ACK.diveSyncData;
		for (int i = 0; i < diveSyncData.UpdatedSlots.Count; i++)
		{
			NKMDiveSlotWithIndexes nKMDiveSlotWithIndexes = diveSyncData.UpdatedSlots[i];
			if (nKMDiveSlotWithIndexes != null)
			{
				nKMDiveGameData.Floor.GetSlot(nKMDiveSlotWithIndexes.SlotSetIndex, nKMDiveSlotWithIndexes.SlotIndex)?.DeepCopyFrom(nKMDiveSlotWithIndexes.Slot);
			}
		}
		if (diveSyncData.UpdatedPlayer != null && (diveSyncData.UpdatedPlayer.State == NKMDivePlayerState.Exploring || diveSyncData.UpdatedPlayer.State == NKMDivePlayerState.SelectArtifact))
		{
			nKMDiveGameData.Floor.Rebuild(nKMDiveGameData.Player.PlayerBase.Distance, nKMDiveGameData.Player.GetNextSlotSetIndex(), diveSyncData.UpdatedPlayer.SlotIndex);
		}
		if (diveSyncData.UpdatedPlayer != null)
		{
			nKMDiveGameData.Player.PlayerBase.DeepCopyFromSource(diveSyncData.UpdatedPlayer);
		}
		if (diveSyncData.UpdatedSquads.Count > 0)
		{
			for (int j = 0; j < diveSyncData.UpdatedSquads.Count; j++)
			{
				NKMDiveSquad nKMDiveSquad = diveSyncData.UpdatedSquads[j];
				if (nKMDiveSquad != null)
				{
					NKMDiveSquad value = null;
					nKMDiveGameData.Player.Squads.TryGetValue(nKMDiveSquad.DeckIndex, out value);
					value?.DeepCopyFromSource(nKMDiveSquad);
				}
			}
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_DIVE)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().OnRecv(cNKMPacket_DIVE_MOVE_FORWARD_ACK);
		}
	}

	public static void OnRecv(NKMPacket_DIVE_GIVE_UP_ACK cNKMPacket_DIVE_GIVE_UP_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_DIVE_GIVE_UP_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		bool bEventDive = false;
		if (myUserData != null)
		{
			if (myUserData.m_DiveGameData != null && myUserData.m_DiveGameData.Floor.Templet.IsEventDive)
			{
				bEventDive = true;
				int cityID = myUserData.m_WorldmapData.GetCityID(myUserData.m_DiveGameData.DiveUid);
				NKMWorldMapCityData cityData = myUserData.m_WorldmapData.GetCityData(cityID);
				if (cityData != null)
				{
					cityData.worldMapEventGroup.eventUid = 0L;
				}
			}
			myUserData.ClearDiveGameData();
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_DIVE)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().OnRecv(cNKMPacket_DIVE_GIVE_UP_ACK, bEventDive);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_DIVE_READY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE_READY().OnRecv(cNKMPacket_DIVE_GIVE_UP_ACK);
		}
	}

	public static void OnRecv(NKMPacket_DIVE_AUTO_ACK cNKMPacket_DIVE_AUTO_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_DIVE_AUTO_ACK.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null && myUserData.m_UserOption != null)
			{
				myUserData.m_UserOption.m_bAutoDive = cNKMPacket_DIVE_AUTO_ACK.isAuto;
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_DIVE)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().OnRecv(cNKMPacket_DIVE_AUTO_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_DIVE_EXPIRE_NOT cNKMPacket_DIVE_EXPIRE_NOT)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMDiveGameData diveGameData = myUserData.m_DiveGameData;
		if (diveGameData != null && diveGameData.Floor.Templet.StageID == cNKMPacket_DIVE_EXPIRE_NOT.stageID)
		{
			myUserData.ClearDiveGameData();
			NKCUtil.ProcessDiveExpireTime();
			switch (NKCScenManager.GetScenManager().GetNowScenID())
			{
			case NKM_SCEN_ID.NSI_HOME:
				NKCScenManager.GetScenManager().Get_SCEN_HOME().OnRecv(cNKMPacket_DIVE_EXPIRE_NOT);
				break;
			case NKM_SCEN_ID.NSI_DIVE_READY:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE_READY().OnRecv(cNKMPacket_DIVE_EXPIRE_NOT);
				break;
			case NKM_SCEN_ID.NSI_DIVE:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().OnRecv(cNKMPacket_DIVE_EXPIRE_NOT);
				break;
			case NKM_SCEN_ID.NSI_WORLDMAP:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(cNKMPacket_DIVE_EXPIRE_NOT);
				break;
			}
		}
	}

	public static void OnRecv(NKMPacket_DIVE_SKIP_ACK cNKMPacket_DIVE_SKIP_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_DIVE_SKIP_ACK.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData()?.m_WorldmapData.ClearEvent(cNKMPacket_DIVE_SKIP_ACK.deletedEventCityId);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			if (cNKMPacket_DIVE_SKIP_ACK.rewardDatas != null)
			{
				foreach (NKMRewardData rewardData in cNKMPacket_DIVE_SKIP_ACK.rewardDatas)
				{
					myUserData.GetReward(rewardData);
				}
			}
			myUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_DIVE_SKIP_ACK.costItems);
		}
		NKCPopupOpSkipProcess.Instance.Open(cNKMPacket_DIVE_SKIP_ACK.rewardDatas, null, NKCUtilString.GET_STRING_DIVE_SAFE_MINING_RESULT);
	}

	public static void OnRecv(NKMPacket_RAID_RESULT_LIST_ACK cNKMPacket_RAID_RESULT_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_RAID_RESULT_LIST_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetNKCRaidDataMgr().SetDataList(cNKMPacket_RAID_RESULT_LIST_ACK.raidResultDataList);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(cNKMPacket_RAID_RESULT_LIST_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_RAID_COOP_LIST_ACK cNKMPacket_RAID_COOP_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_RAID_COOP_LIST_ACK.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(cNKMPacket_RAID_COOP_LIST_ACK);
		}
	}

	public static void OnRecv(NKMPacket_RAID_SET_COOP_ACK cNKMPacket_RAID_SET_COOP_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_RAID_SET_COOP_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetNKCRaidDataMgr().SetRaidCoopOn(cNKMPacket_RAID_SET_COOP_ACK.raidUID, cNKMPacket_RAID_SET_COOP_ACK.raidJoinDataList);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_RAID)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().ResetUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_RAID_SET_COOP_ALL_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		sPacket.raidDetailDataList.ForEach(delegate(NKMRaidDetailData e)
		{
			NKCScenManager.GetScenManager().GetNKCRaidDataMgr().SetData(e);
		});
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			sPacket.raidDetailDataList.ForEach(delegate(NKMRaidDetailData e)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().RefreshCityRaidData(e);
			});
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().RefreshRaidCoopAllButtonState();
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().RefreshEventList();
		}
	}

	public static void OnRecv(NKMPacket_RAID_RESULT_ACCEPT_ACK cNKMPacket_RAID_RESULT_ACCEPT_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_RAID_RESULT_ACCEPT_ACK.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		int cityID = -1;
		nKMUserData?.m_WorldmapData.RemoveEvent(NKM_WORLDMAP_EVENT_TYPE.WET_RAID, cNKMPacket_RAID_RESULT_ACCEPT_ACK.raidUID, out cityID);
		if (nKMUserData != null && cNKMPacket_RAID_RESULT_ACCEPT_ACK.rewardData != null)
		{
			nKMUserData.GetReward(cNKMPacket_RAID_RESULT_ACCEPT_ACK.rewardData);
			NKCUIResult.Instance.OpenComplexResult(nKMUserData.m_ArmyData, cNKMPacket_RAID_RESULT_ACCEPT_ACK.rewardData, delegate
			{
				OnRaidReward(cNKMPacket_RAID_RESULT_ACCEPT_ACK, cityID);
			}, 0L);
		}
		else
		{
			OnRaidReward(cNKMPacket_RAID_RESULT_ACCEPT_ACK, cityID);
		}
	}

	private static void OnRaidReward(NKMPacket_RAID_RESULT_ACCEPT_ACK cNKMPacket_RAID_RESULT_ACCEPT_ACK, int cityID)
	{
		NKCScenManager.CurrentUserData();
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(cNKMPacket_RAID_RESULT_ACCEPT_ACK, cityID);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_RAID || NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_RAID_READY)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
		}
	}

	public static void OnRecv(NKMPacket_RAID_RESULT_ACCEPT_ALL_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		List<int> list = new List<int>();
		foreach (long raidUid in sPacket.raidUids)
		{
			int cityID = -1;
			if (nKMUserData != null)
			{
				nKMUserData.m_WorldmapData.RemoveEvent(NKM_WORLDMAP_EVENT_TYPE.WET_RAID, raidUid, out cityID);
				list.Add(cityID);
			}
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(sPacket, list);
		}
		if (nKMUserData != null && sPacket.rewardData != null)
		{
			nKMUserData.GetReward(sPacket.rewardData);
			NKCUIResult.Instance.OpenComplexResult(nKMUserData.m_ArmyData, sPacket.rewardData, null, 0L);
		}
	}

	public static void OnRecv(NKMPacket_MY_RAID_LIST_ACK cNKMPacket_MY_RAID_LIST_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_MY_RAID_LIST_ACK.errorCode))
		{
			return;
		}
		NKCScenManager.GetScenManager().GetNKCRaidDataMgr().SetDataList(cNKMPacket_MY_RAID_LIST_ACK.myRaidDataList);
		for (int i = 0; i < cNKMPacket_MY_RAID_LIST_ACK.myRaidDataList.Count; i++)
		{
			NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(cNKMPacket_MY_RAID_LIST_ACK.myRaidDataList[i].stageID);
			if (nKMRaidTemplet != null && nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
			{
				NKCPacketSender.Send_NKMPacket_RAID_DETAIL_INFO_REQ(cNKMPacket_MY_RAID_LIST_ACK.myRaidDataList[i].raidUID);
			}
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(cNKMPacket_MY_RAID_LIST_ACK);
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().RefreshRaidCoopAllButtonState();
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().RefreshEventList();
		}
	}

	public static void OnRecv(NKMPacket_RAID_DETAIL_INFO_ACK cNKMPacket_RAID_DETAIL_INFO_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_RAID_DETAIL_INFO_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetNKCRaidDataMgr().SetData(cNKMPacket_RAID_DETAIL_INFO_ACK.raidDetailData);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_RAID)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().OnRecv(cNKMPacket_RAID_DETAIL_INFO_ACK);
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(cNKMPacket_RAID_DETAIL_INFO_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_RAID_POINT_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().GetReward(sPacket.rewardData);
			NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentUserData().m_ArmyData, sPacket.rewardData, "");
		}
	}

	public static void OnRecv(NKMPacket_RAID_SEASON_NOT sPacket)
	{
		if (sPacket.raidSeason != null)
		{
			NKCRaidSeasonManager.RaidSeason = sPacket.raidSeason;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(sPacket);
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_RAID)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().ResetUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_RAID_SWEEP_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			NKCScenManager.GetScenManager().GetNKCRaidDataMgr().SetData(sPacket.raidDetailData);
			if (sPacket.raidResultData.curHP <= 0f)
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(sPacket);
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_RAID)
			{
				NKCUIFadeInOut.FadeOut(0.1f, null, bWhite: false, 1f);
				NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().ResetUI();
			}
			else
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID);
				NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_RAID_POINT_EXTRA_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCRaidSeasonManager.RaidSeason = sPacket.raidSeason;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.GetReward(sPacket.rewardData);
		NKCUIResult.Instance.OpenComplexResult(myUserData.m_ArmyData, sPacket.rewardData, delegate
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WORLDMAP)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OnRecv(sPacket);
			}
		}, 0L);
	}

	public static void OnRecv(NKMPacket_SET_UNIT_SKIN_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(sPacket.unitUID);
			if (unitFromUID != null)
			{
				unitFromUID.m_SkinID = sPacket.skinID;
			}
			armyData.UpdateData(unitFromUID);
		}
	}

	public static void OnRecv(NKMPacket_CONTRACT_PERMANENTLY_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
			NKMArmyData armyData = myUserData.m_ArmyData;
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(sPacket.unitUID);
			if (unitFromUID != null)
			{
				unitFromUID.SetPermanentContract();
				armyData.UpdateData(unitFromUID);
			}
			if (NKCUILifetime.IsInstanceOpen)
			{
				NKCUILifetime.Instance.PlayAni();
			}
		}
	}

	public static void OnRecv(NKMPacket_EXCHANGE_PIECE_TO_UNIT_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
		NKMRewardData nKMRewardData = new NKMRewardData();
		nKMRewardData.UnitDataList.AddRange(sPacket.unitDataList);
		myUserData.GetReward(nKMRewardData);
		NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentUserData().m_ArmyData, nKMRewardData, NKCStringTable.GetString("SI_PF_PERSONNEL_SCOUT_TEXT"));
		if (NKCUIScout.IsInstanceOpen)
		{
			NKCUIScout.Instance.Refresh();
		}
		NKMPieceTemplet nKMPieceTemplet = NKMTempletContainer<NKMPieceTemplet>.Find(sPacket.costItemData.ItemID);
		if (nKMPieceTemplet != null)
		{
			long num = (myUserData.m_ArmyData.IsCollectedUnit(nKMPieceTemplet.m_PieceGetUintId) ? nKMPieceTemplet.m_PieceReq : nKMPieceTemplet.m_PieceReqFirst);
			if (myUserData.m_InventoryData.GetCountMiscItem(nKMPieceTemplet.m_PieceId) < num)
			{
				NKCUIScout.UnregisgerAlarmOff(nKMPieceTemplet.Key);
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		if (sPacket.bestUnitReviewCommentDataList != null)
		{
			for (int i = 0; i < sPacket.bestUnitReviewCommentDataList.Count; i++)
			{
				sPacket.bestUnitReviewCommentDataList[i].content = NKCFilterManager.CheckBadChat(sPacket.bestUnitReviewCommentDataList[i].content);
			}
		}
		if (sPacket.unitReviewCommentDataList != null)
		{
			for (int j = 0; j < sPacket.unitReviewCommentDataList.Count; j++)
			{
				sPacket.unitReviewCommentDataList[j].content = NKCFilterManager.CheckBadChat(sPacket.unitReviewCommentDataList[j].content);
			}
		}
		if (sPacket.myUnitReviewCommentData != null && !string.IsNullOrEmpty(sPacket.myUnitReviewCommentData.content))
		{
			sPacket.myUnitReviewCommentData.content = NKCFilterManager.CheckBadChat(sPacket.myUnitReviewCommentData.content);
		}
		if (NKCUIUnitReview.IsInstanceOpen)
		{
			NKCUIUnitReview.Instance.RecvReviewData(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_COMMENT_LIST_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		if (sPacket.unitReviewCommentDataList != null)
		{
			for (int i = 0; i < sPacket.unitReviewCommentDataList.Count; i++)
			{
				sPacket.unitReviewCommentDataList[i].content = NKCFilterManager.CheckBadChat(sPacket.unitReviewCommentDataList[i].content);
			}
		}
		if (NKCUIUnitReview.IsInstanceOpen)
		{
			NKCUIUnitReview.Instance.RecvCommentList(sPacket.unitReviewCommentDataList);
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_COMMENT_WRITE_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (sPacket.myUnitReviewCommentData != null && !string.IsNullOrEmpty(sPacket.myUnitReviewCommentData.content))
			{
				sPacket.myUnitReviewCommentData.content = NKCFilterManager.CheckBadChat(sPacket.myUnitReviewCommentData.content);
			}
			if (NKCUIUnitReview.IsInstanceOpen)
			{
				NKCUIUnitReview.Instance.RecvMyCommentChanged(sPacket.myUnitReviewCommentData);
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_COMMENT_DELETE_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCUIUnitReview.IsInstanceOpen)
		{
			NKCUIUnitReview.Instance.RecvMyCommentChanged(null);
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_COMMENT_VOTE_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_UNIT_REVIEW_COMMENT_NOT_EXIST)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_REVIEW_IS_ALREADY_DELETE);
		}
		else if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (sPacket.unitReviewCommentData != null && !string.IsNullOrEmpty(sPacket.unitReviewCommentData.content))
			{
				sPacket.unitReviewCommentData.content = NKCFilterManager.CheckBadChat(sPacket.unitReviewCommentData.content);
			}
			if (NKCUIUnitReview.IsInstanceOpen)
			{
				NKCUIUnitReview.Instance.RecvCommentVote(sPacket.unitID, sPacket.unitReviewCommentData, bVote: true);
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_COMMENT_VOTE_CANCEL_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_UNIT_REVIEW_COMMENT_NOT_EXIST)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_REVIEW_IS_ALREADY_DELETE);
		}
		else if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (sPacket.unitReviewCommentData != null && !string.IsNullOrEmpty(sPacket.unitReviewCommentData.content))
			{
				sPacket.unitReviewCommentData.content = NKCFilterManager.CheckBadChat(sPacket.unitReviewCommentData.content);
			}
			if (NKCUIUnitReview.IsInstanceOpen)
			{
				NKCUIUnitReview.Instance.RecvCommentVote(sPacket.unitID, sPacket.unitReviewCommentData, bVote: false);
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_SCORE_VOTE_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_UNIT_REVIEW_SCORE_INTERVAL_HAS_NOT_ELAPSED)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(sPacket.errorCode));
		}
		else if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCUIUnitReview.IsInstanceOpen)
		{
			NKCUIUnitReview.Instance.RecvScoreVoteAck(sPacket.unitID, sPacket.unitReviewScoreData);
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_USER_BAN_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCUnitReviewManager.AddBanList(sPacket.targetUserUid);
			if (NKCUIUnitReview.IsInstanceOpen)
			{
				NKCUIUnitReview.Instance.RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_USER_BAN_CANCEL_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCUnitReviewManager.RemoveBanList(sPacket.targetUserUid);
			if (NKCUIUnitReview.IsInstanceOpen)
			{
				NKCUIUnitReview.Instance.RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_USER_BAN_LIST_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCUnitReviewManager.m_bReceivedUnitReviewBanList = true;
			NKCUnitReviewManager.SetBanList(sPacket.banishList);
			if (NKCUIUnitReview.IsInstanceOpen)
			{
				NKCUIUnitReview.Instance.OnRecvBanList();
			}
		}
	}

	public static void OnRecv(NKMPacket_NEGOTIATE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(sPacket.targetUnitUid);
			NKCNegotiateManager.NegotiateResultUIData uiData = NKCNegotiateManager.MakeResultUIData(nKMUserData, sPacket);
			if (unitFromUID != null)
			{
				unitFromUID.m_UnitLevel = sPacket.targetUnitLevel;
				unitFromUID.m_iUnitLevelEXP = sPacket.targetUnitExp;
				unitFromUID.loyalty = sPacket.targetUnitLoyalty;
			}
			if (NKCUIUnitInfo.IsInstanceOpen)
			{
				NKCUIUnitInfo.Instance.ReserveLevelUpFx(uiData);
			}
			nKMUserData.m_ArmyData.UpdateUnitData(unitFromUID);
		}
	}

	public static void OnRecv(NKMPacket_ATTENDANCE_NOT sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			Debug.LogWarning($"NKMPacket_ATTENDANCE_NOT ERROR - {sPacket.errorCode}");
			if (NKCUIAttendance.IsInstanceOpen)
			{
				NKCUIAttendance.Instance.Close();
			}
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(sPacket.errorCode));
			if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_SYSTEM_CONTENTS_BLOCK)
			{
				NKMAttendanceManager.SetContentBlock();
			}
		}
		else
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME()?.ReserveAttendanceData(sPacket.attendanceData, sPacket.lastUpdateDate);
		}
	}

	public static void OnRecv(NKMPacket_USER_PROFILE_INFO_ACK cNKMPacket_USER_PROFILE_INFO_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_USER_PROFILE_INFO_ACK.errorCode))
		{
			NKCPopupFriendInfo.Instance.Open(cNKMPacket_USER_PROFILE_INFO_ACK.userProfileData, cNKMPacket_USER_PROFILE_INFO_ACK.supportUnitProfileData);
		}
	}

	public static void OnRecv(NKMPacket_GAME_OPTION_CHANGE_ACK cNKMPacket_GAME_OPTION_CHANGE_ACK)
	{
		NKMPopUpBox.CloseWaitBox();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GAME_OPTION_CHANGE_ACK.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMUserOption userOption = myUserData.m_UserOption;
		if (userOption == null)
		{
			return;
		}
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			if (userOption.m_ActionCameraType != cNKMPacket_GAME_OPTION_CHANGE_ACK.actionCameraType)
			{
				userOption.m_ActionCameraType = cNKMPacket_GAME_OPTION_CHANGE_ACK.actionCameraType;
				gameOptionData.SetUseActionCamera(cNKMPacket_GAME_OPTION_CHANGE_ACK.actionCameraType, bForce: true);
			}
			if (userOption.m_bTrackCamera != cNKMPacket_GAME_OPTION_CHANGE_ACK.isTrackCamera)
			{
				userOption.m_bTrackCamera = cNKMPacket_GAME_OPTION_CHANGE_ACK.isTrackCamera;
				gameOptionData.SetUseTrackCamera(cNKMPacket_GAME_OPTION_CHANGE_ACK.isTrackCamera, bForce: true);
			}
			if (userOption.m_bViewSkillCutIn != cNKMPacket_GAME_OPTION_CHANGE_ACK.isViewSkillCutIn)
			{
				userOption.m_bViewSkillCutIn = cNKMPacket_GAME_OPTION_CHANGE_ACK.isViewSkillCutIn;
				gameOptionData.SetViewSkillCutIn(cNKMPacket_GAME_OPTION_CHANGE_ACK.isViewSkillCutIn, bForce: true);
			}
			if (userOption.m_bDefaultPvpAutoRespawn != cNKMPacket_GAME_OPTION_CHANGE_ACK.defaultPvpAutoRespawn)
			{
				userOption.m_bDefaultPvpAutoRespawn = cNKMPacket_GAME_OPTION_CHANGE_ACK.defaultPvpAutoRespawn;
				gameOptionData.SetPvPAutoRespawn(cNKMPacket_GAME_OPTION_CHANGE_ACK.defaultPvpAutoRespawn, bForce: true);
			}
			if (userOption.m_bAutoSyncFriendDeck != cNKMPacket_GAME_OPTION_CHANGE_ACK.autoSyncFriendDeck)
			{
				userOption.m_bAutoSyncFriendDeck = cNKMPacket_GAME_OPTION_CHANGE_ACK.autoSyncFriendDeck;
				gameOptionData.SetAutoSyncFriendDeck(cNKMPacket_GAME_OPTION_CHANGE_ACK.autoSyncFriendDeck, bForce: true);
			}
			gameOptionData.Save();
			NKCUIGameOption.CheckInstanceAndClose();
		}
	}

	public static void OnRecv(NKMPacket_CUTSCENE_DUNGEON_START_ACK sPacket)
	{
		NKCScenManager.CurrentUserData()?.UpdateStagePlayData(sPacket.stagePlayData);
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCCollectionManager.SetVote(sPacket.unitID, sPacket.unitReviewTagData.tagType, sPacket.unitReviewTagData.votedCount, sPacket.unitReviewTagData.isVoted);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().OnRecvReviewTagVoteCancelAck(sPacket);
			}
			else if (NKCUICollectionUnitInfo.IsInstanceOpen)
			{
				NKCUICollectionUnitInfo.Instance.OnRecvReviewTagVoteCancelAck(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_TAG_VOTE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCCollectionManager.SetVote(sPacket.unitID, sPacket.unitReviewTagData.tagType, sPacket.unitReviewTagData.votedCount, sPacket.unitReviewTagData.isVoted);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().OnRecvReviewTagVoteAck(sPacket);
			}
			else if (NKCUICollectionUnitInfo.IsInstanceOpen)
			{
				NKCUICollectionUnitInfo.Instance.OnRecvReviewTagVoteAck(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_REVIEW_TAG_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().OnRecvReviewTagListAck(sPacket);
			}
			else if (NKCUICollectionUnitInfo.IsInstanceOpen)
			{
				NKCUICollectionUnitInfo.Instance.OnRecvReviewTagListAck(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_TEAM_COLLECTION_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().OnRecvTeamCollectionRewardAck(sPacket);
		}
	}

	private static void CloseIngameEmoticonListPopup()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (gameClient != null && gameClient.GetGameHud() != null && gameClient.GetGameHud().GetNKCGameHudEmoticon() != null && gameClient.GetGameHud().GetNKCGameHudEmoticon().IsNKCPopupInGameEmoticonOpen)
			{
				gameClient.GetGameHud().GetNKCGameHudEmoticon().NKCPopupInGameEmoticon?.Close();
			}
		}
	}

	public static void OnRecv(NKMPacket_GAME_EMOTICON_ACK cNKMPacket_GAME_EMOTICON_ACK)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GAME_EMOTICON_ACK.errorCode);
		CloseIngameEmoticonListPopup();
	}

	public static void OnRecv(NKMPacket_GAME_EMOTICON_NOT cNKMPacket_GAME_EMOTICON_NOT)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && gameOptionData.UseEmoticonBlock)
		{
			return;
		}
		if (NKCReplayMgr.IsRecording())
		{
			NKCScenManager.GetScenManager().GetNKCReplayMgr().FillReplayData(cNKMPacket_GAME_EMOTICON_NOT);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (gameClient != null && gameClient.GetGameHud() != null && gameClient.GetGameHud().GetNKCGameHudEmoticon() != null)
			{
				gameClient.GetGameHud().GetNKCGameHudEmoticon().OnRecv(cNKMPacket_GAME_EMOTICON_NOT);
			}
		}
	}

	public static void OnRecv(NKMPacket_EMOTICON_DATA_ACK cNKMPacket_EMOTICON_DATA_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EMOTICON_DATA_ACK.errorCode))
		{
			NKCEmoticonManager.m_lstAniPreset = cNKMPacket_EMOTICON_DATA_ACK.presetData.animationList;
			NKCEmoticonManager.m_lstTextPreset = cNKMPacket_EMOTICON_DATA_ACK.presetData.textList;
			NKCEmoticonManager.SetEmoticonDatas(cNKMPacket_EMOTICON_DATA_ACK.emoticonDatas);
			_ = NKCEmoticonManager.m_bReceivedEmoticonData;
			NKCEmoticonManager.m_bReceivedEmoticonData = true;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY && NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().IsWaitForEmoticon())
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetWaitForEmoticon(bValue: false);
				NKCPopupEmoticonSetting.Instance.Open();
			}
			else if (NKCEmoticonManager.m_bWaitForPopup)
			{
				NKCEmoticonManager.m_bWaitForPopup = false;
				NKCPopupEmoticonSetting.Instance.Open();
			}
		}
	}

	public static void OnRecv(NKMPacket_EMOTICON_ANI_CHANGE_ACK cNKMPacket_EMOTICON_ANI_CHANGE_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EMOTICON_ANI_CHANGE_ACK.errorCode) || !NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			return;
		}
		int presetIndex = cNKMPacket_EMOTICON_ANI_CHANGE_ACK.presetIndex;
		if (NKCEmoticonManager.m_lstAniPreset != null && NKCEmoticonManager.m_lstAniPreset.Count > presetIndex && presetIndex >= 0)
		{
			NKCEmoticonManager.m_lstAniPreset[presetIndex] = cNKMPacket_EMOTICON_ANI_CHANGE_ACK.emoticonId;
			if (NKCPopupEmoticonSetting.IsInstanceOpen)
			{
				NKCPopupEmoticonSetting.Instance.OnRecv(cNKMPacket_EMOTICON_ANI_CHANGE_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_EMOTICON_TEXT_CHANGE_ACK cNKMPacket_EMOTICON_TEXT_CHANGE_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EMOTICON_TEXT_CHANGE_ACK.errorCode) || !NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			return;
		}
		int presetIndex = cNKMPacket_EMOTICON_TEXT_CHANGE_ACK.presetIndex;
		if (NKCEmoticonManager.m_lstTextPreset != null && NKCEmoticonManager.m_lstTextPreset.Count > presetIndex && presetIndex >= 0)
		{
			NKCEmoticonManager.m_lstTextPreset[presetIndex] = cNKMPacket_EMOTICON_TEXT_CHANGE_ACK.emoticonId;
			if (NKCPopupEmoticonSetting.IsInstanceOpen)
			{
				NKCPopupEmoticonSetting.Instance.OnRecv(cNKMPacket_EMOTICON_TEXT_CHANGE_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_EMOTICON_FAVORITES_SET_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCEmoticonManager.UpdateEmoticonData(sPacket.emoticon);
			if (NKCPopupPrivateChatLobby.IsInstanceOpen)
			{
				NKCPopupPrivateChatLobby.Instance.RefreshEmoticonList();
			}
			if (NKCPopupGuildChat.IsInstanceOpen)
			{
				NKCPopupGuildChat.Instance.RefreshEmoticonList();
			}
		}
	}

	public static void OnRecv(NKMPacket_GAME_LOAD_COMPLETE_ACK cNKMPacket_GAME_LOAD_COMPLETE_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GAME_LOAD_COMPLETE_ACK.errorCode))
		{
			if (cNKMPacket_GAME_LOAD_COMPLETE_ACK.errorCode == NKM_ERROR_CODE.NEC_FAIL_GAME_LOAD_INVALID_STATE)
			{
				if (NKCPrivatePVPRoomMgr.LobbyData != null)
				{
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM);
				}
				else
				{
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
				}
			}
			return;
		}
		Debug.Log($"[NKMPacket_GAME_LOAD_COMPLETE_ACK] isIntrude : {cNKMPacket_GAME_LOAD_COMPLETE_ACK.isIntrude}");
		if (NKCReplayMgr.IsReplayRecordingOpened())
		{
			NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
			if (gameData.IsPVP() && !cNKMPacket_GAME_LOAD_COMPLETE_ACK.isIntrude)
			{
				Debug.Log("[NKMPacket_GAME_LOAD_COMPLETE_ACK] CreateNewReplayData");
				NKCScenManager.GetScenManager().GetNKCReplayMgr().CreateNewReplayData(gameData, cNKMPacket_GAME_LOAD_COMPLETE_ACK.gameRuntimeData);
			}
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_LOAD_COMPLETE_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_START_NOT cNKMPacket_GAME_START_NOT)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			if (NKCScenManager.GetScenManager().GetGameClient().GetGameData()
				.GetGameType() == NKM_GAME_TYPE.NGT_WARFARE)
			{
				NKCScenManager.GetScenManager().WarfareGameData.warfareGameState = NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAYING;
			}
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_START_NOT);
		}
	}

	public static void OnRecv(NKMPacket_GAME_INTRUDE_START_NOT cNKMPacket_GAME_INTRUDE_START_NOT)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			if (NKCScenManager.GetScenManager().GetGameClient().GetGameData()
				.GetGameType() == NKM_GAME_TYPE.NGT_WARFARE)
			{
				NKCScenManager.GetScenManager().WarfareGameData.warfareGameState = NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAYING;
			}
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_INTRUDE_START_NOT);
		}
	}

	public static void OnRecv(NKMPacket_GAME_PAUSE_ACK cNKMPacket_GAME_PAUSE_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_PAUSE_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_PAUSE_ACK);
		}
	}

	public static void OnRecv(NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT cPacket_NPT_GAME_SYNC_DATA_PACK_NOT)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAME)
		{
			NKCPacketObjectPool.CloseObject(cPacket_NPT_GAME_SYNC_DATA_PACK_NOT);
			return;
		}
		NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cPacket_NPT_GAME_SYNC_DATA_PACK_NOT);
		NKCPacketObjectPool.CloseObject(cPacket_NPT_GAME_SYNC_DATA_PACK_NOT);
	}

	public static void OnRecv(NKMPacket_GAME_DEV_RESPAWN_ACK cNKMPacket_GAME_DEV_RESPAWN_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_DEV_RESPAWN_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_CHECK_DIE_UNIT_ACK cNKMPacket_GAME_CHECK_DIE_UNIT_ACK)
	{
		NKCScenManager.GetScenManager().GetNowScenID();
		_ = 3;
	}

	public static void OnRecv(NKMPacket_GAME_RESPAWN_ACK cPacket_GAME_RESPAWN_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cPacket_GAME_RESPAWN_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_SHIP_SKILL_ACK cPacket_GAME_SHIP_SKILL_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cPacket_GAME_SHIP_SKILL_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_TACTICAL_COMMAND_ACK cNKMPacket_GAME_TACTICAL_COMMAND_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_TACTICAL_COMMAND_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_AUTO_RESPAWN_ACK cPacket_GAME_AUTO_RESPAWN_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cPacket_GAME_AUTO_RESPAWN_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_UNIT_RETREAT_ACK cNKMPacket_GAME_UNIT_RETREAT_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_UNIT_RETREAT_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_SPEED_2X_ACK cNKMPacket_GAME_SPEED_2X_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_SPEED_2X_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_SPEED_2X_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_AUTO_SKILL_CHANGE_ACK cNKMPacket_GAME_AUTO_SKILL_CHANGE_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_AUTO_SKILL_CHANGE_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_AUTO_SKILL_CHANGE_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_USE_UNIT_SKILL_ACK cNKMPacket_GAME_USE_UNIT_SKILL_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_USE_UNIT_SKILL_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_USE_UNIT_SKILL_ACK);
		}
	}

	public static void OnRecv(NKMPacket_RECONNECT_SERVER_INFO_NOT not)
	{
		NKCConnectGame connectGame = NKCScenManager.GetScenManager().GetConnectGame();
		connectGame.SetRemoteAddress(not.serverIp, not.port);
		connectGame.SetAccessToken(not.accessToken);
		connectGame.ResetConnection();
		connectGame.ConnectToLobbyServer();
	}

	public static void OnRecv(NKMPacket_COMMON_FAIL_ACK ack)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(ack.errorCode);
	}

	public static void OnRecv(NKMPacket_UPDATE_NICKNAME_NOT sPacket)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			nKMUserData.m_UserNickName = sPacket.nickname;
		}
	}

	public static void OnRecv(NKMPacket_EXIT_APP_NOT sPacket)
	{
		if (!(NKCScenManager.GetScenManager() == null))
		{
			NKCScenManager.GetScenManager().GetConnectLogin().SetEnable(bSet: false);
			NKCScenManager.GetScenManager().GetConnectGame().SetEnable(bSet: false);
			NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
			NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
			NKCScenManager.GetScenManager().Get_SCEN_LOGIN().SetErrorCodeForNGS(sPacket.errorCode);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
		}
	}

	public static void OnRecv(NKMPacket_NEXON_NGS_DATA_NOT cNKMPacket_NEXON_NGS_DATA_NOT)
	{
		NKCPMNexonNGS.OnRecv(cNKMPacket_NEXON_NGS_DATA_NOT);
	}

	public static void OnRecv(NKMPacket_NEXON_PC_DATA_NOT cNKMPacket_NEXON_PC_DATA_NOT)
	{
		NKCPMNexonNGS.SetNpaCode(cNKMPacket_NEXON_PC_DATA_NOT.npacode);
	}

	public static void OnRecv(NKMPacket_ACCOUNT_LINK_ACK cNKMPacket_ACCOUNT_LINK_ACK)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_ACCOUNT_LINK_ACK.errorCode);
	}

	public static void OnRecv(NKMPacket_UPDATE_RECONNECT_KEY_NOT cNKMPacket_UPDATE_RECONNECT_KEY_NOT)
	{
		NKCScenManager.GetScenManager().GetConnectGame().SetReconnectKey(cNKMPacket_UPDATE_RECONNECT_KEY_NOT.reconnectKey);
	}

	public static void OnRecv(NKMPacket_INQUIRY_RESPONDED_NOT sPacket)
	{
		NKCUIPopupMessageServer.Instance.Open(NKCUIPopupMessageServer.eMessageStyle.Slide, NKCUtilString.GET_STRING_TOY_CUSTOMER_CENTER_RESPOND);
	}

	public static void OnRecv(NKMPacket_ACCOUNT_UNLINK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPublisherModule.Auth.ResetConnection();
			NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
			NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
		}
	}

	public static void OnRecv(NKMPacket_ACCOUNT_LEAVE_STATE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) || !sPacket.leave || !NKCDefineManager.DEFINE_SB_GB())
		{
			return;
		}
		if (NKCPublisherModule.Auth.IsGuest())
		{
			NKCPublisherModule.Auth.Withdraw(delegate
			{
				NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
				NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
			});
		}
		else
		{
			NKCPublisherModule.Auth.TemporaryWithdrawal(delegate
			{
				NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
				NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
			});
		}
	}

	public static void OnRecv(NKMPacket_ACCOUNT_KICK_NOT sPacket)
	{
		NKMPopUpBox.OpenWaitBox();
		NKCPublisherModule.Auth.Logout(OnLogoutComplete);
	}

	public static void OnLogoutComplete(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError))
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME().ResetFirstLobby();
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TOY_LOGOUT_SUCCESS, MoveToLogin);
		}
	}

	public static void MoveToLogin()
	{
		Log.Debug("[PacketHandlersLobby] MoveToLogin", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketHandlersLobby.cs", 9670);
		NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
		NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_LOGIN && (object)NKCPatchDownloader.Instance != null)
		{
			NKCPatchDownloader.Instance.InitCheckTime();
		}
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
	}

	public static void OnRecv(NKMPacket_MARQUEE_MESSAGE_NOT sPacket)
	{
		if (sPacket.message == null)
		{
			return;
		}
		foreach (string item in sPacket.message)
		{
			NKCUIPopupMessageServer.Instance.Open(NKCUIPopupMessageServer.eMessageStyle.Slide, NKCPublisherModule.Localization.GetTranslationIfJson(item));
		}
	}

	public static void OnRecv(NKMPacket_MESSAGE_NOT sPacket)
	{
		if (sPacket.message == null)
		{
			return;
		}
		foreach (string item in sPacket.message)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(item), NKCPopupMessage.eMessagePosition.Top, bShowFX: false, bPreemptive: false);
		}
	}

	public static void OnRecv(NKMPacket_EVENT_BINGO_RANDOM_MARK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
			myUserData.GetReward(sPacket.rewardData);
			NKMEventManager.GetBingoData(sPacket.eventId)?.SetMileage(sPacket.mileage);
			if (NKCUIEvent.IsInstanceOpen && sPacket.rewardData != null)
			{
				NKCUIEvent.Instance.MarkBingo(sPacket.eventId, sPacket.rewardData.BingoTileList, bRandom: true);
			}
			if (NKCUIModuleHome.IsAnyInstanceOpen())
			{
				NKCUIModuleHome.SendMessage(new NKCUIModuleEventAdapter.EventModuleDataBingo
				{
					eventID = sPacket.eventId,
					bingoList = sPacket.rewardData.BingoTileList,
					bRandom = true
				});
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_BINGO_INDEX_MARK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData().GetReward(sPacket.rewardData);
			NKMEventManager.GetBingoData(sPacket.eventId)?.SetMileage(sPacket.mileage);
			if (NKCUIEvent.IsInstanceOpen && sPacket.rewardData != null)
			{
				NKCUIEvent.Instance.MarkBingo(sPacket.eventId, sPacket.rewardData.BingoTileList, bRandom: false);
			}
			if (NKCUIModuleHome.IsAnyInstanceOpen())
			{
				NKCUIModuleHome.SendMessage(new NKCUIModuleEventAdapter.EventModuleDataBingo
				{
					eventID = sPacket.eventId,
					bingoList = sPacket.rewardData.BingoTileList,
					bRandom = false
				});
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_BINGO_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			myUserData.GetReward(sPacket.rewardData);
			NKMEventManager.GetBingoData(sPacket.eventId)?.RecvReward(sPacket.rewardIndex);
			if (NKCUIEvent.IsInstanceOpen)
			{
				NKCUIEvent.Instance.RefreshUI(sPacket.eventId);
			}
			if (NKCPopupEventBingoReward.IsInstanceOpen)
			{
				NKCPopupEventBingoReward.Instance.Refresh();
			}
			NKCUIModuleHome.UpdateAllModule();
			NKCUIResult.Instance.OpenComplexResult(myUserData.m_ArmyData, sPacket.rewardData, null, 0L);
		}
	}

	public static void OnRecv(NKMPacket_EVENT_BINGO_REWARD_ALL_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData.GetReward(sPacket.rewardData);
		EventBingo bingoData = NKMEventManager.GetBingoData(sPacket.eventId);
		if (bingoData != null)
		{
			foreach (int item in sPacket.hsRewardIndex)
			{
				bingoData.RecvReward(item);
			}
		}
		if (NKCUIEvent.IsInstanceOpen)
		{
			NKCUIEvent.Instance.RefreshUI(sPacket.eventId);
		}
		if (NKCPopupEventBingoReward.IsInstanceOpen)
		{
			NKCPopupEventBingoReward.Instance.Refresh();
		}
		NKCUIModuleHome.UpdateAllModule();
		NKCUIResult.Instance.OpenComplexResult(myUserData.m_ArmyData, sPacket.rewardData, null, 0L);
	}

	public static void OnRecv(NKMPacket_RESET_STAGE_PLAY_COUNT_ACK cNKMPacket_RESET_STAGE_PLAY_COUNT_ACK)
	{
		Debug.Log("OnRecv - NKMPacket_RESET_STAGE_PLAY_COUNT_ACK - NKCPacketHandlersLobby");
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_RESET_STAGE_PLAY_COUNT_ACK.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.UpdateStagePlayData(cNKMPacket_RESET_STAGE_PLAY_COUNT_ACK.stagePlayData);
				nKMUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_RESET_STAGE_PLAY_COUNT_ACK.costItemData);
			}
			if (NKCUIOperationNodeViewer.isOpen())
			{
				NKCUIOperationNodeViewer.Instance.Refresh();
			}
		}
	}

	public static void OnRecv(NKMPacket_ZLONG_USE_COUPON_ACK cNKMPacket_ZLONG_USE_COUPON_ACK)
	{
		Debug.Log("OnRecv - cNKMPacket_ZLONG_USE_COUPON_ACK - NKCPacketHandlersLobby");
		if (cNKMPacket_ZLONG_USE_COUPON_ACK.errorCode == NKM_ERROR_CODE.NEC_FAIL_ZLONG_COUPON_API_RETURN_ERROR)
		{
			NKMPopUpBox.CloseWaitBox();
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_ZLONG_COUPON_API_RETURN_ERROR.ToString() + "_" + cNKMPacket_ZLONG_USE_COUPON_ACK.zlongInfoCode));
		}
		else if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_ZLONG_USE_COUPON_ACK.errorCode))
		{
			NKCPopupCoupon.CheckInstanceAndClose();
			if (NKCScenManager.CurrentUserData() != null)
			{
				NKCScenManager.CurrentUserData().GetReward(cNKMPacket_ZLONG_USE_COUPON_ACK.rewardData);
			}
			NKCUIResult.Instance.OpenComplexResult(NKCScenManager.CurrentUserData().m_ArmyData, cNKMPacket_ZLONG_USE_COUPON_ACK.rewardData, null, 0L);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_ACK sNKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sNKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_ACK.errorCode))
		{
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		if (inventoryData != null)
		{
			inventoryData.UpdateItemInfo(sNKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_ACK.costItemData);
			NKCScenManager.CurrentUserData().SetEquipTuningData(sNKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_ACK.equipTuningCandidate);
			NKMItemManager.SetResetCount(sNKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_ACK.resetCount);
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.m_NKCUIForgeTuning.ResetUI();
				NKCUIForge.Instance.DoAfterSetOptionChanged();
			}
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_ACK sNKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sNKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_ACK.errorCode))
		{
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		if (inventoryData != null)
		{
			NKMEquipItemData itemEquip = inventoryData.GetItemEquip(sNKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_ACK.equipUID);
			if (itemEquip != null)
			{
				itemEquip.m_SetOptionId = sNKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_ACK.setOptionId;
			}
			inventoryData.UpdateItemEquip(itemEquip);
			NKCScenManager.CurrentUserData().SetEquipTuningData(sNKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_ACK.equipTuningCandidate);
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.ResetUI();
				NKCUIForge.Instance.DoAfterSetOptionChangeConfirm();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_UNIT_LIST && NKCUIUnitInfo.IsInstanceOpen)
			{
				NKCUIUnitInfo.Instance.UpdateEquipSlots();
			}
			if (NKCUIInventory.IsInstanceOpen)
			{
				NKCUIInventory.Instance.ResetEquipSlotList();
			}
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_ACK sNKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sNKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_ACK.errorCode))
		{
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		if (inventoryData != null)
		{
			NKMEquipItemData itemEquip = inventoryData.GetItemEquip(sNKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_ACK.equipUID);
			if (itemEquip != null)
			{
				itemEquip.m_SetOptionId = sNKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_ACK.setOptionId;
			}
			if (NKCUIForge.IsInstanceOpen)
			{
				NKCUIForge.Instance.ResetUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_TUNING_CANCEL_ACK sNKMPacket_Equip_Tuning_Cancel_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sNKMPacket_Equip_Tuning_Cancel_ACK.errorCode))
		{
			NKCScenManager.CurrentUserData().SetEquipTuningData(sNKMPacket_Equip_Tuning_Cancel_ACK.equipTuningCandidate);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_TUNING_NOT sNKMPacket_Equip_Tuning_NOT)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sNKMPacket_Equip_Tuning_NOT.errorCode))
		{
			NKCScenManager.CurrentUserData().SetEquipTuningData(sNKMPacket_Equip_Tuning_NOT.equipTuningCandidate);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_CHAT_TRANSLATE_ACK cNKMPacket_GUILD_CHAT_TRANSLATE_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_CHAT_TRANSLATE_ACK.errorCode))
		{
			NKCPublisherModule.Localization.OnTranslateCompleteFromCS_Server(cNKMPacket_GUILD_CHAT_TRANSLATE_ACK.messageUid, "");
		}
		else
		{
			NKCPublisherModule.Localization.OnTranslateCompleteFromCS_Server(cNKMPacket_GUILD_CHAT_TRANSLATE_ACK.messageUid, cNKMPacket_GUILD_CHAT_TRANSLATE_ACK.textTranslated);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DATA_UPDATED_NOT cNKMPacket_GUILD_DATA_UPDATED_NOT)
	{
		NKCGuildManager.SetMyGuildData(cNKMPacket_GUILD_DATA_UPDATED_NOT.guildData);
	}

	public static void OnRecv(NKMPacket_GUILD_CANCEL_REQUEST_NOT cNKMPacket_GUILD_CANCEL_REQUEST_NOT)
	{
		if (cNKMPacket_GUILD_CANCEL_REQUEST_NOT.isRequest)
		{
			NKCGuildManager.RemoveRequestedData(cNKMPacket_GUILD_CANCEL_REQUEST_NOT.guildUid);
		}
		else
		{
			NKCGuildManager.RemoveInvitedData(cNKMPacket_GUILD_CANCEL_REQUEST_NOT.guildUid);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_BAN_NOT cNKMPacket_GUILD_BAN_NOT)
	{
		NKCGuildManager.SetMyData(new PrivateGuildData());
		NKCGuildManager.SetMyGuildData(null);
		NKCGuildCoopManager.ResetGuildCoopState();
		NKCChatManager.ResetGuildMemberChatList();
		NKCPopupMessageGuild.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_FORCE_EXIT_TOAST_MESSAGE_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_FORCE_EXIT_INFORMATION_POPUP_BODY_DESC, bIsGoodNews: false);
	}

	public static void OnRecv(NKMPacket_GUILD_ACCEPT_JOIN_NOT cNKMPacket_GUILD_ACCEPT_JOIN_NOT)
	{
		if (cNKMPacket_GUILD_ACCEPT_JOIN_NOT.isAllow)
		{
			NKCPopupMessageGuild.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_OVERLAY_MESSAGE_HEAD_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_OVERLAY_MESSAGE_BODY_JOIN, cNKMPacket_GUILD_ACCEPT_JOIN_NOT.guildName), bIsGoodNews: true);
		}
		NKCGuildManager.SetMyData(cNKMPacket_GUILD_ACCEPT_JOIN_NOT.privateGuildData);
	}

	public static void OnRecv(NKMPacket_GUILD_LEVEL_UP_NOT cNKMPacket_GUILD_LEVEL_UP_NOT)
	{
		NKCPopupMessageGuild.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_OVERLAY_MESSAGE_HEAD_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_OVERLAY_MESSAGE_BODY_LEVEL_UP, NKCGuildManager.MyGuildData.name, cNKMPacket_GUILD_LEVEL_UP_NOT.guildLevel), bIsGoodNews: true);
	}

	public static void OnRecv(NKMPacket_GUILD_INVITE_NOT cNKMPacket_GUILD_INVITE_NOT)
	{
	}

	public static void OnRecv(NKMPacket_GUILD_DELETED_NOT cNKMPacket_GUILD_DELETED_NOT)
	{
		NKCGuildManager.SetMyData(new PrivateGuildData());
		NKCGuildManager.SetMyGuildData(null);
		NKCGuildCoopManager.ResetGuildCoopState();
	}

	public static void OnRecv(NKMPacket_GUILD_MEMBER_GRADE_UPDATED_NOT cNKMPacket_GUILD_MEMBER_GRADE_UPDATED_NOT)
	{
		if (cNKMPacket_GUILD_MEMBER_GRADE_UPDATED_NOT.gradeBefore > cNKMPacket_GUILD_MEMBER_GRADE_UPDATED_NOT.gradeAfter)
		{
			NKCPopupMessageGuild.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_CHANGE_PERMISSION_TOAST_MESSAGE_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_GRADE_UP_INFORMATION_POPUP_BODY_DESC, NKCGuildManager.MyGuildData.name), bIsGoodNews: true);
		}
		else
		{
			NKCPopupMessageGuild.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_CHANGE_PERMISSION_TOAST_MESSAGE_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_FRADE_DOWN_INFORMATION_POPUP_BODY_DESC, NKCGuildManager.MyGuildData.name), bIsGoodNews: false);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_NOT cNKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_NOT)
	{
		NKCPopupMessageGuild.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_CHANGE_MASTER_TOAST_MESSAGE_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_GRADE_HANDOVER_INFORMATION_POPUP_BODY_DESC, NKCGuildManager.MyGuildData.name), bIsGoodNews: true);
	}

	public static void OnRecv(NKMPacket_GUILD_USER_PROFILE_UPDATED_NOT cNKMPacket_GUILD_USER_PROFILE_UPDATED_NOT)
	{
		NKCGuildManager.ChangeGuildMemberData(cNKMPacket_GUILD_USER_PROFILE_UPDATED_NOT.commonProfile, cNKMPacket_GUILD_USER_PROFILE_UPDATED_NOT.lastOnlineTime);
	}

	public static void OnRecv(NKMPacket_GUILD_JOIN_DISABLETIME_UPDATED_NOT cNKMPacket_GUILD_JOIN_DISABLETIME_UPDATED_NOT)
	{
		NKCGuildManager.SetGuildJoinDisableTime(cNKMPacket_GUILD_JOIN_DISABLETIME_UPDATED_NOT.joinDisableTime);
	}

	public static void OnRecv(NKMPacket_GUILD_UPDATE_NOTICE_NOT cNKMPacket_GUILD_UPDATE_NOTICE_NOT)
	{
		NKCPopupMessageGuild.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_LOBBY_INFORMATION_CHANGE_OVERLAY_TITLE_TEXT, NKCUtilString.GET_STRING_CONSORTIUM_LOBBY_INFORMATION_CHANGE_OVERLAY_BODY_TEXT, bIsGoodNews: true);
	}

	public static void OnRecv(NKMPacket_COMPANY_BUFF_ADD_NOT cNKMPacket_COMPANY_BUFF_ADD_NOT)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKCCompanyBuff.UpsertCompanyBuffData(nKMUserData.m_companyBuffDataList, cNKMPacket_COMPANY_BUFF_ADD_NOT.companyBuffData);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_GUILD_CREATE_ACK cNKMPacket_GUILD_CREATE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_CREATE_ACK.errorCode))
		{
			NKCGuildManager.SetMyData(cNKMPacket_GUILD_CREATE_ACK.privateGuildData);
			NKCGuildManager.SetMyGuildData(cNKMPacket_GUILD_CREATE_ACK.guildData);
			NKCScenManager.CurrentUserData().m_InventoryData.UpdateItemInfo(cNKMPacket_GUILD_CREATE_ACK.costItemDataList);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_CLOSE_ACK cNKMPacket_GUILD_CLOSE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_CLOSE_ACK.errorCode))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_POPUP_CONFIRM_DESC);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_CLOSE_CANCEL_ACK cNKMPacket_GUILD_CLOSE_CANCEL_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_CLOSE_CANCEL_ACK.errorCode))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_CONFIRM_DESC);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_SEARCH_ACK cNKMPacket_GUILD_SEARCH_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_SEARCH_ACK.errorCode))
		{
			NKCGuildManager.OnRecv(cNKMPacket_GUILD_SEARCH_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_LIST_ACK cNKMPacket_GUILD_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_LIST_ACK.errorCode))
		{
			NKCGuildManager.OnRecv(cNKMPacket_GUILD_LIST_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DATA_ACK cNKMPacket_GUILD_DATA_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_DATA_ACK.errorCode))
		{
			NKCPopupGuildInfo.Instance.Open(cNKMPacket_GUILD_DATA_ACK.guildData);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_JOIN_ACK cNKMPacket_GUILD_JOIN_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_JOIN_ACK.errorCode))
		{
			NKCGuildManager.RemoveLastJoinRequestedGuildData();
			if (NKCUIGuildJoin.IsInstanceOpen)
			{
				NKCUIGuildJoin.Instance.RefreshUI();
			}
		}
		else
		{
			NKCGuildManager.SetMyData(cNKMPacket_GUILD_JOIN_ACK.privateGuildData);
			NKCGuildManager.OnRecv(cNKMPacket_GUILD_JOIN_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_CANCEL_JOIN_ACK cNKMPacket_GUILD_CANCEL_JOIN_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_CANCEL_JOIN_ACK.errorCode))
		{
			NKCPacketSender.Send_NKMPacket_GUILD_LIST_REQ(GuildListType.SendRequest);
		}
		else
		{
			NKCGuildManager.OnRecv(cNKMPacket_GUILD_CANCEL_JOIN_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_ACCEPT_JOIN_ACK cNKMPacket_GUILD_ACCEPT_JOIN_ACK)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_ACCEPT_JOIN_ACK.errorCode);
	}

	public static void OnRecv(NKMPacket_GUILD_INVITE_ACK cNKMPacket_GUILD_INVITE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_INVITE_ACK.errorCode))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONSORTIUM_POPUP_INVITE_TITLE, NKCUtilString.GET_STRING_CONSORTIUM_INVITE_SEND_SUCCESS_BODY_DESC);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_CANCEL_INVITE_ACK cNKMPacket_GUILD_CANCEL_INVITE_ACK)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_CANCEL_INVITE_ACK.errorCode);
	}

	public static void OnRecv(NKMPacket_GUILD_ACCEPT_INVITE_ACK cNKMPacket_GUILD_ACCEPT_INVITE_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_ACCEPT_INVITE_ACK.errorCode))
		{
			NKCPacketSender.Send_NKMPacket_GUILD_LIST_REQ(GuildListType.ReceiveInvite);
			return;
		}
		NKCGuildManager.RemoveInvitedData(cNKMPacket_GUILD_ACCEPT_INVITE_ACK.guildUid);
		NKCGuildManager.SetMyData(cNKMPacket_GUILD_ACCEPT_INVITE_ACK.privateGuildData);
		if (NKCUIGuildJoin.IsInstanceOpen)
		{
			NKCUIGuildJoin.Instance.RefreshUI();
		}
	}

	public static void OnRecv(NKMPacket_GUILD_EXIT_ACK cNKMPacket_GUILD_EXIT_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_EXIT_ACK.errorCode))
		{
			NKCGuildManager.SetMyData(new PrivateGuildData
			{
				guildJoinDisableTime = cNKMPacket_GUILD_EXIT_ACK.joinDisableTime
			});
			NKCGuildManager.SetMyGuildData(null);
			NKCGuildCoopManager.ResetGuildCoopState();
			NKCChatManager.ResetGuildMemberChatList();
		}
	}

	public static void OnRecv(NKMPacket_GUILD_SET_MEMBER_GRADE_ACK cNKMPacket_GUILD_SET_MEMBER_GRADE_ACK)
	{
		if (cNKMPacket_GUILD_SET_MEMBER_GRADE_ACK.errorCode != NKM_ERROR_CODE.NEC_OK && NKCPopupFriendInfo.IsInstanceOpen)
		{
			NKCPopupFriendInfo.Instance.Close();
		}
		NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_SET_MEMBER_GRADE_ACK.errorCode);
	}

	public static void OnRecv(NKMPacket_GUILD_BAN_ACK cNKMPacket_GUILD_BAN_ACK)
	{
		if (cNKMPacket_GUILD_BAN_ACK.errorCode != NKM_ERROR_CODE.NEC_OK && NKCPopupFriendInfo.IsInstanceOpen)
		{
			NKCPopupFriendInfo.Instance.Close();
		}
		NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_BAN_ACK.errorCode);
	}

	public static void OnRecv(NKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_ACK cNKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_ACK)
	{
		if (cNKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_ACK.errorCode != NKM_ERROR_CODE.NEC_OK && NKCPopupFriendInfo.IsInstanceOpen)
		{
			NKCPopupFriendInfo.Instance.Close();
		}
		NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_ACK.errorCode);
	}

	public static void OnRecv(NKMPacket_GUILD_MASTER_MIGRATION_ACK cNKMPacket_GUILD_MASTER_MIGRATION_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_MASTER_MIGRATION_ACK.errorCode))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_CONFIRM_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_CONFIRM_POPUP_BODY_DESC);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_UPDATE_DATA_ACK cNKMPacket_GUILD_UPDATE_DATA_ACK)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_UPDATE_DATA_ACK.errorCode);
	}

	public static void OnRecv(NKMPacket_GUILD_UPDATE_NOTICE_ACK cNKMPacket_GUILD_UPDATE_NOTICE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_UPDATE_NOTICE_ACK.errorCode))
		{
			NKCGuildManager.ShowChatNotice(bValue: true);
			NKCGuildManager.SetLastGuildNoticeChangedTimeUTC(NKCSynchronizedTime.GetServerUTCTime());
		}
	}

	public static void OnRecv(NKMPacket_GUILD_UPDATE_MEMBER_GREETING_ACK cNKMPacket_GUILD_UPDATE_MEMBER_GREETING_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_UPDATE_MEMBER_GREETING_ACK.errorCode))
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_INTRODUCE_WRITE_SUCCESS_TOAST_MESSAGE_TEXT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
	}

	public static void OnRecv(NKMPacket_GUILD_ATTENDANCE_ACK cNKMPacket_GUILD_ATTENDANCE_ACK)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_ATTENDANCE_ACK.errorCode))
		{
			return;
		}
		NKMUserData userData = NKCScenManager.CurrentUserData();
		if (userData != null)
		{
			userData.GetReward(cNKMPacket_GUILD_ATTENDANCE_ACK.rewardData, cNKMPacket_GUILD_ATTENDANCE_ACK.additionalReward);
			NKMGuildMemberData nKMGuildMemberData = NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == userData.m_UserUID);
			if (nKMGuildMemberData != null)
			{
				nKMGuildMemberData.lastAttendanceDate = cNKMPacket_GUILD_ATTENDANCE_ACK.lastAttendanceDate;
			}
			if (cNKMPacket_GUILD_ATTENDANCE_ACK.rewardData != null)
			{
				NKCUIResult.Instance.OpenRewardGain(userData.m_ArmyData, cNKMPacket_GUILD_ATTENDANCE_ACK.rewardData, cNKMPacket_GUILD_ATTENDANCE_ACK.additionalReward, NKCUtilString.GET_STRING_CONSORTIUM_POPUP_ATTENDANCE_REWARD_TITLE);
			}
		}
		NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_CONSORTIUM_ATTENDANCE_SUCCESS_TOAST_MESSAGE_TEXT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		NKCUIManager.OnGuildDataChanged();
	}

	public static void OnRecv(NKMPacket_GUILD_RECOMMEND_INVITE_LIST_ACK cNKMPacket_GUILD_RECOMMEND_INVITE_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_RECOMMEND_INVITE_LIST_ACK.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().OnRecv(cNKMPacket_GUILD_RECOMMEND_INVITE_LIST_ACK.list);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DONATION_ACK cNKMPacket_GUILD_DONATION_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_DONATION_ACK.errorCode))
		{
			GuildDonationTemplet guildDonationTemplet = GuildDonationTemplet.Find(cNKMPacket_GUILD_DONATION_ACK.donationId);
			if (guildDonationTemplet != null)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DONATION_SUCCESS_TOAST_TEXT, NKCStringTable.GetString(guildDonationTemplet.DonateText)), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
			NKCGuildManager.MyData.donationCount = cNKMPacket_GUILD_DONATION_ACK.donationCount;
			NKCGuildManager.MyData.lastDailyResetDate = cNKMPacket_GUILD_DONATION_ACK.lastDailyResetDate;
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_GUILD_DONATION_ACK.costItemDataList);
				nKMUserData.GetReward(cNKMPacket_GUILD_DONATION_ACK.rewardData);
				NKCUIResult.Instance.OpenComplexResultFull(nKMUserData.m_ArmyData, cNKMPacket_GUILD_DONATION_ACK.rewardData, cNKMPacket_GUILD_DONATION_ACK.additionalReward, null, 0L);
			}
		}
	}

	public static void OnRecv(NKMPacket_GUILD_BUY_BUFF_ACK cNKMPacket_GUILD_BUY_BUFF_ACK)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (cNKMPacket_GUILD_BUY_BUFF_ACK.errorCode == NKM_ERROR_CODE.NEC_FAIL_GUILD_NOT_ENOUGH_UNION_POINT)
		{
			if (nKMUserData != null)
			{
				NKCGuildManager.MyGuildData.unionPoint = cNKMPacket_GUILD_BUY_BUFF_ACK.unionPoint;
			}
			NKCUIManager.OnGuildDataChanged();
		}
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_BUY_BUFF_ACK.errorCode))
		{
			if (nKMUserData != null)
			{
				nKMUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_GUILD_BUY_BUFF_ACK.costItemDataList);
				nKMUserData.GetReward(cNKMPacket_GUILD_BUY_BUFF_ACK.rewardData);
				NKCGuildManager.MyGuildData.unionPoint = cNKMPacket_GUILD_BUY_BUFF_ACK.unionPoint;
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_GUILD_BUY_WELFARE_POINT_ACK cNKMPacket_GUILD_BUY_WELFARE_POINT_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_BUY_WELFARE_POINT_ACK.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_GUILD_BUY_WELFARE_POINT_ACK.costItemDataList);
				nKMUserData.GetReward(cNKMPacket_GUILD_BUY_WELFARE_POINT_ACK.rewardData);
			}
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(string.Format(NKCStringTable.GetString("SI_DP_SHOP_ITEM_BUY_MESSAGE_ELSE"), NKMItemManager.GetItemMiscTempletByID(23).GetItemName()), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCGuildManager.MyGuildData != null && NKCGuildManager.MyGuildData.guildUid == sPacket.guildUid)
		{
			NKCGuildManager.MyGuildData.dungeonNotice = sPacket.notice;
			NKCGuildManager.ShowChatNotice(bValue: true);
			NKCGuildManager.SetLastDungeonNoticeChangedTimeUTC(NKCSynchronizedTime.GetServerUTCTime());
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_COOP)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshDungeonNotice();
			}
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_NOT sPacket)
	{
		NKCPopupMessageGuild.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_LOBBY_INFORMATION_CHANGE_OVERLAY_TITLE_TEXT, NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_INFORMATION_CHANGE_OVERLAY_BODY_TEXT, bIsGoodNews: true);
		if (NKCGuildManager.MyGuildData != null && NKCGuildManager.MyGuildData.guildUid == sPacket.guildUid)
		{
			NKCGuildManager.MyGuildData.dungeonNotice = sPacket.notice;
			NKCGuildManager.ShowChatNotice(bValue: true);
			NKCGuildManager.SetLastDungeonNoticeChangedTimeUTC(NKCSynchronizedTime.GetServerUTCTime());
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_COOP)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshDungeonNotice();
			}
		}
	}

	public static void OnRecv(NKMPacket_GUILD_RENAME_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCPopupGuildNameChange.IsInstanceOpen)
		{
			NKCPopupGuildNameChange.Instance.Close();
		}
	}

	public static void OnRecv(NKMPacket_GUILD_RENAME_NOT sPacket)
	{
		if (NKCGuildManager.MyGuildData != null && NKCGuildManager.MyGuildData.guildUid > 0 && NKCGuildManager.MyGuildData.guildUid == sPacket.guildUid)
		{
			NKCGuildManager.MyGuildData.name = sPacket.newName;
		}
		NKCUIManager.OnGuildDataChanged();
		string message = ((NKMCommonConst.Guild != null) ? NKCStringTable.GetString(NKMCommonConst.Guild.SystemChatRename) : NKCUtilString.GET_STRING_CONSORTIUM_NAME_CHANGE_NOTICE);
		NKCPopupMessageGuild.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_LOBBY_INFORMATION_CHANGE_OVERLAY_TITLE_TEXT, message, bIsGoodNews: true);
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_INFO_ACK sPacket)
	{
		Debug.Log("[GuildDungeon] NKMPacket_GUILD_DUNGEON_INFO_ACK");
		Debug.Log($"[GuildDungeon] ErrorCode  [{sPacket.errorCode}]");
		Debug.Log($"[GuildDungeon] SeasonId  [{sPacket.seasonId}]");
		Debug.Log($"[GuildDungeon] SessionId  [{sPacket.sessionId}]");
		Debug.Log($"[GuildDungeon] GuildDungeonState  [{sPacket.guildDungeonState}]");
		Debug.Log($"[GuildDungeon] CurrentSessionEndDate  [{sPacket.currentSessionEndDate}]");
		Debug.Log($"[GuildDungeon] m_NextSessionStartDateUTC  [{sPacket.NextSessionStartDate}]");
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCGuildCoopManager.OnRecv(sPacket);
			NKCPublisherModule.Push.UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP.GUILD_DUNGEON_NOTIFY, bForce: true);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().SetCoopDataRecved(bValue: true);
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_COOP)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().Refresh();
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_RAID_READY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID_READY().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_MEMBER_INFO_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCGuildCoopManager.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_SEASON_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			nKMUserData.GetReward(sPacket.rewardData);
			NKCGuildCoopManager.OnRecv(sPacket);
			NKCUIResult.Instance.OpenComplexResult(nKMUserData.m_ArmyData, sPacket.rewardData, delegate
			{
				NKCPopupGuildCoopSeasonReward.Instance.RefreshUI();
			}, 0L);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_TICKET_BUY_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData()?.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
			NKCGuildCoopManager.SetArenaTicketBuyCount(sPacket.currentTicketBuyCount);
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_PLAY_COUNT_BUY_SUCCESS_TEXT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			if (NKCUIPrepareEventDeck.IsInstanceOpen)
			{
				NKCUIPrepareEventDeck.Instance.RefreshUIByContents();
			}
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_SESSION_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMRewardData nKMRewardData = new NKMRewardData();
		if (sPacket.rewardList != null)
		{
			nKMRewardData.SetMiscItemData(sPacket.rewardList);
		}
		NKMRewardData nKMRewardData2 = new NKMRewardData();
		if (sPacket.artifactReward != null)
		{
			for (int i = 0; i < sPacket.artifactReward.Count; i++)
			{
				nKMRewardData2.Upsert(sPacket.artifactReward[i]);
			}
		}
		nKMUserData.GetReward(nKMRewardData);
		nKMUserData.GetReward(nKMRewardData2);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_COOP)
		{
			NKCGuildCoopManager.SetRewarded();
			NKCGuildCoopManager.AddMyPoint(GuildDungeonRewardCategory.RANK, sPacket.clearPoint);
			NKCPopupGuildCoopSessionResult.Instance.Open(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_ARENA_PLAY_NOT sPacket)
	{
		NKCGuildCoopManager.SetArenaPlayStart(sPacket.arenaId, sPacket.userUid);
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_BOSS_PLAY_NOT sPacket)
	{
		NKCGuildCoopManager.SetBossPlayStart(sPacket.userUid);
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_ARENA_PLAY_END_NOT sPacket)
	{
		NKCGuildCoopManager.SetArenaPlayEnd(sPacket);
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_ARENA_PLAY_CANCEL_NOT sPacket)
	{
		NKCGuildCoopManager.SetArenaPlayCancel(sPacket);
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_BOSS_PLAY_END_NOT sPacket)
	{
		NKCGuildCoopManager.SetBossPlayEnd(sPacket);
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_BOSS_PLAY_CANCEL_NOT sPacket)
	{
		NKCGuildCoopManager.SetBossPlayCancel(sPacket);
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_FLAG_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCGuildCoopManager.SetArenaFlag(sPacket.arenaIndex, sPacket.flagIndex);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_COOP)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshArenaSlot(sPacket.arenaIndex);
			}
			if (NKCPopupGuildCoopArtifactList.IsInstanceOpen)
			{
				NKCPopupGuildCoopArtifactList.Instance.Refresh();
			}
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_ARENA_FLAG_NOT sPacket)
	{
		NKCGuildCoopManager.SetArenaFlag(sPacket.arenaIndex, sPacket.flagIndex);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_COOP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshArenaSlot(sPacket.arenaIndex);
		}
		if (NKCPopupGuildCoopArtifactList.IsInstanceOpen)
		{
			NKCPopupGuildCoopArtifactList.Instance.Refresh();
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_BOSS_ORDER_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCGuildCoopManager.SetBossOrderIndex(sPacket.orderIndex);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_BOSS_ORDER_NOT sPacket)
	{
		NKCGuildCoopManager.SetBossOrderIndex(sPacket.orderIndex);
	}

	public static void OnRecv(NKMPacket_GUILD_CHAT_NOT cNKMPacket_GUILD_CHAT_NOT)
	{
		NKCChatManager.OnRecv(cNKMPacket_GUILD_CHAT_NOT);
	}

	public static void OnRecv(NKMPacket_GUILD_CHAT_LIST_NOT cNKMPacket_GUILD_CHAT_LIST_NOT)
	{
		NKCChatManager.OnRecvGuildChatList(cNKMPacket_GUILD_CHAT_LIST_NOT.guildUid, cNKMPacket_GUILD_CHAT_LIST_NOT.messages, bRefreshUI: true);
	}

	public static void OnRecv(NKMPacket_BLOCK_MUTE_NOT cNKMPacket_BLOCK_MUTE_NOT)
	{
		if (NKCScenManager.CurrentUserData().m_UserUID == cNKMPacket_BLOCK_MUTE_NOT.userUid)
		{
			NKCChatManager.SetMuteEndDate(cNKMPacket_BLOCK_MUTE_NOT.endDate);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_CHAT_ACK cNKMPacket_GUILD_CHAT_ACK)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_CHAT_ACK.errorCode);
	}

	public static void OnRecv(NKMPacket_GUILD_CHAT_LIST_ACK cNKMPacket_GUILD_CHAT_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_CHAT_LIST_ACK.errorCode))
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().SetChatDataRecved(bValue: true);
			}
			NKCChatManager.OnRecvGuildChatList(cNKMPacket_GUILD_CHAT_LIST_ACK.guildUid, cNKMPacket_GUILD_CHAT_LIST_ACK.messages, bRefreshUI: false);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_CHAT_COMPLAIN_ACK cNKMPacket_GUILD_CHAT_COMPLAIN_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GUILD_CHAT_COMPLAIN_ACK.errorCode))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONSORTIUM_CHAT_REPORT_CONFIRM_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_CHAT_REPORT_CONFIRM_POPUP_BODY_DESC);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_CHAT_NOT sPacket)
	{
		NKCChatManager.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_PRIVATE_CHAT_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCChatManager.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_CHAT_ACK sPacket)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode);
	}

	public static void OnRecv(NKMPacket_PRIVATE_CHAT_ALL_LIST_ACK sPacket)
	{
		if (sPacket.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			NKCChatManager.OnRecvAllChat(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_ACHIEVE_LIST_ACK cNKMPacket_LEADERBOARD_ACHIEVE_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_LEADERBOARD_ACHIEVE_LIST_ACK.errorCode))
		{
			NKCLeaderBoardManager.OnRecv(cNKMPacket_LEADERBOARD_ACHIEVE_LIST_ACK);
		}
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_SHADOWPALACE_LIST_ACK cNKMPacket_LEADERBOARD_SHADOWPALACE_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_LEADERBOARD_SHADOWPALACE_LIST_ACK.errorCode))
		{
			NKCLeaderBoardManager.OnRecv(cNKMPacket_LEADERBOARD_SHADOWPALACE_LIST_ACK);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_SHADOW_PALACE && NKCUIShadowPalace.IsInstanceOpen)
			{
				NKCUIShadowPalace.GetInstance().OpenRank();
			}
		}
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_FIERCE_LIST_ACK cNKMPacket_LEADERBOARD_FIERCE_LIST_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_LEADERBOARD_FIERCE_LIST_ACK.errorCode))
		{
			NKCLeaderBoardManager.OnRecv(cNKMPacket_LEADERBOARD_FIERCE_LIST_ACK);
		}
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_GUILD_LEVEL_RANK_LIST_ACK sPacket)
	{
		NKCLeaderBoardManager.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_ACK sPacket)
	{
		NKCLeaderBoardManager.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_TIMEATTACK_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaderBoardManager.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_SHADOW_PALACE_START_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			nKMUserData.m_ShadowPalace.currentPalaceId = sPacket.currentPalaceId;
			nKMUserData.m_ShadowPalace.life = 3;
			nKMUserData.m_ShadowPalace.rewardMultiply = sPacket.rewardMultiply;
			NKMPalaceData nKMPalaceData = nKMUserData.m_ShadowPalace.palaceDataList.Find((NKMPalaceData v) => v.palaceId == sPacket.currentPalaceId);
			if (nKMPalaceData == null)
			{
				nKMPalaceData = new NKMPalaceData();
				nKMPalaceData.palaceId = sPacket.currentPalaceId;
				nKMUserData.m_ShadowPalace.palaceDataList.Add(nKMPalaceData);
			}
			nKMPalaceData.currentDungeonId = 0;
			for (int num = 0; num < nKMPalaceData.dungeonDataList.Count; num++)
			{
				nKMPalaceData.dungeonDataList[num].recentTime = 0;
			}
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_SHADOW_BATTLE().SetShadowPalaceID(sPacket.currentPalaceId);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_BATTLE);
	}

	public static void OnRecv(NKMPacket_SHADOW_PALACE_GIVEUP_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.m_ShadowPalace.currentPalaceId = 0;
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_PALACE);
		}
	}

	public static void OnRecv(NKMPacket_SHADOW_PALACE_SKIP_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		myUserData?.m_InventoryData.UpdateItemInfo(sPacket.costItems);
		if (sPacket.rewardDatas != null)
		{
			foreach (NKMRewardData rewardData in sPacket.rewardDatas)
			{
				myUserData.GetReward(rewardData);
			}
		}
		NKCPopupOpSkipProcess.Instance.Open(sPacket.rewardDatas, new List<UnitLoyaltyUpdateData>());
	}

	public static void OnRecv(NKMPacket_FIERCE_DATA_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr()?.UpdateFierceData(sPacket);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME()?.UpdateRightSide3DButton(NKCUILobbyV2.eUIMenu.Worldmap);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT)
		{
			return;
		}
		NKC_SCEN_FIERCE_BATTLE_SUPPORT nKC_SCEN_FIERCE_BATTLE_SUPPORT = NKCScenManager.GetScenManager().Get_NKC_SCEN_FIERCE_BATTLE_SUPPORT();
		if (nKC_SCEN_FIERCE_BATTLE_SUPPORT != null)
		{
			if (nKC_SCEN_FIERCE_BATTLE_SUPPORT.Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_DATA_REQ_WAIT)
			{
				nKC_SCEN_FIERCE_BATTLE_SUPPORT.ScenDataReqWaitUpdate();
				nKC_SCEN_FIERCE_BATTLE_SUPPORT.SetDataReq(bReceived: true);
			}
			else
			{
				nKC_SCEN_FIERCE_BATTLE_SUPPORT.ScenUpdate();
			}
		}
	}

	public static void OnRecv(NKMPacket_FIERCE_SEASON_NOT sPacket)
	{
		NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr()?.Init(sPacket.fierceId);
	}

	public static void OnRecv(NKMPacket_FIERCE_PROFILE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKM_SCEN_ID nowScenID = NKCScenManager.GetScenManager().GetNowScenID();
			if (nowScenID == NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT || nowScenID == NKM_SCEN_ID.NSI_HOME)
			{
				NKCPopupFierceUserInfo.Instance.Open(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr()?.UpdateFierceData(sPacket);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_FIERCE_BATTLE_SUPPORT().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_FIERCE_COMPLETE_RANK_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (sPacket.rewardData != null)
			{
				myUserData?.GetReward(sPacket.rewardData);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT)
			{
				NKCUIPopupFierceBattleRankReward.Instance.Open(sPacket.rewardData);
			}
			NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr()?.SetReceivedRankReward();
		}
	}

	public static void OnRecv(NKMPacket_FIERCE_COMPLETE_POINT_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			if (myUserData.m_InventoryData != null)
			{
				if (sPacket.rewardData != null && sPacket.rewardData.MiscItemDataList != null)
				{
					myUserData.m_InventoryData.AddItemMisc(sPacket.rewardData.MiscItemDataList);
				}
				if (sPacket.rewardData != null && sPacket.rewardData.EquipItemDataList != null)
				{
					myUserData.m_InventoryData.AddItemEquip(sPacket.rewardData.EquipItemDataList);
				}
			}
			if (sPacket.rewardData != null && sPacket.rewardData.MoldItemDataList != null)
			{
				foreach (NKMMoldItemData moldItemData in sPacket.rewardData.MoldItemDataList)
				{
					if (NKMItemManager.GetItemMoldTempletByID(moldItemData.m_MoldID) != null)
					{
						myUserData.m_CraftData.UpdateMoldItem(moldItemData.m_MoldID, moldItemData.m_Count);
					}
				}
			}
		}
		NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr()?.UpdateRecevePointRewardID(sPacket.pointRewardId);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT)
		{
			if (NKCUIPopupFierceBattleScoreReward.IsInstanceOpen)
			{
				NKCUIPopupFierceBattleScoreReward.Instance.Open();
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FIERCE_BATTLE_SUPPORT().OnRecv(sPacket);
		}
		NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentUserData().m_ArmyData, sPacket.rewardData, NKCUtilString.GET_FIERCE_BATTLE_POINT_REWARD_TITLE);
	}

	public static void OnRecv(NKMPacket_FIERCE_COMPLETE_POINT_REWARD_ALL_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			if (myUserData.m_InventoryData != null)
			{
				if (sPacket.rewardData != null && sPacket.rewardData.MiscItemDataList != null)
				{
					myUserData.m_InventoryData.AddItemMisc(sPacket.rewardData.MiscItemDataList);
				}
				if (sPacket.rewardData != null && sPacket.rewardData.EquipItemDataList != null)
				{
					myUserData.m_InventoryData.AddItemEquip(sPacket.rewardData.EquipItemDataList);
				}
			}
			if (sPacket.rewardData != null && sPacket.rewardData.MoldItemDataList != null)
			{
				foreach (NKMMoldItemData moldItemData in sPacket.rewardData.MoldItemDataList)
				{
					if (NKMItemManager.GetItemMoldTempletByID(moldItemData.m_MoldID) != null)
					{
						myUserData.m_CraftData.UpdateMoldItem(moldItemData.m_MoldID, moldItemData.m_Count);
					}
				}
			}
		}
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr != null)
		{
			foreach (int pointRewardId in sPacket.pointRewardIds)
			{
				nKCFierceBattleSupportDataMgr.UpdateRecevePointRewardID(pointRewardId);
			}
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT)
		{
			if (NKCUIPopupFierceBattleScoreReward.IsInstanceOpen)
			{
				NKCUIPopupFierceBattleScoreReward.Instance.Open();
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FIERCE_BATTLE_SUPPORT().OnRecv(sPacket);
		}
		NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentUserData().m_ArmyData, sPacket.rewardData, NKCUtilString.GET_FIERCE_BATTLE_POINT_REWARD_TITLE);
	}

	public static void OnRecv(NKMPacket_FIERCE_PENALTY_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr()?.SetSelfPenalty(sPacket.penaltyIds);
		}
	}

	public static void OnRecv(NKMPacket_UPDATE_MARKET_REVIEW_ACK sPacket)
	{
		if (sPacket.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			NKCPublisherModule.Marketing.SetMarketReviewCompleted();
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PASS_NOT sPacket)
	{
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager != null)
		{
			eventPassDataManager.EventPassId = sPacket.eventPassId;
			eventPassDataManager.EventPassDataReceived = false;
		}
		NKCUIEventPass.RewardRedDot = false;
		NKCUIEventPass.DailyMissionRedDot = false;
		NKCUIEventPass.WeeklyMissionRedDot = false;
		NKCUIEventPass.EventPassDataManager = null;
		NKCPacketSender.Send_NKMPacket_EVENT_PASS_REQ(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
	}

	public static void OnRecv(NKMPacket_EVENT_PASS_ACK cNKMPacket_EVENT_PASS_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EVENT_PASS_ACK.errorCode))
		{
			NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
			eventPassDataManager?.SetEventPassData(cNKMPacket_EVENT_PASS_ACK);
			NKCUILobbyV2 openedUIByType = NKCUIManager.GetOpenedUIByType<NKCUILobbyV2>();
			if (openedUIByType != null)
			{
				openedUIByType.UpdateButton(NKCUILobbyV2.eUIMenu.CounterPass, NKCScenManager.CurrentUserData());
			}
			if (NKCUIEventPass.OpenUIStandby)
			{
				NKCUIEventPass.Instance.Open(eventPassDataManager);
				NKCUIEventPass.OpenUIStandby = false;
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK cNKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData()?.GetReward(cNKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK.rewardData);
			if (NKCUIEventPass.IsInstanceOpen)
			{
				NKCUIEventPass.Instance.OnRecv(cNKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK);
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PASS_MISSION_ACK cNKMPacket_EVENT_PASS_MISSION_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EVENT_PASS_MISSION_ACK.errorCode) && NKCUIEventPass.HasInstance)
		{
			NKCUIEventPass.Instance.OnRecv(cNKMPacket_EVENT_PASS_MISSION_ACK);
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_ACK cNKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_ACK.errorCode) && NKCUIEventPass.IsInstanceOpen)
		{
			NKCUIEventPass.Instance.RefreshFinalMissionCompleted(cNKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_ACK);
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_ACK cNKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.UpdateItemInfo(cNKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_ACK.costItems);
			if (NKCUIEventPass.IsInstanceOpen)
			{
				NKCUIEventPass.Instance.RefreshSelectedDailyMissionSlot(cNKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_ACK.missionInfo);
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_ACK cNKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.UpdateItemInfo(cNKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_ACK.costItemList);
			if (NKCPopupEventPassPurchase.IsInstanceOpen)
			{
				NKCPopupEventPassPurchase.Instance.Close();
			}
			NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
			if (eventPassDataManager != null)
			{
				eventPassDataManager.CorePassPurchased = true;
			}
			NKCMMPManager.OnCustomEvent("38_counterpass_sp");
			if (NKCUIEventPass.IsInstanceOpen)
			{
				NKCUIEventPass.Instance.RefreshPurchaseCorePass(corePassPurchased: true);
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_ACK cNKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.UpdateItemInfo(cNKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_ACK.costItemList);
			if (NKCPopupEventPassPurchase.IsInstanceOpen)
			{
				NKCPopupEventPassPurchase.Instance.Close();
			}
			NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
			if (eventPassDataManager != null)
			{
				eventPassDataManager.CorePassPurchased = true;
			}
			NKCMMPManager.OnCustomEvent("38_counterpass_spplus");
			if (NKCUIEventPass.IsInstanceOpen)
			{
				NKCUIEventPass.Instance.RefreshPurchaseCorePass(corePassPurchased: true, cNKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_ACK.totalExp);
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_PASS_DOT_NOT cNKMPacket_EVENT_PASS_LOBBY_DOT_NOT)
	{
		NKCUIEventPass.RewardRedDot = cNKMPacket_EVENT_PASS_LOBBY_DOT_NOT.passLevelDot;
		NKCUIEventPass.DailyMissionRedDot = cNKMPacket_EVENT_PASS_LOBBY_DOT_NOT.dailyMissionDot;
		NKCUIEventPass.WeeklyMissionRedDot = cNKMPacket_EVENT_PASS_LOBBY_DOT_NOT.weeklyMissionDot;
	}

	public static void OnRecv(NKMPacket_EVENT_PASS_LEVEL_UP_ACK cNKMPacket_EVENT_PASS_LEVEL_UP_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_EVENT_PASS_LEVEL_UP_ACK.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.UpdateItemInfo(cNKMPacket_EVENT_PASS_LEVEL_UP_ACK.costItemList);
			if (NKCUIEventPass.IsInstanceOpen)
			{
				NKCUIEventPass.Instance.RefreshPassTotalExpRelatedInfo(cNKMPacket_EVENT_PASS_LEVEL_UP_ACK.totalExp, initScrollPosition: false);
			}
			NKCMMPManager.OnCustomEvent("38_counterpass_lvup");
		}
	}

	public static void OnRecv(NKMPacket_OPERATOR_LEVELUP_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().m_InventoryData.UpdateItemInfo(sPacket.costItemData);
			NKCScenManager.CurrentUserData().m_ArmyData.UpdateOperatorData(sPacket.operatorUnit);
		}
	}

	public static void OnRecv(NKMPacket_OPERATOR_ENHANCE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().m_InventoryData.UpdateItemInfo(sPacket.costItemDatas);
		NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(sPacket.operatorUnit.uid);
		bool bTryMainSkill = false;
		bool bMainSkillLvUp = false;
		bool bTrySubskill = false;
		bool bSubskillLvUp = false;
		bool bImplantSubskill = false;
		if (sPacket.sourceUnitUid != 0L)
		{
			NKMOperator operatorData2 = NKCOperatorUtil.GetOperatorData(sPacket.sourceUnitUid);
			if (operatorData2.mainSkill.id == sPacket.operatorUnit.mainSkill.id && !NKCOperatorUtil.IsMaximumSkillLevel(operatorData.mainSkill.id, operatorData.mainSkill.level))
			{
				bTryMainSkill = true;
				bMainSkillLvUp = operatorData.mainSkill.level < sPacket.operatorUnit.mainSkill.level;
			}
			if (operatorData.subSkill.id == operatorData2.subSkill.id && !NKCOperatorUtil.IsMaximumSkillLevel(operatorData.subSkill.id, operatorData.subSkill.level))
			{
				bTrySubskill = true;
				bSubskillLvUp = operatorData.subSkill.level < sPacket.operatorUnit.subSkill.level;
			}
		}
		else if (sPacket.tokenItemId != 0)
		{
			int operatorSkill = NKCOperatorUtil.GetOperatorSkill(sPacket.tokenItemId);
			if (operatorData.subSkill.id == operatorSkill && !NKCOperatorUtil.IsMaximumSkillLevel(operatorData.subSkill.id, operatorData.subSkill.level))
			{
				bTrySubskill = true;
				bSubskillLvUp = operatorData.subSkill.level < sPacket.operatorUnit.subSkill.level;
			}
		}
		if (sPacket.transSkill)
		{
			bImplantSubskill = operatorData.subSkill.id != sPacket.operatorUnit.subSkill.id;
		}
		int id = operatorData.subSkill.id;
		int level = operatorData.subSkill.level;
		NKCScenManager.CurrentUserData()?.m_ArmyData.RemoveOperatorEx(sPacket.sourceUnitUid);
		NKCScenManager.CurrentUserData().m_ArmyData.UpdateOperatorData(sPacket.operatorUnit);
		if (NKCUIOperatorInfoPopupSkill.Instance.IsOpen)
		{
			NKCUIOperatorInfoPopupSkill.Instance.OnRecv(bTryMainSkill, bMainSkillLvUp, bTrySubskill, bSubskillLvUp, sPacket.transSkill, bImplantSubskill, id, level, sPacket.tokenItemId);
		}
	}

	public static void OnRecv(NKMPacket_OPERATOR_LOCK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCOperatorUtil.UpdateLockState(sPacket.unitUID, sPacket.locked);
			NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(sPacket.unitUID);
			NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData?.UpdateOperatorData(operatorData);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_UNIT_LIST && NKCUIOperatorInfo.IsInstanceOpen)
			{
				NKCUIOperatorInfo.Instance.UpdateLockState(sPacket.unitUID);
			}
		}
	}

	public static void OnRecv(NKMPacket_OPERATOR_REMOVE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		nKMUserData.m_InventoryData.AddItemMisc(sPacket.rewardItemDataList);
		NKMArmyData armyData = nKMUserData.m_ArmyData;
		armyData.RemoveOperator(sPacket.removeUnitUIDList);
		armyData.AddUnitDeleteRewardList(sPacket.rewardItemDataList);
		if (!armyData.IsEmptyUnitDeleteList)
		{
			NKCPacketSender.Send_NKMPacket_OPERATOR_REMOVE_REQ();
			return;
		}
		if (NKCUIUnitSelectList.IsInstanceOpen)
		{
			NKCUIUnitSelectList.Instance.CloseRemoveMode();
			NKCUIUnitSelectList.Instance.ClearMultipleSelect();
			NKCUINPCMachineGap.PlayVoice(NPC_TYPE.MACHINE_GAP, NPC_ACTION_TYPE.DISMISSAL_RESULT);
		}
		if (armyData.GetUnitDeleteReward().Count > 0)
		{
			NKCUIResult.Instance.OpenItemGain(armyData.GetUnitDeleteReward(), NKCUtilString.GET_STRING_ITEM_GAIN, NKCUtilString.GET_STRING_REMOVE_UNIT);
		}
	}

	public static void OnRecv(NKMPacket_DECK_OPERATOR_SET_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			long num = 0L;
			NKMOperator deckOperatorByIndex = armyData.GetDeckOperatorByIndex(sPacket.deckIndex);
			if (deckOperatorByIndex != null)
			{
				num = deckOperatorByIndex.uid;
			}
			armyData.SetDeckOperatorByIndex(sPacket.oldDeckIndex.m_eDeckType, sPacket.oldDeckIndex.m_iIndex, 0L);
			armyData.SetDeckOperatorByIndex(sPacket.deckIndex.m_eDeckType, sPacket.deckIndex.m_iIndex, sPacket.operatorUid);
			if (NKCUIDeckViewer.IsInstanceOpen)
			{
				NKCUIDeckViewer.Instance.OnRecv(sPacket);
			}
			if (NKCUIUnitSelectList.IsInstanceOpen && num != 0L)
			{
				NKCUIUnitSelectList.Instance.ChangeUnitDeckIndex(num, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE));
			}
		}
	}

	public static void OnRecv(NKMPacket_OPERATOR_EXTRACT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			nKMUserData.m_InventoryData.AddItemMisc(sPacket.rewardItemDatas);
			nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemDatas);
			NKMArmyData armyData = nKMUserData.m_ArmyData;
			armyData.RemoveOperator(sPacket.extractUnitUids);
			armyData.AddUnitDeleteRewardList(sPacket.rewardItemDatas);
			if (NKCPopupOperatorExtract.IsInstanceOpen)
			{
				NKCPopupOperatorExtract.Instance.Close();
			}
			if (NKCUIUnitSelectList.IsInstanceOpen)
			{
				NKCUIUnitSelectList.Instance.CloseExtractMode();
				NKCUIUnitSelectList.Instance.ClearMultipleSelect();
			}
			if (armyData.GetUnitDeleteReward().Count > 0)
			{
				NKCUIResult.Instance.OpenItemGain(armyData.GetUnitDeleteReward(), NKCUtilString.GET_STRING_ITEM_GAIN, NKCUtilString.GET_STRING_REMOVE_UNIT);
			}
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_PRESET_LIST_ACK cPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode) && NKCScenManager.GetScenManager().GetMyUserData() != null)
		{
			NKCEquipPresetDataManager.ListEquipPresetData = cPacket.presetDatas;
			NKCEquipPresetDataManager.RefreshEquipUidHash();
			if (NKCEquipPresetDataManager.OpenUI)
			{
				NKCUIUnitInfo.Instance.OnRecv(cPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_PRESET_ADD_ACK cPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.UpdateItemInfo(cPacket.costItemDataList);
			NKCUIUnitInfo.Instance.EquipPreset?.AddPresetSlot(cPacket.totalPresetCount);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_PRESET_CHANGE_NAME_ACK cPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			NKCUIUnitInfo.Instance.EquipPreset?.UpdatePresetName(cPacket.presetIndex, cPacket.newPresetName);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_PRESET_REGISTER_ALL_ACK cPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			NKCUIUnitInfo.Instance.EquipPreset?.UpdatePresetSlot(cPacket.presetData, registerAll: true);
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_PRESET_REGISTER_ACK cPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			if (NKCUIInventory.IsInstanceOpen)
			{
				NKCUIInventory.Instance.Close();
			}
			NKCUIUnitInfo.Instance.EquipPreset?.UpdatePresetSlot(cPacket.presetData);
			NKCPopupItemEquipBox.CloseItemBox();
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_PRESET_APPLY_ACK cPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		bool flag = false;
		int count = cPacket.updateUnitDatas.Count;
		for (int i = 0; i < count; i++)
		{
			if (cPacket.updateUnitDatas[i] == null)
			{
				Debug.LogWarning($"updateUnitDatas {i} index is null");
				continue;
			}
			NKMPacket_EQUIP_PRESET_APPLY_ACK.UnitEquipUidSet unitEquipUidSet = cPacket.updateUnitDatas[i];
			NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(unitEquipUidSet.unitUid);
			if (unitFromUID == null)
			{
				continue;
			}
			List<NKMUnitData> list = new List<NKMUnitData>();
			int count2 = unitEquipUidSet.equipUids.Count;
			for (int j = 0; j < count2; j++)
			{
				ITEM_EQUIP_POSITION iTEM_EQUIP_POSITION = (ITEM_EQUIP_POSITION)j;
				long equipUid = unitFromUID.GetEquipUid(iTEM_EQUIP_POSITION);
				if (unitEquipUidSet.equipUids[j] <= 0)
				{
					if (equipUid > 0 && !unitFromUID.UnEquipItem(myUserData.m_InventoryData, equipUid, iTEM_EQUIP_POSITION))
					{
						flag = true;
					}
				}
				else
				{
					if (unitEquipUidSet.equipUids[j] == equipUid)
					{
						continue;
					}
					NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(unitEquipUidSet.equipUids[j]);
					if (itemEquip.m_OwnerUnitUID > 0)
					{
						NKMUnitData removeItemUnitData = myUserData.m_ArmyData.GetUnitFromUID(itemEquip.m_OwnerUnitUID);
						if (!removeItemUnitData.UnEquipItem(myUserData.m_InventoryData, unitEquipUidSet.equipUids[j]))
						{
							flag = true;
						}
						if (list.Find((NKMUnitData e) => e.m_UnitUID == removeItemUnitData.m_UnitUID) == null)
						{
							list.Add(removeItemUnitData);
						}
					}
					if (!unitFromUID.EquipItem(myUserData.m_InventoryData, unitEquipUidSet.equipUids[j], out var _, iTEM_EQUIP_POSITION))
					{
						flag = true;
					}
				}
			}
			int count3 = list.Count;
			for (int num = 0; num < count3; num++)
			{
				if (list[num].m_UnitUID != unitFromUID.m_UnitUID)
				{
					myUserData.m_ArmyData.UpdateUnitData(list[num]);
				}
			}
			myUserData.m_ArmyData.UpdateUnitData(unitFromUID);
		}
		NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_GROWTH_EQUIP, NKCUIUnitInfo.Instance.GetNKMUnitData());
		if (flag)
		{
			Debug.LogWarning("Some of EquipPreset apply failed");
		}
		NKCUIUnitInfo.Instance.EquipPreset?.UpdatePresetData(null, setScrollPositon: false);
	}

	public static void OnRecv(NKMPacket_EQUIP_PRESET_NOT cPacket)
	{
		if (NKCUIUnitInfo.IsInstanceOpen && NKCUIUnitInfo.Instance.EquipPresetOpened())
		{
			NKCUIUnitInfo.Instance.EquipPreset?.UpdatePresetData(cPacket.presetDatas, setScrollPositon: false, 0, forceRefresh: true);
			return;
		}
		if (cPacket.presetDatas != null)
		{
			NKCEquipPresetDataManager.ListEquipPresetData = cPacket.presetDatas;
		}
		NKCEquipPresetDataManager.RefreshEquipUidHash();
	}

	public static void OnRecv(NKMPacket_EQUIP_PRESET_CHANGE_INDEX_ACK cPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			if (NKCUIUnitInfo.IsInstanceOpen && NKCUIUnitInfo.Instance.EquipPresetOpened())
			{
				NKCUIUnitInfo.Instance.EquipPreset?.UpdatePresetData(cPacket.presetDatas, setScrollPositon: false, 0, forceRefresh: true);
			}
			else if (cPacket.presetDatas != null)
			{
				NKCEquipPresetDataManager.ListEquipPresetData = cPacket.presetDatas;
			}
		}
	}

	public static void OnRecv(NKMPacket_EQUIP_PRESET_CLEAR_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCEquipPresetDataManager.ResetRemoveTargetIndexList();
			if (NKCUIUnitInfo.IsInstanceOpen && NKCUIUnitInfo.Instance.EquipPresetOpened())
			{
				NKCUIUnitInfo.Instance.EquipPreset?.UpdatePresetData(sPacket.presetDatas, setScrollPositon: false, 0, forceRefresh: true);
			}
			else if (sPacket.presetDatas != null)
			{
				NKCEquipPresetDataManager.ListEquipPresetData = sPacket.presetDatas;
			}
		}
	}

	public static void OnRecv(NKMPacket_CHARGE_ITEM_NOT sPacket)
	{
		if (sPacket.itemData == null || sPacket.itemData.ItemID == 0)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			nKMUserData.SetUpdateDate(sPacket);
			nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.itemData);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				NKCScenManager.GetScenManager().Get_SCEN_HOME().RefreshRechargeEternium();
			}
		}
	}

	public static void OnRecv(NKMPACKET_RACE_TEAM_SELECT_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		if (NKCUIEventSubUIRace.RaceSummary != null)
		{
			NKCUIEventSubUIRace.RaceSummary.racePrivate = sPacket.racePrivate;
			if (NKCUIEvent.IsInstanceOpen)
			{
				NKCUIEvent.Instance.RefreshUI();
			}
		}
		NKCUIEventSubUIRace.OpenRace();
	}

	public static void OnRecv(NKMPACKET_RACE_START_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			myUserData.m_InventoryData.UpdateItemInfo(sPacket.costItemList);
			myUserData.GetReward(sPacket.rewardData);
		}
		if (NKCUIEventSubUIRace.RaceSummary != null)
		{
			NKCUIEventSubUIRace.RaceSummary.racePrivate = sPacket.racePrivate;
			if (NKCUIEvent.IsInstanceOpen)
			{
				NKCUIEvent.Instance.RefreshUI();
			}
		}
		if (NKCPopupEventRace.IsInstanceOpen)
		{
			NKCPopupEventRace.Instance.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPACKET_RACE_RESET_NOT sPacket)
	{
		NKCUIEventSubUIRace.RaceDay = sPacket.currentRaceIndex;
		NKCUIEventSubUIRace.RaceSummary = sPacket.summary;
		if (NKCUIEvent.IsInstanceOpen)
		{
			NKCUIEvent.Instance.RefreshUI();
		}
	}

	public static void OnRecv(NKMPACKET_EVENT_BET_SELECT_TEAM_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			Debug.Log($"<color=red>NKMPACKET_EVENT_BET_SELECT_TEAM_ACK - {sPacket.eventBetPrivate}</color>");
			NKCScenManager.CurrentUserData().GetRaceData().SetRaceEventBetPrivate(sPacket.eventBetPrivate);
			NKCScenManager.CurrentUserData().m_InventoryData.UpdateItemInfo(sPacket.costItemList);
			if (NKCPopupEventRaceV2.IsInstanceOpen)
			{
				NKCPopupEventRaceV2.Instance.OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPACKET_EVENT_BET_BETTING_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			Debug.Log($"<color=red>NKMPACKET_EVENT_BET_BETTING_ACK - {sPacket.eventBetPrivate}</color>");
			NKCScenManager.CurrentUserData().GetRaceData().SetRaceEventBetPrivate(sPacket.eventBetPrivate);
			NKCScenManager.CurrentUserData().m_InventoryData.UpdateItemInfo(sPacket.costItemList);
			if (NKCPopupEventRaceV2.IsInstanceOpen)
			{
				NKCPopupEventRaceV2.Instance.OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPACKET_EVENT_BET_RESULT_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCPopupEventRaceV2.IsInstanceOpen)
			{
				NKCPopupEventRaceV2.Instance.OnRecvPacket();
			}
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			nKMUserData.GetReward(sPacket.rewardData);
			if (NKCPopupEventRaceV2.IsInstanceOpen)
			{
				NKCPopupEventRaceV2.Instance.OnRecv(sPacket);
			}
			else
			{
				NKCPopupMessageToastSimple.Instance.Open(sPacket.rewardData, null);
			}
		}
	}

	public static void OnRecv(NKMPACKET_EVENT_BET_RECORD_NOT sPacket)
	{
		Debug.Log($"<color=red>NKMPACKET_EVENT_BET_RECORD_NOT - event index : {sPacket.record.eventIndex}</color>");
		NKCScenManager.CurrentUserData().GetRaceData().UpdateCurrentRaceBetInfo(sPacket.record.eventIndex, sPacket.record);
		if (NKCPopupEventRaceV2.IsInstanceOpen)
		{
			NKCPopupEventRaceV2.Instance.ResetUI();
		}
	}

	public static void OnRecv(NKMPACKET_EVENT_BET_RESET_NOT sPacket)
	{
		NKCScenManager.CurrentUserData().GetRaceData().SetRaceData(sPacket.eventId, sPacket.eventIndex, sPacket.summary);
	}

	public static void OnRecv(NKMPacket_DUNGEON_SKIP_ACK cPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			myUserData.m_InventoryData.UpdateItemInfo(cPacket.costItems);
			if (cPacket.rewardDatas != null)
			{
				int count = cPacket.rewardDatas.Count;
				for (int i = 0; i < count; i++)
				{
					UpdateUserData(bWin: true, cPacket.rewardDatas[i].dungeonClearData, cPacket.rewardDatas[i].episodeCompleteData);
				}
			}
			if (cPacket.updatedUnits != null)
			{
				foreach (UnitLoyaltyUpdateData updatedUnit in cPacket.updatedUnits)
				{
					NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(updatedUnit.unitUid);
					if (unitFromUID != null)
					{
						unitFromUID.loyalty = updatedUnit.loyalty;
						unitFromUID.SetOfficeRoomId(updatedUnit.officeRoomId, updatedUnit.officeGrade, updatedUnit.heartGaugeStartTime);
					}
				}
			}
			if (cPacket.stagePlayData != null)
			{
				myUserData.UpdateStagePlayData(cPacket.stagePlayData);
			}
		}
		NKCPopupOpSkipProcess.Instance.Open(cPacket.rewardDatas, cPacket.updatedUnits);
		if (NKCUIOperationNodeViewer.isOpen())
		{
			NKCUIOperationNodeViewer.Instance.Refresh();
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_OPEN_SECTION_ACK cPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				myUserData.m_InventoryData.UpdateItemInfo(cPacket.costItems);
				myUserData.OfficeData.UpdateSectionData(cPacket.sectionId, cPacket.newRooms);
			}
			NKCUIOfficeMapFront instance = NKCUIOfficeMapFront.GetInstance();
			if (instance != null)
			{
				instance.UpdateSectionLockState(cPacket.sectionId);
				instance.GetCurrentMinimap().UpdateRoomStateInSection(cPacket.sectionId);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_OPEN_ROOM_ACK cPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				myUserData.m_InventoryData.UpdateItemInfo(cPacket.costItems);
				myUserData.OfficeData.UpdateRoomData(cPacket.room);
			}
			NKCUIOfficeMapFront.GetInstance()?.GetCurrentMinimap()?.UpdateRoomStateAll();
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_SET_ROOM_NAME_ACK cPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			NKCScenManager.CurrentUserData()?.OfficeData.UpdateRoomData(cPacket.room);
			NKCUIOfficeMapFront.GetInstance()?.GetCurrentMinimap()?.UpdateRoomInfo(cPacket.room);
			if (NKCUIPopupOfficeMemberEdit.IsInstanceOpen)
			{
				NKCUIPopupOfficeMemberEdit.Instance.UpdateRoomName(cPacket.room.name);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_SET_ROOM_UNIT_ACK cPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(cPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		int count = cPacket.rooms.Count;
		if (nKMUserData != null)
		{
			int count2 = cPacket.units.Count;
			for (int i = 0; i < count2; i++)
			{
				nKMUserData.m_ArmyData.UpdateUnitData(cPacket.units[i]);
			}
			for (int j = 0; j < count; j++)
			{
				nKMUserData.OfficeData.UpdateRoomData(cPacket.rooms[j]);
			}
		}
		if (NKCUIOfficeMapFront.IsInstanceOpen)
		{
			IOfficeMinimap currentMinimap = NKCUIOfficeMapFront.GetInstance().GetCurrentMinimap();
			if (currentMinimap != null)
			{
				for (int k = 0; k < count; k++)
				{
					currentMinimap.UpdateRoomInfo(cPacket.rooms[k]);
				}
			}
		}
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnRoomUnitUpdated();
			}
		}
		NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_OFFICE_ASSIGN_COMPLETE, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
	}

	public static void OnRecv(NKMPacket_OFFICE_SET_ROOM_WALL_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().OfficeData.UpdateRoomData(sPacket.room);
		NKCScenManager.CurrentUserData().m_ArmyData.UpdateUnitData(sPacket.updatedUnits);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnApplyDecoration(sPacket.room.wallInteriorId);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_SET_ROOM_FLOOR_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().OfficeData.UpdateRoomData(sPacket.room);
		NKCScenManager.CurrentUserData().m_ArmyData.UpdateUnitData(sPacket.updatedUnits);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnApplyDecoration(sPacket.room.floorInteriorId);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_SET_ROOM_BACKGROUND_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().OfficeData.UpdateRoomData(sPacket.room);
		NKCScenManager.CurrentUserData().m_ArmyData.UpdateUnitData(sPacket.updatedUnits);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnApplyDecoration(sPacket.room.backgroundId);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_ADD_FURNITURE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().OfficeData.UpdateRoomData(sPacket.room);
		NKCScenManager.CurrentUserData().OfficeData.UpdateInteriorData(sPacket.changedInterior);
		NKCScenManager.CurrentUserData().m_ArmyData.UpdateUnitData(sPacket.updatedUnits);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnAddFurniture(sPacket.room.id, sPacket.furniture);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_UPDATE_FURNITURE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().OfficeData.UpdateRoomData(sPacket.room);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnFurnitureMove(sPacket.room.id, sPacket.furniture);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_REMOVE_FURNITURE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().OfficeData.UpdateRoomData(sPacket.room);
		NKCScenManager.CurrentUserData().OfficeData.UpdateInteriorData(sPacket.changedInterior);
		NKCScenManager.CurrentUserData().m_ArmyData.UpdateUnitData(sPacket.updatedUnits);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnRemoveFurniture(sPacket.room.id, sPacket.furnitureUid);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_CLEAR_ALL_FURNITURE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().OfficeData.UpdateRoomData(sPacket.room);
		NKCScenManager.CurrentUserData().OfficeData.UpdateInteriorData(sPacket.changedInteriors);
		NKCScenManager.CurrentUserData().m_ArmyData.UpdateUnitData(sPacket.updatedUnits);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnRemoveAllFurnitures(sPacket.room.id);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_TAKE_HEART_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode, bCloseWaitBox: false))
		{
			return;
		}
		NKCScenManager.CurrentUserData().m_ArmyData.UpdateData(sPacket.unit);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnUnitTakeHeart(sPacket.unit);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_STATE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_OFFICE)
			{
				NKCUIOfficeMapFront.ReserveScenID = NKCScenManager.GetScenManager().GetNowScenID();
			}
			NKCScenManager.CurrentUserData().OfficeData.ResetFriendUId();
			NKCScenManager.CurrentUserData().OfficeData.SetFriendData(sPacket.userUid, sPacket.officeState);
			NKCScenManager.GetScenManager().Get_NKC_SCEN_OFFICE().ReserveShortcut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, NKCUIOfficeMapFront.SectionType.Room.ToString());
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OFFICE, bForce: false);
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_RANDOM_VISIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_OFFICE)
			{
				NKCUIOfficeMapFront.ReserveScenID = NKCScenManager.GetScenManager().GetNowScenID();
			}
			NKCScenManager.CurrentUserData().OfficeData.ResetFriendUId();
			NKCScenManager.CurrentUserData().OfficeData.SetFriendData(sPacket.officeState.commonProfile.userUid, sPacket.officeState);
			NKCScenManager.GetScenManager().Get_NKC_SCEN_OFFICE().ReserveShortcut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, NKCUIOfficeMapFront.SectionType.Room.ToString());
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OFFICE, bForce: false);
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_POST_SEND_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		nKMUserData.OfficeData.UpdatePostState(sPacket.postState);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.UpdatePostState();
			}
		}
		NKMCommonProfile profile = nKMUserData.OfficeData.GetFriendProfile();
		if (profile == null)
		{
			return;
		}
		if (!NKCFriendManager.IsFriend(profile.friendCode) && NKCFriendManager.GetFriendCount() < 60)
		{
			string content = string.Format(NKCUtilString.GET_STRING_OFFICE_REQUEST_FRIEND, profile.nickname);
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, content, delegate
			{
				NKMPacket_FRIEND_REQUEST_REQ packet = new NKMPacket_FRIEND_REQUEST_REQ
				{
					friendCode = profile.friendCode
				};
				NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}, delegate
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_OFFICE_BIZ_CARD_SENT"));
			});
		}
		else
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_OFFICE_BIZ_CARD_SENT"));
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_POST_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().OfficeData.UpdatePostList(sPacket.postList);
			if (NKCUIPopupOfficeInteract.IsInstanceOpen)
			{
				NKCUIPopupOfficeInteract.Instance.UpdateBizCardList();
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_POST_RECV_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().OfficeData.UpdatePostState(sPacket.postState);
			NKCScenManager.CurrentUserData().OfficeData.UpdatePostList(sPacket.postList);
			NKCScenManager.CurrentUserData().GetReward(sPacket.rewardData);
			if (NKCUIPopupOfficeInteract.IsInstanceOpen)
			{
				NKCUIPopupOfficeInteract.Instance.UpdateBizCardList();
			}
			NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentUserData().m_ArmyData, sPacket.rewardData, NKCUtilString.GET_STRING_ITEM_GAIN);
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_POST_BROADCAST_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().OfficeData.UpdatePostState(sPacket.postState);
		NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_OFFICE_BIZCARD_SENDED_ALL);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.UpdatePostState();
			}
		}
		if (NKCUIPopupOfficeInteract.IsInstanceOpen)
		{
			NKCUIPopupOfficeInteract.Instance.UpdateSendBizCardAllState();
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_GUEST_LIST_NOT sPacket)
	{
		NKCScenManager.CurrentUserData().OfficeData.UpdateRandomVisitor(sPacket.guestList);
	}

	public static void OnRecv(NKMPacket_OFFICE_PARTY_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			nKMUserData.m_ArmyData.UpdateUnitData(sPacket.units);
			nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItems);
			nKMUserData.GetReward(sPacket.rewardData);
			if (NKCUIOffice.IsInstanceOpen)
			{
				NKCUIPopupOfficePartyStart.Instance.Open(sPacket.rewardData, NKCUIOffice.GetInstance().OnPartyFinished);
			}
			else
			{
				NKCUIPopupOfficePartyStart.Instance.Open(sPacket.rewardData, null);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_PRESET_APPLY_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().OfficeData.UpdateRoomData(sPacket.room);
		NKCScenManager.CurrentUserData().OfficeData.UpdateInteriorData(sPacket.changedInteriors);
		NKCScenManager.CurrentUserData().m_ArmyData.UpdateUnitData(sPacket.updatedUnits);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnApplyPreset(sPacket.room);
			}
		}
		if (NKCOfficeManager.IsAllFurniturePlaced(NKCScenManager.CurrentUserData().OfficeData.GetPreset(sPacket.presetId), sPacket.room))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_PF_OFFICE_DECO_MODE_PRESET_LOAD_COMPLETE"));
		}
		else
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_PF_OFFICE_DECO_MODE_PRESET_LOAD_ERROR"));
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_PRESET_ADD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			int unlockCount = sPacket.totalPresetCount - NKCScenManager.CurrentUserData().OfficeData.GetPresetCount();
			NKCScenManager.CurrentUserData().OfficeData.SetPresetCount(sPacket.totalPresetCount);
			NKCScenManager.CurrentUserData().m_InventoryData.UpdateItemInfo(sPacket.costItemDatas);
			if (NKCUIPopupOfficePresetList.IsInstanceOpen)
			{
				NKCUIPopupOfficePresetList.Instance.Refresh();
				NKCUIPopupOfficePresetList.Instance.PlayUnlockEffect(unlockCount);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_PRESET_RESET_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMOfficePreset nKMOfficePreset = new NKMOfficePreset();
			NKMOfficePreset preset = NKCScenManager.CurrentUserData().OfficeData.GetPreset(sPacket.presetId);
			nKMOfficePreset.presetId = sPacket.presetId;
			if (preset != null)
			{
				nKMOfficePreset.name = preset.name;
			}
			NKCScenManager.CurrentUserData().OfficeData.SetPreset(nKMOfficePreset);
			if (NKCUIPopupOfficePresetList.IsInstanceOpen)
			{
				NKCUIPopupOfficePresetList.Instance.Refresh(sPacket.presetId);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_PRESET_CHANGE_NAME_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().OfficeData.ChangePresetName(sPacket.presetId, sPacket.newPresetName);
			if (NKCUIPopupOfficePresetList.IsInstanceOpen)
			{
				NKCUIPopupOfficePresetList.Instance.Refresh(sPacket.presetId);
			}
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_PRESET_REGISTER_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		if (sPacket.preset != null)
		{
			NKCScenManager.CurrentUserData().OfficeData.SetPreset(sPacket.preset);
			if (NKCUIPopupOfficePresetList.IsInstanceOpen)
			{
				NKCUIPopupOfficePresetList.Instance.Refresh(sPacket.preset.presetId);
			}
		}
		else if (NKCUIPopupOfficePresetList.IsInstanceOpen)
		{
			NKCUIPopupOfficePresetList.Instance.Refresh();
		}
	}

	public static void OnRecv(NKMPacket_OFFICE_PRESET_APPLY_THEMA_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCScenManager.CurrentUserData().OfficeData.UpdateRoomData(sPacket.room);
		NKCScenManager.CurrentUserData().OfficeData.UpdateInteriorData(sPacket.changedInteriors);
		NKCScenManager.CurrentUserData().m_ArmyData.UpdateUnitData(sPacket.updatedUnits);
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice instance = NKCUIOffice.GetInstance();
			if (instance != null)
			{
				instance.OnApplyPreset(sPacket.room);
			}
		}
		NKMOfficeThemePresetTemplet nKMOfficeThemePresetTemplet = NKMOfficeThemePresetTemplet.Find(sPacket.themaIndex);
		if (nKMOfficeThemePresetTemplet != null)
		{
			if (NKCOfficeManager.IsAllFurniturePlaced(nKMOfficeThemePresetTemplet.OfficePreset, sPacket.room))
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_PF_OFFICE_DECO_MODE_PRESET_LOAD_COMPLETE"));
			}
			else
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_PF_OFFICE_DECO_MODE_PRESET_LOAD_ERROR"));
			}
		}
	}

	public static void OnRecv(NKMPacket_RECALL_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (!nKMUserData.m_RecallHistoryData.ContainsKey(sPacket.historyInfo.unitId))
			{
				nKMUserData.m_RecallHistoryData.Add(sPacket.historyInfo.unitId, sPacket.historyInfo);
			}
			else
			{
				nKMUserData.m_RecallHistoryData[sPacket.historyInfo.unitId] = sPacket.historyInfo;
			}
			NKMRewardData nKMRewardData = new NKMRewardData();
			if (sPacket.exchangeUnitDatas != null)
			{
				nKMRewardData.SetUnitData(sPacket.exchangeUnitDatas);
			}
			if (sPacket.rewardList.Count > 0)
			{
				nKMRewardData.SetMiscItemData(sPacket.rewardList);
			}
			nKMUserData.m_ArmyData.RemoveUnitOrShip(sPacket.removeUnitUid);
			nKMUserData.GetReward(nKMRewardData);
			if (nKMUserData != null)
			{
				NKCUIResult.Instance.OpenBoxGain(nKMUserData.m_ArmyData, new List<NKMRewardData> { nKMRewardData }, "", "", OnCloseRecallReward);
			}
		}
	}

	private static void OnCloseRecallReward()
	{
		if (NKCPopupRecall.IsInstanceOpen)
		{
			NKCPopupRecall.Instance.Close();
		}
		if (NKCUIUnitInfo.IsInstanceOpen)
		{
			NKCUIUnitInfo.Instance.Close();
		}
		if (NKCUIShipInfo.IsInstanceOpen)
		{
			NKCUIShipInfo.Instance.Close();
		}
	}

	public static void OnRecv(NKMPacket_KAKAO_MISSION_REFRESH_STATE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().kakaoMissionData = sPacket.missionData;
			if (NKCUIEvent.IsInstanceOpen)
			{
				NKCUIEvent.Instance.RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_WECHAT_COUPON_CHECK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode, bCloseWaitBox: true, null, sPacket.zlongInfoCode))
		{
			NKCUIEventSubUIWechatFollow.SetWechatCouponData(sPacket.data);
			NKCUIEventSubUIWechatFollow.SetSendPacketAfterRefresh(bSet: false);
			if (NKCUIEvent.IsInstanceOpen)
			{
				NKCUIEvent.Instance.RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_WECHAT_COUPON_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCUIEventSubUIWechatFollow.SetWechatCouponData(sPacket.data);
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			nKMUserData.GetReward(sPacket.rewardData);
			if (NKCUIEvent.IsInstanceOpen)
			{
				NKCUIEvent.Instance.RefreshUI();
			}
			NKCUIResult.Instance.OpenComplexResult(nKMUserData.m_ArmyData, sPacket.rewardData, null, 0L);
		}
	}

	public static void OnRecv(NKMPacket_EXTRACT_UNIT_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			nKMUserData.GetReward(sPacket.rewardItems);
			nKMUserData.GetReward(sPacket.synergyItems);
			nKMUserData.m_ArmyData.RemoveUnit(sPacket.extractUnitUidList);
			if (NKCUIPopupRearmamentExtractConfirm.IsInstanceOpen)
			{
				NKCUIPopupRearmamentExtractConfirm.Instance.Close();
			}
			NKCUIRearmament.Instance.OnRecv(sPacket);
			if (NKCGameEventManager.IsWaiting())
			{
				NKCUIResult.Instance.OpenRewardRearmExtract(sPacket.rewardItems, sPacket.synergyItems, NKCUtilString.GET_STRING_REARM_EXTRACT_RESULT_TITLE, "", NKCGameEventManager.WaitFinished);
			}
			else
			{
				NKCUIResult.Instance.OpenRewardRearmExtract(sPacket.rewardItems, sPacket.synergyItems, NKCUtilString.GET_STRING_REARM_EXTRACT_RESULT_TITLE);
			}
		}
	}

	public static void OnRecv(NKMPacket_REARMAMENT_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			NKMArmyData armyData = nKMUserData.m_ArmyData;
			armyData.UpdateUnitData(sPacket.rearmamentUnitData);
			armyData.TryCollectUnit(sPacket.rearmamentUnitData.m_UnitID);
			nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItems);
			if (NKCUIPopupRearmamentConfirm.IsInstanceOpen)
			{
				NKCUIPopupRearmamentConfirm.Instance.Close();
			}
			if (NKCUIPopupRearmamentConfirmBox.IsInstanceOpen)
			{
				NKCUIPopupRearmamentConfirmBox.Instance.Close();
			}
			NKCUIPopupRearmamentResult.Instance.Open(sPacket.rearmamentUnitData);
		}
	}

	public static void OnRecv(NKMPacket_SERVER_KILL_COUNT_NOT sPacket)
	{
		NKCKillCountManager.SetServerKillCountData(sPacket.serverKillCountDataList);
	}

	public static void OnRecv(NKMPacket_KILL_COUNT_USER_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKCKillCountManager.SetKillCountData(sPacket.killCountData);
			nKMUserData.GetReward(sPacket.rewardData);
			if (NKCUIEvent.IsInstanceOpen)
			{
				NKCUIEvent.Instance.RefreshUI();
			}
			NKCUIResult.Instance.OpenRewardGain(nKMUserData.m_ArmyData, sPacket.rewardData, NKCUtilString.GET_STRING_RESULT_MISSION);
		}
	}

	public static void OnRecv(NKMPacket_KILL_COUNT_SERVER_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKCKillCountManager.SetKillCountData(sPacket.killCountData);
		nKMUserData.GetReward(sPacket.rewardData);
		NKCUIResult.Instance.OpenRewardGain(nKMUserData.m_ArmyData, sPacket.rewardData, NKCUtilString.GET_STRING_RESULT_MISSION, "", delegate
		{
			if (NKCPopupEventKillCountReward.IsInstanceOpen)
			{
				NKCPopupEventKillCountReward.Instance.SetData();
			}
			if (NKCUIEvent.IsInstanceOpen)
			{
				NKCUIEventSubUIHorizon.RewardGet = true;
				NKCUIEvent.Instance.RefreshUI();
			}
		});
	}

	public static void OnRecv(NKMPacket_UNIT_MISSION_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && sPacket.missionData != null)
		{
			NKCUnitMissionManager.UpdateCompletedUnitMissionData(sPacket.missionData);
			if (NKCUIPopupCollectionAchievement.IsInstanceOpen)
			{
				NKCUIPopupCollectionAchievement.Instance.Refresh();
			}
			if (NKCUICollectionUnitInfo.IsInstanceOpen)
			{
				NKCUICollectionUnitInfo.Instance.UpdateUnitMissionRedDot();
			}
			if (NKCUICollectionUnitInfoV2.IsInstanceOpen)
			{
				NKCUICollectionUnitInfoV2.Instance.UpdateUnitMissionState();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().OnRecvUnitMissionReward(sPacket.missionData.unitId);
			}
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.GetReward(sPacket.rewardData);
				NKCPopupMessageToastSimple.Instance.Open(sPacket.rewardData, null);
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_MISSION_REWARD_ALL_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && sPacket.missionData.Count > 0)
		{
			NKCUnitMissionManager.UpdateCompletedUnitMissionData(sPacket.missionData);
			if (NKCUIPopupCollectionAchievement.IsInstanceOpen)
			{
				NKCUIPopupCollectionAchievement.Instance.Refresh();
			}
			if (NKCUICollectionUnitInfo.IsInstanceOpen)
			{
				NKCUICollectionUnitInfo.Instance.UpdateUnitMissionRedDot();
			}
			if (NKCUICollectionUnitInfoV2.IsInstanceOpen)
			{
				NKCUICollectionUnitInfoV2.Instance.UpdateUnitMissionState();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().OnRecvUnitMissionReward(sPacket.missionData[0].unitId);
			}
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.GetReward(sPacket.rewardData);
				NKCPopupMessageToastSimple.Instance.Open(sPacket.rewardData, null);
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_MISSION_UPDATED_NOT sPacket)
	{
		NKCUnitMissionManager.UpdateRewardEnableMissionData(sPacket.rewardEnableMissions);
		if (NKCUICollectionUnitInfo.IsInstanceOpen)
		{
			NKCUICollectionUnitInfo.Instance.UpdateUnitMissionRedDot();
		}
		if (NKCUICollectionUnitInfoV2.IsInstanceOpen)
		{
			NKCUICollectionUnitInfoV2.Instance.UpdateUnitMissionState();
		}
		if (NKCUIPopupCollectionAchievement.IsInstanceOpen)
		{
			NKCUIPopupCollectionAchievement.Instance.Refresh();
		}
	}

	public static void OnRecv(NKMPacket_MISC_COLLECTION_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			if (sPacket.miscCollectionData != null)
			{
				nKMUserData.m_InventoryData.AddMiscCollectionData(sPacket.miscCollectionData);
			}
			nKMUserData.GetReward(sPacket.rewardData);
			NKCPopupMessageToastSimple.Instance.Open(sPacket.rewardData, null);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().OnRecvMiscCollectionReward();
			}
		}
	}

	public static void OnRecv(NKMPacket_MISC_COLLECTION_REWARD_ALL_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		foreach (NKMMiscCollectionData miscCollectionData in sPacket.miscCollectionDatas)
		{
			nKMUserData.m_InventoryData.AddMiscCollectionData(miscCollectionData);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().OnRecvMiscCollectionReward();
		}
		nKMUserData.GetReward(sPacket.rewardData);
		NKCUIResult.Instance.OpenRewardGain(nKMUserData.m_ArmyData, sPacket.rewardData, NKCStringTable.GetString("SI_PF_REWARD"));
	}

	public static void OnRecv(NKMPacket_PVP_CASTING_VOTE_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCBanManager.UpdatePVPCastingVoteData(sPacket.pvpCastingVoteData);
			if (NKCPopupGauntletBan.IsInstanceOpen)
			{
				NKCPopupGauntletBan.Instance.UpdateUI();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_PVP_CASTING_VOTE_SHIP_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCBanManager.UpdatePVPCastingVoteData(sPacket.pvpCastingVoteData);
			if (NKCPopupGauntletBan.IsInstanceOpen)
			{
				NKCPopupGauntletBan.Instance.UpdateUI();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_PVP_CASTING_VOTE_OPERATOR_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCBanManager.UpdatePVPCastingVoteData(sPacket.pvpCastingVoteData);
			if (NKCPopupGauntletBan.IsInstanceOpen)
			{
				NKCPopupGauntletBan.Instance.UpdateUI();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_BAR_DAILY_INFO_NOT sPacket)
	{
		NKCEventBarManager.DailyCocktailItemID = sPacket.dailyCocktailItemId;
		NKCEventBarManager.RemainDeliveryLimitValue = sPacket.remainDeliveryLimitValue;
		if (NKCUIEvent.IsInstanceOpen)
		{
			NKCUIEventSubUIBar.RefreshUI = true;
			NKCUIEvent.Instance.RefreshUI();
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_HOME)
		{
			return;
		}
		foreach (NKCUIModuleHome item in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
		{
			if (item.IsOpen && item.GetComponent<NKCUIModuleSubUIBar>() != null)
			{
				NKCUIEventSubUIBar.RefreshUI = true;
				item.UpdateUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_BAR_CREATE_COCKTAIL_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData cNKMUserData = NKCScenManager.CurrentUserData();
		if (cNKMUserData == null)
		{
			return;
		}
		cNKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItems);
		cNKMUserData.GetReward(sPacket.rewardData);
		if (NKCUIEvent.IsInstanceOpen)
		{
			if (sPacket.rewardData.MiscItemDataList.Count > 0)
			{
				NKCUIEventSubUI everOpenedEventSubUI = NKCUIEvent.Instance.GetEverOpenedEventSubUI(NKCUIEventSubUIBar.EventID);
				if (everOpenedEventSubUI != null)
				{
					NKCUIEventSubUIBar component = everOpenedEventSubUI.GetComponent<NKCUIEventSubUIBar>();
					OpenEventBarResult(component);
				}
			}
			NKCUIEventSubUIBar.RefreshUI = true;
			NKCUIEvent.Instance.RefreshUI();
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_HOME)
		{
			return;
		}
		foreach (NKCUIModuleHome item in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
		{
			if (item.IsOpen)
			{
				NKCUIModuleSubUIBar component2 = item.GetComponent<NKCUIModuleSubUIBar>();
				if (component2 != null)
				{
					OpenEventBarResult(component2.EventSubUIBar);
					NKCUIEventSubUIBar.RefreshUI = true;
					item.UpdateUI();
				}
			}
		}
		void OpenEventBarResult(NKCUIEventSubUIBar subUIBar)
		{
			if (!(subUIBar == null) && !(subUIBar.m_eventBarResult == null))
			{
				NKCUIEventBarResult eventBarResult = subUIBar.m_eventBarResult;
				eventBarResult.m_onClose = (NKCUIEventBarResult.OnClose)Delegate.Combine(eventBarResult.m_onClose, (NKCUIEventBarResult.OnClose)delegate
				{
					NKCUIResult.Instance.OpenRewardGain(cNKMUserData.m_ArmyData, sPacket.rewardData, NKCUtilString.GET_STRING_GREMORY_CREATE_RESULT, "", delegate
					{
						subUIBar.ActivateCreateFx();
					});
				});
				subUIBar.m_eventBarResult.Open(sPacket.rewardData.MiscItemDataList[0].ItemID);
			}
		}
	}

	public static void OnRecv(NKMPacket_EVENT_BAR_GET_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItems);
		nKMUserData.GetReward(sPacket.rewardData);
		NKCEventBarManager.RemainDeliveryLimitValue = sPacket.remainDeliveryLimitValue;
		if (NKCUIEvent.IsInstanceOpen)
		{
			NKCUIEventSubUIBar.RefreshUI = true;
			NKCUIEvent.Instance.RefreshUI();
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			foreach (NKCUIModuleHome item in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
			{
				if (item.IsOpen && item.GetComponent<NKCUIModuleSubUIBar>() != null)
				{
					NKCUIEventSubUIBar.RefreshUI = true;
					item.UpdateUI();
				}
			}
		}
		NKCUIResult.Instance.OpenRewardGainWithUnitSD(nKMUserData.m_ArmyData, sPacket.rewardData, null, NKCEventBarManager.RewardPopupUnitID, NKCEventBarManager.RewardPopupSkinID, NKCUtilString.GET_STRING_GREMORY_DAILY_REWARD);
	}

	public static void OnRecv(NKMPacket_AD_INFO_NOT sPacket)
	{
		NKCAdManager.SetItemRewardInfo(sPacket.itemRewardInfos);
		NKCAdManager.SetInventoryExpandRewardInfo(sPacket.inventoryExpandRewardInfos);
	}

	public static void OnRecv(NKMPacket_AD_ITEM_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCMMPManager.OnCustomEvent("34_AD_eternium");
			NKCAdManager.UpdateItemRewardInfo(sPacket.itemRewardInfo);
			NKCScenManager.CurrentUserData()?.GetReward(sPacket.rewardData);
		}
	}

	public static void OnRecv(NKMPacket_AD_INVENTORY_EXPAND_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCMMPManager.OnCustomEvent("35_AD_ch_inventory");
		NKCAdManager.UpdateInventoryRewardInfo(sPacket.inventoryExpandType);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKMInventoryManager.UpdateInventoryCount(sPacket.inventoryExpandType, sPacket.expandedCount, nKMUserData);
			if (NKCUIUnitSelectList.IsInstanceOpen)
			{
				NKCUIUnitSelectList.Instance.UpdateUnitCount();
				NKCUIUnitSelectList.Instance.OnExpandInventory();
			}
			if (NKCUIInventory.IsInstanceOpen)
			{
				NKCUIInventory.Instance.SetCurrentEquipCountUI();
				NKCUIInventory.Instance.OnInventoryAdd();
			}
			if (NKCUIForge.IsInstanceOpen && NKCUIForge.Instance.IsInventoryInstanceOpen())
			{
				NKCUIForge.Instance.Inventory.SetCurrentEquipCountUI();
				NKCUIForge.Instance.Inventory.OnInventoryAdd();
			}
			if (NKCUIDeckViewer.IsInstanceOpen)
			{
				NKCUIDeckViewer.Instance.UpdateUnitCount();
			}
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_COUPON_USE_ACK sPacket)
	{
		Debug.Log("OnRecv - NKMPacket_BSIDE_COUPON_USE_ACK");
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			string message = NKCStringTable.GetString("SI_PF_OPTION_ACCOUNT_COUPON_SUCCESS");
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(message, NKCPopupMessage.eMessagePosition.Middle, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCAccountLinkMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_NOT sPacket)
	{
		NKCAccountLinkMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_CODE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCAccountLinkMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT sPacket)
	{
		NKCAccountLinkMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_SELECT_USERDATA_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCAccountLinkMgr.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_SUCCESS_NOT sPacket)
	{
		NKCAccountLinkMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_CANCEL_ACK sPacket)
	{
		NKCAccountLinkMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_CANCEL_NOT sPacket)
	{
		NKCAccountLinkMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_STEAM_BUY_INIT_ACK sPacket)
	{
		((NKCPMSteamPC.InAppSteam)NKCPublisherModule.InAppPurchase).OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_EVENT_POINT_NOT sPacket)
	{
		NKCScenManager.CurrentUserData()?.GetReward(sPacket.additionalReward);
		if (NKCPopupPointExchange.IsInstanceOpen)
		{
			NKCPopupPointExchange.Instance.RefreshPoint();
			NKCPopupPointExchange.Instance.RefreshMission();
		}
		NKCPopupMessageToastSimple.Instance.Open(sPacket.additionalReward, null);
	}

	public static void OnRecv(NKMPacket_TRIM_START_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData();
			NKCTrimManager.SetTrimModeState(sPacket.trimModeState);
			NKCTrimManager.ProcessTrim();
		}
	}

	public static void OnRecv(NKMPacket_TRIM_RETRY_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCTrimManager.ClearTrimModeState();
			if (NKCUIResult.IsInstanceOpen)
			{
				NKCUIResult.Instance.OnTrimRetryAck();
			}
		}
	}

	public static void OnRecv(NKMPacket_TRIM_RESTORE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData()?.m_InventoryData.UpdateItemInfo(sPacket.costItemData);
		}
	}

	public static void OnRecv(NKMPacket_TRIM_END_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCTrimManager.ClearTrimModeState();
			NKCScenManager.GetScenManager().Get_NKC_SCEN_TRIM_RESULT().SetData(sPacket.trimClearData, sPacket.trimModeState, sPacket.bestScore, sPacket.isFirst);
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (sPacket.trimClearData != null)
			{
				nKMUserData?.GetReward(sPacket.trimClearData.rewardData);
			}
			nKMUserData?.m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_TRIM_RESULT);
		}
	}

	public static void OnRecv(NKMPacket_TRIM_INTERVAL_INFO_NOT sPacket)
	{
		NKCScenManager.CurrentUserData()?.TrimData.SetTrimClearList(sPacket.trimClearList);
		NKCScenManager.CurrentUserData()?.TrimData.SetTrimIntervalData(sPacket.trimIntervalData);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_TRIM && NKCScenManager.GetScenManager().Get_NKC_SCEN_TRIM().TrimIntervalId != sPacket.trimIntervalId)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TRIM_INTERVAL_END, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
			});
			return;
		}
		if (NKCUITrimMain.IsInstanceOpen)
		{
			NKCUITrimMain.GetInstance().RefreshUI();
		}
		if (NKCUIPopupTrimDungeon.IsInstanceOpen)
		{
			NKCUIPopupTrimDungeon.Instance.RefreshUI(resetLevelTab: false);
		}
	}

	public static void OnRecv(NKMPacket_TRIM_DUNGEON_SKIP_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			nKMUserData.TrimData.SetTrimClearData(sPacket.trimClearData);
			nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.costItems);
			if (sPacket.rewardDatas != null)
			{
				foreach (NKMRewardData rewardData in sPacket.rewardDatas)
				{
					nKMUserData.GetReward(rewardData);
				}
			}
			if (sPacket.updatedUnits != null)
			{
				foreach (UnitLoyaltyUpdateData updatedUnit in sPacket.updatedUnits)
				{
					NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(updatedUnit.unitUid);
					if (unitFromUID != null)
					{
						unitFromUID.loyalty = updatedUnit.loyalty;
						unitFromUID.SetOfficeRoomId(updatedUnit.officeRoomId, updatedUnit.officeGrade, updatedUnit.heartGaugeStartTime);
					}
				}
			}
		}
		NKCPopupOpSkipProcess.Instance.Open(sPacket.rewardDatas, sPacket.updatedUnits);
	}

	public static void OnRecv(NKMPacket_EVENT_COLLECTION_NOT sPacket)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			nKMUserData.EventCollectionInfo = sPacket.eventCollectionInfo;
		}
	}

	public static void OnRecv(NKMPacket_EVENT_COLLECTION_MERGE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		foreach (NKMUnitData unitData in sPacket.rewardData.UnitDataList)
		{
			armyData.AddNewUnit(unitData);
		}
		armyData.RemoveUnit(sPacket.consumeTrophyUids);
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_HOME)
		{
			return;
		}
		foreach (NKCUIModuleHome item in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
		{
			if (!item.IsOpen)
			{
				continue;
			}
			item.UpdateUI();
			using IEnumerator<NKCUIModuleSubUIMerge> enumerator3 = item.GetSubUIs<NKCUIModuleSubUIMerge>().GetEnumerator();
			if (enumerator3.MoveNext())
			{
				enumerator3.Current?.OnCompleteMerge(sPacket.collectionMergeId, sPacket.rewardData.UnitDataList);
			}
		}
	}

	public static void OnRecv(NKMPacket_UNIT_TACTIC_UPDATE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		nKMUserData.m_ArmyData.RemoveUnit(sPacket.consumeUnitUids);
		nKMUserData.m_ArmyData.UpdateUnitData(sPacket.unitData);
		if (NKCUITacticUpdate.IsInstanceOpen)
		{
			NKCUITacticUpdate.Instance.OnRecv(sPacket);
		}
		if (NKCTacticUpdateUtil.IsMaxTacticLevel(sPacket.unitData.tacticLevel))
		{
			NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
			if (userProfileData != null && userProfileData.commonProfile != null && userProfileData.commonProfile.mainUnitId == sPacket.unitData.m_UnitID)
			{
				NKCPacketSender.Send_NKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ(userProfileData.commonProfile.mainUnitId, userProfileData.commonProfile.mainUnitSkinId);
			}
		}
	}

	public static void OnRecv(NKMPacket_SERVICE_TRANSFER_REGIST_CODE_ACK sPacket)
	{
		NKCServiceTransferMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_SERVICE_TRANSFER_CODE_COPY_REWARD_ACK sPacket)
	{
		NKCServiceTransferMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_SERVICE_TRANSFER_USER_VALIDATION_ACK sPacket)
	{
		NKCServiceTransferMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_ACK sPacket)
	{
		NKCServiceTransferMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_SERVICE_TRANSFER_CONFIRM_ACK sPacket)
	{
		NKCServiceTransferMgr.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPacket_UNIT_REACTOR_LEVELUP_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			Debug.Log($"<color=red>NKMPacket_UNIT_REACTOR_LEVELUP_ACK : unit id : {sPacket.unitData.m_UnitID} / reactor level : {sPacket.unitData.reactorLevel}</color>");
			nKMUserData?.m_InventoryData.UpdateItemInfo(sPacket.costItemDatas);
			nKMUserData.m_ArmyData.UpdateUnitData(sPacket.unitData);
			if (NKCUIUnitReactor.IsInstanceOpen)
			{
				NKCUIUnitReactor.Instance.OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_DEFENCE_INFO_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (sPacket.defenceTempletId != NKCDefenceDungeonManager.m_DefenceTempletId)
			{
				NKCDefenceDungeonManager.Init();
			}
			return;
		}
		NKCDefenceDungeonManager.SetData(sPacket);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCUIModuleHome.UpdateAllModule();
		}
	}

	public static void OnRecv(NKMPacket_DEFENCE_GAME_START_ACK sPacket)
	{
		NKCPopupOKCancel.ClosePopupBox();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			switch (NKCScenManager.GetScenManager().GetNowScenID())
			{
			case NKM_SCEN_ID.NSI_LOGIN:
			case NKM_SCEN_ID.NSI_GAME:
			case NKM_SCEN_ID.NSI_DUNGEON_ATK_READY:
			case NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON:
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
				NKCPopupMessageManager.AddPopupMessage(sPacket.errorCode);
				break;
			}
			return;
		}
		NKCScenManager.GetScenManager().Get_SCEN_GAME().ReserveGameEndData(null);
		if (sPacket.gameData == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR_SERVER_GAME_DATA, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
		}
		bool flag = false;
		switch (NKCScenManager.GetScenManager().GetNowScenID())
		{
		case NKM_SCEN_ID.NSI_LOGIN:
		case NKM_SCEN_ID.NSI_HOME:
		case NKM_SCEN_ID.NSI_DUNGEON_ATK_READY:
		case NKM_SCEN_ID.NSI_WARFARE_GAME:
		case NKM_SCEN_ID.NSI_WORLDMAP:
		case NKM_SCEN_ID.NSI_GAME_RESULT:
		case NKM_SCEN_ID.NSI_DIVE:
		case NKM_SCEN_ID.NSI_RAID_READY:
		case NKM_SCEN_ID.NSI_TRIM:
			flag = true;
			break;
		case NKM_SCEN_ID.NSI_GAME:
			if (sPacket.gameData == null)
			{
				Debug.LogError("Dungeon Loaded from Game Scene!");
			}
			else if (!NKMDungeonManager.IsRestartAllowed(sPacket.gameData.m_NKM_GAME_TYPE))
			{
				Debug.LogError($"Dungeon Loaded from Game Scene. GameType : {sPacket.gameData.m_NKM_GAME_TYPE}");
			}
			flag = true;
			break;
		}
		if (flag)
		{
			NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(sPacket.costItemDataList);
			NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(sPacket.gameData);
			NKCUtil.PlayStartCutscenAndStartGame(sPacket.gameData);
		}
	}

	public static void OnRecv(NKMPacket_DEFENCE_GAME_END_NOT sPacket)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		NKCUIResult.BattleResultData retVal = NKCBattleResultUtility.MakeCoreBattleResultData(sPacket.gameEndData, bWin: true, 0, sPacket.gameEndData.dungeonClearData.dungeonId, NKCUIBattleStatistics.MakeBattleData(gameClient, sPacket.gameEndData.gameRecord, NKM_GAME_TYPE.NGT_PVE_DEFENCE));
		NKCBattleResultUtility.MakeDefenceBattleResult(ref retVal, sPacket.gameEndData, sPacket.defenceClearData);
		NKCScenManager.GetScenManager().Get_SCEN_GAME().ReserveGameEndData(retVal);
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(sPacket.gameEndData.dungeonClearData.dungeonId);
		if (dungeonTempletBase != null)
		{
			string key = $"{nKMUserData.m_UserUID}_{dungeonTempletBase.m_DungeonStrID}";
			if (retVal != null && retVal.IsWin && PlayerPrefs.HasKey(key))
			{
				PlayerPrefs.DeleteKey(key);
			}
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.DungeonGameClear, sPacket.gameEndData.dungeonClearData.dungeonId);
		}
		UpdateUserData(sPacket.gameEndData);
		NKCDefenceDungeonManager.SetData(sPacket.defenceClearData, sPacket.gameEndData.dungeonClearData.missionResult1, sPacket.gameEndData.dungeonClearData.missionResult2);
		nKMUserData.m_InventoryData.UpdateItemInfo(sPacket.gameEndData.costItemDataList);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && sPacket.gameEndData.giveup)
		{
			if (sPacket.gameEndData.restart)
			{
				int dungeonID = (gameClient?.GetGameData())?.m_DungeonID ?? 0;
				NKCScenManager.GetScenManager().Get_SCEN_GAME().DoAfterRestart(NKM_GAME_TYPE.NGT_PVE_DEFENCE, 0, dungeonID, sPacket.gameEndData.deckIndex);
			}
			else
			{
				NKCScenManager.GetScenManager().Get_SCEN_GAME().OnEndGame(retVal);
			}
		}
		NKCKillCountManager.UpdateKillCountData(sPacket.gameEndData.killCountData);
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_DEFENCE_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCLeaderBoardManager.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_DEFENCE_PROFILE_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCPopupFierceUserInfo.Instance.OpenForDefence(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_DEFENCE_RANK_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode) && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCDefenceDungeonManager.SetRankRewardReceived();
			NKCScenManager.CurrentUserData().GetReward(sPacket.rewardData);
			NKCUIPopupDungeonReward.Instance.Open(sPacket.rewardData);
		}
	}

	public static void OnRecv(NKMPacket_DEFENCE_SCORE_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().GetReward(sPacket.rewardData);
			NKCDefenceDungeonManager.UpdateReceveScoreRewardID(sPacket.scoreRewardId);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				NKCUIModuleHome.UpdateAllModule();
			}
			if (NKCUIPopupDungeonScoreRewardInfo.IsInstanceOpen)
			{
				NKCUIPopupDungeonScoreRewardInfo.Instance.Refresh();
			}
			NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentUserData().m_ArmyData, sPacket.rewardData, NKCUtilString.GET_FIERCE_BATTLE_POINT_REWARD_TITLE);
		}
	}

	public static void OnRecv(NKMPacket_DEFENCE_SCORE_REWARD_ALL_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().GetReward(sPacket.rewardData);
			for (int i = 0; i < sPacket.scoreRewardIds.Count; i++)
			{
				NKCDefenceDungeonManager.UpdateReceveScoreRewardID(sPacket.scoreRewardIds[i]);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				NKCUIModuleHome.UpdateAllModule();
			}
			if (NKCUIPopupDungeonScoreRewardInfo.IsInstanceOpen)
			{
				NKCUIPopupDungeonScoreRewardInfo.Instance.Refresh();
			}
			NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentUserData().m_ArmyData, sPacket.rewardData, NKCUtilString.GET_FIERCE_BATTLE_POINT_REWARD_TITLE);
		}
	}

	public static void OnRecv(NKMPacket_UPDATE_TITLE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKMUserProfileData userProfileData = nKMUserData.UserProfileData;
			if (userProfileData != null)
			{
				userProfileData.commonProfile.titleId = sPacket.titleId;
			}
			if (NKCUIUserInfoV2.IsInstanceOpen)
			{
				NKCUIUserInfoV2.Instance.UpdateTitle();
			}
		}
	}

	public static void OnRecv(NKMPacket_ACCOUNT_UPDATE_BIRTHDAY_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().m_BirthDayData = new NKMUserBirthDayData();
			NKCScenManager.CurrentUserData().m_BirthDayData.BirthDay = sPacket.birthDay;
			NKCScenManager.CurrentUserData().m_BirthDayData.Years = 0;
			if (NKCUIUserInfoV2.IsInstanceOpen)
			{
				NKCUIUserInfoV2.Instance.UpdateBirthday(NKCScenManager.CurrentUserData());
			}
		}
	}

	public static void OnRecv(NKMPacket_ACCOUNT_BIRTHDAY_REWARD_NOT sPacket)
	{
		if (NKCScenManager.CurrentUserData() != null)
		{
			NKCScenManager.CurrentUserData().m_BirthDayData = sPacket.birthDayData;
			NKCLoginCutSceneManager.SetReservedBirthdayCutscene(bValue: true);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				NKCScenManager.GetScenManager().Get_SCEN_HOME().OnHomeEnter();
			}
		}
	}

	public static void OnRecv(NKMPacket_START_SIMULATED_PVP_TEST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			sPacket.replayData.replayName = NKCReplayMgr.MakeReplayDataFileName(sPacket.history.gameUid);
			ReplayRecorder.WriteReplayDataToFile(myUserData.m_UserUID.ToString(), sPacket.replayData);
			myUserData.m_AsyncPvpHistory.Add(sPacket.history);
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCTournamentManager.SetTournamentPredictInfo(sPacket.infos);
			if (NKCUIModuleSubUITournamentLobby.IsInstanceOpen)
			{
				NKCUIModuleSubUITournamentLobby.Instance.ShowPlayoff(keepCheerState: false);
			}
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_PREDICTION_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCTournamentManager.SetTournamentPredictInfo(sPacket.info);
			if (NKCUIModuleSubUITournamentLobby.IsInstanceOpen && NKCUIModuleSubUITournamentLobby.Instance.IsPlayoffOpened())
			{
				NKCUIModuleSubUITournamentLobby.Instance.ShowPlayoff(keepCheerState: false);
				NKCUIModuleSubUITournamentLobby.Instance.StartCheerCoolTime();
			}
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_TOURNAMENT_CHEERING_REGISTERED);
			if (sPacket.rewardData != null && sPacket.rewardData.HasAnyReward())
			{
				NKCScenManager.CurrentUserData()?.GetReward(sPacket.rewardData);
				NKCPopupMessageToastSimple.Instance.Open(sPacket.rewardData, null);
			}
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_PREDICTION_STATISTICS_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCUITournamentCheerStatistic.Instance.Open(sPacket.predicitionStatistics, sPacket.groupIndex);
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_APPLY_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCTournamentManager.LoadLastDeckData();
			return;
		}
		NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_TOURNAMENT_DECK_ENTER_POPUP_COMPLETE, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		NKCTournamentManager.SetTournamentApply(sPacket.deck);
		if (NKCUIModuleHome.IsAnyInstanceOpen())
		{
			NKCUIModuleHome.UpdateAllModule();
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_INFO_ACK sPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode, bCloseWaitBox: false))
		{
			NKCTournamentManager.SetTournamentInfo(sPacket);
			if (NKCUIModuleSubUITournamentLobby.IsInstanceOpen && NKCUIModuleSubUITournamentLobby.Instance.IsPlayoffOpened())
			{
				NKCUIModuleSubUITournamentLobby.Instance.ShowPlayoff(keepCheerState: true);
			}
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_INFO_NOT sPacket)
	{
		NKCTournamentManager.SetTournamentInfoChanged(bChanged: true);
		if (NKCTournamentManager.GetTournamentState() == NKMTournamentState.Ended)
		{
			NKCPacketSender.Send_NKMPacket_TOURNAMENT_INFO_REQ();
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_REPLAY_LINK_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode, bCloseWaitBox: false))
		{
			NKMPopUpBox.CloseWaitBox();
			return;
		}
		NKCUIGauntletAsyncReady instanceGauntletAsyncReady = NKCUITournamentPlayoff.GetInstanceGauntletAsyncReady();
		if (instanceGauntletAsyncReady != null && instanceGauntletAsyncReady.IsOpen)
		{
			instanceGauntletAsyncReady.ReplayFileReq(sPacket.replayLink.replayLink.url, sPacket.replayLink.replayLink.checksum);
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_REWARD_ACK sPacket)
	{
		if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_TOURNAMENT_NOT_PLAY)
		{
			NKMPopUpBox.CloseWaitBox();
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_NO_REWARD, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCScenManager.CurrentUserData().GetReward(sPacket.predictionRewardData);
			NKCScenManager.CurrentUserData().GetReward(sPacket.rankRewardData);
			NKCTournamentManager.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_REWARD_INFO_ACK sPacket)
	{
		if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_TOURNAMENT_NOT_PLAY)
		{
			NKMPopUpBox.CloseWaitBox();
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_TOURNAMENT_LOBBY_NO_REWARD, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCTournamentManager.OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_RANK_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKCTournamentManager.SetRankInfos(sPacket.rankInfos);
		if (sPacket.rankInfos.Count > 0)
		{
			if (NKCUIModuleHome.IsAnyInstanceOpen())
			{
				NKCUIModuleHome.SendMessage(new NKCUIModuleSubUITournament.EventModuleDataTouranment
				{
					bOpenRank = true,
					bOpenResult = false
				});
			}
		}
		else
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GE_STRING_TOURNAMENT_HOF_NO_RECORD, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCTournamentManager.SetCastingVoteUnitList(sPacket.pvpCastingVoteData.unitIdList);
			if (NKCPopupTournamentBan.IsInstanceOpen)
			{
				NKCPopupTournamentBan.Instance.RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCTournamentManager.SetCastingVoteShipList(sPacket.pvpCastingVoteData.shipGroupIdList);
			if (NKCPopupTournamentBan.IsInstanceOpen)
			{
				NKCPopupTournamentBan.Instance.RefreshUI();
			}
		}
	}

	public static void OnRecv(NKMPACKET_EVENT_TEN_RECORD_NOT sPacket)
	{
		NKCMatchTenManager.OnRecv(sPacket);
	}

	public static void OnRecv(NKMPACKET_EVENT_TEN_RESULT_ACK sPacket)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode);
	}

	public static void OnRecv(NKMPACKET_EVENT_TEN_REWARD_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKCMatchTenManager.SetReceivedIds(new List<int> { sPacket.rewardId });
			nKMUserData.GetReward(sPacket.rewardData);
			NKCUIResult.Instance.OpenComplexResult(nKMUserData.m_ArmyData, sPacket.rewardData, delegate
			{
				OnCloseEventMiniGameReward();
			}, 0L);
		}
	}

	public static void OnRecv(NKMPACKET_EVENT_TEN_REWARD_ALL_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKCMatchTenManager.SetReceivedIds(sPacket.rewardIds);
			nKMUserData.GetReward(sPacket.rewardData);
			NKCUIResult.Instance.OpenComplexResult(nKMUserData.m_ArmyData, sPacket.rewardData, delegate
			{
				OnCloseEventMiniGameReward();
			}, 0L);
		}
	}

	private static void OnCloseEventMiniGameReward()
	{
		List<NKCPopupScoreReward> openedUIsByType = NKCUIManager.GetOpenedUIsByType<NKCPopupScoreReward>();
		if (openedUIsByType == null)
		{
			return;
		}
		foreach (NKCPopupScoreReward item in openedUIsByType)
		{
			item.Refresh();
		}
	}

	public static void OnRecv(NKMPacket_SUPPORT_UNIT_LIST_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCUIDeckViewer.IsInstanceOpen)
			{
				NKCUIDeckViewer.Instance.OnRecv(sPacket);
			}
			if (NKCUIPrepareEventDeck.IsInstanceOpen)
			{
				NKCUIPrepareEventDeck.Instance.OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_SET_MY_SUPPORT_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCUIUnitSelectList.IsInstanceOpen)
			{
				NKCUIUnitSelectList.Instance.Close();
			}
			NKCScenManager.CurrentUserData().SetMyUserProfileInfo(sPacket.supportUnitData);
			if (NKCUIUserInfoV2.IsInstanceOpen)
			{
				NKCUIUserInfoV2.Instance.OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_SET_DUNGEON_SUPPORT_UNIT_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (NKCUIPopupAssistSelect.IsInstanceOpen)
			{
				NKCUIPopupAssistSelect.Instance.Close();
			}
			if (NKCUIDeckViewer.IsInstanceOpen)
			{
				NKCUIDeckViewer.Instance.OnRecv(sPacket);
			}
			if (NKCUIPrepareEventDeck.IsInstanceOpen)
			{
				NKCUIPrepareEventDeck.Instance.OnRecv(sPacket);
			}
		}
	}

	public static void OnRecv(NKMPacket_MINI_GAME_LIST_NOT sPacket)
	{
		NKCScenManager.CurrentUserData().ActiveMiniGameTemplets(sPacket.templetIds);
	}

	public static void OnRecv(NKMPacket_MINI_GAME_INFO_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		nKMUserData.SetMiniGameData(sPacket.miniGameData);
		nKMUserData.SetMiniGameReceviedIds(sPacket.rewardIds);
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_HOME)
		{
			return;
		}
		foreach (NKCUIModuleHome item in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
		{
			if (item.IsOpen && item.GetComponent<NKCUIModuleSubUISwordTraining>() != null)
			{
				item.UpdateUI();
			}
		}
	}

	public static void OnRecv(NKMPacket_MINI_GAME_RESULT_ACK sPacket)
	{
		NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode);
	}

	public static void OnRecv(NKMPacket_MINI_GAME_REWARD_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.SetMiniGameReceviedIds(sPacket.rewardId);
				nKMUserData.GetReward(sPacket.rewardData);
				NKCUIResult.Instance.OpenComplexResult(nKMUserData.m_ArmyData, sPacket.rewardData, OnCloseEventMiniGameReward, 0L);
			}
		}
	}

	public static void OnRecv(NKMPacket_MINI_GAME_REWARD_ALL_ACK sPacket)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.SetMiniGameReceviedIds(sPacket.rewardIds);
				nKMUserData.GetReward(sPacket.rewardData);
				NKCUIResult.Instance.OpenComplexResult(nKMUserData.m_ArmyData, sPacket.rewardData, OnCloseEventMiniGameReward, 0L);
			}
		}
	}
}
