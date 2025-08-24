using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_PHASE_START_REQ)]
public sealed class NKMPacket_PHASE_START_REQ : ISerializable
{
	public int stageId;

	public NKMDeckIndex deckIndex;

	public NKMEventDeckData eventDeckData;

	public long supportingUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref eventDeckData);
		stream.PutOrGet(ref supportingUserUid);
	}
}
