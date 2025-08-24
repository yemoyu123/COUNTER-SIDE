using System.Collections.Generic;
using NKC.Office;
using NKM;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;

namespace NKC.Templet.Office;

public class NKCOfficeFurnitureInteractionTemplet : INKMTemplet
{
	public enum ActType
	{
		Common,
		Reaction
	}

	public int IDX;

	public string InteractionGroupID;

	public ActType eActType;

	public int ActProbability;

	public ActTargetType eActTargetType;

	public HashSet<string> m_hsActTargetGroupID;

	public string UnitAni;

	public string InteriorAni;

	public static Dictionary<string, List<NKCOfficeFurnitureInteractionTemplet>> m_dicGroup;

	public int Key => IDX;

	private static NKCOfficeFurnitureInteractionTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCOfficeFurnitureInteractionTemplet.cs", 32))
		{
			return null;
		}
		NKCOfficeFurnitureInteractionTemplet nKCOfficeFurnitureInteractionTemplet = new NKCOfficeFurnitureInteractionTemplet();
		int num = (int)(1u & (cNKMLua.GetData("IDX", ref nKCOfficeFurnitureInteractionTemplet.IDX) ? 1u : 0u) & (cNKMLua.GetData("InteractionGroupID", ref nKCOfficeFurnitureInteractionTemplet.InteractionGroupID) ? 1u : 0u) & (cNKMLua.GetDataEnum<ActType>("ActType", out nKCOfficeFurnitureInteractionTemplet.eActType) ? 1u : 0u) & (cNKMLua.GetData("ActProbability", ref nKCOfficeFurnitureInteractionTemplet.ActProbability) ? 1u : 0u)) & (cNKMLua.GetDataEnum<ActTargetType>("ActTargetType", out nKCOfficeFurnitureInteractionTemplet.eActTargetType) ? 1 : 0);
		if (cNKMLua.GetDataList("ActTargetID", out List<string> result, nullIfEmpty: false))
		{
			nKCOfficeFurnitureInteractionTemplet.m_hsActTargetGroupID = new HashSet<string>(result);
		}
		cNKMLua.GetData("UnitAni", ref nKCOfficeFurnitureInteractionTemplet.UnitAni);
		cNKMLua.GetData("InteriorAni", ref nKCOfficeFurnitureInteractionTemplet.InteriorAni);
		if (num == 0)
		{
			return null;
		}
		return nKCOfficeFurnitureInteractionTemplet;
	}

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKCOfficeFurnitureInteractionTemplet>.Load("AB_SCRIPT", "LUA_INTERACTION_INTERIOR_TEMPLET", "INTERACTION_INTERIOR_TEMPLET", LoadFromLUA);
		m_dicGroup = new Dictionary<string, List<NKCOfficeFurnitureInteractionTemplet>>();
		foreach (NKCOfficeFurnitureInteractionTemplet value in NKMTempletContainer<NKCOfficeFurnitureInteractionTemplet>.Values)
		{
			if (!m_dicGroup.ContainsKey(value.InteractionGroupID))
			{
				m_dicGroup[value.InteractionGroupID] = new List<NKCOfficeFurnitureInteractionTemplet>();
			}
			m_dicGroup[value.InteractionGroupID].Add(value);
		}
	}

	public static List<NKCOfficeFurnitureInteractionTemplet> GetInteractionTempletList(NKMOfficeInteriorTemplet InteriorTemplet, ActType actType)
	{
		if (InteriorTemplet == null)
		{
			return null;
		}
		return GetInteractionTempletList(InteriorTemplet.InteractionGroupID, actType);
	}

	public static List<NKCOfficeFurnitureInteractionTemplet> GetInteractionTempletList(string group, ActType actType)
	{
		if (m_dicGroup == null)
		{
			LoadFromLua();
		}
		if (m_dicGroup.TryGetValue(group, out var value))
		{
			return value.FindAll((NKCOfficeFurnitureInteractionTemplet x) => x.eActType == actType);
		}
		return null;
	}

	public static void Drop()
	{
		m_dicGroup?.Clear();
		m_dicGroup = null;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public bool CheckUnitInteractionCondition(NKCOfficeCharacter character)
	{
		return NKCOfficeManager.IsActTarget(character, eActTargetType, m_hsActTargetGroupID);
	}

	public int GetFirstExclusiveActTarget()
	{
		if (eActTargetType == ActTargetType.Group)
		{
			return -1;
		}
		if (m_hsActTargetGroupID != null)
		{
			foreach (string item in m_hsActTargetGroupID)
			{
				if (int.TryParse(item, out var result))
				{
					return result;
				}
				Debug.Log($"NKCOfficeInteractionTemplet {IDX}: Int.Parse failed for id {item}");
			}
		}
		return -1;
	}
}
