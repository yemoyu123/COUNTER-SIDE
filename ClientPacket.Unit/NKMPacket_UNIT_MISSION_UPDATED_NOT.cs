using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_UNIT_MISSION_UPDATED_NOT)]
public sealed class NKMPacket_UNIT_MISSION_UPDATED_NOT : ISerializable
{
	public List<NKMUnitMissionData> rewardEnableMissions = new List<NKMUnitMissionData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref rewardEnableMissions);
	}
}
