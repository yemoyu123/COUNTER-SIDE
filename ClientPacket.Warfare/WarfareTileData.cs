using Cs.Protocol;
using NKM.Templet;

namespace ClientPacket.Warfare;

public sealed class WarfareTileData : ISerializable
{
	public short index;

	public NKM_WARFARE_MAP_TILE_TYPE tileType;

	public int battleConditionId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref index);
		stream.PutOrGetEnum(ref tileType);
		stream.PutOrGet(ref battleConditionId);
	}
}
