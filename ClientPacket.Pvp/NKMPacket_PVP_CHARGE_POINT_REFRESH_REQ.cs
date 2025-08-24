using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_CHARGE_POINT_REFRESH_REQ)]
public sealed class NKMPacket_PVP_CHARGE_POINT_REFRESH_REQ : ISerializable
{
	public int itemId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref itemId);
	}
}
