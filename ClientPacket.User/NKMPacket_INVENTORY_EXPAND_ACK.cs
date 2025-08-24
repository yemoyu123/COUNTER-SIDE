using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_INVENTORY_EXPAND_ACK)]
public sealed class NKMPacket_INVENTORY_EXPAND_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKM_INVENTORY_EXPAND_TYPE inventoryExpandType;

	public int expandedCount;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref inventoryExpandType);
		stream.PutOrGet(ref expandedCount);
		stream.PutOrGet(ref costItemDataList);
	}
}
