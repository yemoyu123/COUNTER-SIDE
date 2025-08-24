using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_RESET_STAGE_PLAY_COUNT_REQ)]
public sealed class NKMPacket_RESET_STAGE_PLAY_COUNT_REQ : ISerializable
{
	public int stageId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
	}
}
