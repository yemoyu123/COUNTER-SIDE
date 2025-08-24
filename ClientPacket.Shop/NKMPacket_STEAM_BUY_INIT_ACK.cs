using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_STEAM_BUY_INIT_ACK)]
public sealed class NKMPacket_STEAM_BUY_INIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int productId;

	public string orderId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref productId);
		stream.PutOrGet(ref orderId);
	}
}
