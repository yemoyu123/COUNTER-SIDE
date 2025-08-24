using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_CLEAR_REQ)]
public sealed class NKMPacket_EQUIP_PRESET_CLEAR_REQ : ISerializable
{
	public List<int> presetIndices = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref presetIndices);
	}
}
