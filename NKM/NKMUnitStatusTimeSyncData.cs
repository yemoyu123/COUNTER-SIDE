using Cs.Protocol;

namespace NKM;

public class NKMUnitStatusTimeSyncData : ISerializable
{
	public NKM_UNIT_STATUS_EFFECT m_eStatusType;

	public float m_fTime;

	public NKMUnitStatusTimeSyncData()
	{
	}

	public NKMUnitStatusTimeSyncData(NKM_UNIT_STATUS_EFFECT type, float time)
	{
		m_eStatusType = type;
		m_fTime = time;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref m_eStatusType);
		stream.PutOrGet(ref m_fTime);
	}
}
