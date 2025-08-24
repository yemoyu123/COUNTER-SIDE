using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_FAVORITE_UNIT_REQ)]
public sealed class NKMPacket_FAVORITE_UNIT_REQ : ISerializable
{
	public long unitUid;

	public bool isFavorite;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
		stream.PutOrGet(ref isFavorite);
	}
}
