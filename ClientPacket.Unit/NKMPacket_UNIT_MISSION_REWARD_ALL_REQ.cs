using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_UNIT_MISSION_REWARD_ALL_REQ)]
public sealed class NKMPacket_UNIT_MISSION_REWARD_ALL_REQ : ISerializable
{
	public int unitId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitId);
	}
}
