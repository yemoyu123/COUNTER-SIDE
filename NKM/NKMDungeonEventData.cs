namespace NKM;

public class NKMDungeonEventData
{
	public NKMDungeonEventTemplet m_DungeonEventTemplet;

	public float m_fEventLastStartTime = -1f;

	public float m_fEventLastEndTime = -1f;

	public int EventConditionCache1 = -1;

	public float m_fEventExecuteReserveTime;

	public bool m_bEvokeReserved;
}
