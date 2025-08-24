using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_TAKE_HEART_ACK)]
public sealed class NKMPacket_OFFICE_TAKE_HEART_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData unit;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unit);
	}
}
