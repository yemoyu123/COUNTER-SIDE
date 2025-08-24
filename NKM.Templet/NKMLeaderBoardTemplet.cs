using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMLeaderBoardTemplet : INKMTemplet
{
	public int m_BoardID;

	public LeaderBoardType m_BoardTab = LeaderBoardType.BT_NONE;

	public int m_BoardTabSubIndex;

	public string m_BoardTabName;

	public int m_OrderList;

	public LayoutType m_LayoutType;

	public string m_BoardTitle;

	public string m_BoardDesc;

	public int m_BoardCriteria;

	public string m_BoardTabIcon;

	public string m_BoardBackgroundImg;

	public string m_BoardPopupTitle;

	public string m_BoardPopupName;

	public string m_BoardPopupDesc;

	public string m_BoardPopupImg;

	public int Key => m_BoardID;

	public static NKMLeaderBoardTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaderBoardTemplet.cs", 57))
		{
			return null;
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = new NKMLeaderBoardTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_BoardID", ref nKMLeaderBoardTemplet.m_BoardID) ? 1u : 0u) & (cNKMLua.GetData("m_BoardTab", ref nKMLeaderBoardTemplet.m_BoardTab) ? 1u : 0u) & (cNKMLua.GetData("m_BoardTabSubIndex", ref nKMLeaderBoardTemplet.m_BoardTabSubIndex) ? 1u : 0u)) & (cNKMLua.GetData("m_BoardTabName", ref nKMLeaderBoardTemplet.m_BoardTabName) ? 1 : 0);
		cNKMLua.GetData("m_OrderList", ref nKMLeaderBoardTemplet.m_OrderList);
		cNKMLua.GetData("m_LayoutType", ref nKMLeaderBoardTemplet.m_LayoutType);
		cNKMLua.GetData("m_BoardTitle", ref nKMLeaderBoardTemplet.m_BoardTitle);
		cNKMLua.GetData("m_BoardDesc", ref nKMLeaderBoardTemplet.m_BoardDesc);
		cNKMLua.GetData("m_BoardCriteria", ref nKMLeaderBoardTemplet.m_BoardCriteria);
		cNKMLua.GetData("m_BoardTabIcon", ref nKMLeaderBoardTemplet.m_BoardTabIcon);
		cNKMLua.GetData("m_BoardBackgroundImg", ref nKMLeaderBoardTemplet.m_BoardBackgroundImg);
		cNKMLua.GetData("m_BoardPopupTitle", ref nKMLeaderBoardTemplet.m_BoardPopupTitle);
		cNKMLua.GetData("m_BoardPopupName", ref nKMLeaderBoardTemplet.m_BoardPopupName);
		cNKMLua.GetData("m_BoardPopupDesc", ref nKMLeaderBoardTemplet.m_BoardPopupDesc);
		cNKMLua.GetData("m_BoardPopupImg", ref nKMLeaderBoardTemplet.m_BoardPopupImg);
		if (num == 0)
		{
			return null;
		}
		return nKMLeaderBoardTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public string GetTabName()
	{
		return NKCStringTable.GetString(m_BoardTabName);
	}

	public string GetTitle()
	{
		return NKCStringTable.GetString(m_BoardTitle);
	}

	public string GetDesc()
	{
		return NKCStringTable.GetString(m_BoardDesc);
	}

	public string GetPopupTitle()
	{
		return NKCStringTable.GetString(m_BoardPopupTitle);
	}

	public string GetPopupName()
	{
		return NKCStringTable.GetString(m_BoardPopupName);
	}

	public string GetPopupDesc()
	{
		return NKCStringTable.GetString(m_BoardPopupDesc);
	}

	public static NKMLeaderBoardTemplet Find(LeaderBoardType tabType, int criteria)
	{
		return NKMTempletContainer<NKMLeaderBoardTemplet>.Find((NKMLeaderBoardTemplet x) => x.m_BoardTab == tabType && x.m_BoardCriteria == criteria);
	}
}
