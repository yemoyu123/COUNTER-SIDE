using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMGuildBadgeColorTemplet : INKMTemplet
{
	private int m_ID;

	private string m_BadgeColorCode = "";

	private UnlockInfo m_UnlockInfo;

	private bool m_LockbVisible;

	public int Key => m_ID;

	public int ID => m_ID;

	public string BadgeColorCode => m_BadgeColorCode;

	public bool LockVisible => m_LockbVisible;

	public UnlockInfo UnlockInfo => m_UnlockInfo;

	public static NKMGuildBadgeColorTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMGuildBadgeColorTemplet.cs", 26))
		{
			return null;
		}
		NKMGuildBadgeColorTemplet nKMGuildBadgeColorTemplet = new NKMGuildBadgeColorTemplet();
		int num = 1 & (cNKMLua.GetData("ID", ref nKMGuildBadgeColorTemplet.m_ID) ? 1 : 0);
		string rValue = string.Empty;
		int num2 = num & (cNKMLua.GetData("m_BadgeColorCode", ref rValue) ? 1 : 0);
		nKMGuildBadgeColorTemplet.m_BadgeColorCode = $"#{rValue}";
		int num3 = num2 & (cNKMLua.GetData("m_LockbVisible", ref nKMGuildBadgeColorTemplet.m_LockbVisible) ? 1 : 0);
		nKMGuildBadgeColorTemplet.m_UnlockInfo = UnlockInfo.LoadFromLua(cNKMLua);
		if (num3 == 0)
		{
			return null;
		}
		return nKMGuildBadgeColorTemplet;
	}

	public static NKMGuildBadgeColorTemplet Find(int key)
	{
		return NKMTempletContainer<NKMGuildBadgeColorTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
