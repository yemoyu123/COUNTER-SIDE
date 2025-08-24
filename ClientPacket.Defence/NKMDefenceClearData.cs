using Cs.Protocol;

namespace ClientPacket.Defence;

public sealed class NKMDefenceClearData : ISerializable
{
	public int defenceTempletId;

	public int gameScore;

	public int bestScore;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref defenceTempletId);
		stream.PutOrGet(ref gameScore);
		stream.PutOrGet(ref bestScore);
	}
}
