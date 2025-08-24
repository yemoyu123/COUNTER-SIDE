using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_ADD_ACK)]
public sealed class NKMPacket_MENTORING_ADD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public MentoringIdentity identity;

	public FriendListData mentoringData = new FriendListData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref identity);
		stream.PutOrGet(ref mentoringData);
	}
}
