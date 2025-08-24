using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_ENEMY_READY_NOT)]
public sealed class NKMPacket_PRIVATE_PVP_ENEMY_READY_NOT : ISerializable
{
	public NKMDummyDeckData deckData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckData);
	}
}
