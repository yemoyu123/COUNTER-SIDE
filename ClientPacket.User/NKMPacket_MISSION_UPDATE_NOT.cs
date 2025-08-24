using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISSION_UPDATE_NOT)]
public sealed class NKMPacket_MISSION_UPDATE_NOT : ISerializable
{
	public HashSet<NKMMissionData> missionDataList = new HashSet<NKMMissionData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref missionDataList);
	}
}
