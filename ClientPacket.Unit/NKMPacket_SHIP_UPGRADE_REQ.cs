using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_UPGRADE_REQ)]
public sealed class NKMPacket_SHIP_UPGRADE_REQ : ISerializable
{
	public long shipUID;

	public int nextShipID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shipUID);
		stream.PutOrGet(ref nextShipID);
	}
}
