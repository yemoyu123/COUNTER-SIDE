using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_DUNGEON_SKIP_REQ)]
public sealed class NKMPacket_DUNGEON_SKIP_REQ : ISerializable
{
	public int dungeonId;

	public int skip = 1;

	public List<long> unitUids = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref dungeonId);
		stream.PutOrGet(ref skip);
		stream.PutOrGet(ref unitUids);
	}
}
