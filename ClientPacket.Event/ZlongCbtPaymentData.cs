using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class ZlongCbtPaymentData : ISerializable
{
	public double totalPayment;

	public long rewardCount;

	public ZlongCbtPaymentState state;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref totalPayment);
		stream.PutOrGet(ref rewardCount);
		stream.PutOrGetEnum(ref state);
	}
}
