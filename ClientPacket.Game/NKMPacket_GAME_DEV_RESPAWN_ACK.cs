using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_DEV_RESPAWN_ACK)]
public sealed class NKMPacket_GAME_DEV_RESPAWN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData unitData;

	public List<NKMDynamicRespawnUnitData> dynamicRespawnUnitDataTeamA = new List<NKMDynamicRespawnUnitData>();

	public List<NKMDynamicRespawnUnitData> dynamicRespawnUnitDataTeamB = new List<NKMDynamicRespawnUnitData>();

	public NKM_TEAM_TYPE teamType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitData);
		stream.PutOrGet(ref dynamicRespawnUnitDataTeamA);
		stream.PutOrGet(ref dynamicRespawnUnitDataTeamB);
		stream.PutOrGetEnum(ref teamType);
	}
}
