using System;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class ShopChainTabNextResetData : ISerializable
{
	public string tabType;

	public int subIndex;

	public DateTime nextResetUtc;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tabType);
		stream.PutOrGet(ref subIndex);
		stream.PutOrGet(ref nextResetUtc);
	}
}
