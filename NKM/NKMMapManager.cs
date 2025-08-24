using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public class NKMMapManager
{
	public static Dictionary<int, NKMMapTemplet> m_dicNKMMapTempletID = null;

	public static Dictionary<string, NKMMapTemplet> m_dicNKMMapTempletStrID = null;

	public static Dictionary<int, HashSet<int>> m_dicMapTempletByGroupID = new Dictionary<int, HashSet<int>>();

	public static List<int> m_listPVPMap = new List<int>();

	public static bool LoadFromLUA(string fileName, bool bReload = false)
	{
		if (bReload)
		{
			m_dicNKMMapTempletID?.Clear();
			m_dicNKMMapTempletStrID?.Clear();
		}
		m_dicNKMMapTempletID = NKMTempletLoader.LoadDictionary("AB_SCRIPT", fileName, "m_listNKMMapTemplet", NKMMapTemplet.LoadFromLUA);
		if (m_dicNKMMapTempletID != null)
		{
			m_dicNKMMapTempletStrID = m_dicNKMMapTempletID.ToDictionary((KeyValuePair<int, NKMMapTemplet> e) => e.Value.m_MapStrID, (KeyValuePair<int, NKMMapTemplet> e) => e.Value);
			foreach (KeyValuePair<int, NKMMapTemplet> item in m_dicNKMMapTempletID)
			{
				foreach (int groupId in item.Value.m_GroupIdList)
				{
					AddMapGroup(groupId, item.Value.m_MapID);
				}
				if (item.Value.m_bUsePVP)
				{
					m_listPVPMap.Add(item.Value.m_MapID);
				}
			}
		}
		if (m_listPVPMap.Count == 0)
		{
			Log.ErrorAndExit("Invalid PvpMapTemplet.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMapManager.cs", 398);
			return false;
		}
		return m_dicNKMMapTempletID != null;
	}

	public static NKMMapTemplet GetMapTempletByID(int mapID)
	{
		if (m_dicNKMMapTempletID.ContainsKey(mapID))
		{
			return m_dicNKMMapTempletID[mapID];
		}
		return null;
	}

	public static NKMMapTemplet GetMapTempletByStrID(string mapStrID)
	{
		if (m_dicNKMMapTempletStrID.ContainsKey(mapStrID))
		{
			return m_dicNKMMapTempletStrID[mapStrID];
		}
		return null;
	}

	public static List<string> GetTotalMapStrID()
	{
		List<string> list = new List<string>();
		Dictionary<int, NKMMapTemplet>.Enumerator enumerator = m_dicNKMMapTempletID.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMMapTemplet value = enumerator.Current.Value;
			list.Add(value.m_MapStrID);
		}
		return list;
	}

	public static int GetPVPRandomMap()
	{
		int index = NKMRandom.Range(0, m_listPVPMap.Count);
		return m_listPVPMap[index];
	}

	public static int GetRandomMapByGroupId(int groupId)
	{
		if (groupId == 0)
		{
			return 0;
		}
		if (m_dicMapTempletByGroupID.TryGetValue(groupId, out var value))
		{
			int index = NKMRandom.Range(0, value.Count);
			return value.ElementAt(index);
		}
		return 0;
	}

	public static void AddMapGroup(int groupId, int mapId)
	{
		if (m_dicMapTempletByGroupID.TryGetValue(groupId, out var value))
		{
			value.Add(mapId);
			return;
		}
		HashSet<int> hashSet = new HashSet<int>();
		hashSet.Add(mapId);
		m_dicMapTempletByGroupID.Add(groupId, hashSet);
	}
}
