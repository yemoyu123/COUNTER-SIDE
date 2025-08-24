using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCGuideManualTemplet : INKMTemplet
{
	public int ID;

	public string ID_STRING;

	public List<NKCGuideManualTempletData> lstManualTemplets = new List<NKCGuideManualTempletData>();

	private static bool Loaded;

	public int Key => ID;

	public NKCGuideManualTemplet(int id, IEnumerable<NKCGuideManualTempletData> manualTemplets)
	{
		ID = id;
		lstManualTemplets.AddRange(manualTemplets);
		ID_STRING = lstManualTemplets[0].GUIDE_ID_STRING;
	}

	public void Join()
	{
		foreach (NKCGuideManualTempletData lstManualTemplet in lstManualTemplets)
		{
			lstManualTemplet.Join();
		}
	}

	public void Validate()
	{
	}

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKCGuideManualTemplet>.Load(from e in NKMTempletLoader<NKCGuideManualTempletData>.LoadGroup("AB_SCRIPT", "LUA_GUIDE_MANUAL_TEMPLET", "m_GuideManualTemplet", NKCGuideManualTempletData.LoadFromLUA)
			select new NKCGuideManualTemplet(e.Key, e.Value), (NKCGuideManualTemplet e) => e.ID_STRING);
		Loaded = true;
	}

	public static NKCGuideManualTemplet Find(int id)
	{
		if (!Loaded)
		{
			LoadFromLua();
		}
		return NKMTempletContainer<NKCGuideManualTemplet>.Find(id);
	}

	public static NKCGuideManualTemplet Find(string strID)
	{
		if (!Loaded)
		{
			LoadFromLua();
		}
		return NKMTempletContainer<NKCGuideManualTemplet>.Find(strID);
	}

	public string GetTitle()
	{
		return NKCStringTable.GetString(lstManualTemplets[0].CATEGORY_TITLE);
	}

	public static string GetTitle(string strID)
	{
		string result = "";
		NKCGuideManualTemplet nKCGuideManualTemplet = Find(strID);
		if (nKCGuideManualTemplet != null)
		{
			result = NKCStringTable.GetString(nKCGuideManualTemplet.lstManualTemplets[0].CATEGORY_TITLE);
		}
		return result;
	}

	public string GetTitle(int id)
	{
		string result = "";
		NKCGuideManualTemplet nKCGuideManualTemplet = Find(id);
		if (nKCGuideManualTemplet != null)
		{
			result = NKCStringTable.GetString(nKCGuideManualTemplet.lstManualTemplets[0].CATEGORY_TITLE);
		}
		return result;
	}
}
