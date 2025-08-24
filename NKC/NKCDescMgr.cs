using System.Collections.Generic;
using AssetBundles;
using Cs.Logging;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKCDescMgr
{
	private static Dictionary<int, Dictionary<int, NKCDescTemplet>> m_dicNKCDescTemplet = new Dictionary<int, Dictionary<int, NKCDescTemplet>>();

	public static NKCDescTemplet GetDescTemplet(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return null;
		}
		return GetDescTemplet(unitData.m_UnitID, unitData.m_SkinID);
	}

	public static NKCDescTemplet GetDescTemplet(int unitID, int skinID, bool CheckVoiceBundle = true)
	{
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
		if (CheckVoiceBundle)
		{
			if (m_dicNKCDescTemplet.ContainsKey(unitID))
			{
				if (AssetBundleManager.IsBundleExists(NKMSkinManager.GetSkinTemplet(skinID)?.m_VoiceBundleName) && m_dicNKCDescTemplet[unitID].TryGetValue(skinID, out var value))
				{
					return value;
				}
				if (AssetBundleManager.IsBundleExists($"AB_UI_UNIT_VOICE_{nKMUnitTempletBase.m_UnitStrID}") && m_dicNKCDescTemplet[unitID].TryGetValue(0, out var value2))
				{
					return value2;
				}
			}
			if (nKMUnitTempletBase.m_BaseUnitID != 0 && nKMUnitTempletBase.m_BaseUnitID != unitID)
			{
				if (AssetBundleManager.IsBundleExists(NKMSkinManager.GetSkinTemplet(skinID)?.m_VoiceBundleName) && m_dicNKCDescTemplet[nKMUnitTempletBase.m_BaseUnitID].TryGetValue(skinID, out var value3))
				{
					return value3;
				}
				if (AssetBundleManager.IsBundleExists($"AB_UI_UNIT_VOICE_{nKMUnitTempletBase.BaseUnit.m_UnitStrID}") && m_dicNKCDescTemplet.ContainsKey(nKMUnitTempletBase.m_BaseUnitID) && m_dicNKCDescTemplet[nKMUnitTempletBase.m_BaseUnitID].TryGetValue(0, out var value4))
				{
					return value4;
				}
			}
		}
		if (m_dicNKCDescTemplet.ContainsKey(unitID))
		{
			if (m_dicNKCDescTemplet[unitID].TryGetValue(skinID, out var value5))
			{
				return value5;
			}
			if (m_dicNKCDescTemplet[unitID].TryGetValue(0, out var value6))
			{
				return value6;
			}
		}
		if (nKMUnitTempletBase.m_BaseUnitID != 0 && nKMUnitTempletBase.m_BaseUnitID != unitID)
		{
			NKCDescTemplet descTemplet = GetDescTemplet(nKMUnitTempletBase.m_BaseUnitID, skinID, CheckVoiceBundle);
			if (descTemplet != null)
			{
				return descTemplet;
			}
		}
		Log.Error("m_dicNKCDescTemplet Cannot find Key: " + unitID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDescMgr.cs", 168);
		return null;
	}

	public static void Init()
	{
		foreach (KeyValuePair<int, Dictionary<int, NKCDescTemplet>> item in m_dicNKCDescTemplet)
		{
			item.Value.Clear();
		}
		m_dicNKCDescTemplet.Clear();
		LoadFromLUA_Desc("LUA_DESC_TEMPLET");
	}

	public static bool LoadFromLUA_Desc(string fileName)
	{
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT", fileName) && nKMLua.OpenTable("m_dicNKCDescTempletByID"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKCDescTemplet nKCDescTemplet = new NKCDescTemplet();
				if (nKCDescTemplet.LoadFromLUA(nKMLua))
				{
					if (!m_dicNKCDescTemplet.ContainsKey(nKCDescTemplet.m_UnitID))
					{
						m_dicNKCDescTemplet.Add(nKCDescTemplet.m_UnitID, new Dictionary<int, NKCDescTemplet>());
					}
					if (!m_dicNKCDescTemplet[nKCDescTemplet.m_UnitID].ContainsKey(nKCDescTemplet.m_SkinID))
					{
						m_dicNKCDescTemplet[nKCDescTemplet.m_UnitID].Add(nKCDescTemplet.m_SkinID, nKCDescTemplet);
					}
					else
					{
						Log.Error($"m_dicNKCDescTemplet Duplicate unitID:{nKCDescTemplet.m_UnitID}, skinID:{nKCDescTemplet.m_SkinID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDescMgr.cs", 207);
					}
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		return true;
	}
}
