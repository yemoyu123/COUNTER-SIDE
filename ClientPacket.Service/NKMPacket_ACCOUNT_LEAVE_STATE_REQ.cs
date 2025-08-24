using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_ACCOUNT_LEAVE_STATE_REQ)]
public sealed class NKMPacket_ACCOUNT_LEAVE_STATE_REQ : ISerializable
{
	public bool leave;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref leave);
	}
}
