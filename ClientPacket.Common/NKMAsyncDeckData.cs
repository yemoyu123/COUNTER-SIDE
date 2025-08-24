using System.Collections.Generic;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMAsyncDeckData : ISerializable
{
	public short leaderIndex;

	public NKMAsyncUnitData ship = new NKMAsyncUnitData();

	public List<NKMAsyncUnitData> units = new List<NKMAsyncUnitData>();

	public List<NKMEquipItemData> equips = new List<NKMEquipItemData>();

	public int operationPower;

	public NKMOperator operatorUnit;

	public NKMAsyncUnitData banishedUnit = new NKMAsyncUnitData();

	public Dictionary<int, NKMUnitUpData> unitUpData = new Dictionary<int, NKMUnitUpData>();

	public Dictionary<int, NKMBanData> unitBanData = new Dictionary<int, NKMBanData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref leaderIndex);
		stream.PutOrGet(ref ship);
		stream.PutOrGet(ref units);
		stream.PutOrGet(ref equips);
		stream.PutOrGet(ref operationPower);
		stream.PutOrGet(ref operatorUnit);
		stream.PutOrGet(ref banishedUnit);
		stream.PutOrGet(ref unitUpData);
		stream.PutOrGet(ref unitBanData);
	}
}
