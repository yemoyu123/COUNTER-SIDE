using Cs.Protocol;

namespace ClientPacket.Mode;

public sealed class NKMPalaceDungeonData : ISerializable
{
	public int dungeonId;

	public int recentTime;

	public int bestTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref dungeonId);
		stream.PutOrGet(ref recentTime);
		stream.PutOrGet(ref bestTime);
	}
}
