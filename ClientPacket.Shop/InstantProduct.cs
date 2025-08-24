using System;
using Cs.Protocol;

namespace ClientPacket.Shop;

public sealed class InstantProduct : ISerializable
{
	public int productId;

	public DateTime endDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref productId);
		stream.PutOrGet(ref endDate);
	}
}
