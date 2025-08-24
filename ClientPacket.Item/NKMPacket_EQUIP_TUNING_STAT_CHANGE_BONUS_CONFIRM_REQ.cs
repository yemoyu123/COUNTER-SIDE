using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ)]
public sealed class NKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ : ISerializable
{
	public long equipUid;

	public int equipOptionId = -1;

	public NKM_STAT_TYPE statType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipUid);
		stream.PutOrGet(ref equipOptionId);
		stream.PutOrGetEnum(ref statType);
	}
}
