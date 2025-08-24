using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMGuildBadgeMarkTemplet : INKMTemplet
{
	private int m_ID;

	private string m_BadgeMarkImg = "";

	private UnlockInfo m_UnlockInfo;

	private bool m_LockbVisible;

	public int Key => m_ID;

	public int ID => m_ID;

	public string BadgeMarkImg => m_BadgeMarkImg;

	public bool LockVisible => m_LockbVisible;

	public UnlockInfo UnlockInfo => m_UnlockInfo;

	public static NKMGuildBadgeMarkTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMGuildBadgeMarkTemplet.cs", 26))
		{
			return null;
		}
		NKMGuildBadgeMarkTemplet nKMGuildBadgeMarkTemplet = new NKMGuildBadgeMarkTemplet();
		int num = (int)(1u & (cNKMLua.GetData("ID", ref nKMGuildBadgeMarkTemplet.m_ID) ? 1u : 0u) & (cNKMLua.GetData("m_BadgeMarkImg", ref nKMGuildBadgeMarkTemplet.m_BadgeMarkImg) ? 1u : 0u)) & (cNKMLua.GetData("m_LockbVisible", ref nKMGuildBadgeMarkTemplet.m_LockbVisible) ? 1 : 0);
		nKMGuildBadgeMarkTemplet.m_UnlockInfo = UnlockInfo.LoadFromLua(cNKMLua);
		if (num == 0)
		{
			return null;
		}
		return nKMGuildBadgeMarkTemplet;
	}

	public static NKMGuildBadgeMarkTemplet Find(int key)
	{
		return NKMTempletContainer<NKMGuildBadgeMarkTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
