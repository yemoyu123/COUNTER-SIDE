using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK)]
public sealed class NKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int rewardNormalLevel;

	public int rewardCoreLevel;

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardNormalLevel);
		stream.PutOrGet(ref rewardCoreLevel);
		stream.PutOrGet(ref rewardData);
	}
}
