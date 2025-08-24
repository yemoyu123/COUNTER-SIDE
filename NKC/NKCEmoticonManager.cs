using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientPacket.Community;
using Cs.Logging;
using NKM;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKCEmoticonManager
{
	private static List<NKMEmoticonData> m_lstEmoticonDatas = new List<NKMEmoticonData>();

	public static List<int> m_lstAniPreset = new List<int>();

	public static List<int> m_lstTextPreset = new List<int>();

	public static bool m_bReceivedEmoticonData = false;

	public static bool m_bWaitForPopup = false;

	private static string EmoticonKeyForm = "EMOTICON_RECENT_{0}";

	public static IEnumerable<NKMEmoticonData> EmoticonDatas => m_lstEmoticonDatas;

	public static bool HasEmoticon(int id)
	{
		return m_lstEmoticonDatas.FindIndex((NKMEmoticonData e) => e.emoticonId == id) >= 0;
	}

	public static List<int> GetAllEmoticonID()
	{
		return new List<int>(NKMTempletContainer<NKMEmoticonTemplet>.Values.Select((NKMEmoticonTemplet e) => e.m_EmoticonID));
	}

	public static void SetEmoticonDatas(List<NKMEmoticonData> emoticonDatas)
	{
		if (emoticonDatas != null)
		{
			m_lstEmoticonDatas = emoticonDatas;
		}
	}

	public static void AddEmoticonData(int emoticonId)
	{
		if (m_lstEmoticonDatas.FindIndex((NKMEmoticonData e) => e.emoticonId == emoticonId) < 0)
		{
			NKMEmoticonData nKMEmoticonData = new NKMEmoticonData();
			nKMEmoticonData.emoticonId = emoticonId;
			nKMEmoticonData.isFavorites = false;
			m_lstEmoticonDatas.Add(nKMEmoticonData);
		}
	}

	public static void UpdateEmoticonData(NKMEmoticonData emoticonData)
	{
		int num = m_lstEmoticonDatas.FindIndex((NKMEmoticonData e) => e.emoticonId == emoticonData.emoticonId);
		if (num < 0)
		{
			Log.Error($"EmoticonId: {emoticonData.emoticonId} not exist", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMEmoticonManagerEx.cs", 75);
		}
		else
		{
			m_lstEmoticonDatas[num] = emoticonData;
		}
	}

	public static bool IsFavorite(int emoticonId)
	{
		return m_lstEmoticonDatas.Find((NKMEmoticonData e) => e.emoticonId == emoticonId)?.isFavorites ?? false;
	}

	public static void SetFavorite(int emoticonId, bool isFavorite)
	{
		NKMEmoticonData nKMEmoticonData = m_lstEmoticonDatas.Find((NKMEmoticonData e) => e.emoticonId == emoticonId);
		if (nKMEmoticonData != null)
		{
			nKMEmoticonData.isFavorites = isFavorite;
		}
	}

	public static void SaveEmoticonRecent(int emoticonId)
	{
		NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(emoticonId);
		if (nKMEmoticonTemplet == null || nKMEmoticonTemplet.m_EmoticonType != NKM_EMOTICON_TYPE.NET_ANI)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		string key = string.Format(EmoticonKeyForm, nKMUserData.m_UserUID);
		List<int> emoticonRecent = GetEmoticonRecent(onlyFavorite: false);
		if (emoticonRecent.Contains(emoticonId))
		{
			emoticonRecent.Remove(emoticonId);
		}
		if (emoticonRecent.Count >= 12)
		{
			emoticonRecent.RemoveAt(emoticonRecent.Count - 1);
		}
		emoticonRecent.Insert(0, emoticonId);
		StringBuilder stringBuilder = new StringBuilder();
		int count = emoticonRecent.Count;
		for (int i = 0; i < count; i++)
		{
			stringBuilder.Append(emoticonRecent[i]);
			if (i < count - 1)
			{
				stringBuilder.Append(',');
			}
		}
		PlayerPrefs.SetString(key, stringBuilder.ToString());
	}

	public static List<int> GetEmoticonRecent(bool onlyFavorite)
	{
		List<int> list = new List<int>();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return list;
		}
		string text = PlayerPrefs.GetString(string.Format(EmoticonKeyForm, nKMUserData.m_UserUID), "");
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = text.Split(',');
			foreach (string s in array)
			{
				if (!int.TryParse(s, out var id))
				{
					continue;
				}
				if (onlyFavorite)
				{
					NKMEmoticonData nKMEmoticonData = m_lstEmoticonDatas.Find((NKMEmoticonData e) => e.emoticonId == id);
					if (nKMEmoticonData == null || !nKMEmoticonData.isFavorites)
					{
						continue;
					}
				}
				if (!list.Contains(id))
				{
					list.Add(id);
				}
			}
		}
		return list;
	}

	public static void DoAfterLogout()
	{
		m_bReceivedEmoticonData = false;
		if (m_lstEmoticonDatas != null)
		{
			m_lstEmoticonDatas.Clear();
		}
	}
}
