using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_SUICIDE_REQ)]
public sealed class NKMPacket_DIVE_SUICIDE_REQ : ISerializable
{
	public byte selectDeckIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selectDeckIndex);
	}
}
