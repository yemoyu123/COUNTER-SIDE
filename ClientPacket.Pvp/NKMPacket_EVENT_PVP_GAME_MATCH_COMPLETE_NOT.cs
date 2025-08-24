using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PVP_GAME_MATCH_COMPLETE_NOT)]
public sealed class NKMPacket_EVENT_PVP_GAME_MATCH_COMPLETE_NOT : ISerializable
{
	public NKMGameData gameData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref gameData);
	}
}
