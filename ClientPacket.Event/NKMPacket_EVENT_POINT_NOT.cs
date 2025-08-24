using Cs.Protocol;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_POINT_NOT)]
public sealed class NKMPacket_EVENT_POINT_NOT : ISerializable
{
	public long totalEventPoint;

	public NKMRewardData additionalReward;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref totalEventPoint);
		stream.PutOrGet(ref additionalReward);
	}
}
