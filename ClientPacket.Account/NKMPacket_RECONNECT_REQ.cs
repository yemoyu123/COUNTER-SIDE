using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_RECONNECT_REQ)]
public sealed class NKMPacket_RECONNECT_REQ : ISerializable
{
	public string reconnectKey;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref reconnectKey);
	}
}
