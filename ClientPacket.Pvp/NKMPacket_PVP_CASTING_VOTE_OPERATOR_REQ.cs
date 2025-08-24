using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_CASTING_VOTE_OPERATOR_REQ)]
public sealed class NKMPacket_PVP_CASTING_VOTE_OPERATOR_REQ : ISerializable
{
	public List<int> operatorIdList = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref operatorIdList);
	}
}
