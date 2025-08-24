using System;
using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public static class NKMMiniGameManager
{
	private static Dictionary<NKM_MINI_GAME_TYPE, List<NKMMiniGameTemplet>> dicMiniGameTemplets = new Dictionary<NKM_MINI_GAME_TYPE, List<NKMMiniGameTemplet>>();

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKMMiniGameTemplet>.Load("AB_SCRIPT", "LUA_MINIGAME_TEMPLET", "MINIGAME_TEMPLET", NKMMiniGameTemplet.LoadFromLua);
	}

	public static void Join()
	{
		List<NKMMiniGameTemplet> list = new List<NKMMiniGameTemplet>();
		_ = NKMMiniGameTemplet.Values;
		list.AddRange(NKMMiniGameTemplet.Values);
		list.AddRange(NKMMatchTenTemplet2.Values);
		dicMiniGameTemplets = (from templet in list
			group templet by templet.m_MiniGameType).ToDictionary((IGrouping<NKM_MINI_GAME_TYPE, NKMMiniGameTemplet> group) => group.Key, (IGrouping<NKM_MINI_GAME_TYPE, NKMMiniGameTemplet> group) => group.ToList());
	}

	public static void Validate()
	{
		if (!dicMiniGameTemplets.Any() || (from templet in dicMiniGameTemplets.Values.SelectMany((List<NKMMiniGameTemplet> list) => list)
			group templet by templet.Key).Any((IGrouping<int, NKMMiniGameTemplet> group) => group.Count() > 1))
		{
			return;
		}
		foreach (NKM_MINI_GAME_TYPE key in dicMiniGameTemplets.Keys)
		{
			foreach (NKMMiniGameTemplet item in dicMiniGameTemplets[key])
			{
				if (!item.EnableByTag)
				{
					continue;
				}
				foreach (NKMMiniGameTemplet item2 in dicMiniGameTemplets[key])
				{
					if (item != item2 && (item.IntervalTemplet.IsValidTime(item2.IntervalTemplet.StartDate) || item.IntervalTemplet.IsValidTime(item2.IntervalTemplet.EndDate)))
					{
						NKMTempletError.Add("[NKMMiniGameManager] : 템플릿의 인터벌이 서로 겹침", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMiniGameManager.cs", 69);
						return;
					}
				}
			}
		}
	}

	public static NKMMiniGameTemplet GetTemplet(int templetId)
	{
		NKMMiniGameTemplet nKMMiniGameTemplet = null;
		foreach (NKM_MINI_GAME_TYPE value in Enum.GetValues(typeof(NKM_MINI_GAME_TYPE)))
		{
			if (dicMiniGameTemplets.ContainsKey(value))
			{
				nKMMiniGameTemplet = dicMiniGameTemplets[value].FirstOrDefault((NKMMiniGameTemplet e) => e.Key == templetId);
				if (nKMMiniGameTemplet != null)
				{
					break;
				}
			}
		}
		return nKMMiniGameTemplet;
	}

	public static NKMMiniGameTemplet GetActiveTemplet(DateTime dateTime, NKM_MINI_GAME_TYPE type)
	{
		if (!dicMiniGameTemplets.ContainsKey(type))
		{
			return null;
		}
		return dicMiniGameTemplets[type].FirstOrDefault((NKMMiniGameTemplet e) => e.IsOn(dateTime));
	}

	public static Dictionary<NKM_MINI_GAME_TYPE, NKMMiniGameTemplet> GetActiveTemplets(DateTime dateTime)
	{
		Dictionary<NKM_MINI_GAME_TYPE, NKMMiniGameTemplet> dictionary = new Dictionary<NKM_MINI_GAME_TYPE, NKMMiniGameTemplet>();
		foreach (NKM_MINI_GAME_TYPE value in Enum.GetValues(typeof(NKM_MINI_GAME_TYPE)))
		{
			NKMMiniGameTemplet activeTemplet = GetActiveTemplet(dateTime, value);
			if (activeTemplet != null)
			{
				dictionary.Add(value, activeTemplet);
			}
		}
		return dictionary;
	}
}
