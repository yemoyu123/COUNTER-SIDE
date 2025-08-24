using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMGuildBadgeFrameTemplet : INKMTemplet
{
	private int m_ID;

	private string m_BadgeFrameImg = "";

	private UnlockInfo m_UnlockInfo;

	private bool m_LockbVisible;

	public int Key => m_ID;

	public int ID => m_ID;

	public string BadgeFrameImg => m_BadgeFrameImg;

	public UnlockInfo UnlockInfo => m_UnlockInfo;

	public bool LockVisible => m_LockbVisible;

	public static NKMGuildBadgeFrameTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMGuildBadgeFrameTemplet.cs", 26))
		{
			return null;
		}
		NKMGuildBadgeFrameTemplet nKMGuildBadgeFrameTemplet = new NKMGuildBadgeFrameTemplet();
		int num = (int)(1u & (cNKMLua.GetData("ID", ref nKMGuildBadgeFrameTemplet.m_ID) ? 1u : 0u) & (cNKMLua.GetData("m_BadgeFrameImg", ref nKMGuildBadgeFrameTemplet.m_BadgeFrameImg) ? 1u : 0u)) & (cNKMLua.GetData("m_LockbVisible", ref nKMGuildBadgeFrameTemplet.m_LockbVisible) ? 1 : 0);
		nKMGuildBadgeFrameTemplet.m_UnlockInfo = UnlockInfo.LoadFromLua(cNKMLua);
		if (num == 0)
		{
			return null;
		}
		return nKMGuildBadgeFrameTemplet;
	}

	public static NKMGuildBadgeFrameTemplet Find(int key)
	{
		return NKMTempletContainer<NKMGuildBadgeFrameTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
