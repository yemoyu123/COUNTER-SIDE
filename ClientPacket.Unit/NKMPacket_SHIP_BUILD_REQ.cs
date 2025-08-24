using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_BUILD_REQ)]
public sealed class NKMPacket_SHIP_BUILD_REQ : ISerializable
{
	public int shipID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shipID);
	}
}
