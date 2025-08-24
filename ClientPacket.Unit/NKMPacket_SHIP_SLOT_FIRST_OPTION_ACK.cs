using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_SLOT_FIRST_OPTION_ACK)]
public sealed class NKMPacket_SHIP_SLOT_FIRST_OPTION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData shipData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref shipData);
	}
}
