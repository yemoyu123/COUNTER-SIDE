using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_Base : ISerializable
{
	public bool IsRollbackPacket;

	public float m_fGameTime;

	public float m_fAbsoluteGameTime;

	public float m_fRemainGameTime;

	public float m_fShipDamage;

	public float m_fRespawnCostA1;

	public float m_fRespawnCostB1;

	public float m_fRespawnCostAssistA1;

	public float m_fRespawnCostAssistB1;

	public float m_fUsedRespawnCostA1;

	public float m_fUsedRespawnCostB1;

	public NKM_GAME_SPEED_TYPE m_NKM_GAME_SPEED_TYPE;

	public NKM_GAME_AUTO_SKILL_TYPE m_NKM_GAME_AUTO_SKILL_TYPE_A;

	public NKM_GAME_AUTO_SKILL_TYPE m_NKM_GAME_AUTO_SKILL_TYPE_B;

	public List<NKMGameSyncData_DieUnit> m_NKMGameSyncData_DieUnit = new List<NKMGameSyncData_DieUnit>();

	public List<NKMGameSyncData_Unit> m_NKMGameSyncData_Unit = new List<NKMGameSyncData_Unit>();

	public List<NKMGameSyncDataSimple_Unit> m_NKMGameSyncDataSimple_Unit = new List<NKMGameSyncDataSimple_Unit>();

	public List<NKMGameSyncData_ShipSkill> m_NKMGameSyncData_ShipSkill = new List<NKMGameSyncData_ShipSkill>();

	public List<NKMGameSyncData_Deck> m_NKMGameSyncData_Deck = new List<NKMGameSyncData_Deck>();

	public List<NKMGameSyncData_DeckAssist> m_NKMGameSyncData_DeckAssist = new List<NKMGameSyncData_DeckAssist>();

	public List<NKMGameSyncData_GameState> m_NKMGameSyncData_GameState = new List<NKMGameSyncData_GameState>();

	public List<NKMGameSyncData_DungeonEvent> m_NKMGameSyncData_DungeonEvent = new List<NKMGameSyncData_DungeonEvent>();

	public List<NKMGameSyncData_GameEvent> m_NKMGameSyncData_GameEvent = new List<NKMGameSyncData_GameEvent>();

	public NKMGameSyncData_GamePoint m_NKMGameSyncData_GamePoint;

	public void Serialize(IPacketStream stream)
	{
		stream.AsHalf(ref m_fGameTime);
		stream.AsHalf(ref m_fRemainGameTime);
		stream.AsHalf(ref m_fShipDamage);
		stream.AsHalf(ref m_fRespawnCostA1);
		stream.AsHalf(ref m_fRespawnCostB1);
		stream.AsHalf(ref m_fRespawnCostAssistA1);
		stream.AsHalf(ref m_fRespawnCostAssistB1);
		stream.AsHalf(ref m_fUsedRespawnCostA1);
		stream.AsHalf(ref m_fUsedRespawnCostB1);
		stream.PutOrGetEnum(ref m_NKM_GAME_SPEED_TYPE);
		stream.PutOrGetEnum(ref m_NKM_GAME_AUTO_SKILL_TYPE_A);
		stream.PutOrGetEnum(ref m_NKM_GAME_AUTO_SKILL_TYPE_B);
		stream.PutOrGet(ref m_NKMGameSyncData_DieUnit);
		stream.PutOrGet(ref m_NKMGameSyncData_Unit);
		stream.PutOrGet(ref m_NKMGameSyncDataSimple_Unit);
		stream.PutOrGet(ref m_NKMGameSyncData_ShipSkill);
		stream.PutOrGet(ref m_NKMGameSyncData_Deck);
		stream.PutOrGet(ref m_NKMGameSyncData_DeckAssist);
		stream.PutOrGet(ref m_NKMGameSyncData_GameState);
		stream.PutOrGet(ref m_NKMGameSyncData_DungeonEvent);
		stream.PutOrGet(ref m_NKMGameSyncData_GameEvent);
		stream.PutOrGet(ref m_NKMGameSyncData_GamePoint);
	}
}
