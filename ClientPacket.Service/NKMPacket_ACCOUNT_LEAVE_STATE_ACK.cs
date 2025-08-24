using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_ACCOUNT_LEAVE_STATE_ACK)]
public sealed class NKMPacket_ACCOUNT_LEAVE_STATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool leave;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref leave);
	}
}
