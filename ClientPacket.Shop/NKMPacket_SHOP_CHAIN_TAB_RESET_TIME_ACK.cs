using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_CHAIN_TAB_RESET_TIME_ACK)]
public sealed class NKMPacket_SHOP_CHAIN_TAB_RESET_TIME_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<ShopChainTabNextResetData> list = new List<ShopChainTabNextResetData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref list);
	}
}
