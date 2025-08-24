using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Community;

public sealed class MenteeInfo : ISerializable
{
	public MentoringState state;

	public FriendListData data;

	public long missionCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref state);
		stream.PutOrGet(ref data);
		stream.PutOrGet(ref missionCount);
	}
}
