using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_NOT)]
public sealed class NKMPacket_EQUIP_PRESET_NOT : ISerializable
{
	public List<NKMEquipPresetData> presetDatas = new List<NKMEquipPresetData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref presetDatas);
	}
}
