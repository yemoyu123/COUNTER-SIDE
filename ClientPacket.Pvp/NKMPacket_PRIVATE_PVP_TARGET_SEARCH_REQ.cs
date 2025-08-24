using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_TARGET_SEARCH_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_TARGET_SEARCH_REQ : ISerializable
{
	public string searchKeyword;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref searchKeyword);
	}
}
