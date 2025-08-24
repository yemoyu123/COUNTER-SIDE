using Cs.Protocol;

namespace ClientPacket.Mode;

public sealed class PhaseModeState : ISerializable
{
	public int stageId;

	public int phaseIndex;

	public int dungeonId;

	public float totalPlayTime;

	public long supportingUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
		stream.PutOrGet(ref phaseIndex);
		stream.PutOrGet(ref dungeonId);
		stream.PutOrGet(ref totalPlayTime);
		stream.PutOrGet(ref supportingUserUid);
	}
}
