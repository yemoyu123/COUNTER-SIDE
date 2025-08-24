using System.Collections.Generic;
using NKC.Office;
using NKM;
using NKM.Templet.Base;

namespace NKC.Templet.Office;

public class NKCOfficeUnitInteractionTemplet : INKMTemplet
{
	public int UnitActID;

	public string ActorAni;

	public string TargetAni;

	public bool AlignUnit;

	public float ActRange;

	public ActTargetType ActorType;

	public HashSet<string> hsActorGroup;

	public ActTargetType TargetType;

	public HashSet<string> hsTargetGroup;

	public string InteractionSkinGroup;

	private static bool m_sLuaLoaded;

	public bool IsSoloAction
	{
		get
		{
			if (hsTargetGroup != null)
			{
				return hsTargetGroup.Count == 0;
			}
			return true;
		}
	}

	public int Key => UnitActID;

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKCOfficeUnitInteractionTemplet>.Load("AB_SCRIPT", "LUA_INTERACTION_UNIT_TEMPLET", "INTERACTION_UNIT_TEMPLET", LoadFromLUA);
		NKCOfficeManager.BuildSkinInteractionGroup();
		m_sLuaLoaded = true;
	}

	private static NKCOfficeUnitInteractionTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCOfficeUnitInteractionTemplet.cs", 44))
		{
			return null;
		}
		NKCOfficeUnitInteractionTemplet nKCOfficeUnitInteractionTemplet = new NKCOfficeUnitInteractionTemplet();
		int num = (int)(1u & (cNKMLua.GetData("UnitActID", ref nKCOfficeUnitInteractionTemplet.UnitActID) ? 1u : 0u)) & (cNKMLua.GetData("UnitActStrID", ref nKCOfficeUnitInteractionTemplet.ActorAni) ? 1 : 0);
		cNKMLua.GetData("UnitActStrID2", ref nKCOfficeUnitInteractionTemplet.TargetAni);
		cNKMLua.GetData("AlignUnit", ref nKCOfficeUnitInteractionTemplet.AlignUnit);
		cNKMLua.GetData("ActRange", ref nKCOfficeUnitInteractionTemplet.ActRange);
		List<string> result;
		int num2 = (int)((uint)num & (cNKMLua.GetDataEnum<ActTargetType>("UnitType1", out nKCOfficeUnitInteractionTemplet.ActorType) ? 1u : 0u)) & (cNKMLua.GetDataList("UnitID1", out result, nullIfEmpty: false) ? 1 : 0);
		nKCOfficeUnitInteractionTemplet.hsActorGroup = new HashSet<string>(result);
		cNKMLua.GetDataEnum<ActTargetType>("UnitType2", out nKCOfficeUnitInteractionTemplet.TargetType);
		if (cNKMLua.GetDataList("UnitID2", out List<string> result2, nullIfEmpty: false))
		{
			nKCOfficeUnitInteractionTemplet.hsTargetGroup = new HashSet<string>(result2);
		}
		cNKMLua.GetData("InteractionSkinGroup", ref nKCOfficeUnitInteractionTemplet.InteractionSkinGroup);
		if (num2 == 0)
		{
			return null;
		}
		return nKCOfficeUnitInteractionTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public static List<NKCOfficeUnitInteractionTemplet> GetInteractionTempletList(NKCOfficeCharacter character)
	{
		if (character == null)
		{
			return new List<NKCOfficeUnitInteractionTemplet>();
		}
		if (!m_sLuaLoaded)
		{
			LoadFromLua();
		}
		List<NKCOfficeUnitInteractionTemplet> list = new List<NKCOfficeUnitInteractionTemplet>();
		foreach (NKCOfficeUnitInteractionTemplet value in NKMTempletContainer<NKCOfficeUnitInteractionTemplet>.Values)
		{
			if (value.CheckUnitInteractionCondition(character, bTarget: false) && NKCAnimationEventManager.CanPlayAnimEvent(character, value.ActorAni))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public bool CheckUnitInteractionCondition(NKCOfficeCharacter character, bool bTarget)
	{
		if (character == null)
		{
			return false;
		}
		if (bTarget && IsSoloAction)
		{
			return false;
		}
		ActTargetType eActTargetType;
		HashSet<string> hsActTargetGroupID;
		if (bTarget)
		{
			eActTargetType = TargetType;
			hsActTargetGroupID = hsTargetGroup;
		}
		else
		{
			eActTargetType = ActorType;
			hsActTargetGroupID = hsActorGroup;
		}
		return NKCOfficeManager.IsActTarget(character, eActTargetType, hsActTargetGroupID);
	}

	public static void Drop()
	{
		m_sLuaLoaded = false;
	}
}
