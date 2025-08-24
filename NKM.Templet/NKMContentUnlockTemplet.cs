using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMContentUnlockTemplet : INKMTemplet
{
	public int idx;

	public ContentsType m_eContentsType;

	public int m_ContentsValue;

	public UnlockInfo m_UnlockInfo;

	public string m_LockedText;

	public string m_strPopupTitle;

	public string m_strPopupDesc;

	public string m_strPopupImageName;

	public string m_PopupIconAssetBundleName;

	public string m_PopupIconName;

	public int Key => idx;

	public static NKMContentUnlockTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMContentUnlockTemplet.cs", 24))
		{
			return null;
		}
		NKMContentUnlockTemplet nKMContentUnlockTemplet = new NKMContentUnlockTemplet();
		int num = (int)(1u & (cNKMLua.GetData("IDX", ref nKMContentUnlockTemplet.idx) ? 1u : 0u) & (cNKMLua.GetData("eContentsType", ref nKMContentUnlockTemplet.m_eContentsType) ? 1u : 0u)) & (cNKMLua.GetData("m_ContentsValue", ref nKMContentUnlockTemplet.m_ContentsValue) ? 1 : 0);
		nKMContentUnlockTemplet.m_UnlockInfo = UnlockInfo.LoadFromLua(cNKMLua, nullable: false);
		cNKMLua.GetData("m_Text", ref nKMContentUnlockTemplet.m_LockedText);
		cNKMLua.GetData("m_PopupTitle", ref nKMContentUnlockTemplet.m_strPopupTitle);
		cNKMLua.GetData("m_PopupDesc", ref nKMContentUnlockTemplet.m_strPopupDesc);
		cNKMLua.GetData("m_PopupImage", ref nKMContentUnlockTemplet.m_strPopupImageName);
		cNKMLua.GetData("m_PopupIconAssetBundleName", ref nKMContentUnlockTemplet.m_PopupIconAssetBundleName);
		cNKMLua.GetData("m_PopupIconImage", ref nKMContentUnlockTemplet.m_PopupIconName);
		if (num == 0)
		{
			return null;
		}
		return nKMContentUnlockTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_eContentsType == ContentsType.RANK)
		{
			Log.Error("ContentsType.RANK - 사용하지 않는 enum. LEADERBOARD 로 사용", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMContentUnlockTemplet.cs", 54);
		}
	}
}
