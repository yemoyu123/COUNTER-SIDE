using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_LIST_ACK)]
public sealed class NKMPacket_EQUIP_PRESET_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMEquipPresetData> presetDatas = new List<NKMEquipPresetData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref presetDatas);
	}
}
