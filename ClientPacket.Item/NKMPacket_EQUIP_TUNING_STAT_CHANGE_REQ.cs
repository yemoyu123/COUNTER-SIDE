using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_TUNING_STAT_CHANGE_REQ)]
public sealed class NKMPacket_EQUIP_TUNING_STAT_CHANGE_REQ : ISerializable
{
	public long equipUID;

	public int equipOptionID = -1;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipUID);
		stream.PutOrGet(ref equipOptionID);
	}
}
