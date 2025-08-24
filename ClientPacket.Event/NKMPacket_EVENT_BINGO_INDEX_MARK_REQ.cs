using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_BINGO_INDEX_MARK_REQ)]
public sealed class NKMPacket_EVENT_BINGO_INDEX_MARK_REQ : ISerializable
{
	public int eventId;

	public HashSet<int> hsTileIndex = new HashSet<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventId);
		stream.PutOrGet(ref hsTileIndex);
	}
}
