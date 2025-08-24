using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_CASTING_VOTE_SHIP_REQ)]
public sealed class NKMPacket_PVP_CASTING_VOTE_SHIP_REQ : ISerializable
{
	public List<int> shipGroupIdList = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shipGroupIdList);
	}
}
