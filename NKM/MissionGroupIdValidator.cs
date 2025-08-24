using System.Collections.Generic;
using System.Linq;
using Cs.Logging;

namespace NKM;

internal class MissionGroupIdValidator
{
	private Dictionary<int, List<NKMMissionTemplet>> missionTempletsByGroupId = new Dictionary<int, List<NKMMissionTemplet>>();

	public void Add(NKMMissionTemplet templet)
	{
		if (!missionTempletsByGroupId.ContainsKey(templet.m_GroupId))
		{
			missionTempletsByGroupId[templet.m_GroupId] = new List<NKMMissionTemplet>();
		}
		missionTempletsByGroupId[templet.m_GroupId].Add(templet);
	}

	public void Validate()
	{
		foreach (KeyValuePair<int, List<NKMMissionTemplet>> item in missionTempletsByGroupId)
		{
			IEnumerable<IGrouping<int, NKMMissionTemplet>> source = from e in item.Value
				group e by e.m_MissionTabId;
			if (source.Count() > 1)
			{
				IEnumerable<int> values = source.Select((IGrouping<int, NKMMissionTemplet> e) => e.Key);
				Log.ErrorAndExit(string.Format("[MissionGroupIdValidator] \ufffd\ufffd\ufffd\ufffd \ufffdٸ\ufffd \ufffdǿ\ufffd \ufffdߺ\ufffd\ufffd\ufffd \ufffd\u033c\ufffd \ufffd\u05f7\ufffd \ufffd\ufffd\ufffd\u0335\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd. TabIds : {0}, GroupId : {1}", string.Join(",", values), item.Key), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1696);
				break;
			}
			if (item.Value.Count > 1)
			{
				if (item.Value.Any((NKMMissionTemplet e) => e.m_TabTemplet.m_MissionType == NKM_MISSION_TYPE.MENTORING))
				{
					int groupId = item.Value[0].m_GroupId;
					Log.ErrorAndExit($"[MissionGroupIdValidator] \ufffd\ufffd\ufffd丵\ufffd\ufffd \ufffdߺ\ufffd \ufffd\u05f7\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdҰ\ufffd\ufffd\ufffd\ufffdմϴ\ufffd. groupId: {groupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1705);
					break;
				}
				if ((from e in item.Value
					group e by e.m_MissionCond.mission_cond).Count() > 1)
				{
					int groupId2 = item.Value[0].m_GroupId;
					Log.ErrorAndExit($"[MissionGroupIdValidator] \ufffdش\ufffd \ufffd\u05f7\ufffd \ufffd\ufffd\ufffd\u0335\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdٸ\ufffd \ufffd\u033c\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffdǾ\ufffd\ufffd\ufffd\ufffdϴ\ufffd. groupId: {groupId2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 1713);
					break;
				}
			}
		}
	}
}
