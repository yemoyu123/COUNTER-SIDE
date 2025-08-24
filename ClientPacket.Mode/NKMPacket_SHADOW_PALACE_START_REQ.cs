using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_SHADOW_PALACE_START_REQ)]
public sealed class NKMPacket_SHADOW_PALACE_START_REQ : ISerializable
{
	public int palaceId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref palaceId);
	}
}
