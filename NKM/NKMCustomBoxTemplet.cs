using System.Collections.Generic;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMCustomBoxTemplet : INKMTemplet
{
	private int customBoxId;

	private NKM_REWARD_TYPE unitType;

	private int limitBreak;

	private int level;

	private int skillLevel;

	private int tacticUpdate;

	private int reactorLevel;

	private int loyalty;

	private List<int> customOperatorSkillIds;

	public int Key => customBoxId;

	public NKM_REWARD_TYPE UnitType => unitType;

	public int LimitBreak => limitBreak;

	public int Level => level;

	public int SkillLevel => skillLevel;

	public int TacticUpdate => tacticUpdate;

	public int ReactorLevel => reactorLevel;

	public int Loyalty => loyalty;

	public List<int> CustomOperatorSkillIds => customOperatorSkillIds;

	public static NKMCustomBoxTemplet Find(int id)
	{
		return NKMTempletContainer<NKMCustomBoxTemplet>.Find(id);
	}

	public static NKMCustomBoxTemplet LoadFromLua(NKMLua lua)
	{
		NKMCustomBoxTemplet nKMCustomBoxTemplet = new NKMCustomBoxTemplet();
		lua.GetData("CustomBoxID", ref nKMCustomBoxTemplet.customBoxId);
		lua.GetData("UnitType", ref nKMCustomBoxTemplet.unitType);
		lua.GetData("LimitBreak", ref nKMCustomBoxTemplet.limitBreak);
		lua.GetData("Level", ref nKMCustomBoxTemplet.level);
		lua.GetData("SkillLevel", ref nKMCustomBoxTemplet.skillLevel);
		lua.GetData("TacticUpdate", ref nKMCustomBoxTemplet.tacticUpdate);
		lua.GetData("ReactorLevel", ref nKMCustomBoxTemplet.reactorLevel);
		lua.GetData("Loyalty", ref nKMCustomBoxTemplet.loyalty);
		lua.GetDataList("CustomOprSkill", out nKMCustomBoxTemplet.customOperatorSkillIds, nullIfEmpty: false);
		return nKMCustomBoxTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		switch (UnitType)
		{
		case NKM_REWARD_TYPE.RT_UNIT:
			ValidateUnit();
			break;
		case NKM_REWARD_TYPE.RT_SHIP:
			ValidateShip();
			break;
		case NKM_REWARD_TYPE.RT_OPERATOR:
			ValidateOperator();
			break;
		default:
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd\ufffd Ÿ\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 66);
			break;
		}
	}

	public void ValidateUnit()
	{
		if (level < 0 || level > 120)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(Level) \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 75);
		}
		if (limitBreak < 0 || limitBreak > 13)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(LimitBreak) \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 80);
		}
		if (tacticUpdate < 0 || tacticUpdate > 6)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(TacticUpdate) \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 85);
		}
		if (skillLevel < 0 || skillLevel > 11)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(SkillLevel) \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 90);
		}
		if (reactorLevel < 0 || reactorLevel > NKMCommonConst.ReactorMaxLevel)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(ReactorLevel) \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 95);
		}
	}

	public void ValidateShip()
	{
		if (reactorLevel > 0 || loyalty > 0)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffdԼ\ufffd/\ufffd\ufffd\ufffd۷\ufffd\ufffd\ufffd\ufffdͿ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd/\ufffd\u05bb\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 103);
		}
		if (limitBreak < 0 || limitBreak > 8)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdԼ\ufffd \ufffd\ufffd\ufffd\ufffd(LimitBreak) \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 108);
		}
		if (level < 0 || level > 130)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdԼ\ufffd \ufffd\ufffd\ufffd\ufffd(Level) \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 113);
		}
	}

	public void ValidateOperator()
	{
		if (reactorLevel > 0 || loyalty > 0)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffdԼ\ufffd/\ufffd\ufffd\ufffd۷\ufffd\ufffd\ufffd\ufffdͿ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd/\ufffd\u05bb\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 121);
		}
		if (tacticUpdate < 0 || tacticUpdate > 8)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffd\ufffd\ufffd۷\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd..", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 126);
		}
		if (skillLevel < 0 || skillLevel > 11)
		{
			NKMTempletError.Add($"[NKMCustomBoxTemplet:{Key}] \ufffd\ufffd\ufffd۷\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd..", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCustomBoxTemplet.cs", 131);
		}
	}
}
