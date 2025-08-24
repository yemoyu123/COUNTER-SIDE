using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_ACK)]
public sealed class NKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMEquipItemData equipItemData;

	public NKMEquipTuningCandidate equipTuningCandidate = new NKMEquipTuningCandidate();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipItemData);
		stream.PutOrGet(ref equipTuningCandidate);
	}
}
