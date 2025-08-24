using Cs.Protocol;

namespace NKM;

public class NKMTacticalCommandData : ISerializable
{
	public int m_TCID;

	public byte m_Level = 1;

	public float m_fCoolTimeNow;

	public byte m_UseCount;

	public byte m_ComboCount;

	public float m_fComboResetCoolTimeNow;

	public bool m_bCoolTimeOn = true;

	public float m_fActiveTime;

	public virtual void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_TCID);
		stream.PutOrGet(ref m_Level);
		stream.PutOrGet(ref m_fCoolTimeNow);
		stream.PutOrGet(ref m_UseCount);
		stream.PutOrGet(ref m_ComboCount);
		stream.PutOrGet(ref m_fComboResetCoolTimeNow);
		stream.PutOrGet(ref m_bCoolTimeOn);
		stream.PutOrGet(ref m_fActiveTime);
	}

	public void DeepCopyFromSource(NKMTacticalCommandData source)
	{
		m_TCID = source.m_TCID;
		m_Level = source.m_Level;
		m_fCoolTimeNow = source.m_fCoolTimeNow;
		m_UseCount = source.m_UseCount;
		m_ComboCount = source.m_ComboCount;
		m_fComboResetCoolTimeNow = source.m_fComboResetCoolTimeNow;
		m_bCoolTimeOn = source.m_bCoolTimeOn;
		m_fActiveTime = source.m_fActiveTime;
	}

	public void AddComboCount()
	{
		m_ComboCount++;
	}

	public void Update(float fDeltaTime)
	{
		m_fCoolTimeNow -= fDeltaTime;
		if (m_fCoolTimeNow < 0f)
		{
			m_fCoolTimeNow = 0f;
		}
		m_fComboResetCoolTimeNow -= fDeltaTime;
		if (m_fComboResetCoolTimeNow < 0f)
		{
			m_fComboResetCoolTimeNow = 0f;
		}
		m_fActiveTime -= fDeltaTime;
		if (m_fActiveTime < 0f)
		{
			m_fActiveTime = 0f;
		}
	}

	public NKMTacticalCombo GetNKMTacticalComboGoal()
	{
		NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(m_TCID);
		if (tacticalCommandTempletByID == null)
		{
			return null;
		}
		if (m_ComboCount < 0 || m_ComboCount >= tacticalCommandTempletByID.m_listComboType.Count)
		{
			return null;
		}
		return tacticalCommandTempletByID.m_listComboType[m_ComboCount];
	}

	public void SetActiveTime(NKMTacticalCommandTemplet templet)
	{
		m_fActiveTime = templet.m_fActiveTime + templet.m_fActiveTimePerLevel * (float)(m_Level - 1);
	}
}
