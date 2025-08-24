using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_PRACTICE_GAME_LOAD_REQ)]
public sealed class NKMPacket_PRACTICE_GAME_LOAD_REQ : ISerializable
{
	public NKMUnitData practiceUnitData;

	public int dungeonID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref practiceUnitData);
		stream.PutOrGet(ref dungeonID);
	}
}
