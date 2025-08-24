using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_TRIM_START_REQ)]
public sealed class NKMPacket_TRIM_START_REQ : ISerializable
{
	public int trimId;

	public int trimLevel;

	public List<NKMEventDeckData> eventDeckList = new List<NKMEventDeckData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref trimId);
		stream.PutOrGet(ref trimLevel);
		stream.PutOrGet(ref eventDeckList);
	}
}
