using System.Collections.Generic;
using System.Linq;
using NKC;
using NKM.Templet.Base;

namespace NKM;

public class NKMUnitReactorTemplet : INKMTemplet, INKMTempletEx
{
	private int reactorId;

	private string m_OpenTag;

	private readonly int[] skillTempletIds = new int[NKMCommonConst.ReactorMaxLevel];

	public readonly NKMReactorSkillTemplet[] skillTemplets = new NKMReactorSkillTemplet[NKMCommonConst.ReactorMaxLevel];

	private string m_ReactorName;

	private string m_RectorDesc;

	private string m_ReactorIllust;

	private string m_ReactorIcon;

	public int Key => reactorId;

	public static IEnumerable<NKMUnitReactorTemplet> Values => NKMTempletContainer<NKMUnitReactorTemplet>.Values;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public string ReactorName => m_ReactorName;

	public string ReactorDesc => m_RectorDesc;

	public string ReactorIllust => m_ReactorIllust;

	public string ReactorIcon => m_ReactorIcon;

	public int BaseUnitID => reactorId;

	public NKMReactorSkillTemplet GetSkillTemplets(int level)
	{
		return skillTemplets[level - 1];
	}

	public static NKMUnitReactorTemplet Find(int key)
	{
		return NKMTempletContainer<NKMUnitReactorTemplet>.Find(key);
	}

	public static NKMUnitReactorTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitReactorTemplet.cs", 27))
		{
			return null;
		}
		NKMUnitReactorTemplet nKMUnitReactorTemplet = new NKMUnitReactorTemplet
		{
			reactorId = lua.GetInt32("ReactorID")
		};
		lua.GetData("OpenTag", ref nKMUnitReactorTemplet.m_OpenTag);
		lua.GetData("ReactorName", ref nKMUnitReactorTemplet.m_ReactorName);
		lua.GetData("ReactorDesc", ref nKMUnitReactorTemplet.m_RectorDesc);
		lua.GetData("ReactorIllust", ref nKMUnitReactorTemplet.m_ReactorIllust);
		lua.GetData("ReactorIcon", ref nKMUnitReactorTemplet.m_ReactorIcon);
		for (int i = 0; i < NKMCommonConst.ReactorMaxLevel; i++)
		{
			int rValue = 0;
			if (!lua.GetData($"Level{i + 1}", ref rValue))
			{
				break;
			}
			nKMUnitReactorTemplet.skillTempletIds[i] = rValue;
		}
		return nKMUnitReactorTemplet;
	}

	public virtual void Join()
	{
		for (int i = 0; i < skillTempletIds.Length; i++)
		{
			if (skillTempletIds[i] <= 0)
			{
				skillTemplets[i] = null;
				continue;
			}
			NKMReactorSkillTemplet nKMReactorSkillTemplet = NKMReactorSkillTemplet.Find(skillTempletIds[i]);
			if (nKMReactorSkillTemplet != null)
			{
				skillTemplets[i] = nKMReactorSkillTemplet;
				continue;
			}
			NKMTempletError.Add($"[ReactorTemplet:{reactorId}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd skill templet \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd index:{i} level:{i + 1} values:{skillTempletIds[i]}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitReactorTemplet.cs", 75);
		}
	}

	public virtual void Validate()
	{
		if (reactorId <= 0)
		{
			NKMTempletError.Add($"[ReactorTemplet:{reactorId}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd Id \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd reactorId:{reactorId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitReactorTemplet.cs", 84);
		}
		for (int i = 0; i < skillTemplets.Length; i++)
		{
			if (skillTemplets[i] != null && reactorId != skillTemplets[i].ReactorId)
			{
				NKMTempletError.Add($"[ReactorTemplet:{reactorId}] skillTemplet\ufffd\ufffd reactorId \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdٸ\ufffd. reactorId:{reactorId} skillTempletReactorId:{skillTemplets[i].ReactorId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitReactorTemplet.cs", 97);
			}
		}
		if (skillTemplets.Length != NKMCommonConst.ReactorMaxLevel)
		{
			NKMTempletError.Add($"[ReactorTemplet:{reactorId}] skillTemplet\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\u05b4\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdٸ\ufffd. skillTemplets:{skillTemplets.Length} NKMCommonConst.ReactorMaxLevel:{NKMCommonConst.ReactorMaxLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitReactorTemplet.cs", 103);
		}
		List<int> list = (from e in skillTemplets
			where e != null
			select e.Key).ToList();
		if (list.Count != (from e in list
			group e by e).Count())
		{
			NKMTempletError.Add($"[ReactorTemplet:{reactorId}] skillTemplet \ufffd\ufffd \ufffdߺ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. totalCount:{list.Count} groupByCount:{(from e in list
				group e by e).Count()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitReactorTemplet.cs", 109);
		}
	}

	public int GetMaxReactorLevel()
	{
		for (int i = 0; i < skillTemplets.Length; i++)
		{
			if (skillTemplets[i] == null)
			{
				return i;
			}
		}
		return skillTemplets.Length;
	}

	public NKMReactorSkillTemplet[] GetSkillTemplets()
	{
		return skillTemplets;
	}

	public string GetName()
	{
		return NKCStringTable.GetString(ReactorName);
	}

	public void PostJoin()
	{
		Join();
	}
}
