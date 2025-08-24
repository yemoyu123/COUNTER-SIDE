using System;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMStagePlayData : ISerializable
{
	public int stageId;

	public long playCount;

	public long restoreCount;

	public long bestKillCount;

	public DateTime nextResetDate;

	public int bestClearTimeSec;

	public long totalPlayCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
		stream.PutOrGet(ref playCount);
		stream.PutOrGet(ref restoreCount);
		stream.PutOrGet(ref bestKillCount);
		stream.PutOrGet(ref nextResetDate);
		stream.PutOrGet(ref bestClearTimeSec);
		stream.PutOrGet(ref totalPlayCount);
	}
}
