using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_DOT_NOT)]
public sealed class NKMPacket_EVENT_PASS_DOT_NOT : ISerializable
{
	public bool passLevelDot;

	public bool dailyMissionDot;

	public bool weeklyMissionDot;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref passLevelDot);
		stream.PutOrGet(ref dailyMissionDot);
		stream.PutOrGet(ref weeklyMissionDot);
	}
}
