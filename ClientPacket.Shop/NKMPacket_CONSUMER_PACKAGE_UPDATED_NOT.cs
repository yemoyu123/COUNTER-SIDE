using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_CONSUMER_PACKAGE_UPDATED_NOT)]
public sealed class NKMPacket_CONSUMER_PACKAGE_UPDATED_NOT : ISerializable
{
	public List<NKMConsumerPackageData> list = new List<NKMConsumerPackageData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref list);
	}
}
