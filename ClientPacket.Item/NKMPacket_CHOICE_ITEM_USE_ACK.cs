using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CHOICE_ITEM_USE_ACK)]
public sealed class NKMPacket_CHOICE_ITEM_USE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMItemMiscData costItemData;

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref costItemData);
		stream.PutOrGet(ref rewardData);
	}
}
