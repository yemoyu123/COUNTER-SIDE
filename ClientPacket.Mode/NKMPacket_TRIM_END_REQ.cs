using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_TRIM_END_REQ)]
public sealed class NKMPacket_TRIM_END_REQ : ISerializable
{
	public int trimId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref trimId);
	}
}
