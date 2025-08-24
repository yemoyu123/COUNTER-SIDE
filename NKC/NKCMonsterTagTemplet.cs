using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;

namespace NKC;

public class NKCMonsterTagTemplet : INKMTemplet
{
	public int m_UnitID;

	public string m_OpenTag;

	private List<int> m_MonsterTagID;

	public int Key => m_UnitID;

	public List<int> lstTags => m_MonsterTagID;

	public bool EnabledByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static NKCMonsterTagTemplet Find(int idx)
	{
		return NKMTempletContainer<NKCMonsterTagTemplet>.Find(idx);
	}

	public static NKCMonsterTagTemplet LoadFromLUA(NKMLua lua)
	{
		NKCMonsterTagTemplet nKCMonsterTagTemplet = new NKCMonsterTagTemplet();
		int num = (int)(1u & (lua.GetData("m_UnitID", ref nKCMonsterTagTemplet.m_UnitID) ? 1u : 0u)) & (lua.GetDataList("m_MonsterTagID", out nKCMonsterTagTemplet.m_MonsterTagID, nullIfEmpty: false) ? 1 : 0);
		lua.GetData("m_OpenTag", ref nKCMonsterTagTemplet.m_OpenTag);
		if (num == 0)
		{
			return null;
		}
		return nKCMonsterTagTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
