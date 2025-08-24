using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_CHANGE_NAME_ACK)]
public sealed class NKMPacket_EQUIP_PRESET_CHANGE_NAME_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int presetIndex;

	public string newPresetName;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref presetIndex);
		stream.PutOrGet(ref newPresetName);
	}
}
