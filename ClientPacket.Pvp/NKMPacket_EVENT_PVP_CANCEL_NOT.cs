using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PVP_CANCEL_NOT)]
public sealed class NKMPacket_EVENT_PVP_CANCEL_NOT : ISerializable
{
	public long targetUserUid;

	public PrivatePvpCancelType cancelType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref targetUserUid);
		stream.PutOrGetEnum(ref cancelType);
	}
}
