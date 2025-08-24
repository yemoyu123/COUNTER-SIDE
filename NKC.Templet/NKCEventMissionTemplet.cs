using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.Templet;

public class NKCEventMissionTemplet : INKMTemplet
{
	public int m_EventID;

	public NKM_SHORTCUT_TYPE m_ShortcutType;

	public string m_ShortcutParam = "";

	public List<int> m_lstMissionTab = new List<int>();

	public int m_SpecialMissionTab;

	public int Key => m_EventID;

	public static NKCEventMissionTemplet Find(int id)
	{
		return NKMTempletContainer<NKCEventMissionTemplet>.Find(id);
	}

	public static NKCEventMissionTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCEventMissionTemplet.cs", 26))
		{
			return null;
		}
		NKCEventMissionTemplet nKCEventMissionTemplet = new NKCEventMissionTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_EventID", ref nKCEventMissionTemplet.m_EventID);
		cNKMLua.GetData("m_SpecialMissionTab", ref nKCEventMissionTemplet.m_SpecialMissionTab);
		cNKMLua.GetData("m_ShortCutType", ref nKCEventMissionTemplet.m_ShortcutType);
		cNKMLua.GetData("m_ShortCut", ref nKCEventMissionTemplet.m_ShortcutParam);
		if (cNKMLua.OpenTable("m_UseMissionTab"))
		{
			int i = 1;
			for (int rValue = 0; cNKMLua.GetData(i, ref rValue); i++)
			{
				nKCEventMissionTemplet.m_lstMissionTab.Add(rValue);
			}
			cNKMLua.CloseTable();
		}
		if (!(flag & (nKCEventMissionTemplet.m_lstMissionTab.Count > 0)))
		{
			return null;
		}
		return nKCEventMissionTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		foreach (int item in m_lstMissionTab)
		{
			NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(item);
			if (missionTabTemplet == null)
			{
				Debug.LogError($"Event {m_EventID} : MissionTab {item} does not Exist!");
			}
			else if (missionTabTemplet.m_MissionType != NKM_MISSION_TYPE.EVENT)
			{
				Debug.LogError($"Event {m_EventID} : MissionTab {item} is not a EVENT tab!");
			}
		}
	}
}
