using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_RACE_RESET_NOT)]
public sealed class NKMPACKET_RACE_RESET_NOT : ISerializable
{
	public int currentRaceId;

	public int currentRaceIndex;

	public NKMRaceSummary summary = new NKMRaceSummary();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref currentRaceId);
		stream.PutOrGet(ref currentRaceIndex);
		stream.PutOrGet(ref summary);
	}
}
