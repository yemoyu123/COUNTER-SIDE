using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMDungeonSupportData : ISerializable
{
	public long userUid;

	public NKMAsyncUnitEquipData asyncUnitEquip = new NKMAsyncUnitEquipData();

	public NKMDeckIndex deckIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref asyncUnitEquip);
		stream.PutOrGet(ref deckIndex);
	}
}
