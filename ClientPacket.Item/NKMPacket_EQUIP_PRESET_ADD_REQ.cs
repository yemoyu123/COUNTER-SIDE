using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_ADD_REQ)]
public sealed class NKMPacket_EQUIP_PRESET_ADD_REQ : ISerializable
{
	public int addPresetCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref addPresetCount);
	}
}
