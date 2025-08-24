using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_OPERATOR_LOCK_ACK)]
public sealed class NKMPacket_OPERATOR_LOCK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long unitUID;

	public bool locked;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref locked);
	}
}
