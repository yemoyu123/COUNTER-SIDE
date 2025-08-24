using System.Collections.Generic;
using System.Linq;
using NKM.Contract2;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMShipCommandModuleTemplet : INKMTemplet
{
	private const int moduleLockItemCount = 2;

	private int id;

	private string openTag;

	private NKM_UNIT_STYLE_TYPE shipType;

	private NKM_UNIT_GRADE shipGrade;

	private int shipLimitBreakGrade;

	private int commandModuleSlot1Id;

	private int commandModuleSlot2Id;

	private MiscItemUnit[] moduleLockItems = new MiscItemUnit[2];

	private List<MiscItemUnit> shipModuleReqItem = new List<MiscItemUnit>(NKMCommonConst.ShipModuleReqItemCount);

	public int Key => id;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public int Slot1Id => commandModuleSlot1Id;

	public int Slot2Id => commandModuleSlot2Id;

	public int LimitBreakGrade => shipLimitBreakGrade;

	public IReadOnlyList<MiscItemUnit> ModuleLockItems => moduleLockItems.ToList();

	public IReadOnlyList<MiscItemUnit> ModuleReqItems => shipModuleReqItem;

	public static NKMShipCommandModuleTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 558))
		{
			return null;
		}
		NKMShipCommandModuleTemplet nKMShipCommandModuleTemplet = new NKMShipCommandModuleTemplet
		{
			id = lua.GetInt32("ID"),
			openTag = lua.GetString("OpenTag"),
			shipLimitBreakGrade = lua.GetInt32("ShipLimitBreakGrade"),
			commandModuleSlot1Id = lua.GetInt32("CommandModuleSlot1"),
			commandModuleSlot2Id = lua.GetInt32("CommandModuleSlot2")
		};
		lua.GetDataEnum<NKM_UNIT_STYLE_TYPE>("ShipType", out nKMShipCommandModuleTemplet.shipType);
		lua.GetDataEnum<NKM_UNIT_GRADE>("ShipGrade", out nKMShipCommandModuleTemplet.shipGrade);
		for (int i = 0; i < 2; i++)
		{
			int rValue = -1;
			int rValue2 = -1;
			if ((1u & (lua.GetData($"ModuleSlot{i + 1}_LockItemID", ref rValue) ? 1u : 0u) & (lua.GetData($"ModuleSlot{i + 1}_LockItemValue", ref rValue2) ? 1u : 0u)) != 0)
			{
				nKMShipCommandModuleTemplet.moduleLockItems[i] = new MiscItemUnit(rValue, rValue2);
			}
		}
		for (int j = 0; j < NKMCommonConst.ShipModuleReqItemCount; j++)
		{
			int rValue3 = -1;
			int rValue4 = -1;
			if ((1u & (lua.GetData($"ModuleReqItemID{j + 1}", ref rValue3) ? 1u : 0u) & (lua.GetData($"ModuleReqItemValue{j + 1}", ref rValue4) ? 1u : 0u)) != 0)
			{
				nKMShipCommandModuleTemplet.shipModuleReqItem.Add(new MiscItemUnit(rValue3, rValue4));
			}
		}
		return nKMShipCommandModuleTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (shipGrade < NKM_UNIT_GRADE.NUG_N || shipGrade >= NKM_UNIT_GRADE.NUG_COUNT)
		{
			NKMTempletError.Add($"[NKMShipCommandModuleTemplet:{id}] Ÿ\ufffd\ufffd \ufffdԼ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd.  shipGrade:{shipGrade}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 618);
		}
		if (shipType < NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT || shipType > NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL)
		{
			NKMTempletError.Add($"[NKMShipCommandModuleTemplet:{id}] Ÿ\ufffd\ufffd \ufffdԼ\ufffd Ÿ\ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd.  shipType:{shipType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 623);
		}
		for (int i = 0; i < moduleLockItems.Length; i++)
		{
			moduleLockItems[i].Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 628);
		}
		shipModuleReqItem.ForEach(delegate(MiscItemUnit s)
		{
			s.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 631);
		});
		if (shipLimitBreakGrade <= 0)
		{
			NKMTempletError.Add($"[NKMShipCommandModuleTemplet:{id}] shipLimitBreakGrade \ufffd\ufffd\ufffd\ufffd \ufffdùٸ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd.  shipLimitBreakGrade:{shipLimitBreakGrade}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 635);
		}
		IReadOnlyList<NKMCommandModulePassiveTemplet> passiveListsByGroupId = NKMShipModuleGroupTemplet.GetPassiveListsByGroupId(commandModuleSlot1Id);
		if (passiveListsByGroupId != null && passiveListsByGroupId.Count <= 0)
		{
			NKMTempletError.Add($"[NKMShipCommandModuleTemplet:{id}] ModuleSlot1Id\ufffd\ufffd\ufffd\ufffd NKMCommandModulePassive\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. statGroupId:{commandModuleSlot1Id}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 640);
		}
		IReadOnlyList<NKMCommandModulePassiveTemplet> passiveListsByGroupId2 = NKMShipModuleGroupTemplet.GetPassiveListsByGroupId(commandModuleSlot2Id);
		if (passiveListsByGroupId2 != null && passiveListsByGroupId2.Count <= 0)
		{
			NKMTempletError.Add($"[NKMShipCommandModuleTemplet:{id}] ModuleSlot2Id\ufffd\ufffd\ufffd\ufffd NKMCommandModulePassive\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. statGroupId:{commandModuleSlot2Id}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 645);
		}
	}

	public bool IsTargetTemplet(NKM_UNIT_STYLE_TYPE type, NKM_UNIT_GRADE grade, int limitBreakGrade)
	{
		if (shipType == type && shipGrade == grade)
		{
			return LimitBreakGrade == limitBreakGrade;
		}
		return false;
	}
}
