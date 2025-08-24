using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_ADD_ACK)]
public sealed class NKMPacket_EQUIP_PRESET_ADD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int totalPresetCount;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref totalPresetCount);
		stream.PutOrGet(ref costItemDataList);
	}
}
