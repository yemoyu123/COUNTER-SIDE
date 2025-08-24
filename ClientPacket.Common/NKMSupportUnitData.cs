using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMSupportUnitData : ISerializable
{
	public long userUid;

	public NKMAsyncUnitEquipData asyncUnitEquip = new NKMAsyncUnitEquipData();

	public long usedCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref asyncUnitEquip);
		stream.PutOrGet(ref usedCount);
	}
}
