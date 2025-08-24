using Cs.Math;

namespace NKM;

public class NKMDungeonEventTiming
{
	public float m_fEventTimeStart;

	public float m_fEventTimeEnd;

	public float m_fEventBossHPLess;

	public float m_fEventBossHPUpper;

	public bool m_fEventIgnoreBossInitHPLess = true;

	public float m_fEventTimeGap;

	public string m_EventLiveDeckTag = "";

	public string m_EventLiveWarfareDungeonTag = "";

	public string m_EventDieWarfareDungeonTag = "";

	public string m_EventDieUnitTag = "";

	public int m_EventDieUnitTagCount;

	public string m_EventDieDeckTag = "";

	public int m_EventDieDeckTagCount;

	public string m_EventTag = "";

	public int m_EventTagCount;

	public float m_fEventPos;

	public NKM_DUNGEON_EVENT_TYPE m_NKM_DUNGEON_EVENT_TYPE = NKM_DUNGEON_EVENT_TYPE.NDET_DECK;

	public NKMDungeonEventTiming Clone()
	{
		return MemberwiseClone() as NKMDungeonEventTiming;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_fEventTimeStart", ref m_fEventTimeStart);
		cNKMLua.GetData("m_fEventTimeEnd", ref m_fEventTimeEnd);
		cNKMLua.GetData("m_fEventBossHPLess", ref m_fEventBossHPLess);
		cNKMLua.GetData("m_fEventBossHPUpper", ref m_fEventBossHPUpper);
		cNKMLua.GetData("m_fEventIgnoreBossInitHPLess", ref m_fEventIgnoreBossInitHPLess);
		cNKMLua.GetData("m_fEventTimeGap", ref m_fEventTimeGap);
		cNKMLua.GetData("m_EventLiveDeckTag", ref m_EventLiveDeckTag);
		cNKMLua.GetData("m_EventLiveWarfareDungeonTag", ref m_EventLiveWarfareDungeonTag);
		cNKMLua.GetData("m_EventDieWarfareDungeonTag", ref m_EventDieWarfareDungeonTag);
		cNKMLua.GetData("m_EventDieUnitTag", ref m_EventDieUnitTag);
		cNKMLua.GetData("m_EventDieUnitTagCount", ref m_EventDieUnitTagCount);
		cNKMLua.GetData("m_EventDieDeckTag", ref m_EventDieDeckTag);
		cNKMLua.GetData("m_EventDieDeckTagCount", ref m_EventDieDeckTagCount);
		cNKMLua.GetData("m_EventTag", ref m_EventTag);
		cNKMLua.GetData("m_EventTagCount", ref m_EventTagCount);
		cNKMLua.GetData("m_fEventPos", ref m_fEventPos);
		cNKMLua.GetData("m_NKM_DUNGEON_EVENT_TYPE", ref m_NKM_DUNGEON_EVENT_TYPE);
		return true;
	}

	public bool EventTimeCheck(float fGameTime)
	{
		if (fGameTime >= m_fEventTimeStart && fGameTime <= m_fEventTimeEnd)
		{
			return true;
		}
		if (fGameTime >= m_fEventTimeStart && m_fEventTimeEnd.IsNearlyZero())
		{
			return true;
		}
		return false;
	}
}
