using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_REFRESH_ACK)]
public sealed class NKMPacket_SHOP_REFRESH_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMShopRandomData randomShopData;

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref randomShopData);
		stream.PutOrGet(ref costItemData);
	}
}
