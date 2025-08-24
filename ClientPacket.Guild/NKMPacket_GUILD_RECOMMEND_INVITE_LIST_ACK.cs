using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_RECOMMEND_INVITE_LIST_ACK)]
public sealed class NKMPacket_GUILD_RECOMMEND_INVITE_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<FriendListData> list = new List<FriendListData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref list);
	}
}
