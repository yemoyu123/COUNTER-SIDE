using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_FAVORITE_UNIT_ACK)]
public sealed class NKMPacket_FAVORITE_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long unitUid;

	public bool isFavorite;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitUid);
		stream.PutOrGet(ref isFavorite);
	}
}
