using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_POST_ARRIVE_NOT)]
public sealed class NKMPacket_POST_ARRIVE_NOT : ISerializable
{
	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref count);
	}
}
