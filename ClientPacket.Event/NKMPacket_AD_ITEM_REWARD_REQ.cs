using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_AD_ITEM_REWARD_REQ)]
public sealed class NKMPacket_AD_ITEM_REWARD_REQ : ISerializable
{
	public int aditemId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref aditemId);
	}
}
