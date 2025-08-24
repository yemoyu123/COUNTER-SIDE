using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_FIERCE_SEASON_NOT)]
public sealed class NKMPacket_FIERCE_SEASON_NOT : ISerializable
{
	public int fierceId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref fierceId);
	}
}
