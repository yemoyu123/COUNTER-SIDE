using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_LEAGUE_PVP_MATCH_REQ)]
public sealed class NKMPacket_LEAGUE_PVP_MATCH_REQ : ISerializable
{
	public byte selectDeckIndex;

	public NKM_GAME_TYPE gameType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selectDeckIndex);
		stream.PutOrGetEnum(ref gameType);
	}
}
