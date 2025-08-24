using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_INFO_ACK)]
public sealed class NKMPacket_WORLDMAP_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMWorldMapData worldMapData = new NKMWorldMapData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref worldMapData);
	}
}
