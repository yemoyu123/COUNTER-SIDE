using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CRAFT_UNLOCK_SLOT_ACK)]
public sealed class NKMPacket_CRAFT_UNLOCK_SLOT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMCraftSlotData craftSlotData;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref craftSlotData);
		stream.PutOrGet(ref costItemDataList);
	}
}
