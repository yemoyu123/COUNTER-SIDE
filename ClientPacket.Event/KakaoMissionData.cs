using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class KakaoMissionData : ISerializable
{
	public int eventId;

	public KakaoMissionState state;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventId);
		stream.PutOrGetEnum(ref state);
	}
}
