using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_BAR_GET_REWARD_ACK)]
public sealed class NKMPacket_EVENT_BAR_GET_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	public int remainDeliveryLimitValue;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref costItems);
		stream.PutOrGet(ref remainDeliveryLimitValue);
	}
}
