using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_LOAD_COMPLETE_REQ)]
public sealed class NKMPacket_GAME_LOAD_COMPLETE_REQ : ISerializable
{
	public bool isIntrude;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isIntrude);
	}
}
