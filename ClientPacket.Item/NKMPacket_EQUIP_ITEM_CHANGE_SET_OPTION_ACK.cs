using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_ACK)]
public sealed class NKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long equipUID;

	public int setOptionId;

	public List<NKMItemMiscData> costItemData = new List<NKMItemMiscData>();

	public NKMEquipTuningCandidate equipTuningCandidate = new NKMEquipTuningCandidate();

	public NKMResetCount resetCount = new NKMResetCount();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipUID);
		stream.PutOrGet(ref setOptionId);
		stream.PutOrGet(ref costItemData);
		stream.PutOrGet(ref equipTuningCandidate);
		stream.PutOrGet(ref resetCount);
	}
}
