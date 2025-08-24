namespace NKM;

public class NKMInvokedTrigger
{
	public int m_TriggerID;

	public float m_LastTime;

	public short m_TriggerOwnerGameUnitUID;

	public bool m_bFinished;

	public NKMInvokedTrigger(int triggerID, short ownerUID)
	{
		m_TriggerID = triggerID;
		m_LastTime = -1f;
		m_TriggerOwnerGameUnitUID = ownerUID;
		m_bFinished = false;
	}

	public void Restart()
	{
		m_LastTime = -1f;
	}

	public void SetNewTrigger(int id, short ownerUID)
	{
		m_TriggerID = id;
		m_TriggerOwnerGameUnitUID = ownerUID;
		m_bFinished = false;
		m_LastTime = -1f;
	}
}
