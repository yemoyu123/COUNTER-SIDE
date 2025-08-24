using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKC.Templet;

public static class NKCLobbyIconManager
{
	private static Dictionary<int, NKCLobbyIconTemplet> m_DicIDX = new Dictionary<int, NKCLobbyIconTemplet>();

	public static bool LoadFromLUA(string filename, string tabFileName)
	{
		m_DicIDX = NKMTempletLoader.LoadDictionary("AB_SCRIPT", filename, "m_dicNKMMissionTempletByID", NKCLobbyIconTemplet.LoadFromLUA);
		if (m_DicIDX == null)
		{
			return false;
		}
		return true;
	}

	public static List<NKCLobbyIconTemplet> GetAvailableLobbyIconTemplet()
	{
		List<NKCLobbyIconTemplet> list = new List<NKCLobbyIconTemplet>();
		foreach (NKCLobbyIconTemplet templet in NKMTempletContainer<NKCLobbyIconTemplet>.Values)
		{
			if (list.Find((NKCLobbyIconTemplet e) => e.m_ShortCutType == templet.m_ShortCutType) == null && NKCSynchronizedTime.IsEventTime(templet.m_StartTimeUTC, templet.m_EndTimeUTC))
			{
				list.Add(templet);
			}
		}
		list.Sort(CompIDX);
		return list;
	}

	private static int CompIDX(NKCLobbyIconTemplet lItem, NKCLobbyIconTemplet rItem)
	{
		return lItem.IDX.CompareTo(rItem.IDX);
	}
}
