using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_TEN_RECORD_NOT)]
public sealed class NKMPACKET_EVENT_TEN_RECORD_NOT : ISerializable
{
	public int templetId;

	public int score;

	public int remainTime;

	public List<int> scoreRewardIds = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
		stream.PutOrGet(ref score);
		stream.PutOrGet(ref remainTime);
		stream.PutOrGet(ref scoreRewardIds);
	}
}
