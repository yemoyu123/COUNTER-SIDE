using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PVP_REWARD_REQ)]
public sealed class NKMPacket_EVENT_PVP_REWARD_REQ : ISerializable
{
	public int seasonId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref seasonId);
	}
}
