using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CRAFT_INSTANT_COMPLETE_REQ)]
public sealed class NKMPacket_CRAFT_INSTANT_COMPLETE_REQ : ISerializable
{
	public byte index;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref index);
	}
}
