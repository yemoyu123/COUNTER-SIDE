using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class MiscItemData : ISerializable
{
	public int itemId;

	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref itemId);
		stream.PutOrGet(ref count);
	}
}
