using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_SKIP_REQ)]
public sealed class NKMPacket_DIVE_SKIP_REQ : ISerializable
{
	public int stageId;

	public int skipCount;

	public int cityId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
		stream.PutOrGet(ref skipCount);
		stream.PutOrGet(ref cityId);
	}
}
