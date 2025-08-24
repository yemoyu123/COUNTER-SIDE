using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_RACE_TEAM_SELECT_REQ)]
public sealed class NKMPACKET_RACE_TEAM_SELECT_REQ : ISerializable
{
	public RaceTeam selectTeam;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref selectTeam);
	}
}
