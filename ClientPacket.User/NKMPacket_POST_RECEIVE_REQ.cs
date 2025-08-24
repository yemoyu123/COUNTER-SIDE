using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_POST_RECEIVE_REQ)]
public sealed class NKMPacket_POST_RECEIVE_REQ : ISerializable
{
	public long postIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref postIndex);
	}
}
