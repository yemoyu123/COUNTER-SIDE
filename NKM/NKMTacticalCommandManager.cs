using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public class NKMTacticalCommandManager
{
	public static Dictionary<int, NKMTacticalCommandTemplet> m_dicTacticalCommandTempletByID;

	public static Dictionary<string, NKMTacticalCommandTemplet> m_dicTacticalCommandTempletByStrID;

	public static void LoadFromLUA(string fileName)
	{
		m_dicTacticalCommandTempletByID?.Clear();
		m_dicTacticalCommandTempletByID = NKMTempletLoader.LoadDictionary("AB_SCRIPT", fileName, "m_dicTacticalCommandTempletByID", NKMTacticalCommandTemplet.LoadFromLUA);
		if (m_dicTacticalCommandTempletByID == null)
		{
			Log.ErrorAndExit("[NKMTacticalCommandManager] LoadFromLUA failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 513);
			return;
		}
		m_dicTacticalCommandTempletByStrID = m_dicTacticalCommandTempletByID.Values.ToDictionary((NKMTacticalCommandTemplet e) => e.m_TCStrID);
	}

	public static NKMTacticalCommandTemplet GetTacticalCommandTempletByID(int TCID)
	{
		if (m_dicTacticalCommandTempletByID.ContainsKey(TCID))
		{
			return m_dicTacticalCommandTempletByID[TCID];
		}
		return null;
	}

	public static NKMTacticalCommandTemplet GetTacticalCommandTempletByStrID(string TCStrID)
	{
		if (m_dicTacticalCommandTempletByStrID.ContainsKey(TCStrID))
		{
			return m_dicTacticalCommandTempletByStrID[TCStrID];
		}
		return null;
	}
}
