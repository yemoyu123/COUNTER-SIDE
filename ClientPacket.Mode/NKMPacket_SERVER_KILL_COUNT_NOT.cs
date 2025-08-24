using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_SERVER_KILL_COUNT_NOT)]
public sealed class NKMPacket_SERVER_KILL_COUNT_NOT : ISerializable
{
	public List<NKMServerKillCountData> serverKillCountDataList = new List<NKMServerKillCountData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref serverKillCountDataList);
	}
}
