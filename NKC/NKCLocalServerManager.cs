using ClientPacket.Game;
using ClientPacket.User;
using Cs.Protocol;
using NKC.PacketHandler;
using NKM;
using NKM.Templet;
using Protocol;

namespace NKC;

public class NKCLocalServerManager
{
	private static NKCGameServerLocal m_NKCGameServerLocal = new NKCGameServerLocal();

	private static long m_GameUIDIndex = 0L;

	public static NKMOperator s_NKMOperatorTeamA_ForDev = null;

	private static float updateTime = 0f;

	private const float TargetFrameDelta = 1f / 30f;

	public static NKCGameServerLocal GetGameServerLocal()
	{
		return m_NKCGameServerLocal;
	}

	public static long GetGameUIDIndex()
	{
		return m_GameUIDIndex++;
	}

	public static void Update(float fDeltaTime)
	{
		if (m_NKCGameServerLocal.GetGameData() != null && (int)m_NKCGameServerLocal.GetGameRuntimeData().m_NKM_GAME_STATE >= 2 && (int)m_NKCGameServerLocal.GetGameRuntimeData().m_NKM_GAME_STATE <= 4)
		{
			updateTime += fDeltaTime;
			while (updateTime >= 1f / 30f)
			{
				updateTime -= 1f / 30f;
				UpdateInner(1f / 30f);
			}
		}
	}

	public static void UpdateInner(float fDeltaTime)
	{
		switch (m_NKCGameServerLocal.GetGameRuntimeData().m_NKM_GAME_SPEED_TYPE)
		{
		case NKM_GAME_SPEED_TYPE.NGST_1:
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			break;
		case NKM_GAME_SPEED_TYPE.NGST_2:
			m_NKCGameServerLocal.Update(fDeltaTime * 1.5f);
			break;
		case NKM_GAME_SPEED_TYPE.NGST_3:
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			break;
		case NKM_GAME_SPEED_TYPE.NGST_10:
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			m_NKCGameServerLocal.Update(fDeltaTime * 1.1f);
			break;
		case NKM_GAME_SPEED_TYPE.NGST_05:
			m_NKCGameServerLocal.Update(fDeltaTime * 0.6f);
			break;
		default:
			m_NKCGameServerLocal.Update(fDeltaTime);
			break;
		}
	}

	public static bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		if (cNKCMessageData.m_NKC_EVENT_MESSAGE == NKC_EVENT_MESSAGE.NEM_NKCPACKET_SEND_TO_SERVER)
		{
			switch ((ClientPacketId)(ushort)cNKCMessageData.m_MsgID2)
			{
			case ClientPacketId.kNKMPacket_GAME_LOAD_REQ:
				OnRecv((NKMPacket_GAME_LOAD_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_DEV_GAME_LOAD_REQ:
				OnRecv((NKMPacket_DEV_GAME_LOAD_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_PRACTICE_GAME_LOAD_REQ:
				OnRecv((NKMPacket_PRACTICE_GAME_LOAD_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_LOAD_COMPLETE_REQ:
				OnRecv((NKMPacket_GAME_LOAD_COMPLETE_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_RESPAWN_REQ:
				OnRecv((NKMPacket_GAME_RESPAWN_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_UNIT_RETREAT_REQ:
				OnRecv((NKMPacket_GAME_UNIT_RETREAT_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_SHIP_SKILL_REQ:
				OnRecv((NKMPacket_GAME_SHIP_SKILL_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_TACTICAL_COMMAND_REQ:
				OnRecv((NKMPacket_GAME_TACTICAL_COMMAND_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_AUTO_RESPAWN_REQ:
				OnRecv((NKMPacket_GAME_AUTO_RESPAWN_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_OPTION_CHANGE_REQ:
				OnRecv((NKMPacket_GAME_OPTION_CHANGE_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_RESPAWN_REQ:
				OnRecv((NKMPacket_GAME_DEV_RESPAWN_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_SHIP_CHANGE_REQ:
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_DECK_CHANGE_REQ:
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ:
				OnRecv((NKMPacket_GAME_DEV_COOL_TIME_RESET_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_FRAME_MOVE_REQ:
				return true;
			case ClientPacketId.kNKMPacket_GAME_PAUSE_REQ:
				OnRecv((NKMPacket_GAME_PAUSE_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_SPEED_2X_REQ:
				OnRecv((NKMPacket_GAME_SPEED_2X_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_AUTO_SKILL_CHANGE_REQ:
				OnRecv((NKMPacket_GAME_AUTO_SKILL_CHANGE_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_USE_UNIT_SKILL_REQ:
				OnRecv((NKMPacket_GAME_USE_UNIT_SKILL_REQ)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_MONSTER_AUTO_RESPAWN_REQ:
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_ACTIVATE_OPERATOR_REQ:
				return true;
			}
		}
		return false;
	}

	public static void OnRecv(NKMPacket_GAME_LOAD_REQ cNKMPacket_GAME_LOAD_REQ)
	{
		MakeNewLocalGame();
		NKMPacket_GAME_LOAD_ACK nKMPacket_GAME_LOAD_ACK = new NKMPacket_GAME_LOAD_ACK();
		NKMGameData cNKMGameData = new NKMGameData
		{
			m_DungeonID = cNKMPacket_GAME_LOAD_REQ.dungeonID,
			m_TeamASupply = 2
		};
		NKMGameRuntimeData cNKMGameRuntimeData = new NKMGameRuntimeData();
		if (cNKMPacket_GAME_LOAD_REQ.isDev)
		{
			cNKMGameData = MakeDevGameData(ref cNKMGameData, ref cNKMGameRuntimeData);
			m_NKCGameServerLocal.SetGameData(cNKMGameData);
			m_NKCGameServerLocal.SetGameRuntimeData(cNKMGameRuntimeData);
		}
		nKMPacket_GAME_LOAD_ACK.gameData = new NKMGameData();
		nKMPacket_GAME_LOAD_ACK.gameData.DeepCopyFrom(cNKMGameData);
		nKMPacket_GAME_LOAD_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
		NKCLocalPacketHandler.SendPacketToClient(nKMPacket_GAME_LOAD_ACK);
	}

	public static void OnRecv(NKMPacket_DEV_GAME_LOAD_REQ cNKMPacket_GAME_LOAD_REQ)
	{
		MakeNewLocalGame();
		NKMPacket_GAME_LOAD_ACK nKMPacket_GAME_LOAD_ACK = new NKMPacket_GAME_LOAD_ACK();
		m_NKCGameServerLocal.SetGameData(cNKMPacket_GAME_LOAD_REQ.gamedata);
		m_NKCGameServerLocal.SetGameRuntimeData(cNKMPacket_GAME_LOAD_REQ.gameRuntimeData);
		nKMPacket_GAME_LOAD_ACK.gameData = new NKMGameData();
		nKMPacket_GAME_LOAD_ACK.gameData.DeepCopyFrom(cNKMPacket_GAME_LOAD_REQ.gamedata);
		nKMPacket_GAME_LOAD_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
		NKCLocalPacketHandler.SendPacketToClient(nKMPacket_GAME_LOAD_ACK);
	}

	public static void OnRecv(NKMPacket_PRACTICE_GAME_LOAD_REQ cNKMPacket_PRACTICE_GAME_LOAD_REQ)
	{
		MakeNewLocalGame();
		NKMPacket_GAME_LOAD_ACK nKMPacket_GAME_LOAD_ACK = new NKMPacket_GAME_LOAD_ACK();
		if (NKMUnitManager.GetUnitTemplet(cNKMPacket_PRACTICE_GAME_LOAD_REQ.practiceUnitData.m_UnitID) == null)
		{
			nKMPacket_GAME_LOAD_ACK.errorCode = NKM_ERROR_CODE.NEC_FAIL_PRACTICE_GAME_LOAD_REQ_INVALID_UNIT_TEMPLET;
			NKCLocalPacketHandler.SendPacketToClient(nKMPacket_GAME_LOAD_ACK);
			return;
		}
		NKMGameData cNKMGameData = new NKMGameData();
		cNKMGameData.m_DungeonID = cNKMPacket_PRACTICE_GAME_LOAD_REQ.dungeonID;
		cNKMGameData.m_TeamASupply = 2;
		NKMGameRuntimeData cNKMGameRuntimeData = new NKMGameRuntimeData();
		cNKMGameData = MakePracticeGameData(ref cNKMGameData, ref cNKMGameRuntimeData, cNKMPacket_PRACTICE_GAME_LOAD_REQ.practiceUnitData);
		m_NKCGameServerLocal.SetGameData(cNKMGameData);
		m_NKCGameServerLocal.SetGameRuntimeData(cNKMGameRuntimeData);
		nKMPacket_GAME_LOAD_ACK.gameData = new NKMGameData();
		nKMPacket_GAME_LOAD_ACK.gameData.DeepCopyFrom(cNKMGameData);
		nKMPacket_GAME_LOAD_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
		NKCLocalPacketHandler.SendPacketToClient(nKMPacket_GAME_LOAD_ACK);
	}

	public static void MakeNewLocalGame()
	{
		m_NKCGameServerLocal.EndGame();
		m_NKCGameServerLocal.Init();
	}

	public static NKMGameData MakeDevGameData(ref NKMGameData cNKMGameData, ref NKMGameRuntimeData cNKMGameRuntimeData)
	{
		cNKMGameData.m_GameUID = GetGameUIDIndex();
		cNKMGameData.m_bLocal = true;
		cNKMGameData.m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_DEV;
		cNKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_bAutoRespawn = false;
		NKMUnitData nKMUnitData = new NKMUnitData();
		nKMUnitData.m_UnitUID = NpcUid.Get();
		nKMUnitData.m_UnitID = 300016;
		cNKMGameData.m_NKMGameTeamDataA.m_MainShip = nKMUnitData;
		cNKMGameData.m_NKMGameTeamDataA.m_Operator = s_NKMOperatorTeamA_ForDev;
		NKMDungeonManager.MakeOperatorUnitData(cNKMGameData.m_NKMGameTeamDataA);
		NKMDungeonManager.MakeGameTeamData(cNKMGameData, cNKMGameRuntimeData);
		return cNKMGameData;
	}

	public static NKMGameData MakePracticeGameData(ref NKMGameData cNKMGameData, ref NKMGameRuntimeData cNKMGameRuntimeData, NKMUnitData practiceUnitData)
	{
		cNKMGameData.m_GameUID = GetGameUIDIndex();
		cNKMGameData.m_bLocal = true;
		cNKMGameData.m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_PRACTICE;
		cNKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_bAutoRespawn = false;
		cNKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_bAutoRespawn = false;
		NKMUnitData nKMUnitData = new NKMUnitData();
		nKMUnitData.DeepCopyFrom(practiceUnitData);
		nKMUnitData.m_UnitUID = NpcUid.Get();
		for (int i = 0; i < 4; i++)
		{
			long equipUid = nKMUnitData.GetEquipUid((ITEM_EQUIP_POSITION)i);
			if (equipUid != 0L)
			{
				NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(equipUid);
				if (itemEquip != null)
				{
					NKMEquipItemData nKMEquipItemData = new NKMEquipItemData();
					nKMEquipItemData.DeepCopyFrom(itemEquip);
					cNKMGameData.m_NKMGameTeamDataA.m_ItemEquipData.Add(equipUid, nKMEquipItemData);
				}
			}
		}
		NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(practiceUnitData.m_UnitID);
		if (unitTemplet != null && unitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			cNKMGameData.m_NKMGameTeamDataA.m_MainShip = nKMUnitData;
		}
		else
		{
			cNKMGameData.m_NKMGameTeamDataA.m_DeckData.SetListUnitDeck(0, nKMUnitData.m_UnitUID);
			cNKMGameData.m_NKMGameTeamDataA.m_listUnitData.Add(nKMUnitData);
		}
		cNKMGameData.m_NKMGameTeamDataA.m_LeaderUnitUID = nKMUnitData.m_UnitUID;
		NKMDungeonManager.MakeGameTeamData(cNKMGameData, cNKMGameRuntimeData);
		return cNKMGameData;
	}

	public static void LocalGameUnitAllKill(bool bEnemy = false)
	{
		if (m_NKCGameServerLocal != null)
		{
			if (!bEnemy)
			{
				GetGameServerLocal().AllKill(NKM_TEAM_TYPE.NTT_A1);
				GetGameServerLocal().AllKill(NKM_TEAM_TYPE.NTT_A2);
			}
			else
			{
				GetGameServerLocal().AllKill(NKM_TEAM_TYPE.NTT_B1);
				GetGameServerLocal().AllKill(NKM_TEAM_TYPE.NTT_B2);
			}
		}
	}

	public static void OnRecv(NKMPacket_GAME_LOAD_COMPLETE_REQ cNKMPacket_GAME_LOAD_COMPLETE_REQ)
	{
		NKCLocalPacketHandler.SendPacketToClient(new NKMPacket_GAME_LOAD_COMPLETE_ACK
		{
			gameRuntimeData = m_NKCGameServerLocal.GetGameRuntimeData().DeepCopy()
		});
		m_NKCGameServerLocal.StartGame(bIntrude: false);
	}

	public static void OnRecv(NKMPacket_GAME_RESPAWN_REQ cNKMPacket_GAME_RESPAWN_REQ)
	{
		NKMPacket_GAME_RESPAWN_ACK nKMPacket_GAME_RESPAWN_ACK = new NKMPacket_GAME_RESPAWN_ACK();
		nKMPacket_GAME_RESPAWN_ACK.unitUID = cNKMPacket_GAME_RESPAWN_REQ.unitUID;
		nKMPacket_GAME_RESPAWN_ACK.assistUnit = cNKMPacket_GAME_RESPAWN_REQ.assistUnit;
		nKMPacket_GAME_RESPAWN_ACK.errorCode = m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_RESPAWN_REQ, ref nKMPacket_GAME_RESPAWN_ACK.unitUID);
		if (nKMPacket_GAME_RESPAWN_ACK.unitUID <= 0)
		{
			nKMPacket_GAME_RESPAWN_ACK.unitUID = cNKMPacket_GAME_RESPAWN_REQ.unitUID;
		}
		NKCLocalPacketHandler.SendPacketToClient(nKMPacket_GAME_RESPAWN_ACK);
	}

	public static void OnRecv(NKMPacket_GAME_UNIT_RETREAT_REQ cNKMPacket_GAME_UNIT_RETREAT_REQ)
	{
		NKCLocalPacketHandler.SendPacketToClient(new NKMPacket_GAME_UNIT_RETREAT_ACK
		{
			unitUID = cNKMPacket_GAME_UNIT_RETREAT_REQ.unitUID,
			errorCode = m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_UNIT_RETREAT_REQ)
		});
	}

	public static void OnRecv(NKMPacket_GAME_SHIP_SKILL_REQ cNKMPacket_GAME_SHIP_SKILL_REQ)
	{
		NKMPacket_GAME_SHIP_SKILL_ACK nKMPacket_GAME_SHIP_SKILL_ACK = new NKMPacket_GAME_SHIP_SKILL_ACK();
		nKMPacket_GAME_SHIP_SKILL_ACK.shipSkillID = cNKMPacket_GAME_SHIP_SKILL_REQ.shipSkillID;
		nKMPacket_GAME_SHIP_SKILL_ACK.skillPosX = cNKMPacket_GAME_SHIP_SKILL_REQ.skillPosX;
		nKMPacket_GAME_SHIP_SKILL_ACK.errorCode = m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_SHIP_SKILL_REQ, nKMPacket_GAME_SHIP_SKILL_ACK);
		NKCLocalPacketHandler.SendPacketToClient(nKMPacket_GAME_SHIP_SKILL_ACK);
	}

	public static void OnRecv(NKMPacket_GAME_TACTICAL_COMMAND_REQ cNKMPacket_GAME_TACTICAL_COMMAND_REQ)
	{
		NKMPacket_GAME_TACTICAL_COMMAND_ACK nKMPacket_GAME_TACTICAL_COMMAND_ACK = new NKMPacket_GAME_TACTICAL_COMMAND_ACK();
		nKMPacket_GAME_TACTICAL_COMMAND_ACK.cTacticalCommandData = new NKMTacticalCommandData();
		nKMPacket_GAME_TACTICAL_COMMAND_ACK.errorCode = m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_TACTICAL_COMMAND_REQ, nKMPacket_GAME_TACTICAL_COMMAND_ACK);
		NKCLocalPacketHandler.SendPacketToClient(nKMPacket_GAME_TACTICAL_COMMAND_ACK);
	}

	public static void OnRecv(NKMPacket_GAME_AUTO_RESPAWN_REQ cNKMPacket_GAME_AUTO_RESPAWN_REQ)
	{
		NKMPacket_GAME_AUTO_RESPAWN_ACK nKMPacket_GAME_AUTO_RESPAWN_ACK = new NKMPacket_GAME_AUTO_RESPAWN_ACK();
		nKMPacket_GAME_AUTO_RESPAWN_ACK.errorCode = m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_AUTO_RESPAWN_REQ);
		if (nKMPacket_GAME_AUTO_RESPAWN_ACK.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			nKMPacket_GAME_AUTO_RESPAWN_ACK.isAutoRespawn = cNKMPacket_GAME_AUTO_RESPAWN_REQ.isAutoRespawn;
		}
		NKCLocalPacketHandler.SendPacketToClient(nKMPacket_GAME_AUTO_RESPAWN_ACK);
	}

	public static void OnRecv(NKMPacket_GAME_OPTION_CHANGE_REQ cNKMPacket_GAME_OPTION_CHANGE_REQ)
	{
		NKCLocalPacketHandler.SendPacketToClient(new NKMPacket_GAME_OPTION_CHANGE_ACK
		{
			actionCameraType = cNKMPacket_GAME_OPTION_CHANGE_REQ.actionCameraType,
			isTrackCamera = cNKMPacket_GAME_OPTION_CHANGE_REQ.isTrackCamera,
			isViewSkillCutIn = cNKMPacket_GAME_OPTION_CHANGE_REQ.isViewSkillCutIn,
			errorCode = NKM_ERROR_CODE.NEC_OK
		});
	}

	public static void OnRecv(NKMPacket_GAME_DEV_RESPAWN_REQ cNKMPacket_GAME_DEV_RESPAWN_REQ)
	{
		m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_DEV_RESPAWN_REQ);
	}

	public static void OnRecv(NKMPacket_GAME_DEV_COOL_TIME_RESET_REQ cNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ)
	{
		NKCLocalPacketHandler.SendPacketToClient(new NKMPacket_GAME_DEV_COOL_TIME_RESET_ACK
		{
			isSkill = cNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ.isSkill,
			teamType = cNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ.teamType,
			errorCode = m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ)
		});
	}

	public static void OnRecv(NKMPacket_GAME_PAUSE_REQ cNKMPacket_GAME_PAUSE_REQ)
	{
		NKCLocalPacketHandler.SendPacketToClient(new NKMPacket_GAME_PAUSE_ACK
		{
			isPause = cNKMPacket_GAME_PAUSE_REQ.isPause,
			isPauseEvent = cNKMPacket_GAME_PAUSE_REQ.isPauseEvent,
			errorCode = m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_PAUSE_REQ)
		});
	}

	public static void OnRecv(NKMPacket_GAME_SPEED_2X_REQ cNKMPacket_GAME_SPEED_2X_REQ)
	{
		NKCLocalPacketHandler.SendPacketToClient(new NKMPacket_GAME_SPEED_2X_ACK
		{
			errorCode = m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_SPEED_2X_REQ, NKCScenManager.GetScenManager().GetMyUserData()),
			gameSpeedType = m_NKCGameServerLocal.GetGameRuntimeData().m_NKM_GAME_SPEED_TYPE
		});
	}

	public static void OnRecv(NKMPacket_GAME_AUTO_SKILL_CHANGE_REQ cNKMPacket_GAME_AUTO_SKILL_CHANGE_REQ)
	{
		NKCLocalPacketHandler.SendPacketToClient(new NKMPacket_GAME_AUTO_SKILL_CHANGE_ACK
		{
			errorCode = m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_AUTO_SKILL_CHANGE_REQ, NKM_TEAM_TYPE.NTT_A1, NKCScenManager.GetScenManager().GetMyUserData()),
			gameAutoSkillType = m_NKCGameServerLocal.GetGameRuntimeData().GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_NKM_GAME_AUTO_SKILL_TYPE
		});
	}

	public static void OnRecv(NKMPacket_GAME_USE_UNIT_SKILL_REQ cNKMPacket_GAME_USE_UNIT_SKILL_REQ)
	{
		NKMPacket_GAME_USE_UNIT_SKILL_ACK nKMPacket_GAME_USE_UNIT_SKILL_ACK = new NKMPacket_GAME_USE_UNIT_SKILL_ACK();
		byte skillStateID = 0;
		nKMPacket_GAME_USE_UNIT_SKILL_ACK.errorCode = m_NKCGameServerLocal.OnRecv(cNKMPacket_GAME_USE_UNIT_SKILL_REQ, NKM_TEAM_TYPE.NTT_A1, out skillStateID, NKCScenManager.GetScenManager().GetMyUserData());
		nKMPacket_GAME_USE_UNIT_SKILL_ACK.gameUnitUID = cNKMPacket_GAME_USE_UNIT_SKILL_REQ.gameUnitUID;
		nKMPacket_GAME_USE_UNIT_SKILL_ACK.skillStateID = (sbyte)skillStateID;
		NKCLocalPacketHandler.SendPacketToClient(nKMPacket_GAME_USE_UNIT_SKILL_ACK);
	}
}
