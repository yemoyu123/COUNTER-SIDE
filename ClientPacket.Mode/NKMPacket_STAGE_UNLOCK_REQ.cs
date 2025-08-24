using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_STAGE_UNLOCK_REQ)]
public sealed class NKMPacket_STAGE_UNLOCK_REQ : ISerializable
{
	public int stageId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
	}
}
