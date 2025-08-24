using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_ACK)]
public sealed class NKMPacket_EVENT_PASS_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int totalExp;

	public int rewardNormalLevel;

	public int rewardCoreLevel;

	public bool isCorePassPurchased;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref totalExp);
		stream.PutOrGet(ref rewardNormalLevel);
		stream.PutOrGet(ref rewardCoreLevel);
		stream.PutOrGet(ref isCorePassPurchased);
	}
}
