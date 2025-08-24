using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Warfare;

public sealed class WarfareSyncData : ISerializable
{
	public sealed class MovedUnit : ISerializable
	{
		public int unitUID;

		public short tileIndex;

		void ISerializable.Serialize(IPacketStream stream)
		{
			stream.PutOrGet(ref unitUID);
			stream.PutOrGet(ref tileIndex);
		}
	}

	public List<WarfareUnitSyncData> updatedUnits = new List<WarfareUnitSyncData>();

	public List<MovedUnit> movedUnits = new List<MovedUnit>();

	public List<WarfareUnitData> newUnits = new List<WarfareUnitData>();

	public List<int> retreaters = new List<int>();

	public List<WarfareTileData> tiles = new List<WarfareTileData>();

	public WarfareGameSyncData gameState = new WarfareGameSyncData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref updatedUnits);
		stream.PutOrGet(ref movedUnits);
		stream.PutOrGet(ref newUnits);
		stream.PutOrGet(ref retreaters);
		stream.PutOrGet(ref tiles);
		stream.PutOrGet(ref gameState);
	}
}
