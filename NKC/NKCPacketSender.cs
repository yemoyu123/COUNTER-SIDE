using System.Collections.Generic;
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
using Cs.Core.Util;
using Cs.Logging;
using NKC.PacketHandler;
using NKC.UI;
using NKM;
using NKM.Contract2;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKCPacketSender
{
	private static NKMPacket_CONNECT_CHECK_REQ m_NKMPacket_CONNECT_CHECK_REQ = new NKMPacket_CONNECT_CHECK_REQ();

	public static void Send_NKMPacket_PHASE_START_REQ(int stageId, NKMDeckIndex deckIndex, long supportUserUID)
	{
		NKMPacket_PHASE_START_REQ nKMPacket_PHASE_START_REQ = new NKMPacket_PHASE_START_REQ();
		nKMPacket_PHASE_START_REQ.stageId = stageId;
		nKMPacket_PHASE_START_REQ.deckIndex = deckIndex;
		nKMPacket_PHASE_START_REQ.eventDeckData = null;
		nKMPacket_PHASE_START_REQ.supportingUserUid = supportUserUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PHASE_START_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void Send_NKMPacket_PHASE_START_REQ(int stageId, NKMEventDeckData eventDeckData, long supportUserUID)
	{
		NKMPacket_PHASE_START_REQ nKMPacket_PHASE_START_REQ = new NKMPacket_PHASE_START_REQ();
		nKMPacket_PHASE_START_REQ.stageId = stageId;
		nKMPacket_PHASE_START_REQ.deckIndex = NKMDeckIndex.None;
		nKMPacket_PHASE_START_REQ.eventDeckData = eventDeckData;
		nKMPacket_PHASE_START_REQ.supportingUserUid = supportUserUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PHASE_START_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void Send_NKMPacket_GAME_LOAD_REQ(byte DeckIndex, int stageID, int diveID, string dungeonStrID, int palaceID, bool bLocal = false, int multiplyReward = 1, int fierceID = 0, long supportingUserUid = 0L)
	{
		Send_NKMPacket_GAME_LOAD_REQ(DeckIndex, stageID, diveID, NKMDungeonManager.GetDungeonID(dungeonStrID), palaceID, bLocal, multiplyReward, fierceID, supportingUserUid);
	}

	public static void Send_NKMPacket_GAME_LOAD_REQ(byte DeckIndex, int stageID, int diveID, int dungeonID, int palaceID, bool bLocal = false, int multiplyReward = 1, int fierceID = 0, long supportingUserUid = 0L)
	{
		NKMPacket_GAME_LOAD_REQ nKMPacket_GAME_LOAD_REQ = new NKMPacket_GAME_LOAD_REQ();
		nKMPacket_GAME_LOAD_REQ.isDev = false;
		nKMPacket_GAME_LOAD_REQ.selectDeckIndex = DeckIndex;
		nKMPacket_GAME_LOAD_REQ.stageID = stageID;
		nKMPacket_GAME_LOAD_REQ.diveStageID = diveID;
		nKMPacket_GAME_LOAD_REQ.dungeonID = dungeonID;
		nKMPacket_GAME_LOAD_REQ.eventDeckData = null;
		nKMPacket_GAME_LOAD_REQ.palaceID = palaceID;
		nKMPacket_GAME_LOAD_REQ.rewardMultiply = multiplyReward;
		nKMPacket_GAME_LOAD_REQ.fierceBossId = fierceID;
		nKMPacket_GAME_LOAD_REQ.supportingUserUid = supportingUserUid;
		if (!bLocal)
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_LOAD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			return;
		}
		NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_LOAD_REQ);
		NKMPopUpBox.OpenWaitBox();
	}

	public static void Send_NKMPacket_GAME_LOAD_REQ(NKMEventDeckData eventDeckData, int stageID, int diveID, string dungeonStrID, int palaceID, bool bLocal = false, int multiplyReward = 1, int fierceID = 0)
	{
		Send_NKMPacket_GAME_LOAD_REQ(eventDeckData, stageID, diveID, NKMDungeonManager.GetDungeonID(dungeonStrID), palaceID, bLocal, multiplyReward, fierceID, 0L);
	}

	public static void Send_NKMPacket_GAME_LOAD_REQ(NKMEventDeckData eventDeckData, int stageID, int diveID, int dungeonID, int palaceID, bool bLocal = false, int multiplyReward = 1, int fierceID = 0, long supportUserUID = 0L)
	{
		NKMPacket_GAME_LOAD_REQ nKMPacket_GAME_LOAD_REQ = new NKMPacket_GAME_LOAD_REQ();
		nKMPacket_GAME_LOAD_REQ.isDev = false;
		nKMPacket_GAME_LOAD_REQ.selectDeckIndex = 0;
		nKMPacket_GAME_LOAD_REQ.stageID = stageID;
		nKMPacket_GAME_LOAD_REQ.diveStageID = diveID;
		nKMPacket_GAME_LOAD_REQ.dungeonID = dungeonID;
		nKMPacket_GAME_LOAD_REQ.eventDeckData = eventDeckData;
		nKMPacket_GAME_LOAD_REQ.rewardMultiply = multiplyReward;
		nKMPacket_GAME_LOAD_REQ.palaceID = palaceID;
		nKMPacket_GAME_LOAD_REQ.fierceBossId = fierceID;
		nKMPacket_GAME_LOAD_REQ.supportingUserUid = supportUserUID;
		if (!bLocal)
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_LOAD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			return;
		}
		NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_LOAD_REQ);
		NKMPopUpBox.OpenWaitBox();
	}

	public static void Send_NKMPacket_GAME_RESTART_REQ()
	{
		NKMPacket_GAME_RESTART_REQ packet = new NKMPacket_GAME_RESTART_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void Send_NKMPacket_PRACTICE_GAME_LOAD_REQ(NKMUnitData cNKMUnitData, int dungeonID)
	{
		NKCLocalPacketHandler.SendPacketToLocalServer(new NKMPacket_PRACTICE_GAME_LOAD_REQ
		{
			practiceUnitData = cNKMUnitData,
			dungeonID = dungeonID
		});
		NKMPopUpBox.OpenWaitBox();
	}

	public static void Send_NKMPacket_INFORM_MY_LOADING_PROGRESS_REQ(byte progress)
	{
		NKMPacket_INFORM_MY_LOADING_PROGRESS_REQ nKMPacket_INFORM_MY_LOADING_PROGRESS_REQ = new NKMPacket_INFORM_MY_LOADING_PROGRESS_REQ();
		nKMPacket_INFORM_MY_LOADING_PROGRESS_REQ.progress = progress;
		if (NKCReplayMgr.IsPlayingReplay())
		{
			NKCReplayMgr.GetNKCReplaMgr()?.OnRecv(nKMPacket_INFORM_MY_LOADING_PROGRESS_REQ);
		}
		else
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_INFORM_MY_LOADING_PROGRESS_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
	}

	public static void Send_NKMPacket_DEV_GAME_LOAD_REQ(string dungeonStrID)
	{
		Send_NKMPacket_DEV_GAME_LOAD_REQ(NKMDungeonManager.GetDungeonID(dungeonStrID));
	}

	public static void Send_NKMPacket_DEV_GAME_LOAD_REQ(int dungeonID)
	{
		NKCLocalPacketHandler.SendPacketToLocalServer(new NKMPacket_GAME_LOAD_REQ
		{
			isDev = true,
			dungeonID = dungeonID
		});
		NKMPopUpBox.OpenWaitBox();
	}

	public static void Send_NKMPacket_DEV_GAME_LOAD_REQ(NKMGameData gameData, NKMGameRuntimeData runtimeGameData)
	{
		NKMPacket_DEV_GAME_LOAD_REQ nKMPacket_DEV_GAME_LOAD_REQ = new NKMPacket_DEV_GAME_LOAD_REQ();
		ClearGameTeamUnitUID(gameData.m_NKMGameTeamDataA);
		ClearGameTeamUnitUID(gameData.m_NKMGameTeamDataB);
		gameData.m_bLocal = true;
		nKMPacket_DEV_GAME_LOAD_REQ.gamedata = gameData;
		nKMPacket_DEV_GAME_LOAD_REQ.gameRuntimeData = runtimeGameData;
		NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_DEV_GAME_LOAD_REQ);
		NKMPopUpBox.OpenWaitBox();
		static void ClearGameTeamUnitUID(NKMGameTeamData cNKMGameTeamData)
		{
			ClearGameUnitUID(cNKMGameTeamData.m_MainShip);
			for (int i = 0; i < cNKMGameTeamData.m_listUnitData.Count; i++)
			{
				ClearGameUnitUID(cNKMGameTeamData.m_listUnitData[i]);
			}
			for (int j = 0; j < cNKMGameTeamData.m_listAssistUnitData.Count; j++)
			{
				ClearGameUnitUID(cNKMGameTeamData.m_listAssistUnitData[j]);
			}
			for (int k = 0; k < cNKMGameTeamData.m_listEvevtUnitData.Count; k++)
			{
				ClearGameUnitUID(cNKMGameTeamData.m_listEvevtUnitData[k]);
			}
			for (int l = 0; l < cNKMGameTeamData.m_listEnvUnitData.Count; l++)
			{
				ClearGameUnitUID(cNKMGameTeamData.m_listEnvUnitData[l]);
			}
			for (int m = 0; m < cNKMGameTeamData.m_listOperatorUnitData.Count; m++)
			{
				ClearGameUnitUID(cNKMGameTeamData.m_listOperatorUnitData[m]);
			}
		}
		static void ClearGameUnitUID(NKMUnitData unitData)
		{
			unitData.m_listGameUnitUID.Clear();
		}
	}

	public static void Send_Packet_GAME_LOAD_COMPLETE_REQ(bool bIntrude)
	{
		NKMPacket_GAME_LOAD_COMPLETE_REQ nKMPacket_GAME_LOAD_COMPLETE_REQ = new NKMPacket_GAME_LOAD_COMPLETE_REQ();
		nKMPacket_GAME_LOAD_COMPLETE_REQ.isIntrude = bIntrude;
		Debug.Log($"[NKMPacket_GAME_LOAD_COMPLETE_REQ] isIntrude : {nKMPacket_GAME_LOAD_COMPLETE_REQ.isIntrude}");
		if (NKCReplayMgr.IsPlayingReplay())
		{
			NKCReplayMgr.GetNKCReplaMgr()?.OnRecv(nKMPacket_GAME_LOAD_COMPLETE_REQ);
		}
		else if (!NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.m_bLocal)
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_LOAD_COMPLETE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
		else
		{
			NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_LOAD_COMPLETE_REQ);
		}
	}

	public static void Send_NKMPacket_GAME_GIVEUP_REQ()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			if (NKCScenManager.GetScenManager().GetGameClient() != null && NKCScenManager.GetScenManager().GetGameClient().GetGameData() != null && NKCScenManager.GetScenManager().GetGameClient().GetGameData()
				.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_TUTORIAL && !NKCTutorialManager.CanGiveupDungeon(NKCScenManager.GetScenManager().GetGameClient().GetGameData()
				.m_DungeonID))
				{
					Debug.LogWarning("impossible to giveup prologue dungeon");
					return;
				}
				NKMPacket_GAME_GIVEUP_REQ packet = new NKMPacket_GAME_GIVEUP_REQ();
				NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_CHANGE_NICKNAME_REQ(string nickname)
		{
			NKMPacket_CHANGE_NICKNAME_REQ nKMPacket_CHANGE_NICKNAME_REQ = new NKMPacket_CHANGE_NICKNAME_REQ();
			nKMPacket_CHANGE_NICKNAME_REQ.nickname = nickname;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_CHANGE_NICKNAME_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_NKMPacket_CONNECT_CHECK_REQ()
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(m_NKMPacket_CONNECT_CHECK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL, bSendFailBox: false);
		}

		public static void Send_NKMPacket_LOCK_UNIT_REQ(long targetUnitUID, bool bLock)
		{
			NKMPacket_LOCK_UNIT_REQ nKMPacket_LOCK_UNIT_REQ = new NKMPacket_LOCK_UNIT_REQ();
			nKMPacket_LOCK_UNIT_REQ.unitUID = targetUnitUID;
			nKMPacket_LOCK_UNIT_REQ.isLock = bLock;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LOCK_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_FAVORITE_UNIT_REQ(long targetUnitUID, bool bFavorite)
		{
			NKMPacket_FAVORITE_UNIT_REQ nKMPacket_FAVORITE_UNIT_REQ = new NKMPacket_FAVORITE_UNIT_REQ();
			nKMPacket_FAVORITE_UNIT_REQ.unitUid = targetUnitUID;
			nKMPacket_FAVORITE_UNIT_REQ.isFavorite = bFavorite;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FAVORITE_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_DECK_UNIT_SWAP_REQ(NKMDeckIndex deckIndex, int slotFrom, int slotTo)
		{
			NKMPacket_DECK_UNIT_SWAP_REQ nKMPacket_DECK_UNIT_SWAP_REQ = new NKMPacket_DECK_UNIT_SWAP_REQ();
			nKMPacket_DECK_UNIT_SWAP_REQ.deckIndex = deckIndex;
			nKMPacket_DECK_UNIT_SWAP_REQ.slotIndexFrom = (byte)slotFrom;
			nKMPacket_DECK_UNIT_SWAP_REQ.slotIndexTo = (byte)slotTo;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DECK_UNIT_SWAP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_DECK_UNLOCK_REQ(NKM_DECK_TYPE eType)
		{
			NKMPacket_DECK_UNLOCK_REQ nKMPacket_DECK_UNLOCK_REQ = new NKMPacket_DECK_UNLOCK_REQ();
			nKMPacket_DECK_UNLOCK_REQ.deckType = eType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DECK_UNLOCK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_DECK_UNIT_SET_REQ(NKMDeckIndex deckIndex, int slotIndex, long unitUID)
		{
			NKMPacket_DECK_UNIT_SET_REQ nKMPacket_DECK_UNIT_SET_REQ = new NKMPacket_DECK_UNIT_SET_REQ();
			nKMPacket_DECK_UNIT_SET_REQ.deckIndex = deckIndex;
			nKMPacket_DECK_UNIT_SET_REQ.slotIndex = (byte)slotIndex;
			nKMPacket_DECK_UNIT_SET_REQ.unitUID = unitUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DECK_UNIT_SET_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_DECK_SHIP_SET_REQ(NKMDeckIndex deckIndex, long shipUID)
		{
			NKMPacket_DECK_SHIP_SET_REQ nKMPacket_DECK_SHIP_SET_REQ = new NKMPacket_DECK_SHIP_SET_REQ();
			nKMPacket_DECK_SHIP_SET_REQ.deckIndex = deckIndex;
			nKMPacket_DECK_SHIP_SET_REQ.shipUID = shipUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DECK_SHIP_SET_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_Packet_DECK_UNIT_SET_LEADER_REQ(NKMDeckIndex deckIndex, sbyte leaderIndex)
		{
			NKMPacket_DECK_UNIT_SET_LEADER_REQ nKMPacket_DECK_UNIT_SET_LEADER_REQ = new NKMPacket_DECK_UNIT_SET_LEADER_REQ();
			nKMPacket_DECK_UNIT_SET_LEADER_REQ.deckIndex = deckIndex;
			nKMPacket_DECK_UNIT_SET_LEADER_REQ.leaderSlotIndex = leaderIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DECK_UNIT_SET_LEADER_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_Packet_DECK_UNIT_AUTO_SET_REQ(NKMDeckIndex deckIndex, List<long> unitUIDList, long shipUID, long operatorUID)
		{
			if (unitUIDList != null)
			{
				NKMPacket_DECK_UNIT_AUTO_SET_REQ nKMPacket_DECK_UNIT_AUTO_SET_REQ = new NKMPacket_DECK_UNIT_AUTO_SET_REQ();
				nKMPacket_DECK_UNIT_AUTO_SET_REQ.deckIndex = deckIndex;
				nKMPacket_DECK_UNIT_AUTO_SET_REQ.unitUIDList = unitUIDList;
				nKMPacket_DECK_UNIT_AUTO_SET_REQ.shipUID = shipUID;
				nKMPacket_DECK_UNIT_AUTO_SET_REQ.operatorUid = operatorUID;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DECK_UNIT_AUTO_SET_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_DECK_NAME_UPDATE_REQ(NKMDeckIndex deckIndex, string name)
		{
			name = name.Trim().Replace("\r", "").Replace("\n", "");
			NKMPacket_DECK_NAME_UPDATE_REQ nKMPacket_DECK_NAME_UPDATE_REQ = new NKMPacket_DECK_NAME_UPDATE_REQ();
			nKMPacket_DECK_NAME_UPDATE_REQ.deckIndex = deckIndex;
			nKMPacket_DECK_NAME_UPDATE_REQ.name = name;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DECK_NAME_UPDATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_POST_LIST_REQ(long lastindex)
		{
			NKMPacket_POST_LIST_REQ nKMPacket_POST_LIST_REQ = new NKMPacket_POST_LIST_REQ();
			nKMPacket_POST_LIST_REQ.lastPostIndex = lastindex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_POST_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_POST_RECEIVE_REQ(long index)
		{
			NKMPacket_POST_RECEIVE_REQ nKMPacket_POST_RECEIVE_REQ = new NKMPacket_POST_RECEIVE_REQ();
			nKMPacket_POST_RECEIVE_REQ.postIndex = index;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_POST_RECEIVE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_REMOVE_UNIT_REQ(List<long> lstTargetUnitUID)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMArmyData armyData = myUserData.m_ArmyData;
			foreach (long item in lstTargetUnitUID)
			{
				NKMUnitData unitOrTrophyFromUID = armyData.GetUnitOrTrophyFromUID(item);
				if (unitOrTrophyFromUID == null)
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_NO_EXIST_UNIT);
					return;
				}
				NKM_ERROR_CODE canDeleteUnit = NKMUnitManager.GetCanDeleteUnit(unitOrTrophyFromUID, myUserData);
				switch (canDeleteUnit)
				{
				case NKM_ERROR_CODE.NEC_FAIL_UNIT_LOCKED:
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL_LOCKED);
					return;
				case NKM_ERROR_CODE.NEC_FAIL_UNIT_IN_DECK:
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL_IN_DECK);
					return;
				case NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_LOBBYUNIT:
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL_MAINUNIT);
					return;
				case NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_WORLDMAP_LEADER:
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL_WORLDMAP_LEADER);
					return;
				default:
					NKCPopupOKCancel.OpenOKBox(canDeleteUnit);
					return;
				case NKM_ERROR_CODE.NEC_OK:
					break;
				}
			}
			armyData.InitUnitDelete();
			armyData.SetUnitDeleteList(lstTargetUnitUID);
			Send_NKMPacket_REMOVE_UNIT_REQ();
		}

		public static void Send_NKMPacket_REMOVE_UNIT_REQ()
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			if (!armyData.IsEmptyUnitDeleteList)
			{
				List<long> unitDeleteList = armyData.GetUnitDeleteList();
				NKMPacket_REMOVE_UNIT_REQ nKMPacket_REMOVE_UNIT_REQ = new NKMPacket_REMOVE_UNIT_REQ();
				nKMPacket_REMOVE_UNIT_REQ.removeUnitUIDList = unitDeleteList;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_REMOVE_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_RANDOM_ITEM_BOX_OPEN_REQ(int id, int count)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData.m_InventoryData.GetItemMisc(id) == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_ID);
				return;
			}
			if (myUserData.m_InventoryData.GetCountMiscItem(id) < count)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM);
				return;
			}
			NKMPacket_RANDOM_ITEM_BOX_OPEN_REQ nKMPacket_RANDOM_ITEM_BOX_OPEN_REQ = new NKMPacket_RANDOM_ITEM_BOX_OPEN_REQ();
			nKMPacket_RANDOM_ITEM_BOX_OPEN_REQ.itemID = id;
			nKMPacket_RANDOM_ITEM_BOX_OPEN_REQ.count = count;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_RANDOM_ITEM_BOX_OPEN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_CHOICE_ITEM_USE_REQ(int itemID, int rewardID, int count, int setOptionID = 0, int subSkillID = 0, List<NKM_STAT_TYPE> lstStatTypes = null, int potentialOptionId = 0, int potentialOptionId2 = 0)
		{
			if (NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemMisc(itemID) == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_ID);
				return;
			}
			NKMPacket_CHOICE_ITEM_USE_REQ nKMPacket_CHOICE_ITEM_USE_REQ = new NKMPacket_CHOICE_ITEM_USE_REQ();
			nKMPacket_CHOICE_ITEM_USE_REQ.itemId = itemID;
			nKMPacket_CHOICE_ITEM_USE_REQ.rewardId = rewardID;
			nKMPacket_CHOICE_ITEM_USE_REQ.count = count;
			nKMPacket_CHOICE_ITEM_USE_REQ.setOptionId = setOptionID;
			nKMPacket_CHOICE_ITEM_USE_REQ.subSkillId = subSkillID;
			if (lstStatTypes == null)
			{
				lstStatTypes = new List<NKM_STAT_TYPE>();
				lstStatTypes.Add(NKM_STAT_TYPE.NST_RANDOM);
				lstStatTypes.Add(NKM_STAT_TYPE.NST_RANDOM);
			}
			nKMPacket_CHOICE_ITEM_USE_REQ.statTypes = lstStatTypes;
			nKMPacket_CHOICE_ITEM_USE_REQ.potentialOptionId = potentialOptionId;
			nKMPacket_CHOICE_ITEM_USE_REQ.potentialOption2Id = potentialOptionId2;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_CHOICE_ITEM_USE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MISC_CONTRACT_OPEN_REQ(int itemID, int count)
		{
			NKMPacket_MISC_CONTRACT_OPEN_REQ nKMPacket_MISC_CONTRACT_OPEN_REQ = new NKMPacket_MISC_CONTRACT_OPEN_REQ();
			nKMPacket_MISC_CONTRACT_OPEN_REQ.miscItemId = itemID;
			nKMPacket_MISC_CONTRACT_OPEN_REQ.count = count;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MISC_CONTRACT_OPEN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SET_UNIT_SKIN_REQ(long unitUID, int skinID)
		{
			NKMPacket_SET_UNIT_SKIN_REQ nKMPacket_SET_UNIT_SKIN_REQ = new NKMPacket_SET_UNIT_SKIN_REQ();
			nKMPacket_SET_UNIT_SKIN_REQ.unitUID = unitUID;
			nKMPacket_SET_UNIT_SKIN_REQ.skinID = skinID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SET_UNIT_SKIN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ(int unitID, int pageNum = 1, bool bOrderByVotedCount = false)
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ nKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ = new NKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ();
				nKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ.unitID = unitID;
				nKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ.isOrderByVotedCount = bOrderByVotedCount;
				nKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ.pageNumber = pageNum;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ(int unitID, int pageNum = 1, bool bOrderByVotedCount = true)
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ nKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ = new NKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ();
				nKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ.unitID = unitID;
				nKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ.isOrderByVotedCount = bOrderByVotedCount;
				nKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ.pageNumber = pageNum;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ(int unitID, string content, bool bRewrite)
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ nKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ = new NKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ();
				nKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ.unitID = unitID;
				nKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ.content = content;
				nKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ.isRewrite = bRewrite;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_UNIT_REVIEW_COMMENT_DELETE_REQ(int unitID)
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPacket_UNIT_REVIEW_COMMENT_DELETE_REQ nKMPacket_UNIT_REVIEW_COMMENT_DELETE_REQ = new NKMPacket_UNIT_REVIEW_COMMENT_DELETE_REQ();
				nKMPacket_UNIT_REVIEW_COMMENT_DELETE_REQ.unitID = unitID;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_COMMENT_DELETE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_UNIT_REVIEW_COMMENT_VOTE_REQ(int unitID, long commentUID)
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPacket_UNIT_REVIEW_COMMENT_VOTE_REQ nKMPacket_UNIT_REVIEW_COMMENT_VOTE_REQ = new NKMPacket_UNIT_REVIEW_COMMENT_VOTE_REQ();
				nKMPacket_UNIT_REVIEW_COMMENT_VOTE_REQ.unitID = unitID;
				nKMPacket_UNIT_REVIEW_COMMENT_VOTE_REQ.commentUID = commentUID;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_COMMENT_VOTE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_UNIT_REVIEW_COMMENT_VOTE_CANCEL_REQ(int unitID, long commentUID)
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPacket_UNIT_REVIEW_COMMENT_VOTE_CANCEL_REQ nKMPacket_UNIT_REVIEW_COMMENT_VOTE_CANCEL_REQ = new NKMPacket_UNIT_REVIEW_COMMENT_VOTE_CANCEL_REQ();
				nKMPacket_UNIT_REVIEW_COMMENT_VOTE_CANCEL_REQ.unitID = unitID;
				nKMPacket_UNIT_REVIEW_COMMENT_VOTE_CANCEL_REQ.commentUID = commentUID;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_COMMENT_VOTE_CANCEL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_UNIT_REVIEW_SCORE_VOTE_REQ(int unitID, int score)
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPacket_UNIT_REVIEW_SCORE_VOTE_REQ nKMPacket_UNIT_REVIEW_SCORE_VOTE_REQ = new NKMPacket_UNIT_REVIEW_SCORE_VOTE_REQ();
				nKMPacket_UNIT_REVIEW_SCORE_VOTE_REQ.unitID = unitID;
				nKMPacket_UNIT_REVIEW_SCORE_VOTE_REQ.score = score;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_SCORE_VOTE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_UNIT_REVIEW_USER_BAN_REQ(long userUid)
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPacket_UNIT_REVIEW_USER_BAN_REQ nKMPacket_UNIT_REVIEW_USER_BAN_REQ = new NKMPacket_UNIT_REVIEW_USER_BAN_REQ();
				nKMPacket_UNIT_REVIEW_USER_BAN_REQ.targetUserUid = userUid;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_USER_BAN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_UNIT_REVIEW_USER_BAN_CANCEL_REQ(long userUid)
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPacket_UNIT_REVIEW_USER_BAN_CANCEL_REQ nKMPacket_UNIT_REVIEW_USER_BAN_CANCEL_REQ = new NKMPacket_UNIT_REVIEW_USER_BAN_CANCEL_REQ();
				nKMPacket_UNIT_REVIEW_USER_BAN_CANCEL_REQ.targetUserUid = userUid;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_USER_BAN_CANCEL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_UNIT_REVIEW_USER_BAN_LIST_REQ()
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPacket_UNIT_REVIEW_USER_BAN_LIST_REQ packet = new NKMPacket_UNIT_REVIEW_USER_BAN_LIST_REQ();
				NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_Packet_NKMPacket_LIMIT_BREAK_UNIT_REQ(long targetUnitUID)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(targetUnitUID);
			List<NKMItemMiscData> lstCost;
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMUnitLimitBreakManager.CanLimitBreak(myUserData, unitFromUID, out lstCost);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK && nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_FAIL_LIMITBREAK_ALREADY_MAX_LEVEL)
			{
				Debug.LogError("이미 최대치까지 초월한 유닛을 초월 시도");
				return;
			}
			NKMPacket_LIMIT_BREAK_UNIT_REQ nKMPacket_LIMIT_BREAK_UNIT_REQ = new NKMPacket_LIMIT_BREAK_UNIT_REQ();
			nKMPacket_LIMIT_BREAK_UNIT_REQ.unitUID = targetUnitUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LIMIT_BREAK_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_Packet_NKMPacket_UNIT_SKILL_UPGRADE_REQ(long targetUnitUID, int skillID)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(targetUnitUID);
			if (unitFromUID == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_UNIT);
				return;
			}
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMUnitSkillManager.CanTrainSkill(myUserData, unitFromUID, skillID);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
				return;
			}
			NKMPacket_UNIT_SKILL_UPGRADE_REQ nKMPacket_UNIT_SKILL_UPGRADE_REQ = new NKMPacket_UNIT_SKILL_UPGRADE_REQ();
			nKMPacket_UNIT_SKILL_UPGRADE_REQ.unitUID = targetUnitUID;
			nKMPacket_UNIT_SKILL_UPGRADE_REQ.skillID = skillID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_SKILL_UPGRADE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHIP_BUILD_REQ(int shipID)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMShipManager.CanShipBuild(NKCScenManager.GetScenManager().GetMyUserData(), shipID);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_SHIP_BUILD_FAIL, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
				return;
			}
			NKMPacket_SHIP_BUILD_REQ nKMPacket_SHIP_BUILD_REQ = new NKMPacket_SHIP_BUILD_REQ();
			nKMPacket_SHIP_BUILD_REQ.shipID = shipID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHIP_BUILD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHIP_LEVELUP_REQ(long uid, int targetLv = 0)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(uid);
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMShipManager.CanShipLevelup(myUserData, shipFromUID, targetLv);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_SHIP_LEVEL_UP_FAIL, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
				return;
			}
			NKMPacket_SHIP_LEVELUP_REQ nKMPacket_SHIP_LEVELUP_REQ = new NKMPacket_SHIP_LEVELUP_REQ();
			nKMPacket_SHIP_LEVELUP_REQ.shipUID = uid;
			nKMPacket_SHIP_LEVELUP_REQ.nextLevel = targetLv;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHIP_LEVELUP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHIP_DIVISION_REQ(List<long> lstShipUID)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			foreach (long item in lstShipUID)
			{
				NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(item);
				NKM_ERROR_CODE nKM_ERROR_CODE = NKMShipManager.CanShipDivision(myUserData, shipFromUID);
				if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_SHIP_DIVISION_FAIL, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
					return;
				}
			}
			NKMPacket_SHIP_DIVISION_REQ nKMPacket_SHIP_DIVISION_REQ = new NKMPacket_SHIP_DIVISION_REQ();
			nKMPacket_SHIP_DIVISION_REQ.removeShipUIDList = lstShipUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHIP_DIVISION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHIP_UPGRADE_REQ(long ShipUID, int UpgradeShipID)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(ShipUID);
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMShipManager.CanShipUpgrade(myUserData, shipFromUID, UpgradeShipID);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_SHIP_UPGRADE_FAIL, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
				return;
			}
			NKMPacket_SHIP_UPGRADE_REQ nKMPacket_SHIP_UPGRADE_REQ = new NKMPacket_SHIP_UPGRADE_REQ();
			nKMPacket_SHIP_UPGRADE_REQ.shipUID = ShipUID;
			nKMPacket_SHIP_UPGRADE_REQ.nextShipID = UpgradeShipID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHIP_UPGRADE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_LIMIT_BREAK_SHIP_REQ(long shipUid, long consumeShipUid)
		{
			NKMPacket_LIMIT_BREAK_SHIP_REQ nKMPacket_LIMIT_BREAK_SHIP_REQ = new NKMPacket_LIMIT_BREAK_SHIP_REQ();
			nKMPacket_LIMIT_BREAK_SHIP_REQ.shipUid = shipUid;
			nKMPacket_LIMIT_BREAK_SHIP_REQ.consumeShipUid = consumeShipUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LIMIT_BREAK_SHIP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHIP_SLOT_FIRST_OPTION_REQ(long shipUid, int moduleId)
		{
			NKMPacket_SHIP_SLOT_FIRST_OPTION_REQ nKMPacket_SHIP_SLOT_FIRST_OPTION_REQ = new NKMPacket_SHIP_SLOT_FIRST_OPTION_REQ();
			nKMPacket_SHIP_SLOT_FIRST_OPTION_REQ.shipUid = shipUid;
			nKMPacket_SHIP_SLOT_FIRST_OPTION_REQ.moduleId = moduleId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHIP_SLOT_FIRST_OPTION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHIP_SLOT_LOCK_REQ(long shipUid, int moduleId, int slotId, bool locked)
		{
			NKMPacket_SHIP_SLOT_LOCK_REQ nKMPacket_SHIP_SLOT_LOCK_REQ = new NKMPacket_SHIP_SLOT_LOCK_REQ();
			nKMPacket_SHIP_SLOT_LOCK_REQ.shipUid = shipUid;
			nKMPacket_SHIP_SLOT_LOCK_REQ.moduleId = moduleId;
			nKMPacket_SHIP_SLOT_LOCK_REQ.slotId = slotId;
			nKMPacket_SHIP_SLOT_LOCK_REQ.locked = locked;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHIP_SLOT_LOCK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHIP_SLOT_OPTION_CHANGE_REQ(long shipUid, int moduleId)
		{
			NKMPacket_SHIP_SLOT_OPTION_CHANGE_REQ nKMPacket_SHIP_SLOT_OPTION_CHANGE_REQ = new NKMPacket_SHIP_SLOT_OPTION_CHANGE_REQ();
			nKMPacket_SHIP_SLOT_OPTION_CHANGE_REQ.shipUid = shipUid;
			nKMPacket_SHIP_SLOT_OPTION_CHANGE_REQ.moduleId = moduleId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHIP_SLOT_OPTION_CHANGE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_SHIP_SLOT_OPTION_CONFIRM_REQ(long shipUid, int moduleId)
		{
			NKMPacket_SHIP_SLOT_OPTION_CONFIRM_REQ nKMPacket_SHIP_SLOT_OPTION_CONFIRM_REQ = new NKMPacket_SHIP_SLOT_OPTION_CONFIRM_REQ();
			nKMPacket_SHIP_SLOT_OPTION_CONFIRM_REQ.shipUid = shipUid;
			nKMPacket_SHIP_SLOT_OPTION_CONFIRM_REQ.moduleId = moduleId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHIP_SLOT_OPTION_CONFIRM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ()
		{
			NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ packet = new NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static bool Send_NKMPacket_CONTRACT_REQ(int contractID, ContractCostType costType, int contractCnt)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			ContractTempletV2 contractTempletV = ContractTempletV2.Find(contractID);
			NKM_ERROR_CODE nKM_ERROR_CODE = NKCContractDataMgr.CanTryContract(myUserData, contractTempletV, costType, contractCnt);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				if (nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_RESOURCE)
				{
					MiscItemUnit[] singleTryRequireItems = contractTempletV.m_SingleTryRequireItems;
					for (int i = 0; i < singleTryRequireItems.Length; i++)
					{
						if (myUserData.m_InventoryData.GetCountMiscItem(singleTryRequireItems[i].ItemId) < singleTryRequireItems[i].Count * contractCnt && i != 0)
						{
							NKCShopManager.OpenItemLackPopup(singleTryRequireItems[i].ItemId, singleTryRequireItems[i].Count32);
							return false;
						}
					}
				}
				NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
				return false;
			}
			Debug.Log("Send_NKMPacket_CONTRACT_REQ : Contract ID " + contractID + " Cost Type : " + costType.ToString() + ", Contract Count : " + contractCnt);
			NKMPacket_CONTRACT_REQ packet = new NKMPacket_CONTRACT_REQ
			{
				contractId = contractID,
				count = contractCnt,
				costType = costType
			};
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			return true;
		}

		public static bool Send_NKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_REQ(int contractID)
		{
			if (SelectableContractTemplet.Find(contractID) != null)
			{
				Debug.Log("NKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_REQ : Contract ID " + contractID);
				NKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_REQ packet = new NKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_REQ
				{
					contractId = contractID
				};
				NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
				return true;
			}
			return false;
		}

		public static bool Send_NKMPacket_SELECTABLE_CONTRACT_CONFIRM_REQ(int contractID)
		{
			if (SelectableContractTemplet.Find(contractID) != null)
			{
				Debug.Log("NKMPacket_SELECTABLE_CONTRACT_CONFIRM_REQ : Contract ID " + contractID);
				NKMPacket_SELECTABLE_CONTRACT_CONFIRM_REQ packet = new NKMPacket_SELECTABLE_CONTRACT_CONFIRM_REQ
				{
					contractId = contractID
				};
				NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
				return true;
			}
			return false;
		}

		public static void Send_NKMPacket_CONTRACT_STATE_LIST_REQ()
		{
			NKMPacket_CONTRACT_STATE_LIST_REQ packet = new NKMPacket_CONTRACT_STATE_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void NKMPacket_INSTANT_CONTRACT_LIST_REQ()
		{
			NKMPacket_INSTANT_CONTRACT_LIST_REQ packet = new NKMPacket_INSTANT_CONTRACT_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_CUSTOM_PICKUP_REQ(int customPickUpID, ContractCostType costType, int count)
		{
			NKMPacket_CUSTOM_PICKUP_REQ nKMPacket_CUSTOM_PICKUP_REQ = new NKMPacket_CUSTOM_PICKUP_REQ();
			nKMPacket_CUSTOM_PICKUP_REQ.customPickupId = customPickUpID;
			nKMPacket_CUSTOM_PICKUP_REQ.count = count;
			nKMPacket_CUSTOM_PICKUP_REQ.costType = costType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_CUSTOM_PICKUP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_CUSTOM_PICUP_SELECT_TARGET_REQ(int customPickUpId, int targetUnitId)
		{
			NKMPacket_CUSTOM_PICUP_SELECT_TARGET_REQ nKMPacket_CUSTOM_PICUP_SELECT_TARGET_REQ = new NKMPacket_CUSTOM_PICUP_SELECT_TARGET_REQ();
			nKMPacket_CUSTOM_PICUP_SELECT_TARGET_REQ.customPickupId = customPickUpId;
			nKMPacket_CUSTOM_PICUP_SELECT_TARGET_REQ.targetUnitId = targetUnitId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_CUSTOM_PICUP_SELECT_TARGET_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static bool Send_NKMPacket_NEGOTIATE_REQ2(NKMUnitData unitData, NEGOTIATE_BOSS_SELECTION bossSelection, List<MiscItemData> materials)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = NKCNegotiateManager.CanStartNegotiate(NKCScenManager.CurrentUserData(), unitData, bossSelection, materials);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
				return false;
			}
			NKMPacket_NEGOTIATE_REQ nKMPacket_NEGOTIATE_REQ = new NKMPacket_NEGOTIATE_REQ();
			nKMPacket_NEGOTIATE_REQ.unitUid = unitData.m_UnitUID;
			nKMPacket_NEGOTIATE_REQ.negotiateBossSelection = bossSelection;
			nKMPacket_NEGOTIATE_REQ.materials = new List<MiscItemData>();
			for (int i = 0; i < materials.Count; i++)
			{
				if (materials[i].count > 0)
				{
					nKMPacket_NEGOTIATE_REQ.materials.Add(materials[i]);
				}
			}
			if (nKMPacket_NEGOTIATE_REQ.materials.Count == 0)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_NOT_ENOUGH_NEGOTIATE_MATERIALS);
				return false;
			}
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_NEGOTIATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
			return true;
		}

		public static void Send_NKMPacket_CONTRACT_PERMANENTLY_REQ(long unitUID)
		{
			NKMPacket_CONTRACT_PERMANENTLY_REQ nKMPacket_CONTRACT_PERMANENTLY_REQ = new NKMPacket_CONTRACT_PERMANENTLY_REQ();
			nKMPacket_CONTRACT_PERMANENTLY_REQ.unitUID = unitUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_CONTRACT_PERMANENTLY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EXCHANGE_PIECE_TO_UNIT_REQ(int itemID, int count)
		{
			NKMPacket_EXCHANGE_PIECE_TO_UNIT_REQ nKMPacket_EXCHANGE_PIECE_TO_UNIT_REQ = new NKMPacket_EXCHANGE_PIECE_TO_UNIT_REQ();
			nKMPacket_EXCHANGE_PIECE_TO_UNIT_REQ.itemId = itemID;
			nKMPacket_EXCHANGE_PIECE_TO_UNIT_REQ.count = count;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EXCHANGE_PIECE_TO_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_DIVE_SUICIDE_REQ(byte deckIndex)
		{
			NKMPacket_DIVE_SUICIDE_REQ nKMPacket_DIVE_SUICIDE_REQ = new NKMPacket_DIVE_SUICIDE_REQ();
			nKMPacket_DIVE_SUICIDE_REQ.selectDeckIndex = deckIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DIVE_SUICIDE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_DIVE_START_REQ(int cityID, int stageID, List<int> lstDiveDeckIndex, bool bJump)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = NKCDiveManager.CanStart(cityID, stageID, lstDiveDeckIndex, NKCScenManager.GetScenManager().GetMyUserData(), NKCSynchronizedTime.GetServerUTCTime(), bJump);
			switch (nKM_ERROR_CODE)
			{
			case NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM:
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_DIVE_NO_EXIST_COST);
				break;
			case NKM_ERROR_CODE.NEC_FAIL_DIVE_NOT_ENOUGH_SQUAD_COUNT:
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_DIVE_NO_ENOUGH_DECK);
				break;
			default:
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(nKM_ERROR_CODE.ToString()));
				break;
			case NKM_ERROR_CODE.NEC_OK:
			{
				NKMPacket_DIVE_START_REQ nKMPacket_DIVE_START_REQ = new NKMPacket_DIVE_START_REQ();
				nKMPacket_DIVE_START_REQ.stageID = stageID;
				nKMPacket_DIVE_START_REQ.cityID = cityID;
				nKMPacket_DIVE_START_REQ.deckIndexeList = lstDiveDeckIndex;
				nKMPacket_DIVE_START_REQ.isDiveStorm = bJump;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DIVE_START_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
				break;
			}
			}
		}

		public static void Send_NKMPacket_DIVE_SELECT_ARTIFACT_REQ(int artifactID)
		{
			NKMPacket_DIVE_SELECT_ARTIFACT_REQ nKMPacket_DIVE_SELECT_ARTIFACT_REQ = new NKMPacket_DIVE_SELECT_ARTIFACT_REQ();
			nKMPacket_DIVE_SELECT_ARTIFACT_REQ.artifactID = artifactID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DIVE_SELECT_ARTIFACT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_DIVE_SKIP_REQ(int stageId, int skipCount, int cityID = 0)
		{
			NKMPacket_DIVE_SKIP_REQ nKMPacket_DIVE_SKIP_REQ = new NKMPacket_DIVE_SKIP_REQ();
			nKMPacket_DIVE_SKIP_REQ.stageId = stageId;
			nKMPacket_DIVE_SKIP_REQ.skipCount = skipCount;
			nKMPacket_DIVE_SKIP_REQ.cityId = cityID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DIVE_SKIP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_RESULT_LIST_REQ()
		{
			NKMPacket_RAID_RESULT_LIST_REQ packet = new NKMPacket_RAID_RESULT_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_COOP_LIST_REQ()
		{
			NKMPacket_RAID_COOP_LIST_REQ packet = new NKMPacket_RAID_COOP_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_SET_COOP_REQ(long raidUID)
		{
			NKMPacket_RAID_SET_COOP_REQ nKMPacket_RAID_SET_COOP_REQ = new NKMPacket_RAID_SET_COOP_REQ();
			nKMPacket_RAID_SET_COOP_REQ.raidUID = raidUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_RAID_SET_COOP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_SET_COOP_ALL_REQ()
		{
			NKMPacket_RAID_SET_COOP_ALL_REQ packet = new NKMPacket_RAID_SET_COOP_ALL_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_RESULT_ACCEPT_REQ(long raidUID)
		{
			NKMPacket_RAID_RESULT_ACCEPT_REQ nKMPacket_RAID_RESULT_ACCEPT_REQ = new NKMPacket_RAID_RESULT_ACCEPT_REQ();
			nKMPacket_RAID_RESULT_ACCEPT_REQ.raidUID = raidUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_RAID_RESULT_ACCEPT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_RESULT_ACCEPT_ALL_REQ()
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(new NKMPacket_RAID_RESULT_ACCEPT_ALL_REQ(), NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MY_RAID_LIST_REQ()
		{
			NKMPacket_MY_RAID_LIST_REQ packet = new NKMPacket_MY_RAID_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_DETAIL_INFO_REQ(long raidUID)
		{
			NKMPacket_RAID_DETAIL_INFO_REQ nKMPacket_RAID_DETAIL_INFO_REQ = new NKMPacket_RAID_DETAIL_INFO_REQ();
			nKMPacket_RAID_DETAIL_INFO_REQ.raidUID = raidUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_RAID_DETAIL_INFO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_GAME_LOAD_REQ(long raidUID, byte selectDeckIndex, List<int> _Buffs, bool isTryAssist)
		{
			NKMPacket_RAID_GAME_LOAD_REQ nKMPacket_RAID_GAME_LOAD_REQ = new NKMPacket_RAID_GAME_LOAD_REQ();
			nKMPacket_RAID_GAME_LOAD_REQ.raidUID = raidUID;
			nKMPacket_RAID_GAME_LOAD_REQ.selectDeckIndex = selectDeckIndex;
			nKMPacket_RAID_GAME_LOAD_REQ.buffList = _Buffs;
			nKMPacket_RAID_GAME_LOAD_REQ.isTryAssist = isTryAssist;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_RAID_GAME_LOAD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_POINT_REWARD_REQ(int raidPointReward)
		{
			NKMPacket_RAID_POINT_REWARD_REQ nKMPacket_RAID_POINT_REWARD_REQ = new NKMPacket_RAID_POINT_REWARD_REQ();
			nKMPacket_RAID_POINT_REWARD_REQ.raidPointReward = raidPointReward;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_RAID_POINT_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_SWEEP_REQ(long raidUid, bool isTryAssist)
		{
			NKMPacket_RAID_SWEEP_REQ nKMPacket_RAID_SWEEP_REQ = new NKMPacket_RAID_SWEEP_REQ();
			nKMPacket_RAID_SWEEP_REQ.raidUid = raidUid;
			nKMPacket_RAID_SWEEP_REQ.isTryAssist = isTryAssist;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_RAID_SWEEP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RAID_POINT_EXTRA_REWARD_REQ()
		{
			NKMPacket_RAID_POINT_EXTRA_REWARD_REQ packet = new NKMPacket_RAID_POINT_EXTRA_REWARD_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_REQ(int cityID)
		{
			NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_REQ nKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_REQ = new NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_REQ();
			nKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_REQ.cityID = cityID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_WORLDMAP_EVENT_CANCEL_REQ(int cityID)
		{
			NKMPacket_WORLDMAP_EVENT_CANCEL_REQ nKMPacket_WORLDMAP_EVENT_CANCEL_REQ = new NKMPacket_WORLDMAP_EVENT_CANCEL_REQ();
			nKMPacket_WORLDMAP_EVENT_CANCEL_REQ.cityID = cityID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_EVENT_CANCEL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MY_USER_PROFILE_INFO_REQ()
		{
			NKMPacket_MY_USER_PROFILE_INFO_REQ packet = new NKMPacket_MY_USER_PROFILE_INFO_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_USER_PROFILE_INFO_REQ(long userUID, NKM_DECK_TYPE requestDeckType)
		{
			NKMPacket_USER_PROFILE_INFO_REQ nKMPacket_USER_PROFILE_INFO_REQ = new NKMPacket_USER_PROFILE_INFO_REQ();
			nKMPacket_USER_PROFILE_INFO_REQ.userUID = userUID;
			nKMPacket_USER_PROFILE_INFO_REQ.deckType = requestDeckType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_USER_PROFILE_INFO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_UPDATE_DEFENCE_DECK_REQ(NKMDeckData deckData)
		{
			NKMPacket_UPDATE_DEFENCE_DECK_REQ nKMPacket_UPDATE_DEFENCE_DECK_REQ = new NKMPacket_UPDATE_DEFENCE_DECK_REQ();
			nKMPacket_UPDATE_DEFENCE_DECK_REQ.deckData = deckData;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UPDATE_DEFENCE_DECK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_ASYNC_PVP_START_GAME_REQ(long friendCode, byte deckIndex, NKM_GAME_TYPE gameType)
		{
			NKMPacket_ASYNC_PVP_START_GAME_REQ nKMPacket_ASYNC_PVP_START_GAME_REQ = new NKMPacket_ASYNC_PVP_START_GAME_REQ();
			nKMPacket_ASYNC_PVP_START_GAME_REQ.targetFriendCode = friendCode;
			nKMPacket_ASYNC_PVP_START_GAME_REQ.selectDeckIndex = deckIndex;
			nKMPacket_ASYNC_PVP_START_GAME_REQ.gameType = gameType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_ASYNC_PVP_START_GAME_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_LEAGUE_PVP_WEEKLY_RANKER_REQ()
		{
			NKMPacket_LEAGUE_PVP_WEEKLY_RANKER_REQ packet = new NKMPacket_LEAGUE_PVP_WEEKLY_RANKER_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_LEAGUE_PVP_RANK_LIST_REQ(RANK_TYPE rankType, LeaderBoardRangeType range)
		{
			NKMPacket_LEAGUE_PVP_RANK_LIST_REQ nKMPacket_LEAGUE_PVP_RANK_LIST_REQ = new NKMPacket_LEAGUE_PVP_RANK_LIST_REQ();
			nKMPacket_LEAGUE_PVP_RANK_LIST_REQ.rankType = rankType;
			nKMPacket_LEAGUE_PVP_RANK_LIST_REQ.range = range;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LEAGUE_PVP_RANK_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_LEAGUE_PVP_SEASON_INFO_REQ()
		{
			NKMPacket_LEAGUE_PVP_SEASON_INFO_REQ packet = new NKMPacket_LEAGUE_PVP_SEASON_INFO_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_LEAGUE_PVP_SEASON_REWARD_REQ()
		{
			NKMPacket_LEAGUE_PVP_SEASON_REWARD_REQ packet = new NKMPacket_LEAGUE_PVP_SEASON_REWARD_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_PVP_GAME_MATCH_REQ(int seasonId, NKMEventDeckData eventDeckData, NKM_GAME_TYPE gameType)
		{
			NKMPacket_EVENT_PVP_GAME_MATCH_REQ nKMPacket_EVENT_PVP_GAME_MATCH_REQ = new NKMPacket_EVENT_PVP_GAME_MATCH_REQ();
			nKMPacket_EVENT_PVP_GAME_MATCH_REQ.seasonId = seasonId;
			nKMPacket_EVENT_PVP_GAME_MATCH_REQ.eventDeckData = eventDeckData;
			nKMPacket_EVENT_PVP_GAME_MATCH_REQ.gameType = gameType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_PVP_GAME_MATCH_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_EVENT_PVP_GAME_MATCH_CANCEL_REQ()
		{
			NKMPacket_EVENT_PVP_GAME_MATCH_CANCEL_REQ packet = new NKMPacket_EVENT_PVP_GAME_MATCH_CANCEL_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_PVP_SEASON_INFO_REQ(int seasonId)
		{
			NKMPacket_EVENT_PVP_SEASON_INFO_REQ nKMPacket_EVENT_PVP_SEASON_INFO_REQ = new NKMPacket_EVENT_PVP_SEASON_INFO_REQ();
			nKMPacket_EVENT_PVP_SEASON_INFO_REQ.seasonId = seasonId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_PVP_SEASON_INFO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_PVP_REWARD_REQ(int seasonId)
		{
			NKMPacket_EVENT_PVP_REWARD_REQ nKMPacket_EVENT_PVP_REWARD_REQ = new NKMPacket_EVENT_PVP_REWARD_REQ();
			nKMPacket_EVENT_PVP_REWARD_REQ.seasonId = seasonId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_PVP_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GAME_SURRENDER_REQ()
		{
			NKMPacket_GAME_SURRENDER_REQ packet = new NKMPacket_GAME_SURRENDER_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void NKMPacket_PVP_PICK_RATE_REQ(NKM_GAME_TYPE gameType)
		{
			NKMPacket_PVP_PICK_RATE_REQ nKMPacket_PVP_PICK_RATE_REQ = new NKMPacket_PVP_PICK_RATE_REQ();
			nKMPacket_PVP_PICK_RATE_REQ.gameType = gameType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PVP_PICK_RATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_PVP_ROOM_ACCEPT_CODE_REQ(string code)
		{
			NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_REQ nKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_REQ();
			nKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_REQ.code = code;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_PVP_ROOM_KICK_REQ(long targetUserUid)
		{
			NKMPacket_PRIVATE_PVP_LOBBY_KICK_REQ nKMPacket_PRIVATE_PVP_LOBBY_KICK_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_KICK_REQ();
			nKMPacket_PRIVATE_PVP_LOBBY_KICK_REQ.targetUserUid = targetUserUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_KICK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_PVP_ROOM_CHANGE_OPTION_REQ(NKMPrivateGameConfig config)
		{
			NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_OPTION_REQ nKMPacket_PRIVATE_PVP_LOBBY_CHANGE_OPTION_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_OPTION_REQ();
			nKMPacket_PRIVATE_PVP_LOBBY_CHANGE_OPTION_REQ.newConfig = config;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_CHANGE_OPTION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_PRIVATE_PVP_START_REQ()
		{
			NKMPacket_PRIVATE_PVP_LOBBY_START_GAME_SETTING_REQ packet = new NKMPacket_PRIVATE_PVP_LOBBY_START_GAME_SETTING_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_PRIVATE_PVP_SEARCH_USER_REQ(string searchKeyworkd)
		{
			NKMPacket_PRIVATE_PVP_LOBBY_SEARCH_USER_REQ nKMPacket_PRIVATE_PVP_LOBBY_SEARCH_USER_REQ = new NKMPacket_PRIVATE_PVP_LOBBY_SEARCH_USER_REQ();
			nKMPacket_PRIVATE_PVP_LOBBY_SEARCH_USER_REQ.searchKeyword = searchKeyworkd;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_LOBBY_SEARCH_USER_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_PRIVATE_PVP_DRAFT_GIVEUP_REQ()
		{
			NKMPacket_PRIVATE_PVP_DRAFT_GIVEUP_REQ packet = new NKMPacket_PRIVATE_PVP_DRAFT_GIVEUP_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_PRIVATE_PVP_STATE_REQ(LobbyPlayerState changeState)
		{
			NKMPacket_PRIVATE_PVP_STATE_REQ nKMPacket_PRIVATE_PVP_STATE_REQ = new NKMPacket_PRIVATE_PVP_STATE_REQ();
			nKMPacket_PRIVATE_PVP_STATE_REQ.changeState = changeState;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_PVP_STATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_FRIEND_LIST_REQ(NKM_FRIEND_LIST_TYPE listType)
		{
			NKMPacket_FRIEND_LIST_REQ nKMPacket_FRIEND_LIST_REQ = new NKMPacket_FRIEND_LIST_REQ();
			nKMPacket_FRIEND_LIST_REQ.friendListType = listType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_FRIEND_REQUEST_REQ(long friendCode)
		{
			NKMPacket_FRIEND_REQUEST_REQ nKMPacket_FRIEND_REQUEST_REQ = new NKMPacket_FRIEND_REQUEST_REQ();
			nKMPacket_FRIEND_REQUEST_REQ.friendCode = friendCode;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_REQUEST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ(long friendCode)
		{
			NKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ nKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ = new NKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ();
			nKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ.friendCode = friendCode;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_PVP_CHARGE_POINT_REFRESH_REQ(int itemID)
		{
			NKMPacket_PVP_CHARGE_POINT_REFRESH_REQ nKMPacket_PVP_CHARGE_POINT_REFRESH_REQ = new NKMPacket_PVP_CHARGE_POINT_REFRESH_REQ();
			nKMPacket_PVP_CHARGE_POINT_REFRESH_REQ.itemId = itemID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PVP_CHARGE_POINT_REFRESH_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_SET_EMBLEM_REQ(int index, int itemID)
		{
			NKMPacket_SET_EMBLEM_REQ nKMPacket_SET_EMBLEM_REQ = new NKMPacket_SET_EMBLEM_REQ();
			nKMPacket_SET_EMBLEM_REQ.index = (sbyte)index;
			nKMPacket_SET_EMBLEM_REQ.itemId = itemID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SET_EMBLEM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_USER_PROFILE_CHANGE_FRAME_REQ(int frameID)
		{
			NKMPacket_USER_PROFILE_CHANGE_FRAME_REQ nKMPacket_USER_PROFILE_CHANGE_FRAME_REQ = new NKMPacket_USER_PROFILE_CHANGE_FRAME_REQ();
			nKMPacket_USER_PROFILE_CHANGE_FRAME_REQ.selfiFrameId = frameID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_USER_PROFILE_CHANGE_FRAME_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ(int unitId, int skinId)
		{
			NKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ nKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ = new NKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ();
			nKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ.mainCharId = unitId;
			nKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ.mainCharSkinId = skinId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GAME_EMOTICON_REQ(int emoticonID)
		{
			NKMPacket_GAME_EMOTICON_REQ nKMPacket_GAME_EMOTICON_REQ = new NKMPacket_GAME_EMOTICON_REQ();
			nKMPacket_GAME_EMOTICON_REQ.emoticonID = emoticonID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_EMOTICON_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_EMOTICON_DATA_REQ(NKC_OPEN_WAIT_BOX_TYPE eType = NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL)
		{
			NKMPacket_EMOTICON_DATA_REQ packet = new NKMPacket_EMOTICON_DATA_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, eType);
		}

		public static void Send_NKMPacket_EMOTICON_ANI_CHANGE_REQ(int presetIndex, int emoticonID)
		{
			NKMPacket_EMOTICON_ANI_CHANGE_REQ nKMPacket_EMOTICON_ANI_CHANGE_REQ = new NKMPacket_EMOTICON_ANI_CHANGE_REQ();
			nKMPacket_EMOTICON_ANI_CHANGE_REQ.presetIndex = presetIndex;
			nKMPacket_EMOTICON_ANI_CHANGE_REQ.emoticonId = emoticonID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EMOTICON_ANI_CHANGE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EMOTICON_TEXT_CHANGE_REQ(int presetIndex, int emoticonID)
		{
			NKMPacket_EMOTICON_TEXT_CHANGE_REQ nKMPacket_EMOTICON_TEXT_CHANGE_REQ = new NKMPacket_EMOTICON_TEXT_CHANGE_REQ();
			nKMPacket_EMOTICON_TEXT_CHANGE_REQ.presetIndex = presetIndex;
			nKMPacket_EMOTICON_TEXT_CHANGE_REQ.emoticonId = emoticonID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EMOTICON_TEXT_CHANGE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EMOTICON_FAVORITES_SET_REQ(int emoticonId, bool favoritesOption)
		{
			NKMPacket_EMOTICON_FAVORITES_SET_REQ nKMPacket_EMOTICON_FAVORITES_SET_REQ = new NKMPacket_EMOTICON_FAVORITES_SET_REQ();
			nKMPacket_EMOTICON_FAVORITES_SET_REQ.emoticonId = emoticonId;
			nKMPacket_EMOTICON_FAVORITES_SET_REQ.favoritesOption = favoritesOption;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EMOTICON_FAVORITES_SET_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHOP_FIXED_LIST_REQ(NKC_OPEN_WAIT_BOX_TYPE waitboxType)
		{
			NKMPacket_SHOP_FIXED_LIST_REQ packet = new NKMPacket_SHOP_FIXED_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, waitboxType);
		}

		public static void Send_NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ(string productMarketID, List<int> lstSelection = null)
		{
			NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ nKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ = new NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ();
			nKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ.productMarketID = productMarketID;
			nKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ.selectIndices = ((lstSelection != null) ? lstSelection : new List<int>());
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_SHOP_FIX_SHOP_BUY_REQ(int productID, int productCount, List<int> lstSelection = null)
		{
			ShopItemTemplet shop_templet = ShopItemTemplet.Find(productID);
			bool is_init;
			long next_reset_date;
			NKM_ERROR_CODE nKM_ERROR_CODE = NKCShopManager.CanBuyFixShop(NKCScenManager.CurrentUserData(), shop_templet, out is_init, out next_reset_date);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
				return;
			}
			NKMPacket_SHOP_FIX_SHOP_BUY_REQ nKMPacket_SHOP_FIX_SHOP_BUY_REQ = new NKMPacket_SHOP_FIX_SHOP_BUY_REQ();
			nKMPacket_SHOP_FIX_SHOP_BUY_REQ.productID = productID;
			nKMPacket_SHOP_FIX_SHOP_BUY_REQ.productCount = productCount;
			nKMPacket_SHOP_FIX_SHOP_BUY_REQ.selectIndices = ((lstSelection != null) ? lstSelection : new List<int>());
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHOP_FIX_SHOP_BUY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ(string productMarketID, string validationToken, double realCash, string currencyCode, List<int> lstSelection = null)
		{
			Debug.Log("[InappPurchase] NKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ");
			NKCCurrencyTemplet nKCCurrencyTemplet = NKMTempletContainer<NKCCurrencyTemplet>.Find(currencyCode);
			if (nKCCurrencyTemplet == null)
			{
				Debug.Log("[InappPurchase] CurrentyTemplet is null");
			}
			NKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ nKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ = new NKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ();
			nKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ.productMarketID = productMarketID;
			nKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ.validationToken = validationToken;
			nKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ.currencyCode = currencyCode;
			nKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ.currencyType = nKCCurrencyTemplet?.m_Type ?? 0;
			nKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ.realCash = realCash;
			nKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ.selectIndices = ((lstSelection != null) ? lstSelection : new List<int>());
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ(string productMarketID)
		{
			Send_NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ(productMarketID);
		}

		public static void Send_NKMPacket_GAMEBASE_BUY_REQ(string paymentSeq, string accessToken, string paymentId, List<int> selectIndices)
		{
			NKMPacket_GAMEBASE_BUY_REQ packet = new NKMPacket_GAMEBASE_BUY_REQ
			{
				paymentSeq = paymentSeq,
				accessToken = accessToken,
				selectIndices = selectIndices,
				paymentId = paymentId
			};
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_REQ(List<int> lstIndex)
		{
			NKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_REQ nKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_REQ = new NKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_REQ();
			nKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_REQ.slotIndexes = lstIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RANDOM_MISSION_CHANGE_REQ(int tabId, int missionId)
		{
			NKMPacket_RANDOM_MISSION_CHANGE_REQ nKMPacket_RANDOM_MISSION_CHANGE_REQ = new NKMPacket_RANDOM_MISSION_CHANGE_REQ();
			nKMPacket_RANDOM_MISSION_CHANGE_REQ.tabId = tabId;
			nKMPacket_RANDOM_MISSION_CHANGE_REQ.missionId = missionId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_RANDOM_MISSION_CHANGE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MISSION_GIVE_ITEM_REQ(int missionID, int itemCount)
		{
			NKMPacket_MISSION_GIVE_ITEM_REQ nKMPacket_MISSION_GIVE_ITEM_REQ = new NKMPacket_MISSION_GIVE_ITEM_REQ();
			nKMPacket_MISSION_GIVE_ITEM_REQ.missionId = missionID;
			nKMPacket_MISSION_GIVE_ITEM_REQ.count = itemCount;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MISSION_GIVE_ITEM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MISSION_COMPLETE_REQ(NKMMissionTemplet missionTemplet)
		{
			if (missionTemplet == null)
			{
				Log.Error("[Mission] Send Mission Complete Req  MissionTemplet is Null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketSender.cs", 1589);
				return;
			}
			if (missionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.ON_COMPLETE)
			{
				Send_NKMPacket_MISSION_GET_COMPLETE_REWARD_REQ(missionTemplet);
				return;
			}
			NKMPacket_MISSION_COMPLETE_REQ nKMPacket_MISSION_COMPLETE_REQ = new NKMPacket_MISSION_COMPLETE_REQ();
			nKMPacket_MISSION_COMPLETE_REQ.tabId = missionTemplet.m_MissionTabId;
			nKMPacket_MISSION_COMPLETE_REQ.groupId = missionTemplet.m_GroupId;
			nKMPacket_MISSION_COMPLETE_REQ.missionID = missionTemplet.m_MissionID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MISSION_COMPLETE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MISSION_GET_COMPLETE_REWARD_REQ(NKMMissionTemplet missionTemplet)
		{
			if (missionTemplet == null)
			{
				Log.Error("[Mission] Send Mission Complete Req  MissionTemplet is Null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketSender.cs", 1613);
				return;
			}
			NKMPacket_MISSION_GET_COMPLETE_REWARD_REQ nKMPacket_MISSION_GET_COMPLETE_REWARD_REQ = new NKMPacket_MISSION_GET_COMPLETE_REWARD_REQ();
			nKMPacket_MISSION_GET_COMPLETE_REWARD_REQ.missionID = missionTemplet.m_MissionID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MISSION_GET_COMPLETE_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(int missionTabID)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				NKMUserMissionData missionData = NKCScenManager.GetScenManager().GetMyUserData().m_MissionData;
				if (missionData != null && missionData.CheckCompletableMission(myUserData, missionTabID))
				{
					NKMPacket_MISSION_COMPLETE_ALL_REQ nKMPacket_MISSION_COMPLETE_ALL_REQ = new NKMPacket_MISSION_COMPLETE_ALL_REQ();
					nKMPacket_MISSION_COMPLETE_ALL_REQ.tabId = missionTabID;
					NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MISSION_COMPLETE_ALL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
				}
			}
		}

		public static void Send_NKMPacket_GAME_OPTION_CHANGE_REQ(ActionCameraType actionCameraType, bool bTrackCamera, bool bViewSkillCutIn, NKM_GAME_AUTO_RESPAWN_TYPE ePvpAutoRespawn, bool bAutoSyncFriendDeck, bool bLocal = false)
		{
			NKMPacket_GAME_OPTION_CHANGE_REQ nKMPacket_GAME_OPTION_CHANGE_REQ = new NKMPacket_GAME_OPTION_CHANGE_REQ();
			nKMPacket_GAME_OPTION_CHANGE_REQ.actionCameraType = actionCameraType;
			nKMPacket_GAME_OPTION_CHANGE_REQ.isTrackCamera = bTrackCamera;
			nKMPacket_GAME_OPTION_CHANGE_REQ.isViewSkillCutIn = bViewSkillCutIn;
			nKMPacket_GAME_OPTION_CHANGE_REQ.defaultPvpAutoRespawn = ePvpAutoRespawn;
			nKMPacket_GAME_OPTION_CHANGE_REQ.autoSyncFriendDeck = bAutoSyncFriendDeck;
			if (!bLocal)
			{
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_OPTION_CHANGE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
				return;
			}
			NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_OPTION_CHANGE_REQ);
			NKMPopUpBox.OpenWaitBox();
		}

		public static void Send_NKMPacket_UPDATE_PVP_INVITATION_OPTION_REQ(PrivatePvpInvitation invitationOption)
		{
			NKMPacket_UPDATE_PVP_INVITATION_OPTION_REQ nKMPacket_UPDATE_PVP_INVITATION_OPTION_REQ = new NKMPacket_UPDATE_PVP_INVITATION_OPTION_REQ();
			nKMPacket_UPDATE_PVP_INVITATION_OPTION_REQ.value = invitationOption;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UPDATE_PVP_INVITATION_OPTION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_INVENTORY_EXPAND_REQ(NKM_INVENTORY_EXPAND_TYPE inventoryType, int count)
		{
			NKMPacket_INVENTORY_EXPAND_REQ nKMPacket_INVENTORY_EXPAND_REQ = new NKMPacket_INVENTORY_EXPAND_REQ();
			nKMPacket_INVENTORY_EXPAND_REQ.inventoryExpandType = inventoryType;
			nKMPacket_INVENTORY_EXPAND_REQ.count = count;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_INVENTORY_EXPAND_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_REMOVE_EQUIP_ITEM_REQ(List<long> listEquipSlot)
		{
			if (listEquipSlot != null && listEquipSlot.Count > 0)
			{
				NKMPacket_REMOVE_EQUIP_ITEM_REQ nKMPacket_REMOVE_EQUIP_ITEM_REQ = new NKMPacket_REMOVE_EQUIP_ITEM_REQ();
				nKMPacket_REMOVE_EQUIP_ITEM_REQ.removeEquipItemUIDList = new List<long>(listEquipSlot);
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_REMOVE_EQUIP_ITEM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_LOCK_ITEM_REQ(long targetItemUID, bool bLock)
		{
			NKMPacket_LOCK_EQUIP_ITEM_REQ nKMPacket_LOCK_EQUIP_ITEM_REQ = new NKMPacket_LOCK_EQUIP_ITEM_REQ();
			nKMPacket_LOCK_EQUIP_ITEM_REQ.equipItemUID = targetItemUID;
			nKMPacket_LOCK_EQUIP_ITEM_REQ.isLock = bLock;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LOCK_EQUIP_ITEM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bool bEquip, long unitUID, long equipUID, ITEM_EQUIP_POSITION equipPosition)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData == null)
			{
				Debug.Log("유저 정보를 찾을 수 없습니다.");
				return;
			}
			if (nKMUserData.m_InventoryData.GetItemEquip(equipUID) == null)
			{
				Debug.Log($"해당 아이템 정보를 찾을 수 없습니다. : {equipUID}");
				return;
			}
			if (nKMUserData.m_ArmyData.GetUnitOrShipFromUID(unitUID) == null)
			{
				Debug.Log($"해당 유닛 정보를 찾을 수 없습니다. : {unitUID}");
				return;
			}
			NKMPacket_EQUIP_ITEM_EQUIP_REQ nKMPacket_EQUIP_ITEM_EQUIP_REQ = new NKMPacket_EQUIP_ITEM_EQUIP_REQ();
			nKMPacket_EQUIP_ITEM_EQUIP_REQ.isEquip = bEquip;
			nKMPacket_EQUIP_ITEM_EQUIP_REQ.unitUID = unitUID;
			nKMPacket_EQUIP_ITEM_EQUIP_REQ.equipItemUID = equipUID;
			nKMPacket_EQUIP_ITEM_EQUIP_REQ.equipPosition = equipPosition;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_ITEM_EQUIP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_UI_SCEN_CHANGED_REQ(NKM_SCEN_ID scenID)
		{
			NKMPacket_UI_SCEN_CHANGED_REQ nKMPacket_UI_SCEN_CHANGED_REQ = new NKMPacket_UI_SCEN_CHANGED_REQ();
			nKMPacket_UI_SCEN_CHANGED_REQ.scenID = scenID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UI_SCEN_CHANGED_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_CUTSCENE_DUNGEON_START_REQ(int cutsceneDungeonID)
		{
			NKMPacket_CUTSCENE_DUNGEON_START_REQ nKMPacket_CUTSCENE_DUNGEON_START_REQ = new NKMPacket_CUTSCENE_DUNGEON_START_REQ();
			nKMPacket_CUTSCENE_DUNGEON_START_REQ.dungeonID = cutsceneDungeonID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_CUTSCENE_DUNGEON_START_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_NEXON_NGS_DATA_NOT(byte[] buffer)
		{
			if (NKCScenManager.GetScenManager() == null || NKCScenManager.GetScenManager().GetConnectGame() == null || !NKCScenManager.GetScenManager().GetConnectGame().IsConnected)
			{
				Debug.Log("NKMPacket_NEXON_NGS_DATA_NOT, skip because not connected");
				return;
			}
			NKMPacket_NEXON_NGS_DATA_NOT nKMPacket_NEXON_NGS_DATA_NOT = new NKMPacket_NEXON_NGS_DATA_NOT();
			nKMPacket_NEXON_NGS_DATA_NOT.buffer = buffer;
			Debug.Log("NKMPacket_NEXON_NGS_DATA_NOT, buff Size : " + buffer.Length);
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_NEXON_NGS_DATA_NOT, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_WARFARE_GAME_START_REQ(NKMPacket_WARFARE_GAME_START_REQ req)
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(req, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_WARFARE_GAME_MOVE_REQ(byte fromTileIndex, byte toTileIndex)
		{
			NKMPacket_WARFARE_GAME_MOVE_REQ nKMPacket_WARFARE_GAME_MOVE_REQ = new NKMPacket_WARFARE_GAME_MOVE_REQ();
			nKMPacket_WARFARE_GAME_MOVE_REQ.tileIndexFrom = fromTileIndex;
			nKMPacket_WARFARE_GAME_MOVE_REQ.tileIndexTo = toTileIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WARFARE_GAME_MOVE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_WARFARE_GAME_USE_SERVICE_REQ(int warfareGameUnitUID, NKM_WARFARE_SERVICE_TYPE serviceType)
		{
			NKMPacket_WARFARE_GAME_USE_SERVICE_REQ nKMPacket_WARFARE_GAME_USE_SERVICE_REQ = new NKMPacket_WARFARE_GAME_USE_SERVICE_REQ();
			nKMPacket_WARFARE_GAME_USE_SERVICE_REQ.warfareGameUnitUID = warfareGameUnitUID;
			nKMPacket_WARFARE_GAME_USE_SERVICE_REQ.serviceType = serviceType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WARFARE_GAME_USE_SERVICE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_WARFARE_GAME_NEXT_ORDER_REQ()
		{
			Debug.Log("NKMPacket_WARFARE_GAME_NEXT_ORDER_REQ - CurrentScenID : " + NKCScenManager.GetScenManager().GetNowScenID());
			NKMPacket_WARFARE_GAME_NEXT_ORDER_REQ packet = new NKMPacket_WARFARE_GAME_NEXT_ORDER_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_WARFARE_GAME_GIVE_UP_REQ()
		{
			NKMPacket_WARFARE_GAME_GIVE_UP_REQ packet = new NKMPacket_WARFARE_GAME_GIVE_UP_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_WARFARE_GAME_AUTO_REQ(bool bAuto)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			Send_NKMPacket_WARFARE_GAME_AUTO_REQ(bAuto, myUserData.m_UserOption.m_bAutoWarfareRepair);
		}

		public static void Send_NKMPacket_WARFARE_GAME_AUTO_REQ(bool bAuto, bool bRepair)
		{
			NKMPacket_WARFARE_GAME_AUTO_REQ nKMPacket_WARFARE_GAME_AUTO_REQ = new NKMPacket_WARFARE_GAME_AUTO_REQ();
			nKMPacket_WARFARE_GAME_AUTO_REQ.isAuto = bAuto;
			nKMPacket_WARFARE_GAME_AUTO_REQ.isAutoRepair = bRepair;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WARFARE_GAME_AUTO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_WARFARE_GAME_TURN_FINISH_REQ()
		{
			NKMPacket_WARFARE_GAME_TURN_FINISH_REQ packet = new NKMPacket_WARFARE_GAME_TURN_FINISH_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_WARFARE_FRIEND_LIST_REQ()
		{
			NKMPacket_WARFARE_FRIEND_LIST_REQ packet = new NKMPacket_WARFARE_FRIEND_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_WARFARE_RECOVER_REQ(byte deckIndex, short tileIndex)
		{
			NKMPacket_WARFARE_RECOVER_REQ nKMPacket_WARFARE_RECOVER_REQ = new NKMPacket_WARFARE_RECOVER_REQ();
			nKMPacket_WARFARE_RECOVER_REQ.deckIndex = deckIndex;
			nKMPacket_WARFARE_RECOVER_REQ.tileIndex = tileIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WARFARE_RECOVER_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_STAGE_UNLOCK_REQ(int stageID)
		{
			NKMPacket_STAGE_UNLOCK_REQ nKMPacket_STAGE_UNLOCK_REQ = new NKMPacket_STAGE_UNLOCK_REQ();
			nKMPacket_STAGE_UNLOCK_REQ.stageId = stageID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_STAGE_UNLOCK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_FAVORITES_STAGE_REQ()
		{
			NKMPacket_FAVORITES_STAGE_REQ packet = new NKMPacket_FAVORITES_STAGE_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_FAVORITES_STAGE_ADD_REQ(int stageID)
		{
			NKMPacket_FAVORITES_STAGE_ADD_REQ nKMPacket_FAVORITES_STAGE_ADD_REQ = new NKMPacket_FAVORITES_STAGE_ADD_REQ();
			nKMPacket_FAVORITES_STAGE_ADD_REQ.stageId = stageID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FAVORITES_STAGE_ADD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_FAVORITES_STAGE_DELETE_REQ(int stageID)
		{
			NKMPacket_FAVORITES_STAGE_DELETE_REQ nKMPacket_FAVORITES_STAGE_DELETE_REQ = new NKMPacket_FAVORITES_STAGE_DELETE_REQ();
			nKMPacket_FAVORITES_STAGE_DELETE_REQ.stageId = stageID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FAVORITES_STAGE_DELETE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_TEAM_COLLECTION_REWARD_REQ(int teamID)
		{
			NKMPacket_TEAM_COLLECTION_REWARD_REQ nKMPacket_TEAM_COLLECTION_REWARD_REQ = new NKMPacket_TEAM_COLLECTION_REWARD_REQ();
			nKMPacket_TEAM_COLLECTION_REWARD_REQ.teamID = teamID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TEAM_COLLECTION_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_UNIT_REVIEW_TAG_LIST_REQ(int unitID)
		{
			NKMPacket_UNIT_REVIEW_TAG_LIST_REQ nKMPacket_UNIT_REVIEW_TAG_LIST_REQ = new NKMPacket_UNIT_REVIEW_TAG_LIST_REQ();
			nKMPacket_UNIT_REVIEW_TAG_LIST_REQ.unitID = unitID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_TAG_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_UNIT_REVIEW_TAG_VOTE_REQ(int unitID, short tagType)
		{
			NKMPacket_UNIT_REVIEW_TAG_VOTE_REQ nKMPacket_UNIT_REVIEW_TAG_VOTE_REQ = new NKMPacket_UNIT_REVIEW_TAG_VOTE_REQ();
			nKMPacket_UNIT_REVIEW_TAG_VOTE_REQ.unitID = unitID;
			nKMPacket_UNIT_REVIEW_TAG_VOTE_REQ.tagType = tagType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_TAG_VOTE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_REQ(int unitID, short tagType)
		{
			NKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_REQ nKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_REQ = new NKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_REQ();
			nKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_REQ.unitID = unitID;
			nKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_REQ.tagType = tagType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_REFRESH_COMPANY_BUFF_REQ()
		{
			NKMPacket_REFRESH_COMPANY_BUFF_REQ packet = new NKMPacket_REFRESH_COMPANY_BUFF_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_ACCOUNT_LINK_REQ()
		{
			NKMPacket_ACCOUNT_LINK_REQ packet = new NKMPacket_ACCOUNT_LINK_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_ACCOUNT_LEAVE_STATE_REQ(bool bLeave)
		{
			NKMPacket_ACCOUNT_LEAVE_STATE_REQ nKMPacket_ACCOUNT_LEAVE_STATE_REQ = new NKMPacket_ACCOUNT_LEAVE_STATE_REQ();
			nKMPacket_ACCOUNT_LEAVE_STATE_REQ.leave = bLeave;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_ACCOUNT_LEAVE_STATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_BINGO_RANDOM_MARK_REQ(int eventID)
		{
			NKMPacket_EVENT_BINGO_RANDOM_MARK_REQ nKMPacket_EVENT_BINGO_RANDOM_MARK_REQ = new NKMPacket_EVENT_BINGO_RANDOM_MARK_REQ();
			nKMPacket_EVENT_BINGO_RANDOM_MARK_REQ.eventId = eventID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_BINGO_RANDOM_MARK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_EVENT_BINGO_INDEX_MARK_REQ(int eventID, int tileIndex)
		{
			NKMPacket_EVENT_BINGO_INDEX_MARK_REQ nKMPacket_EVENT_BINGO_INDEX_MARK_REQ = new NKMPacket_EVENT_BINGO_INDEX_MARK_REQ();
			nKMPacket_EVENT_BINGO_INDEX_MARK_REQ.eventId = eventID;
			nKMPacket_EVENT_BINGO_INDEX_MARK_REQ.hsTileIndex = new HashSet<int>();
			nKMPacket_EVENT_BINGO_INDEX_MARK_REQ.hsTileIndex.Add(tileIndex);
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_BINGO_INDEX_MARK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_EVENT_BINGO_INDEX_MARK_REQ(int eventID, HashSet<int> hsTileIndex)
		{
			NKMPacket_EVENT_BINGO_INDEX_MARK_REQ nKMPacket_EVENT_BINGO_INDEX_MARK_REQ = new NKMPacket_EVENT_BINGO_INDEX_MARK_REQ();
			nKMPacket_EVENT_BINGO_INDEX_MARK_REQ.eventId = eventID;
			nKMPacket_EVENT_BINGO_INDEX_MARK_REQ.hsTileIndex = hsTileIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_BINGO_INDEX_MARK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_EVENT_BINGO_REWARD_REQ(int eventID, int rewardIndex)
		{
			NKMPacket_EVENT_BINGO_REWARD_REQ nKMPacket_EVENT_BINGO_REWARD_REQ = new NKMPacket_EVENT_BINGO_REWARD_REQ();
			nKMPacket_EVENT_BINGO_REWARD_REQ.eventId = eventID;
			nKMPacket_EVENT_BINGO_REWARD_REQ.rewardIndex = rewardIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_BINGO_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_BINGO_REWARD_ALL_REQ(int eventID)
		{
			NKMPacket_EVENT_BINGO_REWARD_ALL_REQ nKMPacket_EVENT_BINGO_REWARD_ALL_REQ = new NKMPacket_EVENT_BINGO_REWARD_ALL_REQ();
			nKMPacket_EVENT_BINGO_REWARD_ALL_REQ.eventId = eventID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_BINGO_REWARD_ALL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_ZLONG_USE_COUPON_REQ(string code)
		{
			int result = 1;
			if (int.TryParse(NKCDownloadConfig.s_ServerID, out result))
			{
				NKMPacket_ZLONG_USE_COUPON_REQ2 nKMPacket_ZLONG_USE_COUPON_REQ = new NKMPacket_ZLONG_USE_COUPON_REQ2();
				nKMPacket_ZLONG_USE_COUPON_REQ.couponCode = code;
				nKMPacket_ZLONG_USE_COUPON_REQ.zlongServerId = result;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_ZLONG_USE_COUPON_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_SURVEY_COMPLETE_REQ(long id)
		{
			NKMPacket_SURVEY_COMPLETE_REQ nKMPacket_SURVEY_COMPLETE_REQ = new NKMPacket_SURVEY_COMPLETE_REQ();
			nKMPacket_SURVEY_COMPLETE_REQ.surveyId = id;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SURVEY_COMPLETE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_REQ(long equipItemUID, List<MiscItemData> lstData)
		{
			NKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_REQ nKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_REQ = new NKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_REQ();
			nKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_REQ.equipItemUID = equipItemUID;
			nKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_REQ.miscItemList = lstData;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_REQ(long equipUID)
		{
			NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
			NKMEquipItemData itemEquip = inventoryData.GetItemEquip(equipUID);
			if (itemEquip == null)
			{
				return;
			}
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet == null || equipTemplet.SetGroupList == null || equipTemplet.SetGroupList.Count <= 0)
			{
				return;
			}
			bool flag = true;
			if (equipTemplet.m_RandomSetReqItemValue > inventoryData.GetCountMiscItem(equipTemplet.m_RandomSetReqItemID))
			{
				flag = false;
			}
			if (flag && equipTemplet.m_RandomSetReqResource > inventoryData.GetCountMiscItem(1))
			{
				int credit = equipTemplet.m_RandomSetReqResource;
				if (NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT))
				{
					NKCCompanyBuff.SetDiscountOfCreditInEnchantTuning(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
				}
				if (credit > inventoryData.GetCountMiscItem(1))
				{
					flag = false;
				}
			}
			if (!flag)
			{
				Debug.Log($"자원이 부족합니다 - Send_NKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_REQ : id : {itemEquip.m_ItemEquipID}");
				return;
			}
			NKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_REQ nKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_REQ = new NKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_REQ();
			nKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_REQ.equipUID = equipUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_REQ(long equipUID)
		{
			NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(equipUID);
			if (itemEquip != null && NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID) != null)
			{
				NKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_REQ nKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_REQ = new NKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_REQ();
				nKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_REQ.equipUID = equipUID;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_REQ(long equipUID)
		{
			if (equipUID == 0L)
			{
				return;
			}
			NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(equipUID);
			if (itemEquip != null && itemEquip.m_SetOptionId <= 0)
			{
				NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
				if (equipTemplet != null && equipTemplet.m_lstSetGroup.Count > 0)
				{
					NKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_REQ nKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_REQ = new NKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_REQ();
					nKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_REQ.equipUID = equipUID;
					NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
				}
			}
		}

		public static void Send_NKMPacket_Equip_Tuning_Cancel_REQ()
		{
			NKMPacket_EQUIP_TUNING_CANCEL_REQ packet = new NKMPacket_EQUIP_TUNING_CANCEL_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ(long equipUid, int targetOptionID, NKM_STAT_TYPE stateType)
		{
			NKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ nKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ = new NKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ();
			nKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ.equipUid = equipUid;
			nKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ.equipOptionId = targetOptionID;
			nKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ.statType = stateType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_REQ(long equipUid, int targetSetOptionID)
		{
			NKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_REQ nKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_REQ = new NKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_REQ();
			nKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_REQ.equipUid = equipUid;
			nKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_REQ.setOptionId = targetSetOptionID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_EQUIP_UPGRADE_REQ(long equipUID, List<long> consumeEquipItemUidList)
		{
			NKMPacket_EQUIP_UPGRADE_REQ nKMPacket_EQUIP_UPGRADE_REQ = new NKMPacket_EQUIP_UPGRADE_REQ();
			nKMPacket_EQUIP_UPGRADE_REQ.equipUid = equipUID;
			nKMPacket_EQUIP_UPGRADE_REQ.consumeEquipItemUidList = consumeEquipItemUidList;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_UPGRADE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_OPEN_SOCKET_REQ(long equipUId, int socketIndex)
		{
			NKMPacket_EQUIP_OPEN_SOCKET_REQ nKMPacket_EQUIP_OPEN_SOCKET_REQ = new NKMPacket_EQUIP_OPEN_SOCKET_REQ();
			nKMPacket_EQUIP_OPEN_SOCKET_REQ.equipUid = equipUId;
			nKMPacket_EQUIP_OPEN_SOCKET_REQ.socketIndex = socketIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_OPEN_SOCKET_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_REQ(long equipUid, int socketIndex)
		{
			NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_REQ nKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_REQ = new NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_REQ();
			nKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_REQ.equipUid = equipUid;
			nKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_REQ.socketIndex = socketIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CONFIRM_REQ(long equipUid, int socketIndex)
		{
			NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CONFIRM_REQ nKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CONFIRM_REQ = new NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CONFIRM_REQ();
			nKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CONFIRM_REQ.equipUid = equipUid;
			nKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CONFIRM_REQ.socketIndex = socketIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CONFIRM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_REQ()
		{
			NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_REQ packet = new NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_RESET_STAGE_PLAY_COUNT_REQ(int stageID)
		{
			NKMPacket_RESET_STAGE_PLAY_COUNT_REQ nKMPacket_RESET_STAGE_PLAY_COUNT_REQ = new NKMPacket_RESET_STAGE_PLAY_COUNT_REQ();
			nKMPacket_RESET_STAGE_PLAY_COUNT_REQ.stageId = stageID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_RESET_STAGE_PLAY_COUNT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_FAVORITES_STAGE_UPDATE_REQ(Dictionary<int, int> favStageDic)
		{
			NKMPacket_FAVORITES_STAGE_UPDATE_REQ packet = new NKMPacket_FAVORITES_STAGE_UPDATE_REQ
			{
				favoritesStage = favStageDic
			};
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_BACKGROUND_CHANGE_REQ(NKMBackgroundInfo bgInfo)
		{
			NKMPacket_BACKGROUND_CHANGE_REQ nKMPacket_BACKGROUND_CHANGE_REQ = new NKMPacket_BACKGROUND_CHANGE_REQ();
			nKMPacket_BACKGROUND_CHANGE_REQ.backgroundInfo = bgInfo;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_BACKGROUND_CHANGE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_CHAT_TRANSLATE_REQ(long guildUID, long chatUID, string targetLangCode)
		{
			NKMPacket_GUILD_CHAT_TRANSLATE_REQ nKMPacket_GUILD_CHAT_TRANSLATE_REQ = new NKMPacket_GUILD_CHAT_TRANSLATE_REQ();
			nKMPacket_GUILD_CHAT_TRANSLATE_REQ.guildUid = guildUID;
			nKMPacket_GUILD_CHAT_TRANSLATE_REQ.messageUid = chatUID;
			nKMPacket_GUILD_CHAT_TRANSLATE_REQ.targetLanguage = targetLangCode;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_CHAT_TRANSLATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_GUILD_CREATE_REQ(string name, GuildJoinType joinType, long badgeId, string greeting)
		{
			NKMPacket_GUILD_CREATE_REQ nKMPacket_GUILD_CREATE_REQ = new NKMPacket_GUILD_CREATE_REQ();
			nKMPacket_GUILD_CREATE_REQ.guildName = name;
			nKMPacket_GUILD_CREATE_REQ.guildJoinType = joinType;
			nKMPacket_GUILD_CREATE_REQ.badgeId = badgeId;
			nKMPacket_GUILD_CREATE_REQ.greeting = greeting;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_CREATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_CLOSE_REQ(long guildUid)
		{
			NKMPacket_GUILD_CLOSE_REQ nKMPacket_GUILD_CLOSE_REQ = new NKMPacket_GUILD_CLOSE_REQ();
			nKMPacket_GUILD_CLOSE_REQ.guildUid = guildUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_CLOSE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_CLOSE_CANCEL_REQ(long guildUid)
		{
			NKMPacket_GUILD_CLOSE_CANCEL_REQ nKMPacket_GUILD_CLOSE_CANCEL_REQ = new NKMPacket_GUILD_CLOSE_CANCEL_REQ();
			nKMPacket_GUILD_CLOSE_CANCEL_REQ.guildUid = guildUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_CLOSE_CANCEL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_SEARCH_REQ(string keyword)
		{
			NKMPacket_GUILD_SEARCH_REQ nKMPacket_GUILD_SEARCH_REQ = new NKMPacket_GUILD_SEARCH_REQ();
			nKMPacket_GUILD_SEARCH_REQ.keyword = keyword;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_SEARCH_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_GUILD_LIST_REQ(GuildListType listType)
		{
			NKMPacket_GUILD_LIST_REQ nKMPacket_GUILD_LIST_REQ = new NKMPacket_GUILD_LIST_REQ();
			nKMPacket_GUILD_LIST_REQ.guildListType = listType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_GUILD_DATA_REQ(long uid)
		{
			NKMPacket_GUILD_DATA_REQ nKMPacket_GUILD_DATA_REQ = new NKMPacket_GUILD_DATA_REQ();
			nKMPacket_GUILD_DATA_REQ.guildUid = uid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_DATA_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_JOIN_REQ(long guildUid, GuildJoinType joinType)
		{
			NKMPacket_GUILD_JOIN_REQ nKMPacket_GUILD_JOIN_REQ = new NKMPacket_GUILD_JOIN_REQ();
			nKMPacket_GUILD_JOIN_REQ.guildUid = guildUid;
			nKMPacket_GUILD_JOIN_REQ.guildJoinType = joinType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_JOIN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_CANCEL_JOIN_REQ(long guildUid)
		{
			NKMPacket_GUILD_CANCEL_JOIN_REQ nKMPacket_GUILD_CANCEL_JOIN_REQ = new NKMPacket_GUILD_CANCEL_JOIN_REQ();
			nKMPacket_GUILD_CANCEL_JOIN_REQ.guildUid = guildUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_CANCEL_JOIN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_ACCEPT_JOIN_REQ(long guildUid, long userUid, bool bAllow)
		{
			NKMPacket_GUILD_ACCEPT_JOIN_REQ nKMPacket_GUILD_ACCEPT_JOIN_REQ = new NKMPacket_GUILD_ACCEPT_JOIN_REQ();
			nKMPacket_GUILD_ACCEPT_JOIN_REQ.joinUserUid = userUid;
			nKMPacket_GUILD_ACCEPT_JOIN_REQ.guildUid = guildUid;
			nKMPacket_GUILD_ACCEPT_JOIN_REQ.isAllow = bAllow;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_ACCEPT_JOIN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_INVITE_REQ(long guildUid, long userUid)
		{
			NKMPacket_GUILD_INVITE_REQ nKMPacket_GUILD_INVITE_REQ = new NKMPacket_GUILD_INVITE_REQ();
			nKMPacket_GUILD_INVITE_REQ.guildUid = guildUid;
			nKMPacket_GUILD_INVITE_REQ.userUid = userUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_INVITE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_CANCEL_INVITE_REQ(long guildUid, long userUid)
		{
			NKMPacket_GUILD_CANCEL_INVITE_REQ nKMPacket_GUILD_CANCEL_INVITE_REQ = new NKMPacket_GUILD_CANCEL_INVITE_REQ();
			nKMPacket_GUILD_CANCEL_INVITE_REQ.guildUid = guildUid;
			nKMPacket_GUILD_CANCEL_INVITE_REQ.userUid = userUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_CANCEL_INVITE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_ACCEPT_INVITE_REQ(long guildUid, bool bAllow)
		{
			NKMPacket_GUILD_ACCEPT_INVITE_REQ nKMPacket_GUILD_ACCEPT_INVITE_REQ = new NKMPacket_GUILD_ACCEPT_INVITE_REQ();
			nKMPacket_GUILD_ACCEPT_INVITE_REQ.guildUid = guildUid;
			nKMPacket_GUILD_ACCEPT_INVITE_REQ.isAllow = bAllow;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_ACCEPT_INVITE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_EXIT_REQ(long guildUid)
		{
			NKMPacket_GUILD_EXIT_REQ nKMPacket_GUILD_EXIT_REQ = new NKMPacket_GUILD_EXIT_REQ();
			nKMPacket_GUILD_EXIT_REQ.guildUid = guildUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_EXIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_SET_MEMBER_GRADE_REQ(long guildUid, long targetUserUid, GuildMemberGrade grade)
		{
			NKMPacket_GUILD_SET_MEMBER_GRADE_REQ nKMPacket_GUILD_SET_MEMBER_GRADE_REQ = new NKMPacket_GUILD_SET_MEMBER_GRADE_REQ();
			nKMPacket_GUILD_SET_MEMBER_GRADE_REQ.guildUid = guildUid;
			nKMPacket_GUILD_SET_MEMBER_GRADE_REQ.targetUserUid = targetUserUid;
			nKMPacket_GUILD_SET_MEMBER_GRADE_REQ.grade = grade;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_SET_MEMBER_GRADE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_BAN_REQ(long guildUid, long targetUserUid, int banReason)
		{
			NKMPacket_GUILD_BAN_REQ nKMPacket_GUILD_BAN_REQ = new NKMPacket_GUILD_BAN_REQ();
			nKMPacket_GUILD_BAN_REQ.guildUid = guildUid;
			nKMPacket_GUILD_BAN_REQ.targetUserUid = targetUserUid;
			nKMPacket_GUILD_BAN_REQ.banReason = banReason;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_BAN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_REQ(long guildUid, long targetUserUid)
		{
			NKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_REQ nKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_REQ = new NKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_REQ();
			nKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_REQ.guildUid = guildUid;
			nKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_REQ.targetUserUid = targetUserUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_MASTER_MIGRATION_REQ(long guildUid)
		{
			NKMPacket_GUILD_MASTER_MIGRATION_REQ nKMPacket_GUILD_MASTER_MIGRATION_REQ = new NKMPacket_GUILD_MASTER_MIGRATION_REQ();
			nKMPacket_GUILD_MASTER_MIGRATION_REQ.guildUid = guildUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_MASTER_MIGRATION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_UPDATE_DATA_REQ(long guildUid, long badgeId, string greeting, GuildJoinType guildJoinType, GuildChatNoticeType chatNoticeType)
		{
			NKMPacket_GUILD_UPDATE_DATA_REQ nKMPacket_GUILD_UPDATE_DATA_REQ = new NKMPacket_GUILD_UPDATE_DATA_REQ();
			nKMPacket_GUILD_UPDATE_DATA_REQ.guildUid = guildUid;
			nKMPacket_GUILD_UPDATE_DATA_REQ.badgeId = badgeId;
			nKMPacket_GUILD_UPDATE_DATA_REQ.greeting = greeting;
			nKMPacket_GUILD_UPDATE_DATA_REQ.guildJoinType = guildJoinType;
			nKMPacket_GUILD_UPDATE_DATA_REQ.chatNoticeType = chatNoticeType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_UPDATE_DATA_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_UPDATE_NOTICE_REQ(long guildUid, string notice)
		{
			NKMPacket_GUILD_UPDATE_NOTICE_REQ nKMPacket_GUILD_UPDATE_NOTICE_REQ = new NKMPacket_GUILD_UPDATE_NOTICE_REQ();
			nKMPacket_GUILD_UPDATE_NOTICE_REQ.guildUid = guildUid;
			nKMPacket_GUILD_UPDATE_NOTICE_REQ.notice = notice;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_UPDATE_NOTICE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_UPDATE_MEMBER_GREETING_REQ(long guildUid, string greeting)
		{
			NKMPacket_GUILD_UPDATE_MEMBER_GREETING_REQ nKMPacket_GUILD_UPDATE_MEMBER_GREETING_REQ = new NKMPacket_GUILD_UPDATE_MEMBER_GREETING_REQ();
			nKMPacket_GUILD_UPDATE_MEMBER_GREETING_REQ.guildUid = guildUid;
			nKMPacket_GUILD_UPDATE_MEMBER_GREETING_REQ.greeting = greeting;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_UPDATE_MEMBER_GREETING_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_ATTENDANCE_REQ(long guildUid)
		{
			NKMPacket_GUILD_ATTENDANCE_REQ nKMPacket_GUILD_ATTENDANCE_REQ = new NKMPacket_GUILD_ATTENDANCE_REQ();
			nKMPacket_GUILD_ATTENDANCE_REQ.guildUid = guildUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_ATTENDANCE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_RECOMMEND_INVITE_LIST_REQ(long guildUid)
		{
			NKMPacket_GUILD_RECOMMEND_INVITE_LIST_REQ nKMPacket_GUILD_RECOMMEND_INVITE_LIST_REQ = new NKMPacket_GUILD_RECOMMEND_INVITE_LIST_REQ();
			nKMPacket_GUILD_RECOMMEND_INVITE_LIST_REQ.guildUid = guildUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_RECOMMEND_INVITE_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_DONATION_REQ(int donationId, int donationCount)
		{
			NKMPacket_GUILD_DONATION_REQ nKMPacket_GUILD_DONATION_REQ = new NKMPacket_GUILD_DONATION_REQ();
			nKMPacket_GUILD_DONATION_REQ.guildUid = NKCGuildManager.MyData.guildUid;
			nKMPacket_GUILD_DONATION_REQ.donationId = donationId;
			nKMPacket_GUILD_DONATION_REQ.donationCount = donationCount;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_DONATION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_BUY_BUFF_REQ(int welfareId)
		{
			NKMPacket_GUILD_BUY_BUFF_REQ nKMPacket_GUILD_BUY_BUFF_REQ = new NKMPacket_GUILD_BUY_BUFF_REQ();
			nKMPacket_GUILD_BUY_BUFF_REQ.guildUid = NKCGuildManager.MyData.guildUid;
			nKMPacket_GUILD_BUY_BUFF_REQ.welfareId = welfareId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_BUY_BUFF_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_BUY_WELFARE_POINT_REQ(int buyCount)
		{
			NKMPacket_GUILD_BUY_WELFARE_POINT_REQ nKMPacket_GUILD_BUY_WELFARE_POINT_REQ = new NKMPacket_GUILD_BUY_WELFARE_POINT_REQ();
			nKMPacket_GUILD_BUY_WELFARE_POINT_REQ.buyCount = buyCount;
			nKMPacket_GUILD_BUY_WELFARE_POINT_REQ.guildUid = NKCGuildManager.MyData.guildUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_BUY_WELFARE_POINT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_REQ(long guildUid, string notice)
		{
			NKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_REQ nKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_REQ = new NKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_REQ();
			nKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_REQ.guildUid = guildUid;
			nKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_REQ.notice = notice;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_RENAME_REQ(string newName)
		{
			NKMPacket_GUILD_RENAME_REQ nKMPacket_GUILD_RENAME_REQ = new NKMPacket_GUILD_RENAME_REQ();
			nKMPacket_GUILD_RENAME_REQ.newName = newName;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_RENAME_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_DUNGEON_INFO_REQ(long guildUid)
		{
			if (!NKCPopupOKCancel.isOpen())
			{
				Debug.Log($"[GuildDungeon] m_GuildDungeonState  [{NKCGuildCoopManager.m_GuildDungeonState}]");
				Debug.Log($"[GuildDungeon] m_bGuildCoopDataRecved  [{NKCGuildCoopManager.m_bGuildCoopDataRecved}]");
				Debug.Log($"[GuildDungeon] m_NextSessionStartDateUTC  [{NKCGuildCoopManager.m_NextSessionStartDateUTC}]");
				NKMPacket_GUILD_DUNGEON_INFO_REQ nKMPacket_GUILD_DUNGEON_INFO_REQ = new NKMPacket_GUILD_DUNGEON_INFO_REQ();
				nKMPacket_GUILD_DUNGEON_INFO_REQ.guildUid = guildUid;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_DUNGEON_INFO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_GUILD_DUNGEON_MEMBER_INFO_REQ(long guildUid)
		{
			NKMPacket_GUILD_DUNGEON_MEMBER_INFO_REQ nKMPacket_GUILD_DUNGEON_MEMBER_INFO_REQ = new NKMPacket_GUILD_DUNGEON_MEMBER_INFO_REQ();
			nKMPacket_GUILD_DUNGEON_MEMBER_INFO_REQ.guildUid = guildUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_DUNGEON_MEMBER_INFO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ(GuildDungeonRewardCategory category, int rewardCountValue)
		{
			NKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ nKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ = new NKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ();
			nKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ.rewardCategory = category;
			nKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ.rewardCountValue = rewardCountValue;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_DUNGEON_TICKET_BUY_REQ()
		{
			NKMPacket_GUILD_DUNGEON_TICKET_BUY_REQ packet = new NKMPacket_GUILD_DUNGEON_TICKET_BUY_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ(int bossStageId, byte selectedDeckIndex, bool isPracticeMode)
		{
			NKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ nKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ = new NKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ();
			nKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ.bossStageId = bossStageId;
			nKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ.selectDeckIndex = selectedDeckIndex;
			nKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ.isPractice = isPracticeMode;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_DUNGEON_SESSION_REWARD_REQ()
		{
			NKMPacket_GUILD_DUNGEON_SESSION_REWARD_REQ packet = new NKMPacket_GUILD_DUNGEON_SESSION_REWARD_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_DUNGEON_FLAG_REQ(long guildUid, int arenaIndex, int flagIndex)
		{
			NKMPacket_GUILD_DUNGEON_FLAG_REQ nKMPacket_GUILD_DUNGEON_FLAG_REQ = new NKMPacket_GUILD_DUNGEON_FLAG_REQ();
			nKMPacket_GUILD_DUNGEON_FLAG_REQ.guildUid = guildUid;
			nKMPacket_GUILD_DUNGEON_FLAG_REQ.arenaIndex = arenaIndex;
			nKMPacket_GUILD_DUNGEON_FLAG_REQ.flagIndex = flagIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_DUNGEON_FLAG_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ(long guildUid, short orderIndex)
		{
			NKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ nKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ = new NKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ();
			nKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ.guildUid = guildUid;
			nKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ.orderIndex = orderIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_GUILD_CHAT_LIST_REQ(long guildUid)
		{
			NKMPacket_GUILD_CHAT_LIST_REQ nKMPacket_GUILD_CHAT_LIST_REQ = new NKMPacket_GUILD_CHAT_LIST_REQ();
			nKMPacket_GUILD_CHAT_LIST_REQ.guildUid = guildUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_CHAT_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_CHAT_REQ(long guildUid, ChatMessageType messageType, string message, int emotionId)
		{
			NKMPacket_GUILD_CHAT_REQ nKMPacket_GUILD_CHAT_REQ = new NKMPacket_GUILD_CHAT_REQ();
			nKMPacket_GUILD_CHAT_REQ.guildUid = guildUid;
			nKMPacket_GUILD_CHAT_REQ.messageType = messageType;
			nKMPacket_GUILD_CHAT_REQ.message = message;
			nKMPacket_GUILD_CHAT_REQ.emotionId = emotionId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_CHAT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_GUILD_CHAT_COMPLAIN_REQ(long guildUid, long messageUid)
		{
			NKMPacket_GUILD_CHAT_COMPLAIN_REQ nKMPacket_GUILD_CHAT_COMPLAIN_REQ = new NKMPacket_GUILD_CHAT_COMPLAIN_REQ();
			nKMPacket_GUILD_CHAT_COMPLAIN_REQ.guildUid = guildUid;
			nKMPacket_GUILD_CHAT_COMPLAIN_REQ.messageUid = messageUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GUILD_CHAT_COMPLAIN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_PRIVATE_CHAT_REQ(long userUid, string message, int emotionId)
		{
			NKMPacket_PRIVATE_CHAT_REQ nKMPacket_PRIVATE_CHAT_REQ = new NKMPacket_PRIVATE_CHAT_REQ();
			nKMPacket_PRIVATE_CHAT_REQ.userUid = userUid;
			nKMPacket_PRIVATE_CHAT_REQ.message = message;
			nKMPacket_PRIVATE_CHAT_REQ.emotionId = emotionId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_CHAT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_PRIVATE_CHAT_LIST_REQ(long userUid)
		{
			NKMPacket_PRIVATE_CHAT_LIST_REQ nKMPacket_PRIVATE_CHAT_LIST_REQ = new NKMPacket_PRIVATE_CHAT_LIST_REQ();
			nKMPacket_PRIVATE_CHAT_LIST_REQ.userUid = userUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PRIVATE_CHAT_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_PRIVATE_CHAT_ALL_LIST_REQ()
		{
			NKMPacket_PRIVATE_CHAT_ALL_LIST_REQ packet = new NKMPacket_PRIVATE_CHAT_ALL_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_LEADERBOARD_ACHIEVE_LIST_REQ(bool bAll)
		{
			NKMPacket_LEADERBOARD_ACHIEVE_LIST_REQ nKMPacket_LEADERBOARD_ACHIEVE_LIST_REQ = new NKMPacket_LEADERBOARD_ACHIEVE_LIST_REQ();
			nKMPacket_LEADERBOARD_ACHIEVE_LIST_REQ.isAll = bAll;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LEADERBOARD_ACHIEVE_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_LEADERBOARD_SHADOWPALACE_LIST_REQ(int actId, bool bAll = false)
		{
			NKMPacket_LEADERBOARD_SHADOWPALACE_LIST_REQ nKMPacket_LEADERBOARD_SHADOWPALACE_LIST_REQ = new NKMPacket_LEADERBOARD_SHADOWPALACE_LIST_REQ();
			nKMPacket_LEADERBOARD_SHADOWPALACE_LIST_REQ.actId = actId;
			nKMPacket_LEADERBOARD_SHADOWPALACE_LIST_REQ.isAll = bAll;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LEADERBOARD_SHADOWPALACE_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_LEADERBOARD_FIERCE_LIST_REQ(bool bIsAll = false)
		{
			NKMPacket_LEADERBOARD_FIERCE_LIST_REQ nKMPacket_LEADERBOARD_FIERCE_LIST_REQ = new NKMPacket_LEADERBOARD_FIERCE_LIST_REQ();
			nKMPacket_LEADERBOARD_FIERCE_LIST_REQ.isAll = bIsAll;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LEADERBOARD_FIERCE_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ(int bossGroupID, bool bIsAll = false)
		{
			NKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ nKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ = new NKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ();
			nKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ.fierceBossGroupId = bossGroupID;
			nKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ.isAll = bIsAll;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_LEADERBOARD_GUILD_LEVEL_RANK_LIST_REQ()
		{
			NKMPacket_LEADERBOARD_GUILD_LEVEL_RANK_LIST_REQ packet = new NKMPacket_LEADERBOARD_GUILD_LEVEL_RANK_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_REQ(int seasonId)
		{
			NKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_REQ nKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_REQ = new NKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_REQ();
			nKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_REQ.seasonId = seasonId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_LEADERBOARD_TIMEATTACK_LIST_REQ(int stageId, bool bIsAll = false)
		{
			NKMPacket_LEADERBOARD_TIMEATTACK_LIST_REQ nKMPacket_LEADERBOARD_TIMEATTACK_LIST_REQ = new NKMPacket_LEADERBOARD_TIMEATTACK_LIST_REQ();
			nKMPacket_LEADERBOARD_TIMEATTACK_LIST_REQ.stageId = stageId;
			nKMPacket_LEADERBOARD_TIMEATTACK_LIST_REQ.isAll = bIsAll;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LEADERBOARD_TIMEATTACK_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_SHADOW_PALACE_START_REQ(int palaceID)
		{
			NKMPacket_SHADOW_PALACE_START_REQ nKMPacket_SHADOW_PALACE_START_REQ = new NKMPacket_SHADOW_PALACE_START_REQ();
			nKMPacket_SHADOW_PALACE_START_REQ.palaceId = palaceID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHADOW_PALACE_START_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_SHADOW_PALACE_GIVEUP_ACK(int palaceID)
		{
			NKMPacket_SHADOW_PALACE_GIVEUP_REQ nKMPacket_SHADOW_PALACE_GIVEUP_REQ = new NKMPacket_SHADOW_PALACE_GIVEUP_REQ();
			nKMPacket_SHADOW_PALACE_GIVEUP_REQ.palaceId = palaceID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHADOW_PALACE_GIVEUP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_SHADOW_PALACE_SKIP_REQ(int palaceId, int skipCount)
		{
			NKMPacket_SHADOW_PALACE_SKIP_REQ nKMPacket_SHADOW_PALACE_SKIP_REQ = new NKMPacket_SHADOW_PALACE_SKIP_REQ();
			nKMPacket_SHADOW_PALACE_SKIP_REQ.palaceId = palaceId;
			nKMPacket_SHADOW_PALACE_SKIP_REQ.skipCount = skipCount;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHADOW_PALACE_SKIP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_FIERCE_DATA_REQ()
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(new NKMPacket_FIERCE_DATA_REQ(), NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_FIERCE_COMPLETE_RANK_REWARD_REQ()
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(new NKMPacket_FIERCE_COMPLETE_RANK_REWARD_REQ(), NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_FIERCE_COMPLETE_POINT_REWARD_REQ(int pointRewardID)
		{
			NKMPacket_FIERCE_COMPLETE_POINT_REWARD_REQ nKMPacket_FIERCE_COMPLETE_POINT_REWARD_REQ = new NKMPacket_FIERCE_COMPLETE_POINT_REWARD_REQ();
			nKMPacket_FIERCE_COMPLETE_POINT_REWARD_REQ.fiercePointRewardId = pointRewardID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FIERCE_COMPLETE_POINT_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_FIERCE_COMPLETE_POINT_REWARD_ALL_REQ()
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(new NKMPacket_FIERCE_COMPLETE_POINT_REWARD_ALL_REQ(), NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_FIERCE_PROFILE_REQ(long userUID, bool bForce)
		{
			NKMPacket_FIERCE_PROFILE_REQ nKMPacket_FIERCE_PROFILE_REQ = new NKMPacket_FIERCE_PROFILE_REQ();
			nKMPacket_FIERCE_PROFILE_REQ.userUid = userUID;
			nKMPacket_FIERCE_PROFILE_REQ.isForce = bForce;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FIERCE_PROFILE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_FIERCE_PENALTY_REQ(int fierceBossID, List<int> lstPenaltys)
		{
			NKMPacket_FIERCE_PENALTY_REQ nKMPacket_FIERCE_PENALTY_REQ = new NKMPacket_FIERCE_PENALTY_REQ();
			nKMPacket_FIERCE_PENALTY_REQ.fierceBossId = fierceBossID;
			nKMPacket_FIERCE_PENALTY_REQ.penaltyIds = lstPenaltys;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FIERCE_PENALTY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MENTORING_DATA_REQ()
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(new NKMPacket_MENTORING_DATA_REQ(), NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_MENTORING_MATCH_LIST_REQ(bool bForce = false)
		{
			NKMPacket_MENTORING_MATCH_LIST_REQ nKMPacket_MENTORING_MATCH_LIST_REQ = new NKMPacket_MENTORING_MATCH_LIST_REQ();
			nKMPacket_MENTORING_MATCH_LIST_REQ.isForce = bForce;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MENTORING_MATCH_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_kNKMPacket_MENTORING_RECEIVE_LIST_REQ(MentoringIdentity mentoringIdentity, bool bForce = false)
		{
			NKMPacket_MENTORING_RECEIVE_LIST_REQ nKMPacket_MENTORING_RECEIVE_LIST_REQ = new NKMPacket_MENTORING_RECEIVE_LIST_REQ();
			nKMPacket_MENTORING_RECEIVE_LIST_REQ.identity = mentoringIdentity;
			nKMPacket_MENTORING_RECEIVE_LIST_REQ.isForce = bForce;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MENTORING_RECEIVE_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_MENTORING_SEARCH_LIST_REQ(MentoringIdentity mentoringIdentity, string keyword)
		{
			NKMPacket_MENTORING_SEARCH_LIST_REQ nKMPacket_MENTORING_SEARCH_LIST_REQ = new NKMPacket_MENTORING_SEARCH_LIST_REQ();
			nKMPacket_MENTORING_SEARCH_LIST_REQ.identity = mentoringIdentity;
			nKMPacket_MENTORING_SEARCH_LIST_REQ.keyword = keyword;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MENTORING_SEARCH_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_MENTORING_DELETE_MENTEE_REQ(long deleteMenteeUID)
		{
			NKMPacket_MENTORING_DELETE_MENTEE_REQ nKMPacket_MENTORING_DELETE_MENTEE_REQ = new NKMPacket_MENTORING_DELETE_MENTEE_REQ();
			nKMPacket_MENTORING_DELETE_MENTEE_REQ.menteeUid = deleteMenteeUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MENTORING_DELETE_MENTEE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_MENTORING_ADD_REQ(MentoringIdentity mentoringType, long userUid)
		{
			NKMPacket_MENTORING_ADD_REQ nKMPacket_MENTORING_ADD_REQ = new NKMPacket_MENTORING_ADD_REQ();
			nKMPacket_MENTORING_ADD_REQ.identity = mentoringType;
			nKMPacket_MENTORING_ADD_REQ.userUid = userUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MENTORING_ADD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_MENTORING_ACCEPT_MENTOR_REQ(long userUid)
		{
			NKMPacket_MENTORING_ACCEPT_MENTOR_REQ nKMPacket_MENTORING_ACCEPT_MENTOR_REQ = new NKMPacket_MENTORING_ACCEPT_MENTOR_REQ();
			nKMPacket_MENTORING_ACCEPT_MENTOR_REQ.mentorUid = userUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MENTORING_ACCEPT_MENTOR_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_MENTORING_DISACCEPT_MENTOR_REQ(long userUid)
		{
			NKMPacket_MENTORING_DISACCEPT_MENTOR_REQ nKMPacket_MENTORING_DISACCEPT_MENTOR_REQ = new NKMPacket_MENTORING_DISACCEPT_MENTOR_REQ();
			nKMPacket_MENTORING_DISACCEPT_MENTOR_REQ.mentorUid = userUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MENTORING_DISACCEPT_MENTOR_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_MENTORING_INVITE_REWARD_LIST_REQ()
		{
			NKMPacket_MENTORING_INVITE_REWARD_LIST_REQ packet = new NKMPacket_MENTORING_INVITE_REWARD_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_MENTORING_COMPLETE_INVITE_REWARD_REQ(int inviteSuccessRequireCnt)
		{
			NKMPacket_MENTORING_COMPLETE_INVITE_REWARD_REQ nKMPacket_MENTORING_COMPLETE_INVITE_REWARD_REQ = new NKMPacket_MENTORING_COMPLETE_INVITE_REWARD_REQ();
			nKMPacket_MENTORING_COMPLETE_INVITE_REWARD_REQ.inviteSuccessRequireCnt = inviteSuccessRequireCnt;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MENTORING_COMPLETE_INVITE_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_MENTORING_COMPLETE_INVITE_REWARD_ALL_REQ()
		{
			NKMPacket_MENTORING_COMPLETE_INVITE_REWARD_ALL_REQ packet = new NKMPacket_MENTORING_COMPLETE_INVITE_REWARD_ALL_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_MENTORING_SEASON_ID_REQ()
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(new NKMPacket_MENTORING_SEASON_ID_REQ(), NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_OPERATOR_LEVELUP_REQ(long operatorUID, List<MiscItemData> lstMat)
		{
			NKMPacket_OPERATOR_LEVELUP_REQ nKMPacket_OPERATOR_LEVELUP_REQ = new NKMPacket_OPERATOR_LEVELUP_REQ();
			nKMPacket_OPERATOR_LEVELUP_REQ.targetUnitUid = operatorUID;
			List<MiscItemData> list = new List<MiscItemData>();
			foreach (MiscItemData item in lstMat)
			{
				if (item.count > 0)
				{
					list.Add(item);
				}
			}
			nKMPacket_OPERATOR_LEVELUP_REQ.materials = list;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OPERATOR_LEVELUP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_OPERATOR_ENHANCE_REQ(long targetOperatorUID, long matOperatorUID, int tokenItemID, bool bTransfer)
		{
			NKMPacket_OPERATOR_ENHANCE_REQ nKMPacket_OPERATOR_ENHANCE_REQ = new NKMPacket_OPERATOR_ENHANCE_REQ();
			nKMPacket_OPERATOR_ENHANCE_REQ.targetUnitUid = targetOperatorUID;
			nKMPacket_OPERATOR_ENHANCE_REQ.sourceUnitUid = matOperatorUID;
			nKMPacket_OPERATOR_ENHANCE_REQ.tokenItemId = tokenItemID;
			nKMPacket_OPERATOR_ENHANCE_REQ.transSkill = bTransfer;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OPERATOR_ENHANCE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_OPERATOR_LOCK_REQ(long OperatorUID, bool bLock)
		{
			NKMPacket_OPERATOR_LOCK_REQ nKMPacket_OPERATOR_LOCK_REQ = new NKMPacket_OPERATOR_LOCK_REQ();
			nKMPacket_OPERATOR_LOCK_REQ.unitUID = OperatorUID;
			nKMPacket_OPERATOR_LOCK_REQ.locked = bLock;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OPERATOR_LOCK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_OPERATOR_REMOVE_REQ(List<long> lstRemoveOperator)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData == null)
			{
				return;
			}
			NKMArmyData armyData = nKMUserData.m_ArmyData;
			if (armyData == null)
			{
				return;
			}
			foreach (long item in lstRemoveOperator)
			{
				if (!armyData.m_dicMyOperator.ContainsKey(item))
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_NO_EXIST_UNIT);
					return;
				}
				NKM_ERROR_CODE canDeleteOperator = NKMUnitManager.GetCanDeleteOperator(armyData.GetOperatorFromUId(item), nKMUserData);
				switch (canDeleteOperator)
				{
				case NKM_ERROR_CODE.NEC_FAIL_UNIT_LOCKED:
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL_LOCKED);
					return;
				case NKM_ERROR_CODE.NEC_FAIL_UNIT_IN_DECK:
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL_IN_DECK);
					return;
				case NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_LOBBYUNIT:
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL_MAINUNIT);
					return;
				default:
					NKCPopupOKCancel.OpenOKBox(canDeleteOperator);
					return;
				case NKM_ERROR_CODE.NEC_OK:
					break;
				}
			}
			armyData.InitUnitDelete();
			armyData.SetUnitDeleteList(lstRemoveOperator);
			Send_NKMPacket_OPERATOR_REMOVE_REQ();
		}

		public static void Send_NKMPacket_OPERATOR_REMOVE_REQ()
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			if (!armyData.IsEmptyUnitDeleteList)
			{
				List<long> unitDeleteList = armyData.GetUnitDeleteList();
				NKMPacket_OPERATOR_REMOVE_REQ nKMPacket_OPERATOR_REMOVE_REQ = new NKMPacket_OPERATOR_REMOVE_REQ();
				nKMPacket_OPERATOR_REMOVE_REQ.removeUnitUIDList = unitDeleteList;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OPERATOR_REMOVE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_OPERATOR_EXTRACT_REQ(List<long> lstOperatorUID)
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			if (armyData != null)
			{
				armyData.InitUnitDelete();
				armyData.SetUnitDeleteList(lstOperatorUID);
				NKMPacket_OPERATOR_EXTRACT_REQ nKMPacket_OPERATOR_EXTRACT_REQ = new NKMPacket_OPERATOR_EXTRACT_REQ();
				nKMPacket_OPERATOR_EXTRACT_REQ.extractUnitUids = lstOperatorUID;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OPERATOR_EXTRACT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
		}

		public static void Send_NKMPacket_UPDATE_MARKET_REVIEW_REQ(string reviewDesc)
		{
			NKMPacket_UPDATE_MARKET_REVIEW_REQ nKMPacket_UPDATE_MARKET_REVIEW_REQ = new NKMPacket_UPDATE_MARKET_REVIEW_REQ();
			nKMPacket_UPDATE_MARKET_REVIEW_REQ.description = reviewDesc;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UPDATE_MARKET_REVIEW_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_DECK_OPERATOR_SET_REQ(NKMDeckIndex deckIdx, long operatorUID)
		{
			NKMPacket_DECK_OPERATOR_SET_REQ nKMPacket_DECK_OPERATOR_SET_REQ = new NKMPacket_DECK_OPERATOR_SET_REQ();
			nKMPacket_DECK_OPERATOR_SET_REQ.deckIndex = deckIdx;
			nKMPacket_DECK_OPERATOR_SET_REQ.operatorUid = operatorUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DECK_OPERATOR_SET_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_PVP_CASTING_VOTE_UNIT_REQ(List<int> lstUnitIDs)
		{
			if (lstUnitIDs != null)
			{
				NKMPacket_PVP_CASTING_VOTE_UNIT_REQ nKMPacket_PVP_CASTING_VOTE_UNIT_REQ = new NKMPacket_PVP_CASTING_VOTE_UNIT_REQ();
				nKMPacket_PVP_CASTING_VOTE_UNIT_REQ.unitIdList = lstUnitIDs;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PVP_CASTING_VOTE_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
			}
		}

		public static void Send_NKMPacket_PVP_CASTING_VOTE_SHIP_REQ(List<int> lstShipGroupIDs)
		{
			NKMPacket_PVP_CASTING_VOTE_SHIP_REQ nKMPacket_PVP_CASTING_VOTE_SHIP_REQ = new NKMPacket_PVP_CASTING_VOTE_SHIP_REQ();
			nKMPacket_PVP_CASTING_VOTE_SHIP_REQ.shipGroupIdList = lstShipGroupIDs;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PVP_CASTING_VOTE_SHIP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_PVP_CASTING_VOTE_OPERATOR_REQ(List<int> lstOperIDs)
		{
			NKMPacket_PVP_CASTING_VOTE_OPERATOR_REQ nKMPacket_PVP_CASTING_VOTE_OPERATOR_REQ = new NKMPacket_PVP_CASTING_VOTE_OPERATOR_REQ();
			nKMPacket_PVP_CASTING_VOTE_OPERATOR_REQ.operatorIdList = lstOperIDs;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PVP_CASTING_VOTE_OPERATOR_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_DRAFT_PVP_CASTING_VOTE_UNIT_REQ(List<int> lstUnitIDs)
		{
			NKMPacket_DRAFT_PVP_CASTING_VOTE_UNIT_REQ nKMPacket_DRAFT_PVP_CASTING_VOTE_UNIT_REQ = new NKMPacket_DRAFT_PVP_CASTING_VOTE_UNIT_REQ();
			nKMPacket_DRAFT_PVP_CASTING_VOTE_UNIT_REQ.unitIdList = lstUnitIDs;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DRAFT_PVP_CASTING_VOTE_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_DRAFT_PVP_CASTING_VOTE_SHIP_REQ(List<int> lstShipGroupIDs)
		{
			NKMPacket_DRAFT_PVP_CASTING_VOTE_SHIP_REQ nKMPacket_DRAFT_PVP_CASTING_VOTE_SHIP_REQ = new NKMPacket_DRAFT_PVP_CASTING_VOTE_SHIP_REQ();
			nKMPacket_DRAFT_PVP_CASTING_VOTE_SHIP_REQ.shipGroupIdList = lstShipGroupIDs;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DRAFT_PVP_CASTING_VOTE_SHIP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_EVENT_PASS_REQ(NKC_OPEN_WAIT_BOX_TYPE waitBoxType)
		{
			NKMPacket_EVENT_PASS_REQ packet = new NKMPacket_EVENT_PASS_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, waitBoxType);
		}

		public static void Send_NKMPacket_EVENT_PASS_LEVEL_COMPLETE_REQ()
		{
			NKMPacket_EVENT_PASS_LEVEL_COMPLETE_REQ packet = new NKMPacket_EVENT_PASS_LEVEL_COMPLETE_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_PASS_MISSION_REQ(EventPassMissionType eventPassMissionType)
		{
			NKMPacket_EVENT_PASS_MISSION_REQ nKMPacket_EVENT_PASS_MISSION_REQ = new NKMPacket_EVENT_PASS_MISSION_REQ();
			nKMPacket_EVENT_PASS_MISSION_REQ.missionType = eventPassMissionType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_PASS_MISSION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_REQ(EventPassMissionType eventPassMissionType)
		{
			NKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_REQ nKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_REQ = new NKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_REQ();
			nKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_REQ.missionType = eventPassMissionType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_cNKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_REQ(int retryMissionId)
		{
			NKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_REQ nKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_REQ = new NKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_REQ();
			nKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_REQ.missionId = retryMissionId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_REQ()
		{
			NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_REQ packet = new NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_REQ()
		{
			NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_REQ packet = new NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_PASS_LEVEL_UP_REQ(int increasedLevel)
		{
			NKMPacket_EVENT_PASS_LEVEL_UP_REQ nKMPacket_EVENT_PASS_LEVEL_UP_REQ = new NKMPacket_EVENT_PASS_LEVEL_UP_REQ();
			nKMPacket_EVENT_PASS_LEVEL_UP_REQ.increaseLv = increasedLevel;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_PASS_LEVEL_UP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_PRESET_LIST_REQ(NKC_OPEN_WAIT_BOX_TYPE waitBoxType)
		{
			NKMPacket_EQUIP_PRESET_LIST_REQ packet = new NKMPacket_EQUIP_PRESET_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, waitBoxType);
		}

		public static void Send_NKMPacket_EQUIP_PRESET_ADD_REQ(int value)
		{
			NKMPacket_EQUIP_PRESET_ADD_REQ nKMPacket_EQUIP_PRESET_ADD_REQ = new NKMPacket_EQUIP_PRESET_ADD_REQ();
			nKMPacket_EQUIP_PRESET_ADD_REQ.addPresetCount = value;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_PRESET_ADD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_PRESET_NAME_CHANGE_REQ(int presetIndex, string presetName)
		{
			NKMPacket_EQUIP_PRESET_CHANGE_NAME_REQ nKMPacket_EQUIP_PRESET_CHANGE_NAME_REQ = new NKMPacket_EQUIP_PRESET_CHANGE_NAME_REQ();
			nKMPacket_EQUIP_PRESET_CHANGE_NAME_REQ.presetIndex = presetIndex;
			nKMPacket_EQUIP_PRESET_CHANGE_NAME_REQ.newPresetName = presetName;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_PRESET_CHANGE_NAME_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_PRESET_REGISTER_ALL_REQ(int presetIndex, long unitUId)
		{
			NKMPacket_EQUIP_PRESET_REGISTER_ALL_REQ nKMPacket_EQUIP_PRESET_REGISTER_ALL_REQ = new NKMPacket_EQUIP_PRESET_REGISTER_ALL_REQ();
			nKMPacket_EQUIP_PRESET_REGISTER_ALL_REQ.presetIndex = presetIndex;
			nKMPacket_EQUIP_PRESET_REGISTER_ALL_REQ.unitUid = unitUId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_PRESET_REGISTER_ALL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_PRESET_REGISTER_REQ(int presetIndex, ITEM_EQUIP_POSITION equipPosition, long equipUId)
		{
			NKMPacket_EQUIP_PRESET_REGISTER_REQ nKMPacket_EQUIP_PRESET_REGISTER_REQ = new NKMPacket_EQUIP_PRESET_REGISTER_REQ();
			nKMPacket_EQUIP_PRESET_REGISTER_REQ.presetIndex = presetIndex;
			nKMPacket_EQUIP_PRESET_REGISTER_REQ.equipPosition = equipPosition;
			nKMPacket_EQUIP_PRESET_REGISTER_REQ.equipUid = equipUId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_PRESET_REGISTER_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_PRESET_APPLY_REQ(int presetIndex, long applyUnitUId)
		{
			NKMPacket_EQUIP_PRESET_APPLY_REQ nKMPacket_EQUIP_PRESET_APPLY_REQ = new NKMPacket_EQUIP_PRESET_APPLY_REQ();
			nKMPacket_EQUIP_PRESET_APPLY_REQ.presetIndex = presetIndex;
			nKMPacket_EQUIP_PRESET_APPLY_REQ.applyUnitUid = applyUnitUId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_PRESET_APPLY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ(List<NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ.PresetIndexData> changeIndices)
		{
			NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ nKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ = new NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ();
			if (changeIndices != null)
			{
				nKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ.changeIndices = changeIndices;
			}
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EQUIP_PRESET_CLEAR_REQ(List<int> lstTargetIndex)
		{
			NKMPacket_EQUIP_PRESET_CLEAR_REQ nKMPacket_EQUIP_PRESET_CLEAR_REQ = new NKMPacket_EQUIP_PRESET_CLEAR_REQ();
			nKMPacket_EQUIP_PRESET_CLEAR_REQ.presetIndices = lstTargetIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_PRESET_CLEAR_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPACKET_RACE_TEAM_SELECT_REQ(RaceTeam selectedTeam)
		{
			NKMPACKET_RACE_TEAM_SELECT_REQ nKMPACKET_RACE_TEAM_SELECT_REQ = new NKMPACKET_RACE_TEAM_SELECT_REQ();
			nKMPACKET_RACE_TEAM_SELECT_REQ.selectTeam = selectedTeam;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPACKET_RACE_TEAM_SELECT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPACKET_RACE_START_REQ(int selectedLine)
		{
			NKMPACKET_RACE_START_REQ nKMPACKET_RACE_START_REQ = new NKMPACKET_RACE_START_REQ();
			nKMPACKET_RACE_START_REQ.selectLine = selectedLine;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPACKET_RACE_START_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPACKET_EVENT_BET_SELECT_TEAM_REQ(EventBetTeam selectTeam, int betCount)
		{
			NKMPACKET_EVENT_BET_SELECT_TEAM_REQ nKMPACKET_EVENT_BET_SELECT_TEAM_REQ = new NKMPACKET_EVENT_BET_SELECT_TEAM_REQ();
			nKMPACKET_EVENT_BET_SELECT_TEAM_REQ.selectTeam = selectTeam;
			nKMPACKET_EVENT_BET_SELECT_TEAM_REQ.betCount = betCount;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPACKET_EVENT_BET_SELECT_TEAM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPACKET_EVENT_BET_BETTING_REQ(int betCount)
		{
			NKMPACKET_EVENT_BET_BETTING_REQ nKMPACKET_EVENT_BET_BETTING_REQ = new NKMPACKET_EVENT_BET_BETTING_REQ();
			nKMPACKET_EVENT_BET_BETTING_REQ.betCount = betCount;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPACKET_EVENT_BET_BETTING_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPACKET_EVENT_BET_RESULT_REQ()
		{
			NKMPACKET_EVENT_BET_RESULT_REQ packet = new NKMPACKET_EVENT_BET_RESULT_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_DUNGEON_SKIP_REQ(int dungeonId, List<long> lstUnits, int skip)
		{
			NKMPacket_DUNGEON_SKIP_REQ nKMPacket_DUNGEON_SKIP_REQ = new NKMPacket_DUNGEON_SKIP_REQ();
			nKMPacket_DUNGEON_SKIP_REQ.dungeonId = dungeonId;
			nKMPacket_DUNGEON_SKIP_REQ.skip = skip;
			nKMPacket_DUNGEON_SKIP_REQ.unitUids = lstUnits;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DUNGEON_SKIP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_OPEN_SECTION_REQ(int sectionId)
		{
			NKMPacket_OFFICE_OPEN_SECTION_REQ nKMPacket_OFFICE_OPEN_SECTION_REQ = new NKMPacket_OFFICE_OPEN_SECTION_REQ();
			nKMPacket_OFFICE_OPEN_SECTION_REQ.sectionId = sectionId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_OPEN_SECTION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_OPEN_ROOM_REQ(int roomId)
		{
			NKMPacket_OFFICE_OPEN_ROOM_REQ nKMPacket_OFFICE_OPEN_ROOM_REQ = new NKMPacket_OFFICE_OPEN_ROOM_REQ();
			nKMPacket_OFFICE_OPEN_ROOM_REQ.roomId = roomId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_OPEN_ROOM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_SET_ROOM_NAME_REQ(int roomId, string roomName)
		{
			NKMPacket_OFFICE_SET_ROOM_NAME_REQ nKMPacket_OFFICE_SET_ROOM_NAME_REQ = new NKMPacket_OFFICE_SET_ROOM_NAME_REQ();
			nKMPacket_OFFICE_SET_ROOM_NAME_REQ.roomId = roomId;
			nKMPacket_OFFICE_SET_ROOM_NAME_REQ.roomName = roomName;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_SET_ROOM_NAME_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_ROOM_SET_UNIT_REQ(int roomId, List<long> unitUId)
		{
			NKMPacket_OFFICE_SET_ROOM_UNIT_REQ nKMPacket_OFFICE_SET_ROOM_UNIT_REQ = new NKMPacket_OFFICE_SET_ROOM_UNIT_REQ();
			nKMPacket_OFFICE_SET_ROOM_UNIT_REQ.roomId = roomId;
			nKMPacket_OFFICE_SET_ROOM_UNIT_REQ.unitUids = unitUId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_SET_ROOM_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_SET_ROOM_WALL_REQ(int roomID, int interiorID)
		{
			NKMPacket_OFFICE_SET_ROOM_WALL_REQ nKMPacket_OFFICE_SET_ROOM_WALL_REQ = new NKMPacket_OFFICE_SET_ROOM_WALL_REQ();
			nKMPacket_OFFICE_SET_ROOM_WALL_REQ.roomId = roomID;
			nKMPacket_OFFICE_SET_ROOM_WALL_REQ.wallInteriorId = interiorID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_SET_ROOM_WALL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_SET_ROOM_FLOOR_REQ(int roomID, int interiorID)
		{
			NKMPacket_OFFICE_SET_ROOM_FLOOR_REQ nKMPacket_OFFICE_SET_ROOM_FLOOR_REQ = new NKMPacket_OFFICE_SET_ROOM_FLOOR_REQ();
			nKMPacket_OFFICE_SET_ROOM_FLOOR_REQ.roomId = roomID;
			nKMPacket_OFFICE_SET_ROOM_FLOOR_REQ.floorInteriorId = interiorID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_SET_ROOM_FLOOR_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_SET_ROOM_BACKGROUND_REQ(int roomID, int interiorID)
		{
			NKMPacket_OFFICE_SET_ROOM_BACKGROUND_REQ nKMPacket_OFFICE_SET_ROOM_BACKGROUND_REQ = new NKMPacket_OFFICE_SET_ROOM_BACKGROUND_REQ();
			nKMPacket_OFFICE_SET_ROOM_BACKGROUND_REQ.roomId = roomID;
			nKMPacket_OFFICE_SET_ROOM_BACKGROUND_REQ.backgroundId = interiorID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_SET_ROOM_BACKGROUND_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_ADD_FURNITURE_REQ(int roomId, int itemId, OfficePlaneType planeType, int positionX, int positionY, bool inverted)
		{
			NKMPacket_OFFICE_ADD_FURNITURE_REQ nKMPacket_OFFICE_ADD_FURNITURE_REQ = new NKMPacket_OFFICE_ADD_FURNITURE_REQ();
			nKMPacket_OFFICE_ADD_FURNITURE_REQ.roomId = roomId;
			nKMPacket_OFFICE_ADD_FURNITURE_REQ.itemId = itemId;
			nKMPacket_OFFICE_ADD_FURNITURE_REQ.planeType = planeType;
			nKMPacket_OFFICE_ADD_FURNITURE_REQ.positionX = positionX;
			nKMPacket_OFFICE_ADD_FURNITURE_REQ.positionY = positionY;
			nKMPacket_OFFICE_ADD_FURNITURE_REQ.inverted = inverted;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_ADD_FURNITURE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_OFFICE_UPDATE_FURNITURE_REQ(int roomId, long furnitureUid, OfficePlaneType planeType, int positionX, int positionY, bool inverted)
		{
			NKMPacket_OFFICE_UPDATE_FURNITURE_REQ nKMPacket_OFFICE_UPDATE_FURNITURE_REQ = new NKMPacket_OFFICE_UPDATE_FURNITURE_REQ();
			nKMPacket_OFFICE_UPDATE_FURNITURE_REQ.roomId = roomId;
			nKMPacket_OFFICE_UPDATE_FURNITURE_REQ.furnitureUid = furnitureUid;
			nKMPacket_OFFICE_UPDATE_FURNITURE_REQ.planeType = planeType;
			nKMPacket_OFFICE_UPDATE_FURNITURE_REQ.positionX = positionX;
			nKMPacket_OFFICE_UPDATE_FURNITURE_REQ.positionY = positionY;
			nKMPacket_OFFICE_UPDATE_FURNITURE_REQ.inverted = inverted;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_UPDATE_FURNITURE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_OFFICE_REMOVE_FURNITURE_REQ(int roomId, long furnitureUid)
		{
			NKMPacket_OFFICE_REMOVE_FURNITURE_REQ nKMPacket_OFFICE_REMOVE_FURNITURE_REQ = new NKMPacket_OFFICE_REMOVE_FURNITURE_REQ();
			nKMPacket_OFFICE_REMOVE_FURNITURE_REQ.roomId = roomId;
			nKMPacket_OFFICE_REMOVE_FURNITURE_REQ.furnitureUid = furnitureUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_REMOVE_FURNITURE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_OFFICE_CLEAR_ALL_FURNITURE_REQ(int roomId)
		{
			NKMPacket_OFFICE_CLEAR_ALL_FURNITURE_REQ nKMPacket_OFFICE_CLEAR_ALL_FURNITURE_REQ = new NKMPacket_OFFICE_CLEAR_ALL_FURNITURE_REQ();
			nKMPacket_OFFICE_CLEAR_ALL_FURNITURE_REQ.roomId = roomId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_CLEAR_ALL_FURNITURE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}

		public static void Send_NKMPacket_OFFICE_TAKE_HEART_REQ(long unitUid)
		{
			NKMPacket_OFFICE_TAKE_HEART_REQ nKMPacket_OFFICE_TAKE_HEART_REQ = new NKMPacket_OFFICE_TAKE_HEART_REQ();
			nKMPacket_OFFICE_TAKE_HEART_REQ.unitUid = unitUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_TAKE_HEART_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_OFFICE_STATE_REQ(long userUId)
		{
			NKMPacket_OFFICE_STATE_REQ nKMPacket_OFFICE_STATE_REQ = new NKMPacket_OFFICE_STATE_REQ();
			nKMPacket_OFFICE_STATE_REQ.userUid = userUId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_STATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_POST_SEND_REQ(long receiverUserUid)
		{
			NKMPacket_OFFICE_POST_SEND_REQ nKMPacket_OFFICE_POST_SEND_REQ = new NKMPacket_OFFICE_POST_SEND_REQ();
			nKMPacket_OFFICE_POST_SEND_REQ.receiverUserUid = receiverUserUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_POST_SEND_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_POST_LIST_REQ(long lastPostUid = 0L)
		{
			NKMPacket_OFFICE_POST_LIST_REQ nKMPacket_OFFICE_POST_LIST_REQ = new NKMPacket_OFFICE_POST_LIST_REQ();
			nKMPacket_OFFICE_POST_LIST_REQ.lastPostUid = lastPostUid;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_POST_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_POST_RECV_REQ()
		{
			NKMPacket_OFFICE_POST_RECV_REQ packet = new NKMPacket_OFFICE_POST_RECV_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_POST_BROADCAST_REQ()
		{
			NKMPacket_OFFICE_POST_BROADCAST_REQ packet = new NKMPacket_OFFICE_POST_BROADCAST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_RANDOM_VISIT_REQ()
		{
			NKMPacket_OFFICE_RANDOM_VISIT_REQ packet = new NKMPacket_OFFICE_RANDOM_VISIT_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_PARTY_REQ(int roomID)
		{
			NKMPacket_OFFICE_PARTY_REQ nKMPacket_OFFICE_PARTY_REQ = new NKMPacket_OFFICE_PARTY_REQ();
			nKMPacket_OFFICE_PARTY_REQ.roomId = roomID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_PARTY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_PRESET_REGISTER_REQ(int roomID, int presetID)
		{
			NKMPacket_OFFICE_PRESET_REGISTER_REQ nKMPacket_OFFICE_PRESET_REGISTER_REQ = new NKMPacket_OFFICE_PRESET_REGISTER_REQ();
			nKMPacket_OFFICE_PRESET_REGISTER_REQ.roomId = roomID;
			nKMPacket_OFFICE_PRESET_REGISTER_REQ.presetId = presetID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_PRESET_REGISTER_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_PRESET_APPLY_REQ(int roomID, int presetID)
		{
			NKMPacket_OFFICE_PRESET_APPLY_REQ nKMPacket_OFFICE_PRESET_APPLY_REQ = new NKMPacket_OFFICE_PRESET_APPLY_REQ();
			nKMPacket_OFFICE_PRESET_APPLY_REQ.roomId = roomID;
			nKMPacket_OFFICE_PRESET_APPLY_REQ.presetId = presetID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_PRESET_APPLY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_PRESET_ADD_REQ(int addCount)
		{
			NKMPacket_OFFICE_PRESET_ADD_REQ nKMPacket_OFFICE_PRESET_ADD_REQ = new NKMPacket_OFFICE_PRESET_ADD_REQ();
			nKMPacket_OFFICE_PRESET_ADD_REQ.addPresetCount = addCount;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_PRESET_ADD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_PRESET_RESET_REQ(int presetID)
		{
			NKMPacket_OFFICE_PRESET_RESET_REQ nKMPacket_OFFICE_PRESET_RESET_REQ = new NKMPacket_OFFICE_PRESET_RESET_REQ();
			nKMPacket_OFFICE_PRESET_RESET_REQ.presetId = presetID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_PRESET_RESET_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_PRESET_CHANGE_NAME_REQ(int presetID, string name)
		{
			NKMPacket_OFFICE_PRESET_CHANGE_NAME_REQ nKMPacket_OFFICE_PRESET_CHANGE_NAME_REQ = new NKMPacket_OFFICE_PRESET_CHANGE_NAME_REQ();
			nKMPacket_OFFICE_PRESET_CHANGE_NAME_REQ.newPresetName = name;
			nKMPacket_OFFICE_PRESET_CHANGE_NAME_REQ.presetId = presetID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_PRESET_CHANGE_NAME_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_OFFICE_PRESET_APPLY_THEMA_REQ(int roomID, int themeID)
		{
			NKMPacket_OFFICE_PRESET_APPLY_THEMA_REQ nKMPacket_OFFICE_PRESET_APPLY_THEMA_REQ = new NKMPacket_OFFICE_PRESET_APPLY_THEMA_REQ();
			nKMPacket_OFFICE_PRESET_APPLY_THEMA_REQ.roomId = roomID;
			nKMPacket_OFFICE_PRESET_APPLY_THEMA_REQ.themaIndex = themeID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_OFFICE_PRESET_APPLY_THEMA_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_RECALL_UNIT_REQ(long sourceUnitUID, Dictionary<int, int> targetUnitIDs)
		{
			NKMPacket_RECALL_UNIT_REQ nKMPacket_RECALL_UNIT_REQ = new NKMPacket_RECALL_UNIT_REQ();
			nKMPacket_RECALL_UNIT_REQ.recallUnitUid = sourceUnitUID;
			nKMPacket_RECALL_UNIT_REQ.exchangeUnitList = targetUnitIDs;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_RECALL_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_KAKAO_MISSION_REFRESH_STATE_REQ(int eventID)
		{
			NKMPacket_KAKAO_MISSION_REFRESH_STATE_REQ nKMPacket_KAKAO_MISSION_REFRESH_STATE_REQ = new NKMPacket_KAKAO_MISSION_REFRESH_STATE_REQ();
			nKMPacket_KAKAO_MISSION_REFRESH_STATE_REQ.eventId = eventID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_KAKAO_MISSION_REFRESH_STATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_WECHAT_COUPON_CHECK_REQ(int eventTabTempletId, NKC_OPEN_WAIT_BOX_TYPE eNKC_OPEN_WAIT_BOX_TYPE)
		{
			int result = 1;
			if (int.TryParse(NKCDownloadConfig.s_ServerID, out result))
			{
				NKMPacket_WECHAT_COUPON_CHECK_REQ nKMPacket_WECHAT_COUPON_CHECK_REQ = new NKMPacket_WECHAT_COUPON_CHECK_REQ();
				nKMPacket_WECHAT_COUPON_CHECK_REQ.templetId = eventTabTempletId;
				nKMPacket_WECHAT_COUPON_CHECK_REQ.zlongServerId = result;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WECHAT_COUPON_CHECK_REQ, eNKC_OPEN_WAIT_BOX_TYPE);
			}
		}

		public static void Send_NKMPacket_WECHAT_COUPON_REWARD_REQ(int eventTabTempletId)
		{
			NKMPacket_WECHAT_COUPON_REWARD_REQ nKMPacket_WECHAT_COUPON_REWARD_REQ = new NKMPacket_WECHAT_COUPON_REWARD_REQ();
			nKMPacket_WECHAT_COUPON_REWARD_REQ.templetId = eventTabTempletId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WECHAT_COUPON_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_KILL_COUNT_USER_REWARD_REQ(int templetId, int stepId)
		{
			NKMPacket_KILL_COUNT_USER_REWARD_REQ nKMPacket_KILL_COUNT_USER_REWARD_REQ = new NKMPacket_KILL_COUNT_USER_REWARD_REQ();
			nKMPacket_KILL_COUNT_USER_REWARD_REQ.templetId = templetId;
			nKMPacket_KILL_COUNT_USER_REWARD_REQ.stepId = stepId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_KILL_COUNT_USER_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_KILL_COUNT_SERVER_REWARD_REQ(int templetId, int stepId)
		{
			NKMPacket_KILL_COUNT_SERVER_REWARD_REQ nKMPacket_KILL_COUNT_SERVER_REWARD_REQ = new NKMPacket_KILL_COUNT_SERVER_REWARD_REQ();
			nKMPacket_KILL_COUNT_SERVER_REWARD_REQ.templetId = templetId;
			nKMPacket_KILL_COUNT_SERVER_REWARD_REQ.stepId = stepId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_KILL_COUNT_SERVER_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EXTRACT_UNIT_REQ(List<long> lstExtractUnits)
		{
			NKMPacket_EXTRACT_UNIT_REQ nKMPacket_EXTRACT_UNIT_REQ = new NKMPacket_EXTRACT_UNIT_REQ();
			nKMPacket_EXTRACT_UNIT_REQ.extractUnitUidList = lstExtractUnits;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EXTRACT_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_REARMAMENT_UNIT_REQ(long lResourceUnitUID, int iRearmUnitID)
		{
			NKMPacket_REARMAMENT_UNIT_REQ nKMPacket_REARMAMENT_UNIT_REQ = new NKMPacket_REARMAMENT_UNIT_REQ();
			nKMPacket_REARMAMENT_UNIT_REQ.unitUid = lResourceUnitUID;
			nKMPacket_REARMAMENT_UNIT_REQ.rearmamentId = iRearmUnitID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_REARMAMENT_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_UNIT_MISSION_REWARD_REQ(int unitId, int missionId, int stepId)
		{
			NKMPacket_UNIT_MISSION_REWARD_REQ nKMPacket_UNIT_MISSION_REWARD_REQ = new NKMPacket_UNIT_MISSION_REWARD_REQ();
			nKMPacket_UNIT_MISSION_REWARD_REQ.unitId = unitId;
			nKMPacket_UNIT_MISSION_REWARD_REQ.missionId = missionId;
			nKMPacket_UNIT_MISSION_REWARD_REQ.stepId = stepId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_MISSION_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_UNIT_MISSION_REWARD_ALL_REQ(int unitId)
		{
			NKMPacket_UNIT_MISSION_REWARD_ALL_REQ nKMPacket_UNIT_MISSION_REWARD_ALL_REQ = new NKMPacket_UNIT_MISSION_REWARD_ALL_REQ();
			nKMPacket_UNIT_MISSION_REWARD_ALL_REQ.unitId = unitId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_MISSION_REWARD_ALL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MISC_COLLECTION_REWARD_REQ(int miscId)
		{
			NKMPacket_MISC_COLLECTION_REWARD_REQ nKMPacket_MISC_COLLECTION_REWARD_REQ = new NKMPacket_MISC_COLLECTION_REWARD_REQ();
			nKMPacket_MISC_COLLECTION_REWARD_REQ.miscId = miscId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MISC_COLLECTION_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void NKMPacket_MISC_COLLECTION_REWARD_ALL_REQ(NKM_ITEM_MISC_TYPE miscType)
		{
			NKMPacket_MISC_COLLECTION_REWARD_ALL_REQ nKMPacket_MISC_COLLECTION_REWARD_ALL_REQ = new NKMPacket_MISC_COLLECTION_REWARD_ALL_REQ();
			nKMPacket_MISC_COLLECTION_REWARD_ALL_REQ.miscType = miscType;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MISC_COLLECTION_REWARD_ALL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_BAR_CREATE_COCKTAIL_REQ(int cocktailItemId, int count)
		{
			NKMPacket_EVENT_BAR_CREATE_COCKTAIL_REQ nKMPacket_EVENT_BAR_CREATE_COCKTAIL_REQ = new NKMPacket_EVENT_BAR_CREATE_COCKTAIL_REQ();
			nKMPacket_EVENT_BAR_CREATE_COCKTAIL_REQ.cocktailItemId = cocktailItemId;
			nKMPacket_EVENT_BAR_CREATE_COCKTAIL_REQ.count = count;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_BAR_CREATE_COCKTAIL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_BAR_GET_REWARD_REQ(int cocktailItemId)
		{
			NKMPacket_EVENT_BAR_GET_REWARD_REQ nKMPacket_EVENT_BAR_GET_REWARD_REQ = new NKMPacket_EVENT_BAR_GET_REWARD_REQ();
			nKMPacket_EVENT_BAR_GET_REWARD_REQ.cocktailItemId = cocktailItemId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_BAR_GET_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static bool Send_NKMPacket_AD_ITEM_REWARD_REQ(int adItemId)
		{
			NKMPacket_AD_ITEM_REWARD_REQ nKMPacket_AD_ITEM_REWARD_REQ = new NKMPacket_AD_ITEM_REWARD_REQ();
			nKMPacket_AD_ITEM_REWARD_REQ.aditemId = adItemId;
			return NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_AD_ITEM_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static bool Send_NKMPacket_AD_INVENTORY_EXPAND_REQ(NKM_INVENTORY_EXPAND_TYPE inventoryType)
		{
			NKMPacket_AD_INVENTORY_EXPAND_REQ nKMPacket_AD_INVENTORY_EXPAND_REQ = new NKMPacket_AD_INVENTORY_EXPAND_REQ();
			nKMPacket_AD_INVENTORY_EXPAND_REQ.inventoryExpandType = inventoryType;
			return NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_AD_INVENTORY_EXPAND_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_BSIDE_COUPON_USE_REQ(string code)
		{
			NKMPacket_BSIDE_COUPON_USE_REQ nKMPacket_BSIDE_COUPON_USE_REQ = new NKMPacket_BSIDE_COUPON_USE_REQ();
			nKMPacket_BSIDE_COUPON_USE_REQ.couponCode = code;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_BSIDE_COUPON_USE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_ASYNC_PVP_RANK_LIST_REQ(RANK_TYPE type, bool all)
		{
			NKMPacket_ASYNC_PVP_RANK_LIST_REQ nKMPacket_ASYNC_PVP_RANK_LIST_REQ = new NKMPacket_ASYNC_PVP_RANK_LIST_REQ();
			nKMPacket_ASYNC_PVP_RANK_LIST_REQ.rankType = type;
			nKMPacket_ASYNC_PVP_RANK_LIST_REQ.isAll = all;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_ASYNC_PVP_RANK_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_ASYNC_PVP_TARGET_LIST_REQ()
		{
			NKMPacket_ASYNC_PVP_TARGET_LIST_REQ packet = new NKMPacket_ASYNC_PVP_TARGET_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_REVENGE_PVP_TARGET_LIST_REQ()
		{
			NKMPacket_REVENGE_PVP_TARGET_LIST_REQ packet = new NKMPacket_REVENGE_PVP_TARGET_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_NPC_PVP_TARGET_LIST_REQ(int _iTargetTier)
		{
			NKMPacket_NPC_PVP_TARGET_LIST_REQ nKMPacket_NPC_PVP_TARGET_LIST_REQ = new NKMPacket_NPC_PVP_TARGET_LIST_REQ();
			nKMPacket_NPC_PVP_TARGET_LIST_REQ.targetTier = _iTargetTier;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_NPC_PVP_TARGET_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TRIM_START_REQ(int trimId, int trimLevel, List<NKMEventDeckData> eventDeckList)
		{
			NKMPacket_TRIM_START_REQ nKMPacket_TRIM_START_REQ = new NKMPacket_TRIM_START_REQ();
			nKMPacket_TRIM_START_REQ.trimId = trimId;
			nKMPacket_TRIM_START_REQ.trimLevel = trimLevel;
			nKMPacket_TRIM_START_REQ.eventDeckList = eventDeckList;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TRIM_START_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TRIM_RETRY_REQ()
		{
			NKMPacket_TRIM_RETRY_REQ packet = new NKMPacket_TRIM_RETRY_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TRIM_RESTORE_REQ(int trimIntervalId)
		{
			NKMPacket_TRIM_RESTORE_REQ nKMPacket_TRIM_RESTORE_REQ = new NKMPacket_TRIM_RESTORE_REQ();
			nKMPacket_TRIM_RESTORE_REQ.trimIntervalId = trimIntervalId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TRIM_RESTORE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TRIM_END_REQ(int trimId)
		{
			NKMPacket_TRIM_END_REQ nKMPacket_TRIM_END_REQ = new NKMPacket_TRIM_END_REQ();
			nKMPacket_TRIM_END_REQ.trimId = trimId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TRIM_END_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TRIM_DUNGEON_SKIP_REQ(int trimId, int trimLevel, int skipCount, List<NKMEventDeckData> eventDeckList)
		{
			NKMPacket_TRIM_DUNGEON_SKIP_REQ nKMPacket_TRIM_DUNGEON_SKIP_REQ = new NKMPacket_TRIM_DUNGEON_SKIP_REQ();
			nKMPacket_TRIM_DUNGEON_SKIP_REQ.trimId = trimId;
			nKMPacket_TRIM_DUNGEON_SKIP_REQ.trimLevel = trimLevel;
			nKMPacket_TRIM_DUNGEON_SKIP_REQ.skipCount = skipCount;
			nKMPacket_TRIM_DUNGEON_SKIP_REQ.eventDeckList = eventDeckList;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TRIM_DUNGEON_SKIP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_EVENT_COLLECTION_MERGE_REQ(int mergeID, int groupID, List<long> lstConsumeUids)
		{
			NKMPacket_EVENT_COLLECTION_MERGE_REQ nKMPacket_EVENT_COLLECTION_MERGE_REQ = new NKMPacket_EVENT_COLLECTION_MERGE_REQ();
			nKMPacket_EVENT_COLLECTION_MERGE_REQ.collectionMergeId = mergeID;
			nKMPacket_EVENT_COLLECTION_MERGE_REQ.mergeRecipeGroupId = groupID;
			nKMPacket_EVENT_COLLECTION_MERGE_REQ.consumeTrophyUids = lstConsumeUids;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EVENT_COLLECTION_MERGE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_ACCOUNT_UNLINK_REQ()
		{
			NKMPacket_ACCOUNT_UNLINK_REQ packet = new NKMPacket_ACCOUNT_UNLINK_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_UNIT_TACTIC_UPDATE_REQ(long unitUID, List<long> lstConsumeUnitUID)
		{
			NKMPacket_UNIT_TACTIC_UPDATE_REQ nKMPacket_UNIT_TACTIC_UPDATE_REQ = new NKMPacket_UNIT_TACTIC_UPDATE_REQ();
			nKMPacket_UNIT_TACTIC_UPDATE_REQ.unitUid = unitUID;
			nKMPacket_UNIT_TACTIC_UPDATE_REQ.consumeUnitUids = lstConsumeUnitUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_TACTIC_UPDATE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_DEFENCE_INFO_REQ(int defenceTempletId)
		{
			NKMPacket_DEFENCE_INFO_REQ nKMPacket_DEFENCE_INFO_REQ = new NKMPacket_DEFENCE_INFO_REQ();
			nKMPacket_DEFENCE_INFO_REQ.defenceTempletId = defenceTempletId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DEFENCE_INFO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_DEFENCE_GAME_START_REQ(int defenceDungeonId, NKMEventDeckData eventDeckData)
		{
			NKMPacket_DEFENCE_GAME_START_REQ nKMPacket_DEFENCE_GAME_START_REQ = new NKMPacket_DEFENCE_GAME_START_REQ();
			nKMPacket_DEFENCE_GAME_START_REQ.defenceTempletId = defenceDungeonId;
			nKMPacket_DEFENCE_GAME_START_REQ.eventDeckData = eventDeckData;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DEFENCE_GAME_START_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_LEADERBOARD_DEFENCE_LIST_REQ(bool isAll)
		{
			NKMPacket_LEADERBOARD_DEFENCE_LIST_REQ nKMPacket_LEADERBOARD_DEFENCE_LIST_REQ = new NKMPacket_LEADERBOARD_DEFENCE_LIST_REQ();
			NKMDefenceTemplet currentDefenceDungeonTemplet = NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Now);
			if (currentDefenceDungeonTemplet == null)
			{
				Log.Error("[DefenceDungeon] 현재 시간대 활성화된 NKMDefenceTemplet 이 없음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketSender.cs", 3650);
				return;
			}
			nKMPacket_LEADERBOARD_DEFENCE_LIST_REQ.defenceId = currentDefenceDungeonTemplet.Key;
			nKMPacket_LEADERBOARD_DEFENCE_LIST_REQ.isAll = isAll;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LEADERBOARD_DEFENCE_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_DEFENCE_PROFILE_REQ(long userUid, bool isForce)
		{
			NKMPacket_DEFENCE_PROFILE_REQ nKMPacket_DEFENCE_PROFILE_REQ = new NKMPacket_DEFENCE_PROFILE_REQ();
			nKMPacket_DEFENCE_PROFILE_REQ.userUid = userUid;
			nKMPacket_DEFENCE_PROFILE_REQ.isForce = isForce;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DEFENCE_PROFILE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_DEFENCE_RANK_REWARD_REQ()
		{
			NKMPacket_DEFENCE_RANK_REWARD_REQ packet = new NKMPacket_DEFENCE_RANK_REWARD_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_DEFENCE_SCORE_REWARD_REQ(int scoreRewardId)
		{
			NKMPacket_DEFENCE_SCORE_REWARD_REQ nKMPacket_DEFENCE_SCORE_REWARD_REQ = new NKMPacket_DEFENCE_SCORE_REWARD_REQ();
			nKMPacket_DEFENCE_SCORE_REWARD_REQ.scoreRewardId = scoreRewardId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DEFENCE_SCORE_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_DEFENCE_SCORE_REWARD_ALL_REQ()
		{
			NKMPacket_DEFENCE_SCORE_REWARD_ALL_REQ packet = new NKMPacket_DEFENCE_SCORE_REWARD_ALL_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_UNIT_REACTOR_LEVELUP_REQ(long unitUID)
		{
			NKMPacket_UNIT_REACTOR_LEVELUP_REQ nKMPacket_UNIT_REACTOR_LEVELUP_REQ = new NKMPacket_UNIT_REACTOR_LEVELUP_REQ();
			nKMPacket_UNIT_REACTOR_LEVELUP_REQ.unitUid = unitUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UNIT_REACTOR_LEVELUP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_UPDATE_TITLE_REQ(int titleId)
		{
			NKMPacket_UPDATE_TITLE_REQ nKMPacket_UPDATE_TITLE_REQ = new NKMPacket_UPDATE_TITLE_REQ();
			nKMPacket_UPDATE_TITLE_REQ.titleId = titleId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_UPDATE_TITLE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_UPDATE_BIRTHDAY_REQ(BirthDayDate birthDay)
		{
			NKMPacket_ACCOUNT_UPDATE_BIRTHDAY_REQ nKMPacket_ACCOUNT_UPDATE_BIRTHDAY_REQ = new NKMPacket_ACCOUNT_UPDATE_BIRTHDAY_REQ();
			nKMPacket_ACCOUNT_UPDATE_BIRTHDAY_REQ.birthDay = birthDay;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_ACCOUNT_UPDATE_BIRTHDAY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_JUKEBOX_CHANGE_BGM_REQ(NKM_BGM_TYPE bgmType, int bgmId)
		{
			NKMPacket_JUKEBOX_CHANGE_BGM_REQ nKMPacket_JUKEBOX_CHANGE_BGM_REQ = new NKMPacket_JUKEBOX_CHANGE_BGM_REQ();
			nKMPacket_JUKEBOX_CHANGE_BGM_REQ.bgmType = bgmType;
			nKMPacket_JUKEBOX_CHANGE_BGM_REQ.bgmId = bgmId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_JUKEBOX_CHANGE_BGM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}

		public static void Send_NKMPacket_START_SIMULATED_PVP_TEST_REQ(long playerUserUidA, long playerUserUidB)
		{
			NKMPacket_START_SIMULATED_PVP_TEST_REQ nKMPacket_START_SIMULATED_PVP_TEST_REQ = new NKMPacket_START_SIMULATED_PVP_TEST_REQ();
			nKMPacket_START_SIMULATED_PVP_TEST_REQ.playerUserUidA = playerUserUidA;
			nKMPacket_START_SIMULATED_PVP_TEST_REQ.playerUserUidB = playerUserUidB;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_START_SIMULATED_PVP_TEST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_REQ(int tournamentId)
		{
			NKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_REQ nKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_REQ = new NKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_REQ();
			nKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_REQ.templetId = tournamentId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TOURNAMENT_PREDICTION_REQ(int tournamentId, NKMTournamentGroups group, List<long> slotUserUId)
		{
			NKMPacket_TOURNAMENT_PREDICTION_REQ nKMPacket_TOURNAMENT_PREDICTION_REQ = new NKMPacket_TOURNAMENT_PREDICTION_REQ();
			nKMPacket_TOURNAMENT_PREDICTION_REQ.templetId = tournamentId;
			nKMPacket_TOURNAMENT_PREDICTION_REQ.groupIndex = group;
			nKMPacket_TOURNAMENT_PREDICTION_REQ.slotUserUid = slotUserUId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TOURNAMENT_PREDICTION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TOURNAMENT_PREDICTION_STATISTICS_REQ(int tournamentId, NKMTournamentGroups group)
		{
			NKMPacket_TOURNAMENT_PREDICTION_STATISTICS_REQ nKMPacket_TOURNAMENT_PREDICTION_STATISTICS_REQ = new NKMPacket_TOURNAMENT_PREDICTION_STATISTICS_REQ();
			nKMPacket_TOURNAMENT_PREDICTION_STATISTICS_REQ.templetId = tournamentId;
			nKMPacket_TOURNAMENT_PREDICTION_STATISTICS_REQ.groupIndex = group;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TOURNAMENT_PREDICTION_STATISTICS_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TOURNAMENT_INFO_REQ()
		{
			NKMPacket_TOURNAMENT_INFO_REQ packet = new NKMPacket_TOURNAMENT_INFO_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TOURNAMENT_APPLY_REQ(NKMDeckData deckData)
		{
			NKMPacket_TOURNAMENT_APPLY_REQ nKMPacket_TOURNAMENT_APPLY_REQ = new NKMPacket_TOURNAMENT_APPLY_REQ();
			nKMPacket_TOURNAMENT_APPLY_REQ.deck = deckData;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TOURNAMENT_APPLY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_kNKMPacket_TOURNAMENT_REPLAY_LINK_REQ(int tournamentId, NKMTournamentGroups groupIndex, int slotIndex)
		{
			NKMPacket_TOURNAMENT_REPLAY_LINK_REQ nKMPacket_TOURNAMENT_REPLAY_LINK_REQ = new NKMPacket_TOURNAMENT_REPLAY_LINK_REQ();
			nKMPacket_TOURNAMENT_REPLAY_LINK_REQ.tournamentId = tournamentId;
			nKMPacket_TOURNAMENT_REPLAY_LINK_REQ.groupIndex = groupIndex;
			nKMPacket_TOURNAMENT_REPLAY_LINK_REQ.slotIndex = slotIndex;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TOURNAMENT_REPLAY_LINK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TOURNAMENT_REWARD_REQ()
		{
			NKMPacket_TOURNAMENT_REWARD_REQ nKMPacket_TOURNAMENT_REWARD_REQ = new NKMPacket_TOURNAMENT_REWARD_REQ();
			nKMPacket_TOURNAMENT_REWARD_REQ.templetId = NKCTournamentManager.m_TournamentTemplet.Key;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TOURNAMENT_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TOURNAMENT_REWARD_INFO_REQ()
		{
			NKMPacket_TOURNAMENT_REWARD_INFO_REQ nKMPacket_TOURNAMENT_REWARD_INFO_REQ = new NKMPacket_TOURNAMENT_REWARD_INFO_REQ();
			nKMPacket_TOURNAMENT_REWARD_INFO_REQ.templetId = NKCTournamentManager.m_TournamentTemplet.Key;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TOURNAMENT_REWARD_INFO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TOURNAMENT_RANK_REQ()
		{
			NKMPacket_TOURNAMENT_RANK_REQ packet = new NKMPacket_TOURNAMENT_RANK_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_REQ(List<int> unitIdList)
		{
			NKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_REQ nKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_REQ = new NKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_REQ();
			nKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_REQ.tournamentId = NKCTournamentManager.TournamentId;
			nKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_REQ.unitIdList = unitIdList;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_REQ(List<int> shipIdList)
		{
			NKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_REQ nKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_REQ = new NKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_REQ();
			nKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_REQ.tournamentId = NKCTournamentManager.TournamentId;
			nKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_REQ.shipGroupIdList = shipIdList;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPACKET_EVENT_TEN_RESULT_REQ(int newScore, int newRemainTime)
		{
			NKMPACKET_EVENT_TEN_RESULT_REQ nKMPACKET_EVENT_TEN_RESULT_REQ = new NKMPACKET_EVENT_TEN_RESULT_REQ();
			nKMPACKET_EVENT_TEN_RESULT_REQ.newScore = newScore;
			nKMPACKET_EVENT_TEN_RESULT_REQ.newRemainTime = newRemainTime;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPACKET_EVENT_TEN_RESULT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPACKET_EVENT_TEN_REWARD_REQ(int rewardId)
		{
			NKMPACKET_EVENT_TEN_REWARD_REQ nKMPACKET_EVENT_TEN_REWARD_REQ = new NKMPACKET_EVENT_TEN_REWARD_REQ();
			nKMPACKET_EVENT_TEN_REWARD_REQ.rewardId = rewardId;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPACKET_EVENT_TEN_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPACKET_EVENT_TEN_REWARD_ALL_REQ()
		{
			NKMPACKET_EVENT_TEN_REWARD_ALL_REQ packet = new NKMPACKET_EVENT_TEN_REWARD_ALL_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SET_MY_SUPPORT_UNIT_REQ(long unitUID)
		{
			NKMPacket_SET_MY_SUPPORT_UNIT_REQ nKMPacket_SET_MY_SUPPORT_UNIT_REQ = new NKMPacket_SET_MY_SUPPORT_UNIT_REQ();
			nKMPacket_SET_MY_SUPPORT_UNIT_REQ.unitUid = unitUID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SET_MY_SUPPORT_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SUPPORT_UNIT_LIST_REQ()
		{
			NKMPacket_SUPPORT_UNIT_LIST_REQ packet = new NKMPacket_SUPPORT_UNIT_LIST_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_SET_DUNGEON_SUPPORT_UNIT_REQ(NKMDungeonSupportData data)
		{
			NKMPacket_SET_DUNGEON_SUPPORT_UNIT_REQ nKMPacket_SET_DUNGEON_SUPPORT_UNIT_REQ = new NKMPacket_SET_DUNGEON_SUPPORT_UNIT_REQ();
			nKMPacket_SET_DUNGEON_SUPPORT_UNIT_REQ.selectUnitData = data;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SET_DUNGEON_SUPPORT_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MINI_GAME_INFO_REQ(NKM_MINI_GAME_TYPE gameType, int templetID)
		{
			NKMPacket_MINI_GAME_INFO_REQ nKMPacket_MINI_GAME_INFO_REQ = new NKMPacket_MINI_GAME_INFO_REQ();
			nKMPacket_MINI_GAME_INFO_REQ.miniGameType = gameType;
			nKMPacket_MINI_GAME_INFO_REQ.templetId = templetID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MINI_GAME_INFO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MINI_GAME_RESULT_REQ(NKMMiniGameData miniGameData)
		{
			NKMPacket_MINI_GAME_RESULT_REQ nKMPacket_MINI_GAME_RESULT_REQ = new NKMPacket_MINI_GAME_RESULT_REQ();
			nKMPacket_MINI_GAME_RESULT_REQ.miniGameData = miniGameData;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MINI_GAME_RESULT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MINI_GAME_REWARD_REQ(int templetID, int rewardID)
		{
			NKMPacket_MINI_GAME_REWARD_REQ nKMPacket_MINI_GAME_REWARD_REQ = new NKMPacket_MINI_GAME_REWARD_REQ();
			nKMPacket_MINI_GAME_REWARD_REQ.templetId = templetID;
			nKMPacket_MINI_GAME_REWARD_REQ.rewardId = rewardID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MINI_GAME_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}

		public static void Send_NKMPacket_MINI_GAME_REWARD_ALL_REQ(int templetID)
		{
			NKMPacket_MINI_GAME_REWARD_ALL_REQ nKMPacket_MINI_GAME_REWARD_ALL_REQ = new NKMPacket_MINI_GAME_REWARD_ALL_REQ();
			nKMPacket_MINI_GAME_REWARD_ALL_REQ.templetId = templetID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_MINI_GAME_REWARD_ALL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}
