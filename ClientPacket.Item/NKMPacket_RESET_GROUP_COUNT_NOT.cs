using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_RESET_GROUP_COUNT_NOT)]
public sealed class NKMPacket_RESET_GROUP_COUNT_NOT : ISerializable
{
	public List<NKMResetCount> resetCountList = new List<NKMResetCount>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref resetCountList);
	}
}
