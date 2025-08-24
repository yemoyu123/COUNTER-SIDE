using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_BINGO_RANDOM_MARK_ACK)]
public sealed class NKMPacket_EVENT_BINGO_RANDOM_MARK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int eventId;

	public NKMItemMiscData costItemData;

	public int mileage;

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref eventId);
		stream.PutOrGet(ref costItemData);
		stream.PutOrGet(ref mileage);
		stream.PutOrGet(ref rewardData);
	}
}
