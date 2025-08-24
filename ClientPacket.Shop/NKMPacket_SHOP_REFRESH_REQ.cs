using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_REFRESH_REQ)]
public sealed class NKMPacket_SHOP_REFRESH_REQ : ISerializable
{
	public bool isUseCash;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isUseCash);
	}
}
