using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_SET_MY_SUPPORT_UNIT_ACK)]
public sealed class NKMPacket_SET_MY_SUPPORT_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMSupportUnitData supportUnitData = new NKMSupportUnitData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref supportUnitData);
	}
}
