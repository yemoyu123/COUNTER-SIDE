using Cs.Protocol;

namespace ClientPacket.Unit;

public sealed class NKMUnitMissionData : ISerializable
{
	public int unitId;

	public int missionId;

	public int stepId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitId);
		stream.PutOrGet(ref missionId);
		stream.PutOrGet(ref stepId);
	}
}
