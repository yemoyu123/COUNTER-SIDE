using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMRankData : ISerializable
{
	public int rank;

	public long score;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref rank);
		stream.PutOrGet(ref score);
	}
}
