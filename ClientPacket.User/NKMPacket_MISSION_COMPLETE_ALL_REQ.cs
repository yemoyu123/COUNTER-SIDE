using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISSION_COMPLETE_ALL_REQ)]
public sealed class NKMPacket_MISSION_COMPLETE_ALL_REQ : ISerializable
{
	public int tabId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tabId);
	}
}
