using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_SHADOW_PALACE_SKIP_REQ)]
public sealed class NKMPacket_SHADOW_PALACE_SKIP_REQ : ISerializable
{
	public int palaceId;

	public int skipCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref palaceId);
		stream.PutOrGet(ref skipCount);
	}
}
