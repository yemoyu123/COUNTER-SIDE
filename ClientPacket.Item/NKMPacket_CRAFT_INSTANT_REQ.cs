using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CRAFT_INSTANT_REQ)]
public sealed class NKMPacket_CRAFT_INSTANT_REQ : ISerializable
{
	public int moldId;

	public int moldCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref moldId);
		stream.PutOrGet(ref moldCount);
	}
}
