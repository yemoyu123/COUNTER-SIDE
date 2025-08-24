using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_TUNING_NOT)]
public sealed class NKMPacket_EQUIP_TUNING_NOT : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMEquipTuningCandidate equipTuningCandidate = new NKMEquipTuningCandidate();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipTuningCandidate);
	}
}
