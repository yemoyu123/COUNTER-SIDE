using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public class NKMDamageManager
{
	public static Dictionary<int, NKMDamageTemplet> m_dicDamageTempletID = new Dictionary<int, NKMDamageTemplet>();

	public static Dictionary<string, NKMDamageTemplet> m_dicDamageTempletStrID = new Dictionary<string, NKMDamageTemplet>();

	public static void ValidateTempletServerOnly()
	{
		if (m_dicDamageTempletID == null)
		{
			return;
		}
		foreach (NKMDamageTemplet value in m_dicDamageTempletID.Values)
		{
			value.Validate();
		}
	}

	public static void LoadFromLUA(IEnumerable<string> fileNames, IEnumerable<string> listFileName)
	{
		m_dicDamageTempletID.Clear();
		m_dicDamageTempletStrID.Clear();
		foreach (string fileName in fileNames)
		{
			LoadFromLuaBase(fileName);
		}
		foreach (string item in listFileName)
		{
			LoadFromLua(item);
		}
		foreach (KeyValuePair<string, NKMDamageTemplet> item2 in m_dicDamageTempletStrID.Where((KeyValuePair<string, NKMDamageTemplet> e) => !e.Value.luaDataloaded))
		{
			NKMTempletError.Add("[DamageTemplet:" + item2.Key + "] 데미지템플릿베이스는 있으나 데미지템플릿이 없습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 923);
		}
		foreach (KeyValuePair<string, NKMDamageTemplet> item3 in m_dicDamageTempletStrID)
		{
			item3.Value.JoinExtraHitDT();
		}
	}

	private static void LoadFromLuaBase(string basefileName)
	{
		NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT", basefileName) || !nKMLua.OpenTable("m_dicDamageTempletStrID"))
		{
			Log.ErrorAndExit("lua file loading failed. filaName:" + basefileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 938);
			return;
		}
		int num = 1;
		int count = m_dicDamageTempletID.Count;
		while (nKMLua.OpenTable(num++))
		{
			NKMDamageTempletBase nKMDamageTempletBase = new NKMDamageTempletBase();
			nKMDamageTempletBase.LoadFromLUA(nKMLua);
			nKMDamageTempletBase.m_DamageTempletIndex = num + count;
			if (!m_dicDamageTempletStrID.TryGetValue(nKMDamageTempletBase.m_DamageTempletName, out var value))
			{
				value = new NKMDamageTemplet
				{
					m_DamageTempletBase = nKMDamageTempletBase
				};
				m_dicDamageTempletStrID.Add(nKMDamageTempletBase.m_DamageTempletName, value);
			}
			else
			{
				value.m_DamageTempletBase = nKMDamageTempletBase;
			}
			if (!m_dicDamageTempletID.ContainsKey(value.m_DamageTempletBase.m_DamageTempletIndex))
			{
				m_dicDamageTempletID.Add(value.m_DamageTempletBase.m_DamageTempletIndex, value);
			}
			nKMLua.CloseTable();
		}
	}

	private static void LoadFromLua(string fileName)
	{
		NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT", fileName) || !nKMLua.OpenTable("m_dicDamageTempletStrID"))
		{
			Log.ErrorAndExit("lua file loading failed. filaName:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 975);
			return;
		}
		int num = 1;
		while (nKMLua.OpenTable(num++))
		{
			string text = nKMLua.GetString("m_DamageTempletName");
			if (!m_dicDamageTempletStrID.TryGetValue(text, out var value))
			{
				NKMTempletError.Add("[DamageTemplet:" + text + "] 데미지템플릿은 있으나 데미지템플릿베이스가 없습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 985);
			}
			else
			{
				value.LoadFromLUA(nKMLua);
			}
			nKMLua.CloseTable();
		}
	}

	public static NKMDamageTemplet GetTempletByID(int damageID)
	{
		if (m_dicDamageTempletID.ContainsKey(damageID))
		{
			return m_dicDamageTempletID[damageID];
		}
		Log.Error("GetTempletByID null: " + damageID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 1004);
		return null;
	}

	public static NKMDamageTemplet GetTempletByStrID(string damageStrID)
	{
		if (m_dicDamageTempletStrID.ContainsKey(damageStrID))
		{
			return m_dicDamageTempletStrID[damageStrID];
		}
		Log.Error("GetTempletByStrID null: " + damageStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 1017);
		return null;
	}

	public static bool IsHitStunReact(NKM_REACT_TYPE type)
	{
		return type >= NKM_REACT_TYPE.NRT_DAMAGE_A;
	}
}
