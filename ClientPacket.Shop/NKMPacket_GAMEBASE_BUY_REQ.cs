using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_GAMEBASE_BUY_REQ)]
public sealed class NKMPacket_GAMEBASE_BUY_REQ : ISerializable
{
	public string paymentSeq;

	public string accessToken;

	public List<int> selectIndices = new List<int>();

	public string paymentId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref paymentSeq);
		stream.PutOrGet(ref accessToken);
		stream.PutOrGet(ref selectIndices);
		stream.PutOrGet(ref paymentId);
	}
}
