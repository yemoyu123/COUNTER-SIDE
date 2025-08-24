using System;
using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMPointExchangeTemplet : INKMTemplet, INKMTempletEx
{
	private int pointExchangeId;

	private string openTag;

	private string intervalTag;

	private int missionTabId;

	private string shopTabStrId;

	private int shopTabId;

	private List<int> usePointId;

	private string pointExchangeBannerId;

	private string pointExchangePrefabId;

	private NKMIntervalTemplet intervalTemplet;

	private NKMMissionTabTemplet missionTabTemplet;

	private DateTime startDate;

	private DateTime endDate;

	public int Key => pointExchangeId;

	public string IntervalTag => intervalTag;

	public int PointItemId => usePointId.First();

	public DateTime StartDate => startDate;

	public DateTime EndDate => endDate;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public static IEnumerable<NKMPointExchangeTemplet> Values => NKMTempletContainer<NKMPointExchangeTemplet>.Values.OrderBy((NKMPointExchangeTemplet e) => e.intervalTemplet.StartDate);

	public string PrefabId => pointExchangePrefabId;

	public string BannerId => pointExchangeBannerId;

	public int MissionTabId => missionTabId;

	public string ShopTabStrId => shopTabStrId;

	public int ShopTabSubIndex => shopTabId;

	public List<int> UsePointId => usePointId;

	public NKMIntervalTemplet IntervalTemplet => intervalTemplet;

	public static NKMPointExchangeTemplet GetByTime(DateTime current)
	{
		return Values.FirstOrDefault((NKMPointExchangeTemplet e) => e.IsActive(current));
	}

	public static NKMPointExchangeTemplet Find(int key)
	{
		return NKMTempletContainer<NKMPointExchangeTemplet>.Find(key);
	}

	public static NKMPointExchangeTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPointExchangeTemplet.cs", 49))
		{
			return null;
		}
		NKMPointExchangeTemplet nKMPointExchangeTemplet = new NKMPointExchangeTemplet
		{
			pointExchangeId = lua.GetInt32("PointExchangeID"),
			openTag = lua.GetString("OpenTag"),
			intervalTag = lua.GetString("IntervalTag"),
			missionTabId = lua.GetInt32("MissionTabID"),
			shopTabStrId = lua.GetString("ShopTabStrID"),
			shopTabId = lua.GetInt32("ShopTabID"),
			usePointId = new List<int>(),
			pointExchangeBannerId = lua.GetString("PointExchangeBannerID"),
			pointExchangePrefabId = lua.GetString("PointExchangePrefabID")
		};
		if (lua.GetDataList("UsePointID", out List<int> result, nullIfEmpty: false))
		{
			nKMPointExchangeTemplet.usePointId = result;
		}
		return nKMPointExchangeTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void Validate()
	{
		if (usePointId.Count <= 0)
		{
			NKMTempletError.Add($"[NKMPointExchangeTemplet:{Key}] 이벤트에 사용되는 uesPointId 정보가 없음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPointExchangeTemplet.cs", 83);
		}
		else if (usePointId.Any((int e) => NKMItemMiscTemplet.Values.Any((NKMItemMiscTemplet item) => item.m_ItemMiscID == e && !item.EnableByTag)))
		{
			NKMTempletError.Add(string.Format("[NKMPointExchangeTemplet:{0}] 이벤트에 사용되는 재화가 존재하지 않거나 활성화되지 않음. usePointId:{1}", Key, string.Join(",", UsePointId)), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPointExchangeTemplet.cs", 87);
		}
		if (NKMMissionManager.GetMissionTempletListByTabID(missionTabId).Count <= 0)
		{
			NKMTempletError.Add($"[NKMPointExchangeTemplet:{Key}] 실제 미션이 없음. missionTabId:{missionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPointExchangeTemplet.cs", 93);
		}
	}

	private bool IsActive(DateTime current)
	{
		if (!missionTabTemplet.EnableByTag || current < startDate || endDate < current)
		{
			return false;
		}
		return true;
	}

	private void JoinIntervalTemplet()
	{
		if (!string.IsNullOrEmpty(intervalTag))
		{
			intervalTemplet = NKMIntervalTemplet.Find(intervalTag);
			if (intervalTemplet == null)
			{
				intervalTemplet = NKMIntervalTemplet.Unuseable;
				NKMTempletError.Add($"[NKMPointExchangeTemplet:{Key}] 잘못된 interval id:{intervalTag}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPointExchangeTemplet.cs", 117);
			}
			if (intervalTemplet.IsRepeatDate)
			{
				NKMTempletError.Add($"[NKMPointExchangeTemplet:{Key}] 반복 기간설정 사용 불가. id:{intervalTag}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPointExchangeTemplet.cs", 122);
			}
		}
		missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionTabId);
		if (missionTabTemplet == null)
		{
			NKMTempletError.Add($"[NKMPointExchangeTemplet:{Key}] missionTabTemplet이 존재하지 않음. missionTabId:{missionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPointExchangeTemplet.cs", 129);
		}
		startDate = intervalTemplet.StartDate;
		endDate = intervalTemplet.EndDate;
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
