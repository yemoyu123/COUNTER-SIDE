using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_BUY_BUNDLE_TAB_REQ)]
public sealed class NKMPacket_SHOP_BUY_BUNDLE_TAB_REQ : ISerializable
{
	public string tabType;

	public int subIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tabType);
		stream.PutOrGet(ref subIndex);
	}
}
