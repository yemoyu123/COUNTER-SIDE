using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_BLOCK_REQ)]
public sealed class NKMPacket_FRIEND_BLOCK_REQ : ISerializable
{
	public long friendCode;

	public bool isCancel;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref friendCode);
		stream.PutOrGet(ref isCancel);
	}
}
