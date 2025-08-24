using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_CUTSCENE_DUNGEON_START_REQ)]
public sealed class NKMPacket_CUTSCENE_DUNGEON_START_REQ : ISerializable
{
	public int dungeonID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref dungeonID);
	}
}
