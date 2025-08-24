using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_POST_RECV_ACK)]
public sealed class NKMPacket_OFFICE_POST_RECV_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public List<NKMOfficePost> postList = new List<NKMOfficePost>();

	public int postCount;

	public NKMOfficePostState postState = new NKMOfficePostState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref postList);
		stream.PutOrGet(ref postCount);
		stream.PutOrGet(ref postState);
	}
}
