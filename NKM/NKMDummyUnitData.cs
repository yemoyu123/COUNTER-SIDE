using Cs.Protocol;

namespace NKM;

public sealed class NKMDummyUnitData : ISerializable
{
	public int UnitId;

	public int UnitLevel;

	public int SkinId;

	public short LimitBreakLevel;

	public int TacticLevel;

	public int ReactorLevel;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref UnitId);
		stream.PutOrGet(ref UnitLevel);
		stream.PutOrGet(ref SkinId);
		stream.PutOrGet(ref LimitBreakLevel);
		stream.PutOrGet(ref TacticLevel);
		stream.PutOrGet(ref ReactorLevel);
	}

	public NKMUnitData ToUnitData(long unitUid = -1L)
	{
		return NKMDungeonManager.MakeUnitDataFromID(UnitId, unitUid, UnitLevel, LimitBreakLevel, SkinId, TacticLevel, ReactorLevel);
	}

	public GameUnitData ToGameUnitData(long unitUid)
	{
		return new GameUnitData
		{
			unit = ToUnitData(unitUid)
		};
	}
}
