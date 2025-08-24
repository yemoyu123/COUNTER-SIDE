using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CRAFT_COMPLETE_ACK)]
public sealed class NKMPacket_CRAFT_COMPLETE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMCraftSlotData craftSlotData;

	public NKMRewardData createdRewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref craftSlotData);
		stream.PutOrGet(ref createdRewardData);
	}
}
