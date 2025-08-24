using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMFierceBoss : ISerializable
{
	public int bossId;

	public int point;

	public int rankNumber;

	public int rankPercent;

	public NKMEventDeckData deckData;

	public bool isCleared;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref bossId);
		stream.PutOrGet(ref point);
		stream.PutOrGet(ref rankNumber);
		stream.PutOrGet(ref rankPercent);
		stream.PutOrGet(ref deckData);
		stream.PutOrGet(ref isCleared);
	}
}
