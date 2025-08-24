using Cs.Protocol;

namespace ClientPacket.WorldMap;

public sealed class NKMWorldmapCityBuildingData : ISerializable
{
	public long buildUID;

	public int id;

	public int level;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref buildUID);
		stream.PutOrGet(ref id);
		stream.PutOrGet(ref level);
	}
}
