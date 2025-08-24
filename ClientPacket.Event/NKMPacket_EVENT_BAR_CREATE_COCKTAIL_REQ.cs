using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_BAR_CREATE_COCKTAIL_REQ)]
public sealed class NKMPacket_EVENT_BAR_CREATE_COCKTAIL_REQ : ISerializable
{
	public int cocktailItemId;

	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref cocktailItemId);
		stream.PutOrGet(ref count);
	}
}
