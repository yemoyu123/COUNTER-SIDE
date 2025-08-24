using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_RANDOM_MISSION_REFRESH_NOT)]
public sealed class NKMPacket_RANDOM_MISSION_REFRESH_NOT : ISerializable
{
	public int tabId;

	public List<NKMMissionData> missionDataList = new List<NKMMissionData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tabId);
		stream.PutOrGet(ref missionDataList);
	}
}
