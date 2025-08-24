using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMWorldMapBuildingTemplet : INKMTemplet
{
	public class LevelTemplet : INKMTemplet
	{
		public sealed class CostItem
		{
			public int ItemID { get; }

			public int Count { get; }

			public NKMItemMiscTemplet Templet { get; internal set; }

			public CostItem(int itemId, int count)
			{
				ItemID = itemId;
				Count = count;
			}
		}

		private readonly List<CostItem> buildCostItems = new List<CostItem>();

		private CostItem clearCostItem;

		public int id;

		public string name;

		public string description;

		public string information;

		public int level;

		public string iconPath;

		public int reqCityLevel;

		public int reqBuildingID;

		public int reqBuildingLevel;

		public NKM_CITY_BUILDING_STAT cityStatType;

		public int cityStatValue;

		public string ManagerRoomPath;

		public int reqBuildingPoint;

		public int sortIndex;

		public string OpenTag;

		public int notBuildingTogether;

		public int reqClearDiveId;

		public int Key => id;

		public IReadOnlyList<CostItem> BuildCostItems => buildCostItems;

		public CostItem ClearCostItem => clearCostItem;

		public bool EnableByTag => NKMOpenTagManager.IsOpened(OpenTag);

		public static LevelTemplet LoadFromLUA(NKMLua cNKMLua)
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapBuildingTemplet.cs", 99))
			{
				return null;
			}
			LevelTemplet levelTemplet = new LevelTemplet();
			int num = (int)(1u & (cNKMLua.GetData("ID", ref levelTemplet.id) ? 1u : 0u) & (cNKMLua.GetData("NAME", ref levelTemplet.name) ? 1u : 0u) & (cNKMLua.GetData("DESCRIPTION", ref levelTemplet.description) ? 1u : 0u) & (cNKMLua.GetData("INFORMATION", ref levelTemplet.information) ? 1u : 0u)) & (cNKMLua.GetData("LEVEL", ref levelTemplet.level) ? 1 : 0);
			cNKMLua.GetData("ICON_PATH", ref levelTemplet.iconPath);
			cNKMLua.GetData("REQ_CITY_LEVEL", ref levelTemplet.reqCityLevel);
			cNKMLua.GetData("REQ_BUILDING_ID", ref levelTemplet.reqBuildingID);
			cNKMLua.GetData("REQ_BUILDING_LEVEL", ref levelTemplet.reqBuildingLevel);
			int num2 = num & (cNKMLua.GetData("COST_BUILDING_POINT", ref levelTemplet.reqBuildingPoint) ? 1 : 0);
			int rValue = 0;
			cNKMLua.GetData("COST_CREDIT", ref rValue);
			levelTemplet.buildCostItems.Add(new CostItem(1, rValue));
			int rValue2 = 0;
			cNKMLua.GetData("CLEAR_CREDIT", ref rValue2);
			levelTemplet.clearCostItem = new CostItem(1, rValue2);
			cNKMLua.GetData("CITY_STAT_TYPE", ref levelTemplet.cityStatType);
			cNKMLua.GetData("CITY_STAT_VALUE", ref levelTemplet.cityStatValue);
			cNKMLua.GetData("MANAGER_ROOM_PATH", ref levelTemplet.ManagerRoomPath);
			cNKMLua.GetData("DIVE_HIGHEST_CLEARED", ref levelTemplet.reqClearDiveId);
			if (!cNKMLua.GetData("SORT_INDEX", ref levelTemplet.sortIndex))
			{
				levelTemplet.sortIndex = int.MaxValue;
			}
			cNKMLua.GetData("m_OpenTag", ref levelTemplet.OpenTag);
			cNKMLua.GetData("NOT_BUILDING_TOGETHER", ref levelTemplet.notBuildingTogether);
			if (num2 == 0)
			{
				return null;
			}
			return levelTemplet;
		}

		public void Join()
		{
			foreach (CostItem buildCostItem in BuildCostItems)
			{
				buildCostItem.Templet = NKMItemManager.GetItemMiscTempletByID(buildCostItem.ItemID);
				if (buildCostItem.Templet == null)
				{
					Log.ErrorAndExit($"[WorldMapBuildTemplet] 요구 아이템 아이디가 올바르지 않음. m_BuildID : {id}, cost itemId : {buildCostItem.ItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapBuildingTemplet.cs", 146);
				}
			}
			clearCostItem.Templet = NKMItemManager.GetItemMiscTempletByID(clearCostItem.ItemID);
			if (clearCostItem.Templet == null)
			{
				Log.ErrorAndExit($"[WorldMapBuildTemplet] 요구 아이템 아이디가 올바르지 않음. m_BuildID : {id}, cost itemId : {clearCostItem.ItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapBuildingTemplet.cs", 153);
			}
		}

		public void Validate()
		{
			if (reqClearDiveId != 0 && NKMDiveTemplet.Find(reqClearDiveId) == null)
			{
				Log.ErrorAndExit($"[NKMWorldMapBuildingTemplet] 컬럼명: DIVE_HIGHEST_CLEARED, 다이브 클리어 조건의 아이디가 잘못되었습니다. dive id: {reqClearDiveId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapBuildingTemplet.cs", 163);
			}
		}

		public string GetName()
		{
			return NKCStringTable.GetString(name);
		}

		public string GetDesc()
		{
			return NKCStringTable.GetString(description);
		}

		public string GetInfo()
		{
			return NKCStringTable.GetString(information);
		}
	}

	public const int MAX_LEVEL = 10;

	private LevelTemplet[] levelTemplets = new LevelTemplet[10];

	public LevelTemplet[] LevelTemplets => levelTemplets;

	public NKM_CITY_BUILDING_STAT StatType { get; }

	public int Key { get; }

	public NKMWorldMapBuildingTemplet(int id, IEnumerable<LevelTemplet> levelTemplets)
	{
		Key = id;
		foreach (LevelTemplet levelTemplet in levelTemplets)
		{
			this.levelTemplets[levelTemplet.level - 1] = levelTemplet;
		}
		StatType = this.levelTemplets[0].cityStatType;
	}

	public static NKMWorldMapBuildingTemplet Find(int key)
	{
		return NKMTempletContainer<NKMWorldMapBuildingTemplet>.Find(key);
	}

	public LevelTemplet GetLevelTemplet(int level)
	{
		if (level > 0 && level <= levelTemplets.Length)
		{
			return levelTemplets[level - 1];
		}
		return null;
	}

	public int FindMaxLevel()
	{
		for (int i = 0; i < levelTemplets.Length; i++)
		{
			if (levelTemplets[i] == null)
			{
				return i;
			}
		}
		return 10;
	}

	public void Join()
	{
		for (int i = 0; i < 10; i++)
		{
			if (levelTemplets[i] != null)
			{
				levelTemplets[i].Join();
			}
		}
	}

	public void Validate()
	{
	}
}
