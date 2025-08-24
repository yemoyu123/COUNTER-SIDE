using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_BAR_GET_REWARD_REQ)]
public sealed class NKMPacket_EVENT_BAR_GET_REWARD_REQ : ISerializable
{
	public int cocktailItemId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref cocktailItemId);
	}
}
