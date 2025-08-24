using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_LIMIT_BREAK_UNIT_REQ)]
public sealed class NKMPacket_LIMIT_BREAK_UNIT_REQ : ISerializable
{
	public long unitUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUID);
	}
}
