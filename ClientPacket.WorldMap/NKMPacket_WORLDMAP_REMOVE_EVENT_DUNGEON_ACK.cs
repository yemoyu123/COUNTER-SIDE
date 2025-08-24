using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK)]
public sealed class NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int cityID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref cityID);
	}
}
