using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMFierceBossGroupTemplet : INKMTemplet
{
	public const int HardModeLevel = 3;

	private static Dictionary<int, List<NKMFierceBossGroupTemplet>> GroupData;

	public HashSet<int> BossPenaltyGroupID = new HashSet<int>();

	public int FierceBossID;

	public int FierceBossGroupID;

	public int Level;

	public int OperationPower;

	public int DungeonID;

	public int StageReqItemID;

	public int StageReqItemCount;

	public string BCondStrID_1;

	public string BCondStrID_2;

	public int BasePoint;

	public int MaxDamagePoint;

	public int MaxTimePoint;

	public List<int> BCPreconditionGroups = new List<int>();

	public bool UI_HellModeCheck;

	public string UI_BossFaceCard;

	public string UI_BossFaceSlot;

	public string UI_BossTitle;

	public string UI_BossDesc;

	public string UI_BossPrefabStrID;

	public NKMVector2 UI_BossPrefabPos;

	public int UI_BossPrefabScale;

	public bool UI_BossPrefabFlip;

	public int Key => FierceBossID;

	public int MaxBossLevel => GroupData[FierceBossGroupID].Count();

	public static IEnumerable<NKMFierceBossGroupTemplet> Values => NKMTempletContainer<NKMFierceBossGroupTemplet>.Values;

	public static Dictionary<int, List<NKMFierceBossGroupTemplet>> Groups => GroupData;

	public static NKMFierceBossGroupTemplet Find(int bossId)
	{
		return NKMTempletContainer<NKMFierceBossGroupTemplet>.Find(bossId);
	}

	public static NKMFierceBossGroupTemplet LoadFromLUA(NKMLua lua)
	{
		NKMFierceBossGroupTemplet nKMFierceBossGroupTemplet = new NKMFierceBossGroupTemplet();
		bool flag = true;
		flag &= lua.GetData("FierceBossID", ref nKMFierceBossGroupTemplet.FierceBossID);
		flag &= lua.GetData("FierceBossGroupID", ref nKMFierceBossGroupTemplet.FierceBossGroupID);
		flag &= lua.GetData("Level", ref nKMFierceBossGroupTemplet.Level);
		flag &= lua.GetData("OperationPower", ref nKMFierceBossGroupTemplet.OperationPower);
		flag &= lua.GetData("DungeonID", ref nKMFierceBossGroupTemplet.DungeonID);
		lua.GetData("StageReqItemID", ref nKMFierceBossGroupTemplet.StageReqItemID);
		lua.GetData("StageReqItemCount", ref nKMFierceBossGroupTemplet.StageReqItemCount);
		flag &= lua.GetData("BCondStrID_1", ref nKMFierceBossGroupTemplet.BCondStrID_1);
		flag &= lua.GetData("BCondStrID_2", ref nKMFierceBossGroupTemplet.BCondStrID_2);
		flag &= lua.GetData("BasePoint", ref nKMFierceBossGroupTemplet.BasePoint);
		flag &= lua.GetData("MaxDamagePoint", ref nKMFierceBossGroupTemplet.MaxDamagePoint);
		flag &= lua.GetData("MaxTimePoint", ref nKMFierceBossGroupTemplet.MaxTimePoint);
		lua.GetDataList("BossPenaltyGroupID", out List<int> result, nullIfEmpty: false);
		lua.GetDataList("m_PreConditionGroup", out nKMFierceBossGroupTemplet.BCPreconditionGroups, nullIfEmpty: false);
		foreach (int item in result)
		{
			if (!nKMFierceBossGroupTemplet.BossPenaltyGroupID.Contains(item))
			{
				nKMFierceBossGroupTemplet.BossPenaltyGroupID.Add(item);
				continue;
			}
			NKMTempletError.Add($"[FierceBossGroupTemplet:{nKMFierceBossGroupTemplet.FierceBossGroupID}] penalty main group is exist.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceBossGroupTemplet.cs", 79);
			flag = false;
		}
		lua.GetData("UI_HellModeCheck", ref nKMFierceBossGroupTemplet.UI_HellModeCheck);
		lua.GetData("UI_BossFaceCard", ref nKMFierceBossGroupTemplet.UI_BossFaceCard);
		lua.GetData("UI_BossFaceSlot", ref nKMFierceBossGroupTemplet.UI_BossFaceSlot);
		lua.GetData("UI_BossTitle", ref nKMFierceBossGroupTemplet.UI_BossTitle);
		lua.GetData("UI_BossDesc", ref nKMFierceBossGroupTemplet.UI_BossDesc);
		lua.GetData("UI_BossPrefabStrID", ref nKMFierceBossGroupTemplet.UI_BossPrefabStrID);
		lua.GetData("UI_BossPrefabPos", out var rValue, string.Empty);
		if (!string.IsNullOrEmpty(rValue))
		{
			string[] array = rValue.Split(',');
			for (int i = 0; i < 2; i++)
			{
				int.TryParse(array[i], out var result2);
				if (i == 0)
				{
					nKMFierceBossGroupTemplet.UI_BossPrefabPos.x = result2;
				}
				else
				{
					nKMFierceBossGroupTemplet.UI_BossPrefabPos.y = result2;
				}
			}
		}
		lua.GetData("UI_BossPrefabScale", ref nKMFierceBossGroupTemplet.UI_BossPrefabScale);
		lua.GetData("UI_BossPrefabFlip", ref nKMFierceBossGroupTemplet.UI_BossPrefabFlip);
		if (!flag)
		{
			NKMTempletError.Add($"[FierceBossGroupTemplet:{nKMFierceBossGroupTemplet.FierceBossGroupID}] data is invalid", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceBossGroupTemplet.cs", 114);
			return null;
		}
		return nKMFierceBossGroupTemplet;
	}

	public void Join()
	{
		GroupData = (from e in Values
			orderby e.Level
			group e by e.FierceBossGroupID).ToDictionary((IGrouping<int, NKMFierceBossGroupTemplet> e) => e.Key, (IGrouping<int, NKMFierceBossGroupTemplet> e) => e.ToList());
		if (BossPenaltyGroupID.Count == 0)
		{
			BossPenaltyGroupID.Add(0);
		}
	}

	public void Validate()
	{
		if (StageReqItemID != 0 && NKMItemManager.GetItemMiscTempletByID(StageReqItemID) == null)
		{
			NKMTempletError.Add($"[FierceBossGroupTemplet.{Key}] stageReqItemId is Not Exists. StageReqItemId:{StageReqItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceBossGroupTemplet.cs", 140);
		}
	}

	public static NKMFierceBossGroupTemplet GetBossGroupTemplet(int bossGroupId, int level)
	{
		if (!Groups.ContainsKey(bossGroupId))
		{
			return null;
		}
		return Groups[bossGroupId].FirstOrDefault((NKMFierceBossGroupTemplet e) => e.Level == level);
	}

	public bool TryGetNextBossGroupTemplet(out NKMFierceBossGroupTemplet templet)
	{
		templet = null;
		if (IsBossMaxLevel())
		{
			return false;
		}
		int num = Level + 1;
		foreach (NKMFierceBossGroupTemplet item in GroupData[FierceBossGroupID])
		{
			if (item.Level == num)
			{
				templet = item;
				return true;
			}
		}
		return false;
	}

	public bool IsBossMaxLevel()
	{
		return Level == MaxBossLevel;
	}

	public bool IsHardModeLevel()
	{
		return Level >= 3;
	}

	public IEnumerable<NKMFiercePenaltyTemplet> GetBossPenaltyTemplets()
	{
		List<NKMFiercePenaltyTemplet> list = new List<NKMFiercePenaltyTemplet>();
		foreach (int item in BossPenaltyGroupID)
		{
			List<NKMFiercePenaltyTemplet> collection = NKMFiercePenaltyTemplet.PenaltyMainGroups[item];
			list.AddRange(collection);
		}
		return list;
	}

	public static void ValidateGameServerOnly()
	{
		foreach (NKMFierceBossGroupTemplet value in Values)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(value.DungeonID);
			if (dungeonTempletBase == null)
			{
				Log.ErrorAndExit($"해당 보스의 던전 템플릿이 존재하지 않습니다. fierceBossId: {value.FierceBossID}, dungeonId: {value.DungeonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceBossGroupTemplet.cs", 209);
				break;
			}
			if (dungeonTempletBase.m_DungeonType != NKM_DUNGEON_TYPE.NDT_FIERCE)
			{
				Log.ErrorAndExit($"해당 던전의 던전 타입이 유효하지 않습니다. fierceBossId: {value.FierceBossID}, dungeonId: {dungeonTempletBase.m_DungeonID}, dungeonType: {dungeonTempletBase.m_DungeonType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceBossGroupTemplet.cs", 215);
				break;
			}
		}
	}
}
