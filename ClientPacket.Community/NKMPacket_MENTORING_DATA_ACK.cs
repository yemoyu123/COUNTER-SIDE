using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_DATA_ACK)]
public sealed class NKMPacket_MENTORING_DATA_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public FriendListData mentorData = new FriendListData();

	public bool isGraduated;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref mentorData);
		stream.PutOrGet(ref isGraduated);
	}
}
