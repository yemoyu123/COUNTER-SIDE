using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_KILL_COUNT_SERVER_REWARD_REQ)]
public sealed class NKMPacket_KILL_COUNT_SERVER_REWARD_REQ : ISerializable
{
	public int templetId;

	public int stepId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
		stream.PutOrGet(ref stepId);
	}
}
