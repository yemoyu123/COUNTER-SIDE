using Cs.Protocol;

namespace ClientPacket.Warfare;

public sealed class WarfareUnitSyncData : ISerializable
{
	public int warfareGameUnitUID;

	public float hp;

	public bool isTurnEnd;

	public byte supply;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref warfareGameUnitUID);
		stream.PutOrGet(ref hp);
		stream.PutOrGet(ref isTurnEnd);
		stream.PutOrGet(ref supply);
	}
}
