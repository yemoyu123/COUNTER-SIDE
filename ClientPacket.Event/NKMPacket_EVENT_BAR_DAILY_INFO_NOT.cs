using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_BAR_DAILY_INFO_NOT)]
public sealed class NKMPacket_EVENT_BAR_DAILY_INFO_NOT : ISerializable
{
	public int dailyCocktailItemId;

	public int remainDeliveryLimitValue;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref dailyCocktailItemId);
		stream.PutOrGet(ref remainDeliveryLimitValue);
	}
}
