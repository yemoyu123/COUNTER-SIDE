using Cs.Protocol;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_GAME_MOVE_REQ)]
public sealed class NKMPacket_WARFARE_GAME_MOVE_REQ : ISerializable
{
	public byte tileIndexFrom;

	public byte tileIndexTo;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tileIndexFrom);
		stream.PutOrGet(ref tileIndexTo);
	}
}
