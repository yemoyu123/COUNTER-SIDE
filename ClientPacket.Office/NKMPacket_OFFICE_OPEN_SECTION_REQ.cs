using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_OPEN_SECTION_REQ)]
public sealed class NKMPacket_OFFICE_OPEN_SECTION_REQ : ISerializable
{
	public int sectionId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref sectionId);
	}
}
