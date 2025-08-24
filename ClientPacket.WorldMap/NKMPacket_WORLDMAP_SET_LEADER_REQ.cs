using Cs.Protocol;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_SET_LEADER_REQ)]
public sealed class NKMPacket_WORLDMAP_SET_LEADER_REQ : ISerializable
{
	public int cityID;

	public long leaderUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref leaderUID);
	}
}
