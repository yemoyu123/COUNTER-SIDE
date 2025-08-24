using System.Collections.Generic;
using System.Linq;
using ClientPacket.Event;
using Cs.Logging;
using Cs.Math;
using NKM.Templet.Base;

namespace NKM.EventPass;

public sealed class NKMEventPassMissionGroupTemplet : INKMTemplet
{
	private static Dictionary<int, Dictionary<int, List<NKMEventPassMissionGroupTemplet>>>[] missionGroupMap = new Dictionary<int, Dictionary<int, List<NKMEventPassMissionGroupTemplet>>>[2];

	private static int index = 0;

	private int key;

	public int MissionGroupId { get; private set; }

	public List<int> MissionSlots { get; private set; }

	public List<int> MissionIds { get; private set; }

	public EventPassMissionType MissionType { get; private set; }

	public bool IsRetryEnable { get; private set; }

	public int Key => key;

	public static Dictionary<int, List<NKMEventPassMissionGroupTemplet>> GetMissionGroupData(EventPassMissionType missionType, int missionGroupId)
	{
		Dictionary<int, Dictionary<int, List<NKMEventPassMissionGroupTemplet>>> dictionary = missionGroupMap[(int)missionType];
		if (dictionary == null)
		{
			return null;
		}
		if (!dictionary.ContainsKey(missionGroupId))
		{
			return null;
		}
		return dictionary[missionGroupId];
	}

	public static NKMEventPassMissionGroupTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 44))
		{
			return null;
		}
		int rValue = 0;
		bool data = cNKMLua.GetData("MissionGroupID", ref rValue);
		EventPassMissionType result = EventPassMissionType.Daily;
		data &= cNKMLua.GetData("GroupEnum", ref result);
		List<int> list = new List<int>();
		if (cNKMLua.OpenTable("EventMissionWeek"))
		{
			int i = 1;
			for (int rValue2 = 0; cNKMLua.GetData(i, ref rValue2); i++)
			{
				list.Add(rValue2);
			}
			cNKMLua.CloseTable();
		}
		data &= list.Count > 0;
		List<int> list2 = new List<int>();
		if (cNKMLua.OpenTable("MissionSlotIndex"))
		{
			int j = 1;
			for (int rValue3 = 0; cNKMLua.GetData(j, ref rValue3); j++)
			{
				if (list2.Contains(rValue3))
				{
					Log.ErrorAndExit($"[EventPass] 슬롯인덱스가 겹칩니다. missionGroupId: {rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 78);
					return null;
				}
				list2.Add(rValue3);
			}
			cNKMLua.CloseTable();
		}
		data &= list2.Count > 0;
		List<int> list3 = new List<int>();
		if (cNKMLua.OpenTable("MissionID"))
		{
			int k = 1;
			for (int rValue4 = 0; cNKMLua.GetData(k, ref rValue4); k++)
			{
				list3.Add(rValue4);
			}
			cNKMLua.CloseTable();
		}
		if (!(data & (list3.Count > 0)))
		{
			Log.ErrorAndExit($"[EventPassMissionGroupTemplet] data is invalid, mission group id: {rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 106);
			return null;
		}
		NKMEventPassMissionGroupTemplet nKMEventPassMissionGroupTemplet = new NKMEventPassMissionGroupTemplet
		{
			key = ++index,
			MissionGroupId = rValue,
			MissionSlots = list2,
			MissionIds = list3,
			MissionType = result,
			IsRetryEnable = (list2.Count < list3.Count)
		};
		int missionType = (int)nKMEventPassMissionGroupTemplet.MissionType;
		if (missionGroupMap[missionType] == null)
		{
			missionGroupMap[missionType] = new Dictionary<int, Dictionary<int, List<NKMEventPassMissionGroupTemplet>>>();
		}
		Dictionary<int, Dictionary<int, List<NKMEventPassMissionGroupTemplet>>> dictionary = missionGroupMap[missionType];
		if (!dictionary.ContainsKey(rValue))
		{
			dictionary.Add(rValue, new Dictionary<int, List<NKMEventPassMissionGroupTemplet>>());
		}
		foreach (int item in list)
		{
			if (!dictionary[rValue].ContainsKey(item))
			{
				dictionary[rValue].Add(item, new List<NKMEventPassMissionGroupTemplet>());
			}
			dictionary[rValue][item].Add(nKMEventPassMissionGroupTemplet);
		}
		return nKMEventPassMissionGroupTemplet;
	}

	public static List<NKMEventPassMissionGroupTemplet> GetMissionGroupList(EventPassMissionType missionType, int missionGroupId, int week)
	{
		if (missionGroupMap[(int)missionType] == null)
		{
			return null;
		}
		if (!missionGroupMap[(int)missionType].ContainsKey(missionGroupId))
		{
			Log.Error($"missionGroupId doesn't exist, id: {missionGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 157);
			return null;
		}
		if (!missionGroupMap[(int)missionType][missionGroupId].ContainsKey(week))
		{
			return GetLastMissionGroups(missionType, missionGroupId);
		}
		return missionGroupMap[(int)missionType][missionGroupId][week];
	}

	public static SortedList<int, int> GetRandomMissionIds(EventPassMissionType missionType, int missionGroupId, int week, int maxCount)
	{
		List<NKMEventPassMissionGroupTemplet> missionGroupList = GetMissionGroupList(missionType, missionGroupId, week);
		if (missionGroupList == null)
		{
			return null;
		}
		SortedList<int, int> sortedList = new SortedList<int, int>();
		foreach (NKMEventPassMissionGroupTemplet item in missionGroupList)
		{
			if (sortedList.Count >= maxCount)
			{
				break;
			}
			int count = item.MissionSlots.Count;
			for (int i = 0; i < count; i++)
			{
				int num = RandomGenerator.Next(item.MissionIds.Count);
				int missionId = item.MissionIds[num];
				int num2 = item.MissionSlots[i];
				if (sortedList.ContainsKey(num2))
				{
					continue;
				}
				NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionId);
				if (missionTemplet == null)
				{
					Log.Warn($"missionTemplet is null, missionId: {missionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 204);
				}
				else
				{
					if (!missionTemplet.EnableByTag || !missionTemplet.EnableByInterval)
					{
						continue;
					}
					if (sortedList.ContainsValue(missionId))
					{
						missionId = item.MissionIds.FirstOrDefault((int e) => e != missionId);
					}
					if (missionId != 0)
					{
						sortedList.Add(num2, missionId);
					}
				}
			}
		}
		return sortedList;
	}

	public static NKMEventPassMissionGroupTemplet GetMissionGroup(EventPassMissionType missionType, int missionGroupId, int week, int slotIndex)
	{
		List<NKMEventPassMissionGroupTemplet> missionGroupList = GetMissionGroupList(missionType, missionGroupId, week);
		if (missionGroupList == null)
		{
			return null;
		}
		NKMEventPassMissionGroupTemplet nKMEventPassMissionGroupTemplet = missionGroupList.FirstOrDefault((NKMEventPassMissionGroupTemplet e) => e.MissionSlots.Contains(slotIndex));
		if (nKMEventPassMissionGroupTemplet == null)
		{
			return null;
		}
		return nKMEventPassMissionGroupTemplet;
	}

	public static int GetRandomMissionId(EventPassMissionType missionType, int missionGroupId, int week, int slotIndex, IEnumerable<int> exceptMissionIds)
	{
		NKMEventPassMissionGroupTemplet missionGroup = GetMissionGroup(missionType, missionGroupId, week, slotIndex);
		if (missionGroup == null)
		{
			return 0;
		}
		IEnumerable<int> source = missionGroup.MissionIds.Except(exceptMissionIds);
		int num = source.Count();
		if (num == 0)
		{
			return 0;
		}
		int num2 = RandomGenerator.Next(num);
		return source.ElementAt(num2);
	}

	public static int GetRandomMissionId(NKMEventPassMissionGroupTemplet templet, IEnumerable<int> exceptMissionIds)
	{
		IEnumerable<int> source = templet.MissionIds.Except(exceptMissionIds);
		int num = source.Count();
		if (num == 0)
		{
			return 0;
		}
		int num2 = RandomGenerator.Next(num);
		return source.ElementAt(num2);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (MissionType == EventPassMissionType.Weekly && IsRetryEnable)
		{
			Log.ErrorAndExit($"[EventPassMissionGroupTemplet] 주간 미션의 갱신은 활성화되지 않아야합니다. mission group id: {MissionGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 298);
			return;
		}
		if ((from e in MissionIds
			group e by e).Any((IGrouping<int, int> e) => e.Count() > 1))
		{
			string arg = string.Join(", ", MissionIds);
			Log.ErrorAndExit($"[EventPassGroupTemplet] mission id is duplicated, group id: {MissionGroupId}, mission id list:{arg}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 305);
			return;
		}
		foreach (int missionId in MissionIds)
		{
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionId);
			if (missionTemplet == null)
			{
				Log.ErrorAndExit($"mission id is invalid, id: {missionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 314);
				return;
			}
			if (missionTemplet.m_TabTemplet.m_MissionType != NKM_MISSION_TYPE.EVENT_PASS)
			{
				Log.ErrorAndExit($"[EventPass] 해당 미션의 타입이 EVENT_PASS가 아닙니다. missionId: {missionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 320);
				return;
			}
		}
		Dictionary<int, Dictionary<int, List<NKMEventPassMissionGroupTemplet>>> dictionary = missionGroupMap[0];
		if (dictionary == null)
		{
			Log.ErrorAndExit("daily mission group is not initialized", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 328);
			return;
		}
		if (dictionary.Count == 0)
		{
			Log.ErrorAndExit("DailyMissionGroup's data is empry", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 334);
			return;
		}
		Dictionary<int, Dictionary<int, List<NKMEventPassMissionGroupTemplet>>> dictionary2 = missionGroupMap[1];
		if (dictionary2 == null)
		{
			Log.ErrorAndExit("weekly mission group is not initialized", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 341);
		}
		else if (dictionary2.Count == 0)
		{
			Log.ErrorAndExit("WeeklyMissionGroup's data is empty", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 347);
		}
		else if (MissionIds.Count < MissionSlots.Count)
		{
			Log.ErrorAndExit($"mission pool의 개수가 mission slot의 개수 보다 작습니다. index: {key}, mission pool의 개수: {MissionIds.Count}, mission slot의 개수: {MissionSlots.Count} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 353);
		}
	}

	public static void ValidateServerOnly()
	{
		ValidateNonDuplicatedMissions(EventPassMissionType.Daily);
		ValidateNonDuplicatedMissions(EventPassMissionType.Weekly);
	}

	private static void ValidateNonDuplicatedMissions(EventPassMissionType missionType)
	{
		Dictionary<int, Dictionary<int, List<NKMEventPassMissionGroupTemplet>>> dictionary = missionGroupMap[(int)missionType];
		if (dictionary == null)
		{
			return;
		}
		foreach (Dictionary<int, List<NKMEventPassMissionGroupTemplet>> value in dictionary.Values)
		{
			foreach (List<NKMEventPassMissionGroupTemplet> value2 in value.Values)
			{
				HashSet<int> verificationSet = new HashSet<int>();
				value2.ForEach(delegate(NKMEventPassMissionGroupTemplet groupTemplet)
				{
					foreach (int missionId in groupTemplet.MissionIds)
					{
						if (!verificationSet.Add(missionId))
						{
							Log.ErrorAndExit($"[EventPassMissionGroupTemplet] mission id is duplicated, mission group id: {groupTemplet.MissionGroupId}, mission id: {missionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 384);
							break;
						}
					}
				});
			}
		}
	}

	private static List<NKMEventPassMissionGroupTemplet> GetLastMissionGroups(EventPassMissionType missionType, int missionGroupId)
	{
		if (missionGroupMap[(int)missionType] == null)
		{
			return null;
		}
		if (!missionGroupMap[(int)missionType].ContainsKey(missionGroupId))
		{
			Log.Error($"missionGroupId doesn't exist, id: {missionGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassMissionGroupTemplet.cs", 404);
			return null;
		}
		Dictionary<int, List<NKMEventPassMissionGroupTemplet>> dictionary = missionGroupMap[(int)missionType][missionGroupId];
		int num = dictionary.Keys.LastOrDefault();
		if (num == 0)
		{
			return null;
		}
		return dictionary[num];
	}
}
