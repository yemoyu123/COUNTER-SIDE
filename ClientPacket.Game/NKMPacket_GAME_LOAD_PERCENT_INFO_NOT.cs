using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_LOAD_PERCENT_INFO_NOT)]
public sealed class NKMPacket_GAME_LOAD_PERCENT_INFO_NOT : ISerializable
{
	public long userUID;

	public int percent;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUID);
		stream.PutOrGet(ref percent);
	}
}
