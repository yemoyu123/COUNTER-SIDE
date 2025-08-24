using System;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CHARGE_ITEM_NOT)]
public sealed class NKMPacket_CHARGE_ITEM_NOT : ISerializable
{
	public DateTime lastUpdateDate;

	public NKMItemMiscData itemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref lastUpdateDate);
		stream.PutOrGet(ref itemData);
	}
}
