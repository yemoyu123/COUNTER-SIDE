using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_LOCK_UNIT_ACK)]
public sealed class NKMPacket_LOCK_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long unitUID;

	public bool isLock;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref isLock);
	}
}
