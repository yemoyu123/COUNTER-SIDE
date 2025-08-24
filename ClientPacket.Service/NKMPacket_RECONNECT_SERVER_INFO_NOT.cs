using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_RECONNECT_SERVER_INFO_NOT)]
public sealed class NKMPacket_RECONNECT_SERVER_INFO_NOT : ISerializable
{
	public string serverIp;

	public int port;

	public string accessToken;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref serverIp);
		stream.PutOrGet(ref port);
		stream.PutOrGet(ref accessToken);
	}
}
