using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMDiveArtifactTemplet : INKMTemplet
{
	private int m_ArtifactID;

	private string m_ArtifactMiscIconName = string.Empty;

	private string m_ArtifactName = string.Empty;

	private string m_ArtifactMiscDesc_1 = string.Empty;

	private NKM_DIVE_ARTIFACT_CATEGORY m_Category;

	private string m_ArtifactMiscDesc_2 = string.Empty;

	private int m_RefBattleCondition_ID;

	private NKMBattleConditionTemplet m_BattleConditionTemplet;

	private NKM_REWARD_TYPE m_ReturnReward_Type;

	private int m_ReturnReward_Id;

	private int m_ReturnReward_Quantity;

	public int Key => m_ArtifactID;

	public int ArtifactID => m_ArtifactID;

	public string ArtifactMiscIconName => m_ArtifactMiscIconName;

	public string ArtifactName => m_ArtifactName;

	public string ArtifactMiscDesc_1 => m_ArtifactMiscDesc_1;

	public NKM_DIVE_ARTIFACT_CATEGORY Category => m_Category;

	public string ArtifactMiscDesc_2 => m_ArtifactMiscDesc_2;

	public int BattleConditionID => m_RefBattleCondition_ID;

	public NKMBattleConditionTemplet BattleConditionTemplet => m_BattleConditionTemplet;

	public NKM_REWARD_TYPE RewardType => m_ReturnReward_Type;

	public int RewardId => m_ReturnReward_Id;

	public int RewardQuantity => m_ReturnReward_Quantity;

	public string ArtifactName_Translated => NKCStringTable.GetString(ArtifactName);

	public string ArtifactMiscDesc_1_Translated => NKCStringTable.GetString(ArtifactMiscDesc_1);

	public string ArtifactMiscDesc_2_Translated => NKCStringTable.GetString(ArtifactMiscDesc_2);

	public static NKMDiveArtifactTemplet LoadFromLUA(NKMLua lua)
	{
		NKMDiveArtifactTemplet nKMDiveArtifactTemplet = new NKMDiveArtifactTemplet();
		int num = (int)(1u & (lua.GetData("m_ArtifactID", ref nKMDiveArtifactTemplet.m_ArtifactID) ? 1u : 0u) & (lua.GetData("m_ArtifactMiscIconName", ref nKMDiveArtifactTemplet.m_ArtifactMiscIconName) ? 1u : 0u) & (lua.GetData("m_ArtifactName", ref nKMDiveArtifactTemplet.m_ArtifactName) ? 1u : 0u) & (lua.GetData("m_ArtifactMiscDesc_1", ref nKMDiveArtifactTemplet.m_ArtifactMiscDesc_1) ? 1u : 0u) & (lua.GetDataEnum<NKM_DIVE_ARTIFACT_CATEGORY>("m_Category", out nKMDiveArtifactTemplet.m_Category) ? 1u : 0u)) & (lua.GetData("m_ArtifactMiscDesc_2", ref nKMDiveArtifactTemplet.m_ArtifactMiscDesc_2) ? 1 : 0);
		lua.GetData("m_RefBattleCondition_ID", ref nKMDiveArtifactTemplet.m_RefBattleCondition_ID);
		lua.GetDataEnum<NKM_REWARD_TYPE>("m_ReturnReward_TYPE", out nKMDiveArtifactTemplet.m_ReturnReward_Type);
		lua.GetData("m_ReturnReward_ID", ref nKMDiveArtifactTemplet.m_ReturnReward_Id);
		lua.GetData("m_ReturnRewardQuantity", ref nKMDiveArtifactTemplet.m_ReturnReward_Quantity);
		if (num == 0)
		{
			return null;
		}
		return nKMDiveArtifactTemplet;
	}

	public static NKMDiveArtifactTemplet Find(int key)
	{
		return NKMTempletContainer<NKMDiveArtifactTemplet>.Find(key);
	}

	public void Join()
	{
		m_BattleConditionTemplet = NKMBattleConditionManager.GetTempletByID(BattleConditionID);
	}

	public void Validate()
	{
		if (m_ReturnReward_Id < 0)
		{
			NKMTempletError.Add($"[NKMDiveArtifactTemplet:{Key}] ReturnReward Id가 잘못됨. Id:{m_ReturnReward_Id}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveArtifactTemplet.cs", 70);
		}
		if (m_ReturnReward_Quantity < 0)
		{
			NKMTempletError.Add($"[NKMDiveArtifactTemplet:{Key}] ReturnReward Quantity가 잘못됨. Quantity:{m_ReturnReward_Quantity}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveArtifactTemplet.cs", 75);
		}
	}
}
