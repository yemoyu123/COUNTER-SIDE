using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_INTRUDE_START_NOT)]
public sealed class NKMPacket_GAME_INTRUDE_START_NOT : ISerializable
{
	public float gameTime;

	public float absoluteGameTime;

	public NKMGameSyncDataPack gameSyncDataPack;

	public NKMGameTeamDeckData gameTeamDeckDataA;

	public NKMGameTeamDeckData gameTeamDeckDataB;

	public float usedRespawnCost;

	public float respawnCount;

	public Dictionary<int, float> mainShipAStateCoolTimeMap = new Dictionary<int, float>();

	public Dictionary<int, float> mainShipBStateCoolTimeMap = new Dictionary<int, float>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref gameTime);
		stream.PutOrGet(ref absoluteGameTime);
		stream.PutOrGet(ref gameSyncDataPack);
		stream.PutOrGet(ref gameTeamDeckDataA);
		stream.PutOrGet(ref gameTeamDeckDataB);
		stream.PutOrGet(ref usedRespawnCost);
		stream.PutOrGet(ref respawnCount);
		stream.PutOrGet(ref mainShipAStateCoolTimeMap);
		stream.PutOrGet(ref mainShipBStateCoolTimeMap);
	}
}
