using System;
using System.Collections.Generic;
using Cs.Math;
using Cs.Protocol;

namespace NKM;

public class NKMGameRuntimeData : ISerializable
{
	public float m_GameTime;

	public NKM_GAME_SPEED_TYPE m_NKM_GAME_SPEED_TYPE;

	public float m_PrevWaveEndTime;

	public NKM_GAME_STATE m_NKM_GAME_STATE = NKM_GAME_STATE.NGS_STOP;

	public int m_WaveID;

	public float m_fRemainGameTime = 180f;

	public float m_fShipDamage;

	public NKM_TEAM_TYPE m_WinTeam;

	public bool m_bGameEnded;

	public bool m_bPause;

	public bool m_bGiveUp;

	public bool m_bRestart;

	public NKMGameRuntimeTeamData m_NKMGameRuntimeTeamDataA = new NKMGameRuntimeTeamData();

	public NKMGameRuntimeTeamData m_NKMGameRuntimeTeamDataB = new NKMGameRuntimeTeamData();

	public bool m_bPracticeHeal = true;

	public bool m_bPracticeFixedDamage;

	public List<NKMGameSyncData_DungeonEvent> m_lstPermanentDungeonEvent;

	public float GetGamePlayTime()
	{
		float val = m_GameTime - 4f;
		return Math.Max(0f, val);
	}

	public NKMGameRuntimeTeamData GetMyRuntimeTeamData(NKM_TEAM_TYPE myTeamType)
	{
		switch (myTeamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			return m_NKMGameRuntimeTeamDataA;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			return m_NKMGameRuntimeTeamDataB;
		default:
			return null;
		}
	}

	public NKMGameRuntimeTeamData GetEnemyRuntimeTeamData(NKM_TEAM_TYPE myTeamType)
	{
		switch (myTeamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			return m_NKMGameRuntimeTeamDataB;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			return m_NKMGameRuntimeTeamDataA;
		default:
			return null;
		}
	}

	public NKM_DUNGEON_END_TYPE GetDungeonEndType()
	{
		NKM_DUNGEON_END_TYPE result = NKM_DUNGEON_END_TYPE.NORMAL;
		if (m_bGiveUp)
		{
			result = NKM_DUNGEON_END_TYPE.GIVE_UP;
		}
		else if (m_fRemainGameTime.IsNearlyZero())
		{
			result = NKM_DUNGEON_END_TYPE.TIME_OUT;
		}
		return result;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_GameTime);
		stream.PutOrGetEnum(ref m_NKM_GAME_SPEED_TYPE);
		stream.PutOrGet(ref m_PrevWaveEndTime);
		stream.PutOrGetEnum(ref m_NKM_GAME_STATE);
		stream.PutOrGet(ref m_WaveID);
		stream.PutOrGet(ref m_fRemainGameTime);
		stream.PutOrGet(ref m_fShipDamage);
		stream.PutOrGetEnum(ref m_WinTeam);
		stream.PutOrGet(ref m_bGameEnded);
		stream.PutOrGet(ref m_bPause);
		stream.PutOrGet(ref m_bGiveUp);
		stream.PutOrGet(ref m_bRestart);
		stream.PutOrGet(ref m_NKMGameRuntimeTeamDataA);
		stream.PutOrGet(ref m_NKMGameRuntimeTeamDataB);
		stream.PutOrGet(ref m_lstPermanentDungeonEvent);
	}
}
