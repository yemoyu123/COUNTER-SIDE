using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Item;

public sealed class NKMEquipPresetData : ISerializable
{
	public int presetIndex;

	public NKM_EQUIP_PRESET_TYPE presetType;

	public string presetName;

	public List<long> equipUids = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref presetIndex);
		stream.PutOrGetEnum(ref presetType);
		stream.PutOrGet(ref presetName);
		stream.PutOrGet(ref equipUids);
	}
}
