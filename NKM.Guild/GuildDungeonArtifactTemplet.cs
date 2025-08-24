using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildDungeonArtifactTemplet : INKMTemplet
{
	public enum ArtifactProbType
	{
		HIGH,
		MIDDLE,
		LOW
	}

	private int stageRewardArtifactGroup;

	private int artifactId;

	private ArtifactProbType bgProbImage;

	private int artifactOrder;

	private NKM_DIVE_ARTIFACT_CATEGORY artifactCategory;

	private string artifactMiscIconName;

	private string artifactName;

	private string artifactMiscDesc1;

	private string artifactMiscDesc2;

	private int refBattleConditionId;

	private int returnPriceId;

	private int returnPriceValue;

	private NKMBattleConditionTemplet m_BattleConditionTemplet;

	public int Key => stageRewardArtifactGroup;

	public NKMBattleConditionTemplet BattleConditionTemplet => m_BattleConditionTemplet;

	public int RefBattleConditionId => refBattleConditionId;

	public int ReturnPriceId => returnPriceId;

	public int ReturnPriceValue => returnPriceValue;

	public static GuildDungeonArtifactTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonArtifactTemplet.cs", 37))
		{
			return null;
		}
		GuildDungeonArtifactTemplet guildDungeonArtifactTemplet = new GuildDungeonArtifactTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_StageRewardArtifactGroup", ref guildDungeonArtifactTemplet.stageRewardArtifactGroup) ? 1u : 0u) & (cNKMLua.GetData("m_ArtifactID", ref guildDungeonArtifactTemplet.artifactId) ? 1u : 0u)) & (cNKMLua.GetData("m_BgProbImage", ref guildDungeonArtifactTemplet.bgProbImage) ? 1 : 0);
		cNKMLua.GetData("m_ArtifactOrder", ref guildDungeonArtifactTemplet.artifactOrder);
		if (((uint)num & (cNKMLua.GetData("m_ArtifactCategory", ref guildDungeonArtifactTemplet.artifactCategory) ? 1u : 0u) & (cNKMLua.GetData("m_ArtifactMiscIconName", ref guildDungeonArtifactTemplet.artifactMiscIconName) ? 1u : 0u) & (cNKMLua.GetData("m_ArtifactName", ref guildDungeonArtifactTemplet.artifactName) ? 1u : 0u) & (cNKMLua.GetData("m_ArtifactMiscDesc_1", ref guildDungeonArtifactTemplet.artifactMiscDesc1) ? 1u : 0u) & (cNKMLua.GetData("m_ArtifactMiscDesc_2", ref guildDungeonArtifactTemplet.artifactMiscDesc2) ? 1u : 0u) & (cNKMLua.GetData("m_RefBattleConditionID", ref guildDungeonArtifactTemplet.refBattleConditionId) ? 1u : 0u) & (cNKMLua.GetData("m_ReturnPriceID", ref guildDungeonArtifactTemplet.returnPriceId) ? 1u : 0u) & (cNKMLua.GetData("m_ReturnPriceValue", ref guildDungeonArtifactTemplet.returnPriceValue) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return guildDungeonArtifactTemplet;
	}

	public int GetArtifactOrder()
	{
		return artifactOrder;
	}

	public int GetArtifactId()
	{
		return artifactId;
	}

	public void Join()
	{
		m_BattleConditionTemplet = NKMBattleConditionManager.GetTempletByID(refBattleConditionId);
		if (m_BattleConditionTemplet == null)
		{
			NKMTempletError.Add($"[GuildDungeonArtifact] BattleConditionTemplet에 해당 id가 존재하지 않습니다. refBattleCOnditionId: {refBattleConditionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonArtifactTemplet.cs", 77);
		}
	}

	public void Validate()
	{
		if (m_BattleConditionTemplet.UseContentsType != NKMBattleConditionTemplet.USE_CONTENT_TYPE.UCT_GUILD_DUNGEON_ARTIFACT)
		{
			NKMTempletError.Add($"[GuildDungeonArtifactTemplet] Invalid BattleConditionTemplet.UseContentsType. ArtifactId:{artifactId} m_BattleConditionTemplet:{m_BattleConditionTemplet.Key} UseContentsType:{m_BattleConditionTemplet.UseContentsType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonArtifactTemplet.cs", 85);
		}
	}

	public int GetOrder()
	{
		return artifactOrder;
	}

	public ArtifactProbType GetBgProbImage()
	{
		return bgProbImage;
	}

	public string GetIconName()
	{
		return artifactMiscIconName;
	}

	public NKM_DIVE_ARTIFACT_CATEGORY GetCategory()
	{
		return artifactCategory;
	}

	public string GetName()
	{
		return NKCStringTable.GetString(artifactName);
	}

	public string GetDescFull()
	{
		return NKCStringTable.GetString(artifactMiscDesc1);
	}

	public string GetDescShort()
	{
		return NKCStringTable.GetString(artifactMiscDesc2);
	}
}
