using System.Collections.Generic;
using ClientPacket.Community;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_FRIEND_LIST_ACK)]
public sealed class NKMPacket_WARFARE_FRIEND_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<WarfareSupporterListData> friends = new List<WarfareSupporterListData>();

	public List<WarfareSupporterListData> guests = new List<WarfareSupporterListData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref friends);
		stream.PutOrGet(ref guests);
	}
}
