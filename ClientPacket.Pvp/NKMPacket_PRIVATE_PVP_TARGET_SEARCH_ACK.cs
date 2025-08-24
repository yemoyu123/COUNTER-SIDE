using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_TARGET_SEARCH_ACK)]
public sealed class NKMPacket_PRIVATE_PVP_TARGET_SEARCH_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<FriendListData> list = new List<FriendListData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref list);
	}
}
