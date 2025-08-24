using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_ASYNC_PVP_RANK_LIST_ACK)]
public sealed class NKMPacket_ASYNC_PVP_RANK_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public RANK_TYPE rankType;

	public bool isAll;

	public List<NKMUserSimpleProfileData> userProfileDataList = new List<NKMUserSimpleProfileData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref rankType);
		stream.PutOrGet(ref isAll);
		stream.PutOrGet(ref userProfileDataList);
	}
}
