using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_STEAM_BUY_INIT_REQ)]
public sealed class NKMPacket_STEAM_BUY_INIT_REQ : ISerializable
{
	public string steamId;

	public int productId;

	public string language;

	public string country;

	public string itemShopDesc;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref steamId);
		stream.PutOrGet(ref productId);
		stream.PutOrGet(ref language);
		stream.PutOrGet(ref country);
		stream.PutOrGet(ref itemShopDesc);
	}
}
