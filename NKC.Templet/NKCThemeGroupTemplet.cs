using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCThemeGroupTemplet : INKMTemplet
{
	public int ThemeGroupID;

	public string OpenTag;

	public int GroupNumber;

	public HashSet<string> GroupID = new HashSet<string>();

	public string GroupStringKey;

	public string GroupIconName;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(OpenTag);

	public int Key => ThemeGroupID;

	public static void Load()
	{
		if (!NKMTempletContainer<NKCThemeGroupTemplet>.HasValue())
		{
			NKMTempletContainer<NKCThemeGroupTemplet>.Load("ab_script", "LUA_THEME_GROUP_TEMPLET", "THEME_GROUP_TEMPLET", LoadFromLUA);
		}
	}

	public static NKCThemeGroupTemplet Find(int id)
	{
		Load();
		return NKMTempletContainer<NKCThemeGroupTemplet>.Find(id);
	}

	public static NKCThemeGroupTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCThemeGroupTemplet.cs", 38))
		{
			return null;
		}
		NKCThemeGroupTemplet nKCThemeGroupTemplet = new NKCThemeGroupTemplet();
		int num = 1 & (cNKMLua.GetData("ThemeGroupID", ref nKCThemeGroupTemplet.ThemeGroupID) ? 1 : 0);
		cNKMLua.GetData("OpenTag", ref nKCThemeGroupTemplet.OpenTag);
		if (((uint)num & (cNKMLua.GetData("GroupNumber", ref nKCThemeGroupTemplet.GroupNumber) ? 1u : 0u) & (cNKMLua.GetData("GroupID", nKCThemeGroupTemplet.GroupID) ? 1u : 0u) & (cNKMLua.GetData("GroupStringKey", ref nKCThemeGroupTemplet.GroupStringKey) ? 1u : 0u) & (cNKMLua.GetData("GroupIconName", ref nKCThemeGroupTemplet.GroupIconName) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKCThemeGroupTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
