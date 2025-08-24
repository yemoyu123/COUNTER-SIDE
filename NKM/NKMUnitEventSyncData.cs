using Cs.Protocol;

namespace NKM;

public struct NKMUnitEventSyncData : ISerializable
{
	public NKM_UNIT_EVENT_MARK eventType;

	public float value;

	public NKMUnitEventSyncData(NKM_UNIT_EVENT_MARK _type, float _value)
	{
		eventType = _type;
		value = _value;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref eventType);
		stream.PutOrGet(ref value);
	}
}
