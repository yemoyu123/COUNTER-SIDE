using System.Collections.Generic;
using System.Linq;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMPhaseTemplet : INKMTemplet
{
	private readonly List<DungeonReward> rewards = new List<DungeonReward>();

	private int phaseGroupId;

	public int m_UseEventDeck;

	private bool m_bBonus_Resource;

	public string m_CutScenStrIDBefore = "";

	public string m_CutScenStrIDAfter = "";

	public string m_Intro = "";

	public string m_LoadingImage = "";

	public string m_Outro = "";

	public DUNGEON_GAME_MISSION_TYPE m_DGMissionType_1;

	public int m_DGMissionValue_1;

	public DUNGEON_GAME_MISSION_TYPE m_DGMissionType_2;

	public int m_DGMissionValue_2;

	public int m_RewardUserEXP;

	public int m_RewardUnitEXP;

	public int m_RewardCredit_Min;

	public int m_RewardCredit_Max;

	private NKMDeckCondition m_DeckCondition;

	public static IEnumerable<NKMPhaseTemplet> Values => NKMTempletContainer<NKMPhaseTemplet>.Values;

	public int Key => Id;

	public int Id { get; private set; }

	public string StrId { get; private set; }

	public string DebugName => $"[{Id}]{StrId}";

	public bool BonusResult => m_bBonus_Resource;

	public string Name { get; private set; }

	public string Desc { get; private set; }

	public string Icon { get; private set; }

	public int PhaseLevel { get; private set; }

	public IReadOnlyList<DungeonReward> Rewards => rewards;

	public NKMStageTempletV2 StageTemplet { get; internal set; }

	public NKMPhaseGroupTemplet PhaseList { get; private set; }

	public NKMDungeonEventDeckTemplet EventDeckTemplet { get; private set; }

	public NKMDeckCondition DeckCondition => m_DeckCondition;

	public string GetName()
	{
		return NKCStringTable.GetString(Name);
	}

	public string GetDesc()
	{
		return NKCStringTable.GetString(Desc);
	}

	public NKMPhaseOrderTemplet GetPhase(int phaseIndex)
	{
		if (phaseIndex < 0)
		{
			return null;
		}
		if (PhaseList.List.Count > phaseIndex)
		{
			return PhaseList.List[phaseIndex];
		}
		return null;
	}

	public int GetPhaseCount()
	{
		return PhaseList.List.Count;
	}

	public bool IsLastStageIndex(int i)
	{
		return i == PhaseList.List.Count - 1;
	}

	public static NKMPhaseTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMPhaseTemplet.cs", 53))
		{
			return null;
		}
		NKMPhaseTemplet nKMPhaseTemplet = new NKMPhaseTemplet();
		nKMPhaseTemplet.Id = lua.GetInt32("m_PhaseID");
		nKMPhaseTemplet.phaseGroupId = lua.GetInt32("m_PhaseGroupID");
		nKMPhaseTemplet.StrId = lua.GetString("m_PhaseStrID");
		nKMPhaseTemplet.Name = lua.GetString("m_PhaseName");
		nKMPhaseTemplet.Desc = lua.GetString("m_PhaseDesc");
		nKMPhaseTemplet.Icon = lua.GetString("m_PhaseIcon");
		nKMPhaseTemplet.PhaseLevel = lua.GetInt32("m_PhaseLevel");
		lua.GetData("m_UseEventDeck", ref nKMPhaseTemplet.m_UseEventDeck);
		lua.GetData("m_bBonus_Resource", ref nKMPhaseTemplet.m_bBonus_Resource);
		lua.GetData("m_CutScenStrIDBefore", ref nKMPhaseTemplet.m_CutScenStrIDBefore);
		lua.GetData("m_CutScenStrIDAfter", ref nKMPhaseTemplet.m_CutScenStrIDAfter);
		lua.GetData("m_Intro", ref nKMPhaseTemplet.m_Intro);
		lua.GetData("m_LoadingImage", ref nKMPhaseTemplet.m_LoadingImage);
		lua.GetData("m_Outro", ref nKMPhaseTemplet.m_Outro);
		lua.GetData("m_DGMissionType_1", ref nKMPhaseTemplet.m_DGMissionType_1);
		lua.GetData("m_DGMissionValue_1", ref nKMPhaseTemplet.m_DGMissionValue_1);
		lua.GetData("m_DGMissionType_2", ref nKMPhaseTemplet.m_DGMissionType_2);
		lua.GetData("m_DGMissionValue_2", ref nKMPhaseTemplet.m_DGMissionValue_2);
		lua.GetData("m_RewardUserEXP", ref nKMPhaseTemplet.m_RewardUserEXP);
		lua.GetData("m_RewardUnitEXP", ref nKMPhaseTemplet.m_RewardUnitEXP);
		lua.GetData("m_RewardCredit_Min", ref nKMPhaseTemplet.m_RewardCredit_Min);
		lua.GetData("m_RewardCredit_Max", ref nKMPhaseTemplet.m_RewardCredit_Max);
		for (int i = 0; i < DungeonReward.MAX_REWARD_COUNT; i++)
		{
			DungeonReward dungeonReward = new DungeonReward();
			if (lua.GetData($"m_RewardGroupID_{i + 1}", ref dungeonReward.m_RewardGroupID) && lua.GetData($"m_fRewardRate_{i + 1}", ref dungeonReward.m_RewardRate))
			{
				nKMPhaseTemplet.rewards.Add(dungeonReward);
			}
		}
		NKMDeckCondition.LoadFromLua(lua, "DECK_CONDITION", out nKMPhaseTemplet.m_DeckCondition, $"NKMPhaseTemplet Condition Parse Fail : {nKMPhaseTemplet.Id}");
		return nKMPhaseTemplet;
	}

	public static NKMPhaseTemplet Find(int key)
	{
		return NKMTempletContainer<NKMPhaseTemplet>.Find(key);
	}

	public static NKMPhaseTemplet Find(string key)
	{
		return NKMTempletContainer<NKMPhaseTemplet>.Find(key);
	}

	public bool IsUsingEventDeck()
	{
		return m_UseEventDeck != 0;
	}

	public void Join()
	{
		PhaseList = NKMPhaseGroupTemplet.Find(phaseGroupId);
		if (PhaseList == null)
		{
			NKMTempletError.Add($"{DebugName} invalid groupId:{phaseGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMPhaseTemplet.cs", 116);
		}
		if (m_UseEventDeck > 0)
		{
			EventDeckTemplet = NKMDungeonManager.GetEventDeckTemplet(m_UseEventDeck);
			if (EventDeckTemplet == null)
			{
				NKMTempletError.Add($"{DebugName} invalid eventDeckId:{m_UseEventDeck}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMPhaseTemplet.cs", 124);
			}
		}
	}

	public void Validate()
	{
		if (m_UseEventDeck > 0)
		{
			return;
		}
		foreach (NKMPhaseOrderTemplet item in PhaseList.List)
		{
			if (item.Dungeon.m_UseEventDeck != 0)
			{
				NKMTempletError.Add(DebugName + " 이벤트덱 미사용 페이즈 모드는 이벤트덱 미사용 던전만 연결 가능. " + $"phaseGroupId: {item.PhaseGroupId}, dungeon id: {item.Dungeon.m_DungeonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMPhaseTemplet.cs", 137);
			}
		}
	}

	public bool IsFirstPhase(NKMDungeonTempletBase dungeonTemplet)
	{
		return PhaseList.List.First().Dungeon == dungeonTemplet;
	}

	public bool IsLastPhase(NKMDungeonTempletBase dungeonTemplet)
	{
		return PhaseList.List.Last().Dungeon == dungeonTemplet;
	}

	public int DecideRewardCredit()
	{
		return PerThreadRandom.Instance.Next(m_RewardCredit_Min, m_RewardCredit_Max + 1);
	}
}
