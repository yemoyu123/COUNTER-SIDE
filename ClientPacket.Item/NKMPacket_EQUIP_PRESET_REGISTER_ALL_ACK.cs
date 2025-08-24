using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_REGISTER_ALL_ACK)]
public sealed class NKMPacket_EQUIP_PRESET_REGISTER_ALL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMEquipPresetData presetData = new NKMEquipPresetData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref presetData);
	}
}
