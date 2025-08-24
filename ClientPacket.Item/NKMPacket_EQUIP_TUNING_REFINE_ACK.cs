using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_TUNING_REFINE_ACK)]
public sealed class NKMPacket_EQUIP_TUNING_REFINE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKM_EQUIP_REFINE_RESULT equipRefineResult;

	public int precision;

	public NKMEquipItemData equipItemData;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref equipRefineResult);
		stream.PutOrGet(ref precision);
		stream.PutOrGet(ref equipItemData);
		stream.PutOrGet(ref costItemDataList);
	}
}
