using Cs.Protocol;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_DETAIL_INFO_REQ)]
public sealed class NKMPacket_RAID_DETAIL_INFO_REQ : ISerializable
{
	public long raidUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref raidUID);
	}
}
