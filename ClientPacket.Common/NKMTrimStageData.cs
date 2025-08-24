using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMTrimStageData : ISerializable
{
	public int index;

	public int dungeonId;

	public int score;

	public bool isWin;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref index);
		stream.PutOrGet(ref dungeonId);
		stream.PutOrGet(ref score);
		stream.PutOrGet(ref isWin);
	}
}
