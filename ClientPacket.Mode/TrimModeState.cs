using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Mode;

public sealed class TrimModeState : ISerializable
{
	public int trimId;

	public int trimLevel;

	public int nextDungeonId;

	public NKMTrimStageData lastClearStage = new NKMTrimStageData();

	public List<NKMTrimStageData> stageList = new List<NKMTrimStageData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref trimId);
		stream.PutOrGet(ref trimLevel);
		stream.PutOrGet(ref nextDungeonId);
		stream.PutOrGet(ref lastClearStage);
		stream.PutOrGet(ref stageList);
	}
}
