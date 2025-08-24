using Cs.Protocol;

namespace ClientPacket.Office;

public sealed class NKMInteriorData : ISerializable
{
	public int itemId;

	public long count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref itemId);
		stream.PutOrGet(ref count);
	}
}
