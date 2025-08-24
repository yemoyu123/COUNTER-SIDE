using System;
using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKCNewsManager
{
	public class CompTempletOrderAscending : IComparer<NKCNewsTemplet>
	{
		public int Compare(NKCNewsTemplet x, NKCNewsTemplet y)
		{
			if (x.m_Order < y.m_Order)
			{
				return -1;
			}
			if (x.m_Order == y.m_Order)
			{
				if (x.m_DateStartUtc < y.m_DateStartUtc)
				{
					return -1;
				}
				if (x.m_DateStartUtc == y.m_DateStartUtc)
				{
					if (x.Idx < y.Idx)
					{
						return -1;
					}
					return 1;
				}
				return 1;
			}
			return 1;
		}
	}

	public static string NKM_LOCAL_SAVE_NEXT_NEWS_POPUP_SHOW_TIME = "NKM_LOCAL_SAVE_NEXT_NEWS_POPUP_SHOW_TIME";

	private static Dictionary<int, NKCNewsTemplet> m_dicNewsTemplet = null;

	public static Dictionary<int, NKCNewsTemplet> DicNewsTemplet => m_dicNewsTemplet;

	public static bool LoadFromLua()
	{
		m_dicNewsTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT", "LUA_NEWS_TEMPLET", "NEWS_TEMPLET", NKCNewsTemplet.LoadFromLUA);
		if (m_dicNewsTemplet == null)
		{
			return false;
		}
		foreach (KeyValuePair<int, NKCNewsTemplet> item in m_dicNewsTemplet)
		{
			if (item.Value.m_DateStartUtc > item.Value.m_DateEndUtc)
			{
				Log.Error($"IDX {item.Value.Idx} 의 StartDate가 EndDate보다 늦음 - {item.Value.m_DateStart.ToShortDateString()} ~ {item.Value.m_DateEnd.ToShortDateString()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCNewsManager.cs", 89);
				return false;
			}
		}
		return true;
	}

	public static NKCNewsTemplet GetNewsTemplet(int idx)
	{
		if (m_dicNewsTemplet.ContainsKey(idx))
		{
			return m_dicNewsTemplet[idx];
		}
		return null;
	}

	public static List<NKCNewsTemplet> GetActivatedNewsTempletList()
	{
		List<NKCNewsTemplet> list = new List<NKCNewsTemplet>();
		foreach (NKCNewsTemplet value in DicNewsTemplet.Values)
		{
			if (NKCSynchronizedTime.IsEventTime(value.m_DateStartUtc, value.m_DateEndUtc))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static void SortByFilterType(DateTime now, out List<NKCNewsTemplet> newsList, out List<NKCNewsTemplet> noticeList)
	{
		newsList = new List<NKCNewsTemplet>();
		noticeList = new List<NKCNewsTemplet>();
		foreach (KeyValuePair<int, NKCNewsTemplet> item in m_dicNewsTemplet)
		{
			if (item.Value.m_DateEndUtc > now && item.Value.m_DateStartUtc <= now)
			{
				if (item.Value.m_FilterType == eNewsFilterType.NEWS)
				{
					newsList.Add(item.Value);
				}
				else if (item.Value.m_FilterType == eNewsFilterType.NOTICE)
				{
					noticeList.Add(item.Value);
				}
			}
		}
		newsList.Sort(new CompTempletOrderAscending());
		noticeList.Sort(new CompTempletOrderAscending());
	}

	public static bool CheckNeedNewsPopup(DateTime now)
	{
		if (GetActivatedNewsTempletList().Count == 0)
		{
			return false;
		}
		if (!PlayerPrefs.HasKey(GetPreferenceString(NKM_LOCAL_SAVE_NEXT_NEWS_POPUP_SHOW_TIME)))
		{
			return true;
		}
		if (new DateTime(long.Parse(PlayerPrefs.GetString(GetPreferenceString(NKM_LOCAL_SAVE_NEXT_NEWS_POPUP_SHOW_TIME)))) < now)
		{
			return true;
		}
		return false;
	}

	public static string GetPreferenceString(string baseString)
	{
		return $"{NKCScenManager.CurrentUserData().m_UserUID}_{baseString}";
	}
}
