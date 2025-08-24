using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_UPDATE_RECONNECT_KEY_NOT)]
public sealed class NKMPacket_UPDATE_RECONNECT_KEY_NOT : ISerializable
{
	public string reconnectKey;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref reconnectKey);
	}
}
