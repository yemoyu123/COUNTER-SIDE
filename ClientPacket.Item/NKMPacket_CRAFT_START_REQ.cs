using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CRAFT_START_REQ)]
public sealed class NKMPacket_CRAFT_START_REQ : ISerializable
{
	public byte index;

	public int moldID;

	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref index);
		stream.PutOrGet(ref moldID);
		stream.PutOrGet(ref count);
	}
}
