namespace NKM;

public class NKMDungeonEventTemplet
{
	public NKMDungeonEventTiming m_NKMDungeonEventTiming = new NKMDungeonEventTiming();

	public int m_EventID;

	public NKM_EVENT_ACTION_TYPE m_dungeonEventType;

	public int m_EventActionValue;

	public string m_EventActionStrValue = "";

	public NKM_EVENT_START_CONDITION_TYPE m_EventCondition;

	public string m_EventConditionValue1 = "";

	public string m_EventConditionValue2 = "";

	public int m_EventConditionNumValue;

	public bool m_bPause;

	public float m_fEventDelay;

	public static bool IsPermanent(NKM_EVENT_ACTION_TYPE type)
	{
		if (type - 11 <= NKM_EVENT_ACTION_TYPE.SET_ENEMY_BOSS_HP_RATE || type == NKM_EVENT_ACTION_TYPE.MAP_ANIMATION || type == NKM_EVENT_ACTION_TYPE.CHANGE_CAMERA)
		{
			return true;
		}
		return false;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		bool flag = true;
		if (cNKMLua.OpenTable("m_NKMDungeonEventTiming"))
		{
			flag &= m_NKMDungeonEventTiming.LoadFromLUA(cNKMLua);
			cNKMLua.CloseTable();
			cNKMLua.GetData("m_bPause", ref m_bPause);
			flag &= cNKMLua.GetData("m_EventID", ref m_EventID);
			flag &= cNKMLua.GetData("m_EventCondition", ref m_EventCondition);
			cNKMLua.GetData("m_EventConditionValue1", ref m_EventConditionValue1);
			cNKMLua.GetData("m_EventConditionValue2", ref m_EventConditionValue2);
			cNKMLua.GetData("m_EventConditionNumValue", ref m_EventConditionNumValue);
			flag &= cNKMLua.GetData("m_dungeonEventType", ref m_dungeonEventType);
			cNKMLua.GetData("m_EventActionValue", ref m_EventActionValue);
			cNKMLua.GetData("m_EventActionStrValue", ref m_EventActionStrValue);
			cNKMLua.GetData("m_fEventDelay", ref m_fEventDelay);
			return flag;
		}
		return false;
	}
}
