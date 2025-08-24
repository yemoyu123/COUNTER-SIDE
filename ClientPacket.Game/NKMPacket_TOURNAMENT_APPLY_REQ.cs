using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_APPLY_REQ)]
public sealed class NKMPacket_TOURNAMENT_APPLY_REQ : ISerializable
{
	public NKMDeckData deck;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deck);
	}
}
