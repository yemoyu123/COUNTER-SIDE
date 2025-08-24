using Cs.Protocol;

namespace NKM;

public class NKMBuffSyncData : ISerializable
{
	public short m_BuffID;

	public byte m_BuffStatLevel;

	public byte m_BuffTimeLevel;

	public bool m_bNew;

	public bool m_bAffect;

	public byte m_OverlapCount = 1;

	public bool m_bRangeSon;

	public short m_MasterGameUnitUID;

	public bool m_bUseMasterStat = true;

	public void Init()
	{
		m_BuffID = 0;
		m_BuffStatLevel = 0;
		m_BuffTimeLevel = 0;
		m_bNew = false;
		m_bAffect = false;
		m_OverlapCount = 1;
		m_bRangeSon = false;
		m_MasterGameUnitUID = 0;
		m_bUseMasterStat = true;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_BuffID);
		stream.PutOrGet(ref m_BuffStatLevel);
		stream.PutOrGet(ref m_BuffTimeLevel);
		stream.PutOrGet(ref m_bNew);
		stream.PutOrGet(ref m_bAffect);
		stream.PutOrGet(ref m_OverlapCount);
		stream.PutOrGet(ref m_bRangeSon);
		stream.PutOrGet(ref m_MasterGameUnitUID);
		stream.PutOrGet(ref m_bUseMasterStat);
	}
}
