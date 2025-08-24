using System.Collections.Generic;
using NKM.Templet;

namespace NKM;

public static class NKMWarfareMapContainer
{
	public static readonly Dictionary<string, NKMWarfareMapTemplet> mapTemplates = new Dictionary<string, NKMWarfareMapTemplet>();

	public static NKMWarfareMapTemplet GetOrLoad(string strId)
	{
		if (!mapTemplates.ContainsKey(strId))
		{
			LoadFromLUA_LUA_WARFARE_MAP_TEMPLET(strId);
		}
		mapTemplates.TryGetValue(strId, out var value);
		return value;
	}

	public static NKMWarfareMapTemplet ForceLoad(string strId)
	{
		LoadFromLUA_LUA_WARFARE_MAP_TEMPLET(strId);
		mapTemplates.TryGetValue(strId, out var value);
		return value;
	}

	private static bool LoadFromLUA_LUA_WARFARE_MAP_TEMPLET(string strID)
	{
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath("AB_SCRIPT_WARFARE_MAP_TEMPLET_ALL", strID))
			{
				return false;
			}
			NKMWarfareMapTemplet nKMWarfareMapTemplet = new NKMWarfareMapTemplet();
			if (!nKMLua.OpenTable("NKMWarfareMapTemplet"))
			{
				return false;
			}
			if (!nKMWarfareMapTemplet.LoadFromLUA(nKMLua, strID))
			{
				return false;
			}
			if (mapTemplates.ContainsKey(strID))
			{
				mapTemplates[strID] = nKMWarfareMapTemplet;
			}
			else
			{
				mapTemplates.Add(nKMWarfareMapTemplet.m_WarfareMapStrID, nKMWarfareMapTemplet);
			}
		}
		return true;
	}
}
