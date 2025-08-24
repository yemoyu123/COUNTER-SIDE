using System.Collections.Generic;
using ClientPacket.Game;
using NKC.PacketHandler;
using NKM;

namespace NKC;

public class NKCGameServerLocal : NKMGameServerHost
{
	public NKCGameServerLocal()
	{
		m_NKM_GAME_CLASS_TYPE = NKM_GAME_CLASS_TYPE.NGCT_GAME_SERVER_LOCAL;
	}

	public override void ProcessGameState()
	{
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_START && m_fPlayWaitTime > 0f)
		{
			m_fPlayWaitTime -= m_fDeltaTime;
			if (m_fPlayWaitTime <= 0f)
			{
				m_fPlayWaitTime = 0f;
				SetGameState(NKM_GAME_STATE.NGS_PLAY);
				if (GetDungeonType() == NKM_DUNGEON_TYPE.NDT_WAVE)
				{
					m_NKMGameRuntimeData.m_WaveID = 1;
				}
				SyncGameStateChange(m_NKMGameRuntimeData.m_NKM_GAME_STATE, m_NKMGameRuntimeData.m_WinTeam, m_NKMGameRuntimeData.m_WaveID);
			}
		}
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_FINISH)
		{
			return;
		}
		if (!m_NKMGameRuntimeData.m_bGameEnded)
		{
			GameEndFlush();
			m_NKMGameRuntimeData.m_bGameEnded = true;
		}
		if (m_fFinishWaitTime > 0f)
		{
			m_fFinishWaitTime -= m_fDeltaTime;
			if (m_fFinishWaitTime <= 0f)
			{
				m_fFinishWaitTime = 0f;
				SetGameState(NKM_GAME_STATE.NGS_END);
			}
		}
	}

	public override void SetGameData(NKMGameData cNKMGameData)
	{
		base.SetGameData(cNKMGameData);
	}

	public override void StartGame(bool bIntrude)
	{
		base.StartGame(bIntrude);
		NKCLocalPacketHandler.SendPacketToClient(new NKMPacket_GAME_START_NOT());
		SetGameState(NKM_GAME_STATE.NGS_START);
	}

	public override void Update(float deltaTime)
	{
		if (!m_NKMGameRuntimeData.m_bPause)
		{
			base.Update(deltaTime);
		}
	}

	public override void SendSyncDataPackFlush(NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT cPacket_NPT_GAME_SYNC_DATA_PACK_NOT)
	{
		NKCLocalPacketHandler.SendPacketToClient(cPacket_NPT_GAME_SYNC_DATA_PACK_NOT);
	}

	public void GameEndFlush()
	{
		NKCLocalPacketHandler.SendPacketToClient(new NKMPacket_GAME_END_NOT());
	}

	public override NKM_ERROR_CODE OnRecv(NKMPacket_GAME_PAUSE_REQ cNKMPacket_GAME_PAUSE_REQ)
	{
		return base.OnRecv(cNKMPacket_GAME_PAUSE_REQ);
	}

	public override NKM_ERROR_CODE OnRecv(NKMPacket_GAME_DEV_COOL_TIME_RESET_REQ cNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ)
	{
		return base.OnRecv(cNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ);
	}

	public void OnRecv(NKMPacket_GAME_DEV_RESPAWN_REQ cNKMPacket_GAME_DEV_RESPAWN_REQ)
	{
		NKMPacket_GAME_DEV_RESPAWN_ACK cNKMPacket_GAME_DEV_RESPAWN_ACK = new NKMPacket_GAME_DEV_RESPAWN_ACK();
		cNKMPacket_GAME_DEV_RESPAWN_ACK.errorCode = base.OnRecv(cNKMPacket_GAME_DEV_RESPAWN_REQ, ref cNKMPacket_GAME_DEV_RESPAWN_ACK, cNKMPacket_GAME_DEV_RESPAWN_REQ.teamType);
		NKCLocalPacketHandler.SendPacketToClient(cNKMPacket_GAME_DEV_RESPAWN_ACK);
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_RESPAWN_REQ cPacket_GAME_RESPAWN_REQ, ref long respawnUnitUID)
	{
		return base.OnRecv(cPacket_GAME_RESPAWN_REQ, NKM_TEAM_TYPE.NTT_A1, ref respawnUnitUID);
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_UNIT_RETREAT_REQ cNKMPacket_GAME_UNIT_RETREAT_REQ)
	{
		return base.OnRecv(cNKMPacket_GAME_UNIT_RETREAT_REQ, NKM_TEAM_TYPE.NTT_A1);
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_SHIP_SKILL_REQ cNKMPacket_GAME_SHIP_SKILL_REQ, NKMPacket_GAME_SHIP_SKILL_ACK cNKMPacket_GAME_SHIP_SKILL_ACK)
	{
		return OnRecv(cNKMPacket_GAME_SHIP_SKILL_REQ, NKM_TEAM_TYPE.NTT_A1);
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_TACTICAL_COMMAND_REQ cNKMPacket_GAME_TACTICAL_COMMAND_REQ, NKMPacket_GAME_TACTICAL_COMMAND_ACK cNKMPacket_GAME_TACTICAL_COMMAND_ACK)
	{
		return OnRecv(cNKMPacket_GAME_TACTICAL_COMMAND_REQ, cNKMPacket_GAME_TACTICAL_COMMAND_ACK, NKM_TEAM_TYPE.NTT_A1);
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_AUTO_RESPAWN_REQ cPacket_GAME_AUTO_RESPAWN_REQ)
	{
		return OnRecv(cPacket_GAME_AUTO_RESPAWN_REQ, NKM_TEAM_TYPE.NTT_A1, NKCScenManager.GetScenManager().GetMyUserData());
	}

	private void SetUnitDie(NKMUnitData cNKMUnitData)
	{
		if (cNKMUnitData != null)
		{
			List<short> listGameUnitUID = cNKMUnitData.m_listGameUnitUID;
			for (int i = 0; i < listGameUnitUID.Count; i++)
			{
				GetUnit(listGameUnitUID[i])?.GetUnitSyncData().SetHP(0f);
			}
		}
	}
}
