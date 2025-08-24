using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_HEART_BIT_REQ)]
public sealed class NKMPacket_HEART_BIT_REQ : ISerializable
{
	public long time;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref time);
	}
}
