using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_MINI_GAME_RESULT_REQ)]
public sealed class NKMPacket_MINI_GAME_RESULT_REQ : ISerializable
{
	public NKMMiniGameData miniGameData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref miniGameData);
	}
}
