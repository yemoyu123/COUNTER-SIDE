using ClientPacket.Game;
using ClientPacket.Mode;
using ClientPacket.User;
using Cs.Protocol;
using NKM;
using Protocol;
using UnityEngine;

namespace NKC.PacketHandler;

public class NKCLocalPacketHandler
{
	public static void SendPacketToLocalServer(ISerializable cPacket_BASE)
	{
		NKCMessage.SendMessage(NKC_EVENT_MESSAGE.NEM_NKCPACKET_SEND_TO_SERVER, PacketController.Instance.GetId(cPacket_BASE), cPacket_BASE, null, null, bDirect: false, GetMessageLatency());
	}

	public static void SendPacketToClient(ISerializable cPacket_BASE)
	{
		NKCMessage.SendMessage(NKC_EVENT_MESSAGE.NEM_NKCPACKET_SEND_TO_CLIENT, PacketController.Instance.GetId(cPacket_BASE), cPacket_BASE, null, null, bDirect: false, GetMessageLatency());
	}

	private static float GetMessageLatency()
	{
		return 0.01f;
	}

	public static bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		if (cNKCMessageData.m_NKC_EVENT_MESSAGE == NKC_EVENT_MESSAGE.NEM_NKCPACKET_SEND_TO_CLIENT)
		{
			switch ((ClientPacketId)(ushort)cNKCMessageData.m_MsgID2)
			{
			case ClientPacketId.kNKMPacket_DEV_GAME_LOAD_ACK:
				OnRecv((NKMPacket_DEV_GAME_LOAD_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_LOAD_ACK:
				OnRecv((NKMPacket_GAME_LOAD_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_LOAD_COMPLETE_ACK:
				OnRecv((NKMPacket_GAME_LOAD_COMPLETE_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_START_NOT:
				OnRecv((NKMPacket_GAME_START_NOT)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_END_NOT:
				OnRecv((NKMPacket_GAME_END_NOT)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT:
				OnRecv((NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_RESPAWN_ACK:
				OnRecv((NKMPacket_GAME_RESPAWN_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_UNIT_RETREAT_ACK:
				OnRecv((NKMPacket_GAME_UNIT_RETREAT_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_AUTO_RESPAWN_ACK:
				OnRecv((NKMPacket_GAME_AUTO_RESPAWN_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_OPTION_CHANGE_ACK:
				OnRecv((NKMPacket_GAME_OPTION_CHANGE_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_RESPAWN_ACK:
				OnRecv((NKMPacket_GAME_DEV_RESPAWN_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_SHIP_SKILL_ACK:
				OnRecv((NKMPacket_GAME_SHIP_SKILL_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_TACTICAL_COMMAND_ACK:
				OnRecv((NKMPacket_GAME_TACTICAL_COMMAND_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_SHIP_CHANGE_ACK:
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_DECK_CHANGE_ACK:
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_COOL_TIME_RESET_ACK:
				OnRecv((NKMPacket_GAME_DEV_COOL_TIME_RESET_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_FRAME_MOVE_ACK:
				return true;
			case ClientPacketId.kNKMPacket_GAME_PAUSE_ACK:
				OnRecv((NKMPacket_GAME_PAUSE_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_SPEED_2X_ACK:
				OnRecv((NKMPacket_GAME_SPEED_2X_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_AUTO_SKILL_CHANGE_ACK:
				OnRecv((NKMPacket_GAME_AUTO_SKILL_CHANGE_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_USE_UNIT_SKILL_ACK:
				OnRecv((NKMPacket_GAME_USE_UNIT_SKILL_ACK)cNKCMessageData.m_Param1);
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_MONSTER_AUTO_RESPAWN_ACK:
				return true;
			case ClientPacketId.kNKMPacket_GAME_DEV_ACTIVATE_OPERATOR_ACK:
				return true;
			}
		}
		return false;
	}

	public static void OnRecv(NKMPacket_GAME_LOAD_ACK cNKMPacket_GAME_LOAD_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GAME_LOAD_ACK.errorCode) && (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_LOGIN || NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME || cNKMPacket_GAME_LOAD_ACK.gameData.GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE))
		{
			NKCResourceUtility.ClearResource();
			NKCScenManager.GetScenManager().GetGameClient().ClearResource();
			NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(cNKMPacket_GAME_LOAD_ACK.gameData);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
		}
	}

	public static void OnRecv(NKMPacket_DEV_GAME_LOAD_ACK cNKMPacket_GAME_LOAD_ACK)
	{
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_GAME_LOAD_ACK.errorCode) && (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_LOGIN || NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME || cNKMPacket_GAME_LOAD_ACK.gameData.GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE))
		{
			NKCResourceUtility.ClearResource();
			NKCScenManager.GetScenManager().GetGameClient().ClearResource();
			NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(cNKMPacket_GAME_LOAD_ACK.gameData);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
		}
	}

	public static void OnRecv(NKMPacket_GAME_LOAD_COMPLETE_ACK cNKMPacket_GAME_LOAD_COMPLETE_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_LOAD_COMPLETE_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_START_NOT cNKMPacket_GAME_START_NOT)
	{
		NKMPopUpBox.CloseWaitBox();
		NKCScenManager.GetScenManager().GetGameClient().StartGame(bIntrude: false);
	}

	public static void OnRecv(NKMPacket_GAME_END_NOT cNKMPacket_GAME_END_NOT)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnLocalGameEndRecv(cNKMPacket_GAME_END_NOT);
		}
	}

	public static void OnRecv(NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT cNKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT);
			NKCPacketObjectPool.CloseObject(cNKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT);
		}
	}

	public static void OnRecv(NKMPacket_GAME_RESPAWN_ACK cNKMPacket_GAME_RESPAWN_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_RESPAWN_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_RESPAWN_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_UNIT_RETREAT_ACK cNKMPacket_GAME_UNIT_RETREAT_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_UNIT_RETREAT_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_UNIT_RETREAT_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_AUTO_RESPAWN_ACK cNKMPacket_GAME_AUTO_RESPAWN_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_AUTO_RESPAWN_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_AUTO_RESPAWN_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_OPTION_CHANGE_ACK cNKMPacket_GAME_OPTION_CHANGE_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_OPTION_CHANGE_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_OPTION_CHANGE_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_DEV_RESPAWN_ACK cNKMPacket_GAME_DEV_RESPAWN_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_DEV_RESPAWN_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_DEV_RESPAWN_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_SHIP_SKILL_ACK cNKMPacket_GAME_SHIP_SKILL_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_SHIP_SKILL_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_SHIP_SKILL_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_TACTICAL_COMMAND_ACK cNKMPacket_GAME_TACTICAL_COMMAND_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_TACTICAL_COMMAND_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_TACTICAL_COMMAND_ACK);
		}
	}

	public static void OnRecv(NKMPacket_GAME_DEV_COOL_TIME_RESET_ACK cNKMPacket_GAME_DEV_COOL_TIME_RESET_ACK)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_DEV_COOL_TIME_RESET_ACK);
			NKCPacketObjectPool.CloseObject(cNKMPacket_GAME_DEV_COOL_TIME_RESET_ACK);
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

	public static void OnRecv(NKMPacket_RESET_STAGE_PLAY_COUNT_ACK cNKMPacket_RESET_STAGE_PLAY_COUNT_ACK)
	{
		Debug.Log("OnRecv - NKMPacket_RESET_STAGE_PLAY_COUNT_ACK - NKCLocalPacketHandlersLobby");
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(cNKMPacket_RESET_STAGE_PLAY_COUNT_ACK.errorCode))
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.m_InventoryData.UpdateItemInfo(cNKMPacket_RESET_STAGE_PLAY_COUNT_ACK.costItemData);
				nKMUserData.UpdateStagePlayData(cNKMPacket_RESET_STAGE_PLAY_COUNT_ACK.stagePlayData);
			}
		}
	}
}
