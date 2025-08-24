using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public class NKMCommonUnitEvent
{
	public static Dictionary<string, NKMEventHeal> m_dicEventHeal;

	public static bool LoadFromLUA(string fileName, bool bReload)
	{
		if (bReload)
		{
			m_dicEventHeal?.Clear();
		}
		IEnumerable<NKMEventHeal> enumerable = NKMTempletLoader.LoadCommonPath("AB_SCRIPT", fileName, "m_dicEventHeal", NKMEventHeal.LoadFromLUAStatic);
		if (enumerable != null)
		{
			m_dicEventHeal = enumerable.ToDictionary((NKMEventHeal e) => e.m_EventStrID, (NKMEventHeal e) => e);
		}
		return m_dicEventHeal != null;
	}

	public static NKMEventHeal GetNKMEventHeal(string eventID)
	{
		if (m_dicEventHeal.ContainsKey(eventID))
		{
			return m_dicEventHeal[eventID];
		}
		Log.Error("NKMCommonUnitEvent GetNKMEventHeal no m_EventStrID " + eventID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonUnitEvent.cs", 38);
		return null;
	}
}
