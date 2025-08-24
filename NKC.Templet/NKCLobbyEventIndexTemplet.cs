using System;
using System.Collections.Generic;
using Cs.Core.Util;
using Cs.Logging;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCLobbyEventIndexTemplet : INKMTemplet
{
	public int EventLobbyID;

	public string OpenTag;

	public string IntervalTag;

	public UnlockInfo UnlockInfo;

	public int SortIndex;

	public string BannerID;

	public NKM_SHORTCUT_TYPE ShortCutType;

	public string ShortCutParam;

	public List<int> m_lstAlarmMissionTab;

	public bool bLoginEntry;

	public string BigBannerID;

	public bool bBigBanner;

	private const string PlayerPrefKey = "LOBBY_EVENT_SEEN_DATE";

	public NKMIntervalTemplet IntervalTemplet => NKMIntervalTemplet.Find(IntervalTag);

	public bool EnableByTag => NKMOpenTagManager.IsOpened(OpenTag);

	public int Key => EventLobbyID;

	public static NKCLobbyEventIndexTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCLobbyEventIndexTemplet.cs", 35))
		{
			return null;
		}
		NKCLobbyEventIndexTemplet nKCLobbyEventIndexTemplet = new NKCLobbyEventIndexTemplet();
		int num = (int)(1u & (cNKMLua.GetData("EventLobbyID", ref nKCLobbyEventIndexTemplet.EventLobbyID) ? 1u : 0u) & (cNKMLua.GetData("OpenTag", ref nKCLobbyEventIndexTemplet.OpenTag) ? 1u : 0u)) & (cNKMLua.GetData("IntervalTag", ref nKCLobbyEventIndexTemplet.IntervalTag) ? 1 : 0);
		nKCLobbyEventIndexTemplet.UnlockInfo = UnlockInfo.LoadFromLua(cNKMLua);
		int num2 = num & (cNKMLua.GetData("SortIndex", ref nKCLobbyEventIndexTemplet.SortIndex) ? 1 : 0);
		cNKMLua.GetData("BannerID", ref nKCLobbyEventIndexTemplet.BannerID);
		int num3 = num2 & (cNKMLua.GetData("ShortCutType", ref nKCLobbyEventIndexTemplet.ShortCutType) ? 1 : 0);
		cNKMLua.GetData("ShortCutParam", ref nKCLobbyEventIndexTemplet.ShortCutParam);
		cNKMLua.GetDataList("m_lstAlarmMissionTab", out nKCLobbyEventIndexTemplet.m_lstAlarmMissionTab, nullIfEmpty: false);
		nKCLobbyEventIndexTemplet.bLoginEntry = cNKMLua.GetBoolean("bLoginEntry");
		cNKMLua.GetData("BigBannerID", ref nKCLobbyEventIndexTemplet.BigBannerID);
		cNKMLua.GetData("bBigBanner", ref nKCLobbyEventIndexTemplet.bBigBanner);
		if (num3 == 0)
		{
			return null;
		}
		return nKCLobbyEventIndexTemplet;
	}

	public static List<NKCLobbyEventIndexTemplet> GetCurrentLobbyEvents()
	{
		List<NKCLobbyEventIndexTemplet> list = new List<NKCLobbyEventIndexTemplet>();
		foreach (NKCLobbyEventIndexTemplet value in NKMTempletContainer<NKCLobbyEventIndexTemplet>.Values)
		{
			if (value.EnableByTag && NKCSynchronizedTime.IsEventTime(value.IntervalTemplet) && NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in value.UnlockInfo))
			{
				list.Add(value);
			}
		}
		list.Sort((NKCLobbyEventIndexTemplet x, NKCLobbyEventIndexTemplet y) => x.SortIndex.CompareTo(y.SortIndex));
		return list;
	}

	public bool CheckLobbyEventSeen()
	{
		string stringKey = NKCUtil.GetStringKey(string.Format("{0}_{1}", "LOBBY_EVENT_SEEN_DATE", Key));
		if (string.IsNullOrEmpty(stringKey))
		{
			return false;
		}
		if (!DateTime.TryParse(stringKey, out var result))
		{
			return false;
		}
		return result.IsBetween(IntervalTemplet.StartDate, IntervalTemplet.EndDate);
	}

	public void MarkLobbyEventAsSeen()
	{
		NKCUtil.SetStringKey(string.Format("{0}_{1}", "LOBBY_EVENT_SEEN_DATE", Key), NKCSynchronizedTime.ServiceTime.ToString());
		Log.Debug($"LobbyEvent {Key} Seen at {NKCSynchronizedTime.ServiceTime.ToString()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCLobbyEventIndexTemplet.cs", 106);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
