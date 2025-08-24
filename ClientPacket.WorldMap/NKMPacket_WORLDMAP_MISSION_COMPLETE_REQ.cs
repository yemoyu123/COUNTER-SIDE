using Cs.Protocol;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_MISSION_COMPLETE_REQ)]
public sealed class NKMPacket_WORLDMAP_MISSION_COMPLETE_REQ : ISerializable
{
	public int cityID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref cityID);
	}
}
