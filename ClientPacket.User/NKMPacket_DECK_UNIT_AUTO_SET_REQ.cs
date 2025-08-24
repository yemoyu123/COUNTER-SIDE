using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_UNIT_AUTO_SET_REQ)]
public sealed class NKMPacket_DECK_UNIT_AUTO_SET_REQ : ISerializable
{
	public NKMDeckIndex deckIndex;

	public List<long> unitUIDList = new List<long>();

	public long shipUID;

	public long operatorUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref unitUIDList);
		stream.PutOrGet(ref shipUID);
		stream.PutOrGet(ref operatorUid);
	}
}
