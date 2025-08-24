using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMCollectionTeamUpTemplet : INKMTemplet
{
	private int m_TeamID;

	private string m_TeamName = "";

	private int m_UnitID;

	private string m_UnitStrID = "";

	private int m_RewardCriteria;

	private NKM_REWARD_TYPE m_RewardType;

	private int m_RewardID;

	private int m_RewardValue;

	public int Key => m_TeamID;

	public int TeamID => m_TeamID;

	public string TeamName => m_TeamName;

	public int UnitID => m_UnitID;

	public string UnitStrID => m_UnitStrID;

	public int RewardCriteria => m_RewardCriteria;

	public NKM_REWARD_TYPE RewardType => m_RewardType;

	public int RewardID => m_RewardID;

	public int RewardValue => m_RewardValue;

	public static NKMCollectionTeamUpTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionTeamUpTemplet.cs", 29))
		{
			return null;
		}
		NKMCollectionTeamUpTemplet nKMCollectionTeamUpTemplet = new NKMCollectionTeamUpTemplet();
		if ((1u & (cNKMLua.GetData("m_TeamID", ref nKMCollectionTeamUpTemplet.m_TeamID) ? 1u : 0u) & (cNKMLua.GetData("m_TeamName", ref nKMCollectionTeamUpTemplet.m_TeamName) ? 1u : 0u) & (cNKMLua.GetData("m_UnitID", ref nKMCollectionTeamUpTemplet.m_UnitID) ? 1u : 0u) & (cNKMLua.GetData("m_UnitStrID", ref nKMCollectionTeamUpTemplet.m_UnitStrID) ? 1u : 0u) & (cNKMLua.GetData("m_RewardCriteria", ref nKMCollectionTeamUpTemplet.m_RewardCriteria) ? 1u : 0u) & (cNKMLua.GetData("m_RewardType", ref nKMCollectionTeamUpTemplet.m_RewardType) ? 1u : 0u) & (cNKMLua.GetData("m_RewardID", ref nKMCollectionTeamUpTemplet.m_RewardID) ? 1u : 0u) & (cNKMLua.GetData("m_RewardValue", ref nKMCollectionTeamUpTemplet.m_RewardValue) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMCollectionTeamUpTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
