using NKM;
using NKM.Templet;

namespace NKC.Templet;

public class ItemDropInfo
{
	private string m_itemId;

	private DropContent m_contentType;

	private NKM_REWARD_TYPE m_rewardType;

	private int m_contentID;

	private bool m_summary;

	public string ItemID => m_itemId;

	public DropContent ContentType => m_contentType;

	public NKM_REWARD_TYPE RewardType => m_rewardType;

	public int ContentID => m_contentID;

	public bool Summary
	{
		get
		{
			return m_summary;
		}
		set
		{
			m_summary = value;
		}
	}

	public ItemDropInfo(string itemID, DropContent contentType, NKM_REWARD_TYPE rewardType, int contentID)
	{
		m_itemId = itemID;
		m_contentType = contentType;
		m_rewardType = rewardType;
		m_contentID = contentID;
		m_summary = false;
	}

	public static ItemDropInfo LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCItemDropInfoTemplet.cs", 52))
		{
			return null;
		}
		string rValue = string.Empty;
		DropContent result = DropContent.Stage;
		NKM_REWARD_TYPE result2 = NKM_REWARD_TYPE.RT_NONE;
		int rValue2 = 0;
		int num = (int)(1u & (lua.GetData("ItemID", ref rValue) ? 1u : 0u) & (lua.GetData("DropContent", ref result) ? 1u : 0u)) & (lua.GetData("ID", ref rValue2) ? 1 : 0);
		lua.GetData("RewardType", ref result2);
		if (num == 0)
		{
			return null;
		}
		return new ItemDropInfo(rValue, result, result2, rValue2);
	}
}
