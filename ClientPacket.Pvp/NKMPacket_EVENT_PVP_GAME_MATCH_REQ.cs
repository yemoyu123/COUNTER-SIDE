using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PVP_GAME_MATCH_REQ)]
public sealed class NKMPacket_EVENT_PVP_GAME_MATCH_REQ : ISerializable
{
	public int seasonId;

	public NKMEventDeckData eventDeckData;

	public NKM_GAME_TYPE gameType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref seasonId);
		stream.PutOrGet(ref eventDeckData);
		stream.PutOrGetEnum(ref gameType);
	}
}
