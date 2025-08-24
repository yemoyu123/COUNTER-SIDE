using System.Collections.Generic;
using System.Linq;
using ClientPacket.Item;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;
using NKM.Unit;
using NKM.Warfare;

namespace NKM.Templet;

public sealed class NKMUnitTempletBase : INKMTemplet
{
	private static readonly Dictionary<int, NKMUnitTempletBase> shipGroups = new Dictionary<int, NKMUnitTempletBase>();

	public int m_UnitID;

	public string m_UnitStrID = "";

	public int m_ShipGroupID;

	public NKM_UNIT_TYPE m_NKM_UNIT_TYPE;

	public NKM_UNIT_STYLE_TYPE m_NKM_UNIT_STYLE_TYPE = NKM_UNIT_STYLE_TYPE.NUST_COUNTER;

	public NKM_UNIT_STYLE_TYPE m_NKM_UNIT_STYLE_TYPE_SUB;

	public NKM_UNIT_ROLE_TYPE m_NKM_UNIT_ROLE_TYPE;

	public NKM_UNIT_GRADE m_NKM_UNIT_GRADE;

	public bool m_bAwaken;

	public NKM_UNIT_SOURCE_TYPE m_NKM_UNIT_SOURCE_TYPE;

	public NKM_UNIT_SOURCE_TYPE m_NKM_UNIT_SOURCE_TYPE_SUB;

	public NKM_FIND_TARGET_TYPE m_NKM_FIND_TARGET_TYPE;

	public NKM_FIND_TARGET_TYPE m_NKM_FIND_TARGET_TYPE_Desc;

	public List<string> m_lstUnitTag = new List<string>();

	public HashSet<NKM_UNIT_TAG> m_hsUnitTag = new HashSet<NKM_UNIT_TAG>();

	public bool m_bAirUnit;

	public int m_ContractTime = 60;

	public int m_StarGradeMax = 1;

	private string m_FirstOpenTag;

	private string m_BasicOpenTag;

	private string m_Title = "";

	private string m_Name = "";

	private string m_UnitDesc = "";

	private string m_TeamUp = "";

	public List<string> m_lstSearchKeyword;

	public string m_UnitTempletFileName = "";

	public string m_SpriteBundleName = "";

	public string m_SpriteName = "";

	public string m_SpriteMaterialName = "";

	public string m_SpriteBundleNameSub = "";

	public string m_SpriteNameSub = "";

	public string m_SpriteMaterialNameSub = "";

	public string m_FaceCardName = "";

	public string m_SpineIllustName = "";

	public string m_SpineSDName = "";

	public string m_MiniMapFaceName = "";

	public string m_MiniMapFaceNameSub = "";

	public string m_InvenIconName = "";

	public bool m_bExistVoiceBundle;

	public string m_CommonVoiceBundle = "";

	public int m_OprPassiveGroupID;

	public int m_BaseUnitID;

	public int m_RearmGrade;

	public int m_TacticGroup;

	public bool m_bDorm;

	public int m_ReactorId;

	public bool m_bHideBattleResult;

	public HashSet<string> m_hsActGroup;

	public bool m_bContractable = true;

	public bool m_bMonster;

	public bool m_bProfileUnit = true;

	private (int itemID, int count)[] m_OnRemoveItems = new(int, int)[2];

	private (int itemID, int count)[] m_OnExtractItems = new(int, int)[2];

	private (int itemID, int count) m_OnRemoveItemFromContract;

	private (int itemID, int count) m_OnExtractItemFromContract;

	private List<NKMRewardInfo> removeRewards = new List<NKMRewardInfo>();

	private List<NKMRewardInfo> extractRewards = new List<NKMRewardInfo>();

	private NKMRewardInfo removeRewardFromContract;

	private NKMRewardInfo extractRewardFromContract;

	public List<string> m_lstSkillStrID = new List<string>();

	public int Key => m_UnitID;

	public string DebugName
	{
		get
		{
			if (!IsShip())
			{
				return $"[{m_UnitID}]{m_UnitStrID}";
			}
			return $"[{m_ShipGroupID}]{m_UnitStrID}";
		}
	}

	public bool StopDefaultCoolTime => m_hsUnitTag.Contains(NKM_UNIT_TAG.NUT_FURY);

	public bool RespawnFreePos => m_hsUnitTag.Contains(NKM_UNIT_TAG.NUT_RESPAWN_FREE_POS);

	public bool IsRearmUnit
	{
		get
		{
			if (BaseUnit != null && m_RearmGrade > 0)
			{
				return m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER;
			}
			return false;
		}
	}

	public bool IsTrophy => m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER;

	public bool IsEnv => m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_ENV;

	public bool IsReactorUnit
	{
		get
		{
			if (m_ReactorId != 0)
			{
				return m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER;
			}
			return false;
		}
	}

	public string TeamUp => m_TeamUp;

	public static int ShipGroupIdCount => shipGroups.Count;

	public static IEnumerable<NKMUnitTempletBase> Values => NKMTempletContainer<NKMUnitTempletBase>.Values;

	public IReadOnlyList<NKMRewardInfo> RemoveRewards => removeRewards;

	public IReadOnlyList<NKMRewardInfo> ExtractRewards => extractRewards;

	public NKMRewardInfo RemoveRewardFromContract => removeRewardFromContract;

	public NKMRewardInfo ExtractRewardFromContract => extractRewardFromContract;

	public NKMUnitStatTemplet StatTemplet { get; internal set; }

	public NKMUnitTempletBase BaseUnit { get; private set; }

	public NKM_EQUIP_PRESET_TYPE EquipPresetType { get; private set; }

	public IReadOnlyList<NKMUnitMissionTemplet> UnitMissionTemplets { get; private set; }

	public NKMUnitExpTable ExpTable { get; private set; }

	public string Name => m_Name;

	public string Title => m_Title;

	public NKMCollectionUnitTemplet CollectionUnitTemplet { get; internal set; }

	public bool CollectionEnableByTag
	{
		get
		{
			if (!ContractEnableByTag)
			{
				return PickupEnableByTag;
			}
			return true;
		}
	}

	public bool PickupEnableByTag => NKMOpenTagManager.IsOpened(m_FirstOpenTag);

	internal bool ContractEnableByTag => NKMOpenTagManager.IsOpened(m_BasicOpenTag);

	public int GetRespawnLimitCount()
	{
		if (HasUnitTagType(NKM_UNIT_TAG.NUT_LIMIT_1))
		{
			return 1;
		}
		if (HasUnitTagType(NKM_UNIT_TAG.NUT_LIMIT_2))
		{
			return 2;
		}
		if (HasUnitTagType(NKM_UNIT_TAG.NUT_LIMIT_3))
		{
			return 3;
		}
		return -1;
	}

	public static IReadOnlyList<NKMUnitTempletBase> GetDraftBanCandidateList()
	{
		return Values.Where((NKMUnitTempletBase e) => e.CanBeDraftBanUnitCandidate()).ToList();
	}

	public static IReadOnlyList<NKMUnitTempletBase> GetDraftBanShipCandidateList()
	{
		return Values.Where((NKMUnitTempletBase e) => e.CanBeDraftBanShipCandidate()).ToList();
	}

	public static bool CanBeDraftPickCandidate(int unitId)
	{
		return Find(unitId)?.CanBeDraftPickCandidate() ?? false;
	}

	public string GetSkillStrID(int index)
	{
		if (index < m_lstSkillStrID.Count)
		{
			return m_lstSkillStrID[index];
		}
		return string.Empty;
	}

	public NKMUnitMissionTemplet GetUnitMissionTemplet(int missionId)
	{
		return UnitMissionTemplets.FirstOrDefault((NKMUnitMissionTemplet e) => e.MissionId == missionId);
	}

	public NKMUnitMissionStepTemplet GetUnitMissionStepTemplet(int stepId)
	{
		return UnitMissionTemplets.SelectMany((NKMUnitMissionTemplet e) => e.Steps).FirstOrDefault((NKMUnitMissionStepTemplet e) => e.StepId == stepId);
	}

	public override string ToString()
	{
		return $"[{Key}]{m_UnitStrID}";
	}

	public bool CanBeDraftBanUnitCandidate()
	{
		if (IsUnitStyleType() && (m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR || m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SR) && CollectionEnableByTag && m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && !m_bMonster)
		{
			return CollectionUnitTemplet != null;
		}
		return false;
	}

	public bool CanBeDraftBanShipCandidate()
	{
		if (IsUnitStyleType() && (m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR || m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SR) && CollectionEnableByTag && m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP && !m_bMonster)
		{
			return CollectionUnitTemplet != null;
		}
		return false;
	}

	public bool CanBeDraftPickCandidate()
	{
		if (IsUnitStyleType() && CollectionEnableByTag && m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && !m_bMonster)
		{
			return CollectionUnitTemplet != null;
		}
		return false;
	}

	public bool CanExtracte()
	{
		if (IsUnitStyleType() && (m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR || m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SR) && CollectionEnableByTag && m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && !m_bMonster)
		{
			return CollectionUnitTemplet != null;
		}
		return false;
	}

	public bool CanRearmament()
	{
		if (BaseUnit != null && m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER && CollectionEnableByTag)
		{
			return CollectionUnitTemplet != null;
		}
		return false;
	}

	public int GetSkillIndex(string skillStrID)
	{
		return m_lstSkillStrID.IndexOf(skillStrID);
	}

	public int GetSkillIndex(int skillID)
	{
		return GetSkillIndex(NKMUnitSkillManager.GetSkillStrID(skillID));
	}

	public int GetShipSkillIndex(int skillID)
	{
		return GetSkillIndex(NKMShipSkillManager.GetSkillStrID(skillID));
	}

	public bool IsSameBaseUnit(int targetUnit)
	{
		if (targetUnit == m_UnitID)
		{
			return true;
		}
		if (m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return false;
		}
		if (m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP && m_ShipGroupID != 0 && m_ShipGroupID == targetUnit)
		{
			return true;
		}
		NKMUnitTempletBase nKMUnitTempletBase = Find(targetUnit);
		if (nKMUnitTempletBase == null)
		{
			return false;
		}
		if (nKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return false;
		}
		if (m_BaseUnitID != 0 && m_BaseUnitID == targetUnit)
		{
			return true;
		}
		if (nKMUnitTempletBase.m_BaseUnitID != 0 && nKMUnitTempletBase.m_BaseUnitID == m_UnitID)
		{
			return true;
		}
		if (m_BaseUnitID != 0 && m_BaseUnitID == nKMUnitTempletBase.m_BaseUnitID)
		{
			return true;
		}
		return false;
	}

	public int GetSkillCount()
	{
		return m_lstSkillStrID.Count;
	}

	public MovableTileSet GetWarfareMovableTileSet()
	{
		return MovableTileSet.GetTileSet(m_NKM_UNIT_STYLE_TYPE);
	}

	public static NKMUnitTempletBase LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 447))
		{
			return null;
		}
		NKMUnitTempletBase result = new NKMUnitTempletBase();
		bool flag = true;
		cNKMLua.GetData("m_UnitID", ref result.m_UnitID);
		cNKMLua.GetData("m_UnitStrID", ref result.m_UnitStrID);
		cNKMLua.GetData("m_ShipGroupID", ref result.m_ShipGroupID);
		flag &= cNKMLua.GetData("m_NKM_UNIT_TYPE", ref result.m_NKM_UNIT_TYPE);
		flag &= cNKMLua.GetData("m_NKM_UNIT_STYLE_TYPE", ref result.m_NKM_UNIT_STYLE_TYPE);
		cNKMLua.GetData("m_NKM_UNIT_STYLE_TYPE_SUB", ref result.m_NKM_UNIT_STYLE_TYPE_SUB);
		flag &= cNKMLua.GetData("m_NKM_UNIT_ROLE_TYPE", ref result.m_NKM_UNIT_ROLE_TYPE);
		flag &= cNKMLua.GetData("m_NKM_UNIT_GRADE", ref result.m_NKM_UNIT_GRADE);
		cNKMLua.GetData("m_bAwaken", ref result.m_bAwaken);
		cNKMLua.GetData("m_NKM_UNIT_SOURCE_TYPE", ref result.m_NKM_UNIT_SOURCE_TYPE);
		cNKMLua.GetData("m_NKM_UNIT_SOURCE_TYPE_SUB", ref result.m_NKM_UNIT_SOURCE_TYPE_SUB);
		flag &= cNKMLua.GetData("m_NKM_FIND_TARGET_TYPE", ref result.m_NKM_FIND_TARGET_TYPE);
		cNKMLua.GetData("m_NKM_FIND_TARGET_TYPE_Desc", ref result.m_NKM_FIND_TARGET_TYPE_Desc);
		cNKMLua.GetDataList("m_lstUnitTag", out result.m_lstUnitTag, nullIfEmpty: true);
		cNKMLua.GetDataListEnum("m_hsUnitTag", result.m_hsUnitTag);
		LowerCompability("m_bTagPatrol", NKM_UNIT_TAG.NUT_PATROL);
		LowerCompability("m_bTagSwingby", NKM_UNIT_TAG.NUT_SWINGBY);
		LowerCompability("m_bTagRevenge", NKM_UNIT_TAG.NUT_REVENGE);
		LowerCompability("m_bRespawnFreePos", NKM_UNIT_TAG.NUT_RESPAWN_FREE_POS);
		LowerCompability("m_StopDefaultCoolTime", NKM_UNIT_TAG.NUT_FURY);
		cNKMLua.GetData("m_bAirUnit", ref result.m_bAirUnit);
		cNKMLua.GetData("m_ContractTime", ref result.m_ContractTime);
		cNKMLua.GetData("m_StarGradeMax", ref result.m_StarGradeMax);
		cNKMLua.GetData("m_FirstOpenTag", ref result.m_FirstOpenTag);
		cNKMLua.GetData("m_BasicOpenTag", ref result.m_BasicOpenTag);
		cNKMLua.GetData("m_Title", ref result.m_Title);
		cNKMLua.GetData("m_Name", ref result.m_Name);
		cNKMLua.GetData("m_UnitDesc", ref result.m_UnitDesc);
		cNKMLua.GetData("m_TeamUp", ref result.m_TeamUp);
		cNKMLua.GetDataList("m_lstSearchKeyword", out result.m_lstSearchKeyword, nullIfEmpty: true);
		cNKMLua.GetData("m_UnitTempletFileName", ref result.m_UnitTempletFileName);
		cNKMLua.GetData("m_SpriteBundleName", ref result.m_SpriteBundleName);
		cNKMLua.GetData("m_SpriteName", ref result.m_SpriteName);
		cNKMLua.GetData("m_SpriteMaterialName", ref result.m_SpriteMaterialName);
		cNKMLua.GetData("m_SpriteBundleNameSub", ref result.m_SpriteBundleNameSub);
		cNKMLua.GetData("m_SpriteNameSub", ref result.m_SpriteNameSub);
		cNKMLua.GetData("m_SpriteMaterialNameSub", ref result.m_SpriteMaterialNameSub);
		cNKMLua.GetData("m_FaceCardName", ref result.m_FaceCardName);
		cNKMLua.GetData("m_SpineIllustName", ref result.m_SpineIllustName);
		cNKMLua.GetData("m_SpineSDName", ref result.m_SpineSDName);
		cNKMLua.GetData("m_MiniMapFaceName", ref result.m_MiniMapFaceName);
		cNKMLua.GetData("m_MiniMapFaceNameSub", ref result.m_MiniMapFaceNameSub);
		cNKMLua.GetData("m_InvenIconName", ref result.m_InvenIconName);
		cNKMLua.GetData("m_bExistVoiceBundle", ref result.m_bExistVoiceBundle);
		cNKMLua.GetData("m_CommonVoiceBundle", ref result.m_CommonVoiceBundle);
		cNKMLua.GetData("m_bContractable", ref result.m_bContractable);
		cNKMLua.GetData("m_bMonster", ref result.m_bMonster);
		for (int i = 0; i < result.m_OnRemoveItems.Length; i++)
		{
			cNKMLua.GetData($"m_OnRemoveItemID_{i + 1}", ref result.m_OnRemoveItems[i].itemID);
			cNKMLua.GetData($"m_OnRemoveItemCount_{i + 1}", ref result.m_OnRemoveItems[i].count);
		}
		for (int j = 0; j < result.m_OnExtractItems.Length; j++)
		{
			cNKMLua.GetData($"m_OnExtractItemID_{j + 1}", ref result.m_OnExtractItems[j].itemID);
			cNKMLua.GetData($"m_OnExtractItemCount_{j + 1}", ref result.m_OnExtractItems[j].count);
		}
		cNKMLua.GetData("m_OnRemoveItemID_Contract", ref result.m_OnRemoveItemFromContract.itemID);
		cNKMLua.GetData("m_OnRemoveItemCount_Contract", ref result.m_OnRemoveItemFromContract.count);
		cNKMLua.GetData("m_OprPassiveGroupID", ref result.m_OprPassiveGroupID);
		result.m_lstSkillStrID.Clear();
		for (int k = 0; k < 5; k++)
		{
			string rValue = string.Empty;
			cNKMLua.GetData("m_SkillStrID" + (k + 1), ref rValue);
			if (!string.IsNullOrEmpty(rValue))
			{
				result.m_lstSkillStrID.Add(rValue);
			}
		}
		cNKMLua.GetData("m_OnExtractItemID_Contract", ref result.m_OnExtractItemFromContract.itemID);
		cNKMLua.GetData("m_OnExtractItemCount_Contract", ref result.m_OnExtractItemFromContract.count);
		cNKMLua.GetData("m_BaseUnitID", ref result.m_BaseUnitID);
		cNKMLua.GetData("m_RearmGrade", ref result.m_RearmGrade);
		cNKMLua.GetData("m_TacticGroup", ref result.m_TacticGroup);
		if (cNKMLua.GetDataList("m_ActGroup", out List<string> result2, nullIfEmpty: false))
		{
			result.m_hsActGroup = new HashSet<string>(result2);
		}
		else
		{
			result.m_hsActGroup = null;
		}
		cNKMLua.GetData("m_bDorm", ref result.m_bDorm);
		cNKMLua.GetData("m_bProfileMainUnit", ref result.m_bProfileUnit);
		cNKMLua.GetData("m_ReactorID", ref result.m_ReactorId);
		cNKMLua.GetData("m_bHideBattleResult", ref result.m_bHideBattleResult);
		if (!flag)
		{
			return null;
		}
		return result;
		void LowerCompability(string key, NKM_UNIT_TAG tag)
		{
			bool rbValue = false;
			cNKMLua.GetData(key, ref rbValue);
			if (rbValue)
			{
				result.m_hsUnitTag.Add(tag);
			}
		}
	}

	public static NKMUnitTempletBase Find(int key)
	{
		return NKMTempletContainer<NKMUnitTempletBase>.Find(key);
	}

	public static NKMUnitTempletBase Find(string key)
	{
		return NKMTempletContainer<NKMUnitTempletBase>.Find(key);
	}

	public static NKMUnitTempletBase Find(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return null;
		}
		return NKMTempletContainer<NKMUnitTempletBase>.Find(unitData.m_UnitID);
	}

	public static NKMUnitTempletBase FindByShipGroupId(int shipGroupId)
	{
		shipGroups.TryGetValue(shipGroupId, out var value);
		return value;
	}

	public static bool HasSameBaseUnit(int unitIdA, int unitIdB)
	{
		return Find(unitIdA)?.IsSameBaseUnit(unitIdB) ?? false;
	}

	public static List<NKMUnitTempletBase> Get_listNKMUnitTempletBaseForShip()
	{
		return new List<NKMUnitTempletBase>(Values.Where((NKMUnitTempletBase value) => value.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP));
	}

	public static List<NKMUnitTempletBase> Get_listNKMUnitTempletBaseForMonster()
	{
		return new List<NKMUnitTempletBase>(Values.Where((NKMUnitTempletBase value) => value.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && value.m_bMonster));
	}

	public static List<NKMUnitTempletBase> Get_listNKMUnitTempletBaseForUnit()
	{
		return new List<NKMUnitTempletBase>(Values.Where((NKMUnitTempletBase value) => value.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && !value.m_bMonster));
	}

	public void Join()
	{
		EquipPresetType = NKMEquipTemplet.UnitStyleToEquipPreset(m_NKM_UNIT_STYLE_TYPE);
		(int, int)[] onRemoveItems = m_OnRemoveItems;
		for (int i = 0; i < onRemoveItems.Length; i++)
		{
			(int, int) tuple = onRemoveItems[i];
			if (tuple.Item1 > 0)
			{
				NKM_REWARD_TYPE rewardType = NKM_REWARD_TYPE.RT_MISC;
				if (!NKMRewardTemplet.IsValidReward(rewardType, tuple.Item1))
				{
					Log.ErrorAndExit($"[UnitTempletBase] 해약 보상 아이템 정보가 존재하지 않음 m_UnitID : {m_UnitID}, m_OnRemoveItemID : {tuple.Item1}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 643);
				}
				List<NKMRewardInfo> list = removeRewards;
				NKMRewardInfo obj = new NKMRewardInfo
				{
					rewardType = rewardType
				};
				(obj.ID, obj.Count) = tuple;
				list.Add(obj);
			}
		}
		onRemoveItems = m_OnExtractItems;
		for (int i = 0; i < onRemoveItems.Length; i++)
		{
			(int, int) tuple3 = onRemoveItems[i];
			if (tuple3.Item1 > 0)
			{
				NKM_REWARD_TYPE rewardType2 = NKM_REWARD_TYPE.RT_MISC;
				if (!NKMRewardTemplet.IsValidReward(rewardType2, tuple3.Item1))
				{
					Log.ErrorAndExit($"[UnitTempletBase] 추출 보상 아이템 정보가 존재하지 않음 m_UnitID:{m_UnitID}, m_OnExtractItemID:{tuple3.Item1}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 657);
				}
				List<NKMRewardInfo> list2 = extractRewards;
				NKMRewardInfo obj2 = new NKMRewardInfo
				{
					rewardType = rewardType2
				};
				(obj2.ID, obj2.Count) = tuple3;
				list2.Add(obj2);
			}
			if (tuple3.Item1 <= 0 && tuple3.Item2 > 0)
			{
				Log.ErrorAndExit($"[UnitTempletBase] 비 정상적인 유닛 추출 보상 아이템 정보 m_UnitID:{m_UnitID}, m_OnExtractItemID:{tuple3.Item1} m_OnExtractItemCount:{tuple3.Item2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 665);
			}
		}
		if (m_OnRemoveItemFromContract.itemID > 0)
		{
			NKM_REWARD_TYPE rewardType3 = NKM_REWARD_TYPE.RT_MISC;
			if (!NKMRewardTemplet.IsValidReward(rewardType3, m_OnRemoveItemFromContract.itemID))
			{
				Log.ErrorAndExit($"[UnitTempletBase] 채용 해약 보상 아이템 정보가 존재하지 않음 m_UnitID : {m_UnitID}, m_OnRemoveItemFromContract : {m_OnRemoveItemFromContract.itemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 674);
			}
			removeRewardFromContract = new NKMRewardInfo
			{
				rewardType = rewardType3,
				ID = m_OnRemoveItemFromContract.itemID,
				Count = m_OnRemoveItemFromContract.count
			};
		}
		if (m_OnExtractItemFromContract.itemID > 0)
		{
			NKM_REWARD_TYPE rewardType4 = NKM_REWARD_TYPE.RT_MISC;
			if (!NKMRewardTemplet.IsValidReward(rewardType4, m_OnExtractItemFromContract.itemID))
			{
				Log.ErrorAndExit($"[UnitTempletBase] 채용 추출 보상 아이템 정보가 존재하지 않음 m_UnitID : {m_UnitID}, m_OnExtractItemFromContract : {m_OnExtractItemFromContract.itemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 685);
			}
			extractRewardFromContract = new NKMRewardInfo
			{
				rewardType = rewardType4,
				ID = m_OnExtractItemFromContract.itemID,
				Count = m_OnExtractItemFromContract.count
			};
		}
		StatTemplet = NKMUnitManager.GetUnitStatTemplet(m_UnitID);
		if (StatTemplet == null)
		{
			Log.ErrorAndExit($"[UnitTempletBase] NKMUnitTempletBase의 m_UnitID와 매칭되는 NKMUnitStatTemplet을 찾을 수 없음. m_UnitID : {m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 694);
		}
		if (!m_bMonster && m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			if (m_ShipGroupID <= 0)
			{
				Log.ErrorAndExit($"[UnitTempletBase] ship has no group id:{m_ShipGroupID} unitId:{m_UnitID} strid:{m_UnitStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 702);
			}
			if (!shipGroups.ContainsKey(m_ShipGroupID))
			{
				shipGroups.Add(m_ShipGroupID, this);
			}
		}
		if (m_RearmGrade < 0 || m_RearmGrade > 10)
		{
			NKMTempletError.Add($"[Unit]{DebugName} 올바르지 않은 rearmGrade:{m_RearmGrade}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 713);
		}
		if (m_BaseUnitID > 0)
		{
			BaseUnit = Find(m_BaseUnitID);
			if (BaseUnit == null)
			{
				NKMTempletError.Add($"[Unit]{DebugName} 올바르지 않은 baseUnitId:{m_BaseUnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 721);
			}
			else
			{
				if (BaseUnit == this && m_RearmGrade != 0)
				{
					NKMTempletError.Add($"[Unit]{DebugName} 재무장 기본 유닛은 재무장 0등급만 지정 가능. rearmGrade:{m_RearmGrade}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 727);
				}
				if (BaseUnit != this && m_RearmGrade == 0 && m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
				{
					NKMTempletError.Add($"[Unit]{DebugName} 재무장 기본 유닛이 아닌데 재무장 0등급으로 지정됨. rearmGrade:{m_RearmGrade} baseUnitId:{BaseUnit.DebugName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 733);
				}
			}
		}
		if (!m_bMonster && m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			UnitMissionTemplets = NKMTempletContainer<NKMUnitMissionTemplet>.Values.Where((NKMUnitMissionTemplet e) => e.UnitGrade == m_NKM_UNIT_GRADE).ToList();
			if (!UnitMissionTemplets.Any())
			{
				NKMTempletError.Add($"[UnitTempletBase]{DebugName} 유닛 미션 정보가 존재하지 않음 unitGrade:{m_NKM_UNIT_GRADE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 746);
			}
		}
		if (!m_bMonster && (m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL || m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP))
		{
			ExpTable = NKMUnitExpTableContainer.GetTable(m_NKM_UNIT_GRADE, m_RearmGrade, m_bAwaken);
			if (ExpTable == null)
			{
				NKMTempletError.Add($"[UnitTempletBase]{DebugName} 유닛 경험치 테이블 없음. unitGrade:{m_NKM_UNIT_GRADE} rearmGrade:{m_RearmGrade} awaken:{m_bAwaken}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 756);
			}
		}
	}

	public void Validate()
	{
		if (IsUnitStyleType() && (EquipPresetType == NKM_EQUIP_PRESET_TYPE.NEPT_INVLID || EquipPresetType == NKM_EQUIP_PRESET_TYPE.NEPT_NONE))
		{
			NKMTempletError.Add($"[UnitTemplet:{Key}] 장비 프리셋 타입 계산 오류. styleType:{m_NKM_UNIT_STYLE_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 768);
			NKMTempletError.Add($"[UnitTemplet:{Key}] 장비 프리셋 타입 계산 오류. styleType:{m_NKM_UNIT_STYLE_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 769);
		}
		for (int i = 0; i < m_lstSkillStrID.Count; i++)
		{
			switch (m_NKM_UNIT_TYPE)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				foreach (string item in m_lstSkillStrID)
				{
					if (NKMUnitSkillManager.GetSkillTempletContainer(item) == null)
					{
						Log.ErrorAndExit($"[UnitTempletBase] 유닛 스킬 정보가 존재하지 않음 m_UnitID:{m_UnitID} m_SkillStrID:{item}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 782);
					}
				}
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
				foreach (string item2 in m_lstSkillStrID)
				{
					if (NKMShipSkillManager.GetShipSkillTempletByStrID(item2) == null)
					{
						Log.ErrorAndExit($"[UnitTempletBase] 함선 스킬 정보가 존재하지 않음 m_UnitID:{m_UnitID} m_SkillStrID:{item2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 792);
					}
				}
				break;
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				foreach (string item3 in m_lstSkillStrID)
				{
					if (NKMTempletContainer<NKMOperatorSkillTemplet>.Find(item3) == null)
					{
						Log.ErrorAndExit($"[UnitTempletBase] 오퍼레이터 스킬 정보가 존재하지 않음 m_UnitID:{m_UnitID} m_SkillStrID:{item3}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 802);
					}
				}
				break;
			}
		}
		NKM_UNIT_STYLE_TYPE nKM_UNIT_STYLE_TYPE_SUB = m_NKM_UNIT_STYLE_TYPE_SUB;
		if ((uint)nKM_UNIT_STYLE_TYPE_SUB > 5u)
		{
			Log.ErrorAndExit($"[UnitTempletBase] 잘못된 유닛 서브타입. m_UnitID : {m_UnitID}, m_NKM_UNIT_STYLE_TYPE_SUB : {m_NKM_UNIT_STYLE_TYPE_SUB}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 819);
		}
		if (m_lstSkillStrID.Count > 5)
		{
			Log.ErrorAndExit($"[UnitTempletBase] 유닛 스킬의 개수가 비정상. unitId:{m_UnitID} skillCount:{m_lstSkillStrID.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 825);
		}
		if (BaseUnit == null && m_RearmGrade > 0)
		{
			NKMTempletError.Add($"[Unit]{DebugName} 재무장 그룹이 없는 유닛인데 rearmGrade가 설정됨:{m_RearmGrade}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 830);
		}
		if (BaseUnit != null)
		{
			if (m_bMonster)
			{
				NKMTempletError.Add("[Unit]" + DebugName + " 몬스터는 재무장 불가. baseUnit:" + BaseUnit.DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 837);
			}
			if (m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL)
			{
				NKMTempletError.Add($"[Unit]{DebugName} 재무장은 유닛 타입만 가능. unitType:{m_NKM_UNIT_TYPE} baseUnit:{BaseUnit.DebugName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 842);
			}
		}
		if (IsRearmUnit)
		{
			if (!m_lstSkillStrID.Select((string e) => NKMUnitSkillManager.GetSkillTempletContainer(e).SkillType).Any((NKM_SKILL_TYPE e) => e == NKM_SKILL_TYPE.NST_LEADER))
			{
				Log.Error("[Unit]" + DebugName + " 재무장 유닛은 리더스킬을 가져야 함.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 852);
			}
			if (m_ReactorId != 0)
			{
				NKMTempletError.Add("[Unit]" + DebugName + " 재무장 유닛은 얼터니움 리액터 아이디를 가지지 못함.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 857);
			}
			if (BaseUnit.m_ReactorId > 0)
			{
				NKMTempletError.Add($"[Unit]{DebugName} 재무장 원본 유닛은 얼터니움 리액터 아이디를 가지지 못함. baseUnit:{BaseUnit.m_UnitID} reactorId:{BaseUnit.m_ReactorId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 862);
			}
		}
		if (m_ReactorId <= 0)
		{
			return;
		}
		NKMUnitReactorTemplet nKMUnitReactorTemplet = NKMUnitReactorTemplet.Find(m_ReactorId);
		if (nKMUnitReactorTemplet == null)
		{
			NKMTempletError.Add($"[Unit]{DebugName} 얼터니움 리액터 아이디 값이 있으나 템플릿이 없음. reactorId:{m_ReactorId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 871);
			return;
		}
		NKMReactorSkillTemplet[] skillTemplets = nKMUnitReactorTemplet.skillTemplets;
		foreach (NKMReactorSkillTemplet skillTemplet in skillTemplets)
		{
			if (skillTemplet != null && !string.IsNullOrEmpty(skillTemplet.BaseSkillStrId) && !m_lstSkillStrID.Any((string e) => string.Equals(e, skillTemplet.BaseSkillStrId)))
			{
				NKMTempletError.Add($"[Unit]{DebugName} 얼터니움 리액터 아이디 값이 있으나 리액터 템플릿 내 SkillTemplet의 BaseSkillStrId값에 일치하는 값이 없음. unitId:{m_UnitID} reactorId:{m_ReactorId} skillTemplet:{skillTemplet.Key} Templet.BaseSkillStrId:{skillTemplet.BaseSkillStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitTempletBase.cs", 886);
			}
		}
	}

	public bool IsUnitStyleType()
	{
		return IsUnitStyleType(m_NKM_UNIT_STYLE_TYPE);
	}

	public static bool IsUnitStyleType(NKM_UNIT_STYLE_TYPE styleType)
	{
		if ((uint)(styleType - 1) <= 2u)
		{
			return true;
		}
		return false;
	}

	public bool IsShip()
	{
		if (m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT || m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER || m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_SHIP_ETC || m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY || m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL || m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL)
		{
			return true;
		}
		return false;
	}

	public bool HasUnitStyleType(HashSet<NKM_UNIT_STYLE_TYPE> hsType)
	{
		if (!hsType.Contains(m_NKM_UNIT_STYLE_TYPE))
		{
			return hsType.Contains(m_NKM_UNIT_STYLE_TYPE_SUB);
		}
		return true;
	}

	public bool HasUnitStyleType(NKM_UNIT_STYLE_TYPE type)
	{
		if (type == NKM_UNIT_STYLE_TYPE.NUST_INVALID)
		{
			return false;
		}
		if (m_NKM_UNIT_STYLE_TYPE != type)
		{
			return m_NKM_UNIT_STYLE_TYPE_SUB == type;
		}
		return true;
	}

	public bool HasUnitRoleType(HashSet<NKM_UNIT_ROLE_TYPE> hsType)
	{
		return hsType.Contains(m_NKM_UNIT_ROLE_TYPE);
	}

	public bool HasUnitRoleType(NKM_UNIT_ROLE_TYPE type)
	{
		if (type == NKM_UNIT_ROLE_TYPE.NURT_INVALID)
		{
			return false;
		}
		return m_NKM_UNIT_ROLE_TYPE == type;
	}

	public bool HasUnitTagType(HashSet<NKM_UNIT_TAG> hsType)
	{
		return m_hsUnitTag.Overlaps(hsType);
	}

	public bool HasUnitTagType(NKM_UNIT_TAG type)
	{
		return m_hsUnitTag.Contains(type);
	}

	public bool IsAllowUnitStyleType(HashSet<NKM_UNIT_STYLE_TYPE> hsAllowType, HashSet<NKM_UNIT_STYLE_TYPE> hsIgnoreType)
	{
		if (hsAllowType != null && hsAllowType.Count > 0 && !HasUnitStyleType(hsAllowType))
		{
			return false;
		}
		if (hsIgnoreType != null && hsIgnoreType.Count > 0 && HasUnitStyleType(hsIgnoreType))
		{
			return false;
		}
		return true;
	}

	public bool IsAllowUnitRoleType(HashSet<NKM_UNIT_ROLE_TYPE> hsAllow, HashSet<NKM_UNIT_ROLE_TYPE> hsIgnore)
	{
		if (hsAllow != null && hsAllow.Count > 0 && !hsAllow.Contains(m_NKM_UNIT_ROLE_TYPE))
		{
			return false;
		}
		if (hsIgnore != null && hsIgnore.Count > 0 && hsIgnore.Contains(m_NKM_UNIT_ROLE_TYPE))
		{
			return false;
		}
		return true;
	}

	public bool IsAllowUnitTagType(HashSet<NKM_UNIT_TAG> hsAllow, HashSet<NKM_UNIT_TAG> hsIgnore)
	{
		if (hsAllow != null && hsAllow.Count > 0 && !hsAllow.Overlaps(m_hsUnitTag))
		{
			return false;
		}
		if (hsIgnore != null && hsIgnore.Count > 0 && hsIgnore.Overlaps(m_hsUnitTag))
		{
			return false;
		}
		return true;
	}

	public bool HasSourceType(NKM_UNIT_SOURCE_TYPE sourceType)
	{
		if (m_NKM_UNIT_SOURCE_TYPE != sourceType)
		{
			return m_NKM_UNIT_SOURCE_TYPE_SUB == sourceType;
		}
		return true;
	}

	public string GetUnitTitle()
	{
		return NKCStringTable.GetString(m_Title);
	}

	public string GetUnitName()
	{
		return NKCStringTable.GetString(m_Name);
	}

	public string GetUnitDesc()
	{
		if (string.IsNullOrEmpty(m_UnitDesc))
		{
			return "";
		}
		return NKCStringTable.GetString(m_UnitDesc);
	}

	public bool IsUnitDescNullOrEmplty()
	{
		return string.IsNullOrEmpty(m_UnitDesc);
	}
}
