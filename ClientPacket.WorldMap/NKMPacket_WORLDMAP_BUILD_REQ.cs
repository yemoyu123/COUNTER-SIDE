using Cs.Protocol;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_BUILD_REQ)]
public sealed class NKMPacket_WORLDMAP_BUILD_REQ : ISerializable
{
	public int cityID;

	public int buildID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref buildID);
	}
}
