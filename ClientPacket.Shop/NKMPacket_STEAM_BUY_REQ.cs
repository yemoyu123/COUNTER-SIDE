using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_STEAM_BUY_REQ)]
public sealed class NKMPacket_STEAM_BUY_REQ : ISerializable
{
	public string steamId;

	public string orderId;

	public int productId;

	public string country;

	public string currency;

	public List<int> selectIndices = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref steamId);
		stream.PutOrGet(ref orderId);
		stream.PutOrGet(ref productId);
		stream.PutOrGet(ref country);
		stream.PutOrGet(ref currency);
		stream.PutOrGet(ref selectIndices);
	}
}
