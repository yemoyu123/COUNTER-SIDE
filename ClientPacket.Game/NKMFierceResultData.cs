using Cs.Protocol;
using NKM;

namespace ClientPacket.Game;

public sealed class NKMFierceResultData : ISerializable
{
	public int hpPercent;

	public float restTime;

	public int accquirePoint;

	public int bestPoint;

	public NKMEventDeckData bestDeck;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref hpPercent);
		stream.PutOrGet(ref restTime);
		stream.PutOrGet(ref accquirePoint);
		stream.PutOrGet(ref bestPoint);
		stream.PutOrGet(ref bestDeck);
	}
}
