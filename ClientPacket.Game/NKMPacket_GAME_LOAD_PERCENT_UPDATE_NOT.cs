using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_LOAD_PERCENT_UPDATE_NOT)]
public sealed class NKMPacket_GAME_LOAD_PERCENT_UPDATE_NOT : ISerializable
{
	public int percent;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref percent);
	}
}
