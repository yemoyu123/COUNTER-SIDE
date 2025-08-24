using Cs.Protocol;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_CUSTOM_PICUP_SELECT_TARGET_REQ)]
public sealed class NKMPacket_CUSTOM_PICUP_SELECT_TARGET_REQ : ISerializable
{
	public int customPickupId;

	public int targetUnitId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref customPickupId);
		stream.PutOrGet(ref targetUnitId);
	}
}
