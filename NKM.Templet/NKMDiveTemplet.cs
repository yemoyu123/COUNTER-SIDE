using System.Collections.Generic;
using System.Linq;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMDiveTemplet : INKMTemplet
{
	public static int DiveStormCostMiscId = 3;

	public static int DiveStormCostMultiply = 2;

	public static int DiveStormRewardMiscId = 1;

	public static int DiveStormRewardMultiply = 2;

	public const int MaxFirstRewardCount = 3;

	public const int MaxSafeRewardCount = 3;

	private int INDEX_ID;

	private int STAGE_ID;

	private string STAGE_STR_ID = "";

	private string STAGE_NAME = "";

	private string STAGE_NAME_SUB = "";

	private string BACKGROUND_FILENAME;

	private string MUSIC_ASSET_BUNDLE_NAME;

	private string STAGE_MUSIC_NAME;

	private NKM_DIVE_STAGE_TYPE DIVE_STAGE_TYPE;

	private int STAGE_LEVEL;

	private int STAGE_LEVEL_SCALE;

	private int SET_LEVEL_SCALE;

	private int BIND_SET;

	private STAGE_UNLOCK_REQ_TYPE STAGE_UNLOCK_REQ_TYPE;

	private int STAGE_UNLOCK_REQ_VALUE;

	private string OPEN_TAG = "";

	private int STAGE_REQ_ITEM_ID;

	private int STAGE_REQ_ITEM_COUNT;

	private int SQUAD_COUNT;

	private int RANDOM_SET_COUNT;

	private int SLOT_COUNT;

	private int SLOT_EVENT_GROUP_ID;

	private int BOSS_EVENT_GROUP_ID;

	private bool IS_EVENT_DIVE;

	private string cutsceneDiveEnter = string.Empty;

	private string cutsceneDiveStart = string.Empty;

	private string cutsceneDiveBossBefore = string.Empty;

	private string cutsceneDiveBossAfter = string.Empty;

	private readonly List<NKMDiveFirstReward> FIRSTREWARD_LIST = new List<NKMDiveFirstReward>();

	private readonly List<NKMDiveSafeReward> safeRewards = new List<NKMDiveSafeReward>();

	private int startingArtifact;

	private string diveMonsterBattleCondition = string.Empty;

	private int depth = 1;

	private int BOSS_STAGE_REQ_ITEM_ID;

	private int BOSS_STAGE_REQ_ITEM_COUNT;

	private int SAFE_MINE_REQ_ITEM_ID;

	private int SAFE_MINE_REQ_ITEM_COUNT;

	public int Key => STAGE_ID;

	public int IndexID => INDEX_ID;

	public int StageID => STAGE_ID;

	public string StageStrID => STAGE_STR_ID;

	public string StageName => STAGE_NAME;

	public string StageNameSub => STAGE_NAME_SUB;

	public string BackgroundFilename => BACKGROUND_FILENAME;

	public string MusicBundleName => MUSIC_ASSET_BUNDLE_NAME;

	public string MusicFileName => STAGE_MUSIC_NAME;

	public NKM_DIVE_STAGE_TYPE StageType => DIVE_STAGE_TYPE;

	public int StageLevel => STAGE_LEVEL;

	public int StageLevelScale => STAGE_LEVEL_SCALE;

	public int SetLevelScale => SET_LEVEL_SCALE;

	public int BindSet => BIND_SET;

	public STAGE_UNLOCK_REQ_TYPE StageUnlockReqType => STAGE_UNLOCK_REQ_TYPE;

	public int StageUnlockReqValue => STAGE_UNLOCK_REQ_VALUE;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(OPEN_TAG);

	public int StageReqItemId => STAGE_REQ_ITEM_ID;

	public int StageReqItemCount => STAGE_REQ_ITEM_COUNT;

	public int SquadCount => SQUAD_COUNT;

	public int RandomSetCount => RANDOM_SET_COUNT;

	public int SlotCount => SLOT_COUNT;

	public int SlotEventGroupID => SLOT_EVENT_GROUP_ID;

	public int BossEventGroupID => BOSS_EVENT_GROUP_ID;

	public bool IsEventDive => IS_EVENT_DIVE;

	public string CutsceneDiveEnter => cutsceneDiveEnter;

	public string CutsceneDiveStart => cutsceneDiveStart;

	public string CutsceneDiveBossBefore => cutsceneDiveBossBefore;

	public string CutsceneDiveBossAfter => cutsceneDiveBossAfter;

	public bool IsCutsceneMode
	{
		get
		{
			if (string.IsNullOrEmpty(cutsceneDiveEnter) && string.IsNullOrEmpty(cutsceneDiveStart) && string.IsNullOrEmpty(cutsceneDiveBossBefore))
			{
				return !string.IsNullOrEmpty(cutsceneDiveBossAfter);
			}
			return true;
		}
	}

	public int StartArtifactId => startingArtifact;

	public List<NKMDiveFirstReward> FirstRewardList => FIRSTREWARD_LIST;

	public List<NKMDiveSafeReward> SafeRewards => safeRewards;

	public int Depth => depth;

	public string DiveMonsterBattleCondition => diveMonsterBattleCondition;

	public int BossStageReqItemID => BOSS_STAGE_REQ_ITEM_ID;

	public int BossStageReqItemCount => BOSS_STAGE_REQ_ITEM_COUNT;

	public int SafeMineReqItemID => SAFE_MINE_REQ_ITEM_ID;

	public int SafeMineReqItemCount => SAFE_MINE_REQ_ITEM_COUNT;

	public static NKMDiveTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 189))
		{
			return null;
		}
		NKMDiveTemplet nKMDiveTemplet = new NKMDiveTemplet();
		bool flag = true;
		flag &= lua.GetData("INDEX_ID", ref nKMDiveTemplet.INDEX_ID);
		flag &= lua.GetData("STAGE_ID", ref nKMDiveTemplet.STAGE_ID);
		flag &= lua.GetData("STAGE_STR_ID", ref nKMDiveTemplet.STAGE_STR_ID);
		flag &= lua.GetData("STAGE_NAME", ref nKMDiveTemplet.STAGE_NAME);
		flag &= lua.GetData("STAGE_NAME_SUB", ref nKMDiveTemplet.STAGE_NAME_SUB);
		flag &= lua.GetData("BACKGROUND_FILENAME", ref nKMDiveTemplet.BACKGROUND_FILENAME);
		lua.GetData("MUSIC_ASSET_BUNDLE_NAME", ref nKMDiveTemplet.MUSIC_ASSET_BUNDLE_NAME);
		lua.GetData("STAGE_MUSIC_NAME", ref nKMDiveTemplet.STAGE_MUSIC_NAME);
		flag &= lua.GetData("EVENT_DIVE", ref nKMDiveTemplet.IS_EVENT_DIVE);
		flag &= lua.GetData("DIVE_STAGE_TYPE", ref nKMDiveTemplet.DIVE_STAGE_TYPE);
		flag &= lua.GetData("STAGE_LEVEL", ref nKMDiveTemplet.STAGE_LEVEL);
		flag &= lua.GetData("STAGE_LEVEL_SCALE", ref nKMDiveTemplet.STAGE_LEVEL_SCALE);
		flag &= lua.GetData("SET_LEVEL_SCALE", ref nKMDiveTemplet.SET_LEVEL_SCALE);
		flag &= lua.GetData("BIND_SET", ref nKMDiveTemplet.BIND_SET);
		flag &= lua.GetData("STAGE_UNLOCK_REQ_TYPE", ref nKMDiveTemplet.STAGE_UNLOCK_REQ_TYPE);
		flag &= lua.GetData("STAGE_UNLOCK_REQ_VALUE", ref nKMDiveTemplet.STAGE_UNLOCK_REQ_VALUE);
		flag &= lua.GetData("SQUAD_COUNT", ref nKMDiveTemplet.SQUAD_COUNT);
		flag &= lua.GetData("RANDOM_SET_COUNT", ref nKMDiveTemplet.RANDOM_SET_COUNT);
		flag &= lua.GetData("SLOT_COUNT", ref nKMDiveTemplet.SLOT_COUNT);
		flag &= lua.GetData("SLOT_EVENT_GROUP_ID", ref nKMDiveTemplet.SLOT_EVENT_GROUP_ID);
		flag &= lua.GetData("BOSS_EVENT_GROUP_ID", ref nKMDiveTemplet.BOSS_EVENT_GROUP_ID);
		for (int i = 0; i < 3; i++)
		{
			NKMDiveFirstReward item = default(NKMDiveFirstReward);
			lua.GetData($"FIRSTREWARD_TYPE_{i + 1}", ref item.FIRSTREWARD_TYPE);
			lua.GetData($"FIRSTREWARD_ID_{i + 1}", ref item.FIRSTREWARD_ID);
			lua.GetData($"FIRSTREWARD_STRID_{i + 1}", ref item.FIRSTREWARD_STRID);
			lua.GetData($"FIRSTREWARD_QUANTITY_{i + 1}", ref item.FIRSTREWARD_QUANTITY);
			nKMDiveTemplet.FIRSTREWARD_LIST.Add(item);
		}
		lua.GetData("DIVE_ENTER_CUTSCENE", ref nKMDiveTemplet.cutsceneDiveEnter);
		lua.GetData("DIVE_START_CUTSCENE", ref nKMDiveTemplet.cutsceneDiveStart);
		lua.GetData("DIVE_BOSS_BEFORE_CUTSCENE", ref nKMDiveTemplet.cutsceneDiveBossBefore);
		lua.GetData("DIVE_BOSS_AFTER_CUTSCENE", ref nKMDiveTemplet.cutsceneDiveBossAfter);
		lua.GetData("m_OpenTag", ref nKMDiveTemplet.OPEN_TAG);
		for (int j = 0; j < 3; j++)
		{
			NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
			lua.GetData($"SAFE_REWARD_TYPE_{j + 1}", ref result);
			if (result == NKM_REWARD_TYPE.RT_NONE)
			{
				break;
			}
			NKMDiveSafeReward item2 = new NKMDiveSafeReward
			{
				RewardType = result
			};
			lua.GetData($"SAFE_REWARD_ID_{j + 1}", ref item2.RewardId);
			lua.GetData($"SAFE_REWARD_STRID_{j + 1}", ref item2.RewardStrId);
			lua.GetData($"SAFE_REWARD_QUANTITY_{j + 1}", ref item2.RewardQuantity);
			nKMDiveTemplet.safeRewards.Add(item2);
		}
		lua.GetData("STARTING_ARTIFACT", ref nKMDiveTemplet.startingArtifact);
		lua.GetData("DEPTH", ref nKMDiveTemplet.depth);
		lua.GetData("DIVE_MONSTER_BC", ref nKMDiveTemplet.diveMonsterBattleCondition);
		lua.GetData("STAGE_REQ_ITEM_ID", ref nKMDiveTemplet.STAGE_REQ_ITEM_ID);
		lua.GetData("STAGE_REQ_ITEM_COUNT", ref nKMDiveTemplet.STAGE_REQ_ITEM_COUNT);
		lua.GetData("BOSS_STAGE_REQ_ITEM_ID", ref nKMDiveTemplet.BOSS_STAGE_REQ_ITEM_ID);
		lua.GetData("BOSS_STAGE_REQ_ITEM_COUNT", ref nKMDiveTemplet.BOSS_STAGE_REQ_ITEM_COUNT);
		lua.GetData("SAFE_MINE_REQ_ITEM_ID", ref nKMDiveTemplet.SAFE_MINE_REQ_ITEM_ID);
		lua.GetData("SAFE_MINE_REQ_ITEM_COUNT", ref nKMDiveTemplet.SAFE_MINE_REQ_ITEM_COUNT);
		if (!flag)
		{
			return null;
		}
		return nKMDiveTemplet;
	}

	public static NKMDiveTemplet Find(int key)
	{
		return NKMTempletContainer<NKMDiveTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (StageReqItemId != 0 && NKMItemManager.GetItemMiscTempletByID(StageReqItemId) == null)
		{
			NKMTempletError.Add($"[DiveTemplet] 다이브 필요 아이템 정보가 존재하지 않음 StageID:{StageID} StageReqItemId:{StageReqItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 281);
		}
		if (BossStageReqItemID != 0 && NKMItemManager.GetItemMiscTempletByID(BossStageReqItemID) == null)
		{
			NKMTempletError.Add($"[DiveTemplet] 다이브 보스전 필요 아이템 정보가 존재하지 않음 StageID:{StageID} BossStageReqItemID:{BossStageReqItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 286);
		}
		if (SafeMineReqItemID != 0 && NKMItemManager.GetItemMiscTempletByID(SafeMineReqItemID) == null)
		{
			NKMTempletError.Add($"[DiveTemplet] 다이브 안전채굴 필요 아이템 정보가 존재하지 않음 StageID:{StageID} SafeMineReqItemID:{SafeMineReqItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 291);
		}
		if (STAGE_REQ_ITEM_ID == 0 && BOSS_STAGE_REQ_ITEM_ID == 0 && SAFE_MINE_REQ_ITEM_ID == 0)
		{
			NKMTempletError.Add($"[DiveTemplet] 다이브 소모 아이템 정보가 모두 없음 StageID:{StageID} StageReqItem:{StageReqItemId} BossStageReqItemID:{BossStageReqItemID} SafeMineReqItemID:{SafeMineReqItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 298);
		}
		foreach (NKMDiveFirstReward firstReward in FirstRewardList)
		{
			if (!NKMRewardTemplet.IsValidReward(firstReward.FIRSTREWARD_TYPE, firstReward.FIRSTREWARD_ID))
			{
				NKMTempletError.Add($"[DiveTemplet] 첫 클리어 보상 정보가 존재하지 않음 StageID:{StageID} RewardType:{firstReward.FIRSTREWARD_TYPE} RewardID:{firstReward.FIRSTREWARD_ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 305);
			}
		}
		if (!NKMContentUnlockManager.IsValidMissionUnlockType(new UnlockInfo(StageUnlockReqType, StageUnlockReqValue)))
		{
			NKMTempletError.Add($"[DiveTemplet] 해제조건이 유효하지 않음 StageID:{StageID} StageUnlockReqType:{StageUnlockReqType} StageUnlockReqValue:{StageUnlockReqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 311);
		}
		if (SafeRewards.Where((NKMDiveSafeReward e) => e.RewardId <= 0).Any())
		{
			NKMTempletError.Add($"[NKMDiveTemplet:{Key}] 올바르지 않은 안전채굴 보상 Id가 존재", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 316);
		}
		if (SafeRewards.Where((NKMDiveSafeReward e) => e.RewardQuantity <= 0).Any())
		{
			NKMTempletError.Add($"[NKMDiveTemplet:{Key}] 올바르지 않은 안전채굴 보상 Quantity가 존재", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 321);
		}
		if (startingArtifact < 0)
		{
			NKMTempletError.Add($"[NKMDiveTemplet:{Key}] 시작 아티팩트가 부적절한 id로 설정. StartingArtifact:{startingArtifact}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 326);
		}
		if (startingArtifact > 0 && NKMDiveArtifactTemplet.Find(startingArtifact) == null)
		{
			NKMTempletError.Add($"[NKMDiveTemplet:{Key}] 시작 아티팩트가 ArtifactTemplet에 존재하지 않음. StartingArtifact:{startingArtifact}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 331);
		}
		if (SquadCount <= 0 || SquadCount > NKMCommonConst.Deck.MaxDiveDeckCount)
		{
			NKMTempletError.Add($"[NKMDiveTemplet:{Key}] 다이브 덱 편성 숫자가 범위를 벗어남. SquadCount:{SquadCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDiveTemplet.cs", 336);
		}
	}

	public EPISODE_DIFFICULTY GetCommonDifficultyData()
	{
		return StageType switch
		{
			NKM_DIVE_STAGE_TYPE.NDST_NORMAL => EPISODE_DIFFICULTY.NORMAL, 
			NKM_DIVE_STAGE_TYPE.NDST_HARD => EPISODE_DIFFICULTY.HARD, 
			_ => EPISODE_DIFFICULTY.NORMAL, 
		};
	}

	public string Get_STAGE_NAME()
	{
		return NKCStringTable.GetString(StageName);
	}

	public string Get_STAGE_NAME_SUB()
	{
		return NKCStringTable.GetString(StageNameSub);
	}

	public int GetDiveJumpPlusCost()
	{
		return RandomSetCount * DiveStormCostMultiply;
	}

	public string GetCutsceneID(int num)
	{
		return num switch
		{
			1 => CutsceneDiveEnter, 
			2 => CutsceneDiveStart, 
			3 => CutsceneDiveBossBefore, 
			4 => CutsceneDiveBossAfter, 
			_ => "", 
		};
	}
}
