using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class ZlongPaymentCustomParams : ISerializable
{
	public List<int> selectIndices = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selectIndices);
	}
}
