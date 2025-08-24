using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_RAID_SWEEP_REQ)]
public sealed class NKMPacket_RAID_SWEEP_REQ : ISerializable
{
	public long raidUid;

	public bool isTryAssist;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref raidUid);
		stream.PutOrGet(ref isTryAssist);
	}
}
