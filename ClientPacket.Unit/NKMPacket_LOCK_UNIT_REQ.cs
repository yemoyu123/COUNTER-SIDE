using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_LOCK_UNIT_REQ)]
public sealed class NKMPacket_LOCK_UNIT_REQ : ISerializable
{
	public long unitUID;

	public bool isLock;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref isLock);
	}
}
