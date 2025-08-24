using System;
using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMWorldMapEventTemplet : INKMTemplet
{
	public sealed class EventGroupKey
	{
		public int BuildingId { get; }

		public int BuildingLv { get; }

		public EventGroupKey(int id, int lv)
		{
			BuildingId = id;
			BuildingLv = lv;
		}
	}

	public int worldmapEventID;

	public int raidBossId;

	public int groupID;

	public string groupName;

	public NKM_WORLDMAP_EVENT_TYPE eventType;

	public NKM_WORLDMAP_EVENT_GRADE eventGrade;

	public int eventLevel;

	public int stageID;

	public int ratio;

	public string thumbnail;

	public string spineSDName;

	public NKMDiveTemplet diveTemplet;

	public NKMRaidTemplet raidTemplet;

	public NKMDungeonTempletBase dungeonTempletBase;

	public List<EventGroupKey> EventGroupKeys = new List<EventGroupKey>();

	public List<int> reqBuildingIds = new List<int>();

	public int Key => worldmapEventID;

	public static NKMWorldMapEventTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapEventTemplet.cs", 34))
		{
			return null;
		}
		NKMWorldMapEventTemplet nKMWorldMapEventTemplet = new NKMWorldMapEventTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("EVENT_ID", ref nKMWorldMapEventTemplet.worldmapEventID);
		flag &= cNKMLua.GetData("GROUP_ID", ref nKMWorldMapEventTemplet.groupID);
		flag &= cNKMLua.GetData("GROUP_NAME", ref nKMWorldMapEventTemplet.groupName);
		flag &= cNKMLua.GetData("WORLDMAP_EVENT_TYPE", ref nKMWorldMapEventTemplet.eventType);
		flag &= cNKMLua.GetData("WORLDMAP_EVENT_GRADE", ref nKMWorldMapEventTemplet.eventGrade);
		flag &= cNKMLua.GetData("EVENT_LEVEL", ref nKMWorldMapEventTemplet.eventLevel);
		flag &= cNKMLua.GetData("STAGE_ID", ref nKMWorldMapEventTemplet.stageID);
		flag &= cNKMLua.GetData("RATIO", ref nKMWorldMapEventTemplet.ratio);
		cNKMLua.GetData("THUMBNAIL", ref nKMWorldMapEventTemplet.thumbnail);
		cNKMLua.GetData("WORLDMAP_EVENT_SD", ref nKMWorldMapEventTemplet.spineSDName);
		cNKMLua.GetData("Raid_Boss_ID", ref nKMWorldMapEventTemplet.raidBossId);
		string rValue = string.Empty;
		flag &= cNKMLua.GetData("REQ_BUILDING_ID", ref rValue);
		string rValue2 = string.Empty;
		flag &= cNKMLua.GetData("REQ_BUILDING_LEVEL", ref rValue2);
		if (!string.IsNullOrEmpty(rValue) && !string.IsNullOrEmpty(rValue2))
		{
			string[] array = rValue.Split(',');
			string[] array2 = rValue2.Split(',');
			if (array.Length != array2.Length)
			{
				NKMTempletError.Add($"reqBuildingId의 개수와 reqBuildingLevel의 개수 서로 다르니다. event id: {nKMWorldMapEventTemplet.worldmapEventID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapEventTemplet.cs", 65);
				return null;
			}
			for (int i = 0; i < array.Length; i++)
			{
				int num = Convert.ToInt32(array[i]);
				int lv = Convert.ToInt32(array2[i]);
				nKMWorldMapEventTemplet.EventGroupKeys.Add(new EventGroupKey(num, lv));
				nKMWorldMapEventTemplet.reqBuildingIds.Add(num);
			}
		}
		if (!flag)
		{
			return null;
		}
		return nKMWorldMapEventTemplet;
	}

	public static NKMWorldMapEventTemplet Find(int key)
	{
		return NKMTempletContainer<NKMWorldMapEventTemplet>.Find(key);
	}

	public void Join()
	{
		switch (eventType)
		{
		case NKM_WORLDMAP_EVENT_TYPE.WET_DIVE:
			diveTemplet = NKMDiveTemplet.Find(stageID);
			if (diveTemplet == null)
			{
				NKMTempletError.Add($"[NKMWorldMapEventTemplet] 다이브 정보가 존재하지 않음 worldmapEventID:{worldmapEventID}, stageID:{stageID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapEventTemplet.cs", 92);
			}
			break;
		case NKM_WORLDMAP_EVENT_TYPE.WET_RAID:
			raidTemplet = NKMRaidTemplet.Find(stageID);
			if (raidTemplet == null)
			{
				NKMTempletError.Add($"[NKMWorldMapEventTemplet] 레이드 정보가 존재하지 않음 worldmapEventID:{worldmapEventID}, stageID:{stageID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapEventTemplet.cs", 100);
			}
			break;
		}
	}

	public void Validate()
	{
		if (reqBuildingIds.Count == 0)
		{
			NKMTempletError.Add($"reqBuildingIds가 비어있습니다. event id: {worldmapEventID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapEventTemplet.cs", 110);
		}
		else if (EventGroupKeys.Count == 0)
		{
			NKMTempletError.Add($"EventGroupKeys가 비어있습니다. event id: {worldmapEventID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapEventTemplet.cs", 116);
		}
		else if (reqBuildingIds.Count != EventGroupKeys.Count)
		{
			NKMTempletError.Add($"reqBuildingIds의 개수와 EventGroupKeys의 개수가 서로 다릅니다. event id: {worldmapEventID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapEventTemplet.cs", 122);
		}
		else if (raidBossId > 0 && !NKMRaidSeasonTemplet.Values.Where((NKMRaidSeasonTemplet t) => t.RaidBossId == raidBossId).Any())
		{
			NKMTempletError.Add($"NKMWorldMapEventTemplet: 레이드 BossId 에 연결된 RAID_SEASON_TEMPLET 가 없음. RaidBossId: {raidBossId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapEventTemplet.cs", 128);
		}
	}
}
