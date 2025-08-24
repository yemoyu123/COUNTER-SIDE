using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_MINI_GAME_LIST_NOT)]
public sealed class NKMPacket_MINI_GAME_LIST_NOT : ISerializable
{
	public List<int> templetIds = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetIds);
	}
}
