using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_BET_SELECT_TEAM_REQ)]
public sealed class NKMPACKET_EVENT_BET_SELECT_TEAM_REQ : ISerializable
{
	public EventBetTeam selectTeam;

	public int betCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref selectTeam);
		stream.PutOrGet(ref betCount);
	}
}
