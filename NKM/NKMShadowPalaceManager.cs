using System.Collections.Generic;
using System.Linq;
using ClientPacket.Mode;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKM;

public class NKMShadowPalaceManager
{
	private static Dictionary<int, List<NKMShadowBattleTemplet>> dicShadowBattleTemplet = new Dictionary<int, List<NKMShadowBattleTemplet>>();

	public static List<int> GetAllShadowTempletId()
	{
		return NKMTempletContainer<NKMShadowPalaceTemplet>.Keys.ToList();
	}

	public static bool LoadFromLua()
	{
		NKMTempletContainer<NKMShadowPalaceTemplet>.Load("AB_SCRIPT", "LUA_SHADOW_PALACE_TEMPLET", "SHADOW_PALACE_TEMPLET", NKMShadowPalaceTemplet.LoadFromLUA);
		dicShadowBattleTemplet = NKMTempletLoader<NKMShadowBattleTemplet>.LoadGroup("AB_SCRIPT", "LUA_SHADOW_BATTLE_TEMPLET", "SHADOW_BATTLE_TEMPLET", NKMShadowBattleTemplet.LoadFromLUA);
		if (dicShadowBattleTemplet == null)
		{
			return false;
		}
		return true;
	}

	public static List<NKMShadowBattleTemplet> GetBattleTemplets(int palaceID)
	{
		NKMShadowPalaceTemplet nKMShadowPalaceTemplet = NKMTempletContainer<NKMShadowPalaceTemplet>.Find(palaceID);
		if (nKMShadowPalaceTemplet != null && dicShadowBattleTemplet.TryGetValue(nKMShadowPalaceTemplet.BATTLE_GROUP_ID, out var value))
		{
			return value;
		}
		return null;
	}

	public static NKMShadowPalaceTemplet GetPalaceTemplet(int palaceID)
	{
		return NKMTempletContainer<NKMShadowPalaceTemplet>.Find(palaceID);
	}

	private static string GetLastClearedPalaceKey(NKMUserData userData)
	{
		return "LAST_CLEARED_PALACE_" + userData.m_UserUID;
	}

	public static void SaveLastClearedPalace(int palaceID)
	{
		PlayerPrefs.SetInt(GetLastClearedPalaceKey(NKCScenManager.CurrentUserData()), palaceID);
		PlayerPrefs.Save();
	}

	public static int GetLastClearedPalace()
	{
		return PlayerPrefs.GetInt(GetLastClearedPalaceKey(NKCScenManager.CurrentUserData()), 0);
	}

	public static bool IsClearPalace(int palaceId)
	{
		List<NKMShadowBattleTemplet> battleTemplets = GetBattleTemplets(palaceId);
		if (battleTemplets == null)
		{
			return false;
		}
		NKMPalaceData nKMPalaceData = NKCScenManager.CurrentUserData().m_ShadowPalace.palaceDataList.Find((NKMPalaceData e) => e.palaceId == palaceId);
		if (nKMPalaceData == null)
		{
			return false;
		}
		if (battleTemplets.Count != nKMPalaceData.dungeonDataList.Where((NKMPalaceDungeonData e) => e.bestTime != 0).Count())
		{
			return false;
		}
		return true;
	}
}
