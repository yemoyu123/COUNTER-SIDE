using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TRIM_DUNGEON_SKIP_REQ)]
public sealed class NKMPacket_TRIM_DUNGEON_SKIP_REQ : ISerializable
{
	public int trimId;

	public int trimLevel;

	public int skipCount;

	public List<NKMEventDeckData> eventDeckList = new List<NKMEventDeckData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref trimId);
		stream.PutOrGet(ref trimLevel);
		stream.PutOrGet(ref skipCount);
		stream.PutOrGet(ref eventDeckList);
	}
}
