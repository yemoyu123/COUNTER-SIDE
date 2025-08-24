using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_OPERATOR_LOCK_REQ)]
public sealed class NKMPacket_OPERATOR_LOCK_REQ : ISerializable
{
	public long unitUID;

	public bool locked;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref locked);
	}
}
