using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISC_COLLECTION_REWARD_REQ)]
public sealed class NKMPacket_MISC_COLLECTION_REWARD_REQ : ISerializable
{
	public int miscId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref miscId);
	}
}
