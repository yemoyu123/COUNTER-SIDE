using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_START_SIMULATED_PVP_TEST_ACK)]
public sealed class NKMPacket_START_SIMULATED_PVP_TEST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public ReplayData replayData = new ReplayData();

	public PvpSingleHistory history;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref replayData);
		stream.PutOrGet(ref history);
	}
}
