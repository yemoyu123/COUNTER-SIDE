using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ)]
public sealed class NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ : ISerializable
{
	public sealed class PresetIndexData : ISerializable
	{
		public int beforeIndex;

		public int afterIndex;

		void ISerializable.Serialize(IPacketStream stream)
		{
			stream.PutOrGet(ref beforeIndex);
			stream.PutOrGet(ref afterIndex);
		}
	}

	public List<PresetIndexData> changeIndices = new List<PresetIndexData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref changeIndices);
	}
}
