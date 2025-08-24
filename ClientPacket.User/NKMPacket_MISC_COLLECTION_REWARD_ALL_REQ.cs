using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISC_COLLECTION_REWARD_ALL_REQ)]
public sealed class NKMPacket_MISC_COLLECTION_REWARD_ALL_REQ : ISerializable
{
	public NKM_ITEM_MISC_TYPE miscType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref miscType);
	}
}
