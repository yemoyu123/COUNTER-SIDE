using Cs.Protocol;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_REQ)]
public sealed class NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_REQ : ISerializable
{
	public int cityID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref cityID);
	}
}
