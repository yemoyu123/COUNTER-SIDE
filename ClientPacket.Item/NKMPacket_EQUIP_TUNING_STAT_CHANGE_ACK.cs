using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK)]
public sealed class NKMPacket_EQUIP_TUNING_STAT_CHANGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int equipOptionID;

	public NKMEquipItemData equipItemData;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	public NKMEquipTuningCandidate equipTuningCandidate = new NKMEquipTuningCandidate();

	public NKMResetCount resetCount = new NKMResetCount();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipOptionID);
		stream.PutOrGet(ref equipItemData);
		stream.PutOrGet(ref costItemDataList);
		stream.PutOrGet(ref equipTuningCandidate);
		stream.PutOrGet(ref resetCount);
	}
}
