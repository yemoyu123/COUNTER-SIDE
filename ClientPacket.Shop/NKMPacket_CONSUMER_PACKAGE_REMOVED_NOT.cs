using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_CONSUMER_PACKAGE_REMOVED_NOT)]
public sealed class NKMPacket_CONSUMER_PACKAGE_REMOVED_NOT : ISerializable
{
	public List<int> productIds = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref productIds);
	}
}
