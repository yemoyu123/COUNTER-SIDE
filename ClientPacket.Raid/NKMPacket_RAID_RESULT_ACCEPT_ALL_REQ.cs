using Cs.Protocol;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_RESULT_ACCEPT_ALL_REQ)]
public sealed class NKMPacket_RAID_RESULT_ACCEPT_ALL_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
