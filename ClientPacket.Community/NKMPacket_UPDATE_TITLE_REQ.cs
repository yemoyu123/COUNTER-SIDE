using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UPDATE_TITLE_REQ)]
public sealed class NKMPacket_UPDATE_TITLE_REQ : ISerializable
{
	public int titleId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref titleId);
	}
}
