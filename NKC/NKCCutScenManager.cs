using System;
using System.Collections.Generic;
using Cs.Logging;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCCutScenManager
{
	public const string USER_NICKNAME = "<usernickname>";

	private static Dictionary<int, NKCCutScenTemplet> m_dicNKCCutScenTempletByID = new Dictionary<int, NKCCutScenTemplet>();

	private static Dictionary<string, NKCCutScenTemplet> m_dicNKCCutScenTempletByStrID = new Dictionary<string, NKCCutScenTemplet>();

	private static Dictionary<string, NKCCutScenCharTemplet> m_dicNKCCutScenCharTempletByStrID = new Dictionary<string, NKCCutScenCharTemplet>();

	private static Dictionary<string, NKC_CUTSCEN_TYPE> m_dicCutscenTypeByStrID = new Dictionary<string, NKC_CUTSCEN_TYPE>();

	public static void Init()
	{
		ClearCacheData();
		m_dicNKCCutScenCharTempletByStrID.Clear();
		m_dicCutscenTypeByStrID.Clear();
		LoadFromLUA_CutscenFileList("LUA_CUTSCENE_FILE_LIST");
		LoadFromLUA_CutSceneChar("LUA_CUTSCENE_CHAR_TEMPLET");
	}

	public static NKCCutScenTemplet GetCutScenTemple(string _CutScenStrID)
	{
		if (_CutScenStrID == "")
		{
			return null;
		}
		if (m_dicNKCCutScenTempletByStrID.ContainsKey(_CutScenStrID))
		{
			return m_dicNKCCutScenTempletByStrID[_CutScenStrID];
		}
		if (LoadFromLUA_CutScene(_CutScenStrID))
		{
			if (!m_dicNKCCutScenTempletByStrID.ContainsKey(_CutScenStrID))
			{
				Log.Error("[" + _CutScenStrID + "] Cutscene LUA Compile Error!!!!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCutScenManager.cs", 749);
			}
			return m_dicNKCCutScenTempletByStrID[_CutScenStrID];
		}
		Log.Error("m_dicNKCCutScenTempletByStrID Cannot find Key or not loaded : " + _CutScenStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCutScenManager.cs", 756);
		return null;
	}

	public static void ClearCacheData()
	{
		m_dicNKCCutScenTempletByID.Clear();
		m_dicNKCCutScenTempletByStrID.Clear();
	}

	public static NKCCutScenCharTemplet GetCutScenCharTempletByStrID(string _CharStrID)
	{
		if (m_dicNKCCutScenCharTempletByStrID.ContainsKey(_CharStrID))
		{
			return m_dicNKCCutScenCharTempletByStrID[_CharStrID];
		}
		Log.Error("m_dicNKCCutScenCharTempletByStrID Cannot find Key: " + _CharStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCutScenManager.cs", 794);
		return null;
	}

	public static List<string> GetCutScenTempletStrIDList(HashSet<NKC_CUTSCEN_TYPE> setExceptNKC_CUTSCEN_TYPE)
	{
		List<string> list = new List<string>(m_dicCutscenTypeByStrID.Keys);
		if (setExceptNKC_CUTSCEN_TYPE != null && setExceptNKC_CUTSCEN_TYPE.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (setExceptNKC_CUTSCEN_TYPE.Contains(m_dicCutscenTypeByStrID[list[i]]))
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		return list;
	}

	public static void GetCutScenTempletStrIDListFilteringByStr(List<string> _listCutScenTempletStrID, string strFilter)
	{
		for (int i = 0; i < _listCutScenTempletStrID.Count; i++)
		{
			if (!string.IsNullOrEmpty(strFilter) && !_listCutScenTempletStrID[i].Contains(strFilter))
			{
				_listCutScenTempletStrID.RemoveAt(i);
				i--;
			}
		}
	}

	public static List<Tuple<string, string>> GetSelectionRoutes(int id, int index)
	{
		if (m_dicNKCCutScenTempletByID.ContainsKey(id))
		{
			NKCCutScenTemplet nKCCutScenTemplet = m_dicNKCCutScenTempletByID[id];
			if (nKCCutScenTemplet.m_listCutTemplet[index].m_Action != NKCCutTemplet.eCutsceneAction.SELECT)
			{
				Debug.LogError("로직 오류 : 선택지가 아닌 곳에서 선택지 목록을 요청");
				return null;
			}
			List<Tuple<string, string>> list = new List<Tuple<string, string>>();
			for (int i = index; i < nKCCutScenTemplet.m_listCutTemplet.Count; i++)
			{
				NKCCutTemplet nKCCutTemplet = nKCCutScenTemplet.m_listCutTemplet[i];
				if (nKCCutTemplet.m_Action != NKCCutTemplet.eCutsceneAction.SELECT)
				{
					return list;
				}
				list.Add(new Tuple<string, string>(nKCCutTemplet.m_ActionStrKey, nKCCutTemplet.m_Talk));
			}
		}
		return null;
	}

	public static bool LoadFromLUA_CutSceneChar(string fileName)
	{
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT", fileName) && nKMLua.OpenTable("m_dicCutsceneCharTempletStrID"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKCCutScenCharTemplet nKCCutScenCharTemplet = new NKCCutScenCharTemplet();
				if (nKCCutScenCharTemplet.LoadFromLUA(nKMLua))
				{
					if (m_dicNKCCutScenCharTempletByStrID.ContainsKey(nKCCutScenCharTemplet.m_CharStrID))
					{
						Log.Error("m_dicCutsceneCharTempletStrID Duplicate Key: " + nKCCutScenCharTemplet.m_CharStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCutScenManager.cs", 888);
						return false;
					}
					m_dicNKCCutScenCharTempletByStrID.Add(nKCCutScenCharTemplet.m_CharStrID, nKCCutScenCharTemplet);
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		return true;
	}

	public static bool LoadFromLUA_CutscenFileList(string fileName)
	{
		m_dicCutscenTypeByStrID.Clear();
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT", fileName) && nKMLua.OpenTable("m_hsCutscenFileName"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				string rValue = "";
				NKC_CUTSCEN_TYPE result = NKC_CUTSCEN_TYPE.NCT_ETC;
				nKMLua.GetData("m_CutScenFile", ref rValue);
				nKMLua.GetData("m_CutScenType", ref result);
				if (m_dicCutscenTypeByStrID.ContainsKey(rValue))
				{
					Log.Error("Duplicate CutscenFileList File Name: " + rValue, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCutScenManager.cs", 926);
				}
				else
				{
					m_dicCutscenTypeByStrID.Add(rValue, result);
				}
				num++;
				nKMLua.CloseTable();
			}
		}
		nKMLua.LuaClose();
		return true;
	}

	public static bool LoadFromLUA_CutScene(string fileName)
	{
		if (!m_dicCutscenTypeByStrID.ContainsKey(fileName))
		{
			return false;
		}
		if (m_dicNKCCutScenTempletByStrID.ContainsKey(fileName))
		{
			return true;
		}
		bool result = false;
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT_CUTSCENE", fileName))
		{
			if (nKMLua.OpenTable("m_dicNKCCutScenTempletByID"))
			{
				int num = 1;
				while (nKMLua.OpenTable(num))
				{
					NKCCutScenTemplet nKCCutScenTemplet = new NKCCutScenTemplet();
					if (nKCCutScenTemplet.LoadFromLUA(nKMLua, num))
					{
						if (!m_dicNKCCutScenTempletByID.ContainsKey(nKCCutScenTemplet.m_CutScenID))
						{
							m_dicNKCCutScenTempletByID.Add(nKCCutScenTemplet.m_CutScenID, nKCCutScenTemplet);
							if (!m_dicNKCCutScenTempletByStrID.ContainsKey(nKCCutScenTemplet.m_CutScenStrID))
							{
								result = true;
								m_dicNKCCutScenTempletByStrID.Add(nKCCutScenTemplet.m_CutScenStrID, nKCCutScenTemplet);
							}
						}
						else
						{
							m_dicNKCCutScenTempletByID[nKCCutScenTemplet.m_CutScenID].AddCutTemplet(nKCCutScenTemplet);
							nKCCutScenTemplet = null;
						}
					}
					num++;
					nKMLua.CloseTable();
				}
				nKMLua.CloseTable();
			}
			else
			{
				result = false;
			}
		}
		else
		{
			result = false;
		}
		nKMLua.LuaClose();
		return result;
	}
}
