using Cs.Protocol;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_SET_COOP_REQ)]
public sealed class NKMPacket_RAID_SET_COOP_REQ : ISerializable
{
	public long raidUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref raidUID);
	}
}
