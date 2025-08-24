using System.Collections.Generic;
using NKC;

namespace NKM.Unit;

public class NKMUnitTriggerSet
{
	public enum DuplicateInvokeMode
	{
		IGNORE,
		RESTART,
		PARALLEL
	}

	public delegate bool ValidateEvent(INKMUnitStateEvent stateEvent);

	public string m_Name;

	public bool m_bAnimTime;

	public DuplicateInvokeMode m_DuplicateMode = DuplicateInvokeMode.RESTART;

	public bool m_bStopOnStateChange;

	public float m_fEndTime = -1f;

	public bool m_bProcessWhileDying;

	private List<NKMUnitStateEventOneTime> m_lstEvents = new List<NKMUnitStateEventOneTime>();

	public List<NKMUnitStateEventOneTime> Events => m_lstEvents;

	public bool InvokeTimedTriggerClient(float timeBefore, float timeNow, NKCGameClient cNKMGame, NKCUnitClient cNKMUnit)
	{
		bool result = true;
		foreach (NKMUnitStateEventOneTime lstEvent in m_lstEvents)
		{
			if (lstEvent != null && !lstEvent.bStateEnd)
			{
				if (timeNow < lstEvent.m_fEventTime)
				{
					result = false;
				}
				else if (!(lstEvent.m_fEventTime <= timeBefore) && CheckClassMatch(cNKMUnit.Get_NKM_UNIT_CLASS_TYPE(), lstEvent))
				{
					lstEvent.ProcessEventClient(cNKMGame, cNKMUnit, bStateEnd: false, bIgnoreTimer: true);
				}
			}
		}
		if (m_fEndTime >= 0f)
		{
			result = timeNow >= m_fEndTime;
		}
		return result;
	}

	public void InvokeEndTriggerEventClient(NKCGameClient cNKMGame, NKCUnitClient cNKMUnit)
	{
		foreach (NKMUnitStateEventOneTime lstEvent in m_lstEvents)
		{
			if (lstEvent != null && CheckClassMatch(cNKMUnit.Get_NKM_UNIT_CLASS_TYPE(), lstEvent) && lstEvent.bStateEnd)
			{
				lstEvent.ProcessEventClient(cNKMGame, cNKMUnit, bStateEnd: false, bIgnoreTimer: true);
			}
		}
	}

	public float CalculateLastTime()
	{
		if (m_fEndTime >= 0f)
		{
			return m_fEndTime;
		}
		float num = 0f;
		foreach (NKMUnitStateEventOneTime lstEvent in m_lstEvents)
		{
			if (lstEvent.EventStartTime > num)
			{
				num = lstEvent.EventStartTime;
			}
		}
		return num;
	}

	public IEnumerable<T> GetEvents<T>() where T : NKMUnitStateEventOneTime
	{
		if (m_lstEvents == null)
		{
			yield break;
		}
		foreach (NKMUnitStateEventOneTime lstEvent in m_lstEvents)
		{
			if (lstEvent is T)
			{
				yield return lstEvent as T;
			}
		}
	}

	private bool CheckClassMatch(NKM_UNIT_CLASS_TYPE unitClass, NKMUnitStateEventOneTime stateEvent)
	{
		return unitClass switch
		{
			NKM_UNIT_CLASS_TYPE.NCT_UNIT_CLIENT => stateEvent.HostType != EventHostType.Server, 
			NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER => stateEvent.HostType != EventHostType.Client, 
			_ => false, 
		};
	}

	public bool InvokeTimedTrigger(float timeBefore, float timeNow, NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		bool result = true;
		foreach (NKMUnitStateEventOneTime lstEvent in m_lstEvents)
		{
			if (lstEvent != null && CheckClassMatch(cNKMUnit.Get_NKM_UNIT_CLASS_TYPE(), lstEvent) && !lstEvent.bStateEnd && !(lstEvent.m_fEventTime <= timeBefore))
			{
				if (timeNow < lstEvent.m_fEventTime)
				{
					result = false;
				}
				else
				{
					lstEvent.ProcessEvent(cNKMGame, cNKMUnit, bStateEnd: false, bIgnoreTimer: true);
				}
			}
		}
		if (m_fEndTime >= 0f)
		{
			result = timeNow >= m_fEndTime;
		}
		return result;
	}

	public void InvokeEndTriggerEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		foreach (NKMUnitStateEventOneTime lstEvent in m_lstEvents)
		{
			if (lstEvent != null && CheckClassMatch(cNKMUnit.Get_NKM_UNIT_CLASS_TYPE(), lstEvent) && lstEvent.bStateEnd)
			{
				lstEvent.ProcessEvent(cNKMGame, cNKMUnit, bStateEnd: false, bIgnoreTimer: true);
			}
		}
	}

	public static void LoadFromLua(NKMLua cNKMLua, ref Dictionary<int, NKMUnitTriggerSet> dicTriggerSet, ref Dictionary<string, int> dicTriggerSetID)
	{
		if (cNKMLua.OpenTable("m_dicTriggerSet"))
		{
			dicTriggerSetID = new Dictionary<string, int>();
			dicTriggerSet = new Dictionary<int, NKMUnitTriggerSet>();
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				NKMUnitTriggerSet nKMUnitTriggerSet = new NKMUnitTriggerSet();
				nKMUnitTriggerSet.LoadEventFromLua(cNKMLua);
				dicTriggerSet.Add(num, nKMUnitTriggerSet);
				dicTriggerSetID.Add(nKMUnitTriggerSet.m_Name, num);
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		else
		{
			dicTriggerSet = null;
			dicTriggerSetID = null;
		}
	}

	private bool LoadEventFromLua(NKMLua cNKMLua)
	{
		if (!cNKMLua.GetData("m_Name", ref m_Name))
		{
			return false;
		}
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_DuplicateMode", ref m_DuplicateMode);
		cNKMLua.GetData("m_bStopOnStateChange", ref m_bStopOnStateChange);
		cNKMLua.GetData("m_fEndTime", ref m_fEndTime);
		cNKMLua.GetData("m_bProcessWhileDying", ref m_bProcessWhileDying);
		m_lstEvents.Clear();
		LoadEventList<NKMEventText>(cNKMLua, "m_listNKMEventText");
		LoadEventList<NKMEventMove>(cNKMLua, "m_listNKMEventMove");
		LoadEventList<NKMEventStopTime>(cNKMLua, "m_listNKMEventStopTime");
		LoadEventList<NKMEventInvincibleGlobal>(cNKMLua, "m_listNKMEventInvincibleGlobal");
		LoadEventList<NKMEventSound>(cNKMLua, "m_listNKMEventSound");
		LoadEventList<NKMEventColor>(cNKMLua, "m_listNKMEventColor");
		LoadEventList<NKMEventCameraCrash>(cNKMLua, "m_listNKMEventCameraCrash");
		LoadEventList<NKMEventDissolve>(cNKMLua, "m_listNKMEventDissolve");
		LoadEventList<NKMEventEffect>(cNKMLua, "m_listNKMEventEffect");
		LoadEventList<NKMEventHyperSkillCutIn>(cNKMLua, "m_listNKMEventHyperSkillCutIn");
		LoadEventList<NKMEventDamageEffect>(cNKMLua, "m_listNKMEventDamageEffect");
		LoadEventList<NKMEventDEStateChange>(cNKMLua, "m_listNKMEventDEStateChange");
		LoadEventList<NKMEventGameSpeed>(cNKMLua, "m_listNKMEventGameSpeed");
		LoadEventList<NKMEventAnimSpeed>(cNKMLua, "m_listNKMEventAnimSpeed");
		LoadEventList<NKMEventBuff>(cNKMLua, "m_listNKMEventBuff");
		LoadEventList<NKMEventStatus>(cNKMLua, "m_listNKMEventStatus");
		LoadEventList<NKMEventRespawn>(cNKMLua, "m_listNKMEventRespawn");
		LoadEventList<NKMEventDie>(cNKMLua, "m_listNKMEventDie");
		LoadEventList<NKMEventChangeState>(cNKMLua, "m_listNKMEventChangeState");
		LoadEventList<NKMEventAgro>(cNKMLua, "m_listNKMEventAgro");
		LoadEventList<NKMEventHeal>(cNKMLua, "m_listNKMEventHeal");
		LoadEventList<NKMEventStun>(cNKMLua, "m_listNKMEventStun");
		LoadEventList<NKMEventCost>(cNKMLua, "m_listNKMEventCost");
		LoadEventList<NKMEventDispel>(cNKMLua, "m_listNKMEventDispel");
		LoadEventList<NKMEventChangeCooltime>(cNKMLua, "m_listNKMEventChangeCooltime");
		LoadEventList<NKMEventFindTarget>(cNKMLua, "m_listNKMEventFindTarget");
		LoadEventList<NKMEventChangeRemainTime>(cNKMLua, "m_listNKMEventChangeRemainTime");
		LoadEventList<NKMEventVariable>(cNKMLua, "m_listNKMEventVariable");
		LoadEventList<NKMEventConsume>(cNKMLua, "m_listNKMEventConsume");
		LoadEventList<NKMEventSkillCutIn>(cNKMLua, "m_listNKMEventSkillCutIn");
		LoadEventList<NKMEventReaction>(cNKMLua, "m_listNKMEventReaction");
		return true;
	}

	public void LoadEventList<T>(NKMLua cNKMLua, string tableName, ValidateEvent validateEvent = null) where T : NKMUnitStateEventOneTime, new()
	{
		if (cNKMLua.OpenTable(tableName))
		{
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				T val = new T();
				val.LoadFromLUA(cNKMLua);
				validateEvent?.Invoke(val);
				m_lstEvents.Add(val);
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
	}
}
