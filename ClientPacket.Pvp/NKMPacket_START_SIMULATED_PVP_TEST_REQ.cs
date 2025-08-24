using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_START_SIMULATED_PVP_TEST_REQ)]
public sealed class NKMPacket_START_SIMULATED_PVP_TEST_REQ : ISerializable
{
	public long playerUserUidA;

	public long playerUserUidB;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref playerUserUidA);
		stream.PutOrGet(ref playerUserUidB);
	}
}
