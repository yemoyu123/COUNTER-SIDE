using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_LEVELUP_REQ)]
public sealed class NKMPacket_SHIP_LEVELUP_REQ : ISerializable
{
	public long shipUID;

	public int nextLevel;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shipUID);
		stream.PutOrGet(ref nextLevel);
	}
}
