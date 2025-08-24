using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_FAVORITES_STAGE_DELETE_REQ)]
public sealed class NKMPacket_FAVORITES_STAGE_DELETE_REQ : ISerializable
{
	public int stageId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
	}
}
