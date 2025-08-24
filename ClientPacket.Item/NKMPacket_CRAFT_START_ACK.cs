using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CRAFT_START_ACK)]
public sealed class NKMPacket_CRAFT_START_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMCraftSlotData craftSlotData;

	public List<NKMItemMiscData> materialItemDataList = new List<NKMItemMiscData>();

	public NKMResetCount resetCount = new NKMResetCount();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref craftSlotData);
		stream.PutOrGet(ref materialItemDataList);
		stream.PutOrGet(ref resetCount);
	}
}
