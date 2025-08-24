using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class EventInfo : ISerializable
{
	public List<BingoInfo> bingoInfo = new List<BingoInfo>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref bingoInfo);
	}
}
