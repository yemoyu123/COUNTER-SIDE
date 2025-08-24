using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_MOVE_FORWARD_REQ)]
public sealed class NKMPacket_DIVE_MOVE_FORWARD_REQ : ISerializable
{
	public int slotIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref slotIndex);
	}
}
