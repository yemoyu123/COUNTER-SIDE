using System.Collections.Generic;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMAsyncUnitEquipData : ISerializable
{
	public NKMAsyncUnitData asyncUnit = new NKMAsyncUnitData();

	public List<NKMEquipItemData> equips = new List<NKMEquipItemData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref asyncUnit);
		stream.PutOrGet(ref equips);
	}
}
