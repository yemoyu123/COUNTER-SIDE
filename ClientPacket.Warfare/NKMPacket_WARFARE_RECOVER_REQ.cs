using Cs.Protocol;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_RECOVER_REQ)]
public sealed class NKMPacket_WARFARE_RECOVER_REQ : ISerializable
{
	public byte deckIndex;

	public short tileIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref tileIndex);
	}
}
