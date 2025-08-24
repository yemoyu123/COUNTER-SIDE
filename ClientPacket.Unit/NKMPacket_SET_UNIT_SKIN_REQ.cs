using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SET_UNIT_SKIN_REQ)]
public sealed class NKMPacket_SET_UNIT_SKIN_REQ : ISerializable
{
	public long unitUID;

	public int skinID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref skinID);
	}
}
