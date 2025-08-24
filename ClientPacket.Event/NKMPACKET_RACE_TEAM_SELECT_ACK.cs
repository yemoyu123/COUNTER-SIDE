using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_RACE_TEAM_SELECT_ACK)]
public sealed class NKMPACKET_RACE_TEAM_SELECT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRacePrivate racePrivate = new NKMRacePrivate();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref racePrivate);
	}
}
