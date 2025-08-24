using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_CHANGE_NAME_REQ)]
public sealed class NKMPacket_EQUIP_PRESET_CHANGE_NAME_REQ : ISerializable
{
	public int presetIndex;

	public string newPresetName;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref presetIndex);
		stream.PutOrGet(ref newPresetName);
	}
}
