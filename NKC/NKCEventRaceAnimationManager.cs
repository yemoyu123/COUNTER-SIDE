using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKC;

public static class NKCEventRaceAnimationManager
{
	private static Dictionary<RaceEventType, List<NKCEventRaceAnimationTemplet>> m_dicRaceEventSet = new Dictionary<RaceEventType, List<NKCEventRaceAnimationTemplet>>();

	public static bool DataExist => m_dicRaceEventSet.Count > 0;

	public static bool LoadFromLua()
	{
		m_dicRaceEventSet.Clear();
		IEnumerable<NKCEventRaceAnimationTemplet> enumerable = NKMTempletLoader.LoadCommonPath("AB_SCRIPT", "LUA_EVENT_RACE_ANIMATION_TEMPLET", "EVENT_RACE_ANIMATION_TEMPLET", NKCEventRaceAnimationTemplet.LoadFromLua);
		if (m_dicRaceEventSet == null)
		{
			Log.Error("[NKCEventRaceAnimationTemplet] m_dicEventSet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCEventRaceAnimationManager.cs", 22);
			return false;
		}
		foreach (NKCEventRaceAnimationTemplet item in enumerable)
		{
			if (!m_dicRaceEventSet.ContainsKey(item.Key))
			{
				m_dicRaceEventSet.Add(item.Key, new List<NKCEventRaceAnimationTemplet>());
			}
			m_dicRaceEventSet[item.Key].Add(item);
		}
		return true;
	}

	public static List<NKCEventRaceAnimationTemplet> Find(RaceEventType raceEventType)
	{
		if (!m_dicRaceEventSet.TryGetValue(raceEventType, out var value))
		{
			return null;
		}
		return value;
	}

	public static int GetMaxCount(RaceEventType raceEventType)
	{
		return Find(raceEventType).Find((NKCEventRaceAnimationTemplet x) => string.IsNullOrEmpty(x.m_TargetObjName))?.m_MaxCount ?? 0;
	}

	public static int GetMinIndex(RaceEventType raceEventType)
	{
		return Find(raceEventType).Find((NKCEventRaceAnimationTemplet x) => string.IsNullOrEmpty(x.m_TargetObjName))?.m_MinIndex ?? 0;
	}

	public static int GetMaxIndex(RaceEventType raceEventType)
	{
		return Find(raceEventType).Find((NKCEventRaceAnimationTemplet x) => string.IsNullOrEmpty(x.m_TargetObjName))?.m_MaxIndex ?? int.MaxValue;
	}

	public static int GetCapacity(RaceEventType raceEventType)
	{
		return Find(raceEventType).Find((NKCEventRaceAnimationTemplet x) => string.IsNullOrEmpty(x.m_TargetObjName))?.m_SlotCapacity ?? 0;
	}

	public static float GetTotalTime(RaceEventType raceEventType)
	{
		return GetTotalTime(Find(raceEventType));
	}

	public static float GetTotalTime(List<NKCEventRaceAnimationTemplet> lstTemplet)
	{
		NKCEventRaceAnimationTemplet nKCEventRaceAnimationTemplet = lstTemplet.Find((NKCEventRaceAnimationTemplet x) => string.IsNullOrEmpty(x.m_TargetObjName));
		if (nKCEventRaceAnimationTemplet == null)
		{
			return 0f;
		}
		List<NKCAnimationEventTemplet> list = NKCAnimationEventManager.Find(nKCEventRaceAnimationTemplet.m_AnimationEventSetID);
		if (list == null)
		{
			return 0f;
		}
		List<NKCAnimationEventTemplet> list2 = list.FindAll((NKCAnimationEventTemplet x) => x.m_AniEventType == AnimationEventType.SET_MOVE_SPEED);
		list2.Sort(CompByStartTime);
		float num = 0f;
		float num2 = 0f;
		float num3 = 1f;
		if (list2.Count > 0)
		{
			for (int num4 = 0; num4 < list2.Count; num4++)
			{
				if (num4 > 0)
				{
					num2 += (list2[num4].m_StartTime - list2[num4 - 1].m_StartTime) * list2[num4 - 1].m_FloatValue;
					num = list2[num4].m_StartTime;
					num3 = 1f - num2;
				}
				if (num4 == list2.Count - 1)
				{
					num += num3 / list2[num4].m_FloatValue;
				}
			}
		}
		return num;
	}

	private static int CompByStartTime(NKCAnimationEventTemplet left, NKCAnimationEventTemplet right)
	{
		return left.m_StartTime.CompareTo(right.m_StartTime);
	}
}
