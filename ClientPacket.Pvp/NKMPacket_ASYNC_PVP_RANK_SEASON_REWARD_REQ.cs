using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_REQ)]
public sealed class NKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
