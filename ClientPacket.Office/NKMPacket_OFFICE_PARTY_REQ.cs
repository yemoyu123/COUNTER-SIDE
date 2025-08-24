using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_PARTY_REQ)]
public sealed class NKMPacket_OFFICE_PARTY_REQ : ISerializable
{
	public int roomId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref roomId);
	}
}
