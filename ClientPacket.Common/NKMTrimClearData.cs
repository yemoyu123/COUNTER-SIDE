using Cs.Protocol;
using NKM.Templet;

namespace ClientPacket.Common;

public sealed class NKMTrimClearData : ISerializable
{
	public bool isWin;

	public int trimId;

	public int trimLevel;

	public int score;

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isWin);
		stream.PutOrGet(ref trimId);
		stream.PutOrGet(ref trimLevel);
		stream.PutOrGet(ref score);
		stream.PutOrGet(ref rewardData);
	}
}
