using NKM;
using NKM.Templet.Base;

namespace NKC;

public class NKCMonsterTagInfoTemplet : INKMTemplet
{
	public int m_MonsterTagID;

	public string m_MonsterTagDesc;

	public string m_MonsterTagIcon;

	public int Key => m_MonsterTagID;

	public static NKCMonsterTagInfoTemplet Find(int idx)
	{
		return NKMTempletContainer<NKCMonsterTagInfoTemplet>.Find(idx);
	}

	public static NKCMonsterTagInfoTemplet LoadFromLUA(NKMLua lua)
	{
		NKCMonsterTagInfoTemplet nKCMonsterTagInfoTemplet = new NKCMonsterTagInfoTemplet();
		if ((1u & (lua.GetData("m_MonsterTagID", ref nKCMonsterTagInfoTemplet.m_MonsterTagID) ? 1u : 0u) & (lua.GetData("m_MonsterTagDesc", ref nKCMonsterTagInfoTemplet.m_MonsterTagDesc) ? 1u : 0u) & (lua.GetData("m_MonsterTagIcon", ref nKCMonsterTagInfoTemplet.m_MonsterTagIcon) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKCMonsterTagInfoTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
