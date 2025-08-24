using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Game;
using ClientPacket.Lobby;
using ClientPacket.Pvp;
using Cs.Logging;
using NKC.PacketHandler;
using NKC.Publisher;
using NKC.UI;
using NKC.UI.Gauntlet;
using NKC.UI.Result;
using NKM;
using NKM.Templet;

namespace NKC;

public static class NKCLeaguePVPMgr
{
	private static DraftPvpRoomData m_DraftPvpRoomData = null;

	private static DraftPvpRoomData m_ReservedPvpRoomData = null;

	public const int MAX_GLOBAL_BAN_COUNT_UNIT = 2;

	public const int MAX_GLOBAL_BAN_COUNT_SHIP = 1;

	private static NKM_TEAM_TYPE m_myTeamType = NKM_TEAM_TYPE.NTT_INVALID;

	public static bool m_LeagueRoomStarted = false;

	private static int m_requestedBanIndex = -1;

	private static List<PvpPickRateData> pickRates = null;

	public static DraftPvpRoomData DraftRoomData => m_DraftPvpRoomData;

	public static NKM_TEAM_TYPE MyTeamType => m_myTeamType;

	public static NKM_TEAM_TYPE OponentTeamType
	{
		get
		{
			if (m_myTeamType != NKM_TEAM_TYPE.NTT_A1)
			{
				return NKM_TEAM_TYPE.NTT_A1;
			}
			return NKM_TEAM_TYPE.NTT_B1;
		}
	}

	public static List<PvpPickRateData> PickRateData => pickRates;

	public static void InitDraftRoom(DraftPvpRoomData draftPvpRoomData)
	{
		Log.Debug("[DRAFT] InitDraftRoom [" + draftPvpRoomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 41);
		m_LeagueRoomStarted = false;
		m_DraftPvpRoomData = draftPvpRoomData;
		m_requestedBanIndex = -1;
		UpdateMyTeamType();
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM)
		{
			ChangeScene();
		}
	}

	public static void UpdateMyTeamType()
	{
		m_myTeamType = NKM_TEAM_TYPE.NTT_INVALID;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (m_DraftPvpRoomData.draftTeamDataA.userProfileData.commonProfile.userUid == myUserData.m_UserUID)
		{
			m_myTeamType = m_DraftPvpRoomData.draftTeamDataA.teamType;
		}
		else if (m_DraftPvpRoomData.draftTeamDataB.userProfileData.commonProfile.userUid == myUserData.m_UserUID)
		{
			m_myTeamType = m_DraftPvpRoomData.draftTeamDataB.teamType;
		}
	}

	public static DraftPvpRoomData.DraftTeamData GetMyDraftTeamData()
	{
		if (m_myTeamType == NKM_TEAM_TYPE.NTT_A1 || m_myTeamType == NKM_TEAM_TYPE.NTT_B1)
		{
			return GetDraftTeamData(m_myTeamType);
		}
		return null;
	}

	public static DraftPvpRoomData.DraftTeamData GetLeftDraftTeamData()
	{
		if (m_myTeamType == NKM_TEAM_TYPE.NTT_B1)
		{
			return m_DraftPvpRoomData.draftTeamDataB;
		}
		return m_DraftPvpRoomData.draftTeamDataA;
	}

	public static DraftPvpRoomData.DraftTeamData GetRightDraftTeamData()
	{
		if (m_myTeamType == NKM_TEAM_TYPE.NTT_B1)
		{
			return m_DraftPvpRoomData.draftTeamDataA;
		}
		return m_DraftPvpRoomData.draftTeamDataB;
	}

	public static NKMAsyncUnitData GetTeamLeaderUnit(bool isLeft)
	{
		DraftPvpRoomData.DraftTeamData draftTeamData = (isLeft ? GetLeftDraftTeamData() : GetRightDraftTeamData());
		if (draftTeamData == null)
		{
			return null;
		}
		if (draftTeamData.leaderIndex >= draftTeamData.pickUnitList.Count)
		{
			return null;
		}
		return draftTeamData.pickUnitList[draftTeamData.leaderIndex];
	}

	public static NKCUnitSortSystem.eUnitState GetUnitSlotState(NKM_UNIT_TYPE unitType, int unitID, bool checkMyTeamOnly)
	{
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_SHIP:
		{
			DraftPvpRoomData.DraftTeamData leftDraftTeamData2 = GetLeftDraftTeamData();
			if (leftDraftTeamData2 != null)
			{
				foreach (int item in leftDraftTeamData2.globalBanShipGroupIdList.ToList())
				{
					if (NKCBanManager.IsSameGroupShip(unitID, item))
					{
						return NKCUnitSortSystem.eUnitState.LEAGUE_BANNED;
					}
				}
			}
			DraftPvpRoomData.DraftTeamData rightDraftTeamData2 = GetRightDraftTeamData();
			if (rightDraftTeamData2 == null)
			{
				break;
			}
			foreach (int item2 in rightDraftTeamData2.globalBanShipGroupIdList.ToList())
			{
				if (NKCBanManager.IsSameGroupShip(unitID, item2))
				{
					return NKCUnitSortSystem.eUnitState.LEAGUE_BANNED;
				}
			}
			break;
		}
		case NKM_UNIT_TYPE.NUT_NORMAL:
		{
			DraftPvpRoomData.DraftTeamData leftDraftTeamData = GetLeftDraftTeamData();
			if (leftDraftTeamData != null)
			{
				if (leftDraftTeamData.globalBanUnitIdList.Contains(unitID))
				{
					return NKCUnitSortSystem.eUnitState.LEAGUE_BANNED;
				}
				if (leftDraftTeamData.pickUnitList.Find((NKMAsyncUnitData x) => x.unitId == unitID) != null)
				{
					return NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_LEFT;
				}
			}
			if (checkMyTeamOnly)
			{
				return NKCUnitSortSystem.eUnitState.NONE;
			}
			DraftPvpRoomData.DraftTeamData rightDraftTeamData = GetRightDraftTeamData();
			if (rightDraftTeamData != null)
			{
				if (rightDraftTeamData.globalBanUnitIdList.Contains(unitID))
				{
					return NKCUnitSortSystem.eUnitState.LEAGUE_BANNED;
				}
				if (rightDraftTeamData.pickUnitList.Find((NKMAsyncUnitData x) => x.unitId == unitID) != null)
				{
					return NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_RIGHT;
				}
			}
			break;
		}
		}
		return NKCUnitSortSystem.eUnitState.NONE;
	}

	public static List<int> GetPickEnabledSlot(NKM_TEAM_TYPE teamType)
	{
		List<int> list = new List<int>();
		switch (m_DraftPvpRoomData.roomState)
		{
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_1:
			if (teamType == NKM_TEAM_TYPE.NTT_A1)
			{
				list.Add(0);
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_2:
			if (teamType == NKM_TEAM_TYPE.NTT_B1)
			{
				list.Add(0);
				list.Add(1);
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_3:
			if (teamType == NKM_TEAM_TYPE.NTT_A1)
			{
				list.Add(1);
				list.Add(2);
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_4:
			if (teamType == NKM_TEAM_TYPE.NTT_B1)
			{
				list.Add(2);
				list.Add(3);
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_5:
			if (teamType == NKM_TEAM_TYPE.NTT_A1)
			{
				list.Add(3);
				list.Add(4);
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_6:
			if (teamType == NKM_TEAM_TYPE.NTT_B1)
			{
				list.Add(4);
				list.Add(5);
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_7:
			if (teamType == NKM_TEAM_TYPE.NTT_A1)
			{
				list.Add(5);
				list.Add(6);
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_8:
			if (teamType == NKM_TEAM_TYPE.NTT_B1)
			{
				list.Add(6);
				list.Add(7);
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_9:
			if (teamType == NKM_TEAM_TYPE.NTT_A1)
			{
				list.Add(7);
				list.Add(8);
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_10:
			if (teamType == NKM_TEAM_TYPE.NTT_B1)
			{
				list.Add(8);
			}
			break;
		}
		return list;
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_ACCEPT_NOT sPacket)
	{
		NKCPopupOKCancel.ClosePopupBox();
		if (NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH() != null && NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().NKCUIGuantletMatch != null)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().NKCUIGuantletMatch.OnRecv(sPacket);
		}
		InitDraftRoom(sPacket.roomData);
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_UPDATED_NOT sPacket)
	{
		if (!m_LeagueRoomStarted)
		{
			Log.Info("[League][Relogin] Reserved Roomdata [" + sPacket.roomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 228);
			m_ReservedPvpRoomData = sPacket.roomData;
			return;
		}
		if (m_DraftPvpRoomData == null || m_DraftPvpRoomData.roomState != sPacket.roomData.roomState)
		{
			Log.Info($"[League][RoomStateChanged] [{m_DraftPvpRoomData.roomState.ToString()}] -> [{sPacket.roomData.roomState}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 235);
			m_DraftPvpRoomData = sPacket.roomData;
			OnRoomStateChanged();
			return;
		}
		m_DraftPvpRoomData = sPacket.roomData;
		switch (m_DraftPvpRoomData.roomState)
		{
		case DRAFT_PVP_ROOM_STATE.INIT:
		case DRAFT_PVP_ROOM_STATE.BAN_ALL:
		case DRAFT_PVP_ROOM_STATE.BAN_COMPLETE:
			RefreshBanProgress();
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_1:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_2:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_3:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_4:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_5:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_6:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_7:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_8:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_9:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_10:
			RefreshPickProgress(roomStateChanged: false);
			ShowPickNotice();
			break;
		case DRAFT_PVP_ROOM_STATE.BAN_OPPONENT:
		case DRAFT_PVP_ROOM_STATE.PICK_ETC:
		case DRAFT_PVP_ROOM_STATE.DRAFT_COMPLETE:
			RefreshPickProgress(roomStateChanged: false);
			break;
		}
	}

	public static void OnRoomStateChanged()
	{
		CheckLeaveRoomState();
		switch (m_DraftPvpRoomData.roomState)
		{
		case DRAFT_PVP_ROOM_STATE.INIT:
			Log.Debug("[DRAFT] OpenGlobalBan [" + m_DraftPvpRoomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 279);
			OpenGlobalBan();
			break;
		case DRAFT_PVP_ROOM_STATE.BAN_ALL:
			Log.Debug("[DRAFT] OpenCandidateList [" + m_DraftPvpRoomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 285);
			OpenCandidateList();
			break;
		case DRAFT_PVP_ROOM_STATE.BAN_COMPLETE:
			Log.Debug("[DRAFT] OpenFinalResult [" + m_DraftPvpRoomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 291);
			OpenFinalResult();
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_1:
			Log.Debug("[DRAFT] OpenPickSequence [" + m_DraftPvpRoomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 297);
			OpenPickSequence();
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_2:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_3:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_4:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_5:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_6:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_7:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_8:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_9:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_10:
			Log.Debug("[DRAFT] RefreshPickProgress [" + m_DraftPvpRoomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 311);
			RefreshPickProgress(roomStateChanged: true);
			ShowPickNotice();
			break;
		case DRAFT_PVP_ROOM_STATE.BAN_OPPONENT:
			Log.Debug("[DRAFT] RefreshPickProgress [" + m_DraftPvpRoomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 318);
			RefreshPickProgress(roomStateChanged: true);
			if (!IsObserver())
			{
				ShowSequenceGuidePopup(NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_LOCAL_BAN"));
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_ETC:
			Log.Debug("[DRAFT] RefreshPickProgress [" + m_DraftPvpRoomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 327);
			RefreshPickProgress(roomStateChanged: true);
			break;
		case DRAFT_PVP_ROOM_STATE.DRAFT_COMPLETE:
			Log.Debug("[DRAFT] RefreshPickProgress [" + m_DraftPvpRoomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 333);
			RefreshPickProgress(roomStateChanged: true);
			OpenMatchUI();
			break;
		}
	}

	public static void ChangeScene()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM);
	}

	public static void CancelLeaguePvp()
	{
	}

	public static void OpenGlobalBan()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM();
	}

	public static void OpenCandidateList()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM();
	}

	public static void OpenFinalResult()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM();
	}

	public static void RefreshBanProgress()
	{
		if (NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM() != null)
		{
			GetLeftDraftTeamData();
			GetRightDraftTeamData();
		}
	}

	public static void Send_NKMPacket_LEAGUE_PVP_GIVEUP_REQ()
	{
		NKMPacket_LEAGUE_PVP_GIVEUP_REQ packet = new NKMPacket_LEAGUE_PVP_GIVEUP_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_GIVEUP_ACK sPacket)
	{
	}

	public static bool OnRecvExit()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM)
		{
			return false;
		}
		NKCPopupOKCancel.ClosePopupBox();
		CancelAllProcess();
		return true;
	}

	public static bool OnRecv(NKMPacket_EVENT_PVP_EXIT_ACK sNKMPacket_EVENT_PVP_EXIT_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM)
		{
			return false;
		}
		NKCPopupOKCancel.ClosePopupBox();
		CancelAllProcess(NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
		return true;
	}

	public static void Send_NKMPacket_EVENT_PVP_EXIT_REQ()
	{
		NKMPacket_EVENT_PVP_EXIT_REQ packet = new NKMPacket_EVENT_PVP_EXIT_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static bool OnRecv(NKMPacket_EVENT_PVP_CANCEL_NOT sPacket)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM)
		{
			return false;
		}
		if (!IsPlayer(sPacket.targetUserUid))
		{
			Log.Debug($"[EventPvpRoom] TargetUser[{sPacket.targetUserUid}] CancelType[{sPacket.cancelType.ToString()}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 457);
			return false;
		}
		NKCPopupOKCancel.ClosePopupBox();
		CancelAllProcess(NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
		return true;
	}

	public static bool CanPickUnit(NKM_TEAM_TYPE teamType)
	{
		switch (m_DraftPvpRoomData.roomState)
		{
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_1:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_3:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_5:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_7:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_9:
			if (teamType == NKM_TEAM_TYPE.NTT_A1)
			{
				return true;
			}
			break;
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_2:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_4:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_6:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_8:
		case DRAFT_PVP_ROOM_STATE.PICK_UNIT_10:
			if (teamType == NKM_TEAM_TYPE.NTT_B1)
			{
				return true;
			}
			break;
		}
		return false;
	}

	public static bool CanPickETC()
	{
		if (m_DraftPvpRoomData.roomState == DRAFT_PVP_ROOM_STATE.PICK_ETC)
		{
			return true;
		}
		return false;
	}

	public static int GetCurrentSelectedSlot(NKM_TEAM_TYPE teamType)
	{
		if (!CanPickUnit(teamType))
		{
			return -1;
		}
		return GetDraftTeamData(teamType)?.pickUnitList.Count ?? (-1);
	}

	public static void OpenPickSequence()
	{
		NKC_SCEN_GAUNTLET_LEAGUE_ROOM nKC_SCEN_GAUNTLET_LEAGUE_ROOM = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM();
		if (nKC_SCEN_GAUNTLET_LEAGUE_ROOM != null)
		{
			nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.Open(m_DraftPvpRoomData.stateEndTime);
			if (!IsObserver())
			{
				nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.OpenUnitSelect();
			}
			CheckLeaveRoomState();
			ShowPickNotice();
		}
	}

	public static void ShowPickNotice()
	{
		string text = "";
		NKM_TEAM_TYPE nKM_TEAM_TYPE = NKM_TEAM_TYPE.NTT_INVALID;
		if (IsObserver())
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM()?.m_gauntletLeagueMain.HideSequenceGuidePopup();
			return;
		}
		if (!CanPickUnit(m_myTeamType))
		{
			nKM_TEAM_TYPE = OponentTeamType;
			text = "SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_OPPONENT";
		}
		else
		{
			nKM_TEAM_TYPE = MyTeamType;
			text = "SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_USER";
		}
		List<int> pickEnabledSlot = GetPickEnabledSlot(nKM_TEAM_TYPE);
		if (pickEnabledSlot.Count > 0)
		{
			int count = pickEnabledSlot.Count;
			int num = pickEnabledSlot[0];
			int num2 = Math.Min(GetCurrentSelectedSlot(nKM_TEAM_TYPE) - num, count);
			ShowSequenceGuidePopup($"{NKCStringTable.GetString(text)} {num2}/{count}");
		}
	}

	public static void ShowSequenceGuidePopup(string textMessage)
	{
		if (!IsObserver())
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM()?.m_gauntletLeagueMain.ShowSequenceGuidePopup(textMessage);
		}
	}

	public static void RefreshPickProgress(bool roomStateChanged)
	{
		NKC_SCEN_GAUNTLET_LEAGUE_ROOM nKC_SCEN_GAUNTLET_LEAGUE_ROOM = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM();
		if (nKC_SCEN_GAUNTLET_LEAGUE_ROOM == null)
		{
			return;
		}
		if (!nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.IsOpen)
		{
			nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.Open(m_DraftPvpRoomData.stateEndTime);
			if (!IsObserver())
			{
				nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.OpenUnitSelect();
			}
		}
		nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.RefreshDraftData(m_DraftPvpRoomData.stateEndTime, roomStateChanged);
		if (roomStateChanged)
		{
			nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.UpdatePickAnimation();
			if (m_DraftPvpRoomData.roomState == DRAFT_PVP_ROOM_STATE.PICK_ETC)
			{
				nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.OpenShipSelect(IsObserver());
			}
		}
	}

	public static void Send_NKMPacket_DRAFT_PVP_PICK_UNIT_REQ(long unitUID)
	{
		NKMPacket_DRAFT_PVP_PICK_UNIT_REQ nKMPacket_DRAFT_PVP_PICK_UNIT_REQ = new NKMPacket_DRAFT_PVP_PICK_UNIT_REQ();
		nKMPacket_DRAFT_PVP_PICK_UNIT_REQ.unitUid = unitUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DRAFT_PVP_PICK_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_PICK_UNIT_ACK sPacket)
	{
	}

	public static void Send_NKMPacket_DRAFT_PVP_OPPONENT_BAN_REQ(int slotIndex)
	{
		m_requestedBanIndex = slotIndex;
		NKMPacket_DRAFT_PVP_OPPONENT_BAN_REQ nKMPacket_DRAFT_PVP_OPPONENT_BAN_REQ = new NKMPacket_DRAFT_PVP_OPPONENT_BAN_REQ();
		nKMPacket_DRAFT_PVP_OPPONENT_BAN_REQ.unitIndex = slotIndex;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DRAFT_PVP_OPPONENT_BAN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_OPPONENT_BAN_ACK sPacket)
	{
		if (m_requestedBanIndex != -1)
		{
			GetRightDraftTeamData().banishedUnitIndex = m_requestedBanIndex;
			m_requestedBanIndex = -1;
		}
		NKC_SCEN_GAUNTLET_LEAGUE_ROOM nKC_SCEN_GAUNTLET_LEAGUE_ROOM = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM();
		if (nKC_SCEN_GAUNTLET_LEAGUE_ROOM != null)
		{
			nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.OnRecv(sPacket);
			RefreshPickProgress(roomStateChanged: false);
		}
	}

	public static void Send_NKMPacket_DRAFT_PVP_PICK_SHIP_REQ(long shipUID)
	{
		NKMPacket_DRAFT_PVP_PICK_SHIP_REQ nKMPacket_DRAFT_PVP_PICK_SHIP_REQ = new NKMPacket_DRAFT_PVP_PICK_SHIP_REQ();
		nKMPacket_DRAFT_PVP_PICK_SHIP_REQ.shipUid = shipUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DRAFT_PVP_PICK_SHIP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_PICK_SHIP_ACK sPacket)
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM()?.m_gauntletLeagueMain.OpenOperatorSelect(IsObserver());
	}

	public static void Send_NKMPacket_DRAFT_PVP_PICK_OPERATOR_REQ(long operatorUID)
	{
		NKMPacket_DRAFT_PVP_PICK_OPERATOR_REQ nKMPacket_DRAFT_PVP_PICK_OPERATOR_REQ = new NKMPacket_DRAFT_PVP_PICK_OPERATOR_REQ();
		nKMPacket_DRAFT_PVP_PICK_OPERATOR_REQ.operatorUid = operatorUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DRAFT_PVP_PICK_OPERATOR_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_PICK_OPERATOR_ACK sPacket)
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM()?.m_gauntletLeagueMain.OpenLeaderSelect();
	}

	public static void Send_NKMPacket_DRAFT_PVP_PICK_LEADER_REQ(int leaderIndex)
	{
		NKMPacket_DRAFT_PVP_PICK_LEADER_REQ nKMPacket_DRAFT_PVP_PICK_LEADER_REQ = new NKMPacket_DRAFT_PVP_PICK_LEADER_REQ();
		nKMPacket_DRAFT_PVP_PICK_LEADER_REQ.leaderIndex = leaderIndex;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DRAFT_PVP_PICK_LEADER_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_PICK_LEADER_ACK sPacket)
	{
		NKC_SCEN_GAUNTLET_LEAGUE_ROOM nKC_SCEN_GAUNTLET_LEAGUE_ROOM = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM();
		if (nKC_SCEN_GAUNTLET_LEAGUE_ROOM != null)
		{
			nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.ApplyPickETCResults();
			if (GetRightDraftTeamData().leaderIndex == -1)
			{
				ShowSequenceGuidePopup(NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_PICK_SEQUENCE_WAIT"));
			}
		}
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_SELECT_UNIT_ACK sPacket)
	{
	}

	public static void Send_NKMPacket_DRAFT_PVP_SELECT_UNIT_REQ(long unitUID)
	{
		if (CanPickUnit(MyTeamType))
		{
			NKMPacket_DRAFT_PVP_SELECT_UNIT_REQ nKMPacket_DRAFT_PVP_SELECT_UNIT_REQ = new NKMPacket_DRAFT_PVP_SELECT_UNIT_REQ();
			nKMPacket_DRAFT_PVP_SELECT_UNIT_REQ.unitUid = unitUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DRAFT_PVP_SELECT_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}
	}

	public static void OpenMatchUI()
	{
		NKC_SCEN_GAUNTLET_LEAGUE_ROOM nKC_SCEN_GAUNTLET_LEAGUE_ROOM = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM();
		if (nKC_SCEN_GAUNTLET_LEAGUE_ROOM != null)
		{
			nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.HideSequenceGuidePopup();
			nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMain.Close();
			nKC_SCEN_GAUNTLET_LEAGUE_ROOM.m_gauntletLeagueMatch.Open(m_DraftPvpRoomData.stateEndTime);
		}
	}

	public static DraftPvpRoomData.DraftTeamData GetDraftTeamData(NKM_TEAM_TYPE targetTeamType)
	{
		if (m_DraftPvpRoomData.draftTeamDataA.teamType == targetTeamType)
		{
			return m_DraftPvpRoomData.draftTeamDataA;
		}
		return m_DraftPvpRoomData.draftTeamDataB;
	}

	public static bool SelectGlobalBanUnit(NKMUnitTempletBase unitTempletBase)
	{
		if (GetDraftTeamData(m_myTeamType).globalBanUnitIdList.Count >= 2)
		{
			return false;
		}
		NKMPacket_DRAFT_PVP_GLOBAL_BAN_REQ nKMPacket_DRAFT_PVP_GLOBAL_BAN_REQ = new NKMPacket_DRAFT_PVP_GLOBAL_BAN_REQ();
		nKMPacket_DRAFT_PVP_GLOBAL_BAN_REQ.unitId = unitTempletBase.m_UnitID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DRAFT_PVP_GLOBAL_BAN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		return true;
	}

	public static void OnRecv(NKMPacket_DRAFT_PVP_GLOBAL_BAN_ACK sPacket)
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM();
	}

	public static void OnRecv(NKMPacket_PVP_GAME_MATCH_COMPLETE_NOT cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT)
	{
		m_DraftPvpRoomData = null;
		m_myTeamType = NKM_TEAM_TYPE.NTT_INVALID;
	}

	public static void ProcessReLogin(NKMGameData gameData, DraftPvpRoomData draftPvpRoomData)
	{
		NKCCollectionManager.Init();
		if (m_DraftPvpRoomData == null)
		{
			InitDraftRoom(new DraftPvpRoomData
			{
				roomState = DRAFT_PVP_ROOM_STATE.INIT,
				draftTeamDataA = draftPvpRoomData.draftTeamDataA,
				draftTeamDataB = draftPvpRoomData.draftTeamDataB,
				stateEndTime = draftPvpRoomData.stateEndTime
			});
			m_ReservedPvpRoomData = draftPvpRoomData;
		}
		else
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM)
			{
				ChangeScene();
			}
			m_ReservedPvpRoomData = draftPvpRoomData;
			UpdateReservedRoomData();
		}
	}

	public static void UpdateReservedRoomData()
	{
		if (m_ReservedPvpRoomData != null)
		{
			NKMPacket_LEAGUE_PVP_UPDATED_NOT sPacket = new NKMPacket_LEAGUE_PVP_UPDATED_NOT
			{
				roomData = m_ReservedPvpRoomData
			};
			Log.Info("[League][Relogin] RoomState[" + m_ReservedPvpRoomData.roomState.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 848);
			OnRecv(sPacket);
		}
		m_ReservedPvpRoomData = null;
	}

	public static void SetPickRateData(List<PvpPickRateData> pickRatesData)
	{
		pickRates = pickRatesData;
	}

	public static PvpPickRateData GetPickRateData(PvpPickType rankType)
	{
		return pickRates.Find((PvpPickRateData e) => e.type == rankType);
	}

	public static bool OnRecv(NKMPacket_GAME_END_NOT cPacket_GAME_END_NOT)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM)
		{
			return false;
		}
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
		NKCScenManager.GetScenManager().GetGameClient();
		NKCUIResult.BattleResultData resultUIData = NKCUIResult.MakePvPResultData(bATTLE_RESULT_TYPE, cNKMItemMiscData, new NKCUIBattleStatistics.BattleData(), NKM_GAME_TYPE.NGT_PVP_LEAGUE);
		NKCUIGauntletResult.SetResultData(resultUIData);
		if (cPacket_GAME_END_NOT.pvpResultData != null)
		{
			NKCPacketHandlersLobby.UpdateLeagueGiveupUserData(cPacket_GAME_END_NOT.pvpResultData);
		}
		NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(cPacket_GAME_END_NOT.costItemDataList);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().SetDoAtScenStart(delegate
		{
			NKCUIManager.NKCUIGauntletResult.Open(delegate
			{
				NKCUIResult.Instance.OpenComplexResult(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData, resultUIData.m_RewardData, delegate
				{
					NKCContentManager.ShowContentUnlockPopup(delegate
					{
						if (NKCPrivatePVPRoomMgr.LobbyData != null)
						{
							NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM);
						}
						else
						{
							NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
						}
					}, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_RANK_SCORE, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_RANK_SCORE_RECORD, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_ASYNC_SCORE, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_ASYNC_SCORE_RECORD);
				}, resultUIData.m_OrgDoubleToken, resultUIData.m_battleData, bIgnoreAutoClose: true, bAllowRewardDataNull: true);
			});
		});
		return true;
	}

	public static void CheckLeagueModeBuff(NKMBuffData buffData, NKCUnitClient unitClient)
	{
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient != null)
		{
			if (buffData.m_NKMBuffTemplet == NKMPvpCommonConst.Instance.LeaguePvp.UiRageBuff)
			{
				Log.Info("[Leage] RageMod On!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 954);
				gameClient.GetGameHud().SetRageMode(bRageMode: true, unitClient.IsMyTeam());
			}
			if (buffData.m_NKMBuffTemplet == NKMPvpCommonConst.Instance.LeaguePvp.UiDeadlineBuff && gameClient.GetGameHud().DeadlineBuffLevel < buffData.m_BuffSyncData.m_BuffStatLevel)
			{
				Log.Info($"[Leage] Deadline On[{buffData.m_BuffSyncData.m_BuffStatLevel}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaguePVPMgr.cs", 961);
				gameClient.GetGameHud().SetDeadlineMode(buffData.m_BuffSyncData.m_BuffStatLevel, unitClient.GetBuffDescText(NKMPvpCommonConst.Instance.LeaguePvp.DeadlineBuff, buffData.m_BuffSyncData));
			}
		}
	}

	public static bool CanLeaveRoom()
	{
		if (DraftRoomData.roomState >= DRAFT_PVP_ROOM_STATE.BAN_ALL && DraftRoomData.roomState < DRAFT_PVP_ROOM_STATE.PICK_ETC)
		{
			return true;
		}
		return false;
	}

	public static void CheckLeaveRoomState()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM()?.m_gauntletLeagueMain.SetLeaveRoomState();
	}

	public static void OnRecv(NKMPacket_PRIVATE_PVP_DRAFT_GIVEUP_NOT sPacket)
	{
		if (NKCPrivatePVPRoomMgr.LobbyData != null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM);
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	public static bool IsObserver()
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM))
		{
			return false;
		}
		if (m_myTeamType != NKM_TEAM_TYPE.NTT_A1 && m_myTeamType != NKM_TEAM_TYPE.NTT_B1)
		{
			return true;
		}
		return false;
	}

	public static bool IsPlayer(long userUid)
	{
		if (m_DraftPvpRoomData.draftTeamDataA.userProfileData.commonProfile.userUid == userUid || m_DraftPvpRoomData.draftTeamDataB.userProfileData.commonProfile.userUid == userUid)
		{
			return true;
		}
		return false;
	}

	public static bool IsPrivate()
	{
		if (m_DraftPvpRoomData == null)
		{
			return false;
		}
		if (m_DraftPvpRoomData.gameType == NKM_GAME_TYPE.NGT_PVP_PRIVATE)
		{
			return true;
		}
		return false;
	}

	public static bool IsEvent()
	{
		if (m_DraftPvpRoomData == null)
		{
			return false;
		}
		if (m_DraftPvpRoomData.gameType == NKM_GAME_TYPE.NGT_PVP_EVENT)
		{
			return true;
		}
		return false;
	}

	public static bool IsLeague()
	{
		if (m_DraftPvpRoomData == null)
		{
			return false;
		}
		if (m_DraftPvpRoomData.gameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE)
		{
			return true;
		}
		return false;
	}

	private static void CancelAllProcess(NKC_GAUNTLET_LOBBY_TAB receiveTab = NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE)
	{
		ResetData();
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LEAGUE_ROOM)
		{
			if (NKCPrivatePVPRoomMgr.LobbyData != null)
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM);
				return;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(receiveTab);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
		}
	}

	private static void ResetData()
	{
		m_DraftPvpRoomData = null;
		m_ReservedPvpRoomData = null;
		m_myTeamType = NKM_TEAM_TYPE.NTT_INVALID;
		m_LeagueRoomStarted = false;
	}
}
