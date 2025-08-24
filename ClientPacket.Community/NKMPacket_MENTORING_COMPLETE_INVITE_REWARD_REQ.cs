using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_COMPLETE_INVITE_REWARD_REQ)]
public sealed class NKMPacket_MENTORING_COMPLETE_INVITE_REWARD_REQ : ISerializable
{
	public int inviteSuccessRequireCnt;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref inviteSuccessRequireCnt);
	}
}
