using System.Collections.Generic;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMGameEndData : ISerializable
{
	public bool win;

	public bool giveup;

	public bool restart;

	public NKMDungeonClearData dungeonClearData;

	public NKMDeckIndex deckIndex;

	public NKMGameRecord gameRecord;

	public List<UnitLoyaltyUpdateData> updatedUnits = new List<UnitLoyaltyUpdateData>();

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	public long killCountDelta;

	public NKMKillCountData killCountData;

	public float totalPlayTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref win);
		stream.PutOrGet(ref giveup);
		stream.PutOrGet(ref restart);
		stream.PutOrGet(ref dungeonClearData);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref gameRecord);
		stream.PutOrGet(ref updatedUnits);
		stream.PutOrGet(ref costItemDataList);
		stream.PutOrGet(ref killCountDelta);
		stream.PutOrGet(ref killCountData);
		stream.PutOrGet(ref totalPlayTime);
	}
}
